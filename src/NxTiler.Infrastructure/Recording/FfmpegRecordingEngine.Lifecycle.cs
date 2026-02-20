using Microsoft.Extensions.Logging;

namespace NxTiler.Infrastructure.Recording;

public sealed partial class FfmpegRecordingEngine
{
    public async Task StopCurrentSegmentAsync(CancellationToken ct = default)
    {
        if (_ffmpeg is null)
        {
            return;
        }

        if (_ffmpeg.HasExited)
        {
            _ffmpeg = null;
            return;
        }

        try
        {
            await _ffmpeg.StandardInput.WriteAsync('q');
            await _ffmpeg.StandardInput.FlushAsync();

            var exited = await ProcessExitAwaiter.WaitForExitAsync(_ffmpeg, 5000, ct);
            if (!exited)
            {
                _ffmpeg.Kill();
            }
        }
        catch (Exception ex)
        {
            logger.LogDebug(ex, "Graceful ffmpeg stop failed, forcing process termination.");
            try
            {
                _ffmpeg.Kill();
            }
            catch (Exception killEx)
            {
                logger.LogDebug(killEx, "Failed to kill ffmpeg process after graceful stop failure.");
            }
        }

        _ffmpeg = null;
    }

    public void Abort()
    {
        if (_ffmpeg is not null && !_ffmpeg.HasExited)
        {
            try
            {
                _ffmpeg.Kill();
            }
            catch (Exception ex)
            {
                logger.LogDebug(ex, "Failed to kill ffmpeg process while aborting recording.");
            }
        }

        _ffmpeg = null;

        foreach (var segment in _segments)
        {
            TryDelete(segment);
        }

        _segments.Clear();
    }

    private void TryDelete(string path)
    {
        try
        {
            if (File.Exists(path))
            {
                File.Delete(path);
            }
        }
        catch (Exception ex)
        {
            logger.LogDebug(ex, "Failed to delete temporary recording file {Path}", path);
        }
    }
}
