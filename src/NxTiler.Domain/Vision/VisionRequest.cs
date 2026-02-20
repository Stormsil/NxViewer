namespace NxTiler.Domain.Vision;

public sealed record VisionRequest(
    nint TargetWindow,
    float MinConfidence = 0.5f,
    bool AllowObscuredWindowCapture = true
);
