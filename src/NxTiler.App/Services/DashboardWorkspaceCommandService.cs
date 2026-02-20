using NxTiler.Domain.Enums;

namespace NxTiler.App.Services;

public sealed class DashboardWorkspaceCommandService(
    IWorkspaceOrchestrator workspaceOrchestrator) : IDashboardWorkspaceCommandService
{
    public Task RefreshAsync(CancellationToken ct = default) => workspaceOrchestrator.RefreshAsync(ct);

    public Task ArrangeNowAsync(CancellationToken ct = default) => workspaceOrchestrator.ArrangeNowAsync(ct);

    public Task OpenMissingSessionsAsync(CancellationToken ct = default) => workspaceOrchestrator.OpenMissingSessionsAsync(ct);

    public Task CycleModeAsync(CancellationToken ct = default) => workspaceOrchestrator.CycleModeAsync(ct);

    public Task SelectWindowAsync(int index, CancellationToken ct = default) => workspaceOrchestrator.SelectWindowAsync(index, ct);

    public Task ReconnectWindowAsync(int index, CancellationToken ct = default) => workspaceOrchestrator.ReconnectWindowAsync(index, ct);

    public Task NavigateAsync(int delta, CancellationToken ct = default) => workspaceOrchestrator.NavigateAsync(delta, ct);

    public Task ToggleMinimizeAllAsync(CancellationToken ct = default) => workspaceOrchestrator.ToggleMinimizeAllAsync(ct);

    public Task SetAutoArrangeAsync(bool enabled, CancellationToken ct = default) => workspaceOrchestrator.SetAutoArrangeAsync(enabled, ct);

    public Task SetModeAsync(TileMode mode, CancellationToken ct = default) => workspaceOrchestrator.SetModeAsync(mode, ct);
}
