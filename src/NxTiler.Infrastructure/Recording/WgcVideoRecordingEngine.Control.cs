namespace NxTiler.Infrastructure.Recording;

public sealed partial class WgcVideoRecordingEngine
{
    public Task PauseAsync(CancellationToken ct = default)
    {
        ct.ThrowIfCancellationRequested();
        _pauseGate.Reset();
        return Task.CompletedTask;
    }

    public Task ResumeAsync(CancellationToken ct = default)
    {
        ct.ThrowIfCancellationRequested();
        _pauseGate.Set();
        return Task.CompletedTask;
    }
}
