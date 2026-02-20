using NxTiler.Domain.Tracking;

namespace NxTiler.Application.Abstractions;

public interface IWindowTracker
{
    IReadOnlyDictionary<nint, WindowIdentity> TrackedWindows { get; }

    Task OnWindowDiscoveredAsync(nint handle, CancellationToken ct = default);

    void OnTitleChanged(nint handle, string newTitle);

    void OnWindowLost(nint handle);

    event EventHandler<WindowIdentity> WindowDiscovered;

    event EventHandler<WindowIdentity> WindowUpdated;

    event EventHandler<nint> WindowLost;
}
