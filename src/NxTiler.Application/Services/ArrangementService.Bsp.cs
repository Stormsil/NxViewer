using NxTiler.Domain.Windowing;

namespace NxTiler.Application.Services;

public sealed partial class ArrangementService
{
    private static IReadOnlyList<WindowPlacement> BuildBspPlacements(List<TargetWindowInfo> windows, ArrangementContext context)
    {
        var workArea = context.WorkArea;
        var top = workArea.Y + context.TopPad;
        var height = Math.Max(1, workArea.Height - context.TopPad);

        var placements = new List<WindowPlacement>(windows.Count);
        BspSplit(windows, 0, windows.Count, workArea.X, top, workArea.Width, height, context.Gap, splitHorizontal: true, placements);
        return placements;
    }

    private static void BspSplit(
        List<TargetWindowInfo> windows,
        int start,
        int count,
        int x,
        int y,
        int width,
        int height,
        int gap,
        bool splitHorizontal,
        List<WindowPlacement> placements)
    {
        if (count == 0)
        {
            return;
        }

        if (count == 1)
        {
            placements.Add(new WindowPlacement(windows[start].Handle, x, y, Math.Max(1, width), Math.Max(1, height)));
            return;
        }

        var leftCount = count / 2;
        var rightCount = count - leftCount;

        if (splitHorizontal)
        {
            var leftWidth = Math.Max(1, (width - gap) * leftCount / count);
            var rightWidth = Math.Max(1, width - leftWidth - gap);
            BspSplit(windows, start, leftCount, x, y, leftWidth, height, gap, !splitHorizontal, placements);
            BspSplit(windows, start + leftCount, rightCount, x + leftWidth + gap, y, rightWidth, height, gap, !splitHorizontal, placements);
        }
        else
        {
            var topHeight = Math.Max(1, (height - gap) * leftCount / count);
            var bottomHeight = Math.Max(1, height - topHeight - gap);
            BspSplit(windows, start, leftCount, x, y, width, topHeight, gap, !splitHorizontal, placements);
            BspSplit(windows, start + leftCount, rightCount, x, y + topHeight + gap, width, bottomHeight, gap, !splitHorizontal, placements);
        }
    }
}
