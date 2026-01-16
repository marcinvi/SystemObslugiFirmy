// ############################################################################
// Plik: Form5.cs (POPRAWIONY - FIX BŁĘDU CS0104 'System.Action')
// Opis: Rozwiązano konflikt nazw między System.Action a Outlook.Action.
// ############################################################################

using Reklamacje_Dane.Allegro.Issues;
using System;
using System.Collections.Generic;
using MySql.Data.MySqlClient;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows.Forms;
using Microsoft.Office.Interop.Outlook;
using System.Data;
using System.Text;

namespace Reklamacje_Dane
{
    public enum TrybZamowieniaKuriera
    {
        WysylkaDoKlienta,
        OdbiorOdKlienta
    }

    public partial class Form5 : Form
    {
        private readonly string nrZgloszenia;
        private string _allegroDisputeId;
        private int _allegroAccountId;
        private List<Klient> listaKlientow;
        private string _kategoriaProduktu;
        private string _clientEmail;
        private string _clientPhone;
        private int? _nadawcaId;
        private int? _odbiorcaId;
        private readonly DatabaseService _dbService = new DatabaseService(DatabaseHelper.GetConnectionString());
        private readonly TrybZamowieniaKuriera _tryb;
        private int? _idFirmyWlasnej;

        public Form5(string nrZgloszenia, TrybZamowieniaKuriera tryb)
        {
            InitializeComponent();
            this.StartPosition = FormStartPosition.CenterScreen;
            this.nrZgloszenia = nrZgloszenia;
            this._tryb = tryb;
            this.Text = $"DPD - Zlecenie dla zgłoszenia {nrZgloszenia}";
        

            // Włącz sprawdzanie pisowni dla wszystkich TextBoxów
            EnableSpellCheckOnAllTextBoxes();
        }

        public Form5(string nrZgloszenia) : this(nrZgloszenia, TrybZamowieniaKuriera.WysylkaDoKlienta)
        {
        }

        private async void Form5_Load(object sender, EventArgs e)
        {
            await InicjalizujFormularzAsync();
        }

        // Pomocnicza metoda do bezpiecznego wyświetlania komunikatów
        private void SafeShowMessage(string text, string caption, MessageBoxButtons buttons = MessageBoxButtons.OK, MessageBoxIcon icon = MessageBoxIcon.Information)
        {
            if (this.InvokeRequired)
            {
                // POPRAWKA: Użycie System.Action
                this.BeginInvoke(new System.Action(() => MessageBox.Show(this, text, caption, buttons, icon)));
            }
            else
            {
                MessageBox.Show(this, text, caption, buttons, icon);
            }
        }

        #region --- Inicjalizacja i Logika Formularza ---

        private async Task InicjalizujFormularzAsync()
        {
            await PobierzIdFirmyWlasnejAsync();
            if (!_idFirmyWlasnej.HasValue)
            {
                SafeShowMessage("Nie udało się odnaleźć ID firmy własnej w tabeli 'Ustawienia' (klucz: 'IdFirmyWlasnej').\nFormularz nie może kontynuować.", "Błąd krytyczny", MessageBoxButtons.OK, MessageBoxIcon.Error);
                // POPRAWKA: Użycie System.Action
                this.BeginInvoke(new System.Action(() => this.Close()));
                return;
            }

            listaKlientow = await WczytajKlientowAsync();
            await PobierzDaneZgloszeniaAsync();
            cowysylamy.Text = $"{this.nrZgloszenia} {_kategoriaProduktu}";

            if (_tryb == TrybZamowieniaKuriera.OdbiorOdKlienta)
            {
                await UstawDaneKlientaAsync(true);
                await WypelnijDaneFirmoweAsync(false);
                zlecenie.Checked = true;
            }
            else
            {
                await WypelnijDaneFirmoweAsync(true);
                await UstawDaneKlientaAsync(false);
            }

            textBoxNadawcaSearch.TextChanged += (s, ev) => FiltrujListe(textBoxNadawcaSearch.Text, dgvNadawcaSuggestions);
            textBoxOdbiorcaSearch.TextChanged += (s, ev) => FiltrujListe(textBoxOdbiorcaSearch.Text, dgvOdbiorcaSuggestions);
            dgvNadawcaSuggestions.CellDoubleClick += (s, ev) => WybierzKlientaZListy(dgvNadawcaSuggestions, true);
            dgvOdbiorcaSuggestions.CellDoubleClick += (s, ev) => WybierzKlientaZListy(dgvOdbiorcaSuggestions, false);

            try
            {
                string userDataFolder = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "Reklamacje_Dane", "WebView2_UserData");
                Directory.CreateDirectory(userDataFolder);
                var environment = await Microsoft.Web.WebView2.Core.CoreWebView2Environment.CreateAsync(null, userDataFolder);
                await webView21.EnsureCoreWebView2Async(environment);
                webView21.CoreWebView2.DownloadStarting += CoreWebView2_DownloadStarting;
            }
            catch (System.Exception ex)
            {
                SafeShowMessage($"Nie udało się zainicjować komponentu przeglądarki (WebView2). Błąd: {ex.Message}", "Błąd krytyczny WebView2", MessageBoxButtons.OK, MessageBoxIcon.Error);
                // POPRAWKA: Użycie System.Action
                this.BeginInvoke(new System.Action(() => this.Close()));
            }
        }

        private async Task PobierzIdFirmyWlasnejAsync()
        {
            string query = "SELECT WartoscZaszyfrowana FROM Ustawienia WHERE Klucz = 'IdFirmyWlasnej'";
            var result = await _dbService.ExecuteScalarAsync(query);
            if (result != null && int.TryParse(result.ToString(), out int id))
            {
                _idFirmyWlasnej = id;
            }
        }

        private async Task UstawDaneKlientaAsync(bool jakoNadawca)
        {
            string query = @"SELECT k.* FROM Zgloszenia z JOIN Klienci k ON z.KlientID = k.Id WHERE z.NrZgloszenia = @nrZgloszenia";
            var dt = await _dbService.GetDataTableAsync(query, new MySqlParameter("@nrZgloszenia", this.nrZgloszenia));
            if (dt.Rows.Count > 0)
            {
                WypelnijPola(MapToKlient(dt.Rows[0]), jakoNadawca);
            }
        }

