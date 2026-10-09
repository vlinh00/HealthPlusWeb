using System.Security.Claims;
using System.Text;
using System.Text.Json;
using Microsoft.AspNetCore.Components.Authorization;

namespace HealthPlus.Client.Services;

public class CustomAuthenticationStateProvider
    : AuthenticationStateProvider
{
    private readonly AuthService _authService;

    private static readonly ClaimsPrincipal Anonymous =
        new(new ClaimsIdentity());

    public CustomAuthenticationStateProvider(
        AuthService authService)
    {
        _authService = authService;

        _authService.AuthenticationStateChanged +=
            OnAuthenticationStateChanged;
    }

    public override async Task<AuthenticationState>
        GetAuthenticationStateAsync()
    {
        await _authService.InitializeAsync();

        return BuildAuthenticationState();
    }

    private AuthenticationState BuildAuthenticationState()
    {
        if (string.IsNullOrWhiteSpace(_authService.Token))
        {
            return new AuthenticationState(Anonymous);
        }

        var claims = GetClaimsFromJwt(
            _authService.Token);

        var identity = new ClaimsIdentity(
            claims,
            authenticationType: "jwt");

        return new AuthenticationState(
            new ClaimsPrincipal(identity));
    }

    private void OnAuthenticationStateChanged()
    {
        NotifyAuthenticationStateChanged(
            Task.FromResult(
                BuildAuthenticationState()));
    }

    private static IEnumerable<Claim> GetClaimsFromJwt(
        string token)
    {
        var parts = token.Split('.');

        if (parts.Length != 3)
            return [];

        try
        {
            var payload = parts[1];

            var padded = payload
                .Replace('-', '+')
                .Replace('_', '/');

            switch (padded.Length % 4)
            {
                case 2:
                    padded += "==";
                    break;

                case 3:
                    padded += "=";
                    break;
            }

            var bytes = Convert.FromBase64String(padded);

            var json = Encoding.UTF8.GetString(bytes);

            var dictionary =
                JsonSerializer.Deserialize<
                    Dictionary<string, JsonElement>>(json);

            if (dictionary == null)
                return [];

            var claims = new List<Claim>();

            foreach (var item in dictionary)
            {
                if (item.Value.ValueKind ==
                    JsonValueKind.Array)
                {
                    foreach (var value in item.Value.EnumerateArray())
                    {
                        claims.Add(
                            new Claim(
                                MapClaimType(item.Key),
                                value.ToString()));
                    }
                }
                else
                {
                    claims.Add(
                        new Claim(
                            MapClaimType(item.Key),
                            item.Value.ToString()));
                }
            }

            return claims;
        }
        catch
        {
            return [];
        }
    }

    private static string MapClaimType(string claimType)
    {
        return claimType switch
        {
            "sub" =>
                ClaimTypes.NameIdentifier,

            "unique_name" =>
                ClaimTypes.Name,

            "name" =>
                ClaimTypes.Name,

            "email" =>
                ClaimTypes.Email,

            "role" =>
                ClaimTypes.Role,

            _ => claimType
        };
    }
}