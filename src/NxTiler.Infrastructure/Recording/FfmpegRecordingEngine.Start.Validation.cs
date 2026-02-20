namespace NxTiler.Infrastructure.Recording;

public sealed partial class FfmpegRecordingEngine
{
    private bool TryValidateStartParameters(int width, int height, int fps)
    {
        if (width <= 0 || height <= 0)
        {
            LastError = $"Invalid capture dimensions: {width}x{height}.";
            return false;
        }

        if (fps <= 0)
        {
            LastError = $"Invalid FPS: {fps}.";
            return false;
        }

        return true;
    }
}
