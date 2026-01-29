using System.Data.Common;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using MySqlConnector;
using ReklamacjeAPI.Data;
using ReklamacjeAPI.DTOs;
using ReklamacjeAPI.Models;

namespace ReklamacjeAPI.Services;

public class ReturnsService
{
    private const string DecisionStatusName = "Po decyzji";
    private const string CompletedStatusName = "Zakończony";
    private readonly IConfiguration _configuration;
    private readonly ApplicationDbContext _context;
    private readonly AllegroApiClient _allegroApiClient;
    private readonly ReturnSyncProgressService _progressService;
    private readonly NotificationsService _notificationsService;
    private readonly ILogger<ReturnsService> _logger;
    private string? _uwagiMagazynuColumn;
    private static readonly JsonSerializerOptions JsonOptions = new(JsonSerializerDefaults.Web)
    {
        DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull
    };

    public ReturnsService(
        IConfiguration configuration,
        ApplicationDbContext context,
        AllegroApiClient allegroApiClient,
        ReturnSyncProgressService progressService,
        NotificationsService notificationsService,
        ILogger<ReturnsService> logger)
    {
        _configuration = configuration;
        _context = context;
        _allegroApiClient = allegroApiClient;
        _progressService = progressService;
        _notificationsService = notificationsService;
        _logger = logger;
    }

    public async Task<ReturnSyncResponse> SyncReturnsFromAllegroAsync(ReturnSyncRequest? request, string userDisplayName)
    {
        Console.WriteLine($"[SYNC START] Rozpoczynam synchronizację. Użytkownik: {userDisplayName}");
        return await SyncReturnsFromAllegroInternalAsync(request, userDisplayName, null);
    }

    public async Task<ReturnSyncResponse> SyncReturnsFromAllegroInternalAsync(
        ReturnSyncRequest? request,
        string userDisplayName,
        ReturnSyncProgress? progress)
    {
        var startedAt = DateTime.UtcNow;
        var daysBack = request?.DaysBack ?? 60;
        if (daysBack <= 0) daysBack = 60;

        var accounts = await GetAuthorizedAccountsAsync(request?.AccountId);
        if (progress != null)
        {
            progress.TotalAccounts = accounts.Count;
        }

        var response = new ReturnSyncResponse
        {
            AccountsProcessed = accounts.Count,
            StartedAt = startedAt
        };

        if (accounts.Count == 0)
        {
            response.Errors.Add("Brak autoryzowanych kont Allegro.");
            if (progress != null)
            {
                _progressService.Fail(progress, "Brak autoryzowanych kont Allegro.");
            }
            return response;
        }

        var dateFrom = DateTime.UtcNow.AddDays(-daysBack).ToString("yyyy-MM-dd'T'HH:mm:ss.fff'Z'");

        int? defaultStatusId;
        await using (var conn = DbConnectionFactory.CreateMagazynConnection(_configuration))
        {
            await conn.OpenAsync();
            defaultStatusId = await GetDefaultWarehouseStatusIdAsync(conn);
        }

        for (var accountIndex = 0; accountIndex < accounts.Count; accountIndex++)
        {
            var account = accounts[accountIndex];
            Console.WriteLine($"[SYNC ACCOUNT] Konto: {account.Name} (ID: {account.Id})");
            if (progress != null)
            {
                _progressService.UpdateAccount(progress, accountIndex + 1, accounts.Count, account.Name, account.Id);
            }
            try
            {
                int offset = 0;
                const int limit = 100;
                var processedInAccount = 0;
                int? totalInAccount = null;

                while (true)
                {
                    var filters = new Dictionary<string, string> { ["createdAt.gte"] = dateFrom };

                    var list = await _allegroApiClient.GetCustomerReturnsAsync(account.Id, limit, offset, filters);
                    var returns = list?.CustomerReturns ?? new List<AllegroApiClient.CustomerReturnDto>();
                    totalInAccount ??= list?.Count ?? returns.Count;

                    if (returns.Count == 0) break;

                    response.ReturnsFetched += returns.Count;

                    foreach (var ret in returns)
                    {
                        try
                        {
                            var returnLabel = string.IsNullOrWhiteSpace(ret.ReferenceNumber)
                                ? ret.Id
                                : ret.ReferenceNumber;
                            Console.WriteLine($"[SYNC RETURN] Konto: {account.Name} (ID: {account.Id}) Zwrot: {returnLabel}");
                            AllegroApiClient.OrderDetailsDto? orderDetails = null;
                            string? invoiceNumber = null;

                            if (!string.IsNullOrWhiteSpace(ret.OrderId))
                            {
                                try
                                {
                                    orderDetails = await _allegroApiClient.GetOrderDetailsAsync(account.Id, ret.OrderId);
                                    var invoices = await _allegroApiClient.GetInvoicesForOrderAsync(account.Id, ret.OrderId);
                                    invoiceNumber = invoices.FirstOrDefault()?.InvoiceNumber;
                                }
                                catch (Exception apiEx)
                                {
                                    Console.WriteLine($"[SYNC WARN] Brak szczegółów zamówienia {ret.OrderId}: {apiEx.Message}");
                                }
                            }

                            await ProcessSingleReturnSafeAsync(ret, orderDetails, invoiceNumber, account.Id, defaultStatusId);
                            response.ReturnsProcessed++;
                        }
                        catch (Exception itemEx)
                        {
                            if (progress != null)
                            {
                                _progressService.AddError(progress, $"Zwrot {ret.ReferenceNumber}: {itemEx.Message}");
                            }
                            Console.WriteLine($"[SYNC ITEM ERROR] {ret.ReferenceNumber}: {itemEx.Message}");
                        }
                    }

                    offset += limit;
                    if (list?.Count.HasValue == true && offset >= list.Count.Value) break;
                    if (returns.Count < limit) break;

                    await Task.Delay(50);
                }
            }
            catch (Exception ex)
            {
                response.Errors.Add($"Błąd konta {account.Name}: {ex.Message}");
                if (progress != null)
                {
                    _progressService.AddError(progress, $"Konto {account.Name}: {ex.Message}");
                }
                Console.WriteLine(ex.ToString());
            }
        }

        if (response.ReturnsProcessed > 0)
        {
            try
            {
                await using var logConnection = DbConnectionFactory.CreateMagazynConnection(_configuration);
                await logConnection.OpenAsync();
                await AddMagazynDziennikAsync(
                    logConnection,
                    null,
                    userDisplayName,
                    $"Zsynchronizowano zwroty (API Mobile). Pobrane: {response.ReturnsFetched}, Zapisane: {response.ReturnsProcessed}.",
                    null);
            }
            catch { }
        }

        response.FinishedAt = DateTime.UtcNow;
        if (progress != null)
        {
            _progressService.Complete(progress, new ReturnSyncSummary
            {
                AccountsProcessed = response.AccountsProcessed,
                ReturnsFetched = response.ReturnsFetched,
                ReturnsProcessed = response.ReturnsProcessed
            });
        }
        return response;
    }

