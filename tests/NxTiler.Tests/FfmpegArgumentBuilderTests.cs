using NxTiler.Infrastructure.Recording;

namespace NxTiler.Tests;

public sealed class FfmpegArgumentBuilderTests
{
    [Fact]
    public void BuildGdiGrabSegmentArguments_ProducesExpectedCommand()
    {
        var args = FfmpegArgumentBuilder.BuildGdiGrabSegmentArguments(
            fps: 30,
            offsetX: 10,
            offsetY: 20,
            width: 1280,
            height: 720,
            segmentFile: @"C:\tmp\seg001.mp4");

        Assert.Equal(
            "-hide_banner -loglevel error -y -f gdigrab -framerate 30 -offset_x 10 -offset_y 20 " +
            "-video_size 1280x720 -i desktop " +
            "-c:v libx264 -preset ultrafast -pix_fmt yuv420p -an \"C:\\tmp\\seg001.mp4\"",
            args);
    }

    [Fact]
    public void BuildConcatArguments_ProducesExpectedCommand()
    {
        var args = FfmpegArgumentBuilder.BuildConcatArguments(
            listFile: @"C:\tmp\segments.txt",
            outputFile: @"C:\tmp\final.mp4");

        Assert.Equal("-hide_banner -loglevel error -y -f concat -safe 0 -i \"C:\\tmp\\segments.txt\" -c copy \"C:\\tmp\\final.mp4\"", args);
    }

    [Fact]
    public void BuildMaskingArguments_ProducesExpectedCommand()
    {
        var args = FfmpegArgumentBuilder.BuildMaskingArguments(
            inputPath: @"C:\tmp\input.mp4",
            tempMaskedPath: @"C:\tmp\masked_tmp.mp4",
            filterValue: "drawbox=x=1:y=2:w=3:h=4:color=black@1:t=fill");

        Assert.Equal(
            "-hide_banner -loglevel error -y -i \"C:\\tmp\\input.mp4\" " +
            "-vf \"drawbox=x=1:y=2:w=3:h=4:color=black@1:t=fill\" -c:v libx264 -preset veryfast -pix_fmt yuv420p -an \"C:\\tmp\\masked_tmp.mp4\"",
            args);
    }

    [Fact]
    public void BuildImagePipeArguments_ProducesExpectedCommand()
    {
        var args = FfmpegArgumentBuilder.BuildImagePipeArguments(
            fps: 60,
            rawOutputPath: @"C:\tmp\raw.mp4");

        Assert.Equal(
            "-hide_banner -loglevel error -y -f image2pipe -framerate 60 -vcodec bmp -i - " +
            "-c:v libx264 -preset ultrafast -pix_fmt yuv420p -an \"C:\\tmp\\raw.mp4\"",
            args);
    }
}
