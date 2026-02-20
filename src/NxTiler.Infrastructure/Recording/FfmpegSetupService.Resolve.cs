using NxTiler.Infrastructure.Settings;

namespace NxTiler.Infrastructure.Recording;

public sealed partial class FfmpegSetupService
{
    public string? FindFfmpeg()
    {
        var configured = settingsService.Current.Paths.FfmpegPath;
        if (!string.IsNullOrWhiteSpace(configured) && !configured.Equals("ffmpeg", StringComparison.OrdinalIgnoreCase) && File.Exists(configured))
        {
            return configured;
        }

        if (File.Exists(SettingsPaths.LocalFfmpegPath))
        {
            return SettingsPaths.LocalFfmpegPath;
        }

        return IsInPath() ? "ffmpeg" : null;
    }

    public async Task<string?> ResolveAndSaveAsync(CancellationToken ct = default)
    {
        var path = FindFfmpeg();
        if (path is null || path.Equals("ffmpeg", StringComparison.OrdinalIgnoreCase))
        {
            return path;
        }

        var updated = settingsService.Current with
        {
            Paths = settingsService.Current.Paths with { FfmpegPath = path },
        };
        settingsService.Update(updated);
        await settingsService.SaveAsync(ct);
        return path;
    }
}
