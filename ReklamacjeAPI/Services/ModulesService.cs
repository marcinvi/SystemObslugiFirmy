using Microsoft.Extensions.Configuration;
using MySqlConnector;

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

        const string sql = @"
            SELECT DISTINCT m.NazwaModulu
            FROM Uprawnienia u
            JOIN Moduly m ON u.ModulId = m.Id
            JOIN Uzytkownicy usr ON u.UzytkownikId = usr.Id
            WHERE usr.Login = @login
            ORDER BY m.Id";

        await using var connection = DbConnectionFactory.CreateMagazynConnection(_configuration);
        await connection.OpenAsync();
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
}
