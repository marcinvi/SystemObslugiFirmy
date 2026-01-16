// Plik: AdminControl.cs (Wersja z dodanymi ustawieniami DeepL i E-mail)
using System;
using System.Collections.Generic;
using System.Data;
using MySql.Data.MySqlClient;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;
using Reklamacje_Dane.Allegro;
namespace Reklamacje_Dane
{
    public partial class AdminControl : UserControl
    {
        private readonly DatabaseService _db;
        private bool _listsInitialized;
        private bool _isLoaded = false;
        private TextBox _txtDataFolder;
        private Button _btnSaveDataFolder;
        private Button _btnBrowseDataFolder;

        public AdminControl()
        {
            InitializeComponent();
            _db = new DatabaseService(DatabaseHelper.GetConnectionString());

            // Podpięcie handlerów, które nie są w designerze
            if (btnDodajKonto != null) btnDodajKonto.Click += btnDodajKonto_Click;
            if (btnUsunKonto != null) btnUsunKonto.Click += btnUsunKonto_Click;
            if (btnAutoryzujKonto != null) btnAutoryzujKonto.Click += btnAutoryzujKonto_Click;

            // Handlery z designera są już podpięte, ale dla pewności możemy je dodać
            if (btnSaveDpdSettings != null) btnSaveDpdSettings.Click += btnSaveDpdSettings_Click;

            // NOWE HANDLERY
            if (btnSaveDeepLSettings != null) btnSaveDeepLSettings.Click += btnSaveDeepLSettings_Click;
            if (btnSaveEmailSettings != null) btnSaveEmailSettings.Click += btnSaveEmailSettings_Click;
        }

