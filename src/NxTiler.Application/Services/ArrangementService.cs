using NxTiler.Application.Abstractions;
using NxTiler.Domain.Enums;
using NxTiler.Domain.Windowing;

namespace NxTiler.Application.Services;

public sealed partial class ArrangementService : IArrangementService
{
    private const double TargetAspect = 16.0 / 9.0;

    public IReadOnlyList<WindowPlacement> BuildPlacements(
        IReadOnlyList<TargetWindowInfo> windows,
        TileMode mode,
        ArrangementContext context)
    {
        if (windows.Count == 0)
        {
            return Array.Empty<WindowPlacement>();
        }

        var candidates = windows.Where(x => !x.IsMaximized).ToList();
        if (candidates.Count == 0)
        {
            return Array.Empty<WindowPlacement>();
        }

        return mode switch
        {
            TileMode.Grid => BuildGridPlacements(candidates, context),
            TileMode.Focus => BuildFocusPlacements(candidates, context),
            TileMode.MaxSize => BuildMaxSizePlacements(candidates, context),
            TileMode.Column => BuildColumnPlacements(candidates, context),
            TileMode.Row => BuildRowPlacements(candidates, context),
            TileMode.Bsp => BuildBspPlacements(candidates, context),
            _ => BuildGridPlacements(candidates, context),
        };
    }
}
