using System.Text.Json;
using NxTiler.Domain.Settings;

namespace NxTiler.Infrastructure.Settings;

public sealed partial class JsonSettingsService
{
    private static async Task PersistSnapshotAsync(AppSettingsSnapshot snapshot, CancellationToken ct)
    {
        Directory.CreateDirectory(SettingsPaths.RootDir);
        var json = JsonSerializer.Serialize(snapshot, JsonOptions);
        var tempPath = SettingsPaths.SettingsJsonPath + ".tmp";

        try
        {
            await File.WriteAllTextAsync(tempPath, json, Utf8NoBom, ct);
            File.Move(tempPath, SettingsPaths.SettingsJsonPath, overwrite: true);
        }
        finally
        {
            if (File.Exists(tempPath))
            {
                File.Delete(tempPath);
            }
        }
    }
}
