using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using MySqlConnector;
using System;
using ReklamacjeAPI.DTOs;
using ReklamacjeAPI.Services;
using System.Linq;
using System.Threading.Tasks;

namespace ReklamacjeAPI.Controllers
{
    [Authorize]
    [ApiController]
    [Route("api/admin")]
    public class AdminController : ControllerBase
    {
        private readonly IConfiguration _configuration;

        public AdminController(IConfiguration configuration)
        {
            _configuration = configuration;
        }


        [HttpGet("modules")]
        public async Task<IActionResult> GetModules()
        {
            await using var conn = DbConnectionFactory.CreateDefaultConnection(_configuration);
            await conn.OpenAsync();

            var modules = new List<AdminModuleDto>();
            const string sql = "SELECT Id, NazwaModulu FROM Moduly ORDER BY Id";
            await using var cmd = new MySqlCommand(sql, conn);
            await using var reader = await cmd.ExecuteReaderAsync();
            while (await reader.ReadAsync())
            {
                modules.Add(new AdminModuleDto
                {
                    Id = SafeGetInt(reader, "Id"),
                    Name = SafeGetString(reader, "NazwaModulu")
                });
            }

            return Ok(ApiResponse<List<AdminModuleDto>>.SuccessResponse(modules));
        }

        [HttpGet("users")]
        public async Task<IActionResult> GetAllUsers()
        {
            await using var conn = DbConnectionFactory.CreateDefaultConnection(_configuration);
            await conn.OpenAsync();

            var userColumns = await GetTableColumns(conn, "Uzytkownicy");
            var displayCol = PickFirstExisting(userColumns, "Nazwa Wyświetlana", "NazwaWyswietlana") ?? "Login";
            var roleCol = PickFirstExisting(userColumns, "Rola", "Role") ?? "Rola";
            var activeCol = PickFirstExisting(userColumns, "IsActive", "CzyAktywny", "Aktywny") ?? "CzyAktywny";

            var sql = $@"
                SELECT
                    Id,
                    Login,
                    `{displayCol}` AS DisplayName,
                    `{roleCol}` AS RoleName,
                    `{activeCol}` AS ActiveValue
                FROM Uzytkownicy
                ORDER BY Login";

            var users = new List<AdminUserListDto>();
            await using (var cmd = new MySqlCommand(sql, conn))
            await using (var reader = await cmd.ExecuteReaderAsync())
            {
                while (await reader.ReadAsync())
                {
                    users.Add(new AdminUserListDto
                    {
                        Id = SafeGetInt(reader, "Id"),
                        Login = SafeGetString(reader, "Login"),
                        NazwaWyswietlana = SafeGetString(reader, "DisplayName"),
                        Rola = SafeGetString(reader, "RoleName"),
                        IsActive = SafeGetBool(reader, "ActiveValue")
                    });
                }
            }

            var userModuleMap = await GetUserModulesAsync(conn);
            foreach (var user in users)
            {
                user.ModuleIds = userModuleMap.TryGetValue(user.Id, out var moduleIds)
                    ? moduleIds
                    : new List<int>();
            }

            var userModuleMap = await GetUserModulesAsync(conn);
            foreach (var user in users)
            {
                user.ModuleIds = userModuleMap.TryGetValue(user.Id, out var moduleIds)
                    ? moduleIds
                    : new List<int>();
            }

            return Ok(ApiResponse<List<AdminUserListDto>>.SuccessResponse(users));
        }