    private async Task ProcessSingleReturnSafeAsync(
        AllegroApiClient.CustomerReturnDto ret,
        AllegroApiClient.OrderDetailsDto? orderDetails,
        string? invoiceNumber,
        int accountId,
        int? defaultStatusId)
    {
        await using var connection = DbConnectionFactory.CreateMagazynConnection(_configuration);
        await connection.OpenAsync();

        var checkQuery = "SELECT Id FROM AllegroCustomerReturns WHERE AllegroReturnId = @allegroId LIMIT 1";
        int? existingId = null;

        await using (var checkCmd = new MySqlCommand(checkQuery, connection))
        {
            checkCmd.Parameters.AddWithValue("@allegroId", ret.Id);
            var result = await checkCmd.ExecuteScalarAsync();
            if (result != null && result != DBNull.Value)
            {
                existingId = Convert.ToInt32(result);
            }
        }

        if (existingId == null && !string.IsNullOrEmpty(ret.ReferenceNumber))
        {
            var checkRefQuery = "SELECT Id FROM AllegroCustomerReturns WHERE ReferenceNumber = @ref LIMIT 1";
            await using (var checkRefCmd = new MySqlCommand(checkRefQuery, connection))
            {
                checkRefCmd.Parameters.AddWithValue("@ref", ret.ReferenceNumber);
                var result = await checkRefCmd.ExecuteScalarAsync();
                if (result != null && result != DBNull.Value)
                {
                    existingId = Convert.ToInt32(result);
                }
            }
        }

        var returnItem = ret.Items?.FirstOrDefault();
        var lineItem = orderDetails?.LineItems?.FirstOrDefault();
        var buyerFullName = BuildName(orderDetails?.Buyer?.FirstName, orderDetails?.Buyer?.LastName);

        // Wyciąganie adresów z DTO
        var deliveryAddress = orderDetails?.Delivery?.Address;
        var buyerAddress = orderDetails?.Buyer?.Address;
        var invoiceAddress = orderDetails?.Invoice?.Address; // Tutaj jest AddressDto z danymi firmy (Company)

        var productName = returnItem?.Name ?? lineItem?.Offer?.Name;
        var uwagiCol = await ResolveUwagiMagazynuColumnAsync(connection);

        if (existingId.HasValue)
        {
            var updateQuery = $@"
                UPDATE AllegroCustomerReturns SET
                    StatusAllegro = @StatusAllegro,
                    Waybill = @Waybill,
                    CarrierName = @CarrierName,
                    JsonDetails = @JsonDetails,
                    InvoiceNumber = @InvoiceNumber,
                    ProductName = @ProductName,
                    OfferId = @OfferId,
                    Quantity = @Quantity,
                    PaymentType = @PaymentType,
                    FulfillmentStatus = @FulfillmentStatus,
                    Delivery_FirstName = @Delivery_FirstName,
                    Delivery_LastName = @Delivery_LastName,
                    Delivery_Street = @Delivery_Street,
                    Delivery_ZipCode = @Delivery_ZipCode,
                    Delivery_City = @Delivery_City,
                    Delivery_PhoneNumber = @Delivery_PhoneNumber,
                    Buyer_FirstName = @Buyer_FirstName,
                    Buyer_LastName = @Buyer_LastName,
                    Buyer_Street = @Buyer_Street,
                    Buyer_ZipCode = @Buyer_ZipCode,
                    Buyer_City = @Buyer_City,
                    Buyer_PhoneNumber = @Buyer_PhoneNumber,
                    Invoice_CompanyName = @Invoice_CompanyName,
                    Invoice_TaxId = @Invoice_TaxId,
                    Invoice_Street = @Invoice_Street,
                    Invoice_ZipCode = @Invoice_ZipCode,
                    Invoice_City = @Invoice_City,
                    BuyerFullName = @BuyerFullName,
                    ReturnReasonType = @ReturnReasonType,
                    ReturnReasonComment = @ReturnReasonComment,
                    AllegroReturnId = @AllegroReturnId, 
                    AllegroAccountId = @AllegroAccountId
                WHERE Id = @Id
            ";

            await using var updateCmd = new MySqlCommand(updateQuery, connection);
            FillParameters(updateCmd, ret, orderDetails, invoiceNumber, accountId, defaultStatusId, productName,
                           returnItem, lineItem, buyerFullName, deliveryAddress, buyerAddress, invoiceAddress, uwagiCol);
            updateCmd.Parameters.AddWithValue("@Id", existingId.Value);
            await updateCmd.ExecuteNonQueryAsync();
        }
        else
        {
            var insertQuery = $@"
                INSERT INTO AllegroCustomerReturns (
                    AllegroReturnId, AllegroAccountId, ReferenceNumber, OrderId, BuyerLogin, CreatedAt, StatusAllegro,
                    Waybill, JsonDetails, StatusWewnetrznyId, CarrierName, InvoiceNumber, ProductName, OfferId,
                    Quantity, PaymentType, FulfillmentStatus, Delivery_FirstName, Delivery_LastName, Delivery_Street,
                    Delivery_ZipCode, Delivery_City, Delivery_PhoneNumber, Buyer_FirstName, Buyer_LastName, Buyer_Street,
                    Buyer_ZipCode, Buyer_City, Buyer_PhoneNumber, Invoice_CompanyName, Invoice_TaxId, Invoice_Street,
                    Invoice_ZipCode, Invoice_City, BuyerFullName, ReturnReasonType, ReturnReasonComment, {uwagiCol}
                ) VALUES (
                    @AllegroReturnId, @AllegroAccountId, @ReferenceNumber, @OrderId, @BuyerLogin, @CreatedAt, @StatusAllegro,
                    @Waybill, @JsonDetails, @StatusWewnetrznyId, @CarrierName, @InvoiceNumber, @ProductName, @OfferId,
                    @Quantity, @PaymentType, @FulfillmentStatus, @Delivery_FirstName, @Delivery_LastName, @Delivery_Street,
                    @Delivery_ZipCode, @Delivery_City, @Delivery_PhoneNumber, @Buyer_FirstName, @Buyer_LastName, @Buyer_Street,
                    @Buyer_ZipCode, @Buyer_City, @Buyer_PhoneNumber, @Invoice_CompanyName, @Invoice_TaxId, @Invoice_Street,
                    @Invoice_ZipCode, @Invoice_City, @BuyerFullName, @ReturnReasonType, @ReturnReasonComment, @Uwagi
                )
            ";

            await using var insertCmd = new MySqlCommand(insertQuery, connection);
            FillParameters(insertCmd, ret, orderDetails, invoiceNumber, accountId, defaultStatusId, productName,
                           returnItem, lineItem, buyerFullName, deliveryAddress, buyerAddress, invoiceAddress, uwagiCol);
            await insertCmd.ExecuteNonQueryAsync();
        }
    }

