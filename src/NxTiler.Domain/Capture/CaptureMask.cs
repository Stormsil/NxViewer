namespace NxTiler.Domain.Capture;

public sealed record CaptureMask(
    int X,
    int Y,
    int Width,
    int Height,
    string? Label = null
);
