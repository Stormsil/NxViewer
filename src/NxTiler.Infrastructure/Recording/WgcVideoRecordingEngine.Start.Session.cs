using System.Drawing;
using NxTiler.Domain.Capture;

namespace NxTiler.Infrastructure.Recording;

public sealed partial class WgcVideoRecordingEngine
{
    private string InitializeStartSession(nint targetWindow, RecordingProfile profile, Rectangle captureRect)
    {
        _targetWindow = targetWindow;
        _captureRect = captureRect;
        _fps = profile.FramesPerSecond;
        _includeCursor = profile.IncludeCursor;

        _outputFolder = ResolveOutputFolder();
        Directory.CreateDirectory(_outputFolder);

        _ffmpegPath = ResolveFfmpegPath();
        _sessionPrefix = $"rec_{DateTime.Now:yyyyMMdd_HHmmss}";
        var rawOutputPath = Path.Combine(_outputFolder, $"{_sessionPrefix}_wgc_raw.mp4");
        _rawOutputPath = rawOutputPath;
        TryDelete(rawOutputPath);

        return rawOutputPath;
    }

    private string ResolveOutputFolder()
    {
        var folder = settingsService.Current.Paths.RecordingFolder;
        if (string.IsNullOrWhiteSpace(folder))
        {
            folder = Environment.GetFolderPath(Environment.SpecialFolder.MyVideos);
        }

        return folder;
    }

    private string ResolveFfmpegPath()
    {
        var ffmpegPath = settingsService.Current.Paths.FfmpegPath;
        if (string.IsNullOrWhiteSpace(ffmpegPath))
        {
            ffmpegPath = "ffmpeg";
        }

        return ffmpegPath;
    }
}
