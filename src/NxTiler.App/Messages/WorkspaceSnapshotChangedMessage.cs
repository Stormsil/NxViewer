using CommunityToolkit.Mvvm.Messaging.Messages;
using NxTiler.App.Models;

namespace NxTiler.App.Messages;

public sealed class WorkspaceSnapshotChangedMessage(WorkspaceSnapshot value) : ValueChangedMessage<WorkspaceSnapshot>(value);
