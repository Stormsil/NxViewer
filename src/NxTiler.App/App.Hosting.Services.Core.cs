using CommunityToolkit.Mvvm.Messaging;
using Microsoft.Extensions.DependencyInjection;
using NxTiler.App.Logging;
using NxTiler.Application.DependencyInjection;
using NxTiler.Infrastructure.DependencyInjection;

namespace NxTiler.App;

public partial class App
{
    private static void RegisterCoreServices(IServiceCollection services)
    {
        services.AddNxTilerApplication();
        services.AddNxTilerInfrastructure();
        services.AddSingleton<IMessenger>(_ => WeakReferenceMessenger.Default);
        services.AddSingleton<ILogBufferService>(_ => LoggingRuntime.Buffer);
    }
}
