using NxTiler.Infrastructure.Native;

namespace NxTiler.Infrastructure.Windowing;

public sealed partial class WindowEventMonitorService
{
    public Task StartAsync(CancellationToken ct = default)
    {
        ObjectDisposedException.ThrowIf(_disposed, this);
        ct.ThrowIfCancellationRequested();
        if (_hooks.Count > 0)
        {
            return Task.CompletedTask;
        }

        const uint flags = Win32Native.WinEventOutOfContext | Win32Native.WinEventSkipOwnProcess;

        _hooks.Add(Win32Native.SetWinEventHook(Win32Native.EventSystemForeground, Win32Native.EventSystemForeground, nint.Zero, _winEventProc, 0, 0, flags));
        _hooks.Add(Win32Native.SetWinEventHook(Win32Native.EventObjectShow, Win32Native.EventObjectShow, nint.Zero, _winEventProc, 0, 0, flags));
        _hooks.Add(Win32Native.SetWinEventHook(Win32Native.EventObjectHide, Win32Native.EventObjectHide, nint.Zero, _winEventProc, 0, 0, flags));
        _hooks.Add(Win32Native.SetWinEventHook(Win32Native.EventObjectDestroy, Win32Native.EventObjectDestroy, nint.Zero, _winEventProc, 0, 0, flags));
        _hooks.Add(Win32Native.SetWinEventHook(Win32Native.EventObjectNameChange, Win32Native.EventObjectNameChange, nint.Zero, _winEventProc, 0, 0, flags));
        _hooks.Add(Win32Native.SetWinEventHook(Win32Native.EventObjectLocationChange, Win32Native.EventObjectLocationChange, nint.Zero, _winEventProc, 0, 0, flags));

        return Task.CompletedTask;
    }

    public Task StopAsync(CancellationToken ct = default)
    {
        ct.ThrowIfCancellationRequested();
        if (_disposed)
        {
            return Task.CompletedTask;
        }

        StopCore();
        return Task.CompletedTask;
    }

    public void Dispose()
    {
        if (_disposed)
        {
            return;
        }

        _disposed = true;
        _locationDebounce.Tick -= LocationDebounceOnTick;
        StopCore();
    }

    private void StopCore()
    {
        _locationDebounce.Stop();

        foreach (var hook in _hooks)
        {
            if (hook != nint.Zero)
            {
                Win32Native.UnhookWinEvent(hook);
            }
        }

        _hooks.Clear();
    }
}
