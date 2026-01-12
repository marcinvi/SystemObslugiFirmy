using System;
using System.Collections.Generic;
using System.ComponentModel;
using MySql.Data.MySqlClient;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Reklamacje_Dane
{
    public partial class ClientInfoControl : UserControl
    {
        private int _klientId;
        private bool _isInEditMode = false;

        // NOWE: Prywatne pola do przechowywania czystych danych do kopiowania
        private string _imieNazwisko, _nazwaFirmy, _email, _telefon, _ulica, _kodPocztowy, _miejscowosc, _nip;

        public event EventHandler DataChanged;
        public event EventHandler ChangeClientRequested;

        public ClientInfoControl()
        {
            InitializeComponent();
        }

        public async Task LoadClientData(int klientId)
        {
            if (klientId <= 0)
            {
                ShowEmpty();
                return;
            }

            this._klientId = klientId;
            try
            {
                using (var con = DatabaseHelper.GetConnection())
                {
                    await con.OpenAsync();
                    string query = "SELECT * FROM Klienci WHERE Id = @id";
                    using (var cmd = new MySqlCommand(query, con))
                    {
                        cmd.Parameters.AddWithValue("@id", _klientId);
                        using (var reader = await cmd.ExecuteReaderAsync())
                        {
                            if (await reader.ReadAsync())
                            {
                                // Wypełnianie prywatnych pól
                                _imieNazwisko = reader["ImieNazwisko"]?.ToString();
                                _nazwaFirmy = reader["NazwaFirmy"]?.ToString();
                                _email = reader["Email"]?.ToString();
                                _telefon = reader["Telefon"]?.ToString();
                                _ulica = reader["Ulica"]?.ToString();
                                _kodPocztowy = reader["KodPocztowy"]?.ToString();
                                _miejscowosc = reader["Miejscowosc"]?.ToString();
                                _nip = reader["NIP"]?.ToString();

                                // Ustawianie etykiet
                                lblImieNazwisko.Text = $"{_imieNazwisko} | {_nazwaFirmy}";
                                lblEmail.Text = _email;
                                lblTelefon.Text = _telefon;
                                lblAdres1.Text = _ulica;
                                lblAdres2.Text = $"{_kodPocztowy} {_miejscowosc}";
                                contextMenuStrip1.Enabled = true;
                            }
                            else
                            {
                                ShowEmpty();
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Błąd wczytywania danych klienta: " + ex.Message);
                ShowEmpty();
            }
        }

        private void ShowEmpty()
        {
            lblImieNazwisko.Text = "Brak przypisanego klienta";
            lblEmail.Text = "";
            lblTelefon.Text = "";
            lblAdres1.Text = "";
            lblAdres2.Text = "";
            contextMenuStrip1.Enabled = false;
        }

        private void EnterEditMode()
        {
            if (_klientId <= 0) return;

            _isInEditMode = true;

            // Wypełnij pola edycji aktualnymi danymi
            txtImieNazwisko.Text = lblImieNazwisko.Text.Split('|')[0].Trim();
            txtNazwaFirmy.Text = lblImieNazwisko.Text.Contains("|") ? lblImieNazwisko.Text.Split('|')[1].Trim() : "";
            txtMail.Text = lblEmail.Text;
            txtTelefon.Text = lblTelefon.Text;
            txtUlicaNr.Text = lblAdres1.Text;
            txtKodPocztowy.Text = lblAdres2.Text.Split(' ')[0].Trim();
            txtMiejscowosc.Text = lblAdres2.Text.Contains(" ") ? lblAdres2.Text.Substring(lblAdres2.Text.IndexOf(' ')).Trim() : "";

            panelDisplay.Visible = false;
            panelEdit.Visible = true;
        }

        private void LeaveEditMode()
        {
            _isInEditMode = false;
            panelEdit.Visible = false;
            panelDisplay.Visible = true;
        }

        private void edytujDaneKlientaToolStripMenuItem_Click(object sender, EventArgs e)
        {
            EnterEditMode();
        }

        private void zmieńKlientaToolStripMenuItem_Click(object sender, EventArgs e)
        {
            ChangeClientRequested?.Invoke(this, EventArgs.Empty);
        }
        public Dictionary<string, string> GetDataForPrinting()
        {
            var data = new Dictionary<string, string>();

            // ### KLUCZOWA ZMIANA ###
            // Pobieramy dane z prywatnych pól, które są zawsze wypełnione po wczytaniu klienta,
            // a nie z pól tekstowych trybu edycji.

            data.Add("Klient", _imieNazwisko);
            data.Add("Firma", _nazwaFirmy);

            // Poprawne łączenie adresu w jedną całość
            string pelnyAdres = "";
            if (!string.IsNullOrWhiteSpace(_ulica))
            {
                pelnyAdres += _ulica;
            }
            if (!string.IsNullOrWhiteSpace(_kodPocztowy) || !string.IsNullOrWhiteSpace(_miejscowosc))
            {
                if (!string.IsNullOrEmpty(pelnyAdres)) pelnyAdres += "\n"; // Nowa linia dla kodu i miasta
                pelnyAdres += $"{_kodPocztowy} {_miejscowosc}".Trim();
            }
            data.Add("Adres", pelnyAdres);

            data.Add("Email", _email);
            data.Add("Telefon", _telefon);
            data.Add("NIP", _nip);

            return data;
        }
        private async void btnZapisz_Click(object sender, EventArgs e)
        {
            try
            {
                using (var con = DatabaseHelper.GetConnection())
                {
                    await con.OpenAsync();
                    string query = @"UPDATE Klienci SET ImieNazwisko = @imie, NazwaFirmy = @firma, Ulica = @ulica, 
                                     KodPocztowy = @kod, Miejscowosc = @miasto, Email = @mail, Telefon = @tel WHERE Id = @id";
                    using (var cmd = new MySqlCommand(query, con))
                    {
                        cmd.Parameters.AddWithValue("@id", _klientId);
                        cmd.Parameters.AddWithValue("@imie", txtImieNazwisko.Text);
                        cmd.Parameters.AddWithValue("@firma", txtNazwaFirmy.Text);
                        cmd.Parameters.AddWithValue("@ulica", txtUlicaNr.Text);
                        cmd.Parameters.AddWithValue("@kod", txtKodPocztowy.Text);
                        cmd.Parameters.AddWithValue("@miasto", txtMiejscowosc.Text);
                        cmd.Parameters.AddWithValue("@mail", txtMail.Text);
                        cmd.Parameters.AddWithValue("@tel", txtTelefon.Text);
                        await cmd.ExecuteNonQueryAsync();
                    }
                }

                var dziennik = new DziennikLogger();
                await dziennik.DodajAsync(Program.fullName, $"Zaktualizowano dane klienta: {txtImieNazwisko.Text}", (this.ParentForm as Form2)?.NrZgloszeniaPublic);


                MessageBox.Show("Dane klienta zostały zaktualizowane.", "Sukces");
                LeaveEditMode();
                DataChanged?.Invoke(this, EventArgs.Empty); // Informuj Form2, że dane się zmieniły
            }
            catch (Exception ex)
            {
                MessageBox.Show("Błąd zapisu zmian klienta: " + ex.Message);
            }
        }

        private void btnAnuluj_Click(object sender, EventArgs e)
        {
            LeaveEditMode();
        }
        private void contextMenuStrip1_Opening(object sender, CancelEventArgs e)
        {
            // Ta metoda jest wywoływana tuż przed pokazaniem menu
            // i ustawia widoczność opcji na podstawie dostępnych danych.
            skopiujImięNazwiskoToolStripMenuItem.Visible = !string.IsNullOrEmpty(_imieNazwisko);
            skopiujNazwęFirmyToolStripMenuItem.Visible = !string.IsNullOrEmpty(_nazwaFirmy);
            skopiujNIPToolStripMenuItem.Visible = !string.IsNullOrEmpty(_nip);
            skopiujAdresMailowyToolStripMenuItem.Visible = !string.IsNullOrEmpty(_email);
            skopiujNrTelefonuToolStripMenuItem.Visible = !string.IsNullOrEmpty(_telefon);
            skopiujUlicęToolStripMenuItem.Visible = !string.IsNullOrEmpty(_ulica);
            skopiujKodPocztowyToolStripMenuItem.Visible = !string.IsNullOrEmpty(_kodPocztowy);
            skopiujMiejscowośćToolStripMenuItem.Visible = !string.IsNullOrEmpty(_miejscowosc);
        }

        // Prosta metoda pomocnicza do kopiowania
        private void KopiujDoSchowka(string tekst)
        {
            if (!string.IsNullOrEmpty(tekst))
            {
                Clipboard.SetText(tekst);
            }
        }

        private void skopiujImięNazwiskoToolStripMenuItem_Click(object sender, EventArgs e) => KopiujDoSchowka(_imieNazwisko);

        private void skopiujNazwęFirmyToolStripMenuItem_Click(object sender, EventArgs e) => KopiujDoSchowka(_nazwaFirmy);
        private void skopiujNIPToolStripMenuItem_Click(object sender, EventArgs e) => KopiujDoSchowka(_nip);
        private void skopiujAdresMailowyToolStripMenuItem_Click(object sender, EventArgs e) => KopiujDoSchowka(_email);
        private void skopiujNrTelefonuToolStripMenuItem_Click(object sender, EventArgs e) => KopiujDoSchowka(_telefon);
        private void skopiujUlicęToolStripMenuItem_Click(object sender, EventArgs e) => KopiujDoSchowka(_ulica);
        private void skopiujKodPocztowyToolStripMenuItem_Click(object sender, EventArgs e) => KopiujDoSchowka(_kodPocztowy);
        private void skopiujMiejscowośćToolStripMenuItem_Click(object sender, EventArgs e) => KopiujDoSchowka(_miejscowosc);


    }
}
    
