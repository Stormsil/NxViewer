using Microsoft.Extensions.Logging;
using NxTiler.Domain.Capture;

namespace NxTiler.Infrastructure.Recording;

public sealed partial class WgcVideoRecordingEngine
{
    public async Task<string?> StopAsync(IReadOnlyList<CaptureMask> masks, CancellationToken ct = default)
    {
        await _gate.WaitAsync(ct);
        try
        {
            if (!IsRunning)
            {
                return null;
            }

            await StopInternalAsync(forceKill: false, ct);

            if (string.IsNullOrWhiteSpace(_rawOutputPath) || !File.Exists(_rawOutputPath))
            {
                var tail = _stderrTail.Snapshot();
                logger.LogWarning("WGC recording produced no output file. ffmpeg tail: {Tail}", tail);
                return null;
            }

            var finalPath = Path.Combine(_outputFolder, $"{_sessionPrefix}.mp4");
            TryDelete(finalPath);

            if (masks is { Count: > 0 })
            {
                var maskedPath = await ApplyMaskingAsync(_rawOutputPath, finalPath, masks, ct);
                if (!string.IsNullOrWhiteSpace(maskedPath))
                {
                    TryDelete(_rawOutputPath);
                    ResetState();
                    return maskedPath;
                }
            }

            File.Move(_rawOutputPath, finalPath, overwrite: true);
            ResetState();
            return finalPath;
        }
        finally
        {
            _gate.Release();
        }
    }

    public async Task AbortAsync(CancellationToken ct = default)
    {
        await _gate.WaitAsync(ct);
        try
        {
            if (!IsRunning)
            {
                return;
            }

            await StopInternalAsync(forceKill: true, ct);
            if (!string.IsNullOrWhiteSpace(_rawOutputPath))
            {
                TryDelete(_rawOutputPath);
            }

            ResetState();
        }
        finally
        {
            _gate.Release();
        }
    }
}
