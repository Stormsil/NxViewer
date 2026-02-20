using System.Drawing;
using Microsoft.ML.OnnxRuntime.Tensors;

namespace NxTiler.Infrastructure.Vision;

internal interface IYoloPreprocessor
{
    YoloPreprocessResult Preprocess(Bitmap source, int inputSize);
}

internal readonly record struct YoloPreprocessResult(
    DenseTensor<float> Tensor,
    float Scale,
    float PadX,
    float PadY);
