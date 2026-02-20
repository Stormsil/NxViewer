using NxTiler.Domain.Windowing;

namespace NxTiler.Application.Services;

public sealed partial class ArrangementService
{
    private static IReadOnlyList<WindowPlacement> BuildGridPlacements(List<TargetWindowInfo> windows, ArrangementContext context)
    {
        var workArea = context.WorkArea;
        var top = workArea.Y + context.TopPad;
        var height = Math.Max(1, workArea.Height - context.TopPad);

        int rows;
        int cols;

        if (windows.Count is >= 6 and <= 10)
        {
            rows = 2;
            cols = (int)Math.Ceiling(windows.Count / 2.0);
        }
        else
        {
            (rows, cols) = ComputeGrid(windows.Count, workArea.Width, height);
        }

        return BuildLayout(windows, workArea.X, top, workArea.Width, height, rows, cols, context.Gap);
    }

    private static (int Rows, int Cols) ComputeGrid(int count, double areaWidth, double areaHeight)
    {
        if (count <= 0)
        {
            return (0, 0);
        }

        var bestCost = double.MaxValue;
        var bestRows = 1;
        var bestCols = count;

        for (var rows = 1; rows <= count; rows++)
        {
            var cols = (int)Math.Ceiling(count / (double)rows);
            var cellWidth = areaWidth / cols;
            var cellHeight = areaHeight / rows;
            var cellAspect = cellWidth / cellHeight;

            var aspectPenalty = Math.Abs(Math.Log(cellAspect / TargetAspect));
            var emptyPenalty = (rows * cols - count) * 0.12;
            var shapePenalty = Math.Abs(rows - cols) * 0.02;

            var cost = (aspectPenalty * 3.0) + emptyPenalty + shapePenalty;
            if (cost < bestCost - 1e-9)
            {
                bestCost = cost;
                bestRows = rows;
                bestCols = cols;
            }
        }

        return (bestRows, bestCols);
    }
}
