using NxTiler.Domain.Overlay;
using NxTiler.Domain.Windowing;

namespace NxTiler.App.Services;

public sealed partial class OverlayTrackingService
{
    private static (double Left, double Top) ResolveAnchoredPosition(
        WindowBounds targetBounds,
        OverlayAnchor anchor,
        double overlayWidth,
        double overlayHeight)
    {
        const double padding = 8d;

        var targetLeft = (double)targetBounds.X;
        var targetTop = (double)targetBounds.Y;
        var targetWidth = (double)targetBounds.Width;
        var targetHeight = (double)targetBounds.Height;
        var targetRight = targetLeft + targetWidth;
        var targetBottom = targetTop + targetHeight;

        return anchor switch
        {
            OverlayAnchor.TopLeft => (targetLeft + padding, targetTop + padding),
            OverlayAnchor.TopCenter => (targetLeft + ((targetWidth - overlayWidth) / 2d), targetTop + padding),
            OverlayAnchor.TopRight => (targetRight - overlayWidth - padding, targetTop + padding),
            OverlayAnchor.CenterLeft => (targetLeft + padding, targetTop + ((targetHeight - overlayHeight) / 2d)),
            OverlayAnchor.Center => (targetLeft + ((targetWidth - overlayWidth) / 2d), targetTop + ((targetHeight - overlayHeight) / 2d)),
            OverlayAnchor.CenterRight => (targetRight - overlayWidth - padding, targetTop + ((targetHeight - overlayHeight) / 2d)),
            OverlayAnchor.BottomLeft => (targetLeft + padding, targetBottom - overlayHeight - padding),
            OverlayAnchor.BottomCenter => (targetLeft + ((targetWidth - overlayWidth) / 2d), targetBottom - overlayHeight - padding),
            OverlayAnchor.BottomRight => (targetRight - overlayWidth - padding, targetBottom - overlayHeight - padding),
            _ => (targetLeft + padding, targetTop + padding),
        };
    }

    private static (double Left, double Top) ClampToMonitorBounds(
        WindowBounds monitorBounds,
        double left,
        double top,
        double overlayWidth,
        double overlayHeight)
    {
        const double padding = 8d;
        var minLeft = monitorBounds.X + padding;
        var minTop = monitorBounds.Y + padding;
        var maxLeft = monitorBounds.X + monitorBounds.Width - overlayWidth - padding;
        var maxTop = monitorBounds.Y + monitorBounds.Height - overlayHeight - padding;

        left = Math.Clamp(left, minLeft, Math.Max(minLeft, maxLeft));
        top = Math.Clamp(top, minTop, Math.Max(minTop, maxTop));

        return (left, top);
    }
}
