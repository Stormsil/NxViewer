using NxTiler.Domain.Settings;

namespace NxTiler.Domain.Grid;

public sealed record GridPreset(
    string Id,
    string Name,
    GridDimensions Grid,
    GridCellSelection Selection,
    HotkeyBinding? HotkeyBinding = null);
