using CommunityToolkit.Mvvm.Messaging;
using NxTiler.App.Messages;
using NxTiler.App.Models;

namespace NxTiler.App.Services;

public sealed partial class WorkspaceOrchestrator
{
    private void PublishSnapshot()
    {
        var activeIndex = -1;
        if (_focusedWindow != nint.Zero)
        {
            activeIndex = _targets.FindIndex(x => x.Handle == _focusedWindow);
        }

        Snapshot = new WorkspaceSnapshot(
            Windows: _targets.ToList(),
            Mode: _mode,
            AutoArrangeEnabled: _isAutoArrangeEnabled,
            AllMinimized: _allMinimized,
            IsForeignAppActive: _isForeignAppActive,
            FocusedWindow: _focusedWindow,
            ActiveIndex: activeIndex);

        _messenger.Send(new WorkspaceSnapshotChangedMessage(Snapshot));
    }

    private void RaiseStatus(string status)
    {
        _messenger.Send(new WorkspaceStatusChangedMessage(status));
    }
}
