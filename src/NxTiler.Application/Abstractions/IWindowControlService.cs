using NxTiler.Domain.Windowing;

namespace NxTiler.Application.Abstractions;

public interface IWindowControlService
{
    Task MaximizeAsync(nint windowHandle, CancellationToken ct = default);

    Task ApplyPlacementsAsync(IReadOnlyList<WindowPlacement> placements, CancellationToken ct = default);

    Task MinimizeAllAsync(IReadOnlyList<nint> windows, CancellationToken ct = default);

    Task RestoreAllAsync(IReadOnlyList<nint> windows, CancellationToken ct = default);

    Task BringToForegroundAsync(nint windowHandle, CancellationToken ct = default);

    Task<WindowBounds> GetWorkAreaForWindowAsync(nint windowHandle, CancellationToken ct = default);

    Task<WindowBounds> GetMonitorBoundsForWindowAsync(nint windowHandle, CancellationToken ct = default);

    Task<WindowBounds> GetClientAreaScreenBoundsAsync(nint windowHandle, CancellationToken ct = default);

    Task<WindowBounds> GetWindowBoundsAsync(nint windowHandle, CancellationToken ct = default);
}
