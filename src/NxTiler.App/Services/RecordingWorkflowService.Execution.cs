namespace NxTiler.App.Services;

public sealed partial class RecordingWorkflowService
{
    private async Task ExecuteSerializedAsync(Func<CancellationToken, Task> operation, CancellationToken ct)
    {
        await _workflowGate.WaitAsync(ct);
        try
        {
            await operation(ct);
        }
        finally
        {
            _workflowGate.Release();
        }
    }

    private async Task<T> ExecuteSerializedAsync<T>(Func<CancellationToken, Task<T>> operation, CancellationToken ct)
    {
        await _workflowGate.WaitAsync(ct);
        try
        {
            return await operation(ct);
        }
        finally
        {
            _workflowGate.Release();
        }
    }
}
