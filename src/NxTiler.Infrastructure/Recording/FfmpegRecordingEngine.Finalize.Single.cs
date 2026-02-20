using Microsoft.Extensions.Logging;
using NxTiler.Domain.Windowing;

namespace NxTiler.Infrastructure.Recording;

public sealed partial class FfmpegRecordingEngine
{
    private async Task<string> FinalizeSingleSegmentAsync(
        bool shouldMask,
        IReadOnlyList<WindowBounds>? masksPx,
        CancellationToken ct)
    {
        var finalPath = Path.Combine(_outputFolder, $"{_sessionPrefix}.mp4");
        try
        {
            if (!shouldMask)
            {
                File.Move(_segments[0], finalPath, overwrite: true);
                return finalPath;
            }

            var input = _segments[0];
            var maskedPath = await ApplyMaskingAsync(input, finalPath, masksPx!, ct);
            if (!string.IsNullOrWhiteSpace(maskedPath) && File.Exists(maskedPath))
            {
                TryDelete(input);
                return maskedPath;
            }

            // Fallback: keep an unmasked output under the expected final name.
            File.Move(input, finalPath, overwrite: true);
            return finalPath;
        }
        catch (Exception ex)
        {
            logger.LogWarning(ex, "Failed to move single recording segment to final output path.");
            return _segments[0];
        }
    }
}
