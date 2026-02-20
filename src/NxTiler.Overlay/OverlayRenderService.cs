using NxTiler.Application.Abstractions;
using NxTiler.Overlay.Panels;
using NxTiler.Overlay.State;

namespace NxTiler.Overlay;

/// <summary>
/// Thread-safe ImGui overlay render service. State is passed via Interlocked.Exchange.
/// Render happens on the ImGui thread; SetState is safe to call from any thread.
/// </summary>
public sealed class OverlayRenderService : IOverlayRenderService
{
    private volatile OverlayState _state = OverlayState.Empty;
    private bool _visible = true;

    private readonly ControlPanelRenderer _controlPanel = new();
    private readonly GridEditorRenderer _gridEditor = new();
    private readonly RecordingBarRenderer _recordingBar = new();
    private readonly DimMaskRenderer _dimMask = new();
    private readonly SettingsPanelRenderer _settingsPanel = new();
    private readonly HotkeysPanelRenderer _hotkeysPanel = new();
    private readonly RecordingPanelRenderer _recordingPanel = new();
    private readonly LogsPanelRenderer _logsPanel = new();
    private readonly RulesPanelRenderer _rulesPanel = new();
    private readonly PresetsPanelRenderer _presetsPanel = new();

    public void SetCallbacks(OverlayCallbacks cb)
    {
        _controlPanel.OnModeChanged = cb.OnModeChanged;
        _controlPanel.OnAutoArrangeChanged = cb.OnAutoArrangeChanged;
        _controlPanel.OnWindowSelected = cb.OnWindowSelected;
        _controlPanel.OnArrangeNow = cb.OnArrangeNow;
        _controlPanel.OnToggleMinimize = cb.OnToggleMinimize;
        _controlPanel.OnOpenSettings = cb.OnOpenSettings;
        _controlPanel.OnOpenHotkeys = cb.OnOpenHotkeys;
        _controlPanel.OnOpenRecording = cb.OnOpenRecording;
        _controlPanel.OnOpenRules = cb.OnOpenRules;
        _controlPanel.OnOpenPresets = cb.OnOpenPresets;
        _controlPanel.OnOpenLogs = cb.OnOpenLogs;

        _gridEditor.OnSavePreset = cb.OnSaveGridPreset;
        _gridEditor.OnApplyImmediate = cb.OnApplyGridImmediate;
        _gridEditor.OnClose = cb.OnCloseGridEditor;

        _settingsPanel.OnSave = cb.OnSaveSettings;
        _settingsPanel.OnClose = cb.OnCloseSettings;

        _hotkeysPanel.OnSave = cb.OnSaveHotkeys;
        _hotkeysPanel.OnClose = cb.OnCloseHotkeys;

        _recordingPanel.OnStart = cb.OnStartRecording;
        _recordingPanel.OnPauseResume = cb.OnPauseResume;
        _recordingPanel.OnStopAndSave = cb.OnStopAndSave;
        _recordingPanel.OnStopAndDiscard = cb.OnStopAndDiscard;
        _recordingPanel.OnClose = cb.OnCloseRecording;

        _logsPanel.OnClear = cb.OnClearLogs;
        _logsPanel.OnClose = cb.OnCloseLogs;

        _rulesPanel.OnSave = cb.OnSaveRules;
        _rulesPanel.OnClose = cb.OnCloseRules;

        _presetsPanel.OnApplyPreset = cb.OnApplyPreset;
        _presetsPanel.OnDeletePreset = cb.OnDeletePreset;
        _presetsPanel.OnOpenGridEditor = cb.OnOpenGridEditor;
        _presetsPanel.OnClose = cb.OnClosePresets;
    }

    public void SetState(object state)
    {
        if (state is OverlayState overlayState)
        {
            Interlocked.Exchange(ref _state, overlayState);
        }
    }

    public void ShowPanel() => _visible = true;

    public void HidePanel() => _visible = false;

    public void RenderFrame()
    {
        if (!_visible)
        {
            return;
        }

        var s = _state;

        _controlPanel.Render(s);

        if (s.IsGridEditorVisible)
        {
            _gridEditor.Render(s);
        }

        if (s.Recording != NxTiler.Domain.Enums.RecordingState.Idle)
        {
            _recordingBar.Render(s);
        }

        if (s.IsDimMaskVisible)
        {
            _dimMask.Render(s);
        }

        if (s.IsSettingsPanelOpen)
        {
            _settingsPanel.Render(s);
        }

        if (s.IsHotkeysPanelOpen)
        {
            _hotkeysPanel.Render(s);
        }

        if (s.IsRecordingPanelOpen)
        {
            _recordingPanel.Render(s);
        }

        if (s.IsLogsPanelOpen)
        {
            _logsPanel.Render(s);
        }

        if (s.IsRulesPanelOpen)
        {
            _rulesPanel.Render(s);
        }

        if (s.IsPresetsPanelOpen)
        {
            _presetsPanel.Render(s);
        }
    }
}
