using NxTiler.Domain.Windowing;

namespace NxTiler.Application.Services;

public sealed partial class ArrangementService
{
    private static IReadOnlyList<WindowPlacement> BuildFocusPlacements(List<TargetWindowInfo> windows, ArrangementContext context)
    {
        var focusedHandle = context.FocusedWindow ?? windows[0].Handle;
        var focusedWindow = windows.FirstOrDefault(x => x.Handle == focusedHandle) ?? windows[0];
        var others = windows.Where(x => x.Handle != focusedWindow.Handle).ToList();

        var workArea = context.WorkArea;
        var top = workArea.Y + context.TopPad;
        var height = Math.Max(1, workArea.Height - context.TopPad);
        var gap = context.Gap;

        var placements = new List<WindowPlacement>(windows.Count);

        var bottomRows = others.Count > 10 ? 2 : 1;
        var bottomCols = Math.Max(1, (int)Math.Ceiling(others.Count / (double)bottomRows));
        var rowHeight = Math.Max(120, (int)(height * 0.15));
        var bottomAreaHeight = others.Count == 0 ? 0 : (bottomRows * rowHeight) + ((bottomRows - 1) * gap);
        var focusAreaHeight = Math.Max(1, height - bottomAreaHeight - (others.Count == 0 ? 0 : gap));

        placements.Add(new WindowPlacement(focusedWindow.Handle, workArea.X, top, workArea.Width, focusAreaHeight));

        if (others.Count > 0)
        {
            var bottomY = top + focusAreaHeight + gap;
            placements.AddRange(BuildLayout(others, workArea.X, bottomY, workArea.Width, bottomAreaHeight, bottomRows, bottomCols, gap));
        }

        return placements;
    }

    private static IReadOnlyList<WindowPlacement> BuildMaxSizePlacements(List<TargetWindowInfo> windows, ArrangementContext context)
    {
        var focusedHandle = context.FocusedWindow ?? windows[0].Handle;
        var focusedWindow = windows.FirstOrDefault(x => x.Handle == focusedHandle) ?? windows[0];

        var workArea = context.WorkArea;
        return new[]
        {
            new WindowPlacement(focusedWindow.Handle, workArea.X, workArea.Y, workArea.Width, workArea.Height),
        };
    }
}
