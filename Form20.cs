using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Newtonsoft.Json; // Potrzebne do zapisu/odczytu widoku

namespace Reklamacje_Dane
{
    public partial class Form20 : Form
    {
        private BindingSource bindingSource = new BindingSource();

        private List<string> _selectedStatusOgolny = new List<string>();
        private List<string> _selectedStatusKlient = new List<string>();
        private List<string> _selectedStatusProducent = new List<string>();

        private Timer _searchDebounceTimer;

        private readonly ZgloszeniaService _zgloszeniaService = new ZgloszeniaService(new DatabaseService(DatabaseHelper.GetConnectionString()));

        public Form20()
        {
            InitializeComponent();
        

            // Włącz sprawdzanie pisowni dla wszystkich TextBoxów
            EnableSpellCheckOnAllTextBoxes();
        }

        public Form20(HashSet<int> columnsToShow, string filterValue, string labelText, Dictionary<int, string> columnValues, Dictionary<int, int> columnWidths)
        {
            InitializeComponent();
            this.Text = labelText;
            label1.Text = labelText;

            typeof(DataGridView).InvokeMember("DoubleBuffered",
                BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.SetProperty,
                null, dataGridView1, new object[] { true });
        }

        private async void Form20_Load(object sender, EventArgs e)
        {
            _searchDebounceTimer = new Timer { Interval = 500 };
            _searchDebounceTimer.Tick += SearchDebounceTimer_Tick;

            txtGlobalSearch.TextChanged += TxtGlobalSearch_TextChanged;

            dtpZgloszenieOd.Value = DateTime.Now.AddYears(-1);
            dtpZgloszenieDo.Value = DateTime.Now;
            dtpZakupOd.Value = DateTime.Now.AddYears(-1);
            dtpZakupDo.Value = DateTime.Now;

            await WczytajDaneDoDataGridViewAsync(); // Pierwsze ładowanie danych
            SetupFilterRow(); // Konfiguracja wiersza filtra
            LoadDefaultView(); // Wczytanie zapisanego widoku kolumn
            AktualizujListeKolumn();
        }

        private void SetupFilterRow()
        {
            dgvFilterRow.Columns.Clear();
            foreach (DataGridViewColumn col in dataGridView1.Columns)
            {
                dgvFilterRow.Columns.Add(new DataGridViewTextBoxColumn
                {
                    Name = col.Name,
                    HeaderText = "", // Ukrywamy nagłówek wiersza filtra
                    Width = col.Width
                });
            }
            dgvFilterRow.Rows.Add();
            dgvFilterRow.ColumnHeadersVisible = false;
            // --- ZMIANA: Ukrycie selektora wiersza ---
            dgvFilterRow.RowHeadersVisible = false;

            dgvFilterRow.Scroll += (s, e) => { if (dataGridView1.HorizontalScrollingOffset != dgvFilterRow.HorizontalScrollingOffset) dataGridView1.HorizontalScrollingOffset = dgvFilterRow.HorizontalScrollingOffset; };
            dataGridView1.Scroll += (s, e) => { if (dgvFilterRow.HorizontalScrollingOffset != dataGridView1.HorizontalScrollingOffset) dgvFilterRow.HorizontalScrollingOffset = dataGridView1.HorizontalScrollingOffset; };
            dataGridView1.ColumnWidthChanged += (s, e) => { if (dgvFilterRow.Columns.Contains(e.Column.Name)) dgvFilterRow.Columns[e.Column.Name].Width = e.Column.Width; };

            dgvFilterRow.EditingControlShowing += (s, e) => {
                if (e.Control is TextBox tb)
                {
                    tb.TextChanged -= FilterRow_TextChanged; // Unikamy wielokrotnego podpinania
                    tb.TextChanged += FilterRow_TextChanged;
                }
            };
        }

        private void FilterRow_TextChanged(object sender, EventArgs e)
        {
            _searchDebounceTimer.Stop();
            _searchDebounceTimer.Start();
        }

        private void TxtGlobalSearch_TextChanged(object sender, EventArgs e)
        {
            _searchDebounceTimer.Stop();
            _searchDebounceTimer.Start();
        }

        private async void SearchDebounceTimer_Tick(object sender, EventArgs e)
        {
            _searchDebounceTimer.Stop();
            await WczytajDaneDoDataGridViewAsync();
        }

