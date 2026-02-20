namespace NxTiler.Domain.Vision;

public sealed record VisionWorkflowResult(
    bool Success,
    bool ModeEnabled,
    string? EngineName,
    IReadOnlyList<VisionDetection> Detections,
    string Message
);
