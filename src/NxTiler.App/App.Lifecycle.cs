using Serilog;
using System.Windows;

namespace NxTiler.App;

public partial class App
{
    protected override async void OnStartup(StartupEventArgs e)
    {
        RegisterGlobalExceptionHandlers();
        ShutdownMode = ShutdownMode.OnExplicitShutdown;

        try
        {
            await Host.StartAsync();
        }
        catch (Exception ex)
        {
            Log.Fatal(ex, "Application startup failed");
            MessageBox.Show(
                "NxTiler failed to start. Check logs for details.",
                "NxTiler",
                MessageBoxButton.OK,
                MessageBoxImage.Error);
            Shutdown(-1);
        }
        finally
        {
            base.OnStartup(e);
        }
    }

    protected override async void OnExit(ExitEventArgs e)
    {
        try
        {
            await Host.StopAsync();
        }
        catch (Exception ex)
        {
            Log.Error(ex, "Application stop failed");
        }
        finally
        {
            Host.Dispose();
            Log.CloseAndFlush();
            base.OnExit(e);
        }
    }
}
