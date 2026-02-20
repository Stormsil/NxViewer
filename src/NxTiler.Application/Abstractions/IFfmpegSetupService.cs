namespace NxTiler.Application.Abstractions;

public interface IFfmpegSetupService
{
    string? FindFfmpeg();

    Task<string?> ResolveAndSaveAsync(CancellationToken ct = default);

    Task<string?> DownloadAsync(Action<double, string>? progress = null, CancellationToken ct = default);
}
