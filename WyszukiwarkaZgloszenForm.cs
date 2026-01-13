using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;
using Excel = Microsoft.Office.Interop.Excel;

namespace Reklamacje_Dane
{
    /// <summary>
    /// WYSZUKIWARKA ZGŁOSZEŃ v4.0 - SUPER SZYBKA EDYCJA
    /// - Bez lewego panelu (za wolno się budował)
    /// - Możliwość dodawania własnych kolumn
    /// - Cache dla błyskawicznego otwierania
    /// </summary>
    public partial class WyszukiwarkaZgloszenForm : Form
    {
        private List<ComplaintViewModel> _allData = new List<ComplaintViewModel>();
        private List<ComplaintViewModel> _filteredData = new List<ComplaintViewModel>();
        private readonly FastDataService _service = new FastDataService();

        // UI
        private DataGridView _grid;
        private TextBox _txtSearch;
        private Label _lblStats;
        private Panel _loadingOverlay;
        private Label _lblLoading;
        private Panel _filterPanelContainer;
        private Panel _filterPanel;
        private readonly Dictionary<string, TextBox> _columnFilters = new Dictionary<string, TextBox>();

        // Dostępne kolumny (użytkownik może wybierać które pokazać)
        private readonly List<ColumnDefinition> _availableColumns = new List<ColumnDefinition>
        {
            new ColumnDefinition("Id", "ID", 60, false), // Ukryte domyślnie
            new ColumnDefinition("NrZgloszenia", "Nr Zgłoszenia", 120),
            new ColumnDefinition("DataZgloszenia", "Data", 100),
            new ColumnDefinition("Status", "Status", 120),
            new ColumnDefinition("Klient", "Klient", 150),
            new ColumnDefinition("KlientImieNazwisko", "Klient - Imię Nazwisko", 160, false),
            new ColumnDefinition("KlientNazwaFirmy", "Klient - Nazwa Firmy", 160, false),
            new ColumnDefinition("KlientNip", "NIP", 120, false),
            new ColumnDefinition("KlientEmail", "Klient - Email", 160, false),
            new ColumnDefinition("KlientTelefon", "Klient - Telefon", 140, false),
            new ColumnDefinition("KlientUlica", "Klient - Ulica", 160, false),
            new ColumnDefinition("KlientKodPocztowy", "Klient - Kod Pocztowy", 140, false),
            new ColumnDefinition("KlientMiejscowosc", "Klient - Miejscowość", 160, false),
            new ColumnDefinition("Produkt", "Produkt", 200),
            new ColumnDefinition("NazwaSystemowa", "Nazwa Systemowa", 170, false),
            new ColumnDefinition("NazwaKrotka", "Model", 150, false),
            new ColumnDefinition("KodEnova", "Kod Enova", 120, false),
            new ColumnDefinition("KodProducenta", "Kod Prod.", 120, false),
            new ColumnDefinition("Kategoria", "Kategoria", 140, false),
            new ColumnDefinition("ProduktWymagania", "Wymagania Produktu", 180, false),
            new ColumnDefinition("Producent", "Producent", 120),
            new ColumnDefinition("ProducentKontaktMail", "Producent - Kontakt Mail", 180, false),
            new ColumnDefinition("ProducentAdres", "Producent - Adres", 180, false),
            new ColumnDefinition("ProducentPlEng", "Producent - PL/ENG", 140, false),
            new ColumnDefinition("ProducentJezyk", "Producent - Język", 140, false),
            new ColumnDefinition("ProducentFormularz", "Producent - Formularz", 160, false),
            new ColumnDefinition("ProducentWymagania", "Producent - Wymagania", 180, false),
            new ColumnDefinition("SN", "S/N", 100),
            new ColumnDefinition("FV", "Faktura", 100),
            new ColumnDefinition("Skad", "Źródło", 100),
            new ColumnDefinition("DataZakupu", "Data Zakupu", 120, false),
            new ColumnDefinition("OpisUsterki", "Opis Usterki", 200, false),
            new ColumnDefinition("ProduktOpis", "Produkt (opis)", 200, false),
            new ColumnDefinition("AllegroBuyerLogin", "Allegro Login", 140, false),
            new ColumnDefinition("AllegroOrderId", "Allegro Order", 140, false),
            new ColumnDefinition("AllegroDisputeId", "Allegro Dispute", 140, false),
            new ColumnDefinition("AllegroAccountId", "Allegro Konto", 120, false),
            new ColumnDefinition("GwarancjaPlatna", "Gwarancja Płatna", 140, false),
            new ColumnDefinition("StatusKlient", "Status Klient", 140, false),
            new ColumnDefinition("StatusProducent", "Status Producent", 160, false),
            new ColumnDefinition("CzekamyNaDostawe", "Czekamy na Dostawę", 160, false),
            new ColumnDefinition("NrWRL", "Nr WRL", 120, false),
            new ColumnDefinition("NrKWZ2", "Nr KWZ2", 120, false),
            new ColumnDefinition("NrRMA", "Nr RMA", 120, false),
            new ColumnDefinition("NrKPZN", "Nr KPZN", 120, false),
            new ColumnDefinition("CzyNotaRozliczona", "Czy Nota Rozliczona", 160, false),
            new ColumnDefinition("KwotaZwrotu", "Kwota Zwrotu", 120, false),
            new ColumnDefinition("NrFakturyPrzychodu", "Nr Faktury Przychodu", 160, false),
            new ColumnDefinition("KwotaFakturyPrzychoduNetto", "Kwota Faktury Przychodu Netto", 190, false),
            new ColumnDefinition("NrFakturyKosztowej", "Nr Faktury Kosztowej", 160, false),
            new ColumnDefinition("Dzialania", "Działania", 200, false)
        };

