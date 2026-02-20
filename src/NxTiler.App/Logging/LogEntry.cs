namespace NxTiler.App.Logging;

public sealed record LogEntry(
    DateTimeOffset Timestamp,
    LogEntryLevel Level,
    string? Source,
    string Message,
    string? ExceptionText);
