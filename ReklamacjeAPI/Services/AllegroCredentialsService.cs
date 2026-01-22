using MySqlConnector;
using ReklamacjeAPI.Security;

namespace ReklamacjeAPI.Services;

public class AllegroCredentialsService
{
    private readonly IConfiguration _configuration;

    public AllegroCredentialsService(IConfiguration configuration)
    {
        _configuration = configuration;
    }

    public async Task<AllegroAccountCredentials?> GetCredentialsAsync(int accountId)
    {
        await using var connection = DbConnectionFactory.CreateDefaultConnection(_configuration);
        await connection.OpenAsync();

        const string query = @"
            SELECT ClientId,
                   ClientSecretEncrypted,
                   AccessTokenEncrypted,
                   RefreshTokenEncrypted,
                   TokenExpirationDate
            FROM AllegroAccounts
            WHERE Id = @id
            LIMIT 1";

        await using var command = new MySqlCommand(query, connection);
        command.Parameters.AddWithValue("@id", accountId);

        await using var reader = await command.ExecuteReaderAsync();
        if (!await reader.ReadAsync())
        {
            return null;
        }

        var clientId = reader["ClientId"]?.ToString();
        var clientSecretEncrypted = reader["ClientSecretEncrypted"]?.ToString();
        var accessTokenEncrypted = reader["AccessTokenEncrypted"]?.ToString();
        var refreshTokenEncrypted = reader["RefreshTokenEncrypted"]?.ToString();
        var expirationRaw = reader["TokenExpirationDate"]?.ToString();

        var clientSecret = EncryptionHelper.DecryptString(clientSecretEncrypted);
        var accessToken = EncryptionHelper.DecryptString(accessTokenEncrypted);
        var refreshToken = EncryptionHelper.DecryptString(refreshTokenEncrypted);

        DateTime? expirationDate = null;
        if (!string.IsNullOrWhiteSpace(expirationRaw) && DateTime.TryParse(expirationRaw, out var parsed))
        {
            expirationDate = parsed;
        }

        if (string.IsNullOrWhiteSpace(clientId) || string.IsNullOrWhiteSpace(clientSecret))
        {
            return null;
        }

        return new AllegroAccountCredentials(
            accountId,
            clientId,
            clientSecret,
            accessToken,
            refreshToken,
            expirationDate);
    }

    public async Task SaveTokensAsync(int accountId, string accessToken, string? refreshToken, DateTime expiresAt)
    {
        await using var connection = DbConnectionFactory.CreateDefaultConnection(_configuration);
        await connection.OpenAsync();

        var accessTokenEncrypted = EncryptionHelper.EncryptString(accessToken);
        var refreshTokenEncrypted = string.IsNullOrWhiteSpace(refreshToken)
            ? null
            : EncryptionHelper.EncryptString(refreshToken);

        const string query = @"
            UPDATE AllegroAccounts
            SET AccessTokenEncrypted = @accessToken,
                RefreshTokenEncrypted = COALESCE(@refreshToken, RefreshTokenEncrypted),
                TokenExpirationDate = @expiresAt,
                IsAuthorized = 1
            WHERE Id = @id";

        await using var command = new MySqlCommand(query, connection);
        command.Parameters.AddWithValue("@accessToken", accessTokenEncrypted);
        command.Parameters.AddWithValue("@refreshToken", (object?)refreshTokenEncrypted ?? DBNull.Value);
        command.Parameters.AddWithValue("@expiresAt", expiresAt.ToString("o"));
        command.Parameters.AddWithValue("@id", accountId);

        await command.ExecuteNonQueryAsync();
    }
}

public record AllegroAccountCredentials(
    int AccountId,
    string ClientId,
    string ClientSecret,
    string? AccessToken,
    string? RefreshToken,
    DateTime? ExpirationDate);
