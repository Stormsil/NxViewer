// Win32.cs
using System;
using System.Runtime.InteropServices;
using System.Text;

namespace NxTiler
{
    internal static class Win32
    {
        // ===== Delegates =====
        public delegate bool EnumWindowsProc(IntPtr hWnd, IntPtr lParam);

        // ===== user32.dll =====
        [DllImport("user32.dll")] public static extern bool EnumWindows(EnumWindowsProc lpEnumFunc, IntPtr lParam);
        [DllImport("user32.dll")] public static extern bool IsWindowVisible(IntPtr hWnd);

        [DllImport("user32.dll", CharSet = CharSet.Unicode)]
        public static extern int GetWindowText(IntPtr hWnd, StringBuilder lpString, int nMaxCount);

        [DllImport("user32.dll", CharSet = CharSet.Unicode)]
        public static extern int GetClassName(IntPtr hWnd, StringBuilder lpString, int nMaxCount);

        [DllImport("user32.dll")] public static extern bool GetWindowRect(IntPtr hWnd, out RECT lpRect);
        [DllImport("user32.dll")] public static extern bool MoveWindow(IntPtr hWnd, int X, int Y, int nWidth, int nHeight, bool bRepaint);
        [DllImport("user32.dll")] public static extern bool SetWindowPos(IntPtr hWnd, IntPtr hWndInsertAfter, int X, int Y, int cx, int cy, uint uFlags);
        [DllImport("user32.dll")] public static extern bool ShowWindow(IntPtr hWnd, int nCmdShow);
        [DllImport("user32.dll")] public static extern bool GetWindowPlacement(IntPtr hWnd, ref WINDOWPLACEMENT lpwndpl);
        [DllImport("user32.dll")] public static extern uint GetWindowThreadProcessId(IntPtr hWnd, out uint lpdwProcessId);

        [DllImport("user32.dll")] public static extern bool GetClientRect(IntPtr hWnd, out RECT lpRect);
        [DllImport("user32.dll")] public static extern bool ClientToScreen(IntPtr hWnd, ref POINT lpPoint);

        [DllImport("user32.dll")] public static extern bool RegisterHotKey(IntPtr hWnd, int id, uint fsModifiers, uint vk);
        [DllImport("user32.dll")] public static extern bool UnregisterHotKey(IntPtr hWnd, int id);
        [DllImport("user32.dll")] public static extern short GetAsyncKeyState(int vKey);
        [DllImport("user32.dll")] public static extern IntPtr GetForegroundWindow();

        [DllImport("user32.dll")] public static extern IntPtr GetDesktopWindow();

        // Мониторы
        [DllImport("user32.dll")] public static extern IntPtr MonitorFromWindow(IntPtr hwnd, uint dwFlags);

        [DllImport("user32.dll", CharSet = CharSet.Unicode)]
        public static extern bool GetMonitorInfo(IntPtr hMonitor, ref MONITORINFOEX lpmi);
        [DllImport("user32.dll", CharSet = CharSet.Auto)]
        public static extern bool GetMonitorInfo(IntPtr hMonitor, ref MONITORINFO lpmi);

        public const uint MONITOR_DEFAULTTONULL = 0;
        public const uint MONITOR_DEFAULTTOPRIMARY = 1;
        public const uint MONITOR_DEFAULTTONEAREST = 2;

        // ===== dwmapi.dll =====
        [DllImport("dwmapi.dll")]
        public static extern int DwmGetWindowAttribute(IntPtr hwnd, int dwAttribute, out RECT pvAttribute, int cbAttribute);

        public const int DWMWA_EXTENDED_FRAME_BOUNDS = 9;

        // ===== Constants =====
        public const int SW_SHOWNORMAL = 1;
        public const int SW_SHOWMINIMIZED = 2;
        public const int SW_SHOWMAXIMIZED = 3;

        public const uint SWP_NOSIZE = 0x0001;
        public const uint SWP_NOMOVE = 0x0002;
        public const uint SWP_NOZORDER = 0x0004;
        public const uint SWP_NOREDRAW = 0x0008;
        public const uint SWP_NOACTIVATE = 0x0010;

        public const uint MOD_ALT = 0x0001;
        public const uint MOD_CONTROL = 0x0002;
        public const uint MOD_SHIFT = 0x0004;
        public const uint MOD_WIN = 0x0008;
        
        public const int VK_LBUTTON = 0x01;
        public const int VK_TAB = 0x09;

        public static readonly IntPtr HWND_TOP = IntPtr.Zero;
        public static readonly IntPtr HWND_TOPMOST = new IntPtr(-1);
        public static readonly IntPtr HWND_NOTOPMOST = new IntPtr(-2);

        // ===== Structs =====
        [StructLayout(LayoutKind.Sequential)]
        public struct RECT
        {
            public int Left, Top, Right, Bottom;
            public int Width => Right - Left;
            public int Height => Bottom - Top;
            public override string ToString() => $"[{Left},{Top} - {Right},{Bottom}] ({Width}x{Height})";
        }

