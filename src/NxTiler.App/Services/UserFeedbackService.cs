using Microsoft.Extensions.Logging;
using Wpf.Ui;
using Wpf.Ui.Controls;
using Wpf.Ui.Extensions;

namespace NxTiler.App.Services;

public sealed class UserFeedbackService : IUserFeedbackService
{
    private readonly ISnackbarService _snackbarService;
    private readonly IContentDialogService _contentDialogService;
    private readonly IUiDispatcher _uiDispatcher;
    private readonly ILogger<UserFeedbackService> _logger;

    public UserFeedbackService(
        ISnackbarService snackbarService,
        IContentDialogService contentDialogService,
        IUiDispatcher uiDispatcher,
        ILogger<UserFeedbackService> logger)
    {
        _snackbarService = snackbarService;
        _contentDialogService = contentDialogService;
        _uiDispatcher = uiDispatcher;
        _logger = logger;
    }

    public void Info(string title, string message) => Show(title, message, ControlAppearance.Info);

    public void Success(string title, string message) => Show(title, message, ControlAppearance.Success);

    public void Warning(string title, string message) => Show(title, message, ControlAppearance.Caution);

    public void Error(string title, string message) => Show(title, message, ControlAppearance.Danger);

    public async Task<bool> ConfirmDangerAsync(
        string title,
        string message,
        string primaryButtonText,
        CancellationToken ct = default)
    {
        try
        {
            return await _uiDispatcher.InvokeAsync(async () =>
            {
                var dialog = new ContentDialog
                {
                    Title = title,
                    Content = message,
                    PrimaryButtonText = primaryButtonText,
                    CloseButtonText = "Cancel",
                    PrimaryButtonAppearance = ControlAppearance.Danger,
                    CloseButtonAppearance = ControlAppearance.Secondary,
                };

                var result = await _contentDialogService.ShowAsync(dialog, ct);
                return result == ContentDialogResult.Primary;
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to show confirmation dialog.");
            return false;
        }
    }

    private void Show(string title, string message, ControlAppearance appearance)
    {
        _uiDispatcher.Invoke(() =>
        {
            try
            {
                _snackbarService.Show(title, message, appearance);
            }
            catch (Exception ex)
            {
                _logger.LogDebug(ex, "Unable to show snackbar.");
            }
        });
    }
}
