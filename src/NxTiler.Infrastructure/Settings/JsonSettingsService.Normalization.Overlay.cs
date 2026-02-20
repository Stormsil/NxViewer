using NxTiler.Domain.Enums;
using NxTiler.Domain.Overlay;
using NxTiler.Domain.Settings;

namespace NxTiler.Infrastructure.Settings;

public sealed partial class JsonSettingsService
{
    private static OverlayPoliciesSettings NormalizeOverlayPolicies(OverlayPoliciesSettings overlayPolicies)
    {
        var visibilityMode = overlayPolicies.VisibilityMode;
        if (overlayPolicies.HideOnHover && visibilityMode == OverlayVisibilityMode.Always)
        {
            visibilityMode = OverlayVisibilityMode.HideOnHover;
        }

        var scaleWithWindow = overlayPolicies.ScaleWithWindow;
        var anchor = Enum.IsDefined(typeof(OverlayAnchor), overlayPolicies.Anchor)
            ? overlayPolicies.Anchor
            : OverlayAnchor.TopLeft;
        return overlayPolicies with
        {
            VisibilityMode = visibilityMode,
            ScaleWithWindow = scaleWithWindow,
            HideOnHover = visibilityMode == OverlayVisibilityMode.HideOnHover,
            Anchor = anchor,
        };
    }
}
