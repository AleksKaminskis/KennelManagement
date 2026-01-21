using Blazored.LocalStorage;
using Client.Models;
using System.Net.Http.Json;

namespace Client.Services
{
    public class AuthService : IAuthService
    {
        private readonly HttpClient _httpClient;
        private readonly ILocalStorageService _localStorage;
        private const string TokenKey = "authToken";
        private const string UserKey = "authUser";

        public AuthService(HttpClient httpClient, ILocalStorageService localStorage)
        {
            _httpClient = httpClient;
            _localStorage = localStorage;
        }

        public async Task<AuthResponse> LoginAsync(LoginModel model)
        {
            try
            {
                var response = await _httpClient.PostAsJsonAsync("api/auth/login", model);

                if (response.IsSuccessStatusCode)
                {
                    var authResponse = await response.Content.ReadFromJsonAsync<AuthResponse>();
                    if (authResponse != null)
                    {
                        await _localStorage.SetItemAsync(TokenKey, authResponse.Token);
                        await _localStorage.SetItemAsync(UserKey, authResponse);

                        _httpClient.DefaultRequestHeaders.Authorization =
                            new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", authResponse.Token);
                    }
                    return authResponse;
                }

                return null;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Login error: {ex.Message}");
                return null;
            }
        }

        public async Task<AuthResponse> RegisterAsync(RegisterModel model)
        {
            try
            {
                var registerDto = new
                {
                    model.FirstName,
                    model.LastName,
                    model.Email,
                    model.Password,
                    model.Role,
                    PhoneNumber = string.IsNullOrEmpty(model.PhoneNumber) ? "" : model.PhoneNumber,
                    Address = string.IsNullOrEmpty(model.Address) ? "" : model.Address
                };

                var response = await _httpClient.PostAsJsonAsync("api/auth/register", registerDto);

                if (response.IsSuccessStatusCode)
                {
                    var authResponse = await response.Content.ReadFromJsonAsync<AuthResponse>();
                    if (authResponse != null)
                    {
                        await _localStorage.SetItemAsync(TokenKey, authResponse.Token);
                        await _localStorage.SetItemAsync(UserKey, authResponse);
                        
                        _httpClient.DefaultRequestHeaders.Authorization =
                            new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", authResponse.Token);
                    }
                    return authResponse;
                }

                return null;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Registration error: {ex.Message}");
                return null;
            }
        }

        public async Task<bool> IsAuthenticatedAsync()
        {
            try
            {
                var token = await _localStorage.GetItemAsStringAsync(TokenKey);
                return !string.IsNullOrEmpty(token);
            }
            catch
            {
                return false;
            }
        }

        public async Task<string> GetTokenAsync()
        {
            try
            {
                return await _localStorage.GetItemAsStringAsync(TokenKey);
            }
            catch
            {
                return null;
            }
        }

        public async Task LogoutAsync()
        {
            await _localStorage.RemoveItemAsync(TokenKey);
            await _localStorage.RemoveItemAsync(UserKey);
            _httpClient.DefaultRequestHeaders.Authorization = null;
        }
    }
}
