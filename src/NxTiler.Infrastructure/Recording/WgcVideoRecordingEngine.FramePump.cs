using System.Diagnostics;
using System.Drawing;
using System.Drawing.Imaging;
using Microsoft.Extensions.Logging;
using WindowCaptureCL;

namespace NxTiler.Infrastructure.Recording;

public sealed partial class WgcVideoRecordingEngine
{
    private async Task FramePumpLoopAsync(CancellationToken ct)
    {
        var frameInterval = TimeSpan.FromSeconds(1d / _fps);
        var stopwatch = Stopwatch.StartNew();
        var nextFrameAt = stopwatch.Elapsed;

        try
        {
            while (!ct.IsCancellationRequested)
            {
                _pauseGate.Wait(ct);

                using var frame = CaptureFacade.CaptureWindow((IntPtr)_targetWindow, includeCursor: _includeCursor, drawBorder: false);
                using var cropped = CropToCaptureRect(frame, _captureRect);
                await WriteBitmapToFfmpegAsync(cropped, ct);

                nextFrameAt += frameInterval;
                var delay = nextFrameAt - stopwatch.Elapsed;
                if (delay > TimeSpan.Zero)
                {
                    await Task.Delay(delay, ct);
                }
                else
                {
                    nextFrameAt = stopwatch.Elapsed;
                }
            }
        }
        catch (OperationCanceledException)
        {
            // Expected on stop/abort.
        }
        catch (Exception ex)
        {
            logger.LogWarning(ex, "WGC frame pump failed.");
        }
    }

    private async Task WriteBitmapToFfmpegAsync(Bitmap bitmap, CancellationToken ct)
    {
        if (_ffmpeg is null || _ffmpeg.HasExited)
        {
            throw new InvalidOperationException("ffmpeg process is not running.");
        }

        using var ms = new MemoryStream();
        bitmap.Save(ms, ImageFormat.Bmp);
        ms.Position = 0;
        await ms.CopyToAsync(_ffmpeg.StandardInput.BaseStream, ct);
        await _ffmpeg.StandardInput.BaseStream.FlushAsync(ct);
    }
}
