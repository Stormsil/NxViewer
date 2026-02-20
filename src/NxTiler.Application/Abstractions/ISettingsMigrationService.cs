using NxTiler.Domain.Settings;

namespace NxTiler.Application.Abstractions;

public interface ISettingsMigrationService
{
    Task<SettingsMigrationResult> MigrateFromLegacyAsync(CancellationToken ct = default);
}
