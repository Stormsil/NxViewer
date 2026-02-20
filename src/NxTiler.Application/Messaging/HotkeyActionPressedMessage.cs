using NxTiler.Domain.Enums;

namespace NxTiler.Application.Messaging;

public sealed record HotkeyActionPressedMessage(HotkeyAction Action);
