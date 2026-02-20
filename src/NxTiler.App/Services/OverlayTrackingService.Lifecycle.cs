using Microsoft.Extensions.Logging;
using NxTiler.Domain.Windowing;

namespace NxTiler.App.Services;

public sealed partial class OverlayTrackingService
{
    private Task? DetachLoopForStopLocked()
    {
        if (_loopCts is not null)
        {
            _loopCts.Cancel();
        }

        var loopTask = _loopTask;

        _loopTask = null;
        _loopCts?.Dispose();
        _loopCts = null;
        _targetWindow = nint.Zero;
        _request = null;
        _baselineWindowBounds = new WindowBounds(0, 0, 0, 0);
        _lastState = null;

        return loopTask;
    }

    private async Task AwaitLoopStopAsync(Task? loopTask)
    {
        if (loopTask is null)
        {
            return;
        }

        try
        {
            await loopTask;
        }
        catch (OperationCanceledException)
        {
        }
        catch (Exception ex)
        {
            logger.LogDebug(ex, "Overlay tracking loop stopped with exception.");
        }
    }
}
