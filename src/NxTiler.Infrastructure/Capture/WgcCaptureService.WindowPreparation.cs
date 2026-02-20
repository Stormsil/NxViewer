using NxTiler.Domain.Windowing;
using NxTiler.Infrastructure.Native;

namespace NxTiler.Infrastructure.Capture;

public sealed partial class WgcCaptureService
{
    private async Task PrepareWindowForCaptureAsync(nint targetWindow, CancellationToken ct)
    {
        // Ensure the target window is maximized before capture.
        Win32Native.ShowWindow(targetWindow, Win32Native.SwShowMaximized);
        Win32Native.SetForegroundWindow(targetWindow);

        await Task.Delay(220, ct);
    }

    private async Task<WindowBounds> WaitForStableWindowBoundsAsync(nint targetWindow, CancellationToken ct)
    {
        var start = DateTime.UtcNow;
        WindowBounds? previous = null;
        var stableCount = 0;

        while (DateTime.UtcNow - start < StabilizeTimeout)
        {
            ct.ThrowIfCancellationRequested();
            var current = await windowControlService.GetWindowBoundsAsync(targetWindow, ct);

            if (current.Width > 0 && current.Height > 0 && current == previous)
            {
                stableCount++;
                if (stableCount >= StabilizeSamplesRequired)
                {
                    return current;
                }
            }
            else
            {
                stableCount = 0;
            }

            previous = current;
            await Task.Delay(StabilizeDelayMs, ct);
        }

        return previous ?? await windowControlService.GetWindowBoundsAsync(targetWindow, ct);
    }
}
