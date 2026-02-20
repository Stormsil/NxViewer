using NxTiler.Domain.Enums;

namespace NxTiler.App.Services;

public sealed partial class WorkspaceOrchestrator
{
    public async Task StartAsync(CancellationToken ct = default)
    {
        await _lifecycleGate.WaitAsync(ct);
        try
        {
            ObjectDisposedException.ThrowIf(_isDisposed, this);

            if (_isStarted)
            {
                return;
            }

            _isAutoArrangeEnabled = _settingsService.Current.Ui.AutoArrangeEnabled;
            _mode = TileMode.Grid;

            RegisterMessageHandlers();

            try
            {
                _trayService.Initialize(_isAutoArrangeEnabled);
                await _hotkeyService.RegisterAllAsync(_settingsService.Current.Hotkeys, ct);
                if (_isAutoArrangeEnabled)
                {
                    await EnsureWindowMonitorStartedAsync(ct);
                    _autoArrangeTimer.Start();
                }

                await RefreshAsync(ct);
                RaiseStatus("Ready.");
                _isStarted = true;
            }
            catch
            {
                await CleanupFailedStartAsync();
                throw;
            }
        }
        finally
        {
            _lifecycleGate.Release();
        }
    }
}
