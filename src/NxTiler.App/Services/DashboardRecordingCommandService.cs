using NxTiler.Application.Abstractions;
using NxTiler.Domain.Recording;

namespace NxTiler.App.Services;

public sealed class DashboardRecordingCommandService(
    IRecordingWorkflowService recordingWorkflowService) : IDashboardRecordingCommandService
{
    public Task StartMaskEditingAsync(nint targetWindow, CancellationToken ct = default) =>
        recordingWorkflowService.StartMaskEditingAsync(targetWindow, ct);

    public Task StartRecordingAsync(CancellationToken ct = default) =>
        recordingWorkflowService.StartRecordingAsync(ct);

    public Task PauseAsync(CancellationToken ct = default) =>
        recordingWorkflowService.PauseAsync(ct);

    public Task ResumeAsync(CancellationToken ct = default) =>
        recordingWorkflowService.ResumeAsync(ct);

    public Task<RecordingResult> StopAsync(bool save, CancellationToken ct = default) =>
        recordingWorkflowService.StopAsync(save, ct);

    public Task CancelAsync(CancellationToken ct = default) =>
        recordingWorkflowService.CancelAsync(ct);
}
