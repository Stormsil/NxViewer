using System.Runtime.InteropServices;
using System.Text;

namespace NxTiler.Infrastructure.Native;

internal static partial class Win32Native
{
    public const uint ProcessQueryLimitedInformation = 0x1000;

    [DllImport("kernel32.dll", SetLastError = true)]
    public static extern nint OpenProcess(uint dwDesiredAccess, bool bInheritHandle, uint dwProcessId);

    [DllImport("kernel32.dll", CharSet = CharSet.Unicode, SetLastError = true)]
    public static extern bool QueryFullProcessImageName(nint hProcess, uint dwFlags, StringBuilder lpExeName, ref uint lpdwSize);

    [DllImport("kernel32.dll", SetLastError = true)]
    public static extern bool CloseHandle(nint hObject);

    public static string GetClassNameSafe(nint hWnd, int capacity = 256)
    {
        var sb = new StringBuilder(capacity);
        GetClassName(hWnd, sb, sb.Capacity);
        return sb.ToString();
    }

    public static string QueryProcessExePath(uint processId)
    {
        var hProcess = OpenProcess(ProcessQueryLimitedInformation, false, processId);
        if (hProcess == nint.Zero)
        {
            return string.Empty;
        }

        try
        {
            var sb = new StringBuilder(1024);
            var size = (uint)sb.Capacity;
            return QueryFullProcessImageName(hProcess, 0, sb, ref size) ? sb.ToString() : string.Empty;
        }
        finally
        {
            CloseHandle(hProcess);
        }
    }
}
