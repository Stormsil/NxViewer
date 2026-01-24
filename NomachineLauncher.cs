using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading;

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

    public static void OpenIfNeeded(IEnumerable<(string name, string fullPath)> sessions)
    {
        // Просто ассоциацией ОС (быстрее и надёжнее, чем угадывать путь nxplayer)
        foreach (var s in sessions)
        {
            try { Process.Start(new ProcessStartInfo(s.fullPath) { UseShellExecute = true }); }
            catch { /* ignore */ }
            Thread.Sleep(250); // лёгкий разброс, чтобы окна открывались по очереди
        }
    }
}
