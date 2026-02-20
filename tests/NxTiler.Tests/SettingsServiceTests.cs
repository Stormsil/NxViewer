using System.Text.Json;
using System.Text.Json.Nodes;
using System.Linq;
using Microsoft.Extensions.Logging.Abstractions;
using NxTiler.Domain.Enums;
using NxTiler.Domain.Overlay;
using NxTiler.Domain.Settings;
using NxTiler.Infrastructure.Legacy;
using NxTiler.Infrastructure.Settings;

namespace NxTiler.Tests;

public sealed class SettingsServiceTests
{
    [Fact]
    public void Constructor_CreatesJsonAndLegacyBackup_WhenSettingsFileMissing()
    {
        var root = GetSettingsRoot();
        RecreateDirectory(root);

        var migrationService = new SettingsMigrationService(NullLogger<SettingsMigrationService>.Instance);
        var service = new JsonSettingsService(NullLogger<JsonSettingsService>.Instance, migrationService);

        var settingsPath = Path.Combine(root, "settings.json");
        var backupPath = Path.Combine(root, "settings.legacy.backup.json");

        Assert.True(File.Exists(settingsPath));
        Assert.True(File.Exists(backupPath));
        Assert.Equal(AppSettingsSnapshot.CreateDefault().SchemaVersion, service.Current.SchemaVersion);
        Assert.False(service.Current.Ui.StartMinimizedToTray);
    }

    [Fact]
    public void Constructor_FallsBackToLegacy_WhenSettingsJsonIsInvalid()
    {
        var root = GetSettingsRoot();
        RecreateDirectory(root);
        File.WriteAllText(Path.Combine(root, "settings.json"), "{ this is invalid json");

        var migrationService = new SettingsMigrationService(NullLogger<SettingsMigrationService>.Instance);
        var service = new JsonSettingsService(NullLogger<JsonSettingsService>.Instance, migrationService);

        Assert.Equal(AppSettingsSnapshot.CreateDefault().SchemaVersion, service.Current.SchemaVersion);

        var persisted = File.ReadAllText(Path.Combine(root, "settings.json"));
        var parsed = JsonSerializer.Deserialize<AppSettingsSnapshot>(persisted);
        Assert.NotNull(parsed);
        Assert.Equal(AppSettingsSnapshot.CreateDefault().SchemaVersion, parsed.SchemaVersion);
    }

    [Fact]
    public void Constructor_NormalizesSnapshotDebounce_WhenOutOfRange()
    {
        var root = GetSettingsRoot();
        RecreateDirectory(root);

        var source = AppSettingsSnapshot.CreateDefault() with
        {
            Ui = new UiSettings(TrayHintShown: true, AutoArrangeEnabled: true, SnapshotDebounceMs: 1_500),
        };

        var json = JsonSerializer.Serialize(source, new JsonSerializerOptions { WriteIndented = true });
        File.WriteAllText(Path.Combine(root, "settings.json"), json);

        var migrationService = new SettingsMigrationService(NullLogger<SettingsMigrationService>.Instance);
        var service = new JsonSettingsService(NullLogger<JsonSettingsService>.Instance, migrationService);

        Assert.Equal(500, service.Current.Ui.SnapshotDebounceMs);

        var persisted = JsonSerializer.Deserialize<AppSettingsSnapshot>(File.ReadAllText(Path.Combine(root, "settings.json")));
        Assert.NotNull(persisted);
        Assert.Equal(500, persisted.Ui.SnapshotDebounceMs);
        Assert.False(persisted.Ui.StartMinimizedToTray);
    }

    [Fact]
    public void Constructor_DefaultsStartMinimizedToTray_WhenPropertyMissing()
    {
        var root = GetSettingsRoot();
        RecreateDirectory(root);

        var source = AppSettingsSnapshot.CreateDefault() with
        {
            Ui = new UiSettings(
                TrayHintShown: true,
                AutoArrangeEnabled: true,
                SnapshotDebounceMs: 200,
                StartMinimizedToTray: true),
        };

        var rootNode = JsonNode.Parse(JsonSerializer.Serialize(source))!.AsObject();
        rootNode["Ui"]!.AsObject().Remove("StartMinimizedToTray");
        var json = rootNode.ToJsonString(new JsonSerializerOptions { WriteIndented = true });
        File.WriteAllText(Path.Combine(root, "settings.json"), json);

        var migrationService = new SettingsMigrationService(NullLogger<SettingsMigrationService>.Instance);
        var service = new JsonSettingsService(NullLogger<JsonSettingsService>.Instance, migrationService);

        Assert.False(service.Current.Ui.StartMinimizedToTray);

        var persisted = JsonSerializer.Deserialize<AppSettingsSnapshot>(File.ReadAllText(Path.Combine(root, "settings.json")));
        Assert.NotNull(persisted);
        Assert.False(persisted.Ui.StartMinimizedToTray);
    }

