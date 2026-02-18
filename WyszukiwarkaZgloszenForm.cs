using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Text;
using System.Reflection;
using BrightIdeasSoftware;
using Excel = Microsoft.Office.Interop.Excel;

namespace Reklamacje_Dane
{
    public partial class WyszukiwarkaZgloszenForm : Form
    {
        // --- DANE I SERWISY ---
        private List<ComplaintViewModel> _allData = new List<ComplaintViewModel>();
        private List<ComplaintViewModel> _filteredData = new List<ComplaintViewModel>();
        private readonly FastDataService _service = new FastDataService();

        // --- ELEMENTY UI ---
        private FastObjectListView _olv;
        private TextBox _txtSearch;
        private Label _lblStats;
        private Panel _loadingOverlay;
        private Label _lblLoading;
        private Panel _filterPanelContainer;
        private Panel _filterPanel;

        private readonly Dictionary<string, Control> _columnFilters = new Dictionary<string, Control>();
        private Dictionary<string, List<string>> _multiSelectFilters = new Dictionary<string, List<string>>();

        private readonly Timer _searchDebounceTimer = new Timer();

        // --- CACHE ACCESSORÓW ---
        private static readonly Dictionary<string, Func<ComplaintViewModel, string>> _propertyAccessors;

        static WyszukiwarkaZgloszenForm()
        {
            _propertyAccessors = new Dictionary<string, Func<ComplaintViewModel, string>>();
            foreach (var prop in typeof(ComplaintViewModel).GetProperties(BindingFlags.Public | BindingFlags.Instance))
            {
                var p = prop;
                if (p.PropertyType == typeof(string))
                    _propertyAccessors[p.Name] = item => (string)p.GetValue(item) ?? "";
                else if (p.PropertyType == typeof(DateTime?))
                    _propertyAccessors[p.Name] = item => ((DateTime?)p.GetValue(item))?.ToString("yyyy-MM-dd") ?? "";
                else if (p.PropertyType == typeof(int?))
                    _propertyAccessors[p.Name] = item => ((int?)p.GetValue(item))?.ToString() ?? "";
                else if (p.PropertyType == typeof(double?))
                    _propertyAccessors[p.Name] = item => ((double?)p.GetValue(item))?.ToString() ?? "";
                else if (p.PropertyType == typeof(int))
                    _propertyAccessors[p.Name] = item => ((int)p.GetValue(item)).ToString();
                else
                    _propertyAccessors[p.Name] = item => p.GetValue(item)?.ToString() ?? "";
            }
        }

        // --- DEFINICJA KOLUMN ---
        private readonly List<ColumnDefinition> _availableColumns = new List<ColumnDefinition>
        {
            new ColumnDefinition("NrZgloszenia", "Nr Zgłoszenia", 120),
            new ColumnDefinition("DataZgloszenia", "Data", 100),
            new ColumnDefinition("Status", "Status", 120),
            new ColumnDefinition("Klient", "Klient", 150),
            new ColumnDefinition("Produkt", "Produkt", 200),
            new ColumnDefinition("Producent", "Producent", 120),
            new ColumnDefinition("SN", "S/N", 100),
            new ColumnDefinition("OpisUsterki", "Opis Usterki", 200, false),
            new ColumnDefinition("Dzialania", "Działania (Historia)", 250, false),
            new ColumnDefinition("Id", "ID", 60, false),
            new ColumnDefinition("KlientImieNazwisko", "Klient - Imię Nazwisko", 160, false),
            new ColumnDefinition("KlientNazwaFirmy", "Klient - Nazwa Firmy", 160, false),
            new ColumnDefinition("KlientNip", "NIP", 120, false),
            new ColumnDefinition("KlientEmail", "Klient - Email", 160, false),
            new ColumnDefinition("KlientTelefon", "Klient - Telefon", 140, false),
            new ColumnDefinition("KlientUlica", "Klient - Ulica", 160, false),
            new ColumnDefinition("KlientKodPocztowy", "Klient - Kod Pocztowy", 140, false),
            new ColumnDefinition("KlientMiejscowosc", "Klient - Miejscowość", 160, false),
            new ColumnDefinition("NazwaSystemowa", "Nazwa Systemowa", 170, false),
            new ColumnDefinition("NazwaKrotka", "Model", 150, false),
            new ColumnDefinition("KodEnova", "Kod Enova", 120, false),
            new ColumnDefinition("KodProducenta", "Kod Prod.", 120, false),
            new ColumnDefinition("Kategoria", "Kategoria", 140, false),
            new ColumnDefinition("ProduktWymagania", "Wymagania Produktu", 180, false),
            new ColumnDefinition("ProducentKontaktMail", "Producent - Kontakt Mail", 180, false),
            new ColumnDefinition("ProducentAdres", "Producent - Adres", 180, false),
            new ColumnDefinition("ProducentPlEng", "Producent - PL/ENG", 140, false),
            new ColumnDefinition("ProducentJezyk", "Producent - Język", 140, false),
            new ColumnDefinition("ProducentFormularz", "Producent - Formularz", 160, false),
            new ColumnDefinition("ProducentWymagania", "Producent - Wymagania", 180, false),
            new ColumnDefinition("FV", "Faktura", 100, false),
            new ColumnDefinition("Skad", "Źródło", 100, false),
            new ColumnDefinition("DataZakupu", "Data Zakupu", 120, false),
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
            new ColumnDefinition("NrFakturyKosztowej", "Nr Faktury Kosztowej", 160, false)
        };

