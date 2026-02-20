using Microsoft.Extensions.Logging;
using NxTiler.Application.Abstractions;
using NxTiler.Domain.Capture;
using NxTiler.Domain.Enums;
using NxTiler.Domain.Windowing;
using WindowCaptureCL;

namespace NxTiler.Infrastructure.Capture;

public sealed partial class WgcCaptureService(
    IWindowControlService windowControlService,
    ISettingsService settingsService,
    ILogger<WgcCaptureService> logger) : ICaptureService
{
    private const int StabilizeDelayMs = 80;
    private const int StabilizeSamplesRequired = 2;
    private static readonly TimeSpan StabilizeTimeout = TimeSpan.FromMilliseconds(1200);

    public async Task<CaptureResult> CaptureAsync(CaptureRequest request, CancellationToken ct = default)
    {
        if (request.TargetWindow == nint.Zero)
        {
            return CreateFailure("Target window is not specified.");
        }

        if (!CaptureFacade.IsCaptureSupported())
        {
            return CreateFailure("Windows Graphics Capture is not supported on this system.");
        }

        try
        {
            await PrepareWindowForCaptureAsync(request.TargetWindow, ct);
            var baseBounds = await WaitForStableWindowBoundsAsync(request.TargetWindow, ct);

            if (baseBounds.Width <= 0 || baseBounds.Height <= 0)
            {
                return CreateFailure("Target window has invalid bounds.");
            }

            using var sourceBitmap = await Task.Run(
                () => CaptureFacade.CaptureWindow((IntPtr)request.TargetWindow, includeCursor: request.IncludeCursor, drawBorder: false),
                ct);

            using var outputBitmap = CreateOutputBitmap(sourceBitmap, baseBounds, request, out var outputBounds);
            var imageBytes = EncodePng(outputBitmap);

            string? filePath = null;
            if (request.SaveToDisk)
            {
                filePath = await SaveSnapshotAsync(imageBytes, ct);
            }

            if (request.CopyToClipboard)
            {
                try
                {
                    await SetClipboardImageAsync(imageBytes, ct);
                }
                catch (Exception ex)
                {
                    logger.LogWarning(ex, "Snapshot captured but clipboard update failed.");
                }
            }

            return new CaptureResult(
                Success: true,
                FilePath: filePath,
                ImageBytes: imageBytes,
                CaptureBounds: outputBounds);
        }
        catch (OperationCanceledException)
        {
            throw;
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Capture failed for target window {Handle}.", request.TargetWindow);
            return CreateFailure(ex.Message);
        }
    }

    private static CaptureResult CreateFailure(string errorMessage)
    {
        return new CaptureResult(
            Success: false,
            FilePath: null,
            ImageBytes: null,
            CaptureBounds: new WindowBounds(0, 0, 0, 0),
            ErrorMessage: errorMessage);
    }
}