        private async Task WczytajDaneDoDataGridViewAsync()
        {
            var columnFilters = new Dictionary<string, string>();
            if (dgvFilterRow.Rows.Count > 0)
            {
                foreach (DataGridViewCell cell in dgvFilterRow.Rows[0].Cells)
                {
                    if (cell.Value != null && !string.IsNullOrWhiteSpace(cell.Value.ToString()))
                    {
                        string columnName = dgvFilterRow.Columns[cell.ColumnIndex].Name;
                        string dbColumnName = GetDbColumnName(columnName);
                        if (!string.IsNullOrEmpty(dbColumnName))
                        {
                            columnFilters[dbColumnName] = cell.Value.ToString();
                        }
                    }
                }
            }

            var dataTable = await _zgloszeniaService.GetComplaintsAsync(
                txtGlobalSearch.Text,
                columnFilters,
                chkDataZgloszenia.Checked,
                dtpZgloszenieOd.Value,
                dtpZgloszenieDo.Value,
                chkDataZakupu.Checked,
                dtpZakupOd.Value,
                dtpZakupDo.Value,
                _selectedStatusOgolny,
                _selectedStatusKlient,
                _selectedStatusProducent);

            bindingSource.DataSource = dataTable;
            dataGridView1.DataSource = bindingSource;
            label2.Text = "Liczba wyszukań: " + dataGridView1.Rows.Count;
        }

        private string GetDbColumnName(string viewColumnName)
        {
            switch (viewColumnName)
            {
                case "Klient": return "CONCAT(k.ImieNazwisko, ' | ', k.NazwaFirmy)";
                case "Produkt": return "p.NazwaKrotka";
                default: return "z." + viewColumnName; // Założenie, że inne nazwy pasują
            }
        }


        private void AktualizujListeKolumn()
        {
            chkListColumns.ItemCheck -= ChkListColumns_ItemCheck; // Odpinamy, aby uniknąć problemów
            chkListColumns.Items.Clear();
            if (dataGridView1.Columns.Count > 0)
            {
                foreach (DataGridViewColumn col in dataGridView1.Columns.Cast<DataGridViewColumn>().OrderBy(c => c.DisplayIndex))
                {
                    chkListColumns.Items.Add(col.Name, col.Visible);
                }
            }
            chkListColumns.ItemCheck += ChkListColumns_ItemCheck; // Podpinamy z powrotem
        }

        private void ChkListColumns_ItemCheck(object sender, ItemCheckEventArgs e)
        {
            string columnName = chkListColumns.Items[e.Index].ToString();
            if (dataGridView1.Columns.Contains(columnName))
            {
                dataGridView1.Columns[columnName].Visible = e.NewValue == CheckState.Checked;
            }
        }

        private void oko_Click(object sender, EventArgs e) => chkListColumns.Visible = !chkListColumns.Visible;

