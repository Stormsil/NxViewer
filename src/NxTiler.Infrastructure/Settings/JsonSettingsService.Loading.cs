using System.Text.Json;
using Microsoft.Extensions.Logging;
using NxTiler.Domain.Settings;

namespace NxTiler.Infrastructure.Settings;

public sealed partial class JsonSettingsService
{
    private AppSettingsSnapshot LoadInitialSnapshot()
    {
        try
        {
            return Task.Run(() => LoadAsync(CancellationToken.None)).GetAwaiter().GetResult();
        }
        catch (Exception ex)
        {
            _logger.LogWarning(ex, "Failed to load settings from disk. Falling back to defaults.");
            return AppSettingsSnapshot.CreateDefault();
        }
    }

    private async Task<AppSettingsSnapshot> LoadAsync(CancellationToken ct)
    {
        Directory.CreateDirectory(SettingsPaths.RootDir);

        if (!File.Exists(SettingsPaths.SettingsJsonPath))
        {
            var migrationResult = await _migrationService.MigrateFromLegacyAsync(ct);
            var migrated = await LoadFromBackupOrDefaultAsync(migrationResult.BackupPath, ct);
            await PersistSnapshotAsync(migrated, ct);
            _logger.LogInformation("Initialized settings.json from legacy settings.");
            return migrated;
        }

        try
        {
            var json = await File.ReadAllTextAsync(SettingsPaths.SettingsJsonPath, ct);
            var parsed = JsonSerializer.Deserialize<AppSettingsSnapshot>(json, JsonOptions);
            if (parsed is null)
            {
                throw new InvalidOperationException("settings.json is empty or invalid.");
            }

            var normalized = Normalize(parsed);
            if (!ReferenceEquals(parsed, normalized) && !Equals(parsed, normalized))
            {
                await PersistSnapshotAsync(normalized, ct);
            }

            return normalized;
        }
        catch (Exception ex)
        {
            _logger.LogWarning(ex, "Failed to parse settings.json. Falling back to legacy settings for compatibility.");
            var fallback = await LoadFromBackupOrDefaultAsync(SettingsPaths.LegacyBackupPath, ct);
            await PersistSnapshotAsync(fallback, ct);
            return fallback;
        }
    }

    private async Task<AppSettingsSnapshot> LoadFromBackupOrDefaultAsync(string? backupPath, CancellationToken ct)
    {
        if (!string.IsNullOrWhiteSpace(backupPath) && File.Exists(backupPath))
        {
            try
            {
                var backupJson = await File.ReadAllTextAsync(backupPath, ct);
                var backupSnapshot = JsonSerializer.Deserialize<AppSettingsSnapshot>(backupJson, JsonOptions);
                if (backupSnapshot is not null)
                {
                    return Normalize(backupSnapshot);
                }

                _logger.LogWarning("Backup settings snapshot is empty or invalid. Using defaults.");
            }
            catch (Exception ex)
            {
                _logger.LogWarning(ex, "Failed to read backup settings snapshot. Using defaults.");
            }
        }

        return AppSettingsSnapshot.CreateDefault();
    }
}
