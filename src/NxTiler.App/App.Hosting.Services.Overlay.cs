using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using NxTiler.App.Services;
using NxTiler.Application.Abstractions;
using NxTiler.Overlay;

namespace NxTiler.App;

public partial class App
{
    private static void RegisterOverlayServices(IServiceCollection services)
    {
        // Register concrete ImGui overlay types (constructed lazily; only activated when flag is set)
        services.AddSingleton<OverlayRenderService>();
        services.AddSingleton<IOverlayRenderService>(sp => sp.GetRequiredService<OverlayRenderService>());
        services.AddSingleton<OverlayHost>();

        // OverlayHostedService starts the ImGui render loop (checks UseImGuiOverlay on startup)
        services.AddSingleton<OverlayHostedService>();
        services.AddSingleton<IHostedService>(sp => sp.GetRequiredService<OverlayHostedService>());

        // ImGuiOverlayBridge subscribes to workspace messages and pushes OverlayState (checks UseImGuiOverlay on startup)
        services.AddSingleton<ImGuiOverlayBridge>();
        services.AddSingleton<IHostedService>(sp => sp.GetRequiredService<ImGuiOverlayBridge>());
    }
}
