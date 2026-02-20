namespace NxTiler.Domain.Settings;

public sealed record HotkeyBinding(uint Modifiers, int VirtualKey)
{
    public bool IsEmpty => VirtualKey == 0;

    public static HotkeyBinding Empty { get; } = new(0, 0);
}
