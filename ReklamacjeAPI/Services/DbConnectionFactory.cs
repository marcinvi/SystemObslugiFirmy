using Microsoft.Extensions.Configuration;
using MySqlConnector;

namespace ReklamacjeAPI.Services;

public static class DbConnectionFactory
{
    public static MySqlConnection CreateMagazynConnection(IConfiguration configuration)
    {
        var connectionString = configuration.GetConnectionString("MagazynConnection")
            ?? configuration.GetConnectionString("DefaultConnection")
            ?? throw new InvalidOperationException("MagazynConnection or DefaultConnection is not configured.");

        return new MySqlConnection(connectionString);
    }
}
