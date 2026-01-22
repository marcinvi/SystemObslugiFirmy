// Plik: MagazynControl.cs (WERSJA FINALNA - POPRAWIONA LOGIKA SKANERA DPD)
using Reklamacje_Dane.Allegro;
using Reklamacje_Dane.Allegro.Returns;
using System;
using System.Collections.Generic;
using System.Data;
using MySql.Data.MySqlClient;
using System.Drawing;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows.Forms;
using Newtonsoft.Json;
using Button = System.Windows.Forms.Button;
using Control = System.Windows.Forms.Control;
using UserControl = System.Windows.Forms.UserControl;

namespace Reklamacje_Dane
{
    public partial class MagazynControl : UserControl
    {
        private readonly string _fullName;
        private readonly string _userRole;
        private readonly DatabaseService _dbServiceBaza;
        private readonly DatabaseService _dbServiceMagazyn;
        private Timer _searchDebounceTimer;
        private Timer _syncTimer;
        private Button _currentFilterButton;
        private KomunikatorControl _komunikatorControl;
        private string _uwagiMagazynuColumnName;

        public struct ProgressReport
        {
            public int Current;
            public int Total;
            public string Message;
        }

        private class AllegroAccountItem
        {
            public int Id { get; set; }
            public string AccountName { get; set; }
            public override string ToString() => AccountName;
        }
        private class ProcessedReturnData
        {
            public AllegroCustomerReturn Return { get; set; }
            public OrderDetails OrderDetails { get; set; }
            public Reklamacje_Dane.Allegro.Invoice Invoice { get; set; }
        }

        public MagazynControl(string fullName, string userRole)
        {
            InitializeComponent();
            _fullName = fullName;
            _userRole = userRole;
            _dbServiceBaza = new DatabaseService(DatabaseHelper.GetConnectionString());
            _dbServiceMagazyn = new DatabaseService(MagazynDatabaseHelper.GetConnectionString());
            _searchDebounceTimer = new Timer { Interval = 300 };
            _searchDebounceTimer.Tick += (s, e) =>
            {
                _searchDebounceTimer.Stop();
                LoadReturnsFromDbAsync().FireAndForgetSafe(this.FindForm());
            };
            AppEvents.ZwrotyChanged += OnZwrotyChanged;
        }
        private async void OnZwrotyChanged()
        {
            if (!IsHandleCreated)
                return;

            if (InvokeRequired)
            {
                BeginInvoke(new Action(OnZwrotyChanged));
                return;
            }

            try
            {
                await LoadDataAsync();
            }
            catch { /* cicho – odświeżenie nie może wywalić UI */ }
        }

        private async void MagazynControl_Load(object sender, EventArgs e)
        {
            InitializeUiElements();
            AttachEventHandlers();
            await LoadDataAsync();
            txtScannerInput.Focus();
            _syncTimer = new Timer { Interval = 60000 };
            _syncTimer.Tick += SyncTimer_Tick;
            _syncTimer.Start();
        }

        private void InitializeUiElements()
        {
            _currentFilterButton = btnFilterOczekujace;
            SetActiveFilterButton(btnFilterOczekujace);
            comboFilterStatusAllegro.Items.AddRange(new object[]
            {
                "Wszystkie", "Dostarczono", "W drodze", "Gotowy do odbioru", "Utworzono", "Zwrócono prowizję"
            });
            comboFilterStatusAllegro.SelectedItem = "Wszystkie";
            _komunikatorControl = new KomunikatorControl(_dbServiceBaza, _dbServiceMagazyn, _fullName, _userRole)
            {
                Dock = DockStyle.Fill
            };
            this.panelKomunikator.Controls.Add(_komunikatorControl);
        }

        private async Task LoadDataAsync()
        {
            await LoadAllegroAccountsAsync();
            await LoadReturnsFromDbAsync();
            await LoadDziennikAsync();
            if (_komunikatorControl != null)
            {
                await _komunikatorControl.LoadMessagesAsync();
            }
            lblLastRefresh.Text = $"Ostatnie odświeżenie: {DateTime.Now:HH:mm:ss}";
        }

        private void AttachEventHandlers()
        {
            txtSearch.TextChanged += (s, ev) => _searchDebounceTimer.Start();
            txtScannerInput.KeyDown += TxtScannerInput_KeyDown;
            btnFilterOczekujace.Click += FilterButton_Click;
            btnFilterOczekujeNaDecyzje.Click += FilterButton_Click;
            btnFilterPoDecyzji.Click += FilterButton_Click;
            btnFilterWszystkie.Click += FilterButton_Click;
            btnFilterWDrodze.Click += FilterButton_Click;
            comboFilterStatusAllegro.SelectedIndexChanged += (s, ev) => LoadReturnsFromDbAsync().FireAndForgetSafe(this.FindForm());
            refreshIcon.Click += async (s, ev) => await LoadDataAsync();
            dgvReturns.DataBindingComplete += dgvReturns_DataBindingComplete;
            btnDodajRecznie.Click += btnDodajRecznie_Click;
            btnFetchReturns.Click += btnFetchReturns_Click;
            dgvReturns.CellDoubleClick += dgvReturns_CellDoubleClick;
        }

