namespace NxTiler.App.Services;

public sealed partial class OverlayTrackingService
{
    public async Task StopAsync(CancellationToken ct = default)
    {
        Task? loopTask;
        await _gate.WaitAsync(ct);
        try
        {
            ct.ThrowIfCancellationRequested();
            loopTask = DetachLoopForStopLocked();
        }
        finally
        {
            _gate.Release();
        }

        await AwaitLoopStopAsync(loopTask);
    }

    public async ValueTask DisposeAsync()
    {
        Task? loopTask;
        await _gate.WaitAsync();
        try
        {
            loopTask = DetachLoopForStopLocked();
        }
        finally
        {
            _gate.Release();
        }

        await AwaitLoopStopAsync(loopTask);
        _gate.Dispose();
    }
}
