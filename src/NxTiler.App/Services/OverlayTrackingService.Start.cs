using NxTiler.Domain.Overlay;
using NxTiler.Domain.Windowing;

namespace NxTiler.App.Services;

public sealed partial class OverlayTrackingService
{
    public async Task StartAsync(nint targetWindow, OverlayTrackingRequest request, CancellationToken ct = default)
    {
        ArgumentNullException.ThrowIfNull(request);

        Task? previousLoopTask;

        await _gate.WaitAsync(ct);
        try
        {
            previousLoopTask = DetachLoopForStopLocked();

            _targetWindow = targetWindow;
            _request = request;
            _baselineWindowBounds = targetWindow == nint.Zero
                ? new WindowBounds(0, 0, 0, 0)
                : await windowControlService.GetWindowBoundsAsync(targetWindow, ct);
            _lastState = null;

            _loopCts = new CancellationTokenSource();
            var token = _loopCts.Token;
            _loopTask = Task.Run(() => TrackingLoopAsync(token), token);
        }
        finally
        {
            _gate.Release();
        }

        await AwaitLoopStopAsync(previousLoopTask);
    }

    public async Task UpdateTargetWindowAsync(nint targetWindow, CancellationToken ct = default)
    {
        await _gate.WaitAsync(ct);
        try
        {
            _targetWindow = targetWindow;
            _baselineWindowBounds = targetWindow == nint.Zero
                ? new WindowBounds(0, 0, 0, 0)
                : await windowControlService.GetWindowBoundsAsync(targetWindow, ct);
            _lastState = null;
        }
        finally
        {
            _gate.Release();
        }
    }
}
