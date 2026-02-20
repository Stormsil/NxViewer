using System.Diagnostics;
using Microsoft.Extensions.Logging;

namespace NxTiler.Infrastructure.Recording;

public sealed partial class FfmpegRecordingEngine
{
    private async Task RunMaskingProcessAsync(string inputPath, string tempMaskedPath, string filterValue, CancellationToken ct)
    {
        var args = BuildMaskingArguments(inputPath, tempMaskedPath, filterValue);

        using var process = new Process
        {
            StartInfo = new ProcessStartInfo
            {
                FileName = _ffmpegPath,
                Arguments = args,
                UseShellExecute = false,
                CreateNoWindow = true,
                RedirectStandardError = true,
            },
        };

        _stderrTail.Reset();
        _stderrTail.Attach(process);

        process.Start();
        process.BeginErrorReadLine();

        var exited = await ProcessExitAwaiter.WaitForExitAsync(process, 30 * 60 * 1000, ct);
        if (exited)
        {
            return;
        }

        try
        {
            process.Kill();
        }
        catch (Exception ex)
        {
            logger.LogDebug(ex, "Failed to kill ffmpeg masking process after timeout.");
        }
    }
}
