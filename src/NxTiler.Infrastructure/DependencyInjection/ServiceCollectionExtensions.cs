using Microsoft.Extensions.DependencyInjection;
using NxTiler.Application.Abstractions;
using NxTiler.Infrastructure.Capture;
using NxTiler.Infrastructure.Grid;
using NxTiler.Infrastructure.Hotkeys;
using NxTiler.Infrastructure.Nomachine;
using NxTiler.Infrastructure.Recording;
using NxTiler.Infrastructure.Settings;
using NxTiler.Infrastructure.Tray;
using NxTiler.Infrastructure.Vision;
using NxTiler.Infrastructure.Windowing;

namespace NxTiler.Infrastructure.DependencyInjection;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddNxTilerInfrastructure(this IServiceCollection services)
    {
        services.AddSingleton<ISettingsMigrationService, SettingsMigrationService>();
        services.AddSingleton<ISettingsService, JsonSettingsService>();

        services.AddSingleton<IWindowTracker, ProcessWindowTracker>();
        services.AddSingleton<IWindowRulesEngine, WindowRulesEngine>();
        services.AddSingleton<IWindowGroupService, WindowGroupService>();
        services.AddSingleton<IGridPresetService, GridPresetService>();

        services.AddSingleton<IWindowQueryService, Win32WindowQueryService>();
        services.AddSingleton<IWindowControlService, Win32WindowControlService>();
        services.AddSingleton<IHotkeyService, GlobalHotkeyService>();
        services.AddSingleton<IWindowEventMonitorService, WindowEventMonitorService>();
        services.AddSingleton<ITrayService, TrayService>();
        services.AddSingleton<ICaptureService, WgcCaptureService>();
        services.AddSingleton<IVisionEngine, YoloVisionEngine>();
        services.AddSingleton<IVisionEngine, TemplateVisionEngine>();

        services.AddSingleton<INomachineSessionService, NomachineSessionService>();
        services.AddSingleton<IFfmpegSetupService, FfmpegSetupService>();
        services.AddSingleton<IVideoRecordingEngine, WgcVideoRecordingEngine>();
        services.AddTransient<IRecordingEngine, FfmpegRecordingEngine>();

        return services;
    }
}