        private async void btnFetchReturns_Click(object sender, EventArgs e)
        {
            btnFetchReturns.Enabled = false;
            this.Cursor = Cursors.WaitCursor;
            using (var progressForm = new FormProgress())
            {
                progressForm.Show(this.FindForm());
                IProgress<ProgressReport> progress = new Progress<ProgressReport>(report =>
                {
                    progressForm.UpdateProgress(report.Current, report.Total, report.Message);
                });
                try
                {
                    var apiSyncAvailable = false;
                    try
                    {
                        apiSyncAvailable = ApiSyncService.Instance != null
                                           && ApiSyncService.Instance.IsInitialized
                                           && ApiSyncService.Instance.IsAuthenticated;
                    }
                    catch
                    {
                        apiSyncAvailable = false;
                    }

                    int totalProcessed;

                    if (apiSyncAvailable)
                    {
                        totalProcessed = await SyncReturnsFromApiAsync(progress);
                    }
                    else
                    {
                        var selectedAccountItem = (AllegroAccountItem)comboAllegroAccounts.SelectedItem;
                        var accountsToFetch = new List<AllegroAccountItem>();
                        if (selectedAccountItem.Id == 0)
                        {
                            accountsToFetch.AddRange(((List<AllegroAccountItem>)comboAllegroAccounts.DataSource).Where(a => a.Id != 0));
                        }
                        else
                        {
                            accountsToFetch.Add(selectedAccountItem);
                        }
                        totalProcessed = 0;
                        int accountsCount = accountsToFetch.Count;
                        for (int i = 0; i < accountsCount; i++)
                        {
                            var account = accountsToFetch[i];
                            progress.Report(new ProgressReport { Message = $"Sprawdzam konto: {account.AccountName} ({i + 1}/{accountsCount})..." });
                            using (var con = Database.GetNewOpenConnection())
                            {
                                var apiClient = await DatabaseHelper.GetApiClientForAccountAsync(account.Id, con);
                                if (apiClient != null)
                                {
                                    string dateFrom = DateTime.UtcNow.AddDays(-60).ToString("yyyy-MM-dd'T'HH:mm:ss.fff'Z'");
                                    var filters = new Dictionary<string, string> { { "createdAt.gte", dateFrom } };
                                    var result = await apiClient.GetCustomerReturnsAsync(limit: 1000, filters: filters);
                                    if (result?.CustomerReturns != null && result.CustomerReturns.Count > 0)
                                    {
                                        totalProcessed += await SaveReturnsToDbAsync(result.CustomerReturns, account.Id, apiClient, progress);
                                    }
                                }
                            }
                        }
                    }
                    await LogToDziennikAsync($"Zsynchronizowano zwroty. Przetworzono {totalProcessed} wpisów.", null);
                    await LoadDataAsync();
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Wystąpił błąd: " + ex.Message, "Błąd API", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
                finally
                {
                    progressForm.Close();
                    btnFetchReturns.Enabled = true;
                    this.Cursor = Cursors.Default;
                }
            }
        }

        private async Task<int> SaveReturnsToDbAsync(List<AllegroCustomerReturn> returns, int accountId, AllegroApiClient apiClient, IProgress<ProgressReport> progress)
        {
            int total = returns.Count;
            var processedData = new List<ProcessedReturnData>();

            for (int i = 0; i < total; i++)
            {
                var ret = returns[i];
                progress?.Report(new ProgressReport
                {
                    Current = i + 1,
                    Total = total,
                    Message = $"Pobieram szczegóły zwrotu {ret.ReferenceNumber} ({i + 1}/{total})..."
                });

                var orderDetailsTask = apiClient.GetOrderDetailsByCheckoutFormIdAsync(ret.OrderId);
                var invoiceTask = apiClient.GetInvoicesForOrderAsync(ret.OrderId);
                await Task.WhenAll(orderDetailsTask, invoiceTask);

                var orderDetails = orderDetailsTask.Result;
                var invoice = invoiceTask.Result?.FirstOrDefault();

                processedData.Add(new ProcessedReturnData
                {
                    Return = ret,
                    OrderDetails = orderDetails,
                    Invoice = invoice
                });
            }

            progress?.Report(new ProgressReport { Message = "Zapisywanie danych do lokalnej bazy danych..." });

            int processedRecords = 0;
            object defaultStatusId = await _dbServiceMagazyn.ExecuteScalarAsync(
                "SELECT Id FROM Statusy WHERE Nazwa = 'Oczekuje na przyjęcie' AND TypStatusu = 'StatusWewnetrzny'"
            );

            using (var con = MagazynDatabaseHelper.GetConnection())
            {
                await con.OpenAsync();
                using (var transaction = con.BeginTransaction())
                {
                    foreach (var data in processedData)
                    {
                        var ret = data.Return;
                        var orderDetails = data.OrderDetails;
                        var invoice = data.Invoice;
                        var item = ret.Items?.FirstOrDefault();

                        var cmd = new MySqlCommand(@"
                            INSERT INTO AllegroCustomerReturns (
                                AllegroReturnId, AllegroAccountId, ReferenceNumber, OrderId, BuyerLogin, CreatedAt, StatusAllegro, Waybill, JsonDetails, StatusWewnetrznyId, CarrierName, InvoiceNumber, ProductName, OfferId, Quantity, PaymentType, FulfillmentStatus, Delivery_FirstName, Delivery_LastName, Delivery_Street, Delivery_ZipCode, Delivery_City, Delivery_PhoneNumber, Buyer_FirstName, Buyer_LastName, Buyer_Street, Buyer_ZipCode, Buyer_City, Buyer_PhoneNumber, Invoice_CompanyName, Invoice_TaxId, Invoice_Street, Invoice_ZipCode, Invoice_City
                            ) VALUES (
                                @AllegroReturnId, @AllegroAccountId, @ReferenceNumber, @OrderId, @BuyerLogin, @CreatedAt, @StatusAllegro, @Waybill, @JsonDetails, @StatusWewnetrznyId, @CarrierName, @InvoiceNumber, @ProductName, @OfferId, @Quantity, @PaymentType, @FulfillmentStatus, @Delivery_FirstName, @Delivery_LastName, @Delivery_Street, @Delivery_ZipCode, @Delivery_City, @Delivery_PhoneNumber, @Buyer_FirstName, @Buyer_LastName, @Buyer_Street, @Buyer_ZipCode, @Buyer_City, @Buyer_PhoneNumber, @Invoice_CompanyName, @Invoice_TaxId, @Invoice_Street, @Invoice_ZipCode, @Invoice_City
                            )
                            ON DUPLICATE KEY UPDATE
                                StatusAllegro = VALUES(StatusAllegro),
                                Waybill = VALUES(Waybill),
                                CarrierName = VALUES(CarrierName),
                                JsonDetails = VALUES(JsonDetails),
                                InvoiceNumber = VALUES(InvoiceNumber);
                        ", con, transaction);

                        cmd.Parameters.AddWithValue("@AllegroReturnId", ret.Id);
                        cmd.Parameters.AddWithValue("@AllegroAccountId", accountId);
                        cmd.Parameters.AddWithValue("@ReferenceNumber", ret.ReferenceNumber);
                        cmd.Parameters.AddWithValue("@OrderId", ret.OrderId);
                        cmd.Parameters.AddWithValue("@BuyerLogin", (object)ret.Buyer?.Login ?? DBNull.Value);
                        cmd.Parameters.AddWithValue("@CreatedAt", ret.CreatedAt);
                        cmd.Parameters.AddWithValue("@StatusAllegro", ret.Status);
                        cmd.Parameters.AddWithValue("@Waybill", (object)ret.Parcels?.FirstOrDefault()?.Waybill ?? DBNull.Value);
                        cmd.Parameters.AddWithValue("@CarrierName", (object)ret.Parcels?.FirstOrDefault()?.CarrierId ?? DBNull.Value);
                        cmd.Parameters.AddWithValue("@JsonDetails", JsonConvert.SerializeObject(ret));
                        cmd.Parameters.AddWithValue("@StatusWewnetrznyId", defaultStatusId ?? (object)DBNull.Value);
                        cmd.Parameters.AddWithValue("@InvoiceNumber", (object)invoice?.InvoiceNumber ?? DBNull.Value);
                        cmd.Parameters.AddWithValue("@ProductName", (object)item?.Name ?? DBNull.Value);
                        cmd.Parameters.AddWithValue("@OfferId", (object)item?.OfferId ?? DBNull.Value);
                        cmd.Parameters.AddWithValue("@Quantity", (object)item?.Quantity ?? DBNull.Value);
                        cmd.Parameters.AddWithValue("@PaymentType", (object)orderDetails?.Payment?.Type ?? DBNull.Value);
                        cmd.Parameters.AddWithValue("@FulfillmentStatus", (object)orderDetails?.Fulfillment?.Status ?? DBNull.Value);
                        cmd.Parameters.AddWithValue("@Delivery_FirstName", (object)orderDetails?.Delivery?.Address?.FirstName ?? DBNull.Value);
                        cmd.Parameters.AddWithValue("@Delivery_LastName", (object)orderDetails?.Delivery?.Address?.LastName ?? DBNull.Value);
                        cmd.Parameters.AddWithValue("@Delivery_Street", (object)orderDetails?.Delivery?.Address?.Street ?? DBNull.Value);
                        cmd.Parameters.AddWithValue("@Delivery_ZipCode", (object)orderDetails?.Delivery?.Address?.ZipCode ?? DBNull.Value);
                        cmd.Parameters.AddWithValue("@Delivery_City", (object)orderDetails?.Delivery?.Address?.City ?? DBNull.Value);
                        cmd.Parameters.AddWithValue("@Delivery_PhoneNumber", (object)orderDetails?.Delivery?.Address?.PhoneNumber ?? DBNull.Value);
                        cmd.Parameters.AddWithValue("@Buyer_FirstName", (object)orderDetails?.Buyer?.FirstName ?? DBNull.Value);
                        cmd.Parameters.AddWithValue("@Buyer_LastName", (object)orderDetails?.Buyer?.LastName ?? DBNull.Value);
                        cmd.Parameters.AddWithValue("@Buyer_Street", (object)orderDetails?.Buyer?.Address?.Street ?? DBNull.Value);
                        cmd.Parameters.AddWithValue("@Buyer_ZipCode", (object)orderDetails?.Buyer?.Address?.PostCode ?? DBNull.Value);
                        cmd.Parameters.AddWithValue("@Buyer_City", (object)orderDetails?.Buyer?.Address?.City ?? DBNull.Value);
                        cmd.Parameters.AddWithValue("@Buyer_PhoneNumber", (object)orderDetails?.Buyer?.PhoneNumber ?? DBNull.Value);
                        cmd.Parameters.AddWithValue("@Invoice_CompanyName", (object)orderDetails?.Invoice?.Address?.Company?.Name ?? DBNull.Value);
                        cmd.Parameters.AddWithValue("@Invoice_TaxId", (object)orderDetails?.Invoice?.Address?.Company?.TaxId ?? DBNull.Value);
                        cmd.Parameters.AddWithValue("@Invoice_Street", (object)orderDetails?.Invoice?.Address?.Street ?? DBNull.Value);
                        cmd.Parameters.AddWithValue("@Invoice_ZipCode", (object)orderDetails?.Invoice?.Address?.ZipCode ?? DBNull.Value);
                        cmd.Parameters.AddWithValue("@Invoice_City", (object)orderDetails?.Invoice?.Address?.City ?? DBNull.Value);

                        await cmd.ExecuteNonQueryAsync();
                        processedRecords++;
                    }
                    transaction.Commit();
                }
            }
            return processedRecords;
        }


        #region Wczytywanie/filtry/formatowanie siatki

        private async Task LoadAllegroAccountsAsync()
        {
            var accounts = new List<AllegroAccountItem>
            {
                new AllegroAccountItem { Id = 0, AccountName = "Wszystkie Konta" }
            };
            try
            {
                var dt = await _dbServiceBaza.GetDataTableAsync(
                    "SELECT Id, AccountName FROM AllegroAccounts WHERE IsAuthorized = 1"
                );
                foreach (DataRow row in dt.Rows)
                {
                    accounts.Add(new AllegroAccountItem
                    {
                        Id = Convert.ToInt32(row["Id"]),
                        AccountName = row["AccountName"].ToString()
                    });
                }
                comboAllegroAccounts.DataSource = accounts;
                comboAllegroAccounts.DisplayMember = "AccountName";
                comboAllegroAccounts.ValueMember = "Id";
            }
            catch (Exception ex)
            {
                MessageBox.Show("Nie udało się wczytać kont Allegro: " + ex.Message, "Błąd",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private async Task<int> SyncReturnsFromApiAsync(IProgress<ProgressReport> progress)
        {
            progress?.Report(new ProgressReport { Message = "Pobieram zwroty z API..." });

            var magazynReturns = await ApiSyncService.Instance.GetZwrotyMagazynAsync();
            var handloweReturns = await ApiSyncService.Instance.GetZwrotyHandloweAsync();
            var allReturns = magazynReturns
                .Concat(handloweReturns)
                .GroupBy(r => r.Id)
                .Select(g => g.First())
                .ToList();

            if (allReturns.Count == 0)
            {
                return 0;
            }

            var statusMaps = await LoadStatusMapsAsync();
            statusMaps.TryGetValue("StatusWewnetrzny", out var statusWewnetrznyMap);
            statusMaps.TryGetValue("StanProduktu", out var stanProduktuMap);
            statusMaps.TryGetValue("DecyzjaHandlowca", out var decyzjaMap);

            var uwagiColumn = await ResolveUwagiMagazynuColumnNameAsync();

            var processed = 0;
            using (var con = MagazynDatabaseHelper.GetConnection())
            {
                await con.OpenAsync();
                using (var transaction = con.BeginTransaction())
                {
                    for (int i = 0; i < allReturns.Count; i++)
                    {
                        var listItem = allReturns[i];
                        progress?.Report(new ProgressReport
                        {
                            Current = i + 1,
                            Total = allReturns.Count,
                            Message = $"Synchronizuję zwrot {listItem.ReferenceNumber ?? listItem.Id.ToString()} ({i + 1}/{allReturns.Count})..."
                        });

                        var details = await ApiSyncService.Instance.GetZwrotDetailsAsync(listItem.Id);
                        var returnId = details?.Id ?? listItem.Id;
                        var referenceNumber = details?.ReferenceNumber ?? listItem.ReferenceNumber;

                        var createdAt = details?.CreatedAt != default
                            ? details.CreatedAt
                            : listItem.CreatedAt;

                        var statusWewnetrznyId = details?.StatusWewnetrzny != null
                            ? TryGetStatusId(statusWewnetrznyMap, details.StatusWewnetrzny)
                            : TryGetStatusId(statusWewnetrznyMap, listItem.StatusWewnetrzny);

                        var stanProduktuId = details?.StanProduktuId
                            ?? TryGetStatusId(stanProduktuMap, details?.StanProduktuName)
                            ?? TryGetStatusId(stanProduktuMap, listItem.StanProduktu);

                        var decyzjaHandlowcaId = details?.DecyzjaHandlowcaId
                            ?? TryGetStatusId(decyzjaMap, details?.DecyzjaHandlowcaName)
                            ?? TryGetStatusId(decyzjaMap, listItem.DecyzjaHandlowca);

                        var buyerName = details?.BuyerName ?? listItem.BuyerName;
                        var deliveryName = details?.DeliveryName;

                        var buyerSplit = SplitName(buyerName);
                        var deliverySplit = SplitName(deliveryName);

                        var jsonDetails = JsonConvert.SerializeObject(details ?? (object)listItem);

                        int? existingId = null;
                        using (var findCmd = new MySqlCommand(
                                   "SELECT Id FROM AllegroCustomerReturns WHERE Id = @id OR ReferenceNumber = @ref LIMIT 1",
                                   con, transaction))
                        {
                            findCmd.Parameters.AddWithValue("@id", returnId);
                            findCmd.Parameters.AddWithValue("@ref", referenceNumber);
                            var existingObj = await findCmd.ExecuteScalarAsync();
                            if (existingObj != null && existingObj != DBNull.Value)
                            {
                                existingId = Convert.ToInt32(existingObj);
                            }
                        }

                        if (existingId.HasValue)
                        {
                            using (var updateCmd = new MySqlCommand($@"
                                UPDATE AllegroCustomerReturns
                                SET ReferenceNumber = @ReferenceNumber,
                                    OrderId = COALESCE(@OrderId, OrderId),
                                    AllegroReturnId = COALESCE(@AllegroReturnId, AllegroReturnId),
                                    BuyerLogin = COALESCE(@BuyerLogin, BuyerLogin),
                                    CreatedAt = @CreatedAt,
                                    StatusAllegro = COALESCE(@StatusAllegro, StatusAllegro),
                                    Waybill = COALESCE(@Waybill, Waybill),
                                    CarrierName = COALESCE(@CarrierName, CarrierName),
                                    InvoiceNumber = COALESCE(@InvoiceNumber, InvoiceNumber),
                                    ProductName = COALESCE(@ProductName, ProductName),
                                    OfferId = COALESCE(@OfferId, OfferId),
                                    Quantity = COALESCE(@Quantity, Quantity),
                                    {uwagiColumn} = COALESCE(@UwagiMagazynu, {uwagiColumn}),
                                    StatusWewnetrznyId = COALESCE(@StatusWewnetrznyId, StatusWewnetrznyId),
                                    StanProduktuId = COALESCE(@StanProduktuId, StanProduktuId),
                                    DecyzjaHandlowcaId = COALESCE(@DecyzjaHandlowcaId, DecyzjaHandlowcaId),
                                    HandlowiecOpiekunId = COALESCE(@HandlowiecOpiekunId, HandlowiecOpiekunId),
                                    IsManual = @IsManual,
                                    BuyerFullName = COALESCE(@BuyerFullName, BuyerFullName),
                                    Buyer_FirstName = COALESCE(@BuyerFirstName, Buyer_FirstName),
                                    Buyer_LastName = COALESCE(@BuyerLastName, Buyer_LastName),
                                    Buyer_Street = COALESCE(@BuyerStreet, Buyer_Street),
                                    Buyer_PhoneNumber = COALESCE(@BuyerPhone, Buyer_PhoneNumber),
                                    Delivery_FirstName = COALESCE(@DeliveryFirstName, Delivery_FirstName),
                                    Delivery_LastName = COALESCE(@DeliveryLastName, Delivery_LastName),
                                    Delivery_Street = COALESCE(@DeliveryStreet, Delivery_Street),
                                    Delivery_PhoneNumber = COALESCE(@DeliveryPhone, Delivery_PhoneNumber),
                                    JsonDetails = @JsonDetails
                                WHERE Id = @Id",
                                con, transaction))
                            {
                                FillApiReturnParameters(updateCmd, details, listItem, buyerSplit, deliverySplit, createdAt,
                                    statusWewnetrznyId, stanProduktuId, decyzjaHandlowcaId, referenceNumber, jsonDetails);
                                updateCmd.Parameters.AddWithValue("@Id", existingId.Value);
                                await updateCmd.ExecuteNonQueryAsync();
                            }
                        }
                        else
                        {
                            using (var insertCmd = new MySqlCommand($@"
                                INSERT INTO AllegroCustomerReturns (
                                    Id, ReferenceNumber, OrderId, AllegroReturnId, BuyerLogin, CreatedAt, StatusAllegro,
                                    Waybill, CarrierName, InvoiceNumber, ProductName, OfferId, Quantity, {uwagiColumn},
                                    StatusWewnetrznyId, StanProduktuId, DecyzjaHandlowcaId, HandlowiecOpiekunId, IsManual,
                                    BuyerFullName, Buyer_FirstName, Buyer_LastName, Buyer_Street, Buyer_PhoneNumber,
                                    Delivery_FirstName, Delivery_LastName, Delivery_Street, Delivery_PhoneNumber, JsonDetails
                                ) VALUES (
                                    @Id, @ReferenceNumber, @OrderId, @AllegroReturnId, @BuyerLogin, @CreatedAt, @StatusAllegro,
                                    @Waybill, @CarrierName, @InvoiceNumber, @ProductName, @OfferId, @Quantity, @UwagiMagazynu,
                                    @StatusWewnetrznyId, @StanProduktuId, @DecyzjaHandlowcaId, @HandlowiecOpiekunId, @IsManual,
                                    @BuyerFullName, @BuyerFirstName, @BuyerLastName, @BuyerStreet, @BuyerPhone,
                                    @DeliveryFirstName, @DeliveryLastName, @DeliveryStreet, @DeliveryPhone, @JsonDetails
                                )",
                                con, transaction))
                            {
                                FillApiReturnParameters(insertCmd, details, listItem, buyerSplit, deliverySplit, createdAt,
                                    statusWewnetrznyId, stanProduktuId, decyzjaHandlowcaId, referenceNumber, jsonDetails);
                                insertCmd.Parameters.AddWithValue("@Id", returnId);
                                await insertCmd.ExecuteNonQueryAsync();
                            }
                        }

                        processed++;
                    }

                    transaction.Commit();
                }
            }

            return processed;
        }

        private static (string FirstName, string LastName) SplitName(string fullName)
        {
            if (string.IsNullOrWhiteSpace(fullName))
            {
                return (null, null);
            }

            var parts = fullName.Trim().Split(new[] { ' ' }, 2, StringSplitOptions.RemoveEmptyEntries);
            return parts.Length == 1 ? (parts[0], null) : (parts[0], parts[1]);
        }

        private async Task<Dictionary<string, Dictionary<string, int>>> LoadStatusMapsAsync()
        {
            var result = new Dictionary<string, Dictionary<string, int>>(StringComparer.OrdinalIgnoreCase);
            var dt = await _dbServiceMagazyn.GetDataTableAsync("SELECT Id, Nazwa, TypStatusu FROM Statusy");

            foreach (DataRow row in dt.Rows)
            {
                var type = row["TypStatusu"]?.ToString() ?? string.Empty;
                var name = row["Nazwa"]?.ToString() ?? string.Empty;
                if (string.IsNullOrWhiteSpace(type) || string.IsNullOrWhiteSpace(name))
                {
                    continue;
                }

                if (!result.TryGetValue(type, out var map))
                {
                    map = new Dictionary<string, int>(StringComparer.OrdinalIgnoreCase);
                    result[type] = map;
                }

                map[name] = Convert.ToInt32(row["Id"]);
            }

            return result;
        }

        private static int? TryGetStatusId(Dictionary<string, int> map, string name)
        {
            if (map == null || string.IsNullOrWhiteSpace(name))
            {
                return null;
            }

            return map.TryGetValue(name, out var id) ? id : (int?)null;
        }

        private async Task<string> ResolveUwagiMagazynuColumnNameAsync()
        {
            if (!string.IsNullOrWhiteSpace(_uwagiMagazynuColumnName))
            {
                return _uwagiMagazynuColumnName;
            }

            var query = @"
                SELECT COLUMN_NAME
                FROM INFORMATION_SCHEMA.COLUMNS
                WHERE TABLE_SCHEMA = DATABASE()
                  AND TABLE_NAME = 'AllegroCustomerReturns'
                  AND COLUMN_NAME IN ('UwagiMagazynu', 'UwagiMagazyn')
                LIMIT 1";

            var result = await _dbServiceMagazyn.ExecuteScalarAsync(query);
            _uwagiMagazynuColumnName = result?.ToString() ?? "UwagiMagazynu";
            return _uwagiMagazynuColumnName;
        }

        private static void FillApiReturnParameters(
            MySqlCommand command,
            ZwrotSzczegolyApi details,
            ZwrotApi listItem,
            (string FirstName, string LastName) buyerSplit,
            (string FirstName, string LastName) deliverySplit,
            DateTime createdAt,
            int? statusWewnetrznyId,
            int? stanProduktuId,
            int? decyzjaHandlowcaId,
            string referenceNumber,
            string jsonDetails)
        {
            command.Parameters.AddWithValue("@ReferenceNumber", referenceNumber);
            command.Parameters.AddWithValue("@OrderId", details?.OrderId);
            command.Parameters.AddWithValue("@AllegroReturnId", details?.AllegroReturnId);
            command.Parameters.AddWithValue("@BuyerLogin", details?.BuyerLogin);
            command.Parameters.AddWithValue("@CreatedAt", createdAt);
            command.Parameters.AddWithValue("@StatusAllegro", details?.StatusAllegro ?? listItem?.StatusAllegro);
            command.Parameters.AddWithValue("@Waybill", details?.Waybill ?? listItem?.Waybill);
            command.Parameters.AddWithValue("@CarrierName", details?.CarrierName);
            command.Parameters.AddWithValue("@InvoiceNumber", details?.InvoiceNumber);
            command.Parameters.AddWithValue("@ProductName", details?.ProductName ?? listItem?.ProductName);
            command.Parameters.AddWithValue("@OfferId", details?.OfferId);
            command.Parameters.AddWithValue("@Quantity", details?.Quantity);
            command.Parameters.AddWithValue("@UwagiMagazynu", details?.UwagiMagazynu);
            command.Parameters.AddWithValue("@StatusWewnetrznyId", statusWewnetrznyId);
            command.Parameters.AddWithValue("@StanProduktuId", stanProduktuId);
            command.Parameters.AddWithValue("@DecyzjaHandlowcaId", decyzjaHandlowcaId);
            command.Parameters.AddWithValue("@HandlowiecOpiekunId", listItem?.HandlowiecId);
            command.Parameters.AddWithValue("@IsManual", details?.IsManual ?? listItem?.IsManual ?? false);
            command.Parameters.AddWithValue("@BuyerFullName", details?.BuyerName ?? listItem?.BuyerName);
            command.Parameters.AddWithValue("@BuyerFirstName", buyerSplit.FirstName);
            command.Parameters.AddWithValue("@BuyerLastName", buyerSplit.LastName);
            command.Parameters.AddWithValue("@BuyerStreet", details?.BuyerAddress);
            command.Parameters.AddWithValue("@BuyerPhone", details?.BuyerPhone);
            command.Parameters.AddWithValue("@DeliveryFirstName", deliverySplit.FirstName);
            command.Parameters.AddWithValue("@DeliveryLastName", deliverySplit.LastName);
            command.Parameters.AddWithValue("@DeliveryStreet", details?.DeliveryAddress);
            command.Parameters.AddWithValue("@DeliveryPhone", details?.DeliveryPhone);
            command.Parameters.AddWithValue("@JsonDetails", jsonDetails);
        }

        private async Task LoadReturnsFromDbAsync()
        {
            var queryBuilder = new System.Text.StringBuilder(@"
        SELECT
            acr.Id, acr.ReferenceNumber, acr.Waybill, acr.StatusAllegro,
            COALESCE(CONCAT(acr.Delivery_FirstName, ' ', acr.Delivery_LastName),
                     CONCAT(acr.Buyer_FirstName, ' ', acr.Buyer_LastName),
                     acr.BuyerLogin) AS Kupujacy,
            acr.CreatedAt, acr.CarrierName, 
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
            if (_currentFilterButton == btnFilterOczekujace)
            {
                conditions.Add("acr.StatusAllegro = 'DELIVERED'");
                conditions.Add("s2.Nazwa = 'Oczekuje na przyjęcie'");
            }
            else if (_currentFilterButton == btnFilterOczekujeNaDecyzje)
            {
                conditions.Add("s2.Nazwa = 'Oczekuje na decyzję handlowca'");
            }
            else if (_currentFilterButton == btnFilterPoDecyzji)
            {
                conditions.Add("s2.Nazwa = 'Zakończony'");
            }
            else if (_currentFilterButton == btnFilterWDrodze)
            {
                conditions.Add("acr.StatusAllegro = 'IN_TRANSIT'");
            }
            string allegroStatusFilter = comboFilterStatusAllegro.SelectedItem?.ToString();
            if (!string.IsNullOrEmpty(allegroStatusFilter) && allegroStatusFilter != "Wszystkie")
            {
                conditions.Add("acr.StatusAllegro = @statusAllegro");
                parameters.Add(new MySqlParameter("@statusAllegro", TranslateStatusToApi(allegroStatusFilter)));
            }
            if (!string.IsNullOrWhiteSpace(txtSearch.Text))
            {
                conditions.Add(@"(
            acr.ReferenceNumber LIKE @search OR
            acr.BuyerLogin      LIKE @search OR
            acr.Waybill         LIKE @search OR
            CONCAT(acr.Delivery_FirstName, ' ', acr.Delivery_LastName) LIKE @search OR
            CONCAT(acr.Buyer_FirstName, ' ', acr.Buyer_LastName) LIKE @search OR
            acr.ProductName LIKE @search
        )");
                parameters.Add(new MySqlParameter("@search", $"%{txtSearch.Text}%"));
            }
            if (conditions.Any())
            {
                queryBuilder.Append(" WHERE ").Append(string.Join(" AND ", conditions));
            }
            queryBuilder.Append(" ORDER BY acr.CreatedAt DESC");
            var dt = await _dbServiceMagazyn.GetDataTableAsync(queryBuilder.ToString(), parameters.ToArray());
            dt.Columns.Add("StatusAllegroPL", typeof(string));
            foreach (DataRow row in dt.Rows)
            {
                row["StatusAllegroPL"] = TranslateStatus(row["StatusAllegro"].ToString());
            }
            dgvReturns.DataSource = dt;
            FormatReturnsGrid();
            await UpdateFilterCountsAsync();
            lblTotalCount.Text = $"Wyświetlono: {dt.Rows.Count} zwrotów";
        }
        private void SetActiveFilterButton(Button activeButton)
        {
            foreach (Control c in panelFiltryButtons.Controls)
            {
                if (c is Button btn)
                {
                    btn.Font = new Font("Segoe UI", 9F, FontStyle.Regular);
                    btn.BackColor = SystemColors.Control;
                }
            }
            activeButton.Font = new Font(activeButton.Font, FontStyle.Bold);
            activeButton.BackColor = Color.LightSteelBlue;
            _currentFilterButton = activeButton;
        }

        private void FilterButton_Click(object sender, EventArgs e)
        {
            if (!(sender is Button clickedButton)) return;
            if (clickedButton == _currentFilterButton) return;
            SetActiveFilterButton(clickedButton);
            LoadReturnsFromDbAsync().FireAndForgetSafe(this.FindForm());
        }

        private void dgvReturns_DataBindingComplete(object sender, DataGridViewBindingCompleteEventArgs e)
        {
            foreach (DataGridViewRow row in dgvReturns.Rows)
            {
                string status = row.Cells["StatusAllegro"].Value?.ToString();
                if (status == "IN_TRANSIT")
                    row.DefaultCellStyle.BackColor = Color.LightBlue;
                else if (status == "DELIVERED")
                    row.DefaultCellStyle.BackColor = Color.LightGreen;
            }
        }

        private void FormatReturnsGrid()
        {
            if (dgvReturns.DataSource == null) return;
            var columnsConfig = new Dictionary<string, (string Header, int Width, bool Visible, DataGridViewAutoSizeColumnMode AutoSizeMode)>
            {
                { "Id",                 ("ID", 0, false, DataGridViewAutoSizeColumnMode.None) },
                { "ReferenceNumber",    ("Numer Zwrotu", 150, true, DataGridViewAutoSizeColumnMode.None) },
                { "Waybill",            ("Numer Listu", 200, true, DataGridViewAutoSizeColumnMode.None) },
                { "StatusAllegro",      ("Status Original", 0, false, DataGridViewAutoSizeColumnMode.None) },
                { "StatusAllegroPL",    ("Status Allegro", 140, true, DataGridViewAutoSizeColumnMode.None) },
                { "Kupujacy",           ("Kupujący", 0, true, DataGridViewAutoSizeColumnMode.Fill) },
                { "CreatedAt",          ("Data Utworzenia", 150, true, DataGridViewAutoSizeColumnMode.None) },
                { "CarrierName",        ("Przewoźnik", 120, true, DataGridViewAutoSizeColumnMode.None) },
                { "StanProduktu",       ("Stan Produktu", 150, true, DataGridViewAutoSizeColumnMode.None) },
                { "StatusWewnetrzny",   ("Status Wewnętrzny", 150, true, DataGridViewAutoSizeColumnMode.None) },
                { "DecyzjaHandlowca",   ("Decyzja Handlowca", 150, true, DataGridViewAutoSizeColumnMode.None) }
            };
            foreach (var colConfig in columnsConfig)
            {
                if (dgvReturns.Columns.Contains(colConfig.Key))
                {
                    var col = dgvReturns.Columns[colConfig.Key];
                    col.HeaderText = colConfig.Value.Header;
                    col.Width = colConfig.Value.Width;
                    col.Visible = colConfig.Value.Visible;
                    col.AutoSizeMode = colConfig.Value.AutoSizeMode;
                }
            }
        }

        private async Task UpdateFilterCountsAsync()
        {
            string baseQuery = "SELECT COUNT(*) FROM AllegroCustomerReturns acr LEFT JOIN Statusy s ON acr.StatusWewnetrznyId = s.Id";
            var countOczekujace = await _dbServiceMagazyn.ExecuteScalarAsync($"{baseQuery} WHERE s.Nazwa = 'Oczekuje na przyjęcie'");
            var countNaDecyzje = await _dbServiceMagazyn.ExecuteScalarAsync($"{baseQuery} WHERE s.Nazwa = 'Oczekuje na decyzję handlowca'");
            var countPoDecyzji = await _dbServiceMagazyn.ExecuteScalarAsync($"{baseQuery} WHERE s.Nazwa = 'Zakończony'");
            var countWDrodze = await _dbServiceMagazyn.ExecuteScalarAsync("SELECT COUNT(*) FROM AllegroCustomerReturns WHERE StatusAllegro = 'IN_TRANSIT'");
            var countWszystkie = await _dbServiceMagazyn.ExecuteScalarAsync("SELECT COUNT(*) FROM AllegroCustomerReturns");
            btnFilterOczekujace.Text = $"Oczekuje na przyjęcie ({countOczekujace})";
            btnFilterOczekujeNaDecyzje.Text = $"Oczekuje na decyzję ({countNaDecyzje})";
            btnFilterPoDecyzji.Text = $"Po decyzji ({countPoDecyzji})";
            btnFilterWDrodze.Text = $"W drodze do nas ({countWDrodze})";
            btnFilterWszystkie.Text = $"Wszystkie ({countWszystkie})";
        }
        #endregion

        #region Obsługa skanera i otwieranie szczegółów
        private async void TxtScannerInput_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode != Keys.Enter) return;
            e.SuppressKeyPress = true;
            await Task.Delay(50);

            string scannedText = txtScannerInput.Text.Trim();
            if (string.IsNullOrEmpty(scannedText)) return;

            string coreWaybill = ExtractCoreWaybill(scannedText);

            // Sprawdzamy, czy w aktualnie załadowanych danych już jest szukany zwrot.
            var foundRow = FindRowInGrid(dgvReturns, coreWaybill);

            // Jeśli nie znaleziono, to przełączamy filtr na "Wszystkie" i odświeżamy dane.
            if (foundRow == null)
            {
                // Sprawdzamy, czy filtr nie jest już ustawiony na "Wszystkie"
                if (_currentFilterButton != btnFilterWszystkie)
                {
                    // Przełączamy filtr
                    SetActiveFilterButton(btnFilterWszystkie);
                    // Wywołujemy LoadReturnsFromDbAsync, aby załadować wszystkie dane
                    await LoadReturnsFromDbAsync();
                }

                // Ponownie szukamy w odświeżonej (teraz pełnej) siatce danych
                foundRow = FindRowInGrid(dgvReturns, coreWaybill);
            }

            txtScannerInput.Clear();

            if (foundRow != null)
            {
                int returnId = Convert.ToInt32(foundRow["Id"]);
                string statusWewnetrzny = foundRow["StatusWewnetrzny"]?.ToString();
                OpenReturnForm(returnId, statusWewnetrzny);
            }
            else
            {
                var result = MessageBox.Show(
                    $"Nie znaleziono zwrotu dla numeru listu: {coreWaybill}.\n\nCzy chcesz dodać nowy zwrot ręcznie?",
                    "Nie znaleziono",
                    MessageBoxButtons.YesNo,
                    MessageBoxIcon.Question);

                if (result == DialogResult.Yes)
                {
                    btnDodajRecznie_Click(null, null);
                }
            }
        }

        private DataRow FindRowInGrid(DataGridView dgv, string searchText)
        {
            var dt = dgv.DataSource as DataTable;
            if (dt == null) return null;

            // Używamy AsEnumerable dla wydajniejszego wyszukiwania w pamięci
            return dt.AsEnumerable()
                .FirstOrDefault(row =>
                    (row["Waybill"]?.ToString()?.Contains(searchText) ?? false) ||
                    (row["ReferenceNumber"]?.ToString()?.Equals(searchText, StringComparison.OrdinalIgnoreCase) ?? false)
                );
        }

        private string ExtractCoreWaybill(string fullScannedText)
        {
            if (string.IsNullOrWhiteSpace(fullScannedText))
                return string.Empty;

            // Specjalna obsługa dla skanerów DPD (wzorzec: % + 7 znaków + 14-znakowy numer listu + reszta)
            var dpdMatch = Regex.Match(fullScannedText, @"^%.{7}([a-zA-Z0-9]{14})");
            if (dpdMatch.Success)
            {
                return dpdMatch.Groups[1].Value;
            }

            // Ogólne wyszukiwanie długich ciągów alfanumerycznych (dla innych przewoźników)
            var genericMatch = Regex.Match(fullScannedText, @"[a-zA-Z0-9]{10,}");
            if (genericMatch.Success)
            {
                return genericMatch.Value;
            }

            // Ostateczne czyszczenie, jeśli żaden wzorzec nie pasuje
            return Regex.Replace(fullScannedText, @"[^a-zA-Z0-9]", "");
        }

        private void OpenReturnForm(int returnId, string statusWewnetrzny)
        {
            Form formToShow;
            if (statusWewnetrzny == "Zakończony" || statusWewnetrzny == "Archiwalny")
            {
                formToShow = new FormPodsumowanieZwrotu(returnId);
            }
            else
            {
                formToShow = new FormZwrotSzczegoly(returnId, _fullName);
            }
            using (formToShow)
            {
                if (formToShow.ShowDialog(this.FindForm()) == DialogResult.OK)
                {
                    LoadReturnsFromDbAsync().FireAndForgetSafe(this.FindForm());
                    LoadDziennikAsync().FireAndForgetSafe(this.FindForm());
                }
            }
        }

        private void dgvReturns_CellDoubleClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex < 0) return;
            var row = dgvReturns.Rows[e.RowIndex];
            int returnId = Convert.ToInt32(row.Cells["Id"].Value);
            string statusWewnetrzny = row.Cells["StatusWewnetrzny"].Value?.ToString();
            OpenReturnForm(returnId, statusWewnetrzny);
        }
        #endregion

        #region Dodanie ręczne, auto-odświeżanie, dziennik
        private async void btnDodajRecznie_Click(object sender, EventArgs e)
        {
            using (var form = new FormDodajZwrotReczny())
            {
                if (form.ShowDialog() == DialogResult.OK)
                {
                    await LogToDziennikAsync("Dodano nowy zwrot ręcznie.", null);
                    await LoadDataAsync();
                }
            }
        }

        private async void SyncTimer_Tick(object sender, EventArgs e)
        {
            try
            {
                var result = await _dbServiceMagazyn.ExecuteScalarAsync(@"
                    SELECT COUNT(*)
                    FROM AllegroCustomerReturns acr
                    LEFT JOIN Statusy s ON acr.StatusWewnetrznyId = s.Id
                    WHERE s.Nazwa = 'Oczekuje na przyjęcie'
                      AND acr.StatusAllegro = 'DELIVERED'
                ");
                int newDeliveredCount = Convert.ToInt32(result);
                if (newDeliveredCount > 0)
                {
                    if (_currentFilterButton == btnFilterOczekujace)
                    {
                        await LoadDataAsync();
                    }
                    else
                    {
                        await UpdateFilterCountsAsync();
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Błąd automatycznej synchronizacji: {ex.Message}");
            }
        }

        private async Task LoadDziennikAsync()
        {
            string query = "SELECT Data, Uzytkownik, Akcja FROM MagazynDziennik ORDER BY Id DESC LIMIT 100";
            var dt = await _dbServiceMagazyn.GetDataTableAsync(query);
            dataGridViewChangeLog.DataSource = dt;
            FormatDziennikGrid();
        }

        private void FormatDziennikGrid()
        {
            if (dataGridViewChangeLog.DataSource == null) return;
            dataGridViewChangeLog.Columns["Data"].Width = 150;
            dataGridViewChangeLog.Columns["Uzytkownik"].Width = 120;
            dataGridViewChangeLog.Columns["Akcja"].AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;
        }

        private async Task LogToDziennikAsync(string akcja, int? returnId)
        {
            await _dbServiceMagazyn.ExecuteNonQueryAsync(
                "INSERT INTO MagazynDziennik (Data, Uzytkownik, Akcja, DotyczyZwrotuId) VALUES (@data, @uzytkownik, @akcja, @id)",
                new MySqlParameter("@data", DateTime.Now),
                new MySqlParameter("@uzytkownik", _fullName),
                new MySqlParameter("@akcja", akcja),
                new MySqlParameter("@id", (object)returnId ?? DBNull.Value)
            );
        }
        #endregion

        #region Mapowania statusów Allegro EN <-> PL
        private string TranslateStatus(string status)
        {
            switch (status)
            {
                case "DELIVERED": return "Dostarczono";
                case "IN_TRANSIT": return "W drodze";
                case "READY_FOR_PICKUP": return "Gotowy do odbioru";
                case "CREATED": return "Utworzono";
                case "COMMISSION_REFUNDED": return "Zwrócono prowizję";
                default: return status;
            }
        }

        private string TranslateStatusToApi(string statusPL)
        {
            switch (statusPL)
            {
                case "Dostarczono": return "DELIVERED";
                case "W drodze": return "IN_TRANSIT";
                case "Gotowy do odbioru": return "READY_FOR_PICKUP";
                case "Utworzono": return "CREATED";
                case "Zwrócono prowizję": return "COMMISSION_REFUNDED";
                default: return statusPL;
            }
        }
        #endregion
    }

    public static class TaskExtensions
    {
        public static async void FireAndForgetSafe(this Task task, IWin32Window owner)
        {
            try
            {
                await task;
            }
            catch (Exception ex)
            {
                MessageBox.Show(owner, $"Wystąpił nieoczekiwany błąd: {ex.Message}", "Błąd krytyczny", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
    }
}
