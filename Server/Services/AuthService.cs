using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Server.Data;
using Server.DTOs;
using Server.Models;
using Server.Services;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace Server.Services;

public class AuthService : IAuthService
{
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly IConfiguration _configuration;
    private readonly ApplicationDbContext _context;
    private readonly ILogger<AuthService> _logger;

    public AuthService(
        UserManager<ApplicationUser> userManager,
        IConfiguration configuration,
        ApplicationDbContext context,
        ILogger<AuthService> logger)
    {
        _userManager = userManager;
        _configuration = configuration;
        _context = context;
        _logger = logger;
    }

    public async Task<AuthResponseDto?> RegisterAsync(RegisterDto registerDto)
    {
        try
        {
            // Check if user already exists
            var userExists = await _userManager.FindByEmailAsync(registerDto.Email);
            if (userExists != null)
            {
                _logger.LogWarning("Registration failed: User {Email} already exists", registerDto.Email);
                return null;
            }

            // Create the application user
            var user = new ApplicationUser
            {
                UserName = registerDto.Email,
                Email = registerDto.Email,
                FirstName = registerDto.FirstName,
                LastName = registerDto.LastName,
                EmailConfirmed = true
            };

            var result = await _userManager.CreateAsync(user, registerDto.Password);
            if (!result.Succeeded)
            {
                _logger.LogError("User creation failed: {Errors}",
                    string.Join(", ", result.Errors.Select(e => e.Description)));
                return null;
            }

            // Assign role
            var roleResult = await _userManager.AddToRoleAsync(user, registerDto.Role);
            if (!roleResult.Succeeded)
            {
                _logger.LogError("Role assignment failed: {Errors}",
                    string.Join(", ", roleResult.Errors.Select(e => e.Description)));

                // Rollback user creation
                await _userManager.DeleteAsync(user);
                return null;
            }

            // If registering as Customer, create a corresponding Customer record
            if (registerDto.Role.Equals("Customer", StringComparison.OrdinalIgnoreCase))
            {
                try
                {
                    // Check if customer record already exists
                    var existingCustomer = await _context.Customers
                        .FirstOrDefaultAsync(c => c.Email == registerDto.Email);

                    if (existingCustomer == null)
                    {
                        var customer = new Customer
                        {
                            FirstName = registerDto.FirstName,
                            LastName = registerDto.LastName,
                            Email = registerDto.Email,
                            PhoneNumber = registerDto.PhoneNumber ?? string.Empty,
                            Address = registerDto.Address ?? string.Empty,
                            CreatedAt = DateTime.UtcNow
                        };

                        _context.Customers.Add(customer);
                        await _context.SaveChangesAsync();

                        _logger.LogInformation("Customer record created for user {Email}", registerDto.Email);
                    }
                    else
                    {
                        _logger.LogInformation("Customer record already exists for user {Email}", registerDto.Email);
                    }
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Failed to create customer record for {Email}", registerDto.Email);
                }
            }

            _logger.LogInformation("User {Email} registered successfully with role {Role}",
                registerDto.Email, registerDto.Role);

            return await GenerateAuthResponse(user);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error during registration for {Email}", registerDto.Email);
            return null;
        }
    }

    public async Task<AuthResponseDto?> LoginAsync(LoginDto loginDto)
    {
        try
        {
            var user = await _userManager.FindByEmailAsync(loginDto.Email);
            if (user == null)
            {
                _logger.LogWarning("Login failed: User {Email} not found", loginDto.Email);
                return null;
            }

            var isPasswordValid = await _userManager.CheckPasswordAsync(user, loginDto.Password);
            if (!isPasswordValid)
            {
                _logger.LogWarning("Login failed: Invalid password for user {Email}", loginDto.Email);
                return null;
            }

            _logger.LogInformation("User {Email} logged in successfully", loginDto.Email);
            return await GenerateAuthResponse(user);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error during login for {Email}", loginDto.Email);
            return null;
        }
    }

    private async Task<AuthResponseDto> GenerateAuthResponse(ApplicationUser user)
    {
        var roles = await _userManager.GetRolesAsync(user);
        var claims = new List<Claim>
        {
            new Claim(ClaimTypes.NameIdentifier, user.Id),
            new Claim(ClaimTypes.Email, user.Email!),
            new Claim(ClaimTypes.GivenName, user.FirstName),
            new Claim(ClaimTypes.Surname, user.LastName),
            new Claim(JwtRegisteredClaimNames.Sub, user.Email!),
            new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
        };

        foreach (var role in roles)
        {
            claims.Add(new Claim(ClaimTypes.Role, role));
        }

        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(
            _configuration["Jwt:Key"] ?? throw new InvalidOperationException("JWT Key not configured")));
        var credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);
        var expiration = DateTime.UtcNow.AddDays(7);

        var token = new JwtSecurityToken(
            issuer: _configuration["Jwt:Issuer"],
            audience: _configuration["Jwt:Audience"],
            claims: claims,
            expires: expiration,
            signingCredentials: credentials
        );

        return new AuthResponseDto
        {
            Token = new JwtSecurityTokenHandler().WriteToken(token),
            Email = user.Email!,
            FirstName = user.FirstName,
            LastName = user.LastName,
            Roles = roles.ToList(),
            Expiration = expiration
        };
    }
}