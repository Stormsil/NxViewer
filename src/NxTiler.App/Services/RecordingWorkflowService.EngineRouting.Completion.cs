using NxTiler.Domain.Capture;
using NxTiler.Domain.Windowing;

namespace NxTiler.App.Services;

public sealed partial class RecordingWorkflowService
{
    private async Task AbortActiveEngineAsync(CancellationToken token)
    {
        if (_useVideoEngine)
        {
            await _videoRecordingEngine!.AbortAsync(token);
        }
        else
        {
            _recordingEngine.Abort();
        }
    }

    private async Task<string?> FinalizeActiveEngineAsync(IReadOnlyList<WindowBounds> masksPx, CancellationToken token)
    {
        if (_useVideoEngine)
        {
            var captureMasks = masksPx
                .Select(static x => new CaptureMask(x.X, x.Y, x.Width, x.Height))
                .ToArray();
            return await _videoRecordingEngine!.StopAsync(captureMasks, token);
        }

        return await _recordingEngine.FinalizeRecordingAsync(masksPx, token);
    }
}
