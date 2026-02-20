using CommunityToolkit.Mvvm.Messaging;
using NxTiler.App.Messages;
using NxTiler.Domain.Enums;

namespace NxTiler.App.Services;

public sealed partial class WorkspaceOrchestrator
{
    private void WindowEventMonitorArrangeNeeded()
    {
        _ = ExecuteEventActionAsync("window monitor arrange", SmartArrangeAsync);
    }

    private void WindowEventMonitorForegroundChanged(nint foregroundWindow)
    {
        if (!_isAutoArrangeEnabled)
        {
            return;
        }

        var target = _targets.FirstOrDefault(x => x.Handle == foregroundWindow);
        if (target is not null)
        {
            _isForeignAppActive = false;
            if (_mode != TileMode.Grid &&
                (DateTime.UtcNow - _lastModeSwitch).TotalMilliseconds > 1200 &&
                target.Handle != _focusedWindow)
            {
                _focusedWindow = target.Handle;
            }

            return;
        }

        _isForeignAppActive = true;
    }

    private void TrayShowRequested()
    {
        RequestMainWindowToggle();
    }

    private void TrayArrangeRequested()
    {
        _ = ExecuteEventActionAsync("tray arrange", () => ArrangeNowAsync());
    }

    private void TrayAutoArrangeToggled(bool enabled)
    {
        _ = ExecuteEventActionAsync("tray auto-arrange toggle", () => SetAutoArrangeAsync(enabled));
    }

    private void TrayExitRequested()
    {
        _messenger.Send(new ApplicationExitRequestedMessage());
    }

    private void AutoArrangeTimerOnTick(object? sender, EventArgs e)
    {
        _ = ExecuteEventActionAsync("auto-arrange timer", SmartArrangeAsync);
    }
}
