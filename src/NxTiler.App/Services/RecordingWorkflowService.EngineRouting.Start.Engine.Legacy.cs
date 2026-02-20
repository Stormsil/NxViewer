using NxTiler.Domain.Windowing;

namespace NxTiler.App.Services;

public sealed partial class RecordingWorkflowService
{
    private bool TryStartLegacyEngine(WindowBounds bounds, int fps, string recordingFolder, string ffmpegPath)
    {
        return _recordingEngine.Start(
            bounds.X,
            bounds.Y,
            bounds.Width,
            bounds.Height,
            fps,
            recordingFolder,
            ffmpegPath);
    }

    private async Task ShowEngineStartFailureAsync(WindowBounds bounds, CancellationToken token)
    {
        var message = _recordingEngine.LastError ?? "Unable to start recording.";
        if (_recordingMonitorBounds is not null)
        {
            await _recordingOverlayService.ShowMaskEditingAsync(_recordingMonitorBounds, bounds, token);
        }

        RaiseMessage(message);
        await _recordingOverlayService.ShowStatusAsync(message, token);
    }
}
