namespace NxTiler.Domain.Settings;

public sealed partial record AppSettingsSnapshot
{
    private static HotkeysSettings CreateDefaultHotkeysSettings()
    {
        return new HotkeysSettings(
            ToggleOverlay: new HotkeyBinding(0, 112),
            ToggleMainWindow: new HotkeyBinding(2, 112),
            CycleMode: new HotkeyBinding(4, 112),
            ToggleMinimize: new HotkeyBinding(0, 192),
            NavigatePrevious: new HotkeyBinding(0, 37),
            NavigateNext: new HotkeyBinding(0, 39),
            InstantSnapshot: new HotkeyBinding(6, 113),
            RegionSnapshot: new HotkeyBinding(6, 114),
            Record: new HotkeyBinding(0, 113),
            Pause: new HotkeyBinding(0, 114),
            Stop: new HotkeyBinding(0, 115))
        {
            ToggleVision = new HotkeyBinding(6, 115),
        };
    }
}
