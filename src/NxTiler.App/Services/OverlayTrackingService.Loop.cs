using Microsoft.Extensions.Logging;
using NxTiler.Domain.Overlay;

namespace NxTiler.App.Services;

public sealed partial class OverlayTrackingService
{
    private async Task TrackingLoopAsync(CancellationToken ct)
    {
        using var timer = new PeriodicTimer(DefaultPollInterval);
        while (await timer.WaitForNextTickAsync(ct))
        {
            OverlayTrackingState? state = null;
            try
            {
                state = await ComputeStateAsync(ct);
            }
            catch (OperationCanceledException)
            {
                throw;
            }
            catch (Exception ex)
            {
                logger.LogDebug(ex, "Overlay tracking iteration failed.");
            }

            if (state is null)
            {
                continue;
            }

            if (!HasStateChanged(_lastState, state))
            {
                continue;
            }

            _lastState = state;
            TrackingStateChanged?.Invoke(this, state);
        }
    }

    private static bool HasStateChanged(OverlayTrackingState? previous, OverlayTrackingState current)
    {
        if (previous is null)
        {
            return true;
        }

        const double epsilon = 0.5d;
        return previous.IsVisible != current.IsVisible
            || Math.Abs(previous.Left - current.Left) > epsilon
            || Math.Abs(previous.Top - current.Top) > epsilon
            || Math.Abs(previous.Width - current.Width) > epsilon
            || Math.Abs(previous.Height - current.Height) > epsilon;
    }
}
