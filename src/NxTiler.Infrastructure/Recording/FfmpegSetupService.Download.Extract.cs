using System.IO.Compression;
using NxTiler.Infrastructure.Settings;

namespace NxTiler.Infrastructure.Recording;

public sealed partial class FfmpegSetupService
{
    private async Task<string?> InstallFromArchiveAsync(string zipPath, CancellationToken ct)
    {
        using var zip = ZipFile.OpenRead(zipPath);
        var entry = ResolveFfmpegEntry(zip);
        if (entry is null)
        {
            return null;
        }

        entry.ExtractToFile(SettingsPaths.LocalFfmpegPath, overwrite: true);
        await PersistLocalFfmpegPathAsync(ct);
        return SettingsPaths.LocalFfmpegPath;
    }

    private static ZipArchiveEntry? ResolveFfmpegEntry(ZipArchive zip)
    {
        return zip.Entries.FirstOrDefault(x =>
                   x.Name.Equals("ffmpeg.exe", StringComparison.OrdinalIgnoreCase) &&
                   x.FullName.Contains("bin/", StringComparison.OrdinalIgnoreCase))
               ?? zip.Entries.FirstOrDefault(x =>
                   x.Name.Equals("ffmpeg.exe", StringComparison.OrdinalIgnoreCase));
    }

    private async Task PersistLocalFfmpegPathAsync(CancellationToken ct)
    {
        var updated = settingsService.Current with
        {
            Paths = settingsService.Current.Paths with { FfmpegPath = SettingsPaths.LocalFfmpegPath },
        };

        settingsService.Update(updated);
        await settingsService.SaveAsync(ct);
    }
}
