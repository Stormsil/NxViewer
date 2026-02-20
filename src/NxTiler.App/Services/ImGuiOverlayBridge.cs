using CommunityToolkit.Mvvm.Messaging;
using Microsoft.Extensions.Hosting;
using NxTiler.App.Logging;
using NxTiler.App.Messages;
using NxTiler.App.Models;
using NxTiler.Application.Abstractions;
using NxTiler.Domain.Capture;
using NxTiler.Domain.Enums;
using NxTiler.Domain.Grid;
using NxTiler.Domain.Rules;
using NxTiler.Domain.Settings;
using NxTiler.Overlay;
using NxTiler.Overlay.State;

namespace NxTiler.App.Services;

/// <summary>
/// Bridges WorkspaceSnapshot, recording state, and settings messages into ImGui OverlayState.
/// Wires all ImGui panel callbacks back to application services.
/// </summary>
public sealed class ImGuiOverlayBridge(
    IMessenger messenger,
    OverlayRenderService renderService,
    ISettingsService settingsService,
    IDashboardWorkspaceCommandService workspaceCommands,
    IDashboardRecordingCommandService recordingCommands,
    IGridPresetService gridPresetService,
    ILogBufferService logBufferService) : IHostedService
{
    private WorkspaceSnapshot? _lastSnapshot;
    private RecordingState _recordingState = RecordingState.Idle;
    private DateTimeOffset _recordingStartTime = DateTimeOffset.UtcNow;
    private bool _overlayVisible = true;

    private bool _isSettingsOpen;
    private bool _isHotkeysOpen;
    private bool _isRecordingOpen;
    private bool _isLogsOpen;
    private bool _isRulesOpen;
    private bool _isPresetsOpen;
    private bool _isGridEditorOpen;

    private readonly System.Timers.Timer _elapsedTimer = new(1000) { AutoReset = true };

    public Task StartAsync(CancellationToken cancellationToken)
    {
        var callbacks = CreateCallbacks();
        renderService.SetCallbacks(callbacks);

        _elapsedTimer.Elapsed += (_, _) =>
        {
            if (_recordingState == RecordingState.Recording)
            {
                PushState();
            }
        };
        _elapsedTimer.Start();

        messenger.Register<ImGuiOverlayBridge, WorkspaceSnapshotChangedMessage>(this, static (r, m) =>
        {
            r._lastSnapshot = m.Value;
            r.PushState();
        });

        messenger.Register<ImGuiOverlayBridge, RecordingStateChangedMessage>(this, static (r, m) =>
        {
            if (m.Value == RecordingState.Recording && r._recordingState != RecordingState.Recording)
            {
                r._recordingStartTime = DateTimeOffset.UtcNow;
            }
            else if (m.Value == RecordingState.Idle)
            {
                r._recordingStartTime = DateTimeOffset.UtcNow;
            }

            r._recordingState = m.Value;
            r.PushState();
        });

        messenger.Register<ImGuiOverlayBridge, OverlayToggleRequestedMessage>(this, static (r, _) =>
        {
            r._overlayVisible = !r._overlayVisible;
            r.PushState();
        });

        messenger.Register<ImGuiOverlayBridge, ApplicationExitRequestedMessage>(this, static (_, _) =>
        {
            System.Windows.Application.Current?.Dispatcher.InvokeAsync(
                () => System.Windows.Application.Current.Shutdown());
        });

        return Task.CompletedTask;
    }

    public Task StopAsync(CancellationToken cancellationToken)
    {
        _elapsedTimer.Stop();
        _elapsedTimer.Dispose();
        messenger.UnregisterAll(this);
        return Task.CompletedTask;
    }

    private OverlayCallbacks CreateCallbacks() => new(
        OnModeChanged: mode => _ = workspaceCommands.SetModeAsync(mode),
        OnAutoArrangeChanged: enabled => _ = workspaceCommands.SetAutoArrangeAsync(enabled),
        OnArrangeNow: () => _ = workspaceCommands.ArrangeNowAsync(),
        OnToggleMinimize: () => _ = workspaceCommands.ToggleMinimizeAllAsync(),
        OnWindowSelected: idx => _ = workspaceCommands.SelectWindowAsync(idx),

        OnOpenSettings: () => { _isSettingsOpen = true; PushState(); },
        OnOpenHotkeys: () => { _isHotkeysOpen = true; PushState(); },
        OnOpenRecording: () => { _isRecordingOpen = true; PushState(); },
        OnOpenRules: () => { _isRulesOpen = true; PushState(); },
        OnOpenPresets: () => { _isPresetsOpen = true; PushState(); },
        OnOpenLogs: () => { _isLogsOpen = true; PushState(); },

        OnSaveSettings: SaveSettings,
        OnCloseSettings: () => { _isSettingsOpen = false; PushState(); },

        OnSaveHotkeys: SaveHotkeys,
        OnCloseHotkeys: () => { _isHotkeysOpen = false; PushState(); },

        OnStartRecording: () => _ = recordingCommands.StartRecordingAsync(),
        OnPauseResume: PauseOrResume,
        OnStopAndSave: () => _ = recordingCommands.StopAsync(save: true),
        OnStopAndDiscard: () => _ = recordingCommands.StopAsync(save: false),
        OnCloseRecording: () => { _isRecordingOpen = false; PushState(); },

        OnClearLogs: () => { logBufferService.Clear(); PushState(); },
        OnCloseLogs: () => { _isLogsOpen = false; PushState(); },

        OnSaveRules: SaveRules,
        OnCloseRules: () => { _isRulesOpen = false; PushState(); },

        OnApplyPreset: id => _ = gridPresetService.ApplyPresetAsync(id, GetFocusedWindow()),
        OnDeletePreset: id => _ = gridPresetService.DeletePresetAsync(id),
        OnOpenGridEditor: () => { _isGridEditorOpen = true; PushState(); },
        OnClosePresets: () => { _isPresetsOpen = false; PushState(); },

        OnSaveGridPreset: (name, sel, dim) => SaveGridPreset(name, sel, dim),
        OnApplyGridImmediate: (_, _) => { /* TODO: apply grid to focused window */ },
        OnCloseGridEditor: () => { _isGridEditorOpen = false; PushState(); }
    );

    private void PauseOrResume()
    {
        if (_recordingState == RecordingState.Paused)
        {
            _ = recordingCommands.ResumeAsync();
        }
        else
        {
            _ = recordingCommands.PauseAsync();
        }
    }

    private nint GetFocusedWindow() => _lastSnapshot?.FocusedWindow ?? default;

    private void SaveSettings(SettingsPanelState state)
    {
        var current = settingsService.Current;
        var updated = current with
        {
            Filters = current.Filters with
            {
                TitleFilter = state.TitleFilter,
                NameFilter = state.NameFilter,
                SortDescending = state.SortDescending,
            },
            Layout = current.Layout with
            {
                Gap = state.Gap,
                TopPad = state.TopPad,
                DragCooldownMs = state.DragCooldownMs,
                SuspendOnMax = state.SuspendOnMax,
            },
            Paths = current.Paths with
            {
                NxsFolder = state.NxsFolder,
                RecordingFolder = state.RecordingFolder,
                FfmpegPath = state.FfmpegPath,
            },
            Recording = current.Recording with { Fps = state.RecordingFps },
            FeatureFlags = current.FeatureFlags with
            {
                EnableTemplateMatchingFallback = state.EnableTemplateMatchingFallback,
                EnableYoloEngine = state.EnableYoloEngine,
            },
        };

        settingsService.Update(updated);
        _ = settingsService.SaveAsync();
        _isSettingsOpen = false;
        PushState();
    }

    private void SaveHotkeys(HotkeysPanelState state)
    {
        var current = settingsService.Current;
        var updated = current with
        {
            Hotkeys = new HotkeysSettings(
                ToggleOverlay: state.ToggleOverlay,
                ToggleMainWindow: state.ToggleMainWindow,
                CycleMode: state.CycleMode,
                ToggleMinimize: state.ToggleMinimize,
                NavigatePrevious: state.NavigatePrevious,
                NavigateNext: state.NavigateNext,
                InstantSnapshot: state.InstantSnapshot,
                RegionSnapshot: state.RegionSnapshot,
                Record: state.Record,
                Pause: state.Pause,
                Stop: state.Stop)
            {
                ToggleVision = state.ToggleVision,
            },
        };

        settingsService.Update(updated);
        _ = settingsService.SaveAsync();
        _isHotkeysOpen = false;
        PushState();
    }

    private void SaveRules(WindowRulesSettings rules)
    {
        var current = settingsService.Current;
        var updated = current with { Rules = rules };
        settingsService.Update(updated);
        _ = settingsService.SaveAsync();
        _isRulesOpen = false;
        PushState();
    }

    private void SaveGridPreset(string name, GridCellSelection selection, GridDimensions dimensions)
    {
        var preset = new GridPreset(Guid.NewGuid().ToString(), name, dimensions, selection);
        _ = gridPresetService.SavePresetAsync(preset);
        _isGridEditorOpen = false;
        PushState();
    }

    private void PushState()
    {
        var snap = _lastSnapshot;
        var current = settingsService.Current;

        IReadOnlyList<OverlayWindowItem> windows = snap is null
            ? Array.Empty<OverlayWindowItem>()
            : snap.Windows
                .Select((w, i) => new OverlayWindowItem(w.Handle, w.SourceName, i == snap.ActiveIndex))
                .ToList();

        var recordingElapsed = _recordingState == RecordingState.Recording
            ? DateTimeOffset.UtcNow - _recordingStartTime
            : TimeSpan.Zero;

        var settingsState = new SettingsPanelState(
            TitleFilter: current.Filters.TitleFilter,
            NameFilter: current.Filters.NameFilter,
            SortDescending: current.Filters.SortDescending,
            Gap: current.Layout.Gap,
            TopPad: current.Layout.TopPad,
            DragCooldownMs: current.Layout.DragCooldownMs,
            SuspendOnMax: current.Layout.SuspendOnMax,
            NxsFolder: current.Paths.NxsFolder,
            RecordingFolder: current.Paths.RecordingFolder,
            FfmpegPath: current.Paths.FfmpegPath,
            RecordingFps: current.Recording.Fps,
            EnableTemplateMatchingFallback: current.FeatureFlags.EnableTemplateMatchingFallback,
            EnableYoloEngine: current.FeatureFlags.EnableYoloEngine);

        var hotkeys = current.Hotkeys;
        var hotkeysState = new HotkeysPanelState(
            ToggleOverlay: hotkeys.ToggleOverlay,
            ToggleMainWindow: hotkeys.ToggleMainWindow,
            CycleMode: hotkeys.CycleMode,
            ToggleMinimize: hotkeys.ToggleMinimize,
            NavigatePrevious: hotkeys.NavigatePrevious,
            NavigateNext: hotkeys.NavigateNext,
            InstantSnapshot: hotkeys.InstantSnapshot,
            RegionSnapshot: hotkeys.RegionSnapshot,
            Record: hotkeys.Record,
            Pause: hotkeys.Pause,
            Stop: hotkeys.Stop,
            ToggleVision: hotkeys.ToggleVision);

        var logEntries = logBufferService.GetSnapshot()
            .TakeLast(2000)
            .Select(e => new LogEntryItem(e.Timestamp, e.Level.ToString(), e.Message, e.ExceptionText))
            .ToList();

        var logsState = new LogsPanelState(logEntries, string.Empty, 0);

        var rules = current.Rules?.Rules ?? Array.Empty<WindowRule>();
        var presets = gridPresetService.Presets;

        var state = new OverlayState(
            Mode: snap?.Mode ?? TileMode.Grid,
            AutoArrange: snap?.AutoArrangeEnabled ?? false,
            IsVisible: _overlayVisible,
            Windows: windows,
            FocusedIndex: snap?.ActiveIndex ?? -1,
            Recording: _recordingState,
            RecordingElapsed: recordingElapsed,
            IsGridEditorVisible: _isGridEditorOpen,
            IsDimMaskVisible: false,
            Masks: Array.Empty<CaptureMask>(),
            GridEditor: GridEditorState.Default,
            IsSettingsPanelOpen: _isSettingsOpen,
            IsHotkeysPanelOpen: _isHotkeysOpen,
            IsRecordingPanelOpen: _isRecordingOpen,
            IsLogsPanelOpen: _isLogsOpen,
            IsRulesPanelOpen: _isRulesOpen,
            IsPresetsPanelOpen: _isPresetsOpen,
            Settings: settingsState,
            Hotkeys: hotkeysState,
            Logs: logsState,
            Rules: rules,
            GridPresets: presets);

        renderService.SetState(state);
    }
}
