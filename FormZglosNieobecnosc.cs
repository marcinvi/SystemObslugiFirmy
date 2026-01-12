using System;
using System.Collections.Generic;
using System.Data;
using MySql.Data.MySqlClient;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Reklamacje_Dane
{
    public partial class FormZglosNieobecnosc : Form
    {
        private readonly DatabaseService _dbServiceBaza;
        private readonly DatabaseService _dbServiceMagazyn;
        private readonly bool _isRemoteWorkRequest;

        private class UserItem
        {
            public int Id { get; set; }
            public string NazwaWyswietlana { get; set; }
            public override string ToString() => NazwaWyswietlana;
        }

        public FormZglosNieobecnosc(bool isRemoteWorkRequest)
        {
            InitializeComponent();
            _dbServiceBaza = new DatabaseService(DatabaseHelper.GetConnectionString());
            _dbServiceMagazyn = new DatabaseService(MagazynDatabaseHelper.GetConnectionString());
            _isRemoteWorkRequest = isRemoteWorkRequest;

            if (_isRemoteWorkRequest)
            {
                this.Text = "Wniosek o pracę zdalną";
                label3.Visible = false;
                comboZastepca.Visible = false;
            }
        

            // Włącz sprawdzanie pisowni dla wszystkich TextBoxów
            EnableSpellCheckOnAllTextBoxes();
        }

        private async void FormZglosNieobecnosc_Load(object sender, EventArgs e)
        {
            if (!_isRemoteWorkRequest)
            {
                await LoadHandlowcyAsync();
            }
        }

        private async Task LoadHandlowcyAsync()
        {
            var dt = await _dbServiceBaza.GetDataTableAsync(
                "SELECT Id, \"Nazwa Wyświetlana\" FROM Uzytkownicy WHERE Rola = 'Handlowiec' AND Id != @currentUserId ORDER BY \"Nazwa Wyświetlana\"",
                new MySqlParameter("@currentUserId", SessionManager.CurrentUserId));

            var users = new List<UserItem>();
            foreach (DataRow row in dt.Rows)
            {
                users.Add(new UserItem { Id = Convert.ToInt32(row["Id"]), NazwaWyswietlana = row["Nazwa Wyświetlana"].ToString() });
            }
            comboZastepca.DataSource = users;
            comboZastepca.DisplayMember = "NazwaWyswietlana";
            comboZastepca.ValueMember = "Id";
        }

        private async void btnOk_Click(object sender, EventArgs e)
        {
            var dataOd = dtpDataOd.Value.Date;
            var dataDo = dtpDataDo.Value.Date;

            if (dataDo < dataOd)
            {
                MessageBox.Show("Data końcowa nie może być wcześniejsza niż data początkowa.", "Błąd", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            if (!_isRemoteWorkRequest)
            {
                if (comboZastepca.SelectedItem == null)
                {
                    MessageBox.Show("Musisz wybrać osobę na zastępstwo.", "Błąd", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                var zastepca = (UserItem)comboZastepca.SelectedItem;

                try
                {
                    await _dbServiceMagazyn.ExecuteNonQueryAsync(
                        "INSERT INTO Delegacje (UzytkownikId, ZastepcaId, DataOd, DataDo) VALUES (@uzytkownik, @zastepca, @dataOd, @dataDo)",
                        new MySqlParameter("@uzytkownik", SessionManager.CurrentUserId),
                        new MySqlParameter("@zastepca", zastepca.Id),
                        new MySqlParameter("@dataOd", dataOd),
                        new MySqlParameter("@dataDo", dataDo));

                    MessageBox.Show("Nieobecność została zapisana.", "Sukces", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    this.Close();
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Błąd podczas zapisu nieobecności: " + ex.Message, "Błąd krytyczny", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
            else
            {
                // Logika dla pracy zdalnej (można dodać w przyszłości, np. zapis do innej tabeli)
                MessageBox.Show($"Wniosek o pracę zdalną w dniach od {dataOd:dd.MM.yyyy} do {dataDo:dd.MM.yyyy} został zarejestrowany.", "Informacja", MessageBoxButtons.OK, MessageBoxIcon.Information);
                this.Close();
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