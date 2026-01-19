using System.Linq;
using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MySqlConnector;
using ReklamacjeAPI.DTOs;
using ReklamacjeAPI.Services;

namespace ReklamacjeAPI.Controllers;

[Authorize]
[ApiController]
[Route("api/returns")]
public class ReturnsController : ControllerBase
{
    private readonly IConfiguration _configuration;

    public ReturnsController(IConfiguration configuration)
    {
        _configuration = configuration;
    }

    [HttpGet]
    public async Task<ActionResult<ApiResponse<PaginatedResponse<ReturnListItemDto>>>> GetReturns(
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 20,
        [FromQuery] string? statusWewnetrzny = null,
        [FromQuery] string? statusAllegro = null,
        [FromQuery] int? handlowiecId = null,
        [FromQuery] string? search = null)
    {
        if (page < 1 || pageSize < 1 || pageSize > 200)
        {
            return BadRequest(ApiResponse<PaginatedResponse<ReturnListItemDto>>.ErrorResponse(
                "Nieprawidłowe parametry paginacji"));
        }

        var conditions = new List<string>();
        var parameters = new List<MySqlParameter>();

        if (!string.IsNullOrWhiteSpace(statusWewnetrzny))
        {
            conditions.Add("s2.Nazwa = @statusWew");
            parameters.Add(new MySqlParameter("@statusWew", statusWewnetrzny));
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
                acr.ProductName LIKE @search
            )");
            parameters.Add(new MySqlParameter("@search", $"%{search}%"));
        }

        var whereClause = conditions.Count > 0 ? "WHERE " + string.Join(" AND ", conditions) : string.Empty;

        var countQuery = $@"
            SELECT COUNT(*)
            FROM AllegroCustomerReturns acr
            LEFT JOIN Statusy s1 ON acr.StanProduktuId = s1.Id
            LEFT JOIN Statusy s2 ON acr.StatusWewnetrznyId = s2.Id
            LEFT JOIN Statusy s3 ON acr.DecyzjaHandlowcaId = s3.Id
            {whereClause};";

        var dataQuery = $@"
            SELECT
                acr.Id,
                acr.ReferenceNumber,
                acr.Waybill,
                COALESCE(acr.BuyerFullName, acr.BuyerLogin, 'Nieznany klient') AS BuyerName,
                acr.ProductName,
                acr.CreatedAt,
                acr.StatusAllegro,
                s2.Nazwa AS StatusWewnetrzny,
                s1.Nazwa AS StanProduktu,
                s3.Nazwa AS DecyzjaHandlowca,
                acr.HandlowiecOpiekunId,
                IFNULL(acr.IsManual, 0) AS IsManual
            FROM AllegroCustomerReturns acr
            LEFT JOIN Statusy s1 ON acr.StanProduktuId = s1.Id
            LEFT JOIN Statusy s2 ON acr.StatusWewnetrznyId = s2.Id
            LEFT JOIN Statusy s3 ON acr.DecyzjaHandlowcaId = s3.Id
            {whereClause}
            ORDER BY acr.CreatedAt DESC
            LIMIT @limit OFFSET @offset;";

        await using var connection = DbConnectionFactory.CreateMagazynConnection(_configuration);
        await connection.OpenAsync();

        var countCommand = new MySqlCommand(countQuery, connection);
        countCommand.Parameters.AddRange(parameters.ToArray());
        var totalItems = Convert.ToInt32(await countCommand.ExecuteScalarAsync());

        var dataCommand = new MySqlCommand(dataQuery, connection);
        dataCommand.Parameters.AddRange(parameters.ToArray());
        dataCommand.Parameters.AddWithValue("@limit", pageSize);
        dataCommand.Parameters.AddWithValue("@offset", (page - 1) * pageSize);

        var items = new List<ReturnListItemDto>();
        await using (var reader = await dataCommand.ExecuteReaderAsync())
        {
            while (await reader.ReadAsync())
            {
                items.Add(new ReturnListItemDto
                {
                    Id = reader.GetInt32("Id"),
                    ReferenceNumber = reader.GetString("ReferenceNumber"),
                    Waybill = reader.IsDBNull("Waybill") ? null : reader.GetString("Waybill"),
                    BuyerName = reader.GetString("BuyerName"),
                    ProductName = reader.IsDBNull("ProductName") ? null : reader.GetString("ProductName"),
                    CreatedAt = reader.GetDateTime("CreatedAt"),
                    StatusAllegro = reader.IsDBNull("StatusAllegro") ? null : reader.GetString("StatusAllegro"),
                    StatusWewnetrzny = reader.IsDBNull("StatusWewnetrzny") ? null : reader.GetString("StatusWewnetrzny"),
                    StanProduktu = reader.IsDBNull("StanProduktu") ? null : reader.GetString("StanProduktu"),
                    DecyzjaHandlowca = reader.IsDBNull("DecyzjaHandlowca") ? null : reader.GetString("DecyzjaHandlowca"),
                    HandlowiecId = reader.IsDBNull("HandlowiecOpiekunId") ? null : reader.GetInt32("HandlowiecOpiekunId"),
                    IsManual = reader.GetInt32("IsManual") == 1
                });
            }
        }

        var response = new PaginatedResponse<ReturnListItemDto>
        {
            Items = items,
            Page = page,
            PageSize = pageSize,
            TotalItems = totalItems
        };

