using System.Drawing;
using NxTiler.Infrastructure.Vision;

namespace NxTiler.Tests;

public sealed class YoloPreprocessorTests
{
    [Fact]
    public void Preprocess_ComputesExpectedScalePaddingAndTensorShape()
    {
        using var source = new Bitmap(width: 100, height: 50);
        using (var graphics = Graphics.FromImage(source))
        {
            graphics.Clear(Color.White);
        }

        var preprocessor = new YoloPreprocessor();

        var result = preprocessor.Preprocess(source, inputSize: 640);

        Assert.Equal(6.4f, result.Scale, precision: 3);
        Assert.Equal(0f, result.PadX, precision: 3);
        Assert.Equal(160f, result.PadY, precision: 3);

        Assert.Equal(1, result.Tensor.Dimensions[0]);
        Assert.Equal(3, result.Tensor.Dimensions[1]);
        Assert.Equal(640, result.Tensor.Dimensions[2]);
        Assert.Equal(640, result.Tensor.Dimensions[3]);
    }
}
