using NxTiler.Domain.Vision;

namespace NxTiler.App.Services;

public sealed partial class VisionWorkflowService
{
    private VisionRequest BuildVisionRequest(nint targetWindow)
    {
        return new VisionRequest(
            TargetWindow: targetWindow,
            MinConfidence: Math.Clamp(settingsService.Current.Vision.ConfidenceThreshold, 0.01f, 1f),
            AllowObscuredWindowCapture: true);
    }
}
