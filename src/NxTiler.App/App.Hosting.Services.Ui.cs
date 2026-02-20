using Microsoft.Extensions.DependencyInjection;
using NxTiler.App.Services;
using NxTiler.Application.Abstractions;

namespace NxTiler.App;

public partial class App
{
    private static void RegisterUiInfrastructureServices(IServiceCollection services)
    {
        services.AddSingleton<IUiDispatcher, WpfUiDispatcher>();
        services.AddSingleton<ICursorPositionProvider, Win32CursorPositionProvider>();
    }
}
