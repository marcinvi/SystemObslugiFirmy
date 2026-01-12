using System;
using System.Drawing;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Data;
using MySql.Data.MySqlClient; // WAŻNE
using System.Collections.Generic;

namespace Reklamacje_Dane
{
    public partial class FormDpdTracking : Form
    {
        private DpdTrackingService _trackingService;
        private Microsoft.Web.WebView2.WinForms.WebView2 webView;
        private ContextMenuStrip _contextMenu;

        public FormDpdTracking()
        {
            InitializeComponent();
            InitializeContextMenu();
            this.Load += FormDpdTracking_Load;

            // Eventy
            btnCheckStatus.Click += btnCheckStatus_Click;
            btnSzukaj.Click += BtnSzukaj_Click;

            // Kliknięcie wiersza -> pokaż historię
            dgvPrzesylki.CellClick += Dgv_CellClick;
            dgvDzisiajDostarczone.CellClick += Dgv_CellClick;
            dgvDzisiajProblemy.CellClick += Dgv_CellClick;
            dgvArchiwum.CellClick += Dgv_CellClick;
        

            // Włącz sprawdzanie pisowni dla wszystkich TextBoxów
            EnableSpellCheckOnAllTextBoxes();
        }

        private void InitializeContextMenu()
        {
            _contextMenu = new ContextMenuStrip();
            _contextMenu.Items.Add("📄 Zobacz zgłoszenie", null, (s, e) => {
                var item = GetSelectedShipment();
                if (item != null) new Form2(item.NrZgloszenia).Show();
            });
            _contextMenu.Items.Add("🚚 Zamów ponownie", null, (s, e) => {
                var item = GetSelectedShipment();
                if (item != null) new Form5(item.NrZgloszenia).Show();
            });
            _contextMenu.Items.Add(new ToolStripSeparator());
            _contextMenu.Items.Add("✅ Usuń z monitorowania", null, async (s, e) => {
                var item = GetSelectedShipment();
                if (item != null && MessageBox.Show("Usunąć?", "Potwierdź", MessageBoxButtons.YesNo) == DialogResult.Yes)
                {
                    await _trackingService.MarkShipmentAsCompletedAsync(item.Id);
                    await LoadAllDataAsync();
                }
            });

            // Podpięcie do wszystkich gridów
            dgvPrzesylki.ContextMenuStrip = _contextMenu;
            dgvDzisiajDostarczone.ContextMenuStrip = _contextMenu;
            dgvDzisiajProblemy.ContextMenuStrip = _contextMenu;
            dgvArchiwum.ContextMenuStrip = _contextMenu;

            // Obsługa PPM (żeby zaznaczało wiersz)
            MouseEventHandler rclick = (s, e) => {
                var dgv = s as DataGridView;
                if (e.Button == MouseButtons.Right)
                {
                    var hti = dgv.HitTest(e.X, e.Y);
                    if (hti.RowIndex >= 0)
                    {
                        dgv.ClearSelection();
                        dgv.Rows[hti.RowIndex].Selected = true;
                        _contextMenu.Tag = dgv; // Zapamiętaj, który grid wywołał menu
                    }
                }
            };
            dgvPrzesylki.MouseDown += rclick;
            dgvDzisiajDostarczone.MouseDown += rclick;
            dgvDzisiajProblemy.MouseDown += rclick;
            dgvArchiwum.MouseDown += rclick;
        }

        private PrzesylkaViewModel GetSelectedShipment()
        {
            var dgv = _contextMenu.Tag as DataGridView;
            if (dgv != null && dgv.CurrentRow != null)
                return dgv.CurrentRow.DataBoundItem as PrzesylkaViewModel;
            return null;
        }

        private async void FormDpdTracking_Load(object sender, EventArgs e)
        {
            try
            {
                SetStatus("Inicjalizacja...");
                // Ukryte WebView do komunikacji z API DPD
                this.webView = new Microsoft.Web.WebView2.WinForms.WebView2 { Visible = false };
                this.Controls.Add(this.webView);
                await this.webView.EnsureCoreWebView2Async(null);

                _trackingService = new DpdTrackingService(this.webView);
                _trackingService.ProgressUpdated += msg => this.Invoke(new Action(() => SetStatus(msg)));

                await LoadAllDataAsync();
            }
            catch (Exception ex) { SetStatus("Błąd startu: " + ex.Message, true); }
        }

        private async Task LoadAllDataAsync()
        {
            // 1. Tab "W Drodze" (Aktywne)
            var active = await _trackingService.GetActiveShipmentsForDisplayAsync();
            dgvPrzesylki.DataSource = active;
            FormatGrid(dgvPrzesylki);

            // 2. Tab "Dzisiaj" (SQL z bazy)
            await LoadDailySummaryAsync();

            SetStatus($"Załadowano {active.Count} przesyłek w drodze.");
        }

