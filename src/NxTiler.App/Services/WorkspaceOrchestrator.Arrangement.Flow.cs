using NxTiler.Domain.Enums;
using NxTiler.Domain.Rules;
using NxTiler.Domain.Windowing;

namespace NxTiler.App.Services;

public sealed partial class WorkspaceOrchestrator
{
    private async Task SmartArrangeAsync()
    {
        if (!_isAutoArrangeEnabled)
        {
            return;
        }

        if (_recordingWorkflowService.State is RecordingState.Recording or RecordingState.Paused or RecordingState.Saving)
        {
            return;
        }

        if (_isForeignAppActive)
        {
            await RefreshAsync();
            return;
        }

        await RefreshAsync();
        if (_settingsService.Current.Layout.SuspendOnMax && _targets.Any(static x => x.IsMaximized))
        {
            RaiseStatus("Paused: maximized window detected.");
            return;
        }

        await ArrangeUsingCurrentTargetsAsync();
    }

    private async Task ArrangeUsingCurrentTargetsAsync(CancellationToken ct = default)
    {
        await _arrangementGate.WaitAsync(ct);
        try
        {
            await ArrangeInternalAsync(ct);
        }
        finally
        {
            _arrangementGate.Release();
        }
    }

    private async Task ArrangeInternalAsync(CancellationToken ct)
    {
        var candidates = _targets.Where(static x => !x.IsMaximized).ToList();
        if (candidates.Count == 0)
        {
            RaiseStatus("Nothing to arrange.");
            PublishSnapshot();
            return;
        }

        if (_mode != TileMode.Grid && (_focusedWindow == nint.Zero || candidates.All(x => x.Handle != _focusedWindow)))
        {
            _focusedWindow = candidates[0].Handle;
        }

        var areaSource = _mode == TileMode.MaxSize
            ? (_focusedWindow == nint.Zero ? candidates[0].Handle : _focusedWindow)
            : candidates[0].Handle;
        var workArea = await _windowControlService.GetWorkAreaForWindowAsync(areaSource, ct);

        var context = new ArrangementContext(
            Mode: _mode,
            WorkArea: workArea,
            Gap: _settingsService.Current.Layout.Gap,
            TopPad: _settingsService.Current.Layout.TopPad,
            FocusedWindow: _focusedWindow == nint.Zero ? null : _focusedWindow);

        var placements = _arrangementService.BuildPlacements(candidates, _mode, context);
        await _windowControlService.ApplyPlacementsAsync(placements, ct);

        // Arrange each window group independently with its own TileMode
        var totalGrouped = 0;
        if (_groupWindows.Count > 0)
        {
            var groupConfigs = _settingsService.Current.Groups.Groups
                .ToDictionary(static g => g.Id, StringComparer.Ordinal);

            foreach (var (groupId, groupWindowsList) in _groupWindows)
            {
                var groupCandidates = groupWindowsList.Where(static x => !x.IsMaximized).ToList();
                if (groupCandidates.Count == 0)
                {
                    continue;
                }

                groupConfigs.TryGetValue(groupId, out var groupConfig);
                var groupMode = groupConfig?.TileMode ?? TileMode.Grid;

                var groupArea = await _windowControlService.GetWorkAreaForWindowAsync(groupCandidates[0].Handle, ct);
                var groupContext = new ArrangementContext(
                    Mode: groupMode,
                    WorkArea: groupArea,
                    Gap: _settingsService.Current.Layout.Gap,
                    TopPad: _settingsService.Current.Layout.TopPad,
                    FocusedWindow: null);

                var groupPlacements = _arrangementService.BuildPlacements(groupCandidates, groupMode, groupContext);
                await _windowControlService.ApplyPlacementsAsync(groupPlacements, ct);
                totalGrouped += groupPlacements.Count;
            }
        }

        RaiseStatus($"Mode: {_mode}. Arranged {placements.Count} windows" +
            (totalGrouped > 0 ? $" + {totalGrouped} in groups." : "."));
        PublishSnapshot();
    }
}
