using ImGuiNET;
using NxTiler.Domain.Enums;
using NxTiler.Overlay.State;
using System.Numerics;

namespace NxTiler.Overlay.Panels;

/// <summary>Recording status bar: timer + pause/resume + stop.</summary>
public sealed class RecordingBarRenderer
{
    public Action? OnPauseResume { get; set; }
    public Action? OnStop { get; set; }

    public void Render(OverlayState state)
    {
        if (state.Recording == RecordingState.Idle)
        {
            return;
        }

        var io = ImGui.GetIO();
        ImGui.SetNextWindowPos(new Vector2(io.DisplaySize.X / 2f, io.DisplaySize.Y - 60f),
            ImGuiCond.Always, new Vector2(0.5f, 0.5f));
        ImGui.SetNextWindowBgAlpha(0.9f);

        ImGui.Begin("##RecordingBar",
            ImGuiWindowFlags.NoDecoration
            | ImGuiWindowFlags.AlwaysAutoResize
            | ImGuiWindowFlags.NoMove
            | ImGuiWindowFlags.NoNav);

        var elapsed = state.RecordingElapsed;
        var isPaused = state.Recording == RecordingState.Paused;

        ImGui.PushStyleColor(ImGuiCol.Text,
            ImGui.ColorConvertFloat4ToU32(isPaused
                ? new Vector4(1f, 0.85f, 0.1f, 1f)
                : new Vector4(1f, 0.2f, 0.2f, 1f)));
        ImGui.Text($"● {elapsed:mm\\:ss}");
        ImGui.PopStyleColor();

        ImGui.SameLine();

        if (ImGui.SmallButton(isPaused ? "▶" : "⏸"))
        {
            OnPauseResume?.Invoke();
        }

        ImGui.SameLine();

        if (ImGui.SmallButton("⏹"))
        {
            OnStop?.Invoke();
        }

        ImGui.End();
    }
}
