using NxTiler.Domain.Windowing;

namespace NxTiler.App.Services;

public interface IRecordingOverlayService
{
    Task ShowMaskEditingAsync(WindowBounds monitorBounds, WindowBounds captureBounds, CancellationToken ct = default);

    Task EnterRecordingModeAsync(CancellationToken ct = default);

    Task EnterPauseEditModeAsync(CancellationToken ct = default);

    Task ReEnterRecordingModeAsync(CancellationToken ct = default);

    Task ShowStatusAsync(string message, CancellationToken ct = default);

    Task<IReadOnlyList<WindowBounds>> GetMaskRectsPxAsync(CancellationToken ct = default);

    Task CloseAsync(CancellationToken ct = default);
}
