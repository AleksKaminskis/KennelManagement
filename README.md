# Kennel Management System

## Overview

Kennel Management is a single-page application composed of a Blazor WebAssembly client and an ASP.NET Core Web API server. The system manages customers, dogs, kennels and bookings and includes authentication/authorization using ASP.NET Core Identity and JWT tokens.

- Target framework: .NET 8
- Client: Blazor WebAssembly (WebAssembly hosted client)
- Server: ASP.NET Core Web API with Entity Framework Core

## Features

- User registration and login (JWT)
- Role-based access (Admin / Staff / User)
- CRUD for Customers, Dogs, Kennels and Bookings
- Seeded sample data for development
- Swagger/OpenAPI for API exploration
- Input sanitization middleware
- CORS configured to allow the Blazor client

## Solution structure

- `Client/` - Blazor WebAssembly project (UI)
  - `Client/Pages/` - Blazor pages (Login, Register, Dogs, Customers, Bookings, etc.)
  - `Client/Services/` - client-side services (API wrapper, auth provider)
  - `Client/wwwroot/` - static assets and client appsettings

- `Server/` - ASP.NET Core Web API
  - `Server/Controllers/` - API controllers (Auth, Dogs, Customers, Bookings, Kennels)
  - `Server/Data/` - `ApplicationDbContext`, migrations and seeding (`DbSeeder`)
  - `Server/DTOs/` - DTO types used by controllers
  - `Server/Models/` - EF models and Identity user model
  - `Server/Services/` - auth and supporting services
  - `Server/Program.cs` - app startup, DI, authentication, DB configuration

## Getting started (local)

Prerequisites:
- .NET 8 SDK
- (Optional) SQL Server or Azure SQL for production-like runs

Run locally using the in-memory database (recommended for development):

1. Ensure `Server/appsettings.Development.json` sets `UseInMemoryDatabase` to `true` (or provide a SQL connection string in `ConnectionStrings:DefaultConnection`).
2. Start the Server:
   - `cd Server`
   - `dotnet run`
3. Start the Client:
   - `cd Client`
   - `dotnet run`
4. Open the client in the browser (URL printed by the client run). The API Swagger UI is available on the server (typically at `/swagger`).

Configuration keys of interest (in `Server/appsettings*.json` or environment variables / Key Vault):
- `ConnectionStrings:DefaultConnection` - SQL Server connection string
- `Jwt:Key` - symmetric key used to sign JWTs
- `Jwt:Issuer` - JWT issuer
- `Jwt:Audience` - JWT audience
- `KeyVaultEndpoint` - Azure Key Vault endpoint (optional)
- `UseInMemoryDatabase` - toggles in-memory DB for Development
- `AllowedOrigins` / CORS policy name `AllowBlazorClient`

The application seeds roles and an admin user on startup via `DbSeeder`.

## User guide

- Register a new account via the client `Register` page.
- Log in to receive a JWT token and unlock authenticated routes.
- Admin users can create/edit/delete customers, dogs, kennels and bookings.
- Use the `Bookings` pages to create boarding reservations for dogs.
- Use the API `Swagger` UI to explore endpoints and perform API-level testing.

Common client pages: `Client/Pages/Login.razor`, `Register.razor`, `Dogs.razor`, `Customers.razor`, `Bookings.razor`, `Dashboard.razor`.

## Technical architecture

- Client (Blazor WebAssembly):
  - Uses a custom `AuthenticationStateProvider` and `AuthService` to manage JWT tokens and authentication state.
  - `ApiService` attaches the JWT `Authorization` header and handles HTTP calls to the API.

- Server (ASP.NET Core Web API):
  - ASP.NET Core Identity with Entity Framework Core for user and role management.
  - JWT Bearer authentication configured in `Program.cs`.
  - EF Core database provider: In-memory for development; SQL Server in production (`DefaultConnection`).
  - Input sanitization middleware applied globally to protect against basic input attacks.
  - Swagger configured with Bearer authentication for testing secured endpoints.
  - `DbSeeder` applies migrations (when not using in-memory DB) and seeds initial data and roles on startup.

## Security

- JWT tokens for authentication and role-based authorization.
- Password policies enforced by Identity (configured in `Program.cs`).
- Sensitive configuration can be stored in Azure Key Vault; `Program.cs` reads `KeyVaultEndpoint` and uses `DefaultAzureCredential`.

## Cloud resources (recommended / integration points)

This project includes integration points and configuration suitable for standard Azure deployments:

- Azure App Service or Azure Static Web Apps for hosting the Blazor client.
- Azure App Service for hosting the API server.
- Azure SQL Database (or Managed Instance) for persistent storage; set `ConnectionStrings:DefaultConnection`.
- Azure Key Vault to store secrets (JWT signing key, DB connection string). `Program.cs` can read `KeyVaultEndpoint` and uses `DefaultAzureCredential`.
- CI/CD: `azure-pipelines.yml` and publish profiles in `*/Properties/PublishProfiles/` for automated deployments.

Deployment notes:
- Ensure `Jwt:Key`, `Jwt:Issuer`, `Jwt:Audience`, and `ConnectionStrings:DefaultConnection` are set in Key Vault or App Service settings.
- Add the deployed client origin to the server CORS configuration (`AllowedOrigins`) to allow browser requests.

## Build & publish

Server (API):
- `cd Server`
- `dotnet publish -c Release -o ./publish`

Client (Blazor WASM):
- `cd Client`
- `dotnet publish -c Release -o ./publish`

Deploy the server `publish` output to an App Service and the client `publish` output to a static host or App Service. Configure environment variables or link Azure Key Vault to provide secrets.

## Troubleshooting

- JWT authentication issues: confirm `Jwt:Key`, `Jwt:Issuer`, and `Jwt:Audience` match between client and server.
- Database connection issues in production: ensure `DefaultConnection` is set and firewall rules allow the app to access the database.
- Development convenience: enable `UseInMemoryDatabase` in `appsettings.Development.json` to avoid SQL setup.

---

README created at repository root. Adjust environment-specific values before production deployment.