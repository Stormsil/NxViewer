using ImGuiNET;
using NxTiler.Domain.Enums;
using NxTiler.Overlay.State;
using System.Numerics;

namespace NxTiler.Overlay.Panels;

/// <summary>
/// ImGui control panel: replaces WPF OverlayWindow.
/// Shows mode, auto toggle, window list, recording indicator, panel-open buttons.
/// </summary>
public sealed class ControlPanelRenderer
{
    private static readonly string[] ModeLabels =
    {
        "Сетка", "Фокус", "Макс", "Колонки", "Строки", "BSP",
    };

    private static readonly uint ColorActive = ImGui.ColorConvertFloat4ToU32(new Vector4(0.2f, 0.6f, 1.0f, 1.0f));
    private static readonly uint ColorRecording = ImGui.ColorConvertFloat4ToU32(new Vector4(1.0f, 0.2f, 0.2f, 1.0f));

    public Action<TileMode>? OnModeChanged { get; set; }
    public Action<bool>? OnAutoArrangeChanged { get; set; }
    public Action<int>? OnWindowSelected { get; set; }
    public Action? OnArrangeNow { get; set; }
    public Action? OnToggleMinimize { get; set; }
    public Action? OnOpenSettings { get; set; }
    public Action? OnOpenHotkeys { get; set; }
    public Action? OnOpenRecording { get; set; }
    public Action? OnOpenRules { get; set; }
    public Action? OnOpenPresets { get; set; }
    public Action? OnOpenLogs { get; set; }

    public void Render(OverlayState state)
    {
        if (!state.IsVisible)
        {
            return;
        }

        var flags = ImGuiWindowFlags.NoTitleBar
            | ImGuiWindowFlags.NoResize
            | ImGuiWindowFlags.AlwaysAutoResize
            | ImGuiWindowFlags.NoScrollbar
            | ImGuiWindowFlags.NoNav;

        ImGui.SetNextWindowBgAlpha(0.88f);
        ImGui.Begin("##NxTilerControl", flags);

        // Drag handle (8px invisible button at top)
        ImGui.InvisibleButton("##drag", new Vector2(ImGui.GetContentRegionAvail().X, 8));
        if (ImGui.IsItemActive())
        {
            ImGui.SetWindowPos(ImGui.GetWindowPos() + ImGui.GetMouseDragDelta());
            ImGui.ResetMouseDragDelta();
        }

        // Mode label — click to cycle
        var modeIndex = (int)state.Mode;
        var modeLabel = modeIndex < ModeLabels.Length ? ModeLabels[modeIndex] : state.Mode.ToString();
        ImGui.PushStyleColor(ImGuiCol.Text, ColorActive);
        if (ImGui.Selectable($"  {modeLabel}  ", false, ImGuiSelectableFlags.None, new Vector2(0, 0)))
        {
            var nextMode = (TileMode)(((int)state.Mode + 1) % 6);
            OnModeChanged?.Invoke(nextMode);
        }

        ImGui.PopStyleColor();
        ImGui.SameLine();

        // Auto toggle
        var autoLabel = state.AutoArrange ? "[A]" : "[ ]";
        if (ImGui.SmallButton($"Auto {autoLabel}"))
        {
            OnAutoArrangeChanged?.Invoke(!state.AutoArrange);
        }

        ImGui.SameLine();

        if (ImGui.SmallButton("↻"))
        {
            OnArrangeNow?.Invoke();
        }

        ImGui.SameLine();

        if (ImGui.SmallButton("⊡"))
        {
            OnToggleMinimize?.Invoke();
        }

        // Recording indicator
        if (state.Recording != RecordingState.Idle)
        {
            ImGui.SameLine();
            ImGui.PushStyleColor(ImGuiCol.Text, ColorRecording);
            var elapsed = state.RecordingElapsed;
            ImGui.Text($"● {elapsed:mm\\:ss}");
            ImGui.PopStyleColor();
        }

        ImGui.Separator();

        // Window list
        for (var i = 0; i < state.Windows.Count; i++)
        {
            var win = state.Windows[i];
            var isFocused = i == state.FocusedIndex;
            if (isFocused)
            {
                ImGui.PushStyleColor(ImGuiCol.Button, ColorActive);
            }

            if (ImGui.Button($"{win.DisplayName}##win{i}", new Vector2(-1, 0)))
            {
                OnWindowSelected?.Invoke(i);
            }

            if (isFocused)
            {
                ImGui.PopStyleColor();
            }
        }

        ImGui.Separator();

        // Panel shortcut buttons
        if (ImGui.SmallButton("[S]"))
        {
            OnOpenSettings?.Invoke();
        }

        ImGui.SameLine();

        if (ImGui.SmallButton("[K]"))
        {
            OnOpenHotkeys?.Invoke();
        }

        ImGui.SameLine();

        if (ImGui.SmallButton("[R]"))
        {
            OnOpenRecording?.Invoke();
        }

        ImGui.SameLine();

        if (ImGui.SmallButton("[W]"))
        {
            OnOpenRules?.Invoke();
        }

        ImGui.SameLine();

        if (ImGui.SmallButton("[G]"))
        {
            OnOpenPresets?.Invoke();
        }

        ImGui.SameLine();

        if (ImGui.SmallButton("[L]"))
        {
            OnOpenLogs?.Invoke();
        }

        ImGui.End();
    }
}
