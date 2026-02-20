namespace NxTiler.Application.Abstractions;

/// <summary>ImGui-based overlay render service (Phase 4 feature flag: UseImGuiOverlay).</summary>
public interface IOverlayRenderService
{
    void RenderFrame();

    void ShowPanel();

    void HidePanel();

    /// <summary>Thread-safe state update. Uses Interlocked.Exchange internally.</summary>
    void SetState(object state);
}
