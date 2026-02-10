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

    // NOWA METODA: Pobiera listę loginów dla aplikacji mobilnej
    [HttpGet("users")]
    public async Task<ActionResult<ApiResponse<List<string>>>> GetUsers()
    {
        var logins = await _context.Users
            .Where(u => u.IsActive)
            .OrderBy(u => u.Login)
            .Select(u => u.Login)
            .ToListAsync();

        return Ok(ApiResponse<List<string>>.SuccessResponse(logins));
    }

    [HttpPost("login")]
    public async Task<ActionResult<ApiResponse<LoginResponse>>> Login([FromBody] LoginRequest request)
    {
        var user = await _context.Users
            .FirstOrDefaultAsync(u => u.Login == request.Login);

        if (user == null || !PasswordCompatibilityHelper.Verify(request.Password, user.PasswordHash))
        {
            return Unauthorized(ApiResponse<LoginResponse>.ErrorResponse("Nieprawidłowy login lub hasło"));
        }

        if (!user.IsActive)
        {
            return Unauthorized(ApiResponse<LoginResponse>.ErrorResponse("Konto jest nieaktywne"));
        }

        var accessToken = _authService.GenerateJwtToken(user);
        var refreshToken = await _authService.CreateRefreshToken(user);

        _context.RefreshTokens.Add(refreshToken);
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
                // DisplayName może być nullem w bazie, więc robimy fallback do loginu
                NazwaWyswietlana = !string.IsNullOrEmpty(user.DisplayName) ? user.DisplayName : user.Login,
                Email = user.Email,
                Aktywny = user.IsActive
            }
        };

        return Ok(ApiResponse<LoginResponse>.SuccessResponse(response, "Zalogowano pomyślnie"));
    }

    [HttpPost("refresh")]
    public async Task<ActionResult<ApiResponse<LoginResponse>>> Refresh([FromBody] RefreshTokenRequest request)
    {
        var refreshToken = await _context.RefreshTokens
            .Include(rt => rt.User)
            .FirstOrDefaultAsync(rt => rt.Token == request.RefreshToken && !rt.IsRevoked);

        if (refreshToken == null || refreshToken.ExpiryDate < DateTime.UtcNow)
        {
            return Unauthorized(ApiResponse<LoginResponse>.ErrorResponse("Nieprawidłowy lub wygasły refresh token"));
        }

        refreshToken.IsRevoked = true;

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
                NazwaWyswietlana = !string.IsNullOrEmpty(refreshToken.User.DisplayName) ? refreshToken.User.DisplayName : refreshToken.User.Login,
                Email = refreshToken.User.Email,
                Aktywny = refreshToken.User.IsActive
            }
        };

        return Ok(ApiResponse<LoginResponse>.SuccessResponse(response));
    }

    [HttpPost("logout")]
    public async Task<ActionResult<ApiResponse<object>>> Logout()
    {
        var userId = int.Parse(User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value ?? "0");
        if (userId == 0)
        {
            return Ok(ApiResponse<object>.SuccessResponse(null, "Autoryzacja wyłączona - brak aktywnej sesji."));
        }

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

    [HttpGet("validate")]
    public ActionResult<ApiResponse<UserDto>> Validate()
    {
        var userId = int.Parse(User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value ?? "0");
        var displayName = User.FindFirst("DisplayName")?.Value ?? "";
        var email = User.FindFirst(System.Security.Claims.ClaimTypes.Email)?.Value;

        if (userId == 0)
        {
            return Ok(ApiResponse<UserDto>.SuccessResponse(new UserDto
            {
                Id = 0,
                Login = "anonymous",
                NazwaWyswietlana = "Anonim",
                Email = null,
                Aktywny = true
            }, "Autoryzacja wyłączona"));
        }

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
