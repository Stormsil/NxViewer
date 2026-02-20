using Microsoft.Extensions.Hosting;

namespace NxTiler.Overlay;

/// <summary>
/// Starts and stops the ImGui overlay host as a .NET hosted service.
/// </summary>
public sealed class OverlayHostedService(OverlayHost overlay) : IHostedService
{
    private bool _started;

    public Task StartAsync(CancellationToken cancellationToken)
    {
        _started = true;
        // Run() blocks until the overlay is closed; fire-and-forget so the host doesn't wait.
        _ = overlay.Run();
        return Task.CompletedTask;
    }

    public Task StopAsync(CancellationToken cancellationToken)
    {
        if (_started)
        {
            overlay.Close();
        }

        return Task.CompletedTask;
    }
}
