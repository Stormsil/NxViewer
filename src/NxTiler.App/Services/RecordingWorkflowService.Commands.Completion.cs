using Microsoft.Extensions.Logging;
using NxTiler.Domain.Enums;
using NxTiler.Domain.Recording;

namespace NxTiler.App.Services;

public sealed partial class RecordingWorkflowService
{
    public Task<RecordingResult> StopAsync(bool save, CancellationToken ct = default)
    {
        return ExecuteSerializedAsync(async token =>
        {
            var stopAction = save ? RecordingWorkflowAction.StopSaveBegin : RecordingWorkflowAction.StopDiscard;
            if (!CanExecute(stopAction, "Stop"))
            {
                return new RecordingResult(false, null, "Recording is not active.");
            }

            if (!save)
            {
                await AbortActiveEngineAsync(token);

                await _recordingOverlayService.CloseAsync(token);
                Reset();
                ApplyTransition(RecordingWorkflowAction.StopDiscard, "Recording discarded.");
                return new RecordingResult(false, null, "Recording discarded.");
            }

            ApplyTransition(RecordingWorkflowAction.StopSaveBegin, "Finalizing recording...");
            var masksPx = await _recordingOverlayService.GetMaskRectsPxAsync(token);
            _logger.LogInformation("Recording: masks px relative to capture: {@Masks}", masksPx);
            var outputPath = await FinalizeActiveEngineAsync(masksPx, token);

            await _recordingOverlayService.CloseAsync(token);

            var usedVideoEngine = _useVideoEngine;
            Reset();
            if (string.IsNullOrWhiteSpace(outputPath))
            {
                var errorMessage = usedVideoEngine
                    ? "WGC recording finished, no file created."
                    : _recordingEngine.LastError ?? "Recording finished, no file created.";
                RaiseMessage(errorMessage);
                ApplyTransition(RecordingWorkflowAction.CompleteSaving, "Recording finished, no file created.");
                return new RecordingResult(false, null, errorMessage);
            }

            ApplyTransition(RecordingWorkflowAction.CompleteSaving, $"Saved: {outputPath}");
            return new RecordingResult(true, outputPath, "Completed");
        }, ct);
    }

    public Task CancelAsync(CancellationToken ct = default)
    {
        return ExecuteSerializedAsync(async token =>
        {
            if (!CanExecute(RecordingWorkflowAction.Cancel, "Cancel"))
            {
                return;
            }

            await AbortActiveEngineAsync(token);

            await _recordingOverlayService.CloseAsync(token);
            Reset();
            ApplyTransition(RecordingWorkflowAction.Cancel, "Recording cancelled.");
        }, ct);
    }
}
