using System.Collections.Generic;

namespace NxTiler.App.Logging;

public sealed class LogBufferService : ILogBufferService
{
    public static LogBufferService Instance { get; } = new();

    private const int MaxEntries = 5000;

    private readonly object _gate = new();
    private readonly Queue<LogEntry> _entries = new();

    private LogBufferService()
    {
    }

    public event EventHandler<LogEntry>? EntryAdded;

    public IReadOnlyList<LogEntry> GetSnapshot()
    {
        lock (_gate)
        {
            return _entries.ToArray();
        }
    }

    public void Clear()
    {
        lock (_gate)
        {
            _entries.Clear();
        }
    }

    internal void Add(LogEntry entry)
    {
        lock (_gate)
        {
            while (_entries.Count >= MaxEntries)
            {
                _entries.Dequeue();
            }

            _entries.Enqueue(entry);
        }

        EntryAdded?.Invoke(this, entry);
    }
}
