using System.Windows.Threading;
using CommunityToolkit.Mvvm.Messaging;
using NxTiler.Application.Abstractions;
using NxTiler.Infrastructure.Native;

namespace NxTiler.Infrastructure.Windowing;

public sealed partial class WindowEventMonitorService : IWindowEventMonitorService
{
    private readonly HashSet<nint> _trackedHandles = new();
    private readonly int _selfPid;
    private readonly Win32Native.WinEventDelegate _winEventProc;
    private readonly List<nint> _hooks = new();
    private readonly DispatcherTimer _locationDebounce;
    private readonly IMessenger _messenger;
    private readonly IWindowTracker? _windowTracker;
    private bool _locationPending;
    private bool _disposed;

    public WindowEventMonitorService(IMessenger messenger, IWindowTracker? windowTracker = null)
    {
        _messenger = messenger;
        _windowTracker = windowTracker;
        _selfPid = Environment.ProcessId;
        _winEventProc = OnWinEvent;

        _locationDebounce = new DispatcherTimer { Interval = TimeSpan.FromMilliseconds(150) };
        _locationDebounce.Tick += LocationDebounceOnTick;
    }

    public Task UpdateTrackedWindowsAsync(IReadOnlyCollection<nint> handles, CancellationToken ct = default)
    {
        ct.ThrowIfCancellationRequested();

        _trackedHandles.Clear();
        foreach (var handle in handles)
        {
            _trackedHandles.Add(handle);
        }

        return Task.CompletedTask;
    }

    public bool IsTracked(nint handle) => _trackedHandles.Contains(handle);
}
