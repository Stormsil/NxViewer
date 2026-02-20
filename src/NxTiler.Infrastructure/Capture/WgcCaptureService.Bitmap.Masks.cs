using System.Drawing;
using NxTiler.Domain.Capture;
using NxTiler.Domain.Windowing;

namespace NxTiler.Infrastructure.Capture;

public sealed partial class WgcCaptureService
{
    private static void ApplyMasks(Bitmap bitmap, WindowBounds outputBounds, IReadOnlyList<CaptureMask>? masks)
    {
        if (masks is null || masks.Count == 0 || bitmap.Width <= 0 || bitmap.Height <= 0)
        {
            return;
        }

        var canvasBounds = new Rectangle(0, 0, bitmap.Width, bitmap.Height);
        var scaleX = outputBounds.Width > 0 ? bitmap.Width / (double)outputBounds.Width : 1d;
        var scaleY = outputBounds.Height > 0 ? bitmap.Height / (double)outputBounds.Height : 1d;

        using var graphics = Graphics.FromImage(bitmap);
        using var brush = new SolidBrush(Color.Black);

        foreach (var mask in masks)
        {
            if (!Intersects(outputBounds, mask))
            {
                continue;
            }

            var intersection = Intersect(outputBounds, new WindowBounds(mask.X, mask.Y, mask.Width, mask.Height));
            if (intersection.Width <= 0 || intersection.Height <= 0)
            {
                continue;
            }

            var mapped = new Rectangle(
                x: (int)Math.Round((intersection.X - outputBounds.X) * scaleX),
                y: (int)Math.Round((intersection.Y - outputBounds.Y) * scaleY),
                width: Math.Max(1, (int)Math.Round(intersection.Width * scaleX)),
                height: Math.Max(1, (int)Math.Round(intersection.Height * scaleY)));

            mapped = Rectangle.Intersect(mapped, canvasBounds);
            if (mapped.Width > 0 && mapped.Height > 0)
            {
                graphics.FillRectangle(brush, mapped);
            }
        }
    }
}