        public WyszukiwarkaZgloszenForm()
        {
        //    InitializeComponent();
            SetupUI();
            this.Load += WyszukiwarkaZgloszenForm_Load;
        

            // Włącz sprawdzanie pisowni dla wszystkich TextBoxów
            EnableSpellCheckOnAllTextBoxes();
        }

        private async void WyszukiwarkaZgloszenForm_Load(object sender, EventArgs e)
        {
            this.WindowState = FormWindowState.Maximized;
            await LoadDataAsync();
        }

        private void SetupUI()
        {
            this.Text = "Wyszukiwarka Zgłoszeń - Szybka Edycja";
            this.Font = new Font("Segoe UI", 9F);
            this.BackColor = Color.White;
            this.Size = new Size(1400, 800);
            this.StartPosition = FormStartPosition.CenterScreen;

            // Loading overlay
            _loadingOverlay = new Panel { Dock = DockStyle.Fill, BackColor = Color.FromArgb(250, 250, 250), Visible = false };
            _lblLoading = new Label 
            { 
                Text = "Ładowanie...", 
                Font = new Font("Segoe UI Light", 20), 
                ForeColor = Color.Gray,
                AutoSize = true,
                Location = new Point(500, 300)
            };
            _loadingOverlay.Controls.Add(_lblLoading);
            _loadingOverlay.Resize += (s, e) => 
            {
                _lblLoading.Left = (_loadingOverlay.Width - _lblLoading.Width) / 2;
                _lblLoading.Top = (_loadingOverlay.Height - _lblLoading.Height) / 2;
            };

            // Main layout
            var mainPanel = new Panel { Dock = DockStyle.Fill };

            // Top bar
            var topBar = new Panel { Dock = DockStyle.Top, Height = 60, BackColor = Color.FromArgb(245, 245, 245), Padding = new Padding(15, 10, 15, 10) };
            
            var lblTitle = new Label { Text = "🔍 Wyszukaj:", Font = new Font("Segoe UI Semibold", 11), AutoSize = true, Location = new Point(15, 18) };
            
         
            
                _txtSearch = new TextBox
                {
                    Width = 400, 
                Font = new Font("Segoe UI", 11),
                //PlaceholderText = "Wpisz nr zgłoszenia, klienta, produkt, SN..."
            };
            _txtSearch.TextChanged += (s, e) => ApplyFilters();

            var btnRefresh = new Button 
            { 
                Text = "🔄 Odśwież", 
          
                Size = new Size(100, 32),
                FlatStyle = FlatStyle.Flat,
                BackColor = Color.FromArgb(220, 220, 220),
                Cursor = Cursors.Hand
            };
            btnRefresh.FlatAppearance.BorderSize = 0;
            btnRefresh.Click += async (s, e) => await LoadDataAsync(true);

            var btnColumns = new Button 
            { 
                Text = "⚙ Kolumny", 
               
                Size = new Size(100, 32),
                FlatStyle = FlatStyle.Flat,
                BackColor = Color.FromArgb(0, 120, 215),
                ForeColor = Color.White,
                Cursor = Cursors.Hand
            };
            btnColumns.FlatAppearance.BorderSize = 0;
            btnColumns.Click += ShowColumnSelector;

            var btnExport = new Button 
            { 
                Text = "📊 Export", 
            
                Size = new Size(100, 32),
                FlatStyle = FlatStyle.Flat,
                BackColor = Color.FromArgb(16, 124, 16),
                ForeColor = Color.White,
                Cursor = Cursors.Hand
            };
            btnExport.FlatAppearance.BorderSize = 0;
            btnExport.Click += (s, e) => ExportToExcel();

            _lblStats = new Label 
            { 
                Text = "Gotowy", 
                Font = new Font("Segoe UI Semibold", 10), 
               
                AutoSize = true 
            };
                var labelWidth = TextRenderer.MeasureText(lblTitle.Text, lblTitle.Font).Width;
                var searchLeft = lblTitle.Left + labelWidth + 12;
                _txtSearch.Location = new Point(searchLeft, 15);
                btnRefresh.Location = new Point(_txtSearch.Right + 10, 14);
                btnColumns.Location = new Point(btnRefresh.Right + 10, 14);
                btnExport.Location = new Point(btnColumns.Right + 10, 14);
                _lblStats.Location = new Point(btnExport.Right + 20, 20);


                topBar.Controls.AddRange(new Control[] { lblTitle, _txtSearch, btnRefresh, btnColumns, btnExport, _lblStats });

            _filterPanelContainer = new Panel
            {
                Dock = DockStyle.Top,
                Height = 34,
                BackColor = Color.FromArgb(250, 250, 250)
            };

            _filterPanel = new Panel
            {
                Dock = DockStyle.Fill,
                BackColor = Color.FromArgb(250, 250, 250)
            };
            _filterPanelContainer.Controls.Add(_filterPanel);

            // Grid
            _grid = new DataGridView
            {
                Dock = DockStyle.Fill,
                AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill,
                BackgroundColor = Color.White,
                BorderStyle = BorderStyle.None,
                RowHeadersVisible = false,
                AllowUserToAddRows = false,
                AllowUserToDeleteRows = false,
                ReadOnly = true,
                SelectionMode = DataGridViewSelectionMode.FullRowSelect,
                EnableHeadersVisualStyles = false,
                ColumnHeadersHeight = 35,
                RowTemplate = { Height = 28 },
                GridColor = Color.FromArgb(230, 230, 230)
            };

            _grid.ColumnHeadersDefaultCellStyle.BackColor = Color.FromArgb(240, 240, 240);
            _grid.ColumnHeadersDefaultCellStyle.Font = new Font("Segoe UI Semibold", 9);
            _grid.ColumnHeadersDefaultCellStyle.ForeColor = Color.FromArgb(64, 64, 64);
            _grid.DefaultCellStyle.SelectionBackColor = Color.FromArgb(230, 240, 255);
            _grid.DefaultCellStyle.SelectionForeColor = Color.Black;

            _grid.CellDoubleClick += (s, e) => 
            {
                if (e.RowIndex >= 0)
                {
                    var item = _grid.Rows[e.RowIndex].DataBoundItem as ComplaintViewModel;
                    if (item != null) new Form2(item.NrZgloszenia).Show();
                }
            };

            _grid.RowPrePaint += (s, e) => 
            {
                if (e.RowIndex >= 0 && e.RowIndex < _grid.Rows.Count)
                {
                    var item = _grid.Rows[e.RowIndex].DataBoundItem as ComplaintViewModel;
                    if (item != null && item.Skad != null && item.Skad.Contains("Allegro"))
                        _grid.Rows[e.RowIndex].DefaultCellStyle.BackColor = Color.FromArgb(255, 250, 235);
                }
            };

            _grid.ColumnWidthChanged += (s, e) => SyncFilterWidth(e.Column);
            _grid.ColumnDisplayIndexChanged += (s, e) => BuildColumnFilters();
            _grid.ColumnStateChanged += (s, e) =>
            {
                if (e.StateChanged == DataGridViewElementStates.Visible)
                {
                    BuildColumnFilters();
                }
            };
            _grid.Scroll += (s, e) =>
            {
                if (e.ScrollOrientation == ScrollOrientation.HorizontalScroll)
                {
                    _filterPanel.Left = -_grid.HorizontalScrollingOffset;
                }
            };

            mainPanel.Controls.Add(_grid);
            mainPanel.Controls.Add(_filterPanelContainer);
            mainPanel.Controls.Add(topBar);

            this.Controls.Add(mainPanel);
            this.Controls.Add(_loadingOverlay);

            SetupGridColumns();
        }

