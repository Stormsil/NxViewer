using NxTiler.Domain.Windowing;

namespace NxTiler.Infrastructure.Vision;

internal sealed partial class YoloOutputParser
{
    private bool TryResolveCandidateBounds(
        float cx,
        float cy,
        float w,
        float h,
        YoloPreprocessResult preprocess,
        int originalWidth,
        int originalHeight,
        WindowBounds windowBounds,
        out WindowBounds bounds)
    {
        var left = cx - (w / 2f);
        var top = cy - (h / 2f);
        var right = cx + (w / 2f);
        var bottom = cy + (h / 2f);

        var originalLeft = Clamp((left - preprocess.PadX) / preprocess.Scale, 0, originalWidth);
        var originalTop = Clamp((top - preprocess.PadY) / preprocess.Scale, 0, originalHeight);
        var originalRight = Clamp((right - preprocess.PadX) / preprocess.Scale, 0, originalWidth);
        var originalBottom = Clamp((bottom - preprocess.PadY) / preprocess.Scale, 0, originalHeight);

        var width = (int)Math.Round(originalRight - originalLeft);
        var height = (int)Math.Round(originalBottom - originalTop);
        if (width < 1 || height < 1)
        {
            bounds = new WindowBounds(0, 0, 0, 0);
            return false;
        }

        bounds = new WindowBounds(
            X: windowBounds.X + (int)Math.Round(originalLeft),
            Y: windowBounds.Y + (int)Math.Round(originalTop),
            Width: width,
            Height: height);
        return true;
    }
}
