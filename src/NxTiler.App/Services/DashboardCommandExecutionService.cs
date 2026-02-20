using Microsoft.Extensions.Logging;

namespace NxTiler.App.Services;

public sealed class DashboardCommandExecutionService(
    IUserFeedbackService userFeedbackService,
    ILogger<DashboardCommandExecutionService> logger) : IDashboardCommandExecutionService
{
    public async Task ExecuteAsync(
        Func<CancellationToken, Task> action,
        Func<bool> canStart,
        Action<bool> setBusy,
        Action<string> setStatus,
        CancellationToken ct = default)
    {
        if (!canStart())
        {
            return;
        }

        setBusy(true);
        try
        {
            await action(ct);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Dashboard command failed.");
            setStatus(ex.Message);
            userFeedbackService.Error("Command failed", ex.Message);
        }
        finally
        {
            setBusy(false);
        }
    }
}
