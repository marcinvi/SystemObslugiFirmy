using Microsoft.Extensions.Configuration;
using MySqlConnector;
using System.Globalization;
using System.Linq;
using System.Text;


namespace ReklamacjeAPI.Services;

public class ModulesService
{
    private readonly IConfiguration _configuration;

    public ModulesService(IConfiguration configuration)
    {
        _configuration = configuration;
    }

    public async Task<List<string>> GetAssignedModulesAsync(string login)
    {
        var modules = new List<string>();
        if (string.IsNullOrWhiteSpace(login))
        {
            return modules;
        }

        await using var connection = DbConnectionFactory.CreateMagazynConnection(_configuration);
        await connection.OpenAsync();

        modules = await GetModulesFromPermissionsAsync(connection, login);
        if (modules.Count > 0)
        {
            return modules;
        }

        // Fallback po roli - przydatny po migracji do API, gdy tabela Uprawnienia nie została jeszcze uzupełniona
        var role = await GetUserRoleAsync(connection, login);
        if (string.IsNullOrWhiteSpace(role))
        {
            return modules;
        }

        var allModules = await GetAllModulesAsync(connection);
        return MapDefaultModulesByRole(allModules, role);
    }

    private static async Task<List<string>> GetModulesFromPermissionsAsync(MySqlConnection connection, string login)
    {
        const string sql = @"
            SELECT DISTINCT m.NazwaModulu
            FROM Uprawnienia u
            JOIN Moduly m ON u.ModulId = m.Id
            JOIN uzytkownicy usr ON u.UzytkownikId = usr.Id
            WHERE LOWER(TRIM(usr.Login)) = LOWER(TRIM(@login))
            ORDER BY m.Id";

        var modules = new List<string>();
        await using var command = new MySqlCommand(sql, connection);
        command.Parameters.AddWithValue("@login", login);
        await using var reader = await command.ExecuteReaderAsync();
        while (await reader.ReadAsync())
        {
            if (reader["NazwaModulu"] is string moduleName && !string.IsNullOrWhiteSpace(moduleName))
            {
                modules.Add(moduleName);
            }
        }

        return modules;
    }

    private static async Task<string?> GetUserRoleAsync(MySqlConnection connection, string login)
    {
        const string roleSql = @"
            SELECT Rola
            FROM uzytkownicy
            WHERE LOWER(TRIM(Login)) = LOWER(TRIM(@login))
            LIMIT 1";

        await using var command = new MySqlCommand(roleSql, connection);
        command.Parameters.AddWithValue("@login", login);
        var result = await command.ExecuteScalarAsync();
        return result?.ToString();
    }

    private static async Task<List<string>> GetAllModulesAsync(MySqlConnection connection)
    {
        const string sql = @"SELECT NazwaModulu FROM Moduly ORDER BY Id";
        var modules = new List<string>();
        await using var command = new MySqlCommand(sql, connection);
        await using var reader = await command.ExecuteReaderAsync();
        while (await reader.ReadAsync())
        {
            var moduleName = reader["NazwaModulu"]?.ToString();
            if (!string.IsNullOrWhiteSpace(moduleName))
            {
                modules.Add(moduleName);
            }
        }

        return modules;
    }

    private static List<string> MapDefaultModulesByRole(List<string> allModules, string role)
    {
        var normalizedRole = Normalize(role);

        if (normalizedRole.Contains("admin"))
        {
            return allModules;
        }

        var result = new List<string>();
        foreach (var module in allModules)
        {
            var key = Normalize(module);

            if (normalizedRole.Contains("magazyn"))
            {
                if (key.Contains("magazyn") || key.Contains("zwroty") || key.Contains("returns"))
                    result.Add(module);
            }
            else if (normalizedRole.Contains("handlowiec") || normalizedRole.Contains("sprzedaz") || normalizedRole.Contains("sales"))
            {
                if (key.Contains("handlowiec") || key.Contains("sprzedaz") || key.Contains("sales") || key.Contains("zwroty") || key.Contains("returns"))
                    result.Add(module);
            }
            else if (normalizedRole.Contains("reklamacje") || normalizedRole.Contains("complaint"))
            {
                if (key.Contains("reklamacje") || key.Contains("complaint"))
                    result.Add(module);
            }
            else if (normalizedRole.Contains("weryfikacja"))
            {
                if (key.Contains("weryfikacja") || key.Contains("zwroty") || key.Contains("returns"))
                    result.Add(module);
            }
        }

        return result;
    }

    private static string Normalize(string value)
    {
        if (string.IsNullOrWhiteSpace(value))
            return string.Empty;

        var normalized = value.Trim().ToLowerInvariant().Normalize(NormalizationForm.FormD);
        var chars = normalized.Where(c => CharUnicodeInfo.GetUnicodeCategory(c) != UnicodeCategory.NonSpacingMark).ToArray();
        return new string(chars);
    }
}
