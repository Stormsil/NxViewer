using System.Diagnostics;

namespace NxTiler.Infrastructure.Recording;

public sealed partial class WgcVideoRecordingEngine
{
    private void StartFfmpegProcess(string rawOutputPath)
    {
        _stderrTail.Reset();
        var args = BuildRawPipeArguments(rawOutputPath);

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

        _stderrTail.Attach(_ffmpeg);

        _ffmpeg.Start();
        _ffmpeg.BeginErrorReadLine();

        if (_ffmpeg.WaitForExit(250))
        {
            _ffmpeg.WaitForExit();
            var tail = _stderrTail.Snapshot();
            var message = string.IsNullOrWhiteSpace(tail)
                ? $"ffmpeg exited immediately (code {_ffmpeg.ExitCode})."
                : $"ffmpeg exited immediately (code {_ffmpeg.ExitCode}). {tail}";
            _ffmpeg = null;
            throw new InvalidOperationException(message);
        }
    }

    private void StartFramePump(CancellationToken ct)
    {
        _framePumpCts = CancellationTokenSource.CreateLinkedTokenSource(ct);
        _pauseGate.Set();
        _framePumpTask = Task.Run(() => FramePumpLoopAsync(_framePumpCts.Token), CancellationToken.None);
    }
}
