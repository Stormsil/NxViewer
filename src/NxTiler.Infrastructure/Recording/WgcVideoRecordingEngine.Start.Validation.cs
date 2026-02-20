using System.Drawing;
using NxTiler.Domain.Capture;
using NxTiler.Domain.Windowing;
using WindowCaptureCL;

namespace NxTiler.Infrastructure.Recording;

public sealed partial class WgcVideoRecordingEngine
{
    private void ValidateStartInputs(nint targetWindow, WindowBounds captureBounds, RecordingProfile profile)
    {
        if (IsRunning)
        {
            throw new InvalidOperationException("Recording is already running.");
        }

        if (targetWindow == nint.Zero)
        {
            throw new InvalidOperationException("Target window handle is required for WGC recording.");
        }

        if (captureBounds.Width <= 0 || captureBounds.Height <= 0)
        {
            throw new InvalidOperationException($"Invalid capture bounds: {captureBounds}.");
        }

        if (profile.FramesPerSecond <= 0)
        {
            throw new InvalidOperationException($"Invalid FPS: {profile.FramesPerSecond}.");
        }
    }

    private Rectangle ResolveAlignedCaptureRectForStart(nint targetWindow, WindowBounds captureBounds, bool includeCursor)
    {
        using var probe = CaptureFacade.CaptureWindow((IntPtr)targetWindow, includeCursor: includeCursor, drawBorder: false);
        if (probe.Width <= 0 || probe.Height <= 0)
        {
            throw new InvalidOperationException("Failed to capture probe frame.");
        }

        var captureRect = ResolveCaptureRect(targetWindow, captureBounds, probe.Width, probe.Height);
        if (captureRect.Width <= 0 || captureRect.Height <= 0)
        {
            throw new InvalidOperationException("Capture rectangle is invalid after alignment.");
        }

        return captureRect;
    }
}
