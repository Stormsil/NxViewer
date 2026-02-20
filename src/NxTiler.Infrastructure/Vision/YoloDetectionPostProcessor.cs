using NxTiler.Domain.Windowing;

namespace NxTiler.Infrastructure.Vision;

internal static class YoloDetectionPostProcessor
{
    public static List<YoloCandidate> NonMaximumSuppression(IReadOnlyList<YoloCandidate> candidates, float iouThreshold)
    {
        var ordered = candidates
            .OrderByDescending(static x => x.Confidence)
            .ToList();
        var selected = new List<YoloCandidate>(ordered.Count);

        while (ordered.Count > 0)
        {
            var current = ordered[0];
            ordered.RemoveAt(0);
            selected.Add(current);

            ordered.RemoveAll(other =>
                other.ClassId == current.ClassId &&
                ComputeIou(current.Bounds, other.Bounds) >= iouThreshold);
        }

        return selected;
    }

    private static float ComputeIou(WindowBounds a, WindowBounds b)
    {
        var left = Math.Max(a.X, b.X);
        var top = Math.Max(a.Y, b.Y);
        var right = Math.Min(a.X + a.Width, b.X + b.Width);
        var bottom = Math.Min(a.Y + a.Height, b.Y + b.Height);

        var intersectionWidth = Math.Max(0, right - left);
        var intersectionHeight = Math.Max(0, bottom - top);
        if (intersectionWidth == 0 || intersectionHeight == 0)
        {
            return 0f;
        }

        var intersection = intersectionWidth * intersectionHeight;
        var union = (a.Width * a.Height) + (b.Width * b.Height) - intersection;
        return union <= 0 ? 0f : intersection / (float)union;
    }
}
