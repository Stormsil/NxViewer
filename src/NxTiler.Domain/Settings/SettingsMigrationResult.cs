namespace NxTiler.Domain.Settings;

public sealed record SettingsMigrationResult(bool Migrated, string? BackupPath, string Message);
