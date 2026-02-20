using System.Drawing;
using NxTiler.Domain.Windowing;
using NxTiler.Infrastructure.Native;

namespace NxTiler.Infrastructure.Recording;

public sealed partial class WgcVideoRecordingEngine
{
    private static Rectangle ResolveCaptureRect(nint targetWindow, WindowBounds captureBounds, int frameWidth, int frameHeight)
    {
        var windowRect = Win32Native.GetWindowBoundsPx(targetWindow);
        var x = captureBounds.X - windowRect.Left;
        var y = captureBounds.Y - windowRect.Top;

        var rect = new Rectangle(x, y, captureBounds.Width, captureBounds.Height);
        var frameRect = new Rectangle(0, 0, frameWidth, frameHeight);
        rect = Rectangle.Intersect(rect, frameRect);

        if (rect.Width <= 0 || rect.Height <= 0)
        {
            rect = frameRect;
        }

        // Keep dimensions even for yuv420p encoders.
        var evenWidth = (rect.Width / 2) * 2;
        var evenHeight = (rect.Height / 2) * 2;
        return new Rectangle(rect.X, rect.Y, Math.Max(2, evenWidth), Math.Max(2, evenHeight));
    }

    private static Bitmap CropToCaptureRect(Bitmap source, Rectangle rect)
    {
        var frameRect = new Rectangle(0, 0, source.Width, source.Height);
        var clip = Rectangle.Intersect(frameRect, rect);
        if (clip.Width <= 0 || clip.Height <= 0)
        {
            return new Bitmap(source);
        }

        return source.Clone(clip, source.PixelFormat);
    }
}
