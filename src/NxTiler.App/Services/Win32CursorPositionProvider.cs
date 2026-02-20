using NxTiler.App.Native;

namespace NxTiler.App.Services;

public sealed class Win32CursorPositionProvider : ICursorPositionProvider
{
    public bool TryGetCursorPosition(out int x, out int y)
    {
        return CursorInterop.TryGetCursorPosition(out x, out y);
    }
}
