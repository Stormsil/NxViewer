using Microsoft.Extensions.Logging;
using NxTiler.Domain.Windowing;

namespace NxTiler.Infrastructure.Recording;

public sealed partial class FfmpegRecordingEngine
{
    private async Task<string> ApplyMaskingAsync(string inputPath, string finalOutputPath, IReadOnlyList<WindowBounds> masksPx, CancellationToken ct)
    {
        try
        {
            var filters = BuildMaskingFilters(masksPx);
            if (filters.Count == 0)
            {
                return inputPath;
            }

            var tempMaskedPath = Path.Combine(_outputFolder, $"{_sessionPrefix}_masked_tmp.mp4");
            TryDelete(tempMaskedPath);

            var vf = string.Join(',', filters);
            logger.LogInformation("Recording: applying {Count} mask(s) via ffmpeg drawbox. Frame={W}x{H}", filters.Count, _width, _height);
            logger.LogDebug("Recording: ffmpeg mask filter: {Filter}", vf);
            await RunMaskingProcessAsync(inputPath, tempMaskedPath, vf, ct);

            if (!File.Exists(tempMaskedPath))
            {
                var tail = _stderrTail.Snapshot();
                LastError = string.IsNullOrWhiteSpace(tail) ? "ffmpeg masking failed." : $"ffmpeg masking failed.\n{tail}";
                return string.Empty;
            }

            return TryPromoteMaskedOutput(tempMaskedPath, finalOutputPath);
        }
        catch (Exception ex)
        {
            logger.LogWarning(ex, "Masking pass failed.");
            LastError = $"Masking pass failed: {ex.Message}";
            return string.Empty;
        }
    }
}
