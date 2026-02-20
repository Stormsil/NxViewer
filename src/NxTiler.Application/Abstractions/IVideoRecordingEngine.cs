using NxTiler.Domain.Capture;
using NxTiler.Domain.Windowing;

namespace NxTiler.Application.Abstractions;

public interface IVideoRecordingEngine
{
    bool IsRunning { get; }

    Task StartAsync(nint targetWindow, WindowBounds captureBounds, RecordingProfile profile, CancellationToken ct = default);

    Task PauseAsync(CancellationToken ct = default);

    Task ResumeAsync(CancellationToken ct = default);

    Task<string?> StopAsync(IReadOnlyList<CaptureMask> masks, CancellationToken ct = default);

    Task AbortAsync(CancellationToken ct = default);
}