        return Ok(ApiResponse<PaginatedResponse<ReturnListItemDto>>.SuccessResponse(response));
    }

    [HttpGet("{id:int}")]
    public async Task<ActionResult<ApiResponse<ReturnDetailsDto>>> GetReturn(int id)
    {
        await using var connection = DbConnectionFactory.CreateMagazynConnection(_configuration);
        await connection.OpenAsync();

        var uwagiColumn = await ResolveUwagiMagazynuColumnAsync(connection);

        var query = $@"
            SELECT
                acr.Id,
                acr.ReferenceNumber,
                acr.CreatedAt,
                acr.StatusAllegro,
                acr.StatusWewnetrznyId,
                s2.Nazwa AS StatusWewnetrznyName,
                acr.BuyerLogin,
                acr.BuyerFullName,
                acr.Delivery_FirstName,
                acr.Delivery_LastName,
                acr.Buyer_FirstName,
                acr.Buyer_LastName,
                acr.Delivery_PhoneNumber,
                acr.Buyer_PhoneNumber,
                acr.Delivery_Street,
                acr.Delivery_ZipCode,
                acr.Delivery_City,
                acr.Buyer_Street,
                acr.Buyer_ZipCode,
                acr.Buyer_City,
                acr.Waybill,
                acr.CarrierName,
                acr.ProductName,
                acr.OfferId,
                acr.Quantity,
                acr.{uwagiColumn} AS UwagiMagazynu,
                acr.StanProduktuId,
                s1.Nazwa AS StanProduktuName,
                acr.PrzyjetyPrzezId,
                u.`Nazwa Wyświetlana` AS PrzyjetyPrzezName,
                acr.DataPrzyjecia,
                acr.DecyzjaHandlowcaId,
                s3.Nazwa AS DecyzjaHandlowcaName,
                acr.KomentarzHandlowca,
                acr.DataDecyzji,
                IFNULL(acr.IsManual, 0) AS IsManual
            FROM AllegroCustomerReturns acr
            LEFT JOIN Statusy s1 ON acr.StanProduktuId = s1.Id
            LEFT JOIN Statusy s2 ON acr.StatusWewnetrznyId = s2.Id
            LEFT JOIN Statusy s3 ON acr.DecyzjaHandlowcaId = s3.Id
            LEFT JOIN Uzytkownicy u ON u.Id = acr.PrzyjetyPrzezId
            WHERE acr.Id = @id
            LIMIT 1;";

        await using var command = new MySqlCommand(query, connection);
        command.Parameters.AddWithValue("@id", id);

        await using var reader = await command.ExecuteReaderAsync();
        if (!await reader.ReadAsync())
        {
            return NotFound(ApiResponse<ReturnDetailsDto>.ErrorResponse("Zwrot nie znaleziony"));
        }

        var buyerName = BuildBuyerName(reader);
        var buyerPhone = BuildPhone(reader);
        var buyerAddress = BuildAddress(reader);

        var dto = new ReturnDetailsDto
        {
            Id = reader.GetInt32("Id"),
            ReferenceNumber = reader.GetString("ReferenceNumber"),
            CreatedAt = reader.GetDateTime("CreatedAt"),
            StatusWewnetrzny = reader.IsDBNull("StatusWewnetrznyName") ? null : reader.GetString("StatusWewnetrznyName"),
            StatusAllegro = reader.IsDBNull("StatusAllegro") ? null : reader.GetString("StatusAllegro"),
            BuyerLogin = reader.IsDBNull("BuyerLogin") ? null : reader.GetString("BuyerLogin"),
            BuyerName = buyerName,
            BuyerPhone = buyerPhone,
            BuyerAddress = buyerAddress,
            Waybill = reader.IsDBNull("Waybill") ? null : reader.GetString("Waybill"),
            CarrierName = reader.IsDBNull("CarrierName") ? null : reader.GetString("CarrierName"),
            ProductName = reader.IsDBNull("ProductName") ? null : reader.GetString("ProductName"),
            OfferId = reader.IsDBNull("OfferId") ? null : reader.GetString("OfferId"),
            Quantity = reader.IsDBNull("Quantity") ? null : reader.GetInt32("Quantity"),
            Reason = null,
            UwagiMagazynu = reader.IsDBNull("UwagiMagazynu") ? null : reader.GetString("UwagiMagazynu"),
            StanProduktuId = reader.IsDBNull("StanProduktuId") ? null : reader.GetInt32("StanProduktuId"),
            StanProduktuName = reader.IsDBNull("StanProduktuName") ? null : reader.GetString("StanProduktuName"),
            PrzyjetyPrzezId = reader.IsDBNull("PrzyjetyPrzezId") ? null : reader.GetInt32("PrzyjetyPrzezId"),
            PrzyjetyPrzezName = reader.IsDBNull("PrzyjetyPrzezName") ? null : reader.GetString("PrzyjetyPrzezName"),
            DataPrzyjecia = reader.IsDBNull("DataPrzyjecia") ? null : reader.GetDateTime("DataPrzyjecia"),
            DecyzjaHandlowcaId = reader.IsDBNull("DecyzjaHandlowcaId") ? null : reader.GetInt32("DecyzjaHandlowcaId"),
            DecyzjaHandlowcaName = reader.IsDBNull("DecyzjaHandlowcaName") ? null : reader.GetString("DecyzjaHandlowcaName"),
            KomentarzHandlowca = reader.IsDBNull("KomentarzHandlowca") ? null : reader.GetString("KomentarzHandlowca"),
            DataDecyzji = reader.IsDBNull("DataDecyzji") ? null : reader.GetDateTime("DataDecyzji"),
            IsManual = reader.GetInt32("IsManual") == 1
        };

        return Ok(ApiResponse<ReturnDetailsDto>.SuccessResponse(dto));
    }

    [HttpPatch("{id:int}/warehouse")]
    public async Task<ActionResult<ApiResponse<object>>> UpdateWarehouse(int id, [FromBody] ReturnWarehouseUpdateRequest request)
    {
        await using var connection = DbConnectionFactory.CreateMagazynConnection(_configuration);
        await connection.OpenAsync();

        var uwagiColumn = await ResolveUwagiMagazynuColumnAsync(connection);

        var query = $@"
            UPDATE AllegroCustomerReturns
            SET StanProduktuId = @stanId,
                {uwagiColumn} = @uwagi,
                DataPrzyjecia = @data,
                PrzyjetyPrzezId = @przyjety
            WHERE Id = @id;";

        await using var command = new MySqlCommand(query, connection);
        command.Parameters.AddWithValue("@stanId", request.StanProduktuId);
        command.Parameters.AddWithValue("@uwagi", request.UwagiMagazynu ?? string.Empty);
        command.Parameters.AddWithValue("@data", request.DataPrzyjecia);
        command.Parameters.AddWithValue("@przyjety", request.PrzyjetyPrzezId);
        command.Parameters.AddWithValue("@id", id);

        var updated = await command.ExecuteNonQueryAsync();
        if (updated == 0)
        {
            return NotFound(ApiResponse<object>.ErrorResponse("Zwrot nie znaleziony"));
        }

        return Ok(ApiResponse<object>.SuccessResponse(null, "Zapisano zmiany magazynu"));
    }

    [HttpPost("{id:int}/forward-to-sales")]
    public async Task<ActionResult<ApiResponse<object>>> ForwardToSales(int id, [FromBody] ReturnForwardToSalesRequest request)
    {
        var userId = GetUserId();
        if (userId <= 0)
        {
            return Unauthorized(ApiResponse<object>.ErrorResponse("Nieprawidłowy użytkownik"));
        }

        var displayName = GetDisplayName();

        await using var connection = DbConnectionFactory.CreateMagazynConnection(_configuration);
        await connection.OpenAsync();
        await using var transaction = await connection.BeginTransactionAsync();

        try
        {
            var uwagiColumn = await ResolveUwagiMagazynuColumnAsync(connection);

            var returnCommand = new MySqlCommand(@"
                SELECT ReferenceNumber, BuyerLogin, AllegroAccountId
                FROM AllegroCustomerReturns
                WHERE Id = @id
                LIMIT 1;", connection, transaction);
            returnCommand.Parameters.AddWithValue("@id", id);

            string referenceNumber;
            string? buyerLogin;
            int? allegroAccountId;

            await using (var reader = await returnCommand.ExecuteReaderAsync())
            {
                if (!await reader.ReadAsync())
                {
                    return NotFound(ApiResponse<object>.ErrorResponse("Zwrot nie znaleziony"));
                }

                referenceNumber = reader.GetString("ReferenceNumber");
                buyerLogin = reader.IsDBNull("BuyerLogin") ? null : reader.GetString("BuyerLogin");
                allegroAccountId = reader.IsDBNull("AllegroAccountId") ? null : reader.GetInt32("AllegroAccountId");
            }

            if (!allegroAccountId.HasValue)
            {
                return BadRequest(ApiResponse<object>.ErrorResponse(
                    "Nie można automatycznie przekazać zwrotu ręcznego. Skontaktuj się z handlowcem bezpośrednio."));
            }

            var opiekunCommand = new MySqlCommand(@"
                SELECT OpiekunId
                FROM AllegroAccountOpiekun
                WHERE AllegroAccountId = @id
                LIMIT 1;", connection, transaction);
            opiekunCommand.Parameters.AddWithValue("@id", allegroAccountId.Value);

            var opiekunObj = await opiekunCommand.ExecuteScalarAsync();
            if (opiekunObj == null || opiekunObj == DBNull.Value)
            {
                return BadRequest(ApiResponse<object>.ErrorResponse(
                    "Dla tego konta Allegro nie przypisano opiekuna."));
            }

            var opiekunId = Convert.ToInt32(opiekunObj);
            var recipientId = await ResolveDelegateAsync(connection, transaction, opiekunId) ?? opiekunId;

            var statusId = await GetStatusIdAsync(connection, transaction, "Oczekuje na decyzję handlowca", "StatusWewnetrzny");
            if (statusId == null)
            {
                return BadRequest(ApiResponse<object>.ErrorResponse(
                    "Brak statusu 'Oczekuje na decyzję handlowca'."));
            }

            var updateQuery = $@"
                UPDATE AllegroCustomerReturns
                SET StanProduktuId = @stanId,
                    {uwagiColumn} = @uwagi,
                    DataPrzyjecia = @data,
                    PrzyjetyPrzezId = @przyjety,
                    StatusWewnetrznyId = @statusId,
                    HandlowiecOpiekunId = @opiekunId
                WHERE Id = @id;";

            var updateCommand = new MySqlCommand(updateQuery, connection, transaction);
            updateCommand.Parameters.AddWithValue("@stanId", request.StanProduktuId);
            updateCommand.Parameters.AddWithValue("@uwagi", request.UwagiMagazynu ?? string.Empty);
            updateCommand.Parameters.AddWithValue("@data", DateTime.Now);
            updateCommand.Parameters.AddWithValue("@przyjety", userId);
            updateCommand.Parameters.AddWithValue("@statusId", statusId.Value);
            updateCommand.Parameters.AddWithValue("@opiekunId", opiekunId);
            updateCommand.Parameters.AddWithValue("@id", id);
            await updateCommand.ExecuteNonQueryAsync();

            var messageText = $"Prośba o decyzję dla zwrotu nr {referenceNumber} od {buyerLogin}";
            var messageCommand = new MySqlCommand(@"
                INSERT INTO Wiadomosci (NadawcaId, OdbiorcaId, Tresc, DataWyslania, DotyczyZwrotuId, CzyPrzeczytana)
                VALUES (@nadawca, @odbiorca, @tresc, @data, @zwrotId, 0);", connection, transaction);
            messageCommand.Parameters.AddWithValue("@nadawca", userId);
            messageCommand.Parameters.AddWithValue("@odbiorca", recipientId);
            messageCommand.Parameters.AddWithValue("@tresc", messageText);
            messageCommand.Parameters.AddWithValue("@data", DateTime.Now);
            messageCommand.Parameters.AddWithValue("@zwrotId", id);
            await messageCommand.ExecuteNonQueryAsync();

            var actionText = $"Zwrot przekazany do: {recipientId}. Stan: '{request.StanProduktuId}'. Uwagi: {request.UwagiMagazynu}";
            var actionCommand = new MySqlCommand(@"
                INSERT INTO ZwrotDzialania (ZwrotId, Data, Uzytkownik, Tresc)
                VALUES (@zwrotId, @data, @uzytkownik, @tresc);", connection, transaction);
            actionCommand.Parameters.AddWithValue("@zwrotId", id);
            actionCommand.Parameters.AddWithValue("@data", DateTime.Now);
            actionCommand.Parameters.AddWithValue("@uzytkownik", displayName);
            actionCommand.Parameters.AddWithValue("@tresc", actionText);
            await actionCommand.ExecuteNonQueryAsync();

            var dziennikCommand = new MySqlCommand(@"
                INSERT INTO MagazynDziennik (Data, Uzytkownik, Akcja, DotyczyZwrotuId)
                VALUES (@data, @uzytkownik, @akcja, @id);", connection, transaction);
            dziennikCommand.Parameters.AddWithValue("@data", DateTime.Now);
            dziennikCommand.Parameters.AddWithValue("@uzytkownik", displayName);
            dziennikCommand.Parameters.AddWithValue("@akcja", $"Przekazano zwrot do: {recipientId}.");
            dziennikCommand.Parameters.AddWithValue("@id", id);
            await dziennikCommand.ExecuteNonQueryAsync();

            await transaction.CommitAsync();

            return Ok(ApiResponse<object>.SuccessResponse(null, "Zwrot przekazany do handlowca"));
        }
        catch
        {
            await transaction.RollbackAsync();
            throw;
        }
    }

    [HttpPatch("{id:int}/decision")]
    public async Task<ActionResult<ApiResponse<ReturnDecisionResponse>>> SaveDecision(int id, [FromBody] ReturnDecisionRequest request)
    {
        var userId = GetUserId();
        if (userId <= 0)
        {
            return Unauthorized(ApiResponse<ReturnDecisionResponse>.ErrorResponse("Nieprawidłowy użytkownik"));
        }

        var displayName = GetDisplayName();

        await using var connection = DbConnectionFactory.CreateMagazynConnection(_configuration);
        await connection.OpenAsync();
        await using var transaction = await connection.BeginTransactionAsync();

        try
        {
            var statusId = await GetStatusIdAsync(connection, transaction, "Zakończony", null);
            if (statusId == null)
            {
                return BadRequest(ApiResponse<ReturnDecisionResponse>.ErrorResponse("Brak statusu 'Zakończony'."));
            }

            var update = new MySqlCommand(@"
                UPDATE AllegroCustomerReturns
                SET DecyzjaHandlowcaId = @decyzjaId,
                    KomentarzHandlowca = @komentarz,
                    StatusWewnetrznyId = @statusId,
                    DataDecyzji = @dataDecyzji
                WHERE Id = @id;", connection, transaction);
            update.Parameters.AddWithValue("@decyzjaId", request.DecyzjaId);
            update.Parameters.AddWithValue("@komentarz", request.Komentarz ?? string.Empty);
            update.Parameters.AddWithValue("@statusId", statusId.Value);
            update.Parameters.AddWithValue("@dataDecyzji", DateTime.Now);
            update.Parameters.AddWithValue("@id", id);
            await update.ExecuteNonQueryAsync();

            var decisionName = await GetStatusNameByIdAsync(connection, transaction, request.DecyzjaId) ?? request.DecyzjaId.ToString();
            var actionText = $"Podjęto decyzję: {decisionName}. Komentarz: {request.Komentarz}";
            var action = new MySqlCommand(@"
                INSERT INTO ZwrotDzialania (ZwrotId, Data, Uzytkownik, Tresc)
                VALUES (@zwrotId, @data, @uzytkownik, @tresc);", connection, transaction);
            action.Parameters.AddWithValue("@zwrotId", id);
            action.Parameters.AddWithValue("@data", DateTime.Now);
            action.Parameters.AddWithValue("@uzytkownik", displayName);
            action.Parameters.AddWithValue("@tresc", actionText);
            await action.ExecuteNonQueryAsync();

            await MarkAndReplyMessageAsync(connection, transaction, id, userId, decisionName, request.Komentarz);

            await transaction.CommitAsync();

            var response = new ReturnDecisionResponse
            {
                ReturnId = id,
                StatusWewnetrzny = "Zakończony",
                DecyzjaHandlowca = decisionName,
                DataDecyzji = DateTime.Now
            };

            return Ok(ApiResponse<ReturnDecisionResponse>.SuccessResponse(response, "Decyzja zapisana"));
        }
        catch
        {
            await transaction.RollbackAsync();
            throw;
        }
    }

    [HttpPost("manual")]
    public async Task<ActionResult<ApiResponse<object>>> CreateManualReturn([FromBody] ReturnManualCreateRequest request)
    {
        if (request.WybraniHandlowcy.Count == 0)
        {
            return BadRequest(ApiResponse<object>.ErrorResponse("Wybierz co najmniej jednego handlowca"));
        }

        var userId = GetUserId();
        var displayName = GetDisplayName();
        if (userId <= 0)
        {
            return Unauthorized(ApiResponse<object>.ErrorResponse("Nieprawidłowy użytkownik"));
        }

        await using var connection = DbConnectionFactory.CreateMagazynConnection(_configuration);
        await connection.OpenAsync();
        await using var transaction = await connection.BeginTransactionAsync();

        try
        {
            var statusId = await GetStatusIdAsync(connection, transaction, "Oczekuje na decyzję handlowca", "StatusWewnetrzny");
            if (statusId == null)
            {
                return BadRequest(ApiResponse<object>.ErrorResponse("Brak statusu 'Oczekuje na decyzję handlowca'."));
            }

            var uwagiColumn = await ResolveUwagiMagazynuColumnAsync(connection);
            var referenceNumber = await GenerateReferenceNumberAsync(connection, transaction);

            var insert = $@"
                INSERT INTO AllegroCustomerReturns (
                    ReferenceNumber, Waybill, CreatedAt, DataPrzyjecia, PrzyjetyPrzezId,
                    StatusAllegro, StatusWewnetrznyId, StanProduktuId, IsManual,
                    ManualSenderDetails, CarrierName, ProductName,
                    Delivery_FirstName, Delivery_LastName, Delivery_Street, Delivery_ZipCode, Delivery_City, Delivery_PhoneNumber,
                    {uwagiColumn}, BuyerFullName
                ) VALUES (
                    @ReferenceNumber, @Waybill, @CreatedAt, @DataPrzyjecia, @PrzyjetyPrzezId,
                    @StatusAllegro, @StatusWewnetrznyId, @StanProduktuId, @IsManual,
                    @ManualSenderDetails, @CarrierName, @ProductName,
                    @Delivery_FirstName, @Delivery_LastName, @Delivery_Street, @Delivery_ZipCode, @Delivery_City, @Delivery_PhoneNumber,
                    @Uwagi, @BuyerFullName
                );
                SELECT LAST_INSERT_ID();";

            var nameParts = request.BuyerFullName.Split(' ', 2, StringSplitOptions.RemoveEmptyEntries);
            var manualDetails = new
            {
                FullName = request.BuyerFullName,
                Street = request.BuyerStreet,
                ZipCode = request.BuyerZipCode,
                City = request.BuyerCity,
                Phone = request.BuyerPhone
            };

            var insertCommand = new MySqlCommand(insert, connection, transaction);
            insertCommand.Parameters.AddWithValue("@ReferenceNumber", referenceNumber);
            insertCommand.Parameters.AddWithValue("@Waybill", request.NumerListu);
            insertCommand.Parameters.AddWithValue("@CreatedAt", DateTime.Now);
            insertCommand.Parameters.AddWithValue("@DataPrzyjecia", DateTime.Now);
            insertCommand.Parameters.AddWithValue("@PrzyjetyPrzezId", userId);
            insertCommand.Parameters.AddWithValue("@StatusAllegro", "MANUAL");
            insertCommand.Parameters.AddWithValue("@StatusWewnetrznyId", statusId.Value);
            insertCommand.Parameters.AddWithValue("@StanProduktuId", request.StanProduktuId);
            insertCommand.Parameters.AddWithValue("@IsManual", 1);
            insertCommand.Parameters.AddWithValue("@ManualSenderDetails", System.Text.Json.JsonSerializer.Serialize(manualDetails));
            insertCommand.Parameters.AddWithValue("@CarrierName", request.Przewoznik ?? string.Empty);
            insertCommand.Parameters.AddWithValue("@ProductName", request.Produkt ?? string.Empty);
            insertCommand.Parameters.AddWithValue("@Delivery_FirstName", nameParts.Length > 0 ? nameParts[0] : string.Empty);
            insertCommand.Parameters.AddWithValue("@Delivery_LastName", nameParts.Length > 1 ? nameParts[1] : string.Empty);
            insertCommand.Parameters.AddWithValue("@Delivery_Street", request.BuyerStreet ?? string.Empty);
            insertCommand.Parameters.AddWithValue("@Delivery_ZipCode", request.BuyerZipCode ?? string.Empty);
            insertCommand.Parameters.AddWithValue("@Delivery_City", request.BuyerCity ?? string.Empty);
            insertCommand.Parameters.AddWithValue("@Delivery_PhoneNumber", request.BuyerPhone ?? string.Empty);
            insertCommand.Parameters.AddWithValue("@Uwagi", request.UwagiMagazynu ?? string.Empty);
            insertCommand.Parameters.AddWithValue("@BuyerFullName", request.BuyerFullName);

            var newReturnId = Convert.ToInt32(await insertCommand.ExecuteScalarAsync());

            foreach (var handlowiec in request.WybraniHandlowcy)
            {
                var messageCommand = new MySqlCommand(@"
                    INSERT INTO Wiadomosci (NadawcaId, OdbiorcaId, Tresc, DataWyslania, DotyczyZwrotuId, CzyPrzeczytana)
                    VALUES (@nadawca, @odbiorca, @tresc, @data, @zwrotId, 0);", connection, transaction);
                messageCommand.Parameters.AddWithValue("@nadawca", userId);
                messageCommand.Parameters.AddWithValue("@odbiorca", handlowiec);
                messageCommand.Parameters.AddWithValue("@tresc", $"Nowy zwrot ręczny ({referenceNumber}) oczekuje na Twoją decyzję.");
                messageCommand.Parameters.AddWithValue("@data", DateTime.Now);
                messageCommand.Parameters.AddWithValue("@zwrotId", newReturnId);
                await messageCommand.ExecuteNonQueryAsync();
            }

            var action = new MySqlCommand(@"
                INSERT INTO ZwrotDzialania (ZwrotId, Data, Uzytkownik, Tresc)
                VALUES (@zwrotId, @data, @uzytkownik, @tresc);", connection, transaction);
            action.Parameters.AddWithValue("@zwrotId", newReturnId);
            action.Parameters.AddWithValue("@data", DateTime.Now);
            action.Parameters.AddWithValue("@uzytkownik", displayName);
            action.Parameters.AddWithValue("@tresc", "Zwrot przekazany do decyzji handlowców.");
            await action.ExecuteNonQueryAsync();

            var dziennik = new MySqlCommand(@"
                INSERT INTO MagazynDziennik (Data, Uzytkownik, Akcja, DotyczyZwrotuId)
                VALUES (@data, @uzytkownik, @akcja, @id);", connection, transaction);
            dziennik.Parameters.AddWithValue("@data", DateTime.Now);
            dziennik.Parameters.AddWithValue("@uzytkownik", displayName);
            dziennik.Parameters.AddWithValue("@akcja", $"Dodano zwrot ręczny: {referenceNumber}.");
            dziennik.Parameters.AddWithValue("@id", newReturnId);
            await dziennik.ExecuteNonQueryAsync();

            await transaction.CommitAsync();

            return CreatedAtAction(nameof(GetReturn), new { id = newReturnId },
                ApiResponse<object>.SuccessResponse(null, "Zwrot ręczny dodany"));
        }
        catch
        {
            await transaction.RollbackAsync();
            throw;
        }
    }

    [HttpPatch("{id:int}/archive")]
    public async Task<ActionResult<ApiResponse<object>>> ArchiveReturn(int id)
    {
        await using var connection = DbConnectionFactory.CreateMagazynConnection(_configuration);
        await connection.OpenAsync();

        var statusId = await GetStatusIdAsync(connection, null, "Archiwalny", null);
        if (statusId == null)
        {
            return BadRequest(ApiResponse<object>.ErrorResponse("Brak statusu 'Archiwalny'."));
        }

        var command = new MySqlCommand(@"
            UPDATE AllegroCustomerReturns
            SET StatusWewnetrznyId = @statusId
            WHERE Id = @id;", connection);
        command.Parameters.AddWithValue("@statusId", statusId.Value);
        command.Parameters.AddWithValue("@id", id);

        var updated = await command.ExecuteNonQueryAsync();
        if (updated == 0)
        {
            return NotFound(ApiResponse<object>.ErrorResponse("Zwrot nie znaleziony"));
        }

        return Ok(ApiResponse<object>.SuccessResponse(null, "Zwrot zarchiwizowany"));
    }

    [HttpGet("{id:int}/actions")]
    public async Task<ActionResult<ApiResponse<List<ReturnActionDto>>>> GetActions(int id)
    {
        await using var connection = DbConnectionFactory.CreateMagazynConnection(_configuration);
        await connection.OpenAsync();

        var command = new MySqlCommand(@"
            SELECT Id, ZwrotId, Data, Uzytkownik, Tresc
            FROM ZwrotDzialania
            WHERE ZwrotId = @id
            ORDER BY Data DESC;", connection);
        command.Parameters.AddWithValue("@id", id);

        var actions = new List<ReturnActionDto>();
        await using var reader = await command.ExecuteReaderAsync();
        while (await reader.ReadAsync())
        {
            actions.Add(new ReturnActionDto
            {
                Id = reader.GetInt32("Id"),
                ReturnId = reader.GetInt32("ZwrotId"),
                Data = reader.GetDateTime("Data"),
                Uzytkownik = reader.GetString("Uzytkownik"),
                Tresc = reader.GetString("Tresc")
            });
        }

        return Ok(ApiResponse<List<ReturnActionDto>>.SuccessResponse(actions));
    }

    [HttpPost("{id:int}/actions")]
    public async Task<ActionResult<ApiResponse<ReturnActionDto>>> AddAction(int id, [FromBody] ReturnActionCreateRequest request)
    {
        if (string.IsNullOrWhiteSpace(request.Tresc))
        {
            return BadRequest(ApiResponse<ReturnActionDto>.ErrorResponse("Treść działania nie może być pusta"));
        }

        var displayName = GetDisplayName();

        await using var connection = DbConnectionFactory.CreateMagazynConnection(_configuration);
        await connection.OpenAsync();

        var command = new MySqlCommand(@"
            INSERT INTO ZwrotDzialania (ZwrotId, Data, Uzytkownik, Tresc)
            VALUES (@zwrotId, @data, @uzytkownik, @tresc);
            SELECT LAST_INSERT_ID();", connection);
        command.Parameters.AddWithValue("@zwrotId", id);
        command.Parameters.AddWithValue("@data", DateTime.Now);
        command.Parameters.AddWithValue("@uzytkownik", displayName);
        command.Parameters.AddWithValue("@tresc", request.Tresc.Trim());

        var newId = Convert.ToInt32(await command.ExecuteScalarAsync());
        var dto = new ReturnActionDto
        {
            Id = newId,
            ReturnId = id,
            Data = DateTime.Now,
            Uzytkownik = displayName,
            Tresc = request.Tresc.Trim()
        };

        return CreatedAtAction(nameof(GetActions), new { id }, ApiResponse<ReturnActionDto>.SuccessResponse(dto));
    }

    [HttpGet("summary")]
    public async Task<ActionResult<ApiResponse<ReturnSummaryResponse>>> GetSummary(
        [FromQuery] int? handlowiecId = null,
        [FromQuery] string? status = null,
        [FromQuery] DateTime? dateFrom = null,
        [FromQuery] DateTime? dateTo = null)
    {
        var conditions = new List<string>();
        var parameters = new List<MySqlParameter>();

        if (handlowiecId.HasValue)
        {
            conditions.Add("acr.HandlowiecOpiekunId = @handlowiecId");
            parameters.Add(new MySqlParameter("@handlowiecId", handlowiecId.Value));
        }

        if (!string.IsNullOrWhiteSpace(status))
        {
            conditions.Add("s2.Nazwa = @status");
            parameters.Add(new MySqlParameter("@status", status));
        }

        if (dateFrom.HasValue)
        {
            conditions.Add("DATE(acr.CreatedAt) >= DATE(@dateFrom)");
            parameters.Add(new MySqlParameter("@dateFrom", dateFrom.Value.Date));
        }

        if (dateTo.HasValue)
        {
            conditions.Add("DATE(acr.CreatedAt) <= DATE(@dateTo)");
            parameters.Add(new MySqlParameter("@dateTo", dateTo.Value.Date));
        }

        var whereClause = conditions.Count > 0 ? "WHERE " + string.Join(" AND ", conditions) : string.Empty;

        await using var connection = DbConnectionFactory.CreateMagazynConnection(_configuration);
        await connection.OpenAsync();

        var uwagiColumn = await ResolveUwagiMagazynuColumnAsync(connection);

        var listQuery = $@"
            SELECT
                acr.Id,
                acr.ReferenceNumber,
                acr.ProductName,
                u.`Nazwa Wyświetlana` AS PrzyjalNazwa,
                acr.{uwagiColumn} AS UwagiMagazynu,
                acr.KomentarzHandlowca,
                s2.Nazwa AS StatusWew,
                s3.Nazwa AS DecyzjaHandl
            FROM AllegroCustomerReturns acr
            LEFT JOIN Statusy s2 ON s2.Id = acr.StatusWewnetrznyId
            LEFT JOIN Statusy s3 ON s3.Id = acr.DecyzjaHandlowcaId
            LEFT JOIN Uzytkownicy u ON u.Id = acr.PrzyjetyPrzezId
            {whereClause}
            ORDER BY acr.CreatedAt DESC;";

        var statsQuery = $@"
            SELECT
                COUNT(acr.Id) AS Total,
                SUM(CASE WHEN IFNULL(s2.Nazwa, '') = 'Oczekuje na decyzję handlowca' THEN 1 ELSE 0 END) AS DoDecyzji,
                SUM(CASE WHEN IFNULL(s2.Nazwa, '') = 'Zakończony' THEN 1 ELSE 0 END) AS Zakonczone
            FROM AllegroCustomerReturns acr
            LEFT JOIN Statusy s2 ON s2.Id = acr.StatusWewnetrznyId
            {whereClause};";

        var listCommand = new MySqlCommand(listQuery, connection);
        listCommand.Parameters.AddRange(parameters.ToArray());

        var items = new List<ReturnSummaryItemDto>();
        await using (var reader = await listCommand.ExecuteReaderAsync())
        {
            while (await reader.ReadAsync())
            {
                items.Add(new ReturnSummaryItemDto
                {
                    Id = reader.GetInt32("Id"),
                    NumerZwrotu = reader.GetString("ReferenceNumber"),
                    Produkt = reader.IsDBNull("ProductName") ? string.Empty : reader.GetString("ProductName"),
                    KtoPrzyjal = reader.IsDBNull("PrzyjalNazwa") ? "nie przyjęte" : reader.GetString("PrzyjalNazwa"),
                    KtoPodjalDecyzje = string.Empty,
                    JakaDecyzja = reader.IsDBNull("DecyzjaHandl") ? "" : reader.GetString("DecyzjaHandl"),
                    UwagiMagazynu = reader.IsDBNull("UwagiMagazynu") ? string.Empty : reader.GetString("UwagiMagazynu"),
                    UwagiHandlowca = reader.IsDBNull("KomentarzHandlowca") ? string.Empty : reader.GetString("KomentarzHandlowca"),
                    Status = reader.IsDBNull("StatusWew") ? "" : reader.GetString("StatusWew")
                });
            }
        }

        var statsCommand = new MySqlCommand(statsQuery, connection);
        statsCommand.Parameters.AddRange(parameters.ToArray());
        await using var statsReader = await statsCommand.ExecuteReaderAsync();
        var stats = new ReturnSummaryStatsDto();
        if (await statsReader.ReadAsync())
        {
            stats.Total = statsReader.IsDBNull("Total") ? 0 : Convert.ToInt32(statsReader["Total"]);
            stats.DoDecyzji = statsReader.IsDBNull("DoDecyzji") ? 0 : Convert.ToInt32(statsReader["DoDecyzji"]);
            stats.Zakonczone = statsReader.IsDBNull("Zakonczone") ? 0 : Convert.ToInt32(statsReader["Zakonczone"]);
        }

        var response = new ReturnSummaryResponse
        {
            Items = items,
            Stats = stats
        };

        return Ok(ApiResponse<ReturnSummaryResponse>.SuccessResponse(response));
    }

    [HttpGet("summary/export")]
    public async Task<ActionResult> ExportSummary(
        [FromQuery] int? handlowiecId = null,
        [FromQuery] string? status = null,
        [FromQuery] DateTime? dateFrom = null,
        [FromQuery] DateTime? dateTo = null)
    {
        var response = await GetSummary(handlowiecId, status, dateFrom, dateTo);
        if (response.Result is ObjectResult objectResult && objectResult.StatusCode != 200)
        {
            return objectResult;
        }

        var payload = (response.Result as OkObjectResult)?.Value as ApiResponse<ReturnSummaryResponse>;
        var data = payload?.Data;
        if (data == null)
        {
            return BadRequest(ApiResponse<object>.ErrorResponse("Brak danych do eksportu"));
        }

        var lines = new List<string>
        {
            "Numer Zwrotu;Produkt;Kto przyjął;Kto podjął decyzję;Jaka decyzja;Uwagi Magazynu;Uwagi Handlowca;Status"
        };
        foreach (var item in data.Items)
        {
            lines.Add(string.Join(";", new[]
            {
                item.NumerZwrotu,
                item.Produkt,
                item.KtoPrzyjal,
                item.KtoPodjalDecyzje,
                item.JakaDecyzja,
                item.UwagiMagazynu,
                item.UwagiHandlowca,
                item.Status
            }.Select(v => (v ?? string.Empty).Replace(';', ','))));
        }

        var csv = string.Join(Environment.NewLine, lines);
        return File(System.Text.Encoding.UTF8.GetBytes(csv), "text/csv", "zwroty_podsumowanie.csv");
    }

    [HttpPost("{id:int}/forward-to-complaints")]
    public async Task<ActionResult<ApiResponse<object>>> ForwardToComplaints(int id, [FromBody] ForwardToComplaintRequest request)
    {
        if (id != request.ReturnId)
        {
            return BadRequest(ApiResponse<object>.ErrorResponse("Nieprawidłowe ID zwrotu"));
        }

        await using var connection = DbConnectionFactory.CreateMagazynConnection(_configuration);
        await connection.OpenAsync();

        var command = new MySqlCommand(@"
            INSERT INTO NiezarejestrowaneZwrotyReklamacyjne
            (
                DataPrzekazania, PrzekazanePrzez, IdZwrotuWMagazynie,
                DaneKlienta, DaneProduktu, NumerFaktury, NumerSeryjny,
                UwagiMagazynu, KomentarzHandlowca,
                ImieKlienta, NazwiskoKlienta, EmailKlienta, TelefonKlienta,
                AdresUlica, AdresKodPocztowy, AdresMiasto,
                NazwaProduktu, NIP, DataZakupu, OpisUsterki
            )
            VALUES
            (
                @data, @kto, @idZw,
                @daneKlienta, @daneProduktu, @fv, @sn,
                @uwagiMag, @komHandl,
                @imie, @nazw, @email, @tel,
                @ulica, @kod, @miasto,
                @nazwaProd, @nip, @dataZakupu, @opis
            );", connection);

        var daneKlienta = $"{request.DaneKlienta.Imie} {request.DaneKlienta.Nazwisko} | {request.DaneKlienta.Adres.Ulica}, {request.DaneKlienta.Adres.Kod} {request.DaneKlienta.Adres.Miasto} | tel: {request.DaneKlienta.Telefon}";
        var opisUsterki = string.Join(Environment.NewLine, new[]
        {
            request.PowodKlienta,
            string.IsNullOrWhiteSpace(request.UwagiMagazynu) ? null : $"Uwagi magazynu: {request.UwagiMagazynu}",
            string.IsNullOrWhiteSpace(request.UwagiHandlowca) ? null : $"Uwagi handlowca: {request.UwagiHandlowca}",
            $"Przekazał: {request.Przekazal}"
        }.Where(line => !string.IsNullOrWhiteSpace(line)));

        command.Parameters.AddWithValue("@data", DateTime.Now);
        command.Parameters.AddWithValue("@kto", request.Przekazal);
        command.Parameters.AddWithValue("@idZw", request.ReturnId);
        command.Parameters.AddWithValue("@daneKlienta", daneKlienta);
        command.Parameters.AddWithValue("@daneProduktu", request.Produkt.Nazwa);
        command.Parameters.AddWithValue("@fv", request.Produkt.NrFaktury ?? string.Empty);
        command.Parameters.AddWithValue("@sn", request.Produkt.NrSeryjny ?? string.Empty);
        command.Parameters.AddWithValue("@uwagiMag", request.UwagiMagazynu ?? string.Empty);
        command.Parameters.AddWithValue("@komHandl", request.UwagiHandlowca ?? string.Empty);
        command.Parameters.AddWithValue("@imie", request.DaneKlienta.Imie);
        command.Parameters.AddWithValue("@nazw", request.DaneKlienta.Nazwisko);
        command.Parameters.AddWithValue("@email", request.DaneKlienta.Email ?? string.Empty);
        command.Parameters.AddWithValue("@tel", request.DaneKlienta.Telefon ?? string.Empty);
        command.Parameters.AddWithValue("@ulica", request.DaneKlienta.Adres.Ulica ?? string.Empty);
        command.Parameters.AddWithValue("@kod", request.DaneKlienta.Adres.Kod ?? string.Empty);
        command.Parameters.AddWithValue("@miasto", request.DaneKlienta.Adres.Miasto ?? string.Empty);
        command.Parameters.AddWithValue("@nazwaProd", request.Produkt.Nazwa);
        command.Parameters.AddWithValue("@nip", string.Empty);
        command.Parameters.AddWithValue("@dataZakupu", DBNull.Value);
        command.Parameters.AddWithValue("@opis", opisUsterki);

        await command.ExecuteNonQueryAsync();

        return Ok(ApiResponse<object>.SuccessResponse(null, "Przekazano do reklamacji"));
    }

    private int GetUserId()
    {
        var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        return int.TryParse(userIdClaim, out var id) ? id : 0;
    }

    private string GetDisplayName()
    {
        return User.FindFirst("DisplayName")?.Value
            ?? User.Identity?.Name
            ?? "System";
    }

    private static string BuildBuyerName(MySqlDataReader reader)
    {
        if (!reader.IsDBNull("BuyerFullName"))
        {
            return reader.GetString("BuyerFullName");
        }

        var first = reader.IsDBNull("Delivery_FirstName") ? string.Empty : reader.GetString("Delivery_FirstName");
        var last = reader.IsDBNull("Delivery_LastName") ? string.Empty : reader.GetString("Delivery_LastName");
        var full = (first + " " + last).Trim();

        if (!string.IsNullOrWhiteSpace(full))
        {
            return full;
        }

        var buyerFirst = reader.IsDBNull("Buyer_FirstName") ? string.Empty : reader.GetString("Buyer_FirstName");
        var buyerLast = reader.IsDBNull("Buyer_LastName") ? string.Empty : reader.GetString("Buyer_LastName");
        full = (buyerFirst + " " + buyerLast).Trim();

        if (!string.IsNullOrWhiteSpace(full))
        {
            return full;
        }

        return reader.IsDBNull("BuyerLogin") ? "Brak danych" : reader.GetString("BuyerLogin");
    }

    private static string? BuildPhone(MySqlDataReader reader)
    {
        if (!reader.IsDBNull("Delivery_PhoneNumber"))
        {
            return reader.GetString("Delivery_PhoneNumber");
        }

        return reader.IsDBNull("Buyer_PhoneNumber") ? null : reader.GetString("Buyer_PhoneNumber");
    }

    private static string? BuildAddress(MySqlDataReader reader)
    {
        if (!reader.IsDBNull("Delivery_Street"))
        {
            var zip = reader.IsDBNull("Delivery_ZipCode") ? string.Empty : reader.GetString("Delivery_ZipCode");
            var city = reader.IsDBNull("Delivery_City") ? string.Empty : reader.GetString("Delivery_City");
            return $"{reader.GetString("Delivery_Street")}, {zip} {city}".Trim();
        }

        if (!reader.IsDBNull("Buyer_Street"))
        {
            var zip = reader.IsDBNull("Buyer_ZipCode") ? string.Empty : reader.GetString("Buyer_ZipCode");
            var city = reader.IsDBNull("Buyer_City") ? string.Empty : reader.GetString("Buyer_City");
            return $"{reader.GetString("Buyer_Street")}, {zip} {city}".Trim();
        }

        return null;
    }

    private static async Task<string> ResolveUwagiMagazynuColumnAsync(MySqlConnection connection)
    {
        var command = new MySqlCommand(@"
            SELECT COLUMN_NAME
            FROM INFORMATION_SCHEMA.COLUMNS
            WHERE TABLE_SCHEMA = DATABASE()
              AND TABLE_NAME = 'AllegroCustomerReturns'
              AND COLUMN_NAME IN ('UwagiMagazynu', 'UwagiMagazyn')
            LIMIT 1;", connection);
        var result = await command.ExecuteScalarAsync();
        return result?.ToString() ?? "UwagiMagazynu";
    }

    private static async Task<int?> GetStatusIdAsync(MySqlConnection connection, MySqlTransaction? transaction, string name, string? type)
    {
        var query = "SELECT Id FROM Statusy WHERE Nazwa = @n" + (string.IsNullOrWhiteSpace(type) ? "" : " AND TypStatusu = @t") + " LIMIT 1;";
        var command = new MySqlCommand(query, connection, transaction);
        command.Parameters.AddWithValue("@n", name);
        if (!string.IsNullOrWhiteSpace(type))
        {
            command.Parameters.AddWithValue("@t", type);
        }

        var result = await command.ExecuteScalarAsync();
        return result == null || result == DBNull.Value ? null : Convert.ToInt32(result);
    }

    private static async Task<string?> GetStatusNameByIdAsync(MySqlConnection connection, MySqlTransaction transaction, int id)
    {
        var command = new MySqlCommand("SELECT Nazwa FROM Statusy WHERE Id = @id LIMIT 1;", connection, transaction);
        command.Parameters.AddWithValue("@id", id);
        var result = await command.ExecuteScalarAsync();
        return result == null || result == DBNull.Value ? null : result.ToString();
    }

    private static async Task<int?> ResolveDelegateAsync(MySqlConnection connection, MySqlTransaction transaction, int userId)
    {
        var command = new MySqlCommand(@"
            SELECT ZastepcaId
            FROM Delegacje
            WHERE UzytkownikId = @u
              AND CURDATE() BETWEEN DataOd AND DataDo
              AND IFNULL(CzyAktywna,1)=1
            LIMIT 1;", connection, transaction);
        command.Parameters.AddWithValue("@u", userId);

        var result = await command.ExecuteScalarAsync();
        if (result == null || result == DBNull.Value)
        {
            return null;
        }

        return Convert.ToInt32(result);
    }

    private static async Task<string> GenerateReferenceNumberAsync(MySqlConnection connection, MySqlTransaction transaction)
    {
        var monthYear = DateTime.Now.ToString("MM/yy");
        var pattern = $"R/%/{monthYear}";

        var command = new MySqlCommand(@"
            SELECT ReferenceNumber
            FROM AllegroCustomerReturns
            WHERE ReferenceNumber LIKE @pattern
            ORDER BY ReferenceNumber DESC
            LIMIT 1;", connection, transaction);
        command.Parameters.AddWithValue("@pattern", pattern);

        var lastNumber = (await command.ExecuteScalarAsync())?.ToString();
        var nextId = 1;
        if (!string.IsNullOrWhiteSpace(lastNumber))
        {
            var parts = lastNumber.Split('/');
            if (parts.Length >= 2 && int.TryParse(parts[1], out var parsed))
            {
                nextId = parsed + 1;
            }
        }

        return $"R/{nextId:D3}/{monthYear}";
    }

    private static async Task MarkAndReplyMessageAsync(
        MySqlConnection connection,
        MySqlTransaction transaction,
        int returnId,
        int recipientId,
        string decyzjaName,
        string? komentarz)
    {
        var findCommand = new MySqlCommand(@"
            SELECT Id, NadawcaId
            FROM Wiadomosci
            WHERE DotyczyZwrotuId = @zwrotId AND OdbiorcaId = @odb
            ORDER BY Id DESC
            LIMIT 1;", connection, transaction);
        findCommand.Parameters.AddWithValue("@zwrotId", returnId);
        findCommand.Parameters.AddWithValue("@odb", recipientId);

        int? messageId = null;
        int? senderId = null;

        await using (var reader = await findCommand.ExecuteReaderAsync())
        {
            if (await reader.ReadAsync())
            {
                messageId = reader.GetInt32("Id");
                senderId = reader.GetInt32("NadawcaId");
            }
        }

        if (!messageId.HasValue || !senderId.HasValue)
        {
            return;
        }

        var update = new MySqlCommand(@"
            UPDATE Wiadomosci
            SET CzyOdpowiedziano = 1
            WHERE Id = @id;", connection, transaction);
        update.Parameters.AddWithValue("@id", messageId.Value);
        await update.ExecuteNonQueryAsync();

        var reply = new MySqlCommand(@"
            INSERT INTO Wiadomosci (NadawcaId, OdbiorcaId, Tytul, Tresc, DataWyslania, DotyczyZwrotuId, ParentMessageId)
            VALUES (@nad, @odb, @tytul, @tresc, @data, @zwrotId, @parentId);", connection, transaction);
        reply.Parameters.AddWithValue("@nad", recipientId);
        reply.Parameters.AddWithValue("@odb", senderId.Value);
        reply.Parameters.AddWithValue("@tytul", $"Re: Prośba o decyzję dla zwrotu nr {returnId}");
        reply.Parameters.AddWithValue("@tresc", $"Podjęto decyzję: '{decyzjaName}'. Komentarz: {komentarz}");
        reply.Parameters.AddWithValue("@data", DateTime.Now);
        reply.Parameters.AddWithValue("@zwrotId", returnId);
        reply.Parameters.AddWithValue("@parentId", messageId.Value);
        await reply.ExecuteNonQueryAsync();
    }
}
