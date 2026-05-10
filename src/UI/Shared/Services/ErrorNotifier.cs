using MudBlazor;

namespace Shared.Services;

/// <summary>
/// Global error notifier service.
/// Displays error messages in MudBlazor snackbar/toast notifications.
/// Injected into ErrorHandlingHttpMessageHandler to show HTTP errors globally.
/// </summary>
public class ErrorNotifier
{
    private readonly ISnackbar _snackbar;

    public ErrorNotifier(ISnackbar snackbar)
    {
        _snackbar = snackbar;
    }

    /// <summary>
    /// Show error message as a MudBlazor snackbar.
    /// </summary>
    public void ShowError(string message)
    {
        _snackbar.Add(message, Severity.Error, config =>
        {
            config.VisibleStateDuration = 5000; // 5 seconds
            config.HideTransitionDuration = 500;
            config.ShowTransitionDuration = 500;
            config.ShowCloseIcon = true;
            config.CloseAfterNavigation = false;
        });
    }

    /// <summary>
    /// Show success message as a MudBlazor snackbar.
    /// </summary>
    public void ShowSuccess(string message)
    {
        _snackbar.Add(message, Severity.Success, config =>
        {
            config.VisibleStateDuration = 3000; // 3 seconds
            config.HideTransitionDuration = 500;
            config.ShowTransitionDuration = 500;
            config.ShowCloseIcon = false;
        });
    }

    /// <summary>
    /// Show warning message as a MudBlazor snackbar.
    /// </summary>
    public void ShowWarning(string message)
    {
        _snackbar.Add(message, Severity.Warning, config =>
        {
            config.VisibleStateDuration = 4000; // 4 seconds
            config.HideTransitionDuration = 500;
            config.ShowTransitionDuration = 500;
            config.ShowCloseIcon = true;
        });
    }

    /// <summary>
    /// Show info message as a MudBlazor snackbar.
    /// </summary>
    public void ShowInfo(string message)
    {
        _snackbar.Add(message, Severity.Info, config =>
        {
            config.VisibleStateDuration = 3000;
            config.HideTransitionDuration = 500;
            config.ShowTransitionDuration = 500;
            config.ShowCloseIcon = false;
        });
    }
}
