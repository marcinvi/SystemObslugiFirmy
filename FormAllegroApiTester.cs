// ========================================
// FormAllegroApiTester.cs
// Formularz testowy API Allegro
// Data: 2026-01-09
// WERSJA: NAPRAWIONA (wszystkie błędy usunięte)
// ========================================

using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;
using MySql.Data.MySqlClient;
using Reklamacje_Dane.Allegro;
using Reklamacje_Dane.Allegro.Issues;
using AllegroChat = Reklamacje_Dane.Allegro.Issues.ChatMessage;

namespace Reklamacje_Dane
{
    public partial class FormAllegroApiTester : Form
    {
        private AllegroApiClient _apiClient;
        private AllegroApiTesterService _testerService;
        private int _selectedAccountId;
        private List<Issue> _currentIssues;

        public FormAllegroApiTester()
        {
            InitializeComponent();
            this.Load += FormAllegroApiTester_Load;
        

            // Włącz sprawdzanie pisowni dla wszystkich TextBoxów
            EnableSpellCheckOnAllTextBoxes();
        }

        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            this.Text = "🧪 Allegro API Tester";
            this.Size = new Size(1400, 900);
            this.StartPosition = FormStartPosition.CenterScreen;
            this.Font = new Font("Segoe UI", 9F);

