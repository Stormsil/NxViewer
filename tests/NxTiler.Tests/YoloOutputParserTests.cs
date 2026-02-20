using Microsoft.ML.OnnxRuntime.Tensors;
using NxTiler.Domain.Windowing;
using NxTiler.Infrastructure.Vision;

namespace NxTiler.Tests;

public sealed class YoloOutputParserTests
{
    [Fact]
    public void Parse_ParsesChannelFirstTensor()
    {
        var output = new DenseTensor<float>(new[] { 1, 6, 10 });
        output[0, 0, 0] = 320f;
        output[0, 1, 0] = 320f;
        output[0, 2, 0] = 320f;
        output[0, 3, 0] = 320f;
        output[0, 4, 0] = 0.2f;
        output[0, 5, 0] = 0.9f;

        var parser = new YoloOutputParser();
        var preprocess = new YoloPreprocessResult(new DenseTensor<float>(new[] { 1, 3, 1, 1 }), 1f, 0f, 0f);
        var windowBounds = new WindowBounds(X: 100, Y: 200, Width: 640, Height: 640);

        var detections = parser.Parse(output, minConfidence: 0.5f, preprocess, 640, 640, windowBounds);

        var detection = Assert.Single(detections);
        Assert.Equal(1, detection.ClassId);
        Assert.Equal(0.9f, detection.Confidence, precision: 3);
        Assert.Equal(260, detection.Bounds.X);
        Assert.Equal(360, detection.Bounds.Y);
        Assert.Equal(320, detection.Bounds.Width);
        Assert.Equal(320, detection.Bounds.Height);
    }

    [Fact]
    public void Parse_ParsesChannelLastTensor()
    {
        var output = new DenseTensor<float>(new[] { 1, 10, 6 });
        output[0, 0, 0] = 320f;
        output[0, 0, 1] = 320f;
        output[0, 0, 2] = 320f;
        output[0, 0, 3] = 320f;
        output[0, 0, 4] = 0.2f;
        output[0, 0, 5] = 0.9f;

        var parser = new YoloOutputParser();
        var preprocess = new YoloPreprocessResult(new DenseTensor<float>(new[] { 1, 3, 1, 1 }), 1f, 0f, 0f);
        var windowBounds = new WindowBounds(X: 0, Y: 0, Width: 640, Height: 640);

        var detections = parser.Parse(output, minConfidence: 0.5f, preprocess, 640, 640, windowBounds);

        var detection = Assert.Single(detections);
        Assert.Equal(1, detection.ClassId);
        Assert.Equal(0.9f, detection.Confidence, precision: 3);
        Assert.Equal(160, detection.Bounds.X);
        Assert.Equal(160, detection.Bounds.Y);
        Assert.Equal(320, detection.Bounds.Width);
        Assert.Equal(320, detection.Bounds.Height);
    }
}
