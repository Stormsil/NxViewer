using System.Text.Json;
using Microsoft.Extensions.Logging;
using NxTiler.Application.Abstractions;
using NxTiler.Domain.Settings;

namespace NxTiler.Infrastructure.Settings;

public sealed class SettingsMigrationService(ILogger<SettingsMigrationService> logger) : ISettingsMigrationService
{
    private const int LegacyReadTimeoutMs = 3000;

    private static readonly JsonSerializerOptions JsonOptions = new()
    {
        WriteIndented = true,
    };

    public async Task<SettingsMigrationResult> MigrateFromLegacyAsync(CancellationToken ct = default)
    {
        try
        {
            Directory.CreateDirectory(SettingsPaths.RootDir);

            if (File.Exists(SettingsPaths.SettingsJsonPath))
            {
                return new SettingsMigrationResult(false, null, "JSON settings already exist.");
            }

            var (legacyReadSucceeded, snapshot) = await TryReadLegacySnapshotAsync(ct);
            var backupJson = JsonSerializer.Serialize(snapshot, JsonOptions);
            await File.WriteAllTextAsync(SettingsPaths.LegacyBackupPath, backupJson, ct);

            logger.LogInformation("Legacy settings snapshot was saved to {BackupPath}", SettingsPaths.LegacyBackupPath);
            return new SettingsMigrationResult(
                Migrated: legacyReadSucceeded,
                BackupPath: SettingsPaths.LegacyBackupPath,
                Message: legacyReadSucceeded
                    ? "Legacy settings migrated."
                    : "Legacy migration timed out. Backup contains defaults.");
        }
        catch (Exception ex)
        {
            logger.LogWarning(ex, "Failed to migrate legacy settings");
            return new SettingsMigrationResult(false, null, "Legacy migration failed. Defaults will be used.");
        }
    }

    private async Task<(bool Success, AppSettingsSnapshot Snapshot)> TryReadLegacySnapshotAsync(CancellationToken ct)
    {
        try
        {
            var readTask = Task.Run(static () => LegacySettingsMapper.Read(), ct);
            var completed = await Task.WhenAny(readTask, Task.Delay(LegacyReadTimeoutMs, ct));
            if (completed == readTask)
            {
                return (true, await readTask);
            }

            logger.LogWarning("Legacy settings read timed out after {TimeoutMs}ms. Using defaults.", LegacyReadTimeoutMs);
            return (false, AppSettingsSnapshot.CreateDefault());
        }
        catch (OperationCanceledException) when (ct.IsCancellationRequested)
        {
            throw;
        }
        catch (Exception ex)
        {
            logger.LogWarning(ex, "Legacy settings read failed. Using defaults.");
            return (false, AppSettingsSnapshot.CreateDefault());
        }
    }
}
