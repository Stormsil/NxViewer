using ImGuiNET;
using NxTiler.Domain.Settings;
using NxTiler.Overlay.State;
using System.Numerics;
using System.Runtime.InteropServices;
using System.Text;

namespace NxTiler.Overlay.Panels;

/// <summary>
/// ImGui hotkeys panel: replaces WPF HotkeysPage.
/// Shows all hotkey bindings; clicking [Изменить] enters key-capture mode.
/// </summary>
public sealed class HotkeysPanelRenderer
{
    [DllImport("user32.dll")]
    private static extern short GetAsyncKeyState(int vKey);

    private static readonly string[] ActionNames =
    {
        "Показать/скрыть оверлей",
        "Показать/скрыть окно",
        "Переключить режим",
        "Свернуть/развернуть",
        "Предыдущее окно",
        "Следующее окно",
        "Снимок экрана",
        "Снимок региона",
        "Запись",
        "Пауза",
        "Стоп",
        "Vision",
    };

    public Action<HotkeysPanelState>? OnSave { get; set; }
    public Action? OnClose { get; set; }

    private HotkeyBinding[] _editedBindings = new HotkeyBinding[12];
    private int _listeningIndex = -1;
    private readonly bool[] _prevKeyState = new bool[256];
    private HotkeysPanelState? _lastLoaded;

    public void Render(OverlayState state)
    {
        if (!state.IsHotkeysPanelOpen)
        {
            _lastLoaded = null;
            _listeningIndex = -1;
            return;
        }

        if (_lastLoaded != state.Hotkeys)
        {
            LoadFromState(state.Hotkeys);
        }

        ImGui.SetNextWindowSize(new Vector2(520, 440), ImGuiCond.FirstUseEver);
        ImGui.SetNextWindowBgAlpha(0.96f);
        var open = true;
        if (!ImGui.Begin("Хоткеи##hotkeys", ref open, ImGuiWindowFlags.NoCollapse))
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

        // Key capture modal
        if (_listeningIndex >= 0)
        {
            var detected = TryDetectKeyPress();
            if (detected.HasValue)
            {
                if (detected.Value.vk == 0x1B) // ESC — cancel
                {
                    _listeningIndex = -1;
                }
                else
                {
                    _editedBindings[_listeningIndex] = new HotkeyBinding(detected.Value.mods, detected.Value.vk);
                    _listeningIndex = -1;
                }
            }

            ImGui.OpenPopup("##capturePopup");
        }

        var dummy = true;
        if (ImGui.BeginPopupModal("##capturePopup", ref dummy,
                ImGuiWindowFlags.AlwaysAutoResize | ImGuiWindowFlags.NoTitleBar))
        {
            ImGui.Text("Нажмите клавишу...");
            ImGui.Text("(ESC для отмены)");
            if (_listeningIndex < 0)
            {
                ImGui.CloseCurrentPopup();
            }

            ImGui.EndPopup();
        }

        // Hotkey table
        if (ImGui.BeginTable("##hotkeyTable", 3,
                ImGuiTableFlags.Borders | ImGuiTableFlags.RowBg | ImGuiTableFlags.SizingStretchProp))
        {
            ImGui.TableSetupColumn("Действие", ImGuiTableColumnFlags.WidthStretch);
            ImGui.TableSetupColumn("Хоткей", ImGuiTableColumnFlags.WidthFixed, 160);
            ImGui.TableSetupColumn("##edit", ImGuiTableColumnFlags.WidthFixed, 80);
            ImGui.TableHeadersRow();

            for (var i = 0; i < ActionNames.Length; i++)
            {
                ImGui.TableNextRow();
                ImGui.TableSetColumnIndex(0);
                ImGui.TextUnformatted(ActionNames[i]);

                ImGui.TableSetColumnIndex(1);
                ImGui.TextUnformatted(BindingToString(_editedBindings[i]));

                ImGui.TableSetColumnIndex(2);
                if (ImGui.SmallButton($"Изменить##{i}"))
                {
                    _listeningIndex = i;
                    ResetKeyStateTracking();
                }
            }

            ImGui.EndTable();
        }

        ImGui.Spacing();
        ImGui.Separator();
        ImGui.Spacing();

        if (ImGui.Button("Сохранить##hotkeySave"))
        {
            var saved = new HotkeysPanelState(
                _editedBindings[0], _editedBindings[1], _editedBindings[2],
                _editedBindings[3], _editedBindings[4], _editedBindings[5],
                _editedBindings[6], _editedBindings[7], _editedBindings[8],
                _editedBindings[9], _editedBindings[10], _editedBindings[11]);
            OnSave?.Invoke(saved);
        }

        ImGui.SameLine();

        if (ImGui.Button("Отмена##hotkeyCancel"))
        {
            OnClose?.Invoke();
        }

        ImGui.End();
    }

