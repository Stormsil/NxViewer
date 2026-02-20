using System.Drawing;
using Microsoft.Extensions.Logging;

namespace NxTiler.Infrastructure.Recording;

public sealed partial class WgcVideoRecordingEngine
{
    private void TryDelete(string? path)
    {
        if (string.IsNullOrWhiteSpace(path))
        {
            return;
        }

        try
        {
            if (File.Exists(path))
            {
                File.Delete(path);
            }
        }
        catch (Exception ex)
        {
            logger.LogDebug(ex, "Failed to delete temporary file {Path}.", path);
        }
    }

    private void ResetState()
    {
        _framePumpCts?.Dispose();
        _framePumpCts = null;
        _framePumpTask = null;
        _targetWindow = nint.Zero;
        _captureRect = Rectangle.Empty;
        _fps = 0;
        _includeCursor = false;
        _rawOutputPath = null;
        IsRunning = false;
    }
}
