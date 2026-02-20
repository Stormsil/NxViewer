using NxTiler.Domain.Enums;

namespace NxTiler.App.Services;

public interface IDashboardWorkspaceCommandService
{
    Task RefreshAsync(CancellationToken ct = default);

    Task ArrangeNowAsync(CancellationToken ct = default);

    Task OpenMissingSessionsAsync(CancellationToken ct = default);

    Task CycleModeAsync(CancellationToken ct = default);

    Task SelectWindowAsync(int index, CancellationToken ct = default);

    Task ReconnectWindowAsync(int index, CancellationToken ct = default);

    Task NavigateAsync(int delta, CancellationToken ct = default);

    Task ToggleMinimizeAllAsync(CancellationToken ct = default);

    Task SetAutoArrangeAsync(bool enabled, CancellationToken ct = default);

    Task SetModeAsync(TileMode mode, CancellationToken ct = default);
}
