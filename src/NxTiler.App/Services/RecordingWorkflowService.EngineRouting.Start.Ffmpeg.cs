using Microsoft.Extensions.Logging;

namespace NxTiler.App.Services;

public sealed partial class RecordingWorkflowService
{
    private async Task<string?> ResolveFfmpegPathAsync(CancellationToken token)
    {
        var ffmpegPath = await _ffmpegSetupService.ResolveAndSaveAsync(token);
        _logger.LogInformation("Recording: ffmpeg resolved to {Path}.", ffmpegPath ?? "<null>");
        if (ffmpegPath is not null)
        {
            return ffmpegPath;
        }

        var message = "ffmpeg not found. Downloading...";
        RaiseMessage(message);
        await _recordingOverlayService.ShowStatusAsync(message, token);

        ffmpegPath = await _ffmpegSetupService.DownloadAsync((progress, status) =>
        {
            RaiseMessage(status);
            _ = progress; // keep signature explicit; progress currently only used by UI text.
            var ignore = _recordingOverlayService.ShowStatusAsync(status, CancellationToken.None);
        }, token);

        _logger.LogInformation("Recording: ffmpeg download result {Path}.", ffmpegPath ?? "<null>");
        if (ffmpegPath is not null)
        {
            return ffmpegPath;
        }

        message = "Failed to install ffmpeg. Configure path in settings or check network access.";
        RaiseMessage(message);
        await _recordingOverlayService.ShowStatusAsync(message, token);
        return null;
    }
}
