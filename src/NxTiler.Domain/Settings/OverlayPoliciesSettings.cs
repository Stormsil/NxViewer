using NxTiler.Domain.Overlay;
using NxTiler.Domain.Enums;

namespace NxTiler.Domain.Settings;

public sealed record OverlayPoliciesSettings(
    OverlayVisibilityMode VisibilityMode,
    bool ScaleWithWindow,
    bool HideOnHover,
    OverlayAnchor Anchor = OverlayAnchor.TopLeft
);
