using CommunityToolkit.Mvvm.Messaging.Messages;

namespace NxTiler.App.Messages;

public sealed class RecordingBarVisibilityMessage(bool value) : ValueChangedMessage<bool>(value);