        [StructLayout(LayoutKind.Sequential)]
        public struct MONITORINFO
        {
            public int cbSize;
            public RECT rcMonitor;
            public RECT rcWork;
            public uint dwFlags;
        }

        [StructLayout(LayoutKind.Sequential)]
        public struct POINT
        {
            public int X, Y;
        }

        [StructLayout(LayoutKind.Sequential)]
        public struct WINDOWPLACEMENT
        {
            public int length;
            public int flags;
            public int showCmd;
            public POINT ptMinPosition;
            public POINT ptMaxPosition;
            public RECT rcNormalPosition;
        }

        [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Unicode)]
        public struct MONITORINFOEX
        {
            public int cbSize;
            public RECT rcMonitor;  // полная область монитора (px)
            public RECT rcWork;     // рабочая область (px), без таскбара/доков
            public uint dwFlags;
            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 32)]
            public string szDevice;
        }

        // ===== Helpers =====

        public static string GetWindowText(IntPtr hWnd, int capacity = 512)
        {
            var sb = new StringBuilder(capacity);
            GetWindowText(hWnd, sb, sb.Capacity);
            return sb.ToString();
        }

        public static string GetClassNameSafe(IntPtr hWnd, int capacity = 256)
        {
            var sb = new StringBuilder(capacity);
            GetClassName(hWnd, sb, sb.Capacity);
            return sb.ToString();
        }

        // Возвращает разницу между "Грязным" rect (GetWindowRect) и "Чистым" (ExtendedFrameBounds)
        public static (int left, int top, int right, int bottom) GetInvisibleBorder(IntPtr hWnd)
        {
            RECT rcWindow;
            GetWindowRect(hWnd, out rcWindow);

            RECT rcFrame;
            if (DwmGetWindowAttribute(hWnd, DWMWA_EXTENDED_FRAME_BOUNDS, out rcFrame, Marshal.SizeOf<RECT>()) != 0)
                return (0, 0, 0, 0);

            // Invisible border is the difference between Window Rect and Frame Rect
            int left = rcFrame.Left - rcWindow.Left;
            int top = rcFrame.Top - rcWindow.Top; // usually 0 on Win10 side/bottom borders
            int right = rcWindow.Right - rcFrame.Right;
            int bottom = rcWindow.Bottom - rcFrame.Bottom;

            return (left, top, right, bottom);
        }
        
        public static (int left, int top, int right, int bottom) GetNonClientThickness(IntPtr hWnd)
        {
            // Внешняя граница окна (без тени), Win10/11
            RECT efb;
            int hr = DwmGetWindowAttribute(hWnd, DWMWA_EXTENDED_FRAME_BOUNDS, out efb, Marshal.SizeOf<RECT>());
            if (hr != 0) // если DWM выключен — fallback на обычный прямоугольник
                GetWindowRect(hWnd, out efb);

            // Клиентский прямоугольник (0..W, 0..H)
            GetClientRect(hWnd, out RECT cr);
            POINT ptClientTL = new POINT { X = 0, Y = 0 };
            ClientToScreen(hWnd, ref ptClientTL); // позиция клиента в экране

            int left = ptClientTL.X - efb.Left;
            int top = ptClientTL.Y - efb.Top;
            int width = cr.Right - cr.Left;
            int height = cr.Bottom - cr.Top;
            int right = (efb.Right - efb.Left) - width - left;
            int bottom = (efb.Bottom - efb.Top) - height - top;

            if (left < 0) left = 0;
            if (top < 0) top = 0;
            if (right < 0) right = 0;
            if (bottom < 0) bottom = 0;

            return (left, top, right, bottom);
        }

        public static (int x, int y, int w, int h) GetWorkAreaPxForWindow(IntPtr hWnd)
        {
            var hMon = MonitorFromWindow(hWnd, MONITOR_DEFAULTTONEAREST);
            if (hMon != IntPtr.Zero)
            {
                var mi = new MONITORINFO();
                mi.cbSize = Marshal.SizeOf(mi);
                if (GetMonitorInfo(hMon, ref mi))
                {
                    return (mi.rcWork.Left, mi.rcWork.Top, mi.rcWork.Right - mi.rcWork.Left, mi.rcWork.Bottom - mi.rcWork.Top);
                }
            }

            GetWindowRect(GetDesktopWindow(), out RECT rDesk);
            return (rDesk.Left, rDesk.Top, rDesk.Width, rDesk.Height);
        }

        public static RECT GetWindowBoundsPx(IntPtr hWnd)
        {
            if (DwmGetWindowAttribute(hWnd, DWMWA_EXTENDED_FRAME_BOUNDS, out RECT r, Marshal.SizeOf<RECT>()) == 0)
                return r;
            GetWindowRect(hWnd, out r);
            return r;
        }
    }
}