        private void SetupGridColumns()
        {
            _grid.AutoGenerateColumns = false;
            _grid.Columns.Clear();

            foreach (var colDef in _availableColumns.Where(c => c.VisibleByDefault))
            {
                var col = new DataGridViewTextBoxColumn
                {
                    Name = colDef.PropertyName,
                    DataPropertyName = colDef.PropertyName,
                    HeaderText = colDef.DisplayName,
                    Width = colDef.Width,
                    Visible = colDef.VisibleByDefault
                };
                _grid.Columns.Add(col);
            }

            BuildColumnFilters();
        }

        private void BuildColumnFilters()
        {
            var existingFilters = _columnFilters.ToDictionary(kvp => kvp.Key, kvp => kvp.Value.Text);
            _columnFilters.Clear();
            _filterPanel.Controls.Clear();

            int x = 0;
            var visibleColumns = _grid.Columns.Cast<DataGridViewColumn>()
                .Where(c => c.Visible)
                .OrderBy(c => c.DisplayIndex)
                .ToList();

            foreach (var column in visibleColumns)
            {
                var textBox = new TextBox
                {
                    Width = column.Width,
                    Height = 24,
                    Font = new Font("Segoe UI", 9F),
                    BorderStyle = BorderStyle.FixedSingle,
                    Tag = column.DataPropertyName,
                    Location = new Point(x, 4)
                };

                if (existingFilters.TryGetValue(column.DataPropertyName, out var value))
                {
                    textBox.Text = value;
                }

                textBox.TextChanged += (s, e) => ApplyFilters();

                _filterPanel.Controls.Add(textBox);
                _columnFilters[column.DataPropertyName] = textBox;

                x += column.Width;
            }

            _filterPanel.Width = Math.Max(x, _filterPanelContainer.Width);
            _filterPanel.Left = -_grid.HorizontalScrollingOffset;
        }

