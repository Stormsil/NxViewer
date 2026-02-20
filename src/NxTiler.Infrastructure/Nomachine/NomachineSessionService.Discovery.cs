using NxTiler.Application.Parsing;
using NxTiler.Domain.Windowing;

namespace NxTiler.Infrastructure.Nomachine;

public sealed partial class NomachineSessionService
{
    public Task<IReadOnlyList<SessionFileInfo>> FindSessionsAsync(string folder, string filterRegexOrEmpty, CancellationToken ct = default)
    {
        ct.ThrowIfCancellationRequested();

        var files = Directory.Exists(folder)
            ? Directory.EnumerateFiles(folder, "*.nxs").ToList()
            : [];

        var names = files.Select(Path.GetFileNameWithoutExtension)
            .Where(static x => !string.IsNullOrWhiteSpace(x))
            .Cast<string>();

        var filteredNames = FilterSessionNames(names, filterRegexOrEmpty)
            .ToHashSet(StringComparer.OrdinalIgnoreCase);

        IReadOnlyList<SessionFileInfo> result = files
            .Select(path => new SessionFileInfo(Path.GetFileNameWithoutExtension(path), path))
            .Where(static x => !string.IsNullOrWhiteSpace(x.Name))
            .Where(x => filteredNames.Contains(x.Name))
            .OrderBy(static x => SessionNameParser.ParseNumericSuffix(x.Name))
            .ThenBy(static x => x.Name, StringComparer.OrdinalIgnoreCase)
            .ToList();

        return Task.FromResult(result);
    }
}
