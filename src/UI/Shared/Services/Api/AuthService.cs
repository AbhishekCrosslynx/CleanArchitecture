using System.Net.Http.Json;
using SharedContracts.ApiRoutes;
using SharedContracts.DTOs.Users.Requests;
using SharedContracts.DTOs.Users.Responses;

namespace Shared.Services.Api;

/// <summary>
/// Provides authentication and user management operations, including user login, registration, and retrieval by
/// identifier.
/// </summary>
/// <remarks>This service is intended to be used as a client for authentication-related API endpoints. It requires
/// an initialized <see cref="HttpClient"/> instance, typically configured with the base address of the authentication
/// server. All methods are asynchronous and should be awaited. The service does not cache results; each method call
/// issues a new HTTP request.</remarks>
public class AuthService
{
    private readonly HttpClient _httpClient;

    public AuthService(HttpClient httpClient)
    {
        _httpClient = httpClient;
    }

    /// <summary>
    /// Authenticates a user asynchronously using the provided login request and returns a JWT access token.
    /// </summary>
    /// <remarks>The returned token should be included in subsequent requests that require authentication. If
    /// authentication fails, an exception may be thrown by the underlying HTTP client.</remarks>
    /// <param name="request">The login request containing user credentials and any additional authentication parameters. Cannot be null.</param>
    /// <returns>A string containing the JWT access token issued upon successful authentication.</returns>
    public async Task<string> LoginAsync(LoginUserRequest request)
    {
        HttpResponseMessage response = await _httpClient.PostAsJsonAsync(AuthRoutes.Login, request);
        string token = await response.Content.ReadAsStringAsync();
        return token.Trim('"');
    }

    /// <summary>
    /// Registers a new user asynchronously using the specified registration request.
    /// </summary>
    /// <param name="request">The registration details for the new user. Cannot be null.</param>
    /// <returns>A <see cref="Guid"/> representing the unique identifier of the newly registered user.</returns>
    /// <exception cref="InvalidOperationException">Thrown if the server response does not contain a valid user identifier.</exception>
    public async Task<Guid> RegisterAsync(RegisterUserRequest request)
    {
        HttpResponseMessage response = await _httpClient.PostAsJsonAsync(AuthRoutes.Register, request);
        string content = await response.Content.ReadAsStringAsync();
        return Guid.TryParse(content.Trim('"'), out Guid id)
            ? id
            : throw new InvalidOperationException("Invalid userId returned from server.");
    }


    /// <summary>
    /// Retrieves the profile information for the currently authenticated user.
    /// </summary>
    /// <remarks>The request is made using the credentials associated with the current HTTP client instance. Ensure
    /// that the client is properly authenticated before calling this method.</remarks>
    /// <returns>A task that represents the asynchronous operation. The task result contains a <see cref="UserResponse"/> object with
    /// the user's profile data.</returns>
    public async Task<UserResponse> GetMyProfileAsync()
    {
        HttpResponseMessage response = await _httpClient.GetAsync(AuthRoutes.Profile);
        return await response.Content.ReadFromJsonAsync<UserResponse>();
    }
}