        private void SyncFilterWidth(DataGridViewColumn column)
        {
            if (column == null || !column.Visible) return;

            if (_columnFilters.TryGetValue(column.DataPropertyName, out var textBox))
            {
                textBox.Width = column.Width;
                BuildColumnFilters();
            }
        }

        private async Task LoadDataAsync(bool forceRefresh = false)
        {
            ShowLoading(true);
            try
            {
                if (forceRefresh || !DataCache.Instance.HasData())
                {
                    if (forceRefresh) DataCache.Instance.Clear();
                    
                    _allData = await Task.Run(() => _service.LoadAllComplaintsAsync());
                    DataCache.Instance.SetData(_allData);
                }
                else
                {
                    _allData = DataCache.Instance.GetData();
                }

                ApplyFilters();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Błąd ładowania: {ex.Message}", "Błąd", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            finally
            {
                ShowLoading(false);
                _txtSearch.Focus();
            }
        }

        private void ApplyFilters()
        {
            var searchText = _txtSearch.Text.ToLower();
            
            if (string.IsNullOrWhiteSpace(searchText))
            {
                _filteredData = _allData;
            }
            else
            {
                var terms = searchText.Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
                _filteredData = _allData.Where(x => terms.All(term => x.SearchVector.Contains(term))).ToList();
            }

            foreach (var filter in _columnFilters.Where(kvp => !string.IsNullOrWhiteSpace(kvp.Value.Text)))
            {
                var term = filter.Value.Text.Trim().ToLower();
                _filteredData = _filteredData
                    .Where(item => ColumnMatches(item, filter.Key, term))
                    .ToList();
            }

            _grid.DataSource = _filteredData;
            _lblStats.Text = $"Wyniki: {_filteredData.Count} / {_allData.Count}";
            _lblStats.ForeColor = _filteredData.Count > 0 ? Color.Green : Color.Red;
        }

        private bool ColumnMatches(ComplaintViewModel item, string propertyName, string term)
        {
            if (item == null || string.IsNullOrWhiteSpace(propertyName)) return false;

            var prop = item.GetType().GetProperty(propertyName);
            if (prop == null) return false;

            var value = prop.GetValue(item);
            string text = string.Empty;

            // C# 7.3 nie obsługuje wyrażeń switch (value switch { ... }), 
            // dlatego używamy dopasowania wzorca (pattern matching) w instrukcjach if.
            if (value is DateTime dt)
            {
                text = dt.ToString("yyyy-MM-dd HH:mm");
            }
            else if (value != null)
            {
                text = value.ToString();
            }

            // Dobrą praktyką jest zamiana obu ciągów na małe litery przed porównaniem
            return text.ToLower().Contains(term.ToLower());
        }

        private void ShowColumnSelector(object sender, EventArgs e)
        {
            var menu = new ContextMenuStrip { Font = new Font("Segoe UI", 9) };
            menu.Items.Add(new ToolStripLabel("Wybierz kolumny do wyświetlenia:") { Font = new Font("Segoe UI Semibold", 9) });
            menu.Items.Add(new ToolStripSeparator());

            foreach (var colDef in _availableColumns)
            {
                var item = new ToolStripMenuItem(colDef.DisplayName);
                item.Checked = _grid.Columns.Cast<DataGridViewColumn>().Any(c => c.Name == colDef.PropertyName && c.Visible);
                item.Click += (s, ev) => ToggleColumn(colDef);
                menu.Items.Add(item);
            }

            menu.Show(Cursor.Position);
        }

        private void ToggleColumn(ColumnDefinition colDef)
        {
            var existing = _grid.Columns.Cast<DataGridViewColumn>().FirstOrDefault(c => c.Name == colDef.PropertyName);
            
            if (existing != null)
            {
                existing.Visible = !existing.Visible;
            }
            else
            {
                var col = new DataGridViewTextBoxColumn
                {
                    Name = colDef.PropertyName,
                    DataPropertyName = colDef.PropertyName,
                    HeaderText = colDef.DisplayName,
                    Width = colDef.Width
                };
                _grid.Columns.Add(col);
            }
        }

        private void ShowLoading(bool show)
        {
            _loadingOverlay.Visible = show;
            if (show) _loadingOverlay.BringToFront();
            Application.DoEvents();
        }

        private void ExportToExcel()
        {
            if (_filteredData.Count == 0) return;
            
            ShowLoading(true);
            try
            {
                var app = new Excel.Application();
                var wb = app.Workbooks.Add();
                var ws = (Excel.Worksheet)wb.Sheets[1];

                // Headers
                int colIdx = 1;
                var visibleCols = _grid.Columns.Cast<DataGridViewColumn>().Where(c => c.Visible).ToList();
                foreach (var c in visibleCols) ws.Cells[1, colIdx++] = c.HeaderText;

                // Data
                object[,] data = new object[_filteredData.Count, visibleCols.Count];
                for (int i = 0; i < _filteredData.Count; i++)
                {
                    var item = _filteredData[i];
                    for (int j = 0; j < visibleCols.Count; j++)
                    {
                        var prop = item.GetType().GetProperty(visibleCols[j].DataPropertyName);
                        data[i, j] = prop?.GetValue(item) ?? "";
                    }
                }

                ws.Range[ws.Cells[2, 1], ws.Cells[_filteredData.Count + 1, visibleCols.Count]].Value = data;
                ws.Columns.AutoFit();
                app.Visible = true;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Błąd exportu: {ex.Message}");
            }
            finally
            {
                ShowLoading(false);
            }
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

        // Pomocnicza klasa definicji kolumny
        private class ColumnDefinition
        {
            public string PropertyName { get; set; }
            public string DisplayName { get; set; }
            public int Width { get; set; }
            public bool VisibleByDefault { get; set; }

            public ColumnDefinition(string prop, string display, int width, bool visible = true)
            {
                PropertyName = prop;
                DisplayName = display;
                Width = width;
                VisibleByDefault = visible;
            }
        }
    }

    // Cache singleton (taki sam jak wcześniej)
    public sealed class DataCache
    {
        private static readonly Lazy<DataCache> _instance = new Lazy<DataCache>(() => new DataCache());
        public static DataCache Instance => _instance.Value;

        private List<ComplaintViewModel> _cachedData;
        private DateTime _lastUpdate;

        private DataCache() { }

        public bool HasData() => _cachedData != null && _cachedData.Count > 0;
        public List<ComplaintViewModel> GetData() => _cachedData;
        public void SetData(List<ComplaintViewModel> data)
        {
            _cachedData = data;
            _lastUpdate = DateTime.Now;
        }
        public void Clear() => _cachedData = null;
        public DateTime LastUpdate => _lastUpdate;
}
}
