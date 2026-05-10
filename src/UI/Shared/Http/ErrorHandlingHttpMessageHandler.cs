using Shared.Services;
using System.Net;

namespace Shared.Http;

/// <summary>
/// Global HTTP error handling interceptor.
/// Intercepts ALL HTTP responses and automatically shows error notifications
/// for non-success status codes (4xx, 5xx).
/// 
/// This DelegatingHandler is chained AFTER AuthenticationHttpMessageHandler,
/// so the flow is:
/// 1. AuthenticationHttpMessageHandler adds JWT token
/// 2. Request goes to API
/// 3. ErrorHandlingHttpMessageHandler checks response
/// 4. If error, shows snackbar notification
/// 
/// NO service or state container needs manual error handling!
/// </summary>
public class ErrorHandlingHttpMessageHandler : DelegatingHandler
{
    private readonly ErrorNotifier _errorNotifier;

    public ErrorHandlingHttpMessageHandler(ErrorNotifier errorNotifier)
    {
        _errorNotifier = errorNotifier;
    }

    protected override async Task<HttpResponseMessage> SendAsync(
        HttpRequestMessage request,
        CancellationToken cancellationToken)
    {
        HttpResponseMessage? response = null;

        try
        {
            // Execute the request (passes through AuthenticationHttpMessageHandler first)
            response = await base.SendAsync(request, cancellationToken);

            // Check if response is success (2xx)
            if (!response.IsSuccessStatusCode)
            {
                // Show user-friendly error message based on status code
                await HandleErrorResponseAsync(response, cancellationToken);

                // Still throw so services know the request failed
                response.EnsureSuccessStatusCode();
            }

            return response;
        }
        catch (HttpRequestException)
        {
            // Re-throw - error already shown via HandleErrorResponseAsync
            throw;
        }
        catch (TaskCanceledException) when (!cancellationToken.IsCancellationRequested)
        {
            // Request timed out (not user-cancelled)
            _errorNotifier.ShowError("Request timed out. Please try again.");
            throw new HttpRequestException("Request timed out", null, HttpStatusCode.RequestTimeout);
        }
        catch (Exception ex)
        {
            // Network errors, DNS failures, etc.
            _errorNotifier.ShowError($"Network error: {ex.Message}");
            throw;
        }
    }

    /// <summary>
    /// Show user-friendly error message based on HTTP status code.
    /// </summary>
    private async Task HandleErrorResponseAsync(
        HttpResponseMessage response,
        CancellationToken cancellationToken)
    {
        string? errorDetail = null;

        // Try to read error message from response body (if API returns one)
        try
        {
            errorDetail = await response.Content.ReadAsStringAsync(cancellationToken);
            if (string.IsNullOrWhiteSpace(errorDetail) || errorDetail.Length > 200)
            {
                errorDetail = null; // Ignore empty or huge responses
            }
        }
        catch
        {
            // Ignore - use default message
        }

        // Get user-friendly message based on status code
        string message = response.StatusCode switch
        {
            // 4xx Client Errors
            HttpStatusCode.BadRequest => errorDetail ?? "Invalid request. Please check your input.",
            HttpStatusCode.Unauthorized => "You are not logged in. Please log in to continue.",
            HttpStatusCode.Forbidden => "You don't have permission to perform this action.",
            HttpStatusCode.NotFound => "The requested resource was not found.",
            HttpStatusCode.Conflict => errorDetail ?? "This operation conflicts with existing data.",
            HttpStatusCode.UnprocessableEntity => errorDetail ?? "Validation failed. Please check your input.",

            // 5xx Server Errors
            HttpStatusCode.InternalServerError => "Server error. Please try again later.",
            HttpStatusCode.BadGateway => "Server is temporarily unavailable. Please try again.",
            HttpStatusCode.ServiceUnavailable => "Service is under maintenance. Please try again later.",
            HttpStatusCode.GatewayTimeout => "Server timed out. Please try again.",

            // Default
            _ => $"Request failed ({(int)response.StatusCode}). Please try again."
        };

        _errorNotifier.ShowError(message);
    }
}
