using System.Diagnostics;
using System.IO;
using Microsoft.Extensions.Logging;
using NxTiler.Domain.Windowing;

namespace NxTiler.App.Services;

public sealed partial class WorkspaceOrchestrator
{
    public async Task OpenMissingSessionsAsync(CancellationToken ct = default)
    {
        List<SessionFileInfo> toOpen;

        await _arrangementGate.WaitAsync(ct);
        try
        {
            await RefreshTargetsAsync(ct);

            var sessions = await _nomachineSessionService.FindSessionsAsync(
                _settingsService.Current.Paths.NxsFolder,
                _settingsService.Current.Filters.NameFilter,
                ct);

            var disabledSessions = _settingsService.Current.DisabledSessions.ToHashSet(StringComparer.OrdinalIgnoreCase);
            var runningNames = _targets.Select(static x => x.SourceName).ToHashSet(StringComparer.OrdinalIgnoreCase);

            toOpen = sessions.Where(x => !disabledSessions.Contains(x.Name) && !runningNames.Contains(x.Name)).ToList();
            if (toOpen.Count == 0)
            {
                RaiseStatus("No matching sessions to open.");
                PublishSnapshot();
                return;
            }

            RaiseStatus($"Opening {toOpen.Count} missing sessions...");
        }
        finally
        {
            _arrangementGate.Release();
        }

        await _nomachineSessionService.OpenIfNeededAsync(toOpen, ct);
        await Task.Delay(1200, ct);
        await ArrangeNowAsync(ct);
    }

    public async Task ReconnectWindowAsync(int index, CancellationToken ct = default)
    {
        await RefreshAsync(ct);
        if (index < 0 || index >= _targets.Count)
        {
            _logger.LogWarning("Reconnect failed: index {Index} out of range (count {Count})", index, _targets.Count);
            return;
        }

        var target = _targets[index];
        try
        {
            var proc = Process.GetProcessById((int)target.ProcessId);
            _logger.LogInformation("Killing process {ProcessId} for session {SessionName}", target.ProcessId, target.SourceName);
            proc.Kill();
        }
        catch (Exception ex)
        {
            _logger.LogDebug(ex, "Unable to kill process for {SessionName}", target.SourceName);
        }

        RaiseStatus($"Reconnecting {target.SourceName}...");
        await Task.Delay(2000, ct);

        var nxsPath = Path.Combine(_settingsService.Current.Paths.NxsFolder, target.SourceName + ".nxs");
        _logger.LogInformation("Attempting to relaunch session {SessionName} from {Path}", target.SourceName, nxsPath);

        await _nomachineSessionService.LaunchSessionAsync(target.SourceName, _settingsService.Current.Paths.NxsFolder, ct);
        await Task.Delay(2500, ct);
        await ArrangeNowAsync(ct);
    }
}
