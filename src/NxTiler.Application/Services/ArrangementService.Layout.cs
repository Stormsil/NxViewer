using NxTiler.Domain.Windowing;

namespace NxTiler.Application.Services;

public sealed partial class ArrangementService
{
    private static IReadOnlyList<WindowPlacement> BuildLayout(
        List<TargetWindowInfo> windows,
        int startX,
        int startY,
        int width,
        int height,
        int rows,
        int cols,
        int gap)
    {
        var placements = new List<WindowPlacement>(windows.Count);
        var totalGapW = (cols - 1) * gap;
        var totalGapH = (rows - 1) * gap;
        var baseWidth = Math.Max(1, (width - totalGapW) / cols);
        var baseHeight = Math.Max(1, (height - totalGapH) / rows);
        var remainderWidth = Math.Max(0, (width - totalGapW) % cols);
        var remainderHeight = Math.Max(0, (height - totalGapH) % rows);

        var index = 0;
        var y = startY;

        for (var row = 0; row < rows; row++)
        {
            var currentHeight = baseHeight + (row < remainderHeight ? 1 : 0);
            var x = startX;

            for (var col = 0; col < cols; col++)
            {
                if (index >= windows.Count)
                {
                    break;
                }

                var currentWidth = baseWidth + (col < remainderWidth ? 1 : 0);
                placements.Add(new WindowPlacement(windows[index].Handle, x, y, currentWidth, currentHeight));
                x += currentWidth + gap;
                index++;
            }

            y += currentHeight + gap;
        }

        return placements;
    }
}
