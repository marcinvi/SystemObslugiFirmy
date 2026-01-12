using System;
using System.Collections.Generic;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;
using Excel = Microsoft.Office.Interop.Excel;
using System.Runtime.InteropServices;

namespace Reklamacje_Dane
{
    public partial class WyszukiwarkaZgloszenForm : Form
    {
        private DataTable dtZgloszeniaFull = new DataTable();
        private Timer filterDelayTimer;
        private string currentAdvancedFilter = "";
        private string currentAdvancedFilterName = "";

        private Dictionary<string, List<string>> activeTagFilters = new Dictionary<string, List<string>>();
        private readonly ComplaintSearchService _searchService = new ComplaintSearchService();

        // --- NOWE: per-kolumna filtr + operatory ---
        private Panel filterRowPanel;
        private Dictionary<string, TextBox> columnFilterBoxes = new Dictionary<string, TextBox>();
        private Dictionary<string, ComboBox> columnFilterOperators = new Dictionary<string, ComboBox>();

        public WyszukiwarkaZgloszenForm()
        {
            InitializeComponent();
            InitializeCustomComponents();
            SetupFilterDelayTimer();
        }

        private async void WyszukiwarkaZgloszenForm_Load(object sender, EventArgs e)
        {
            this.WindowState = FormWindowState.Maximized;

            // ✅ KROK 1: Załaduj dane
            await QueryAndFillDataAsync();

            // ✅ KROK 1.5: BRUTALNIE ukryj WSZYSTKIE kolumny NATYCHMIAST!
            foreach (DataGridViewColumn col in dataGridViewZgloszenia.Columns)
            {
                col.Visible = false;  // UKRYJ WSZYSTKO!
            }

            // ✅ KROK 2: POCZEKAJ aż UI się zaktualizuje, POTEM pokaż tylko 9
            this.BeginInvoke(new Action(() =>
            {
                // ✅ POKAŻ tylko 9
                SetDefaultColumnVisibility();
                PopulateColumnChooser();

                // ✅ KROK 3: Zbuduj textboxy nad kolumnami (dla widocznych kolumn)
                BuildColumnFilterRow();
                HookGridLayoutEvents();

                // ✅ KROK 5: Zastosuj filtry
                ApplyFiltersToDataView();
            }));

            // ✅ Załaduj filtry boczne (autocomplete + tag-filtry)
            var sw = System.Diagnostics.Stopwatch.StartNew();
            Console.WriteLine("⏱️ Start ładowania filtrów...");

            await SetupAllAutoCompleteAsync();
            Console.WriteLine($"✅ Autocomplete załadowane w {sw.ElapsedMilliseconds}ms");

            sw.Restart();
            await SetupAllTagFiltersAsync();
            Console.WriteLine($"✅ Tag-filtry załadowane w {sw.ElapsedMilliseconds}ms");
        }

        #region Initialization and Setup

        private TableLayoutPanel resultsLayout; // host na filtr + grid

        private void InitializeCustomComponents()
        {
            // ✅ WYŁĄCZ auto-generowanie kolumn (potem ręcznie ukryjemy niepotrzebne)
            dataGridViewZgloszenia.AutoGenerateColumns = true;

            // Styl nagłówków itp.
            dataGridViewZgloszenia.EnableHeadersVisualStyles = false;
            dataGridViewZgloszenia.ColumnHeadersDefaultCellStyle.BackColor = Color.FromArgb(45, 45, 48);
            dataGridViewZgloszenia.ColumnHeadersDefaultCellStyle.ForeColor = Color.White;
            dataGridViewZgloszenia.ColumnHeadersDefaultCellStyle.Font = new Font("Segoe UI", 10F, FontStyle.Bold);
            dataGridViewZgloszenia.DefaultCellStyle.Font = new Font("Segoe UI", 9.5F);
            dataGridViewZgloszenia.AlternatingRowsDefaultCellStyle.BackColor = Color.FromArgb(242, 242, 242);
            dataGridViewZgloszenia.RowTemplate.Height = 28;

            // --- NOWY układ: TableLayout wewnątrz panelWyniki (2 wiersze)
            resultsLayout = new TableLayoutPanel
            {
                Dock = DockStyle.Fill,
                ColumnCount = 1,
                RowCount = 2,
                BackColor = Color.White
            };
            resultsLayout.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100F));
            resultsLayout.RowStyles.Add(new RowStyle(SizeType.Absolute, 50F));  // wiersz filtrów + legenda
            resultsLayout.RowStyles.Add(new RowStyle(SizeType.Percent, 100F));  // grid

