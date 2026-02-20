using NxTiler.Domain.Grid;

namespace NxTiler.Domain.Settings;

public sealed record GridPresetsSettings(
    GridDimensions DefaultGrid,
    IReadOnlyList<GridPreset> Presets);
