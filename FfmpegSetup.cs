using System;
using System.Diagnostics;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;

namespace NxTiler
{
    public static class FfmpegSetup
    {
        private static readonly string LocalDir = Path.Combine(
            Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "NxTiler");
        private static readonly string LocalFfmpeg = Path.Combine(LocalDir, "ffmpeg.exe");

        private const string DownloadUrl =
            "https://www.gyan.dev/ffmpeg/builds/ffmpeg-release-essentials.zip";

        /// <summary>
        /// Returns a valid path to ffmpeg.exe, or null if not available.
        /// Checks: 1) configured path  2) local AppData  3) system PATH
        /// </summary>
        public static string? FindFfmpeg()
        {
            // 1. Check configured path
            var configured = AppSettings.Default.FfmpegPath;
            if (!string.IsNullOrWhiteSpace(configured) && configured != "ffmpeg" && File.Exists(configured))
                return configured;

            // 2. Check local install
            if (File.Exists(LocalFfmpeg))
                return LocalFfmpeg;

            // 3. Check system PATH
            if (IsInPath())
                return "ffmpeg";

            return null;
        }

        /// <summary>
        /// Resolves ffmpeg path: finds existing or returns null.
        /// Also updates AppSettings if local copy exists.
        /// </summary>
        public static string? ResolveAndSave()
        {
            var path = FindFfmpeg();
            if (path != null && path != "ffmpeg")
            {
                AppSettings.Default.FfmpegPath = path;
                AppSettings.Default.Save();
            }
            return path;
        }

        /// <summary>
        /// Downloads and extracts ffmpeg to LocalAppData/NxTiler/.
        /// Reports progress via callback (0.0-1.0 range, -1 for indeterminate).
        /// </summary>
        public static async Task<string?> DownloadAsync(Action<double, string>? progress = null)
        {
            Directory.CreateDirectory(LocalDir);
            var zipPath = Path.Combine(LocalDir, "ffmpeg_download.zip");

            try
            {
                // Download
                progress?.Invoke(0, "Скачивание ffmpeg...");
                using var http = new HttpClient();
                http.Timeout = TimeSpan.FromMinutes(10);

                using var response = await http.GetAsync(DownloadUrl, HttpCompletionOption.ResponseHeadersRead);
                response.EnsureSuccessStatusCode();

                var totalBytes = response.Content.Headers.ContentLength ?? -1;
                using var stream = await response.Content.ReadAsStreamAsync();
                using var fileStream = File.Create(zipPath);

                var buffer = new byte[81920];
                long downloaded = 0;
                int read;
                while ((read = await stream.ReadAsync(buffer)) > 0)
                {
                    await fileStream.WriteAsync(buffer.AsMemory(0, read));
                    downloaded += read;
                    if (totalBytes > 0)
                        progress?.Invoke((double)downloaded / totalBytes,
                            $"Скачивание: {downloaded / 1048576} / {totalBytes / 1048576} МБ");
                    else
                        progress?.Invoke(-1, $"Скачивание: {downloaded / 1048576} МБ");
                }
                fileStream.Close();

                // Extract ffmpeg.exe from zip
                progress?.Invoke(-1, "Распаковка ffmpeg.exe...");
                using var zip = ZipFile.OpenRead(zipPath);
                var entry = zip.Entries.FirstOrDefault(e =>
                    e.Name.Equals("ffmpeg.exe", StringComparison.OrdinalIgnoreCase) &&
                    e.FullName.Contains("bin/", StringComparison.OrdinalIgnoreCase));

                if (entry == null)
                {
                    // Fallback: any ffmpeg.exe in the archive
                    entry = zip.Entries.FirstOrDefault(e =>
                        e.Name.Equals("ffmpeg.exe", StringComparison.OrdinalIgnoreCase));
                }

                if (entry == null)
                    return null;

                entry.ExtractToFile(LocalFfmpeg, overwrite: true);

                // Update settings
                AppSettings.Default.FfmpegPath = LocalFfmpeg;
                AppSettings.Default.Save();

                progress?.Invoke(1, "ffmpeg установлен.");
                return LocalFfmpeg;
            }
            catch (Exception ex)
            {
                progress?.Invoke(-1, $"Ошибка: {ex.Message}");
                return null;
            }
            finally
            {
                // Cleanup zip
                try { File.Delete(zipPath); } catch { }
            }
        }

        private static bool IsInPath()
        {
            try
            {
                var psi = new ProcessStartInfo
                {
                    FileName = "ffmpeg",
                    Arguments = "-version",
                    UseShellExecute = false,
                    CreateNoWindow = true,
                    RedirectStandardOutput = true,
                    RedirectStandardError = true
                };
                using var proc = Process.Start(psi);
                proc?.WaitForExit(3000);
                return proc != null && proc.HasExited && proc.ExitCode == 0;
            }
            catch
            {
                return false;
            }
        }
    }
}
