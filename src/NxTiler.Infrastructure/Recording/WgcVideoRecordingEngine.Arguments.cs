namespace NxTiler.Infrastructure.Recording;

public sealed partial class WgcVideoRecordingEngine
{
    private string BuildRawPipeArguments(string rawOutputPath)
    {
        return FfmpegArgumentBuilder.BuildImagePipeArguments(_fps, rawOutputPath);
    }

    private static string BuildMaskingArguments(string inputPath, string tempMaskedPath, string filterValue)
    {
        return FfmpegArgumentBuilder.BuildMaskingArguments(inputPath, tempMaskedPath, filterValue);
    }
}
