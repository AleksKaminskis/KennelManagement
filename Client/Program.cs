using Blazored.LocalStorage;
using Client;
using Client.Services;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;

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

builder.Services.AddScoped(sp => new HttpClient { BaseAddress = new Uri(apiBaseAddress) });

// Add Blazored LocalStorage
builder.Services.AddBlazoredLocalStorage();

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
