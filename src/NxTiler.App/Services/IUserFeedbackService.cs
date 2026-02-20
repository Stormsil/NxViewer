namespace NxTiler.App.Services;

public interface IUserFeedbackService
{
    void Info(string title, string message);

    void Success(string title, string message);

    void Warning(string title, string message);

    void Error(string title, string message);

    Task<bool> ConfirmDangerAsync(
        string title,
        string message,
        string primaryButtonText,
        CancellationToken ct = default);
}
