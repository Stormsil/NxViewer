using System.Diagnostics;
using Microsoft.Extensions.Logging;

namespace NxTiler.Infrastructure.Recording;

public sealed partial class FfmpegRecordingEngine
{
    public bool StartNewSegment()
    {
        if (string.IsNullOrWhiteSpace(_outputFolder))
        {
            LastError = "Recording output folder is not configured.";
            return false;
        }

        if (_width <= 0 || _height <= 0)
        {
            LastError = $"Invalid capture dimensions: {_width}x{_height}.";
            return false;
        }

        if (_fps <= 0)
        {
            LastError = $"Invalid FPS: {_fps}.";
            return false;
        }

        if (IsRunning)
        {
            LastError = "Recording segment is already running.";
            return false;
        }

        LastError = null;

        var segmentFile = Path.Combine(_outputFolder, $"{_sessionPrefix}_seg{_segments.Count:D3}.mp4");
        _segments.Add(segmentFile);

        var args = BuildSegmentArguments(segmentFile);

        _ffmpeg = new Process
        {
            StartInfo = new ProcessStartInfo
            {
                FileName = _ffmpegPath,
                Arguments = args,
                UseShellExecute = false,
                CreateNoWindow = true,
                RedirectStandardInput = true,
                RedirectStandardError = true,
            },
        };

        try
        {
            _stderrTail.Reset();
            _stderrTail.Attach(_ffmpeg);

            _ffmpeg.Start();
            _ffmpeg.BeginErrorReadLine();

            // If ffmpeg fails due to invalid bounds or monitor layout issues, it typically exits immediately.
            if (_ffmpeg.WaitForExit(250))
            {
                // Ensure all stderr events are flushed.
                _ffmpeg.WaitForExit();

                var exitCode = _ffmpeg.ExitCode;
                var tail = _stderrTail.Snapshot();

                _ffmpeg = null;
                _segments.RemoveAt(_segments.Count - 1);
                TryDelete(segmentFile);

                LastError = string.IsNullOrWhiteSpace(tail)
                    ? $"ffmpeg exited immediately after start (code {exitCode})."
                    : $"ffmpeg exited immediately after start (code {exitCode}).\n{tail}";

                return false;
            }

            return true;
        }
        catch (Exception ex)
        {
            _segments.RemoveAt(_segments.Count - 1);
            _ffmpeg = null;
            LastError = $"Failed to start ffmpeg: {ex.Message}\nPath: \"{_ffmpegPath}\"\nSpecify full path in Settings.";
            logger.LogWarning(ex, "Failed to start ffmpeg process for recording segment.");
            return false;
        }
    }
}
