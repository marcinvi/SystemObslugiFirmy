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
        private Button _btnExtraFilters;

        private readonly Dictionary<string, Control> _columnFilters = new Dictionary<string, Control>();
        private Dictionary<string, List<string>> _multiSelectFilters = new Dictionary<string, List<string>>();
        private Dictionary<string, DateFilterState> _dateFilters = new Dictionary<string, DateFilterState>();
        private ExtraFiltersState _extraFilters = new ExtraFiltersState(); // Nowe dodatkowe filtry

        private readonly Timer _searchDebounceTimer = new Timer();

        // --- CACHE ACCESSORÓW ---
        private static readonly Dictionary<string, Func<ComplaintViewModel, string>> _propertyAccessors;
        private static readonly Dictionary<string, Func<ComplaintViewModel, DateTime?>> _dateAccessors;

        static WyszukiwarkaZgloszenForm()
        {
            _propertyAccessors = new Dictionary<string, Func<ComplaintViewModel, string>>();
            _dateAccessors = new Dictionary<string, Func<ComplaintViewModel, DateTime?>>();

            foreach (var prop in typeof(ComplaintViewModel).GetProperties(BindingFlags.Public | BindingFlags.Instance))
            {
                var p = prop;
                if (p.PropertyType == typeof(DateTime?))
                {
                    _propertyAccessors[p.Name] = item => ((DateTime?)p.GetValue(item))?.ToString("yyyy-MM-dd") ?? "";
                    _dateAccessors[p.Name] = item => (DateTime?)p.GetValue(item);
                }
                else if (p.PropertyType == typeof(string))
                    _propertyAccessors[p.Name] = item => (string)p.GetValue(item) ?? "";
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
            new ColumnDefinition("NrZgloszenia", "NR", 70),
            new ColumnDefinition("DataZgloszenia", "Data Zgł.", 90),
            new ColumnDefinition("DataZakupu", "Data Zak.", 90),
            new ColumnDefinition("Klient", "Klient", 250),
            new ColumnDefinition("Produkt", "Produkt", 200), // FillsFreeSpace będzie ustawione w kodzie!
            new ColumnDefinition("Producent", "Producent", 110),
            new ColumnDefinition("SN", "SN", 110),
            new ColumnDefinition("NrWRL", "Nr WRL", 110),
            new ColumnDefinition("NrKWZ2", "Nr KWZ2", 110),
            new ColumnDefinition("StatusKlient", "Status klient", 180),
            new ColumnDefinition("StatusProducent", "Status producent", 220),

            // Ukryte
            new ColumnDefinition("Status", "Status Ogólny", 120, false),
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
            new ColumnDefinition("ProduktOpis", "Produkt (opis)", 200, false),
            new ColumnDefinition("AllegroBuyerLogin", "Allegro Login", 140, false),
            new ColumnDefinition("AllegroOrderId", "Allegro Order", 140, false),
            new ColumnDefinition("AllegroDisputeId", "Allegro Dispute", 140, false),
            new ColumnDefinition("AllegroAccountId", "Allegro Konto", 120, false),
            new ColumnDefinition("GwarancjaPlatna", "Gwarancja Płatna", 140, false),
            new ColumnDefinition("CzekamyNaDostawe", "Czekamy na Dostawę", 160, false),
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

            this.WindowState = FormWindowState.Maximized;
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

            // NOWY PRZYCISK: DODATKOWE FILTRY
            _btnExtraFilters = CreateStyledButton("Filtry Zawaans.", Color.FromArgb(255, 193, 7), Color.Black, new Point(btnRefresh.Right + 10, 17));
            _btnExtraFilters.Width = 140;
            _btnExtraFilters.Click += ShowExtraFilters;

            var btnColumns = CreateStyledButton("Kolumny", Color.FromArgb(0, 120, 215), Color.White, new Point(_btnExtraFilters.Right + 10, 17));
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

            topBar.Controls.AddRange(new Control[] { lblTitle, _txtSearch, btnHelp, btnRefresh, _btnExtraFilters, btnColumns, btnExport, _lblStats });

            // --- FILTER PANEL ---
            _filterPanelContainer = new Panel { Dock = DockStyle.Top, Height = 34, BackColor = Color.WhiteSmoke };
            _filterPanel = new Panel { Dock = DockStyle.Fill, BackColor = Color.WhiteSmoke };
            _filterPanelContainer.Controls.Add(_filterPanel);

            // --- FastObjectListView ---
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

                CustomSorter = (col, order) =>
                {
                    if (col.AspectName == "NrZgloszenia")
                        _olv.ListViewItemSorter = new NrZgloszeniaComparer(order);
                    else
                        _olv.ListViewItemSorter = new ColumnComparer(col, order);
                }
            };

            _olv.HeaderFormatStyle = new HeaderFormatStyle();
            _olv.HeaderFormatStyle.Normal.BackColor = Color.FromArgb(235, 235, 235);
            _olv.HeaderFormatStyle.Normal.ForeColor = Color.FromArgb(50, 50, 50);
            _olv.HeaderFormatStyle.Normal.Font = new Font("Segoe UI Semibold", 9.5f);

            _olv.HighlightBackgroundColor = Color.FromArgb(204, 232, 255);
            _olv.HighlightForegroundColor = Color.Black;
            _olv.UnfocusedHighlightBackgroundColor = Color.FromArgb(220, 235, 252);
            _olv.UnfocusedHighlightForegroundColor = Color.Black;

            _olv.FormatRow += (s, e) =>
            {
                var item = (ComplaintViewModel)e.Model;
                if (item?.Skad != null && item.Skad.Contains("Allegro"))
                    e.Item.BackColor = Color.FromArgb(255, 248, 230);
            };

            _olv.CellToolTipShowing += (s, e) =>
            {
                if (e.SubItem != null && e.SubItem.Text != null && e.SubItem.Text.Length > 50)
                    e.Text = e.SubItem.Text;
            };

            _olv.DoubleClick += (s, e) =>
            {
                var item = _olv.SelectedObject as ComplaintViewModel;
                if (item != null) new Form2(item.NrZgloszenia).Show();
            };

            _olv.ColumnWidthChanged += (s, e) => RecalcFilterPositions();
            _olv.Scroll += (s, e) =>
            {
                var scrollPos = GetHorizontalScrollPosition(_olv);
                _filterPanel.Left = -scrollPos;
            };

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
                    IsVisible = colDef.VisibleByDefault
                };

                // PRODUKT WYPEŁNIA RESZTĘ EKRANU
                if (colDef.PropertyName == "Produkt")
                {
                    col.FillsFreeSpace = true;
                    col.MinimumWidth = 150;
                }
                else
                {
                    col.FillsFreeSpace = false;
                }

                if (colDef.PropertyName.Contains("Data"))
                    col.AspectToStringFormat = "{0:yyyy-MM-dd}";

                _olv.AllColumns.Add(col);
            }

            _olv.RebuildColumns();
            BuildColumnFilters();
        }

        private void BuildColumnFilters()
        {
            _filterPanel.SuspendLayout();

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
                bool isDateCol = key.Contains("Data");

                Control ctrl;

                if (isDateCol)
                {
                    bool hasSelection = _dateFilters.ContainsKey(key) && _dateFilters[key].IsActive;
                    var btn = new Button
                    {
                        Width = Math.Max(col.Width - 1, 10),
                        Height = 26,
                        Text = hasSelection ? "[Aktywny]" : "▼",
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
                    btn.Click += (s, e) => ShowDateFilter(capturedKey, btn);
                    ctrl = btn;
                }
                else if (isStatusCol)
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
                    if (textFiltersState.TryGetValue(key, out string val)) tb.Text = val;
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
                _service.OnProgress = (msg) =>
                {
                    if (this.InvokeRequired) this.BeginInvoke(new Action(() => { _lblLoading.Text = msg; _lblLoading.Refresh(); }));
                    else { _lblLoading.Text = msg; _lblLoading.Refresh(); }
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
                MessageBox.Show($"Błąd pobierania danych: {ex.Message}", "Błąd", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            finally
            {
                ShowLoading(false);
                if (this.Visible && _txtSearch.Enabled) this.BeginInvoke(new Action(() => _txtSearch.Focus()));
            }
        }

        private void ApplyFilters()
        {
            if (this.InvokeRequired) { this.Invoke(new Action(ApplyFilters)); return; }

            var query = _txtSearch.Text;
            var parsedQuery = string.IsNullOrWhiteSpace(query) ? null : query;

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

            var activeDateFilters = _dateFilters.Where(x => x.Value.IsActive).ToList();

            bool hasQuery = parsedQuery != null;
            bool hasTextFilters = activeTextFilters.Count > 0;
            bool hasMultiFilters = activeMultiFilters.Count > 0;
            bool hasDateFilters = activeDateFilters.Count > 0;
            bool hasExtraFilters = _extraFilters.IsActive;

            if (!hasQuery && !hasTextFilters && !hasMultiFilters && !hasDateFilters && !hasExtraFilters)
            {
                _filteredData = _allData;
            }
            else
            {
                var result = new List<ComplaintViewModel>(_allData.Count / 2);
                DateTime now = DateTime.Now;

                for (int i = 0; i < _allData.Count; i++)
                {
                    var item = _allData[i];

                    if (hasQuery && !SearchEngine.Match(item.SearchVector, parsedQuery)) continue;

                    bool match = true;

                    // --- Filtry Dodatkowe (Wiek, Typ Klienta, Termin 14 Dni) ---
                    if (hasExtraFilters)
                    {
                        // Typ Klienta
                        if (_extraFilters.TypKlienta == 1 && string.IsNullOrWhiteSpace(item.KlientNip)) match = false; // Tylko B2B
                        if (_extraFilters.TypKlienta == 2 && !string.IsNullOrWhiteSpace(item.KlientNip)) match = false; // Tylko B2C

                        // Wiek urządzenia (Czas od zakupu do usterki)
                        if (match && _extraFilters.WiekUrzadzenia > 0)
                        {
                            var zakup = item.DataZakupu;
                            var zglosz = item.DataZgloszenia;
                            if (!zakup.HasValue || !zglosz.HasValue) match = false;
                            else
                            {
                                var dni = (zglosz.Value - zakup.Value).TotalDays;
                                if (_extraFilters.WiekUrzadzenia == 1 && dni > 30) match = false; // Do 30 dni
                                if (_extraFilters.WiekUrzadzenia == 2 && (dni <= 30 || dni > 180)) match = false; // 1-6 mies
                                if (_extraFilters.WiekUrzadzenia == 3 && (dni <= 180 || dni > 365)) match = false; // 6-12 mies
                                if (_extraFilters.WiekUrzadzenia == 4 && dni <= 365) match = false; // > 1 rok
                            }
                        }

                        // Przekroczone 14 dni
                        if (match && _extraFilters.Termin14Dni == 1)
                        {
                            if (!item.DataZgloszenia.HasValue) match = false;
                            else if ((now - item.DataZgloszenia.Value).TotalDays <= 14) match = false;
                        }
                    }
                    if (!match) continue;


                    // --- Filtry Tekstowe ---
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

                    // --- Filtry MultiSelect (Statusy) ---
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

                    // --- Filtry Dat ---
                    if (hasDateFilters)
                    {
                        for (int f = 0; f < activeDateFilters.Count; f++)
                        {
                            var filter = activeDateFilters[f];
                            if (_dateAccessors.TryGetValue(filter.Key, out var dateAccessor))
                            {
                                var dateVal = dateAccessor(item);
                                if (filter.Value.IsEmptyOnly)
                                {
                                    if (dateVal.HasValue) { match = false; break; }
                                }
                                else
                                {
                                    if (!dateVal.HasValue) { match = false; break; }
                                    if (filter.Value.DateFrom.HasValue && dateVal.Value.Date < filter.Value.DateFrom.Value.Date) { match = false; break; }
                                    if (filter.Value.DateTo.HasValue && dateVal.Value.Date > filter.Value.DateTo.Value.Date) { match = false; break; }
                                }
                            }
                        }
                        if (!match) continue;
                    }

                    result.Add(item);
                }
                _filteredData = result;
            }

            _olv.SetObjects(_filteredData);

            _lblStats.Text = $"Wyniki: {_filteredData.Count} / {_allData.Count}";
            _lblStats.ForeColor = _filteredData.Count > 0 ? Color.Green : Color.Red;

            _btnExtraFilters.BackColor = _extraFilters.IsActive ? Color.Orange : Color.FromArgb(255, 193, 7);
            _btnExtraFilters.Font = new Font(_btnExtraFilters.Font, _extraFilters.IsActive ? FontStyle.Bold : FontStyle.Regular);
            _btnExtraFilters.Text = _extraFilters.IsActive ? "Filtry Zaw. (ON)" : "Filtry Zaawans.";
        }

        private void ShowExtraFilters(object sender, EventArgs e)
        {
            using (var form = new ExtraFiltersForm(_extraFilters))
            {
                var screenPoint = _btnExtraFilters.PointToScreen(new Point(0, _btnExtraFilters.Height));
                form.StartPosition = FormStartPosition.Manual;
                form.Location = screenPoint;

                if (form.ShowDialog() == DialogResult.OK)
                {
                    _extraFilters = form.Result;
                    ApplyFilters();
                }
            }
        }

        private void ShowDateFilter(string propertyName, Button senderBtn)
        {
            var currentState = _dateFilters.ContainsKey(propertyName) ? _dateFilters[propertyName] : new DateFilterState();

            using (var form = new DateFilterForm(currentState))
            {
                var screenPoint = senderBtn.PointToScreen(new Point(0, senderBtn.Height));
                form.StartPosition = FormStartPosition.Manual;
                form.Location = screenPoint;

                if (form.ShowDialog() == DialogResult.OK)
                {
                    if (!form.Result.IsActive)
                        _dateFilters.Remove(propertyName);
                    else
                        _dateFilters[propertyName] = form.Result;

                    ApplyFilters();
                    BuildColumnFilters();
                }
            }
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
                        if (olvCol != null) olvCol.IsVisible = kvp.Value;
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
            if (show) { _loadingOverlay.BringToFront(); _loadingOverlay.Refresh(); }
            else { _loadingOverlay.SendToBack(); this.Invalidate(true); this.Update(); }
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

                    List<string> headers = new List<string>();
                    List<string> props = new List<string>();

                    this.Invoke(new Action(() =>
                    {
                        foreach (ColumnHeader ch in _olv.Columns)
                        {
                            var col = ch as OLVColumn;
                            if (col != null) { headers.Add(col.Text); props.Add(col.AspectName); }
                        }
                    }));

                    for (int i = 0; i < headers.Count; i++) ws.Cells[1, i + 1] = headers[i];

                    var accessors = new List<Func<ComplaintViewModel, string>>();
                    foreach (var propName in props)
                    {
                        if (_propertyAccessors.TryGetValue(propName, out var acc)) accessors.Add(acc);
                        else accessors.Add(_ => "");
                    }

                    object[,] data = new object[_filteredData.Count, headers.Count];
                    for (int i = 0; i < _filteredData.Count; i++)
                    {
                        var item = _filteredData[i];
                        for (int j = 0; j < accessors.Count; j++) data[i, j] = accessors[j](item);
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
            public ColumnDefinition(string p, string d, int w, bool v = true) { PropertyName = p; DisplayName = d; Width = w; VisibleByDefault = v; }
        }
    }

    // === KLASY POMOCNICZE DLA DODATKOWYCH FILTRÓW (WIEK URZĄDZENIA ITP) ===
    public class ExtraFiltersState
    {
        public int WiekUrzadzenia { get; set; } = 0; // 0=Wszystkie, 1=Do 30 dni, 2=1-6 mies, 3=6-12 mies, 4=Powyżej 1 roku
        public int TypKlienta { get; set; } = 0; // 0=Wszyscy, 1=Firma, 2=Konsument
        public int Termin14Dni { get; set; } = 0; // 0=Wszystkie, 1=Przekroczone 14 dni

        public bool IsActive => WiekUrzadzenia > 0 || TypKlienta > 0 || Termin14Dni > 0;
    }

    public class ExtraFiltersForm : Form
    {
        public ExtraFiltersState Result { get; private set; }
        private ComboBox cbWiek, cbTypKlienta, cbSla;

        public ExtraFiltersForm(ExtraFiltersState initialState)
        {
            this.FormBorderStyle = FormBorderStyle.FixedToolWindow;
            this.StartPosition = FormStartPosition.Manual;
            this.Width = 300;
            this.Height = 280;
            this.BackColor = Color.White;
            this.Font = new Font("Segoe UI", 9f);
            this.Text = "Filtry Zaawansowane";

            var lblWiek = new Label { Text = "Czas od zakupu do awarii:", Location = new Point(15, 15), AutoSize = true, Font = new Font("Segoe UI Semibold", 9f) };
            cbWiek = new ComboBox { Location = new Point(15, 35), Width = 250, DropDownStyle = ComboBoxStyle.DropDownList };
            cbWiek.Items.AddRange(new string[] { "Dowolny", "Do 30 dni (DOA)", "Od 1 do 6 miesięcy", "Od 6 do 12 miesięcy", "Powyżej 12 miesięcy" });
            cbWiek.SelectedIndex = initialState.WiekUrzadzenia;

            var lblTyp = new Label { Text = "Typ Klienta:", Location = new Point(15, 75), AutoSize = true, Font = new Font("Segoe UI Semibold", 9f) };
            cbTypKlienta = new ComboBox { Location = new Point(15, 95), Width = 250, DropDownStyle = ComboBoxStyle.DropDownList };
            cbTypKlienta.Items.AddRange(new string[] { "Wszyscy", "Tylko Firmy (B2B)", "Tylko Konsumenci (B2C)" });
            cbTypKlienta.SelectedIndex = initialState.TypKlienta;

            var lblSla = new Label { Text = "Czas otwartej reklamacji:", Location = new Point(15, 135), AutoSize = true, Font = new Font("Segoe UI Semibold", 9f) };
            cbSla = new ComboBox { Location = new Point(15, 155), Width = 250, DropDownStyle = ComboBoxStyle.DropDownList };
            cbSla.Items.AddRange(new string[] { "Wszystkie", "Przekroczone 14 dni od zgłoszenia" });
            cbSla.SelectedIndex = initialState.Termin14Dni;

            var btnApply = new Button { Text = "Zastosuj", Location = new Point(95, 200), Width = 100, BackColor = Color.FromArgb(0, 120, 215), ForeColor = Color.White, FlatStyle = FlatStyle.Flat };
            btnApply.Click += BtnApply_Click;

            this.Controls.AddRange(new Control[] { lblWiek, cbWiek, lblTyp, cbTypKlienta, lblSla, cbSla, btnApply });
            this.Deactivate += (s, e) => this.Close();
        }

        private void BtnApply_Click(object sender, EventArgs e)
        {
            Result = new ExtraFiltersState
            {
                WiekUrzadzenia = cbWiek.SelectedIndex,
                TypKlienta = cbTypKlienta.SelectedIndex,
                Termin14Dni = cbSla.SelectedIndex
            };
            this.DialogResult = DialogResult.OK;
            this.Close();
        }
    }


    // === KLASY POMOCNICZE DO FILTRU DATY ===
    public class DateFilterState
    {
        public bool IsEmptyOnly { get; set; }
        public DateTime? DateFrom { get; set; }
        public DateTime? DateTo { get; set; }
        public bool IsActive => IsEmptyOnly || DateFrom.HasValue || DateTo.HasValue;
    }

    public class DateFilterForm : Form
    {
        public DateFilterState Result { get; private set; }

        private RadioButton rbAll, rbEmpty, rbExact, rbRange;
        private DateTimePicker dtpExact, dtpFrom, dtpTo;

        public DateFilterForm(DateFilterState initialState)
        {
            this.FormBorderStyle = FormBorderStyle.FixedToolWindow;
            this.StartPosition = FormStartPosition.Manual;
            this.Width = 280;
            this.Height = 250;
            this.BackColor = Color.White;
            this.Font = new Font("Segoe UI", 9f);
            this.Text = "Filtruj Datę";

            rbAll = new RadioButton { Text = "Wszystkie (Zdejmij filtr)", Location = new Point(15, 15), Width = 200, Checked = !initialState.IsActive };
            rbEmpty = new RadioButton { Text = "Pokaż tylko puste (bez daty)", Location = new Point(15, 45), Width = 200, Checked = initialState.IsEmptyOnly };

            rbExact = new RadioButton { Text = "Konkretna data:", Location = new Point(15, 75), Width = 200 };
            dtpExact = new DateTimePicker { Location = new Point(35, 100), Width = 120, Format = DateTimePickerFormat.Short, Enabled = false };
            if (initialState.DateFrom.HasValue && initialState.DateFrom == initialState.DateTo) { rbExact.Checked = true; dtpExact.Value = initialState.DateFrom.Value; }

            rbRange = new RadioButton { Text = "Zakres dat:", Location = new Point(15, 130), Width = 200 };
            dtpFrom = new DateTimePicker { Location = new Point(35, 155), Width = 100, Format = DateTimePickerFormat.Short, Enabled = false };
            var lblDash = new Label { Text = "-", Location = new Point(140, 158), Width = 15 };
            dtpTo = new DateTimePicker { Location = new Point(155, 155), Width = 100, Format = DateTimePickerFormat.Short, Enabled = false };
            if (initialState.DateFrom.HasValue && initialState.DateFrom != initialState.DateTo) { rbRange.Checked = true; dtpFrom.Value = initialState.DateFrom.Value; }
            if (initialState.DateTo.HasValue && initialState.DateFrom != initialState.DateTo) { rbRange.Checked = true; dtpTo.Value = initialState.DateTo.Value; }

            rbAll.CheckedChanged += (s, e) => UpdateUI();
            rbEmpty.CheckedChanged += (s, e) => UpdateUI();
            rbExact.CheckedChanged += (s, e) => UpdateUI();
            rbRange.CheckedChanged += (s, e) => UpdateUI();

            var btnApply = new Button { Text = "Zastosuj", Location = new Point(85, 195), Width = 90, BackColor = Color.FromArgb(0, 120, 215), ForeColor = Color.White, FlatStyle = FlatStyle.Flat };
            btnApply.Click += BtnApply_Click;

            this.Controls.AddRange(new Control[] { rbAll, rbEmpty, rbExact, dtpExact, rbRange, dtpFrom, lblDash, dtpTo, btnApply });
            this.Deactivate += (s, e) => this.Close(); // Zamknij po kliknięciu obok
            UpdateUI();
        }

        private void UpdateUI()
        {
            dtpExact.Enabled = rbExact.Checked;
            dtpFrom.Enabled = rbRange.Checked;
            dtpTo.Enabled = rbRange.Checked;
        }

        private void BtnApply_Click(object sender, EventArgs e)
        {
            Result = new DateFilterState();
            if (rbEmpty.Checked) { Result.IsEmptyOnly = true; }
            else if (rbExact.Checked) { Result.DateFrom = dtpExact.Value.Date; Result.DateTo = dtpExact.Value.Date; }
            else if (rbRange.Checked) { Result.DateFrom = dtpFrom.Value.Date; Result.DateTo = dtpTo.Value.Date; }

            this.DialogResult = DialogResult.OK;
            this.Close();
        }
    }

    // === Custom comparer dla sortowania Nr Zgłoszenia ===
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

    // --- CACHE & SEARCH ---
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