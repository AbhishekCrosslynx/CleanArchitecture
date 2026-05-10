using Blazored.LocalStorage;

namespace Shared.Http;

/// <summary>
/// HTTP message handler that intercepts all HTTP requests and adds JWT authentication token.
/// This is the Blazor equivalent of Angular's HTTP Interceptor.
/// Registered as a DelegatingHandler in DI container.
/// </summary>
public class AuthenticationHttpMessageHandler : DelegatingHandler
{
    private readonly ILocalStorageService _localStorage;

    public AuthenticationHttpMessageHandler(ILocalStorageService localStorage)
    {
        _localStorage = localStorage;
    }

    protected override async Task<HttpResponseMessage> SendAsync(
        HttpRequestMessage request,
        CancellationToken cancellationToken)
    {
        // Try to get JWT token from local storage
        string? token = await _localStorage.GetItemAsync<string>(StorageKeys.AuthToken, cancellationToken);

        // If token exists, add it to Authorization header
        if (!string.IsNullOrWhiteSpace(token))
        {
            request.Headers.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);
        }

        // Continue with the request
        return await base.SendAsync(request, cancellationToken);
    }
}
