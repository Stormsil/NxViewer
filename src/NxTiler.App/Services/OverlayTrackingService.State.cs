using NxTiler.Domain.Overlay;
using NxTiler.Domain.Windowing;

namespace NxTiler.App.Services;

public sealed partial class OverlayTrackingService
{
    private async Task<OverlayTrackingState?> ComputeStateAsync(CancellationToken ct)
    {
        OverlayTrackingRequest? requestSnapshot;
        nint targetWindowSnapshot;
        WindowBounds baselineSnapshot;

        await _gate.WaitAsync(ct);
        try
        {
            requestSnapshot = _request;
            targetWindowSnapshot = _targetWindow;
            baselineSnapshot = _baselineWindowBounds;
        }
        finally
        {
            _gate.Release();
        }

        if (requestSnapshot is null || targetWindowSnapshot == nint.Zero)
        {
            return null;
        }

        var targetBounds = await windowControlService.GetWindowBoundsAsync(targetWindowSnapshot, ct);
        if (targetBounds.Width <= 0 || targetBounds.Height <= 0)
        {
            return new OverlayTrackingState(
                IsVisible: false,
                Left: 0,
                Top: 0,
                Width: requestSnapshot.BaseWidth,
                Height: requestSnapshot.BaseHeight);
        }

        var monitorBounds = await windowControlService.GetMonitorBoundsForWindowAsync(targetWindowSnapshot, ct);
        var width = requestSnapshot.BaseWidth;
        var height = requestSnapshot.BaseHeight;

        if (requestSnapshot.ScaleWithWindow && baselineSnapshot.Width > 0 && baselineSnapshot.Height > 0)
        {
            var scaleX = targetBounds.Width / (double)baselineSnapshot.Width;
            var scaleY = targetBounds.Height / (double)baselineSnapshot.Height;
            width = Math.Max(120d, requestSnapshot.BaseWidth * scaleX);
            height = Math.Max(32d, requestSnapshot.BaseHeight * scaleY);
        }

        var (left, top) = ResolveAnchoredPosition(
            targetBounds,
            requestSnapshot.Anchor,
            width,
            height);
        (left, top) = ClampToMonitorBounds(monitorBounds, left, top, width, height);

        var isVisible = ResolveVisibility(requestSnapshot.VisibilityMode, targetBounds, cursorPositionProvider);
        return new OverlayTrackingState(
            IsVisible: isVisible,
            Left: left,
            Top: top,
            Width: width,
            Height: height);
    }
}
