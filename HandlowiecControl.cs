// Plik: HandlowiecControl.cs (WERSJA Z POPRAWIONYM KONSTRUKTOREM)
// Opis: Zmieniono konstruktor, aby przyjmował 'string fullName' zamiast 'int userId',
//       w celu zachowania spójności z resztą aplikacji i naprawy błędu CS1503.
//       ID użytkownika jest teraz pobierane automatycznie wewnątrz kontrolki.

using System;
using System.Collections.Generic;
using System.Data;
using MySql.Data.MySqlClient;
using System.Drawing;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Reklamacje_Dane
{
    public partial class HandlowiecControl : UserControl
    {
        private readonly string _fullName;
        private int _userId; // Zmieniono na pole, które zostanie wypełnione w Load
        private readonly DatabaseService _dbServiceBaza;
        private readonly DatabaseService _dbServiceMagazyn;
        private Timer _searchDebounceTimer;
        private Button _currentFilterButton;
        private KomunikatorControl _komunikatorControl;

        // ########## POPRAWKA - Zmieniono konstruktor ##########
        public HandlowiecControl(string fullName, string userRole) // userRole jest ignorowany, ale zachowuje spójność
        {
            InitializeComponent();
            _fullName = fullName;
            // _userId zostanie pobrane w metodzie Load
            _dbServiceBaza = new DatabaseService(DatabaseHelper.GetConnectionString());
            _dbServiceMagazyn = new DatabaseService(MagazynDatabaseHelper.GetConnectionString());
        }

        private async void HandlowiecControl_Load(object sender, EventArgs e)
        {
            // Najpierw pobierz ID użytkownika na podstawie jego nazwy
            bool success = await FetchUserIdAsync();
            if (!success)
            {
                MessageBox.Show("Nie udało się zidentyfikować użytkownika. Moduł Handlowca nie może zostać załadowany.", "Błąd krytyczny", MessageBoxButtons.OK, MessageBoxIcon.Error);
                this.Parent.Controls.Remove(this); // Usuń kontrolkę, jeśli nie można zidentyfikować użytkownika
                return;
            }

            InitializeUiElements();
            AttachEventHandlers();
            await LoadDataAsync();
        }

        // --- Nowa metoda do pobierania ID użytkownika ---
        private async Task<bool> FetchUserIdAsync()
        {
            try
            {
                var userIdObj = await _dbServiceBaza.ExecuteScalarAsync(
                    "SELECT Id FROM Uzytkownicy WHERE `Nazwa Wyświetlana` = @fullName",
                    new MySqlParameter("@fullName", _fullName)
                );

                if (userIdObj != null && userIdObj != DBNull.Value)
                {
                    _userId = Convert.ToInt32(userIdObj);
                    return true;
                }
                return false;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Błąd podczas pobierania ID użytkownika: {ex.Message}");
                return false;
            }
        }


        private void InitializeUiElements()
        {
            _currentFilterButton = btnFilterDoDecyzji;
            SetActiveFilterButton(btnFilterDoDecyzji);

            _searchDebounceTimer = new Timer { Interval = 300 };

            _komunikatorControl = new KomunikatorControl(_dbServiceBaza, _dbServiceMagazyn, _fullName, "Handlowiec")
            {
                Dock = DockStyle.Fill
            };
            this.panelKomunikator.Controls.Add(_komunikatorControl);
        }

        private void AttachEventHandlers()
        {
            _searchDebounceTimer.Tick += (s, e) =>
            {
                _searchDebounceTimer.Stop();
                LoadReturnsFromDbAsync().FireAndForgetSafe(this.FindForm());
            };
            txtSearch.TextChanged += (s, ev) => _searchDebounceTimer.Start();

            btnFilterDoDecyzji.Click += FilterButton_Click;
            btnFilterZakonczone.Click += FilterButton_Click;

            dgvReturns.CellDoubleClick += dgvReturns_CellDoubleClick;
            refreshIcon.Click += async (s, ev) => await LoadDataAsync();
        }

        private async Task LoadDataAsync()
        {
            this.Cursor = Cursors.WaitCursor;
            await LoadReturnsFromDbAsync();
            if (_komunikatorControl != null)
            {
                await _komunikatorControl.LoadMessagesAsync();
            }
            lblLastRefresh.Text = $"Ostatnie odświeżenie: {DateTime.Now:HH:mm:ss}";
            this.Cursor = Cursors.Default;
        }

        private async Task LoadReturnsFromDbAsync()
        {
            var buyerExpression = "COALESCE(acr.BuyerFullName, acr.BuyerLogin, 'Nieznany klient')";
            var queryBuilder = new System.Text.StringBuilder($@"
        SELECT
            acr.Id,
            acr.ReferenceNumber,
            acr.Waybill,
            {buyerExpression} AS Kupujacy,
            acr.ProductName,
            acr.CreatedAt,
            IFNULL(s1.Nazwa, 'Nieprzypisany') AS StanProduktu,
            IFNULL(s2.Nazwa, 'Nieprzypisany') AS StatusWewnetrzny,
            IFNULL(s3.Nazwa, 'Nieprzypisany') AS DecyzjaHandlowca,
            acr.IsManual
        FROM AllegroCustomerReturns acr
        LEFT JOIN Statusy s1 ON acr.StanProduktuId = s1.Id
        LEFT JOIN Statusy s2 ON acr.StatusWewnetrznyId = s2.Id
        LEFT JOIN Statusy s3 ON acr.DecyzjaHandlowcaId = s3.Id
    ");

            var conditions = new List<string>();
            var parameters = new List<MySqlParameter>();

            conditions.Add(@"(
        acr.HandlowiecOpiekunId = @userId 
        OR acr.Id IN (SELECT DotyczyZwrotuId FROM Wiadomosci WHERE OdbiorcaId = @userId)
    )");
            parameters.Add(new MySqlParameter("@userId", _userId));

            if (_currentFilterButton == btnFilterDoDecyzji)
            {
                conditions.Add("s2.Nazwa = 'Oczekuje na decyzję handlowca'");
            }
            else if (_currentFilterButton == btnFilterZakonczone)
            {
                conditions.Add("s2.Nazwa = 'Zakończony'");
            }

            if (!string.IsNullOrWhiteSpace(txtSearch.Text))
            {
                conditions.Add($@"(
            acr.ReferenceNumber LIKE @search OR
            acr.Waybill LIKE @search OR
            {buyerExpression} LIKE @search OR
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
            dgvReturns.DataSource = dt;
            FormatReturnsGrid();
            await UpdateFilterCountsAsync();
            lblTotalCount.Text = $"Wyświetlono: {dt.Rows.Count} spraw";
        }

        private void dgvReturns_CellDoubleClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex < 0) return;

            var row = dgvReturns.Rows[e.RowIndex];
            int returnId = Convert.ToInt32(row.Cells["Id"].Value);
            bool isManual = Convert.ToInt32(row.Cells["IsManual"].Value) == 1;

            Form formToShow;

            if (isManual)
            {
                formToShow = new FormHandlowiecSzczegolyReczny(returnId, _fullName);
            }
            else
            {
                formToShow = new FormHandlowiecSzczegoly(returnId, _fullName);
            }

            using (formToShow)
            {
                if (formToShow.ShowDialog(this.FindForm()) == DialogResult.OK)
                {
                    LoadDataAsync().FireAndForgetSafe(this.FindForm());
                }
            }
        }

        private async Task UpdateFilterCountsAsync()
        {
            string baseQuery = @"
                FROM AllegroCustomerReturns acr
                LEFT JOIN Statusy s ON acr.StatusWewnetrznyId = s.Id
                WHERE (acr.HandlowiecOpiekunId = @userId OR acr.Id IN (SELECT DotyczyZwrotuId FROM Wiadomosci WHERE OdbiorcaId = @userId))";

            var param = new MySqlParameter("@userId", _userId);

            var countDoDecyzji = await _dbServiceMagazyn.ExecuteScalarAsync($"SELECT COUNT(*) {baseQuery} AND s.Nazwa = 'Oczekuje na decyzję handlowca'", param);
            var countZakonczone = await _dbServiceMagazyn.ExecuteScalarAsync($"SELECT COUNT(*) {baseQuery} AND s.Nazwa = 'Zakończony'", param);

            btnFilterDoDecyzji.Text = $"Nowe sprawy ({countDoDecyzji})";
            btnFilterZakonczone.Text = $"Zakończone ({countZakonczone})";
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
            if (!(sender is Button clickedButton) || clickedButton == _currentFilterButton) return;
            SetActiveFilterButton(clickedButton);
            LoadReturnsFromDbAsync().FireAndForgetSafe(this.FindForm());
        }

        private void FormatReturnsGrid()
        {
            if (dgvReturns.DataSource == null) return;
            var columnsConfig = new Dictionary<string, (string Header, bool Visible, DataGridViewAutoSizeColumnMode AutoSizeMode)>
            {
                { "Id", ("ID", false, DataGridViewAutoSizeColumnMode.None) },
                { "ReferenceNumber", ("Numer Sprawy", true, DataGridViewAutoSizeColumnMode.AllCells) },
                { "Waybill", ("Numer Listu", true, DataGridViewAutoSizeColumnMode.AllCells) },
                { "Kupujacy", ("Klient", true, DataGridViewAutoSizeColumnMode.Fill) },
                { "ProductName", ("Zwracany produkt", true, DataGridViewAutoSizeColumnMode.Fill) },
                { "CreatedAt", ("Data utworzenia", true, DataGridViewAutoSizeColumnMode.AllCells) },
                { "StanProduktu", ("Stan produktu (Magazyn)", true, DataGridViewAutoSizeColumnMode.AllCells) },
                { "StatusWewnetrzny", ("Status sprawy", true, DataGridViewAutoSizeColumnMode.AllCells) },
                { "DecyzjaHandlowca", ("Twoja decyzja", true, DataGridViewAutoSizeColumnMode.AllCells) },
                { "IsManual", ("Ręczny", false, DataGridViewAutoSizeColumnMode.None) }
            };

            foreach (var colConfig in columnsConfig)
            {
                if (dgvReturns.Columns.Contains(colConfig.Key))
                {
                    var col = dgvReturns.Columns[colConfig.Key];
                    col.HeaderText = colConfig.Value.Header;
                    col.Visible = colConfig.Value.Visible;
                    col.AutoSizeMode = colConfig.Value.AutoSizeMode;
                }
            }
        }

        private void dgvReturns_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {

        }
    }
}
