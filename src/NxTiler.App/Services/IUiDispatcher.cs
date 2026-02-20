namespace NxTiler.App.Services;

public interface IUiDispatcher
{
    void Invoke(Action action);

    Task<T> InvokeAsync<T>(Func<Task<T>> action, CancellationToken ct = default);
}
