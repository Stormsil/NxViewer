using Microsoft.Extensions.Logging;
using NxTiler.Domain.Capture;
using NxTiler.Domain.Windowing;

namespace NxTiler.Infrastructure.Recording;

public sealed partial class WgcVideoRecordingEngine
{
    public async Task StartAsync(nint targetWindow, WindowBounds captureBounds, RecordingProfile profile, CancellationToken ct = default)
    {
        await _gate.WaitAsync(ct);
        try
        {
            ValidateStartInputs(targetWindow, captureBounds, profile);
            var captureRect = ResolveAlignedCaptureRectForStart(targetWindow, captureBounds, profile.IncludeCursor);
            var rawOutputPath = InitializeStartSession(targetWindow, profile, captureRect);

            StartFfmpegProcess(rawOutputPath);
            StartFramePump(ct);

            IsRunning = true;
            logger.LogInformation(
                "WGC recording started. Window={Window} Rect={Rect} FPS={Fps} Output={Output}",
                targetWindow,
                _captureRect,
                _fps,
                _rawOutputPath);
        }
        finally
        {
            _gate.Release();
        }
    }
}
