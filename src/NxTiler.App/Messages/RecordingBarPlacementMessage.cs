using CommunityToolkit.Mvvm.Messaging.Messages;
using NxTiler.Domain.Windowing;

namespace NxTiler.App.Messages;

public sealed class RecordingBarPlacementMessage(WindowBounds value) : ValueChangedMessage<WindowBounds>(value);
