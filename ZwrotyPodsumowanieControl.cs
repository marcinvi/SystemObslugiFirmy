// === ZwrotyPodsumowanieControl.cs (lista w stylu HandlowiecControl; super podsumowanie) ===
using System;
using System.Collections.Generic;
using System.Data;
using MySql.Data.MySqlClient;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Reklamacje_Dane;

namespace Reklamacje_Dane
{
    public partial class ZwrotyPodsumowanieControl : UserControl
    {
        private readonly DatabaseService _dbMagazyn;
        private readonly DatabaseService _dbBaza;

        private readonly Timer _searchDebounce = new Timer();
        private readonly Timer _autoRefresh = new Timer();
        private Button _activeFilterButton;

        private List<ComboItem> _handlowcy = new List<ComboItem>();
        private readonly Dictionary<long, string> _usersById = new Dictionary<long, string>();
        private List<string> _statusy = new List<string>();

        public ZwrotyPodsumowanieControl()
        {
            InitializeComponent();

            _dbMagazyn = new DatabaseService(MagazynDatabaseHelper.GetConnectionString());
            _dbBaza = new DatabaseService(DatabaseHelper.GetConnectionString());

            _searchDebounce.Interval = 300;
            _searchDebounce.Tick += (s, e) => { _searchDebounce.Stop(); _ = LoadReturnsAsync(); };

            _autoRefresh.Interval = 60000;
            _autoRefresh.Tick += async (s, e) => await LoadReturnsAsync();
            _autoRefresh.Start();

            AttachHandlers();
        }

        private void AttachHandlers()
        {
            txtSearch.TextChanged += (s, e) => _searchDebounce.Start();

            btnFilterWszystkie.Click += FilterButton_Click;
            btnFilterDoDecyzji.Click += FilterButton_Click;
            btnFilterZakonczone.Click += FilterButton_Click;

            btnExportCsv.Click += async (s, e) => await ExportCsvAsync();
            btnRefresh.Click += async (s, e) => await LoadReturnsAsync();

            cmbHandlowiec.SelectedIndexChanged += async (s, e) => await LoadReturnsAsync();
            cmbStatus.SelectedIndexChanged += async (s, e) => await LoadReturnsAsync();

            dtpFrom.ValueChanged += async (s, e) => await LoadReturnsAsync();
            dtpTo.ValueChanged += async (s, e) => await LoadReturnsAsync();
            chkDateFrom.CheckedChanged += async (s, e) => await LoadReturnsAsync();
            chkDateTo.CheckedChanged += async (s, e) => await LoadReturnsAsync();

            dgvReturns.CellDoubleClick += DgvReturns_CellDoubleClick;
        }

        private async void ZwrotyPodsumowanieControl_Load(object sender, EventArgs e)
        {
            ApplyGridStyle();
            SetActiveFilterButton(btnFilterWszystkie);
            await LoadLookupsAsync();
            await LoadReturnsAsync();
        }

        private void ApplyGridStyle()
        {
            try { dgvReturns.EnableDoubleBuffer(); } catch { }
            dgvReturns.BorderStyle = BorderStyle.None;
            dgvReturns.BackgroundColor = Color.White;
            dgvReturns.RowHeadersVisible = false;
            dgvReturns.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
            dgvReturns.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            dgvReturns.MultiSelect = false;
            dgvReturns.AllowUserToAddRows = false;
            dgvReturns.AllowUserToDeleteRows = false;
            dgvReturns.ReadOnly = true;

            var header = dgvReturns.ColumnHeadersDefaultCellStyle;
            header.BackColor = Color.FromArgb(68, 114, 196);
            header.ForeColor = Color.White;
            header.Font = new Font("Segoe UI", 9F, FontStyle.Bold);
            dgvReturns.EnableHeadersVisualStyles = false;

            dgvReturns.AlternatingRowsDefaultCellStyle.BackColor = Color.FromArgb(240, 240, 240);
            dgvReturns.DefaultCellStyle.SelectionBackColor = Color.FromArgb(33, 150, 243);
            dgvReturns.DefaultCellStyle.SelectionForeColor = Color.Black;
        }

