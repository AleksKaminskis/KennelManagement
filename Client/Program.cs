using Blazored.LocalStorage;
using Client;
using Client.Services;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Text.Json.Serialization.Metadata;

var builder = WebAssemblyHostBuilder.CreateDefault(args);
builder.RootComponents.Add<App>("#app");
builder.RootComponents.Add<HeadOutlet>("head::after");

// Configure HttpClient
var apiBaseAddress = builder.Configuration["ApiBaseAddress"] ?? "https://localhost:7000";

// Ensure the base address ends with /
if (!apiBaseAddress.EndsWith("/"))
{
    apiBaseAddress += "/";
}

Console.WriteLine($"========================================");
Console.WriteLine($"Kennel Management Client Starting");
Console.WriteLine($"API Base Address: {apiBaseAddress}");
Console.WriteLine($"Environment: {builder.HostEnvironment.Environment}");
Console.WriteLine($"========================================");

// Configure JSON serialization options to avoid nullable reference type issues
var jsonOptions = new JsonSerializerOptions
{
    PropertyNameCaseInsensitive = true,
    DefaultIgnoreCondition = JsonIgnoreCondition.Never,
    PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
    // This is the key fix for the NullabilityInfoContext error
    TypeInfoResolver = new System.Text.Json.Serialization.Metadata.DefaultJsonTypeInfoResolver()
};

builder.Services.AddScoped(sp =>
{
    var httpClient = new HttpClient { BaseAddress = new Uri(apiBaseAddress) };
    return httpClient;
});

// Add Blazored LocalStorage with JSON options
builder.Services.AddBlazoredLocalStorage(config =>
{
    config.JsonSerializerOptions.PropertyNameCaseInsensitive = true;
    config.JsonSerializerOptions.DefaultIgnoreCondition = JsonIgnoreCondition.Never;
});

// Add Authentication
builder.Services.AddAuthorizationCore();
builder.Services.AddScoped<AuthenticationStateProvider, CustomAuthenticationStateProvider>();
builder.Services.AddScoped<CustomAuthenticationStateProvider>(provider =>
    (CustomAuthenticationStateProvider)provider.GetRequiredService<AuthenticationStateProvider>());

// Add Services
builder.Services.AddScoped<IAuthService, AuthService>();
builder.Services.AddScoped<ApiService>();

// Add refresh service
builder.Services.AddSingleton<RefreshService>();

await builder.Build().RunAsync();
