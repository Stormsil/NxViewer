using NxTiler.Domain.Windowing;

namespace NxTiler.App.Services;

public sealed partial class WorkspaceOrchestrator
{
    private async Task RefreshTargetsAsync(CancellationToken ct)
    {
        _targets.Clear();
        var queried = await _windowQueryService.QueryAsync(CreateWindowQueryOptions(), ct);

        IReadOnlyList<TargetWindowInfo> candidates;
        var useRules = _windowRulesEngine is not null
            && _settingsService.Current.FeatureFlags.EnableRulesEngine;

        if (useRules)
        {
            var evaluation = _windowRulesEngine!.EvaluateAll(queried);

            // Store group windows for arrangement (preserves full TargetWindowInfo)
            _groupWindows = evaluation.Groups;

            // Route group windows to the group service for external consumers (e.g. UI views)
            if (_windowGroupService is not null)
            {
                // Clear assignments for windows no longer visible
                var allHandles = queried.Select(static x => x.Handle).ToHashSet();
                foreach (var handle in _windowGroupService.Groups.Values
                    .SelectMany(static g => g.Windows)
                    .Select(static w => w.Handle)
                    .Where(h => !allHandles.Contains(h))
                    .ToList())
                {
                    _windowGroupService.RemoveWindow(handle);
                }

                foreach (var (groupId, groupWindowsList) in evaluation.Groups)
                {
                    foreach (var window in groupWindowsList)
                    {
                        _windowGroupService.AssignWindow(window.Handle, groupId);
                    }
                }
            }

            candidates = evaluation.Included;
        }
        else
        {
            _groupWindows = new Dictionary<string, IReadOnlyList<TargetWindowInfo>>();
            candidates = queried;
        }

        _targets.AddRange(candidates);
        await _windowEventMonitorService.UpdateTrackedWindowsAsync(_targets.Select(static x => x.Handle).ToArray(), ct);
    }

    private WindowQueryOptions CreateWindowQueryOptions()
    {
        return new WindowQueryOptions(
            SelfProcessId: Environment.ProcessId,
            TitleFilter: _settingsService.Current.Filters.TitleFilter,
            SessionNameFilter: _settingsService.Current.Filters.NameFilter,
            SortDescending: _settingsService.Current.Filters.SortDescending);
    }
}
