using NxTiler.Application.Parsing;
using NxTiler.Domain.Tracking;
using NxTiler.Domain.Windowing;
using NxTiler.Infrastructure.Native;

namespace NxTiler.Infrastructure.Windowing;

public sealed partial class Win32WindowQueryService
{
    public Task<IReadOnlyList<TargetWindowInfo>> QueryAsync(WindowQueryOptions options, CancellationToken ct = default)
    {
        ct.ThrowIfCancellationRequested();

        var titleRegex = BuildRegex(options.TitleFilter);
        var nameRegex = BuildRegex(options.SessionNameFilter);
        var discovered = new List<TargetWindowInfo>();
        var trackedWindows = windowTracker?.TrackedWindows;

        Win32Native.EnumWindows((handle, _) =>
        {
            if (!Win32Native.IsWindowVisible(handle))
            {
                return true;
            }

            var title = Win32Native.GetWindowTextSafe(handle);
            if (string.IsNullOrWhiteSpace(title))
            {
                return true;
            }

            Win32Native.GetWindowThreadProcessId(handle, out var processId);
            if (processId == (uint)options.SelfProcessId)
            {
                return true;
            }

            // Check tracker cache first; fall back to title-based NoMachine detection
            WindowIdentity? identity = null;
            trackedWindows?.TryGetValue(handle, out identity);

            var isNoMachineByIdentity = identity is not null &&
                identity.ExeBaseName.StartsWith("nxplayer", StringComparison.OrdinalIgnoreCase);
            var isNoMachineByTitle = title.Contains("NoMachine", StringComparison.OrdinalIgnoreCase);

            if (!isNoMachineByIdentity && !isNoMachineByTitle)
            {
                return true;
            }

            // Prefer NXS file name from CommandLine; fall back to title parse
            var sessionName = identity?.SessionNameFromNxs
                ?? SessionNameParser.ExtractSessionNameFromTitle(title);

            if (string.IsNullOrWhiteSpace(sessionName) || sessionName.Equals("NoMachine", StringComparison.OrdinalIgnoreCase))
            {
                return true;
            }

            if (nameRegex is not null && !nameRegex.IsMatch(sessionName))
            {
                return true;
            }

            if (titleRegex is not null && !titleRegex.IsMatch(title))
            {
                return true;
            }

            var placement = new Win32Native.WindowPlacement { length = System.Runtime.InteropServices.Marshal.SizeOf<Win32Native.WindowPlacement>() };
            Win32Native.GetWindowPlacement(handle, ref placement);

            discovered.Add(new TargetWindowInfo(
                Handle: handle,
                Title: title,
                SourceName: sessionName,
                IsMaximized: placement.showCmd == Win32Native.SwShowMaximized,
                ProcessId: processId)
            {
                Identity = identity,
            });

            return true;
        }, nint.Zero);

        var filtered = ApplyPreferredFilter(discovered, options.PreferNames);
        var ordered = options.SortDescending
            ? filtered.OrderByDescending(static x => SessionNameParser.ParseNumericSuffix(x.SourceName))
                .ThenByDescending(static x => x.SourceName, StringComparer.OrdinalIgnoreCase)
            : filtered.OrderBy(static x => SessionNameParser.ParseNumericSuffix(x.SourceName))
                .ThenBy(static x => x.SourceName, StringComparer.OrdinalIgnoreCase);

        return Task.FromResult<IReadOnlyList<TargetWindowInfo>>(ordered.ToList());
    }

    private static IEnumerable<TargetWindowInfo> ApplyPreferredFilter(
        IReadOnlyList<TargetWindowInfo> discovered,
        IReadOnlyCollection<string>? preferNames)
    {
        IEnumerable<TargetWindowInfo> filtered = discovered;
        if (preferNames is { Count: > 0 })
        {
            var preferred = new HashSet<string>(preferNames, StringComparer.OrdinalIgnoreCase);
            filtered = filtered.Where(x => preferred.Contains(x.SourceName));
        }

        return filtered;
    }
}
