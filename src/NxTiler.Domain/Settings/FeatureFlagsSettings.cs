namespace NxTiler.Domain.Settings;

public sealed record FeatureFlagsSettings(
    bool UseWgcRecordingEngine,
    bool EnableTemplateMatchingFallback,
    bool EnableYoloEngine
)
{
    /// <summary>When true, window rules engine filters and routes tiling targets. Default: true.</summary>
    public bool EnableRulesEngine { get; init; } = true;
}
