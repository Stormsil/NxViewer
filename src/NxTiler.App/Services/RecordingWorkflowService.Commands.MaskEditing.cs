using Microsoft.Extensions.Logging;
using NxTiler.Domain.Enums;

namespace NxTiler.App.Services;

public sealed partial class RecordingWorkflowService
{
    public Task StartMaskEditingAsync(nint targetWindow = 0, CancellationToken ct = default)
    {
        return ExecuteSerializedAsync(async token =>
        {
            _logger.LogInformation("Recording: StartMaskEditing requested. State={State} Target={Target}.", State, targetWindow);
            if (!CanExecute(RecordingWorkflowAction.StartMaskEditing, "StartMaskEditing"))
            {
                return;
            }

            nint handle = targetWindow;
            if (handle == nint.Zero)
            {
                var target = await ResolveRecordingTargetAsync(token);
                handle = target?.Handle ?? nint.Zero;
            }

            if (handle == nint.Zero)
            {
                RaiseMessage("No windows available for recording.");
                return;
            }

            _recordingTargetWindow = handle;
            _useVideoEngine = false;
            _recordingBounds = await _windowControlService.GetClientAreaScreenBoundsAsync(handle, token);
            _logger.LogInformation("Recording: Capture bounds resolved: {@Bounds}.", _recordingBounds);
            if (_recordingBounds.Width <= 0 || _recordingBounds.Height <= 0)
            {
                RaiseMessage("Cannot determine target client area.");
                return;
            }

            var monitorBounds = await _windowControlService.GetMonitorBoundsForWindowAsync(handle, token);
            _recordingMonitorBounds = monitorBounds;
            _logger.LogInformation("Recording: Monitor bounds resolved: {@Bounds}.", monitorBounds);
            await _recordingOverlayService.ShowMaskEditingAsync(monitorBounds, _recordingBounds, token);

            ApplyTransition(RecordingWorkflowAction.StartMaskEditing, "Mask editing started. Press Record to start capture.");
        }, ct);
    }
}