        [HttpPost("users")]
        public async Task<IActionResult> CreateUser([FromBody] AdminCreateUserDto dto)
        {
            await using var conn = DbConnectionFactory.CreateDefaultConnection(_configuration);
            await conn.OpenAsync();

            var userColumns = await GetTableColumns(conn, "Uzytkownicy");
            var displayCol = PickFirstExisting(userColumns, "Nazwa Wyświetlana", "NazwaWyswietlana");
            var roleCol = PickFirstExisting(userColumns, "Rola", "Role");
            var activeCol = PickFirstExisting(userColumns, "IsActive", "CzyAktywny", "Aktywny");
            var passwordCol = PickFirstExisting(userColumns, "Hasło", "Haslo", "HasloHash", "PasswordHash");

            if (passwordCol == null)
            {
                return StatusCode(500, ApiResponse<object>.ErrorResponse("Brak kolumny hasła w tabeli Uzytkownicy."));
            }

            await using (var checkCmd = new MySqlCommand("SELECT COUNT(1) FROM Uzytkownicy WHERE Login = @login", conn))
            {
                checkCmd.Parameters.AddWithValue("@login", dto.Login);
                var exists = Convert.ToInt32(await checkCmd.ExecuteScalarAsync()) > 0;
                if (exists)
                {
                    return BadRequest(ApiResponse<object>.ErrorResponse("Użytkownik o takim loginie już istnieje."));
                }
            }

            string passwordHash = BCrypt.Net.BCrypt.HashPassword(dto.Password);
            var columns = new List<string> { "Login", $"`{passwordCol}`" };
            var values = new List<string> { "@login", "@pass" };

            if (displayCol != null)
            {
                columns.Add($"`{displayCol}`");
                values.Add("@display");
            }
            if (roleCol != null)
            {
                columns.Add($"`{roleCol}`");
                values.Add("@role");
            }
            if (activeCol != null)
            {
                columns.Add($"`{activeCol}`");
                values.Add("@active");
            }

            var insertSql = $"INSERT INTO Uzytkownicy ({string.Join(", ", columns)}) VALUES ({string.Join(", ", values)})";
            await using var insertCmd = new MySqlCommand(insertSql, conn);
            insertCmd.Parameters.AddWithValue("@login", dto.Login);
            insertCmd.Parameters.AddWithValue("@pass", passwordHash);
            insertCmd.Parameters.AddWithValue("@display", string.IsNullOrWhiteSpace(dto.NazwaWyswietlana) ? dto.Login : dto.NazwaWyswietlana);
            insertCmd.Parameters.AddWithValue("@role", string.IsNullOrWhiteSpace(dto.Rola) ? "User" : dto.Rola);
            insertCmd.Parameters.AddWithValue("@active", 1);
            await insertCmd.ExecuteNonQueryAsync();
            var newUserId = Convert.ToInt32(insertCmd.LastInsertedId);
            await ReplaceUserModulesAsync(conn, newUserId, dto.ModuleIds);

            return Ok(ApiResponse<object>.SuccessResponse(null, "Użytkownik dodany."));
        }

        [HttpPut("users/{id}")]
        public async Task<IActionResult> UpdateUser(int id, [FromBody] AdminUpdateUserDto dto)
        {
            await using var conn = DbConnectionFactory.CreateDefaultConnection(_configuration);
            await conn.OpenAsync();

            var userColumns = await GetTableColumns(conn, "Uzytkownicy");
            var displayCol = PickFirstExisting(userColumns, "Nazwa Wyświetlana", "NazwaWyswietlana");
            var roleCol = PickFirstExisting(userColumns, "Rola", "Role");
            var activeCol = PickFirstExisting(userColumns, "IsActive", "CzyAktywny", "Aktywny");

            var updates = new List<string>();
            await using var cmd = new MySqlCommand();
            cmd.Connection = conn;
            cmd.Parameters.AddWithValue("@id", id);

            if (displayCol != null)
            {
                updates.Add($"`{displayCol}` = @display");
                cmd.Parameters.AddWithValue("@display", dto.NazwaWyswietlana ?? string.Empty);
            }
            if (roleCol != null)
            {
                updates.Add($"`{roleCol}` = @role");
                cmd.Parameters.AddWithValue("@role", dto.Rola ?? "User");
            }
            if (activeCol != null)
            {
                updates.Add($"`{activeCol}` = @active");
                cmd.Parameters.AddWithValue("@active", dto.IsActive ? 1 : 0);
            }

            if (updates.Count == 0)
            {
                return StatusCode(500, ApiResponse<object>.ErrorResponse("Brak obsługiwanych kolumn do aktualizacji użytkownika."));
            }

            cmd.CommandText = $"UPDATE Uzytkownicy SET {string.Join(", ", updates)} WHERE Id = @id";
            var rows = await cmd.ExecuteNonQueryAsync();
            if (rows == 0)
            {
                return NotFound(ApiResponse<object>.ErrorResponse("Nie znaleziono użytkownika."));
            }

            await ReplaceUserModulesAsync(conn, id, dto.ModuleIds);
            return Ok(ApiResponse<object>.SuccessResponse(null, "Dane zaktualizowane."));
        }

