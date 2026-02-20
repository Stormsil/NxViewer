using CommunityToolkit.Mvvm.Messaging.Messages;
using NxTiler.Domain.Enums;

namespace NxTiler.App.Messages;

public sealed class RecordingStateChangedMessage(RecordingState value) : ValueChangedMessage<RecordingState>(value);
