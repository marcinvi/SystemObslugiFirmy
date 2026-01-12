// Plik: Faktura.cs
using System;
using MySql.Data.MySqlClient;
using System.Globalization;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Drawing; // Dodano, by używać Point i Size

namespace Reklamacje_Dane
{
    public partial class FakturaForm : Form
    {
        private string nrZgloszenia;
        private bool isCalculating = false;
        private readonly DatabaseService _dbService;

        // Stałe wysokości formularza dla różnych stanów
        private const int MinimalHeight = 330;
        private const int ExpandedHeight = 330;

        public FakturaForm(string nrZgloszenia)
        {
            InitializeComponent();
            this.StartPosition = FormStartPosition.CenterScreen;
            this.nrZgloszenia = nrZgloszenia;
            _dbService = new DatabaseService(DatabaseHelper.GetConnectionString());
        }

        private async void FakturaForm_Load(object sender, EventArgs e)
        {
            comboBoxCurrency.SelectedItem = "PLN";
            Rodzaj.SelectedIndex = 0;
            // Początkowo ustawiamy wysokość na minimalną i ukrywamy panel
            this.Height = MinimalHeight;
            pnlKosztowa.Visible = false;
        }

        private async void btnLogin_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(Rodzaj.Text) || string.IsNullOrWhiteSpace(NrFaktury.Text))
            {
                ToastManager.ShowToast("Błąd walidacji", "Proszę wypełnić pola Rodzaj i Numer Faktury.", NotificationType.Warning);
                return;
            }

            try
            {
                // Użycie transakcji dla zapewnienia spójności
                using (var con = DatabaseHelper.GetConnection())
                {
                    await con.OpenAsync();
                    using (var transaction = con.BeginTransaction())
                    {
                        try
                        {
                            string query = @"INSERT INTO Faktury 
                                             (NrZgloszenia, Rodzaj, NrFaktury, OdKogo, KwotaBrutto, KwotaNetto, Waluta) 
                                             VALUES 
                                             (@nrZgloszenia, @rodzaj, @nrFaktury, @odKogo, @kwotaBrutto, @kwotaNetto, @waluta)";

                            using (var cmd = new MySqlCommand(query, con, transaction))
                            {
                                cmd.Parameters.AddWithValue("@nrZgloszenia", this.nrZgloszenia);
                                cmd.Parameters.AddWithValue("@rodzaj", Rodzaj.Text);
                                cmd.Parameters.AddWithValue("@nrFaktury", NrFaktury.Text.Trim());
                                cmd.Parameters.AddWithValue("@odKogo", Rodzaj.Text == "Kosztowa" ? Odkogo.Text : "");
                                cmd.Parameters.AddWithValue("@kwotaBrutto", numBrutto.Value);
                                cmd.Parameters.AddWithValue("@kwotaNetto", numNetto.Value);
                                cmd.Parameters.AddWithValue("@waluta", comboBoxCurrency.Text);

                                await cmd.ExecuteNonQueryAsync();
                            }

                            var dziennik = new DziennikLogger();
                            await dziennik.DodajAsync(con, transaction, Program.fullName, $"Dodano fakturę {Rodzaj.Text} nr {NrFaktury.Text.Trim()}", this.nrZgloszenia);

                            var dzialanie = new Dzialaniee();
                            dzialanie.DodajNoweDzialanie(con, transaction, this.nrZgloszenia, Program.fullName, $"Dodano fakturę {Rodzaj.Text} nr {NrFaktury.Text.Trim()}");

                            transaction.Commit();
                            ToastManager.ShowToast("Sukces", "Faktura została pomyślnie dodana.", NotificationType.Success);
                        }
                        catch (Exception ex)
                        {
                            transaction.Rollback();
                            throw new Exception("Błąd podczas zapisywania faktury: " + ex.Message, ex);
                        }
                    }
                }
                UpdateManager.NotifySubscribers();
                this.Close();
            }
            catch (Exception ex)
            {
                ToastManager.ShowToast("Błąd krytyczny", "Wystąpił błąd podczas dodawania faktury: " + ex.Message, NotificationType.Error);
            }
        }

        #region Logika pomocnicza formularza

        private async void WczytajDostawcow()
        {
            Odkogo.Items.Clear();
            Odkogo.Items.Add("Dostawca ręczny");
            try
            {
                // Użycie DatabaseService do bezpiecznego dostępu
                string query = "SELECT NazwaProducenta FROM Producenci UNION SELECT NazwaFirmy FROM Klienci WHERE NazwaFirmy IS NOT NULL AND NazwaFirmy != '' ORDER BY 1";
                var dt = await _dbService.GetDataTableAsync(query);

              //  foreach (DataRow row in dt.Rows)
                {
               //     Odkogo.Items.Add(row[0].ToString());
                }
            }
            catch (Exception ex)
            {
                ToastManager.ShowToast("Błąd", "Błąd wczytywania dostawców: " + ex.Message, NotificationType.Error);
            }
        }

        private void Rodzaj_SelectedIndexChanged(object sender, EventArgs e)
        {
            bool isKosztowa = (Rodzaj.Text == "Kosztowa");
            pnlKosztowa.Visible = isKosztowa;

            // Animacja rozmiaru okna
            if (isKosztowa)
            {
                this.Size = new Size(this.Width, ExpandedHeight);
                WczytajDostawcow();
            }
            else
            {
                this.Size = new Size(this.Width, MinimalHeight);
            }
        }

        private void chkBezVAT_CheckedChanged(object sender, EventArgs e)
        {
            if (isCalculating) return;
            isCalculating = true;

            if (chkBezVAT.Checked)
            {
                numNetto.Value = numBrutto.Value;
            }
            else
            {
                numNetto.Value = Math.Round(numBrutto.Value / 1.23m, 2);
            }
            isCalculating = false;
        }

        private void numBrutto_ValueChanged(object sender, EventArgs e)
        {
            if (isCalculating) return;
            isCalculating = true;

            if (chkBezVAT.Checked)
            {
                numNetto.Value = numBrutto.Value;
            }
            else
            {
                numNetto.Value = Math.Round(numBrutto.Value / 1.23m, 2);
            }
            isCalculating = false;
        }

        private void numNetto_ValueChanged(object sender, EventArgs e)
        {
            if (isCalculating) return;
            isCalculating = true;

            if (chkBezVAT.Checked)
            {
                numBrutto.Value = numNetto.Value;
            }
            else
            {
                numBrutto.Value = Math.Round(numNetto.Value * 1.23m, 2);
            }
            isCalculating = false;
        }

        #endregion
    }
}