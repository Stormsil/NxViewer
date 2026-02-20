using Microsoft.ML.OnnxRuntime;

namespace NxTiler.Infrastructure.Vision;

internal interface IYoloSessionProvider
{
    YoloSessionContext GetOrCreate(string modelPath);
}

internal readonly record struct YoloSessionContext(
    InferenceSession Session,
    IReadOnlyList<string> Labels);
