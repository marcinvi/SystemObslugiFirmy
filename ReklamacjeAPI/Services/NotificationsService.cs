using MySqlConnector;
using ReklamacjeAPI.DTOs;

namespace ReklamacjeAPI.Services;

public class NotificationsService
{
    private readonly IConfiguration _configuration;

    public NotificationsService(IConfiguration configuration)
    {
        _configuration = configuration;
    }

    /// <summary>
    /// Pobiera powiadomienia dla użytkownika
    /// </summary>
    public async Task<List<NotificationDto>> GetNotificationsAsync(int userId, bool? onlyUnread = null)
    {
        await using var connection = DbConnectionFactory.CreateMagazynConnection(_configuration);
        await connection.OpenAsync();

        var readColumn = await ResolveCzyPrzeczytanaColumnAsync(connection);

        var query = $@"
            SELECT 
                w.Id,
                w.NadawcaId,
                w.OdbiorcaId,
                w.Tytul,
                w.Tresc,
                w.DataWyslania,
                w.DotyczyZwrotuId,
                w.{readColumn} AS CzyPrzeczytana,
                u1.`Nazwa Wyświetlana` AS NadawcaNazwa,
                u2.`Nazwa Wyświetlana` AS OdbiorcaNazwa
            FROM Wiadomosci w
            LEFT JOIN uzytkownicy u1 ON w.NadawcaId = u1.Id
            LEFT JOIN uzytkownicy u2 ON w.OdbiorcaId = u2.Id
            WHERE w.OdbiorcaId = @userId";

        if (onlyUnread.HasValue && onlyUnread.Value)
        {
            query += $" AND w.{readColumn} = 0";
        }

        query += " ORDER BY w.DataWyslania DESC LIMIT 100";

        await using var command = new MySqlCommand(query, connection);
        command.Parameters.AddWithValue("@userId", userId);

        var results = new List<NotificationDto>();
        await using var reader = await command.ExecuteReaderAsync();
        while (await reader.ReadAsync())
        {
            results.Add(new NotificationDto
            {
                Id = reader.GetInt32("Id"),
                NadawcaId = reader.GetInt32("NadawcaId"),
                OdbiorcaId = reader.GetInt32("OdbiorcaId"),
                NadawcaNazwa = reader["NadawcaNazwa"]?.ToString() ?? $"Użytkownik {reader.GetInt32("NadawcaId")}",
                OdbiorcaNazwa = reader["OdbiorcaNazwa"]?.ToString() ?? $"Użytkownik {reader.GetInt32("OdbiorcaId")}",
                Tytul = reader["Tytul"] as string,
                Tresc = reader["Tresc"]?.ToString() ?? string.Empty,
                DataWyslania = reader.GetDateTime("DataWyslania"),
                DotyczyZwrotuId = reader["DotyczyZwrotuId"] == DBNull.Value ? null : Convert.ToInt32(reader["DotyczyZwrotuId"]),
                CzyPrzeczytana = reader["CzyPrzeczytana"] != DBNull.Value && Convert.ToBoolean(reader["CzyPrzeczytana"])
            });
        }

        return results;
    }

    /// <summary>
    /// Pobiera liczbę nieprzeczytanych powiadomień
    /// </summary>
    public async Task<int> GetUnreadCountAsync(int userId)
    {
        await using var connection = DbConnectionFactory.CreateMagazynConnection(_configuration);
        await connection.OpenAsync();

        var readColumn = await ResolveCzyPrzeczytanaColumnAsync(connection);

        var query = $@"
            SELECT COUNT(*) 
            FROM Wiadomosci 
            WHERE OdbiorcaId = @userId AND {readColumn} = 0";

        await using var command = new MySqlCommand(query, connection);
        command.Parameters.AddWithValue("@userId", userId);

        var result = await command.ExecuteScalarAsync();
        return Convert.ToInt32(result ?? 0);
    }

