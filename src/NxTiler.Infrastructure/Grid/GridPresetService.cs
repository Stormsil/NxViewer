using NxTiler.Application.Abstractions;
using NxTiler.Domain.Grid;
using NxTiler.Domain.Windowing;

namespace NxTiler.Infrastructure.Grid;

public sealed class GridPresetService(
    ISettingsService settingsService,
    IWindowControlService windowControlService) : IGridPresetService
{
    public IReadOnlyList<GridPreset> Presets => settingsService.Current.GridPresets.Presets;

    public async Task ApplyPresetAsync(string presetId, nint windowHandle, CancellationToken ct = default)
    {
        var preset = Presets.FirstOrDefault(p => p.Id == presetId)
            ?? throw new InvalidOperationException($"Preset '{presetId}' not found.");

        var workArea = await windowControlService.GetWorkAreaForWindowAsync(windowHandle, ct);
        var (x, y, w, h) = preset.Selection.ToPixelRect(workArea, preset.Grid);

        var placement = new WindowPlacement(windowHandle, x, y, w, h);
        await windowControlService.ApplyPlacementsAsync(new[] { placement }, ct);
    }

    public Task SavePresetAsync(GridPreset preset, CancellationToken ct = default)
    {
        ct.ThrowIfCancellationRequested();

        var current = settingsService.Current;
        var existingList = current.GridPresets.Presets.ToList();

        var idx = existingList.FindIndex(p => p.Id == preset.Id);
        if (idx >= 0)
        {
            existingList[idx] = preset;
        }
        else
        {
            existingList.Add(preset);
        }

        var updatedGridPresets = current.GridPresets with { Presets = existingList };
        settingsService.Update(current with { GridPresets = updatedGridPresets });
        return settingsService.SaveAsync(ct);
    }

    public Task DeletePresetAsync(string presetId, CancellationToken ct = default)
    {
        ct.ThrowIfCancellationRequested();

        var current = settingsService.Current;
        var newList = current.GridPresets.Presets.Where(p => p.Id != presetId).ToList();
        var updatedGridPresets = current.GridPresets with { Presets = newList };
        settingsService.Update(current with { GridPresets = updatedGridPresets });
        return settingsService.SaveAsync(ct);
    }
}
