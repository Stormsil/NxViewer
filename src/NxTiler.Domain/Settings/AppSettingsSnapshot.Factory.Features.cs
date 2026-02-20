using NxTiler.Domain.Enums;
using NxTiler.Domain.Overlay;

namespace NxTiler.Domain.Settings;

public sealed partial record AppSettingsSnapshot
{
    private static CaptureSettings CreateDefaultCaptureSettings()
    {
        return new CaptureSettings(
            PreferWgc: true,
            CopySnapshotToClipboardByDefault: true,
            SnapshotFolder: Environment.GetFolderPath(Environment.SpecialFolder.MyPictures));
    }

    private static VisionSettings CreateDefaultVisionSettings()
    {
        return new VisionSettings(
            Enabled: false,
            ConfidenceThreshold: 0.5f,
            PreferredEngine: "template")
        {
            TemplateDirectory = string.Empty,
            YoloModelPath = string.Empty,
        };
    }

    private static OverlayPoliciesSettings CreateDefaultOverlayPoliciesSettings()
    {
        return new OverlayPoliciesSettings(
            VisibilityMode: OverlayVisibilityMode.Always,
            ScaleWithWindow: true,
            HideOnHover: false,
            Anchor: OverlayAnchor.TopLeft);
    }

    private static FeatureFlagsSettings CreateDefaultFeatureFlagsSettings()
    {
        return new FeatureFlagsSettings(
            UseWgcRecordingEngine: true,
            EnableTemplateMatchingFallback: true,
            EnableYoloEngine: false);
    }
}
