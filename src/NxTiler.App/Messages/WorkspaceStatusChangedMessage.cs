using CommunityToolkit.Mvvm.Messaging.Messages;

namespace NxTiler.App.Messages;

public sealed class WorkspaceStatusChangedMessage(string value) : ValueChangedMessage<string>(value);
