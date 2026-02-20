using NxTiler.Domain.Windowing;

namespace NxTiler.Infrastructure.Recording;

public sealed partial class FfmpegRecordingEngine
{
    private List<string> BuildMaskingFilters(IReadOnlyList<WindowBounds> masksPx)
    {
        // Clamp masks to the recorded frame size.
        var filters = new List<string>(masksPx.Count);
        foreach (var m in masksPx)
        {
            // Masks are relative to the *requested* capture bounds; if the engine clipped the area,
            // shift boxes so they still align to the actual recorded frame.
            var shiftedX = m.X + (_sourceX - _actualLeft);
            var shiftedY = m.Y + (_sourceY - _actualTop);

            var x = Math.Max(0, shiftedX);
            var y = Math.Max(0, shiftedY);
            var w = m.Width;
            var h = m.Height;
            if (w <= 0 || h <= 0)
            {
                continue;
            }

            if (x >= _width || y >= _height)
            {
                continue;
            }

            if (x + w > _width)
            {
                w = _width - x;
            }

            if (y + h > _height)
            {
                h = _height - y;
            }

            if (w <= 0 || h <= 0)
            {
                continue;
            }

            filters.Add(BuildDrawBoxFilter(x, y, w, h));
        }

        return filters;
    }

    private static string BuildDrawBoxFilter(int x, int y, int w, int h)
    {
        return $"drawbox=x={x}:y={y}:w={w}:h={h}:color=black@1:t=fill";
    }
}
