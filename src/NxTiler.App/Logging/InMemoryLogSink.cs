using Serilog.Core;
using Serilog.Events;

namespace NxTiler.App.Logging;

public sealed class InMemoryLogSink(LogBufferService buffer) : ILogEventSink
{
    public void Emit(LogEvent logEvent)
    {
        if (logEvent is null)
        {
            return;
        }

        var source = TryGetSourceContext(logEvent);
        var entry = new LogEntry(
            logEvent.Timestamp,
            ToLevel(logEvent.Level),
            source,
            logEvent.RenderMessage(),
            logEvent.Exception?.ToString());

        buffer.Add(entry);
    }

    private static string? TryGetSourceContext(LogEvent logEvent)
    {
        if (!logEvent.Properties.TryGetValue("SourceContext", out var value))
        {
            return null;
        }

        var text = value.ToString();
        return text.Length >= 2 && text[0] == '"' && text[^1] == '"'
            ? text.Substring(1, text.Length - 2)
            : text;
    }

    private static LogEntryLevel ToLevel(LogEventLevel level)
    {
        return level switch
        {
            LogEventLevel.Verbose => LogEntryLevel.Verbose,
            LogEventLevel.Debug => LogEntryLevel.Debug,
            LogEventLevel.Information => LogEntryLevel.Information,
            LogEventLevel.Warning => LogEntryLevel.Warning,
            LogEventLevel.Error => LogEntryLevel.Error,
            LogEventLevel.Fatal => LogEntryLevel.Fatal,
            _ => LogEntryLevel.Information,
        };
    }
}
