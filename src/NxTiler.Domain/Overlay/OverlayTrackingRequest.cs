using NxTiler.Domain.Enums;

namespace NxTiler.Domain.Overlay;

public sealed record OverlayTrackingRequest(
    double BaseWidth,
    double BaseHeight,
    OverlayVisibilityMode VisibilityMode,
    bool ScaleWithWindow,
    OverlayAnchor Anchor = OverlayAnchor.TopLeft
);