        [HttpPost("users/{id}/reset-password")]
        public async Task<IActionResult> ResetPassword(int id, [FromBody] AdminResetPasswordDto dto)
        {
            await using var conn = DbConnectionFactory.CreateDefaultConnection(_configuration);
            await conn.OpenAsync();

            var userColumns = await GetTableColumns(conn, "Uzytkownicy");
            var passwordCol = PickFirstExisting(userColumns, "Hasło", "Haslo", "HasloHash", "PasswordHash");
            if (passwordCol == null)
            {
                return StatusCode(500, ApiResponse<object>.ErrorResponse("Brak kolumny hasła w tabeli Uzytkownicy."));
            }

            var sql = $"UPDATE Uzytkownicy SET `{passwordCol}` = @pass WHERE Id = @id";
            await using var cmd = new MySqlCommand(sql, conn);
            cmd.Parameters.AddWithValue("@pass", BCrypt.Net.BCrypt.HashPassword(dto.NewPassword));
            cmd.Parameters.AddWithValue("@id", id);
            var rows = await cmd.ExecuteNonQueryAsync();

            if (rows == 0)
            {
                return NotFound(ApiResponse<object>.ErrorResponse("Nie znaleziono użytkownika."));
            }

            return Ok(ApiResponse<object>.SuccessResponse(null, "Hasło zostało zresetowane."));
        }



        private static async Task<Dictionary<int, List<int>>> GetUserModulesAsync(MySqlConnection conn)
        {
            var result = new Dictionary<int, List<int>>();
            const string sql = "SELECT UzytkownikId, ModulId FROM Uprawnienia ORDER BY UzytkownikId, ModulId";

            await using var cmd = new MySqlCommand(sql, conn);
            await using var reader = await cmd.ExecuteReaderAsync();
            while (await reader.ReadAsync())
            {
                var userId = SafeGetInt(reader, "UzytkownikId");
                var moduleId = SafeGetInt(reader, "ModulId");
                if (!result.TryGetValue(userId, out var modules))
                {
                    modules = new List<int>();
                    result[userId] = modules;
                }

                modules.Add(moduleId);
            }

            return result;
        }

        private static async Task ReplaceUserModulesAsync(MySqlConnection conn, int userId, List<int>? moduleIds)
        {
            await using var delCmd = new MySqlCommand("DELETE FROM Uprawnienia WHERE UzytkownikId = @uid", conn);
            delCmd.Parameters.AddWithValue("@uid", userId);
            await delCmd.ExecuteNonQueryAsync();

            if (moduleIds == null || moduleIds.Count == 0)
            {
                return;
            }

            foreach (var moduleId in moduleIds.Distinct())
            {
                await using var insCmd = new MySqlCommand("INSERT INTO Uprawnienia (UzytkownikId, ModulId) VALUES (@uid, @mid)", conn);
                insCmd.Parameters.AddWithValue("@uid", userId);
                insCmd.Parameters.AddWithValue("@mid", moduleId);
                await insCmd.ExecuteNonQueryAsync();
            }
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

        private static string? PickFirstExisting(HashSet<string> columns, params string[] candidates)
            => candidates.FirstOrDefault(columns.Contains);

        private static int SafeGetInt(MySqlDataReader reader, string name)
            => int.TryParse(reader[name]?.ToString(), out var v) ? v : 0;

        private static string SafeGetString(MySqlDataReader reader, string name)
            => reader[name]?.ToString() ?? string.Empty;

        private static bool SafeGetBool(MySqlDataReader reader, string name)
        {
            var value = reader[name];
            if (value is bool b) return b;
            if (value is sbyte sb) return sb != 0;
            if (value is byte bb) return bb != 0;
            if (value is short sh) return sh != 0;
            if (value is int i) return i != 0;
            return bool.TryParse(value?.ToString(), out var parsed) && parsed;
        }
    }
}
