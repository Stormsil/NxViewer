using System.Windows;
using System.Windows.Media.Imaging;

namespace NxTiler.Infrastructure.Capture;

public sealed partial class WgcCaptureService
{
    private async Task<string> SaveSnapshotAsync(byte[] imageBytes, CancellationToken ct)
    {
        var configuredFolder = settingsService.Current.Capture.SnapshotFolder;
        var snapshotFolder = string.IsNullOrWhiteSpace(configuredFolder)
            ? Environment.GetFolderPath(Environment.SpecialFolder.MyPictures)
            : configuredFolder;

        Directory.CreateDirectory(snapshotFolder);

        var fileName = $"NxTiler_{DateTime.Now:yyyyMMdd_HHmmss_fff}.png";
        var filePath = Path.Combine(snapshotFolder, fileName);
        await File.WriteAllBytesAsync(filePath, imageBytes, ct);
        return filePath;
    }

    private static Task SetClipboardImageAsync(byte[] imageBytes, CancellationToken ct)
    {
        if (imageBytes.Length == 0)
        {
            throw new ArgumentException("Snapshot bytes are empty.", nameof(imageBytes));
        }

        return SetClipboardImageInStaThreadAsync(imageBytes, ct);
    }

    private static async Task SetClipboardImageInStaThreadAsync(byte[] imageBytes, CancellationToken ct)
    {
        var completion = new TaskCompletionSource(TaskCreationOptions.RunContinuationsAsynchronously);

        var thread = new Thread(() =>
        {
            try
            {
                if (ct.IsCancellationRequested)
                {
                    completion.TrySetCanceled(ct);
                    return;
                }

                SetClipboardImageCore(imageBytes);
                completion.TrySetResult();
            }
            catch (Exception ex)
            {
                completion.TrySetException(ex);
            }
        });

        thread.SetApartmentState(ApartmentState.STA);
        thread.IsBackground = true;
        thread.Start();

        if (ct.CanBeCanceled)
        {
            using var registration = ct.Register(() => completion.TrySetCanceled(ct));
            await completion.Task;
            return;
        }

        await completion.Task;
    }

    private static void SetClipboardImageCore(byte[] imageBytes)
    {
        using var stream = new MemoryStream(imageBytes);
        var image = new BitmapImage();
        image.BeginInit();
        image.CacheOption = BitmapCacheOption.OnLoad;
        image.StreamSource = stream;
        image.EndInit();
        image.Freeze();

        Clipboard.SetImage(image);
        Clipboard.Flush();
    }
}