    [Fact]
    public void Constructor_DefaultsNewHotkeys_WhenMissingInExistingSettings()
    {
        var root = GetSettingsRoot();
        RecreateDirectory(root);

        var source = AppSettingsSnapshot.CreateDefault();
        var rootNode = JsonNode.Parse(JsonSerializer.Serialize(source))!.AsObject();
        var hotkeys = rootNode["Hotkeys"]!.AsObject();
        hotkeys.Remove("InstantSnapshot");
        hotkeys.Remove("RegionSnapshot");
        hotkeys.Remove("ToggleVision");

        var json = rootNode.ToJsonString(new JsonSerializerOptions { WriteIndented = true });
        File.WriteAllText(Path.Combine(root, "settings.json"), json);

        var migrationService = new SettingsMigrationService(NullLogger<SettingsMigrationService>.Instance);
        var service = new JsonSettingsService(NullLogger<JsonSettingsService>.Instance, migrationService);

        var defaults = AppSettingsSnapshot.CreateDefault().Hotkeys;
        Assert.Equal(defaults.InstantSnapshot, service.Current.Hotkeys.InstantSnapshot);
        Assert.Equal(defaults.RegionSnapshot, service.Current.Hotkeys.RegionSnapshot);
        Assert.Equal(defaults.ToggleVision, service.Current.Hotkeys.ToggleVision);
    }

    [Fact]
    public void Constructor_DefaultsVisionPaths_WhenMissingInExistingSettings()
    {
        var root = GetSettingsRoot();
        RecreateDirectory(root);

        var source = AppSettingsSnapshot.CreateDefault() with
        {
            Vision = AppSettingsSnapshot.CreateDefault().Vision with
            {
                TemplateDirectory = @"C:\Temp\Templates",
                YoloModelPath = @"C:\Models\yolo.onnx",
            },
        };

        var rootNode = JsonNode.Parse(JsonSerializer.Serialize(source))!.AsObject();
        var vision = rootNode["Vision"]!.AsObject();
        vision.Remove("TemplateDirectory");
        vision.Remove("YoloModelPath");

        var json = rootNode.ToJsonString(new JsonSerializerOptions { WriteIndented = true });
        File.WriteAllText(Path.Combine(root, "settings.json"), json);

        var migrationService = new SettingsMigrationService(NullLogger<SettingsMigrationService>.Instance);
        var service = new JsonSettingsService(NullLogger<JsonSettingsService>.Instance, migrationService);

        Assert.Equal(string.Empty, service.Current.Vision.TemplateDirectory);
        Assert.Equal(string.Empty, service.Current.Vision.YoloModelPath);
    }

    [Fact]
    public void Constructor_NormalizesOverlayVisibilityMode_FromLegacyHideOnHoverFlag()
    {
        var root = GetSettingsRoot();
        RecreateDirectory(root);

        var source = AppSettingsSnapshot.CreateDefault() with
        {
            OverlayPolicies = new OverlayPoliciesSettings(
                VisibilityMode: OverlayVisibilityMode.Always,
                ScaleWithWindow: true,
                HideOnHover: true),
        };

        var json = JsonSerializer.Serialize(source, new JsonSerializerOptions { WriteIndented = true });
        File.WriteAllText(Path.Combine(root, "settings.json"), json);

        var migrationService = new SettingsMigrationService(NullLogger<SettingsMigrationService>.Instance);
        var service = new JsonSettingsService(NullLogger<JsonSettingsService>.Instance, migrationService);

        Assert.Equal(OverlayVisibilityMode.HideOnHover, service.Current.OverlayPolicies.VisibilityMode);
        Assert.True(service.Current.OverlayPolicies.HideOnHover);

        var persisted = JsonSerializer.Deserialize<AppSettingsSnapshot>(File.ReadAllText(Path.Combine(root, "settings.json")));
        Assert.NotNull(persisted);
        Assert.Equal(OverlayVisibilityMode.HideOnHover, persisted.OverlayPolicies.VisibilityMode);
        Assert.True(persisted.OverlayPolicies.HideOnHover);
    }

