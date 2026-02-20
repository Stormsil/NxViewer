using Microsoft.Extensions.Logging;
using NxTiler.Domain.Enums;

namespace NxTiler.App.Services;

public sealed partial class RecordingWorkflowService
{
    private void HandleOverlayConfirmRequested()
    {
        Func<CancellationToken, Task>? action = State switch
        {
            RecordingState.MaskEditing => StartRecordingAsync,
            RecordingState.Paused => ResumeAsync,
            _ => null,
        };

        if (action is null)
        {
            _logger.LogDebug("Recording overlay confirm ignored. State={State}", State);
            return;
        }

        _ = ExecuteOverlayActionAsync(action, "overlay confirm");
    }

    private void HandleOverlayCancelRequested()
    {
        _ = ExecuteOverlayActionAsync(CancelAsync, "overlay cancel");
    }

    private async Task ExecuteOverlayActionAsync(Func<CancellationToken, Task> action, string operationName)
    {
        try
        {
            await action(CancellationToken.None);
        }
        catch (OperationCanceledException)
        {
            _logger.LogDebug("Recording overlay operation cancelled: {OperationName}", operationName);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Recording overlay operation failed: {OperationName}", operationName);
        }
    }
}
