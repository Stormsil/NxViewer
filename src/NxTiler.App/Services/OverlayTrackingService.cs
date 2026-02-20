using Microsoft.Extensions.Logging;
using NxTiler.Application.Abstractions;
using NxTiler.Domain.Enums;
using NxTiler.Domain.Overlay;
using NxTiler.Domain.Windowing;

namespace NxTiler.App.Services;

public sealed partial class OverlayTrackingService(
    IWindowControlService windowControlService,
    ICursorPositionProvider cursorPositionProvider,
    ILogger<OverlayTrackingService> logger) : IOverlayTrackingService
{
    private static readonly TimeSpan DefaultPollInterval = TimeSpan.FromMilliseconds(60);

    private readonly SemaphoreSlim _gate = new(1, 1);

    private CancellationTokenSource? _loopCts;
    private Task? _loopTask;
    private nint _targetWindow;
    private OverlayTrackingRequest? _request;
    private WindowBounds _baselineWindowBounds = new(0, 0, 0, 0);
    private OverlayTrackingState? _lastState;

    public event EventHandler<OverlayTrackingState>? TrackingStateChanged;
}
