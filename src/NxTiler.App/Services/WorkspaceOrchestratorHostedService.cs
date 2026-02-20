using Microsoft.Extensions.Hosting;

namespace NxTiler.App.Services;

/// <summary>
/// Bridges IWorkspaceOrchestrator into the Generic Host lifecycle so
/// that StartAsync (tray + hotkeys) is called automatically on app start.
/// </summary>
public sealed class WorkspaceOrchestratorHostedService(IWorkspaceOrchestrator orchestrator) : IHostedService
{
    public Task StartAsync(CancellationToken cancellationToken)
        => orchestrator.StartAsync(cancellationToken);

    public async Task StopAsync(CancellationToken cancellationToken)
        => await orchestrator.DisposeAsync();
}
