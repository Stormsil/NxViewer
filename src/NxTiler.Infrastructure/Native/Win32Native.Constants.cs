namespace NxTiler.Infrastructure.Native;

internal static partial class Win32Native
{
    public const uint EventSystemForeground = 0x0003;
    public const uint EventObjectShow = 0x8002;
    public const uint EventObjectHide = 0x8003;
    public const uint EventObjectDestroy = 0x8001;
    public const uint EventObjectNameChange = 0x800C;
    public const uint EventObjectLocationChange = 0x800B;

    public const uint WinEventOutOfContext = 0x0000;
    public const uint WinEventSkipOwnProcess = 0x0002;

    public const uint MonitorDefaultToNearest = 2;

    public const int DwmwaExtendedFrameBounds = 9;

    public const int SwShowNormal = 1;
    public const int SwShowMinimized = 2;
    public const int SwShowMaximized = 3;

    public const uint SwpNoSize = 0x0001;
    public const uint SwpNoMove = 0x0002;
    public const uint SwpNoActivate = 0x0010;

    public const int GwlExStyle = -20;
    public const int WsExTransparent = 0x00000020;
    public const int WsExLayered = 0x00080000;

    public const uint ModAlt = 0x0001;
    public const uint ModControl = 0x0002;
    public const uint ModShift = 0x0004;
    public const uint ModWin = 0x0008;

    public const int VkLButton = 0x01;

    public const int SmXVirtualScreen = 76;
    public const int SmYVirtualScreen = 77;
    public const int SmCxVirtualScreen = 78;
    public const int SmCyVirtualScreen = 79;

    public static readonly nint HwndTop = nint.Zero;
    public static readonly nint HwndTopmost = new(-1);
    public static readonly nint HwndNoTopmost = new(-2);
}
