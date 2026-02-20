using Microsoft.Extensions.Logging;
using NxTiler.Infrastructure.Native;

namespace NxTiler.Infrastructure.Recording;

public sealed partial class FfmpegRecordingEngine
{
    private bool TryConfigureCaptureGeometry(int x, int y, int width, int height)
    {
        // ffmpeg gdigrab uses coordinates relative to the virtual desktop origin (top-left of the virtual screen).
        // Windows screen coords can be negative when monitors are positioned left/up of the primary.
        var virtualScreen = Win32Native.GetVirtualScreenRectPx();

        var capLeft = x;
        var capTop = y;
        var capRight = x + width;
        var capBottom = y + height;

        var virtLeft = virtualScreen.Left;
        var virtTop = virtualScreen.Top;
        var virtRight = virtLeft + virtualScreen.Width;
        var virtBottom = virtTop + virtualScreen.Height;

        var left = Math.Max(capLeft, virtLeft);
        var top = Math.Max(capTop, virtTop);
        var right = Math.Min(capRight, virtRight);
        var bottom = Math.Min(capBottom, virtBottom);

        var clippedWidth = right - left;
        var clippedHeight = bottom - top;
        if (clippedWidth <= 0 || clippedHeight <= 0)
        {
            LastError =
                $"Capture area is outside the virtual screen.\n" +
                $"Capture: ({capLeft},{capTop}) {width}x{height}\n" +
                $"Virtual: ({virtLeft},{virtTop}) {virtualScreen.Width}x{virtualScreen.Height}";
            return false;
        }

        // libx264 yuv420p requires even dimensions
        _sourceX = x;
        _sourceY = y;
        _actualLeft = left;
        _actualTop = top;

        _x = left - virtLeft;
        _y = top - virtTop;
        _width = (clippedWidth / 2) * 2;
        _height = (clippedHeight / 2) * 2;
        if (_width <= 0 || _height <= 0)
        {
            LastError =
                $"Capture area is too small after alignment.\n" +
                $"Capture: ({capLeft},{capTop}) {width}x{height}";
            return false;
        }

        logger.LogInformation(
            "Recording: gdigrab config. Virtual=({VL},{VT}) {VW}x{VH}; Requested=({X},{Y}) {W}x{H}; Actual=({AL},{AT}) {AW}x{AH}; Offset=({OX},{OY}) Size={SW}x{SH}",
            virtLeft,
            virtTop,
            virtualScreen.Width,
            virtualScreen.Height,
            x,
            y,
            width,
            height,
            _actualLeft,
            _actualTop,
            clippedWidth,
            clippedHeight,
            _x,
            _y,
            _width,
            _height);
        return true;
    }
}
