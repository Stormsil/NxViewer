using NxTiler.Domain.Recording;

namespace NxTiler.App.Services;

public interface IDashboardRecordingCommandService
{
    Task StartMaskEditingAsync(nint targetWindow, CancellationToken ct = default);

    Task StartRecordingAsync(CancellationToken ct = default);

    Task PauseAsync(CancellationToken ct = default);

    Task ResumeAsync(CancellationToken ct = default);

    Task<RecordingResult> StopAsync(bool save, CancellationToken ct = default);

    Task CancelAsync(CancellationToken ct = default);
}
