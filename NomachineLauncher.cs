using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace NxTiler;

public static class NomachineLauncher
{
    public static List<(string name, string fullPath)> FindSessions(string folder, string filterRegexOrEmpty)
    {
        var files = Directory.Exists(folder)
            ? Directory.EnumerateFiles(folder, "*.nxs").ToList()
            : new List<string>();

        var names = files.Select(p => Path.GetFileNameWithoutExtension(p));
        var filteredNames = Tiler.FilterSessionNames(names, filterRegexOrEmpty).ToHashSet(StringComparer.OrdinalIgnoreCase);

        return files
            .Select(p => (Path.GetFileNameWithoutExtension(p), p))
            .Where(t => filteredNames.Contains(t.Item1))
            .OrderBy(t => t.Item1, StringComparer.OrdinalIgnoreCase)
            .ToList();
    }

    public static async void OpenIfNeeded(IEnumerable<(string name, string fullPath)> sessions)
    {
        foreach (var s in sessions)
        {
            try { Process.Start(new ProcessStartInfo(s.fullPath) { UseShellExecute = true }); }
            catch { /* ignore */ }
            await Task.Delay(250); 
        }
    }

    public static async void LaunchSession(string sessionName, string nxsFolder)
    {
        // Try to find the specific .nxs file
        var path = Path.Combine(nxsFolder, sessionName + ".nxs");
        if (File.Exists(path))
        {
             try { Process.Start(new ProcessStartInfo(path) { UseShellExecute = true }); }
             catch { }
        }
    }
}
