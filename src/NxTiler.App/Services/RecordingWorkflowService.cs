using Microsoft.Extensions.Logging;
using CommunityToolkit.Mvvm.Messaging;
using NxTiler.App.Messages;
using NxTiler.Application.Abstractions;
using NxTiler.Domain.Enums;
using NxTiler.Domain.Windowing;

namespace NxTiler.App.Services;

public sealed partial class RecordingWorkflowService : IRecordingWorkflowService
{
    private readonly IWindowQueryService _windowQueryService;
    private readonly IWindowControlService _windowControlService;
    private readonly IRecordingOverlayService _recordingOverlayService;
    private readonly ISettingsService _settingsService;
    private readonly IFfmpegSetupService _ffmpegSetupService;
    private readonly IRecordingEngine _recordingEngine;
    private readonly IVideoRecordingEngine? _videoRecordingEngine;
    private readonly IMessenger _messenger;
    private readonly ILogger<RecordingWorkflowService> _logger;
    private readonly SemaphoreSlim _workflowGate = new(1, 1);
    private readonly RecordingWorkflowStateMachine _stateMachine = new();

    private nint _recordingTargetWindow = nint.Zero;
    private WindowBounds? _recordingMonitorBounds;
    private WindowBounds? _recordingBounds;
    private bool _useVideoEngine;

    public RecordingWorkflowService(
        IWindowQueryService windowQueryService,
        IWindowControlService windowControlService,
        IRecordingOverlayService recordingOverlayService,
        ISettingsService settingsService,
        IFfmpegSetupService ffmpegSetupService,
        IRecordingEngine recordingEngine,
        ILogger<RecordingWorkflowService> logger,
        IMessenger? messenger = null,
        IVideoRecordingEngine? videoRecordingEngine = null)
    {
        _windowQueryService = windowQueryService;
        _windowControlService = windowControlService;
        _recordingOverlayService = recordingOverlayService;
        _settingsService = settingsService;
        _ffmpegSetupService = ffmpegSetupService;
        _recordingEngine = recordingEngine;
        _videoRecordingEngine = videoRecordingEngine;
        _messenger = messenger ?? WeakReferenceMessenger.Default;
        _logger = logger;

        _messenger.Register<RecordingWorkflowService, RecordingOverlayConfirmRequestedMessage>(this, static (recipient, _) =>
        {
            recipient.HandleOverlayConfirmRequested();
        });
        _messenger.Register<RecordingWorkflowService, RecordingOverlayCancelRequestedMessage>(this, static (recipient, _) =>
        {
            recipient.HandleOverlayCancelRequested();
        });
    }

    public RecordingState State => _stateMachine.State;

    public WindowBounds? ActiveCaptureBounds => _recordingBounds;

    private void Reset()
    {
        _recordingTargetWindow = nint.Zero;
        _recordingMonitorBounds = null;
        _recordingBounds = null;
        _useVideoEngine = false;
    }

    private async Task<TargetWindowInfo?> ResolveRecordingTargetAsync(CancellationToken ct)
    {
        var settings = _settingsService.Current;
        var options = new WindowQueryOptions(
            SelfProcessId: Environment.ProcessId,
            TitleFilter: settings.Filters.TitleFilter,
            SessionNameFilter: settings.Filters.NameFilter,
            SortDescending: settings.Filters.SortDescending);

        var windows = await _windowQueryService.QueryAsync(options, ct);
        var candidates = windows.Where(static x => !x.IsMaximized).ToList();
        if (candidates.Count > 0)
        {
            return candidates[0];
        }

        return windows.FirstOrDefault();
    }
}
