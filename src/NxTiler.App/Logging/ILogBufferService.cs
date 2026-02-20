namespace NxTiler.App.Logging;

public interface ILogBufferService
{
    event EventHandler<LogEntry>? EntryAdded;

    IReadOnlyList<LogEntry> GetSnapshot();

    void Clear();
}
