using NxTiler.Domain.Capture;

namespace NxTiler.Application.Abstractions;

public interface ICaptureWorkflowService
{
    Task<CaptureResult> RunInstantSnapshotAsync(nint targetWindow, CancellationToken ct = default);

    Task<CaptureResult> RunRegionSnapshotAsync(nint targetWindow, IReadOnlyList<CaptureMask> masks, CancellationToken ct = default);
}
