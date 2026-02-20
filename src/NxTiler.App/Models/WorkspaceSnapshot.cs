using NxTiler.Domain.Enums;
using NxTiler.Domain.Windowing;

namespace NxTiler.App.Models;

public sealed record WorkspaceSnapshot(
    IReadOnlyList<TargetWindowInfo> Windows,
    TileMode Mode,
    bool AutoArrangeEnabled,
    bool AllMinimized,
    bool IsForeignAppActive,
    nint FocusedWindow,
    int ActiveIndex);
