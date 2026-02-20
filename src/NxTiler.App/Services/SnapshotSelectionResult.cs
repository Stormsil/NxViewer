using NxTiler.Domain.Capture;
using NxTiler.Domain.Windowing;

namespace NxTiler.App.Services;

public sealed record SnapshotSelectionResult(
    WindowBounds Region,
    IReadOnlyList<CaptureMask> Masks
);
