using System.Collections.Concurrent;
using System.Management;
using Microsoft.Extensions.Logging;
using NxTiler.Application.Abstractions;
using NxTiler.Domain.Tracking;
using NxTiler.Infrastructure.Native;

namespace NxTiler.Infrastructure.Windowing;

public sealed class ProcessWindowTracker(ILogger<ProcessWindowTracker> logger) : IWindowTracker
{
    private static readonly TimeSpan WmiTimeout = TimeSpan.FromSeconds(5);
    private readonly ConcurrentDictionary<nint, WindowIdentity> _cache = new();

    public IReadOnlyDictionary<nint, WindowIdentity> TrackedWindows => _cache;

    public event EventHandler<WindowIdentity>? WindowDiscovered;
    public event EventHandler<WindowIdentity>? WindowUpdated;
    public event EventHandler<nint>? WindowLost;

    public async Task OnWindowDiscoveredAsync(nint handle, CancellationToken ct = default)
    {
        ct.ThrowIfCancellationRequested();

        Win32Native.GetWindowThreadProcessId(handle, out var processId);
        var className = Win32Native.GetClassNameSafe(handle);
        var title = Win32Native.GetWindowTextSafe(handle);
        var exePath = Win32Native.QueryProcessExePath(processId);
        var exeBaseName = Path.GetFileName(exePath);

        var identity = new WindowIdentity(
            Handle: handle,
            ProcessId: processId,
            ExePath: exePath,
            ExeBaseName: exeBaseName,
            CommandLine: string.Empty,
            WindowClassName: className,
            LastKnownTitle: title);

        _cache[handle] = identity;
        WindowDiscovered?.Invoke(this, identity);

        _ = EnrichWithCommandLineAsync(handle, processId, identity, ct);
    }

    public void OnTitleChanged(nint handle, string newTitle)
    {
        if (_cache.TryGetValue(handle, out var existing))
        {
            var updated = existing with { LastKnownTitle = newTitle };
            _cache[handle] = updated;
            WindowUpdated?.Invoke(this, updated);
        }
    }

    public void OnWindowLost(nint handle)
    {
        _cache.TryRemove(handle, out _);
        WindowLost?.Invoke(this, handle);
    }

    private async Task EnrichWithCommandLineAsync(nint handle, uint processId, WindowIdentity initial, CancellationToken ct)
    {
        try
        {
            using var cts = CancellationTokenSource.CreateLinkedTokenSource(ct);
            cts.CancelAfter(WmiTimeout);

            var commandLine = await Task.Run(() => QueryCommandLine(processId), cts.Token);

            if (!_cache.TryGetValue(handle, out var current))
            {
                return;
            }

            var enriched = current with { CommandLine = commandLine };
            _cache[handle] = enriched;
            WindowUpdated?.Invoke(this, enriched);
        }
        catch (OperationCanceledException)
        {
            logger.LogDebug("WMI CommandLine query timed out for PID {ProcessId}.", processId);
        }
        catch (Exception ex)
        {
            logger.LogDebug(ex, "WMI CommandLine query failed for PID {ProcessId}.", processId);
        }
    }

    private static string QueryCommandLine(uint processId)
    {
        using var searcher = new ManagementObjectSearcher(
            $"SELECT CommandLine FROM Win32_Process WHERE ProcessId = {processId}");

        using var results = searcher.Get();
        foreach (ManagementObject obj in results)
        {
            return obj["CommandLine"]?.ToString() ?? string.Empty;
        }

        return string.Empty;
    }
}
