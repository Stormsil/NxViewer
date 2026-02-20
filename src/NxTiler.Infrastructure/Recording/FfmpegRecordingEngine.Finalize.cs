using NxTiler.Domain.Windowing;

namespace NxTiler.Infrastructure.Recording;

public sealed partial class FfmpegRecordingEngine
{
    public async Task<string> FinalizeRecordingAsync(IReadOnlyList<WindowBounds>? masksPx = null, CancellationToken ct = default)
    {
        await StopCurrentSegmentAsync(ct);
        _segments.RemoveAll(static path => !File.Exists(path));

        if (_segments.Count == 0)
        {
            var tail = _stderrTail.Snapshot();
            LastError = string.IsNullOrWhiteSpace(tail)
                ? "ffmpeg did not produce any output files."
                : $"ffmpeg did not produce any output files.\n{tail}";
            return string.Empty;
        }

        var shouldMask = masksPx is { Count: > 0 };

        if (_segments.Count == 1)
        {
            return await FinalizeSingleSegmentAsync(shouldMask, masksPx, ct);
        }

        return await FinalizeMultipleSegmentsAsync(shouldMask, masksPx, ct);
    }
}
