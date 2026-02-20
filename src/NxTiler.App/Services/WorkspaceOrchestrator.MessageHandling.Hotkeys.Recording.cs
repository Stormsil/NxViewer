using NxTiler.Domain.Enums;

namespace NxTiler.App.Services;

public sealed partial class WorkspaceOrchestrator
{
    private async Task HandleStartOrConfirmRecordingHotkeyAsync()
    {
        if (_recordingWorkflowService.State == RecordingState.Idle)
        {
            await _recordingWorkflowService.StartMaskEditingAsync(_focusedWindow);
            return;
        }

        if (_recordingWorkflowService.State == RecordingState.MaskEditing)
        {
            await _recordingWorkflowService.StartRecordingAsync();
        }
    }

    private async Task HandlePauseOrResumeRecordingHotkeyAsync()
    {
        if (_recordingWorkflowService.State == RecordingState.Recording)
        {
            await _recordingWorkflowService.PauseAsync();
            return;
        }

        if (_recordingWorkflowService.State == RecordingState.Paused)
        {
            await _recordingWorkflowService.ResumeAsync();
        }
    }

    private async Task HandleStopOrCancelRecordingHotkeyAsync()
    {
        if (_recordingWorkflowService.State == RecordingState.MaskEditing)
        {
            await _recordingWorkflowService.CancelAsync();
            return;
        }

        if (_recordingWorkflowService.State is RecordingState.Recording or RecordingState.Paused)
        {
            await _recordingWorkflowService.StopAsync(save: true);
        }
    }
}
