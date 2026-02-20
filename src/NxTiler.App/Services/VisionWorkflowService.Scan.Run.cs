using Microsoft.Extensions.Logging;
using NxTiler.Domain.Vision;

namespace NxTiler.App.Services;

public sealed partial class VisionWorkflowService
{
    private async Task<VisionWorkflowResult> RunScanCoreAsync(nint targetWindow, CancellationToken ct)
    {
        var handle = await ResolveTargetWindowAsync(targetWindow, ct);
        if (handle == nint.Zero)
        {
            return new VisionWorkflowResult(
                Success: false,
                ModeEnabled: IsEnabled,
                EngineName: null,
                Detections: Array.Empty<VisionDetection>(),
                Message: "No target window found for vision.");
        }

        var engine = ResolveEngine();
        if (engine is null)
        {
            return new VisionWorkflowResult(
                Success: false,
                ModeEnabled: IsEnabled,
                EngineName: null,
                Detections: Array.Empty<VisionDetection>(),
                Message: "No vision engine is enabled by feature flags.");
        }

        try
        {
            var request = BuildVisionRequest(handle);
            var detections = await engine.DetectAsync(request, ct);
            return new VisionWorkflowResult(
                Success: true,
                ModeEnabled: IsEnabled,
                EngineName: engine.Name,
                Detections: detections,
                Message: $"Vision scan complete ({engine.Name}).");
        }
        catch (OperationCanceledException)
        {
            throw;
        }
        catch (Exception ex)
        {
            var fallbackResult = await TryRunYoloFallbackAsync(engine, handle, ex, ct);
            if (fallbackResult is not null)
            {
                return fallbackResult;
            }

            logger.LogWarning(ex, "Vision scan failed for window {Handle}.", handle);
            return new VisionWorkflowResult(
                Success: false,
                ModeEnabled: IsEnabled,
                EngineName: engine.Name,
                Detections: Array.Empty<VisionDetection>(),
                Message: ex.Message);
        }
    }
}
