using System;
using System.Data;
using MySql.Data.MySqlClient;
using System.Drawing;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Reklamacje_Dane
{
    /// <summary>
    /// Formularz do przeglądania i zarządzania zwrotami Allegro
    /// </summary>
    public partial class FormAllegroReturns : Form
    {
        private DataGridView dgvReturns;
        private ComboBox cmbAccounts;
        private ComboBox cmbStatus;
        private Button btnRefresh;
        private Button btnSync;
        private Label lblStats;

        public FormAllegroReturns()
        {
            InitializeComponent();
            this.Load += FormAllegroReturns_Load;
        

            // Włącz sprawdzanie pisowni dla wszystkich TextBoxów
            EnableSpellCheckOnAllTextBoxes();
        }

        private void InitializeComponent()
        {
            this.Text = "Zwroty Allegro";
            this.Size = new Size(1200, 700);
            this.StartPosition = FormStartPosition.CenterScreen;

            // ComboBox - Konta
            var lblAccount = new Label
            {
                Text = "Konto:",
                Location = new Point(10, 15),
                AutoSize = true
            };
            this.Controls.Add(lblAccount);

            cmbAccounts = new ComboBox
            {
                Location = new Point(60, 12),
                Width = 200,
                DropDownStyle = ComboBoxStyle.DropDownList
            };
            cmbAccounts.SelectedIndexChanged += async (s, e) => await LoadReturnsAsync();
            this.Controls.Add(cmbAccounts);

            // ComboBox - Status
            var lblStatus = new Label
            {
                Text = "Status:",
                Location = new Point(280, 15),
                AutoSize = true
            };
            this.Controls.Add(lblStatus);

            cmbStatus = new ComboBox
            {
                Location = new Point(330, 12),
                Width = 150,
                DropDownStyle = ComboBoxStyle.DropDownList
            };
            cmbStatus.Items.AddRange(new object[]
            {
                "Wszystkie",
                "CREATED",
                "IN_TRANSIT",
                "DELIVERED",
                "FINISHED",
                "REJECTED"
            });
            cmbStatus.SelectedIndex = 0;
            cmbStatus.SelectedIndexChanged += async (s, e) => await LoadReturnsAsync();
            this.Controls.Add(cmbStatus);

            // Przycisk Odśwież
            btnRefresh = new Button
            {
                Text = "Odśwież",
                Location = new Point(500, 10),
                Width = 100
            };
            btnRefresh.Click += async (s, e) => await LoadReturnsAsync();
            this.Controls.Add(btnRefresh);

            // Przycisk Synchronizuj
            btnSync = new Button
            {
                Text = "Synchronizuj",
                Location = new Point(610, 10),
                Width = 120
            };
            btnSync.Click += btnSync_Click;
            this.Controls.Add(btnSync);

            // Label - Statystyki
            lblStats = new Label
            {
                Text = "Zwrotów: 0",
                Location = new Point(750, 15),
                AutoSize = true,
                Font = new Font(this.Font, FontStyle.Bold)
            };
            this.Controls.Add(lblStats);

            // DataGridView
            dgvReturns = new DataGridView
            {
                Location = new Point(10, 50),
                Size = new Size(1160, 600),
                AllowUserToAddRows = false,
                AllowUserToDeleteRows = false,
                ReadOnly = true,
                SelectionMode = DataGridViewSelectionMode.FullRowSelect,
                MultiSelect = false,
                AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill
            };
            dgvReturns.CellDoubleClick += dgvReturns_CellDoubleClick;
            this.Controls.Add(dgvReturns);
        }

        private async void FormAllegroReturns_Load(object sender, EventArgs e)
        {
            await LoadAccountsAsync();
        }

        private async Task LoadAccountsAsync()
        {
            try
            {
                using (var con = DatabaseHelper.GetConnection())
                {
                    await con.OpenAsync();
                    var cmd = new MySqlCommand(
                        "SELECT Id, SellerId FROM AllegroAccounts WHERE IsAuthorized = 1", 
                        con
                    );
                    
                    var dt = new DataTable();
                    using (var adapter = new MySqlDataAdapter(cmd))
                    {
                        await Task.Run(() => adapter.Fill(dt));
                    }

                    cmbAccounts.DisplayMember = "SellerId";
                    cmbAccounts.ValueMember = "Id";
                    cmbAccounts.DataSource = dt;

                    if (dt.Rows.Count > 0)
                    {
                        cmbAccounts.SelectedIndex = 0;
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    $"Błąd podczas wczytywania kont: {ex.Message}",
                    "Błąd",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error
                );
            }
        }

        private async Task LoadReturnsAsync()
        {
            if (cmbAccounts.SelectedValue == null)
            {
                dgvReturns.DataSource = null;
                lblStats.Text = "Zwrotów: 0";
                return;
            }

            int accountId = Convert.ToInt32(cmbAccounts.SelectedValue);
            string statusFilter = cmbStatus.SelectedItem?.ToString();

            try
            {
                btnRefresh.Enabled = false;
                Cursor = Cursors.WaitCursor;

                using (var con = DatabaseHelper.GetConnection())
                {
                    await con.OpenAsync();

                    string query = @"
                        SELECT
                            Id,
                            AllegroReturnId,
                            ReferenceNumber AS 'Nr Referencyjny',
                            OrderId AS 'Nr Zamówienia',
                            BuyerLogin AS 'Kupujący',
                            ProductName AS 'Produkt',
                            Quantity AS 'Ilość',
                            ReturnReasonType AS 'Powód',
                            StatusAllegro AS 'Status',
                            CreatedAt AS 'Data utworzenia',
                            Waybill AS 'Nr przesyłki',
                            CarrierName AS 'Przewoźnik',
                            CONCAT(
                                COALESCE(Delivery_FirstName, ''), ' ',
                                COALESCE(Delivery_LastName, '')
                            ) AS 'Klient',
                            Delivery_City AS 'Miasto',
                            CASE
                                WHEN StatusWewnetrznyId IS NOT NULL THEN 'Zarejestrowany'
                                ELSE 'Nowy'
                            END AS 'Status wewnętrzny'
                        FROM AllegroCustomerReturns
                        WHERE AllegroAccountId = @AccountId";

                    if (!string.IsNullOrEmpty(statusFilter) && statusFilter != "Wszystkie")
                    {
                        query += " AND StatusAllegro = @Status";
                    }

                    query += " ORDER BY CreatedAt DESC";

                    var dt = new DataTable();
                    using (var adapter = new MySqlDataAdapter(query, con))
                    {
                        adapter.SelectCommand.Parameters.AddWithValue("@AccountId", accountId);
                        if (!string.IsNullOrEmpty(statusFilter) && statusFilter != "Wszystkie")
                        {
                            adapter.SelectCommand.Parameters.AddWithValue("@Status", statusFilter);
                        }
                        await Task.Run(() => adapter.Fill(dt));
                    }

                    dgvReturns.DataSource = dt;
                    FormatReturnsGrid();

                    lblStats.Text = $"Zwrotów: {dt.Rows.Count}";
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    $"Błąd podczas wczytywania zwrotów: {ex.Message}",
                    "Błąd",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error
                );
            }
            finally
            {
                btnRefresh.Enabled = true;
                Cursor = Cursors.Default;
            }
        }

        private void FormatReturnsGrid()
        {
            // Ukryj niepotrzebne kolumny
            if (dgvReturns.Columns.Contains("Id"))
                dgvReturns.Columns["Id"].Visible = false;
            if (dgvReturns.Columns.Contains("AllegroReturnId"))
                dgvReturns.Columns["AllegroReturnId"].Visible = false;
            if (dgvReturns.Columns.Contains("OrderId"))
                dgvReturns.Columns["OrderId"].Visible = false;

            // Ustaw szerokości kolumn
            if (dgvReturns.Columns.Contains("Nr Referencyjny"))
                dgvReturns.Columns["Nr Referencyjny"].Width = 100;
            if (dgvReturns.Columns.Contains("Kupujący"))
                dgvReturns.Columns["Kupujący"].Width = 120;
            if (dgvReturns.Columns.Contains("Produkt"))
                dgvReturns.Columns["Produkt"].AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;
            if (dgvReturns.Columns.Contains("Status"))
                dgvReturns.Columns["Status"].Width = 100;
            if (dgvReturns.Columns.Contains("Data utworzenia"))
                dgvReturns.Columns["Data utworzenia"].Width = 150;

            // Pokoloruj wiersze według statusu
            foreach (DataGridViewRow row in dgvReturns.Rows)
            {
                var statusValue = row.Cells["Status"]?.Value?.ToString();
                var statusWewnetrzny = row.Cells["Status wewnętrzny"]?.Value?.ToString();

                if (statusWewnetrzny == "Nowy")
                {
                    row.DefaultCellStyle.BackColor = Color.LightYellow;
                    row.DefaultCellStyle.Font = new Font(dgvReturns.Font, FontStyle.Bold);
                }
                else if (statusValue == "DELIVERED")
                {
                    row.DefaultCellStyle.BackColor = Color.LightGreen;
                }
                else if (statusValue == "FINISHED")
                {
                    row.DefaultCellStyle.BackColor = Color.LightGray;
                }
                else if (statusValue == "REJECTED")
                {
                    row.DefaultCellStyle.BackColor = Color.LightCoral;
                }
            }
        }

        private async void btnSync_Click(object sender, EventArgs e)
        {
            try
            {
                btnSync.Enabled = false;
                btnSync.Text = "Synchronizacja...";
                Cursor = Cursors.WaitCursor;

                var syncService = new AllegroSyncServiceExtended();
                var progress = new Progress<string>(msg => 
                    System.Diagnostics.Debug.WriteLine(msg)
                );

                var result = await syncService.SynchronizeReturnsAsync(progress);

                MessageBox.Show(
                    $"Synchronizacja zakończona!\n\n" +
                    $"Przetworzonych: {result.TotalProcessed}\n" +
                    $"Nowych: {result.NewReturns}\n" +
                    $"Zaktualizowanych: {result.UpdatedReturns}",
                    "Synchronizacja",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Information
                );

                await LoadReturnsAsync();
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    $"Błąd synchronizacji: {ex.Message}",
                    "Błąd",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error
                );
            }
            finally
            {
                btnSync.Enabled = true;
                btnSync.Text = "Synchronizuj";
                Cursor = Cursors.Default;
            }
        }

        private void dgvReturns_CellDoubleClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex < 0) return;

            var returnId = Convert.ToInt32(dgvReturns.Rows[e.RowIndex].Cells["Id"].Value);

            MessageBox.Show(
                $"Otwieranie szczegółów zwrotu ID: {returnId}\n\n" +
                "TODO: Formularz szczegółów zwrotu",
                "Szczegóły zwrotu"
            );
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
}
