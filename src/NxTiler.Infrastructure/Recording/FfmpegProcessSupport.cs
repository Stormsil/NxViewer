using System.Diagnostics;

namespace NxTiler.Infrastructure.Recording;

internal sealed class ProcessStderrTail(int maxLines = 50)
{
    private readonly object _sync = new();
    private readonly List<string> _lines = [];

    public void Reset()
    {
        lock (_sync)
        {
            _lines.Clear();
        }
    }

    public void Attach(Process process)
    {
        process.ErrorDataReceived += (_, e) =>
        {
            Append(e.Data);
        };
    }

    public void Append(string? line)
    {
        if (string.IsNullOrWhiteSpace(line))
        {
            return;
        }

        lock (_sync)
        {
            _lines.Add(line);
            if (_lines.Count > maxLines)
            {
                _lines.RemoveRange(0, _lines.Count - maxLines);
            }
        }
    }

    public string Snapshot()
    {
        lock (_sync)
        {
            return string.Join(Environment.NewLine, _lines);
        }
    }
}

internal static class ProcessExitAwaiter
{
    public static async Task<bool> WaitForExitAsync(Process process, int timeoutMs, CancellationToken ct)
    {
        if (process.HasExited)
        {
            return true;
        }

        var tcs = new TaskCompletionSource<bool>(TaskCreationOptions.RunContinuationsAsynchronously);
        void Handler(object? _, EventArgs __) => tcs.TrySetResult(true);

        process.EnableRaisingEvents = true;
        process.Exited += Handler;

        try
        {
            if (process.HasExited)
            {
                return true;
            }

            using var cts = CancellationTokenSource.CreateLinkedTokenSource(ct);
            var delayTask = Task.Delay(timeoutMs, cts.Token);
            var completed = await Task.WhenAny(tcs.Task, delayTask);
            if (completed == tcs.Task)
            {
                cts.Cancel();
                return true;
            }

            return false;
        }
        finally
        {
            process.Exited -= Handler;
        }
    }
}
