using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using NxTiler.App.Services;
using NxTiler.Application.Abstractions;

namespace NxTiler.App;

public partial class App
{
    private static void RegisterWorkflowServices(IServiceCollection services)
    {
        services.AddSingleton<IRecordingOverlayService, RecordingOverlayService>();
        services.AddSingleton<ISnapshotSelectionService, SnapshotSelectionService>();
        services.AddSingleton<ICaptureWorkflowService, CaptureWorkflowService>();
        services.AddSingleton<IVisionWorkflowService, VisionWorkflowService>();
        services.AddSingleton<IRecordingWorkflowService, RecordingWorkflowService>();
        services.AddSingleton<IWorkspaceOrchestrator, WorkspaceOrchestrator>();
        services.AddSingleton<IDashboardWorkspaceCommandService, DashboardWorkspaceCommandService>();
        services.AddSingleton<IDashboardRecordingCommandService, DashboardRecordingCommandService>();
        services.AddSingleton<IDashboardCommandExecutionService, DashboardCommandExecutionService>();

        // Start the orchestrator (initialises tray icon + hotkeys) as a hosted service
        services.AddSingleton<IHostedService>(sp =>
            new WorkspaceOrchestratorHostedService(
                sp.GetRequiredService<IWorkspaceOrchestrator>()));
    }
}
