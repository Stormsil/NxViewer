namespace NxTiler.Overlay.State;

public sealed record SettingsPanelState(
    string TitleFilter,
    string NameFilter,
    bool SortDescending,
    int Gap,
    int TopPad,
    int DragCooldownMs,
    bool SuspendOnMax,
    string NxsFolder,
    string RecordingFolder,
    string FfmpegPath,
    int RecordingFps,
    bool EnableTemplateMatchingFallback,
    bool EnableYoloEngine)
{
    public static readonly SettingsPanelState Empty = new(
        string.Empty, string.Empty, false,
        8, 0, 200, false,
        string.Empty, string.Empty, string.Empty, 30,
        false, false);
}