    private void FillParameters(MySqlCommand command,
        AllegroApiClient.CustomerReturnDto ret,
        AllegroApiClient.OrderDetailsDto? orderDetails,
        string? invoiceNumber,
        int accountId,
        int? defaultStatusId,
        string? productName,
        AllegroApiClient.CustomerReturnItemDto? returnItem,
        AllegroApiClient.OrderLineItemDto? lineItem,
        string? buyerFullName,
        AllegroApiClient.OrderDeliveryAddressDto? deliveryAddress,
        AllegroApiClient.OrderBuyerAddressDto? buyerAddress,
        AllegroApiClient.OrderInvoiceAddressDto? invoiceAddress, // Poprawiony typ
        string uwagiColumnName)
    {
        command.Parameters.AddWithValue("@AllegroReturnId", ret.Id ?? string.Empty);
        command.Parameters.AddWithValue("@AllegroAccountId", accountId);
        command.Parameters.AddWithValue("@ReferenceNumber", ret.ReferenceNumber ?? string.Empty);
        command.Parameters.AddWithValue("@OrderId", (object?)ret.OrderId ?? DBNull.Value);
        command.Parameters.AddWithValue("@BuyerLogin", (object?)ret.Buyer?.Login ?? DBNull.Value);
        command.Parameters.AddWithValue("@CreatedAt", (object?)ret.CreatedAt ?? DBNull.Value);
        command.Parameters.AddWithValue("@StatusAllegro", (object?)ret.Status ?? DBNull.Value);
        command.Parameters.AddWithValue("@Waybill", (object?)ret.Parcels?.FirstOrDefault()?.Waybill ?? DBNull.Value);
        command.Parameters.AddWithValue("@CarrierName", (object?)ret.Parcels?.FirstOrDefault()?.CarrierId ?? DBNull.Value);
        command.Parameters.AddWithValue("@JsonDetails", JsonSerializer.Serialize(ret, JsonOptions));
        command.Parameters.AddWithValue("@StatusWewnetrznyId", defaultStatusId ?? (object)DBNull.Value);
        command.Parameters.AddWithValue("@InvoiceNumber", (object?)invoiceNumber ?? DBNull.Value);
        command.Parameters.AddWithValue("@ProductName", (object?)productName ?? DBNull.Value);

        command.Parameters.AddWithValue("@OfferId", (object?)returnItem?.OfferId ?? DBNull.Value);

        object? quantity = null;
        if (returnItem != null) quantity = returnItem.Quantity;
        else if (lineItem != null) quantity = lineItem.Quantity;
        command.Parameters.AddWithValue("@Quantity", quantity ?? DBNull.Value);

        command.Parameters.AddWithValue("@PaymentType", (object?)orderDetails?.Payment?.Type ?? DBNull.Value);
        command.Parameters.AddWithValue("@FulfillmentStatus", (object?)orderDetails?.Fulfillment?.Status ?? DBNull.Value);

        command.Parameters.AddWithValue("@Delivery_FirstName", (object?)deliveryAddress?.FirstName ?? DBNull.Value);
        command.Parameters.AddWithValue("@Delivery_LastName", (object?)deliveryAddress?.LastName ?? DBNull.Value);
        command.Parameters.AddWithValue("@Delivery_Street", (object?)deliveryAddress?.Street ?? DBNull.Value);
        command.Parameters.AddWithValue("@Delivery_ZipCode", (object?)deliveryAddress?.ZipCode ?? DBNull.Value);
        command.Parameters.AddWithValue("@Delivery_City", (object?)deliveryAddress?.City ?? DBNull.Value);
        command.Parameters.AddWithValue("@Delivery_PhoneNumber", (object?)deliveryAddress?.PhoneNumber ?? DBNull.Value);

        command.Parameters.AddWithValue("@Buyer_FirstName", (object?)orderDetails?.Buyer?.FirstName ?? DBNull.Value);
        command.Parameters.AddWithValue("@Buyer_LastName", (object?)orderDetails?.Buyer?.LastName ?? DBNull.Value);
        command.Parameters.AddWithValue("@Buyer_Street", (object?)buyerAddress?.Street ?? DBNull.Value);
        command.Parameters.AddWithValue("@Buyer_ZipCode", (object?)buyerAddress?.PostCode ?? DBNull.Value);
        command.Parameters.AddWithValue("@Buyer_City", (object?)buyerAddress?.City ?? DBNull.Value);
        command.Parameters.AddWithValue("@Buyer_PhoneNumber", (object?)orderDetails?.Buyer?.PhoneNumber ?? DBNull.Value);

        // NAPRAWA BŁĘDU: Pobieramy dane firmy z obiektu invoiceAddress (OrderInvoiceAddressDto)
        command.Parameters.AddWithValue("@Invoice_CompanyName", (object?)invoiceAddress?.Company?.Name ?? DBNull.Value);
        command.Parameters.AddWithValue("@Invoice_TaxId", (object?)invoiceAddress?.Company?.TaxId ?? DBNull.Value);

        command.Parameters.AddWithValue("@Invoice_Street", (object?)invoiceAddress?.Street ?? DBNull.Value);
        command.Parameters.AddWithValue("@Invoice_ZipCode", (object?)invoiceAddress?.ZipCode ?? DBNull.Value);
        command.Parameters.AddWithValue("@Invoice_City", (object?)invoiceAddress?.City ?? DBNull.Value);

        command.Parameters.AddWithValue("@BuyerFullName", string.IsNullOrWhiteSpace(buyerFullName) ? DBNull.Value : buyerFullName);
        command.Parameters.AddWithValue("@ReturnReasonType", (object?)returnItem?.Reason?.Type ?? DBNull.Value);
        command.Parameters.AddWithValue("@ReturnReasonComment", (object?)returnItem?.Reason?.UserComment ?? DBNull.Value);

        if (command.CommandText.Contains("INSERT INTO"))
        {
            command.Parameters.AddWithValue("@Uwagi", DBNull.Value);
        }
    }

