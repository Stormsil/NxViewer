using NxTiler.Domain.Windowing;

namespace NxTiler.Domain.Grid;

public sealed record GridCellSelection(int Col1, int Row1, int Col2, int Row2)
{
    /// <summary>Converts this grid selection to a pixel rect within the given work area.</summary>
    public (int X, int Y, int Width, int Height) ToPixelRect(WindowBounds workArea, GridDimensions grid)
    {
        var cellW = (double)workArea.Width / grid.Cols;
        var cellH = (double)workArea.Height / grid.Rows;

        var minCol = Math.Min(Col1, Col2);
        var maxCol = Math.Max(Col1, Col2);
        var minRow = Math.Min(Row1, Row2);
        var maxRow = Math.Max(Row1, Row2);

        var x = workArea.X + (int)(minCol * cellW);
        var y = workArea.Y + (int)(minRow * cellH);
        var w = (int)((maxCol - minCol + 1) * cellW);
        var h = (int)((maxRow - minRow + 1) * cellH);

        return (x, y, w, h);
    }
}
