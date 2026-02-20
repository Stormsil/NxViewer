using System.Windows.Threading;
using CommunityToolkit.Mvvm.Messaging;
using Microsoft.Extensions.Logging;
using NxTiler.App.Models;
using NxTiler.Application.Abstractions;
using NxTiler.Domain.Enums;
using NxTiler.Domain.Windowing;

namespace NxTiler.App.Services;

public sealed partial class WorkspaceOrchestrator : IWorkspaceOrchestrator
{
    private readonly IWindowQueryService _windowQueryService;
    private readonly IArrangementService _arrangementService;
    private readonly IWindowControlService _windowControlService;
    private readonly IWindowEventMonitorService _windowEventMonitorService;
    private readonly INomachineSessionService _nomachineSessionService;
    private readonly ISettingsService _settingsService;
    private readonly IHotkeyService _hotkeyService;
    private readonly ITrayService _trayService;
    private readonly ICaptureWorkflowService? _captureWorkflowService;
    private readonly IVisionWorkflowService? _visionWorkflowService;
    private readonly IRecordingWorkflowService _recordingWorkflowService;
    private readonly IWindowRulesEngine? _windowRulesEngine;
    private readonly IWindowGroupService? _windowGroupService;
    private readonly IGridPresetService? _gridPresetService;
    private readonly IMessenger _messenger;
    private readonly ILogger<WorkspaceOrchestrator> _logger;
    private readonly DispatcherTimer _autoArrangeTimer = new() { Interval = TimeSpan.FromSeconds(5) };
    private readonly SemaphoreSlim _arrangementGate = new(1, 1);
    private readonly SemaphoreSlim _lifecycleGate = new(1, 1);

    private readonly List<TargetWindowInfo> _targets = [];
    private IReadOnlyDictionary<string, IReadOnlyList<TargetWindowInfo>> _groupWindows =
        new Dictionary<string, IReadOnlyList<TargetWindowInfo>>();

    private bool _isStarted;
    private bool _isAutoArrangeEnabled;
    private bool _isWindowMonitorActive;
    private bool _allMinimized;
    private bool _isForeignAppActive;
    private bool _isDisposed;
    private TileMode _mode = TileMode.Grid;
    private nint _focusedWindow = nint.Zero;
    private DateTime _lastModeSwitch = DateTime.MinValue;

    public WorkspaceOrchestrator(
        IWindowQueryService windowQueryService,
        IArrangementService arrangementService,
        IWindowControlService windowControlService,
        IWindowEventMonitorService windowEventMonitorService,
        INomachineSessionService nomachineSessionService,
        ISettingsService settingsService,
        IHotkeyService hotkeyService,
        ITrayService trayService,
        IRecordingWorkflowService recordingWorkflowService,
        ILogger<WorkspaceOrchestrator> logger,
        IMessenger? messenger = null,
        ICaptureWorkflowService? captureWorkflowService = null,
        IVisionWorkflowService? visionWorkflowService = null,
        IWindowRulesEngine? windowRulesEngine = null,
        IWindowGroupService? windowGroupService = null,
        IGridPresetService? gridPresetService = null)
    {
        _windowQueryService = windowQueryService;
        _arrangementService = arrangementService;
        _windowControlService = windowControlService;
        _windowEventMonitorService = windowEventMonitorService;
        _nomachineSessionService = nomachineSessionService;
        _settingsService = settingsService;
        _hotkeyService = hotkeyService;
        _trayService = trayService;
        _captureWorkflowService = captureWorkflowService;
        _visionWorkflowService = visionWorkflowService;
        _recordingWorkflowService = recordingWorkflowService;
        _windowRulesEngine = windowRulesEngine;
        _windowGroupService = windowGroupService;
        _gridPresetService = gridPresetService;
        _messenger = messenger ?? WeakReferenceMessenger.Default;
        _logger = logger;

        _autoArrangeTimer.Tick += AutoArrangeTimerOnTick;
    }

    public WorkspaceSnapshot Snapshot { get; private set; } = new(
        Windows: Array.Empty<TargetWindowInfo>(),
        Mode: TileMode.Grid,
        AutoArrangeEnabled: false,
        AllMinimized: false,
        IsForeignAppActive: false,
        FocusedWindow: nint.Zero,
        ActiveIndex: -1);
}
