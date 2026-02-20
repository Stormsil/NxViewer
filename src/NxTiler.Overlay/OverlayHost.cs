using ClickableTransparentOverlay;

namespace NxTiler.Overlay;

/// <summary>
/// Hosts the ImGui overlay. Inherits from ClickableTransparentOverlay.Overlay.
/// Runs its render loop on a dedicated thread.
/// </summary>
public sealed class OverlayHost(OverlayRenderService renderService) : global::ClickableTransparentOverlay.Overlay
{
    protected override Task PostInitialized()
    {
        return Task.CompletedTask;
    }

    protected override void Render()
    {
        renderService.RenderFrame();
    }
}
