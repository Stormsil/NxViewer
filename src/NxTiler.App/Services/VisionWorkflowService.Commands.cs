using NxTiler.Domain.Vision;

namespace NxTiler.App.Services;

public sealed partial class VisionWorkflowService
{
    public Task<VisionWorkflowResult> ToggleModeAsync(nint targetWindow, CancellationToken ct = default)
    {
        return ExecuteSerializedAsync(async token =>
        {
            if (IsEnabled)
            {
                IsEnabled = false;
                return new VisionWorkflowResult(
                    Success: true,
                    ModeEnabled: false,
                    EngineName: null,
                    Detections: Array.Empty<VisionDetection>(),
                    Message: "Vision mode OFF.");
            }

            IsEnabled = true;
            var scan = await RunScanCoreAsync(targetWindow, token);
            var message = scan.Success
                ? $"Vision mode ON ({scan.EngineName}). Detections: {scan.Detections.Count}."
                : $"Vision mode ON, scan failed: {scan.Message}";

            return scan with
            {
                ModeEnabled = true,
                Message = message,
            };
        }, ct);
    }

    public Task<VisionWorkflowResult> RunScanAsync(nint targetWindow, CancellationToken ct = default)
    {
        return ExecuteSerializedAsync(token => RunScanCoreAsync(targetWindow, token), ct);
    }
}
