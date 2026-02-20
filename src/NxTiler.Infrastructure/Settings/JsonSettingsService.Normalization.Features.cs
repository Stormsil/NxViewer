using NxTiler.Domain.Settings;

namespace NxTiler.Infrastructure.Settings;

public sealed partial class JsonSettingsService
{
    private static HotkeysSettings NormalizeHotkeys(HotkeysSettings source, HotkeysSettings defaults)
    {
        return source with
        {
            ToggleOverlay = source.ToggleOverlay ?? defaults.ToggleOverlay,
            ToggleMainWindow = source.ToggleMainWindow ?? defaults.ToggleMainWindow,
            CycleMode = source.CycleMode ?? defaults.CycleMode,
            ToggleMinimize = source.ToggleMinimize ?? defaults.ToggleMinimize,
            NavigatePrevious = source.NavigatePrevious ?? defaults.NavigatePrevious,
            NavigateNext = source.NavigateNext ?? defaults.NavigateNext,
            InstantSnapshot = source.InstantSnapshot ?? defaults.InstantSnapshot,
            RegionSnapshot = source.RegionSnapshot ?? defaults.RegionSnapshot,
            Record = source.Record ?? defaults.Record,
            Pause = source.Pause ?? defaults.Pause,
            Stop = source.Stop ?? defaults.Stop,
            ToggleVision = source.ToggleVision ?? defaults.ToggleVision,
        };
    }

    private static CaptureSettings NormalizeCapture(CaptureSettings source, CaptureSettings defaults)
    {
        return source with
        {
            SnapshotFolder = string.IsNullOrWhiteSpace(source.SnapshotFolder)
                ? defaults.SnapshotFolder
                : source.SnapshotFolder,
        };
    }

    private static VisionSettings NormalizeVision(VisionSettings source, VisionSettings defaults)
    {
        return source with
        {
            ConfidenceThreshold = source.ConfidenceThreshold <= 0
                ? defaults.ConfidenceThreshold
                : source.ConfidenceThreshold,
            PreferredEngine = string.IsNullOrWhiteSpace(source.PreferredEngine)
                ? defaults.PreferredEngine
                : source.PreferredEngine,
            TemplateDirectory = source.TemplateDirectory ?? defaults.TemplateDirectory,
            YoloModelPath = source.YoloModelPath ?? defaults.YoloModelPath,
        };
    }

    private static FeatureFlagsSettings NormalizeFeatureFlags(FeatureFlagsSettings source)
    {
        return source with
        {
            UseWgcRecordingEngine = source.UseWgcRecordingEngine,
            EnableTemplateMatchingFallback = source.EnableTemplateMatchingFallback,
            EnableYoloEngine = source.EnableYoloEngine,
        };
    }
}
