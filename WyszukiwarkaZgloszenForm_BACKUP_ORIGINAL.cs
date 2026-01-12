using System;
using System.Collections.Generic;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Reflection;
using Excel = Microsoft.Office.Interop.Excel;

namespace Reklamacje_Dane
{
    public partial class WyszukiwarkaZgloszenForm : Form
    {
        // --- DANE (Szybki RAM) ---
        private List<ComplaintViewModel> _allData = new List<ComplaintViewModel>();
        private List<ComplaintViewModel> _filteredData = new List<ComplaintViewModel>();
        private readonly FastDataService _service = new FastDataService();

        // --- UI ---
        private DataGridView _grid;
        private TextBox _txtMainSearch;
        private Label _lblStats;
        private Panel _filterRowPanel; // Pasek z textboxami nad kolumnami
        private FlowLayoutPanel _sidePanel; // Panel boczny
        private Panel _loadingOverlay; // "Artystyczny" ekran ładowania
        private Label _lblLoadingText;
        private ProgressBar _loadingProgress;

        // --- MAPOWANIE ---
        // Klucz: Nazwa kolumny w Gridzie, Wartość: TextBox filtrujący
        private Dictionary<string, TextBox> _colFilters = new Dictionary<string, TextBox>();
        // Filtry boczne
        private Dictionary<string, HashSet<string>> _sideFilters = new Dictionary<string, HashSet<string>>();

        public WyszukiwarkaZgloszenForm()
        {
          //  InitializeComponent();
            SetupArtisticUI(); // Budujemy interfejs kodem
        }

        private async void ModernSearchForm_Load(object sender, EventArgs e)
        {
            // Wymuszenie pełnego ekranu
            this.WindowState = FormWindowState.Maximized;
            
            // Start synchronizacji
            await SynchronizeDataAsync();
        }

        // --- LOGIKA BIZNESOWA ---

