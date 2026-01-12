using Reklamacje_Dane.Allegro;
using System;
using System.Collections.Generic;
using System.Data;
using MySql.Data.MySqlClient;
using System.Drawing;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Reklamacje_Dane
{
    public partial class FormAllegroManager : Form
    {
        private class AllegroAccountItem
        {
            public int Id { get; set; }
            public string SellerId { get; set; }

            public override string ToString()
            {
                return SellerId;
            }
        }

        public FormAllegroManager()
        {
            InitializeComponent();
            this.Load += FormAllegroManager_Load;
        

            // Włącz sprawdzanie pisowni dla wszystkich TextBoxów
            EnableSpellCheckOnAllTextBoxes();
        }

        private async void FormAllegroManager_Load(object sender, EventArgs e)
        {
            await LoadAccountsAsync();
        }

        private async Task LoadAccountsAsync()
        {
            var accounts = new List<AllegroAccountItem>();
            try
            {
                using (var con = DatabaseHelper.GetConnection())
                {
                    await con.OpenAsync();
                    var cmd = new MySqlCommand("SELECT Id, SellerId FROM AllegroAccounts WHERE IsAuthorized = 1", con);
                    using (var reader = await cmd.ExecuteReaderAsync())
                    {
                        while (await reader.ReadAsync())
                        {
                            accounts.Add(new AllegroAccountItem
                            {
                                Id = Convert.ToInt32(reader["Id"]),
                                SellerId = reader["SellerId"].ToString()
                            });
                        }
                    }
                }

                cmbAccounts.DataSource = accounts;
                cmbAccounts.DisplayMember = "SellerId";
                cmbAccounts.ValueMember = "Id";

                if (accounts.Count > 0)
                {
                    cmbAccounts.SelectedIndex = 0;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Błąd podczas wczytywania kont Allegro: " + ex.Message, "Błąd Bazy Danych");
            }
        }

        private async Task LoadDisputesAsync()
        {
            if (cmbAccounts.SelectedItem == null)
            {
                dgvDisputes.DataSource = null;
                return;
            }

            var selectedAccount = (AllegroAccountItem)cmbAccounts.SelectedItem;
            var dt = new DataTable();

            try
            {
                using (var con = DatabaseHelper.GetConnection())
                {
                    // [KLUCZOWA POPRAWKA] Użycie CAST, aby wszystkie wyniki były traktowane jako tekst
                    string query = @"
                        SELECT
                            DisputeId,
                            BuyerLogin AS Kupujący,
                            Subject AS Temat,
                            OpenedAt AS Otwarto,
                            StatusAllegro AS 'Status Allegro',
                            HasNewMessages,
                            CASE
                                WHEN ComplaintId IS NULL OR ComplaintId = 0 THEN 'Niezarejestrowane'
                                ELSE CAST(ComplaintId AS CHAR)
                            END AS 'Status Rejestracji'
                        FROM AllegroDisputes
                        WHERE AllegroAccountId = @AccountId
                        ORDER BY OpenedAt DESC";

                    using (var adapter = new MySqlDataAdapter(query, con))
                    {
                        adapter.SelectCommand.Parameters.AddWithValue("@AccountId", selectedAccount.Id);
                        await Task.Run(() => adapter.Fill(dt));
                    }
                }
                dgvDisputes.DataSource = dt;
                FormatDisputesGrid();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Błąd podczas wczytywania dyskusji: " + ex.Message, "Błąd Bazy Danych");
            }
        }

        private void FormatDisputesGrid()
        {
            if (dgvDisputes.Columns.Contains("DisputeId"))
                dgvDisputes.Columns["DisputeId"].Visible = false;
            if (dgvDisputes.Columns.Contains("HasNewMessages"))
                dgvDisputes.Columns["HasNewMessages"].Visible = false;

            if (dgvDisputes.Columns.Contains("Temat"))
                dgvDisputes.Columns["Temat"].AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;
            if (dgvDisputes.Columns.Contains("Kupujący"))
                dgvDisputes.Columns["Kupujący"].Width = 120;
            if (dgvDisputes.Columns.Contains("Status Rejestracji"))
                dgvDisputes.Columns["Status Rejestracji"].Width = 150;

            foreach (DataGridViewRow row in dgvDisputes.Rows)
            {
                var hasNewValue = row.Cells["HasNewMessages"].Value;
                if (hasNewValue != null && hasNewValue != DBNull.Value)
                {
                    if (Convert.ToInt64(hasNewValue) == 1)
                    {
                        row.DefaultCellStyle.Font = new Font(dgvDisputes.Font, FontStyle.Bold);
                    }
                }
            }
        }

        private async void cmbAccounts_SelectedIndexChanged(object sender, EventArgs e)
        {
            await LoadDisputesAsync();
        }

        private async void btnRefresh_Click(object sender, EventArgs e)
        {
            await LoadDisputesAsync();
        }

        private async void dgvDisputes_CellDoubleClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex < 0) return;

            var disputeId = dgvDisputes.Rows[e.RowIndex].Cells["DisputeId"].Value?.ToString();
            if (string.IsNullOrEmpty(disputeId)) return;

            var selectedAccount = (AllegroAccountItem)cmbAccounts.SelectedItem;

            try
            {
                (string clientId, string clientSecret, AllegroToken token) = await GetApiCredentials(selectedAccount.Id);

                if (token == null)
                {
                    MessageBox.Show("Nie udało się pobrać danych autoryzacyjnych dla tego konta.", "Błąd");
                    return;
                }

                var apiClient = new AllegroApiClient(clientId, clientSecret) { Token = token };

                using (var issueForm = new FormAllegroIssue(disputeId))
                {
                    // Nie musimy już przekazywać apiClient, ponieważ FormAllegroIssue
                    // potrafi go sobie samodzielnie utworzyć na podstawie ID konta.
                    issueForm.ShowDialog(this);
                }
                

                await LoadDisputesAsync();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Błąd otwierania czatu: " + ex.Message, "Błąd Krytyczny");
            }
        }

        private async Task<(string, string, AllegroToken)> GetApiCredentials(int accountId)
        {
            using (var con = DatabaseHelper.GetConnection())
            {
                await con.OpenAsync();
                var cmd = new MySqlCommand("SELECT ClientId, ClientSecretEncrypted, AccessTokenEncrypted, RefreshTokenEncrypted, TokenExpirationDate FROM AllegroAccounts WHERE Id = @Id", con);
                cmd.Parameters.AddWithValue("@Id", accountId);

                using (var reader = await cmd.ExecuteReaderAsync())
                {
                    if (await reader.ReadAsync())
                    {
                        var token = new AllegroToken
                        {
                            AccessToken = EncryptionHelper.DecryptString(reader["AccessTokenEncrypted"].ToString()),
                            RefreshToken = EncryptionHelper.DecryptString(reader["RefreshTokenEncrypted"].ToString()),
                            ExpirationDate = DateTime.Parse(reader["TokenExpirationDate"].ToString())
                        };
                        string clientId = reader["ClientId"].ToString();
                        string clientSecret = EncryptionHelper.DecryptString(reader["ClientSecretEncrypted"].ToString());
                        return (clientId, clientSecret, token);
                    }
                }
            }
            return (null, null, null);
        }

        private void dgvDisputes_CellContentClick(object sender, DataGridViewCellEventArgs e)
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
}