using NxTiler.Application.Abstractions;
using NxTiler.Infrastructure.Native;

namespace NxTiler.Infrastructure.Windowing;

public sealed partial class Win32WindowControlService : IWindowControlService
{
    public Task MaximizeAsync(nint windowHandle, CancellationToken ct = default)
    {
        ct.ThrowIfCancellationRequested();
        Win32Native.ShowWindow(windowHandle, Win32Native.SwShowMaximized);
        Win32Native.SetForegroundWindow(windowHandle);
        return Task.CompletedTask;
    }

    public Task MinimizeAllAsync(IReadOnlyList<nint> windows, CancellationToken ct = default)
    {
        foreach (var handle in windows)
        {
            ct.ThrowIfCancellationRequested();
            Win32Native.ShowWindow(handle, Win32Native.SwShowMinimized);
        }

        return Task.CompletedTask;
    }

    public Task RestoreAllAsync(IReadOnlyList<nint> windows, CancellationToken ct = default)
    {
        foreach (var handle in windows)
        {
            ct.ThrowIfCancellationRequested();
            Win32Native.ShowWindow(handle, Win32Native.SwShowNormal);
        }

        return Task.CompletedTask;
    }

    public Task BringToForegroundAsync(nint windowHandle, CancellationToken ct = default)
    {
        ct.ThrowIfCancellationRequested();
        Win32Native.SetForegroundWindow(windowHandle);
        return Task.CompletedTask;
    }
}
