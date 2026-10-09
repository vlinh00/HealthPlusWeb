using System.Net.Http.Json;
using System.Text.Json;
using Microsoft.JSInterop;
using HealthPlus.Client.Models.Auth;
using HealthPlus.Client.Models.Common;

namespace HealthPlus.Client.Services;

public class AuthService
{
    private const string TokenKey = "healthplus_token";
    private const string UserKey = "healthplus_user";

    private readonly HttpClient _httpClient;
    private readonly IJSRuntime _js;

    public string? Token { get; private set; }

    public LoginResponse? CurrentUser { get; private set; }

    public bool IsAuthenticated =>
        !string.IsNullOrWhiteSpace(Token);

    public event Action? AuthenticationStateChanged;

    public AuthService(
        HttpClient httpClient,
        IJSRuntime js)
    {
        _httpClient = httpClient;
        _js = js;
    }

    public async Task InitializeAsync()
    {
        Token = await _js.InvokeAsync<string?>(
            "localStorage.getItem",
            TokenKey);

        var userJson = await _js.InvokeAsync<string?>(
            "localStorage.getItem",
            UserKey);

        if (!string.IsNullOrWhiteSpace(userJson))
        {
            CurrentUser =
                JsonSerializer.Deserialize<LoginResponse>(
                    userJson);
        }
    }

    public async Task<(bool Success, string Message)> LoginAsync(
        LoginRequest request)
    {
        var response = await _httpClient.PostAsJsonAsync(
            "api/auth/login",
            request);

        var result =
            await response.Content
                .ReadFromJsonAsync<ApiResponse<LoginResponse>>();

        if (!response.IsSuccessStatusCode)
        {
            return (
                false,
                result?.Message ?? "Login failed.");
        }

        if (result == null ||
            !result.Success ||
            result.Data == null)
        {
            return (
                false,
                result?.Message ?? "Login failed.");
        }

        Token = result.Data.Token;
        CurrentUser = result.Data;

        await _js.InvokeVoidAsync(
            "localStorage.setItem",
            TokenKey,
            Token);

        await _js.InvokeVoidAsync(
            "localStorage.setItem",
            UserKey,
            JsonSerializer.Serialize(CurrentUser));

        AuthenticationStateChanged?.Invoke();

        return (true, result.Message);
    }

    public async Task<(bool Success, string Message)> RegisterAsync(
        RegisterRequest request)
    {
        var response = await _httpClient.PostAsJsonAsync(
            "api/auth/register",
            request);

        var result =
            await response.Content
                .ReadFromJsonAsync<ApiResponse<object>>();

        if (!response.IsSuccessStatusCode)
        {
            return (
                false,
                result?.Message ?? "Registration failed.");
        }

        if (result == null || !result.Success)
        {
            return (
                false,
                result?.Message ?? "Registration failed.");
        }

        return (true, result.Message);
    }

    public async Task LogoutAsync()
    {
        Token = null;
        CurrentUser = null;

        await _js.InvokeVoidAsync(
            "localStorage.removeItem",
            TokenKey);

        await _js.InvokeVoidAsync(
            "localStorage.removeItem",
            UserKey);

        AuthenticationStateChanged?.Invoke();
    }
}