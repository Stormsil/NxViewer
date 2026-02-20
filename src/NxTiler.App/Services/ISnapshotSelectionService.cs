using NxTiler.Domain.Windowing;

namespace NxTiler.App.Services;

public interface ISnapshotSelectionService
{
    Task<SnapshotSelectionResult?> SelectRegionAndMasksAsync(WindowBounds monitorBounds, CancellationToken ct = default);
}
