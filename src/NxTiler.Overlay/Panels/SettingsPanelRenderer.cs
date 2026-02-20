using ImGuiNET;
using NxTiler.Overlay.State;
using System.Numerics;

namespace NxTiler.Overlay.Panels;

/// <summary>
/// ImGui settings panel: replaces WPF SettingsPage.
/// Tabs: Фильтры | Раскладка | Пути | Vision
/// </summary>
public sealed class SettingsPanelRenderer
{
    public Action<SettingsPanelState>? OnSave { get; set; }
    public Action? OnClose { get; set; }

    // Editable local copies — populated when panel opens
    private string _titleFilter = string.Empty;
    private string _nameFilter = string.Empty;
    private bool _sortDescending;
    private int _gap;
    private int _topPad;
    private int _dragCooldownMs;
    private bool _suspendOnMax;
    private string _nxsFolder = string.Empty;
    private string _recordingFolder = string.Empty;
    private string _ffmpegPath = string.Empty;
    private int _recordingFps;
    private bool _enableTemplateMatchingFallback;
    private bool _enableYoloEngine;

    private SettingsPanelState? _lastLoaded;

    public void Render(OverlayState state)
    {
        if (!state.IsSettingsPanelOpen)
        {
            _lastLoaded = null;
            return;
        }

        // Populate locals from state on first open
        if (_lastLoaded != state.Settings)
        {
            LoadFromState(state.Settings);
        }

        ImGui.SetNextWindowSize(new Vector2(700, 500), ImGuiCond.FirstUseEver);
        ImGui.SetNextWindowBgAlpha(0.96f);
        var open = true;
        if (!ImGui.Begin("Настройки##settings", ref open,
                ImGuiWindowFlags.NoCollapse | ImGuiWindowFlags.NoScrollWithMouse))
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

        if (ImGui.BeginTabBar("##settingsTabs"))
        {
            if (ImGui.BeginTabItem("Фильтры"))
            {
                ImGui.Spacing();
                ImGui.InputText("Фильтр по заголовку##titleFilter", ref _titleFilter, 512);
                ImGui.InputText("Фильтр по имени##nameFilter", ref _nameFilter, 512);
                ImGui.Checkbox("Сортировка по убыванию##sortDesc", ref _sortDescending);
                ImGui.EndTabItem();
            }

            if (ImGui.BeginTabItem("Раскладка"))
            {
                ImGui.Spacing();
                ImGui.InputInt("Отступ (Gap)##gap", ref _gap);
                ImGui.InputInt("Верхний отступ (TopPad)##topPad", ref _topPad);
                ImGui.InputInt("Задержка перетаскивания (мс)##dragCooldown", ref _dragCooldownMs);
                ImGui.Checkbox("Приостановить при максимизации##suspendOnMax", ref _suspendOnMax);
                ImGui.EndTabItem();
            }

            if (ImGui.BeginTabItem("Пути"))
            {
                ImGui.Spacing();
                ImGui.InputText("Папка NXS##nxsFolder", ref _nxsFolder, 1024);
                ImGui.InputText("Папка записей##recFolder", ref _recordingFolder, 1024);
                ImGui.InputText("Путь к FFmpeg##ffmpegPath", ref _ffmpegPath, 1024);
                ImGui.InputInt("FPS записи##fps", ref _recordingFps);
                ImGui.EndTabItem();
            }

            if (ImGui.BeginTabItem("Vision"))
            {
                ImGui.Spacing();
                ImGui.Checkbox("Откат к сопоставлению шаблонов##templateFallback", ref _enableTemplateMatchingFallback);
                ImGui.Checkbox("Движок YOLO##yoloEngine", ref _enableYoloEngine);
                ImGui.EndTabItem();
            }

            ImGui.EndTabBar();
        }

        ImGui.Spacing();
        ImGui.Separator();
        ImGui.Spacing();

        if (ImGui.Button("Сохранить##settingsSave"))
        {
            var saved = new SettingsPanelState(
                _titleFilter, _nameFilter, _sortDescending,
                _gap, _topPad, _dragCooldownMs, _suspendOnMax,
                _nxsFolder, _recordingFolder, _ffmpegPath, _recordingFps,
                _enableTemplateMatchingFallback, _enableYoloEngine);
            OnSave?.Invoke(saved);
        }

        ImGui.SameLine();

        if (ImGui.Button("Отмена##settingsCancel"))
        {
            OnClose?.Invoke();
        }

        ImGui.End();
    }

    private void LoadFromState(SettingsPanelState s)
    {
        _titleFilter = s.TitleFilter;
        _nameFilter = s.NameFilter;
        _sortDescending = s.SortDescending;
        _gap = s.Gap;
        _topPad = s.TopPad;
        _dragCooldownMs = s.DragCooldownMs;
        _suspendOnMax = s.SuspendOnMax;
        _nxsFolder = s.NxsFolder;
        _recordingFolder = s.RecordingFolder;
        _ffmpegPath = s.FfmpegPath;
        _recordingFps = s.RecordingFps;
        _enableTemplateMatchingFallback = s.EnableTemplateMatchingFallback;
        _enableYoloEngine = s.EnableYoloEngine;
        _lastLoaded = s;
    }
}
