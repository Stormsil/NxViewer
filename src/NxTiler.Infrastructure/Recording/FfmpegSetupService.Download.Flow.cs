using System.Net.Http;
using Microsoft.Extensions.Logging;
using NxTiler.Infrastructure.Settings;

namespace NxTiler.Infrastructure.Recording;

public sealed partial class FfmpegSetupService
{
    public async Task<string?> DownloadAsync(Action<double, string>? progress = null, CancellationToken ct = default)
    {
        Directory.CreateDirectory(SettingsPaths.FfmpegDir);
        var zipPath = Path.Combine(SettingsPaths.FfmpegDir, "ffmpeg_download.zip");

        try
        {
            progress?.Invoke(0, "Downloading ffmpeg...");
            using var http = new HttpClient { Timeout = TimeSpan.FromMinutes(10) };
            await DownloadArchiveAsync(http, zipPath, progress, ct);

            progress?.Invoke(-1, "Extracting ffmpeg...");
            var installed = await InstallFromArchiveAsync(zipPath, ct);
            if (installed is null)
            {
                return null;
            }

            progress?.Invoke(1, "ffmpeg installed.");
            return installed;
        }
        catch (Exception ex)
        {
            logger.LogWarning(ex, "Failed to download ffmpeg.");
            progress?.Invoke(-1, "Failed to install ffmpeg.");
            return null;
        }
        finally
        {
            TryDeleteZip(zipPath);
        }
    }
}
