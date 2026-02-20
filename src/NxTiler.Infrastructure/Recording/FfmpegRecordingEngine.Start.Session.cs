using Microsoft.Extensions.Logging;

namespace NxTiler.Infrastructure.Recording;

public sealed partial class FfmpegRecordingEngine
{
    private void InitializeStartSession(int fps, string folder, string ffmpegPath)
    {
        _fps = fps;
        _outputFolder = folder;
        _ffmpegPath = ffmpegPath;
        _sessionPrefix = $"rec_{DateTime.Now:yyyyMMdd_HHmmss}";
        _segments.Clear();
        LastError = null;
    }

    private bool EnsureOutputFolderExists()
    {
        try
        {
            Directory.CreateDirectory(_outputFolder);
            return true;
        }
        catch (Exception ex)
        {
            LastError = $"Failed to create recording folder: {ex.Message}\nFolder: \"{_outputFolder}\"";
            logger.LogWarning(ex, "Failed to create recording output folder {Folder}", _outputFolder);
            return false;
        }
    }
}
