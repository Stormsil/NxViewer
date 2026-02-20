using NxTiler.Domain.Windowing;

namespace NxTiler.App.Services;

public sealed partial class RecordingOverlayService
{
    public Task<IReadOnlyList<WindowBounds>> GetMaskRectsPxAsync(CancellationToken ct = default)
    {
        return Task.FromResult<IReadOnlyList<WindowBounds>>(Array.Empty<WindowBounds>());
    }
}