        public WyszukiwarkaZgloszenForm()
        {
            this.DoubleBuffered = true;
            InitializeComponentManual();
            this.Load += async (s, e) => await LoadDataAsync();
        }

        private void InitializeComponentManual()
        {
            this.SuspendLayout();
            this.Text = "Wyszukiwarka Zgłoszeń - Power Search";
            this.Font = new Font("Segoe UI", 9.5f);
            this.BackColor = Color.White;
            this.Size = new Size(1400, 800);
            this.StartPosition = FormStartPosition.CenterScreen;

            var mainPanel = new Panel { Dock = DockStyle.Fill };
            this.Controls.Add(mainPanel);

            // --- LOADING OVERLAY ---
            _loadingOverlay = new Panel { Dock = DockStyle.Fill, BackColor = Color.FromArgb(245, 245, 245), Visible = true };
            _lblLoading = new Label
            {
                Text = "Ładowanie danych...",
                Font = new Font("Segoe UI Light", 18),
                ForeColor = Color.DimGray,
                AutoSize = true
            };
            _loadingOverlay.Controls.Add(_lblLoading);
            _loadingOverlay.Resize += (s, e) =>
            {
                _lblLoading.Left = (_loadingOverlay.Width - _lblLoading.Width) / 2;
                _lblLoading.Top = (_loadingOverlay.Height - _lblLoading.Height) / 2;
            };

            // --- TOP BAR ---
            var topBar = new Panel { Dock = DockStyle.Top, Height = 65, BackColor = Color.WhiteSmoke, Padding = new Padding(15) };

            var lblTitle = new Label { Text = "Szukaj:", Font = new Font("Segoe UI Semibold", 11), AutoSize = true, Location = new Point(15, 20) };

            _txtSearch = new TextBox
            {
                Width = 400,
                Font = new Font("Segoe UI", 11),
                Location = new Point(lblTitle.Right + 10, 17),
                BorderStyle = BorderStyle.FixedSingle
            };
            _txtSearch.TextChanged += (s, e) => ScheduleFilterUpdate();

            var btnHelp = new Button
            {
                Text = "?",
                Size = new Size(30, 27),
                Location = new Point(_txtSearch.Right + 5, 17),
                FlatStyle = FlatStyle.Flat,
                BackColor = Color.LightYellow,
                Cursor = Cursors.Help
            };
            btnHelp.FlatAppearance.BorderSize = 1;
            btnHelp.FlatAppearance.BorderColor = Color.Silver;
            new ToolTip().SetToolTip(btnHelp, "Wspiera: AND (spacja), OR (lub), NOT (-), \"fraza\"");

            var btnRefresh = CreateStyledButton("Odśwież", Color.White, Color.Black, new Point(btnHelp.Right + 20, 17));
            btnRefresh.Click += async (s, e) => await LoadDataAsync(true);

            var btnColumns = CreateStyledButton("Kolumny", Color.FromArgb(0, 120, 215), Color.White, new Point(btnRefresh.Right + 10, 17));
            btnColumns.Click += ShowColumnSelector;

            var btnExport = CreateStyledButton("Export Excel", Color.FromArgb(16, 124, 16), Color.White, new Point(btnColumns.Right + 10, 17));
            btnExport.Click += (s, e) => ExportToExcel();

            _lblStats = new Label
            {
                Text = "Oczekiwanie...",
                Font = new Font("Segoe UI", 10),
                AutoSize = true,
                Location = new Point(btnExport.Right + 20, 22),
                ForeColor = Color.DimGray
            };

            topBar.Controls.AddRange(new Control[] { lblTitle, _txtSearch, btnHelp, btnRefresh, btnColumns, btnExport, _lblStats });

            // --- FILTER PANEL ---
            _filterPanelContainer = new Panel { Dock = DockStyle.Top, Height = 34, BackColor = Color.WhiteSmoke };
            _filterPanel = new Panel { Dock = DockStyle.Fill, BackColor = Color.WhiteSmoke };
            _filterPanelContainer.Controls.Add(_filterPanel);

            // --- FastObjectListView (ZAMIAST DataGridView) ---
            _olv = new FastObjectListView
            {
                Dock = DockStyle.Fill,
                FullRowSelect = true,
                MultiSelect = false,
                ShowGroups = false,
                GridLines = true,
                BackColor = Color.White,
                UseAlternatingBackColors = true,
                AlternateRowBackColor = Color.FromArgb(250, 250, 250),
                BorderStyle = BorderStyle.None,
                HeaderStyle = ColumnHeaderStyle.Clickable,
                Font = new Font("Segoe UI", 9f),
                RowHeight = 30,
                View = View.Details,

                // Sortowanie po NrZgloszenia — customowe
                CustomSorter = (col, order) =>
                {
                    if (col.AspectName == "NrZgloszenia")
                    {
                        _olv.ListViewItemSorter = new NrZgloszeniaComparer(order);
                    }
                    else
                    {
                        _olv.ListViewItemSorter = new ColumnComparer(col, order);
                    }
                }
            };

            // Styl nagłówków
            _olv.HeaderFormatStyle = new HeaderFormatStyle();
            _olv.HeaderFormatStyle.Normal.BackColor = Color.FromArgb(235, 235, 235);
            _olv.HeaderFormatStyle.Normal.ForeColor = Color.FromArgb(50, 50, 50);
            _olv.HeaderFormatStyle.Normal.Font = new Font("Segoe UI Semibold", 9.5f);

            // Styl zaznaczenia
            _olv.HighlightBackgroundColor = Color.FromArgb(204, 232, 255);
            _olv.HighlightForegroundColor = Color.Black;
            _olv.UnfocusedHighlightBackgroundColor = Color.FromArgb(220, 235, 252);
            _olv.UnfocusedHighlightForegroundColor = Color.Black;

            // Kolorowanie wierszy Allegro
            _olv.FormatRow += (s, e) =>
            {
                var item = (ComplaintViewModel)e.Model;
                if (item?.Skad != null && item.Skad.Contains("Allegro"))
                    e.Item.BackColor = Color.FromArgb(255, 248, 230);
            };

            // Tooltip dla długich tekstów
            _olv.CellToolTipShowing += (s, e) =>
            {
                if (e.SubItem != null && e.SubItem.Text != null && e.SubItem.Text.Length > 50)
                    e.Text = e.SubItem.Text;
            };

            // Podwójne kliknięcie → otwórz Form2
            _olv.DoubleClick += (s, e) =>
            {
                var item = _olv.SelectedObject as ComplaintViewModel;
                if (item != null) new Form2(item.NrZgloszenia).Show();
            };

            // Synchronizacja filtrów przy zmianie szerokości kolumn
            _olv.ColumnWidthChanged += (s, e) => RecalcFilterPositions();

            // Sync horizontal scroll z panelem filtrów
            _olv.Scroll += (s, e) =>
            {
                var scrollPos = GetHorizontalScrollPosition(_olv);
                _filterPanel.Left = -scrollPos;
            };

            // --- KOLEJNOŚĆ DODAWANIA ---
            mainPanel.Controls.Add(_olv);
            mainPanel.Controls.Add(_filterPanelContainer);
            mainPanel.Controls.Add(topBar);

            this.Controls.Add(_loadingOverlay);
            _loadingOverlay.BringToFront();

            SetupColumns();
            ConfigureSearchDebounce();

            this.ResumeLayout(false);
            this.PerformLayout();
        }

