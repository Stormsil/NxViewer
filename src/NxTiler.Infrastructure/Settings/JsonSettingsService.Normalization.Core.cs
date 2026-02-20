using NxTiler.Domain.Settings;

namespace NxTiler.Infrastructure.Settings;

public sealed partial class JsonSettingsService
{
    private static AppSettingsSnapshot Normalize(AppSettingsSnapshot snapshot)
    {
        var defaults = AppSettingsSnapshot.CreateDefault();
        var capture = NormalizeCapture(snapshot.Capture ?? defaults.Capture, defaults.Capture);
        var vision = NormalizeVision(snapshot.Vision ?? defaults.Vision, defaults.Vision);
        var overlayPolicies = NormalizeOverlayPolicies(snapshot.OverlayPolicies ?? defaults.OverlayPolicies);
        var featureFlags = NormalizeFeatureFlags(snapshot.FeatureFlags ?? defaults.FeatureFlags);
        var hotkeys = NormalizeHotkeys(snapshot.Hotkeys ?? defaults.Hotkeys, defaults.Hotkeys);
        var filters = NormalizeFilters(snapshot.Filters, defaults.Filters);
        var layout = NormalizeLayout(snapshot.Layout, defaults.Layout);
        var paths = NormalizePaths(snapshot.Paths, defaults.Paths);
        var recording = NormalizeRecording(snapshot.Recording, defaults.Recording);
        var ui = NormalizeUi(snapshot.Ui, defaults.Ui);
        var disabledSessions = NormalizeDisabledSessions(snapshot.DisabledSessions);
        var schemaVersion = NormalizeSchemaVersion(snapshot.SchemaVersion, defaults.SchemaVersion);
        var rules = NormalizeRules(snapshot.Rules ?? defaults.Rules, defaults.Rules);
        var groups = NormalizeGroups(snapshot.Groups ?? defaults.Groups);
        var gridPresets = NormalizeGridPresets(snapshot.GridPresets ?? defaults.GridPresets, defaults.GridPresets);

        return snapshot with
        {
            Filters = filters,
            Layout = layout,
            Paths = paths,
            Recording = recording,
            Hotkeys = hotkeys,
            Ui = ui,
            Capture = capture,
            Vision = vision,
            OverlayPolicies = overlayPolicies with
            {
                VisibilityMode = overlayPolicies.VisibilityMode,
                ScaleWithWindow = overlayPolicies.ScaleWithWindow,
                HideOnHover = overlayPolicies.HideOnHover,
            },
            FeatureFlags = featureFlags,
            DisabledSessions = disabledSessions,
            SchemaVersion = schemaVersion,
            Rules = rules,
            Groups = groups,
            GridPresets = gridPresets,
        };
    }
}
