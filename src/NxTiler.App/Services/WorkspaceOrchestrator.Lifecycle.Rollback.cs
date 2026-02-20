using Microsoft.Extensions.Logging;

namespace NxTiler.App.Services;

public sealed partial class WorkspaceOrchestrator
{
    private async Task CleanupFailedStartAsync()
    {
        _isStarted = false;
        _autoArrangeTimer.Stop();
        _messenger.UnregisterAll(this);

        try
        {
            await _hotkeyService.UnregisterAllAsync();
        }
        catch (Exception ex)
        {
            _logger.LogDebug(ex, "Failed to rollback hotkeys after start failure.");
        }

        try
        {
            await EnsureWindowMonitorStoppedAsync();
        }
        catch (Exception ex)
        {
            _logger.LogDebug(ex, "Failed to rollback monitor after start failure.");
        }

        try
        {
            _trayService.Dispose();
        }
        catch (Exception ex)
        {
            _logger.LogDebug(ex, "Failed to rollback tray after start failure.");
        }
    }
}
