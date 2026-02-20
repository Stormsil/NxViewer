using NxTiler.Domain.Grid;

namespace NxTiler.Application.Abstractions;

public interface IGridPresetService
{
    IReadOnlyList<GridPreset> Presets { get; }

    Task ApplyPresetAsync(string presetId, nint windowHandle, CancellationToken ct = default);

    Task SavePresetAsync(GridPreset preset, CancellationToken ct = default);

    Task DeletePresetAsync(string presetId, CancellationToken ct = default);
}
