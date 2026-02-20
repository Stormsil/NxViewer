using Microsoft.ML.OnnxRuntime.Tensors;
using NxTiler.Domain.Windowing;

namespace NxTiler.Infrastructure.Vision;

internal sealed partial class YoloOutputParser
{
    private readonly record struct YoloOutputLayout(bool ChannelFirst, int Features, int Boxes);

    public List<YoloCandidate> Parse(
        Tensor<float> output,
        float minConfidence,
        YoloPreprocessResult preprocess,
        int originalWidth,
        int originalHeight,
        WindowBounds windowBounds)
    {
        if (output.Rank != 3)
        {
            throw new InvalidOperationException($"Unsupported YOLO output rank: {output.Rank}. Expected rank 3.");
        }

        if (!TryResolveOutputLayout(output, out var layout))
        {
            return [];
        }

        return ParseCandidates(
            output,
            layout,
            minConfidence,
            preprocess,
            originalWidth,
            originalHeight,
            windowBounds);
    }

    private static bool TryResolveOutputLayout(Tensor<float> output, out YoloOutputLayout layout)
    {
        var dim1 = output.Dimensions[1];
        var dim2 = output.Dimensions[2];
        var channelFirst = dim1 < dim2;
        var features = channelFirst ? dim1 : dim2;
        var boxes = channelFirst ? dim2 : dim1;
        layout = new YoloOutputLayout(channelFirst, features, boxes);

        return features >= 6 && boxes > 0;
    }
}
