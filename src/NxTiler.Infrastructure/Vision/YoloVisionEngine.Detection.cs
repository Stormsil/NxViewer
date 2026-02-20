using System.Drawing;
using Microsoft.Extensions.Logging;
using Microsoft.ML.OnnxRuntime;
using NxTiler.Domain.Vision;
using WindowCaptureCL;

namespace NxTiler.Infrastructure.Vision;

public sealed partial class YoloVisionEngine
{
    public async Task<IReadOnlyList<VisionDetection>> DetectAsync(VisionRequest request, CancellationToken ct = default)
    {
        ct.ThrowIfCancellationRequested();

        if (request.TargetWindow == nint.Zero)
        {
            return Array.Empty<VisionDetection>();
        }

        var modelPath = ResolveModelPath();
        if (string.IsNullOrWhiteSpace(modelPath))
        {
            throw new InvalidOperationException(
                "YOLO model path is not configured. Set Vision.YoloModelPath or NXTILER_YOLO_MODEL.");
        }

        if (!File.Exists(modelPath))
        {
            throw new FileNotFoundException("YOLO model file was not found.", modelPath);
        }

        if (!CaptureFacade.IsCaptureSupported())
        {
            throw new PlatformNotSupportedException("Windows Graphics Capture is not supported on this system.");
        }

        var bounds = await _windowControlService.GetWindowBoundsAsync(request.TargetWindow, ct);
        if (bounds.Width <= 0 || bounds.Height <= 0)
        {
            return Array.Empty<VisionDetection>();
        }

        var sessionContext = _sessionProvider.GetOrCreate(modelPath);
        var session = sessionContext.Session;
        var labels = sessionContext.Labels;

        using var frame = await Task.Run(
            () => CaptureFacade.CaptureWindow((IntPtr)request.TargetWindow, includeCursor: false, drawBorder: false),
            ct);

        var preprocess = _preprocessor.Preprocess(frame, DefaultInputSize);
        var inputName = session.InputMetadata.Keys.First();
        var input = NamedOnnxValue.CreateFromTensor(inputName, preprocess.Tensor);
        using var results = session.Run([input]);

        var output = results.FirstOrDefault()
            ?? throw new InvalidOperationException("YOLO model returned no outputs.");
        var tensor = output.AsTensor<float>();

        var candidates = _outputParser.Parse(
            tensor,
            request.MinConfidence,
            preprocess,
            frame.Width,
            frame.Height,
            bounds);

        var selected = YoloDetectionPostProcessor.NonMaximumSuppression(candidates, NmsIouThreshold);
        var detections = BuildDetections(selected, labels);
        _logger.LogDebug("YOLO scan complete for {TargetWindow}. Detections={Count}.", request.TargetWindow, detections.Count);
        return detections;
    }

    private static List<VisionDetection> BuildDetections(IReadOnlyList<YoloCandidate> selected, IReadOnlyList<string> labels)
    {
        var timestamp = DateTime.UtcNow;
        var detections = new List<VisionDetection>(selected.Count);
        foreach (var candidate in selected)
        {
            var label = ResolveLabel(candidate.ClassId, labels);
            detections.Add(new VisionDetection(
                Label: label,
                Confidence: candidate.Confidence,
                Bounds: candidate.Bounds,
                TimestampUtc: timestamp));
        }

        return detections;
    }
}
