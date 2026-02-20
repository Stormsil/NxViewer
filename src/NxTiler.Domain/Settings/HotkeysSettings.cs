namespace NxTiler.Domain.Settings;

public sealed record HotkeysSettings(
    HotkeyBinding ToggleOverlay,
    HotkeyBinding ToggleMainWindow,
    HotkeyBinding CycleMode,
    HotkeyBinding ToggleMinimize,
    HotkeyBinding NavigatePrevious,
    HotkeyBinding NavigateNext,
    HotkeyBinding InstantSnapshot,
    HotkeyBinding RegionSnapshot,
    HotkeyBinding Record,
    HotkeyBinding Pause,
    HotkeyBinding Stop
)
{
    public HotkeyBinding ToggleVision { get; init; } = new(6, 115);
}
