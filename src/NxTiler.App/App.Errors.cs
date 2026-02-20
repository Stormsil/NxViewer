using Microsoft.Extensions.DependencyInjection;
using NxTiler.App.Services;
using Serilog;
using System.Windows.Threading;

namespace NxTiler.App;

public partial class App
{
    private static void RegisterGlobalExceptionHandlers()
    {
        TaskScheduler.UnobservedTaskException += OnTaskSchedulerUnobservedTaskException;
        AppDomain.CurrentDomain.UnhandledException += OnCurrentDomainUnhandledException;
    }

    private static void OnTaskSchedulerUnobservedTaskException(object? sender, UnobservedTaskExceptionEventArgs args)
    {
        Log.Error(args.Exception, "Unobserved task exception");
        args.SetObserved();
    }

    private static void OnCurrentDomainUnhandledException(object? sender, UnhandledExceptionEventArgs args)
    {
        if (args.ExceptionObject is Exception ex)
        {
            Log.Fatal(ex, "AppDomain unhandled exception");
        }
    }

    private void OnDispatcherUnhandledException(object sender, DispatcherUnhandledExceptionEventArgs e)
    {
        Log.Fatal(e.Exception, "Dispatcher unhandled exception");
        Services.GetService<IUserFeedbackService>()?.Error(
            "NxTiler",
            "A fatal error occurred. See logs for details.");
        e.Handled = true;
    }
}
