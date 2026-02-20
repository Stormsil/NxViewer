using NxTiler.Domain.Windowing;
using NxTiler.Infrastructure.Native;

namespace NxTiler.Infrastructure.Windowing;

public sealed partial class Win32WindowControlService
{
    public Task ApplyPlacementsAsync(IReadOnlyList<WindowPlacement> placements, CancellationToken ct = default)
    {
        foreach (var placement in placements)
        {
            ct.ThrowIfCancellationRequested();
            SafeSetWindowPos(placement.Handle, placement.X, placement.Y, placement.Width, placement.Height);
        }

        return Task.CompletedTask;
    }

    private static void SafeSetWindowPos(nint handle, int x, int y, int width, int height)
    {
        var current = Win32Native.GetWindowBoundsPx(handle);

        if (Math.Abs(current.Left - x) <= 1 &&
            Math.Abs(current.Top - y) <= 1 &&
            Math.Abs(current.Width - width) <= 1 &&
            Math.Abs(current.Height - height) <= 1)
        {
            return;
        }

        var border = Win32Native.GetInvisibleBorder(handle);
        var targetX = x - border.Left;
        var targetY = y - border.Top;
        var targetW = width + border.Left + border.Right;
        var targetH = height + border.Top + border.Bottom;

        Win32Native.SetWindowPos(
            handle,
            Win32Native.HwndNoTopmost,
            0,
            0,
            0,
            0,
            Win32Native.SwpNoMove | Win32Native.SwpNoSize | Win32Native.SwpNoActivate);
        Win32Native.SetWindowPos(handle, Win32Native.HwndTop, targetX, targetY, targetW, targetH, Win32Native.SwpNoActivate);
    }
}
