using NxTiler.Domain.Capture;
using NxTiler.Domain.Enums;
using NxTiler.Domain.Grid;
using NxTiler.Domain.Rules;

namespace NxTiler.Overlay.State;

public sealed record OverlayState(
    TileMode Mode,
    bool AutoArrange,
    bool IsVisible,
    IReadOnlyList<OverlayWindowItem> Windows,
    int FocusedIndex,
    RecordingState Recording,
    TimeSpan RecordingElapsed,
    bool IsGridEditorVisible,
    bool IsDimMaskVisible,
    IReadOnlyList<CaptureMask> Masks,
    GridEditorState GridEditor,
    bool IsSettingsPanelOpen,
    bool IsHotkeysPanelOpen,
    bool IsRecordingPanelOpen,
    bool IsLogsPanelOpen,
    bool IsRulesPanelOpen,
    bool IsPresetsPanelOpen,
    SettingsPanelState Settings,
    HotkeysPanelState Hotkeys,
    LogsPanelState Logs,
    IReadOnlyList<WindowRule> Rules,
    IReadOnlyList<GridPreset> GridPresets)
{
    public static readonly OverlayState Empty = new(
        Mode: TileMode.Grid,
        AutoArrange: false,
        IsVisible: false,
        Windows: Array.Empty<OverlayWindowItem>(),
        FocusedIndex: -1,
        Recording: RecordingState.Idle,
        RecordingElapsed: TimeSpan.Zero,
        IsGridEditorVisible: false,
        IsDimMaskVisible: false,
        Masks: Array.Empty<CaptureMask>(),
        GridEditor: GridEditorState.Default,
        IsSettingsPanelOpen: false,
        IsHotkeysPanelOpen: false,
        IsRecordingPanelOpen: false,
        IsLogsPanelOpen: false,
        IsRulesPanelOpen: false,
        IsPresetsPanelOpen: false,
        Settings: SettingsPanelState.Empty,
        Hotkeys: HotkeysPanelState.Empty,
        Logs: LogsPanelState.Empty,
        Rules: Array.Empty<WindowRule>(),
        GridPresets: Array.Empty<GridPreset>());
}
