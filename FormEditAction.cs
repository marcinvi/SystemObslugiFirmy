// ############################################################################
// Plik: FormEditAction.cs (WERSJA Z POBIERANIEM DANYCH Z BAZY)
// Opis: Pobiera aktualną treść działania z bazy przy otwarciu.
// ############################################################################

using System;
using MySql.Data.MySqlClient;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Reklamacje_Dane
{
    public partial class FormEditAction : Form
    {
        private readonly int _actionId;
        private string _originalTresc;
        private readonly string _nrZgloszenia;

        // Konstruktor
        public FormEditAction(int actionId, string ignoredText, string nrZgloszenia)
        {
            InitializeComponent();
            this.StartPosition = FormStartPosition.CenterParent;

            _actionId = actionId;
            _nrZgloszenia = nrZgloszenia;

            // Uruchamiamy ładowanie danych przy starcie
            this.Load += FormEditAction_Load;
        

            // Włącz sprawdzanie pisowni dla wszystkich TextBoxów
            EnableSpellCheckOnAllTextBoxes();
        }

        private async void FormEditAction_Load(object sender, EventArgs e)
        {
            try
            {
                // Pobieramy świeżą treść z bazy danych
                using (var con = DatabaseHelper.GetConnectionAsync())
                {
                    await con.OpenAsync();
                    string query = "SELECT Tresc FROM Dzialania WHERE Id = @id";
                    using (var cmd = new MySqlCommand(query, con))
                    {
                        cmd.Parameters.AddWithValue("@id", _actionId);
                        var result = await cmd.ExecuteScalarAsync();

                        if (result != null)
                        {
                            _originalTresc = result.ToString();
                            txtTresc.Text = _originalTresc;

                            // Ustaw kursor na końcu tekstu
                            txtTresc.SelectionStart = txtTresc.Text.Length;
                            txtTresc.SelectionLength = 0;
                        }
                        else
                        {
                            MessageBox.Show("Nie znaleziono działania w bazie danych.", "Błąd", MessageBoxButtons.OK, MessageBoxIcon.Error);
                            this.Close();
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Błąd pobierania treści: {ex.Message}", "Błąd", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private async void btnZapisz_Click(object sender, EventArgs e)
        {
            string nowaTresc = txtTresc.Text.Trim();

            // Walidacja: Czy treść się zmieniła?
            if (nowaTresc == _originalTresc)
            {
                this.DialogResult = DialogResult.Cancel;
                this.Close();
                return;
            }

            try
            {
                using (var con = DatabaseHelper.GetConnectionAsync())
                {
                    await con.OpenAsync();
                    string query = "UPDATE Dzialania SET Tresc = @tresc WHERE Id = @id";
                    using (var cmd = new MySqlCommand(query, con))
                    {
                        cmd.Parameters.AddWithValue("@tresc", nowaTresc);
                        cmd.Parameters.AddWithValue("@id", _actionId);
                        await cmd.ExecuteNonQueryAsync();
                    }
                }

                // Logowanie zmiany w dzienniku
                var dziennik = new DziennikLogger();
                string logMessage = $"Zmieniono treść działania (ID: {_actionId})";
                // Opcjonalnie: można logować starą i nową treść, ale przy długich tekstach to zaśmieca dziennik
                await dziennik.DodajAsync(Program.fullName, logMessage, _nrZgloszenia);

                MessageBox.Show("Działanie zostało zaktualizowane.", "Sukces", MessageBoxButtons.OK, MessageBoxIcon.Information);
                this.DialogResult = DialogResult.OK;
                this.Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Błąd zapisu: " + ex.Message, "Błąd", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private async void btnUsun_Click(object sender, EventArgs e)
        {
            var confirmResult = MessageBox.Show(
                "Czy na pewno chcesz usunąć to działanie?\nOperacja jest nieodwracalna.",
                "Potwierdź usunięcie",
                MessageBoxButtons.YesNo,
                MessageBoxIcon.Warning);

            if (confirmResult == DialogResult.Yes)
            {
                try
                {
                    using (var con = DatabaseHelper.GetConnectionAsync())
                    {
                        await con.OpenAsync();
                        using (var cmd = new MySqlCommand("DELETE FROM Dzialania WHERE Id = @id", con))
                        {
                            cmd.Parameters.AddWithValue("@id", _actionId);
                            await cmd.ExecuteNonQueryAsync();
                        }
                    }

                    var dziennik = new DziennikLogger();
                    await dziennik.DodajAsync(Program.fullName, $"Usunięto działanie (ID: {_actionId})", _nrZgloszenia);

                    MessageBox.Show("Usunięto pomyślnie.", "Info", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    this.DialogResult = DialogResult.OK; // OK wymusi odświeżenie listy w Form2
                    this.Close();
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Błąd usuwania: " + ex.Message, "Błąd", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }

        private void btnAnuluj_Click(object sender, EventArgs e)
        {
            this.DialogResult = DialogResult.Cancel;
            this.Close();
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