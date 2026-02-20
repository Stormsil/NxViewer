using Microsoft.Extensions.Logging;
using NxTiler.Domain.Capture;
using NxTiler.Domain.Recording;
using NxTiler.Domain.Windowing;

namespace NxTiler.App.Services;

public sealed partial class RecordingWorkflowService
{
    private async Task<bool> TryStartPreferredEngineAsync(WindowBounds bounds, int fps, CancellationToken token)
    {
        if (!_useVideoEngine)
        {
            return false;
        }

        try
        {
            await _videoRecordingEngine!.StartAsync(
                _recordingTargetWindow,
                bounds,
                new RecordingProfile(fps),
                token);
            return true;
        }
        catch (Exception ex)
        {
            _logger.LogWarning(ex, "WGC recording engine failed to start.");
            _useVideoEngine = false;
            RaiseMessage($"WGC engine start failed: {ex.Message}. Falling back to legacy engine.");
            return false;
        }
    }
}
