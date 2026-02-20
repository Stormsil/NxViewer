using ImageSearchCL.API;
using Microsoft.Extensions.Logging;
using NxTiler.Domain.Vision;
using NxTiler.Domain.Windowing;
using WindowCaptureCL;

namespace NxTiler.Infrastructure.Vision;

public sealed partial class TemplateVisionEngine
{
    public async Task<IReadOnlyList<VisionDetection>> DetectAsync(VisionRequest request, CancellationToken ct = default)
    {
        if (request.TargetWindow == nint.Zero || !CaptureFacade.IsCaptureSupported())
        {
            return Array.Empty<VisionDetection>();
        }

        var templateFiles = ResolveTemplateFiles();
        if (templateFiles.Length == 0)
        {
            return Array.Empty<VisionDetection>();
        }

        var windowBounds = await windowControlService.GetWindowBoundsAsync(request.TargetWindow, ct);
        if (windowBounds.Width <= 0 || windowBounds.Height <= 0)
        {
            return Array.Empty<VisionDetection>();
        }

        using var frame = await Task.Run(
            () => CaptureFacade.CaptureWindow((IntPtr)request.TargetWindow, includeCursor: false, drawBorder: false),
            ct);

        return CollectTemplateDetections(templateFiles, frame, request.MinConfidence, windowBounds, ct);
    }

    private string[] ResolveTemplateFiles()
    {
        var templateDirectory = ResolveTemplateDirectory();
        var templateFiles = EnumerateTemplateFiles(templateDirectory).Take(MaxTemplateFiles).ToArray();
        if (templateFiles.Length == 0)
        {
            logger.LogDebug("Template vision scan skipped: no templates found in {Directory}.", templateDirectory);
        }

        return templateFiles;
    }

    private IReadOnlyList<VisionDetection> CollectTemplateDetections(
        IReadOnlyList<string> templateFiles,
        System.Drawing.Bitmap frame,
        float minConfidence,
        WindowBounds windowBounds,
        CancellationToken ct)
    {
        var detections = new List<VisionDetection>(templateFiles.Count);
        foreach (var templatePath in templateFiles)
        {
            ct.ThrowIfCancellationRequested();

            try
            {
                var match = ImageSearch.Find(templatePath, frame, minConfidence);
                if (match is null)
                {
                    continue;
                }

                var bounds = new WindowBounds(
                    X: windowBounds.X + match.X,
                    Y: windowBounds.Y + match.Y,
                    Width: match.Width,
                    Height: match.Height);

                detections.Add(new VisionDetection(
                    Label: Path.GetFileNameWithoutExtension(templatePath),
                    Confidence: (float)match.Confidence,
                    Bounds: bounds,
                    TimestampUtc: match.Timestamp));
            }
            catch (Exception ex)
            {
                logger.LogDebug(ex, "Template {Template} failed during scan.", templatePath);
            }
        }

        return detections
            .OrderByDescending(static x => x.Confidence)
            .ToArray();
    }
}
