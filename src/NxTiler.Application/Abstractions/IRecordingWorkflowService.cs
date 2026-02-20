using NxTiler.Domain.Enums;
using NxTiler.Domain.Recording;
using NxTiler.Domain.Windowing;

namespace NxTiler.Application.Abstractions;

public interface IRecordingWorkflowService
{
    RecordingState State { get; }
    WindowBounds? ActiveCaptureBounds { get; }

    Task StartMaskEditingAsync(nint targetWindow = 0, CancellationToken ct = default);

    Task StartRecordingAsync(CancellationToken ct = default);

    Task PauseAsync(CancellationToken ct = default);

    Task ResumeAsync(CancellationToken ct = default);

    Task<RecordingResult> StopAsync(bool save, CancellationToken ct = default);

    Task CancelAsync(CancellationToken ct = default);
}
