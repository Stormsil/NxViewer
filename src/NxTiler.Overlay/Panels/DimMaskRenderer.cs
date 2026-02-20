using ImGuiNET;
using NxTiler.Overlay.State;
using System.Numerics;

namespace NxTiler.Overlay.Panels;

/// <summary>
/// Screen dimming overlay with cutout masks.
/// Replaces WPF DimOverlayWindow + MaskOverlayWindow.
/// </summary>
public sealed class DimMaskRenderer
{
    private static readonly uint DimColor = ImGui.ColorConvertFloat4ToU32(new Vector4(0f, 0f, 0f, 0.65f));
    private static readonly uint CutoutColor = ImGui.ColorConvertFloat4ToU32(new Vector4(0f, 0f, 0f, 0f));

    public void Render(OverlayState state)
    {
        if (!state.IsDimMaskVisible)
        {
            return;
        }

        var io = ImGui.GetIO();
        var screenSize = io.DisplaySize;

        ImGui.SetNextWindowPos(Vector2.Zero);
        ImGui.SetNextWindowSize(screenSize);
        ImGui.SetNextWindowBgAlpha(0f);

        ImGui.Begin("##DimMask",
            ImGuiWindowFlags.NoDecoration
            | ImGuiWindowFlags.NoMove
            | ImGuiWindowFlags.NoSavedSettings
            | ImGuiWindowFlags.NoNav
            | ImGuiWindowFlags.NoInputs);

        var drawList = ImGui.GetWindowDrawList();

        // Full-screen dim
        drawList.AddRectFilled(Vector2.Zero, screenSize, DimColor);

        // Cut out each mask region (override with transparent)
        foreach (var mask in state.Masks)
        {
            var maskMin = new Vector2(mask.X, mask.Y);
            var maskMax = new Vector2(mask.X + mask.Width, mask.Y + mask.Height);
            drawList.AddRectFilled(maskMin, maskMax, CutoutColor);
        }

        ImGui.End();
    }
}
