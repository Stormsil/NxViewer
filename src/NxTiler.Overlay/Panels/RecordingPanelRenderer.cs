using ImGuiNET;
using NxTiler.Domain.Enums;
using NxTiler.Overlay.State;
using System.Numerics;

namespace NxTiler.Overlay.Panels;

/// <summary>
/// ImGui recording control panel: replaces WPF RecordingPage.
/// Shows recording state, elapsed time, and Start/Pause/Stop/Discard controls.
/// </summary>
public sealed class RecordingPanelRenderer
{
    private static readonly uint ColorIdle = ImGui.ColorConvertFloat4ToU32(new Vector4(0.6f, 0.6f, 0.6f, 1.0f));
    private static readonly uint ColorRecording = ImGui.ColorConvertFloat4ToU32(new Vector4(1.0f, 0.2f, 0.2f, 1.0f));
    private static readonly uint ColorPaused = ImGui.ColorConvertFloat4ToU32(new Vector4(1.0f, 0.8f, 0.2f, 1.0f));

    public Action? OnStart { get; set; }
    public Action? OnPauseResume { get; set; }
    public Action? OnStopAndSave { get; set; }
    public Action? OnStopAndDiscard { get; set; }
    public Action? OnClose { get; set; }

    public void Render(OverlayState state)
    {
        if (!state.IsRecordingPanelOpen)
        {
            return;
        }

        ImGui.SetNextWindowSize(new Vector2(380, 260), ImGuiCond.FirstUseEver);
        ImGui.SetNextWindowBgAlpha(0.96f);
        var open = true;
        if (!ImGui.Begin("Запись##recording", ref open, ImGuiWindowFlags.NoCollapse))
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

        // State badge
        var (stateText, stateColor) = state.Recording switch
        {
            RecordingState.Idle => ("Ожидание", ColorIdle),
            RecordingState.Recording => ("● Запись", ColorRecording),
            RecordingState.Paused => ("⏸ Пауза", ColorPaused),
            RecordingState.MaskEditing => ("Редактирование маски", ColorPaused),
            _ => ("—", ColorIdle),
        };

        ImGui.PushStyleColor(ImGuiCol.Text, stateColor);
        ImGui.TextUnformatted(stateText);
        ImGui.PopStyleColor();

        // Elapsed time
        if (state.Recording != RecordingState.Idle)
        {
            ImGui.SameLine();
            var el = state.RecordingElapsed;
            ImGui.Text($"  {el:mm\\:ss}");
        }

        ImGui.Spacing();
        ImGui.Separator();
        ImGui.Spacing();

        // Action buttons
        if (state.Recording == RecordingState.Idle)
        {
            if (ImGui.Button("Начать запись##recStart", new Vector2(-1, 36)))
            {
                OnStart?.Invoke();
            }
        }
        else
        {
            var pauseLabel = state.Recording == RecordingState.Paused
                ? "Продолжить##recResume"
                : "Пауза##recPause";

            if (ImGui.Button(pauseLabel, new Vector2(-1, 36)))
            {
                OnPauseResume?.Invoke();
            }

            ImGui.Spacing();

            if (ImGui.Button("Стоп и Сохранить##recSave", new Vector2(-1, 36)))
            {
                OnStopAndSave?.Invoke();
            }

            ImGui.Spacing();

            if (ImGui.Button("Стоп и Отмена##recDiscard", new Vector2(-1, 36)))
            {
                OnStopAndDiscard?.Invoke();
            }
        }

        ImGui.End();
    }
}
