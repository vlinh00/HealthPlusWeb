using System.Net.Http.Headers;
using System.Net.Http.Json;
using HealthPlus.Client.Services;

namespace HealthPlus.Client.Services;

public class ApiService
{
    private readonly HttpClient _httpClient;
    private readonly AuthService _authService;

    public ApiService(
        HttpClient httpClient,
        AuthService authService)
    {
        _httpClient = httpClient;
        _authService = authService;
    }

    private void AddAuthorizationHeader(HttpRequestMessage request)
    {
        if (!string.IsNullOrWhiteSpace(_authService.Token))
        {
            request.Headers.Authorization =
                new AuthenticationHeaderValue(
                    "Bearer",
                    _authService.Token);
        }
    }

    public async Task<T?> GetAsync<T>(string url)
    {
        using var request = new HttpRequestMessage(
            HttpMethod.Get,
            url);

        AddAuthorizationHeader(request);

        var response = await _httpClient.SendAsync(request);

        response.EnsureSuccessStatusCode();

        return await response.Content.ReadFromJsonAsync<T>();
    }

    public async Task<TResponse?> PostAsync<TRequest, TResponse>(
        string url,
        TRequest requestData)
    {
        using var request = new HttpRequestMessage(
            HttpMethod.Post,
            url)
        {
            Content = JsonContent.Create(requestData)
        };

        AddAuthorizationHeader(request);

        var response = await _httpClient.SendAsync(request);

        response.EnsureSuccessStatusCode();

        return await response.Content.ReadFromJsonAsync<TResponse>();
    }

    public async Task<TResponse?> PutAsync<TRequest, TResponse>(
        string url,
        TRequest requestData)
    {
        using var request = new HttpRequestMessage(
            HttpMethod.Put,
            url)
        {
            Content = JsonContent.Create(requestData)
        };

        AddAuthorizationHeader(request);

        var response = await _httpClient.SendAsync(request);

        response.EnsureSuccessStatusCode();

        return await response.Content.ReadFromJsonAsync<TResponse>();
    }

    public async Task DeleteAsync(string url)
    {
        using var request = new HttpRequestMessage(
            HttpMethod.Delete,
            url);

        AddAuthorizationHeader(request);

        var response = await _httpClient.SendAsync(request);

        response.EnsureSuccessStatusCode();
    }
}