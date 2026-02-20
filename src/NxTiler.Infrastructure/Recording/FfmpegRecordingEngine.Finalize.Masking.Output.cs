using Microsoft.Extensions.Logging;

namespace NxTiler.Infrastructure.Recording;

public sealed partial class FfmpegRecordingEngine
{
    private string TryPromoteMaskedOutput(string tempMaskedPath, string finalOutputPath)
    {
        try
        {
            File.Move(tempMaskedPath, finalOutputPath, overwrite: true);
            return finalOutputPath;
        }
        catch (Exception ex)
        {
            logger.LogWarning(ex, "Failed to move masked file to final output path.");
            return tempMaskedPath;
        }
    }
}
