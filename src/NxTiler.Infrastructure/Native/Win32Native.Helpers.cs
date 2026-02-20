using System.Runtime.InteropServices;
using System.Text;

namespace NxTiler.Infrastructure.Native;

internal static partial class Win32Native
{
    public static string GetWindowTextSafe(nint hWnd, int capacity = 512)
    {
        var sb = new StringBuilder(capacity);
        GetWindowText(hWnd, sb, sb.Capacity);
        return sb.ToString();
    }

    public static Rect GetWindowBoundsPx(nint hWnd)
    {
        if (DwmGetWindowAttribute(hWnd, DwmwaExtendedFrameBounds, out Rect dwmRect, Marshal.SizeOf<Rect>()) == 0)
        {
            return dwmRect;
        }

        GetWindowRect(hWnd, out var rawRect);
        return rawRect;
    }

    public static (int Left, int Top, int Right, int Bottom) GetInvisibleBorder(nint hWnd)
    {
        GetWindowRect(hWnd, out var rawRect);

        if (DwmGetWindowAttribute(hWnd, DwmwaExtendedFrameBounds, out var frameRect, Marshal.SizeOf<Rect>()) != 0)
        {
            return (0, 0, 0, 0);
        }

        var left = frameRect.Left - rawRect.Left;
        var top = frameRect.Top - rawRect.Top;
        var right = rawRect.Right - frameRect.Right;
        var bottom = rawRect.Bottom - frameRect.Bottom;

        return (left, top, right, bottom);
    }

    public static (int X, int Y, int Width, int Height) GetWorkAreaPxForWindow(nint hWnd)
    {
        var monitor = MonitorFromWindow(hWnd, MonitorDefaultToNearest);
        if (monitor != nint.Zero)
        {
            var info = new MonitorInfo { cbSize = Marshal.SizeOf<MonitorInfo>() };
            if (GetMonitorInfo(monitor, ref info))
            {
                return (info.rcWork.Left, info.rcWork.Top, info.rcWork.Width, info.rcWork.Height);
            }
        }

        GetWindowRect(GetDesktopWindow(), out var desktopRect);
        return (desktopRect.Left, desktopRect.Top, desktopRect.Width, desktopRect.Height);
    }

    public static (int X, int Y, int Width, int Height) GetMonitorRectPxForWindow(nint hWnd)
    {
        var monitor = MonitorFromWindow(hWnd, MonitorDefaultToNearest);
        if (monitor != nint.Zero)
        {
            var info = new MonitorInfo { cbSize = Marshal.SizeOf<MonitorInfo>() };
            if (GetMonitorInfo(monitor, ref info))
            {
                return (info.rcMonitor.Left, info.rcMonitor.Top, info.rcMonitor.Width, info.rcMonitor.Height);
            }
        }

        GetWindowRect(GetDesktopWindow(), out var desktopRect);
        return (desktopRect.Left, desktopRect.Top, desktopRect.Width, desktopRect.Height);
    }

    public static (int X, int Y, int Width, int Height) GetClientAreaScreenRect(nint hWnd)
    {
        GetClientRect(hWnd, out var clientRect);
        var point = new Point { X = 0, Y = 0 };
        ClientToScreen(hWnd, ref point);
        return (point.X, point.Y, clientRect.Width, clientRect.Height);
    }

    public static (int Left, int Top, int Width, int Height) GetVirtualScreenRectPx()
    {
        var left = GetSystemMetrics(SmXVirtualScreen);
        var top = GetSystemMetrics(SmYVirtualScreen);
        var width = GetSystemMetrics(SmCxVirtualScreen);
        var height = GetSystemMetrics(SmCyVirtualScreen);
        return (left, top, width, height);
    }
}