        private int GetHorizontalScrollPosition(ListView lv)
        {
            // Odczytaj pozycję scrollowania z nagłówka ListView
            const int LVM_GETSCROLLINFO = 0x1000 + 20; // alternatywa
            try
            {
                if (lv.Items.Count > 0)
                {
                    var firstItem = lv.GetItemRect(0, ItemBoundsPortion.Entire);
                    return -firstItem.Left;
                }
            }
            catch { }
            return 0;
        }

        private Button CreateStyledButton(string text, Color bg, Color fg, Point loc)
        {
            var btn = new Button
            {
                Text = text,
                Size = new Size(110, 29),
                Location = loc,
                FlatStyle = FlatStyle.Flat,
                BackColor = bg,
                ForeColor = fg,
                Cursor = Cursors.Hand,
                Font = new Font("Segoe UI", 9)
            };
            btn.FlatAppearance.BorderSize = 1;
            btn.FlatAppearance.BorderColor = Color.FromArgb(200, 200, 200);
            return btn;
        }

        private void ConfigureSearchDebounce()
        {
            _searchDebounceTimer.Interval = 250;
            _searchDebounceTimer.Tick += (s, e) =>
            {
                _searchDebounceTimer.Stop();
                ApplyFilters();
            };
        }

        private void SetupColumns()
        {
            _olv.AllColumns.Clear();
            _olv.Columns.Clear();

            foreach (var colDef in _availableColumns)
            {
                var col = new OLVColumn
                {
                    Name = colDef.PropertyName,
                    AspectName = colDef.PropertyName,
                    Text = colDef.DisplayName,
                    Width = colDef.Width,
                    MinimumWidth = 50,
                    IsVisible = colDef.VisibleByDefault,
                    FillsFreeSpace = false
                };

                // Formatowanie dat
                if (colDef.PropertyName == "DataZgloszenia" || colDef.PropertyName == "DataZakupu")
                    col.AspectToStringFormat = "{0:yyyy-MM-dd}";

                _olv.AllColumns.Add(col);
            }

            _olv.RebuildColumns();
            BuildColumnFilters();
        }

