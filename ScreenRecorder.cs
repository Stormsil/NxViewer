using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Threading.Tasks;

namespace NxTiler
{
    public class ScreenRecorder
    {
        private Process? _ffmpeg;
        private readonly List<string> _segments = new();
        private string _outputFolder = "";
        private string _ffmpegPath = "ffmpeg";
        private string _sessionPrefix = "";
        private int _x, _y, _w, _h, _fps;

        public bool IsRunning => _ffmpeg != null && !_ffmpeg.HasExited;
        public string? LastError { get; private set; }

        /// <returns>true if started successfully, false on error (see LastError)</returns>
        public bool Start(int x, int y, int w, int h, int fps, string folder, string ffmpegPath)
        {
            _x = x; _y = y; _w = w; _h = h; _fps = fps;
            _outputFolder = folder;
            _ffmpegPath = ffmpegPath;
            _sessionPrefix = $"rec_{DateTime.Now:yyyyMMdd_HHmmss}";
            _segments.Clear();
            LastError = null;

            Directory.CreateDirectory(_outputFolder);
            return StartNewSegment();
        }

        /// <returns>true if started successfully</returns>
        public bool StartNewSegment()
        {
            var segmentFile = Path.Combine(_outputFolder, $"{_sessionPrefix}_seg{_segments.Count:D3}.mp4");
            _segments.Add(segmentFile);

            var args = $"-f gdigrab -framerate {_fps} -offset_x {_x} -offset_y {_y} " +
                       $"-video_size {_w}x{_h} -i desktop " +
                       $"-c:v libx264 -preset ultrafast -pix_fmt yuv420p -an \"{segmentFile}\"";

            _ffmpeg = new Process
            {
                StartInfo = new ProcessStartInfo
                {
                    FileName = _ffmpegPath,
                    Arguments = args,
                    UseShellExecute = false,
                    CreateNoWindow = true,
                    RedirectStandardInput = true,
                    RedirectStandardError = true
                }
            };
            try
            {
                _ffmpeg.Start();
                // Discard stderr to prevent buffer deadlock
                _ffmpeg.BeginErrorReadLine();
                return true;
            }
            catch (System.ComponentModel.Win32Exception ex)
            {
                _segments.RemoveAt(_segments.Count - 1);
                _ffmpeg = null;
                LastError = $"Failed to start ffmpeg: {ex.Message}\nPath: \"{_ffmpegPath}\"\nSpecify full path in Settings → Screen Recording.";
                return false;
            }
        }

        public async Task StopCurrentSegment()
        {
            if (_ffmpeg == null || _ffmpeg.HasExited) return;

            try
            {
                // Send 'q' to gracefully stop ffmpeg
                _ffmpeg.StandardInput.Write("q");
                _ffmpeg.StandardInput.Flush();

                // Wait up to 5 seconds
                var exited = await WaitForExitAsync(_ffmpeg, 5000);
                if (!exited)
                {
                    _ffmpeg.Kill();
                }
            }
            catch
            {
                try { _ffmpeg.Kill(); } catch { }
            }
            _ffmpeg = null;
        }

        public async Task<string> FinalizeRecording()
        {
            await StopCurrentSegment();

            // Remove segments that don't exist (failed recordings)
            _segments.RemoveAll(s => !File.Exists(s));

            if (_segments.Count == 0)
                return "";

            if (_segments.Count == 1)
            {
                // Single segment - just rename
                var finalPath = Path.Combine(_outputFolder, $"{_sessionPrefix}.mp4");
                try { File.Move(_segments[0], finalPath, overwrite: true); }
                catch { finalPath = _segments[0]; }
                return finalPath;
            }

            // Multiple segments - concat via ffmpeg
            var listFile = Path.Combine(_outputFolder, $"{_sessionPrefix}_list.txt");
            var lines = new List<string>();
            foreach (var seg in _segments)
            {
                lines.Add($"file '{seg.Replace('\\', '/').Replace("'", "'\\''")}'");
            }
            File.WriteAllLines(listFile, lines);

            var finalOutput = Path.Combine(_outputFolder, $"{_sessionPrefix}.mp4");
            var concatArgs = $"-f concat -safe 0 -i \"{listFile}\" -c copy \"{finalOutput}\"";

            var concatProc = new Process
            {
                StartInfo = new ProcessStartInfo
                {
                    FileName = _ffmpegPath,
                    Arguments = concatArgs,
                    UseShellExecute = false,
                    CreateNoWindow = true,
                    RedirectStandardError = true
                }
            };
            concatProc.Start();
            concatProc.BeginErrorReadLine();
            var concatExited = await WaitForExitAsync(concatProc, 30000);
            if (!concatExited)
            {
                try { concatProc.Kill(); } catch { }
            }

            // Cleanup temp files
            if (File.Exists(finalOutput))
            {
                foreach (var seg in _segments)
                {
                    try { File.Delete(seg); } catch { }
                }
                try { File.Delete(listFile); } catch { }
            }
            else
            {
                // Concat failed, return first segment
                finalOutput = _segments[0];
            }

            return finalOutput;
        }

        public void Abort()
        {
            if (_ffmpeg != null && !_ffmpeg.HasExited)
            {
                try { _ffmpeg.Kill(); } catch { }
            }
            _ffmpeg = null;
            // Clean up all segment files
            foreach (var seg in _segments)
            {
                try { File.Delete(seg); } catch { }
            }
            _segments.Clear();
        }

        private static async Task<bool> WaitForExitAsync(Process process, int timeoutMs)
        {
            var tcs = new TaskCompletionSource<bool>();
            process.EnableRaisingEvents = true;
            process.Exited += (_, _) => tcs.TrySetResult(true);

            if (process.HasExited) return true;

            var completed = await Task.WhenAny(tcs.Task, Task.Delay(timeoutMs));
            return completed == tcs.Task;
        }
    }
}
