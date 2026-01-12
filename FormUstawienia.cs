using System;
using MySql.Data.MySqlClient;
using System.Drawing;
using System.Threading.Tasks;
using System.Windows.Forms;
using Reklamacje_Dane.Allegro;
using Reklamacje_Dane.Allegro.Issues;
namespace Reklamacje_Dane
{
    public partial class FormUstawienia : Form
    {
        public FormUstawienia()
        {
            InitializeComponent();
        

            // Włącz sprawdzanie pisowni dla wszystkich TextBoxów
            EnableSpellCheckOnAllTextBoxes();
        }

        private async void FormUstawienia_Load(object sender, EventArgs e)
        {
            SetupListView();
            await RefreshAccountsStatusAsync();
        }

        private void SetupListView()
        {
            listViewKonta.Columns.Clear();
            listViewKonta.View = View.Details;
            listViewKonta.FullRowSelect = true;

            listViewKonta.Columns.Add("ID", 50, HorizontalAlignment.Left);
            listViewKonta.Columns.Add("Nazwa konta (Seller ID)", 250, HorizontalAlignment.Left);
            listViewKonta.Columns.Add("Status autoryzacji", 200, HorizontalAlignment.Center);
        }

        private async Task RefreshAccountsStatusAsync()
        {
            listViewKonta.Items.Clear();

            using (var con = DatabaseHelper.GetConnection())
            {
                await con.OpenAsync();
                var cmd = new MySqlCommand("SELECT Id, AccountName, IsAuthorized, TokenExpirationDate FROM AllegroAccounts", con);

                using (var reader = await cmd.ExecuteReaderAsync())
                {
                    while (await reader.ReadAsync())
                    {
                        int accountId = Convert.ToInt32(reader["Id"]);
                        string sellerId = reader["AccountName"].ToString();
                        bool isAuthorized = reader["IsAuthorized"] != DBNull.Value && Convert.ToBoolean(reader["IsAuthorized"]);
                        DateTime expirationDate = reader["TokenExpirationDate"] != DBNull.Value ? Convert.ToDateTime(reader["TokenExpirationDate"]) : DateTime.MinValue;

                        string statusText;
                        Color statusColor;

                        if (isAuthorized && expirationDate > DateTime.Now)
                        {
                            statusText = "Autoryzowane";
                            statusColor = Color.ForestGreen;
                        }
                        else if (isAuthorized && expirationDate <= DateTime.Now)
                        {
                            statusText = "Wygasło (wymaga odświeżenia)";
                            statusColor = Color.OrangeRed;
                        }
                        else
                        {
                            statusText = "Wymaga autoryzacji";
                            statusColor = Color.Crimson;
                        }

                        ListViewItem item = new ListViewItem(accountId.ToString());
                        item.SubItems.Add(sellerId);
                        item.SubItems.Add(statusText);

                        item.Tag = accountId;

                        item.UseItemStyleForSubItems = false;
                        item.SubItems[2].BackColor = statusColor;
                        item.SubItems[2].ForeColor = Color.White;
                        item.SubItems[2].Font = new Font(listViewKonta.Font, FontStyle.Bold);

                        listViewKonta.Items.Add(item);
                    }
                }
            }
        }

        private async void btnDodaj_Click(object sender, EventArgs e)
        {
            using (var form = new FormDodajEdytujKontoAllegro())
            {
                if (form.ShowDialog() == DialogResult.OK)
                {
                    await RefreshAccountsStatusAsync();
                }
            }
        }

        private async void btnUsun_Click(object sender, EventArgs e)
        {
            if (listViewKonta.SelectedItems.Count == 0)
            {
                MessageBox.Show("Proszę zaznaczyć konto do usunięcia.", "Informacja");
                return;
            }

            if (MessageBox.Show("Czy na pewno chcesz usunąć zaznaczone konto?", "Potwierdzenie", MessageBoxButtons.YesNo, MessageBoxIcon.Warning) == DialogResult.Yes)
            {
                int accountId = (int)listViewKonta.SelectedItems[0].Tag;

                using (var con = DatabaseHelper.GetConnection())
                {
                    await con.OpenAsync();
                    var cmd = new MySqlCommand("DELETE FROM AllegroAccounts WHERE Id = @id", con);
                    cmd.Parameters.AddWithValue("@id", accountId);
                    await cmd.ExecuteNonQueryAsync();
                }
                await RefreshAccountsStatusAsync();
            }
        }

