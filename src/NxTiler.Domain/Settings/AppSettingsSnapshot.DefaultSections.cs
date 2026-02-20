namespace NxTiler.Domain.Settings;

public sealed partial record AppSettingsSnapshot
{
    public CaptureSettings Capture { get; init; } = CreateDefaultCaptureSettings();

    public VisionSettings Vision { get; init; } = CreateDefaultVisionSettings();

    public OverlayPoliciesSettings OverlayPolicies { get; init; } = CreateDefaultOverlayPoliciesSettings();

    public FeatureFlagsSettings FeatureFlags { get; init; } = CreateDefaultFeatureFlagsSettings();

    public WindowRulesSettings Rules { get; init; } = CreateDefaultWindowRulesSettings();

    public WindowGroupsSettings Groups { get; init; } = CreateDefaultWindowGroupsSettings();

    public GridPresetsSettings GridPresets { get; init; } = CreateDefaultGridPresetsSettings();
}
