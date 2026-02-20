using System.Diagnostics;
using Microsoft.Extensions.Logging;
using NxTiler.Domain.Capture;

namespace NxTiler.Infrastructure.Recording;

public sealed partial class WgcVideoRecordingEngine
{
    private async Task<string?> ApplyMaskingAsync(string inputPath, string finalOutputPath, IReadOnlyList<CaptureMask> masks, CancellationToken ct)
    {
        var filters = new List<string>();
        foreach (var mask in masks)
        {
            if (mask.Width <= 0 || mask.Height <= 0)
            {
                continue;
            }

            var x = Math.Max(0, mask.X);
            var y = Math.Max(0, mask.Y);
            var w = mask.Width;
            var h = mask.Height;

            if (x >= _captureRect.Width || y >= _captureRect.Height)
            {
                continue;
            }

            if (x + w > _captureRect.Width)
            {
                w = _captureRect.Width - x;
            }

            if (y + h > _captureRect.Height)
            {
                h = _captureRect.Height - y;
            }

            if (w <= 0 || h <= 0)
            {
                continue;
            }

            filters.Add($"drawbox=x={x}:y={y}:w={w}:h={h}:color=black@1:t=fill");
        }

        if (filters.Count == 0)
        {
            return null;
        }

        var tempMasked = Path.Combine(_outputFolder, $"{_sessionPrefix}_masked_tmp.mp4");
        TryDelete(tempMasked);

        var vf = string.Join(',', filters);
        var args = BuildMaskingArguments(inputPath, tempMasked, vf);

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

        if (!await ProcessExitAwaiter.WaitForExitAsync(process, 30 * 60 * 1000, ct))
        {
            try
            {
                process.Kill();
            }
            catch (Exception ex)
            {
                logger.LogDebug(ex, "Failed to kill ffmpeg masking process after timeout.");
            }
        }

        if (!File.Exists(tempMasked))
        {
            var tail = _stderrTail.Snapshot();
            logger.LogWarning("WGC masking pass failed. ffmpeg tail: {Tail}", tail);
            return null;
        }

        File.Move(tempMasked, finalOutputPath, overwrite: true);
        return finalOutputPath;
    }
}
