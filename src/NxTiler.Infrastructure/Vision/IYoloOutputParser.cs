using Microsoft.ML.OnnxRuntime.Tensors;
using NxTiler.Domain.Windowing;

namespace NxTiler.Infrastructure.Vision;

internal interface IYoloOutputParser
{
    List<YoloCandidate> Parse(
        Tensor<float> output,
        float minConfidence,
        YoloPreprocessResult preprocess,
        int originalWidth,
        int originalHeight,
        WindowBounds windowBounds);
}
