using NxTiler.Domain.Windowing;

namespace NxTiler.Application.Abstractions;

public interface IRecordingEngine
{
    bool IsRunning { get; }

    string? LastError { get; }

    bool Start(int x, int y, int width, int height, int fps, string folder, string ffmpegPath);

    bool StartNewSegment();

    Task StopCurrentSegmentAsync(CancellationToken ct = default);

    Task<string> FinalizeRecordingAsync(IReadOnlyList<WindowBounds>? masksPx = null, CancellationToken ct = default);

    void Abort();
}
