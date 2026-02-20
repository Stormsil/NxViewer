using System.Diagnostics;
using Microsoft.Extensions.Logging;
using NxTiler.Domain.Windowing;

namespace NxTiler.Infrastructure.Recording;

public sealed partial class FfmpegRecordingEngine
{
    private async Task<string> FinalizeMultipleSegmentsAsync(
        bool shouldMask,
        IReadOnlyList<WindowBounds>? masksPx,
        CancellationToken ct)
    {
        var listFile = Path.Combine(_outputFolder, $"{_sessionPrefix}_list.txt");
        File.WriteAllLines(listFile, _segments.Select(static x => $"file '{x.Replace('\\', '/').Replace("'", "'\\''")}'"));

        var finalOutput = Path.Combine(_outputFolder, $"{_sessionPrefix}.mp4");
        var concatOutput = shouldMask
            ? Path.Combine(_outputFolder, $"{_sessionPrefix}_concat_tmp.mp4")
            : finalOutput;
        TryDelete(concatOutput);

        var concatArgs = BuildConcatArguments(listFile, concatOutput);

        using var concatProcess = new Process
        {
            StartInfo = new ProcessStartInfo
            {
                FileName = _ffmpegPath,
                Arguments = concatArgs,
                UseShellExecute = false,
                CreateNoWindow = true,
                RedirectStandardError = true,
            },
        };

        concatProcess.Start();
        concatProcess.BeginErrorReadLine();

        var concatExited = await ProcessExitAwaiter.WaitForExitAsync(concatProcess, 30000, ct);
        if (!concatExited)
        {
            try
            {
                concatProcess.Kill();
            }
            catch (Exception ex)
            {
                logger.LogDebug(ex, "Failed to kill ffmpeg concat process after timeout.");
            }
        }

        if (!File.Exists(concatOutput))
        {
            return _segments[0];
        }

        // Raw output is ready; we can drop segments regardless of whether the optional masking pass succeeds.
        foreach (var segment in _segments)
        {
            TryDelete(segment);
        }

        TryDelete(listFile);

        if (!shouldMask)
        {
            return finalOutput;
        }

        var masked = await ApplyMaskingAsync(concatOutput, finalOutput, masksPx!, ct);
        if (!string.IsNullOrWhiteSpace(masked) && File.Exists(masked))
        {
            TryDelete(concatOutput);
            return masked;
        }

        // Fallback: if masking failed, publish the concatenated unmasked result as the final output.
        try
        {
            File.Move(concatOutput, finalOutput, overwrite: true);
        }
        catch (Exception ex)
        {
            logger.LogWarning(ex, "Failed to move concatenated file to final output path after masking failure.");
        }

        return finalOutput;
    }
}
