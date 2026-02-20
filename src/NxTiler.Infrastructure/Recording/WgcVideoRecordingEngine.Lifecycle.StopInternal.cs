using Microsoft.Extensions.Logging;

namespace NxTiler.Infrastructure.Recording;

public sealed partial class WgcVideoRecordingEngine
{
    private async Task StopInternalAsync(bool forceKill, CancellationToken ct)
    {
        _framePumpCts?.Cancel();

        if (_framePumpTask is not null)
        {
            try
            {
                await _framePumpTask.WaitAsync(TimeSpan.FromSeconds(5), ct);
            }
            catch (Exception ex) when (ex is TimeoutException or OperationCanceledException)
            {
                logger.LogDebug(ex, "WGC frame pump stop timeout/cancellation.");
            }
            catch (Exception ex)
            {
                logger.LogDebug(ex, "WGC frame pump stop failed.");
            }
        }

        _pauseGate.Set();

        if (_ffmpeg is not null)
        {
            try
            {
                if (!_ffmpeg.HasExited)
                {
                    if (forceKill)
                    {
                        _ffmpeg.Kill();
                    }
                    else
                    {
                        await _ffmpeg.StandardInput.FlushAsync(ct);
                        _ffmpeg.StandardInput.Close();
                        if (!await ProcessExitAwaiter.WaitForExitAsync(_ffmpeg, 10000, ct))
                        {
                            _ffmpeg.Kill();
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                logger.LogDebug(ex, "Failed to stop ffmpeg process cleanly.");
                try
                {
                    if (_ffmpeg is { HasExited: false })
                    {
                        _ffmpeg.Kill();
                    }
                }
                catch (Exception killEx)
                {
                    logger.LogDebug(killEx, "Failed to kill ffmpeg process after stop failure.");
                }
            }
            finally
            {
                _ffmpeg.Dispose();
                _ffmpeg = null;
            }
        }
    }
}