        private async void btnAutoryzuj_Click(object sender, EventArgs e)
        {
            if (listViewKonta.SelectedItems.Count == 0)
            {
                MessageBox.Show("Proszę zaznaczyć konto do autoryzacji.", "Informacja");
                return;
            }

            int selectedAccountId = (int)listViewKonta.SelectedItems[0].Tag;

            var accountData = await GetAccountDetailsFromDb(selectedAccountId);

            if (!accountData.HasValue)
            {
                MessageBox.Show("Nie znaleziono danych konfiguracyjnych dla tego konta.", "Błąd");
                return;
            }

            try
            {
                this.Cursor = Cursors.WaitCursor;
                var apiClient = new AllegroApiClient(accountData.Value.ClientId, accountData.Value.ClientSecret);

                await apiClient.AuthorizeAsync();

                if (apiClient.Token != null)
                {
                    await SaveTokenToDb(selectedAccountId, apiClient.Token);
                    MessageBox.Show("Konto zostało pomyślnie autoryzowane!", "Sukces");
                }
                else
                {
                    MessageBox.Show("Proces autoryzacji został przerwany lub nie powiódł się.", "Błąd");
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Wystąpił błąd podczas autoryzacji: {ex.Message}", "Błąd krytyczny");
            }
            finally
            {
                this.Cursor = Cursors.Default;
                await RefreshAccountsStatusAsync();
            }
        }

        // --- NOWA METODA DLA PRZYCISKU TESTOWEGO ---
        private async void btnTestRefresh_Click(object sender, EventArgs e)
        {
            if (listViewKonta.SelectedItems.Count == 0)
            {
                MessageBox.Show("Proszę zaznaczyć konto do przetestowania.", "Informacja");
                return;
            }

            int selectedAccountId = (int)listViewKonta.SelectedItems[0].Tag;
            string accountName = listViewKonta.SelectedItems[0].SubItems[1].Text;

            MessageBox.Show($"Rozpoczynanie testu odświeżania tokena dla konta: {accountName}", "Test Odświeżania", MessageBoxButtons.OK, MessageBoxIcon.Information);

            try
            {
                this.Cursor = Cursors.WaitCursor;
                // Wywołujemy tę samą logikę, co w tle, ale w izolacji
                using (var con = Database.GetNewOpenConnection())
                {
                    // POPRAWKA: Przekazujemy otwarte połączenie do metody.
                    var apiClient = await DatabaseHelper.GetApiClientForAccountAsync(selectedAccountId, con);
                    if (apiClient != null)
                    {
                        // Jeśli GetApiClientForAccountAsync się powiodło, to znaczy, że odświeżanie
                        // (jeśli było potrzebne) zakończyło się sukcesem.
                        MessageBox.Show($"Test zakończony pomyślnie dla konta '{accountName}'.\nToken jest ważny i/lub został odświeżony.", "Sukces", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    }
                    else
                    {
                        // GetApiClientForAccountAsync zwrócił null, co po ostatniej poprawce oznacza,
                        // że token odświeżający jest nieważny.
                        MessageBox.Show($"Test zakończony niepowodzeniem dla konta '{accountName}'.\nToken odświeżający prawdopodobnie wygasł i konto wymaga ponownej, pełnej autoryzacji.", "Błąd Odświeżania", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Wystąpił nieoczekiwany błąd podczas testu: {ex.Message}", "Błąd krytyczny");
            }
            finally
            {
                this.Cursor = Cursors.Default;
                // Zawsze odśwież listę, aby zobaczyć aktualny status
                await RefreshAccountsStatusAsync();
            }
        }

        private async Task<(string ClientId, string ClientSecret)?> GetAccountDetailsFromDb(int accountId)
        {
            using (var con = DatabaseHelper.GetConnection())
            {
                await con.OpenAsync();
                var cmd = new MySqlCommand("SELECT ClientId, ClientSecretEncrypted FROM AllegroAccounts WHERE Id = @id", con);
                cmd.Parameters.AddWithValue("@id", accountId);

                using (var reader = await cmd.ExecuteReaderAsync())
                {
                    if (await reader.ReadAsync())
                    {
                        return (
                            reader["ClientId"].ToString(),
                            EncryptionHelper.DecryptString(reader["ClientSecretEncrypted"].ToString())
                        );
                    }
                }
            }
            return null;
        }

        private async Task SaveTokenToDb(int accountId, Allegro.AllegroToken token)
        {
            string encryptedAccessToken = EncryptionHelper.EncryptString(token.AccessToken);
            string encryptedRefreshToken = EncryptionHelper.EncryptString(token.RefreshToken);

            using (var con = DatabaseHelper.GetConnection())
            {
                await con.OpenAsync();
                var cmd = new MySqlCommand(
                    "UPDATE AllegroAccounts SET " +
                    "AccessTokenEncrypted = @access, " +
                    "RefreshTokenEncrypted = @refresh, " +
                    "TokenExpirationDate = @expires, " +
                    "IsAuthorized = 1 " +
                    "WHERE Id = @id", con);

                cmd.Parameters.AddWithValue("@access", encryptedAccessToken);
                cmd.Parameters.AddWithValue("@refresh", encryptedRefreshToken);
                cmd.Parameters.AddWithValue("@expires", token.ExpirationDate);
                cmd.Parameters.AddWithValue("@id", accountId);

                await cmd.ExecuteNonQueryAsync();
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            // Tworzymy instancję formularza w bloku using (dobre dla pamięci)
            using (var form = new FormZarzadzajKontami())
            {
                // Otwieramy jako okno modalne (blokuje spód do czasu zamknięcia)
                var result = form.ShowDialog();

               
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
}
}