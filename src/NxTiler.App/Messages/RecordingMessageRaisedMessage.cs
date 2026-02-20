using CommunityToolkit.Mvvm.Messaging.Messages;

namespace NxTiler.App.Messages;

public sealed class RecordingMessageRaisedMessage(string value) : ValueChangedMessage<string>(value);
