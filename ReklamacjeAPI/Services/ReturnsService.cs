using System.Data.Common;
using System.Text;
using Microsoft.EntityFrameworkCore;
using MySqlConnector;
using ReklamacjeAPI.Data;
using ReklamacjeAPI.DTOs;
using ReklamacjeAPI.Models;

namespace ReklamacjeAPI.Services;

public class ReturnsService
{
    private readonly IConfiguration _configuration;
    private readonly ApplicationDbContext _context;
    private string? _uwagiMagazynuColumn;

    public ReturnsService(IConfiguration configuration, ApplicationDbContext context)
    {
        _configuration = configuration;
        _context = context;
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
                countCommand.Parameters.AddWithValue(parameter.ParameterName, parameter.Value);
            }

            var total = await countCommand.ExecuteScalarAsync();
            response.TotalItems = Convert.ToInt32(total ?? 0);
        }

        await using (var command = new MySqlCommand(queryBuilder.ToString(), connection))
        {
            foreach (var parameter in parameters)
            {
                command.Parameters.AddWithValue(parameter.ParameterName, parameter.Value);
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
        await using var connection = DbConnectionFactory.CreateMagazynConnection(_configuration);
        await connection.OpenAsync();
        var uwagiColumn = await ResolveUwagiMagazynuColumnAsync(connection);

        var query = $@"
            SELECT
                acr.*,
                IFNULL(s1.Nazwa, 'Nieprzypisany') AS StanProduktuName,
                IFNULL(s2.Nazwa, 'Nieprzypisany') AS StatusWewnetrzny,
                IFNULL(s3.Nazwa, 'Nieprzypisany') AS DecyzjaHandlowca,
                acr.{uwagiColumn} AS UwagiMagazynuResolved
            FROM AllegroCustomerReturns acr
            LEFT JOIN Statusy s1 ON acr.StanProduktuId = s1.Id
            LEFT JOIN Statusy s2 ON acr.StatusWewnetrznyId = s2.Id
            LEFT JOIN Statusy s3 ON acr.DecyzjaHandlowcaId = s3.Id
            WHERE acr.Id = @id
            LIMIT 1";

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

        string? buyerStreetRaw = reader["Buyer_Street"] as string;
        string? buyerZipRaw = reader["Buyer_ZipCode"] as string;
        string? buyerCityRaw = reader["Buyer_City"] as string;
        string buyerAddressRaw = string.Join(" ", new[] { buyerStreetRaw, buyerZipRaw, buyerCityRaw }
            .Where(s => !string.IsNullOrWhiteSpace(s)));

        string? deliveryStreet = reader["Delivery_Street"] as string;
        string? deliveryZip = reader["Delivery_ZipCode"] as string;
        string? deliveryCity = reader["Delivery_City"] as string;
        string deliveryAddress = string.Join(" ", new[] { deliveryStreet, deliveryZip, deliveryCity }
            .Where(s => !string.IsNullOrWhiteSpace(s)));

        string? deliveryName = BuildName(reader["Delivery_FirstName"] as string, reader["Delivery_LastName"] as string);

        string buyerFullName = GetOptionalString(reader, "BuyerFullName")
            ?? BuildName(reader["Buyer_FirstName"] as string, reader["Buyer_LastName"] as string);

        var przyjetyPrzezId = GetNullableInt(reader, "PrzyjetyPrzezId");
        var przyjetyPrzezName = await GetUserDisplayNameAsync(przyjetyPrzezId);

        var reason = GetOptionalString(reader, "ReturnReasonComment");
        if (string.IsNullOrWhiteSpace(reason))
        {
            reason = GetOptionalString(reader, "ReturnReasonType");
        }

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
            BuyerAddressRaw = string.IsNullOrWhiteSpace(buyerAddressRaw) ? null : buyerAddressRaw,
            BuyerPhoneRaw = reader["Buyer_PhoneNumber"] as string,
            DeliveryName = string.IsNullOrWhiteSpace(deliveryName) ? null : deliveryName,
            DeliveryAddress = string.IsNullOrWhiteSpace(deliveryAddress) ? null : deliveryAddress,
            DeliveryPhone = reader["Delivery_PhoneNumber"] as string,
            Waybill = reader["Waybill"] as string,
            CarrierName = reader["CarrierName"] as string,
            ProductName = reader["ProductName"] as string,
            OfferId = reader["OfferId"] as string,
            Quantity = GetNullableInt(reader, "Quantity"),
            Reason = reason,
            UwagiMagazynu = reader["UwagiMagazynuResolved"] as string,
            StanProduktuId = GetNullableInt(reader, "StanProduktuId"),
            StanProduktuName = reader["StanProduktuName"] as string,
            PrzyjetyPrzezId = przyjetyPrzezId,
            PrzyjetyPrzezName = przyjetyPrzezName,
            DataPrzyjecia = reader["DataPrzyjecia"] == DBNull.Value
                ? null
                : reader.GetDateTime("DataPrzyjecia"),
            DecyzjaHandlowcaId = GetNullableInt(reader, "DecyzjaHandlowcaId"),
            DecyzjaHandlowcaName = reader["DecyzjaHandlowca"] as string,
            KomentarzHandlowca = reader["KomentarzHandlowca"] as string,
            DataDecyzji = reader["DataDecyzji"] as DateTime?,
            IsManual = GetOptionalBool(reader, "IsManual")
        };
    }

    public async Task<bool> UpdateWarehouseAsync(int id, ReturnWarehouseUpdateRequest request)
    {
        await using var connection = DbConnectionFactory.CreateMagazynConnection(_configuration);
        await connection.OpenAsync();
        var uwagiColumn = await ResolveUwagiMagazynuColumnAsync(connection);

        var statusPrzyjetyId = await GetStatusIdAsync(connection, "Przyjęty do magazynu", "StatusWewnetrzny");

        var query = $@"
            UPDATE AllegroCustomerReturns
            SET StanProduktuId = @stanId,
                {uwagiColumn} = @uwagi,
                DataPrzyjecia = @data,
                PrzyjetyPrzezId = @pracownikId,
                StatusWewnetrznyId = @statusId
            WHERE Id = @id";

        await using var command = new MySqlCommand(query, connection);
        command.Parameters.AddWithValue("@stanId", request.StanProduktuId);
        command.Parameters.AddWithValue("@uwagi", (object?)request.UwagiMagazynu ?? DBNull.Value);
        command.Parameters.AddWithValue("@data", request.DataPrzyjecia);
        command.Parameters.AddWithValue("@pracownikId", request.PrzyjetyPrzezId);
        command.Parameters.AddWithValue("@id", id);
        command.Parameters.AddWithValue("@statusId", (object?)statusPrzyjetyId ?? DBNull.Value);

        return await command.ExecuteNonQueryAsync() > 0;
    }

    public async Task<ReturnDecisionResponse?> SaveDecisionAsync(int id, ReturnDecisionRequest request, string userDisplayName)
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

        var decisionTimestamp = DateTime.Now;
        await using (var updateCommand = new MySqlCommand(updateQuery, connection))
        {
            updateCommand.Parameters.AddWithValue("@decyzjaId", request.DecyzjaId);
            updateCommand.Parameters.AddWithValue("@komentarz", (object?)request.Komentarz ?? DBNull.Value);
            updateCommand.Parameters.AddWithValue("@data", decisionTimestamp);
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
        string statusWewnetrzny;
        string decyzjaHandlowca;

        await using (var reader = await fetchCommand.ExecuteReaderAsync())
        {
            if (!await reader.ReadAsync())
            {
                return null;
            }

            statusWewnetrzny = reader["StatusWewnetrzny"]?.ToString() ?? string.Empty;
            decyzjaHandlowca = reader["DecyzjaHandlowca"]?.ToString() ?? string.Empty;
        }

        await AddReturnActionInternalAsync(connection, id, userDisplayName,
            $"Podjęto decyzję: {decyzjaHandlowca}. Komentarz: {request.Komentarz}");

        return new ReturnDecisionResponse
        {
            ReturnId = id,
            StatusWewnetrzny = statusWewnetrzny,
            DecyzjaHandlowca = decyzjaHandlowca,
            DataDecyzji = decisionTimestamp
        };
    }

    public async Task<List<ReturnActionDto>> GetActionsAsync(int returnId)
    {
        await using var connection = DbConnectionFactory.CreateMagazynConnection(_configuration);
        await connection.OpenAsync();

        const string query = @"
            SELECT Id, ZwrotId, Data, Uzytkownik, Tresc
            FROM ZwrotDzialania
            WHERE ZwrotId = @returnId
            ORDER BY Data DESC";

        await using var command = new MySqlCommand(query, connection);
        command.Parameters.AddWithValue("@returnId", returnId);

        var results = new List<ReturnActionDto>();
        await using var reader = await command.ExecuteReaderAsync();
        while (await reader.ReadAsync())
        {
            results.Add(new ReturnActionDto
            {
                Id = reader.GetInt32("Id"),
                ReturnId = reader.GetInt32("ZwrotId"),
                Data = reader.GetDateTime("Data"),
                Uzytkownik = reader["Uzytkownik"]?.ToString() ?? string.Empty,
                Tresc = reader["Tresc"]?.ToString() ?? string.Empty
            });
        }

        return results;
    }

    public async Task<ReturnActionDto?> AddActionAsync(int returnId, string userDisplayName, ReturnActionCreateRequest request)
    {
        await using var connection = DbConnectionFactory.CreateMagazynConnection(_configuration);
        await connection.OpenAsync();
        if (!await ReturnExistsAsync(connection, returnId))
        {
            return null;
        }

        var actionId = await AddReturnActionInternalAsync(connection, returnId, userDisplayName, request.Tresc);
        if (!actionId.HasValue)
        {
            return null;
        }

        return new ReturnActionDto
        {
            Id = actionId.Value,
            ReturnId = returnId,
            Data = DateTime.Now,
            Uzytkownik = userDisplayName,
            Tresc = request.Tresc
        };
    }

    public async Task<int?> CreateManualReturnAsync(ReturnManualCreateRequest request, int userId, string userDisplayName)
    {
        await using var connection = DbConnectionFactory.CreateMagazynConnection(_configuration);
        await connection.OpenAsync();
        await using var transaction = await connection.BeginTransactionAsync();

        try
        {
            var statusDocelowyId = await GetStatusIdAsync(connection, "Oczekuje na decyzję handlowca", "StatusWewnetrzny");
            if (!statusDocelowyId.HasValue)
            {
                return null;
            }

            var uwagiColumn = await ResolveUwagiMagazynuColumnAsync(connection);
            var referenceNumber = await GenerateNewReferenceNumberAsync(connection, transaction);

            var insertQuery = $@"
                INSERT INTO AllegroCustomerReturns (
                    ReferenceNumber, Waybill, CreatedAt, DataPrzyjecia, PrzyjetyPrzezId, StatusAllegro, StatusWewnetrznyId,
                    StanProduktuId, IsManual, ManualSenderDetails, CarrierName, ProductName,
                    Delivery_FirstName, Delivery_LastName, Delivery_Street, Delivery_ZipCode, Delivery_City, Delivery_PhoneNumber,
                    {uwagiColumn}, BuyerFullName
                ) VALUES (
                    @ReferenceNumber, @Waybill, @CreatedAt, @DataPrzyjecia, @PrzyjetyPrzezId, @StatusAllegro, @StatusWewnetrznyId,
                    @StanProduktuId, @IsManual, @ManualSenderDetails, @CarrierName, @ProductName,
                    @Delivery_FirstName, @Delivery_LastName, @Delivery_Street, @Delivery_ZipCode, @Delivery_City, @Delivery_PhoneNumber,
                    @Uwagi, @BuyerFullName
                );
                SELECT LAST_INSERT_ID();";

            var senderDetails = new
            {
                FullName = request.BuyerFullName,
                Street = request.BuyerStreet,
                ZipCode = request.BuyerZipCode,
                City = request.BuyerCity,
                Phone = request.BuyerPhone
            };

            var nameParts = request.BuyerFullName.Trim()
                .Split(new[] { ' ' }, 2, StringSplitOptions.RemoveEmptyEntries);

            await using (var command = new MySqlCommand(insertQuery, connection, transaction))
            {
                command.Parameters.AddWithValue("@ReferenceNumber", referenceNumber);
                command.Parameters.AddWithValue("@Waybill", request.NumerListu);
                command.Parameters.AddWithValue("@CreatedAt", DateTime.Now);
                command.Parameters.AddWithValue("@DataPrzyjecia", DateTime.Now);
                command.Parameters.AddWithValue("@PrzyjetyPrzezId", userId);
                command.Parameters.AddWithValue("@StatusAllegro", "MANUAL");
                command.Parameters.AddWithValue("@StatusWewnetrznyId", statusDocelowyId.Value);
                command.Parameters.AddWithValue("@StanProduktuId", request.StanProduktuId);
                command.Parameters.AddWithValue("@IsManual", 1);
                command.Parameters.AddWithValue("@ManualSenderDetails", System.Text.Json.JsonSerializer.Serialize(senderDetails));
                command.Parameters.AddWithValue("@CarrierName", (object?)request.Przewoznik ?? DBNull.Value);
                command.Parameters.AddWithValue("@ProductName", (object?)request.Produkt ?? DBNull.Value);
                command.Parameters.AddWithValue("@Delivery_FirstName", nameParts.Length > 0 ? nameParts[0] : string.Empty);
                command.Parameters.AddWithValue("@Delivery_LastName", nameParts.Length > 1 ? nameParts[1] : string.Empty);
                command.Parameters.AddWithValue("@Delivery_Street", (object?)request.BuyerStreet ?? DBNull.Value);
                command.Parameters.AddWithValue("@Delivery_ZipCode", (object?)request.BuyerZipCode ?? DBNull.Value);
                command.Parameters.AddWithValue("@Delivery_City", (object?)request.BuyerCity ?? DBNull.Value);
                command.Parameters.AddWithValue("@Delivery_PhoneNumber", (object?)request.BuyerPhone ?? DBNull.Value);
                command.Parameters.AddWithValue("@Uwagi", (object?)request.UwagiMagazynu ?? DBNull.Value);
                command.Parameters.AddWithValue("@BuyerFullName", request.BuyerFullName);

                var newIdObj = await command.ExecuteScalarAsync();
                if (newIdObj == null)
                {
                    await transaction.RollbackAsync();
                    return null;
                }

                var returnId = Convert.ToInt32(newIdObj);
                var readColumn = await ResolveCzyPrzeczytanaColumnAsync(connection);

                foreach (var handlowiecId in request.WybraniHandlowcy)
                {
                    var messageCommand = new MySqlCommand($@"
                        INSERT INTO Wiadomosci (NadawcaId, OdbiorcaId, Tresc, DataWyslania, DotyczyZwrotuId, {readColumn})
                        VALUES (@nadawcaId, @odbiorcaId, @tresc, @data, @zwrotId, 0)", connection, transaction);
                    messageCommand.Parameters.AddWithValue("@nadawcaId", userId);
                    messageCommand.Parameters.AddWithValue("@odbiorcaId", handlowiecId);
                    messageCommand.Parameters.AddWithValue("@tresc", $"Nowy zwrot ręczny ({referenceNumber}) oczekuje na Twoją decyzję.");
                    messageCommand.Parameters.AddWithValue("@data", DateTime.Now);
                    messageCommand.Parameters.AddWithValue("@zwrotId", returnId);
                    await messageCommand.ExecuteNonQueryAsync();
                }

                var odbiorcyLista = request.WybraniHandlowcy.Count > 0
                    ? string.Join(", ", request.WybraniHandlowcy)
                    : "brak";

                await AddReturnActionInternalAsync(connection, returnId, userDisplayName,
                    $"Zwrot przekazany do decyzji handlowców: {odbiorcyLista}.", transaction);

                await AddMagazynDziennikAsync(connection, returnId, userDisplayName,
                    $"Dodano i przekazano do decyzji zwrot ręczny: {referenceNumber}. Odbiorcy: {odbiorcyLista}.", transaction);

                await transaction.CommitAsync();
                return returnId;
            }
        }
        catch
        {
            await transaction.RollbackAsync();
            throw;
        }
    }

    public async Task<bool> ForwardToSalesAsync(int returnId, ReturnForwardToSalesRequest request, int senderId, string userDisplayName)
    {
        await using var connection = DbConnectionFactory.CreateMagazynConnection(_configuration);
        await connection.OpenAsync();
        var uwagiColumn = await ResolveUwagiMagazynuColumnAsync(connection);
        var statusDocelowyId = await GetStatusIdAsync(connection, "Oczekuje na decyzję handlowca", "StatusWewnetrzny");
        var readColumn = await ResolveCzyPrzeczytanaColumnAsync(connection);

        const string returnQuery = @"
            SELECT Id, ReferenceNumber, AllegroAccountId
            FROM AllegroCustomerReturns
            WHERE Id = @id
            LIMIT 1";
        await using var returnCommand = new MySqlCommand(returnQuery, connection);
        returnCommand.Parameters.AddWithValue("@id", returnId);
        await using var returnReader = await returnCommand.ExecuteReaderAsync();
        if (!await returnReader.ReadAsync())
        {
            return false;
        }

        var referenceNumber = returnReader["ReferenceNumber"]?.ToString() ?? $"ZWROT-{returnId}";
        var accountId = returnReader["AllegroAccountId"] == DBNull.Value
            ? (int?)null
            : Convert.ToInt32(returnReader["AllegroAccountId"]);
        await returnReader.CloseAsync();

        if (!accountId.HasValue)
        {
            return false;
        }

        const string opiekunQuery = "SELECT OpiekunId FROM AllegroAccountOpiekun WHERE AllegroAccountId = @id LIMIT 1";
        await using var opiekunCommand = new MySqlCommand(opiekunQuery, connection);
        opiekunCommand.Parameters.AddWithValue("@id", accountId.Value);
        var opiekunIdObj = await opiekunCommand.ExecuteScalarAsync();
        if (opiekunIdObj == null || opiekunIdObj == DBNull.Value)
        {
            return false;
        }

        var opiekunId = Convert.ToInt32(opiekunIdObj);
        var odbiorcaId = opiekunId;

        const string delegacjaQuery = @"
            SELECT ZastepcaId
            FROM Delegacje
            WHERE UzytkownikId = @opiekunId
              AND CURDATE() BETWEEN DataOd AND DataDo
              AND CzyAktywna = 1
            LIMIT 1";
        await using var delegacjaCommand = new MySqlCommand(delegacjaQuery, connection);
        delegacjaCommand.Parameters.AddWithValue("@opiekunId", opiekunId);
        var zastepcaIdObj = await delegacjaCommand.ExecuteScalarAsync();
        if (zastepcaIdObj != null && zastepcaIdObj != DBNull.Value)
        {
            odbiorcaId = Convert.ToInt32(zastepcaIdObj);
        }

        var opiekunName = await GetUserDisplayNameByIdAsync(opiekunId);
        var odbiorcaName = await GetUserDisplayNameByIdAsync(odbiorcaId);
        var odbiorcaLabel = odbiorcaId != opiekunId
            ? $"{odbiorcaName} (zastępstwo za {opiekunName})"
            : opiekunName;

        var query = $@"
            UPDATE AllegroCustomerReturns
            SET StanProduktuId = @stanId,
                {uwagiColumn} = @uwagi,
                StatusWewnetrznyId = @statusId
            WHERE Id = @id";

        await using var command = new MySqlCommand(query, connection);
        command.Parameters.AddWithValue("@stanId", request.StanProduktuId);
        command.Parameters.AddWithValue("@uwagi", (object?)request.UwagiMagazynu ?? DBNull.Value);
        command.Parameters.AddWithValue("@statusId", (object?)statusDocelowyId ?? DBNull.Value);
        command.Parameters.AddWithValue("@id", returnId);

        var updated = await command.ExecuteNonQueryAsync() > 0;
        if (updated)
        {
            var messageCommand = new MySqlCommand($@"
                INSERT INTO Wiadomosci (NadawcaId, OdbiorcaId, Tresc, DataWyslania, DotyczyZwrotuId, {readColumn})
                VALUES (@nadawcaId, @odbiorcaId, @tresc, @data, @zwrotId, 0)", connection);
            messageCommand.Parameters.AddWithValue("@nadawcaId", senderId);
            messageCommand.Parameters.AddWithValue("@odbiorcaId", odbiorcaId);
            messageCommand.Parameters.AddWithValue("@tresc", $"Zwrot {referenceNumber} oczekuje na Twoją decyzję.");
            messageCommand.Parameters.AddWithValue("@data", DateTime.Now);
            messageCommand.Parameters.AddWithValue("@zwrotId", returnId);
            await messageCommand.ExecuteNonQueryAsync();

            await AddReturnActionInternalAsync(connection, returnId, userDisplayName,
                $"Zwrot przekazany do decyzji handlowca ({odbiorcaLabel}).");

            await AddMagazynDziennikAsync(connection, returnId, userDisplayName,
                $"Przekazano do decyzji handlowca: {odbiorcaLabel}.", null);
        }

        return updated;
    }

    public async Task<List<StatusDto>> GetStatusesAsync(string type)
    {
        await using var connection = DbConnectionFactory.CreateMagazynConnection(_configuration);
        await connection.OpenAsync();
        const string query = @"
            SELECT Id, Nazwa, TypStatusu
            FROM Statusy
            WHERE TypStatusu = @type
            ORDER BY Nazwa";
        await using var command = new MySqlCommand(query, connection);
        command.Parameters.AddWithValue("@type", type);
        await using var reader = await command.ExecuteReaderAsync();

        var results = new List<StatusDto>();
        while (await reader.ReadAsync())
        {
            results.Add(new StatusDto
            {
                Id = Convert.ToInt32(reader["Id"]),
                Nazwa = reader["Nazwa"]?.ToString() ?? string.Empty,
                Typ = reader["TypStatusu"]?.ToString() ?? string.Empty
            });
        }

        return results;
    }

    private async Task<string> GetUserDisplayNameByIdAsync(int userId)
    {
        await using var connection = DbConnectionFactory.CreateDefaultConnection(_configuration);
        await connection.OpenAsync();
        const string query = "SELECT `Nazwa Wyświetlana` FROM Uzytkownicy WHERE Id = @id LIMIT 1";
        await using var command = new MySqlCommand(query, connection);
        command.Parameters.AddWithValue("@id", userId);
        var result = await command.ExecuteScalarAsync();
        return result?.ToString() ?? $"Użytkownik {userId}";
    }

    public async Task<bool> ArchiveReturnAsync(int returnId, string userDisplayName)
    {
        await using var connection = DbConnectionFactory.CreateMagazynConnection(_configuration);
        await connection.OpenAsync();
        var statusId = await GetStatusIdAsync(connection, "Archiwalny", "StatusWewnetrzny");

        const string query = @"
            UPDATE AllegroCustomerReturns
            SET StatusWewnetrznyId = @statusId
            WHERE Id = @id";

        await using var command = new MySqlCommand(query, connection);
        command.Parameters.AddWithValue("@statusId", (object?)statusId ?? DBNull.Value);
        command.Parameters.AddWithValue("@id", returnId);
        var updated = await command.ExecuteNonQueryAsync() > 0;
        if (updated)
        {
            await AddReturnActionInternalAsync(connection, returnId, userDisplayName, "Zwrot zarchiwizowany.");
        }

        return updated;
    }

    public async Task<ReturnSummaryResponse> GetSummaryAsync(int? handlowiecId, string? status, DateTime? dateFrom, DateTime? dateTo)
    {
        await using var connection = DbConnectionFactory.CreateMagazynConnection(_configuration);
        await connection.OpenAsync();
        var uwagiColumn = await ResolveUwagiMagazynuColumnAsync(connection);

        var filters = new List<string>();
        var parameters = new List<MySqlParameter>();

        if (handlowiecId.HasValue)
        {
            filters.Add("acr.HandlowiecOpiekunId = @handlowiecId");
            parameters.Add(new MySqlParameter("@handlowiecId", handlowiecId.Value));
        }

        if (!string.IsNullOrWhiteSpace(status))
        {
            filters.Add("s2.Nazwa = @status");
            parameters.Add(new MySqlParameter("@status", status));
        }

        if (dateFrom.HasValue)
        {
            filters.Add("date(acr.CreatedAt) >= date(@from)");
            parameters.Add(new MySqlParameter("@from", dateFrom.Value.ToString("yyyy-MM-dd")));
        }

        if (dateTo.HasValue)
        {
            filters.Add("date(acr.CreatedAt) <= date(@to)");
            parameters.Add(new MySqlParameter("@to", dateTo.Value.ToString("yyyy-MM-dd")));
        }

        var whereSql = filters.Count > 0 ? " WHERE " + string.Join(" AND ", filters) : string.Empty;

        var query = $@"
            SELECT
                acr.Id,
                acr.ReferenceNumber,
                acr.ProductName,
                acr.HandlowiecOpiekunId,
                acr.PrzyjetyPrzezId,
                acr.DataPrzyjecia,
                acr.{uwagiColumn} AS UwagiMagazynu,
                IFNULL(s2.Nazwa, 'Nieznany') AS StatusWew,
                IFNULL(s3.Nazwa, 'Nieznany') AS DecyzjaHandl
            FROM AllegroCustomerReturns acr
            LEFT JOIN Statusy s2 ON s2.Id = acr.StatusWewnetrznyId
            LEFT JOIN Statusy s3 ON s3.Id = acr.DecyzjaHandlowcaId
            {whereSql}
            ORDER BY acr.CreatedAt DESC";

        var items = new List<ReturnSummaryItemDto>();
        var returnIds = new List<int>();
        var przyjecia = new Dictionary<int, (int? UserId, DateTime? Date)>();

        await using (var command = new MySqlCommand(query, connection))
        {
            foreach (var parameter in parameters)
            {
                command.Parameters.AddWithValue(parameter.ParameterName, parameter.Value);
            }

            await using var reader = await command.ExecuteReaderAsync();
            while (await reader.ReadAsync())
            {
                var returnId = reader.GetInt32("Id");
                returnIds.Add(returnId);

                przyjecia[returnId] = (GetNullableInt(reader, "PrzyjetyPrzezId"),
                    reader["DataPrzyjecia"] == DBNull.Value ? null : reader.GetDateTime("DataPrzyjecia"));

                items.Add(new ReturnSummaryItemDto
                {
                    Id = returnId,
                    NumerZwrotu = reader["ReferenceNumber"]?.ToString() ?? string.Empty,
                    Produkt = reader["ProductName"]?.ToString() ?? string.Empty,
                    KtoPrzyjal = string.Empty,
                    KtoPodjalDecyzje = string.Empty,
                    JakaDecyzja = reader["DecyzjaHandl"]?.ToString() ?? string.Empty,
                    UwagiMagazynu = reader["UwagiMagazynu"]?.ToString() ?? string.Empty,
                    UwagiHandlowca = string.Empty,
                    Status = reader["StatusWew"]?.ToString() ?? string.Empty
                });
            }
        }

        var userNames = await GetUserDisplayNamesAsync(przyjecia.Values
            .Select(value => value.UserId)
            .Where(id => id.HasValue)
            .Select(id => id!.Value)
            .Distinct()
            .ToList());

        var decisionMap = (await GetDecisionActionsAsync(connection, returnIds))
            .GroupBy(action => action.ReturnId)
            .ToDictionary(group => group.Key, group => group.First());

        for (var i = 0; i < items.Count; i++)
        {
            var item = items[i];
            if (decisionMap.TryGetValue(item.Id, out var matching))
            {
                item.KtoPodjalDecyzje = matching.User;
                item.UwagiHandlowca = ExtractKomentarz(matching.Content);
            }

            item.KtoPrzyjal = "nie przyjęte";
            if (przyjecia.TryGetValue(item.Id, out var intake) && intake.Date.HasValue)
            {
                if (intake.UserId.HasValue && userNames.TryGetValue(intake.UserId.Value, out var name))
                {
                    item.KtoPrzyjal = name;
                }
                else
                {
                    item.KtoPrzyjal = "(przyjęte)";
                }
            }
        }

        var stats = await GetSummaryStatsAsync(connection, whereSql, parameters);

        return new ReturnSummaryResponse
        {
            Items = items,
            Stats = stats
        };
    }

    public async Task<int?> ForwardToComplaintsAsync(int returnId, ForwardToComplaintRequest request, int userId)
    {
        var klient = await EnsureKlientAsync(request.DaneKlienta);
        var produkt = await EnsureProduktAsync(request.Produkt);

        var zgloszenie = new Zgloszenie
        {
            NrZgloszenia = await GenerateZgloszenieNumberAsync(),
            IdKlienta = klient.Id,
            IdProduktu = produkt?.Id,
            Usterka = request.PowodKlienta,
            Priorytet = "Normalny",
            PrzypisanyDo = null,
            StatusOgolny = "Nowe",
            Uwagi = request.UwagiHandlowca,
            DataZgloszenia = DateTime.UtcNow,
            DataModyfikacji = DateTime.UtcNow
        };

        _context.Zgloszenia.Add(zgloszenie);
        await _context.SaveChangesAsync();

        _context.Dzialania.Add(new Dzialanie
        {
            IdZgloszenia = zgloszenie.Id,
            IdUzytkownika = userId,
            TypDzialania = "utworzenie",
            Opis = "Zgłoszenie utworzone z poziomu zwrotu",
            StatusNowy = "Nowe",
            DataDzialania = DateTime.UtcNow
        });

        await _context.SaveChangesAsync();

        await using var connection = DbConnectionFactory.CreateMagazynConnection(_configuration);
        await connection.OpenAsync();
        var query = "UPDATE AllegroCustomerReturns SET ZgloszenieId = @zgloszenieId WHERE Id = @id";
        await using var updateCommand = new MySqlCommand(query, connection);
        updateCommand.Parameters.AddWithValue("@zgloszenieId", zgloszenie.Id);
        updateCommand.Parameters.AddWithValue("@id", returnId);
        await updateCommand.ExecuteNonQueryAsync();

        await AddReturnActionInternalAsync(connection, returnId, request.Przekazal,
            $"Przekazano do reklamacji. Zgłoszenie: {zgloszenie.NrZgloszenia} (ID {zgloszenie.Id}).");

        return zgloszenie.Id;
    }

    public async Task<ReturnDetailsDto?> GetReturnByCodeAsync(string code)
    {
        if (string.IsNullOrWhiteSpace(code))
        {
            return null;
        }

        await using var connection = DbConnectionFactory.CreateMagazynConnection(_configuration);
        await connection.OpenAsync();

        const string query = @"
            SELECT Id
            FROM AllegroCustomerReturns
            WHERE ReferenceNumber = @code OR Waybill = @code
            LIMIT 1";

        await using var command = new MySqlCommand(query, connection);
        command.Parameters.AddWithValue("@code", code);
        var idObj = await command.ExecuteScalarAsync();
        if (idObj == null)
        {
            return null;
        }

        return await GetReturnDetailsAsync(Convert.ToInt32(idObj));
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

    private static bool HasColumn(DbDataReader reader, string columnName)
    {
        for (var i = 0; i < reader.FieldCount; i++)
        {
            if (string.Equals(reader.GetName(i), columnName, StringComparison.OrdinalIgnoreCase))
            {
                return true;
            }
        }

        return false;
    }

    private static string? GetOptionalString(DbDataReader reader, string columnName)
    {
        if (!HasColumn(reader, columnName))
        {
            return null;
        }

        return reader[columnName] == DBNull.Value ? null : reader[columnName]?.ToString();
    }

    private static bool GetOptionalBool(DbDataReader reader, string columnName)
    {
        if (!HasColumn(reader, columnName))
        {
            return false;
        }

        return reader[columnName] != DBNull.Value && Convert.ToBoolean(reader[columnName]);
    }

    private static int? GetNullableInt(DbDataReader reader, string columnName)
    {
        if (!HasColumn(reader, columnName) || reader[columnName] == DBNull.Value)
        {
            return null;
        }

        return Convert.ToInt32(reader[columnName]);
    }

    private async Task<string?> GetUserDisplayNameAsync(int? userId)
    {
        if (!userId.HasValue)
        {
            return null;
        }

        await using var connection = DbConnectionFactory.CreateDefaultConnection(_configuration);
        await connection.OpenAsync();
        await using var command = new MySqlCommand("SELECT `Nazwa Wyświetlana` FROM Uzytkownicy WHERE Id = @id LIMIT 1", connection);
        command.Parameters.AddWithValue("@id", userId.Value);
        var result = await command.ExecuteScalarAsync();
        return result?.ToString();
    }

    private async Task<Dictionary<int, string>> GetUserDisplayNamesAsync(List<int> userIds)
    {
        var output = new Dictionary<int, string>();
        if (userIds.Count == 0)
        {
            return output;
        }

        await using var connection = DbConnectionFactory.CreateDefaultConnection(_configuration);
        await connection.OpenAsync();

        var placeholders = userIds.Select((_, index) => $"@p{index}").ToList();
        var query = $@"
            SELECT Id, `Nazwa Wyświetlana`
            FROM Uzytkownicy
            WHERE Id IN ({string.Join(", ", placeholders)})";

        await using var command = new MySqlCommand(query, connection);
        for (var i = 0; i < userIds.Count; i++)
        {
            command.Parameters.AddWithValue(placeholders[i], userIds[i]);
        }

        await using var reader = await command.ExecuteReaderAsync();
        while (await reader.ReadAsync())
        {
            output[reader.GetInt32("Id")] = reader["Nazwa Wyświetlana"]?.ToString() ?? string.Empty;
        }

        return output;
    }

    private async Task<int?> AddReturnActionInternalAsync(MySqlConnection connection, int returnId, string userDisplayName, string content, MySqlTransaction? transaction = null)
    {
        var query = @"INSERT INTO ZwrotDzialania (ZwrotId, Data, Uzytkownik, Tresc) VALUES (@zwrotId, @data, @uzytkownik, @tresc)";
        await using var command = new MySqlCommand(query, connection, transaction);
        command.Parameters.AddWithValue("@zwrotId", returnId);
        command.Parameters.AddWithValue("@data", DateTime.Now);
        command.Parameters.AddWithValue("@uzytkownik", userDisplayName);
        command.Parameters.AddWithValue("@tresc", content);
        var rows = await command.ExecuteNonQueryAsync();
        if (rows == 0)
        {
            return null;
        }

        return (int)command.LastInsertedId;
    }

    private static async Task<bool> ReturnExistsAsync(MySqlConnection connection, int returnId)
    {
        const string query = "SELECT 1 FROM AllegroCustomerReturns WHERE Id = @id LIMIT 1";
        await using var command = new MySqlCommand(query, connection);
        command.Parameters.AddWithValue("@id", returnId);
        var result = await command.ExecuteScalarAsync();
        return result != null;
    }

    private async Task AddMagazynDziennikAsync(MySqlConnection connection, int returnId, string userDisplayName, string action, MySqlTransaction? transaction)
    {
        var query = @"INSERT INTO MagazynDziennik (Data, Uzytkownik, Akcja, DotyczyZwrotuId)
                      VALUES (@data, @uzytkownik, @akcja, @id)";
        await using var command = new MySqlCommand(query, connection, transaction);
        command.Parameters.AddWithValue("@data", DateTime.Now);
        command.Parameters.AddWithValue("@uzytkownik", userDisplayName);
        command.Parameters.AddWithValue("@akcja", action);
        command.Parameters.AddWithValue("@id", returnId);
        await command.ExecuteNonQueryAsync();
    }

    private async Task<string> GenerateNewReferenceNumberAsync(MySqlConnection connection, MySqlTransaction transaction)
    {
        var monthYear = DateTime.Now.ToString("MM/yy");
        var pattern = $"R/%/{monthYear}";

        await using var command = new MySqlCommand(
            "SELECT ReferenceNumber FROM AllegroCustomerReturns WHERE ReferenceNumber LIKE @pattern ORDER BY ReferenceNumber DESC LIMIT 1",
            connection,
            transaction);
        command.Parameters.AddWithValue("@pattern", pattern);
        var lastNumberStr = (await command.ExecuteScalarAsync())?.ToString();

        var nextId = 1;
        if (!string.IsNullOrWhiteSpace(lastNumberStr))
        {
            var parts = lastNumberStr.Split('/');
            if (parts.Length >= 2 && int.TryParse(parts[1], out var parsed))
            {
                nextId = parsed + 1;
            }
        }

        return $"R/{nextId:D3}/{monthYear}";
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

    private async Task<List<(int ReturnId, string User, string Content)>> GetDecisionActionsAsync(MySqlConnection connection, List<int> returnIds)
    {
        var results = new List<(int ReturnId, string User, string Content)>();
        if (returnIds.Count == 0)
        {
            return results;
        }

        var placeholders = returnIds.Select((_, index) => $"@id{index}").ToList();
        var query = $@"
            SELECT ZwrotId, Uzytkownik, Tresc
            FROM ZwrotDzialania
            WHERE ZwrotId IN ({string.Join(", ", placeholders)})
              AND Tresc LIKE 'Podjęto decyzję:%'
            ORDER BY Data DESC";

        await using var command = new MySqlCommand(query, connection);
        for (var i = 0; i < returnIds.Count; i++)
        {
            command.Parameters.AddWithValue(placeholders[i], returnIds[i]);
        }

        await using var reader = await command.ExecuteReaderAsync();
        while (await reader.ReadAsync())
        {
            results.Add((
                reader.GetInt32("ZwrotId"),
                reader["Uzytkownik"]?.ToString() ?? string.Empty,
                reader["Tresc"]?.ToString() ?? string.Empty));
        }

        return results;
    }

    private static string ExtractKomentarz(string tresc)
    {
        if (string.IsNullOrWhiteSpace(tresc))
        {
            return string.Empty;
        }

        var idx = tresc.IndexOf("Komentarz:", StringComparison.OrdinalIgnoreCase);
        if (idx < 0)
        {
            return string.Empty;
        }

        return tresc[(idx + "Komentarz:".Length)..].Trim();
    }

    private async Task<ReturnSummaryStatsDto> GetSummaryStatsAsync(
        MySqlConnection connection,
        string whereSql,
        IReadOnlyCollection<MySqlParameter> parameters)
    {
        var query = $@"
            SELECT
                COUNT(acr.Id) AS Total,
                SUM(CASE WHEN IFNULL(s2.Nazwa, '') = 'Oczekuje na decyzję handlowca' THEN 1 ELSE 0 END) AS DoDecyzji,
                SUM(CASE WHEN IFNULL(s2.Nazwa, '') = 'Zakończony' THEN 1 ELSE 0 END) AS Zakonczone
            FROM AllegroCustomerReturns acr
            LEFT JOIN Statusy s2 ON s2.Id = acr.StatusWewnetrznyId
            {whereSql}";

        await using var command = new MySqlCommand(query, connection);
        foreach (var parameter in parameters)
        {
            command.Parameters.AddWithValue(parameter.ParameterName, parameter.Value);
        }

        await using var reader = await command.ExecuteReaderAsync();
        if (!await reader.ReadAsync())
        {
            return new ReturnSummaryStatsDto();
        }

        return new ReturnSummaryStatsDto
        {
            Total = Convert.ToInt32(reader["Total"] ?? 0),
            DoDecyzji = Convert.ToInt32(reader["DoDecyzji"] ?? 0),
            Zakonczone = Convert.ToInt32(reader["Zakonczone"] ?? 0)
        };
    }

    private async Task<Klient> EnsureKlientAsync(ComplaintCustomerDto customer)
    {
        var fullName = $"{customer.Imie} {customer.Nazwisko}".Trim();
        if (!string.IsNullOrWhiteSpace(customer.Email))
        {
            var existing = await _context.Klienci.FirstOrDefaultAsync(k => k.Email == customer.Email);
            if (existing != null)
            {
                return existing;
            }
        }

        if (!string.IsNullOrWhiteSpace(customer.Telefon))
        {
            var existing = await _context.Klienci.FirstOrDefaultAsync(k => k.Telefon == customer.Telefon);
            if (existing != null)
            {
                return existing;
            }
        }

        var klient = new Klient
        {
            ImieNazwisko = fullName,
            Email = customer.Email,
            Telefon = customer.Telefon,
            Adres = customer.Adres?.Ulica,
            KodPocztowy = customer.Adres?.Kod,
            Miasto = customer.Adres?.Miasto,
            DataDodania = DateTime.UtcNow
        };

        _context.Klienci.Add(klient);
        await _context.SaveChangesAsync();
        return klient;
    }

    private async Task<Produkt?> EnsureProduktAsync(ComplaintProductDto product)
    {
        if (string.IsNullOrWhiteSpace(product.Nazwa))
        {
            return null;
        }

        var existing = await _context.Produkty.FirstOrDefaultAsync(p => p.Nazwa == product.Nazwa);
        if (existing != null)
        {
            return existing;
        }

        var produkt = new Produkt
        {
            Nazwa = product.Nazwa,
            NumerSeryjny = product.NrSeryjny,
            DataDodania = DateTime.UtcNow
        };

        _context.Produkty.Add(produkt);
        await _context.SaveChangesAsync();
        return produkt;
    }

    private async Task<string> GenerateZgloszenieNumberAsync()
    {
        var lastZgloszenie = await _context.Zgloszenia
            .OrderByDescending(z => z.Id)
            .FirstOrDefaultAsync();

        var nextNumber = lastZgloszenie != null ? lastZgloszenie.Id + 1 : 1;
        return $"R/{nextNumber}/{DateTime.UtcNow.Year}";
    }
}
