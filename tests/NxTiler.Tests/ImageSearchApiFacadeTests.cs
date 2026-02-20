using System.Drawing;
using System.Drawing.Imaging;
using ImageSearchCL.API;

namespace NxTiler.Tests;

public sealed class ImageSearchApiFacadeTests
{
    [Fact]
    public void Search_ForAny_WithEmptyPaths_Throws()
    {
        var error = Assert.Throws<ArgumentException>(() => Search.ForAny(Array.Empty<string>()));
        Assert.Contains("At least one image path is required", error.Message, StringComparison.OrdinalIgnoreCase);
    }

    [Fact]
    public void Search_ForAny_WithBlankPath_Throws()
    {
        var error = Assert.Throws<ArgumentException>(() => Search.ForAny("a.png", " "));
        Assert.Contains("null or empty", error.Message, StringComparison.OrdinalIgnoreCase);
    }

    [Fact]
    public void Search_ForAny_WithReferenceImages_AllowsFluentConfig()
    {
        using var imageA = CreateReferenceImage();
        using var imageB = CreateReferenceImage();
        using var builder = Search.ForAny(imageA, imageB)
            .WithConfidence(0.9)
            .WithMovementThreshold(3.0);

        Assert.NotNull(builder);
    }

    [Fact]
    public void SearchBuilder_In_WithNullCapture_Throws()
    {
        using var image = CreateReferenceImage();
        using var builder = Search.For(image);

        Assert.Throws<ArgumentNullException>(() => builder.In(null!));
    }

    [Fact]
    public void ImageSearchConfiguration_Reset_RestoresDefaults()
    {
        ImageSearchConfiguration.DefaultConfidence = 0.91;
        ImageSearchConfiguration.EnableDebugOverlay = true;
        ImageSearchConfiguration.DefaultMovementThreshold = 12.0;

        ImageSearchConfiguration.Reset();

        Assert.Equal(0.8, ImageSearchConfiguration.DefaultConfidence, 4);
        Assert.False(ImageSearchConfiguration.EnableDebugOverlay);
        Assert.Equal(5.0, ImageSearchConfiguration.DefaultMovementThreshold, 4);
    }

    [Fact]
    public void Images_AddContainsRemove_AreCaseInsensitive()
    {
        using var images = new Images();
        using var image = CreateReferenceImage();
        images.Add("LoGo", image);

        Assert.True(images.Contains("logo"));
        Assert.Same(image, images["LOGO"]);
        Assert.True(images.Remove("logo"));
        Assert.False(images.Contains("logo"));
    }

    private static ReferenceImage CreateReferenceImage()
    {
        var bitmap = new Bitmap(3, 3, PixelFormat.Format24bppRgb);
        return new ReferenceImage(bitmap);
    }
}
