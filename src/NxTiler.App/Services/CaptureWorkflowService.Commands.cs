using NxTiler.Domain.Capture;
using NxTiler.Domain.Enums;
using NxTiler.Domain.Windowing;

namespace NxTiler.App.Services;

public sealed partial class CaptureWorkflowService
{
    public Task<CaptureResult> RunInstantSnapshotAsync(nint targetWindow, CancellationToken ct = default)
    {
        return ExecuteSerializedAsync(async token =>
        {
            var handle = await ResolveTargetWindowAsync(targetWindow, token);
            if (handle == nint.Zero)
            {
                return CreateFailure("No target window found for snapshot.");
            }

            var request = new CaptureRequest(
                Mode: CaptureMode.InstantWindowSnapshot,
                TargetWindow: handle,
                SaveToDisk: true,
                CopyToClipboard: settingsService.Current.Capture.CopySnapshotToClipboardByDefault,
                IncludeCursor: false,
                Masks: null);

            var result = await captureService.CaptureAsync(request, token);
            LogCaptureResult("instant", handle, result);
            return result;
        }, ct);
    }

    public Task<CaptureResult> RunRegionSnapshotAsync(
        nint targetWindow,
        IReadOnlyList<CaptureMask> masks,
        CancellationToken ct = default)
    {
        return ExecuteSerializedAsync(async token =>
        {
            var handle = await ResolveTargetWindowAsync(targetWindow, token);
            if (handle == nint.Zero)
            {
                return CreateFailure("No target window found for region snapshot.");
            }

            await windowControlService.MaximizeAsync(handle, token);
            await Task.Delay(220, token);

            WindowBounds region;
            IReadOnlyList<CaptureMask> effectiveMasks = masks;
            if (snapshotSelectionService is not null)
            {
                var monitorBounds = await windowControlService.GetMonitorBoundsForWindowAsync(handle, token);
                var selection = await snapshotSelectionService.SelectRegionAndMasksAsync(monitorBounds, token);
                if (selection is null)
                {
                    return CreateFailure("Region snapshot canceled.");
                }

                region = selection.Region;
                effectiveMasks = selection.Masks;
            }
            else
            {
                region = await windowControlService.GetClientAreaScreenBoundsAsync(handle, token);
            }

            var request = new CaptureRequest(
                Mode: CaptureMode.RegionSnapshot,
                TargetWindow: handle,
                Region: region,
                SaveToDisk: true,
                CopyToClipboard: settingsService.Current.Capture.CopySnapshotToClipboardByDefault,
                IncludeCursor: false,
                Masks: effectiveMasks);

            var result = await captureService.CaptureAsync(request, token);
            LogCaptureResult("region", handle, result);
            return result;
        }, ct);
    }
}
