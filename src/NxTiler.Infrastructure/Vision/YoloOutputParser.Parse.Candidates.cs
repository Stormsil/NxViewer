using Microsoft.ML.OnnxRuntime.Tensors;
using NxTiler.Domain.Windowing;

namespace NxTiler.Infrastructure.Vision;

internal sealed partial class YoloOutputParser
{
    private List<YoloCandidate> ParseCandidates(
        Tensor<float> output,
        YoloOutputLayout layout,
        float minConfidence,
        YoloPreprocessResult preprocess,
        int originalWidth,
        int originalHeight,
        WindowBounds windowBounds)
    {
        var results = new List<YoloCandidate>(layout.Boxes);
        for (var boxIndex = 0; boxIndex < layout.Boxes; boxIndex++)
        {
            if (!TryBuildCandidate(
                output,
                layout,
                boxIndex,
                minConfidence,
                preprocess,
                originalWidth,
                originalHeight,
                windowBounds,
                out var candidate))
            {
                continue;
            }

            results.Add(candidate);
        }

        return results;
    }

    private bool TryBuildCandidate(
        Tensor<float> output,
        YoloOutputLayout layout,
        int boxIndex,
        float minConfidence,
        YoloPreprocessResult preprocess,
        int originalWidth,
        int originalHeight,
        WindowBounds windowBounds,
        out YoloCandidate candidate)
    {
        float Get(int featureIndex) => ReadFeature(output, layout.ChannelFirst, boxIndex, featureIndex);

        var cx = Get(0);
        var cy = Get(1);
        var w = Get(2);
        var h = Get(3);
        if (w <= 0 || h <= 0)
        {
            candidate = default;
            return false;
        }

        if (!TryResolveCandidateScoreAndClass(Get, layout.Features, minConfidence, out var classId, out var confidence))
        {
            candidate = default;
            return false;
        }

        if (!TryResolveCandidateBounds(cx, cy, w, h, preprocess, originalWidth, originalHeight, windowBounds, out var bounds))
        {
            candidate = default;
            return false;
        }

        candidate = new YoloCandidate(classId, confidence, bounds);
        return true;
    }

    private static float ReadFeature(Tensor<float> output, bool channelFirst, int boxIndex, int featureIndex)
    {
        return channelFirst
            ? output[0, featureIndex, boxIndex]
            : output[0, boxIndex, featureIndex];
    }
}
