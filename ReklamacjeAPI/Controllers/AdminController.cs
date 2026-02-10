using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ReklamacjeAPI.Data;
using ReklamacjeAPI.DTOs;
using ReklamacjeAPI.Models;
// using ReklamacjeAPI.Security; // Odkomentuj jeśli używasz EncryptionHelper zamiast BCrypt

namespace ReklamacjeAPI.Controllers
{
    [Authorize]
    [ApiController]
    [Route("api/admin")]
    public class AdminController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public AdminController(ApplicationDbContext context)
        {
            _context = context;
        }

        private bool IsAdmin()
        {
            // Sprawdza czy użytkownik ma rolę Admin
            return User.IsInRole("Admin");
        }

        // 1. Pobranie listy użytkowników
        [HttpGet("users")]
        public async Task<IActionResult> GetAllUsers()
        {
            // Opcjonalnie: odkomentuj linię niżej, aby blokować dostęp nie-adminom
            // if (!IsAdmin()) return Forbid();

            // ZMIANA: _context.Users zamiast _context.Uzytkownicy
            var users = await _context.Users
                .Select(u => new AdminUserListDto
                {
                    Id = u.Id,
                    Login = u.Login,
                    NazwaWyswietlana = u.DisplayName,
                    Rola = u.Role,
                    IsActive = u.IsActive
                })
                .OrderBy(u => u.Login)
                .ToListAsync();

            return Ok(ApiResponse<List<AdminUserListDto>>.SuccessResponse(users));
        }

        // 2. Dodanie nowego użytkownika
        [HttpPost("users")]
        public async Task<IActionResult> CreateUser([FromBody] AdminCreateUserDto dto)
        {
            // if (!IsAdmin()) return Forbid();

            // ZMIANA: _context.Users zamiast _context.Uzytkownicy
            if (await _context.Users.AnyAsync(u => u.Login == dto.Login))
            {
                return BadRequest(ApiResponse<object>.ErrorResponse("Użytkownik o takim loginie już istnieje."));
            }

            // Haszowanie hasła (Używamy BCrypt)
            // Upewnij się, że masz paczkę: dotnet add package BCrypt.Net-Next
            string passwordHash = BCrypt.Net.BCrypt.HashPassword(dto.Password);

            var newUser = new User
            {
                Login = dto.Login,
                PasswordHash = passwordHash,
                DisplayName = dto.NazwaWyswietlana,
                Role = dto.Rola,
                IsActive = true
            };

            // ZMIANA: _context.Users zamiast _context.Uzytkownicy
            _context.Users.Add(newUser);
            await _context.SaveChangesAsync();

            return Ok(ApiResponse<object>.SuccessResponse(null, "Użytkownik dodany."));
        }

        // 3. Edycja użytkownika
        [HttpPut("users/{id}")]
        public async Task<IActionResult> UpdateUser(int id, [FromBody] AdminUpdateUserDto dto)
        {
            // if (!IsAdmin()) return Forbid();

            // ZMIANA: _context.Users zamiast _context.Uzytkownicy
            var user = await _context.Users.FindAsync(id);
            if (user == null) return NotFound(ApiResponse<object>.ErrorResponse("Nie znaleziono użytkownika."));

            user.DisplayName = dto.NazwaWyswietlana;
            user.Role = dto.Rola;
            user.IsActive = dto.IsActive;

            await _context.SaveChangesAsync();
            return Ok(ApiResponse<object>.SuccessResponse(null, "Dane zaktualizowane."));
        }

        // 4. Reset hasła
        [HttpPost("users/{id}/reset-password")]
        public async Task<IActionResult> ResetPassword(int id, [FromBody] AdminResetPasswordDto dto)
        {
            // if (!IsAdmin()) return Forbid();

            // ZMIANA: _context.Users zamiast _context.Uzytkownicy
            var user = await _context.Users.FindAsync(id);
            if (user == null) return NotFound(ApiResponse<object>.ErrorResponse("Nie znaleziono użytkownika."));

            user.PasswordHash = BCrypt.Net.BCrypt.HashPassword(dto.NewPassword);

            await _context.SaveChangesAsync();
            return Ok(ApiResponse<object>.SuccessResponse(null, "Hasło zostało zresetowane."));
        }
    }
}