using Microsoft.VisualBasic; // Do InputBox (KWZ)
using System;
using System.Collections.Generic;
using MySql.Data.MySqlClient;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Reklamacje_Dane
{
    public partial class Form12 : Form
    {
        private string nrZgloszenia;
        private string originalStatusOgolny;
        private string originalStatusKlient;
        private string originalNrWRL;
        private string originalKwotaZwrotu;

        // Dodajemy serwis magazynowy
        private readonly MagazynService _magazynService;

        public Form12(string nrZgloszenia)
        {
            InitializeComponent();
            this.StartPosition = FormStartPosition.CenterScreen;
            this.nrZgloszenia = nrZgloszenia;
            _magazynService = new MagazynService(); // Inicjalizacja
        

            // Włącz sprawdzanie pisowni dla wszystkich TextBoxów
            EnableSpellCheckOnAllTextBoxes();
        }

        private async void Form12_Load(object sender, EventArgs e)
        {
            await WczytajIWypelnijStatusy();
        }

        // ... (Metody WczytajIWypelnijStatusy, ZarzadzajStatusemKlienta itp. pozostają BEZ ZMIAN) ...
        // ... Wklej je tutaj z Twojego oryginalnego kodu ...
        // ... (Skracam, żeby odpowiedź była czytelna) ...

        private async Task WczytajIWypelnijStatusy()
        {
            cmbStatusOgolny.Items.Add("Procesowana");
            cmbStatusOgolny.Items.Add("Zakończona");

            try
            {
                using (var con = DatabaseHelper.GetConnection())
                {
                    await con.OpenAsync();
                    string query = "SELECT StatusOgolny, StatusKlient, NrWRL, KwotaZwrotu FROM Zgloszenia WHERE NrZgloszenia = @nrZgloszenia";
                    using (var cmd = new MySqlCommand(query, con))
                    {
                        cmd.Parameters.AddWithValue("@nrZgloszenia", this.nrZgloszenia);
                        using (var reader = await cmd.ExecuteReaderAsync())
                        {
                            if (await reader.ReadAsync())
                            {
                                originalStatusOgolny = reader["StatusOgolny"]?.ToString() ?? "Procesowana";
                                originalStatusKlient = reader["StatusKlient"]?.ToString() ?? "Zgłoszone";
                                originalNrWRL = reader["NrWRL"]?.ToString() ?? "";
                                originalKwotaZwrotu = reader["KwotaZwrotu"]?.ToString() ?? "";

                                cmbStatusOgolny.SelectedItem = originalStatusOgolny;
                                txtNrWRL.Text = originalNrWRL;
                                txtKwotaZwrotu.Text = originalKwotaZwrotu;

                                ZarzadzajStatusemKlienta();
                                cmbStatusKlient.SelectedItem = originalStatusKlient;
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Błąd wczytywania statusów: " + ex.Message, "Błąd Krytyczny");
                this.Close();
            }
        }

        private void cmbStatusOgolny_SelectedIndexChanged(object sender, EventArgs e) => ZarzadzajStatusemKlienta();
        private void cmbStatusKlient_SelectedIndexChanged(object sender, EventArgs e) => ZarzadzajPolamiDodatkowymi();

        private void ZarzadzajStatusemKlienta()
        {
            string wybranyStatusOgolny = cmbStatusOgolny.SelectedItem?.ToString();
            if (wybranyStatusOgolny == null) return;

            cmbStatusKlient.DataSource = null;

            if (wybranyStatusOgolny == "Procesowana")
            {
                cmbStatusKlient.DataSource = new List<string> { "Zgłoszone" };
                cmbStatusKlient.Enabled = false;
            }
            else
            {
                var statusyZakonczenia = new List<string>
                {
                    "Uznana - Wysyłka nowego towaru", "Uznana - Naprawa", "Uznana - Zwrot pieniędzy",
                    "Nieuznana - Usterki nie stwierdzono", "Nieuznana - Po okresie reklamacji",
                    "Nieuznana - Usterka z winy użytkowania", "Nieuznana - brak kontaktu z klientem"
                };
                cmbStatusKlient.DataSource = statusyZakonczenia;
                cmbStatusKlient.Enabled = true;
            }
            ZarzadzajPolamiDodatkowymi();
        }

        private void ZarzadzajPolamiDodatkowymi()
        {
            lblNrWRL.Visible = false; txtNrWRL.Visible = false;
            lblKwotaZwrotu.Visible = false; txtKwotaZwrotu.Visible = false;

            if (cmbStatusKlient.SelectedItem == null) return;
            string wybranyStatusKlienta = cmbStatusKlient.SelectedItem.ToString();

            switch (wybranyStatusKlienta)
            {
                case "Uznana - Wysyłka nowego towaru":
                case "Uznana - Naprawa":
                    lblNrWRL.Visible = true; txtNrWRL.Visible = true;
                    chkPoinformujKlienta.Top = txtNrWRL.Bottom + 20;
                    break;
                case "Uznana - Zwrot pieniędzy":
                    lblKwotaZwrotu.Visible = true; txtKwotaZwrotu.Visible = true;
                    chkPoinformujKlienta.Top = txtKwotaZwrotu.Bottom + 20;
                    break;
                default:
                    chkPoinformujKlienta.Top = cmbStatusKlient.Bottom + 20;
                    break;
            }
            btnZapisz.Top = chkPoinformujKlienta.Bottom + 15;
            this.ClientSize = new System.Drawing.Size(452, btnZapisz.Bottom + 20);
        }

        // --- GŁÓWNA LOGIKA ZAPISU (INTEGRACJA) ---

        private async void btnZapisz_Click(object sender, EventArgs e)
        {
            string nowyStatusOgolny = cmbStatusOgolny.SelectedItem.ToString();
            string nowyStatusKlient = cmbStatusKlient.SelectedItem.ToString();
            string nowyNrWRL = txtNrWRL.Text;
            string nowaKwotaZwrotu = txtKwotaZwrotu.Text;

            if (nowyStatusOgolny == originalStatusOgolny && nowyStatusKlient == originalStatusKlient &&
                nowyNrWRL == originalNrWRL && nowaKwotaZwrotu == originalKwotaZwrotu)
            {
                MessageBox.Show("Nie wprowadzono żadnych zmian.", "Informacja");
                return;
            }

            // ----------------------------------------------------------------
            // 1. OBSŁUGA MAGAZYNOWA (PYTANIA DO UŻYTKOWNIKA)
            // Wykonujemy to PRZED transakcją, bo to wymaga interakcji z UI
            // ----------------------------------------------------------------

            // Scenariusz: WYMIANA / ZWROT KASY -> Co ze starym sprzętem?
            if (nowyStatusKlient.Contains("Wysyłka nowego") || nowyStatusKlient.Contains("Zwrot pieniędzy"))
            {
                using (var decForm = new FormDecyzjaSprzet())
                {
                    if (decForm.ShowDialog() == DialogResult.OK)
                    {
                        await ObsluzDecyzjeMagazynowa(decForm.WybranaDecyzja);
                    }
                    else
                    {
                        return; // Anulowano decyzję -> przerywamy zapis
                    }
                }
            }
            // 2. Logika dla NAPRAWY (Zużycie części + CROSS-LOGGING)
            else if (nowyStatusKlient.Contains("Naprawa") || nowyStatusKlient.Contains("Naprawiono"))
            {
                if (MessageBox.Show("Czy użyto części z magazynu (z demontażu)?", "Części", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
                {
                    using (var formSzukaj = new FormWybierzCzesc())
                    {
                        if (formSzukaj.ShowDialog() == DialogResult.OK && formSzukaj.WybranaCzesc != null)
                        {
                            var czesc = formSzukaj.WybranaCzesc;

                            // A. Zdejmujemy ze stanu (przypisujemy do AKTUALNEGO zgłoszenia)
                            await _magazynService.UzyjCzescAsync(czesc.Id, this.nrZgloszenia);

                            // B. Log u BIORCY (Aktualne zgłoszenie)
                            string logBiorca = $"NAPRAWA: Zamontowano część '{czesc.NazwaCzesci}' pochodzącą z dawcy: {czesc.ModelDawcy} (Zgł. {czesc.ZgloszenieDawcy}).";
                            await new DziennikLogger().DodajAsync(Program.fullName, logBiorca, this.nrZgloszenia);
                            new Dzialaniee().DodajNoweDzialanie(this.nrZgloszenia, Program.fullName, logBiorca);

                            // C. Log u DAWCY (Tamtego zgłoszenia) - TO JEST NOWOŚĆ
                            if (!string.IsNullOrEmpty(czesc.ZgloszenieDawcy))
                            {
                                string logDawca = $"MAGAZYN: Część '{czesc.NazwaCzesci}' została pobrana i użyta do naprawy zgłoszenia {this.nrZgloszenia}.";
                                await new DziennikLogger().DodajAsync(Program.fullName, logDawca, czesc.ZgloszenieDawcy);
                                new Dzialaniee().DodajNoweDzialanie(czesc.ZgloszenieDawcy, Program.fullName, logDawca);
                            }

                            MessageBox.Show("Część przypisana. Historia zaktualizowana w obu zgłoszeniach.", "Sukces", MessageBoxButtons.OK, MessageBoxIcon.Information);
                        }
                    }
                }
            }

            // ----------------------------------------------------------------
            // 2. ZAPIS DANYCH (TRANSAKCJA) - Twój oryginalny kod
            // ----------------------------------------------------------------

            var sb = new StringBuilder();
            if (nowyStatusOgolny != originalStatusOgolny) sb.Append($"Zmieniono Status Ogólny z '{originalStatusOgolny}' na '{nowyStatusOgolny}'. ");
            if (nowyStatusKlient != originalStatusKlient) sb.Append($"Zmieniono Status Klienta z '{originalStatusKlient}' na '{nowyStatusKlient}'. ");
            if (nowyNrWRL != originalNrWRL) sb.Append($"Zmieniono Nr WRL z '{originalNrWRL}' na '{nowyNrWRL}'. ");
            if (nowaKwotaZwrotu != originalKwotaZwrotu) sb.Append($"Zmieniono Kwotę zwrotu z '{originalKwotaZwrotu}' na '{nowaKwotaZwrotu}'. ");
            string logMessage = sb.ToString().Trim();

            using (var con = DatabaseHelper.GetConnection())
            {
                await con.OpenAsync();
                using (var transaction = con.BeginTransaction())
                {
                    try
                    {
                        string updateQuery = @"UPDATE Zgloszenia SET StatusOgolny = @statusOgolny, StatusKlient = @statusKlient, NrWRL = @nrWRL, KwotaZwrotu = @kwotaZwrotu WHERE NrZgloszenia = @nrZgloszenia";
                        using (var cmdUpdate = new MySqlCommand(updateQuery, con, transaction))
                        {
                            cmdUpdate.Parameters.AddWithValue("@statusOgolny", nowyStatusOgolny);
                            cmdUpdate.Parameters.AddWithValue("@statusKlient", nowyStatusKlient);
                            cmdUpdate.Parameters.AddWithValue("@nrWRL", nowyNrWRL);
                            cmdUpdate.Parameters.AddWithValue("@kwotaZwrotu", nowaKwotaZwrotu);
                            cmdUpdate.Parameters.AddWithValue("@nrZgloszenia", this.nrZgloszenia);
                            await cmdUpdate.ExecuteNonQueryAsync();
                        }

                        var dziennik = new DziennikLogger();
                        var dzialanie = new Dzialaniee();
                        await dziennik.DodajAsync(con, transaction, Program.fullName, logMessage, this.nrZgloszenia);
                        dzialanie.DodajNoweDzialanie(con, transaction, this.nrZgloszenia, Program.fullName, logMessage);

                        transaction.Commit();
                    }
                    catch (Exception ex)
                    {
                        transaction.Rollback();
                        MessageBox.Show("Błąd zapisu zmian: " + ex.Message, "Błąd Krytyczny");
                        return;
                    }
                }
            }

            MessageBox.Show("Statusy zostały zaktualizowane.", "Sukces");
            UpdateManager.NotifySubscribers();

            if (chkPoinformujKlienta.Checked)
            {
                using (var form4 = new Form4(this.nrZgloszenia, $"Zmieniono status zgłoszenia na: {nowyStatusKlient}."))
                {
                    form4.ShowDialog();
                }
            }

            this.Close();
        }

        // --- POMOCNICZA METODA MAGAZYNOWA ---

        private async Task ObsluzDecyzjeMagazynowa(DecyzjaMagazynowa decyzja)
        {
            if (decyzja == DecyzjaMagazynowa.BrakAkcji || decyzja == DecyzjaMagazynowa.Anuluj) return;

            string model = "Nieznany";
            string sn = "";
            int pid = 0;
            string kat = "";

            // Pobieramy dane o produkcie
            try
            {
                using (var con = DatabaseHelper.GetConnection())
                {
                    await con.OpenAsync();
                    var cmd = new MySqlCommand("SELECT p.NazwaSystemowa, z.NrSeryjny, p.Id, p.Kategoria FROM Zgloszenia z LEFT JOIN Produkty p ON z.ProduktID=p.Id WHERE z.NrZgloszenia=@nr", con);
                    cmd.Parameters.AddWithValue("@nr", this.nrZgloszenia);
                    using (var r = await cmd.ExecuteReaderAsync())
                    {
                        if (await r.ReadAsync())
                        {
                            model = r["NazwaSystemowa"]?.ToString() ?? "";
                            sn = r["NrSeryjny"]?.ToString() ?? "";
                            pid = r["Id"] != DBNull.Value ? Convert.ToInt32(r["Id"]) : 0;
                            kat = r["Kategoria"]?.ToString() ?? "";
                        }
                    }
                }
            }
            catch { }

            // Sprawdzamy czy już jest na stanie
            var stan = await _magazynService.PobierzStanAsync(this.nrZgloszenia);
            if (stan == null)
            {
                await _magazynService.PrzyjmijNaMagazynAsync(this.nrZgloszenia, model, sn, "Automatyczne przyjęcie przy zmianie statusu");
            }

            // Obsługa konkretnej decyzji
            using (var formMag = new FormMagazynAction(this.nrZgloszenia, pid, model, sn, kat))
            {
                if (decyzja == DecyzjaMagazynowa.NaCzesci)
                {
                    MessageBox.Show("Otwieram kartę magazynową.\n1. Zmień status na 'DAWCA CZĘŚCI'.\n2. Zaznacz odzyskane części.", "Instrukcja");
                }
                else if (decyzja == DecyzjaMagazynowa.Utylizacja)
                {
                    MessageBox.Show("Otwieram kartę magazynową.\nZmień status na 'ZŁOM / Utylizacja'.", "Instrukcja");
                }
                else if (decyzja == DecyzjaMagazynowa.NaStan)
                {
                    string dok = Interaction.InputBox("Podaj numer Korekty Faktury lub KWZ:", "Wymagany dokument", "");
                    if (!string.IsNullOrWhiteSpace(dok))
                    {
                        await _magazynService.AktualizujStatusAsync(this.nrZgloszenia, "Przyjęty na stan", "Magazyn Główny", false, "Dokument: " + dok);
                    }
                }
                formMag.ShowDialog();
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