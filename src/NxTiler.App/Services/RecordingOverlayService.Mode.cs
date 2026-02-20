namespace NxTiler.App.Services;

public sealed partial class RecordingOverlayService
{
    public Task EnterRecordingModeAsync(CancellationToken ct = default) => Task.CompletedTask;

    public Task EnterPauseEditModeAsync(CancellationToken ct = default) => Task.CompletedTask;

    public Task ReEnterRecordingModeAsync(CancellationToken ct = default) => Task.CompletedTask;

    public Task ShowStatusAsync(string message, CancellationToken ct = default) => Task.CompletedTask;
}
