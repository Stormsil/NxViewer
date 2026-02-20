using Microsoft.Extensions.Logging;
using NxTiler.Application.Abstractions;

namespace NxTiler.App.Services;

public sealed partial class CaptureWorkflowService(
    ICaptureService captureService,
    IWindowQueryService windowQueryService,
    IWindowControlService windowControlService,
    ISettingsService settingsService,
    ILogger<CaptureWorkflowService> logger,
    ISnapshotSelectionService? snapshotSelectionService = null) : ICaptureWorkflowService
{
    private readonly SemaphoreSlim _gate = new(1, 1);
}
