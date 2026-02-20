namespace NxTiler.Domain.Settings;

public sealed record PathsSettings(
    string NxsFolder,
    string RecordingFolder,
    string FfmpegPath
);