        private async void AdminControl_Load(object sender, EventArgs e)
        {
            if (_isLoaded) return; // Zabezpieczenie przed wielokrotnym wywołaniem
            _isLoaded = true;

            try
            {
                EnsureListViews();
                await EnsureTablesAsync();
                await LoadModulesAsync();
                await LoadUsersAsync();
                await LoadAllegroAccountsAsync();
                await LoadDpdSettingsAsync();
                // NOWE METODY
                await LoadDeepLSettingsAsync();
                await LoadEmailSettingsAsync();
                EnsureDataFolderSettingsUi();
                await LoadDataFolderSettingsAsync();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Błąd inicjalizacji Admin: " + ex.Message, "Administracja",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        // Metoda pomocnicza do pobierania pojedynczego ustawienia
        private async Task<string> GetSettingAsync(string key)
        {
            var value = await _db.ExecuteScalarAsync(
                "SELECT WartoscZaszyfrowana FROM Ustawienia WHERE Klucz = @k",
                new MySqlParameter("@k", key)
            );
            return value?.ToString() ?? "";
        }

        #region Ustawienia Aplikacji (DPD, DeepL, E-mail)

        private async Task LoadDpdSettingsAsync()
        {
            txtDpdLogin.Text = await GetSettingAsync("LoginDPD");
            txtDpdClientId.Text = await GetSettingAsync("KlientDPD");
            txtDpdPassword.Text = await GetSettingAsync("haslodpd");
        }

        private async void btnSaveDpdSettings_Click(object sender, EventArgs e)
        {
            try
            {
                await UpsertPlainSettingAsync("LoginDPD", txtDpdLogin.Text);
                await UpsertPlainSettingAsync("KlientDPD", txtDpdClientId.Text);
                await UpsertPlainSettingAsync("haslodpd", txtDpdPassword.Text);
                MessageBox.Show("Ustawienia DPD zapisane.", "Ustawienia", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show("Błąd zapisu ustawień DPD: " + ex.Message, "Ustawienia", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private async Task LoadDeepLSettingsAsync()
        {
            string encryptedKey = await GetSettingAsync("DeepL");
            txtDeepLApiKey.Text = !string.IsNullOrEmpty(encryptedKey) ? EncryptionHelper.DecryptString(encryptedKey) : "";
        }

        private async void btnSaveDeepLSettings_Click(object sender, EventArgs e)
        {
            try
            {
                string encryptedKey = EncryptionHelper.EncryptString(txtDeepLApiKey.Text);
                await UpsertPlainSettingAsync("DeepL", encryptedKey);
                MessageBox.Show("Klucz API DeepL zapisany.", "Ustawienia", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show("Błąd zapisu klucza API DeepL: " + ex.Message, "Ustawienia", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private async Task LoadEmailSettingsAsync()
        {
            txtDefaultEmail.Text = await GetSettingAsync("domyslnymail");
        }

        private async void btnSaveEmailSettings_Click(object sender, EventArgs e)
        {
            try
            {
                await UpsertPlainSettingAsync("domyslnymail", txtDefaultEmail.Text);
                MessageBox.Show("Domyślny e-mail magazynu zapisany.", "Ustawienia", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show("Błąd zapisu e-maila: " + ex.Message, "Ustawienia", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void EnsureDataFolderSettingsUi()
        {
            if (_txtDataFolder != null || tabPageSettings == null)
            {
                return;
            }

            var groupBoxFiles = new GroupBox
            {
                Text = "Ustawienia plików",
                Width = 520,
                Height = 140,
                Left = 10,
                Top = groupBoxEmail.Bottom + 15
            };

            var lblPath = new Label
            {
                Text = "Ścieżka do folderu danych:",
                AutoSize = true,
                Left = 15,
                Top = 30
            };

            _txtDataFolder = new TextBox
            {
                Left = 15,
                Top = 55,
                Width = 370
            };

            _btnBrowseDataFolder = new Button
            {
                Text = "Przeglądaj",
                Left = _txtDataFolder.Right + 10,
                Top = 52,
                Width = 90
            };
            _btnBrowseDataFolder.Click += (s, e) =>
            {
                using (var dialog = new FolderBrowserDialog())
                {
                    dialog.SelectedPath = _txtDataFolder.Text;
                    if (dialog.ShowDialog(this) == DialogResult.OK)
                    {
                        _txtDataFolder.Text = dialog.SelectedPath;
                    }
                }
            };

            _btnSaveDataFolder = new Button
            {
                Text = "Zapisz ścieżkę",
                Left = 15,
                Top = 90,
                Width = 140
            };
            _btnSaveDataFolder.Click += btnSaveDataFolder_Click;

            groupBoxFiles.Controls.Add(lblPath);
            groupBoxFiles.Controls.Add(_txtDataFolder);
            groupBoxFiles.Controls.Add(_btnBrowseDataFolder);
            groupBoxFiles.Controls.Add(_btnSaveDataFolder);

            tabPageSettings.Controls.Add(groupBoxFiles);
        }

        private async Task LoadDataFolderSettingsAsync()
        {
            if (_txtDataFolder == null)
            {
                return;
            }

            var value = await GetSettingAsync("DataFolderPath");
            if (string.IsNullOrWhiteSpace(value))
            {
                value = AppPaths.GetDataRootPath();
            }
            _txtDataFolder.Text = value;
        }

        private async void btnSaveDataFolder_Click(object sender, EventArgs e)
        {
            try
            {
                await UpsertPlainSettingAsync("DataFolderPath", _txtDataFolder.Text);
                AppPaths.SetDataRootPath(_txtDataFolder.Text);
                MessageBox.Show("Ścieżka danych zapisana.", "Ustawienia", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show("Błąd zapisu ścieżki: " + ex.Message, "Ustawienia", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private async Task UpsertPlainSettingAsync(string key, string value)
        {
            var existing = await _db.ExecuteScalarAsync("SELECT COUNT(*) FROM Ustawienia WHERE Klucz=@k", new MySqlParameter("@k", key));
            if (Convert.ToInt32(existing) == 0)
            {
                await _db.ExecuteNonQueryAsync("INSERT INTO Ustawienia (Klucz, WartoscZaszyfrowana) VALUES (@k,@v)", new MySqlParameter("@k", key), new MySqlParameter("@v", value ?? ""));
            }
            else
            {
                await _db.ExecuteNonQueryAsync("UPDATE Ustawienia SET WartoscZaszyfrowana=@v WHERE Klucz=@k", new MySqlParameter("@k", key), new MySqlParameter("@v", value ?? ""));
            }
        }
        #endregion

        #region Pozostałe metody (bez zmian)

        private void EnsureListViews()
        {
            if (_listsInitialized) return;
            _listsInitialized = true;
            listViewUsers.Columns.Clear();
            listViewUsers.View = View.Details;
            listViewUsers.FullRowSelect = true;
            listViewUsers.Columns.Add("Id", 60);
            listViewUsers.Columns.Add("Login", 160);
            listViewUsers.Columns.Add("Nazwa wyświetlana", 220);
            listViewUsers.Columns.Add("Rola", 120);
            listViewKonta.Columns.Clear();
            listViewKonta.View = View.Details;
            listViewKonta.FullRowSelect = true;
            listViewKonta.Columns.Add("Id", 60);
            listViewKonta.Columns.Add("Konto", 240);
            listViewKonta.Columns.Add("Status", 160);
            listViewKonta.Columns.Add("Wygasa", 160);
        }
        private async Task EnsureTablesAsync()
        {
            await _db.ExecuteNonQueryAsync("CREATE TABLE IF NOT EXISTS Ustawienia (Id INT AUTO_INCREMENT PRIMARY KEY, Klucz VARCHAR(255) UNIQUE, WartoscZaszyfrowana TEXT);");
            await _db.ExecuteNonQueryAsync("CREATE TABLE IF NOT EXISTS Moduly (Id INT AUTO_INCREMENT PRIMARY KEY, NazwaModulu VARCHAR(255) UNIQUE);");
            await _db.ExecuteNonQueryAsync("CREATE TABLE IF NOT EXISTS Uprawnienia (UzytkownikId INT, ModulId INT, PRIMARY KEY (UzytkownikId, ModulId));");
            await _db.ExecuteNonQueryAsync("CREATE TABLE IF NOT EXISTS AllegroAccounts (Id INT AUTO_INCREMENT PRIMARY KEY, AccountName VARCHAR(255), ClientId VARCHAR(255), ClientSecretEncrypted TEXT, AccessTokenEncrypted TEXT, RefreshTokenEncrypted TEXT, TokenExpirationDate DATETIME, IsAuthorized TINYINT DEFAULT 0);");
            await _db.ExecuteNonQueryAsync("CREATE TABLE IF NOT EXISTS Uzytkownicy (Id INT AUTO_INCREMENT PRIMARY KEY, Login VARCHAR(255) UNIQUE, `Nazwa Wyświetlana` VARCHAR(255), Hasło VARCHAR(255), Rola VARCHAR(100));");
        }
        private List<(int Id, string Nazwa)> _modules = new List<(int, string)>();
        private async Task LoadModulesAsync()
        {
            _modules.Clear();
            var dt = await _db.GetDataTableAsync("SELECT DISTINCT Id, NazwaModulu FROM Moduly ORDER BY Id");
            foreach (DataRow r in dt.Rows)
                _modules.Add((Convert.ToInt32(r["Id"]), Convert.ToString(r["NazwaModulu"])));
            checkedListBoxModules.Items.Clear();
            foreach (var m in _modules)
                checkedListBoxModules.Items.Add($"{m.Id}: {m.Nazwa}");
        }
        private async Task<List<int>> GetUserModuleIdsAsync(int userId)
        {
            var ids = new List<int>();
            var dt = await _db.GetDataTableAsync("SELECT ModulId FROM Uprawnienia WHERE UzytkownikId = @u", new MySqlParameter("@u", userId));
            foreach (DataRow r in dt.Rows) ids.Add(Convert.ToInt32(r["ModulId"]));
            return ids;
        }
        private async Task SetUserModulesAsync(int userId, IEnumerable<int> newModuleIds)
        {
            await _db.ExecuteNonQueryAsync("DELETE FROM Uprawnienia WHERE UzytkownikId = @u", new MySqlParameter("@u", userId));
            foreach (var mid in newModuleIds.Distinct())
            {
                await _db.ExecuteNonQueryAsync("INSERT INTO Uprawnienia (UzytkownikId, ModulId) VALUES (@u,@m)", new MySqlParameter("@u", userId), new MySqlParameter("@m", mid));
            }
        }
        private async Task LoadUsersAsync()
        {
            listViewUsers.Items.Clear();
            var dt = await _db.GetDataTableAsync("SELECT Id, Login, `Nazwa Wyświetlana` as Name, IFNULL(Rola,'') as Rola FROM Uzytkownicy ORDER BY Login");
            foreach (DataRow r in dt.Rows)
            {
                var it = new ListViewItem(Convert.ToString(r["Id"]))
                {
                    Tag = Convert.ToInt32(r["Id"])
                };
                it.SubItems.Add(Convert.ToString(r["Login"]));
                it.SubItems.Add(Convert.ToString(r["Name"]));
                it.SubItems.Add(Convert.ToString(r["Rola"]));
                listViewUsers.Items.Add(it);
            }
            panelUserDetails.Enabled = false;
            txtLogin.Clear();
            txtNazwaWyswietlana.Clear();
            comboRola.Items.Clear();
            comboRola.Items.AddRange(new[] { "Admin", "Reklamacje", "Magazyn", "Handlowiec", "Weryfikacja" });
            comboRola.SelectedIndex = -1;
            checkedListBoxModules.Items.Clear();
            foreach (var m in _modules) checkedListBoxModules.Items.Add($"{m.Id}: {m.Nazwa}");
        }
        private async void listViewUsers_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (listViewUsers.SelectedItems.Count == 0)
            {
                panelUserDetails.Enabled = false;
                return;
            }
            panelUserDetails.Enabled = true;
            int userId = (int)listViewUsers.SelectedItems[0].Tag;
            var dt = await _db.GetDataTableAsync("SELECT Id, Login, `Nazwa Wyświetlana` as Name, IFNULL(Rola,'') as Rola FROM Uzytkownicy WHERE Id = @id", new MySqlParameter("@id", userId));
            if (dt.Rows.Count == 0)
            {
                panelUserDetails.Enabled = false;
                return;
            }
            var row = dt.Rows[0];
            txtLogin.Text = Convert.ToString(row["Login"]);
            txtNazwaWyswietlana.Text = Convert.ToString(row["Name"]);
            var role = Convert.ToString(row["Rola"]);
            comboRola.SelectedIndex = string.IsNullOrEmpty(role) ? -1 : comboRola.FindStringExact(role);
            var userMods = await GetUserModuleIdsAsync(userId);
            for (int i = 0; i < checkedListBoxModules.Items.Count; i++)
            {
                var text = checkedListBoxModules.Items[i].ToString();
                int colon = text.IndexOf(':');
                int id = colon > 0 ? int.Parse(text.Substring(0, colon)) : -1;
                checkedListBoxModules.SetItemChecked(i, userMods.Contains(id));
            }
        }
        private async void btnSaveChanges_Click(object sender, EventArgs e)
        {
            if (listViewUsers.SelectedItems.Count == 0) return;
            int userId = (int)listViewUsers.SelectedItems[0].Tag;
            string login = txtLogin.Text?.Trim() ?? "";
            string name = txtNazwaWyswietlana.Text?.Trim() ?? "";
            string role = comboRola.SelectedItem?.ToString() ?? "";
            if (string.IsNullOrWhiteSpace(login))
            {
                MessageBox.Show("Login jest wymagany.", "Użytkownicy", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                txtLogin.Focus();
                return;
            }
            try
            {
                await _db.ExecuteNonQueryAsync("UPDATE Uzytkownicy SET Login=@l, `Nazwa Wyświetlana`=@n, Rola=@r WHERE Id=@id", new MySqlParameter("@l", login), new MySqlParameter("@n", name), new MySqlParameter("@r", string.IsNullOrEmpty(role) ? (object)DBNull.Value : role), new MySqlParameter("@id", userId));
                var selectedModuleIds = new List<int>();
                foreach (var item in checkedListBoxModules.CheckedItems)
                {
                    var txt = item.ToString();
                    int colon = txt.IndexOf(':');
                    if (colon > 0 && int.TryParse(txt.Substring(0, colon), out int mid))
                        selectedModuleIds.Add(mid);
                }
                await SetUserModulesAsync(userId, selectedModuleIds);
                MessageBox.Show("Zmiany użytkownika zapisane.", "Użytkownicy", MessageBoxButtons.OK, MessageBoxIcon.Information);
                await LoadUsersAsync();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Błąd zapisu użytkownika: " + ex.Message, "Użytkownicy", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
        private async void btnResetPassword_Click(object sender, EventArgs e)
        {
            if (listViewUsers.SelectedItems.Count == 0) return;
            int userId = (int)listViewUsers.SelectedItems[0].Tag;
            string login = listViewUsers.SelectedItems[0].SubItems[1].Text;
            string newPass = PromptPassword($"Nowe hasło dla użytkownika: {login}");
            if (string.IsNullOrEmpty(newPass)) return;
            try
            {
                string hash = EncryptionHelper.HashPassword(newPass);
                await _db.ExecuteNonQueryAsync("UPDATE Uzytkownicy SET Hasło=@p WHERE Id=@id", new MySqlParameter("@p", hash), new MySqlParameter("@id", userId));
                MessageBox.Show("Hasło zresetowane.", "Użytkownicy", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show("Błąd resetu hasła: " + ex.Message, "Użytkownicy", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
        private string PromptPassword(string title)
        {
            using (var f = new Form())
            using (var t = new TextBox())
            using (var ok = new Button())
            using (var no = new Button())
            {
                f.Text = title;
                f.StartPosition = FormStartPosition.CenterParent;
                f.FormBorderStyle = FormBorderStyle.FixedDialog;
                f.MaximizeBox = false; f.MinimizeBox = false;
                f.ClientSize = new Size(340, 120);
                var lbl = new Label { Text = "Hasło:", Left = 12, Top = 16, Width = 60 };
                t.Left = 80; t.Top = 12; t.Width = 240; t.PasswordChar = '•';
                ok.Text = "OK"; ok.Left = 160; ok.Top = 60; ok.Width = 70; ok.DialogResult = DialogResult.OK;
                no.Text = "Anuluj"; no.Left = 250; no.Top = 60; no.Width = 70; no.DialogResult = DialogResult.Cancel;
                f.Controls.AddRange(new Control[] { lbl, t, ok, no });
                f.AcceptButton = ok; f.CancelButton = no;
                return f.ShowDialog() == DialogResult.OK ? t.Text : null;
            }
        }

        private async Task LoadAllegroAccountsAsync()
        {
            listViewKonta.Items.Clear();
            var dt = await _db.GetDataTableAsync("SELECT Id, AccountName, IFNULL(IsAuthorized,0) as IsAuth, IFNULL(TokenExpirationDate,'') as Exp FROM AllegroAccounts ORDER BY AccountName");
            foreach (DataRow r in dt.Rows)
            {
                int id = Convert.ToInt32(r["Id"]);
                string name = Convert.ToString(r["AccountName"]);
                bool isAuth = Convert.ToInt32(r["IsAuth"]) != 0;
                DateTime exp = DateTime.MinValue;
                DateTime.TryParse(Convert.ToString(r["Exp"]), out exp);
                string status = (isAuth && exp > DateTime.Now) ? "Autoryzowane" : "Wymaga autoryzacji";
                string expTxt = exp == DateTime.MinValue ? "" : exp.ToString("yyyy-MM-dd HH:mm");
                var it = new ListViewItem(id.ToString()) { Tag = id };
                it.SubItems.Add(name);
                it.SubItems.Add(status);
                it.SubItems.Add(expTxt);
                listViewKonta.Items.Add(it);
            }
        }
        private async void btnDodajKonto_Click(object sender, EventArgs e)
        {
            using (var f = new Form())
            {
                f.Text = "Nowe konto Allegro";
                f.StartPosition = FormStartPosition.CenterParent;
                f.FormBorderStyle = FormBorderStyle.FixedDialog;
                f.ClientSize = new Size(420, 200);
                f.MaximizeBox = false; f.MinimizeBox = false;
                var lbl1 = new Label { Left = 12, Top = 16, Width = 120, Text = "Nazwa konta:" };
                var lbl2 = new Label { Left = 12, Top = 48, Width = 120, Text = "Client ID:" };
                var lbl3 = new Label { Left = 12, Top = 80, Width = 120, Text = "Client Secret:" };
                var t1 = new TextBox { Left = 140, Top = 12, Width = 250 };
                var t2 = new TextBox { Left = 140, Top = 44, Width = 250 };
                var t3 = new TextBox { Left = 140, Top = 76, Width = 250, PasswordChar = '•' };
                var ok = new Button { Left = 230, Top = 120, Width = 75, Text = "Zapisz", DialogResult = DialogResult.OK };
                var no = new Button { Left = 315, Top = 120, Width = 75, Text = "Anuluj", DialogResult = DialogResult.Cancel };
                f.Controls.AddRange(new Control[] { lbl1, lbl2, lbl3, t1, t2, t3, ok, no });
                f.AcceptButton = ok; f.CancelButton = no;
                if (f.ShowDialog(this) != DialogResult.OK) return;
                string accountName = t1.Text?.Trim() ?? "";
                string clientId = t2.Text?.Trim() ?? "";
                string clientSecret = t3.Text ?? "";
                if (string.IsNullOrWhiteSpace(accountName) || string.IsNullOrWhiteSpace(clientId) || string.IsNullOrWhiteSpace(clientSecret))
                {
                    MessageBox.Show("Wypełnij wszystkie pola.", "Allegro", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }
                try
                {
                    await _db.ExecuteNonQueryAsync("INSERT INTO AllegroAccounts (AccountName, ClientId, ClientSecretEncrypted, IsAuthorized) VALUES (@n,@cid,@sec,0)", new MySqlParameter("@n", accountName), new MySqlParameter("@cid", clientId), new MySqlParameter("@sec", EncryptionHelper.EncryptString(clientSecret)));
                    await LoadAllegroAccountsAsync();
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Błąd dodawania konta: " + ex.Message, "Allegro", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }
        private async void btnUsunKonto_Click(object sender, EventArgs e)
        {
            if (listViewKonta.SelectedItems.Count == 0) return;
            int id = (int)listViewKonta.SelectedItems[0].Tag;
            if (MessageBox.Show("Usunąć wybrane konto Allegro?", "Allegro", MessageBoxButtons.YesNo, MessageBoxIcon.Question) != DialogResult.Yes)
                return;
            try
            {
                await _db.ExecuteNonQueryAsync("DELETE FROM AllegroAccounts WHERE Id=@id", new MySqlParameter("@id", id));
                await LoadAllegroAccountsAsync();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Błąd usuwania konta: " + ex.Message, "Allegro", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
        private async void btnAutoryzujKonto_Click(object sender, EventArgs e)
        {
            if (listViewKonta.SelectedItems.Count == 0) return;
            int id = (int)listViewKonta.SelectedItems[0].Tag;
            try
            {
                var dt = await _db.GetDataTableAsync("SELECT ClientId, ClientSecretEncrypted FROM AllegroAccounts WHERE Id=@id", new MySqlParameter("@id", id));
                if (dt.Rows.Count == 0) throw new Exception("Nie znaleziono konta.");
                string clientId = Convert.ToString(dt.Rows[0]["ClientId"]);
                string clientSecret = EncryptionHelper.DecryptString(Convert.ToString(dt.Rows[0]["ClientSecretEncrypted"]));
                var api = new AllegroApiClient(clientId, clientSecret);
                await api.AuthorizeAsync();
                await DatabaseHelper.SaveRefreshedTokenAsync(
      id,
      api.Token.AccessToken,
      api.Token.RefreshToken,
      api.Token.ExpirationDate
  );
                await _db.ExecuteNonQueryAsync("UPDATE AllegroAccounts SET IsAuthorized=1, TokenExpirationDate=@exp WHERE Id=@id", new MySqlParameter("@exp", api.Token.ExpirationDate), new MySqlParameter("@id", id));
                await LoadAllegroAccountsAsync();
                MessageBox.Show("Konto autoryzowane.", "Allegro", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show("Błąd autoryzacji konta: " + ex.Message, "Allegro", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
        #endregion
    }
}