    public async Task<PaginatedResponse<ReturnListItemDto>> GetReturnsAsync(
        int page,
        int pageSize,
        string? statusWewnetrzny,
        string? excludeStatusWewnetrzny,
        string? statusAllegro,
        int? handlowiecId,
        string? search,
        bool sortByLastAction)
    {
        if (page <= 0)
        {
            page = 1;
        }

        var queryBuilder = new StringBuilder(@"
            SELECT
                acr.Id,
                acr.ReferenceNumber,
                acr.Waybill,
                acr.CreatedAt,
                COALESCE(dz.LastActionDate, acr.CreatedAt) AS LastActionDate,
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
            LEFT JOIN (
                SELECT ZwrotId, MAX(Data) AS LastActionDate
                FROM ZwrotDzialania
                GROUP BY ZwrotId
            ) dz ON dz.ZwrotId = acr.Id
        ");

        var conditions = new List<string>();
        var parameters = new List<MySqlParameter>();

        if (!string.IsNullOrWhiteSpace(statusWewnetrzny))
        {
            if (IsDecisionStatusFilter(statusWewnetrzny))
            {
                conditions.Add("s2.Nazwa IN (@statusPoDecyzji, @statusZakonczony)");
                parameters.Add(new MySqlParameter("@statusPoDecyzji", DecisionStatusName));
                parameters.Add(new MySqlParameter("@statusZakonczony", CompletedStatusName));
            }
            else
            {
                conditions.Add("s2.Nazwa = @statusWewnetrzny");
                parameters.Add(new MySqlParameter("@statusWewnetrzny", statusWewnetrzny));
            }
        }

        if (!string.IsNullOrWhiteSpace(excludeStatusWewnetrzny))
        {
            conditions.Add("(s2.Nazwa IS NULL OR s2.Nazwa <> @excludeStatusWewnetrzny)");
            parameters.Add(new MySqlParameter("@excludeStatusWewnetrzny", excludeStatusWewnetrzny));
        }

        if (!string.IsNullOrWhiteSpace(statusAllegro))
        {
            conditions.Add("acr.StatusAllegro = @statusAllegro");
            parameters.Add(new MySqlParameter("@statusAllegro", statusAllegro));
        }

        if (handlowiecId.HasValue)
        {
            conditions.Add(@"(
                acr.HandlowiecOpiekunId = @handlowiecId
                OR acr.Id IN (SELECT DotyczyZwrotuId FROM Wiadomosci WHERE OdbiorcaId = @handlowiecId)
            )");
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
        var orderBy = sortByLastAction
            ? " ORDER BY LastActionDate DESC"
            : " ORDER BY acr.CreatedAt DESC";
        queryBuilder.Append(orderBy);

        if (pageSize > 0)
        {
            queryBuilder.Append(" LIMIT @limit OFFSET @offset");
            parameters.Add(new MySqlParameter("@limit", pageSize));
            parameters.Add(new MySqlParameter("@offset", (page - 1) * pageSize));
        }

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

        if (pageSize <= 0)
        {
            response.PageSize = response.TotalItems > 0 ? response.TotalItems : 1;
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
                acr.{uwagiColumn} AS UwagiMagazynuResolved,
                COALESCE(acr.HandlowiecOpiekunId, aao.OpiekunId) AS AssignedSalesId,
                uo.`Nazwa Wyświetlana` AS AssignedSalesName
            FROM AllegroCustomerReturns acr
            LEFT JOIN Statusy s1 ON acr.StanProduktuId = s1.Id
            LEFT JOIN Statusy s2 ON acr.StatusWewnetrznyId = s2.Id
            LEFT JOIN Statusy s3 ON acr.DecyzjaHandlowcaId = s3.Id
            LEFT JOIN AllegroAccountOpiekun aao
                ON aao.AllegroAccountId = acr.AllegroAccountId
                AND (aao.CzyAktywny = 1 OR aao.CzyAktywny IS NULL)
            LEFT JOIN uzytkownicy uo
                ON uo.Id = COALESCE(acr.HandlowiecOpiekunId, aao.OpiekunId)
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

        var allegroAccountId = GetNullableInt(reader, "AllegroAccountId");
        var allegroAccountName = await GetAllegroAccountNameAsync(allegroAccountId);

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
            InvoiceNumber = GetOptionalString(reader, "InvoiceNumber"),
            AllegroAccountName = allegroAccountName,
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
            IsManual = GetOptionalBool(reader, "IsManual"),
            AllegroReturnId = reader["AllegroReturnId"] as string,
            OrderId = reader["OrderId"] as string,
            AssignedSalesId = GetNullableInt(reader, "AssignedSalesId"),
            AssignedSalesName = reader["AssignedSalesName"] as string
        };
    }

    public async Task<bool> UpdateWarehouseAsync(int id, ReturnWarehouseUpdateRequest request, string userDisplayName)
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

        var updated = await command.ExecuteNonQueryAsync() > 0;
        if (updated)
        {
            var stanName = await GetStatusNameByIdAsync(connection, request.StanProduktuId);
            var actionText = "Przyjęto zwrot do magazynu.";
            if (!string.IsNullOrWhiteSpace(stanName))
            {
                actionText += $" Stan produktu: {stanName}.";
            }

            if (!string.IsNullOrWhiteSpace(request.UwagiMagazynu))
            {
                actionText += $" Uwagi: {request.UwagiMagazynu}";
            }

            await AddReturnActionInternalAsync(connection, id, userDisplayName, actionText);
            await AddMagazynDziennikAsync(connection, id, userDisplayName, actionText, null);
        }

        return updated;
    }

    public async Task<ReturnDecisionResponse?> SaveDecisionAsync(int id, int userId, ReturnDecisionRequest request, string userDisplayName)
    {
        await using var connection = DbConnectionFactory.CreateMagazynConnection(_configuration);
        await connection.OpenAsync();

        var statusDecyzjiId = await GetStatusIdAsync(connection, DecisionStatusName, "StatusWewnetrzny");
        if (statusDecyzjiId == null)
        {
            statusDecyzjiId = await GetStatusIdAsync(connection, CompletedStatusName, "StatusWewnetrzny");
            if (statusDecyzjiId == null)
            {
                _logger.LogWarning("Brak statusu '{DecisionStatus}' ani '{CompletedStatus}' w Statusy. Zwrot={ReturnId}.",
                    DecisionStatusName,
                    CompletedStatusName,
                    id);
            }
        }

        var updateQuery = statusDecyzjiId.HasValue
            ? @"
                UPDATE AllegroCustomerReturns
                SET DecyzjaHandlowcaId = @decyzjaId,
                    KomentarzHandlowca = @komentarz,
                    DataDecyzji = @data,
                    StatusWewnetrznyId = @statusId
                WHERE Id = @id"
            : @"
                UPDATE AllegroCustomerReturns
                SET DecyzjaHandlowcaId = @decyzjaId,
                    KomentarzHandlowca = @komentarz,
                    DataDecyzji = @data
                WHERE Id = @id";

        var decisionTimestamp = DateTime.Now;
        await using (var updateCommand = new MySqlCommand(updateQuery, connection))
        {
            updateCommand.Parameters.AddWithValue("@decyzjaId", request.DecyzjaId);
            updateCommand.Parameters.AddWithValue("@komentarz", (object?)request.Komentarz ?? DBNull.Value);
            updateCommand.Parameters.AddWithValue("@data", decisionTimestamp);
            if (statusDecyzjiId.HasValue)
            {
                updateCommand.Parameters.AddWithValue("@statusId", statusDecyzjiId.Value);
            }
            updateCommand.Parameters.AddWithValue("@id", id);

            await updateCommand.ExecuteNonQueryAsync();
        }

        const string fetchQuery = @"
            SELECT acr.ReferenceNumber, s2.Nazwa AS StatusWewnetrzny, s3.Nazwa AS DecyzjaHandlowca
            FROM AllegroCustomerReturns acr
            LEFT JOIN Statusy s2 ON acr.StatusWewnetrznyId = s2.Id
            LEFT JOIN Statusy s3 ON acr.DecyzjaHandlowcaId = s3.Id
            WHERE acr.Id = @id";

        await using var fetchCommand = new MySqlCommand(fetchQuery, connection);
        fetchCommand.Parameters.AddWithValue("@id", id);
        string statusWewnetrzny;
        string decyzjaHandlowca;
        string referenceNumber;

        await using (var reader = await fetchCommand.ExecuteReaderAsync())
        {
            if (!await reader.ReadAsync())
            {
                return null;
            }

            referenceNumber = reader["ReferenceNumber"]?.ToString() ?? $"ZWROT-{id}";
            statusWewnetrzny = reader["StatusWewnetrzny"]?.ToString() ?? string.Empty;
            decyzjaHandlowca = reader["DecyzjaHandlowca"]?.ToString() ?? string.Empty;
        }

        await AddReturnActionInternalAsync(connection, id, userDisplayName,
            $"Podjęto decyzję: {decyzjaHandlowca}. Komentarz: {request.Komentarz}");

        if (userId > 0)
        {
            await _notificationsService.NotifyMagazynDecisionMadeAsync(id, referenceNumber, decyzjaHandlowca, userId);
        }

        return new ReturnDecisionResponse
        {
            ReturnId = id,
            StatusWewnetrzny = statusWewnetrzny,
            DecyzjaHandlowca = decyzjaHandlowca,
            DataDecyzji = decisionTimestamp
        };
    }

    public async Task<bool> ForwardToWarehouseAsync(int id, ReturnForwardToWarehouseRequest request, string userDisplayName)
    {
        await using var connection = DbConnectionFactory.CreateMagazynConnection(_configuration);
        await connection.OpenAsync();

        var statusId = await FindWarehouseStatusIdAsync(connection);
        if (statusId.HasValue)
        {
            await using var updateCommand = new MySqlCommand(@"
                UPDATE AllegroCustomerReturns
                SET StatusWewnetrznyId = @statusId
                WHERE Id = @id", connection);
            updateCommand.Parameters.AddWithValue("@statusId", statusId.Value);
            updateCommand.Parameters.AddWithValue("@id", id);
            await updateCommand.ExecuteNonQueryAsync();
        }

        var actionContent = string.IsNullOrWhiteSpace(request.Komentarz)
            ? "Przekazano do Magazynu."
            : $"Przekazano do Magazynu. Komentarz: {request.Komentarz}";

        await AddReturnActionInternalAsync(connection, id, userDisplayName, actionContent);

        return statusId.HasValue;
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

    public async Task<ReturnActionDto?> AddActionAsync(int returnId, int userId, string userDisplayName, ReturnActionCreateRequest request)
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

        if (userId > 0)
        {
            var referenceNumber = await GetReturnReferenceNumberAsync(connection, returnId)
                ?? $"ZWROT-{returnId}";
            await _notificationsService.NotifyJournalEntryAsync(returnId, referenceNumber, userId, request.Tresc);
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

    public async Task<ReturnRefundContextDto?> GetRefundContextAsync(int returnId)
    {
        var info = await GetReturnAllegroInfoAsync(returnId);
        if (info == null || info.AllegroAccountId == null || string.IsNullOrWhiteSpace(info.OrderId))
        {
            return null;
        }

        var orderDetails = await _allegroApiClient.GetOrderDetailsAsync(info.AllegroAccountId.Value, info.OrderId);
        if (orderDetails == null || string.IsNullOrWhiteSpace(orderDetails.Payment?.Id))
        {
            return null;
        }

        var context = new ReturnRefundContextDto
        {
            OrderId = info.OrderId,
            PaymentId = orderDetails.Payment?.Id ?? string.Empty
        };

        if (orderDetails.LineItems != null)
        {
            foreach (var item in orderDetails.LineItems)
            {
                if (string.IsNullOrWhiteSpace(item.Id))
                {
                    continue;
                }

                context.LineItems.Add(new RefundLineItemContextDto
                {
                    Id = item.Id,
                    Name = item.Offer?.Name ?? "Nieznany produkt",
                    Quantity = item.Quantity,
                    Price = new RefundValueDto
                    {
                        Amount = item.Price?.Amount ?? "0.00",
                        Currency = item.Price?.Currency ?? "PLN"
                    }
                });
            }
        }

        if (orderDetails.Delivery?.Cost != null && !string.IsNullOrWhiteSpace(orderDetails.Delivery.Cost.Amount))
        {
            context.Delivery = new RefundValueDto
            {
                Amount = orderDetails.Delivery.Cost.Amount ?? "0.00",
                Currency = orderDetails.Delivery.Cost.Currency ?? "PLN"
            };
        }

        return context;
    }

    public async Task<bool> RejectReturnAsync(int returnId, RejectCustomerReturnRequestDto request, string userDisplayName)
    {
        var info = await GetReturnAllegroInfoAsync(returnId);
        if (info == null || info.AllegroAccountId == null || string.IsNullOrWhiteSpace(info.AllegroReturnId))
        {
            return false;
        }

        await _allegroApiClient.RejectCustomerReturnAsync(info.AllegroAccountId.Value, info.AllegroReturnId, request);

        await using var connection = DbConnectionFactory.CreateMagazynConnection(_configuration);
        await connection.OpenAsync();

        var reason = request.Rejection?.Reason;
        var actionText = $"[API] Odrzucono zwrot w Allegro. Powód: {request.Rejection?.Code}";
        if (!string.IsNullOrWhiteSpace(reason))
        {
            actionText += $", Uzasadnienie: {reason}";
        }

        await AddReturnActionInternalAsync(connection, returnId, userDisplayName, actionText);
        return true;
    }

    public async Task<bool> RefundPaymentAsync(int returnId, PaymentRefundRequestDto request, string userDisplayName)
    {
        var info = await GetReturnAllegroInfoAsync(returnId);
        if (info == null || info.AllegroAccountId == null)
        {
            return false;
        }

        if (request.Payment == null || string.IsNullOrWhiteSpace(request.Payment.Id))
        {
            return false;
        }

        await _allegroApiClient.RefundPaymentAsync(info.AllegroAccountId.Value, request);

        var refundAmount = CalculateRefundAmount(request);
        var actionText = $"[API] Zlecono zwrot wpłaty w Allegro na kwotę {refundAmount:C}.";
        if (!string.IsNullOrWhiteSpace(request.SellerComment))
        {
            actionText += $" Komentarz: {request.SellerComment}";
        }

        await using var connection = DbConnectionFactory.CreateMagazynConnection(_configuration);
        await connection.OpenAsync();
        await AddReturnActionInternalAsync(connection, returnId, userDisplayName, actionText);

        return true;
    }

    public async Task<int?> CreateManualReturnAsync(ReturnManualCreateRequest request, int userId, string userDisplayName)
    {
        if (string.IsNullOrWhiteSpace(request.NumerListu)
            || string.IsNullOrWhiteSpace(request.BuyerFullName)
            || request.StanProduktuId <= 0
            || request.WybraniHandlowcy == null
            || request.WybraniHandlowcy.Count == 0)
        {
            return null;
        }

        await using var connection = DbConnectionFactory.CreateMagazynConnection(_configuration);
        await connection.OpenAsync();
        await using var transaction = await connection.BeginTransactionAsync();

        try
        {
            var statusDocelowyId = await GetStatusIdAsync(connection, "Oczekuje na decyzję handlowca", "StatusWewnetrzny", transaction);
            if (!statusDocelowyId.HasValue)
            {
                return null;
            }

            var uwagiColumn = await ResolveUwagiMagazynuColumnAsync(connection, transaction);
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

            var buyerFullName = request.BuyerFullName.Trim();
            var senderDetails = new
            {
                FullName = buyerFullName,
                Street = request.BuyerStreet,
                ZipCode = request.BuyerZipCode,
                City = request.BuyerCity,
                Phone = request.BuyerPhone
            };

            var nameParts = buyerFullName
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
                command.Parameters.AddWithValue("@BuyerFullName", buyerFullName);

                var newIdObj = await command.ExecuteScalarAsync();
                if (newIdObj == null)
                {
                    await transaction.RollbackAsync();
                    return null;
                }

                var returnId = Convert.ToInt32(newIdObj);
                var readColumn = await ResolveCzyPrzeczytanaColumnAsync(connection, transaction);

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

    public async Task<ReturnManualMetaDto> GetManualReturnMetaAsync()
    {
        var handlowcy = new List<ManualReturnRecipientDto>();
        await using (var connection = DbConnectionFactory.CreateDefaultConnection(_configuration))
        {
            await connection.OpenAsync();
            const string query = @"
                SELECT Id, `Nazwa Wyświetlana` AS NazwaWyswietlana
                FROM uzytkownicy
                WHERE Rola = 'Handlowiec'
                ORDER BY `Nazwa Wyświetlana`";
            await using var command = new MySqlCommand(query, connection);
            await using var reader = await command.ExecuteReaderAsync();
            while (await reader.ReadAsync())
            {
                handlowcy.Add(new ManualReturnRecipientDto
                {
                    Id = Convert.ToInt32(reader["Id"]),
                    NazwaWyswietlana = reader["NazwaWyswietlana"]?.ToString() ?? string.Empty
                });
            }
        }

        var produkty = new List<string>();
        var przewoznicy = new List<string>();
        await using (var connection = DbConnectionFactory.CreateMagazynConnection(_configuration))
        {
            await connection.OpenAsync();
            const string productQuery = @"
                SELECT DISTINCT ProductName
                FROM AllegroCustomerReturns
                WHERE ProductName IS NOT NULL AND TRIM(ProductName) != ''
                ORDER BY ProductName";
            await using (var command = new MySqlCommand(productQuery, connection))
            await using (var reader = await command.ExecuteReaderAsync())
            {
                while (await reader.ReadAsync())
                {
                    produkty.Add(reader["ProductName"]?.ToString() ?? string.Empty);
                }
            }

            const string carrierQuery = @"
                SELECT DISTINCT CarrierName
                FROM AllegroCustomerReturns
                WHERE CarrierName IS NOT NULL AND TRIM(CarrierName) != ''
                ORDER BY CarrierName";
            await using (var command = new MySqlCommand(carrierQuery, connection))
            await using (var reader = await command.ExecuteReaderAsync())
            {
                while (await reader.ReadAsync())
                {
                    przewoznicy.Add(reader["CarrierName"]?.ToString() ?? string.Empty);
                }
            }
        }

        return new ReturnManualMetaDto
        {
            Handlowcy = handlowcy,
            Produkty = produkty,
            Przewoznicy = przewoznicy
        };
    }

    public async Task<string> GetUserDisplayNameByIdAsync(int userId)
    {
        await using var connection = DbConnectionFactory.CreateDefaultConnection(_configuration);
        await connection.OpenAsync();
        const string query = "SELECT `Nazwa Wyświetlana` FROM uzytkownicy WHERE Id = @id LIMIT 1";
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

    public async Task<int?> ForwardToComplaintsAsync(int returnId, ForwardToComplaintRequest request, string userDisplayName)
    {
        await using var connection = DbConnectionFactory.CreateMagazynConnection(_configuration);
        await connection.OpenAsync();

        await InsertUnregisteredComplaintAsync(connection, returnId, request);
        var przekazal = string.IsNullOrWhiteSpace(request.Przekazal) ? userDisplayName : request.Przekazal;
        var actionText = $"Przekazano fizycznie na reklamacje. Przekazał: {przekazal}.";
        await AddReturnActionInternalAsync(connection, returnId, userDisplayName, actionText);
        await AddMagazynDziennikAsync(connection, returnId, userDisplayName, actionText, null);

        return 0;
    }

    public async Task<bool> CompleteReturnAsync(int returnId, string userDisplayName)
    {
        await using var connection = DbConnectionFactory.CreateMagazynConnection(_configuration);
        await connection.OpenAsync();

        var statusId = await GetStatusIdAsync(connection, CompletedStatusName, "StatusWewnetrzny");
        if (!statusId.HasValue)
        {
            return false;
        }

        const string query = @"
            UPDATE AllegroCustomerReturns
            SET StatusWewnetrznyId = @statusId
            WHERE Id = @id";

        await using var command = new MySqlCommand(query, connection);
        command.Parameters.AddWithValue("@statusId", statusId.Value);
        command.Parameters.AddWithValue("@id", returnId);

        var updated = await command.ExecuteNonQueryAsync() > 0;
        if (updated)
        {
            const string actionText = "Zwrot zakończony.";
            await AddReturnActionInternalAsync(connection, returnId, userDisplayName, actionText);
            await AddMagazynDziennikAsync(connection, returnId, userDisplayName, actionText, null);
        }

        return updated;
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

    private async Task<int?> GetStatusIdAsync(MySqlConnection connection, string name, string type, MySqlTransaction? transaction = null)
    {
        const string query = "SELECT Id FROM Statusy WHERE Nazwa = @name AND TypStatusu = @type LIMIT 1";
        await using var command = new MySqlCommand(query, connection, transaction);
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
        await using var command = new MySqlCommand("SELECT Id FROM uzytkownicy WHERE Login = @login LIMIT 1", connection);
        command.Parameters.AddWithValue("@login", login);
        var result = await command.ExecuteScalarAsync();
        return result == null ? null : Convert.ToInt32(result);
    }

    private async Task<string> ResolveUwagiMagazynuColumnAsync(MySqlConnection connection, MySqlTransaction? transaction = null)
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
        await using var command = new MySqlCommand(query, connection, transaction);
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
        await using var command = new MySqlCommand("SELECT `Nazwa Wyświetlana` FROM uzytkownicy WHERE Id = @id LIMIT 1", connection);
        command.Parameters.AddWithValue("@id", userId.Value);
        var result = await command.ExecuteScalarAsync();
        return result?.ToString();
    }

    private async Task<string?> GetAllegroAccountNameAsync(int? accountId)
    {
        if (!accountId.HasValue)
        {
            return null;
        }

        await using var connection = DbConnectionFactory.CreateDefaultConnection(_configuration);
        await connection.OpenAsync();
        await using var command = new MySqlCommand("SELECT AccountName FROM AllegroAccounts WHERE Id = @id LIMIT 1", connection);
        command.Parameters.AddWithValue("@id", accountId.Value);
        var result = await command.ExecuteScalarAsync();
        return result?.ToString();
    }

    private async Task<int?> FindWarehouseStatusIdAsync(MySqlConnection connection)
    {
        var candidates = new[]
        {
            "Przekazano do magazynu",
            "Do Magazynu",
            "Oczekuje na działania magazynu",
            "Oczekuje na magazyn",
            "W magazynie"
        };

        foreach (var name in candidates)
        {
            var id = await GetStatusIdByExactNameAsync(connection, name);
            if (id.HasValue)
            {
                return id;
            }
        }

        return await GetStatusIdByLikeAsync(connection, "%magaz%");
    }

    private static async Task<int?> GetStatusIdByExactNameAsync(MySqlConnection connection, string name)
    {
        await using var command = new MySqlCommand("SELECT Id FROM Statusy WHERE Nazwa = @name LIMIT 1", connection);
        command.Parameters.AddWithValue("@name", name);
        var result = await command.ExecuteScalarAsync();
        return result == null ? null : Convert.ToInt32(result);
    }

    private static async Task<int?> GetStatusIdByLikeAsync(MySqlConnection connection, string pattern)
    {
        await using var command = new MySqlCommand("SELECT Id FROM Statusy WHERE LOWER(Nazwa) LIKE LOWER(@pattern) LIMIT 1", connection);
        command.Parameters.AddWithValue("@pattern", pattern);
        var result = await command.ExecuteScalarAsync();
        return result == null ? null : Convert.ToInt32(result);
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
            FROM uzytkownicy
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

    private async Task<ReturnAllegroInfo?> GetReturnAllegroInfoAsync(int returnId)
    {
        await using var connection = DbConnectionFactory.CreateMagazynConnection(_configuration);
        await connection.OpenAsync();

        const string query = @"
            SELECT AllegroAccountId, AllegroReturnId, OrderId, ReferenceNumber
            FROM AllegroCustomerReturns
            WHERE Id = @id
            LIMIT 1";

        await using var command = new MySqlCommand(query, connection);
        command.Parameters.AddWithValue("@id", returnId);

        await using var reader = await command.ExecuteReaderAsync();
        if (!await reader.ReadAsync())
        {
            return null;
        }

        return new ReturnAllegroInfo(
            returnId,
            reader["AllegroAccountId"] == DBNull.Value ? null : Convert.ToInt32(reader["AllegroAccountId"]),
            reader["AllegroReturnId"] as string,
            reader["OrderId"] as string,
            reader["ReferenceNumber"]?.ToString());
    }

    private static decimal CalculateRefundAmount(PaymentRefundRequestDto request)
    {
        decimal total = 0;
        if (request.LineItems != null)
        {
            foreach (var item in request.LineItems)
            {
                if (item?.Value?.Amount == null)
                {
                    continue;
                }

                if (decimal.TryParse(item.Value.Amount.Replace(',', '.'), System.Globalization.NumberStyles.Any,
                        System.Globalization.CultureInfo.InvariantCulture, out var amount))
                {
                    total += amount;
                }
            }
        }

        if (request.Delivery?.Amount != null
            && decimal.TryParse(request.Delivery.Amount.Replace(',', '.'), System.Globalization.NumberStyles.Any,
                System.Globalization.CultureInfo.InvariantCulture, out var deliveryAmount))
        {
            total += deliveryAmount;
        }

        return total;
    }

    private async Task<int?> AddReturnActionInternalAsync(MySqlConnection connection, int? returnId, string userDisplayName, string content, MySqlTransaction? transaction = null)
    {
        var query = @"INSERT INTO ZwrotDzialania (ZwrotId, Data, Uzytkownik, Tresc) VALUES (@zwrotId, @data, @uzytkownik, @tresc)";
        await using var command = new MySqlCommand(query, connection, transaction);
        command.Parameters.AddWithValue("@zwrotId", returnId ?? 0);
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

    private static async Task<string?> GetReturnReferenceNumberAsync(MySqlConnection connection, int returnId)
    {
        const string query = "SELECT ReferenceNumber FROM AllegroCustomerReturns WHERE Id = @id LIMIT 1";
        await using var command = new MySqlCommand(query, connection);
        command.Parameters.AddWithValue("@id", returnId);
        var result = await command.ExecuteScalarAsync();
        return result?.ToString();
    }

    private async Task AddMagazynDziennikAsync(MySqlConnection connection, int? returnId, string userDisplayName, string action, MySqlTransaction? transaction)
    {
        var query = @"INSERT INTO MagazynDziennik (Data, Uzytkownik, Akcja, DotyczyZwrotuId)
                      VALUES (@data, @uzytkownik, @akcja, @id)";
        await using var command = new MySqlCommand(query, connection, transaction);
        command.Parameters.AddWithValue("@data", DateTime.Now);
        command.Parameters.AddWithValue("@uzytkownik", userDisplayName);
        command.Parameters.AddWithValue("@akcja", action);
        command.Parameters.AddWithValue("@id", returnId ?? (object)DBNull.Value);
        await command.ExecuteNonQueryAsync();
    }

    private async Task<int?> GetDefaultWarehouseStatusIdAsync(MySqlConnection connection)
    {
        await using var command = new MySqlCommand(
            "SELECT Id FROM Statusy WHERE Nazwa = 'Oczekuje na przyjęcie' AND TypStatusu = 'StatusWewnetrzny' LIMIT 1",
            connection);
        var result = await command.ExecuteScalarAsync();
        return result == null || result == DBNull.Value ? null : Convert.ToInt32(result);
    }

    private async Task<List<AllegroAccountInfo>> GetAuthorizedAccountsAsync(int? accountId)
    {
        var accounts = new List<AllegroAccountInfo>();
        await using var connection = DbConnectionFactory.CreateDefaultConnection(_configuration);
        await connection.OpenAsync();

        var query = "SELECT Id, AccountName FROM AllegroAccounts WHERE IsAuthorized = 1";
        if (accountId.HasValue)
        {
            query += " AND Id = @id";
        }

        await using var command = new MySqlCommand(query, connection);
        if (accountId.HasValue)
        {
            command.Parameters.AddWithValue("@id", accountId.Value);
        }

        await using var reader = await command.ExecuteReaderAsync();
        while (await reader.ReadAsync())
        {
            accounts.Add(new AllegroAccountInfo(reader.GetInt32("Id"), reader["AccountName"]?.ToString() ?? "Nieznane"));
        }

        return accounts;
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

    private async Task<string> ResolveCzyPrzeczytanaColumnAsync(MySqlConnection connection, MySqlTransaction? transaction = null)
    {
        const string query = @"
            SELECT COLUMN_NAME
            FROM INFORMATION_SCHEMA.COLUMNS
            WHERE TABLE_SCHEMA = DATABASE()
              AND TABLE_NAME = 'Wiadomosci'
              AND COLUMN_NAME IN ('CzyPrzeczytana', 'CzyOdczytana')
            LIMIT 1";

        await using var command = new MySqlCommand(query, connection, transaction);
        var result = await command.ExecuteScalarAsync();
        return result?.ToString() ?? "CzyPrzeczytana";
    }

    private static async Task<string?> GetStatusNameByIdAsync(MySqlConnection connection, int statusId)
    {
        await using var command = new MySqlCommand("SELECT Nazwa FROM Statusy WHERE Id = @id LIMIT 1", connection);
        command.Parameters.AddWithValue("@id", statusId);
        var result = await command.ExecuteScalarAsync();
        return result?.ToString();
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
                SUM(CASE WHEN IFNULL(s2.Nazwa, '') IN ('{DecisionStatusName}', '{CompletedStatusName}') THEN 1 ELSE 0 END) AS Zakonczone
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

    private static bool IsDecisionStatusFilter(string statusWewnetrzny)
        => statusWewnetrzny.Equals(DecisionStatusName, StringComparison.OrdinalIgnoreCase)
           || statusWewnetrzny.Equals(CompletedStatusName, StringComparison.OrdinalIgnoreCase);

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

    private static async Task InsertUnregisteredComplaintAsync(MySqlConnection connection, int returnId, ForwardToComplaintRequest request)
    {
        var customer = request.DaneKlienta ?? new ComplaintCustomerDto();
        var address = customer.Adres ?? new ComplaintAddressDto();
        var product = request.Produkt ?? new ComplaintProductDto();

        var fullName = $"{customer.Imie} {customer.Nazwisko}".Trim();
        var daneKlientaZbiorczo = string.Join(" | ", new[]
        {
            fullName,
            $"{address.Ulica}, {address.Kod} {address.Miasto}".Trim(),
            string.IsNullOrWhiteSpace(customer.Telefon) ? null : $"tel: {customer.Telefon}",
            string.IsNullOrWhiteSpace(customer.Email) ? null : $"e-mail: {customer.Email}"
        }.Where(value => !string.IsNullOrWhiteSpace(value)));

        const string insertSql = @"
            INSERT INTO NiezarejestrowaneZwrotyReklamacyjne
            (
                DataPrzekazania, PrzekazanePrzez, IdZwrotuWMagazynie,
                DaneKlienta, DaneProduktu, NumerFaktury, NumerSeryjny, UwagiMagazynu, KomentarzHandlowca,
                ImieKlienta, NazwiskoKlienta, EmailKlienta, TelefonKlienta,
                AdresUlica, AdresKodPocztowy, AdresMiasto,
                NazwaProduktu, NIP, DataZakupu, OpisUsterki
            )
            VALUES
            (
                @data, @kto, @idZw,
                @daneKlienta, @daneProduktu, @fv, @sn, @uwagiMag, @komHandl,
                @imie, @nazw, @email, @tel,
                @ulica, @kod, @miasto,
                @nazwaProd, @nip, @dataZakupu, @opis
            );";

        await using var cmd = new MySqlCommand(insertSql, connection);
        cmd.Parameters.AddWithValue("@data", DateTime.Now);
        cmd.Parameters.AddWithValue("@kto", request.Przekazal ?? string.Empty);
        cmd.Parameters.AddWithValue("@idZw", returnId);

        cmd.Parameters.AddWithValue("@daneKlienta", string.IsNullOrWhiteSpace(daneKlientaZbiorczo) ? DBNull.Value : daneKlientaZbiorczo);
        cmd.Parameters.AddWithValue("@daneProduktu", string.IsNullOrWhiteSpace(product.Nazwa) ? DBNull.Value : product.Nazwa);
        cmd.Parameters.AddWithValue("@fv", string.IsNullOrWhiteSpace(product.NrFaktury) ? DBNull.Value : product.NrFaktury);
        cmd.Parameters.AddWithValue("@sn", string.IsNullOrWhiteSpace(product.NrSeryjny) ? DBNull.Value : product.NrSeryjny);
        cmd.Parameters.AddWithValue("@uwagiMag", string.IsNullOrWhiteSpace(request.UwagiMagazynu) ? DBNull.Value : request.UwagiMagazynu);
        cmd.Parameters.AddWithValue("@komHandl", string.IsNullOrWhiteSpace(request.UwagiHandlowca) ? DBNull.Value : request.UwagiHandlowca);

        cmd.Parameters.AddWithValue("@imie", string.IsNullOrWhiteSpace(customer.Imie) ? DBNull.Value : customer.Imie);
        cmd.Parameters.AddWithValue("@nazw", string.IsNullOrWhiteSpace(customer.Nazwisko) ? DBNull.Value : customer.Nazwisko);
        cmd.Parameters.AddWithValue("@email", string.IsNullOrWhiteSpace(customer.Email) ? DBNull.Value : customer.Email);
        cmd.Parameters.AddWithValue("@tel", string.IsNullOrWhiteSpace(customer.Telefon) ? DBNull.Value : customer.Telefon);

        cmd.Parameters.AddWithValue("@ulica", string.IsNullOrWhiteSpace(address.Ulica) ? DBNull.Value : address.Ulica);
        cmd.Parameters.AddWithValue("@kod", string.IsNullOrWhiteSpace(address.Kod) ? DBNull.Value : address.Kod);
        cmd.Parameters.AddWithValue("@miasto", string.IsNullOrWhiteSpace(address.Miasto) ? DBNull.Value : address.Miasto);

        cmd.Parameters.AddWithValue("@nazwaProd", string.IsNullOrWhiteSpace(product.Nazwa) ? DBNull.Value : product.Nazwa);
        cmd.Parameters.AddWithValue("@nip", DBNull.Value);
        cmd.Parameters.AddWithValue("@dataZakupu", DBNull.Value);
        cmd.Parameters.AddWithValue("@opis", string.IsNullOrWhiteSpace(request.PowodKlienta) ? DBNull.Value : request.PowodKlienta);

        await cmd.ExecuteNonQueryAsync();
    }

    private async Task<string> GenerateZgloszenieNumberAsync()
    {
        var lastZgloszenie = await _context.Zgloszenia
            .OrderByDescending(z => z.Id)
            .FirstOrDefaultAsync();

        var nextNumber = lastZgloszenie != null ? lastZgloszenie.Id + 1 : 1;
        return $"R/{nextNumber}/{DateTime.UtcNow.Year}";
    }

    private sealed record AllegroAccountInfo(int Id, string Name);

    private sealed record ReturnAllegroInfo(
        int ReturnId,
        int? AllegroAccountId,
        string? AllegroReturnId,
        string? OrderId,
        string? ReferenceNumber);
}
