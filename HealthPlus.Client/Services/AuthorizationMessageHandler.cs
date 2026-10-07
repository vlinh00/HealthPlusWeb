using System.Net.Http.Headers;

namespace HealthPlus.Client.Services;

public class AuthorizationMessageHandler : DelegatingHandler
{
    private readonly AuthService _authService;

    public AuthorizationMessageHandler(AuthService authService)
    {
        _authService = authService;
    }

    protected override Task<HttpResponseMessage> SendAsync(
        HttpRequestMessage request,
        CancellationToken cancellationToken)
    {
        if (!string.IsNullOrWhiteSpace(_authService.Token))
        {
            request.Headers.Authorization =
                new AuthenticationHeaderValue(
                    "Bearer",
                    _authService.Token);
        }

        return base.SendAsync(request, cancellationToken);
    }
}