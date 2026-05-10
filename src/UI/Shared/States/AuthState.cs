using Blazored.LocalStorage;
using Shared.Services.Api;
using SharedContracts.DTOs.Users.Requests;
using SharedContracts.DTOs.Users.Responses;

namespace Shared.States;

public class AuthState
{
    private readonly AuthService _authService;
    private readonly ILocalStorageService _localStorage;

    public bool IsLoading { get; private set; }
    public string? Error { get; private set; }

    private void SetLoading(bool value)
    {
        IsLoading = value;
        Notify();
    }

    private void SetError(string? error)
    {
        Error = error;
        Notify();
    }

    public bool IsAuthenticated { get; private set; }
    public string? UserName { get; private set; }
    public UserResponse? CurrentUser { get; private set; }

    public event EventHandler? OnChange;

    public AuthState(AuthService authService, ILocalStorageService localStorage)
    {
        _authService = authService;
        _localStorage = localStorage;
    }

    public async Task<bool> LoginAsync(LoginUserRequest request)
    {
        try
        {
            SetLoading(true);
            SetError(null);

            string token = await _authService.LoginAsync(request);

            await _localStorage.SetItemAsync(StorageKeys.AuthToken, token);

            CurrentUser = await _authService.GetMyProfileAsync();
            IsAuthenticated = true;
            UserName = CurrentUser?.Email;

            Notify();
            return true;
        }
        catch (Exception ex)
        {
            SetError(ex.Message);
            return false;
        }
        finally
        {
            SetLoading(false);
        }
    }

    public async Task LoadProfile()
    {
        CurrentUser = await _authService.GetMyProfileAsync();

        UserName = CurrentUser?.Email;
        IsAuthenticated = CurrentUser != null;

        Notify();
    }

    public async Task Logout()
    {
        await _localStorage.RemoveItemAsync(StorageKeys.AuthToken);

        IsAuthenticated = false;
        UserName = null;
        CurrentUser = null;

        Notify();
    }

    public async Task InitializeAsync()
    {
        string? token = await _localStorage.GetItemAsync<string>(StorageKeys.AuthToken);

        if (string.IsNullOrWhiteSpace(token))
        {
            IsAuthenticated = false;
            Notify();
            return;
        }

        try
        {
            CurrentUser = await _authService.GetMyProfileAsync();
            IsAuthenticated = CurrentUser != null;
            UserName = CurrentUser?.Email;
        }
        catch
        {
            // token invalid or expired
            await _localStorage.RemoveItemAsync(StorageKeys.AuthToken);
            IsAuthenticated = false;
        }

        Notify();
    }

    private void Notify()
    {
        OnChange?.Invoke(this, EventArgs.Empty);
    }
}
