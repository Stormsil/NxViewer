using System.Net.Http;

namespace NxTiler.Infrastructure.Recording;

public sealed partial class FfmpegSetupService
{
    private static async Task DownloadArchiveAsync(HttpClient http, string zipPath, Action<double, string>? progress, CancellationToken ct)
    {
        using var response = await http.GetAsync(DownloadUrl, HttpCompletionOption.ResponseHeadersRead, ct);
        response.EnsureSuccessStatusCode();

        var totalBytes = response.Content.Headers.ContentLength ?? -1;
        await using var sourceStream = await response.Content.ReadAsStreamAsync(ct);
        await using var destinationStream = File.Create(zipPath);

        var buffer = new byte[81920];
        long downloaded = 0;

        while (true)
        {
            var read = await sourceStream.ReadAsync(buffer, ct);
            if (read == 0)
            {
                break;
            }

            await destinationStream.WriteAsync(buffer.AsMemory(0, read), ct);
            downloaded += read;
            ReportDownloadProgress(progress, downloaded, totalBytes);
        }
    }

    private static void ReportDownloadProgress(Action<double, string>? progress, long downloadedBytes, long totalBytes)
    {
        if (totalBytes > 0)
        {
            progress?.Invoke(
                (double)downloadedBytes / totalBytes,
                $"Downloading: {downloadedBytes / 1048576} / {totalBytes / 1048576} MB");
            return;
        }

        progress?.Invoke(-1, $"Downloading: {downloadedBytes / 1048576} MB");
    }
}
