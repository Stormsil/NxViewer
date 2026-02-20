using Serilog.Core;
using Serilog.Events;

namespace NxTiler.App.Logging;

public static class LoggingRuntime
{
    // Default stays aligned with current appsettings.json (Information).
    public static LoggingLevelSwitch LevelSwitch { get; } = new(LogEventLevel.Information);

    public static LogBufferService Buffer { get; } = LogBufferService.Instance;
}
