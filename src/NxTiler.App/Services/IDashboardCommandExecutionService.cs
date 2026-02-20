namespace NxTiler.App.Services;

public interface IDashboardCommandExecutionService
{
    Task ExecuteAsync(
        Func<CancellationToken, Task> action,
        Func<bool> canStart,
        Action<bool> setBusy,
        Action<string> setStatus,
        CancellationToken ct = default);
}