        private async Task WypelnijDaneFirmoweAsync(bool jakoNadawca)
        {
            string query = "SELECT * FROM Klienci WHERE Id = @id";
            var dt = await _dbService.GetDataTableAsync(query, new MySqlParameter("@id", _idFirmyWlasnej.Value));

            if (dt.Rows.Count > 0)
            {
                var firmaJakoKlient = MapToKlient(dt.Rows[0]);
                WypelnijPola(firmaJakoKlient, jakoNadawca);
            }
            else
            {
                SafeShowMessage($"Nie znaleziono danych firmy własnej w tabeli 'Klienci' dla Id = {_idFirmyWlasnej.Value}.", "Błąd danych", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
        }

        #endregion

        #region --- Obsługa Pobierania Etykiet PDF ---

        private void CoreWebView2_DownloadStarting(object sender, Microsoft.Web.WebView2.Core.CoreWebView2DownloadStartingEventArgs e)
        {
            try
            {
                string dirName = nrZgloszenia.Replace('/', '.');
                string dirPath = Path.Combine(System.Windows.Forms.Application.StartupPath, "Dane", dirName);
                Directory.CreateDirectory(dirPath);
                string fileName = Path.GetFileName(e.ResultFilePath) ?? $"Dokument_{DateTime.Now:yyyyMMddHHmmss}.pdf";
                string finalPath = Path.Combine(dirPath, fileName);
                e.ResultFilePath = finalPath;
                e.Handled = true;
                e.DownloadOperation.StateChanged += (s, args) =>
                {
                    if (e.DownloadOperation.State == Microsoft.Web.WebView2.Core.CoreWebView2DownloadState.Completed)
                    {
                        // POPRAWKA: Użycie System.Action
                        this.BeginInvoke(new System.Action(() =>
                        {
                            try { Process.Start(new ProcessStartInfo(finalPath) { UseShellExecute = true }); }
                            catch (System.Exception ex) { MessageBox.Show(this, $"Nie można otworzyć pliku PDF. Błąd: {ex.Message}", "Błąd otwierania pliku", MessageBoxButtons.OK, MessageBoxIcon.Error); }
                        }));
                    }
                };
            }
            catch (System.Exception ex)
            {
                // POPRAWKA: Użycie System.Action
                this.BeginInvoke(new System.Action(() => MessageBox.Show(this, $"Krytyczny błąd podczas przechwytywania pobierania: {ex.Message}", "Błąd pobierania", MessageBoxButtons.OK, MessageBoxIcon.Error)));
            }
        }

        #endregion

        #region --- GŁÓWNA LOGIKA: Zamawianie i Powiadomienia ---

        private async void Zamow_Click(object sender, EventArgs e)
        {
            if (!WalidujDane(true) || !WalidujDane(false)) return;

            Zamow.Enabled = false;
            string numerListu = null;

            Form busyForm = null;
            busyForm = CreateBusyForm();
            busyForm.Show(this);
            busyForm.Refresh();

            try
            {
                var daneLogowania = await PobierzDaneLogowaniaDpdAsync();
                if (daneLogowania == null) { busyForm.Close(); return; }

                bool loggedIn = await ZalogujDoDpd(daneLogowania);
                if (!loggedIn)
                {
                    busyForm.Close();
                    SafeShowMessage("Logowanie nie powiodło się. Proces został przerwany.", "Błąd logowania", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }

                if (zlecenie.Checked)
                {
                    numerListu = await HandleZlecenieOdbioruAsync();
                }
                else
                {
                    numerListu = await HandleGenerowanieListuAsync();
                }

                busyForm.Close();

                if (string.IsNullOrEmpty(numerListu))
                {
                    SafeShowMessage("Automatyczne pobranie numeru listu nie powiodło się.", "Błąd automatyzacji", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    numerListu = ShowManualTrackingNumberDialog();
                    if (string.IsNullOrEmpty(numerListu))
                    {
                        SafeShowMessage("Anulowano operację. Żadne działanie nie zostało zapisane.", "Anulowano", MessageBoxButtons.OK, MessageBoxIcon.Information);
                        return;
                    }
                }

                int nadawcaIdDoZapisu = _nadawcaId.HasValue ? _nadawcaId.Value : -1;
                int odbiorcaIdDoZapisu = _odbiorcaId.HasValue ? _odbiorcaId.Value : -1;

                await DodajPrzesylkeDoBazyAsync(numerListu, nadawcaIdDoZapisu, odbiorcaIdDoZapisu, zlecenie.Checked, zwrotna.Checked);

                string nadawcaDisplay = !string.IsNullOrWhiteSpace(textBoxNadawcaFirma.Text) ? textBoxNadawcaFirma.Text : textBoxNadawcaNazwa.Text;
                string odbiorcaDisplay = !string.IsNullOrWhiteSpace(textBoxOdbiorcaFirma.Text) ? textBoxOdbiorcaFirma.Text : textBoxOdbiorcaNazwa.Text;
                string akcjaDoBazy = $"Zamówiono kuriera od {nadawcaDisplay} do {odbiorcaDisplay}. Numer listu: {numerListu}";

                new Dzialaniee().DodajNoweDzialanie(nrZgloszenia, Program.fullName, akcjaDoBazy);
                await new DziennikLogger().DodajAsync(Program.fullName, akcjaDoBazy, nrZgloszenia);

                // POPRAWKA: Użycie System.Action
                this.BeginInvoke(new System.Action(async () =>
                {
                    MessageBox.Show(this, $"Pomyślnie przetworzono zamówienie kuriera.\nNumer listu: {numerListu}\n\nZapisano działanie w bazie. Teraz możesz wysłać powiadomienie do klienta.", "Sukces", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    await HandleNotifications(numerListu);
                }));
            }
            catch (System.Exception ex)
            {
                if (busyForm != null && !busyForm.IsDisposed) busyForm.Close();
                SafeShowMessage($"Wystąpił nieoczekiwany błąd w procesie zamawiania kuriera: {ex.Message}", "Błąd krytyczny", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            finally
            {
                if (busyForm != null && !busyForm.IsDisposed) busyForm.Close();
                Zamow.Enabled = true;
            }
        }

        #endregion

        #region --- Automatyzacja WebView2 ---

        private async Task<bool> ZalogujDoDpd(Dictionary<string, string> daneLogowania)
        {
            for (int i = 0; i < 2; i++)
            {
                webView21.CoreWebView2.Navigate("https://online.dpd.com.pl/logout.do");
                await Task.Delay(3000);

                string loginScript = $@"
                    document.querySelector('[name=""customerId""]').value = '{daneLogowania["customerId"]}';
                    document.querySelector('[name=""userName""]').value = '{daneLogowania["username"]}';
                    document.querySelector('[name=""password""]').value = '{daneLogowania["password"]}';
                    document.querySelector('[name=""password""]').form.submit();
                ";
                await webView21.CoreWebView2.ExecuteScriptAsync(loginScript);
                await Task.Delay(5000);

                string errorCheckScript = "document.getElementById('errosBlock') !== null && document.getElementById('errosBlock').innerText.includes('Nieprawidłowe hasło')";
                string errorResult = await webView21.CoreWebView2.ExecuteScriptAsync(errorCheckScript);

                if (bool.TryParse(errorResult, out bool hasError) && hasError)
                {
                    if (i == 0)
                    {
                        string newPassword = null;
                        // POPRAWKA: Użycie System.Action
                        this.Invoke(new System.Action(() => { newPassword = ShowNewPasswordDialog(); }));

                        if (string.IsNullOrEmpty(newPassword)) return false;

                        await SaveNewPasswordAsync(newPassword);
                        daneLogowania["password"] = newPassword;
                    }
                    else
                    {
                        return false;
                    }
                }
                else
                {
                    return true;
                }
            }
            return false;
        }

        private async Task<string> HandleGenerowanieListuAsync()
        {
            webView21.CoreWebView2.Navigate("https://online.dpd.com.pl/shipment/single-packages.go");
            await Task.Delay(5000);
            await WypelnijFormularzDpdAsync();

            await webView21.CoreWebView2.ExecuteScriptAsync("document.getElementById('save-package-button')?.click();");
            await Task.Delay(3000);

            await webView21.CoreWebView2.ExecuteScriptAsync("document.getElementById('packageDataCol_PKG.DATA.UNIQUE.FLAG[0].tableButtonSelect')?.click();");
            await Task.Delay(1000);

            await webView21.CoreWebView2.ExecuteScriptAsync("document.getElementById('printLabelsButton')?.click();");
            await Task.Delay(3000);

            await webView21.CoreWebView2.ExecuteScriptAsync("document.querySelector('input[name=\"printType\"][value=\"LABEL_PRINTER\"]')?.click();");
            await Task.Delay(1000);

            await webView21.CoreWebView2.ExecuteScriptAsync("document.getElementById('printButton')?.click();");
            await Task.Delay(5000);

            await webView21.CoreWebView2.ExecuteScriptAsync("document.getElementById('downloadButton')?.click();");
            await Task.Delay(3000);

            webView21.CoreWebView2.Navigate("https://online.dpd.com.pl/packages-confirmed-manage.go");
            await Task.Delay(5000);

            await webView21.CoreWebView2.ExecuteScriptAsync("document.getElementById('packageCol_PKGS.UNIQUE.FLAG[0].tableButtonSelect')?.click();");
            await Task.Delay(1000);

            await webView21.CoreWebView2.ExecuteScriptAsync("document.getElementById('printProtocol')?.click();");
            await Task.Delay(3000);

            await webView21.CoreWebView2.ExecuteScriptAsync("document.getElementById('printButton')?.click();");
            await Task.Delay(5000);

            await webView21.CoreWebView2.ExecuteScriptAsync("document.getElementById('downloadButton')?.click();");
            await Task.Delay(3000);

            string nadawca = "", odbiorca = "";
            // POPRAWKA: Użycie System.Action
            this.Invoke(new System.Action(() => {
                nadawca = !string.IsNullOrWhiteSpace(textBoxNadawcaFirma.Text) ? textBoxNadawcaFirma.Text : textBoxNadawcaNazwa.Text;
                odbiorca = !string.IsNullOrWhiteSpace(textBoxOdbiorcaFirma.Text) ? textBoxOdbiorcaFirma.Text : textBoxOdbiorcaNazwa.Text;
            }));

            string numerListu = await ZnajdzNumerListuPrzewozowegoAsync(nadawca, odbiorca);

            return numerListu;
        }

        private async Task<string> HandleZlecenieOdbioruAsync()
        {
            webView21.CoreWebView2.Navigate("https://online.dpd.com.pl/shipment/single-packages.go");
            await Task.Delay(5000);
            await WypelnijFormularzDpdAsync();

            await webView21.CoreWebView2.ExecuteScriptAsync("document.getElementById('CR-checkbox')?.click();");
            await Task.Delay(2000);

            await webView21.CoreWebView2.ExecuteScriptAsync("document.getElementById('save-cr__button')?.click();");
            await Task.Delay(5000);

            string nadawca = "", odbiorca = "";
            // POPRAWKA: Użycie System.Action
            this.Invoke(new System.Action(() => {
                nadawca = !string.IsNullOrWhiteSpace(textBoxNadawcaFirma.Text) ? textBoxNadawcaFirma.Text : textBoxNadawcaNazwa.Text;
                odbiorca = !string.IsNullOrWhiteSpace(textBoxOdbiorcaFirma.Text) ? textBoxOdbiorcaFirma.Text : textBoxOdbiorcaNazwa.Text;
            }));

            string numerListu = await ZnajdzNumerListuPrzewozowegoAsync(nadawca, odbiorca);
            return numerListu;
        }

        private async Task WypelnijFormularzDpdAsync()
        {
            string f1 = "", f2 = "", f3 = "", f4 = "", f5 = "", f6 = "", f7 = "", t1 = "", t2 = "", t3 = "", t4 = "", t5 = "", t6 = "", t7 = "", content = "", weight = "";

            // POPRAWKA: Użycie System.Action
            this.Invoke(new System.Action(() => {
                f1 = textBoxNadawcaFirma.Text; f2 = textBoxNadawcaNazwa.Text; f3 = textBoxNadawcaUlica.Text;
                f4 = textBoxNadawcaKodPocztowy.Text; f5 = textBoxNadawcaMiasto.Text; f6 = textBoxNadawcaTelefon.Text;
                f7 = textBoxNadawcaMail.Text;
                t1 = textBoxOdbiorcaFirma.Text; t2 = textBoxOdbiorcaNazwa.Text; t3 = textBoxOdbiorcaUlica.Text;
                t4 = textBoxOdbiorcaKodPocztowy.Text; t5 = textBoxOdbiorcaMiasto.Text; t6 = textBoxOdbiorcaTelefon.Text;
                t7 = string.IsNullOrEmpty(textBoxOdbiorcaMail.Text) ? "brak@email.pl" : textBoxOdbiorcaMail.Text;
                content = cowysylamy.Text; weight = waga.Text.Replace(",", ".");
            }));

            Func<string, string> escape = (s) => s.Replace("'", "\\'").Replace("\r", "").Replace("\n", "");

            string scriptNadawca = $@"
                window.fillFieldBySelector = function(selector, val) {{
                    const el = document.querySelector(selector);
                    if (el) {{
                        el.value = val;
                        el.dispatchEvent(new Event('input', {{ bubbles: true }}));
                        el.dispatchEvent(new Event('blur', {{ bubbles: true }}));
                    }}
                }};
                fillFieldBySelector('[formcontrolname=""company""]', '{escape(f1)}');
                fillFieldBySelector('[formcontrolname=""name""]', '{escape(f2)}');
                fillFieldBySelector('[formcontrolname=""street""]', '{escape(f3)}');
                fillFieldBySelector('[formcontrolname=""postalCode""]', '{escape(f4)}');
                fillFieldBySelector('[formcontrolname=""city""]', '{escape(f5)}');
                fillFieldBySelector('[formcontrolname=""phone""]', '{escape(f6)}');
                fillFieldBySelector('[formcontrolname=""email""]', '{escape(f7)}');
            ";
            await webView21.CoreWebView2.ExecuteScriptAsync(scriptNadawca);
            await Task.Delay(1500);

            string scriptOdbiorca = $@"
                fillFieldBySelector('#receiver-company__input', '{escape(t1)}');
                fillFieldBySelector('#receiver-name__input', '{escape(t2)}');
                fillFieldBySelector('#receiver-street__input', '{escape(t3)}');
                fillFieldBySelector('#receiver-postal-code__input', '{escape(t4)}');
            ";
            await webView21.CoreWebView2.ExecuteScriptAsync(scriptOdbiorca);
            await Task.Delay(2000);

            string scriptOdbiorcaReszta = $@"
                fillFieldBySelector('#receiver-city__input', '{escape(t5)}');
                fillFieldBySelector('#receiver-telephone__input', '{escape(t6)}');
                fillFieldBySelector('#receiver-email__input', '{escape(t7)}');
                fillFieldBySelector('[formcontrolname=""contents""]', '{escape(content)}');
                fillFieldBySelector('[formcontrolname=""weight""]', '{escape(weight)}');
            ";
            await webView21.CoreWebView2.ExecuteScriptAsync(scriptOdbiorcaReszta);
            await Task.Delay(1000);
        }

        #endregion

        #region --- Logika Danych i Wyszukiwania ---

        private async Task PobierzDaneZgloszeniaAsync()
        {
            string query = @"
                SELECT z.allegroDisputeId, ad.AllegroAccountId, p.Kategoria, k.Email, k.Telefon
                FROM Zgloszenia z 
                LEFT JOIN Produkty p ON z.ProduktID = p.Id
                LEFT JOIN Klienci k ON z.KlientID = k.Id
                LEFT JOIN AllegroDisputes ad ON z.allegroDisputeId = ad.DisputeId
                WHERE z.NrZgloszenia = @nrZgloszenia";

            var dt = await _dbService.GetDataTableAsync(query, new MySqlParameter("@nrZgloszenia", this.nrZgloszenia));

            if (dt.Rows.Count > 0)
            {
                var row = dt.Rows[0];
                _allegroDisputeId = row["allegroDisputeId"]?.ToString();
                _allegroAccountId = row["AllegroAccountId"] == DBNull.Value ? 0 : Convert.ToInt32(row["AllegroAccountId"]);
                _kategoriaProduktu = row["Kategoria"]?.ToString() ?? "Brak Kategorii";
                _clientEmail = row["Email"]?.ToString();
                _clientPhone = row["Telefon"]?.ToString();
            }
        }

        private async Task<Dictionary<string, string>> PobierzDaneLogowaniaDpdAsync()
        {
            var daneLogowania = new Dictionary<string, string>();
            string query = "SELECT Klucz, WartoscZaszyfrowana FROM Ustawienia WHERE Klucz IN ('KlientDPD', 'LoginDPD', 'haslodpd')";
            var dt = await _dbService.GetDataTableAsync(query);

            foreach (DataRow row in dt.Rows)
            {
                string klucz = row["Klucz"].ToString();
                string wartosc = row["WartoscZaszyfrowana"].ToString();
                switch (klucz)
                {
                    case "KlientDPD": daneLogowania["username"] = wartosc; break;
                    case "LoginDPD": daneLogowania["customerId"] = wartosc; break;
                    case "haslodpd": daneLogowania["password"] = wartosc; break;
                }
            }

            if (daneLogowania.Count < 3)
            {
                SafeShowMessage("Nie udało się pobrać kompletnych danych logowania DPD z bazy danych. Sprawdź tabelę 'Ustawienia'.", "Błąd Konfiguracji", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return null;
            }
            return daneLogowania;
        }

        private void FiltrujListe(string fraza, DataGridView dgv)
        {
            if (string.IsNullOrWhiteSpace(fraza))
            {
                dgv.Visible = false;
                dgv.DataSource = null;
                return;
            }

            var keywords = fraza.Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries).Select(k => k.ToLowerInvariant()).ToArray();
            var pasujacyKlienci = listaKlientow
                .Where(k => keywords.All(kw => k.DaneDoWyswietlenia.ToLowerInvariant().Contains(kw)))
                .Select(k => new { k.Id, k.DaneDoWyswietlenia, KlientObj = k })
                .Take(10)
                .ToList();

            if (pasujacyKlienci.Any())
            {
                dgv.DataSource = pasujacyKlienci;
                if (dgv.Columns["Id"] != null) dgv.Columns["Id"].Visible = false;
                if (dgv.Columns["KlientObj"] != null) dgv.Columns["KlientObj"].Visible = false;
                if (dgv.Columns["DaneDoWyswietlenia"] != null)
                {
                    dgv.Columns["DaneDoWyswietlenia"].HeaderText = "Klient";
                    dgv.Columns["DaneDoWyswietlenia"].AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;
                }
                dgv.Visible = true;
                dgv.BringToFront();
            }
            else
            {
                dgv.Visible = false;
            }
        }

        private async Task<List<Klient>> WczytajKlientowAsync()
        {
            var klienci = new List<Klient>();
            var dt = await _dbService.GetDataTableAsync("SELECT * FROM Klienci");
            foreach (System.Data.DataRow row in dt.Rows)
            {
                klienci.Add(MapToKlient(row));
            }
            return klienci;
        }

        private Klient MapToKlient(DataRow row)
        {
            var klient = new Klient
            {
                Id = Convert.ToInt32(row["Id"]),
                ImieNazwisko = row["ImieNazwisko"].ToString(),
                NazwaFirmy = row["NazwaFirmy"].ToString(),
                Ulica = row["Ulica"].ToString(),
                KodPocztowy = row["KodPocztowy"].ToString(),
                Miejscowosc = row["Miejscowosc"].ToString(),
                Email = row["Email"].ToString(),
                Telefon = row["Telefon"].ToString(),
                NIP = row["NIP"].ToString()
            };
            var parts = new[] { klient.ImieNazwisko, klient.NazwaFirmy, klient.NIP, klient.Email }.Where(s => !string.IsNullOrWhiteSpace(s));
            klient.DaneDoWyswietlenia = string.Join(" | ", parts);
            return klient;
        }

        private void WypelnijPola(Klient klient, bool czyNadawca)
        {
            if (czyNadawca)
            {
                _nadawcaId = klient.Id;
                textBoxNadawcaNazwa.Text = klient.ImieNazwisko;
                textBoxNadawcaFirma.Text = klient.NazwaFirmy;
                textBoxNadawcaUlica.Text = klient.Ulica;
                textBoxNadawcaKodPocztowy.Text = klient.KodPocztowy;
                textBoxNadawcaMiasto.Text = klient.Miejscowosc;
                textBoxNadawcaMail.Text = klient.Email;
                textBoxNadawcaTelefon.Text = klient.Telefon;
            }
            else
            {
                _odbiorcaId = klient.Id;
                textBoxOdbiorcaNazwa.Text = klient.ImieNazwisko;
                textBoxOdbiorcaFirma.Text = klient.NazwaFirmy;
                textBoxOdbiorcaUlica.Text = klient.Ulica;
                textBoxOdbiorcaKodPocztowy.Text = klient.KodPocztowy;
                textBoxOdbiorcaMiasto.Text = klient.Miejscowosc;
                textBoxOdbiorcaMail.Text = klient.Email;
                textBoxOdbiorcaTelefon.Text = klient.Telefon;
            }
        }

        private void WybierzKlientaZListy(DataGridView dgv, bool czyNadawca)
        {
            if (dgv.CurrentRow != null && dgv.CurrentRow.DataBoundItem != null)
            {
                var dataBoundItem = dgv.CurrentRow.DataBoundItem;
                Klient wybranyKlient = (Klient)dataBoundItem.GetType().GetProperty("KlientObj").GetValue(dataBoundItem, null);

                if (wybranyKlient != null)
                {
                    WypelnijPola(wybranyKlient, czyNadawca);
                    dgv.Visible = false;
                    dgv.DataSource = null;

                    if (czyNadawca)
                    {
                        textBoxNadawcaSearch.Clear();
                    }
                    else
                    {
                        textBoxOdbiorcaSearch.Clear();
                    }
                }
            }
        }

        private async Task<string> ZnajdzNumerListuPrzewozowegoAsync(string nadawca, string odbiorca)
        {
            webView21.CoreWebView2.Navigate("https://online.dpd.com.pl/packages-history.go");
            await Task.Delay(5000);

            string script = $@"
            (function() {{
                const clean = (text) => text ? text.trim().toLowerCase() : '';
                const szukanyNadawca = clean('{nadawca.Replace("'", "\\'")}');
                const szukanyOdbiorca = clean('{odbiorca.Replace("'", "\\'")}');
                const tabela = document.getElementById('PKGS.HISTORY.UNIQUE.FLAG');
                if (!tabela) return '';

                const wiersze = tabela.querySelectorAll('tbody.tbody tr');
                for (let i = 0; i < wiersze.length; i++) {{
                    const wiersz = wiersze[i];
                    const komorki = wiersz.cells;
                    if (komorki.length < 8) continue;
                    const odbiorcaFirma = clean(komorki[3].innerText);
                    const odbiorcaNazwisko = clean(komorki[4].innerText);
                    const nadawcaFirma = clean(komorki[6].innerText);
                    const nadawcaNazwisko = clean(komorki[7].innerText);
                    const czyPasujeOdbiorca = odbiorcaFirma.includes(szukanyOdbiorca) || odbiorcaNazwisko.includes(szukanyOdbiorca);
                    const czyPasujeNadawca = nadawcaFirma.includes(szukanyNadawca) || nadawcaNazwisko.includes(szukanyNadawca);
                    if (czyPasujeNadawca && czyPasujeOdbiorca) {{
                        const link = komorki[0].querySelector('a');
                        if (link) return link.innerText.trim();
                    }}
                }}
                return '';
            }})();";

            string wynik = await webView21.CoreWebView2.ExecuteScriptAsync(script);

            if (string.IsNullOrEmpty(wynik) || wynik.ToLower() == "null")
            {
                return string.Empty;
            }
            return wynik.Trim('"');
        }

        private bool WalidujDane(bool czyNadawca)
        {
            string typ = czyNadawca ? "Nadawca" : "Odbiorca";
            TextBox txtNazwa = czyNadawca ? textBoxNadawcaNazwa : textBoxOdbiorcaNazwa;
            TextBox txtFirma = czyNadawca ? textBoxNadawcaFirma : textBoxOdbiorcaFirma;
            TextBox txtUlica = czyNadawca ? textBoxNadawcaUlica : textBoxOdbiorcaUlica;
            TextBox txtKod = czyNadawca ? textBoxNadawcaKodPocztowy : textBoxOdbiorcaKodPocztowy;
            TextBox txtMail = czyNadawca ? textBoxNadawcaMail : textBoxOdbiorcaMail;

            if (string.IsNullOrWhiteSpace(txtNazwa.Text) && string.IsNullOrWhiteSpace(txtFirma.Text))
            { MessageBox.Show($"({typ}) Musisz podać Imię i Nazwisko lub Nazwę firmy.", "Błąd Walidacji"); return false; }
            if (!Regex.IsMatch(txtUlica.Text, @"\d"))
            { MessageBox.Show($"({typ}) Adres w polu 'Ulica' musi zawierać numer domu/mieszkania.", "Błąd Walidacji"); return false; }
            if (!Regex.IsMatch(txtKod.Text, @"^\d{2}-?\d{3}$"))
            { MessageBox.Show($"({typ}) Kod pocztowy musi być w formacie XX-XXX lub XXXXX.", "Błąd Walidacji"); return false; }
            if (!string.IsNullOrWhiteSpace(txtMail.Text) && !Regex.IsMatch(txtMail.Text, @"^[^@\s]+@[^@\s]+\.[^@\s]+$"))
            { MessageBox.Show($"({typ}) Podany adres e-mail jest nieprawidłowy.", "Błąd Walidacji"); return false; }
            return true;
        }

        #endregion

        #region --- Logika Bazy Danych ---

        private async Task DodajPrzesylkeDoBazyAsync(string numerListu, int nadawcaId, int odbiorcaId, bool isZlecenieOdbioru, bool isZwrotna)
        {
            string query = @"
                INSERT INTO Przesylki (NumerListu, NadawcaId, OdbiorcaId, NrZgloszenia, OstatniStatus, DataNadania, IsZlecenieOdbioru, IsPaczkaZwrotna)
                VALUES (@NumerListu, @NadawcaId, @OdbiorcaId, @NrZgloszenia, 'Zamówiony', @DataNadania, @IsZlecenieOdbioru, @IsPaczkaZwrotna)";

            var parameters = new[]
            {
                new MySqlParameter("@NumerListu", numerListu),
                new MySqlParameter("@NadawcaId", nadawcaId),
                new MySqlParameter("@OdbiorcaId", odbiorcaId),
                new MySqlParameter("@NrZgloszenia", this.nrZgloszenia),
                new MySqlParameter("@DataNadania", DateTime.Now),
                new MySqlParameter("@IsZlecenieOdbioru", isZlecenieOdbioru ? 1 : 0),
                new MySqlParameter("@IsPaczkaZwrotna", isZwrotna ? 1 : 0)
            };
            await _dbService.ExecuteNonQueryAsync(query, parameters);
        }

        private async Task SaveNewPasswordAsync(string newPassword)
        {
            string query = "UPDATE Ustawienia SET WartoscZaszyfrowana = @pass WHERE Klucz = 'haslodpd'";
            await _dbService.ExecuteNonQueryAsync(query, new MySqlParameter("@pass", newPassword));
        }

        #endregion

        #region --- Metody Pomocnicze (UI) ---

        private Form CreateBusyForm()
        {
            var form = new Form
            {
                Text = "Pracuję...",
                ControlBox = false,
                FormBorderStyle = FormBorderStyle.FixedDialog,
                StartPosition = FormStartPosition.CenterParent,
                Size = new System.Drawing.Size(400, 150)
            };
            var label = new Label
            {
                Text = "Ja teraz ciężko pracuję, byś Ty mógł odetchnąć.\nZamawiam kuriera, to może potrwać minutę...",
                Dock = DockStyle.Top,
                TextAlign = System.Drawing.ContentAlignment.MiddleCenter,
                Font = new System.Drawing.Font("Segoe UI", 10F, System.Drawing.FontStyle.Bold),
                Padding = new Padding(10),
                Height = 70
            };
            var progressBar = new ProgressBar
            {
                Style = ProgressBarStyle.Marquee,
                Dock = DockStyle.Bottom,
                Height = 30
            };
            form.Controls.Add(label);
            form.Controls.Add(progressBar);
            return form;
        }

        private string ShowManualTrackingNumberDialog()
        {
            string result = null;
            if (this.InvokeRequired)
            {
                // POPRAWKA: Użycie System.Action
                this.Invoke(new System.Action(() => result = ShowManualTrackingNumberDialogInternal()));
            }
            else
            {
                result = ShowManualTrackingNumberDialogInternal();
            }
            return result;
        }

        private string ShowManualTrackingNumberDialogInternal()
        {
            using (var form = new Form())
            {
                var label = new Label();
                var textBox = new TextBox();
                var buttonOk = new Button();
                var buttonCancel = new Button();

                form.Text = "Wprowadź numer listu ręcznie";
                form.ClientSize = new System.Drawing.Size(380, 130);
                form.StartPosition = FormStartPosition.CenterParent;
                form.FormBorderStyle = FormBorderStyle.FixedDialog;
                form.MaximizeBox = false;
                form.MinimizeBox = false;

                label.Text = "Nie udało się pobrać numeru automatycznie.\nWprowadź numer listu przewozowego:";
                label.Location = new System.Drawing.Point(12, 9);
                label.Size = new System.Drawing.Size(356, 40);

                textBox.Location = new System.Drawing.Point(15, 55);
                textBox.Size = new System.Drawing.Size(350, 22);

                buttonOk.Text = "Zatwierdź";
                buttonOk.DialogResult = DialogResult.OK;
                buttonOk.Location = new System.Drawing.Point(265, 90);

                buttonCancel.Text = "Anuluj";
                buttonCancel.DialogResult = DialogResult.Cancel;
                buttonCancel.Location = new System.Drawing.Point(180, 90);

                form.Controls.AddRange(new Control[] { label, textBox, buttonOk, buttonCancel });
                form.AcceptButton = buttonOk;
                form.CancelButton = buttonCancel;

                if (form.ShowDialog(this) == DialogResult.OK)
                {
                    return textBox.Text.Trim();
                }
                return null;
            }
        }

        private string ShowNewPasswordDialog()
        {
            using (var form = new Form())
            {
                var label = new Label();
                var textBox = new TextBox();
                var buttonOk = new Button();
                var buttonCancel = new Button();

                form.Text = "Błędne hasło DPD";
                form.ClientSize = new System.Drawing.Size(380, 130);
                form.StartPosition = FormStartPosition.CenterParent;
                form.FormBorderStyle = FormBorderStyle.FixedDialog;
                form.MaximizeBox = false;
                form.MinimizeBox = false;

                label.Text = "Wykryto błędne hasło. Wprowadź nowe hasło,\nktóre zostanie zapisane w bazie danych:";
                label.Location = new System.Drawing.Point(12, 9);
                label.Size = new System.Drawing.Size(356, 40);

                textBox.Location = new System.Drawing.Point(15, 55);
                textBox.Size = new System.Drawing.Size(350, 22);
                textBox.PasswordChar = '*';

                buttonOk.Text = "Zapisz i ponów";
                buttonOk.DialogResult = DialogResult.OK;
                buttonOk.Location = new System.Drawing.Point(235, 90);
                buttonOk.Size = new System.Drawing.Size(130, 28);

                buttonCancel.Text = "Anuluj";
                buttonCancel.DialogResult = DialogResult.Cancel;
                buttonCancel.Location = new System.Drawing.Point(150, 90);
                buttonCancel.Size = new System.Drawing.Size(80, 28);

                form.Controls.AddRange(new Control[] { label, textBox, buttonOk, buttonCancel });
                form.AcceptButton = buttonOk;
                form.CancelButton = buttonCancel;

                if (form.ShowDialog(this) == DialogResult.OK)
                {
                    return textBox.Text;
                }
                return null;
            }
        }

        #endregion

        #region --- Logika Powiadomień (zmieniona) ---

        private async Task HandleNotifications(string trackingNumber)
        {
            bool isEmailAvailable = !string.IsNullOrEmpty(_clientEmail);
            bool isSmsAvailable = !string.IsNullOrEmpty(_clientPhone);
            bool isAllegroAvailable = !string.IsNullOrEmpty(_allegroDisputeId);

            if (!isEmailAvailable && !isSmsAvailable && !isAllegroAvailable) return;

            var result = MessageBox.Show(this, "Czy chcesz wysłać powiadomienie do klienta o zamówieniu kuriera?", "Powiadomienie", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
            if (result == DialogResult.No) return;

            string trackingLink = $"https://tracktrace.dpd.com.pl/parcelDetails?typ=1&p1={trackingNumber}";
            var messageBuilder = new StringBuilder();

            if (zlecenie.Checked)
            {
                messageBuilder.AppendLine($"Informujemy, że w zgłoszeniu nr {nrZgloszenia} została zamówiona przesyłka do odbioru od Ciebie.");
                messageBuilder.AppendLine($"Numer listu przewozowego: {trackingNumber}");
                messageBuilder.AppendLine($"\nMożesz ją śledzić pod adresem:\n{trackingLink}");
                messageBuilder.AppendLine("\nKurier będzie miał przy sobie list przewozowy (etykietę) i podjedzie po odbiór następnego dnia roboczego.");
                messageBuilder.AppendLine();

                if (_kategoriaProduktu != null && _kategoriaProduktu.ToLower().Contains("lodówka"))
                {
                    messageBuilder.AppendLine("Proszę o przygotowanie lodówki do odbioru przez kuriera oraz odpowiednie oznaczenie góry i dołu paczki.");
                    messageBuilder.AppendLine("Jest to niezwykle ważne, ponieważ przenoszenie urządzenia w nieprawidłowej pozycji może doprowadzić do wycieku oleju do układu chłodniczego, co w konsekwencji może uniemożliwić naprawę.");
                    messageBuilder.AppendLine("\nProszę również o odpowiednie zabezpieczenie wnętrza lodówki, najlepiej wykorzystując oryginalne opakowanie wraz ze styropianowymi osłonami.");
                    messageBuilder.AppendLine("W przypadku braku oryginalnych materiałów należy upewnić się, że urządzenie jest solidnie unieruchomione, a wszystkie luźne elementy, takie jak półki czy szuflady, są zabezpieczone przed przemieszczaniem się.");
                }
                else
                {
                    messageBuilder.AppendLine("Proszę pamiętać o odpowiednim i bezpiecznym zapakowaniu przesyłki przed odbiorem.");
                    messageBuilder.AppendLine("Urządzenie powinno być zabezpieczone w taki sposób, aby zminimalizować ryzyko uszkodzeń podczas transportu.");
                    messageBuilder.AppendLine("Rekomendujemy wykorzystanie oryginalnego opakowania lub solidnego kartonu z wypełnieniem, które zabezpieczy produkt przed przemieszczaniem się.");
                    messageBuilder.AppendLine("Proszę upewnić się, że paczka jest prawidłowo oznaczona oraz gotowa do transportu w momencie przyjazdu kuriera.");
                }
            }
            else
            {
                messageBuilder.AppendLine($"Informujemy, że w zgłoszeniu nr {nrZgloszenia} została nadana do Ciebie przesyłka.");
                messageBuilder.AppendLine($"Numer listu przewozowego: {trackingNumber}");
                messageBuilder.AppendLine($"\nMożesz ją śledzić pod adresem:\n{trackingLink}");
                messageBuilder.AppendLine("\nProszę pamiętać o sprawdzeniu stanu przesyłki przy odbiorze, a w razie problemów sporządzić protokół szkody z kurierem.");
            }

            string defaultMessage = messageBuilder.ToString();
            string messageToSend = ShowEditMessageDialog("Edytuj treść powiadomienia", defaultMessage);

            if (messageToSend == null) return;

            using (var form4 = new Form4(nrZgloszenia, messageToSend))
            {
                form4.ShowDialog(this);
            }

            var dziennik = new DziennikLogger();
            await dziennik.DodajAsync(Program.fullName, "Otworzono formularz powiadomień kuriera (Form4).", nrZgloszenia);
        }

        private Form CreateNotificationForm(bool email, bool sms, bool allegro)
        {
            var form = new Form
            {
                Text = "Wybierz kanał powiadomienia",
                StartPosition = FormStartPosition.CenterParent,
                FormBorderStyle = FormBorderStyle.FixedDialog,
                ClientSize = new System.Drawing.Size(450, 150),
                MaximizeBox = false,
                MinimizeBox = false
            };

            var panel = new FlowLayoutPanel { Dock = DockStyle.Fill, Padding = new Padding(15), WrapContents = false, FlowDirection = FlowDirection.TopDown };

            var lblInfo = new Label { Text = "Zaznacz, którymi kanałami chcesz poinformować klienta:", AutoSize = true, Margin = new Padding(0, 0, 0, 10) };
            panel.Controls.Add(lblInfo);

            var chkEmail = new CheckBox { Name = "chkEmail", Text = $"Mailowo (na: {_clientEmail})", AutoSize = true, Visible = email, Checked = email };
            var chkSms = new CheckBox { Name = "chkSms", Text = $"SMS (na: {_clientPhone})", AutoSize = true, Visible = sms };
            var chkAllegro = new CheckBox { Name = "chkAllegro", Text = "W dyskusji na Allegro", AutoSize = true, Visible = allegro };

            panel.Controls.Add(chkEmail);
            panel.Controls.Add(chkSms);
            panel.Controls.Add(chkAllegro);

            var btnOk = new Button { Text = "Dalej", DialogResult = DialogResult.OK, Dock = DockStyle.Right };
            var btnCancel = new Button { Text = "Anuluj", DialogResult = DialogResult.Cancel, Dock = DockStyle.Right };

            var buttonPanel = new FlowLayoutPanel { Dock = DockStyle.Bottom, FlowDirection = FlowDirection.RightToLeft, Height = 40, Padding = new Padding(5) };
            buttonPanel.Controls.Add(btnOk);
            buttonPanel.Controls.Add(btnCancel);

            form.Controls.Add(panel);
            form.Controls.Add(buttonPanel);
            form.AcceptButton = btnOk;
            form.CancelButton = btnCancel;

            return form;
        }

        private string ShowEditMessageDialog(string title, string defaultMessage)
        {
            string result = null;

            if (this.InvokeRequired)
            {
                // POPRAWKA: Użycie System.Action
                this.Invoke(new System.Action(() =>
                {
                    result = ShowEditMessageDialogInternal(title, defaultMessage);
                }));
            }
            else
            {
                result = ShowEditMessageDialogInternal(title, defaultMessage);
            }
            return result;
        }

        private string ShowEditMessageDialogInternal(string title, string defaultMessage)
        {
            using (var form = new Form())
            {
                var textBox = new TextBox();
                var buttonOk = new Button();
                var buttonCancel = new Button();
                var panelButtons = new FlowLayoutPanel();

                form.Text = title;
                form.ClientSize = new System.Drawing.Size(500, 350);
                form.StartPosition = FormStartPosition.CenterParent;
                form.FormBorderStyle = FormBorderStyle.Sizable;
                form.MaximizeBox = false;
                form.MinimizeBox = false;

                textBox.Text = defaultMessage;
                textBox.Multiline = true;
                textBox.AcceptsReturn = true;
                textBox.Dock = DockStyle.Fill;
                textBox.ScrollBars = ScrollBars.Vertical;
                textBox.Font = new Font("Segoe UI", 9.5F);

                buttonOk.Text = "Wyślij";
                buttonOk.DialogResult = DialogResult.OK;
                buttonOk.AutoSize = true;

                buttonCancel.Text = "Anuluj";
                buttonCancel.DialogResult = DialogResult.Cancel;
                buttonCancel.AutoSize = true;

                panelButtons.Dock = DockStyle.Bottom;
                panelButtons.Height = 40;
                panelButtons.FlowDirection = FlowDirection.RightToLeft;
                panelButtons.Padding = new Padding(0, 5, 5, 0);

                panelButtons.Controls.Add(buttonOk);
                panelButtons.Controls.Add(buttonCancel);

                form.Controls.Add(textBox);
                form.Controls.Add(panelButtons);
                form.CancelButton = buttonCancel;

                return form.ShowDialog(this) == DialogResult.OK ? textBox.Text : null;
            }
        }

        private async Task SendEmail(string message)
        {
            try
            {
                var outlookApp = new Microsoft.Office.Interop.Outlook.Application();
                if (outlookApp.Session.Accounts.Count == 0)
                {
                    SafeShowMessage("Nie znaleziono żadnych kont e-mail w programie Outlook.", "Błąd Outlook", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                // Ponieważ okno wyboru konta jest w UI, musimy to wywołać w Invoke
                Account selectedAccount = null;
                // POPRAWKA: Użycie System.Action
                this.Invoke(new System.Action(() => selectedAccount = ChooseOutlookAccount(outlookApp.Session.Accounts)));

                if (selectedAccount == null) return;

                MailItem mail = (MailItem)outlookApp.CreateItem(OlItemType.olMailItem);
                mail.To = _clientEmail;
                mail.Subject = $"Aktualizacja w zgłoszeniu nr {nrZgloszenia}";
                mail.Body = message;
                mail.SendUsingAccount = selectedAccount;
                mail.Display(false);
            }
            catch (System.Exception ex)
            {
                SafeShowMessage($"[BŁĄD E-MAIL]\nNie udało się utworzyć wiadomości w programie Outlook.\n\nSzczegóły: {ex.Message}", "Błąd wysyłki e-mail", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            await Task.CompletedTask;
        }

        private Account ChooseOutlookAccount(Accounts accounts)
        {
            if (accounts.Count == 1) return accounts[1];

            using (var form = new Form())
            {
                var listBox = new ListBox();
                var buttonOk = new Button();
                var labelInfo = new Label();

                form.Text = "Wybierz konto e-mail";
                form.StartPosition = FormStartPosition.CenterParent;
                form.FormBorderStyle = FormBorderStyle.FixedDialog;
                form.ClientSize = new System.Drawing.Size(400, 220);
                form.MaximizeBox = false;
                form.MinimizeBox = false;

                labelInfo.Text = "Wybierz konto, z którego chcesz wysłać wiadomość:";
                labelInfo.Dock = DockStyle.Top;
                labelInfo.Padding = new Padding(10);
                labelInfo.AutoSize = true;

                listBox.Dock = DockStyle.Fill;
                foreach (Account account in accounts)
                {
                    listBox.Items.Add(account.DisplayName);
                }
                listBox.SelectedIndex = 0;

                buttonOk.Text = "OK";
                buttonOk.DialogResult = DialogResult.OK;
                buttonOk.Dock = DockStyle.Bottom;
                buttonOk.Padding = new Padding(5);

                form.Controls.Add(listBox);
                form.Controls.Add(labelInfo);
                form.Controls.Add(buttonOk);
                form.AcceptButton = buttonOk;

                if (form.ShowDialog() == DialogResult.OK)
                {
                    string selectedName = listBox.SelectedItem.ToString();
                    foreach (Account account in accounts)
                    {
                        if (account.DisplayName == selectedName) return account;
                    }
                }
            }
            return null;
        }

        private async Task SendSms(string message)
        {
            if (string.IsNullOrWhiteSpace(_clientPhone))
            {
                SafeShowMessage("Brak numeru telefonu dla tego klienta.", "Błąd", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                await Task.CompletedTask;
                return;
            }

            try
            {
                string encodedMessage = Uri.EscapeDataString(message);
                string phoneNumberDigitsOnly = new string(_clientPhone.Where(char.IsDigit).ToArray());
                string uri = $"sms:{phoneNumberDigitsOnly}?body={encodedMessage}";
                Process.Start(new ProcessStartInfo(uri) { UseShellExecute = true });
            }
            catch (System.Exception ex)
            {
                SafeShowMessage("Nie udało się uruchomić aplikacji do wysyłania SMS. Upewnij się, że 'Łącze z telefonem' jest skonfigurowane.\n\n" + $"Błąd: {ex.Message}", "Błąd automatyzacji", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            await Task.CompletedTask;
        }

        private async Task SendAllegroMessage(string message)
        {
            if (string.IsNullOrEmpty(_allegroDisputeId)) return;

            try
            {
                using (var con = Database.GetNewOpenConnection())
                {
                    var apiClient = await DatabaseHelper.GetApiClientForAccountAsync(_allegroAccountId, con);
                    if (apiClient == null)
                    {
                        SafeShowMessage("Nie udało się uzyskać klienta API dla konta Allegro.", "Błąd");
                        return;
                    }

                    var request = new NewMessageRequest { Text = message };
                    await apiClient.SendMessageAsync(_allegroDisputeId, request);
                    SafeShowMessage("Wiadomość została wysłana w dyskusji Allegro.", "Sukces");
                    await new DziennikLogger().DodajAsync(Program.fullName, "Wysłano wiadomość w dyskusji Allegro.", nrZgloszenia);
                }
            }
            catch (System.Exception ex)
            {
                SafeShowMessage($"Błąd podczas wysyłania wiadomości na Allegro: {ex.Message}", "Błąd krytyczny");
            }
        }

        #endregion

        // Puste metody obsługi zdarzeń (wymagane przez Designer)
        private void textBoxNadawcaTelefon_TextChanged(object sender, EventArgs e) { }
        private void labelNadawcaTelefon_Click(object sender, EventArgs e) { }
        private void textBoxNadawcaMail_TextChanged(object sender, EventArgs e) { }
        private void label3_Click(object sender, EventArgs e) { }
    
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
            catch (System.Exception ex)
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
