using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using Microsoft.IdentityModel.Tokens;
using ReklamacjeAPI.Models;

namespace ReklamacjeAPI.Services;

public interface IAuthService
{
    string GenerateJwtToken(User user);
    string GenerateRefreshToken();
    ClaimsPrincipal? ValidateToken(string token);
    Task<RefreshToken> CreateRefreshToken(User user);
}

public class AuthService : IAuthService
{
    private readonly IConfiguration _configuration;

    public AuthService(IConfiguration configuration)
    {
        _configuration = configuration;
    }

    public string GenerateJwtToken(User user)
    {
        var secret = _configuration["JwtSettings:Secret"] 
            ?? throw new InvalidOperationException("JWT Secret not configured");
        var issuer = _configuration["JwtSettings:Issuer"];
        var audience = _configuration["JwtSettings:Audience"];
        var expiryMinutes = int.Parse(_configuration["JwtSettings:ExpiryMinutes"] ?? "60");

        var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secret));
        var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);

        var claims = new[]
        {
            new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
            new Claim(ClaimTypes.Name, user.Login),
            new Claim("DisplayName", user.DisplayNameOrLogin),
            new Claim(ClaimTypes.Email, user.Email ?? ""),
            new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
        };

        var token = new JwtSecurityToken(
            issuer: issuer,
            audience: audience,
            claims: claims,
            expires: DateTime.UtcNow.AddMinutes(expiryMinutes),
            signingCredentials: credentials
        );

        return new JwtSecurityTokenHandler().WriteToken(token);
    }

    public string GenerateRefreshToken()
    {
        var randomNumber = new byte[64];
        using var rng = RandomNumberGenerator.Create();
        rng.GetBytes(randomNumber);
        return Convert.ToBase64String(randomNumber);
    }

    public ClaimsPrincipal? ValidateToken(string token)
    {
        var secret = _configuration["JwtSettings:Secret"] 
            ?? throw new InvalidOperationException("JWT Secret not configured");
        var issuer = _configuration["JwtSettings:Issuer"];
        var audience = _configuration["JwtSettings:Audience"];

        var tokenHandler = new JwtSecurityTokenHandler();
        var key = Encoding.UTF8.GetBytes(secret);

        try
        {
            var principal = tokenHandler.ValidateToken(token, new TokenValidationParameters
            {
                ValidateIssuerSigningKey = true,
                IssuerSigningKey = new SymmetricSecurityKey(key),
                ValidateIssuer = true,
                ValidIssuer = issuer,
                ValidateAudience = true,
                ValidAudience = audience,
                ValidateLifetime = false, // We check this manually
                ClockSkew = TimeSpan.Zero
            }, out SecurityToken validatedToken);

            return principal;
        }
        catch
        {
            return null;
        }
    }

    public async Task<RefreshToken> CreateRefreshToken(User user)
    {
        var expiryDays = int.Parse(_configuration["JwtSettings:RefreshTokenExpiryDays"] ?? "7");

        return new RefreshToken
        {
            UserId = user.Id,
            Token = GenerateRefreshToken(),
            ExpiryDate = DateTime.UtcNow.AddDays(expiryDays),
            CreatedAt = DateTime.UtcNow
        };
    }
}