            // Przenieś istniejący grid do layoutu
            panelWyniki.Controls.Remove(dataGridViewZgloszenia);
            resultsLayout.Controls.Add(dataGridViewZgloszenia, 0, 1);
            panelWyniki.Controls.Add(resultsLayout);

            // Panel wiersza filtrów (już nie Dock=Top w panelu, tylko w komórce layoutu)
            filterRowPanel = new Panel
            {
                Dock = DockStyle.Fill,
                Height = 50,
                BackColor = Color.White
            };
            resultsLayout.Controls.Add(filterRowPanel, 0, 0);

            // Mniejsze migotanie przy przewijaniu
            typeof(DataGridView).GetProperty("DoubleBuffered",
                System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.NonPublic)
                ?.SetValue(dataGridViewZgloszenia, true, null);
            dataGridViewZgloszenia.RowPrePaint += DataGridViewZgloszenia_RowPrePaint;
        }

        private void DataGridViewZgloszenia_RowPrePaint(object sender, DataGridViewRowPrePaintEventArgs e)
        {
            var grid = (DataGridView)sender;
            if (e.RowIndex < 0 || e.RowIndex >= grid.Rows.Count) return;

            var row = grid.Rows[e.RowIndex];
            var skad = row.Cells["Skad"].Value?.ToString() ?? "";

            // Allegro? – miękki pomarańcz jak "warning"
            bool isAllegro = skad.StartsWith("Allegro", StringComparison.OrdinalIgnoreCase);

            // Kolory
            var allegroBack = Color.FromArgb(255, 243, 205);   // jasny pomarańcz (bootstrap warning-100)
            var allegroSel = Color.FromArgb(255, 193, 7);     // ciemniejszy przy zaznaczeniu
            var normalBack = Color.White;

            // Ustaw tylko raz dla całego wiersza: DefaultCellStyle
            if (isAllegro)
            {
                row.DefaultCellStyle.BackColor = allegroBack;
                row.DefaultCellStyle.SelectionBackColor = allegroSel;
                row.DefaultCellStyle.SelectionForeColor = Color.Black; // czytelność na pomarańczu
            }
            else
            {
                // wróć do "normalnego" tła (jeśli używasz AlternatingRows, możesz tu wykrywać parzystość)
                row.DefaultCellStyle.BackColor = normalBack;
                row.DefaultCellStyle.SelectionBackColor = grid.DefaultCellStyle.SelectionBackColor;
                row.DefaultCellStyle.SelectionForeColor = grid.DefaultCellStyle.SelectionForeColor;
            }
        }


        private void SetupFilterDelayTimer()
        {
            filterDelayTimer = new Timer { Interval = 350 };
            filterDelayTimer.Tick += (sender, e) =>
            {
                filterDelayTimer.Stop();
                ApplyFiltersToDataView();
            };
        }

        private async Task SetupAllAutoCompleteAsync()
        {
            // ✅ NAPRAWIONE: Bez Task.Run() - wykonuje się równolegle ale na wątku UI!
            var tasks = new List<Task>();

            tasks.Add(Task.Run(async () =>
            {
                try
                {
                    var data = await _searchService.GetDistinctValuesAsync("klienci", "ImieNazwisko");
                    // ✅ Invoke dla aktualizacji UI
                    this.Invoke(new Action(() => LoadAutoCompleteForTextBox(txtKlient, data)));
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"❌ BŁĄD: klienci.ImieNazwisko: {ex.Message}");
                }
            }));

            tasks.Add(Task.Run(async () =>
            {
                try
                {
                    var data = await _searchService.GetDistinctValuesAsync("Zgloszenia", "NrFaktury");
                    this.Invoke(new Action(() => LoadAutoCompleteForTextBox(txtNrFaktury, data)));
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"❌ BŁĄD: Zgloszenia.NrFaktury: {ex.Message}");
                }
            }));

            tasks.Add(Task.Run(async () =>
            {
                try
                {
                    var data = await _searchService.GetDistinctValuesAsync("Zgloszenia", "NrSeryjny");
                    this.Invoke(new Action(() => LoadAutoCompleteForTextBox(txtNrSeryjny, data)));
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"❌ BŁĄD: Zgloszenia.NrSeryjny: {ex.Message}");
                }
            }));

            tasks.Add(Task.Run(async () =>
            {
                try
                {
                    var data1 = await _searchService.GetDistinctValuesAsync("Produkty", "NazwaSystemowa");
                    var data2 = await _searchService.GetDistinctValuesAsync("Produkty", "NazwaKrotka");
                    var data3 = await _searchService.GetDistinctValuesAsync("Produkty", "KodProducenta");
                    var productValues = data1.Union(data2).Union(data3).Distinct().ToList();
                    this.Invoke(new Action(() => LoadAutoCompleteForTextBox(txtProdukt, productValues)));
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"❌ BŁĄD: Produkty: {ex.Message}");
                }
            }));

            await Task.WhenAll(tasks);
        }

        private async Task SetupAllTagFiltersAsync()
        {
            // ✅ NAPRAWIONE: Invoke dla aktualizacji UI
            var tasks = new List<Task>();

            tasks.Add(Task.Run(async () =>
            {
                try
                {
                    var data = await _searchService.GetDistinctValuesAsync("Zgloszenia", "StatusOgolny");
                    this.Invoke(new Action(() => CreateTagFilter(groupBoxStatusOgolny, "StatusOgolny", data)));
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"❌ BŁĄD: Zgloszenia.StatusOgolny: {ex.Message}");
                }
            }));

            tasks.Add(Task.Run(async () =>
            {
                try
                {
                    var data = await _searchService.GetDistinctValuesAsync("Zgloszenia", "StatusKlient");
                    this.Invoke(new Action(() => CreateTagFilter(groupBoxStatusKlient, "StatusKlient", data)));
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"❌ BŁĄD: Zgloszenia.StatusKlient: {ex.Message}");
                }
            }));

            tasks.Add(Task.Run(async () =>
            {
                try
                {
                    var data = await _searchService.GetDistinctValuesAsync("Zgloszenia", "StatusProducent");
                    this.Invoke(new Action(() => CreateTagFilter(groupBoxStatusProducent, "StatusProducent", data)));
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"❌ BŁĄD: Zgloszenia.StatusProducent: {ex.Message}");
                }
            }));

            tasks.Add(Task.Run(async () =>
            {
                try
                {
                    var data = await _searchService.GetDistinctValuesAsync("Producenci", "NazwaProducenta");
                    this.Invoke(new Action(() => CreateTagFilter(groupBoxProducent, "NazwaProducenta", data)));
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"❌ BŁĄD: Producenci.NazwaProducenta: {ex.Message}");
                }
            }));

            tasks.Add(Task.Run(async () =>
            {
                try
                {
                    var data = await _searchService.GetDistinctValuesAsync("Zgloszenia", "Skad");
                    this.Invoke(new Action(() => CreateTagFilter(groupBoxSkad, "Skad", data)));
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"❌ BŁĄD: Zgloszenia.Skad: {ex.Message}");
                }
            }));

            await Task.WhenAll(tasks);
        }

        private void SetDefaultColumnVisibility()
        {
            var defaultColumns = new HashSet<string>
            {
                "NrZgloszenia", "DataZgloszenia", "StatusKlient", "StatusProducent",
                "ImieNazwisko", "NazwaFirmy", "Kategoria", "NazwaKrotka", "NazwaProducenta"
            };

            // ✅ ITERUJ 2 RAZY dla pewności (czasem trzeba)
            for (int i = 0; i < 2; i++)
            {
                foreach (DataGridViewColumn col in dataGridViewZgloszenia.Columns)
                {
                    col.Visible = defaultColumns.Contains(col.Name);
                }
            }
        }

        #endregion

        #region Data Loading

        private async Task QueryAndFillDataAsync()
        {
            try
            {
                dtZgloszeniaFull = await _searchService.GetComplaintsAsync();
                dataGridViewZgloszenia.DataSource = dtZgloszeniaFull;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[WyszukiwarkaZgloszenForm] Błąd pobierania danych: {ex}");
                MessageBox.Show("Wystąpił błąd podczas pobierania danych:\n" + ex.Message, "Błąd Krytyczny", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void LoadAutoCompleteForTextBox(TextBox textBox, List<string> data)
        {
            var autoCompleteCollection = new AutoCompleteStringCollection();
            autoCompleteCollection.AddRange(data.ToArray());
            textBox.AutoCompleteCustomSource = autoCompleteCollection;
            textBox.AutoCompleteMode = AutoCompleteMode.Suggest;
            textBox.AutoCompleteSource = AutoCompleteSource.CustomSource;
        }

        #endregion

        #region Column Filter Row Z OPERATORAMI

        private void BuildColumnFilterRow()
        {
            filterRowPanel.SuspendLayout();
            filterRowPanel.Controls.Clear();
            columnFilterBoxes.Clear();
            columnFilterOperators.Clear();

            // ✅ LEGENDA na górze
            var legend = new Label
            {
                Text = "💡 [ ]=zawiera | [I]=wszystkie słowa | [LUB]=którekolwiek | [BEZ]=nie zawiera | [=]=dokładnie",
                AutoSize = true,
                Location = new Point(5, 2),
                Font = new Font("Segoe UI", 7.5F, FontStyle.Italic),
                ForeColor = Color.Gray,
                BackColor = Color.FromArgb(250, 250, 250),
                Padding = new Padding(3, 1, 3, 1)
            };
            filterRowPanel.Controls.Add(legend);

            // Start wyrównania względem scrolla poziomego
            int x = -dataGridViewZgloszenia.HorizontalScrollingOffset;
            int yOperator = 18;  // Operator pod legendą
            int yTextBox = 18;   // TextBox obok

            // Użyj kolejności widocznej (DisplayIndex), nie kolejności kolekcji
            var visibleOrdered = dataGridViewZgloszenia.Columns
                .Cast<DataGridViewColumn>()
                .Where(c => c.Visible)
                .OrderBy(c => c.DisplayIndex)
                .ToList();

            foreach (var col in visibleOrdered)
            {
                // ComboBox z operatorami (45px szerokości)
                var cbo = new ComboBox
                {
                    Tag = col.Name,
                    DropDownStyle = ComboBoxStyle.DropDownList,
                    Font = new Font("Segoe UI", 8F),
                    Width = 45,
                    Height = 22
                };
                cbo.Items.AddRange(new string[] { "", "I", "LUB", "BEZ", "=" });
                cbo.SelectedIndex = 0;  // Domyślnie puste (zawiera)
                cbo.SelectedIndexChanged += (s, e) => { filterDelayTimer.Stop(); filterDelayTimer.Start(); };

                cbo.SetBounds(x + 1, yOperator, 45, 22);
                filterRowPanel.Controls.Add(cbo);
                columnFilterOperators[col.Name] = cbo;

                // TextBox z filtrem
                var tb = new TextBox
                {
                    Tag = col.Name,
                    BorderStyle = BorderStyle.FixedSingle,
                    Font = new Font("Segoe UI", 9F)
                };
                tb.TextChanged += ColumnFilter_TextChanged;

                tb.SetBounds(x + 47, yTextBox, Math.Max(40, col.Width - 48), 22);
                filterRowPanel.Controls.Add(tb);
                columnFilterBoxes[col.Name] = tb;

                x += col.Width;
            }

            filterRowPanel.ResumeLayout();

            // Na wszelki wypadek dopasuj pozycje (gdyby ktoś zmienił szerokość)
            RepositionFilterBoxes();
        }

        private void ColumnFilter_TextChanged(object sender, EventArgs e)
        {
            filterDelayTimer.Stop();
            filterDelayTimer.Start();
        }

        private void HookGridLayoutEvents()
        {
            dataGridViewZgloszenia.Scroll += (s, e) => RepositionFilterBoxes();
            dataGridViewZgloszenia.ColumnWidthChanged += (s, e) => RepositionFilterBoxes();
            dataGridViewZgloszenia.ColumnDisplayIndexChanged += (s, e) => BuildColumnFilterRow();
            dataGridViewZgloszenia.Resize += (s, e) => RepositionFilterBoxes();
        }

        private void RepositionFilterBoxes()
        {
            if (columnFilterBoxes.Count == 0) return;

            int x = -dataGridViewZgloszenia.HorizontalScrollingOffset;
            int yOperator = 18;
            int yTextBox = 18;

            foreach (var col in dataGridViewZgloszenia.Columns.Cast<DataGridViewColumn>().OrderBy(c => c.DisplayIndex))
            {
                if (!col.Visible) continue;

                if (columnFilterOperators.TryGetValue(col.Name, out var cbo))
                {
                    cbo.Left = x + 1;
                    cbo.Top = yOperator;
                    cbo.Width = 45;
                }

                if (columnFilterBoxes.TryGetValue(col.Name, out var tb))
                {
                    tb.Left = x + 47;
                    tb.Top = yTextBox;
                    tb.Width = Math.Max(40, col.Width - 48);
                    tb.Height = 22;
                }

                x += col.Width;
            }
        }


        #endregion

        #region Filtering Logic Z OPERATORAMI

        private void OnFilterChanged(object sender, EventArgs e)
        {
            filterDelayTimer.Stop();
            filterDelayTimer.Start();
        }

        private void ApplyFiltersToDataView()
        {
            var filterExpression = new List<string>();

            // 1) Szukanie ogólne
            if (!string.IsNullOrWhiteSpace(txtSzukajOgolnie.Text))
            {
                string escVal = EscapeLike(txtSzukajOgolnie.Text);
                var conditions = new[]
                {
                    "NrZgloszenia","ImieNazwisko","NazwaFirmy","NIP","Email","Telefon",
                    "KodProducenta","NazwaSystemowa","NazwaKrotka","KodEnova","Kategoria",
                    "NazwaProducenta","NrSeryjny","NrFaktury","OpisUsterki","Dzialania"
                };
                filterExpression.Add($"({string.Join(" OR ", conditions.Select(c => $"CONVERT([{c}], 'System.String') LIKE '%{escVal}%'"))})");
            }

            // 2) Konkrety z panelu po lewej
            AddTextFilterToExpression(filterExpression, "NrZgloszenia", txtNrZgloszenia.Text);
            AddTextFilterToExpression(filterExpression, "ImieNazwisko", txtKlient.Text);
            AddTextFilterToExpression(filterExpression,
                "(CONVERT([NazwaSystemowa], 'System.String') LIKE '%{0}%' OR " +
                " CONVERT([KodProducenta], 'System.String') LIKE '%{0}%' OR " +
                " CONVERT([NazwaKrotka], 'System.String') LIKE '%{0}%' OR " +
                " CONVERT([KodEnova], 'System.String') LIKE '%{0}%')",
                txtProdukt.Text);
            AddTextFilterToExpression(filterExpression, "NrSeryjny", txtNrSeryjny.Text);
            AddTextFilterToExpression(filterExpression, "NrFaktury", txtNrFaktury.Text);

            // 3) Tag-filtry
            foreach (var entry in activeTagFilters)
            {
                if (entry.Value.Any())
                {
                    string inValues = string.Join("', '", entry.Value.Select(v => v.Replace("'", "''")));
                    filterExpression.Add($"[{entry.Key}] IN ('{inValues}')");
                }
            }

            // 4) ✅ Wiersz filtrów kolumnowych Z OPERATORAMI
            foreach (var kv in columnFilterBoxes)
            {
                var col = kv.Key;
                var val = kv.Value.Text;
                if (string.IsNullOrWhiteSpace(val) || !dtZgloszeniaFull.Columns.Contains(col)) continue;

                // Pobierz operator
                string op = "";
                if (columnFilterOperators.TryGetValue(col, out var cbo))
                {
                    op = cbo.SelectedItem?.ToString() ?? "";
                }

                string filterClause = BuildColumnFilterClause(col, val, op);
                if (!string.IsNullOrWhiteSpace(filterClause))
                {
                    filterExpression.Add(filterClause);
                }
            }

            // 5) Zaawansowany
            if (!string.IsNullOrWhiteSpace(currentAdvancedFilter))
            {
                filterExpression.Add(currentAdvancedFilter);
            }

            try
            {
                dtZgloszeniaFull.DefaultView.RowFilter = string.Join(" AND ", filterExpression);
            }
            catch
            {
                dtZgloszeniaFull.DefaultView.RowFilter = "";
            }

            lblRecordCount.Text = $"Znaleziono: {dtZgloszeniaFull.DefaultView.Count}";
            lblActiveAdvancedFilter.Text = string.IsNullOrWhiteSpace(currentAdvancedFilterName) ? "" : $"Aktywny filtr: {currentAdvancedFilterName}";
        }

        private string BuildColumnFilterClause(string columnName, string value, string op)
        {
            string escaped = EscapeLike(value);
            string colStr = $"CONVERT([{columnName}], 'System.String')";

            switch (op)
            {
                case "I":  // AND - wszystkie słowa
                    var words = value.Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
                    if (words.Length == 0) return "";
                    var andClauses = words.Select(w => $"{colStr} LIKE '%{EscapeLike(w)}%'");
                    return $"({string.Join(" AND ", andClauses)})";

                case "LUB":  // OR - którekolwiek słowo
                    var wordsOr = value.Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
                    if (wordsOr.Length == 0) return "";
                    var orClauses = wordsOr.Select(w => $"{colStr} LIKE '%{EscapeLike(w)}%'");
                    return $"({string.Join(" OR ", orClauses)})";

                case "BEZ":  // NOT - nie zawiera
                    return $"(NOT {colStr} LIKE '%{escaped}%' OR {colStr} IS NULL)";

                case "=":  // Dokładnie
                    return $"{colStr} = '{escaped}'";

                default:  // "" - zawiera (domyślnie)
                    return $"{colStr} LIKE '%{escaped}%'";
            }
        }

        private static string EscapeLike(string value) => value.Replace("'", "''");

        private void AddTextFilterToExpression(List<string> expression, string columnName, string value)
        {
            if (!string.IsNullOrWhiteSpace(value))
            {
                string safeValue = EscapeLike(value);
                expression.Add(columnName.Contains("{0}")
                    ? string.Format(columnName, safeValue)
                    : $"CONVERT([{columnName}], 'System.String') LIKE '%{safeValue}%'");
            }
        }

        #endregion

        #region Tag-Based Filter UI (TWÓJ STARY KOD - BEZ ZMIAN!)

        private void CreateTagFilter(GroupBox groupBox, string columnName, List<string> allValues)
        {
            activeTagFilters[columnName] = new List<string>();
            groupBox.Controls.Clear();

            var flpSelected = new FlowLayoutPanel { Dock = DockStyle.Top, AutoSize = true, MaximumSize = new Size(groupBox.Width - 10, 80), AutoScroll = true };
            var txtSearch = new TextBox { Dock = DockStyle.Top };
            var lstSuggestions = new ListBox { Dock = DockStyle.Fill, Visible = false };

            txtSearch.TextChanged += (s, e) =>
            {
                string searchTerm = txtSearch.Text.ToLower();
                if (string.IsNullOrWhiteSpace(searchTerm))
                {
                    lstSuggestions.Visible = false;
                    return;
                }
                var suggestions = allValues
                    .Where(v => v != null && v.ToLower().Contains(searchTerm) && !activeTagFilters[columnName].Contains(v))
                    .ToList();
                lstSuggestions.DataSource = suggestions;
                lstSuggestions.Visible = suggestions.Any();
            };

            lstSuggestions.MouseClick += (s, e) =>
            {
                if (lstSuggestions.SelectedItem == null) return;
                string selectedValue = lstSuggestions.SelectedItem.ToString();

                activeTagFilters[columnName].Add(selectedValue);
                AddTagLabel(flpSelected, selectedValue, columnName);

                txtSearch.Clear();
                lstSuggestions.Visible = false;
                OnFilterChanged(s, e);
            };

            groupBox.Controls.Add(lstSuggestions);
            groupBox.Controls.Add(txtSearch);
            groupBox.Controls.Add(flpSelected);
        }

        private void AddTagLabel(FlowLayoutPanel panel, string value, string columnName)
        {
            var tagLabel = new Label
            {
                Text = value + " ⓧ",
                Padding = new Padding(4),
                Margin = new Padding(2),
                AutoSize = true,
                BackColor = Color.FromArgb(0, 120, 215),
                ForeColor = Color.White,
                Font = new Font("Segoe UI", 9F, FontStyle.Bold),
                Cursor = Cursors.Hand,
                Tag = value
            };
            tagLabel.Click += (s, e) =>
            {
                activeTagFilters[columnName].Remove(value);
                panel.Controls.Remove(tagLabel);
                OnFilterChanged(s, e);
            };
            panel.Controls.Add(tagLabel);
        }

        #endregion

        #region Buttons and Menus

        private void btnDodatkoweFiltry_Click(object sender, EventArgs e)
        {
            contextMenuDodatkoweFiltry.Show(btnDodatkoweFiltry, new Point(0, btnDodatkoweFiltry.Height));
        }

        private void SetAdvancedFilter(string filter, string name)
        {
            currentAdvancedFilter = filter;
            currentAdvancedFilterName = name;
            ApplyFiltersToDataView();
        }

        private void ClearAllFilters()
        {
            currentAdvancedFilter = "";
            currentAdvancedFilterName = "";

            txtSzukajOgolnie.Clear();
            txtNrZgloszenia.Clear();
            txtKlient.Clear();
            txtProdukt.Clear();
            txtNrSeryjny.Clear();
            txtNrFaktury.Clear();

            foreach (var key in activeTagFilters.Keys.ToList()) activeTagFilters[key].Clear();

            foreach (var gb in panelFiltry.Controls.OfType<GroupBox>())
            {
                gb.Controls.OfType<FlowLayoutPanel>().FirstOrDefault()?.Controls.Clear();
                gb.Controls.OfType<TextBox>().FirstOrDefault()?.Clear();
            }

            foreach (var tb in columnFilterBoxes.Values) tb.Clear();
            foreach (var cbo in columnFilterOperators.Values) cbo.SelectedIndex = 0;

            ApplyFiltersToDataView();
        }

        private void btnWyczyscFiltry_Click(object sender, EventArgs e) => ClearAllFilters();

        private void btnExportExcel_Click(object sender, EventArgs e)
        {
            var view = dtZgloszeniaFull.DefaultView;
            if (view.Count == 0)
            {
                MessageBox.Show("Brak danych do wyeksportowania.", "Informacja", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            Excel.Application excelApp = null;
            Excel.Workbook workbook = null;
            Excel.Worksheet worksheet = null;

            try
            {
                Cursor.Current = Cursors.WaitCursor;
                excelApp = new Excel.Application { Visible = true };
                workbook = excelApp.Workbooks.Add();
                worksheet = (Excel.Worksheet)workbook.Sheets[1];

                int colIdx = 1;
                var visibleCols = dataGridViewZgloszenia.Columns.Cast<DataGridViewColumn>().Where(c => c.Visible).OrderBy(c => c.DisplayIndex).ToList();
                foreach (var col in visibleCols)
                {
                    worksheet.Cells[1, colIdx++] = col.HeaderText;
                }

                object[,] data = new object[view.Count, visibleCols.Count];
                for (int i = 0; i < view.Count; i++)
                {
                    int j = 0;
                    foreach (var col in visibleCols)
                    {
                        data[i, j++] = view[i][col.Name];
                    }
                }

                Excel.Range startCell = worksheet.Cells[2, 1];
                Excel.Range endCell = worksheet.Cells[view.Count + 1, visibleCols.Count];
                worksheet.Range[startCell, endCell].Value = data;

                Excel.Range headerRange = worksheet.Range[worksheet.Cells[1, 1], worksheet.Cells[1, visibleCols.Count]];
                headerRange.Font.Bold = true;
                headerRange.Interior.Color = ColorTranslator.ToOle(Color.LightGray);

                Excel.Range fullRange = worksheet.Range[worksheet.Cells[1, 1], endCell];
                fullRange.Borders.LineStyle = Excel.XlLineStyle.xlContinuous;
                fullRange.Columns.AutoFit();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Wystąpił błąd podczas eksportu do programu Excel: " + ex.Message, "Błąd Eksportu", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            finally
            {
                Cursor.Current = Cursors.Default;
                if (worksheet != null) Marshal.ReleaseComObject(worksheet);
                if (workbook != null) Marshal.ReleaseComObject(workbook);
                if (excelApp != null) Marshal.ReleaseComObject(excelApp);
            }
        }

        private void btnWybierzKolumny_Click(object sender, EventArgs e)
        {
            contextMenuKolumny.Show(btnWybierzKolumny, new Point(0, btnWybierzKolumny.Height));
        }

        private void PopulateColumnChooser()
        {
            contextMenuKolumny.Items.Clear();
            foreach (DataGridViewColumn col in dataGridViewZgloszenia.Columns)
            {
                var item = new ToolStripMenuItem(col.Name)
                {
                    Checked = col.Visible,
                    CheckOnClick = true
                };
                contextMenuKolumny.Items.Add(item);
            }
        }

        private void contextMenuKolumny_Closing(object sender, ToolStripDropDownClosingEventArgs e)
        {
            if (e.CloseReason == ToolStripDropDownCloseReason.ItemClicked)
            {
                e.Cancel = true;
                return;
            }

            foreach (ToolStripMenuItem item in contextMenuKolumny.Items)
            {
                if (dataGridViewZgloszenia.Columns.Contains(item.Text))
                {
                    dataGridViewZgloszenia.Columns[item.Text].Visible = item.Checked;
                }
            }

            // Odtwórz wiersz filtrów dla aktualnie widocznych kolumn
            BuildColumnFilterRow();
            ApplyFiltersToDataView();
        }

        #region Advanced Filter Menu Items
        private void noweReklamacjeToolStripMenuItem_Click(object sender, EventArgs e) => SetAdvancedFilter("DataZakupu >= '" + DateTime.Now.AddMonths(-3).ToString("yyyy-MM-dd") + "'", "Nowe produkty (< 3 msc)");
        private void przeterminowaneToolStripMenuItem_Click(object sender, EventArgs e) => SetAdvancedFilter("StatusOgolny = 'Nowe' AND DataZgloszenia <= '" + DateTime.Now.AddDays(-3).ToString("yyyy-MM-dd") + "'", "Przeterminowane (nowe > 3 dni)");
        private void nierozliczoneNotyToolStripMenuItem_Click(object sender, EventArgs e) => SetAdvancedFilter("CzyNotaRozliczona = 0 AND NrKPZN IS NOT NULL AND NrKPZN <> ''", "Nierozliczone noty");
        private void oczekujaceNaDostaweToolStripMenuItem_Click(object sender, EventArgs e) => SetAdvancedFilter("CzekamyNaDostawe = 'Czekamy'", "Oczekujące na dostawę");
        private void zgloszeniaAllegroToolStripMenuItem_Click(object sender, EventArgs e) => SetAdvancedFilter("Skad LIKE 'Allegro%'", "Zgłoszenia z Allegro");
        private void wyczyscFiltryDodatkoweToolStripMenuItem_Click(object sender, EventArgs e) => ClearAllFilters();
        #endregion

        private void dataGridViewZgloszenia_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex >= 0)
            {
                var nrZgloszenia = dataGridViewZgloszenia.Rows[e.RowIndex].Cells["NrZgloszenia"].Value?.ToString();
                if (!string.IsNullOrEmpty(nrZgloszenia))
                {
                    new Form2(nrZgloszenia).Show();
                }
            }
        }

        #endregion
    }
}
