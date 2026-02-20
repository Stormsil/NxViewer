using System.Drawing;
using Microsoft.ML.OnnxRuntime;
using NxTiler.Domain.Windowing;
using NxTiler.Infrastructure.Vision;

namespace NxTiler.Tests;

public sealed class YoloModelSmokeTests
{
    private const string SmokeModelEnv = "NXTILER_YOLO_MODEL_SMOKE";
    private const string SmokeImagesEnv = "NXTILER_YOLO_SMOKE_IMAGES";
    private const int InputSize = 640;

    [Fact]
    public void Smoke_ModelInferenceAndParsing_WhenConfigured()
    {
        var modelPath = ResolveModelPath();
        if (string.IsNullOrWhiteSpace(modelPath) || !File.Exists(modelPath))
        {
            return;
        }

        using var sessionProvider = new YoloSessionProvider();
        var sessionContext = sessionProvider.GetOrCreate(modelPath);
        var preprocessor = new YoloPreprocessor();
        var parser = new YoloOutputParser();
        var inputName = sessionContext.Session.InputMetadata.Keys.First();

        var imageCandidates = ResolveImageInputs().ToList();
        if (imageCandidates.Count == 0)
        {
            using var synthetic = CreateSyntheticImage(InputSize, InputSize);
            RunSmokeForImage(synthetic, sessionContext.Session, inputName, preprocessor, parser);
            return;
        }

        foreach (var imagePath in imageCandidates)
        {
            using var image = new Bitmap(imagePath);
            RunSmokeForImage(image, sessionContext.Session, inputName, preprocessor, parser);
        }
    }

    private static void RunSmokeForImage(
        Bitmap image,
        InferenceSession session,
        string inputName,
        YoloPreprocessor preprocessor,
        YoloOutputParser parser)
    {
        var preprocess = preprocessor.Preprocess(image, InputSize);
        var input = NamedOnnxValue.CreateFromTensor(inputName, preprocess.Tensor);
        using var results = session.Run([input]);
        var output = results.FirstOrDefault()
            ?? throw new InvalidOperationException("YOLO smoke run returned no outputs.");
        var tensor = output.AsTensor<float>();

        var candidates = parser.Parse(
            tensor,
            minConfidence: 0.25f,
            preprocess,
            originalWidth: image.Width,
            originalHeight: image.Height,
            windowBounds: new WindowBounds(0, 0, image.Width, image.Height));

        Assert.NotNull(candidates);
    }

    private static string? ResolveModelPath()
    {
        return Environment.GetEnvironmentVariable(SmokeModelEnv)
               ?? Environment.GetEnvironmentVariable("NXTILER_YOLO_MODEL");
    }

    private static IEnumerable<string> ResolveImageInputs()
    {
        var raw = Environment.GetEnvironmentVariable(SmokeImagesEnv);
        if (string.IsNullOrWhiteSpace(raw))
        {
            yield break;
        }

        if (Directory.Exists(raw))
        {
            foreach (var path in Directory.EnumerateFiles(raw, "*.*", SearchOption.TopDirectoryOnly)
                         .Where(static path =>
                             path.EndsWith(".png", StringComparison.OrdinalIgnoreCase)
                             || path.EndsWith(".jpg", StringComparison.OrdinalIgnoreCase)
                             || path.EndsWith(".jpeg", StringComparison.OrdinalIgnoreCase)
                             || path.EndsWith(".bmp", StringComparison.OrdinalIgnoreCase)))
            {
                yield return path;
            }

            yield break;
        }

        foreach (var candidate in raw.Split(';', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries))
        {
            if (File.Exists(candidate))
            {
                yield return candidate;
            }
        }
    }

    private static Bitmap CreateSyntheticImage(int width, int height)
    {
        var image = new Bitmap(width, height);
        using var graphics = Graphics.FromImage(image);
        graphics.Clear(Color.Black);
        graphics.FillRectangle(Brushes.White, width / 4, height / 4, width / 2, height / 2);
        return image;
    }
}
