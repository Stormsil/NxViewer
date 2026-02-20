using NxTiler.Domain.Enums;
using NxTiler.Domain.Windowing;

namespace NxTiler.App.Services;

public sealed partial class OverlayTrackingService
{
    private static bool ResolveVisibility(
        OverlayVisibilityMode visibilityMode,
        WindowBounds targetBounds,
        ICursorPositionProvider cursorProvider)
    {
        var cursorInside = IsCursorInside(targetBounds, cursorProvider);
        return visibilityMode switch
        {
            OverlayVisibilityMode.Always => true,
            OverlayVisibilityMode.OnHover => cursorInside,
            OverlayVisibilityMode.HideOnHover => !cursorInside,
            _ => true,
        };
    }

    private static bool IsCursorInside(WindowBounds bounds, ICursorPositionProvider cursorProvider)
    {
        if (!cursorProvider.TryGetCursorPosition(out var x, out var y))
        {
            return false;
        }

        return x >= bounds.X
            && y >= bounds.Y
            && x < bounds.X + bounds.Width
            && y < bounds.Y + bounds.Height;
    }
}
