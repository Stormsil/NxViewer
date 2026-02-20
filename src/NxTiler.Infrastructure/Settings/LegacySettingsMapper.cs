using NxTiler.Domain.Settings;
using NxTiler.Infrastructure.Legacy;

namespace NxTiler.Infrastructure.Settings;

internal static class LegacySettingsMapper
{
    public static AppSettingsSnapshot Read()
    {
        var legacy = LegacyAppSettings.Default;
        var defaultHotkeys = AppSettingsSnapshot.CreateDefault().Hotkeys;

        var disabledSessions = legacy.DisabledFiles.Cast<string>()
            .Where(static x => !string.IsNullOrWhiteSpace(x))
            .Distinct(StringComparer.OrdinalIgnoreCase)
            .ToArray();

        return new AppSettingsSnapshot(
            Filters: new FiltersSettings(
                TitleFilter: legacy.TitleFilter ?? string.Empty,
                NameFilter: string.IsNullOrWhiteSpace(legacy.NameFilter) ? AppSettingsSnapshot.CreateDefault().Filters.NameFilter : legacy.NameFilter,
                SortDescending: legacy.SortDesc),
            Layout: new LayoutSettings(
                Gap: legacy.Gap,
                TopPad: legacy.TopPad,
                SuspendOnMax: legacy.SuspendOnMax,
                DragCooldownMs: legacy.DragCooldownMs <= 0 ? 1500 : legacy.DragCooldownMs),
            Paths: new PathsSettings(
                NxsFolder: legacy.NxsFolder,
                RecordingFolder: legacy.RecordingFolder,
                FfmpegPath: legacy.FfmpegPath),
            Recording: new RecordingSettings(legacy.RecordingFps <= 0 ? 30 : legacy.RecordingFps),
            Hotkeys: new HotkeysSettings(
                ToggleOverlay: new HotkeyBinding(legacy.HkOverlayMod, legacy.HkOverlayKey),
                ToggleMainWindow: new HotkeyBinding(legacy.HkMainMod, legacy.HkMainKey),
                CycleMode: new HotkeyBinding(legacy.HkFocusMod, legacy.HkFocusKey),
                ToggleMinimize: new HotkeyBinding(legacy.HkMinimizeMod, legacy.HkMinimizeKey),
                NavigatePrevious: new HotkeyBinding(legacy.HkPrevMod, legacy.HkPrevKey),
                NavigateNext: new HotkeyBinding(legacy.HkNextMod, legacy.HkNextKey),
                InstantSnapshot: defaultHotkeys.InstantSnapshot,
                RegionSnapshot: defaultHotkeys.RegionSnapshot,
                Record: new HotkeyBinding(legacy.HkRecordMod, legacy.HkRecordKey),
                Pause: new HotkeyBinding(legacy.HkRecPauseMod, legacy.HkRecPauseKey),
                Stop: new HotkeyBinding(legacy.HkRecStopMod, legacy.HkRecStopKey))
            {
                ToggleVision = defaultHotkeys.ToggleVision,
            },
            Overlay: new OverlaySettings(legacy.OverlayLeft, legacy.OverlayTop),
            Ui: new UiSettings(
                TrayHintShown: legacy.TrayHintShown,
                AutoArrangeEnabled: false,
                SnapshotDebounceMs: 120,
                StartMinimizedToTray: false),
            DisabledSessions: disabledSessions,
            SchemaVersion: AppSettingsSnapshot.CreateDefault().SchemaVersion);
    }
}
