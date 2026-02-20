using CommunityToolkit.Mvvm.Messaging;

namespace NxTiler.App.Services;

public sealed partial class WorkspaceOrchestrator
{
    public async ValueTask DisposeAsync()
    {
        await _lifecycleGate.WaitAsync();
        try
        {
            if (_isDisposed)
            {
                return;
            }

            _isDisposed = true;
            _isStarted = false;
            _autoArrangeTimer.Stop();
            _autoArrangeTimer.Tick -= AutoArrangeTimerOnTick;
            _messenger.UnregisterAll(this);

            _trayService.Dispose();

            await _hotkeyService.UnregisterAllAsync();
            _hotkeyService.Dispose();

            await EnsureWindowMonitorStoppedAsync();
            _windowEventMonitorService.Dispose();

            _arrangementGate.Dispose();
        }
        finally
        {
            _lifecycleGate.Release();
        }
    }
}
