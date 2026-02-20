using NxTiler.Domain.Settings;

namespace NxTiler.Overlay.State;

public sealed record HotkeysPanelState(
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
    HotkeyBinding Stop,
    HotkeyBinding ToggleVision)
{
    public static readonly HotkeysPanelState Empty = new(
        HotkeyBinding.Empty, HotkeyBinding.Empty, HotkeyBinding.Empty,
        HotkeyBinding.Empty, HotkeyBinding.Empty, HotkeyBinding.Empty,
        HotkeyBinding.Empty, HotkeyBinding.Empty, HotkeyBinding.Empty,
        HotkeyBinding.Empty, HotkeyBinding.Empty, HotkeyBinding.Empty);
}
