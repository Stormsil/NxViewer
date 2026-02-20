using CommunityToolkit.Mvvm.Messaging;
using NxTiler.App.Messages;
using NxTiler.Application.Messaging;

namespace NxTiler.App.Services;

public sealed partial class WorkspaceOrchestrator
{
    private void RegisterMessageHandlers()
    {
        _messenger.Register<WorkspaceOrchestrator, HotkeyActionPressedMessage>(this, static (recipient, message) =>
        {
            recipient.HotkeyServiceOnHotkeyPressed(message.Action);
        });

        _messenger.Register<WorkspaceOrchestrator, TrayShowRequestedMessage>(this, static (recipient, _) =>
        {
            recipient.TrayShowRequested();
        });

        _messenger.Register<WorkspaceOrchestrator, TrayArrangeRequestedMessage>(this, static (recipient, _) =>
        {
            recipient.TrayArrangeRequested();
        });

        _messenger.Register<WorkspaceOrchestrator, TrayAutoArrangeToggledMessage>(this, static (recipient, message) =>
        {
            recipient.TrayAutoArrangeToggled(message.Enabled);
        });

        _messenger.Register<WorkspaceOrchestrator, TrayExitRequestedMessage>(this, static (recipient, _) =>
        {
            recipient.TrayExitRequested();
        });

        _messenger.Register<WorkspaceOrchestrator, WindowArrangeNeededMessage>(this, static (recipient, _) =>
        {
            recipient.WindowEventMonitorArrangeNeeded();
        });

        _messenger.Register<WorkspaceOrchestrator, WindowForegroundChangedMessage>(this, static (recipient, message) =>
        {
            recipient.WindowEventMonitorForegroundChanged(message.Handle);
        });
    }
}
