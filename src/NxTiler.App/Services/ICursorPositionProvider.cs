namespace NxTiler.App.Services;

public interface ICursorPositionProvider
{
    bool TryGetCursorPosition(out int x, out int y);
}