        private void BuildColumnFilters()
        {
            _filterPanel.SuspendLayout();

            // Zapamiętaj tekst w filtrach
            var textFiltersState = new Dictionary<string, string>();
            foreach (var kvp in _columnFilters)
            {
                if (kvp.Value is TextBox tb && !string.IsNullOrEmpty(tb.Text))
                    textFiltersState[kvp.Key] = tb.Text;
            }

            _columnFilters.Clear();
            _filterPanel.Controls.Clear();

            int x = 0;
            foreach (ColumnHeader ch in _olv.Columns)
            {
                var col = ch as OLVColumn;
                if (col == null) continue;

                string key = col.AspectName;
                bool isStatusCol = key.Contains("Status") || key == "Kategoria" || key == "Producent";

                Control ctrl;

                if (isStatusCol)
                {
                    bool hasSelection = _multiSelectFilters.ContainsKey(key) && _multiSelectFilters[key].Any();

                    var btn = new Button
                    {
                        Width = Math.Max(col.Width - 1, 10),
                        Height = 26,
                        Text = hasSelection ? $"[{_multiSelectFilters[key].Count}]" : "▼",
                        Tag = key,
                        Location = new Point(x, 4),
                        FlatStyle = FlatStyle.Flat,
                        BackColor = hasSelection ? Color.AliceBlue : Color.White,
                        Font = new Font("Segoe UI", 8.5f, hasSelection ? FontStyle.Bold : FontStyle.Regular),
                        TextAlign = ContentAlignment.MiddleCenter
                    };
                    btn.FlatAppearance.BorderColor = Color.Silver;
                    btn.FlatAppearance.BorderSize = 1;
                    var capturedKey = key;
                    btn.Click += (s, e) => ShowMultiSelectFilter(capturedKey, btn);
                    ctrl = btn;
                }
                else
                {
                    var tb = new TextBox
                    {
                        Width = Math.Max(col.Width - 1, 10),
                        Height = 26,
                        Font = new Font("Segoe UI", 9.5f),
                        BorderStyle = BorderStyle.FixedSingle,
                        Tag = key,
                        Location = new Point(x, 4)
                    };
                    if (textFiltersState.TryGetValue(key, out string val))
                        tb.Text = val;
                    tb.TextChanged += (s, e) => ScheduleFilterUpdate();
                    ctrl = tb;
                }

                _filterPanel.Controls.Add(ctrl);
                _columnFilters[key] = ctrl;
                x += col.Width;
            }

            _filterPanel.Width = Math.Max(x, _filterPanelContainer.Width);
            _filterPanel.ResumeLayout(true);
        }

