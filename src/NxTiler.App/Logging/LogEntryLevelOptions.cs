namespace NxTiler.App.Logging;

public static class LogEntryLevelOptions
{
    public static IReadOnlyList<LogEntryLevel> All { get; } =
        new[]
        {
            LogEntryLevel.Verbose,
            LogEntryLevel.Debug,
            LogEntryLevel.Information,
            LogEntryLevel.Warning,
            LogEntryLevel.Error,
            LogEntryLevel.Fatal,
        };
}
