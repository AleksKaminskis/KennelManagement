using Blazored.LocalStorage;
using Client.Models;
using Microsoft.AspNetCore.Components.Authorization;
using System.Security.Claims;


namespace Client.Services;

public class CustomAuthenticationStateProvider : AuthenticationStateProvider
{
    private readonly ILocalStorageService _localStorage;
    private readonly HttpClient _httpClient;

    public CustomAuthenticationStateProvider(ILocalStorageService localStorage, HttpClient httpClient)
    {
        _localStorage = localStorage;
        _httpClient = httpClient;
    }

    public override async Task<AuthenticationState> GetAuthenticationStateAsync()
    {
        try
        {
            var token = await _localStorage.GetItemAsStringAsync("authToken");

            if (string.IsNullOrEmpty(token))
            {
                return new AuthenticationState(new ClaimsPrincipal(new ClaimsIdentity()));
            }

            var user = await _localStorage.GetItemAsync<AuthResponse>("authUser");
            if (user == null)
            {
                return new AuthenticationState(new ClaimsPrincipal(new ClaimsIdentity()));
            }

            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.Email, user.Email ?? string.Empty),
                new Claim(ClaimTypes.GivenName, user.FirstName ?? string.Empty),
                new Claim(ClaimTypes.Surname, user.LastName ?? string.Empty)
            };

            if (user.Roles != null && user.Roles.Count > 0)
            {
                foreach (var role in user.Roles)
                {
                    claims.Add(new Claim(ClaimTypes.Role, role));
                }
            }

            var identity = new ClaimsIdentity(claims, "jwt");
            var claimsPrincipal = new ClaimsPrincipal(identity);

            _httpClient.DefaultRequestHeaders.Authorization =
                new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);

            return new AuthenticationState(claimsPrincipal);
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Authentication state error: {ex.Message}");
            return new AuthenticationState(new ClaimsPrincipal(new ClaimsIdentity()));
        }
    }

    public async void NotifyUserAuthentication(AuthResponse user)
    {
        var claims = new List<Claim>
        {
            new Claim(ClaimTypes.Email, user.Email ?? string.Empty),
            new Claim(ClaimTypes.GivenName, user.FirstName ?? string.Empty),
            new Claim(ClaimTypes.Surname, user.LastName ?? string.Empty)
        };

        if (user.Roles != null && user.Roles.Count > 0)
        {
            foreach (var role in user.Roles)
            {
                claims.Add(new Claim(ClaimTypes.Role, role));
            }
        }

        var identity = new ClaimsIdentity(claims, "jwt");
        var claimsPrincipal = new ClaimsPrincipal(identity);
        var authState = Task.FromResult(new AuthenticationState(claimsPrincipal));
        NotifyAuthenticationStateChanged(authState);
    }

    public void NotifyUserLogout()
    {
        var anonymousUser = new ClaimsPrincipal(new ClaimsIdentity());
        var authState = Task.FromResult(new AuthenticationState(anonymousUser));
        NotifyAuthenticationStateChanged(authState);
    }
}