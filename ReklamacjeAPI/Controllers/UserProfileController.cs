using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MySqlConnector;
using ReklamacjeAPI.DTOs;
using ReklamacjeAPI.Services;
using System.Linq;
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

        [HttpGet]
        public async Task<IActionResult> GetProfile()
        {
            var userId = GetCurrentUserId();
            if (userId == 0) return Unauthorized();

            await using var conn = DbConnectionFactory.CreateDefaultConnection(_configuration);
            await conn.OpenAsync();

            var userColumns = await GetTableColumns(conn, "Uzytkownicy");
            var displayCol = PickFirstExisting(userColumns, "Nazwa Wyświetlana", "NazwaWyswietlana") ?? "Login";
            var hasEmail = userColumns.Contains("Email");
            var phoneCol = PickFirstExisting(userColumns, "Telefon", "Phone", "NrTelefonu");

            var selected = new List<string>
            {
                "Login",
                $"`{displayCol}` AS DisplayName"
            };
            if (hasEmail) selected.Add("Email");
            if (phoneCol != null) selected.Add($"`{phoneCol}` AS Telefon");

            var query = $"SELECT {string.Join(", ", selected)} FROM Uzytkownicy WHERE Id = @id";
            var userDto = new UserProfileDto { Delegacje = new List<DelegacjaDto>() };

            await using (var cmd = new MySqlCommand(query, conn))
            {
                cmd.Parameters.AddWithValue("@id", userId);
                await using var reader = await cmd.ExecuteReaderAsync();
                if (!await reader.ReadAsync())
                {
                    return NotFound(ApiResponse<UserProfileDto>.ErrorResponse("Nie znaleziono użytkownika."));
                }

                userDto.Login = reader["Login"]?.ToString() ?? string.Empty;
                userDto.NazwaWyswietlana = reader["DisplayName"]?.ToString() ?? userDto.Login;
                userDto.Email = hasEmail ? reader["Email"]?.ToString() : null;
                userDto.Telefon = phoneCol != null ? reader["Telefon"]?.ToString() : null;
            }

            var hasDelegacjeTable = await TableExists(conn, "Delegacje");
            if (hasDelegacjeTable)
            {
                var delQuery = @"
                    SELECT d.Id, d.DataOd, d.DataDo, d.Typ, u.`Nazwa Wyświetlana` as ZastepcaNazwa
                    FROM Delegacje d
                    LEFT JOIN Uzytkownicy u ON d.ZastepcaId = u.Id
                    WHERE d.UzytkownikId = @uid AND d.DataDo >= CURDATE() AND d.CzyAktywna = 1
                    ORDER BY d.DataOd ASC";

                await using var cmd = new MySqlCommand(delQuery, conn);
                cmd.Parameters.AddWithValue("@uid", userId);
                await using var reader = await cmd.ExecuteReaderAsync();
                while (await reader.ReadAsync())
                {
                    userDto.Delegacje.Add(new DelegacjaDto
                    {
                        Id = reader.GetInt32("Id"),
                        DataOd = reader.GetDateTime("DataOd"),
                        DataDo = reader.GetDateTime("DataDo"),
                        Typ = reader["Typ"]?.ToString(),
                        ZastepcaNazwa = reader["ZastepcaNazwa"]?.ToString() ?? "Brak"
                    });
                }
            }

            return Ok(ApiResponse<UserProfileDto>.SuccessResponse(userDto));
        }

        [HttpPost("contact")]
        public async Task<IActionResult> UpdateContact([FromBody] UpdateContactRequest request)
        {
            var userId = GetCurrentUserId();
            if (userId == 0) return Unauthorized();

            await using var conn = DbConnectionFactory.CreateDefaultConnection(_configuration);
            await conn.OpenAsync();

            var userColumns = await GetTableColumns(conn, "Uzytkownicy");
            var phoneCol = PickFirstExisting(userColumns, "Telefon", "Phone", "NrTelefonu");

            var updates = new List<string>();
            await using var cmd = new MySqlCommand();
            cmd.Connection = conn;
            cmd.Parameters.AddWithValue("@id", userId);

            if (userColumns.Contains("Email"))
            {
                updates.Add("Email = @email");
                cmd.Parameters.AddWithValue("@email", request.Email);
            }
            if (phoneCol != null)
            {
                updates.Add($"`{phoneCol}` = @phone");
                cmd.Parameters.AddWithValue("@phone", request.Telefon);
            }

            if (updates.Count == 0)
            {
                return BadRequest(ApiResponse<object>.ErrorResponse("Brak kolumn kontaktowych (Email/Telefon) w tabeli Uzytkownicy."));
            }

            cmd.CommandText = $"UPDATE Uzytkownicy SET {string.Join(", ", updates)} WHERE Id = @id";
            await cmd.ExecuteNonQueryAsync();
            return Ok(ApiResponse<object>.SuccessResponse(null));
        }

        [HttpPost("password")]
        public async Task<IActionResult> ChangePassword([FromBody] ChangePasswordRequest request)
        {
            var userId = GetCurrentUserId();
            if (userId == 0) return Unauthorized();

            await using var conn = DbConnectionFactory.CreateDefaultConnection(_configuration);
            await conn.OpenAsync();

            var userColumns = await GetTableColumns(conn, "Uzytkownicy");
            var passwordCol = PickFirstExisting(userColumns, "Hasło", "Haslo", "HasloHash", "PasswordHash");
            if (passwordCol == null)
            {
                return StatusCode(500, ApiResponse<object>.ErrorResponse("Brak kolumny hasła w tabeli Uzytkownicy."));
            }

            var query = $"SELECT `{passwordCol}` FROM Uzytkownicy WHERE Id = @id";
            string currentHash = string.Empty;
            await using (var cmd = new MySqlCommand(query, conn))
            {
                cmd.Parameters.AddWithValue("@id", userId);
                currentHash = (await cmd.ExecuteScalarAsync())?.ToString() ?? string.Empty;
            }

            if (string.IsNullOrWhiteSpace(currentHash) || !BCrypt.Net.BCrypt.Verify(request.OldPassword, currentHash))
            {
                return BadRequest(ApiResponse<object>.ErrorResponse("Stare hasło jest nieprawidłowe."));
            }

            string newHash = BCrypt.Net.BCrypt.HashPassword(request.NewPassword);
            var updateQuery = $"UPDATE Uzytkownicy SET `{passwordCol}` = @h WHERE Id = @id";
            await using (var cmd = new MySqlCommand(updateQuery, conn))
            {
                cmd.Parameters.AddWithValue("@h", newHash);
                cmd.Parameters.AddWithValue("@id", userId);
                await cmd.ExecuteNonQueryAsync();
            }

            return Ok(ApiResponse<object>.SuccessResponse(null));
        }

        [HttpPost("delegation")]
        public async Task<IActionResult> AddDelegation([FromBody] AddDelegationRequest request)
        {
            var userId = GetCurrentUserId();
            await using var conn = DbConnectionFactory.CreateDefaultConnection(_configuration);
            await conn.OpenAsync();

            var query = @"
                INSERT INTO Delegacje (UzytkownikId, ZastepcaId, DataOd, DataDo, Typ, CzyAktywna)
                VALUES (@uid, @zid, @od, @do, @typ, 1)";

            await using var cmd = new MySqlCommand(query, conn);
            cmd.Parameters.AddWithValue("@uid", userId);
            cmd.Parameters.AddWithValue("@zid", request.ZastepcaId > 0 ? request.ZastepcaId : (object)DBNull.Value);
            cmd.Parameters.AddWithValue("@od", request.DataOd);
            cmd.Parameters.AddWithValue("@do", request.DataDo);
            cmd.Parameters.AddWithValue("@typ", request.Typ);

            await cmd.ExecuteNonQueryAsync();
            return Ok(ApiResponse<object>.SuccessResponse(null));
        }

        [HttpDelete("delegation/{id}")]
        public async Task<IActionResult> DeleteDelegation(int id)
        {
            var userId = GetCurrentUserId();
            await using var conn = DbConnectionFactory.CreateDefaultConnection(_configuration);
            await conn.OpenAsync();

            var query = "DELETE FROM Delegacje WHERE Id = @id AND UzytkownikId = @uid";
            await using var cmd = new MySqlCommand(query, conn);
            cmd.Parameters.AddWithValue("@id", id);
            cmd.Parameters.AddWithValue("@uid", userId);

            await cmd.ExecuteNonQueryAsync();
            return Ok(ApiResponse<object>.SuccessResponse(null));
        }

        [HttpGet("users-list")]
        public async Task<IActionResult> GetUsersForReplacement()
        {
            var userId = GetCurrentUserId();
            await using var conn = DbConnectionFactory.CreateDefaultConnection(_configuration);
            await conn.OpenAsync();

            var userColumns = await GetTableColumns(conn, "Uzytkownicy");
            var displayCol = PickFirstExisting(userColumns, "Nazwa Wyświetlana", "NazwaWyswietlana") ?? "Login";
            var roleCol = PickFirstExisting(userColumns, "Rola", "Role");

            var query = roleCol == null
                ? $"SELECT Id, `{displayCol}` AS DisplayName FROM Uzytkownicy WHERE Id <> @uid ORDER BY `{displayCol}`"
                : $"SELECT Id, `{displayCol}` AS DisplayName FROM Uzytkownicy WHERE Id <> @uid AND (`{roleCol}` IN ('Handlowiec', 'Magazyn')) ORDER BY `{displayCol}`";

            var list = new List<SimpleUserDto>();
            await using var cmd = new MySqlCommand(query, conn);
            cmd.Parameters.AddWithValue("@uid", userId);
            await using var reader = await cmd.ExecuteReaderAsync();
            while (await reader.ReadAsync())
            {
                list.Add(new SimpleUserDto
                {
                    Id = reader.GetInt32("Id"),
                    Name = reader["DisplayName"]?.ToString() ?? string.Empty
                });
            }

            return Ok(ApiResponse<List<SimpleUserDto>>.SuccessResponse(list));
        }

        private static async Task<HashSet<string>> GetTableColumns(MySqlConnection conn, string tableName)
        {
            var result = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
            const string sql = @"
                SELECT COLUMN_NAME
                FROM INFORMATION_SCHEMA.COLUMNS
                WHERE TABLE_SCHEMA = DATABASE() AND TABLE_NAME = @table";

            await using var cmd = new MySqlCommand(sql, conn);
            cmd.Parameters.AddWithValue("@table", tableName);
            await using var reader = await cmd.ExecuteReaderAsync();
            while (await reader.ReadAsync())
            {
                var name = reader["COLUMN_NAME"]?.ToString();
                if (!string.IsNullOrWhiteSpace(name)) result.Add(name);
            }

            return result;
        }

        private static async Task<bool> TableExists(MySqlConnection conn, string tableName)
        {
            const string sql = @"
                SELECT COUNT(1)
                FROM INFORMATION_SCHEMA.TABLES
                WHERE TABLE_SCHEMA = DATABASE() AND TABLE_NAME = @table";

            await using var cmd = new MySqlCommand(sql, conn);
            cmd.Parameters.AddWithValue("@table", tableName);
            return Convert.ToInt32(await cmd.ExecuteScalarAsync()) > 0;
        }

        private static string? PickFirstExisting(HashSet<string> columns, params string[] candidates)
            => candidates.FirstOrDefault(columns.Contains);
    }

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
        public string? Email { get; set; }
        public string? Telefon { get; set; }
    }

    public class ChangePasswordRequest
    {
        public string OldPassword { get; set; }
        public string NewPassword { get; set; }
    }

    public class AddDelegationRequest
    {
        public int? ZastepcaId { get; set; }
        public DateTime DataOd { get; set; }
        public DateTime DataDo { get; set; }
        public string Typ { get; set; }
    }

    public class SimpleUserDto
    {
        public int Id { get; set; }
        public string Name { get; set; }

        public override string ToString() => Name;
    }
}
