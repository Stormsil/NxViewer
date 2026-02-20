using Microsoft.Extensions.Logging;
using NxTiler.Domain.Capture;

namespace NxTiler.App.Services;

public sealed partial class WorkspaceOrchestrator
{
    private async Task ExecuteSnapshotAsync(bool region)
    {
        if (_captureWorkflowService is null)
        {
            _logger.LogDebug("Capture workflow is not registered. Snapshot hotkey ignored.");
            return;
        }

        var result = region
            ? await _captureWorkflowService.RunRegionSnapshotAsync(_focusedWindow, Array.Empty<CaptureMask>())
            : await _captureWorkflowService.RunInstantSnapshotAsync(_focusedWindow);

        if (result.Success)
        {
            RaiseStatus(result.FilePath is { Length: > 0 } path
                ? $"Snapshot saved: {path}"
                : "Snapshot captured.");
            return;
        }

        RaiseStatus($"Snapshot failed: {result.ErrorMessage ?? "unknown error"}");
    }

    private async Task ExecuteVisionToggleAsync()
    {
        if (_visionWorkflowService is null)
        {
            _logger.LogDebug("Vision workflow is not registered. Vision hotkey ignored.");
            return;
        }

        var result = await _visionWorkflowService.ToggleModeAsync(_focusedWindow);
        RaiseStatus(result.Message);
    }
}
