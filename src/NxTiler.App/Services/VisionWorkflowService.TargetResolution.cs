using NxTiler.Domain.Windowing;

namespace NxTiler.App.Services;

public sealed partial class VisionWorkflowService
{
    private async Task<nint> ResolveTargetWindowAsync(nint preferredTargetWindow, CancellationToken ct)
    {
        if (preferredTargetWindow != nint.Zero)
        {
            return preferredTargetWindow;
        }

        var settings = settingsService.Current;
        var options = new WindowQueryOptions(
            SelfProcessId: Environment.ProcessId,
            TitleFilter: settings.Filters.TitleFilter,
            SessionNameFilter: settings.Filters.NameFilter,
            SortDescending: settings.Filters.SortDescending);

        var windows = await windowQueryService.QueryAsync(options, ct);
        var candidate = windows.FirstOrDefault(static x => !x.IsMaximized) ?? windows.FirstOrDefault();
        return candidate?.Handle ?? nint.Zero;
    }
}