            // Panel główny
            var mainPanel = new TableLayoutPanel
            {
                Dock = DockStyle.Fill,
                ColumnCount = 2,
                RowCount = 1,
                Padding = new Padding(10)
            };
            mainPanel.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 60F));
            mainPanel.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 40F));

            // ===== LEWA STRONA - Kontrolki =====
            var leftPanel = new Panel { Dock = DockStyle.Fill };
            var leftLayout = new TableLayoutPanel
            {
                Dock = DockStyle.Fill,
                RowCount = 6,
                ColumnCount = 1,
                Padding = new Padding(5)
            };

            // Sekcja wyboru konta
            var accountPanel = CreateAccountPanel();
            leftLayout.Controls.Add(accountPanel, 0, 0);
            leftLayout.RowStyles.Add(new RowStyle(SizeType.Absolute, 100F));

            // Sekcja testów Issues
            var issuesPanel = CreateIssuesTestPanel();
            leftLayout.Controls.Add(issuesPanel, 0, 1);
            leftLayout.RowStyles.Add(new RowStyle(SizeType.Absolute, 200F));

            // Sekcja testów Chat
            var chatPanel = CreateChatTestPanel();
            leftLayout.Controls.Add(chatPanel, 0, 2);
            leftLayout.RowStyles.Add(new RowStyle(SizeType.Absolute, 150F));

            // Sekcja testów Email i Order
            var emailOrderPanel = CreateEmailOrderTestPanel();
            leftLayout.Controls.Add(emailOrderPanel, 0, 3);
            leftLayout.RowStyles.Add(new RowStyle(SizeType.Absolute, 150F));

            // Grid z wynikami
            var gridPanel = CreateGridPanel();
            leftLayout.Controls.Add(gridPanel, 0, 4);
            leftLayout.RowStyles.Add(new RowStyle(SizeType.Percent, 100F));

            // Przyciski akcji
            var actionsPanel = CreateActionsPanel();
            leftLayout.Controls.Add(actionsPanel, 0, 5);
            leftLayout.RowStyles.Add(new RowStyle(SizeType.Absolute, 60F));

            leftPanel.Controls.Add(leftLayout);
            mainPanel.Controls.Add(leftPanel, 0, 0);

            // ===== PRAWA STRONA - Logi =====
            var logPanel = CreateLogPanel();
            mainPanel.Controls.Add(logPanel, 1, 0);

            this.Controls.Add(mainPanel);
        }

        #region Panel Creation Methods

        private GroupBox CreateAccountPanel()
        {
            var panel = new GroupBox
            {
                Text = "1️⃣ Wybór konta Allegro",
                Dock = DockStyle.Fill,
                Padding = new Padding(10)
            };

            cmbAccounts = new ComboBox
            {
                Dock = DockStyle.Top,
                DropDownStyle = ComboBoxStyle.DropDownList,
                Font = new Font("Segoe UI", 10F, FontStyle.Bold)
            };
            cmbAccounts.SelectedIndexChanged += CmbAccounts_SelectedIndexChanged;

            var infoLabel = new Label
            {
                Text = "Wybierz konto do testowania API",
                Dock = DockStyle.Top,
                ForeColor = Color.Gray,
                Padding = new Padding(0, 5, 0, 5)
            };

            lblAccountStatus = new Label
            {
                Text = "Status: Nie wybrano konta",
                Dock = DockStyle.Top,
                Font = new Font("Segoe UI", 9F, FontStyle.Bold),
                ForeColor = Color.OrangeRed,
                Padding = new Padding(0, 10, 0, 0)
            };

            panel.Controls.Add(lblAccountStatus);
            panel.Controls.Add(cmbAccounts);
            panel.Controls.Add(infoLabel);

            return panel;
        }

        private GroupBox CreateIssuesTestPanel()
        {
            var panel = new GroupBox
            {
                Text = "2️⃣ Test Issues (Dyskusje i Reklamacje)",
                Dock = DockStyle.Fill,
                Padding = new Padding(10)
            };

            var layout = new TableLayoutPanel
            {
                Dock = DockStyle.Fill,
                RowCount = 3,
                ColumnCount = 2
            };

            btnTestIssues = new Button
            {
                Text = "🔍 Pobierz wszystkie Issues",
                Dock = DockStyle.Fill,
                Height = 40,
                BackColor = Color.FromArgb(0, 120, 215),
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat,
                Font = new Font("Segoe UI", 10F, FontStyle.Bold)
            };
            btnTestIssues.Click += BtnTestIssues_Click;

            btnTestIssueDetails = new Button
            {
                Text = "📋 Pobierz szczegóły Issue",
                Dock = DockStyle.Fill,
                Height = 40,
                BackColor = Color.FromArgb(0, 150, 136),
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat
            };
            btnTestIssueDetails.Click += BtnTestIssueDetails_Click;

            txtIssueId = new TextBox
            {
                Dock = DockStyle.Fill,
                Font = new Font("Segoe UI", 9F)
            };

            var infoLabel = new Label
            {
                Text = "💡 Issues = dyskusje + reklamacje razem",
                Dock = DockStyle.Fill,
                ForeColor = Color.Gray,
                TextAlign = ContentAlignment.MiddleLeft
            };

            var countLabel = new Label
            {
                Text = "Znaleziono: 0",
                Dock = DockStyle.Fill,
                Font = new Font("Segoe UI", 9F, FontStyle.Bold),
                TextAlign = ContentAlignment.MiddleLeft
            };
            lblIssuesCount = countLabel;

            layout.Controls.Add(btnTestIssues, 0, 0);
            layout.SetColumnSpan(btnTestIssues, 2);
            layout.Controls.Add(new Label { Text = "Issue ID:", Dock = DockStyle.Fill, TextAlign = ContentAlignment.MiddleRight }, 0, 1);
            layout.Controls.Add(txtIssueId, 1, 1);
            layout.Controls.Add(btnTestIssueDetails, 0, 2);
            layout.Controls.Add(countLabel, 1, 2);

            panel.Controls.Add(layout);
            return panel;
        }

        private GroupBox CreateChatTestPanel()
        {
            var panel = new GroupBox
            {
                Text = "3️⃣ Test Chat (Wiadomości)",
                Dock = DockStyle.Fill,
                Padding = new Padding(10)
            };

            var layout = new TableLayoutPanel
            {
                Dock = DockStyle.Fill,
                RowCount = 2,
                ColumnCount = 2
            };

            btnTestChat = new Button
            {
                Text = "💬 Pobierz czat dla Issue",
                Dock = DockStyle.Fill,
                Height = 40,
                BackColor = Color.FromArgb(156, 39, 176),
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat,
                Font = new Font("Segoe UI", 10F, FontStyle.Bold)
            };
            btnTestChat.Click += BtnTestChat_Click;

            lblChatCount = new Label
            {
                Text = "Wiadomości: 0",
                Dock = DockStyle.Fill,
                Font = new Font("Segoe UI", 9F, FontStyle.Bold),
                TextAlign = ContentAlignment.MiddleLeft
            };

            var infoLabel = new Label
            {
                Text = "⚠️ Wymaga Issue ID (wpisz ręcznie lub wybierz z grida)",
                Dock = DockStyle.Fill,
                ForeColor = Color.OrangeRed,
                Font = new Font("Segoe UI", 8F)
            };

            layout.Controls.Add(btnTestChat, 0, 0);
            layout.SetColumnSpan(btnTestChat, 2);
            layout.Controls.Add(lblChatCount, 0, 1);
            layout.Controls.Add(infoLabel, 1, 1);

            panel.Controls.Add(layout);
            return panel;
        }

        private GroupBox CreateEmailOrderTestPanel()
        {
            var panel = new GroupBox
            {
                Text = "4️⃣ Test Email i Zamówienia",
                Dock = DockStyle.Fill,
                Padding = new Padding(10)
            };

            var layout = new TableLayoutPanel
            {
                Dock = DockStyle.Fill,
                RowCount = 3,
                ColumnCount = 2
            };

            txtCheckoutFormId = new TextBox
            {
                Dock = DockStyle.Fill,
                Font = new Font("Segoe UI", 9F)
            };

            btnTestEmail = new Button
            {
                Text = "📧 Pobierz Email kupującego",
                Dock = DockStyle.Fill,
                Height = 40,
                BackColor = Color.FromArgb(255, 152, 0),
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat
            };
            btnTestEmail.Click += BtnTestEmail_Click;

            btnTestOrderDetails = new Button
            {
                Text = "📦 Pobierz szczegóły zamówienia",
                Dock = DockStyle.Fill,
                Height = 40,
                BackColor = Color.FromArgb(76, 175, 80),
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat
            };
            btnTestOrderDetails.Click += BtnTestOrderDetails_Click;

            layout.Controls.Add(new Label { Text = "CheckoutForm ID:", Dock = DockStyle.Fill, TextAlign = ContentAlignment.MiddleRight }, 0, 0);
            layout.Controls.Add(txtCheckoutFormId, 1, 0);
            layout.Controls.Add(btnTestEmail, 0, 1);
            layout.Controls.Add(btnTestOrderDetails, 1, 1);

            var infoLabel = new Label
            {
                Text = "💡 Email i dane zamówienia pochodzą z /order/checkout-forms",
                Dock = DockStyle.Fill,
                ForeColor = Color.Gray,
                Font = new Font("Segoe UI", 8F)
            };
            layout.Controls.Add(infoLabel, 0, 2);
            layout.SetColumnSpan(infoLabel, 2);

            panel.Controls.Add(layout);
            return panel;
        }

        private GroupBox CreateGridPanel()
        {
            var panel = new GroupBox
            {
                Text = "📊 Wyniki testów",
                Dock = DockStyle.Fill,
                Padding = new Padding(10)
            };

            dgvResults = new DataGridView
            {
                Dock = DockStyle.Fill,
                AllowUserToAddRows = false,
                AllowUserToDeleteRows = false,
                ReadOnly = true,
                SelectionMode = DataGridViewSelectionMode.FullRowSelect,
                MultiSelect = false,
                AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill,
                BackgroundColor = Color.White,
                BorderStyle = BorderStyle.None
            };
            dgvResults.SelectionChanged += DgvResults_SelectionChanged;

            panel.Controls.Add(dgvResults);
            return panel;
        }

        private Panel CreateActionsPanel()
        {
            var panel = new Panel
            {
                Dock = DockStyle.Fill,
                Padding = new Padding(0, 10, 0, 0)
            };

            var layout = new FlowLayoutPanel
            {
                Dock = DockStyle.Fill,
                FlowDirection = FlowDirection.LeftToRight,
                WrapContents = false
            };

            btnClearLogs = new Button
            {
                Text = "🗑️ Wyczyść logi",
                Width = 150,
                Height = 40,
                BackColor = Color.Gray,
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat
            };
            btnClearLogs.Click += (s, args) => txtLogs.Clear();

            btnOpenDatabase = new Button
            {
                Text = "💾 Otwórz bazę testową",
                Width = 180,
                Height = 40,
                BackColor = Color.FromArgb(63, 81, 181),
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat
            };
            btnOpenDatabase.Click += BtnOpenDatabase_Click;

            btnExportLogs = new Button
            {
                Text = "📄 Eksportuj logi",
                Width = 150,
                Height = 40,
                BackColor = Color.FromArgb(33, 150, 243),
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat
            };
            btnExportLogs.Click += BtnExportLogs_Click;

            layout.Controls.Add(btnClearLogs);
            layout.Controls.Add(btnOpenDatabase);
            layout.Controls.Add(btnExportLogs);

            panel.Controls.Add(layout);
            return panel;
        }

        private GroupBox CreateLogPanel()
        {
            var panel = new GroupBox
            {
                Text = "📝 Logi szczegółowe (Request/Response)",
                Dock = DockStyle.Fill,
                Padding = new Padding(10)
            };

            txtLogs = new TextBox
            {
                Dock = DockStyle.Fill,
                Multiline = true,
                ScrollBars = ScrollBars.Both,
                Font = new Font("Consolas", 9F),
                BackColor = Color.FromArgb(30, 30, 30),
                ForeColor = Color.FromArgb(220, 220, 220),
                BorderStyle = BorderStyle.None,
                WordWrap = false
            };

            panel.Controls.Add(txtLogs);
            return panel;
        }

        #endregion

        #region Event Handlers

        private async void FormAllegroApiTester_Load(object sender, EventArgs e)
        {
            await LoadAccountsAsync();
            AppendLog("✅ Formularz załadowany. Wybierz konto Allegro aby rozpocząć testy.");
        }

        private async Task LoadAccountsAsync()
        {
            try
            {
                using (var con = new MySqlConnection(DatabaseHelper.GetConnectionString()))
                {
                    await con.OpenAsync();
                    var query = "SELECT Id, AccountName FROM AllegroAccounts WHERE IsAuthorized = 1 ORDER BY AccountName";
                    using (var cmd = new MySqlCommand(query, con))
                    using (var reader = await cmd.ExecuteReaderAsync())
                    {
                        var accounts = new List<dynamic>();
                        while (await reader.ReadAsync())
                        {
                            accounts.Add(new
                            {
                                Id = reader.GetInt32("Id"),
                                Name = reader.GetString("AccountName")
                            });
                        }

                        cmbAccounts.DisplayMember = "Name";
                        cmbAccounts.ValueMember = "Id";
                        cmbAccounts.DataSource = accounts;
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Błąd ładowania kont: {ex.Message}", "Błąd", 
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private async void CmbAccounts_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (cmbAccounts.SelectedValue == null) return;

            _selectedAccountId = Convert.ToInt32(cmbAccounts.SelectedValue);
            
            try
            {
                lblAccountStatus.Text = "Status: Łączenie...";
                lblAccountStatus.ForeColor = Color.Orange;
                Application.DoEvents();

                _apiClient = new AllegroApiClient(
                    await GetClientIdAsync(_selectedAccountId),
                    await GetClientSecretAsync(_selectedAccountId)
                );
                await _apiClient.InitializeAsync(_selectedAccountId);

                _testerService = new AllegroApiTesterService(_apiClient, _selectedAccountId, AppendLog);

                lblAccountStatus.Text = $"Status: ✅ Połączono ({cmbAccounts.Text})";
                lblAccountStatus.ForeColor = Color.Green;

                AppendLog($"✅ Połączono z kontem: {cmbAccounts.Text}");
                AppendLog($"   Token expires: {_apiClient.Token?.ExpirationDate:yyyy-MM-dd HH:mm}");
            }
            catch (Exception ex)
            {
                lblAccountStatus.Text = "Status: ❌ Błąd połączenia";
                lblAccountStatus.ForeColor = Color.Red;
                MessageBox.Show($"Nie można połączyć z kontem:\n{ex.Message}", "Błąd", 
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private async void BtnTestIssues_Click(object sender, EventArgs e)
        {
            if (!ValidateApiClient()) return;

            btnTestIssues.Enabled = false;
            btnTestIssues.Text = "⏳ Pobieram...";

            try
            {
                var result = await _testerService.TestGetIssuesAsync();
                
                if (result.Success && result.Data != null)
                {
                    var issues = result.Data as List<Issue>;
                    if (issues != null)
                    {
                        _currentIssues = issues;
                        DisplayIssuesInGrid(issues);
                        lblIssuesCount.Text = $"Znaleziono: {issues.Count}";
                        
                        MessageBox.Show(
                            $"✅ Pobrano {issues.Count} issues\n" +
                            $"Czas: {result.DurationMs}ms\n\n" +
                            $"Sprawdź logi po prawej stronie!",
                            "Sukces", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    }
                }
                else
                {
                    MessageBox.Show($"❌ {result.ErrorMessage}", "Błąd", 
                        MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
            finally
            {
                btnTestIssues.Enabled = true;
                btnTestIssues.Text = "🔍 Pobierz wszystkie Issues";
            }
        }

        private async void BtnTestIssueDetails_Click(object sender, EventArgs e)
        {
            if (!ValidateApiClient()) return;
            
            var issueId = txtIssueId.Text.Trim();
            if (string.IsNullOrEmpty(issueId))
            {
                MessageBox.Show("Podaj Issue ID lub wybierz Issue z grida!", "Brak ID", 
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            btnTestIssueDetails.Enabled = false;
            btnTestIssueDetails.Text = "⏳ Pobieram...";

            try
            {
                var result = await _testerService.TestGetIssueDetailsAsync(issueId);
                
                if (result.Success && result.Data != null)
                {
                    var issue = result.Data as Issue;
                    if (issue != null)
                    {
                        // Wypełnij CheckoutFormId automatycznie
                        if (!string.IsNullOrEmpty(issue.CheckoutForm?.Id))
                        {
                            txtCheckoutFormId.Text = issue.CheckoutForm.Id;
                        }
                        
                        MessageBox.Show(
                            $"✅ Pobrano szczegóły Issue\n\n" +
                            $"Type: {issue.Type}\n" +
                            $"Status: {issue.CurrentState?.Status}\n" +
                            $"CheckoutForm: {issue.CheckoutForm?.Id}\n\n" +
                            $"Sprawdź logi po prawej!",
                            "Sukces", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    }
                }
                else
                {
                    MessageBox.Show($"❌ {result.ErrorMessage}", "Błąd", 
                        MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
            finally
            {
                btnTestIssueDetails.Enabled = true;
                btnTestIssueDetails.Text = "📋 Pobierz szczegóły Issue";
            }
        }

        private async void BtnTestChat_Click(object sender, EventArgs e)
        {
            if (!ValidateApiClient()) return;
            
            var issueId = txtIssueId.Text.Trim();
            if (string.IsNullOrEmpty(issueId))
            {
                MessageBox.Show("Podaj Issue ID lub wybierz Issue z grida!", "Brak ID", 
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            btnTestChat.Enabled = false;
            btnTestChat.Text = "⏳ Pobieram...";

            try
            {
                var result = await _testerService.TestGetChatAsync(issueId);
                
                if (result.Success && result.Data != null)
                {
                    var messages = result.Data as List<AllegroChat>;
                    if (messages != null)
                    {
                        DisplayChatInGrid(messages);
                        lblChatCount.Text = $"Wiadomości: {messages.Count}";
                        
                        MessageBox.Show(
                            $"✅ Pobrano {messages.Count} wiadomości\n" +
                            $"Czas: {result.DurationMs}ms\n\n" +
                            $"Sprawdź logi po prawej stronie!",
                            "Sukces", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    }
                }
                else
                {
                    MessageBox.Show($"❌ {result.ErrorMessage}", "Błąd", 
                        MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
            finally
            {
                btnTestChat.Enabled = true;
                btnTestChat.Text = "💬 Pobierz czat dla Issue";
            }
        }

        private async void BtnTestEmail_Click(object sender, EventArgs e)
        {
            if (!ValidateApiClient()) return;
            
            var checkoutFormId = txtCheckoutFormId.Text.Trim();
            if (string.IsNullOrEmpty(checkoutFormId))
            {
                MessageBox.Show("Podaj CheckoutForm ID!\n\nMożesz go uzyskać wybierając Issue z grida.", 
                    "Brak ID", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            btnTestEmail.Enabled = false;
            btnTestEmail.Text = "⏳ Pobieram...";

            try
            {
                var result = await _testerService.TestGetBuyerEmailAsync(checkoutFormId);
                
                if (result.Success && result.Data != null)
                {
                    var emails = result.Data as List<string>;
                    var email = emails?.FirstOrDefault();
                    
                    if (!string.IsNullOrEmpty(email))
                    {
                        MessageBox.Show(
                            $"✅ Email kupującego:\n\n{email}\n\n" +
                            (email.Contains("@allegromail.pl") 
                                ? "ℹ️ To maskowany email Allegro (ale funkcjonalny)" 
                                : ""),
                            "Sukces", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    }
                    else
                    {
                        MessageBox.Show(
                            $"❌ Nie udało się pobrać emaila!\n\n" +
                            $"Sprawdź logi - może brak autoryzacji?",
                            "Błąd", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }
                else
                {
                    MessageBox.Show(
                        $"❌ Nie udało się pobrać emaila!\n\n" +
                        $"Sprawdź logi - może brak autoryzacji?",
                        "Błąd", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
            finally
            {
                btnTestEmail.Enabled = true;
                btnTestEmail.Text = "📧 Pobierz Email kupującego";
            }
        }

        private async void BtnTestOrderDetails_Click(object sender, EventArgs e)
        {
            if (!ValidateApiClient()) return;
            
            var checkoutFormId = txtCheckoutFormId.Text.Trim();
            if (string.IsNullOrEmpty(checkoutFormId))
            {
                MessageBox.Show("Podaj CheckoutForm ID!", "Brak ID", 
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            btnTestOrderDetails.Enabled = false;
            btnTestOrderDetails.Text = "⏳ Pobieram...";

            try
            {
                var result = await _testerService.TestGetOrderDetailsAsync(checkoutFormId);
                
                if (result.Success && result.Data != null)
                {
                    var order = result.Data as OrderDetails;
                    if (order != null)
                    {
                        MessageBox.Show(
                            $"✅ Pobrano szczegóły zamówienia\n\n" +
                            $"Email: {order.Buyer?.Email}\n" +
                            $"Imię: {order.Buyer?.FirstName} {order.Buyer?.LastName}\n" +
                            $"Telefon: {order.Buyer?.PhoneNumber}\n" +
                            $"Kwota: {order.Payment?.TotalToPay?.Amount} {order.Payment?.TotalToPay?.Currency}\n\n" +
                            $"Sprawdź logi dla pełnych danych!",
                            "Sukces", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    }
                }
                else
                {
                    MessageBox.Show($"❌ {result.ErrorMessage}", "Błąd", 
                        MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
            finally
            {
                btnTestOrderDetails.Enabled = true;
                btnTestOrderDetails.Text = "📦 Pobierz szczegóły zamówienia";
            }
        }

        private void DgvResults_SelectionChanged(object sender, EventArgs e)
        {
            if (dgvResults.SelectedRows.Count == 0) return;

            var row = dgvResults.SelectedRows[0];
            
            // Wypełnij Issue ID
            if (row.Cells["Id"]?.Value != null)
            {
                txtIssueId.Text = row.Cells["Id"].Value.ToString();
            }
            
            // Wypełnij CheckoutForm ID
            if (row.Cells["CheckoutFormId"]?.Value != null)
            {
                txtCheckoutFormId.Text = row.Cells["CheckoutFormId"].Value.ToString();
            }
        }

        private void BtnOpenDatabase_Click(object sender, EventArgs e)
        {
            try
            {
                MessageBox.Show(
                    "📊 Tabele testowe:\n\n" +
                    "• AllegroApiTestLogs - wszystkie logi API\n" +
                    "• AllegroIssues_TEST - issues\n" +
                    "• AllegroChatMessages_TEST - wiadomości\n" +
                    "• AllegroOrderDetails_TEST - zamówienia\n\n" +
                    "Otwórz phpMyAdmin lub HeidiSQL aby zobaczyć dane.",
                    "Tabele testowe", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Błąd: {ex.Message}", "Błąd", 
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void BtnExportLogs_Click(object sender, EventArgs e)
        {
            try
            {
                var saveDialog = new SaveFileDialog
                {
                    Filter = "Text files (*.txt)|*.txt|All files (*.*)|*.*",
                    FileName = $"AllegroApiTest_{DateTime.Now:yyyyMMdd_HHmmss}.txt",
                    Title = "Zapisz logi"
                };

                if (saveDialog.ShowDialog() == DialogResult.OK)
                {
                    System.IO.File.WriteAllText(saveDialog.FileName, txtLogs.Text);
                    MessageBox.Show($"✅ Logi zapisane do:\n{saveDialog.FileName}", "Sukces", 
                        MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Błąd zapisu: {ex.Message}", "Błąd", 
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        #endregion

        #region Helper Methods

        private bool ValidateApiClient()
        {
            if (_apiClient == null || _testerService == null)
            {
                MessageBox.Show("Wybierz konto Allegro!", "Brak połączenia", 
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return false;
            }
            return true;
        }

        private void AppendLog(string message)
        {
            if (txtLogs.InvokeRequired)
            {
                txtLogs.Invoke(new Action(() => AppendLog(message)));
                return;
            }

            txtLogs.AppendText(message + Environment.NewLine);
            txtLogs.SelectionStart = txtLogs.Text.Length;
            txtLogs.ScrollToCaret();
        }

        private void DisplayIssuesInGrid(List<Issue> issues)
        {
            if (issues == null) return;

            var displayData = issues.Select(i => new
            {
                Id = i.Id,
                Type = i.Type,
                Subject = i.Subject,
                Status = i.CurrentState?.Status,
                BuyerLogin = i.Buyer?.Login,
                OpenedDate = i.OpenedDate,
                CheckoutFormId = i.CheckoutForm?.Id,
                OfferName = i.Offer?.Name
            }).ToList();

            dgvResults.DataSource = displayData;
            
            // Formatuj kolumny
            if (dgvResults.Columns["Id"] != null)
                dgvResults.Columns["Id"].Width = 100;
            if (dgvResults.Columns["Type"] != null)
                dgvResults.Columns["Type"].Width = 80;
            if (dgvResults.Columns["OpenedDate"] != null)
                dgvResults.Columns["OpenedDate"].DefaultCellStyle.Format = "yyyy-MM-dd HH:mm";
        }

        private void DisplayChatInGrid(List<AllegroChat> messages)
        {
            if (messages == null) return;

            var displayData = messages.Select(m => new
            {
                Id = m.Id,
                Author = m.Author?.Login,
                Role = m.Author?.Role,
                CreatedAt = m.CreatedAt,
                Text = m.Text
            }).ToList();

            dgvResults.DataSource = displayData;
            
            // Formatuj kolumny
            if (dgvResults.Columns["CreatedAt"] != null)
                dgvResults.Columns["CreatedAt"].DefaultCellStyle.Format = "yyyy-MM-dd HH:mm:ss";
            if (dgvResults.Columns["Text"] != null)
                dgvResults.Columns["Text"].AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;
        }

        private async Task<string> GetClientIdAsync(int accountId)
        {
            using (var con = new MySqlConnection(DatabaseHelper.GetConnectionString()))
            {
                await con.OpenAsync();
                var query = "SELECT ClientId FROM AllegroAccounts WHERE Id = @Id";
                using (var cmd = new MySqlCommand(query, con))
                {
                    cmd.Parameters.AddWithValue("@Id", accountId);
                    return (string)await cmd.ExecuteScalarAsync();
                }
            }
        }

        private async Task<string> GetClientSecretAsync(int accountId)
        {
            using (var con = new MySqlConnection(DatabaseHelper.GetConnectionString()))
            {
                await con.OpenAsync();
                var query = "SELECT ClientSecret FROM AllegroAccounts WHERE Id = @Id";
                using (var cmd = new MySqlCommand(query, con))
                {
                    cmd.Parameters.AddWithValue("@Id", accountId);
                    return EncryptionHelper.DecryptString((string)await cmd.ExecuteScalarAsync());
                }
            }
        }

        #endregion

        #region Fields

        private System.ComponentModel.IContainer components = null;
        private ComboBox cmbAccounts;
        private Label lblAccountStatus;
        private Label lblIssuesCount;
        private Label lblChatCount;
        private Button btnTestIssues;
        private Button btnTestIssueDetails;
        private Button btnTestChat;
        private Button btnTestEmail;
        private Button btnTestOrderDetails;
        private Button btnClearLogs;
        private Button btnOpenDatabase;
        private Button btnExportLogs;
        private TextBox txtIssueId;
        private TextBox txtCheckoutFormId;
        private TextBox txtLogs;
        private DataGridView dgvResults;

        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #endregion
    
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
