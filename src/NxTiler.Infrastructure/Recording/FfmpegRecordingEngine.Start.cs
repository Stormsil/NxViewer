namespace NxTiler.Infrastructure.Recording;

public sealed partial class FfmpegRecordingEngine
{
    public bool Start(int x, int y, int width, int height, int fps, string folder, string ffmpegPath)
    {
        if (!TryValidateStartParameters(width, height, fps))
        {
            return false;
        }

        if (!TryConfigureCaptureGeometry(x, y, width, height))
        {
            return false;
        }

        InitializeStartSession(fps, folder, ffmpegPath);
        if (!EnsureOutputFolderExists())
        {
            return false;
        }

        return StartNewSegment();
    }
}
