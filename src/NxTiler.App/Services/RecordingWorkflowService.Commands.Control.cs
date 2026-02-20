using Microsoft.Extensions.Logging;
using NxTiler.Domain.Enums;

namespace NxTiler.App.Services;

public sealed partial class RecordingWorkflowService
{
    public Task StartRecordingAsync(CancellationToken ct = default)
    {
        return ExecuteSerializedAsync(async token =>
        {
            _logger.LogInformation("Recording: Start requested. State={State}.", State);
            if (!CanExecute(RecordingWorkflowAction.StartRecording, "StartRecording") || _recordingBounds is null)
            {
                return;
            }

            var ffmpegPath = await ResolveFfmpegPathAsync(token);
            if (ffmpegPath is null)
            {
                return;
            }

            var started = await StartRecordingEngineAsync(ffmpegPath, token);
            if (!started)
            {
                return;
            }

            ApplyTransition(RecordingWorkflowAction.StartRecording, "Recording...");
        }, ct);
    }

    public Task PauseAsync(CancellationToken ct = default)
    {
        return ExecuteSerializedAsync(async token =>
        {
            if (!CanExecute(RecordingWorkflowAction.Pause, "Pause"))
            {
                return;
            }

            if (_useVideoEngine)
            {
                await _videoRecordingEngine!.PauseAsync(token);
            }
            else
            {
                await _recordingEngine.StopCurrentSegmentAsync(token);
            }

            await _recordingOverlayService.EnterPauseEditModeAsync(token);
            ApplyTransition(RecordingWorkflowAction.Pause, "Recording paused.");
        }, ct);
    }

    public Task ResumeAsync(CancellationToken ct = default)
    {
        return ExecuteSerializedAsync(async token =>
        {
            if (!CanExecute(RecordingWorkflowAction.Resume, "Resume"))
            {
                return;
            }

            // Hide edit chrome before starting the next segment to avoid it being captured at the segment boundary.
            await _recordingOverlayService.ReEnterRecordingModeAsync(token);

            if (_useVideoEngine)
            {
                await _videoRecordingEngine!.ResumeAsync(token);
            }
            else if (!_recordingEngine.StartNewSegment())
            {
                await _recordingOverlayService.EnterPauseEditModeAsync(token);
                RaiseMessage(_recordingEngine.LastError ?? "Unable to resume recording.");
                return;
            }

            ApplyTransition(RecordingWorkflowAction.Resume, "Recording resumed.");
        }, ct);
    }
}
