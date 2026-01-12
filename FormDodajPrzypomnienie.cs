using System;
using System.Collections.Generic;
using MySql.Data.MySqlClient;
using System.Windows.Forms;

namespace Reklamacje_Dane
{
    public partial class FormDodajPrzypomnienie : Form
    {
        private int? _reminderId; // null => tryb dodawania

        // Konstruktor do tworzenia nowego przypomnienia
        public FormDodajPrzypomnienie()
        {
            InitializeComponent();
            this.Text = "Dodaj nowe przypomnienie";
            _reminderId = null;
        

            // Włącz sprawdzanie pisowni dla wszystkich TextBoxów
            EnableSpellCheckOnAllTextBoxes();
        }

        // Konstruktor do edycji istniejącego przypomnienia
        public FormDodajPrzypomnienie(int reminderId)
        {
            InitializeComponent();
            this.Text = "Edytuj przypomnienie";
            _reminderId = reminderId;
            LoadReminderData();
        }

        private void LoadReminderData()
        {
            if (_reminderId == null) return;

            using (var con = DatabaseHelper.GetConnection())
            {
                con.Open();
                using (var cmd = new MySqlCommand(
                    "SELECT Tresc, DataPrzypomnienia, DotyczyZgloszenia FROM Przypomnienia WHERE Id = @id", con))
                {
                    cmd.Parameters.AddWithValue("@id", _reminderId.Value);

                    using (var reader = cmd.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            txtTresc.Text = reader.IsDBNull(0) ? "" : reader.GetString(0);
                            dtpData.Value = reader.IsDBNull(1) ? DateTime.Now : reader.GetDateTime(1);
                            txtNrZgloszenia.Text = reader.IsDBNull(2) ? "" : reader.GetString(2);
                        }
                    }
                }
            }
        }

        private void btnZapisz_Click(object sender, EventArgs e)
        {
            using (var con = DatabaseHelper.GetConnection())
            {
                con.Open();
                MySqlCommand cmd;

                if (_reminderId == null) // Tryb dodawania
                {
                    cmd = new MySqlCommand(
                        "INSERT INTO Przypomnienia (Tresc, DataPrzypomnienia, DotyczyZgloszenia, CzyZrealizowane) " +
                        "VALUES (@tresc, @data, @nr, 0)", con);
                }
                else // Tryb edycji
                {
                    cmd = new MySqlCommand(
                        "UPDATE Przypomnienia SET Tresc = @tresc, DataPrzypomnienia = @data, DotyczyZgloszenia = @nr " +
                        "WHERE Id = @id", con);
                    cmd.Parameters.AddWithValue("@id", _reminderId.Value);
                }

                cmd.Parameters.AddWithValue("@tresc", txtTresc.Text);
                cmd.Parameters.AddWithValue("@data", dtpData.Value);
                cmd.Parameters.AddWithValue("@nr",
                    string.IsNullOrWhiteSpace(txtNrZgloszenia.Text)
                        ? (object)DBNull.Value
                        : txtNrZgloszenia.Text);

                cmd.ExecuteNonQuery();
            }

            this.DialogResult = DialogResult.OK;
            this.Close();
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