        private async Task LoadDailySummaryAsync()
        {
            // Zapytanie o paczki, które miały dzisiaj status końcowy lub problemowy
            string today = DateTime.Now.ToString("yyyy-MM-dd");
            string queryBase = @"
                SELECT p.Id, p.NumerListu, p.NrZgloszenia, p.OstatniStatus,
                CONCAT(IFNULL(n.ImieNazwisko, ''), ' ', IFNULL(n.NazwaFirmy, '')) AS NazwaNadawcy,
                CONCAT(IFNULL(o.ImieNazwisko, ''), ' ', IFNULL(o.NazwaFirmy, '')) AS NazwaOdbiorcy
                FROM Przesylki p
                LEFT JOIN Klienci n ON p.NadawcaId = n.Id
                LEFT JOIN Klienci o ON p.OdbiorcaId = o.Id
                JOIN HistoriaPrzesylek h ON p.Id = h.PrzesylkaId
                WHERE DATE(h.DataStatusu) = CURDATE() ";

            // Dostarczone dzisiaj
            var delivered = await GetShipmentsFromSql(queryBase + " AND (h.OpisStatusu LIKE '%Doręczono%' OR h.OpisStatusu LIKE '%Zrealizowano%') GROUP BY p.Id");
            dgvDzisiajDostarczone.DataSource = delivered;
            FormatGrid(dgvDzisiajDostarczone);

            // Problemy dzisiaj
            var problems = await GetShipmentsFromSql(queryBase + " AND (h.OpisStatusu LIKE '%Błąd%' OR h.OpisStatusu LIKE '%Zwrot%' OR h.OpisStatusu LIKE '%Awizo%') GROUP BY p.Id");
            dgvDzisiajProblemy.DataSource = problems;
            FormatGrid(dgvDzisiajProblemy);
        }

        private async void BtnSzukaj_Click(object sender, EventArgs e)
        {
            try
            {
                btnSzukaj.Enabled = false;
                string whereClause = "WHERE date(p.DataNadania) BETWEEN date(@d1) AND date(@d2)";

                string filter = cmbStatus.SelectedItem?.ToString();
                if (filter == "Dostarczona") whereClause += " AND p.OstatniStatus LIKE '%Doręczono%'";
                else if (filter == "W Doręczeniu") whereClause += " AND p.OstatniStatus LIKE '%Wydanie%'";
                else if (filter == "Problem/Zwrot") whereClause += " AND (p.OstatniStatus LIKE '%Błąd%' OR p.OstatniStatus LIKE '%Zwrot%')";

                string query = $@"
                    SELECT p.Id, p.NumerListu, p.NrZgloszenia, p.OstatniStatus,
                    CONCAT(IFNULL(n.ImieNazwisko, ''), ' ', IFNULL(n.NazwaFirmy, '')) AS NazwaNadawcy,
                    CONCAT(IFNULL(o.ImieNazwisko, ''), ' ', IFNULL(o.NazwaFirmy, '')) AS NazwaOdbiorcy
                    FROM Przesylki p
                    LEFT JOIN Klienci n ON p.NadawcaId = n.Id
                    LEFT JOIN Klienci o ON p.OdbiorcaId = o.Id
                    {whereClause}
                    ORDER BY p.Id DESC LIMIT 100";

                var par = new[] {
                    new MySqlParameter("@d1", dtpOd.Value.ToString("yyyy-MM-dd")),
                    new MySqlParameter("@d2", dtpDo.Value.ToString("yyyy-MM-dd"))
                };

                var results = await GetShipmentsFromSql(query, par);
                dgvArchiwum.DataSource = results;
                FormatGrid(dgvArchiwum);
            }
            finally { btnSzukaj.Enabled = true; }
        }

        // Pomocnicza metoda do SQL
        private async Task<List<PrzesylkaViewModel>> GetShipmentsFromSql(string query, MySqlParameter[] p = null)
        {
            var list = new List<PrzesylkaViewModel>();
            using (var con = DatabaseHelper.GetConnection())
            {
                await con.OpenAsync();
                using (var cmd = new MySqlCommand(query, con))
                {
                    if (p != null) cmd.Parameters.AddRange(p);
                    using (var r = await cmd.ExecuteReaderAsync())
                    {
                        while (await r.ReadAsync())
                        {
                            list.Add(new PrzesylkaViewModel
                            {
                                Id = Convert.ToInt32(r["Id"]),
                                NumerListu = r["NumerListu"].ToString(),
                                NrZgloszenia = r["NrZgloszenia"].ToString(),
                                OstatniStatus = r["OstatniStatus"].ToString(),
                                NazwaNadawcy = r["NazwaNadawcy"].ToString(),
                                NazwaOdbiorcy = r["NazwaOdbiorcy"].ToString()
                            });
                        }
                    }
                }
            }
            return list;
        }

