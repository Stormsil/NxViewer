using NxTiler.Domain.Windowing;

namespace NxTiler.Application.Services;

public sealed partial class ArrangementService
{
    private static IReadOnlyList<WindowPlacement> BuildColumnPlacements(List<TargetWindowInfo> windows, ArrangementContext context)
    {
        var workArea = context.WorkArea;
        var top = workArea.Y + context.TopPad;
        var height = Math.Max(1, workArea.Height - context.TopPad);

        return BuildLayout(windows, workArea.X, top, workArea.Width, height, 1, windows.Count, context.Gap);
    }
}
