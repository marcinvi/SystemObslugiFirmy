using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using System.Threading.Tasks;
using MySql.Data.MySqlClient;
using System.Text;
using System.IO;
using Google.Apis.Auth.OAuth2;
using Google.Apis.Services;
using Google.Apis.Sheets.v4;
using Google.Apis.Sheets.v4.Data;

namespace Reklamacje_Dane
{
    public partial class FormUniversalWizardV2 : Form
    {
        #region Fields & Properties

        private readonly WizardSource _source;
        private ComplaintInitialData _initialData;
        private readonly DatabaseService _dbService;

        private EnhancedClientViewModel _selectedClient = null;
        private EnhancedProductViewModel _selectedProduct = null;
        private bool _isInitializing = false;

        // UI Controls
        private Panel _mainScrollPanel;
        private Panel _livePreviewPanel;
        private Panel _sectionSource;
        private Panel _sectionClient;
        private Panel _sectionProduct;
        private Panel _sectionDetails;
        private Panel _sectionButtons;

        private TextBox _txtClientSearch;
        private DataGridView _gridClients;
        private CheckBox _chkSaveAsNew;
        private CheckBox _chkIsCompany;
        private TextBox _txtImie, _txtFirma, _txtNIP, _txtUlica, _txtKod, _txtMiasto, _txtEmail, _txtTelefon;
        private TextBox _txtOsobaKontaktowa;
        private Panel _pnlConflictWarning;
        private Label _lblConflictWarning;

        private TextBox _txtProductSearch;
        private DataGridView _gridProducts;
        private Button _btnSkipProduct;

        private TextBox _txtFaktura, _txtSerialNumber, _txtDescription;
        private DateTimePicker _dtpPurchaseDate;
        private RadioButton _rbGwarancja, _rbPlatna;
        private Label _lblWarrantyStatus;
        private Label _lblFakturaWarning, _lblSerialWarning;

        private RadioButton _rbEnaTruck, _rbTruckShop;
        private ListView _listViewInitialSelection;
        private Panel _pnlInitialSelection;

        private Label _lblPreviewStatus;
        private ProgressBar _progressCompletion;
        private Label _lblPreviewClient;
        private Label _lblPreviewProduct;
        private Label _lblPreviewInvoice;
        private Label _lblPreviewSerial;
        private Label _lblPreviewWarranty;
        private Panel _pnlPreviewWarnings;

        private Button _btnSave, _btnCancel;

        private Timer _clientSearchTimer;
        private Timer _productSearchTimer;

        #endregion

        #region Constructor & Initialization

        public FormUniversalWizardV2(WizardSource source)
        {
            _source = source;
            _dbService = new DatabaseService(DatabaseHelper.GetConnectionString());

            this.Text = "Nowe Zgłoszenie - Smart Wizard V2";
            this.Size = new Size(1400, 900);
            this.StartPosition = FormStartPosition.CenterScreen;
            this.Font = new Font("Segoe UI", 9.5F);
            this.BackColor = System.Drawing.Color.FromArgb(245, 247, 250);
            this.KeyPreview = true;

            _clientSearchTimer = new Timer { Interval = 300 };
            _clientSearchTimer.Tick += async (s, e) => { _clientSearchTimer.Stop(); await SearchClientsAsync(); };

            // ✅ NAPRAWA 1: Szybszy timer + osobna metoda z try-catch
            _productSearchTimer = new Timer { Interval = 250 };
            _productSearchTimer.Tick += ProductSearchTimer_Tick;

            this.KeyDown += Form_KeyDown;
            this.Load += FormUniversalWizardV2_Load;
            this.FormClosing += FormUniversalWizardV2_FormClosing;

            SetupUI();
        

            // Włącz sprawdzanie pisowni dla wszystkich TextBoxów
            EnableSpellCheckOnAllTextBoxes();
        }

