using ImGuiNET;
using NxTiler.Overlay.State;
using System.Numerics;

namespace NxTiler.Overlay.Panels;

/// <summary>
/// ImGui logs panel: replaces WPF LogsPage.
/// Shows filtered log entries with auto-scroll.
/// </summary>
public sealed class LogsPanelRenderer
{
    private static readonly uint ColorDebug = ImGui.ColorConvertFloat4ToU32(new Vector4(0.6f, 0.6f, 0.6f, 1.0f));
    private static readonly uint ColorInfo = ImGui.ColorConvertFloat4ToU32(new Vector4(1.0f, 1.0f, 1.0f, 1.0f));
    private static readonly uint ColorWarning = ImGui.ColorConvertFloat4ToU32(new Vector4(1.0f, 0.85f, 0.2f, 1.0f));
    private static readonly uint ColorError = ImGui.ColorConvertFloat4ToU32(new Vector4(1.0f, 0.3f, 0.3f, 1.0f));

    private static readonly string[] LevelNames = { "Verbose", "Debug", "Info", "Warning", "Error", "Fatal" };

    public Action? OnClear { get; set; }
    public Action? OnClose { get; set; }

    private string _searchFilter = string.Empty;
    private int _minLevel = 2; // Information
    private bool _autoScroll = true;

    public void Render(OverlayState state)
    {
        if (!state.IsLogsPanelOpen)
        {
            return;
        }

        ImGui.SetNextWindowSize(new Vector2(800, 500), ImGuiCond.FirstUseEver);
        ImGui.SetNextWindowBgAlpha(0.96f);
        var open = true;
        if (!ImGui.Begin("Логи##logs", ref open, ImGuiWindowFlags.NoCollapse))
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

        // Toolbar
        ImGui.SetNextItemWidth(120);
        ImGui.Combo("Уровень##logLevel", ref _minLevel, LevelNames, LevelNames.Length);
        ImGui.SameLine();
        ImGui.SetNextItemWidth(200);
        ImGui.InputText("Поиск##logSearch", ref _searchFilter, 256);
        ImGui.SameLine();
        if (ImGui.Button("Очистить##logClear"))
        {
            OnClear?.Invoke();
        }

        ImGui.SameLine();
        ImGui.Checkbox("Авто-скролл##logAutoScroll", ref _autoScroll);

        ImGui.Separator();

        // Log list
        ImGui.BeginChild("##logList", new Vector2(0, -1), ImGuiChildFlags.None, ImGuiWindowFlags.HorizontalScrollbar);

        var entries = state.Logs.Entries;
        var filter = _searchFilter;
        var minLevel = _minLevel;

        for (var i = 0; i < entries.Count; i++)
        {
            var entry = entries[i];

            // Level filter
            var levelOk = LevelToIndex(entry.Level) >= minLevel;
            if (!levelOk) continue;

            // Search filter
            if (!string.IsNullOrEmpty(filter) &&
                !entry.Message.Contains(filter, StringComparison.OrdinalIgnoreCase))
            {
                continue;
            }

            var color = LevelToColor(entry.Level);
            ImGui.PushStyleColor(ImGuiCol.Text, color);
            ImGui.TextUnformatted($"[{entry.Timestamp:HH:mm:ss}] [{entry.Level,-7}] {entry.Message}");
            ImGui.PopStyleColor();

            if (entry.ExceptionText is not null)
            {
                ImGui.PushStyleColor(ImGuiCol.Text, ColorError);
                ImGui.TextUnformatted(entry.ExceptionText);
                ImGui.PopStyleColor();
            }
        }

        if (_autoScroll && ImGui.GetScrollY() >= ImGui.GetScrollMaxY() - 8)
        {
            ImGui.SetScrollHereY(1.0f);
        }

        ImGui.EndChild();
        ImGui.End();
    }

    private static int LevelToIndex(string level) => level switch
    {
        "Verbose" => 0,
        "Debug" => 1,
        "Information" => 2,
        "Warning" => 3,
        "Error" => 4,
        "Fatal" => 5,
        _ => 2,
    };

    private static uint LevelToColor(string level) => level switch
    {
        "Verbose" or "Debug" => ColorDebug,
        "Warning" => ColorWarning,
        "Error" or "Fatal" => ColorError,
        _ => ColorInfo,
    };
}
