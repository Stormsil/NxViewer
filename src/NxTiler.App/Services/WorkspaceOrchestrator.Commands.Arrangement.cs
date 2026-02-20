using NxTiler.Domain.Enums;

namespace NxTiler.App.Services;

public sealed partial class WorkspaceOrchestrator
{
    public async Task RefreshAsync(CancellationToken ct = default)
    {
        await _arrangementGate.WaitAsync(ct);
        try
        {
            await RefreshTargetsAsync(ct);
            PublishSnapshot();
        }
        finally
        {
            _arrangementGate.Release();
        }
    }

    public async Task ArrangeNowAsync(CancellationToken ct = default)
    {
        await _arrangementGate.WaitAsync(ct);
        try
        {
            await RefreshTargetsAsync(ct);
            await ArrangeInternalAsync(ct);
        }
        finally
        {
            _arrangementGate.Release();
        }
    }

    public async Task CycleModeAsync(CancellationToken ct = default)
    {
        _mode = (TileMode)(((int)_mode + 1) % 6);
        _lastModeSwitch = DateTime.UtcNow;

        if (_mode != TileMode.Grid && !_isAutoArrangeEnabled)
        {
            await SetAutoArrangeAsync(true, ct);
        }

        await ArrangeNowAsync(ct);
    }

    public async Task SetModeAsync(TileMode mode, CancellationToken ct = default)
    {
        _mode = mode;
        _lastModeSwitch = DateTime.UtcNow;
        await ArrangeNowAsync(ct);
    }

    public async Task SetAutoArrangeAsync(bool enabled, CancellationToken ct = default)
    {
        if (_isAutoArrangeEnabled == enabled)
        {
            _trayService.SetAutoArrangeState(enabled);
            PublishSnapshot();
            return;
        }

        _isAutoArrangeEnabled = enabled;
        if (enabled)
        {
            await EnsureWindowMonitorStartedAsync(ct);
            _autoArrangeTimer.Start();
        }
        else
        {
            _autoArrangeTimer.Stop();
            await EnsureWindowMonitorStoppedAsync(ct);
        }

        var updated = _settingsService.Current with
        {
            Ui = _settingsService.Current.Ui with { AutoArrangeEnabled = enabled },
        };
        _settingsService.Update(updated);
        await _settingsService.SaveAsync(ct);

        _trayService.SetAutoArrangeState(enabled);
        RaiseStatus(enabled ? "Auto-Arrange: ON" : "Auto-Arrange: OFF");
        PublishSnapshot();
    }
}
