namespace NxTiler.Infrastructure.Native;

internal static partial class Win32Native
{
    public delegate bool EnumWindowsProc(nint hWnd, nint lParam);
    public delegate void WinEventDelegate(
        nint hWinEventHook,
        uint eventType,
        nint hwnd,
        int idObject,
        int idChild,
        uint dwEventThread,
        uint dwmsEventTime);

}
