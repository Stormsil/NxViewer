using Microsoft.Extensions.Logging;
using NxTiler.Domain.Windowing;

namespace NxTiler.App.Services;

public sealed partial class RecordingWorkflowService
{
    private async Task<bool> StartRecordingEngineAsync(string ffmpegPath, CancellationToken token)
    {
        if (_recordingBounds is null)
        {
            RaiseMessage("Cannot determine target client area.");
            return false;
        }

        var bounds = _recordingBounds;
        var recording = _settingsService.Current.Recording;
        var paths = _settingsService.Current.Paths;
        _useVideoEngine = _settingsService.Current.FeatureFlags.UseWgcRecordingEngine && _videoRecordingEngine is not null;
        _logger.LogInformation(
            "Recording: Engine start with bounds={@Bounds} fps={Fps} folder={Folder} ffmpeg={Ffmpeg}. UseWgc={UseWgc}",
            _recordingBounds,
            recording.Fps,
            paths.RecordingFolder,
            ffmpegPath,
            _useVideoEngine);
        await _recordingOverlayService.EnterRecordingModeAsync(token);

        var preferVideoEngine = _useVideoEngine;
        var started = await TryStartPreferredEngineAsync(bounds, recording.Fps, token);
        if (!started && !TryStartLegacyEngine(bounds, recording.Fps, paths.RecordingFolder, ffmpegPath))
        {
            await ShowEngineStartFailureAsync(bounds, token);
            return false;
        }

        if (preferVideoEngine && !_useVideoEngine)
        {
            RaiseMessage("Legacy recording engine started as fallback.");
        }

        return true;
    }
}