    /// <summary>
    /// Oznacza powiadomienie jako przeczytane
    /// </summary>
    public async Task<bool> MarkAsReadAsync(int notificationId, int userId)
    {
        await using var connection = DbConnectionFactory.CreateMagazynConnection(_configuration);
        await connection.OpenAsync();

        var readColumn = await ResolveCzyPrzeczytanaColumnAsync(connection);

        var query = $@"
            UPDATE Wiadomosci 
            SET {readColumn} = 1 
            WHERE Id = @id AND OdbiorcaId = @userId";

        await using var command = new MySqlCommand(query, connection);
        command.Parameters.AddWithValue("@id", notificationId);
        command.Parameters.AddWithValue("@userId", userId);

        return await command.ExecuteNonQueryAsync() > 0;
    }

    /// <summary>
    /// Oznacza wszystkie powiadomienia jako przeczytane
    /// </summary>
    public async Task<int> MarkAllAsReadAsync(int userId)
    {
        await using var connection = DbConnectionFactory.CreateMagazynConnection(_configuration);
        await connection.OpenAsync();

        var readColumn = await ResolveCzyPrzeczytanaColumnAsync(connection);

        var query = $@"
            UPDATE Wiadomosci 
            SET {readColumn} = 1 
            WHERE OdbiorcaId = @userId AND {readColumn} = 0";

        await using var command = new MySqlCommand(query, connection);
        command.Parameters.AddWithValue("@userId", userId);

        return await command.ExecuteNonQueryAsync();
    }

    /// <summary>
    /// Wysyła powiadomienie do handlowca o nowym zwrocie
    /// </summary>
    public async Task NotifyHandlowiecNewReturnAsync(int handlowiecId, int returnId, string referenceNumber, int senderId)
    {
        await using var connection = DbConnectionFactory.CreateMagazynConnection(_configuration);
        await connection.OpenAsync();

        var readColumn = await ResolveCzyPrzeczytanaColumnAsync(connection);

        var query = $@"
            INSERT INTO Wiadomosci (NadawcaId, OdbiorcaId, Tytul, Tresc, DataWyslania, DotyczyZwrotuId, {readColumn})
            VALUES (@nadawcaId, @odbiorcaId, @tytul, @tresc, @data, @zwrotId, 0)";

        await using var command = new MySqlCommand(query, connection);
        command.Parameters.AddWithValue("@nadawcaId", senderId);
        command.Parameters.AddWithValue("@odbiorcaId", handlowiecId);
        command.Parameters.AddWithValue("@tytul", "Nowy zwrot do decyzji");
        command.Parameters.AddWithValue("@tresc", $"Zwrot {referenceNumber} oczekuje na Twoją decyzję.");
        command.Parameters.AddWithValue("@data", DateTime.Now);
        command.Parameters.AddWithValue("@zwrotId", returnId);

        await command.ExecuteNonQueryAsync();
    }

    /// <summary>
    /// Wysyła powiadomienie do magazynu o podjętej decyzji
    /// </summary>
    public async Task NotifyMagazynDecisionMadeAsync(int returnId, string referenceNumber, string decyzja, int handlowiecId)
    {
        await using var connection = DbConnectionFactory.CreateMagazynConnection(_configuration);
        await connection.OpenAsync();

        // Pobierz wszystkich użytkowników Magazyn
        var magazyniersQuery = @"
            SELECT Id 
            FROM uzytkownicy 
            WHERE Rola = 'Magazyn'";

        var magazynierIds = new List<int>();
        await using (var cmd = new MySqlCommand(magazyniersQuery, connection))
        await using (var reader = await cmd.ExecuteReaderAsync())
        {
            while (await reader.ReadAsync())
            {
                magazynierIds.Add(reader.GetInt32("Id"));
            }
        }

        if (magazynierIds.Count == 0)
        {
            return;
        }

        var readColumn = await ResolveCzyPrzeczytanaColumnAsync(connection);

        var insertQuery = $@"
            INSERT INTO Wiadomosci (NadawcaId, OdbiorcaId, Tytul, Tresc, DataWyslania, DotyczyZwrotuId, {readColumn})
            VALUES (@nadawcaId, @odbiorcaId, @tytul, @tresc, @data, @zwrotId, 0)";

        foreach (var magazynierId in magazynierIds)
        {
            await using var command = new MySqlCommand(insertQuery, connection);
            command.Parameters.AddWithValue("@nadawcaId", handlowiecId);
            command.Parameters.AddWithValue("@odbiorcaId", magazynierId);
            command.Parameters.AddWithValue("@tytul", "Handlowiec podjął decyzję");
            command.Parameters.AddWithValue("@tresc", $"Zwrot {referenceNumber} - podjęto decyzję: {decyzja}");
            command.Parameters.AddWithValue("@data", DateTime.Now);
            command.Parameters.AddWithValue("@zwrotId", returnId);

            await command.ExecuteNonQueryAsync();
        }
    }

