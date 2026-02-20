using NxTiler.Domain.Windowing;

namespace NxTiler.Domain.Capture;

public sealed record CaptureResult(
    bool Success,
    string? FilePath,
    byte[]? ImageBytes,
    WindowBounds CaptureBounds,
    string? ErrorMessage = null
);
