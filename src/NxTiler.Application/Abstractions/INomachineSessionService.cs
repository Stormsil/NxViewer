using NxTiler.Domain.Windowing;

namespace NxTiler.Application.Abstractions;

public interface INomachineSessionService
{
    Task<IReadOnlyList<SessionFileInfo>> FindSessionsAsync(string folder, string filterRegexOrEmpty, CancellationToken ct = default);

    Task OpenIfNeededAsync(IEnumerable<SessionFileInfo> sessions, CancellationToken ct = default);

    Task LaunchSessionAsync(string sessionName, string nxsFolder, CancellationToken ct = default);
}
