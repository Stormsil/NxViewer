namespace NxTiler.Domain.Overlay;

public sealed record OverlayTrackingState(
    bool IsVisible,
    double Left,
    double Top,
    double Width,
    double Height
);
