using MySqlConnector;
using ReklamacjeAPI.DTOs;

namespace ReklamacjeAPI.Services;

public class MessagesService
{
    private readonly IConfiguration _configuration;
    private string? _czyPrzeczytanaColumn;

    public MessagesService(IConfiguration configuration)
    {
        _configuration = configuration;
    }

    public async Task<List<MessageDto>> GetMessagesAsync(int? returnId)
    {
        await using var connection = DbConnectionFactory.CreateMagazynConnection(_configuration);
        await connection.OpenAsync();
        var readColumn = await ResolveCzyPrzeczytanaColumnAsync(connection);

        var query = $@"
            SELECT Id, NadawcaId, OdbiorcaId, Tytul, Tresc, DataWyslania, DotyczyZwrotuId, {readColumn} AS CzyPrzeczytana
            FROM Wiadomosci";

        var parameters = new List<MySqlParameter>();
        if (returnId.HasValue)
        {
            query += " WHERE DotyczyZwrotuId = @returnId";
            parameters.Add(new MySqlParameter("@returnId", returnId.Value));
        }

        query += " ORDER BY DataWyslania DESC";

        await using var command = new MySqlCommand(query, connection);
        foreach (var parameter in parameters)
        {
            command.Parameters.AddWithValue(parameter.ParameterName, parameter.Value);
        }

        var results = new List<MessageDto>();
        await using var reader = await command.ExecuteReaderAsync();
        while (await reader.ReadAsync())
        {
            results.Add(new MessageDto
            {
                Id = reader.GetInt32("Id"),
                NadawcaId = reader.GetInt32("NadawcaId"),
                OdbiorcaId = reader.GetInt32("OdbiorcaId"),
                Tytul = reader["Tytul"] as string,
                Tresc = reader["Tresc"]?.ToString() ?? string.Empty,
                DataWyslania = reader.GetDateTime("DataWyslania"),
                DotyczyZwrotuId = reader["DotyczyZwrotuId"] == DBNull.Value
                    ? null
                    : Convert.ToInt32(reader["DotyczyZwrotuId"]),
                CzyPrzeczytana = reader["CzyPrzeczytana"] != DBNull.Value && Convert.ToBoolean(reader["CzyPrzeczytana"])
            });
        }

        return results;
    }

    public async Task<MessageDto?> CreateMessageAsync(MessageCreateRequest request)
    {
        await using var connection = DbConnectionFactory.CreateMagazynConnection(_configuration);
        await connection.OpenAsync();
        var readColumn = await ResolveCzyPrzeczytanaColumnAsync(connection);

        var query = $@"
            INSERT INTO Wiadomosci (NadawcaId, OdbiorcaId, Tytul, Tresc, DataWyslania, DotyczyZwrotuId, {readColumn})
            VALUES (@nadawcaId, @odbiorcaId, @tytul, @tresc, @data, @zwrotId, 0);
            SELECT LAST_INSERT_ID();";

        await using var command = new MySqlCommand(query, connection);
        command.Parameters.AddWithValue("@nadawcaId", request.NadawcaId);
        command.Parameters.AddWithValue("@odbiorcaId", request.OdbiorcaId);
        command.Parameters.AddWithValue("@tytul", (object?)request.Tytul ?? DBNull.Value);
        command.Parameters.AddWithValue("@tresc", request.Tresc);
        command.Parameters.AddWithValue("@data", DateTime.Now);
        command.Parameters.AddWithValue("@zwrotId", (object?)request.DotyczyZwrotuId ?? DBNull.Value);

        var newIdObj = await command.ExecuteScalarAsync();
        if (newIdObj == null)
        {
            return null;
        }

        var newId = Convert.ToInt32(newIdObj);
        return await GetMessageByIdAsync(connection, readColumn, newId);
    }

    public async Task<bool> MarkReadAsync(int id)
    {
        await using var connection = DbConnectionFactory.CreateMagazynConnection(_configuration);
        await connection.OpenAsync();
        var readColumn = await ResolveCzyPrzeczytanaColumnAsync(connection);

        var query = $"UPDATE Wiadomosci SET {readColumn} = 1 WHERE Id = @id";
        await using var command = new MySqlCommand(query, connection);
        command.Parameters.AddWithValue("@id", id);

        return await command.ExecuteNonQueryAsync() > 0;
    }

    private async Task<string> ResolveCzyPrzeczytanaColumnAsync(MySqlConnection connection)
    {
        if (!string.IsNullOrWhiteSpace(_czyPrzeczytanaColumn))
        {
            return _czyPrzeczytanaColumn;
        }

        const string query = @"
            SELECT COLUMN_NAME
            FROM INFORMATION_SCHEMA.COLUMNS
            WHERE TABLE_SCHEMA = DATABASE()
              AND TABLE_NAME = 'Wiadomosci'
              AND COLUMN_NAME IN ('CzyPrzeczytana', 'CzyOdczytana')
            LIMIT 1";

        await using var command = new MySqlCommand(query, connection);
        var result = await command.ExecuteScalarAsync();
        _czyPrzeczytanaColumn = result?.ToString() ?? "CzyPrzeczytana";
        return _czyPrzeczytanaColumn;
    }

    private static async Task<MessageDto?> GetMessageByIdAsync(MySqlConnection connection, string readColumn, int id)
    {
        var query = $@"
            SELECT Id, NadawcaId, OdbiorcaId, Tytul, Tresc, DataWyslania, DotyczyZwrotuId, {readColumn} AS CzyPrzeczytana
            FROM Wiadomosci
            WHERE Id = @id";

        await using var command = new MySqlCommand(query, connection);
        command.Parameters.AddWithValue("@id", id);
        await using var reader = await command.ExecuteReaderAsync();
        if (!await reader.ReadAsync())
        {
            return null;
        }

        return new MessageDto
        {
            Id = reader.GetInt32("Id"),
            NadawcaId = reader.GetInt32("NadawcaId"),
            OdbiorcaId = reader.GetInt32("OdbiorcaId"),
            Tytul = reader["Tytul"] as string,
            Tresc = reader["Tresc"]?.ToString() ?? string.Empty,
            DataWyslania = reader.GetDateTime("DataWyslania"),
            DotyczyZwrotuId = reader["DotyczyZwrotuId"] == DBNull.Value
                ? null
                : Convert.ToInt32(reader["DotyczyZwrotuId"]),
            CzyPrzeczytana = reader["CzyPrzeczytana"] != DBNull.Value && Convert.ToBoolean(reader["CzyPrzeczytana"])
        };
    }
}
