namespace NxTiler.Infrastructure.Settings;

internal static class SettingsPaths
{
    public static string RootDir { get; } = ResolveRootDir();

    public static string SettingsJsonPath { get; } = Path.Combine(RootDir, "settings.json");

    public static string LegacyBackupPath { get; } = Path.Combine(RootDir, "settings.legacy.backup.json");

    public static string FfmpegDir { get; } = Path.Combine(RootDir, "ffmpeg");

    public static string LocalFfmpegPath { get; } = Path.Combine(FfmpegDir, "ffmpeg.exe");

    public static string LogsDir { get; } = Path.Combine(RootDir, "Logs");

    private static string ResolveRootDir()
    {
        var overrideRoot = Environment.GetEnvironmentVariable("NXTILER_APPDATA");
        if (!string.IsNullOrWhiteSpace(overrideRoot))
        {
            return overrideRoot;
        }

        return Path.Combine(
            Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData),
            "NxTiler");
    }
}
