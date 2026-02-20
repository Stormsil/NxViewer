namespace NxTiler.Domain.Settings;

public sealed record UiSettings(
    bool TrayHintShown,
    bool AutoArrangeEnabled,
    int SnapshotDebounceMs = 120,
    bool StartMinimizedToTray = false);