    private void LoadFromState(HotkeysPanelState s)
    {
        _editedBindings[0] = s.ToggleOverlay;
        _editedBindings[1] = s.ToggleMainWindow;
        _editedBindings[2] = s.CycleMode;
        _editedBindings[3] = s.ToggleMinimize;
        _editedBindings[4] = s.NavigatePrevious;
        _editedBindings[5] = s.NavigateNext;
        _editedBindings[6] = s.InstantSnapshot;
        _editedBindings[7] = s.RegionSnapshot;
        _editedBindings[8] = s.Record;
        _editedBindings[9] = s.Pause;
        _editedBindings[10] = s.Stop;
        _editedBindings[11] = s.ToggleVision;
        _lastLoaded = s;
    }

    private void ResetKeyStateTracking()
    {
        for (var i = 0; i < _prevKeyState.Length; i++)
        {
            _prevKeyState[i] = (GetAsyncKeyState(i) & 0x8000) != 0;
        }
    }

    private (int vk, uint mods)? TryDetectKeyPress()
    {
        uint mods = 0;
        if ((GetAsyncKeyState(0x11) & 0x8000) != 0) mods |= 0x0002; // Ctrl
        if ((GetAsyncKeyState(0x10) & 0x8000) != 0) mods |= 0x0004; // Shift
        if ((GetAsyncKeyState(0x12) & 0x8000) != 0) mods |= 0x0001; // Alt

        int? detected = null;
        for (var vk = 0x08; vk <= 0xFE; vk++)
        {
            // Skip modifier keys
            if (vk is 0x10 or 0x11 or 0x12 or 0x5B or 0x5C or
                      0xA0 or 0xA1 or 0xA2 or 0xA3 or 0xA4 or 0xA5)
            {
                _prevKeyState[vk] = (GetAsyncKeyState(vk) & 0x8000) != 0;
                continue;
            }

            var down = (GetAsyncKeyState(vk) & 0x8000) != 0;
            if (down && !_prevKeyState[vk])
            {
                detected = vk;
            }

            _prevKeyState[vk] = down;
        }

        if (detected.HasValue)
        {
            return (detected.Value, mods);
        }

        return null;
    }

    private static string BindingToString(HotkeyBinding b)
    {
        if (b.IsEmpty) return "—";
        var sb = new StringBuilder();
        if ((b.Modifiers & 0x0008) != 0) sb.Append("Win+");
        if ((b.Modifiers & 0x0001) != 0) sb.Append("Alt+");
        if ((b.Modifiers & 0x0004) != 0) sb.Append("Shift+");
        if ((b.Modifiers & 0x0002) != 0) sb.Append("Ctrl+");
        sb.Append(VkToString(b.VirtualKey));
        return sb.ToString();
    }

    private static string VkToString(int vk) => vk switch
    {
        0x08 => "Backspace", 0x09 => "Tab", 0x0D => "Enter",
        0x1B => "Esc", 0x20 => "Space",
        0x21 => "PgUp", 0x22 => "PgDn", 0x23 => "End", 0x24 => "Home",
        0x25 => "Left", 0x26 => "Up", 0x27 => "Right", 0x28 => "Down",
        0x2C => "PrtSc", 0x2D => "Ins", 0x2E => "Del",
        >= 0x30 and <= 0x39 => ((char)vk).ToString(),
        >= 0x41 and <= 0x5A => ((char)vk).ToString(),
        0x70 => "F1", 0x71 => "F2", 0x72 => "F3", 0x73 => "F4",
        0x74 => "F5", 0x75 => "F6", 0x76 => "F7", 0x77 => "F8",
        0x78 => "F9", 0x79 => "F10", 0x7A => "F11", 0x7B => "F12",
        0xBA => ";", 0xBB => "=", 0xBC => ",", 0xBD => "-",
        0xBE => ".", 0xBF => "/", 0xC0 => "`", 0xDB => "[",
        0xDC => "\\", 0xDD => "]", 0xDE => "'",
        _ => $"VK({vk:X2})",
    };
}
