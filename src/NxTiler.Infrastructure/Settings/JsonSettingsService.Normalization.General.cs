using NxTiler.Domain.Settings;

namespace NxTiler.Infrastructure.Settings;

public sealed partial class JsonSettingsService
{
    private static FiltersSettings NormalizeFilters(FiltersSettings source, FiltersSettings defaults)
    {
        return source with
        {
            NameFilter = string.IsNullOrWhiteSpace(source.NameFilter) ? defaults.NameFilter : source.NameFilter,
        };
    }

    private static LayoutSettings NormalizeLayout(LayoutSettings source, LayoutSettings defaults)
    {
        return source with
        {
            DragCooldownMs = source.DragCooldownMs <= 0 ? defaults.DragCooldownMs : source.DragCooldownMs,
        };
    }

    private static PathsSettings NormalizePaths(PathsSettings source, PathsSettings defaults)
    {
        return source with
        {
            NxsFolder = string.IsNullOrWhiteSpace(source.NxsFolder) ? defaults.NxsFolder : source.NxsFolder,
            RecordingFolder = string.IsNullOrWhiteSpace(source.RecordingFolder) ? defaults.RecordingFolder : source.RecordingFolder,
            FfmpegPath = string.IsNullOrWhiteSpace(source.FfmpegPath) ? defaults.FfmpegPath : source.FfmpegPath,
        };
    }

    private static RecordingSettings NormalizeRecording(RecordingSettings source, RecordingSettings defaults)
    {
        return source with
        {
            Fps = source.Fps <= 0 ? defaults.Fps : source.Fps,
        };
    }

    private static UiSettings NormalizeUi(UiSettings source, UiSettings defaults)
    {
        return source with
        {
            SnapshotDebounceMs = Math.Clamp(
                source.SnapshotDebounceMs <= 0 ? defaults.SnapshotDebounceMs : source.SnapshotDebounceMs,
                25,
                500),
            StartMinimizedToTray = source.StartMinimizedToTray,
        };
    }

    private static IReadOnlyCollection<string> NormalizeDisabledSessions(IReadOnlyCollection<string>? source)
    {
        return (source ?? Array.Empty<string>())
            .Where(static x => !string.IsNullOrWhiteSpace(x))
            .Distinct(StringComparer.OrdinalIgnoreCase)
            .ToArray();
    }

    private static int NormalizeSchemaVersion(int source, int defaults)
    {
        return source <= 0 ? defaults : source;
    }
}
