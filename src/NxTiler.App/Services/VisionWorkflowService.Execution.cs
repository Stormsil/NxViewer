namespace NxTiler.App.Services;

public sealed partial class VisionWorkflowService
{
    private async Task<T> ExecuteSerializedAsync<T>(Func<CancellationToken, Task<T>> operation, CancellationToken ct)
    {
        await _gate.WaitAsync(ct);
        try
        {
            return await operation(ct);
        }
        finally
        {
            _gate.Release();
        }
    }
}
