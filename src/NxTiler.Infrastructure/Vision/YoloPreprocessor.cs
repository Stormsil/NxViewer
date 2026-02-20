using System.Drawing;
using System.Drawing.Imaging;
using System.Runtime.InteropServices;
using Microsoft.ML.OnnxRuntime.Tensors;

namespace NxTiler.Infrastructure.Vision;

internal sealed class YoloPreprocessor : IYoloPreprocessor
{
    public YoloPreprocessResult Preprocess(Bitmap source, int inputSize)
    {
        var scale = Math.Min(inputSize / (float)source.Width, inputSize / (float)source.Height);
        var resizedWidth = Math.Max(1, (int)Math.Round(source.Width * scale));
        var resizedHeight = Math.Max(1, (int)Math.Round(source.Height * scale));
        var padX = (inputSize - resizedWidth) / 2f;
        var padY = (inputSize - resizedHeight) / 2f;

        using var canvas = new Bitmap(inputSize, inputSize, PixelFormat.Format24bppRgb);
        using (var graphics = Graphics.FromImage(canvas))
        {
            graphics.Clear(Color.Black);
            graphics.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.HighQualityBilinear;
            graphics.DrawImage(source, padX, padY, resizedWidth, resizedHeight);
        }

        var tensor = new DenseTensor<float>(new[] { 1, 3, inputSize, inputSize });
        var rect = new Rectangle(0, 0, inputSize, inputSize);
        var data = canvas.LockBits(rect, ImageLockMode.ReadOnly, PixelFormat.Format24bppRgb);
        try
        {
            var stride = Math.Abs(data.Stride);
            var buffer = new byte[stride * inputSize];
            Marshal.Copy(data.Scan0, buffer, 0, buffer.Length);

            for (var y = 0; y < inputSize; y++)
            {
                var rowOffset = y * stride;
                for (var x = 0; x < inputSize; x++)
                {
                    var index = rowOffset + (x * 3);
                    var b = buffer[index];
                    var g = buffer[index + 1];
                    var r = buffer[index + 2];

                    tensor[0, 0, y, x] = r / 255f;
                    tensor[0, 1, y, x] = g / 255f;
                    tensor[0, 2, y, x] = b / 255f;
                }
            }
        }
        finally
        {
            canvas.UnlockBits(data);
        }

        return new YoloPreprocessResult(tensor, scale, padX, padY);
    }
}
