using NxTiler.Domain.Windowing;

namespace NxTiler.App.Services;

public sealed partial class RecordingOverlayService
{
    public Task ShowMaskEditingAsync(WindowBounds monitorBounds, WindowBounds captureBounds, CancellationToken ct = default)
    {
        return Task.CompletedTask;
    }

    public Task CloseAsync(CancellationToken ct = default)
    {
        return Task.CompletedTask;
    }
}
