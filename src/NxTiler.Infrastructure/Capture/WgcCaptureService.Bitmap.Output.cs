using System.Drawing;
using System.Drawing.Imaging;
using NxTiler.Domain.Capture;
using NxTiler.Domain.Enums;
using NxTiler.Domain.Windowing;

namespace NxTiler.Infrastructure.Capture;

public sealed partial class WgcCaptureService
{
    private static Bitmap CreateOutputBitmap(
        Bitmap sourceBitmap,
        WindowBounds baseBounds,
        CaptureRequest request,
        out WindowBounds outputBounds)
    {
        outputBounds = baseBounds;

        if (request.Mode != CaptureMode.RegionSnapshot || request.Region is null)
        {
            var full = new Bitmap(sourceBitmap);
            ApplyMasks(full, outputBounds, request.Masks);
            return full;
        }

        var intersection = Intersect(baseBounds, request.Region);
        if (intersection.Width <= 0 || intersection.Height <= 0)
        {
            var fallback = new Bitmap(sourceBitmap);
            ApplyMasks(fallback, outputBounds, request.Masks);
            return fallback;
        }

        outputBounds = intersection;

        var sourceRect = ToBitmapRectangle(baseBounds, outputBounds, sourceBitmap.Size);
        if (sourceRect.Width <= 0 || sourceRect.Height <= 0)
        {
            var fallback = new Bitmap(sourceBitmap);
            outputBounds = baseBounds;
            ApplyMasks(fallback, outputBounds, request.Masks);
            return fallback;
        }

        var cropped = new Bitmap(sourceRect.Width, sourceRect.Height, PixelFormat.Format32bppPArgb);
        using (var graphics = Graphics.FromImage(cropped))
        {
            graphics.DrawImage(
                sourceBitmap,
                new Rectangle(0, 0, cropped.Width, cropped.Height),
                sourceRect,
                GraphicsUnit.Pixel);
        }

        ApplyMasks(cropped, outputBounds, request.Masks);
        return cropped;
    }
}
