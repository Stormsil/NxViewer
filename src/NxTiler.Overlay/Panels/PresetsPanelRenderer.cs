using ImGuiNET;
using NxTiler.Domain.Grid;
using NxTiler.Overlay.State;
using System.Numerics;

namespace NxTiler.Overlay.Panels;

/// <summary>
/// ImGui grid presets panel: replaces WPF presets page.
/// Shows preset list with mini-grid previews; apply, delete, and new preset actions.
/// </summary>
public sealed class PresetsPanelRenderer
{
    public Action<string>? OnApplyPreset { get; set; }
    public Action<string>? OnDeletePreset { get; set; }
    public Action? OnOpenGridEditor { get; set; }
    public Action? OnClose { get; set; }

    private static readonly uint ColorGrid = ImGui.ColorConvertFloat4ToU32(new Vector4(0.5f, 0.5f, 0.5f, 1.0f));
    private static readonly uint ColorSelected = ImGui.ColorConvertFloat4ToU32(new Vector4(0.2f, 0.6f, 1.0f, 0.7f));
    private static readonly uint ColorCellBorder = ImGui.ColorConvertFloat4ToU32(new Vector4(0.7f, 0.7f, 0.7f, 0.5f));

    public void Render(OverlayState state)
    {
        if (!state.IsPresetsPanelOpen)
        {
            return;
        }

        ImGui.SetNextWindowSize(new Vector2(600, 440), ImGuiCond.FirstUseEver);
        ImGui.SetNextWindowBgAlpha(0.96f);
        var open = true;
        if (!ImGui.Begin("Пресеты сетки##presets", ref open, ImGuiWindowFlags.NoCollapse))
        {
            ImGui.End();
            if (!open) OnClose?.Invoke();
            return;
        }

        if (!open)
        {
            ImGui.End();
            OnClose?.Invoke();
            return;
        }

        var presets = state.GridPresets;

        if (presets.Count == 0)
        {
            ImGui.TextDisabled("Нет пресетов. Создайте первый через редактор сетки.");
        }

        for (var i = 0; i < presets.Count; i++)
        {
            var preset = presets[i];
            ImGui.PushID(i);

            // Mini-grid preview
            DrawMiniGrid(preset, new Vector2(80, 60));
            ImGui.SameLine(0, 12);

            // Name and controls
            ImGui.BeginGroup();
            ImGui.TextUnformatted(preset.Name);
            ImGui.TextDisabled($"{preset.Grid.Cols}×{preset.Grid.Rows}  " +
                $"Col [{preset.Selection.Col1}–{preset.Selection.Col2}] " +
                $"Row [{preset.Selection.Row1}–{preset.Selection.Row2}]");

            if (ImGui.SmallButton("Применить##applyPreset"))
            {
                OnApplyPreset?.Invoke(preset.Id);
            }

            ImGui.SameLine();
            if (ImGui.SmallButton("Удалить##deletePreset"))
            {
                OnDeletePreset?.Invoke(preset.Id);
            }

            ImGui.EndGroup();
            ImGui.Separator();
            ImGui.PopID();
        }

        ImGui.Spacing();
        if (ImGui.Button("+ Новый пресет##newPreset"))
        {
            OnOpenGridEditor?.Invoke();
        }

        ImGui.End();
    }

    private static void DrawMiniGrid(GridPreset preset, Vector2 size)
    {
        var pos = ImGui.GetCursorScreenPos();
        var drawList = ImGui.GetWindowDrawList();

        var cols = preset.Grid.Cols;
        var rows = preset.Grid.Rows;
        var cellW = size.X / cols;
        var cellH = size.Y / rows;

        var selMinCol = Math.Min(preset.Selection.Col1, preset.Selection.Col2);
        var selMaxCol = Math.Max(preset.Selection.Col1, preset.Selection.Col2);
        var selMinRow = Math.Min(preset.Selection.Row1, preset.Selection.Row2);
        var selMaxRow = Math.Max(preset.Selection.Row1, preset.Selection.Row2);

        // Draw grid cells
        for (var row = 0; row < rows; row++)
        {
            for (var col = 0; col < cols; col++)
            {
                var cellPos = pos + new Vector2(col * cellW, row * cellH);
                var cellEnd = cellPos + new Vector2(cellW, cellH);

                var isSelected = col >= selMinCol && col <= selMaxCol &&
                                 row >= selMinRow && row <= selMaxRow;

                if (isSelected)
                {
                    drawList.AddRectFilled(cellPos, cellEnd, ColorSelected, 2);
                }

                drawList.AddRect(cellPos, cellEnd, ColorCellBorder, 0, ImDrawFlags.None, 0.5f);
            }
        }

        // Outer border
        drawList.AddRect(pos, pos + size, ColorGrid, 2, ImDrawFlags.None, 1.0f);

        // Reserve the space
        ImGui.Dummy(size);
    }
}
