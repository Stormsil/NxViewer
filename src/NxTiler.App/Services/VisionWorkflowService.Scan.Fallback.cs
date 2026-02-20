using Microsoft.Extensions.Logging;
using NxTiler.Application.Abstractions;
using NxTiler.Domain.Vision;

namespace NxTiler.App.Services;

public sealed partial class VisionWorkflowService
{
    private async Task<VisionWorkflowResult?> TryRunYoloFallbackAsync(
        IVisionEngine engine,
        nint handle,
        Exception originalException,
        CancellationToken ct)
    {
        if (!engine.Name.Equals("yolo", StringComparison.OrdinalIgnoreCase))
        {
            return null;
        }

        var fallback = ResolveTemplateFallback();
        if (fallback is null)
        {
            return null;
        }

        logger.LogWarning(
            originalException,
            "Vision scan failed in YOLO engine. Falling back to {FallbackEngine}.",
            fallback.Name);

        try
        {
            var request = BuildVisionRequest(handle);
            var fallbackDetections = await fallback.DetectAsync(request, ct);
            return new VisionWorkflowResult(
                Success: true,
                ModeEnabled: IsEnabled,
                EngineName: fallback.Name,
                Detections: fallbackDetections,
                Message: $"Vision scan complete ({fallback.Name} fallback from yolo).");
        }
        catch (Exception fallbackEx) when (fallbackEx is not OperationCanceledException)
        {
            logger.LogWarning(
                fallbackEx,
                "Vision fallback engine {FallbackEngine} also failed.",
                fallback.Name);
            return new VisionWorkflowResult(
                Success: false,
                ModeEnabled: IsEnabled,
                EngineName: fallback.Name,
                Detections: Array.Empty<VisionDetection>(),
                Message: $"{originalException.Message}. Fallback failed: {fallbackEx.Message}");
        }
    }
}
