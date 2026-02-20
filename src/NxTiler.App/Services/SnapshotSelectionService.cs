using NxTiler.Domain.Windowing;

namespace NxTiler.App.Services;

public sealed class SnapshotSelectionService : ISnapshotSelectionService
{
    public Task<SnapshotSelectionResult?> SelectRegionAndMasksAsync(
        WindowBounds monitorBounds, CancellationToken ct = default)
    {
        return Task.FromResult<SnapshotSelectionResult?>(null);
    }
}
