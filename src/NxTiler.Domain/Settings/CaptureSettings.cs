namespace NxTiler.Domain.Settings;

public sealed record CaptureSettings(
    bool PreferWgc,
    bool CopySnapshotToClipboardByDefault,
    string SnapshotFolder
);
