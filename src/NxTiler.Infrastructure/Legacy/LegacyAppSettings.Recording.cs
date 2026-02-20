using System.Configuration;

namespace NxTiler.Infrastructure.Legacy;

internal sealed partial class LegacyAppSettings
{
    [UserScopedSetting]
    public string RecordingFolder
    {
        get
        {
            var value = (string)this[nameof(RecordingFolder)];
            return string.IsNullOrEmpty(value)
                ? Environment.GetFolderPath(Environment.SpecialFolder.MyVideos)
                : value;
        }
        set => this[nameof(RecordingFolder)] = value;
    }

    [UserScopedSetting]
    [DefaultSettingValue("30")]
    public int RecordingFps { get => (int)this[nameof(RecordingFps)]; set => this[nameof(RecordingFps)] = value; }

    [UserScopedSetting]
    [DefaultSettingValue("ffmpeg")]
    public string FfmpegPath
    {
        get
        {
            var value = (string)this[nameof(FfmpegPath)];
            return string.IsNullOrEmpty(value) ? "ffmpeg" : value;
        }
        set => this[nameof(FfmpegPath)] = value;
    }
}
