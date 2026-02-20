namespace NxTiler.Domain.Settings;

public sealed record VisionSettings(
    bool Enabled,
    float ConfidenceThreshold,
    string PreferredEngine
)
{
    public string TemplateDirectory { get; init; } = string.Empty;

    public string YoloModelPath { get; init; } = string.Empty;
}
