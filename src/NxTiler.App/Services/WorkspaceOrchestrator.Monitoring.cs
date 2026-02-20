namespace NxTiler.App.Services;

public sealed partial class WorkspaceOrchestrator
{
    private async Task EnsureWindowMonitorStartedAsync(CancellationToken ct = default)
    {
        if (_isWindowMonitorActive)
        {
            return;
        }

        await _windowEventMonitorService.StartAsync(ct);
        _isWindowMonitorActive = true;
    }

    private async Task EnsureWindowMonitorStoppedAsync(CancellationToken ct = default)
    {
        if (!_isWindowMonitorActive)
        {
            return;
        }

        await _windowEventMonitorService.StopAsync(ct);
        _isWindowMonitorActive = false;
        _isForeignAppActive = false;
    }
}