        private void RecalcFilterPositions()
        {
            int x = 0;
            foreach (ColumnHeader ch in _olv.Columns)
            {
                var col = ch as OLVColumn;
                if (col == null) continue;

                if (_columnFilters.TryGetValue(col.AspectName, out var ctrl))
                {
                    ctrl.Width = Math.Max(col.Width - 1, 10);
                    ctrl.Left = x;
                }
                x += col.Width;
            }
            _filterPanel.Width = Math.Max(x, _filterPanelContainer.Width);
        }

        private async Task LoadDataAsync(bool forceRefresh = false)
        {
            ShowLoading(true);
            try
            {
                // Podłącz progress do loading overlay
                _service.OnProgress = (msg) =>
                {
                    if (this.InvokeRequired)
                        this.BeginInvoke(new Action(() => { _lblLoading.Text = msg; _lblLoading.Refresh(); }));
                    else
                        { _lblLoading.Text = msg; _lblLoading.Refresh(); }
                };

                if (forceRefresh || !DataCache.Instance.HasData())
                {
                    if (forceRefresh) DataCache.Instance.Clear();
                    _allData = await _service.LoadAllComplaintsAsync();
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
                MessageBox.Show($"Błąd pobierania danych: {ex.Message}\n{ex.StackTrace}", "Błąd", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            finally
            {
                ShowLoading(false);
                if (this.Visible && _txtSearch.Enabled)
                    this.BeginInvoke(new Action(() => _txtSearch.Focus()));
            }
        }

        private void ApplyFilters()
        {
            if (this.InvokeRequired) { this.Invoke(new Action(ApplyFilters)); return; }

            var query = _txtSearch.Text;
            var parsedQuery = string.IsNullOrWhiteSpace(query) ? null : query;

            // Zbierz aktywne filtry
            var activeTextFilters = new List<KeyValuePair<string, string>>();
            foreach (var kvp in _columnFilters)
            {
                if (kvp.Value is TextBox tb && !string.IsNullOrWhiteSpace(tb.Text))
                    activeTextFilters.Add(new KeyValuePair<string, string>(kvp.Key, tb.Text.Trim().ToLowerInvariant()));
            }

            var activeMultiFilters = new List<KeyValuePair<string, HashSet<string>>>();
            foreach (var kvp in _multiSelectFilters)
            {
                if (kvp.Value != null && kvp.Value.Count > 0)
                    activeMultiFilters.Add(new KeyValuePair<string, HashSet<string>>(kvp.Key, new HashSet<string>(kvp.Value)));
            }

            bool hasQuery = parsedQuery != null;
            bool hasTextFilters = activeTextFilters.Count > 0;
            bool hasMultiFilters = activeMultiFilters.Count > 0;

            if (!hasQuery && !hasTextFilters && !hasMultiFilters)
            {
                _filteredData = _allData;
            }
            else
            {
                var result = new List<ComplaintViewModel>(_allData.Count / 2);

                for (int i = 0; i < _allData.Count; i++)
                {
                    var item = _allData[i];

                    if (hasQuery && !SearchEngine.Match(item.SearchVector, parsedQuery))
                        continue;

                    bool match = true;
                    if (hasTextFilters)
                    {
                        for (int f = 0; f < activeTextFilters.Count; f++)
                        {
                            var filter = activeTextFilters[f];
                            if (_propertyAccessors.TryGetValue(filter.Key, out var accessor))
                            {
                                if (accessor(item).IndexOf(filter.Value, StringComparison.OrdinalIgnoreCase) < 0)
                                { match = false; break; }
                            }
                        }
                        if (!match) continue;
                    }

                    if (hasMultiFilters)
                    {
                        for (int f = 0; f < activeMultiFilters.Count; f++)
                        {
                            var filter = activeMultiFilters[f];
                            if (_propertyAccessors.TryGetValue(filter.Key, out var accessor))
                            {
                                if (!filter.Value.Contains(accessor(item)))
                                { match = false; break; }
                            }
                        }
                        if (!match) continue;
                    }

                    result.Add(item);
                }
                _filteredData = result;
            }

            // === FastObjectListView: SetObjects — ZERO kaskady zdarzeń, ZERO migotania ===
            _olv.SetObjects(_filteredData);

            _lblStats.Text = $"Wyniki: {_filteredData.Count} / {_allData.Count}";
            _lblStats.ForeColor = _filteredData.Count > 0 ? Color.Green : Color.Red;
        }

        private void ShowMultiSelectFilter(string propertyName, Button senderBtn)
        {
            if (!_propertyAccessors.TryGetValue(propertyName, out var accessor)) return;

            var uniqueValues = new HashSet<string>();
            for (int i = 0; i < _allData.Count; i++)
            {
                var val = accessor(_allData[i]);
                if (!string.IsNullOrEmpty(val)) uniqueValues.Add(val);
            }

            var sortedValues = uniqueValues.OrderBy(v => v).ToList();
            var currentSelection = _multiSelectFilters.ContainsKey(propertyName)
                ? _multiSelectFilters[propertyName] : new List<string>();

            using (var form = new MultiSelectFilterForm(sortedValues, currentSelection))
            {
                var screenPoint = senderBtn.PointToScreen(new Point(0, senderBtn.Height));
                form.StartPosition = FormStartPosition.Manual;
                form.Location = screenPoint;

                if (form.ShowDialog() == DialogResult.OK)
                {
                    if (form.SelectedValues.Count == 0)
                        _multiSelectFilters.Remove(propertyName);
                    else
                        _multiSelectFilters[propertyName] = form.SelectedValues;

                    ApplyFilters();
                    BuildColumnFilters();
                }
            }
        }

        private void ScheduleFilterUpdate()
        {
            _searchDebounceTimer.Stop();
            _searchDebounceTimer.Start();
        }

        private void ShowColumnSelector(object sender, EventArgs e)
        {
            var availMap = _availableColumns.ToDictionary(c => c.PropertyName, c => c.DisplayName);
            var visMap = new Dictionary<string, bool>();

            foreach (var colDef in _availableColumns)
            {
                var olvCol = _olv.AllColumns.FirstOrDefault(c => c.AspectName == colDef.PropertyName);
                visMap[colDef.PropertyName] = olvCol != null && olvCol.IsVisible;
            }

            using (var form = new ColumnSelectorForm(availMap, visMap))
            {
                if (form.ShowDialog() == DialogResult.OK)
                {
                    foreach (var kvp in form.Result)
                    {
                        var olvCol = _olv.AllColumns.FirstOrDefault(c => c.AspectName == kvp.Key);
                        if (olvCol != null)
                            olvCol.IsVisible = kvp.Value;
                    }

                    _olv.RebuildColumns();
                    BuildColumnFilters();
                }
            }
        }

        private void ShowLoading(bool show)
        {
            if (this.InvokeRequired) { this.Invoke(new Action(() => ShowLoading(show))); return; }

            _loadingOverlay.Visible = show;
            if (show)
            {
                _loadingOverlay.BringToFront();
                _loadingOverlay.Refresh();
            }
            else
            {
                _loadingOverlay.SendToBack();
                this.Invalidate(true);
                this.Update();
            }
        }

        private void ExportToExcel()
        {
            if (_filteredData.Count == 0) return;
            ShowLoading(true);
            Task.Run(() =>
            {
                try
                {
                    var app = new Excel.Application();
                    var wb = app.Workbooks.Add();
                    var ws = (Excel.Worksheet)wb.Sheets[1];

                    // Pobierz widoczne kolumny
                    List<string> headers = new List<string>();
                    List<string> props = new List<string>();

                    this.Invoke(new Action(() =>
                    {
                        foreach (ColumnHeader ch in _olv.Columns)
                        {
                            var col = ch as OLVColumn;
                            if (col != null)
                            {
                                headers.Add(col.Text);
                                props.Add(col.AspectName);
                            }
                        }
                    }));

                    for (int i = 0; i < headers.Count; i++)
                        ws.Cells[1, i + 1] = headers[i];

                    var accessors = new List<Func<ComplaintViewModel, string>>();
                    foreach (var propName in props)
                    {
                        if (_propertyAccessors.TryGetValue(propName, out var acc))
                            accessors.Add(acc);
                        else
                            accessors.Add(_ => "");
                    }

                    object[,] data = new object[_filteredData.Count, headers.Count];
                    for (int i = 0; i < _filteredData.Count; i++)
                    {
                        var item = _filteredData[i];
                        for (int j = 0; j < accessors.Count; j++)
                            data[i, j] = accessors[j](item);
                    }

                    ws.Range[ws.Cells[2, 1], ws.Cells[_filteredData.Count + 1, headers.Count]].Value = data;
                    ws.Columns.AutoFit();
                    app.Visible = true;
                }
                catch (Exception ex) { MessageBox.Show($"Błąd exportu: {ex.Message}"); }
                finally { ShowLoading(false); }
            });
        }

        private class ColumnDefinition
        {
            public string PropertyName { get; set; }
            public string DisplayName { get; set; }
            public int Width { get; set; }
            public bool VisibleByDefault { get; set; }
            public ColumnDefinition(string p, string d, int w, bool v = true)
            { PropertyName = p; DisplayName = d; Width = w; VisibleByDefault = v; }
        }
    }

    // === Custom comparer dla sortowania Nr Zgłoszenia (np. "123/2024") ===
    public class NrZgloszeniaComparer : System.Collections.IComparer
    {
        private readonly SortOrder _order;
        public NrZgloszeniaComparer(SortOrder order) { _order = order; }

        public int Compare(object x, object y)
        {
            var a = (x as OLVListItem)?.RowObject as ComplaintViewModel;
            var b = (y as OLVListItem)?.RowObject as ComplaintViewModel;

            var ta = ParseNr(a?.NrZgloszenia);
            var tb = ParseNr(b?.NrZgloszenia);

            int result = ta.Item1.CompareTo(tb.Item1);
            if (result == 0) result = ta.Item2.CompareTo(tb.Item2);

            return _order == SortOrder.Descending ? -result : result;
        }

        private static Tuple<int, int> ParseNr(string nr)
        {
            if (string.IsNullOrWhiteSpace(nr)) return Tuple.Create(0, 0);
            var parts = nr.Split('/');
            if (parts.Length < 2) return Tuple.Create(0, 0);
            int.TryParse(parts[0], out int przed);
            int.TryParse(parts[1], out int po);
            return Tuple.Create(po, przed);
        }
    }

    // --- CACHE & SEARCH (BEZ ZMIAN) ---

    public sealed class DataCache
    {
        private static readonly Lazy<DataCache> _instance = new Lazy<DataCache>(() => new DataCache());
        public static DataCache Instance => _instance.Value;
        private List<ComplaintViewModel> _cachedData;
        private DateTime _lastUpdate;
        private DataCache() { }
        public bool HasData() => _cachedData != null && _cachedData.Count > 0;
        public List<ComplaintViewModel> GetData() => _cachedData;
        public void SetData(List<ComplaintViewModel> data) { _cachedData = data; _lastUpdate = DateTime.Now; }
        public void Clear() => _cachedData = null;
        public DateTime LastUpdate => _lastUpdate;
    }

    public class SearchEngine
    {
        public static bool Match(string searchVector, string query)
        {
            if (string.IsNullOrWhiteSpace(query)) return true;
            var tokens = ParseQuery(query.ToLowerInvariant());

            bool hasOrOperator = false;
            for (int i = 0; i < tokens.Count; i++)
            { if (tokens[i].Type == TokenType.OR) { hasOrOperator = true; break; } }

            if (hasOrOperator)
            {
                bool anyMatch = false;
                for (int i = 0; i < tokens.Count; i++)
                {
                    var token = tokens[i];
                    if (token.Type == TokenType.NOT) { if (searchVector.IndexOf(token.Value, StringComparison.Ordinal) >= 0) return false; }
                    else if (token.Type == TokenType.Term) { if (searchVector.IndexOf(token.Value, StringComparison.Ordinal) >= 0) anyMatch = true; }
                }
                return anyMatch;
            }
            else
            {
                for (int i = 0; i < tokens.Count; i++)
                {
                    var token = tokens[i];
                    if (token.Type == TokenType.Term) { if (searchVector.IndexOf(token.Value, StringComparison.Ordinal) < 0) return false; }
                    else if (token.Type == TokenType.NOT) { if (searchVector.IndexOf(token.Value, StringComparison.Ordinal) >= 0) return false; }
                }
                return true;
            }
        }

        private static string _lastQuery;
        private static List<Token> _lastTokens;

        private static List<Token> ParseQuery(string query)
        {
            if (query == _lastQuery && _lastTokens != null) return _lastTokens;

            var tokens = new List<Token>();
            var parts = SplitKeepingQuotes(query);
            for (int i = 0; i < parts.Count; i++)
            {
                string p = parts[i];
                if (p == "lub" || p == "or" || p == "|") { tokens.Add(new Token { Type = TokenType.OR }); continue; }
                if (p == "i" || p == "and" || p == "&" || p == "+") { tokens.Add(new Token { Type = TokenType.AND }); continue; }
                if (p.StartsWith("-") && p.Length > 1) { tokens.Add(new Token { Type = TokenType.NOT, Value = p.Substring(1) }); continue; }
                if (p == "bez" || p == "not") { if (i + 1 < parts.Count) { tokens.Add(new Token { Type = TokenType.NOT, Value = parts[i + 1] }); i++; } continue; }
                tokens.Add(new Token { Type = TokenType.Term, Value = p.Trim('"') });
            }

            _lastQuery = query;
            _lastTokens = tokens;
            return tokens;
        }

        private static List<string> SplitKeepingQuotes(string input)
        {
            var result = new List<string>();
            bool inQuote = false;
            int start = 0;
            for (int i = 0; i < input.Length; i++)
            {
                if (input[i] == '"') inQuote = !inQuote;
                else if (input[i] == ' ' && !inQuote)
                {
                    if (i > start) result.Add(input.Substring(start, i - start));
                    start = i + 1;
                }
            }
            if (start < input.Length) result.Add(input.Substring(start));
            return result.Where(s => !string.IsNullOrWhiteSpace(s)).ToList();
        }

        private enum TokenType { Term, AND, OR, NOT }
        private class Token { public TokenType Type; public string Value; }
    }
}