        private async void Dgv_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            var dgv = sender as DataGridView;
            if (e.RowIndex < 0 || dgv.CurrentRow == null) return;

            if (dgv.CurrentRow.DataBoundItem is PrzesylkaViewModel shipment)
            {
                // Jeśli jesteśmy w Tab 1 -> aktualizujemy historię na dole
                if (tabMain.SelectedTab == tabWDrodze)
                {
                    lblHistoriaHeader.Text = $"📋 Historia: {shipment.NumerListu} ({shipment.NrZgloszenia})";
                    try
                    {
                        dgvHistoriaAktywne.DataSource = null;
                        var history = await _trackingService.GetShipmentHistoryAsync(shipment.NumerListu);
                        dgvHistoriaAktywne.DataSource = history;
                        FormatHistoryGrid(dgvHistoriaAktywne);
                    }
                    catch { }
                }
                // Można dodać obsługę w innych tabach (np. popup ze szczegółami)
            }
        }

        private async void btnCheckStatus_Click(object sender, EventArgs e)
        {
            try
            {
                this.Cursor = Cursors.WaitCursor;
                await _trackingService.UpdateAllActiveShipmentsStatusAsync();
                await LoadAllDataAsync();
            }
            finally { this.Cursor = Cursors.Default; }
        }

        private void FormatGrid(DataGridView dgv)
        {
            if (dgv.DataSource == null) return;
            if (dgv.Columns.Contains("Id")) dgv.Columns["Id"].Visible = false;

            if (dgv.Columns.Contains("NumerListu")) { dgv.Columns["NumerListu"].HeaderText = "List Przewozowy"; dgv.Columns["NumerListu"].Width = 140; }
            if (dgv.Columns.Contains("NrZgloszenia")) { dgv.Columns["NrZgloszenia"].HeaderText = "Zgłoszenie"; dgv.Columns["NrZgloszenia"].Width = 100; }
            if (dgv.Columns.Contains("OstatniStatus")) { dgv.Columns["OstatniStatus"].HeaderText = "Status"; dgv.Columns["OstatniStatus"].AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill; }
            if (dgv.Columns.Contains("NazwaNadawcy")) { dgv.Columns["NazwaNadawcy"].HeaderText = "Nadawca"; dgv.Columns["NazwaNadawcy"].Width = 150; }
            if (dgv.Columns.Contains("NazwaOdbiorcy")) { dgv.Columns["NazwaOdbiorcy"].HeaderText = "Odbiorca"; dgv.Columns["NazwaOdbiorcy"].Width = 150; }
        }

        private void FormatHistoryGrid(DataGridView dgv)
        {
            if (dgv.DataSource == null) return;
            if (dgv.Columns.Contains("DataStatusu")) { dgv.Columns["DataStatusu"].HeaderText = "Czas"; dgv.Columns["DataStatusu"].Width = 140; dgv.Columns["DataStatusu"].DefaultCellStyle.Format = "yyyy-MM-dd HH:mm"; }
            if (dgv.Columns.Contains("OpisStatusu")) { dgv.Columns["OpisStatusu"].HeaderText = "Zdarzenie"; dgv.Columns["OpisStatusu"].AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill; }
            if (dgv.Columns.Contains("Oddzial")) { dgv.Columns["Oddzial"].HeaderText = "Oddział"; dgv.Columns["Oddzial"].Width = 100; }
        }

        private void SetStatus(string msg, bool err = false) { statusLabelInfo.Text = msg; statusLabelInfo.ForeColor = err ? Color.Red : Color.Black; }
    
        /// <summary>
        /// Włącza sprawdzanie pisowni po polsku dla wszystkich TextBoxów w formularzu
        /// </summary>
        private void EnableSpellCheckOnAllTextBoxes()
        {
            try
            {
                // Włącz sprawdzanie pisowni dla wszystkich kontrolek typu TextBox i RichTextBox
                foreach (Control control in GetAllControls(this))
                {
                    if (control is RichTextBox richTextBox)
                    {
                        richTextBox.EnableSpellCheck(true);
                    }
                    else if (control is TextBox textBox && !(textBox is SpellCheckTextBox))
                    {
                        // Dla zwykłych TextBoxów - bez podkreślania (bo nie obsługują kolorów)
                        textBox.EnableSpellCheck(false);
                    }
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Błąd włączania sprawdzania pisowni: {ex.Message}");
            }
        }

        /// <summary>
        /// Rekurencyjnie pobiera wszystkie kontrolki z kontenera
        /// </summary>
        private IEnumerable<Control> GetAllControls(Control container)
        {
            foreach (Control control in container.Controls)
            {
                yield return control;

                if (control.HasChildren)
                {
                    foreach (Control child in GetAllControls(control))
                    {
                        yield return child;
                    }
                }
            }
        }
}
}