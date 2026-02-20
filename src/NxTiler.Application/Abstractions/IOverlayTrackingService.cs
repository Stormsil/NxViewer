using NxTiler.Domain.Overlay;

namespace NxTiler.Application.Abstractions;

public interface IOverlayTrackingService : IAsyncDisposable
{
    event EventHandler<OverlayTrackingState>? TrackingStateChanged;

    Task StartAsync(nint targetWindow, OverlayTrackingRequest request, CancellationToken ct = default);

    Task UpdateTargetWindowAsync(nint targetWindow, CancellationToken ct = default);

    Task StopAsync(CancellationToken ct = default);
}
