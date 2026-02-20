using NxTiler.Domain.Enums;

namespace NxTiler.Domain.Windowing;

public sealed record ArrangementContext(
    TileMode Mode,
    WindowBounds WorkArea,
    int Gap,
    int TopPad,
    nint? FocusedWindow
);