        private async void FormUniversalWizardV2_Load(object sender, EventArgs e)
        {
            _isInitializing = true;

            try
            {
                // ✅ KLUCZOWA ZMIANA: Załaduj produkty do RAM NA STARCIE!
                this.Cursor = Cursors.WaitCursor;
                this.Text = "Ładowanie produktów do pamięci...";

                await SmartSearchService.PreloadProductCacheAsync();

                this.Text = "Nowe Zgłoszenie - Smart Wizard V2";
                this.Cursor = Cursors.Default;

                if (_source == WizardSource.Manual)
                {
                    _initialData = new ComplaintInitialData();
                    _sectionSource.Visible = true;
                    _pnlInitialSelection.Visible = false;
                    _mainScrollPanel.Visible = true;
                    _chkSaveAsNew.Checked = true;
                }
                else
                {
                    _pnlInitialSelection.Visible = true;
                    _mainScrollPanel.Visible = false;
                    await LoadInitialSelectionAsync();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Błąd inicjalizacji: {ex.Message}", "Błąd");
            }
            finally
            {
                _isInitializing = false;
                this.Cursor = Cursors.Default;
                UpdateLivePreview();
            }
        }

        private void FormUniversalWizardV2_FormClosing(object sender, FormClosingEventArgs e)
        {
            // ✅ Wyczyść cache przy zamykaniu (opcjonalne - statyczny cache będzie dostępny dla innych instancji)
            // SmartSearchService.InvalidateCache();
        }

        #endregion

        #region UI Setup

        private void SetupUI()
        {
            this.SuspendLayout();

            var mainContainer = new TableLayoutPanel
            {
                Dock = DockStyle.Fill,
                ColumnCount = 2,
                RowCount = 1,
                BackColor = System.Drawing.Color.Transparent
            };
            mainContainer.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 70F));
            mainContainer.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 30F));
            mainContainer.RowStyles.Add(new RowStyle(SizeType.Percent, 100F));

            _pnlInitialSelection = CreateInitialSelectionPanel();
            _pnlInitialSelection.Dock = DockStyle.Fill;

            _mainScrollPanel = new Panel
            {
                Dock = DockStyle.Fill,
                AutoScroll = true,
                BackColor = System.Drawing.Color.White,
                Padding = new System.Windows.Forms.Padding(20),
                Visible = false
            };

            var formContainer = new FlowLayoutPanel
            {
                FlowDirection = FlowDirection.TopDown,
                WrapContents = false,
                AutoSize = true,
                AutoSizeMode = AutoSizeMode.GrowAndShrink,
                Width = 850,
                Anchor = AnchorStyles.Left | AnchorStyles.Right | AnchorStyles.Top
            };

            _sectionSource = CreateSourceSection();
            _sectionClient = CreateClientSection();
            _sectionProduct = CreateProductSection();
            _sectionDetails = CreateDetailsSection();
            _sectionButtons = CreateButtonsSection();

            formContainer.Controls.Add(_sectionSource);
            formContainer.Controls.Add(_sectionClient);
            formContainer.Controls.Add(_sectionProduct);
            formContainer.Controls.Add(_sectionDetails);
            formContainer.Controls.Add(_sectionButtons);

            _mainScrollPanel.Controls.Add(formContainer);

            _livePreviewPanel = CreateLivePreviewPanel();

            mainContainer.Controls.Add(_pnlInitialSelection, 0, 0);
            mainContainer.SetColumnSpan(_pnlInitialSelection, 2);
            mainContainer.Controls.Add(_mainScrollPanel, 0, 0);
            mainContainer.Controls.Add(_livePreviewPanel, 1, 0);

            this.Controls.Add(mainContainer);
            this.ResumeLayout();
        }

        private Panel CreateInitialSelectionPanel()
        {
            var panel = new Panel { BackColor = System.Drawing.Color.White, Padding = new System.Windows.Forms.Padding(40) };

            var lblTitle = new Label
            {
                Text = "Wybierz źródło do przetworzenia",
                Font = new Font("Segoe UI Semibold", 14F, FontStyle.Bold),
                AutoSize = true,
                Location = new Point(20, 20)
            };

            _listViewInitialSelection = new ListView
            {
                View = View.Details,
                FullRowSelect = true,
                GridLines = true,
                Location = new Point(20, 60),
                Size = new Size(1000, 600),
                Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right | AnchorStyles.Bottom
            };
            _listViewInitialSelection.Columns.Add("ID", 150);
            _listViewInitialSelection.Columns.Add("Dane", 400);
            _listViewInitialSelection.Columns.Add("Szczegóły", 400);
            _listViewInitialSelection.DoubleClick += ListViewInitialSelection_DoubleClick;

            panel.Controls.Add(lblTitle);
            panel.Controls.Add(_listViewInitialSelection);

            return panel;
        }

        private Panel CreateSourceSection()
        {
            var section = CreateSection("📋 Źródło Zgłoszenia", 100);
            section.Visible = false;

            var flowPanel = new FlowLayoutPanel
            {
                FlowDirection = FlowDirection.LeftToRight,
                AutoSize = true,
                Padding = new System.Windows.Forms.Padding(10, 40, 10, 10)
            };

            _rbEnaTruck = new RadioButton { Text = "Ena-Truck", AutoSize = true, Checked = true, Margin = new System.Windows.Forms.Padding(0, 0, 20, 0) };
            _rbTruckShop = new RadioButton { Text = "Truck-Shop", AutoSize = true };

            flowPanel.Controls.Add(_rbEnaTruck);
            flowPanel.Controls.Add(_rbTruckShop);

            section.Controls.Add(flowPanel);
            return section;
        }

        private Panel CreateClientSection()
        {
            var section = CreateSection("👤 Klient", 780);

            var layout = new TableLayoutPanel
            {
                Location = new Point(15, 45),
                Width = 870,
                Height = 720,
                ColumnCount = 2,
                RowCount = 16,
                Padding = new System.Windows.Forms.Padding(5)
            };
            layout.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 50F));
            layout.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 50F));

            int row = 0;

            var lblSearchHint = new Label
            {
                Text = "🔍 Szukaj klienta (Ctrl+K):",
                Font = new Font("Segoe UI Semibold", 9.5F, FontStyle.Bold),
                AutoSize = true,
                Dock = DockStyle.Fill,
                Padding = new System.Windows.Forms.Padding(0, 5, 0, 0)
            };
            layout.Controls.Add(lblSearchHint, 0, row);
            layout.SetColumnSpan(lblSearchHint, 2);
            row++;

            _txtClientSearch = new TextBox
            {
                Font = new Font("Segoe UI", 11F),
                Height = 32,
                Dock = DockStyle.Fill
            };
            _txtClientSearch.SetPlaceholder("Wpisz nazwisko, firmę, email, telefon...");
            _txtClientSearch.TextChanged += (s, e) =>
            {
                if (!_isInitializing)
                {
                    _clientSearchTimer.Stop();
                    _clientSearchTimer.Start();
                    _selectedClient = null;
                    _chkSaveAsNew.Checked = true;
                }
            };
            layout.Controls.Add(_txtClientSearch, 0, row);
            layout.SetColumnSpan(_txtClientSearch, 2);
            row++;

            _gridClients = new DataGridView
            {
                Height = 160,
                Dock = DockStyle.Fill,
                BackgroundColor = System.Drawing.Color.White,
                BorderStyle = BorderStyle.FixedSingle,
                RowHeadersVisible = false,
                AllowUserToAddRows = false,
                AllowUserToDeleteRows = false,
                ReadOnly = true,
                SelectionMode = DataGridViewSelectionMode.FullRowSelect,
                AutoGenerateColumns = false,
                ColumnHeadersHeight = 35,
                RowTemplate = { Height = 28 }
            };

            _gridClients.Columns.Add(new DataGridViewTextBoxColumn { DataPropertyName = "ConfidenceDisplay", HeaderText = "Match", Width = 80 });
            _gridClients.Columns.Add(new DataGridViewTextBoxColumn { DataPropertyName = "DisplayName", HeaderText = "Klient", AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill });
            _gridClients.Columns.Add(new DataGridViewTextBoxColumn { DataPropertyName = "Email", HeaderText = "Email", Width = 180 });
            _gridClients.Columns.Add(new DataGridViewTextBoxColumn { DataPropertyName = "Telefon", HeaderText = "Telefon", Width = 110 });

            _gridClients.CellClick += GridClients_CellClick;
            _gridClients.CellDoubleClick += GridClients_DoubleClick;

            layout.Controls.Add(_gridClients, 0, row);
            layout.SetColumnSpan(_gridClients, 2);
            layout.SetRowSpan(_gridClients, 2);
            row += 2;

            _chkSaveAsNew = new CheckBox
            {
                Text = "✔ Zapisz jako nowego klienta",
                Checked = true,
                AutoSize = true,
                Font = new Font("Segoe UI", 9.5F, FontStyle.Bold),
                ForeColor = System.Drawing.Color.FromArgb(0, 120, 215),
                Padding = new System.Windows.Forms.Padding(0, 5, 0, 5)
            };
            _chkSaveAsNew.CheckedChanged += (s, e) =>
            {
                if (_chkSaveAsNew.Checked)
                {
                    _selectedClient = null;
                    _gridClients.ClearSelection();
                }
            };
            layout.Controls.Add(_chkSaveAsNew, 0, row);
            layout.SetColumnSpan(_chkSaveAsNew, 2);
            row++;

            _pnlConflictWarning = new Panel
            {
                Dock = DockStyle.Fill,
                BackColor = System.Drawing.Color.FromArgb(255, 243, 205),
                BorderStyle = BorderStyle.FixedSingle,
                Visible = false,
                Padding = new System.Windows.Forms.Padding(10)
            };
            _lblConflictWarning = new Label
            {
                Text = "⚠ Wykryto konflikt danych!",
                ForeColor = System.Drawing.Color.FromArgb(133, 77, 14),
                AutoSize = true,
                Font = new Font("Segoe UI", 9F, FontStyle.Bold)
            };
            _pnlConflictWarning.Controls.Add(_lblConflictWarning);
            layout.Controls.Add(_pnlConflictWarning, 0, row);
            layout.SetColumnSpan(_pnlConflictWarning, 2);
            row++;

            _chkIsCompany = new CheckBox { Text = "To firma", AutoSize = true, Padding = new System.Windows.Forms.Padding(0, 5, 0, 0) };
            _chkIsCompany.CheckedChanged += ChkIsCompany_Changed;
            layout.Controls.Add(_chkIsCompany, 0, row);
            row++;

            AddFormField(layout, ref row, "Imię i Nazwisko:", out _txtImie);
            AddFormField(layout, ref row, "Nazwa Firmy:", out _txtFirma);
            AddFormField(layout, ref row, "Osoba kontaktowa:", out _txtOsobaKontaktowa);
            AddFormField(layout, ref row, "NIP:", out _txtNIP);
            AddFormField(layout, ref row, "Ulica:", out _txtUlica);
            AddFormField(layout, ref row, "Kod pocztowy:", out _txtKod);
            AddFormField(layout, ref row, "Miejscowość:", out _txtMiasto);
            AddFormField(layout, ref row, "Email:", out _txtEmail);
            AddFormField(layout, ref row, "Telefon:", out _txtTelefon);

            _txtOsobaKontaktowa.SetPlaceholder("np. Jan Kowalski");
            _txtKod.SetPlaceholder("00-000");
            _txtTelefon.SetPlaceholder("+48 123 456 789");

            section.Controls.Add(layout);
            return section;
        }

        private Panel CreateProductSection()
        {
            var section = CreateSection("🔧 Produkt", 450);

            var layout = new TableLayoutPanel
            {
                Location = new Point(15, 45),
                Width = 870,
                Height = 390,
                ColumnCount = 1,
                RowCount = 4,
                Padding = new System.Windows.Forms.Padding(5)
            };

            int row = 0;

            var lblSearch = new Label
            {
                Text = "🔍 Szukaj produktu (Ctrl+F):",
                Font = new Font("Segoe UI Semibold", 9.5F, FontStyle.Bold),
                AutoSize = true,
                Padding = new System.Windows.Forms.Padding(0, 5, 0, 0)
            };
            layout.Controls.Add(lblSearch, 0, row++);

            // ✅ NAPRAWA 2: MaxLength=100 + krótszy placeholder
            _txtProductSearch = new TextBox
            {
                Font = new Font("Segoe UI", 11F),
                Height = 32,
                Dock = DockStyle.Fill,
                MaxLength = 100
            };
            _txtProductSearch.SetPlaceholder("np. lodówka vigo, waeco CFX");
            _txtProductSearch.TextChanged += (s, e) =>
            {
                if (!_isInitializing)
                {
                    _productSearchTimer.Stop();
                    _productSearchTimer.Start();
                }
            };
            layout.Controls.Add(_txtProductSearch, 0, row++);

            _gridProducts = new DataGridView
            {
                Height = 250,
                Dock = DockStyle.Fill,
                BackgroundColor = System.Drawing.Color.White,
                BorderStyle = BorderStyle.FixedSingle,
                RowHeadersVisible = false,
                AllowUserToAddRows = false,
                AllowUserToDeleteRows = false,
                ReadOnly = true,
                SelectionMode = DataGridViewSelectionMode.FullRowSelect,
                AutoGenerateColumns = false
            };

            // ✅ POPRAWKA: Użyj DisplayName - ma wbudowany fallback do NazwaSystemowa
            _gridProducts.Columns.Add(new DataGridViewTextBoxColumn { DataPropertyName = "ConfidenceDisplay", HeaderText = "Match", Width = 80 });
            _gridProducts.Columns.Add(new DataGridViewTextBoxColumn { DataPropertyName = "DisplayName", HeaderText = "Produkt", AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill });
            _gridProducts.Columns.Add(new DataGridViewTextBoxColumn { DataPropertyName = "Producent", HeaderText = "Producent", Width = 120 });
            _gridProducts.Columns.Add(new DataGridViewTextBoxColumn { DataPropertyName = "KodProducenta", HeaderText = "Kod Prod.", Width = 120 });

            _gridProducts.CellClick += (s, e) =>
            {
                if (e.RowIndex >= 0)
                {
                    _selectedProduct = _gridProducts.Rows[e.RowIndex].DataBoundItem as EnhancedProductViewModel;
                    UpdateLivePreview();
                }
            };

            _gridProducts.CellDoubleClick += (s, e) =>
            {
                if (e.RowIndex >= 0)
                {
                    _selectedProduct = _gridProducts.Rows[e.RowIndex].DataBoundItem as EnhancedProductViewModel;
                    UpdateLivePreview();
                    _txtFaktura.Focus();
                }
            };

            layout.Controls.Add(_gridProducts, 0, row++);

            _btnSkipProduct = new Button
            {
                Text = "⚠ Nie znalazłem - wypełnię później",
                Height = 35,
                Dock = DockStyle.Fill,
                BackColor = System.Drawing.Color.FromArgb(255, 193, 7),
                ForeColor = System.Drawing.Color.Black,
                FlatStyle = FlatStyle.Flat
            };
            _btnSkipProduct.FlatAppearance.BorderSize = 0;
            _btnSkipProduct.Click += (s, e) =>
            {
                _selectedProduct = null;
                MessageBox.Show("OK - będziesz mógł dodać produkt później.", "Info", MessageBoxButtons.OK, MessageBoxIcon.Information);
                _txtFaktura.Focus();
            };
            layout.Controls.Add(_btnSkipProduct, 0, row++);

            section.Controls.Add(layout);
            return section;
        }

        private Panel CreateDetailsSection()
        {
            var section = CreateSection("📝 Szczegóły Zgłoszenia", 450);

            var layout = new TableLayoutPanel
            {
                Location = new Point(15, 45),
                Width = 870,
                Height = 390,
                ColumnCount = 2,
                RowCount = 9,
                Padding = new System.Windows.Forms.Padding(5)
            };
            layout.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 50F));
            layout.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 50F));

            int row = 0;

            var lblFV = new Label { Text = "Numer Faktury:", AutoSize = true, Dock = DockStyle.Fill, Padding = new System.Windows.Forms.Padding(0, 5, 0, 0) };
            _txtFaktura = new TextBox { Dock = DockStyle.Fill };
            _txtFaktura.TextChanged += async (s, e) =>
            {
                if (!_isInitializing)
                {
                    UpdateLivePreview();
                    await ValidateFakturaAsync();
                }
            };
            _lblFakturaWarning = new Label { Text = "", AutoSize = true, ForeColor = System.Drawing.Color.Red, Dock = DockStyle.Fill };

            layout.Controls.Add(lblFV, 0, row);
            layout.Controls.Add(_txtFaktura, 1, row);
            row++;
            layout.Controls.Add(_lblFakturaWarning, 0, row);
            layout.SetColumnSpan(_lblFakturaWarning, 2);
            row++;

            var lblSN = new Label { Text = "Numer Seryjny:", AutoSize = true, Dock = DockStyle.Fill };
            _txtSerialNumber = new TextBox { Dock = DockStyle.Fill };
            _txtSerialNumber.Leave += async (s, e) => await ValidateSerialAsync();
            _lblSerialWarning = new Label { Text = "", AutoSize = true, ForeColor = System.Drawing.Color.Red, Dock = DockStyle.Fill };

            layout.Controls.Add(lblSN, 0, row);
            layout.Controls.Add(_txtSerialNumber, 1, row);
            row++;
            layout.Controls.Add(_lblSerialWarning, 0, row);
            layout.SetColumnSpan(_lblSerialWarning, 2);
            row++;

            var lblData = new Label { Text = "Data Zakupu:", AutoSize = true, Dock = DockStyle.Fill };
            _dtpPurchaseDate = new DateTimePicker { Dock = DockStyle.Fill, Format = DateTimePickerFormat.Short };
            _dtpPurchaseDate.ValueChanged += (s, e) =>
            {
                if (!_isInitializing)
                {
                    UpdateLivePreview();
                    ValidateFakturaDate();
                    UpdateWarrantyStatus();
                }
            };

            layout.Controls.Add(lblData, 0, row);
            layout.Controls.Add(_dtpPurchaseDate, 1, row);
            row++;

            var lblTyp = new Label { Text = "Typ:", AutoSize = true, Dock = DockStyle.Fill };
            var pnlTyp = new FlowLayoutPanel { Dock = DockStyle.Fill, FlowDirection = FlowDirection.LeftToRight };
            _rbGwarancja = new RadioButton { Text = "Gwarancyjna", Checked = true, AutoSize = true, Margin = new System.Windows.Forms.Padding(0, 0, 20, 0) };
            _rbPlatna = new RadioButton { Text = "Płatna", AutoSize = true };
            pnlTyp.Controls.Add(_rbGwarancja);
            pnlTyp.Controls.Add(_rbPlatna);

            layout.Controls.Add(lblTyp, 0, row);
            layout.Controls.Add(pnlTyp, 1, row);
            row++;

            _lblWarrantyStatus = new Label
            {
                Text = "Obliczanie...",
                AutoSize = true,
                Dock = DockStyle.Fill,
                Font = new Font("Segoe UI", 9.5F, FontStyle.Bold)
            };
            layout.Controls.Add(_lblWarrantyStatus, 0, row);
            layout.SetColumnSpan(_lblWarrantyStatus, 2);
            row++;

            var lblOpis = new Label { Text = "Opis Usterki:", AutoSize = true, Dock = DockStyle.Fill };
            _txtDescription = new TextBox
            {
                Multiline = true,
                Height = 120,
                Dock = DockStyle.Fill,
                ScrollBars = ScrollBars.Vertical
            };
            _txtDescription.TextChanged += (s, e) => { if (!_isInitializing) UpdateLivePreview(); };

            layout.Controls.Add(lblOpis, 0, row);
            layout.SetColumnSpan(lblOpis, 2);
            row++;
            layout.Controls.Add(_txtDescription, 0, row);
            layout.SetColumnSpan(_txtDescription, 2);

            section.Controls.Add(layout);
            return section;
        }

        private Panel CreateLivePreviewPanel()
        {
            var panel = new Panel
            {
                Dock = DockStyle.Fill,
                BackColor = System.Drawing.Color.White,
                Padding = new System.Windows.Forms.Padding(15),
                BorderStyle = BorderStyle.FixedSingle
            };

            var title = new Label
            {
                Text = "📨 Podgląd na żywo",
                Font = new Font("Segoe UI Semibold", 12F, FontStyle.Bold),
                ForeColor = System.Drawing.Color.FromArgb(0, 120, 215),
                AutoSize = true,
                Location = new Point(10, 10)
            };

            _lblPreviewStatus = new Label
            {
                Text = "Status: 0% Complete",
                AutoSize = true,
                Location = new Point(10, 40)
            };

            _progressCompletion = new ProgressBar
            {
                Width = 350,
                Height = 25,
                Location = new Point(10, 60),
                Anchor = AnchorStyles.Left | AnchorStyles.Right | AnchorStyles.Top
            };

            _lblPreviewClient = CreatePreviewLabel("👤 Klient:", "Nie wybrano", 100);
            _lblPreviewProduct = CreatePreviewLabel("🔧 Produkt:", "Nie wybrano", 140);
            _lblPreviewInvoice = CreatePreviewLabel("📄 Faktura:", "Brak", 180);
            _lblPreviewSerial = CreatePreviewLabel("🔢 S/N:", "Brak", 220);
            _lblPreviewWarranty = CreatePreviewLabel("🛡 Gwarancja:", "Obliczanie...", 260);

            _pnlPreviewWarnings = new Panel
            {
                Location = new Point(10, 320),
                Width = 350,
                Height = 200,
                BorderStyle = BorderStyle.FixedSingle,
                BackColor = System.Drawing.Color.FromArgb(255, 250, 240),
                AutoScroll = true,
                Anchor = AnchorStyles.Left | AnchorStyles.Right | AnchorStyles.Top
            };

            var lblWarnings = new Label
            {
                Text = "⚠ Ostrzeżenia:",
                Font = new Font("Segoe UI", 9F, FontStyle.Bold),
                AutoSize = true,
                Location = new Point(5, 5)
            };
            _pnlPreviewWarnings.Controls.Add(lblWarnings);

            panel.Controls.Add(title);
            panel.Controls.Add(_lblPreviewStatus);
            panel.Controls.Add(_progressCompletion);
            panel.Controls.Add(_lblPreviewClient);
            panel.Controls.Add(_lblPreviewProduct);
            panel.Controls.Add(_lblPreviewInvoice);
            panel.Controls.Add(_lblPreviewSerial);
            panel.Controls.Add(_lblPreviewWarranty);
            panel.Controls.Add(_pnlPreviewWarnings);

            return panel;
        }

        private Panel CreateButtonsSection()
        {
            var section = new Panel
            {
                Width = 900,
                Height = 80,
                Margin = new System.Windows.Forms.Padding(0, 0, 0, 20)
            };

            _btnSave = new Button
            {
                Text = "💾 ZAPISZ ZGŁOSZENIE (Ctrl+S)",
                Width = 350,
                Height = 50,
                Location = new Point(20, 15),
                BackColor = System.Drawing.Color.FromArgb(40, 167, 69),
                ForeColor = System.Drawing.Color.White,
                Font = new Font("Segoe UI", 11F, FontStyle.Bold),
                FlatStyle = FlatStyle.Flat,
                Cursor = Cursors.Hand
            };
            _btnSave.FlatAppearance.BorderSize = 0;
            _btnSave.Click += async (s, e) => await SaveComplaintAsync();

            _btnCancel = new Button
            {
                Text = "❌ Anuluj (Escape)",
                Width = 200,
                Height = 50,
                Location = new Point(390, 15),
                BackColor = System.Drawing.Color.FromArgb(220, 53, 69),
                ForeColor = System.Drawing.Color.White,
                Font = new Font("Segoe UI", 10F, FontStyle.Bold),
                FlatStyle = FlatStyle.Flat,
                Cursor = Cursors.Hand
            };
            _btnCancel.FlatAppearance.BorderSize = 0;
            _btnCancel.Click += (s, e) => this.Close();

            section.Controls.Add(_btnSave);
            section.Controls.Add(_btnCancel);

            return section;
        }

        private Panel CreateSection(string title, int height)
        {
            var section = new Panel
            {
                Width = 900,
                Height = height,
                BorderStyle = BorderStyle.FixedSingle,
                BackColor = System.Drawing.Color.White,
                Margin = new System.Windows.Forms.Padding(0, 0, 0, 20)
            };

            var lblTitle = new Label
            {
                Text = title,
                Font = new Font("Segoe UI Semibold", 12F, FontStyle.Bold),
                ForeColor = System.Drawing.Color.FromArgb(0, 120, 215),
                AutoSize = true,
                Location = new Point(15, 10)
            };

            section.Controls.Add(lblTitle);
            return section;
        }

        private void AddFormField(TableLayoutPanel layout, ref int row, string label, out TextBox textBox)
        {
            var lbl = new Label { Text = label, AutoSize = true, Dock = DockStyle.Fill, Padding = new System.Windows.Forms.Padding(0, 5, 0, 0) };
            textBox = new TextBox { Dock = DockStyle.Fill };
            textBox.TextChanged += (s, e) => { if (!_isInitializing) UpdateLivePreview(); };

            layout.Controls.Add(lbl, 0, row);
            layout.Controls.Add(textBox, 1, row);
            row++;
        }

        private Label CreatePreviewLabel(string title, string value, int y)
        {
            return new Label
            {
                Text = $"{title}\n{value}",
                AutoSize = false,
                Width = 350,
                Height = 35,
                Location = new Point(10, y),
                Font = new Font("Segoe UI", 9F)
            };
        }

        #endregion

        #region Event Handlers

        // ✅ NAPRAWA 5: Metoda z try-catch dla product search timer
        private async void ProductSearchTimer_Tick(object sender, EventArgs e)
        {
            _productSearchTimer.Stop();
            try
            {
                await SearchProductsAsync();
            }
            catch (Exception ex)
            {
                _gridProducts.DataSource = null;
                MessageBox.Show($"Błąd wyszukiwania produktu:\n{ex.Message}", "Błąd", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
        }

        private async Task SearchClientsAsync()
        {
            string query = _txtClientSearch.Text;

            // ✅ NAPRAWA: Jeśli textbox pusty ALE mamy _initialData, szukaj po danych!
            if (string.IsNullOrWhiteSpace(query) || query.Length < 2)
            {
                // Jeśli mamy _initialData z danymi - użyj ich do wyszukania!
                if (_initialData != null && !string.IsNullOrWhiteSpace(_initialData.Email))
                {
                    query = _initialData.Email; // Użyj email jako query
                }
                else if (_initialData != null && !string.IsNullOrWhiteSpace(_initialData.Telefon))
                {
                    query = _initialData.Telefon; // Lub telefonu
                }
                else if (_initialData != null && !string.IsNullOrWhiteSpace(_initialData.ImieNazwisko))
                {
                    query = _initialData.ImieNazwisko; // Lub imienia
                }
                else
                {
                    _gridClients.DataSource = null;
                    return;
                }
            }

            try
            {
                var results = await SmartSearchService.SmartSearchClientsAsync(query, _initialData);
                _gridClients.DataSource = results;

                // ✅ DEBUG: Pokaż co znaleziono
                if (results.Count > 0)
                {

                }
            }
            catch (Exception ex)
            {
                System.Windows.Forms.MessageBox.Show($"Błąd wyszukiwania klientów:\n{ex.Message}", "Błąd");
            }
        }

        // ✅ NAPRAWA 6: Lepsza obsługa błędów + limit długości
        private async Task SearchProductsAsync()
        {
            if (_isInitializing) return;

            string query = _txtProductSearch.Text?.Trim() ?? "";

            if (query.Length < 2)
            {
                _gridProducts.DataSource = null;
                return;
            }

            try
            {
                // Limit długości query na wszelki wypadek
                if (query.Length > 100)
                    query = query.Substring(0, 100);

                var results = await SmartSearchService.SmartSearchProductsAsync(query);
                _gridProducts.DataSource = results;
            }
            catch (Exception ex)
            {
                _gridProducts.DataSource = null;
                System.Diagnostics.Debug.WriteLine($"SearchProductsAsync Error: {ex.Message}");
                // Nie pokazuj MessageBox - może być irytujące przy każdej literze
            }
        }

        private void GridClients_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex < 0) return;

            _selectedClient = _gridClients.Rows[e.RowIndex].DataBoundItem as EnhancedClientViewModel;
            if (_selectedClient == null) return;

            _chkSaveAsNew.Checked = false;

            if (_selectedClient.HasConflict)
            {
                _pnlConflictWarning.Visible = true;
                _lblConflictWarning.Text = $"⚠ {_selectedClient.ConflictReason}";
            }
            else
            {
                _pnlConflictWarning.Visible = false;
            }

            UpdateLivePreview();
        }

        private void GridClients_DoubleClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex < 0) return;

            _selectedClient = _gridClients.Rows[e.RowIndex].DataBoundItem as EnhancedClientViewModel;
            if (_selectedClient == null) return;

            _chkSaveAsNew.Checked = false;
            FillClientFields(_selectedClient);
            _txtProductSearch.Focus();
        }

        private void FillClientFields(EnhancedClientViewModel client)
        {
            _isInitializing = true;
            _txtImie.Text = client.ImieNazwisko;
            _txtFirma.Text = client.NazwaFirmy;
            _txtNIP.Text = client.NIP;
            _txtUlica.Text = client.Ulica;
            _txtKod.Text = client.KodPocztowy;
            _txtMiasto.Text = client.Miejscowosc;
            _txtEmail.Text = client.Email;
            _txtTelefon.Text = client.Telefon;
            _chkIsCompany.Checked = client.IsCompany;
            _isInitializing = false;
        }

        private void ChkIsCompany_Changed(object sender, EventArgs e)
        {
            UpdateWarrantyStatus();
        }

        private void UpdateLivePreview()
        {
            int completion = 0;
            if (_selectedClient != null || _chkSaveAsNew.Checked) completion += 30;
            if (_selectedProduct != null) completion += 25;
            if (!string.IsNullOrEmpty(_txtFaktura.Text)) completion += 15;
            if (!string.IsNullOrEmpty(_txtSerialNumber.Text)) completion += 10;
            if (!string.IsNullOrEmpty(_txtDescription.Text)) completion += 20;

            _progressCompletion.Value = completion;
            _lblPreviewStatus.Text = $"Status: {completion}% Complete";

            string clientText = _selectedClient?.DisplayName ?? (_chkSaveAsNew.Checked ? "NOWY KLIENT" : "Nie wybrano");
            _lblPreviewClient.Text = $"👤 Klient:\n{clientText}";

            string productText = _selectedProduct?.DisplayName ?? "Nie wybrano";
            _lblPreviewProduct.Text = $"🔧 Produkt:\n{productText}";

            string fvText = string.IsNullOrEmpty(_txtFaktura.Text) ? "Brak" : _txtFaktura.Text;
            _lblPreviewInvoice.Text = $"📄 Faktura:\n{fvText}";

            string snText = string.IsNullOrEmpty(_txtSerialNumber.Text) ? "Brak" : _txtSerialNumber.Text;
            _lblPreviewSerial.Text = $"🔢 S/N:\n{snText}";

            UpdateWarrantyStatus();
            UpdateWarningsPanel();
        }

        private void UpdateWarrantyStatus()
        {
            try
            {
                DateTime purchaseDate = _dtpPurchaseDate.Value;
                int months = _chkIsCompany.Checked ? 12 : 24;
                DateTime warrantyEnd = purchaseDate.AddMonths(months);

                bool isValid = DateTime.Now <= warrantyEnd;

                string status = isValid
                    ? $"✔ Ważna do {warrantyEnd:dd.MM.yyyy}"
                    : $"❌ Zakończona ({warrantyEnd:dd.MM.yyyy})";

                _lblPreviewWarranty.Text = $"🛡 Gwarancja:\n{status}";
                _lblPreviewWarranty.ForeColor = isValid ? System.Drawing.Color.ForestGreen : System.Drawing.Color.Red;

                _lblWarrantyStatus.Text = status;
                _lblWarrantyStatus.ForeColor = isValid ? System.Drawing.Color.ForestGreen : System.Drawing.Color.Red;
            }
            catch
            {
                _lblPreviewWarranty.Text = "🛡 Gwarancja:\nObliczanie...";
            }
        }

        private void UpdateWarningsPanel()
        {
            _pnlPreviewWarnings.Controls.Clear();

            var title = new Label
            {
                Text = "⚠ Ostrzeżenia:",
                Font = new Font("Segoe UI", 9F, FontStyle.Bold),
                AutoSize = true,
                Location = new Point(5, 5)
            };
            _pnlPreviewWarnings.Controls.Add(title);

            int y = 30;

            if (_selectedClient == null && !_chkSaveAsNew.Checked)
            {
                AddWarning("Nie wybrano klienta", y);
                y += 25;
            }

            if (_selectedProduct == null)
            {
                AddWarning("Nie wybrano produktu", y);
                y += 25;
            }

            if (string.IsNullOrEmpty(_txtFaktura.Text))
            {
                AddWarning("Brak numeru faktury", y);
                y += 25;
            }

            if (string.IsNullOrEmpty(_txtDescription.Text))
            {
                AddWarning("Brak opisu usterki", y);
                y += 25;
            }

            if (y == 30)
            {
                var okLabel = new Label
                {
                    Text = "✔ Brak ostrzeżeń - gotowe do zapisu!",
                    ForeColor = System.Drawing.Color.ForestGreen,
                    AutoSize = true,
                    Location = new Point(5, 30)
                };
                _pnlPreviewWarnings.Controls.Add(okLabel);
            }
        }

        private void AddWarning(string text, int y)
        {
            var lbl = new Label
            {
                Text = $"• {text}",
                ForeColor = System.Drawing.Color.FromArgb(255, 120, 0),
                AutoSize = true,
                Location = new Point(10, y)
            };
            _pnlPreviewWarnings.Controls.Add(lbl);
        }

        private async Task ValidateFakturaAsync()
        {
            _lblFakturaWarning.Text = "";

            if (string.IsNullOrEmpty(_txtFaktura.Text))
                return;

            var (exists, number) = await SmartSearchService.CheckInvoiceNumberAsync(_txtFaktura.Text);
            if (exists)
            {
                _lblFakturaWarning.Text = $"⚠ UWAGA: Faktura istnieje w zgłoszeniu {number}!";
            }
        }

        private async Task ValidateSerialAsync()
        {
            _lblSerialWarning.Text = "";

            if (string.IsNullOrEmpty(_txtSerialNumber.Text))
                return;

            var (exists, number) = await SmartSearchService.CheckSerialNumberAsync(_txtSerialNumber.Text);
            if (exists)
            {
                _lblSerialWarning.Text = $"⚠ UWAGA: S/N istnieje w zgłoszeniu {number}!";
            }
        }

        private void ValidateFakturaDate()
        {
            if (!SmartSearchService.ValidateInvoiceDateConsistency(_txtFaktura.Text, _dtpPurchaseDate.Value))
            {
                _lblFakturaWarning.Text = "⚠ Data zakupu nie zgadza się z miesiącem w FV!";
            }
        }

        private void Form_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Control)
            {
                switch (e.KeyCode)
                {
                    case Keys.K:
                        _txtClientSearch.Focus();
                        e.Handled = true;
                        break;
                    case Keys.F:
                        _txtProductSearch.Focus();
                        e.Handled = true;
                        break;
                    case Keys.S:
                        if (_btnSave.Enabled)
                            _ = SaveComplaintAsync();
                        e.Handled = true;
                        break;
                }
            }
            else if (e.KeyCode == Keys.Escape)
            {
                this.Close();
                e.Handled = true;
            }
        }

        #endregion

        #region Initial Selection Loading

        private async Task LoadInitialSelectionAsync()
        {
            _listViewInitialSelection.Items.Clear();
            _listViewInitialSelection.Enabled = false;

            try
            {
                if (_source == WizardSource.Allegro)
                {
                    this.Text = "Wybierz dyskusję Allegro";
                    using (var con = DatabaseHelper.GetConnection())
                    {
                        await con.OpenAsync();
                        var cmd = new MySqlCommand("SELECT DisputeId, BuyerLogin, Subject, ProductName FROM AllegroDisputes WHERE ComplaintId IS NULL ORDER BY OpenedAt DESC", con);
                        using (var reader = await cmd.ExecuteReaderAsync())
                        {
                            while (await reader.ReadAsync())
                            {
                                var item = new ListViewItem(reader["DisputeId"].ToString());
                                item.SubItems.Add(reader["BuyerLogin"].ToString());
                                item.SubItems.Add(reader["Subject"].ToString() ?? reader["ProductName"].ToString());
                                item.Tag = reader["DisputeId"].ToString();
                                _listViewInitialSelection.Items.Add(item);
                            }
                        }
                    }
                }
                else if (_source == WizardSource.GoogleSheet)
                {
                    this.Text = "Wybierz zgłoszenie z arkusza";
                    string[] sheetsToRead = { "B", "Z" };
                    string credentialsPath = "reklamacje-baza-c36d05b0ffdb.json";
                    string spreadsheetId = "1VXGP4Cckt6NmSHtiv-Um7nqg-itLMczAGd-5a_Tc4Ds";

                    foreach (var sheetName in sheetsToRead)
                    {
                        var values = await GoogleSheetsDataService.GetSheetValuesAsync(credentialsPath, spreadsheetId, $"{sheetName}!A:P");
                        if (values != null && values.Count > 0)
                        {
                            for (int i = 1; i < values.Count; i++)
                            {
                                var row = values[i];
                                if (row.Cast<object>().Any(cell => !string.IsNullOrWhiteSpace(cell?.ToString())))
                                {
                                    var item = new ListViewItem(GetValueFromRow(row, 0));
                                    item.SubItems.Add(GetValueFromRow(row, 2));
                                    item.SubItems.Add(GetValueFromRow(row, 12));
                                    item.Tag = new ZgloszenieZArkusza { RowData = row, SourceSheet = sheetName, RowIndex = i + 1 };
                                    _listViewInitialSelection.Items.Add(item);
                                }
                            }
                        }
                    }
                }
                else if (_source == WizardSource.Zwroty)
                {
                    this.Text = "Wybierz zwrot";
                    using (var con = DatabaseHelper.GetConnection())
                    {
                        await con.OpenAsync();
                        using (var cmd = new MySqlCommand(@"
                            SELECT Id,
                                   IFNULL(DaneKlienta,'')      AS DaneKlienta,
                                   IFNULL(DaneProduktu,'')     AS DaneProduktu,
                                   IFNULL(NumerFaktury,'')     AS NumerFaktury,
                                   IFNULL(PrzekazanePrzez,'') AS PrzekazanePrzez,
                                   IFNULL(DataPrzekazania,'') AS DataPrzekazania
                            FROM NiezarejestrowaneZwrotyReklamacyjne
                            WHERE IFNULL(CzyZarejestrowane,0)=0
                            ORDER BY datetime(DataPrzekazania) DESC", con))
                        using (var rd = await cmd.ExecuteReaderAsync())
                        {
                            while (await rd.ReadAsync())
                            {
                                var item = new ListViewItem(Convert.ToString(rd["Id"]));
                                item.SubItems.Add(Convert.ToString(rd["DaneKlienta"]));
                                item.SubItems.Add(Convert.ToString(rd["DaneProduktu"]));
                                item.Tag = rd["Id"];
                                _listViewInitialSelection.Items.Add(item);
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Błąd wczytywania: {ex.Message}", "Błąd");
                this.Close();
            }
            finally
            {
                _listViewInitialSelection.Enabled = true;
            }
        }

        private async void ListViewInitialSelection_DoubleClick(object sender, EventArgs e)
        {
            if (_listViewInitialSelection.SelectedItems.Count == 0) return;

            var selectedTag = _listViewInitialSelection.SelectedItems[0].Tag;
            _initialData = await ConvertToInitialData(selectedTag);

            _isInitializing = true;
            PopulateFormWithInitialData();
            _isInitializing = false;

            _pnlInitialSelection.Visible = false;
            _mainScrollPanel.Visible = true;

            await SearchClientsAsync();
        }

        private async Task<ComplaintInitialData> ConvertToInitialData(object sourceData)
        {
            var data = new ComplaintInitialData { OriginalObject = sourceData };

            // ALLEGRO
            if (_source == WizardSource.Allegro && sourceData is string disputeId)
            {
                data.Id = disputeId;
                using (var con = DatabaseHelper.GetConnection())
                {
                    await con.OpenAsync();
                    var cmd = new MySqlCommand(
                        "SELECT ad.*, aa.AccountName FROM AllegroDisputes ad JOIN AllegroAccounts aa ON ad.AllegroAccountId = aa.Id WHERE ad.DisputeId = @id",
                        con);
                    cmd.Parameters.AddWithValue("@id", disputeId);
                    using (var reader = await cmd.ExecuteReaderAsync())
                    {
                        if (await reader.ReadAsync())
                        {
                            data.AllegroAccountId = Convert.ToInt32(reader["AllegroAccountId"]);
                            data.allegroDisputeId = reader["DisputeId"].ToString();
                            data.allegroOrderId = reader["OrderId"].ToString();
                            data.allegroBuyerLogin = reader["BuyerLogin"].ToString();

                            data.ImieNazwisko = $"{reader["BuyerFirstName"]} {reader["BuyerLastName"]}".Trim();
                            if (string.IsNullOrWhiteSpace(data.ImieNazwisko))
                                data.ImieNazwisko = reader["BuyerLogin"].ToString();

                            data.NazwaFirmy = reader["DeliveryCompanyName"].ToString();
                            data.Ulica = reader["DeliveryStreet"].ToString();
                            data.KodPocztowy = reader["DeliveryZipCode"].ToString();
                            data.Miejscowosc = reader["DeliveryCity"].ToString();
                            data.Email = reader["BuyerEmail"].ToString();
                            data.Telefon = reader["DeliveryPhoneNumber"].ToString();

                            data.NazwaProduktu = reader["ProductName"].ToString();
                            data.OpisUsterki = reader["InitialMessageText"].ToString();

                            if (DateTime.TryParse(reader["BoughtAt"]?.ToString(), out var boughtAt))
                                data.DataZakupu = boughtAt;

                            data.SourceName = $"Allegro - {reader["AccountName"]}";
                        }
                    }
                }
                return data;
            }

            // GOOGLE SHEET
            if (_source == WizardSource.GoogleSheet && sourceData is ZgloszenieZArkusza sheetRow)
            {
                data.Id = sheetRow.RowIndex.ToString();
                var row = sheetRow.RowData;

                data.ImieNazwisko = GetValueFromRow(row, 2);
                data.NazwaFirmy = GetValueFromRow(row, 1);
                data.Ulica = GetValueFromRow(row, 3);
                data.KodPocztowy = GetValueFromRow(row, 4);
                data.Miejscowosc = GetValueFromRow(row, 5);
                data.Telefon = GetValueFromRow(row, 6);
                data.Email = GetValueFromRow(row, 7);
                data.NIP = GetValueFromRow(row, 13);

                data.NazwaProduktu = GetValueFromRow(row, 10);
                data.OpisUsterki = GetValueFromRow(row, 12);
                data.NumerFaktury = GetValueFromRow(row, 8);
                data.NumerSeryjny = GetValueFromRow(row, 11);

                if (DateTime.TryParse(GetValueFromRow(row, 9), out var dataZakupu))
                    data.DataZakupu = dataZakupu;

                data.SourceName = (sheetRow.SourceSheet == "B") ? "Truck-Shop" : "Ena-Truck";
                return data;
            }

            // ZWROTY
            if (_source == WizardSource.Zwroty && (sourceData is long || (sourceData is string && long.TryParse((string)sourceData, out _))))
            {
                long niezId = sourceData is long l ? l : Convert.ToInt64((string)sourceData);
                data.Id = niezId.ToString();

                using (var con = DatabaseHelper.GetConnection())
                {
                    await con.OpenAsync();
                    using (var cmd = new MySqlCommand(@"
                SELECT
                    Id,
                    IFNULL(DaneKlienta,'')        AS DaneKlienta,
                    IFNULL(DaneProduktu,'')       AS DaneProduktu,
                    IFNULL(NumerFaktury,'')       AS NumerFaktury,
                    IFNULL(NumerSeryjny,'')       AS NumerSeryjny,
                    IFNULL(ImieKlienta,'')        AS ImieKlienta,
                    IFNULL(NazwiskoKlienta,'')    AS NazwiskoKlienta,
                    IFNULL(EmailKlienta,'')       AS EmailKlienta,
                    IFNULL(TelefonKlienta,'')     AS TelefonKlienta,
                    IFNULL(AdresUlica,'')         AS AdresUlica,
                    IFNULL(AdresKodPocztowy,'')   AS AdresKodPocztowy,
                    IFNULL(AdresMiasto,'')        AS AdresMiasto,
                    IFNULL(NazwaProduktu,'')      AS NazwaProduktu,
                    IFNULL(NIP,'')                AS NIP,
                    DataZakupu,
                    IFNULL(OpisUsterki,'')        AS OpisUsterki,
                    IFNULL(UwagiMagazynu,'')      AS UwagiMagazynu,
                    IFNULL(KomentarzHandlowca,'') AS KomentarzHandlowca,
                    IFNULL(PrzekazanePrzez,'')    AS PrzekazanePrzez
                FROM NiezarejestrowaneZwrotyReklamacyjne
                WHERE Id=@id;", con))
                    {
                        cmd.Parameters.AddWithValue("@id", niezId);
                        using (var rd = await cmd.ExecuteReaderAsync())
                        {
                            if (await rd.ReadAsync())
                            {
                                var imie = Convert.ToString(rd["ImieKlienta"]);
                                var nazw = Convert.ToString(rd["NazwiskoKlienta"]);
                                data.ImieNazwisko = $"{imie} {nazw}".Trim();

                                data.Email = Convert.ToString(rd["EmailKlienta"]);
                                data.Telefon = Convert.ToString(rd["TelefonKlienta"]);
                                data.Ulica = Convert.ToString(rd["AdresUlica"]);
                                data.KodPocztowy = Convert.ToString(rd["AdresKodPocztowy"]);
                                data.Miejscowosc = Convert.ToString(rd["AdresMiasto"]);
                                data.NIP = Convert.ToString(rd["NIP"]);

                                var nazwaProduktuNowa = Convert.ToString(rd["NazwaProduktu"]);
                                var daneProduktuStare = Convert.ToString(rd["DaneProduktu"]);
                                data.NazwaProduktu = string.IsNullOrWhiteSpace(nazwaProduktuNowa) ? daneProduktuStare : nazwaProduktuNowa;
                                data.NumerFaktury = Convert.ToString(rd["NumerFaktury"]);
                                data.NumerSeryjny = Convert.ToString(rd["NumerSeryjny"]);
                                if (DateTime.TryParse(Convert.ToString(rd["DataZakupu"]), out var dz))
                                    data.DataZakupu = dz;

                                var opisZBazy = Convert.ToString(rd["OpisUsterki"]);
                                if (!string.IsNullOrWhiteSpace(opisZBazy))
                                {
                                    data.OpisUsterki = opisZBazy;
                                }
                                else
                                {
                                    var uwagiMag = Convert.ToString(rd["UwagiMagazynu"]);
                                    var uwagiHand = Convert.ToString(rd["KomentarzHandlowca"]);
                                    var przekazal = Convert.ToString(rd["PrzekazanePrzez"]);
                                    var sb = new StringBuilder();
                                    if (!string.IsNullOrWhiteSpace(uwagiMag)) sb.AppendLine($"Uwagi magazynu: {uwagiMag}");
                                    if (!string.IsNullOrWhiteSpace(uwagiHand)) sb.AppendLine($"Uwagi handlowca: {uwagiHand}");
                                    if (!string.IsNullOrWhiteSpace(przekazal)) sb.AppendLine($"Przekazał: {przekazal}");
                                    data.OpisUsterki = sb.ToString().Trim();
                                }

                                if (string.IsNullOrWhiteSpace(data.ImieNazwisko) || string.IsNullOrWhiteSpace(data.Ulica))
                                {
                                    var daneKlientaStare = Convert.ToString(rd["DaneKlienta"]);
                                    if (!string.IsNullOrWhiteSpace(daneKlientaStare))
                                    {
                                        var parts = daneKlientaStare.Split(new[] { " | " }, StringSplitOptions.None);
                                        if (string.IsNullOrWhiteSpace(data.ImieNazwisko) && parts.Length >= 1)
                                            data.ImieNazwisko = parts[0];

                                        if (string.IsNullOrWhiteSpace(data.Ulica) && parts.Length >= 2)
                                        {
                                            var addr = parts[1];
                                            var idx = addr.LastIndexOf(',');
                                            if (idx > -1)
                                            {
                                                data.Ulica = addr.Substring(0, idx).Trim();
                                                var rest = addr.Substring(idx + 1).Trim();
                                                var sp = rest.Split(new[] { ' ' }, 2, StringSplitOptions.RemoveEmptyEntries);
                                                if (sp.Length == 2) { data.KodPocztowy = sp[0]; data.Miejscowosc = sp[1]; }
                                            }
                                            else { data.Ulica = addr; }
                                        }

                                        if (string.IsNullOrWhiteSpace(data.Telefon))
                                        {
                                            var telPart = parts.FirstOrDefault(p => p.StartsWith("tel:", StringComparison.OrdinalIgnoreCase));
                                            if (telPart != null) data.Telefon = telPart.Substring(4).Trim();
                                        }
                                        if (string.IsNullOrWhiteSpace(data.Email))
                                        {
                                            var mailPart = parts.FirstOrDefault(p => p.IndexOf("e-mail:", StringComparison.OrdinalIgnoreCase) >= 0);
                                            if (mailPart != null) data.Email = mailPart.Split(new[] { ':' }, 2)[1].Trim();
                                        }
                                    }
                                }

                                data.SourceName = "Allegro — Zwrot";
                            }
                        }
                    }
                }

                return data;
            }

            data.SourceName = "Nieznane";
            return data;
        }

        private string GetValueFromRow(IList<object> row, int index)
        {
            return row.Count > index && row[index] != null ? row[index].ToString() : "";
        }

        private void PopulateFormWithInitialData()
        {
            _txtImie.Text = _initialData.ImieNazwisko;
            _txtFirma.Text = _initialData.NazwaFirmy;
            _txtNIP.Text = _initialData.NIP;
            _txtUlica.Text = _initialData.Ulica;
            _txtKod.Text = _initialData.KodPocztowy;
            _txtMiasto.Text = _initialData.Miejscowosc;
            _txtEmail.Text = _initialData.Email;
            _txtTelefon.Text = _initialData.Telefon;
            _txtProductSearch.Text = _initialData.NazwaProduktu;
            _txtDescription.Text = _initialData.OpisUsterki;
            _txtFaktura.Text = _initialData.NumerFaktury;
            _txtSerialNumber.Text = _initialData.NumerSeryjny;
            _chkIsCompany.Checked = _initialData.IsCompany;
            if (_initialData.DataZakupu.HasValue)
            {
                _dtpPurchaseDate.Value = _initialData.DataZakupu.Value;
            }
        }

        #endregion

        #region Save Logic

        private async Task SaveComplaintAsync()
        {
            if (_selectedClient == null && !_chkSaveAsNew.Checked)
            {
                MessageBox.Show("Wybierz klienta!", "Błąd");
                return;
            }

            if (string.IsNullOrEmpty(_txtDescription.Text))
            {
                MessageBox.Show("Opis usterki jest wymagany!", "Błąd");
                return;
            }

            this.Cursor = Cursors.WaitCursor;
            _btnSave.Enabled = false;

            try
            {
                int clientId = _chkSaveAsNew.Checked ? await AddNewClientAsync() : _selectedClient.Id;
                string complaintNumber = await GenerateComplaintNumberAsync();
                string source = DetermineComplaintSource();

                await InsertComplaintAsync(complaintNumber, clientId, source);
                await PerformPostSaveActionsAsync(complaintNumber);

                MessageBox.Show($"Zgłoszenie {complaintNumber} utworzone!", "Sukces");

                this.DialogResult = DialogResult.OK;
                this.Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Błąd zapisu: {ex.Message}\n\n{ex.StackTrace}", "Błąd");
            }
            finally
            {
                this.Cursor = Cursors.Default;
                _btnSave.Enabled = true;
            }
        }

        private async Task<int> AddNewClientAsync()
        {
            string query = @"INSERT INTO Klienci (ImieNazwisko, NazwaFirmy, NIP, Ulica, KodPocztowy, Miejscowosc, Email, Telefon) 
                             VALUES (@imie, @firma, @nip, @ulica, @kod, @miasto, @mail, @tel); 
                             SELECT LAST_INSERT_ID();";

            var parameters = new[] {
                new MySqlParameter("@imie", _txtImie.Text),
                new MySqlParameter("@firma", _txtFirma.Text),
                new MySqlParameter("@nip", _txtNIP.Text),
                new MySqlParameter("@ulica", _txtUlica.Text),
                new MySqlParameter("@kod", _txtKod.Text),
                new MySqlParameter("@miasto", _txtMiasto.Text),
                new MySqlParameter("@mail", _txtEmail.Text),
                new MySqlParameter("@tel", _txtTelefon.Text)
            };

            var newId = await _dbService.ExecuteScalarAsync(query, parameters);
            return Convert.ToInt32(newId);
        }

        private async Task<string> GenerateComplaintNumberAsync()
        {
            string year = DateTime.Now.ToString("yy");
            string pattern = $"%/{year}";
            string query = "SELECT NrZgloszenia FROM Zgloszenia WHERE NrZgloszenia LIKE @pattern ORDER BY CAST(SUBSTRING(NrZgloszenia, 1, LOCATE('/', NrZgloszenia) - 1) AS SIGNED) DESC LIMIT 1";
            var result = await _dbService.ExecuteScalarAsync(query, new MySqlParameter("@pattern", pattern));

            int nextNumber = 1;
            if (result != null && !string.IsNullOrEmpty(result.ToString()))
            {
                nextNumber = int.Parse(result.ToString().Split('/')[0]) + 1;
            }

            return $"{nextNumber:D3}/{year}";
        }

        private string DetermineComplaintSource()
        {
            if (_source == WizardSource.Manual)
                return _rbEnaTruck.Checked ? "Ena-Truck" : "Truck-Shop";

            return _initialData?.SourceName ?? "Nieznane";
        }

        private async Task InsertComplaintAsync(string number, int clientId, string source)
        {
            string query = @"INSERT INTO Zgloszenia 
                (NrZgloszenia, KlientID, ProduktID, DataZgloszenia, DataZakupu, NrFaktury, NrSeryjny, OpisUsterki, GwarancjaPlatna, skad, StatusOgolny, StatusKlient, StatusProducent, allegroDisputeId, allegroOrderId, allegroBuyerLogin, allegroAccountId)
                VALUES (@nr, @klientId, @produktId, @dataZglosz, @dataZakup, @nrFv, @nrSn, @opis, @typ, @skad, 'Procesowana', 'Zgłoszone', 'Oczekuje na zgłoszenie', @disputeId, @orderId, @buyerLogin, @accountId)";

            var parameters = new List<MySqlParameter> {
                new MySqlParameter("@nr", number),
                new MySqlParameter("@klientId", clientId),
                new MySqlParameter("@produktId", _selectedProduct?.Id ?? (object)DBNull.Value),
                new MySqlParameter("@dataZglosz", DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss")),
                new MySqlParameter("@dataZakup", _dtpPurchaseDate.Value.ToString("yyyy-MM-dd")),
                new MySqlParameter("@nrFv", _txtFaktura.Text),
                new MySqlParameter("@nrSn", _txtSerialNumber.Text),
                new MySqlParameter("@opis", _txtDescription.Text),
                new MySqlParameter("@typ", _rbPlatna.Checked ? "Płatna" : "Gwarancyjna"),
                new MySqlParameter("@skad", source),
                new MySqlParameter("@disputeId", _source == WizardSource.Allegro ? (object)_initialData.allegroDisputeId : DBNull.Value),
                new MySqlParameter("@orderId", _source == WizardSource.Allegro ? (object)_initialData.allegroOrderId : DBNull.Value),
                new MySqlParameter("@buyerLogin", _source == WizardSource.Allegro ? (object)_initialData.allegroBuyerLogin : DBNull.Value),
                new MySqlParameter("@accountId", _source == WizardSource.Allegro ? (object)_initialData.AllegroAccountId : DBNull.Value)
            };

            await _dbService.ExecuteNonQueryAsync(query, parameters.ToArray());
        }

        private async Task PerformPostSaveActionsAsync(string complaintNumber)
        {
            string sourceText = DetermineComplaintSource();
            await new DziennikLogger().DodajAsync(Program.fullName, $"Utworzono zgłoszenie", complaintNumber);
            new Dzialaniee().DodajNoweDzialanie(complaintNumber, Program.fullName, $"Utworzono zgłoszenie (Smart Wizard V2)");

            if (_source == WizardSource.Allegro)
            {
                var complaintIdResult = await _dbService.ExecuteScalarAsync("SELECT Id FROM Zgloszenia WHERE NrZgloszenia = @nr", new MySqlParameter("@nr", complaintNumber));
                long complaintId = Convert.ToInt64(complaintIdResult);
                string query = "UPDATE AllegroDisputes SET ComplaintId = @complaintId WHERE DisputeId = @disputeId";
                await _dbService.ExecuteNonQueryAsync(query, new MySqlParameter("@complaintId", complaintId), new MySqlParameter("@disputeId", _initialData.Id));
            }

            if (_source == WizardSource.GoogleSheet)
            {
                var zgloszenie = _initialData.OriginalObject as ZgloszenieZArkusza;
                if (zgloszenie != null)
                {
                    try
                    {
                        GoogleCredential credential;
                        using (var stream = new FileStream("reklamacje-baza-c36d05b0ffdb.json", FileMode.Open, FileAccess.Read))
                        {
                            credential = GoogleCredential.FromStream(stream).CreateScoped(new[] { SheetsService.Scope.Spreadsheets });
                        }
                        var service = new SheetsService(new BaseClientService.Initializer() { HttpClientInitializer = credential });
                        string spreadsheetId = "1VXGP4Cckt6NmSHtiv-Um7nqg-itLMczAGd-5a_Tc4Ds";
                        var spreadsheet = await service.Spreadsheets.Get(spreadsheetId).ExecuteAsync();
                        var sheet = spreadsheet.Sheets.FirstOrDefault(s => s.Properties.Title == zgloszenie.SourceSheet);
                        if (sheet?.Properties?.SheetId != null)
                        {
                            var request = new Request { DeleteDimension = new DeleteDimensionRequest { Range = new DimensionRange { SheetId = sheet.Properties.SheetId.Value, Dimension = "ROWS", StartIndex = zgloszenie.RowIndex - 1, EndIndex = zgloszenie.RowIndex } } };
                            var batchUpdateRequest = new BatchUpdateSpreadsheetRequest { Requests = new List<Request> { request } };
                            await service.Spreadsheets.BatchUpdate(batchUpdateRequest, spreadsheetId).ExecuteAsync();
                        }
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show($"Nie udało się usunąć wiersza z arkusza: {ex.Message}", "Ostrzeżenie");
                    }
                }
            }

            if (_source == WizardSource.Zwroty)
            {
                long niezId;
                if (long.TryParse(_initialData?.Id ?? "", out niezId))
                {
                    using (var con = DatabaseHelper.GetConnection())
                    {
                        await con.OpenAsync();
                        using (var cmd = new MySqlCommand(@"UPDATE NiezarejestrowaneZwrotyReklamacyjne SET CzyZarejestrowane = 1 WHERE Id = @id", con))
                        {
                            cmd.Parameters.AddWithValue("@id", niezId);
                            await cmd.ExecuteNonQueryAsync();
                        }
                    }
                }
            }
        }

        #endregion
    }

    /// <summary>
    /// Klasa pomocnicza dla zgłoszeń z Google Sheets
    /// </summary>
    public class ZgloszenieZArkusza
    {
        public IList<object> RowData { get; set; }
        public string SourceSheet { get; set; }
        public int RowIndex { get; set; }
    
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