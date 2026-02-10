// Plik: Controllers/UserProfileController.cs
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MySqlConnector;
using ReklamacjeAPI.DTOs;
using ReklamacjeAPI.Services;
using ReklamacjeAPI.Security;
using System.Security.Claims;

namespace ReklamacjeAPI.Controllers
{
    [Authorize]
    [ApiController]
    [Route("api/profile")]
    public class UserProfileController : ControllerBase
    {
        private readonly IConfiguration _configuration;

        public UserProfileController(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        private int GetCurrentUserId()
        {
            var claim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            return int.TryParse(claim, out var id) ? id : 0;
        }

        // 1. Pobranie danych profilu (Email, Telefon, Aktywne delegacje)
        [HttpGet]
        public async Task<IActionResult> GetProfile()
        {
            var userId = GetCurrentUserId();
            if (userId == 0) return Unauthorized();

            await using var conn = DbConnectionFactory.CreateDefaultConnection(_configuration);
            await conn.OpenAsync();

            // Pobieramy dane usera + ewentualnie email/telefon (jeśli są w tabeli Uzytkownicy lub Klienci - zakładam Uzytkownicy)
            // Uwaga: Jeśli w tabeli Uzytkownicy nie ma kolumn Email/Telefon, trzeba je dodać (ALTER TABLE Uzytkownicy ADD Email VARCHAR(255)...)
            var query = "SELECT Login, `Nazwa Wyświetlana`, Email, Telefon FROM Uzytkownicy WHERE Id = @id";

            var userDto = new UserProfileDto();

            await using (var cmd = new MySqlCommand(query, conn))
            {
                cmd.Parameters.AddWithValue("@id", userId);
                await using var reader = await cmd.ExecuteReaderAsync();
                if (await reader.ReadAsync())
                {
                    userDto.Login = reader["Login"].ToString();
                    userDto.NazwaWyswietlana = reader["Nazwa Wyświetlana"].ToString();
                    userDto.Email = reader["Email"]?.ToString();
                    userDto.Telefon = reader["Telefon"]?.ToString();
                }
            }

            // Pobieramy aktywne/przyszłe delegacje
            userDto.Delegacje = new List<DelegacjaDto>();
            var delQuery = @"
                SELECT d.Id, d.DataOd, d.DataDo, d.Typ, u.`Nazwa Wyświetlana` as ZastepcaNazwa
                FROM Delegacje d
                LEFT JOIN Uzytkownicy u ON d.ZastepcaId = u.Id
                WHERE d.UzytkownikId = @uid AND d.DataDo >= CURDATE() AND d.CzyAktywna = 1
                ORDER BY d.DataOd ASC";

            await using (var cmd = new MySqlCommand(delQuery, conn))
            {
                cmd.Parameters.AddWithValue("@uid", userId);
                await using var reader = await cmd.ExecuteReaderAsync();
                while (await reader.ReadAsync())
                {
                    userDto.Delegacje.Add(new DelegacjaDto
                    {
                        Id = reader.GetInt32("Id"),
                        DataOd = reader.GetDateTime("DataOd"),
                        DataDo = reader.GetDateTime("DataDo"),
                        Typ = reader["Typ"].ToString(),
                        ZastepcaNazwa = reader["ZastepcaNazwa"]?.ToString() ?? "Brak"
                    });
                }
            }

            return Ok(ApiResponse<UserProfileDto>.SuccessResponse(userDto));
        }

        // 2. Aktualizacja danych kontaktowych
        [HttpPost("contact")]
        public async Task<IActionResult> UpdateContact([FromBody] UpdateContactRequest request)
        {
            var userId = GetCurrentUserId();
            await using var conn = DbConnectionFactory.CreateDefaultConnection(_configuration);
            await conn.OpenAsync();

            var query = "UPDATE Uzytkownicy SET Email = @email, Telefon = @phone WHERE Id = @id";
            await using var cmd = new MySqlCommand(query, conn);
            cmd.Parameters.AddWithValue("@email", request.Email);
            cmd.Parameters.AddWithValue("@phone", request.Telefon);
            cmd.Parameters.AddWithValue("@id", userId);

            await cmd.ExecuteNonQueryAsync();
            return Ok(ApiResponse<object>.SuccessResponse(null));
        }

        // 3. Zmiana hasła
        [HttpPost("password")]
        public async Task<IActionResult> ChangePassword([FromBody] ChangePasswordRequest request)
        {
            var userId = GetCurrentUserId();
            await using var conn = DbConnectionFactory.CreateDefaultConnection(_configuration);
            await conn.OpenAsync();

            // Sprawdź stare hasło (używając EncryptionHelper z twojego projektu)
            // Tutaj uproszczenie - musisz użyć logiki weryfikacji hasła identycznej jak w AuthService
            var query = "SELECT Hasło FROM Uzytkownicy WHERE Id = @id";
            string currentHash = "";
            await using (var cmd = new MySqlCommand(query, conn))
            {
                cmd.Parameters.AddWithValue("@id", userId);
                currentHash = (await cmd.ExecuteScalarAsync())?.ToString();
            }

            if (!BCrypt.Net.BCrypt.Verify(request.OldPassword, currentHash))
            {
                return BadRequest(ApiResponse<object>.ErrorResponse("Stare hasło jest nieprawidłowe."));
            }

            string newHash = BCrypt.Net.BCrypt.HashPassword(request.NewPassword);
            var updateQuery = "UPDATE Uzytkownicy SET Hasło = @h WHERE Id = @id";
            await using (var cmd = new MySqlCommand(updateQuery, conn))
            {
                cmd.Parameters.AddWithValue("@h", newHash);
                cmd.Parameters.AddWithValue("@id", userId);
                await cmd.ExecuteNonQueryAsync();
            }

            return Ok(ApiResponse<object>.SuccessResponse(null));
        }

        // 4. Dodanie Urlopu/Delegacji
        [HttpPost("delegation")]
        public async Task<IActionResult> AddDelegation([FromBody] AddDelegationRequest request)
        {
            var userId = GetCurrentUserId();
            await using var conn = DbConnectionFactory.CreateDefaultConnection(_configuration);
            await conn.OpenAsync();

            // Upewnij się, że tabela Delegacje istnieje (struktura jak w ReturnsService)
            var query = @"
                INSERT INTO Delegacje (UzytkownikId, ZastepcaId, DataOd, DataDo, Typ, CzyAktywna)
                VALUES (@uid, @zid, @od, @do, @typ, 1)";

            await using var cmd = new MySqlCommand(query, conn);
            cmd.Parameters.AddWithValue("@uid", userId);
            cmd.Parameters.AddWithValue("@zid", request.ZastepcaId > 0 ? request.ZastepcaId : (object)DBNull.Value);
            cmd.Parameters.AddWithValue("@od", request.DataOd);
            cmd.Parameters.AddWithValue("@do", request.DataDo);
            cmd.Parameters.AddWithValue("@typ", request.Typ); // 'Urlop' lub 'Delegacja'

            await cmd.ExecuteNonQueryAsync();
            return Ok(ApiResponse<object>.SuccessResponse(null));
        }

        // 5. Usunięcie delegacji
        [HttpDelete("delegation/{id}")]
        public async Task<IActionResult> DeleteDelegation(int id)
        {
            var userId = GetCurrentUserId();
            await using var conn = DbConnectionFactory.CreateDefaultConnection(_configuration);
            await conn.OpenAsync();

            // Usuwamy tylko jeśli należy do usera
            var query = "DELETE FROM Delegacje WHERE Id = @id AND UzytkownikId = @uid";
            await using var cmd = new MySqlCommand(query, conn);
            cmd.Parameters.AddWithValue("@id", id);
            cmd.Parameters.AddWithValue("@uid", userId);

            await cmd.ExecuteNonQueryAsync();
            return Ok(ApiResponse<object>.SuccessResponse(null));
        }

        // 6. Lista użytkowników do wyboru zastępcy
        [HttpGet("users-list")]
        public async Task<IActionResult> GetUsersForReplacement()
        {
            var userId = GetCurrentUserId();
            await using var conn = DbConnectionFactory.CreateDefaultConnection(_configuration);
            await conn.OpenAsync();

            var list = new List<SimpleUserDto>();
            // Pobieramy wszystkich poza samym sobą
            var query = "SELECT Id, `Nazwa Wyświetlana` FROM Uzytkownicy WHERE Id <> @uid AND (Rola IN ('Handlowiec', 'Magazyn')) ORDER BY `Nazwa Wyświetlana`";

            await using var cmd = new MySqlCommand(query, conn);
            cmd.Parameters.AddWithValue("@uid", userId);
            await using var reader = await cmd.ExecuteReaderAsync();
            while (await reader.ReadAsync())
            {
                list.Add(new SimpleUserDto
                {
                    Id = reader.GetInt32("Id"),
                    Name = reader["Nazwa Wyświetlana"].ToString()
                });
            }
            return Ok(ApiResponse<List<SimpleUserDto>>.SuccessResponse(list));
        }
    }

    // DTOs (Umieść w odpowiednim namespace)
    public class UserProfileDto
    {
        public string Login { get; set; }
        public string NazwaWyswietlana { get; set; }
        public string? Email { get; set; }
        public string? Telefon { get; set; }
        public List<DelegacjaDto> Delegacje { get; set; }
    }
    public class DelegacjaDto
    {
        public int Id { get; set; }
        public DateTime DataOd { get; set; }
        public DateTime DataDo { get; set; }
        public string Typ { get; set; }
        public string ZastepcaNazwa { get; set; }
    }
    public class UpdateContactRequest
    {
        public string Email { get; set; }
        public string Telefon { get; set; }
    }
    public class ChangePasswordRequest
    {
        public string OldPassword { get; set; }
        public string NewPassword { get; set; }
    }
    public class AddDelegationRequest
    {
        public DateTime DataOd { get; set; }
        public DateTime DataDo { get; set; }
        public string Typ { get; set; } // "Urlop", "Delegacja"
        public int? ZastepcaId { get; set; }
    }
    public class SimpleUserDto
    {
        public int Id { get; set; }
        public string Name { get; set; }
    }
}