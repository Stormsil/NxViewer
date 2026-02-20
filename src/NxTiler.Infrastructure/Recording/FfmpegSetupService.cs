using Microsoft.Extensions.Logging;
using NxTiler.Application.Abstractions;

namespace NxTiler.Infrastructure.Recording;

public sealed partial class FfmpegSetupService(ISettingsService settingsService, ILogger<FfmpegSetupService> logger) : IFfmpegSetupService
{
    private const string DownloadUrl = "https://www.gyan.dev/ffmpeg/builds/ffmpeg-release-essentials.zip";
}
