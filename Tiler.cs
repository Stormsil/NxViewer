// Tiler.cs
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

namespace NxTiler
{
    // Описание окна NoMachine
    public class TargetWindow
    {
        public IntPtr Hwnd { get; init; }
        public string Title { get; init; } = "";
        public string SourceName { get; init; } = "";
        public bool IsMaximized { get; set; }

        public string HwndHex => $"0x{Hwnd.ToInt64():X}";
        public string State => IsMaximized ? "Maximized" : "Normal";
    }

    public static class Tiler
    {
        private const double TargetAspect = 16.0 / 9.0; // целевой аспект

        public static (int rows, int cols) ComputeGrid(int n, double areaW, double areaH)
        {
            if (n <= 0) return (0, 0);

            double bestCost = double.MaxValue;
            int bestRows = 1, bestCols = n;

            for (int rows = 1; rows <= n; rows++)
            {
                int cols = (int)Math.Ceiling(n / (double)rows);
                double cellW = areaW / cols;
                double cellH = areaH / rows;
                double cellAR = cellW / cellH;

                double arPenalty = Math.Abs(Math.Log(cellAR / TargetAspect));
                double emptyPenalty = (rows * cols - n) * 0.12;
                double shapePenalty = Math.Abs(rows - cols) * 0.02;

                double cost = arPenalty * 3.0 + emptyPenalty + shapePenalty;
                if (cost < bestCost - 1e-9)
                {
                    bestCost = cost; bestRows = rows; bestCols = cols;
                }
            }
            return (bestRows, bestCols);
        }

        // ВНИМАНИЕ: используем ПОЛНОСТЬЮ квалифицированный тип System.Windows.Rect
        public static System.Windows.Rect[] BuildCells(
            int count,
            System.Windows.Rect workArea,
            int rows,
            int cols,
            int gap = 6)
        {
            var cells = new List<System.Windows.Rect>(count);

            double cellW = Math.Floor((workArea.Width - (cols + 1) * gap) / cols);
            double cellH = Math.Floor((workArea.Height - (rows + 1) * gap) / rows);

            int k = 0;
            for (int r = 0; r < rows; r++)
            {
                for (int c = 0; c < cols && k < count; c++, k++)
                {
                    double x = workArea.X + gap + c * (cellW + gap);
                    double y = workArea.Y + gap + r * (cellH + gap);
                    cells.Add(new System.Windows.Rect(x, y, cellW, cellH));
                }
            }
            return cells.ToArray();
        }

        public static IEnumerable<string> FilterSessionNames(IEnumerable<string> filenamesNoExt, string regex)
        {
            if (string.IsNullOrWhiteSpace(regex)) return filenamesNoExt;
            var re = new Regex(regex, RegexOptions.IgnoreCase);
            return filenamesNoExt.Where(n => re.IsMatch(n));
        }
    }
}
