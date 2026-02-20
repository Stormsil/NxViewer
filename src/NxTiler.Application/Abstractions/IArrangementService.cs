using NxTiler.Domain.Enums;
using NxTiler.Domain.Windowing;

namespace NxTiler.Application.Abstractions;

public interface IArrangementService
{
    IReadOnlyList<WindowPlacement> BuildPlacements(
        IReadOnlyList<TargetWindowInfo> windows,
        TileMode mode,
        ArrangementContext context);
}
