using NxTiler.Domain.Enums;
using NxTiler.Domain.Grid;
using NxTiler.Domain.Rules;
using NxTiler.Domain.Settings;
using NxTiler.Overlay.State;

namespace NxTiler.Overlay;

public sealed record OverlayCallbacks(
    // Control panel
    Action<TileMode> OnModeChanged,
    Action<bool> OnAutoArrangeChanged,
    Action OnArrangeNow,
    Action OnToggleMinimize,
    Action<int> OnWindowSelected,
    Action OnOpenSettings,
    Action OnOpenHotkeys,
    Action OnOpenRecording,
    Action OnOpenRules,
    Action OnOpenPresets,
    Action OnOpenLogs,
    // Settings panel
    Action<SettingsPanelState> OnSaveSettings,
    Action OnCloseSettings,
    // Hotkeys panel
    Action<HotkeysPanelState> OnSaveHotkeys,
    Action OnCloseHotkeys,
    // Recording panel
    Action OnStartRecording,
    Action OnPauseResume,
    Action OnStopAndSave,
    Action OnStopAndDiscard,
    Action OnCloseRecording,
    // Logs panel
    Action OnClearLogs,
    Action OnCloseLogs,
    // Rules panel
    Action<WindowRulesSettings> OnSaveRules,
    Action OnCloseRules,
    // Presets panel
    Action<string> OnApplyPreset,
    Action<string> OnDeletePreset,
    Action OnOpenGridEditor,
    Action OnClosePresets,
    // Grid editor
    Action<string, GridCellSelection, GridDimensions> OnSaveGridPreset,
    Action<GridCellSelection, GridDimensions> OnApplyGridImmediate,
    Action OnCloseGridEditor);
