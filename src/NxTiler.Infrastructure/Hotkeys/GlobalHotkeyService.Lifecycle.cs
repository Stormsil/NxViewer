using NxTiler.Infrastructure.Native;

namespace NxTiler.Infrastructure.Hotkeys;

public sealed partial class GlobalHotkeyService
{
    public Task UnregisterAllAsync(CancellationToken ct = default)
    {
        ct.ThrowIfCancellationRequested();
        if (_disposed)
        {
            return Task.CompletedTask;
        }

        UnregisterAllCore();
        return Task.CompletedTask;
    }

    public void Dispose()
    {
        if (_disposed)
        {
            return;
        }

        _disposed = true;
        UnregisterAllCore();
        if (_source is not null)
        {
            _source.RemoveHook(WndProc);
            _source.Dispose();
            _source = null;
        }
    }

    private void UnregisterAllCore()
    {
        if (_source is null)
        {
            return;
        }

        foreach (var id in _registeredIds)
        {
            Win32Native.UnregisterHotKey(_source.Handle, id);
        }

        _registeredIds.Clear();
        _idToAction.Clear();
    }
}