        private void SetActiveFilterButton(Button b)
        {
            if (_activeFilterButton != null)
                _activeFilterButton.BackColor = Color.White;

            _activeFilterButton = b;
            _activeFilterButton.BackColor = Color.FromArgb(226, 239, 255);
        }

        private async Task LoadLookupsAsync()
        {
            var dtIds = await _dbMagazyn.GetDataTableAsync(@"
                SELECT DISTINCT HandlowiecOpiekunId AS Id
                FROM AllegroCustomerReturns
                WHERE HandlowiecOpiekunId IS NOT NULL AND HandlowiecOpiekunId <> ''");

            var ids = new List<long>();
            foreach (DataRow r in dtIds.Rows)
            {
                var id = SafeGetLong(r["Id"]);
                if (id > 0) ids.Add(id);
            }

            _usersById.Clear();
            _handlowcy = new List<ComboItem> { new ComboItem { Id = -1, Nazwa = "— wszyscy —" } };

            if (ids.Count > 0)
            {
                try
                {
                    var ph = string.Join(",", ids.Select((_, i) => "@p" + i));
                    var pars = ids.Select((v, i) => new MySqlParameter("@p" + i, v)).ToArray();

                    var dtUsers = await _dbBaza.GetDataTableAsync($@"
                        SELECT Id, `Nazwa Wyświetlana` AS Nazwa
                        FROM Uzytkownicy
                        WHERE Id IN ({ph})
                        ORDER BY Nazwa", pars);

                    foreach (DataRow r in dtUsers.Rows)
                    {
                        long id = SafeGetLong(r["Id"]);
                        string nazwa = SafeGetString(r["Nazwa"]);
                        if (id > 0)
                        {
                            _usersById[id] = nazwa;
                            _handlowcy.Add(new ComboItem { Id = id, Nazwa = nazwa });
                        }
                    }

                    cmbHandlowiec.Enabled = true; lblHandlowiec.Enabled = true;
                }
                catch (MySqlException ex) when (ex.Message.Contains("no such table: Uzytkownicy"))
                {
                    cmbHandlowiec.Enabled = false; lblHandlowiec.Enabled = false;
                }
            }
            else
            {
                cmbHandlowiec.Enabled = false; lblHandlowiec.Enabled = false;
            }

            cmbHandlowiec.DataSource = _handlowcy;
            cmbHandlowiec.DisplayMember = "Nazwa";
            cmbHandlowiec.ValueMember = "Id";
            cmbHandlowiec.SelectedIndex = 0;

            var dtS = await _dbMagazyn.GetDataTableAsync(@"
                SELECT DISTINCT s.Nazwa
                FROM AllegroCustomerReturns acr
                LEFT JOIN Statusy s ON s.Id = acr.StatusWewnetrznyId
                WHERE s.Nazwa IS NOT NULL
                ORDER BY s.Nazwa");
            _statusy = dtS.AsEnumerable().Select(r => SafeGetString(r["Nazwa"])).ToList();
            _statusy.Insert(0, "— wszystkie —");
            cmbStatus.DataSource = _statusy;
            cmbStatus.SelectedIndex = 0;

            dtpFrom.Value = DateTime.Today.AddDays(-30);
            dtpTo.Value = DateTime.Today;
        }

        private async Task LoadReturnsAsync()
        {
            try
            {
                var sb = new StringBuilder();
                sb.AppendLine(@"
                    SELECT
                        acr.Id,
                        acr.ReferenceNumber,
                        acr.ProductName,
                        acr.HandlowiecOpiekunId,
                        acr.PrzyjetyPrzezId,
                        acr.DataPrzyjecia,
                        acr.UwagiMagazynu,
                        IFNULL(s2.Nazwa, 'Nieznany') AS StatusWew,
                        IFNULL(s3.Nazwa, 'Nieznany') AS DecyzjaHandl
                    FROM AllegroCustomerReturns acr
                    LEFT JOIN Statusy s2 ON s2.Id = acr.StatusWewnetrznyId
                    LEFT JOIN Statusy s3 ON s3.Id = acr.DecyzjaHandlowcaId
                ");

                var where = new List<string>();
                var p = new List<MySqlParameter>();

                if (_activeFilterButton == btnFilterDoDecyzji)
                    where.Add("s2.Nazwa = 'Oczekuje na decyzję handlowca'");
                else if (_activeFilterButton == btnFilterZakonczone)
                    where.Add("s2.Nazwa = 'Zakończony'");

                if (cmbHandlowiec.Enabled && cmbHandlowiec.SelectedItem is ComboItem ci && ci.Id > 0)
                {
                    where.Add("acr.HandlowiecOpiekunId = @hid");
                    p.Add(new MySqlParameter("@hid", ci.Id));
                }

                if (cmbStatus.SelectedIndex > 0)
                {
                    where.Add("s2.Nazwa = @status");
                    p.Add(new MySqlParameter("@status", cmbStatus.SelectedItem.ToString()));
                }

                if (chkDateFrom.Checked)
                {
                    where.Add("date(acr.CreatedAt) >= date(@from)");
                    p.Add(new MySqlParameter("@from", dtpFrom.Value.Date.ToString("yyyy-MM-dd")));
                }
                if (chkDateTo.Checked)
                {
                    where.Add("date(acr.CreatedAt) <= date(@to)");
                    p.Add(new MySqlParameter("@to", dtpTo.Value.Date.ToString("yyyy-MM-dd")));
                }

                if (!string.IsNullOrWhiteSpace(txtSearch.Text))
                {
                    where.Add("(acr.ReferenceNumber LIKE @q OR acr.ProductName LIKE @q)");
                    p.Add(new MySqlParameter("@q", $"%{txtSearch.Text}%"));
                }

                if (where.Any())
                    sb.Append(" WHERE ").Append(string.Join(" AND ", where));
                sb.Append(" ORDER BY acr.CreatedAt DESC");

                var dt = await _dbMagazyn.GetDataTableAsync(sb.ToString(), p.ToArray());

                var view = BuildDisplayTable();
                foreach (DataRow r in dt.Rows)
                {
                    long id = SafeGetLong(r["Id"]);
                    string nr = SafeGetString(r["ReferenceNumber"]);
                    string produkt = SafeGetString(r["ProductName"]);

                    string ktoPrzyjal = "nie przyjęte";
                    DateTime? dataPrzyj = SafeGetDate(r["DataPrzyjecia"]);
                    long przyjalId = SafeGetLong(r["PrzyjetyPrzezId"]);
                    if (dataPrzyj.HasValue)
                    {
                        if (przyjalId > 0 && _usersById.TryGetValue(przyjalId, out var nm)) ktoPrzyjal = nm;
                        else ktoPrzyjal = "(przyjęte)";
                    }

                    string ktoPodjal = "";
                    string uwagiHand = "";

                    var ldt = await _dbMagazyn.GetDataTableAsync(
                        @"SELECT Uzytkownik, Tresc FROM ZwrotDzialania 
                          WHERE ZwrotId = @id AND Tresc LIKE 'Podjęto decyzję:%' 
                          ORDER BY Data DESC LIMIT 1",
                        new MySqlParameter("@id", id)
                    );
                    if (ldt.Rows.Count > 0)
                    {
                        ktoPodjal = SafeGetString(ldt.Rows[0]["Uzytkownik"]);
                        uwagiHand = ExtractKomentarz(SafeGetString(ldt.Rows[0]["Tresc"]));
                    }

                    string jakaDecyzja = SafeGetString(r["DecyzjaHandl"]);
                    string uwagiMag = SafeGetString(r["UwagiMagazynu"]);
                    string status = SafeGetString(r["StatusWew"]);

                    view.Rows.Add(id, nr, produkt, ktoPrzyjal, ktoPodjal, jakaDecyzja, uwagiMag, uwagiHand, status);
                }

                dgvReturns.DataSource = view;
                FormatGridColumns();
                lblCount.Text = $"Wyświetlono: {view.Rows.Count}";

                await UpdateCountersAsync();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Błąd podczas ładowania zwrotów: " + ex.Message, "Błąd",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private async Task UpdateCountersAsync()
        {
            var whereParts = new List<string>();
            var p = new List<MySqlParameter>();

            if (cmbHandlowiec.Enabled && cmbHandlowiec.SelectedItem is ComboItem ci && ci.Id > 0)
            {
                whereParts.Add("acr.HandlowiecOpiekunId = @hid");
                p.Add(new MySqlParameter("@hid", ci.Id));
            }
            if (chkDateFrom.Checked)
            {
                whereParts.Add("date(acr.CreatedAt) >= date(@from)");
                p.Add(new MySqlParameter("@from", dtpFrom.Value.Date.ToString("yyyy-MM-dd")));
            }
            if (chkDateTo.Checked)
            {
                whereParts.Add("date(acr.CreatedAt) <= date(@to)");
                p.Add(new MySqlParameter("@to", dtpTo.Value.Date.ToString("yyyy-MM-dd")));
            }
            if (!string.IsNullOrWhiteSpace(txtSearch.Text))
            {
                whereParts.Add("(acr.ReferenceNumber LIKE @q OR acr.ProductName LIKE @q)");
                p.Add(new MySqlParameter("@q", $"%{txtSearch.Text}%"));
            }

            string whereSql = whereParts.Any() ? " WHERE " + string.Join(" AND ", whereParts) : string.Empty;

            var dt = await _dbMagazyn.GetDataTableAsync($@"
                SELECT
                    COUNT(acr.Id) AS Total,
                    SUM(CASE WHEN IFNULL(s2.Nazwa, '') = 'Oczekuje na decyzję handlowca' THEN 1 ELSE 0 END) AS DoDecyzji,
                    SUM(CASE WHEN IFNULL(s2.Nazwa, '') = 'Zakończony' THEN 1 ELSE 0 END) AS Zakonczone
                FROM AllegroCustomerReturns acr
                LEFT JOIN Statusy s2 ON s2.Id = acr.StatusWewnetrznyId
                LEFT JOIN Statusy s3 ON s3.Id = acr.DecyzjaHandlowcaId
                {whereSql}
            ", p.ToArray());

            if (dt.Rows.Count > 0)
            {
                lblTotal.Text = "Razem: " + SafeGetInt(dt.Rows[0]["Total"]);
                lblDoDecyzji.Text = "Do decyzji: " + SafeGetInt(dt.Rows[0]["DoDecyzji"]);
                lblZakonczone.Text = "Zakończone: " + SafeGetInt(dt.Rows[0]["Zakonczone"]);
            }
        }

        private static string ExtractKomentarz(string tresc)
        {
            if (string.IsNullOrWhiteSpace(tresc)) return "";
            var idx = tresc.IndexOf("Komentarz:", StringComparison.OrdinalIgnoreCase);
            if (idx < 0) return "";
            return tresc.Substring(idx + "Komentarz:".Length).Trim();
        }

        private DataTable BuildDisplayTable()
        {
            var t = new DataTable();
            t.Columns.Add("Id", typeof(long));
            t.Columns.Add("Numer Zwrotu", typeof(string));
            t.Columns.Add("Produkt", typeof(string));
            t.Columns.Add("Kto przyjął fizycznie", typeof(string));
            t.Columns.Add("Kto podjął decyzję", typeof(string));
            t.Columns.Add("Jaka decyzja", typeof(string));
            t.Columns.Add("Uwagi Magazynu", typeof(string));
            t.Columns.Add("Uwagi Handlowca", typeof(string));
            t.Columns.Add("Jaki status", typeof(string));
            return t;
        }

        private DataGridViewColumn ColByNameOrHeader(string nameOrHeader)
            => dgvReturns.Columns.Cast<DataGridViewColumn>()
               .FirstOrDefault(c => c.Name == nameOrHeader || c.HeaderText == nameOrHeader);

        private void FormatGridColumns()
        {
            var colId = ColByNameOrHeader("Id");
            if (colId != null) colId.Visible = false;

            foreach (var header in new[] {
                "Numer Zwrotu","Produkt","Kto przyjął fizycznie","Kto podjął decyzję",
                "Jaka decyzja","Uwagi Magazynu","Uwagi Handlowca","Jaki status"})
            {
                var c = ColByNameOrHeader(header);
                if (c != null) c.AutoSizeMode = DataGridViewAutoSizeColumnMode.DisplayedCells;
            }
        }

        private async Task ExportCsvAsync()
        {
            if (dgvReturns.DataSource is DataTable dt)
            {
                using (var sfd = new SaveFileDialog())
                {
                    sfd.Filter = "CSV (*.csv)|*.csv";
                    sfd.FileName = $"zwroty_podsumowanie_{DateTime.Now:yyyyMMdd_HHmm}.csv";
                    if (sfd.ShowDialog(this.FindForm()) == DialogResult.OK)
                    {
                        try
                        {
                            using (var w = new System.IO.StreamWriter(sfd.FileName, false, Encoding.UTF8))
                            {
                                for (int i = 0; i < dt.Columns.Count; i++)
                                {
                                    if (i > 0) w.Write(';');
                                    w.Write(dt.Columns[i].ColumnName.Replace(';', ','));
                                }
                                w.WriteLine();
                                foreach (DataRow row in dt.Rows)
                                {
                                    for (int i = 0; i < dt.Columns.Count; i++)
                                    {
                                        if (i > 0) w.Write(';');
                                        var val = (row[i]?.ToString() ?? "").Replace(';', ',').Replace("\r", " ").Replace("\n", " ");
                                        w.Write(val);
                                    }
                                    w.WriteLine();
                                }
                            }
                            MessageBox.Show("Wyeksportowano CSV.", "OK", MessageBoxButtons.OK, MessageBoxIcon.Information);
                        }
                        catch (Exception ex)
                        {
                            MessageBox.Show("Błąd eksportu: " + ex.Message, "Błąd", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        }
                    }
                }
            }
        }

        private void FilterButton_Click(object sender, EventArgs e)
        {
            if (sender is Button b)
            {
                if (b != _activeFilterButton) SetActiveFilterButton(b);
                _ = LoadReturnsAsync();
            }
        }

        private void DgvReturns_CellDoubleClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex < 0) return;
            var row = dgvReturns.Rows[e.RowIndex];
            var colId = ColByNameOrHeader("Id");
            if (colId == null) return;

            if (int.TryParse((row.Cells[colId.Index].Value ?? "").ToString(), out int returnId) && returnId > 0)
            {
                using (var f = new FormZwrotPodsumowanie(returnId))
                    f.ShowDialog(this.FindForm());
            }
        }

        private static long SafeGetLong(object o)
        {
            if (o == null || o == DBNull.Value) return 0;
            if (o is long l) return l;
            if (o is int i) return i;
            if (long.TryParse(o.ToString(), out var p)) return p;
            return 0;
        }
        private static int SafeGetInt(object o)
        {
            if (o == null || o == DBNull.Value) return 0;
            if (o is int i) return i;
            if (o is long l) return (int)l;
            if (int.TryParse(o.ToString(), out var p)) return p;
            return 0;
        }
        private static string SafeGetString(object o) => (o == null || o == DBNull.Value) ? "" : o.ToString();
        private static DateTime? SafeGetDate(object o) { if (o == null || o == DBNull.Value) return null; if (o is DateTime d) return d; if (DateTime.TryParse(o.ToString(), out var p)) return p; return null; }

        private class ComboItem
        {
            public long Id { get; set; }
            public string Nazwa { get; set; }
            public override string ToString() => Nazwa;
        }
    }
}