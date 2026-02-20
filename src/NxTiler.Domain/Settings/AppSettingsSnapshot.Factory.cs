using NxTiler.Domain.Overlay;

namespace NxTiler.Domain.Settings;

public sealed partial record AppSettingsSnapshot
{
    public static AppSettingsSnapshot CreateDefault()
    {
        return new AppSettingsSnapshot(
            Filters: new FiltersSettings(string.Empty, "^(WoW|Poe)\\d+$", false),
            Layout: new LayoutSettings(Gap: 1, TopPad: 28, SuspendOnMax: true, DragCooldownMs: 1500),
            Paths: new PathsSettings(
                NxsFolder: Environment.ExpandEnvironmentVariables(@"%USERPROFILE%\\Documents\\NoMachine"),
                RecordingFolder: Environment.GetFolderPath(Environment.SpecialFolder.MyVideos),
                FfmpegPath: "ffmpeg"),
            Recording: new RecordingSettings(Fps: 30),
            Hotkeys: CreateDefaultHotkeysSettings(),
            Overlay: new OverlaySettings(Left: -1, Top: -1),
            Ui: new UiSettings(
                TrayHintShown: false,
                AutoArrangeEnabled: false,
                SnapshotDebounceMs: 120,
                StartMinimizedToTray: false),
            DisabledSessions: Array.Empty<string>(),
            SchemaVersion: 4)
        {
            Capture = CreateDefaultCaptureSettings(),
            Vision = CreateDefaultVisionSettings(),
            OverlayPolicies = CreateDefaultOverlayPoliciesSettings(),
            FeatureFlags = CreateDefaultFeatureFlagsSettings(),
            Rules = CreateDefaultWindowRulesSettings(),
            Groups = CreateDefaultWindowGroupsSettings(),
            GridPresets = CreateDefaultGridPresetsSettings(),
        };
    }
}
