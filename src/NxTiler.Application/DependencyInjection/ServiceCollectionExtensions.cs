using Microsoft.Extensions.DependencyInjection;
using NxTiler.Application.Abstractions;
using NxTiler.Application.Services;

namespace NxTiler.Application.DependencyInjection;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddNxTilerApplication(this IServiceCollection services)
    {
        services.AddSingleton<IArrangementService, ArrangementService>();
        return services;
    }
}
