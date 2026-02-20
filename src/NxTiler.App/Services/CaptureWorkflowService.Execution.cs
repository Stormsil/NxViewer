using Microsoft.Extensions.Logging;
using NxTiler.Domain.Capture;
using NxTiler.Domain.Windowing;

namespace NxTiler.App.Services;

public sealed partial class CaptureWorkflowService
{
    private static CaptureResult CreateFailure(string message)
    {
        return new CaptureResult(
            Success: false,
            FilePath: null,
            ImageBytes: null,
            CaptureBounds: new WindowBounds(0, 0, 0, 0),
            ErrorMessage: message);
    }

    private async Task<T> ExecuteSerializedAsync<T>(Func<CancellationToken, Task<T>> operation, CancellationToken ct)
    {
        await _gate.WaitAsync(ct);
        try
        {
            return await operation(ct);
        }
        finally
        {
            _gate.Release();
        }
    }

    private void LogCaptureResult(string workflow, nint handle, CaptureResult result)
    {
        if (result.Success)
        {
            logger.LogInformation(
                "Capture workflow ({Workflow}) completed for {Handle}. File={FilePath}",
                workflow,
                handle,
                result.FilePath ?? "<memory>");
            return;
        }

        logger.LogWarning(
            "Capture workflow ({Workflow}) failed for {Handle}. Error={Error}",
            workflow,
            handle,
            result.ErrorMessage ?? "unknown");
    }
}
