using System.Runtime.InteropServices;

namespace NxTiler.App.Native;

internal static class CaptureExclusionInterop
{
    // https://learn.microsoft.com/windows/win32/api/winuser/nf-winuser-setwindowdisplayaffinity
    // WDA_EXCLUDEFROMCAPTURE (0x11) hides the window from most capture APIs on Windows 10 2004+.
    private const uint WdaNone = 0x0;
    private const uint WdaExcludeFromCapture = 0x11;

    [DllImport("user32.dll")]
    private static extern bool SetWindowDisplayAffinity(nint hWnd, uint dwAffinity);

    public static void TryExcludeFromCapture(nint hWnd)
    {
        if (hWnd == nint.Zero)
        {
            return;
        }

        try
        {
            _ = SetWindowDisplayAffinity(hWnd, WdaExcludeFromCapture);
        }
        catch
        {
            // Best-effort: some Windows builds / window types may not support this.
        }
    }

    public static void TryIncludeInCapture(nint hWnd)
    {
        if (hWnd == nint.Zero)
        {
            return;
        }

        try
        {
            _ = SetWindowDisplayAffinity(hWnd, WdaNone);
        }
        catch
        {
        }
    }
}