        private async Task SynchronizeDataAsync()
        {
            try
            {
                ShowLoading(true, "Synchronizacja danych z bazą...");
                
                // 1. Pobranie danych w tle
                _allData = await Task.Run(() => _service.LoadAllComplaintsAsync());
                
                // 2. Budowanie filtrów bocznych na podstawie danych
                ShowLoading(true, "Budowanie indeksów...");
                BuildSideFilters();

                // 3. Wstępne filtrowanie
                ApplyFilters();

                // 4. Ukrycie filtrów dla ukrytych kolumn (synchronizacja)
                RepositionColumnFilters();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Błąd synchronizacji: " + ex.Message, "Ups...", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            finally
            {
                ShowLoading(false);
                _txtMainSearch.Focus();
            }
        }

        private void ApplyFilters()
        {
            var searchTerms = _txtMainSearch.Text.ToLower().Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
            
            // LINQ w pamięci - Błyskawica
            IEnumerable<ComplaintViewModel> query = _allData;

            // 1. Główne szukanie
            if (searchTerms.Length > 0)
            {
                query = query.Where(x => searchTerms.All(term => x.SearchVector.Contains(term)));
            }

            // 2. Filtry w kolumnach
            foreach (var kv in _colFilters)
            {
                string text = kv.Value.Text.ToLower();
                if (string.IsNullOrWhiteSpace(text)) continue;
                
                // Jeśli kolumna ukryta, pomijamy jej filtr (opcjonalne, ale logiczne)
                if (!_grid.Columns[kv.Key].Visible) continue;

                // Szybkie mapowanie po nazwie właściwości
                switch (kv.Key)
                {
                    case "NrZgloszenia": query = query.Where(x => x.NrZgloszenia != null && x.NrZgloszenia.ToLower().Contains(text)); break;
                    case "Klient":       query = query.Where(x => x.Klient != null && x.Klient.ToLower().Contains(text)); break;
                    case "Produkt":      query = query.Where(x => x.Produkt != null && x.Produkt.ToLower().Contains(text)); break;
                    case "SN":           query = query.Where(x => x.SN != null && x.SN.ToLower().Contains(text)); break;
                    case "FV":           query = query.Where(x => x.FV != null && x.FV.ToLower().Contains(text)); break;
                    case "Status":       query = query.Where(x => x.Status != null && x.Status.ToLower().Contains(text)); break;
                    case "Producent":    query = query.Where(x => x.Producent != null && x.Producent.ToLower().Contains(text)); break;
                    case "Skad":         query = query.Where(x => x.Skad != null && x.Skad.ToLower().Contains(text)); break;
                }
            }

            // 3. Filtry boczne
            foreach (var category in _sideFilters)
            {
                if (category.Value.Count > 0)
                {
                    switch (category.Key)
                    {
                        case "Status": query = query.Where(x => category.Value.Contains(x.Status)); break;
                        case "Źródło": query = query.Where(x => category.Value.Contains(x.Skad)); break;
                        case "Producent": query = query.Where(x => category.Value.Contains(x.Producent)); break;
                    }
                }
            }

            _filteredData = query.ToList();
            _grid.DataSource = _filteredData;
            
            // Ładny licznik z kolorowaniem
            _lblStats.Text = $"WYNIKI: {_filteredData.Count}";
            _lblStats.ForeColor = _filteredData.Count > 0 ? Color.SeaGreen : Color.IndianRed;
        }

        // --- BUDOWANIE INTERFEJSU (ARTYSTYCZNE ARCYDZIEŁO ;)) ---

        private void SetupArtisticUI()
        {
            this.Text = "System Analizy Zgłoszeń";
            this.Font = new Font("Segoe UI", 9.5F, FontStyle.Regular);
            this.BackColor = Color.FromArgb(245, 247, 250); // Nowoczesne tło
            this.Icon = SystemIcons.Application; // Możesz dać swoją ikonę

            // 1. Ekran Ładowania (Overlay)
            SetupLoadingOverlay();

            // 2. Główny Layout
            var mainLayout = new TableLayoutPanel { Dock = DockStyle.Fill, ColumnCount = 2, RowCount = 2, BackColor = Color.Transparent };
            mainLayout.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 260F)); // Panel boczny
            mainLayout.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100F));  // Grid
            mainLayout.RowStyles.Add(new RowStyle(SizeType.Absolute, 70F));        // Header
            mainLayout.RowStyles.Add(new RowStyle(SizeType.Percent, 100F));        // Content

            // 3. Panel Górny (Header)
            var headerPanel = new Panel { Dock = DockStyle.Fill, BackColor = Color.White, Padding = new Padding(15) };
            mainLayout.SetColumnSpan(headerPanel, 2); // Header na całą szerokość

            var lblTitle = new Label { Text = "Wyszukiwarka", Font = new Font("Segoe UI", 16, FontStyle.Bold), ForeColor = Color.FromArgb(45, 45, 45), AutoSize = true, Location = new Point(15, 18) };
            
            _txtMainSearch = new TextBox { Location = new Point(180, 20), Width = 500, Font = new Font("Segoe UI", 12), BorderStyle = BorderStyle.FixedSingle };
            _txtMainSearch.TextChanged += (s, e) => ApplyFilters();
            
            // "Placeholder" hack
            var lblHint = new Label { Text = "Wpisz cokolwiek (nr, nazwisko, sn)...", ForeColor = Color.Gray, Location = new Point(180, 48), AutoSize = true, Font = new Font("Segoe UI", 8) };

            _lblStats = new Label { Text = "Gotowy", Font = new Font("Segoe UI", 12, FontStyle.Bold), Location = new Point(700, 22), AutoSize = true };

            var btnExport = new Button { Text = "Eksportuj Widok", FlatStyle = FlatStyle.Flat, BackColor = Color.FromArgb(0, 150, 136), ForeColor = Color.White, Size = new Size(140, 35), Location = new Point(headerPanel.Width - 160, 18), Anchor = AnchorStyles.Top | AnchorStyles.Right, Cursor = Cursors.Hand };
            btnExport.FlatAppearance.BorderSize = 0;
            btnExport.Click += (s, e) => ExportToExcel();

            var btnRefresh = new Button { Text = "⟳", FlatStyle = FlatStyle.Flat, BackColor = Color.WhiteSmoke, Size = new Size(35, 35), Location = new Point(headerPanel.Width - 210, 18), Anchor = AnchorStyles.Top | AnchorStyles.Right, Cursor = Cursors.Hand };
            btnRefresh.Click += async (s, e) => await SynchronizeDataAsync();

            headerPanel.Controls.AddRange(new Control[] { lblTitle, _txtMainSearch, lblHint, _lblStats, btnExport, btnRefresh });

            // 4. Panel Boczny
            _sidePanel = new FlowLayoutPanel { Dock = DockStyle.Fill, AutoScroll = true, BackColor = Color.White, Padding = new Padding(10) };
            // Linia oddzielająca
            _sidePanel.Paint += (s, e) => e.Graphics.DrawLine(Pens.LightGray, _sidePanel.Width - 1, 0, _sidePanel.Width - 1, _sidePanel.Height);

            // 5. Panel Centralny (Grid + Filtry Kolumn)
            var gridContainer = new Panel { Dock = DockStyle.Fill, Padding = new Padding(0, 10, 10, 10) };
            
            _filterRowPanel = new Panel { Dock = DockStyle.Top, Height = 30, BackColor = Color.FromArgb(240, 240, 240) };
            
            _grid = new DataGridView
            {
                Dock = DockStyle.Fill,
                AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill,
                BackgroundColor = Color.White,
                BorderStyle = BorderStyle.None,
                RowHeadersVisible = false,
                AllowUserToAddRows = false,
                AllowUserToOrderColumns = true, // Można przesuwać kolumny!
                ReadOnly = true,
                SelectionMode = DataGridViewSelectionMode.FullRowSelect,
                EnableHeadersVisualStyles = false,
                ColumnHeadersHeight = 40
            };

            // Stylizacja Grida
            _grid.ColumnHeadersDefaultCellStyle.BackColor = Color.FromArgb(245, 245, 245);
            _grid.ColumnHeadersDefaultCellStyle.ForeColor = Color.FromArgb(64, 64, 64);
            _grid.ColumnHeadersDefaultCellStyle.Font = new Font("Segoe UI", 9.5F, FontStyle.Bold);
            _grid.ColumnHeadersBorderStyle = DataGridViewHeaderBorderStyle.Single;
            _grid.DefaultCellStyle.SelectionBackColor = Color.FromArgb(232, 240, 254); // Google style selection
            _grid.DefaultCellStyle.SelectionForeColor = Color.Black;
            _grid.RowTemplate.Height = 32;
            _grid.GridColor = Color.WhiteSmoke;

            // Double Buffering
            typeof(DataGridView).GetProperty("DoubleBuffered", BindingFlags.NonPublic | BindingFlags.Instance)?.SetValue(_grid, true, null);

            // Zdarzenia Grida
            _grid.CellDoubleClick += (s, e) => {
                if (e.RowIndex >= 0) {
                    var item = _grid.Rows[e.RowIndex].DataBoundItem as ComplaintViewModel;
                    if (item != null) new Form2(item.NrZgloszenia).Show();
                }
            };
            _grid.RowPrePaint += (s, e) => {
                if (e.RowIndex >= 0 && e.RowIndex < _grid.Rows.Count) {
                    var item = _grid.Rows[e.RowIndex].DataBoundItem as ComplaintViewModel;
                    if (item != null && item.Skad != null && item.Skad.StartsWith("Allegro")) 
                        _grid.Rows[e.RowIndex].DefaultCellStyle.BackColor = Color.FromArgb(255, 250, 240); // Delikatny pomarańcz
                }
            };
            _grid.ColumnHeaderMouseClick += Grid_ColumnHeaderMouseClick; // MENU KOLUMN

            // Synchronizacja filtrów
            _grid.ColumnWidthChanged += (s, e) => RepositionColumnFilters();
            _grid.Scroll += (s, e) => RepositionColumnFilters();
            _grid.Resize += (s, e) => RepositionColumnFilters();
            _grid.ColumnDisplayIndexChanged += (s, e) => RepositionColumnFilters();

            gridContainer.Controls.Add(_grid);
            gridContainer.Controls.Add(_filterRowPanel);

            // Składanie wszystkiego
            mainLayout.Controls.Add(headerPanel, 0, 0);
            mainLayout.Controls.Add(_sidePanel, 0, 1);
            mainLayout.Controls.Add(gridContainer, 1, 1);

            this.Controls.Add(mainLayout);
            this.Controls.Add(_loadingOverlay); // Overlay na wierzch

            // Definicja Kolumn
            SetupGridColumns();
        }

        private void SetupLoadingOverlay()
        {
            _loadingOverlay = new Panel { Dock = DockStyle.Fill, BackColor = Color.FromArgb(250, 250, 250) };
            
            var centerPanel = new Panel { AutoSize = true, Anchor = AnchorStyles.None };
            centerPanel.Location = new Point(this.Width/2 - 150, this.Height/2 - 50);
            
            _lblLoadingText = new Label { Text = "Ładowanie...", Font = new Font("Segoe UI Light", 24), AutoSize = true, Location = new Point(0, 0), ForeColor = Color.FromArgb(64, 64, 64) };
            _loadingProgress = new ProgressBar { Style = ProgressBarStyle.Marquee, Width = 300, Height = 5, Location = new Point(0, 50) };
            
            centerPanel.Controls.Add(_lblLoadingText);
            centerPanel.Controls.Add(_loadingProgress);
            
            _loadingOverlay.Controls.Add(centerPanel);
            
            // Centrowanie przy starcie
            _loadingOverlay.Resize += (s, e) => {
                centerPanel.Left = (_loadingOverlay.Width - centerPanel.Width) / 2;
                centerPanel.Top = (_loadingOverlay.Height - centerPanel.Height) / 2;
            };
        }

        private void ShowLoading(bool show, string text = "")
        {
            if (show)
            {
                _lblLoadingText.Text = text;
                _loadingOverlay.Visible = true;
                _loadingOverlay.BringToFront();
                Application.DoEvents(); // Wymuś rysowanie
            }
            else
            {
                _loadingOverlay.Visible = false;
                _loadingOverlay.SendToBack();
            }
        }

        // --- ZARZĄDZANIE KOLUMNAMI I FILTRAMI ---

        private void SetupGridColumns()
        {
            _grid.AutoGenerateColumns = false;
            _colFilters.Clear();
            _filterRowPanel.Controls.Clear();

            // Dodaj kolumny i filtry
            AddColumn("NrZgloszenia", "Nr Zgłoszenia", 120);
            AddColumn("DataZgloszenia", "Data", 100);
            AddColumn("Status", "Status", 120);
            AddColumn("Klient", "Klient", 150);
            AddColumn("Produkt", "Produkt", 200);
            AddColumn("SN", "S/N", 100);
            AddColumn("FV", "Faktura", 100);
            AddColumn("Skad", "Źródło", 80);
            AddColumn("Producent", "Producent", 120);

            RepositionColumnFilters();
        }

        private void AddColumn(string property, string header, int width)
        {
            // Grid Column
            var col = new DataGridViewTextBoxColumn { Name = property, DataPropertyName = property, HeaderText = header, Width = width };
            _grid.Columns.Add(col);

            // Filter TextBox
            var tb = new TextBox
            {
                Tag = property,
                BorderStyle = BorderStyle.FixedSingle,
                Font = new Font("Segoe UI", 9F),
                Height = 22,
                BackColor = Color.White
            };
            tb.TextChanged += (s, e) => ApplyFilters();
            
            // Estetyka fokusu
            tb.Enter += (s, e) => tb.BackColor = Color.Azure;
            tb.Leave += (s, e) => tb.BackColor = Color.White;

            _filterRowPanel.Controls.Add(tb);
            _colFilters.Add(property, tb);
        }

        private void RepositionColumnFilters()
        {
            if (_colFilters.Count == 0) return;

            int x = -_grid.HorizontalScrollingOffset;
            
            // Iterujemy po kolumnach w kolejności wyświetlania (DisplayIndex)
            var displayCols = _grid.Columns.Cast<DataGridViewColumn>().OrderBy(c => c.DisplayIndex).ToList();

            foreach (var col in displayCols)
            {
                if (_colFilters.TryGetValue(col.Name, out var tb))
                {
                    if (col.Visible)
                    {
                        tb.Visible = true;
                        // Pozycjonowanie z małym marginesem
                        tb.SetBounds(x + 1, 4, Math.Max(0, col.Width - 2), 22);
                    }
                    else
                    {
                        tb.Visible = false;
                    }
                }
                if (col.Visible) x += col.Width;
            }
        }

        // --- MENU KONTEKSTOWE KOLUMN (PRAWY KLIK) ---
        private void Grid_ColumnHeaderMouseClick(object sender, DataGridViewCellMouseEventArgs e)
        {
            if (e.Button == MouseButtons.Right)
            {
                var menu = new ContextMenuStrip();
                menu.Items.Add(new ToolStripLabel("Wybierz kolumny:") { Font = new Font("Segoe UI", 9, FontStyle.Bold) });
                menu.Items.Add(new ToolStripSeparator());

                foreach (DataGridViewColumn col in _grid.Columns)
                {
                    var item = new ToolStripMenuItem(col.HeaderText);
                    item.Checked = col.Visible;
                    item.Click += (s, ev) => 
                    { 
                        col.Visible = !col.Visible; 
                        RepositionColumnFilters(); // Przelicz pozycje textboxów
                    };
                    menu.Items.Add(item);
                }
                menu.Show(Cursor.Position);
            }
        }

        // --- FILTRY BOCZNE ---
        private void BuildSideFilters()
        {
            _sidePanel.Controls.Clear();
            
            var btnReset = new Button { Text = "WYCZYŚĆ WSZYSTKO", Height = 40, Dock = DockStyle.Top, FlatStyle = FlatStyle.Flat, BackColor = Color.IndianRed, ForeColor = Color.White, Font = new Font("Segoe UI", 9, FontStyle.Bold), Cursor = Cursors.Hand };
            btnReset.Click += (s, e) => ResetAllFilters();
            _sidePanel.Controls.Add(btnReset);

            CreateCheckGroup("Status", x => x.Status);
            CreateCheckGroup("Źródło", x => x.Skad);
            CreateCheckGroup("Producent", x => x.Producent);
        }

        private void CreateCheckGroup(string title, Func<ComplaintViewModel, string> selector)
        {
            var values = _allData.Select(selector).Where(x => !string.IsNullOrWhiteSpace(x)).Distinct().OrderBy(x => x).ToList();
            if (values.Count == 0) return;

            var gb = new GroupBox { Text = title, AutoSize = true, Padding = new Padding(5, 20, 5, 5), Font = new Font("Segoe UI", 9, FontStyle.Bold) };
            var flow = new FlowLayoutPanel { Dock = DockStyle.Fill, AutoSize = true, FlowDirection = FlowDirection.TopDown, WrapContents = false };

            foreach (var val in values)
            {
                var cb = new CheckBox { Text = val, AutoSize = true, Font = new Font("Segoe UI", 9, FontStyle.Regular), Tag = val, Cursor = Cursors.Hand };
                cb.CheckedChanged += (s, e) => {
                    if (!_sideFilters.ContainsKey(title)) _sideFilters[title] = new HashSet<string>();
                    if (cb.Checked) _sideFilters[title].Add(val); else _sideFilters[title].Remove(val);
                    ApplyFilters();
                };
                flow.Controls.Add(cb);
            }
            gb.Controls.Add(flow);
            _sidePanel.Controls.Add(gb);
        }

        private void ResetAllFilters()
        {
            _txtMainSearch.Text = "";
            foreach (var tb in _colFilters.Values) tb.Text = "";
            _sideFilters.Clear();
            
            // Odznacz checkboxy
            foreach(Control c in _sidePanel.Controls) 
                if(c is GroupBox gb) 
                    foreach(Control f in gb.Controls) 
                        if(f is FlowLayoutPanel flow) 
                            foreach(Control cb in flow.Controls) 
                                if(cb is CheckBox checkbox) checkbox.Checked = false;
            
            ApplyFilters();
        }

        // --- EKSPORT DO EXCELA ---
        private void ExportToExcel()
        {
            if (_filteredData.Count == 0) return;
            try
            {
                ShowLoading(true, "Eksportowanie do Excela...");
                var app = new Excel.Application();
                var wb = app.Workbooks.Add();
                var ws = (Excel.Worksheet)wb.Sheets[1];

                // Nagłówki
                int colIdx = 1;
                var visibleCols = _grid.Columns.Cast<DataGridViewColumn>().Where(c => c.Visible).OrderBy(c => c.DisplayIndex).ToList();
                foreach (var c in visibleCols) ws.Cells[1, colIdx++] = c.HeaderText;

                // Dane (Szybki zrzut tablicy)
                object[,] data = new object[_filteredData.Count, visibleCols.Count];
                for (int i = 0; i < _filteredData.Count; i++)
                {
                    var item = _filteredData[i];
                    for(int j=0; j<visibleCols.Count; j++)
                    {
                        // Reflection jest wolne, ale przy exporcie akceptowalne. 
                        // Można to przyspieszyć hardcodując switcha jak w ApplyFilters
                        var prop = item.GetType().GetProperty(visibleCols[j].DataPropertyName);
                        data[i, j] = prop?.GetValue(item) ?? "";
                    }
                }

                var range = ws.Range[ws.Cells[2, 1], ws.Cells[_filteredData.Count + 1, visibleCols.Count]];
                range.Value = data;
                ws.Columns.AutoFit();
                app.Visible = true;
            }
            catch (Exception ex) { MessageBox.Show(ex.Message); }
            finally { ShowLoading(false); }
        }
    }
}
