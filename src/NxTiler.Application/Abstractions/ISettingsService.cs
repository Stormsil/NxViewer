using NxTiler.Domain.Settings;

namespace NxTiler.Application.Abstractions;

public interface ISettingsService
{
    AppSettingsSnapshot Current { get; }

    void Update(AppSettingsSnapshot snapshot);

    Task SaveAsync(CancellationToken ct = default);

    Task ReloadAsync(CancellationToken ct = default);
}