    [Fact]
    public void Constructor_NormalizesOverlayAnchor_WhenInvalidValue()
    {
        var root = GetSettingsRoot();
        RecreateDirectory(root);

        var source = AppSettingsSnapshot.CreateDefault();
        var rootNode = JsonNode.Parse(JsonSerializer.Serialize(source))!.AsObject();
        var overlayPolicies = rootNode["OverlayPolicies"]!.AsObject();
        overlayPolicies["Anchor"] = 999;

        var json = rootNode.ToJsonString(new JsonSerializerOptions { WriteIndented = true });
        File.WriteAllText(Path.Combine(root, "settings.json"), json);

        var migrationService = new SettingsMigrationService(NullLogger<SettingsMigrationService>.Instance);
        var service = new JsonSettingsService(NullLogger<JsonSettingsService>.Instance, migrationService);

        Assert.Equal(OverlayAnchor.TopLeft, service.Current.OverlayPolicies.Anchor);

        var persisted = JsonSerializer.Deserialize<AppSettingsSnapshot>(File.ReadAllText(Path.Combine(root, "settings.json")));
        Assert.NotNull(persisted);
        Assert.Equal(OverlayAnchor.TopLeft, persisted.OverlayPolicies.Anchor);
    }

    [Fact]
    public void Constructor_MigratesLegacyValues_AndPersistsBackup()
    {
        var root = GetSettingsRoot();
        RecreateDirectory(root);

        var legacy = LegacyAppSettings.Default;
        var snapshot = LegacySnapshot.Capture(legacy);
        try
        {
            legacy.TitleFilter = "NoMachine.*";
            legacy.NameFilter = "^(WoW|Poe|D4)\\d+$";
            legacy.SortDesc = true;
            legacy.Gap = 6;
            legacy.TopPad = 31;
            legacy.SuspendOnMax = false;
            legacy.DragCooldownMs = 2100;
            legacy.NxsFolder = @"C:\\Data\\NoMachine";
            legacy.RecordingFolder = @"C:\\Data\\Recordings";
            legacy.RecordingFps = 50;
            legacy.FfmpegPath = @"C:\\Tools\\ffmpeg.exe";
            legacy.OverlayLeft = 120.5;
            legacy.OverlayTop = 340.75;
            legacy.TrayHintShown = true;

            legacy.HkOverlayMod = 2;
            legacy.HkOverlayKey = 113;
            legacy.HkMainMod = 4;
            legacy.HkMainKey = 114;
            legacy.HkFocusMod = 8;
            legacy.HkFocusKey = 115;
            legacy.HkMinimizeMod = 1;
            legacy.HkMinimizeKey = 192;
            legacy.HkPrevMod = 0;
            legacy.HkPrevKey = 37;
            legacy.HkNextMod = 0;
            legacy.HkNextKey = 39;
            legacy.HkRecordMod = 2;
            legacy.HkRecordKey = 116;
            legacy.HkRecPauseMod = 2;
            legacy.HkRecPauseKey = 117;
            legacy.HkRecStopMod = 2;
            legacy.HkRecStopKey = 118;

            legacy.DisabledFiles.Clear();
            legacy.DisabledFiles.Add("WoW01");
            legacy.DisabledFiles.Add("wow01");
            legacy.DisabledFiles.Add("Poe02");
            legacy.DisabledFiles.Add(string.Empty);

            var migrationService = new SettingsMigrationService(NullLogger<SettingsMigrationService>.Instance);
            var service = new JsonSettingsService(NullLogger<JsonSettingsService>.Instance, migrationService);
            var migrated = service.Current;

            Assert.Equal("NoMachine.*", migrated.Filters.TitleFilter);
            Assert.Equal("^(WoW|Poe|D4)\\d+$", migrated.Filters.NameFilter);
            Assert.True(migrated.Filters.SortDescending);
            Assert.Equal(6, migrated.Layout.Gap);
            Assert.Equal(31, migrated.Layout.TopPad);
            Assert.False(migrated.Layout.SuspendOnMax);
            Assert.Equal(2100, migrated.Layout.DragCooldownMs);
            Assert.Equal(@"C:\\Data\\NoMachine", migrated.Paths.NxsFolder);
            Assert.Equal(@"C:\\Data\\Recordings", migrated.Paths.RecordingFolder);
            Assert.Equal(@"C:\\Tools\\ffmpeg.exe", migrated.Paths.FfmpegPath);
            Assert.Equal(50, migrated.Recording.Fps);
            Assert.Equal(120.5, migrated.Overlay.Left);
            Assert.Equal(340.75, migrated.Overlay.Top);
            Assert.True(migrated.Ui.TrayHintShown);
            Assert.False(migrated.Ui.AutoArrangeEnabled);
            Assert.False(migrated.Ui.StartMinimizedToTray);
            Assert.Equal(2u, migrated.Hotkeys.ToggleOverlay.Modifiers);
            Assert.Equal(113, migrated.Hotkeys.ToggleOverlay.VirtualKey);
            Assert.Equal(2u, migrated.Hotkeys.Record.Modifiers);
            Assert.Equal(116, migrated.Hotkeys.Record.VirtualKey);
            Assert.Equal(2u, migrated.Hotkeys.Stop.Modifiers);
            Assert.Equal(118, migrated.Hotkeys.Stop.VirtualKey);
            Assert.Equal(AppSettingsSnapshot.CreateDefault().Hotkeys.InstantSnapshot, migrated.Hotkeys.InstantSnapshot);
            Assert.Equal(AppSettingsSnapshot.CreateDefault().Hotkeys.RegionSnapshot, migrated.Hotkeys.RegionSnapshot);
            Assert.Equal(AppSettingsSnapshot.CreateDefault().Hotkeys.ToggleVision, migrated.Hotkeys.ToggleVision);
            Assert.Equal(2, migrated.DisabledSessions.Count);
            Assert.Contains("WoW01", migrated.DisabledSessions);
            Assert.Contains("Poe02", migrated.DisabledSessions);
            Assert.Equal(AppSettingsSnapshot.CreateDefault().SchemaVersion, migrated.SchemaVersion);

            var backupPath = Path.Combine(root, "settings.legacy.backup.json");
            Assert.True(File.Exists(backupPath));

            var backup = JsonSerializer.Deserialize<AppSettingsSnapshot>(File.ReadAllText(backupPath));
            Assert.NotNull(backup);
            Assert.Equal(migrated.Filters.NameFilter, backup.Filters.NameFilter);
            Assert.Equal(migrated.Paths.NxsFolder, backup.Paths.NxsFolder);
            Assert.Equal(migrated.Hotkeys.Record.VirtualKey, backup.Hotkeys.Record.VirtualKey);
            Assert.Equal(backup.DisabledSessions.Count, migrated.DisabledSessions.Count);
        }
        finally
        {
            snapshot.Restore(legacy);
        }
    }