        private void dataGridView1_CellDoubleClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex >= 0)
            {
                var nrZgloszenia = dataGridView1.Rows[e.RowIndex].Cells["NrZgloszenia"].Value?.ToString();
                if (!string.IsNullOrEmpty(nrZgloszenia))
                {
                    new Form2(nrZgloszenia).Show();
                }
            }
        }

        private void chkDataZgloszenia_CheckedChanged(object sender, EventArgs e)
        {
            dtpZgloszenieOd.Enabled = chkDataZgloszenia.Checked;
            dtpZgloszenieDo.Enabled = chkDataZgloszenia.Checked;
        }

        private void chkDataZakupu_CheckedChanged(object sender, EventArgs e)
        {
            dtpZakupOd.Enabled = chkDataZakupu.Checked;
            dtpZakupDo.Enabled = chkDataZakupu.Checked;
        }

        private async void btnApplyFilters_Click(object sender, EventArgs e)
        {
            await WczytajDaneDoDataGridViewAsync();
        }

        private async void btnResetFilters_Click(object sender, EventArgs e)
        {
            txtGlobalSearch.Clear();
            chkDataZgloszenia.Checked = false;
            chkDataZakupu.Checked = false;

            _selectedStatusOgolny.Clear();
            _selectedStatusKlient.Clear();
            _selectedStatusProducent.Clear();

            btnFilterStatusOgolny.Text = "Status Ogólny (Wszystkie)";
            btnFilterStatusKlient.Text = "Status Klient (Wszystkie)";
            btnFilterStatusProducent.Text = "Status Producent (Wszystkie)";

            if (dgvFilterRow.Rows.Count > 0) dgvFilterRow.Rows[0].Cells.Cast<DataGridViewCell>().ToList().ForEach(c => c.Value = null);

            await WczytajDaneDoDataGridViewAsync();
        }

        private async Task ShowStatusFilterPopupAsync(string columnName, List<string> selectedValues, Button senderButton)
        {
            var allStatusy = await _zgloszeniaService.GetDistinctValuesAsync(columnName);

            using (var popup = new Form())
            {
                popup.Text = $"Wybierz {columnName}";
                popup.StartPosition = FormStartPosition.CenterParent;
                popup.Size = new Size(400, 500);
                popup.FormBorderStyle = FormBorderStyle.FixedDialog;
                popup.MaximizeBox = false;
                popup.MinimizeBox = false;

                var chkList = new CheckedListBox { Dock = DockStyle.Fill, CheckOnClick = true, Font = new Font("Segoe UI", 10) };
                for (int i = 0; i < allStatusy.Count; i++) chkList.Items.Add(allStatusy[i], selectedValues.Contains(allStatusy[i]));

                var btnOk = new Button { Text = "Zastosuj", Dock = DockStyle.Bottom, Height = 40, Font = new Font("Segoe UI", 10, FontStyle.Bold) };
                btnOk.Click += (s, ev) =>
                {
                    selectedValues.Clear();
                    foreach (var item in chkList.CheckedItems) selectedValues.Add(item.ToString());
                    senderButton.Text = selectedValues.Any() ? $"{columnName} ({selectedValues.Count})" : $"{columnName} (Wszystkie)";
                    popup.DialogResult = DialogResult.OK;
                    popup.Close();
                };

                popup.Controls.Add(chkList);
                popup.Controls.Add(btnOk);
                popup.ShowDialog(this);
            }
        }

        private async void btnFilterStatusOgolny_Click(object sender, EventArgs e) => await ShowStatusFilterPopupAsync("StatusOgolny", _selectedStatusOgolny, btnFilterStatusOgolny);
        private async void btnFilterStatusKlient_Click(object sender, EventArgs e) => await ShowStatusFilterPopupAsync("StatusKlient", _selectedStatusKlient, btnFilterStatusKlient);
        private async void btnFilterStatusProducent_Click(object sender, EventArgs e) => await ShowStatusFilterPopupAsync("StatusProducent", _selectedStatusProducent, btnFilterStatusProducent);

        // --- ZAPIS I ODZCZYT WIDOKU ---
        private void btnSaveView_Click(object sender, EventArgs e)
        {
            var viewSettings = dataGridView1.Columns.Cast<DataGridViewColumn>()
                .Select(c => new ColumnSetting
                {
                    Name = c.Name,
                    Visible = c.Visible,
                    Width = c.Width,
                    DisplayIndex = c.DisplayIndex
                }).ToList();

            string settingsJson = JsonConvert.SerializeObject(viewSettings);
            Properties.Settings.Default.Form20View = settingsJson;
            Properties.Settings.Default.Save();
            MessageBox.Show("Widok domyślny został zapisany.", "Zapisano", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        private void LoadDefaultView()
        {
            string settingsJson = Properties.Settings.Default.Form20View;
            if (!string.IsNullOrEmpty(settingsJson))
            {
                try
                {
                    var viewSettings = JsonConvert.DeserializeObject<List<ColumnSetting>>(settingsJson);
                    foreach (var setting in viewSettings)
                    {
                        if (dataGridView1.Columns.Contains(setting.Name))
                        {
                            var col = dataGridView1.Columns[setting.Name];
                            col.Visible = setting.Visible;
                            col.Width = setting.Width;
                            col.DisplayIndex = setting.DisplayIndex;
                        }
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Nie udało się wczytać zapisanego widoku. Błąd: " + ex.Message);
                }
            }
        }

        private void Form20_FormClosing(object sender, FormClosingEventArgs e)
        {
            _searchDebounceTimer?.Dispose();
        }

        private void dataGridView1_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {

        }

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

    // Klasa pomocnicza do zapisu ustawień widoku
    public class ColumnSetting
    {
        public string Name { get; set; }
        public bool Visible { get; set; }
        public int Width { get; set; }
        public int DisplayIndex { get; set; }
    }
}
