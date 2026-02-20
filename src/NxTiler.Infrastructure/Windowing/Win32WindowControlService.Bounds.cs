using NxTiler.Domain.Windowing;
using NxTiler.Infrastructure.Native;

namespace NxTiler.Infrastructure.Windowing;

public sealed partial class Win32WindowControlService
{
    public Task<WindowBounds> GetWorkAreaForWindowAsync(nint windowHandle, CancellationToken ct = default)
    {
        ct.ThrowIfCancellationRequested();
        var area = Win32Native.GetWorkAreaPxForWindow(windowHandle);
        return Task.FromResult(new WindowBounds(area.X, area.Y, area.Width, area.Height));
    }

    public Task<WindowBounds> GetMonitorBoundsForWindowAsync(nint windowHandle, CancellationToken ct = default)
    {
        ct.ThrowIfCancellationRequested();
        var area = Win32Native.GetMonitorRectPxForWindow(windowHandle);
        return Task.FromResult(new WindowBounds(area.X, area.Y, area.Width, area.Height));
    }

    public Task<WindowBounds> GetClientAreaScreenBoundsAsync(nint windowHandle, CancellationToken ct = default)
    {
        ct.ThrowIfCancellationRequested();
        var area = Win32Native.GetClientAreaScreenRect(windowHandle);
        return Task.FromResult(new WindowBounds(area.X, area.Y, area.Width, area.Height));
    }

    public Task<WindowBounds> GetWindowBoundsAsync(nint windowHandle, CancellationToken ct = default)
    {
        ct.ThrowIfCancellationRequested();
        var bounds = Win32Native.GetWindowBoundsPx(windowHandle);
        return Task.FromResult(new WindowBounds(bounds.Left, bounds.Top, bounds.Width, bounds.Height));
    }
}
