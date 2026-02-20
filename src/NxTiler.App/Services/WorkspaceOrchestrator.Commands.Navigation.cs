using CommunityToolkit.Mvvm.Messaging;
using NxTiler.App.Messages;
using NxTiler.Domain.Enums;

namespace NxTiler.App.Services;

public sealed partial class WorkspaceOrchestrator
{
    public async Task NavigateAsync(int delta, CancellationToken ct = default)
    {
        await RefreshAsync(ct);

        var candidates = _targets.Where(static x => !x.IsMaximized).ToList();
        if (candidates.Count == 0)
        {
            return;
        }

        var currentIndex = 0;
        if (_focusedWindow != nint.Zero)
        {
            var current = candidates.FindIndex(x => x.Handle == _focusedWindow);
            if (current >= 0)
            {
                currentIndex = current;
            }
        }

        var nextIndex = (currentIndex + delta + candidates.Count) % candidates.Count;
        _focusedWindow = candidates[nextIndex].Handle;
        _lastModeSwitch = DateTime.UtcNow;

        if (_mode != TileMode.Grid)
        {
            await _windowControlService.BringToForegroundAsync(_focusedWindow, ct);
        }

        await ArrangeUsingCurrentTargetsAsync(ct);
    }

    public async Task SelectWindowAsync(int index, CancellationToken ct = default)
    {
        await RefreshAsync(ct);
        if (index < 0 || index >= _targets.Count)
        {
            return;
        }

        _focusedWindow = _targets[index].Handle;
        await _windowControlService.BringToForegroundAsync(_focusedWindow, ct);

        await ArrangeUsingCurrentTargetsAsync(ct);
    }

    public async Task ToggleMinimizeAllAsync(CancellationToken ct = default)
    {
        await RefreshAsync(ct);
        var handles = _targets.Select(static x => x.Handle).ToArray();
        _allMinimized = !_allMinimized;

        if (_allMinimized)
        {
            await _windowControlService.MinimizeAllAsync(handles, ct);
            PublishSnapshot();
        }
        else
        {
            await _windowControlService.RestoreAllAsync(handles, ct);
            await ArrangeUsingCurrentTargetsAsync(ct);
        }
    }

    public void RequestMainWindowToggle()
    {
        _messenger.Send(new MainWindowToggleRequestedMessage());
    }
}
