using System.Drawing;
using NxTiler.Domain.Capture;
using NxTiler.Domain.Windowing;

namespace NxTiler.Infrastructure.Capture;

public sealed partial class WgcCaptureService
{
    private static WindowBounds Intersect(WindowBounds left, WindowBounds right)
    {
        var x = Math.Max(left.X, right.X);
        var y = Math.Max(left.Y, right.Y);
        var rightEdge = Math.Min(left.X + left.Width, right.X + right.Width);
        var bottomEdge = Math.Min(left.Y + left.Height, right.Y + right.Height);
        var width = Math.Max(0, rightEdge - x);
        var height = Math.Max(0, bottomEdge - y);
        return new WindowBounds(x, y, width, height);
    }

    private static bool Intersects(WindowBounds bounds, CaptureMask mask)
    {
        return mask.Width > 0
            && mask.Height > 0
            && mask.X < bounds.X + bounds.Width
            && mask.X + mask.Width > bounds.X
            && mask.Y < bounds.Y + bounds.Height
            && mask.Y + mask.Height > bounds.Y;
    }

    private static Rectangle ToBitmapRectangle(WindowBounds sourceBounds, WindowBounds selectionBounds, Size bitmapSize)
    {
        if (sourceBounds.Width <= 0 || sourceBounds.Height <= 0)
        {
            return new Rectangle(0, 0, bitmapSize.Width, bitmapSize.Height);
        }

        var scaleX = bitmapSize.Width / (double)sourceBounds.Width;
        var scaleY = bitmapSize.Height / (double)sourceBounds.Height;

        var x = (int)Math.Round((selectionBounds.X - sourceBounds.X) * scaleX);
        var y = (int)Math.Round((selectionBounds.Y - sourceBounds.Y) * scaleY);
        var width = (int)Math.Round(selectionBounds.Width * scaleX);
        var height = (int)Math.Round(selectionBounds.Height * scaleY);

        var rect = new Rectangle(x, y, Math.Max(1, width), Math.Max(1, height));
        return Rectangle.Intersect(rect, new Rectangle(0, 0, bitmapSize.Width, bitmapSize.Height));
    }
}
