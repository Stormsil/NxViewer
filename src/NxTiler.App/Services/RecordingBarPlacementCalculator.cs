using NxTiler.Domain.Windowing;

namespace NxTiler.App.Services;

internal static class RecordingBarPlacementCalculator
{
    public static bool TryCalculate(
        System.Windows.Window ownerWindow,
        System.Windows.Window barWindow,
        WindowBounds captureBounds,
        out double left,
        out double top)
    {
        barWindow.UpdateLayout();

        var barWidth = barWindow.ActualWidth;
        var barHeight = barWindow.ActualHeight;
        if (barWidth <= 0 || barHeight <= 0)
        {
            left = 0;
            top = 0;
            return false;
        }

        var source = System.Windows.PresentationSource.FromVisual(ownerWindow);
        var transform = source?.CompositionTarget?.TransformFromDevice ?? System.Windows.Media.Matrix.Identity;
        var scaleX = transform.M11;
        var scaleY = transform.M22;

        var captureLeft = captureBounds.X * scaleX;
        var captureTop = captureBounds.Y * scaleY;
        var captureBottom = (captureBounds.Y + captureBounds.Height) * scaleY;

        left = captureLeft + ((captureBounds.Width * scaleX - barWidth) / 2.0);
        top = captureTop - barHeight - 6;
        if (top < System.Windows.SystemParameters.VirtualScreenTop)
        {
            top = captureBottom + 6;
        }

        var minX = System.Windows.SystemParameters.VirtualScreenLeft;
        var maxX = minX + System.Windows.SystemParameters.VirtualScreenWidth - barWidth;
        left = Math.Clamp(left, minX, maxX);

        return true;
    }
}
