namespace NxTiler.Domain.Settings;

public sealed partial record AppSettingsSnapshot(
    FiltersSettings Filters,
    LayoutSettings Layout,
    PathsSettings Paths,
    RecordingSettings Recording,
    HotkeysSettings Hotkeys,
    OverlaySettings Overlay,
    UiSettings Ui,
    IReadOnlyCollection<string> DisabledSessions,
    int SchemaVersion)
{
}
