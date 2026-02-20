namespace NxTiler.App.Services;

public sealed partial class RecordingOverlayService
{
    private async Task InvokeOnUiAsync(Action action, CancellationToken ct)
    {
        await _uiDispatcher.InvokeAsync(() =>
        {
            ct.ThrowIfCancellationRequested();
            action();
            return Task.FromResult(true);
        }, ct);
    }
}
