using System.Diagnostics;
using Microsoft.Extensions.Logging;

namespace NxTiler.Infrastructure.Recording;

public sealed partial class FfmpegSetupService
{
    private bool IsInPath()
    {
        try
        {
            var process = Process.Start(new ProcessStartInfo
            {
                FileName = "ffmpeg",
                Arguments = "-version",
                UseShellExecute = false,
                RedirectStandardOutput = true,
                RedirectStandardError = true,
                CreateNoWindow = true,
            });

            if (process is null)
            {
                return false;
            }

            process.WaitForExit(3000);
            return process.HasExited && process.ExitCode == 0;
        }
        catch (Exception ex)
        {
            logger.LogDebug(ex, "ffmpeg is not available in PATH.");
            return false;
        }
    }
}
