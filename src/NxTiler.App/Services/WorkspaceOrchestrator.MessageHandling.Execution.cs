using Microsoft.Extensions.Logging;

namespace NxTiler.App.Services;

public sealed partial class WorkspaceOrchestrator
{
    private async Task ExecuteEventActionAsync(string operationName, Func<Task> action)
    {
        try
        {
            await action();
        }
        catch (OperationCanceledException)
        {
            _logger.LogDebug("Operation cancelled: {OperationName}", operationName);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Operation failed: {OperationName}", operationName);
        }
    }
}
