namespace NxTiler.Infrastructure.Recording;

internal static class FfmpegArgumentBuilder
{
    public static string BuildGdiGrabSegmentArguments(
        int fps,
        int offsetX,
        int offsetY,
        int width,
        int height,
        string segmentFile)
    {
        return
            $"-hide_banner -loglevel error -y -f gdigrab -framerate {fps} -offset_x {offsetX} -offset_y {offsetY} " +
            $"-video_size {width}x{height} -i desktop " +
            $"-c:v libx264 -preset ultrafast -pix_fmt yuv420p -an \"{segmentFile}\"";
    }

    public static string BuildConcatArguments(string listFile, string outputFile)
    {
        return $"-hide_banner -loglevel error -y -f concat -safe 0 -i \"{listFile}\" -c copy \"{outputFile}\"";
    }

    public static string BuildMaskingArguments(string inputPath, string tempMaskedPath, string filterValue)
    {
        return
            $"-hide_banner -loglevel error -y -i \"{inputPath}\" " +
            $"-vf \"{filterValue}\" -c:v libx264 -preset veryfast -pix_fmt yuv420p -an \"{tempMaskedPath}\"";
    }

    public static string BuildImagePipeArguments(int fps, string rawOutputPath)
    {
        return
            $"-hide_banner -loglevel error -y -f image2pipe -framerate {fps} -vcodec bmp -i - " +
            $"-c:v libx264 -preset ultrafast -pix_fmt yuv420p -an \"{rawOutputPath}\"";
    }
}
