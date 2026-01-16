using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ReklamacjeAPI.Data;
using ReklamacjeAPI.DTOs;
using ReklamacjeAPI.Models;
using ReklamacjeAPI.Services;

namespace ReklamacjeAPI.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AuthController : ControllerBase
{
    private readonly ApplicationDbContext _context;
    private readonly IAuthService _authService;

    public AuthController(ApplicationDbContext context, IAuthService authService)
    {
        _context = context;
        _authService = authService;
    }

    [HttpPost("login")]
    public async Task<ActionResult<ApiResponse<LoginResponse>>> Login([FromBody] LoginRequest request)
    {
        // Find user
        var user = await _context.Users
            .FirstOrDefaultAsync(u => u.Login == request.Login);

        if (user == null || !BCrypt.Net.BCrypt.Verify(request.Password, user.PasswordHash))
        {
            return Unauthorized(ApiResponse<LoginResponse>.ErrorResponse("Nieprawidłowy login lub hasło"));
        }

        if (!user.IsActive)
        {
            return Unauthorized(ApiResponse<LoginResponse>.ErrorResponse("Konto jest nieaktywne"));
        }

        // Generate tokens
        var accessToken = _authService.GenerateJwtToken(user);
        var refreshToken = await _authService.CreateRefreshToken(user);

        // Save refresh token
        _context.RefreshTokens.Add(refreshToken);

        // Update last login
        user.LastLogin = DateTime.UtcNow;

        await _context.SaveChangesAsync();

        var response = new LoginResponse
        {
            Token = accessToken,
            RefreshToken = refreshToken.Token,
            ExpiresAt = refreshToken.ExpiryDate,
            User = new UserDto
            {
                Id = user.Id,
                Login = user.Login,
                NazwaWyswietlana = user.DisplayName,
                Email = user.Email,
                Aktywny = user.IsActive
            }
        };

        return Ok(ApiResponse<LoginResponse>.SuccessResponse(response, "Zalogowano pomyślnie"));
    }

    [HttpPost("refresh")]
    public async Task<ActionResult<ApiResponse<LoginResponse>>> Refresh([FromBody] RefreshTokenRequest request)
    {
        // Find refresh token
        var refreshToken = await _context.RefreshTokens
            .Include(rt => rt.User)
            .FirstOrDefaultAsync(rt => rt.Token == request.RefreshToken && !rt.IsRevoked);

        if (refreshToken == null || refreshToken.ExpiryDate < DateTime.UtcNow)
        {
            return Unauthorized(ApiResponse<LoginResponse>.ErrorResponse("Nieprawidłowy lub wygasły refresh token"));
        }

        // Revoke old token
        refreshToken.IsRevoked = true;

        // Generate new tokens
        var accessToken = _authService.GenerateJwtToken(refreshToken.User);
        var newRefreshToken = await _authService.CreateRefreshToken(refreshToken.User);

        _context.RefreshTokens.Add(newRefreshToken);
        await _context.SaveChangesAsync();

        var response = new LoginResponse
        {
            Token = accessToken,
            RefreshToken = newRefreshToken.Token,
            ExpiresAt = newRefreshToken.ExpiryDate,
            User = new UserDto
            {
                Id = refreshToken.User.Id,
                Login = refreshToken.User.Login,
                NazwaWyswietlana = refreshToken.User.DisplayName,
                Email = refreshToken.User.Email,
                Aktywny = refreshToken.User.IsActive
            }
        };

        return Ok(ApiResponse<LoginResponse>.SuccessResponse(response));
    }

    [Authorize]
    [HttpPost("logout")]
    public async Task<ActionResult<ApiResponse<object>>> Logout()
    {
        var userId = int.Parse(User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value ?? "0");

        // Revoke all refresh tokens for this user
        var tokens = await _context.RefreshTokens
            .Where(rt => rt.UserId == userId && !rt.IsRevoked)
            .ToListAsync();

        foreach (var token in tokens)
        {
            token.IsRevoked = true;
        }

        await _context.SaveChangesAsync();

        return Ok(ApiResponse<object>.SuccessResponse(null, "Wylogowano pomyślnie"));
    }

    [Authorize]
    [HttpGet("validate")]
    public ActionResult<ApiResponse<UserDto>> Validate()
    {
        var userId = int.Parse(User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value ?? "0");
        var displayName = User.FindFirst("DisplayName")?.Value ?? "";
        var email = User.FindFirst(System.Security.Claims.ClaimTypes.Email)?.Value;

        var userDto = new UserDto
        {
            Id = userId,
            Login = User.Identity?.Name ?? "",
            NazwaWyswietlana = displayName,
            Email = email,
            Aktywny = true
        };

        return Ok(ApiResponse<UserDto>.SuccessResponse(userDto, "Token jest ważny"));
    }
}
