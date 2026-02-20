namespace NxTiler.Application.Abstractions;

public interface IWindowEventMonitorService : IDisposable
{
    Task StartAsync(CancellationToken ct = default);

    Task StopAsync(CancellationToken ct = default);

    Task UpdateTrackedWindowsAsync(IReadOnlyCollection<nint> handles, CancellationToken ct = default);

    bool IsTracked(nint handle);
}
