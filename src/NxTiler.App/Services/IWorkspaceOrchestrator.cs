using NxTiler.App.Models;
using NxTiler.Domain.Enums;

namespace NxTiler.App.Services;

public interface IWorkspaceOrchestrator : IAsyncDisposable
{
    WorkspaceSnapshot Snapshot { get; }

    Task StartAsync(CancellationToken ct = default);

    Task RefreshAsync(CancellationToken ct = default);

    Task ArrangeNowAsync(CancellationToken ct = default);

    Task OpenMissingSessionsAsync(CancellationToken ct = default);

    Task CycleModeAsync(CancellationToken ct = default);

    Task SetModeAsync(TileMode mode, CancellationToken ct = default);

    Task SetAutoArrangeAsync(bool enabled, CancellationToken ct = default);

    Task NavigateAsync(int delta, CancellationToken ct = default);

    Task SelectWindowAsync(int index, CancellationToken ct = default);

    Task ReconnectWindowAsync(int index, CancellationToken ct = default);

    Task ToggleMinimizeAllAsync(CancellationToken ct = default);

    void RequestMainWindowToggle();
}
