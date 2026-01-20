using System.Text;
using MySqlConnector;
using ReklamacjeAPI.DTOs;

namespace ReklamacjeAPI.Services;

public class ReturnsService
{
    private readonly IConfiguration _configuration;
    private string? _uwagiMagazynuColumn;

    public ReturnsService(IConfiguration configuration)
    {
        _configuration = configuration;
    }

    public async Task<PaginatedResponse<ReturnListItemDto>> GetReturnsAsync(
        int page,
        int pageSize,
        string? statusWewnetrzny,
        string? statusAllegro,
        int? handlowiecId,
        string? search)
    {
        var queryBuilder = new StringBuilder(@"
            SELECT
                acr.Id,
                acr.ReferenceNumber,
                acr.Waybill,
                acr.CreatedAt,
                acr.StatusAllegro,
                acr.ProductName,
                acr.HandlowiecOpiekunId,
                acr.IsManual,
                COALESCE(
                    NULLIF(acr.BuyerFullName, ''),
                    CONCAT(acr.Delivery_FirstName, ' ', acr.Delivery_LastName),
                    CONCAT(acr.Buyer_FirstName, ' ', acr.Buyer_LastName),
                    acr.BuyerLogin
                ) AS BuyerName,
                IFNULL(s1.Nazwa, 'Nieprzypisany') AS StanProduktu,
                IFNULL(s2.Nazwa, 'Nieprzypisany') AS StatusWewnetrzny,
                IFNULL(s3.Nazwa, 'Nieprzypisany') AS DecyzjaHandlowca
            FROM AllegroCustomerReturns acr
            LEFT JOIN Statusy s1 ON acr.StanProduktuId = s1.Id
            LEFT JOIN Statusy s2 ON acr.StatusWewnetrznyId = s2.Id
            LEFT JOIN Statusy s3 ON acr.DecyzjaHandlowcaId = s3.Id
        ");

        var conditions = new List<string>();
        var parameters = new List<MySqlParameter>();

        if (!string.IsNullOrWhiteSpace(statusWewnetrzny))
        {
            conditions.Add("s2.Nazwa = @statusWewnetrzny");
            parameters.Add(new MySqlParameter("@statusWewnetrzny", statusWewnetrzny));
        }

        if (!string.IsNullOrWhiteSpace(statusAllegro))
        {
            conditions.Add("acr.StatusAllegro = @statusAllegro");
            parameters.Add(new MySqlParameter("@statusAllegro", statusAllegro));
        }

        if (handlowiecId.HasValue)
        {
            conditions.Add("acr.HandlowiecOpiekunId = @handlowiecId");
            parameters.Add(new MySqlParameter("@handlowiecId", handlowiecId.Value));
        }

        if (!string.IsNullOrWhiteSpace(search))
        {
            conditions.Add(@"(
                acr.ReferenceNumber LIKE @search OR
                acr.BuyerLogin LIKE @search OR
                acr.Waybill LIKE @search OR
                CONCAT(acr.Delivery_FirstName, ' ', acr.Delivery_LastName) LIKE @search OR
                CONCAT(acr.Buyer_FirstName, ' ', acr.Buyer_LastName) LIKE @search OR
                acr.ProductName LIKE @search
            )");
            parameters.Add(new MySqlParameter("@search", $"%{search}%"));
        }

        if (conditions.Count > 0)
        {
            queryBuilder.Append(" WHERE ").Append(string.Join(" AND ", conditions));
        }

        var countQuery = $"SELECT COUNT(*) FROM ({queryBuilder}) AS c";
        queryBuilder.Append(" ORDER BY acr.CreatedAt DESC LIMIT @limit OFFSET @offset");
        parameters.Add(new MySqlParameter("@limit", pageSize));
        parameters.Add(new MySqlParameter("@offset", (page - 1) * pageSize));

        var response = new PaginatedResponse<ReturnListItemDto>
        {
            Page = page,
            PageSize = pageSize
        };

        await using var connection = DbConnectionFactory.CreateMagazynConnection(_configuration);
        await connection.OpenAsync();

        await using (var countCommand = new MySqlCommand(countQuery, connection))
        {
            foreach (var parameter in parameters.Where(p => p.ParameterName != "@limit" && p.ParameterName != "@offset"))
            {
                countCommand.Parameters.Add(parameter.ParameterName, parameter.Value);
            }

            var total = await countCommand.ExecuteScalarAsync();
            response.TotalItems = Convert.ToInt32(total ?? 0);
        }

        await using (var command = new MySqlCommand(queryBuilder.ToString(), connection))
        {
            foreach (var parameter in parameters)
            {
                command.Parameters.Add(parameter.ParameterName, parameter.Value);
            }

            await using var reader = await command.ExecuteReaderAsync();
            while (await reader.ReadAsync())
            {
                response.Items.Add(new ReturnListItemDto
                {
                    Id = reader.GetInt32("Id"),
                    ReferenceNumber = reader["ReferenceNumber"]?.ToString() ?? string.Empty,
                    Waybill = reader["Waybill"] as string,
                    BuyerName = reader["BuyerName"]?.ToString() ?? string.Empty,
                    ProductName = reader["ProductName"] as string,
                    CreatedAt = reader.GetDateTime("CreatedAt"),
                    StatusAllegro = reader["StatusAllegro"] as string,
                    StatusWewnetrzny = reader["StatusWewnetrzny"] as string,
                    StanProduktu = reader["StanProduktu"] as string,
                    DecyzjaHandlowca = reader["DecyzjaHandlowca"] as string,
                    HandlowiecId = reader["HandlowiecOpiekunId"] == DBNull.Value
                        ? null
                        : Convert.ToInt32(reader["HandlowiecOpiekunId"]),
                    IsManual = reader["IsManual"] != DBNull.Value && Convert.ToBoolean(reader["IsManual"])
                });
            }
        }

        return response;
    }

    public async Task<ReturnDetailsDto?> GetReturnDetailsAsync(int id)
    {
        const string query = @"
            SELECT
                acr.*,
                IFNULL(s1.Nazwa, 'Nieprzypisany') AS StanProduktuName,
                IFNULL(s2.Nazwa, 'Nieprzypisany') AS StatusWewnetrzny,
                IFNULL(s3.Nazwa, 'Nieprzypisany') AS DecyzjaHandlowca
            FROM AllegroCustomerReturns acr
            LEFT JOIN Statusy s1 ON acr.StanProduktuId = s1.Id
            LEFT JOIN Statusy s2 ON acr.StatusWewnetrznyId = s2.Id
            LEFT JOIN Statusy s3 ON acr.DecyzjaHandlowcaId = s3.Id
            WHERE acr.Id = @id
            LIMIT 1";

        await using var connection = DbConnectionFactory.CreateMagazynConnection(_configuration);
        await connection.OpenAsync();
        await using var command = new MySqlCommand(query, connection);
        command.Parameters.AddWithValue("@id", id);

        await using var reader = await command.ExecuteReaderAsync();
        if (!await reader.ReadAsync())
        {
            return null;
        }

        string? buyerStreet = reader["Delivery_Street"] as string ?? reader["Buyer_Street"] as string;
        string? buyerZip = reader["Delivery_ZipCode"] as string ?? reader["Buyer_ZipCode"] as string;
        string? buyerCity = reader["Delivery_City"] as string ?? reader["Buyer_City"] as string;
        string buyerAddress = string.Join(" ", new[] { buyerStreet, buyerZip, buyerCity }.Where(s => !string.IsNullOrWhiteSpace(s)));

        string buyerFullName = reader["BuyerFullName"] as string
            ?? BuildName(reader["Buyer_FirstName"] as string, reader["Buyer_LastName"] as string);

        return new ReturnDetailsDto
        {
            Id = reader.GetInt32("Id"),
            ReferenceNumber = reader["ReferenceNumber"]?.ToString() ?? string.Empty,
            CreatedAt = reader.GetDateTime("CreatedAt"),
            StatusWewnetrzny = reader["StatusWewnetrzny"] as string,
            StatusAllegro = reader["StatusAllegro"] as string,
            BuyerLogin = reader["BuyerLogin"] as string,
            BuyerName = string.IsNullOrWhiteSpace(buyerFullName)
                ? BuildName(reader["Delivery_FirstName"] as string, reader["Delivery_LastName"] as string)
                : buyerFullName,
            BuyerPhone = reader["Buyer_PhoneNumber"] as string ?? reader["Delivery_PhoneNumber"] as string,
            BuyerAddress = string.IsNullOrWhiteSpace(buyerAddress) ? null : buyerAddress,
            Waybill = reader["Waybill"] as string,
            CarrierName = reader["CarrierName"] as string,
            ProductName = reader["ProductName"] as string,
            OfferId = reader["OfferId"] as string,
            Quantity = reader["Quantity"] as int?,
            Reason = null,
            UwagiMagazynu = reader["UwagiMagazyn"] as string ?? reader["UwagiMagazynu"] as string,
            StanProduktuId = reader["StanProduktuId"] as int?,
            StanProduktuName = reader["StanProduktuName"] as string,
            PrzyjetyPrzezId = reader["PrzyjetyPrzezId"] as int?,
            PrzyjetyPrzezName = null,
            DataPrzyjecia = reader["DataPrzyjecia"] as DateTime?,
            DecyzjaHandlowcaId = reader["DecyzjaHandlowcaId"] as int?,
            DecyzjaHandlowcaName = reader["DecyzjaHandlowca"] as string,
            KomentarzHandlowca = reader["KomentarzHandlowca"] as string,
            DataDecyzji = reader["DataDecyzji"] as DateTime?,
            IsManual = Convert.ToBoolean(reader["IsManual"] ?? false)
        };
    }

    public async Task<bool> UpdateWarehouseAsync(int id, ReturnWarehouseUpdateRequest request)
    {
        await using var connection = DbConnectionFactory.CreateMagazynConnection(_configuration);
        await connection.OpenAsync();
        var uwagiColumn = await ResolveUwagiMagazynuColumnAsync(connection);

        var query = $@"
            UPDATE AllegroCustomerReturns
            SET StanProduktuId = @stanId,
                {uwagiColumn} = @uwagi,
                DataPrzyjecia = @data,
                PrzyjetyPrzezId = @pracownikId
            WHERE Id = @id";

        await using var command = new MySqlCommand(query, connection);
        command.Parameters.AddWithValue("@stanId", request.StanProduktuId);
        command.Parameters.AddWithValue("@uwagi", (object?)request.UwagiMagazynu ?? DBNull.Value);
        command.Parameters.AddWithValue("@data", request.DataPrzyjecia);
        command.Parameters.AddWithValue("@pracownikId", request.PrzyjetyPrzezId);
        command.Parameters.AddWithValue("@id", id);

        return await command.ExecuteNonQueryAsync() > 0;
    }

    public async Task<ReturnDecisionResponse?> SaveDecisionAsync(int id, ReturnDecisionRequest request)
    {
        await using var connection = DbConnectionFactory.CreateMagazynConnection(_configuration);
        await connection.OpenAsync();

        var statusZakonczonyId = await GetStatusIdAsync(connection, "Zakończony", "StatusWewnetrzny");
        if (statusZakonczonyId == null)
        {
            return null;
        }

        const string updateQuery = @"
            UPDATE AllegroCustomerReturns
            SET DecyzjaHandlowcaId = @decyzjaId,
                KomentarzHandlowca = @komentarz,
                DataDecyzji = @data,
                StatusWewnetrznyId = @statusId
            WHERE Id = @id";

        await using (var updateCommand = new MySqlCommand(updateQuery, connection))
        {
            updateCommand.Parameters.AddWithValue("@decyzjaId", request.DecyzjaId);
            updateCommand.Parameters.AddWithValue("@komentarz", (object?)request.Komentarz ?? DBNull.Value);
            updateCommand.Parameters.AddWithValue("@data", DateTime.Now);
            updateCommand.Parameters.AddWithValue("@statusId", statusZakonczonyId.Value);
            updateCommand.Parameters.AddWithValue("@id", id);

            await updateCommand.ExecuteNonQueryAsync();
        }

        const string fetchQuery = @"
            SELECT s2.Nazwa AS StatusWewnetrzny, s3.Nazwa AS DecyzjaHandlowca
            FROM AllegroCustomerReturns acr
            LEFT JOIN Statusy s2 ON acr.StatusWewnetrznyId = s2.Id
            LEFT JOIN Statusy s3 ON acr.DecyzjaHandlowcaId = s3.Id
            WHERE acr.Id = @id";

        await using var fetchCommand = new MySqlCommand(fetchQuery, connection);
        fetchCommand.Parameters.AddWithValue("@id", id);
        await using var reader = await fetchCommand.ExecuteReaderAsync();
        if (!await reader.ReadAsync())
        {
            return null;
        }

        return new ReturnDecisionResponse
        {
            ReturnId = id,
            StatusWewnetrzny = reader["StatusWewnetrzny"]?.ToString() ?? string.Empty,
            DecyzjaHandlowca = reader["DecyzjaHandlowca"]?.ToString() ?? string.Empty,
            DataDecyzji = DateTime.Now
        };
    }

    private async Task<int?> GetStatusIdAsync(MySqlConnection connection, string name, string type)
    {
        const string query = "SELECT Id FROM Statusy WHERE Nazwa = @name AND TypStatusu = @type LIMIT 1";
        await using var command = new MySqlCommand(query, connection);
        command.Parameters.AddWithValue("@name", name);
        command.Parameters.AddWithValue("@type", type);
        var result = await command.ExecuteScalarAsync();
        return result == null ? null : Convert.ToInt32(result);
    }

    public async Task<int?> GetUserIdByLoginAsync(string login)
    {
        if (string.IsNullOrWhiteSpace(login))
        {
            return null;
        }

        await using var connection = DbConnectionFactory.CreateDefaultConnection(_configuration);
        await connection.OpenAsync();
        await using var command = new MySqlCommand("SELECT Id FROM Uzytkownicy WHERE Login = @login LIMIT 1", connection);
        command.Parameters.AddWithValue("@login", login);
        var result = await command.ExecuteScalarAsync();
        return result == null ? null : Convert.ToInt32(result);
    }

    private async Task<string> ResolveUwagiMagazynuColumnAsync(MySqlConnection connection)
    {
        if (!string.IsNullOrWhiteSpace(_uwagiMagazynuColumn))
        {
            return _uwagiMagazynuColumn;
        }

        const string query = @"
            SELECT COLUMN_NAME
            FROM INFORMATION_SCHEMA.COLUMNS
            WHERE TABLE_SCHEMA = DATABASE()
              AND TABLE_NAME = 'AllegroCustomerReturns'
              AND COLUMN_NAME IN ('UwagiMagazynu', 'UwagiMagazyn')
            LIMIT 1";
        await using var command = new MySqlCommand(query, connection);
        var result = await command.ExecuteScalarAsync();
        _uwagiMagazynuColumn = result?.ToString() ?? "UwagiMagazynu";
        return _uwagiMagazynuColumn;
    }

    private static string BuildName(string? firstName, string? lastName)
    {
        return string.Join(" ", new[] { firstName, lastName }.Where(s => !string.IsNullOrWhiteSpace(s)));
    }
}
