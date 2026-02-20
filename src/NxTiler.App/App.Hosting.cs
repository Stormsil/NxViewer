using System.IO;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using NxTiler.App.Logging;
using Serilog;
using Serilog.Events;

namespace NxTiler.App;

public partial class App
{
    private static IHost CreateHost()
    {
        return Microsoft.Extensions.Hosting.Host
            .CreateDefaultBuilder()
            .ConfigureAppConfiguration(ConfigureAppConfiguration)
            .UseSerilog(ConfigureSerilog)
            .ConfigureServices(ConfigureServices)
            .Build();
    }

    private static void ConfigureAppConfiguration(HostBuilderContext _, IConfigurationBuilder config)
    {
        config.SetBasePath(AppContext.BaseDirectory);
        config.AddJsonFile("appsettings.json", optional: true, reloadOnChange: true);
    }

    private static void ConfigureSerilog(
        HostBuilderContext context,
        IServiceProvider _,
        LoggerConfiguration loggerConfiguration)
    {
        var logsDirectory = Path.Combine(
            Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData),
            "NxTiler",
            "Logs");
        Directory.CreateDirectory(logsDirectory);

        var logPath = Path.Combine(logsDirectory, "log-.txt");
        loggerConfiguration
            .ReadFrom.Configuration(context.Configuration)
            .MinimumLevel.ControlledBy(LoggingRuntime.LevelSwitch)
            .MinimumLevel.Override("Microsoft", LogEventLevel.Warning)
            .MinimumLevel.Override("System", LogEventLevel.Warning)
            .Enrich.FromLogContext()
            .WriteTo.Sink(new InMemoryLogSink(LoggingRuntime.Buffer))
            .WriteTo.Debug()
            .WriteTo.File(
                path: logPath,
                rollingInterval: RollingInterval.Day,
                retainedFileCountLimit: 30,
                outputTemplate: "{Timestamp:yyyy-MM-dd HH:mm:ss.fff} [{Level:u3}] {Message:lj}{NewLine}{Exception}");
    }

    private static void ConfigureServices(HostBuilderContext _, IServiceCollection services)
    {
        RegisterCoreServices(services);
        RegisterUiInfrastructureServices(services);
        RegisterWorkflowServices(services);
        RegisterOverlayServices(services);
        RegisterViewModels(services);
        RegisterViews(services);
    }
}
