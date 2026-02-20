using ImGuiNET;
using NxTiler.Domain.Grid;
using NxTiler.Overlay.State;
using System.Numerics;

namespace NxTiler.Overlay.Panels;

/// <summary>
/// Divvy-style grid editor. Full-screen transparent overlay, LMB drag to select cells.
/// After drag: mini modal with name + save/apply/cancel.
/// </summary>
public sealed class GridEditorRenderer
{
    private bool _isDragging;
    private int _dragStartCol;
    private int _dragStartRow;
    private int _dragCurrentCol;
    private int _dragCurrentRow;
    private bool _showSaveModal;
    private string _presetName = string.Empty;

    public Action<string, GridCellSelection, GridDimensions>? OnSavePreset { get; set; }
    public Action<GridCellSelection, GridDimensions>? OnApplyImmediate { get; set; }
    public Action? OnClose { get; set; }

    public void Render(OverlayState state)
    {
        if (!state.IsGridEditorVisible)
        {
            return;
        }

        var io = ImGui.GetIO();
        var screenSize = io.DisplaySize;
        var cols = state.GridEditor.Cols;
        var rows = state.GridEditor.Rows;

        ImGui.SetNextWindowPos(Vector2.Zero);
        ImGui.SetNextWindowSize(screenSize);
        ImGui.SetNextWindowBgAlpha(0.3f);

        var windowFlags = ImGuiWindowFlags.NoDecoration
            | ImGuiWindowFlags.NoMove
            | ImGuiWindowFlags.NoSavedSettings
            | ImGuiWindowFlags.NoNav;

        ImGui.Begin("##GridEditor", windowFlags);

        var drawList = ImGui.GetWindowDrawList();
        var cellW = screenSize.X / cols;
        var cellH = screenSize.Y / rows;

        // Draw grid lines
        for (var c = 0; c <= cols; c++)
        {
            drawList.AddLine(new Vector2(c * cellW, 0), new Vector2(c * cellW, screenSize.Y),
                ImGui.ColorConvertFloat4ToU32(new Vector4(0.5f, 0.5f, 0.5f, 0.5f)));
        }

        for (var r = 0; r <= rows; r++)
        {
            drawList.AddLine(new Vector2(0, r * cellH), new Vector2(screenSize.X, r * cellH),
                ImGui.ColorConvertFloat4ToU32(new Vector4(0.5f, 0.5f, 0.5f, 0.5f)));
        }

        // Compute hover cell
        var mousePos = ImGui.GetMousePos();
        var hoverCol = (int)(mousePos.X / cellW);
        var hoverRow = (int)(mousePos.Y / cellH);
        hoverCol = Math.Clamp(hoverCol, 0, cols - 1);
        hoverRow = Math.Clamp(hoverRow, 0, rows - 1);

        // Drag handling
        if (ImGui.IsMouseClicked(ImGuiMouseButton.Left) && !_showSaveModal)
        {
            _isDragging = true;
            _dragStartCol = hoverCol;
            _dragStartRow = hoverRow;
            _dragCurrentCol = hoverCol;
            _dragCurrentRow = hoverRow;
        }

        if (_isDragging && ImGui.IsMouseDown(ImGuiMouseButton.Left))
        {
            _dragCurrentCol = hoverCol;
            _dragCurrentRow = hoverRow;
        }

        if (_isDragging && ImGui.IsMouseReleased(ImGuiMouseButton.Left))
        {
            _isDragging = false;
            _showSaveModal = true;
            _presetName = string.Empty;
        }

        // Draw selection highlight
        if (_isDragging || _showSaveModal)
        {
            var minCol = Math.Min(_dragStartCol, _dragCurrentCol);
            var maxCol = Math.Max(_dragStartCol, _dragCurrentCol);
            var minRow = Math.Min(_dragStartRow, _dragCurrentRow);
            var maxRow = Math.Max(_dragStartRow, _dragCurrentRow);

            var selX = minCol * cellW;
            var selY = minRow * cellH;
            var selW = (maxCol - minCol + 1) * cellW;
            var selH = (maxRow - minRow + 1) * cellH;

            drawList.AddRectFilled(
                new Vector2(selX, selY),
                new Vector2(selX + selW, selY + selH),
                ImGui.ColorConvertFloat4ToU32(new Vector4(0.2f, 0.6f, 1.0f, 0.35f)));
        }

        // ESC to cancel
        if (ImGui.IsKeyPressed(ImGuiKey.Escape))
        {
            _isDragging = false;
            _showSaveModal = false;
            OnClose?.Invoke();
        }

        ImGui.End();

        // Save modal
        if (_showSaveModal)
        {
            RenderSaveModal(cols, rows);
        }
    }

    private void RenderSaveModal(int cols, int rows)
    {
        var io = ImGui.GetIO();
        ImGui.SetNextWindowPos(io.DisplaySize / 2, ImGuiCond.Always, new Vector2(0.5f, 0.5f));
        ImGui.SetNextWindowBgAlpha(0.95f);

        if (ImGui.Begin("##GridSave", ImGuiWindowFlags.NoDecoration | ImGuiWindowFlags.AlwaysAutoResize | ImGuiWindowFlags.NoMove))
        {
            ImGui.Text("Пресет сетки");
            ImGui.Separator();

            ImGui.Text("Название:");
            ImGui.SameLine();
            ImGui.SetNextItemWidth(160);
            ImGui.InputText("##name", ref _presetName, 64);

            ImGui.Spacing();

            var selection = new GridCellSelection(
                Math.Min(_dragStartCol, _dragCurrentCol),
                Math.Min(_dragStartRow, _dragCurrentRow),
                Math.Max(_dragStartCol, _dragCurrentCol),
                Math.Max(_dragStartRow, _dragCurrentRow));
            var grid = new GridDimensions(cols, rows);

            if (ImGui.Button("Применить"))
            {
                OnApplyImmediate?.Invoke(selection, grid);
                _showSaveModal = false;
                OnClose?.Invoke();
            }

            ImGui.SameLine();

            if (ImGui.Button("Сохранить") && !string.IsNullOrWhiteSpace(_presetName))
            {
                OnSavePreset?.Invoke(_presetName, selection, grid);
                _showSaveModal = false;
                OnClose?.Invoke();
            }

            ImGui.SameLine();

            if (ImGui.Button("Отмена"))
            {
                _showSaveModal = false;
            }
        }

        ImGui.End();
    }
}
