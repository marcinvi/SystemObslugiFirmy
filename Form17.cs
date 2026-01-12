using System;
using System.Collections.Generic;
using MySql.Data.MySqlClient;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Reklamacje_Dane
{
    public partial class Form17 : Form
    {
        private string nrZgloszenia;
        private string originalProduktNazwa;
        private string originalSeryjny;
        private string originalFaktura;
        private string originalData;

        // Klasa pomocnicza do przechowywania produktów na liście
        private class ProduktListItem
        {
            public int Id { get; set; }
            public string Nazwa { get; set; }
            public override string ToString() => Nazwa;
        }

        public Form17(string nrZgloszenia)
        {
            InitializeComponent();
            this.StartPosition = FormStartPosition.CenterScreen;
            this.nrZgloszenia = nrZgloszenia;
        

            // Włącz sprawdzanie pisowni dla wszystkich TextBoxów
            EnableSpellCheckOnAllTextBoxes();
        }

        private async void Form17_Load_1(object sender, EventArgs e)
        {
            await WczytajListeProduktow();
            await WczytajDaneZgloszenia();
        }

        private async Task WczytajListeProduktow()
        {
            var produkty = new List<ProduktListItem>();
            try
            {
                using (var con = DatabaseHelper.GetConnection())
                {
                    await con.OpenAsync();
                    string query = "SELECT Id, NazwaKrotka FROM Produkty ORDER BY NazwaKrotka";
                    using (var cmd = new MySqlCommand(query, con))
                    {
                        using (var reader = await cmd.ExecuteReaderAsync())
                        {
                            while (await reader.ReadAsync())
                            {
                                produkty.Add(new ProduktListItem
                                {
                                    Id = Convert.ToInt32(reader["Id"]),
                                    Nazwa = reader["NazwaKrotka"].ToString()
                                });
                            }
                        }
                    }
                }
                Produkt.DataSource = produkty;
                Produkt.DisplayMember = "Nazwa";
                Produkt.ValueMember = "Id";
            }
            catch (Exception ex)
            {
                MessageBox.Show("Błąd wczytywania listy produktów: " + ex.Message);
            }
        }

        private async Task WczytajDaneZgloszenia()
        {
            try
            {
                using (var con = DatabaseHelper.GetConnection())
                {
                    await con.OpenAsync();
                    string query = "SELECT ProduktID, NrSeryjny, NrFaktury, DataZakupu FROM Zgloszenia WHERE NrZgloszenia = @nrZgloszenia";
                    using (var cmd = new MySqlCommand(query, con))
                    {
                        cmd.Parameters.AddWithValue("@nrZgloszenia", this.nrZgloszenia);
                        using (var reader = await cmd.ExecuteReaderAsync())
                        {
                            if (await reader.ReadAsync())
                            {
                                // Ustawiamy kontrolki
                                Produkt.SelectedValue = Convert.ToInt32(reader["ProduktID"]);
                                Nrseryjny.Text = reader["NrSeryjny"].ToString();
                                Nrfaktury.Text = reader["NrFaktury"].ToString();
                                Datazakupu.Text = reader["DataZakupu"].ToString();

                                // Zapisujemy oryginalne wartości do porównania
                                originalProduktNazwa = Produkt.Text;
                                originalSeryjny = Nrseryjny.Text;
                                originalFaktura = Nrfaktury.Text;
                                originalData = Datazakupu.Text;
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Błąd wczytywania danych zgłoszenia: " + ex.Message);
            }
        }

        private async void btnZapisz_Click(object sender, EventArgs e)
        {
            // Budujemy raport ze zmianami
            StringBuilder sb = new StringBuilder();
            if (Produkt.Text != originalProduktNazwa) sb.AppendLine($"Produkt z \"{originalProduktNazwa}\" na \"{Produkt.Text}\"");
            if (Nrseryjny.Text != originalSeryjny) sb.AppendLine($"Numer seryjny z \"{originalSeryjny}\" na \"{Nrseryjny.Text}\"");
            if (Nrfaktury.Text != originalFaktura) sb.AppendLine($"Nr Faktury z \"{originalFaktura}\" na \"{Nrfaktury.Text}\"");
            if (Datazakupu.Text != originalData) sb.AppendLine($"Data z \"{originalData}\" na \"{Datazakupu.Text}\"");

            string wynikZmian = sb.ToString();

            try
            {
                using (var con = DatabaseHelper.GetConnection())
                {
                    await con.OpenAsync();
                    string query = "UPDATE Zgloszenia SET ProduktID = @produktID, NrSeryjny = @nrSeryjny, NrFaktury = @nrFaktury, DataZakupu = @dataZakupu WHERE NrZgloszenia = @nrZgloszenia";
                    using (var cmd = new MySqlCommand(query, con))
                    {
                        cmd.Parameters.AddWithValue("@produktID", Produkt.SelectedValue);
                        cmd.Parameters.AddWithValue("@nrSeryjny", Nrseryjny.Text);
                        cmd.Parameters.AddWithValue("@nrFaktury", Nrfaktury.Text);
                        cmd.Parameters.AddWithValue("@dataZakupu", Datazakupu.Text);
                        cmd.Parameters.AddWithValue("@nrZgloszenia", this.nrZgloszenia);
                        await cmd.ExecuteNonQueryAsync();
                    }
                }

                if (string.IsNullOrEmpty(wynikZmian))
                {
                    MessageBox.Show("Brak zmian do zapisania.");
                }
                else
                {
                    var dziennik = new DziennikLogger();
                    await dziennik.DodajAsync(Program.fullName, "Zmieniono dane zakupu: " + wynikZmian.Replace("\r\n", " "), this.nrZgloszenia);
                    MessageBox.Show("Zmiany zostały zapisane.", "Sukces", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }

                UpdateManager.NotifySubscribers();
                this.Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Błąd podczas zapisu zmian: " + ex.Message);
            }
        }

        private void dateTimePicker1_ValueChanged_1(object sender, EventArgs e)
        {
            Datazakupu.Text = dateTimePicker1.Value.ToString("yyyy-MM-dd");
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