    /// <summary>
    /// Wysyła powiadomienie o nowym wpisie w dzienniku zwrotu
    /// </summary>
    public async Task NotifyJournalEntryAsync(int returnId, string referenceNumber, int authorId, string entryText)
    {
        await using var connection = DbConnectionFactory.CreateMagazynConnection(_configuration);
        await connection.OpenAsync();

        // Pobierz opiekuna zwrotu i wszystkich magazynierów
        var recipientsQuery = @"
            SELECT DISTINCT u.Id, u.Rola
            FROM uzytkownicy u
            LEFT JOIN AllegroCustomerReturns acr ON acr.HandlowiecOpiekunId = u.Id
            WHERE (acr.Id = @returnId AND u.Rola = 'Handlowiec')
               OR u.Rola = 'Magazyn'";

        var recipientIds = new List<int>();
        await using (var cmd = new MySqlCommand(recipientsQuery, connection))
        {
            cmd.Parameters.AddWithValue("@returnId", returnId);
            await using var reader = await cmd.ExecuteReaderAsync();
            while (await reader.ReadAsync())
            {
                var userId = reader.GetInt32("Id");
                if (userId != authorId) // Nie wysyłaj powiadomienia autorowi wpisu
                {
                    recipientIds.Add(userId);
                }
            }
        }

        if (recipientIds.Count == 0)
        {
            return;
        }

        var readColumn = await ResolveCzyPrzeczytanaColumnAsync(connection);

        var insertQuery = $@"
            INSERT INTO Wiadomosci (NadawcaId, OdbiorcaId, Tytul, Tresc, DataWyslania, DotyczyZwrotuId, {readColumn})
            VALUES (@nadawcaId, @odbiorcaId, @tytul, @tresc, @data, @zwrotId, 0)";

        // Skróć treść wpisu do 100 znaków
        var shortEntry = entryText.Length > 100 ? entryText.Substring(0, 97) + "..." : entryText;

        foreach (var recipientId in recipientIds)
        {
            await using var command = new MySqlCommand(insertQuery, connection);
            command.Parameters.AddWithValue("@nadawcaId", authorId);
            command.Parameters.AddWithValue("@odbiorcaId", recipientId);
            command.Parameters.AddWithValue("@tytul", "Nowy wpis w dzienniku zwrotu");
            command.Parameters.AddWithValue("@tresc", $"Zwrot {referenceNumber}: {shortEntry}");
            command.Parameters.AddWithValue("@data", DateTime.Now);
            command.Parameters.AddWithValue("@zwrotId", returnId);

            await command.ExecuteNonQueryAsync();
        }
    }

    private async Task<string> ResolveCzyPrzeczytanaColumnAsync(MySqlConnection connection)
    {
        const string query = @"
            SELECT COLUMN_NAME
            FROM INFORMATION_SCHEMA.COLUMNS
            WHERE TABLE_SCHEMA = DATABASE()
              AND TABLE_NAME = 'Wiadomosci'
              AND COLUMN_NAME IN ('CzyPrzeczytana', 'CzyOdczytana')
            LIMIT 1";

        await using var command = new MySqlCommand(query, connection);
        var result = await command.ExecuteScalarAsync();
        return result?.ToString() ?? "CzyPrzeczytana";
    }
}
