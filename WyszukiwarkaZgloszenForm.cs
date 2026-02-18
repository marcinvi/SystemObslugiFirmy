using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Text;
using System.Reflection;
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
        private DataGridView _grid;
        private TextBox _txtSearch;
        private Label _lblStats;
        private Panel _loadingOverlay;
        private Label _lblLoading;
        private Panel _filterPanelContainer;
        private Panel _filterPanel;

        private readonly Dictionary<string, Control> _columnFilters = new Dictionary<string, Control>();
        private Dictionary<string, List<string>> _multiSelectFilters = new Dictionary<string, List<string>>();
        private readonly Dictionary<string, Func<ComplaintViewModel, object>> _propertyAccessors = new Dictionary<string, Func<ComplaintViewModel, object>>(StringComparer.Ordinal);

        private readonly Timer _searchDebounceTimer = new Timer();
        private string _currentSortColumn;
        private bool _sortAscending = true;

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
            InitializeComponentManual(); // Zamiast SetupUI w konstruktorze
            BuildPropertyAccessorCache();

            // Fix na ładowanie UI: Używamy OnLoad zamiast Shown, czasem jest stabilniejsze dla layoutu
            this.Load += async (s, e) => await LoadDataAsync();
            EnableSpellCheckOnAllTextBoxes();
        }

        private void InitializeComponentManual()
        {
            // WAŻNE: Zatrzymujemy logikę rysowania na czas budowania UI
            this.SuspendLayout();

            this.Text = "Wyszukiwarka Zgłoszeń - Power Search";
            this.Font = new Font("Segoe UI", 9.5f);
            this.BackColor = Color.White;
            this.Size = new Size(1400, 800);
            this.StartPosition = FormStartPosition.CenterScreen;

            // --- GŁÓWNY KONTENER ---
            var mainPanel = new Panel { Dock = DockStyle.Fill };
            this.Controls.Add(mainPanel);

            // --- LOADING OVERLAY (Musi być dodany do this.Controls na końcu, żeby był NA WIERZCHU) ---
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
            // Nie dodajemy go jeszcze, dodamy na końcu metody

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

            // --- GRID ---
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
                ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.DisableResizing,
                RowTemplate = { Height = 30 },
                GridColor = Color.LightGray
            };

            _grid.ColumnHeadersDefaultCellStyle.BackColor = Color.FromArgb(235, 235, 235);
            _grid.ColumnHeadersDefaultCellStyle.Font = new Font("Segoe UI Semibold", 9.5f);
            _grid.ColumnHeadersDefaultCellStyle.ForeColor = Color.FromArgb(50, 50, 50);
            _grid.DefaultCellStyle.SelectionBackColor = Color.FromArgb(204, 232, 255);
            _grid.DefaultCellStyle.SelectionForeColor = Color.Black;
            _grid.DefaultCellStyle.Font = new Font("Segoe UI", 9f);
            _grid.DefaultCellStyle.WrapMode = DataGridViewTriState.False;

            _grid.CellDoubleClick += (s, e) =>
            {
                if (e.RowIndex >= 0)
                {
                    var item = _grid.Rows[e.RowIndex].DataBoundItem as ComplaintViewModel;
                    if (item != null) new Form2(item.NrZgloszenia).Show();
                }
            };

            _grid.CellFormatting += (s, e) =>
            {
                if (e.Value != null && e.Value.ToString().Length > 50)
                    _grid.Rows[e.RowIndex].Cells[e.ColumnIndex].ToolTipText = e.Value.ToString();
            };

            _grid.RowPrePaint += (s, e) =>
            {
                if (e.RowIndex >= 0 && e.RowIndex < _grid.Rows.Count)
                {
                    if (e.RowIndex % 2 == 0) _grid.Rows[e.RowIndex].DefaultCellStyle.BackColor = Color.FromArgb(250, 250, 250);

                    var item = _grid.Rows[e.RowIndex].DataBoundItem as ComplaintViewModel;
                    if (item != null && item.Skad != null && item.Skad.Contains("Allegro"))
                        _grid.Rows[e.RowIndex].DefaultCellStyle.BackColor = Color.FromArgb(255, 248, 230);
                }
            };

            _grid.ColumnWidthChanged += (s, e) => SyncFilterWidth(e.Column);
            _grid.ColumnDisplayIndexChanged += (s, e) => BuildColumnFilters();
            _grid.ColumnStateChanged += (s, e) => { if (e.StateChanged == DataGridViewElementStates.Visible) BuildColumnFilters(); };
            _grid.ColumnHeaderMouseClick += (s, e) => SortByColumn(_grid.Columns[e.ColumnIndex]);
            _grid.Scroll += (s, e) => { if (e.ScrollOrientation == ScrollOrientation.HorizontalScroll) _filterPanel.Left = -_grid.HorizontalScrollingOffset; };

            // --- KOLEJNOŚĆ DODAWANIA MA ZNACZENIE DLA DOCKINGU (Odwrócona logika) ---
            // 1. Grid (Fill)
            // 2. FilterPanel (Top)
            // 3. TopBar (Top)
            // W WinForms: ostatni dodany ma priorytet dokowania "na górze".

            mainPanel.Controls.Add(_grid);                // Na samym dole
            mainPanel.Controls.Add(_filterPanelContainer); // Nad gridem
            mainPanel.Controls.Add(topBar);               // Na samej górze

            // Dodajemy Overlay na sam wierzch formularza
            this.Controls.Add(_loadingOverlay);
            _loadingOverlay.BringToFront();

            SetupGridColumns();
            ConfigureSearchDebounce();

            // WAŻNE: Przywracamy rysowanie
            this.ResumeLayout(false);
            this.PerformLayout();
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
            _searchDebounceTimer.Interval = 350;
            _searchDebounceTimer.Tick += (s, e) =>
            {
                _searchDebounceTimer.Stop();
                ApplyFilters();
            };
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
                    Visible = colDef.VisibleByDefault,
                    MinimumWidth = 50
                };
                _grid.Columns.Add(col);
            }

            BuildColumnFilters();
        }

        private void BuildPropertyAccessorCache()
        {
            _propertyAccessors.Clear();
            var properties = typeof(ComplaintViewModel).GetProperties(BindingFlags.Instance | BindingFlags.Public);
            foreach (var property in properties)
            {
                _propertyAccessors[property.Name] = item => property.GetValue(item, null);
            }
        }

        private void BuildColumnFilters()
        {
            // Zatrzymujemy layout panelu filtrów na czas budowania
            _filterPanel.SuspendLayout();

            var textFiltersState = _columnFilters.Where(kvp => kvp.Value is TextBox).ToDictionary(k => k.Key, v => ((TextBox)v.Value).Text);
            _columnFilters.Clear();
            _filterPanel.Controls.Clear();

            int x = 0;
            var visibleColumns = _grid.Columns.Cast<DataGridViewColumn>()
                .Where(c => c.Visible)
                .OrderBy(c => c.DisplayIndex)
                .ToList();

            foreach (var column in visibleColumns)
            {
                bool isStatusCol = column.DataPropertyName.Contains("Status") ||
                                   column.DataPropertyName == "Kategoria" ||
                                   column.DataPropertyName == "Producent";

                Control ctrl;

                if (isStatusCol)
                {
                    var btn = new Button
                    {
                        Width = Math.Max(column.Width - 1, 10),
                        Height = 26,
                        Text = _multiSelectFilters.ContainsKey(column.DataPropertyName) && _multiSelectFilters[column.DataPropertyName].Any()
                               ? $"[{_multiSelectFilters[column.DataPropertyName].Count}]"
                               : "▼",
                        Tag = column.DataPropertyName,
                        Location = new Point(x, 4),
                        FlatStyle = FlatStyle.Flat,
                        BackColor = Color.White,
                        Font = new Font("Segoe UI", 8.5f),
                        TextAlign = ContentAlignment.MiddleCenter
                    };
                    btn.FlatAppearance.BorderColor = Color.Silver;
                    btn.FlatAppearance.BorderSize = 1;

                    if (_multiSelectFilters.ContainsKey(column.DataPropertyName) && _multiSelectFilters[column.DataPropertyName].Any())
                    {
                        btn.BackColor = Color.AliceBlue;
                        btn.Font = new Font("Segoe UI", 8.5f, FontStyle.Bold);
                    }

                    btn.Click += (s, e) => ShowMultiSelectFilter(column.DataPropertyName, btn);
                    ctrl = btn;
                }
                else
                {
                    var tb = new TextBox
                    {
                        Width = Math.Max(column.Width - 1, 10),
                        Height = 26,
                        Font = new Font("Segoe UI", 9.5f),
                        BorderStyle = BorderStyle.FixedSingle,
                        Tag = column.DataPropertyName,
                        Location = new Point(x, 4)
                    };
                    if (textFiltersState.TryGetValue(column.DataPropertyName, out string val)) tb.Text = val;
                    tb.TextChanged += (s, e) => ScheduleFilterUpdate();
                    ctrl = tb;
                }

                _filterPanel.Controls.Add(ctrl);
                _columnFilters[column.DataPropertyName] = ctrl;
                x += column.Width;
            }

            _filterPanel.Width = Math.Max(x, _filterPanelContainer.Width);
            _filterPanel.Left = -_grid.HorizontalScrollingOffset;

            // Przywracamy layout
            _filterPanel.ResumeLayout(true);
        }

        private void SyncFilterWidth(DataGridViewColumn column)
        {
            if (column == null || !column.Visible) return;
            if (_columnFilters.TryGetValue(column.DataPropertyName, out var ctrl))
            {
                ctrl.Width = Math.Max(column.Width - 1, 10);
                BuildColumnFilters();
            }
        }

        private async Task LoadDataAsync(bool forceRefresh = false)
        {
            ShowLoading(true);
            try
            {
                await Task.Run(async () =>
                {
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
                });
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
                {
                    // Używamy bezpiecznego focusowania
                    this.BeginInvoke(new Action(() => _txtSearch.Focus()));
                }
            }
        }

        private void ApplyFilters()
        {
            if (this.InvokeRequired)
            {
                this.Invoke(new Action(ApplyFilters));
                return;
            }

            // Zabezpieczenie przed migotaniem podczas filtrowania
            _grid.SuspendLayout();

            var queryPlan = SearchEngine.Compile(_txtSearch.Text);

            var textFilters = new List<Tuple<Func<ComplaintViewModel, object>, string>>();
            foreach (var kvp in _columnFilters)
            {
                if (!(kvp.Value is TextBox tb) || string.IsNullOrWhiteSpace(tb.Text)) continue;
                if (!_propertyAccessors.TryGetValue(kvp.Key, out var getter)) continue;
                textFilters.Add(Tuple.Create(getter, tb.Text.Trim()));
            }

            var multiFilters = new List<Tuple<Func<ComplaintViewModel, object>, HashSet<string>>>();
            foreach (var kvp in _multiSelectFilters)
            {
                if (kvp.Value == null || kvp.Value.Count == 0) continue;
                if (!_propertyAccessors.TryGetValue(kvp.Key, out var getter)) continue;
                multiFilters.Add(Tuple.Create(getter, new HashSet<string>(kvp.Value, StringComparer.Ordinal)));
            }

            var filtered = new List<ComplaintViewModel>(_allData.Count);

            foreach (var item in _allData)
            {
                if (!SearchEngine.Match(item.SearchVector, queryPlan)) continue;

                bool pass = true;

                for (int i = 0; i < textFilters.Count; i++)
                {
                    var value = textFilters[i].Item1(item)?.ToString() ?? string.Empty;
                    if (value.IndexOf(textFilters[i].Item2, StringComparison.OrdinalIgnoreCase) < 0)
                    {
                        pass = false;
                        break;
                    }
                }

                if (!pass) continue;

                for (int i = 0; i < multiFilters.Count; i++)
                {
                    var value = multiFilters[i].Item1(item)?.ToString() ?? string.Empty;
                    if (!multiFilters[i].Item2.Contains(value))
                    {
                        pass = false;
                        break;
                    }
                }

                if (pass) filtered.Add(item);
            }

            _filteredData = filtered;

            _grid.DataSource = _filteredData;

            _grid.ResumeLayout();
            _lblStats.Text = $"Wyniki: {_filteredData.Count} / {_allData.Count}";
            _lblStats.ForeColor = _filteredData.Count > 0 ? Color.Green : Color.Red;
        }

        private void ShowMultiSelectFilter(string propertyName, Button senderBtn)
        {
            if (!_propertyAccessors.TryGetValue(propertyName, out var getter)) return;

            var uniqueValues = _allData
                .Select(item => getter(item)?.ToString() ?? "")
                .Where(val => !string.IsNullOrEmpty(val))
                .Distinct()
                .OrderBy(val => val)
                .ToList();

            var currentSelection = _multiSelectFilters.ContainsKey(propertyName)
                ? _multiSelectFilters[propertyName]
                : new List<string>();

            using (var form = new MultiSelectFilterForm(uniqueValues, currentSelection))
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

        private void SortByColumn(DataGridViewColumn column)
        {
            if (column == null || string.IsNullOrWhiteSpace(column.DataPropertyName)) return;

            var propertyName = column.DataPropertyName;
            if (_currentSortColumn == propertyName) _sortAscending = !_sortAscending;
            else { _currentSortColumn = propertyName; _sortAscending = true; }

            Func<ComplaintViewModel, object> selector = item =>
            {
                if (propertyName == "NrZgloszenia") return ParseNrZgloszenia(item?.NrZgloszenia);
                if (_propertyAccessors.TryGetValue(propertyName, out var getter)) return getter(item);
                return null;
            };

            _filteredData = _sortAscending
                ? _filteredData.OrderBy(selector).ToList()
                : _filteredData.OrderByDescending(selector).ToList();

            _grid.DataSource = _filteredData;
        }

        private static Tuple<int, int> ParseNrZgloszenia(string nr)
        {
            if (string.IsNullOrWhiteSpace(nr)) return Tuple.Create(0, 0);
            var parts = nr.Split('/');
            if (parts.Length < 2) return Tuple.Create(0, 0);
            int.TryParse(parts[0], out int przed);
            int.TryParse(parts[1], out int po);
            return Tuple.Create(po, przed);
        }

        private void ShowColumnSelector(object sender, EventArgs e)
        {
            var availMap = _availableColumns.ToDictionary(c => c.PropertyName, c => c.DisplayName);
            var visMap = new Dictionary<string, bool>();

            foreach (var colDef in _availableColumns)
            {
                var gridCol = _grid.Columns[colDef.PropertyName];
                visMap[colDef.PropertyName] = gridCol != null && gridCol.Visible;
            }

            using (var form = new ColumnSelectorForm(availMap, visMap))
            {
                if (form.ShowDialog() == DialogResult.OK)
                {
                    _grid.SuspendLayout();

                    foreach (var kvp in form.Result)
                    {
                        var gridCol = _grid.Columns[kvp.Key];
                        if (gridCol != null)
                        {
                            gridCol.Visible = kvp.Value;
                        }
                        else if (kvp.Value)
                        {
                            var def = _availableColumns.First(c => c.PropertyName == kvp.Key);
                            _grid.Columns.Add(new DataGridViewTextBoxColumn
                            {
                                Name = def.PropertyName,
                                DataPropertyName = def.PropertyName,
                                HeaderText = def.DisplayName,
                                Width = def.Width,
                                Visible = true
                            });
                        }
                    }

                    int displayIdx = 0;
                    foreach (var def in _availableColumns)
                    {
                        if (_grid.Columns.Contains(def.PropertyName) && _grid.Columns[def.PropertyName].Visible)
                            _grid.Columns[def.PropertyName].DisplayIndex = displayIdx++;
                    }

                    _grid.ResumeLayout();
                    BuildColumnFilters();
                }
            }
        }

        private void ShowLoading(bool show)
        {
            if (this.InvokeRequired)
            {
                this.Invoke(new Action(() => ShowLoading(show)));
                return;
            }

            _loadingOverlay.Visible = show;

            if (show)
            {
                _loadingOverlay.BringToFront();
                _loadingOverlay.Refresh();
            }
            else
            {
                _loadingOverlay.SendToBack();

                // Wymuszenie pełnego odrysowania po schowaniu overlay'a.
                // Bez tego po pierwszym ładowaniu zdarza się, że textboxy/przyciski
                // są aktywne, ale nie są widoczne do czasu kliknięcia.
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

                    List<string> headers = new List<string>();
                    List<string> props = new List<string>();

                    this.Invoke(new Action(() => {
                        foreach (DataGridViewColumn c in _grid.Columns)
                        {
                            if (c.Visible)
                            {
                                headers.Add(c.HeaderText);
                                props.Add(c.DataPropertyName);
                            }
                        }
                    }));

                    for (int i = 0; i < headers.Count; i++) ws.Cells[1, i + 1] = headers[i];

                    object[,] data = new object[_filteredData.Count, headers.Count];
                    for (int i = 0; i < _filteredData.Count; i++)
                    {
                        var item = _filteredData[i];
                        for (int j = 0; j < props.Count; j++)
                        {
                            var prop = item.GetType().GetProperty(props[j]);
                            data[i, j] = prop?.GetValue(item) ?? "";
                        }
                    }

                    ws.Range[ws.Cells[2, 1], ws.Cells[_filteredData.Count + 1, headers.Count]].Value = data;
                    ws.Columns.AutoFit();
                    app.Visible = true;
                }
                catch (Exception ex) { MessageBox.Show($"Błąd exportu: {ex.Message}"); }
                finally { ShowLoading(false); }
            });
        }

        private void EnableSpellCheckOnAllTextBoxes()
        {
            foreach (Control c in GetAllControls(this)) if (c is RichTextBox r) r.EnableSpellCheck(true);
        }
        private IEnumerable<Control> GetAllControls(Control container)
        {
            foreach (Control c in container.Controls) { yield return c; if (c.HasChildren) foreach (Control child in GetAllControls(c)) yield return child; }
        }

        private class ColumnDefinition
        {
            public string PropertyName { get; set; }
            public string DisplayName { get; set; }
            public int Width { get; set; }
            public bool VisibleByDefault { get; set; }
            public ColumnDefinition(string p, string d, int w, bool v = true)
            {
                PropertyName = p; DisplayName = d; Width = w; VisibleByDefault = v;
            }
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
            return Match(searchVector, Compile(query));
        }

        internal static CompiledQuery Compile(string query)
        {
            if (string.IsNullOrWhiteSpace(query)) return CompiledQuery.Empty;

            var normalized = query.ToLowerInvariant();
            var tokens = ParseQuery(normalized);
            return new CompiledQuery(tokens, tokens.Any(t => t.Type == TokenType.OR));
        }

        internal static bool Match(string searchVector, CompiledQuery compiled)
        {
            if (compiled == null || compiled.IsEmpty) return true;
            if (string.IsNullOrEmpty(searchVector)) return false;

            searchVector = searchVector.ToLowerInvariant();
            var tokens = compiled.Tokens;
            bool hasOrOperator = compiled.HasOr;

            if (hasOrOperator)
            {
                bool anyMatch = false;
                bool excluded = false;
                foreach (var token in tokens)
                {
                    if (token.Type == TokenType.NOT) { if (searchVector.Contains(token.Value)) { excluded = true; break; } }
                    else if (token.Type == TokenType.Term) { if (searchVector.Contains(token.Value)) anyMatch = true; }
                }
                return anyMatch && !excluded;
            }
            else
            {
                foreach (var token in tokens)
                {
                    if (token.Type == TokenType.Term) { if (!searchVector.Contains(token.Value)) return false; }
                    else if (token.Type == TokenType.NOT) { if (searchVector.Contains(token.Value)) return false; }
                }
                return true;
            }
        }

        private static List<Token> ParseQuery(string query)
        {
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

        internal enum TokenType { Term, AND, OR, NOT }
        internal class Token { internal TokenType Type; internal string Value; }

        internal sealed class CompiledQuery
        {
            public static readonly CompiledQuery Empty = new CompiledQuery(new List<Token>(), false);

            public bool IsEmpty => Tokens.Count == 0;
            private List<Token> Tokens { get; }
            public bool HasOr { get; }

            private CompiledQuery(List<Token> tokens, bool hasOr)
            {
                Tokens = tokens ?? new List<Token>();
                HasOr = hasOr;
            }
        }
    }
}