    [Fact]
    public async Task SaveAsync_UsesAtomicReplace_AndDoesNotLeaveTempFile()
    {
        var root = GetSettingsRoot();
        RecreateDirectory(root);

        var migrationService = new SettingsMigrationService(NullLogger<SettingsMigrationService>.Instance);
        var service = new JsonSettingsService(NullLogger<JsonSettingsService>.Instance, migrationService);

        var updated = service.Current with
        {
            Layout = service.Current.Layout with
            {
                Gap = service.Current.Layout.Gap + 1,
            },
        };

        service.Update(updated);
        await service.SaveAsync();

        var settingsPath = Path.Combine(root, "settings.json");
        var tempPath = settingsPath + ".tmp";

        Assert.True(File.Exists(settingsPath));
        Assert.False(File.Exists(tempPath));

        var persisted = JsonSerializer.Deserialize<AppSettingsSnapshot>(File.ReadAllText(settingsPath));
        Assert.NotNull(persisted);
        Assert.Equal(updated.Layout.Gap, persisted.Layout.Gap);
    }

    private static string GetSettingsRoot()
    {
        var value = Environment.GetEnvironmentVariable("NXTILER_APPDATA");
        Assert.False(string.IsNullOrWhiteSpace(value));
        return value!;
    }

    private static void RecreateDirectory(string path)
    {
        if (Directory.Exists(path))
        {
            Directory.Delete(path, recursive: true);
        }

        Directory.CreateDirectory(path);
    }

