using System.Diagnostics;
using Microsoft.Extensions.Logging;
using NxTiler.Domain.Windowing;

namespace NxTiler.Infrastructure.Nomachine;

public sealed partial class NomachineSessionService
{
    public async Task OpenIfNeededAsync(IEnumerable<SessionFileInfo> sessions, CancellationToken ct = default)
    {
        foreach (var session in sessions)
        {
            ct.ThrowIfCancellationRequested();
            try
            {
                _ = Process.Start(new ProcessStartInfo(session.FullPath) { UseShellExecute = true });
            }
            catch (Exception ex)
            {
                logger.LogWarning(ex, "Failed to launch session file: {Path}", session.FullPath);
            }

            await Task.Delay(250, ct);
        }
    }

    public async Task LaunchSessionAsync(string sessionName, string nxsFolder, CancellationToken ct = default)
    {
        ct.ThrowIfCancellationRequested();

        if (!Directory.Exists(nxsFolder))
        {
            logger.LogWarning("LaunchSession failed: Folder not found {Folder}", nxsFolder);
            return;
        }

        var path = Path.Combine(nxsFolder, sessionName + ".nxs");
        if (!File.Exists(path))
        {
            var files = Directory.GetFiles(nxsFolder, "*.nxs");
            // 1. Try case-insensitive exact match
            var match = files.FirstOrDefault(f => Path.GetFileNameWithoutExtension(f).Equals(sessionName, StringComparison.OrdinalIgnoreCase));

            // 2. Try fuzzy match (if file contains session name)
            if (match == null)
            {
                match = files.FirstOrDefault(f => Path.GetFileNameWithoutExtension(f).Contains(sessionName, StringComparison.OrdinalIgnoreCase));
            }

            if (match != null)
            {
                path = match;
            }
            else
            {
                logger.LogWarning("LaunchSession failed: No .nxs file matching '{SessionName}' found in {Folder}", sessionName, nxsFolder);
                return;
            }
        }

        try
        {
            logger.LogInformation("Launching session: {Path}", path);
            var psi = new ProcessStartInfo
            {
                FileName = path,
                UseShellExecute = true,
                WindowStyle = ProcessWindowStyle.Normal,
            };
            Process.Start(psi);
            await Task.CompletedTask;
        }
        catch (Exception ex)
        {
            logger.LogWarning(ex, "Failed to launch session: {SessionName} (Path: {Path})", sessionName, path);
        }
    }
}
