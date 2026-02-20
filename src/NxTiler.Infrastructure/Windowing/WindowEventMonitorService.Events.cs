using CommunityToolkit.Mvvm.Messaging;
using NxTiler.Application.Messaging;
using NxTiler.Infrastructure.Native;

namespace NxTiler.Infrastructure.Windowing;

public sealed partial class WindowEventMonitorService
{
    private void LocationDebounceOnTick(object? sender, EventArgs e)
    {
        _locationDebounce.Stop();
        if (_locationPending)
        {
            _locationPending = false;
            _messenger.Send(new WindowArrangeNeededMessage());
        }
    }

    private void OnWinEvent(nint hook, uint eventType, nint hwnd, int idObject, int idChild, uint eventThread, uint eventTime)
    {
        if (idObject != 0)
        {
            return;
        }

        Win32Native.GetWindowThreadProcessId(hwnd, out var pid);
        if (pid == (uint)_selfPid)
        {
            return;
        }

        switch (eventType)
        {
            case Win32Native.EventSystemForeground:
                _messenger.Send(new WindowForegroundChangedMessage(hwnd));
                if (_trackedHandles.Contains(hwnd))
                {
                    _messenger.Send(new WindowArrangeNeededMessage());
                }

                break;

            case Win32Native.EventObjectLocationChange:
                if (_trackedHandles.Contains(hwnd))
                {
                    _locationPending = true;
                    _locationDebounce.Stop();
                    _locationDebounce.Start();
                }

                break;

            case Win32Native.EventObjectShow:
                if (_windowTracker is not null)
                {
                    _ = _windowTracker.OnWindowDiscoveredAsync(hwnd);
                }
                else
                {
                    var showTitle = Win32Native.GetWindowTextSafe(hwnd);
                    if (showTitle.Contains("NoMachine", StringComparison.OrdinalIgnoreCase))
                    {
                        _messenger.Send(new WindowArrangeNeededMessage());
                    }
                }

                break;

            case Win32Native.EventObjectNameChange:
                var nameTitle = Win32Native.GetWindowTextSafe(hwnd);
                if (_windowTracker is not null)
                {
                    _windowTracker.OnTitleChanged(hwnd, nameTitle);
                }

                if (_trackedHandles.Contains(hwnd) || nameTitle.Contains("NoMachine", StringComparison.OrdinalIgnoreCase))
                {
                    _messenger.Send(new WindowArrangeNeededMessage());
                }

                break;

            case Win32Native.EventObjectHide:
            case Win32Native.EventObjectDestroy:
                _windowTracker?.OnWindowLost(hwnd);
                if (_trackedHandles.Contains(hwnd))
                {
                    _messenger.Send(new WindowArrangeNeededMessage());
                }

                break;
        }
    }
}
