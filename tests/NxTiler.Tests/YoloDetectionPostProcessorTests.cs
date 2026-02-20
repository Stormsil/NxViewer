using NxTiler.Domain.Windowing;
using NxTiler.Infrastructure.Vision;

namespace NxTiler.Tests;

public sealed class YoloDetectionPostProcessorTests
{
    [Fact]
    public void NonMaximumSuppression_RemovesOverlapsWithinSameClass()
    {
        var candidates = new[]
        {
            new YoloCandidate(0, 0.95f, new WindowBounds(0, 0, 100, 100)),
            new YoloCandidate(0, 0.80f, new WindowBounds(5, 5, 100, 100)),
            new YoloCandidate(1, 0.90f, new WindowBounds(5, 5, 100, 100)),
        };

        var selected = YoloDetectionPostProcessor.NonMaximumSuppression(candidates, iouThreshold: 0.45f);

        Assert.Equal(2, selected.Count);
        Assert.Contains(selected, static x => x.ClassId == 0 && x.Confidence == 0.95f);
        Assert.Contains(selected, static x => x.ClassId == 1 && x.Confidence == 0.90f);
    }
}
