using NxTiler.Domain.Windowing;

namespace NxTiler.Domain.Vision;

public sealed record VisionDetection(
    string Label,
    float Confidence,
    WindowBounds Bounds,
    DateTime TimestampUtc
);
