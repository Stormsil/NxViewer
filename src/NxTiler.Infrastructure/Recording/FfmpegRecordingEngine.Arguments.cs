namespace NxTiler.Infrastructure.Recording;

public sealed partial class FfmpegRecordingEngine
{
    private string BuildSegmentArguments(string segmentFile)
    {
        return FfmpegArgumentBuilder.BuildGdiGrabSegmentArguments(_fps, _x, _y, _width, _height, segmentFile);
    }

    private static string BuildConcatArguments(string listFile, string outputFile)
    {
        return FfmpegArgumentBuilder.BuildConcatArguments(listFile, outputFile);
    }

    private string BuildMaskingArguments(string inputPath, string tempMaskedPath, string filterValue)
    {
        return FfmpegArgumentBuilder.BuildMaskingArguments(inputPath, tempMaskedPath, filterValue);
    }
}
