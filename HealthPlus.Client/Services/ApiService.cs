using System.Net.Http.Headers;
using System.Net.Http.Json;
using HealthPlus.Client.Services;
using Microsoft.AspNetCore.Components.Forms;

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


    public async Task<TResponse?> UploadFileAsync<TResponse>(
        string path,
        IBrowserFile file,
        CancellationToken cancellationToken = default)
    {
        using var form = new MultipartFormDataContent();

        var stream = file.OpenReadStream(
            maxAllowedSize: 5 * 1024 * 1024,
            cancellationToken);

        using (stream)
        using (var fileContent = new StreamContent(stream))
        {
            fileContent.Headers.ContentType =
                new System.Net.Http.Headers.MediaTypeHeaderValue(
                    file.ContentType);

            form.Add(fileContent, "file", file.Name);

            using var request = new HttpRequestMessage(
                HttpMethod.Post, path)
            {
                Content = form
            };

            AddAuthorizationHeader(request);

            using var response = await _httpClient.SendAsync(
                request, cancellationToken);

            var result = await response.Content
                .ReadFromJsonAsync<TResponse>(
                    cancellationToken: cancellationToken);

            if (!response.IsSuccessStatusCode)
            {
                var message = result is not null
                    ? System.Text.Json.JsonSerializer.Serialize(result)
                    : $"Upload thất bại: {(int)response.StatusCode}";

                throw new HttpRequestException(message);
            }

            return result;
        }
    }

}