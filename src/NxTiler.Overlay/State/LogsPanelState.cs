namespace NxTiler.Overlay.State;

public sealed record LogEntryItem(
    DateTimeOffset Timestamp,
    string Level,
    string Message,
    string? ExceptionText);

public sealed record LogsPanelState(
    IReadOnlyList<LogEntryItem> Entries,
    string SearchFilter,
    int MinLevel)
{
    public static readonly LogsPanelState Empty = new(
        Array.Empty<LogEntryItem>(), string.Empty, 0);
}