    private sealed record LegacySnapshot(
        string TitleFilter,
        string NameFilter,
        bool SortDesc,
        int Gap,
        int TopPad,
        bool SuspendOnMax,
        int DragCooldownMs,
        string NxsFolder,
        string RecordingFolder,
        int RecordingFps,
        string FfmpegPath,
        double OverlayLeft,
        double OverlayTop,
        bool TrayHintShown,
        int HkOverlayKey,
        uint HkOverlayMod,
        int HkMainKey,
        uint HkMainMod,
        int HkFocusKey,
        uint HkFocusMod,
        int HkMinimizeKey,
        uint HkMinimizeMod,
        int HkPrevKey,
        uint HkPrevMod,
        int HkNextKey,
        uint HkNextMod,
        int HkRecordKey,
        uint HkRecordMod,
        int HkRecPauseKey,
        uint HkRecPauseMod,
        int HkRecStopKey,
        uint HkRecStopMod,
        string[] DisabledFiles)
    {
        public static LegacySnapshot Capture(LegacyAppSettings legacy)
        {
            return new LegacySnapshot(
                legacy.TitleFilter,
                legacy.NameFilter,
                legacy.SortDesc,
                legacy.Gap,
                legacy.TopPad,
                legacy.SuspendOnMax,
                legacy.DragCooldownMs,
                legacy.NxsFolder,
                legacy.RecordingFolder,
                legacy.RecordingFps,
                legacy.FfmpegPath,
                legacy.OverlayLeft,
                legacy.OverlayTop,
                legacy.TrayHintShown,
                legacy.HkOverlayKey,
                legacy.HkOverlayMod,
                legacy.HkMainKey,
                legacy.HkMainMod,
                legacy.HkFocusKey,
                legacy.HkFocusMod,
                legacy.HkMinimizeKey,
                legacy.HkMinimizeMod,
                legacy.HkPrevKey,
                legacy.HkPrevMod,
                legacy.HkNextKey,
                legacy.HkNextMod,
                legacy.HkRecordKey,
                legacy.HkRecordMod,
                legacy.HkRecPauseKey,
                legacy.HkRecPauseMod,
                legacy.HkRecStopKey,
                legacy.HkRecStopMod,
                legacy.DisabledFiles.Cast<string>().ToArray());
        }

        public void Restore(LegacyAppSettings legacy)
        {
            legacy.TitleFilter = TitleFilter;
            legacy.NameFilter = NameFilter;
            legacy.SortDesc = SortDesc;
            legacy.Gap = Gap;
            legacy.TopPad = TopPad;
            legacy.SuspendOnMax = SuspendOnMax;
            legacy.DragCooldownMs = DragCooldownMs;
            legacy.NxsFolder = NxsFolder;
            legacy.RecordingFolder = RecordingFolder;
            legacy.RecordingFps = RecordingFps;
            legacy.FfmpegPath = FfmpegPath;
            legacy.OverlayLeft = OverlayLeft;
            legacy.OverlayTop = OverlayTop;
            legacy.TrayHintShown = TrayHintShown;
            legacy.HkOverlayKey = HkOverlayKey;
            legacy.HkOverlayMod = HkOverlayMod;
            legacy.HkMainKey = HkMainKey;
            legacy.HkMainMod = HkMainMod;
            legacy.HkFocusKey = HkFocusKey;
            legacy.HkFocusMod = HkFocusMod;
            legacy.HkMinimizeKey = HkMinimizeKey;
            legacy.HkMinimizeMod = HkMinimizeMod;
            legacy.HkPrevKey = HkPrevKey;
            legacy.HkPrevMod = HkPrevMod;
            legacy.HkNextKey = HkNextKey;
            legacy.HkNextMod = HkNextMod;
            legacy.HkRecordKey = HkRecordKey;
            legacy.HkRecordMod = HkRecordMod;
            legacy.HkRecPauseKey = HkRecPauseKey;
            legacy.HkRecPauseMod = HkRecPauseMod;
            legacy.HkRecStopKey = HkRecStopKey;
            legacy.HkRecStopMod = HkRecStopMod;

            legacy.DisabledFiles.Clear();
            foreach (var value in DisabledFiles)
            {
                legacy.DisabledFiles.Add(value);
            }
        }
    }
}
