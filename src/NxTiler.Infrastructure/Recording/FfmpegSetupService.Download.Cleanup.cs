using Microsoft.Extensions.Logging;

namespace NxTiler.Infrastructure.Recording;

public sealed partial class FfmpegSetupService
{
    private void TryDeleteZip(string zipPath)
    {
        try
        {
            if (File.Exists(zipPath))
            {
                File.Delete(zipPath);
            }
        }
        catch (Exception ex)
        {
            logger.LogDebug(ex, "Failed to cleanup temporary ffmpeg zip.");
        }
    }
}
