using System;
using System.Collections.Generic;
using MySql.Data.MySqlClient;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using DeepL;
using Microsoft.Web.WebView2.Core;
using PdfSharp.Pdf;
using PdfSharp.Fonts;

namespace Reklamacje_Dane
{
    public partial class FormReportToManufacturer : Form
    {
        #region Zmienne i Pola
        private readonly string _nrZgloszenia;
        private readonly string _producent;
        private string _adresEmailProducenta;
        private readonly string _complaintFolderPath;
        private string _wymaganiaProducenta;
        private bool _czyProducentWymagaFormularza;
        private string _deepLApiKey;

        // NOWOŚĆ: Zmienna na język producenta
        private string _jezykProducenta = "EN"; // Domyślnie angielski

        // Dane z bazy
        private string _dbKodProduktu;
        private string _dbOpisUsterki;
        private string _dbNumerFaktury;
        private DateTime? _dbDataSprzedazy;
        private string _dbNrSeryjny;
        private int _dbKlientIdInt;
        private int _dbZgloszenieIdInt;

        private Dictionary<string, string> _placeholders = new Dictionary<string, string>();

        // Przeglądarka
        private Microsoft.Web.WebView2.WinForms.WebView2 webView;
        #endregion

        public FormReportToManufacturer(string nrZgloszenia, string producent, string adresEmail)
        {
            GlobalFontSettings.FontResolver = new PdfFontResolver();
            _nrZgloszenia = nrZgloszenia;
            _producent = producent;
            _adresEmailProducenta = adresEmail;

            string appDir = AppDomain.CurrentDomain.BaseDirectory;
            _complaintFolderPath = Path.Combine(appDir, "Dane", nrZgloszenia.Replace('/', '.'));
            if (!Directory.Exists(_complaintFolderPath)) Directory.CreateDirectory(_complaintFolderPath);

            InitializeComponent();
            InitializeWebView();

            this.Load += Form_Load;
        

            // Włącz sprawdzanie pisowni dla wszystkich TextBoxów
            EnableSpellCheckOnAllTextBoxes();
        }

        private void InitializeWebView()
        {
            webView = new Microsoft.Web.WebView2.WinForms.WebView2();
            webView.Dock = DockStyle.Fill;
            webView.Visible = true;
            if (pnlWebViewContainer != null)
            {
                pnlWebViewContainer.Controls.Add(webView);
                webView.BringToFront();
            }
        }

        private async void Form_Load(object sender, EventArgs e)
        {
            // Usunięto kod modyfikujący Splitter w zdarzeniu Shown, 
            // ponieważ jest to teraz poprawnie ustawione w InitializeComponent

            await LoadAccountsAsync();
            await LoadComplaintDataAsync();
            LoadFiles();

            if (_czyProducentWymagaFormularza) CreateManufacturerTabs();

            // Wypełnienie danych
            if (txtEdytowalnyKod != null) txtEdytowalnyKod.Text = _dbKodProduktu;
            if (txtEdytowalnyOpis != null) txtEdytowalnyOpis.Text = _dbOpisUsterki;
            if (txtEdytowalnaFaktura != null) txtEdytowalnaFaktura.Text = _dbNumerFaktury;
            if (txtEdytowalnySN != null) txtEdytowalnySN.Text = _dbNrSeryjny;
            if (dtpEdytowalnaData != null && _dbDataSprzedazy.HasValue) dtpEdytowalnaData.Value = _dbDataSprzedazy.Value;

            if (webView != null) await webView.EnsureCoreWebView2Async();

            BindEvents();
            ShowEmailPreviewMode();
        }

        private void BindEvents()
        {
            EventHandler refreshStandard = (s, e) => {
                if (!btnShowMailPreview.Visible) RefreshHtmlPreview();
            };

            chkIncSN.CheckedChanged += refreshStandard;
            chkIncFV.CheckedChanged += refreshStandard;
            chkIncData.CheckedChanged += refreshStandard;
            txtEdytowalnyKod.TextChanged += refreshStandard;
            txtEdytowalnyOpis.TextChanged += refreshStandard;
            txtEdytowalnaFaktura.TextChanged += refreshStandard;
            if (txtEdytowalnySN != null) txtEdytowalnySN.TextChanged += refreshStandard;
            txtEmailNotes.TextChanged += refreshStandard;
            dtpEdytowalnaData.ValueChanged += refreshStandard;

            listFiles.ItemChecked += (s, e) => { if (!btnShowMailPreview.Visible) RefreshHtmlPreview(); };
            listFiles.SelectedIndexChanged += ListFiles_SelectedIndexChanged;

            btnTranslate.Click += BtnTranslate_Click;
            btnSend.Click += BtnSend_Click;
            btnShowMailPreview.Click += (s, e) => ShowEmailPreviewMode();
        }

        #region Ładowanie Danych
        private async Task LoadAccountsAsync()
        {
            var accounts = new List<KontoPocztowe>();
            try
            {
                using (var con = DatabaseHelper.GetConnection())
                {
                    await con.OpenAsync();
                    string sql = "SELECT * FROM KontaPocztowe ORDER BY CzyDomyslne DESC, NazwaWyswietlana ASC";
                    using (var cmd = new MySqlCommand(sql, con))
                    using (var r = await cmd.ExecuteReaderAsync())
                    {
                        while (await r.ReadAsync())
                        {
                            accounts.Add(new KontoPocztowe
                            {
                                Id = Convert.ToInt32(r["Id"]),
                                NazwaWyswietlana = r["NazwaWyswietlana"].ToString(),
                                AdresEmail = r["AdresEmail"].ToString(),
                                Login = r["Login"].ToString(),
                                Haslo = r["Haslo"].ToString(),
                                SmtpHost = r["SmtpHost"].ToString(),
                                SmtpPort = Convert.ToInt32(r["SmtpPort"]),
                                SmtpSsl = Convert.ToBoolean(r["SmtpSsl"]),
                                Podpis = r["PodpisHtml"]?.ToString()
                            });
                        }
                    }
                }
                cmbNadawca.DataSource = accounts;
                cmbNadawca.DisplayMember = "NazwaWyswietlana";
                if (accounts.Count > 0) cmbNadawca.SelectedIndex = 0;
            }
            catch (Exception ex) { MessageBox.Show("Błąd kont: " + ex.Message); }
        }

        private async Task LoadComplaintDataAsync()
        {
            try
            {
                using (var con = DatabaseHelper.GetConnection())
                {
                    await con.OpenAsync();
                    // ZMIANA SQL: Pobieramy prod.Jezyk
                    string query = @"SELECT z.Id AS ZgloszenieID, z.NrZgloszenia, z.DataZgloszenia, z.OpisUsterki, z.NrSeryjny, z.DataZakupu, z.NrFaktury, z.KlientID,
                                     p.KodProducenta, p.NazwaKrotka,
                                     k.ImieNazwisko, k.Email AS KlientEmail, k.Telefon, k.Miejscowosc,
                                     prod.Formularz, prod.Wymagania, prod.KontaktMail, prod.Jezyk,
                                     (SELECT Wartosczaszyfrowana FROM Ustawienia WHERE klucz = 'DeepL') AS DeepLKey
                                     FROM Zgloszenia z
                                     LEFT JOIN Produkty p ON z.ProduktID = p.Id
                                     LEFT JOIN klienci k ON z.KlientID = k.Id
                                     LEFT JOIN Producenci prod ON TRIM(UPPER(prod.NazwaProducenta)) = TRIM(UPPER(@producent))
                                     WHERE z.NrZgloszenia = @nr";

                    using (var cmd = new MySqlCommand(query, con))
                    {
                        cmd.Parameters.AddWithValue("@nr", _nrZgloszenia);
                        cmd.Parameters.AddWithValue("@producent", _producent);
                        using (var r = await cmd.ExecuteReaderAsync())
                        {
                            if (await r.ReadAsync())
                            {
                                string reqForm = r["Formularz"]?.ToString() ?? "nie";
                                _czyProducentWymagaFormularza = reqForm.Equals("tak", StringComparison.OrdinalIgnoreCase);
                                _wymaganiaProducenta = r["Wymagania"]?.ToString();
                                _deepLApiKey = r["DeepLKey"]?.ToString();

                                // LOGIKA JĘZYKA
                                string lang = r["Jezyk"]?.ToString();
                                if (!string.IsNullOrWhiteSpace(lang) && (lang.ToUpper().Contains("PL") || lang.ToUpper().Contains("POL")))
                                {
                                    _jezykProducenta = "PL";
                                    // Jeśli PL, domyślnie nie musimy tłumaczyć
                                    if (chkZachowajPolski != null) chkZachowajPolski.Checked = true;
                                }
                                else
                                {
                                    _jezykProducenta = "EN";
                                }

                                string dbMail = r["KontaktMail"]?.ToString();
                                if (!string.IsNullOrWhiteSpace(dbMail)) _adresEmailProducenta = dbMail;

                                _dbKodProduktu = r["KodProducenta"]?.ToString();
                                _dbOpisUsterki = r["OpisUsterki"]?.ToString();
                                _dbNumerFaktury = r["NrFaktury"]?.ToString();
                                _dbNrSeryjny = r["NrSeryjny"]?.ToString();

                                int.TryParse(r["KlientID"]?.ToString(), out _dbKlientIdInt);
                                int.TryParse(r["ZgloszenieID"]?.ToString(), out _dbZgloszenieIdInt);

                                string ds = r["DataZakupu"]?.ToString();
                                if (DateTime.TryParse(ds, out DateTime d)) _dbDataSprzedazy = d;

                                _placeholders["{{NrZgloszenia}}"] = _nrZgloszenia;
                                _placeholders["{{NrSeryjny}}"] = _dbNrSeryjny;
                                _placeholders["{{KlientNazwa}}"] = r["ImieNazwisko"]?.ToString();
                                _placeholders["{{PracownikImie}}"] = Program.fullName;

                                txtEmailTo.Text = _adresEmailProducenta;
                                txtEmailSubject.Text = $"Complaint / Reklamacja: {_nrZgloszenia} ({_dbKodProduktu})";

                                var lblReq = this.Controls.Find("lblWymaganiaInfo", true).FirstOrDefault() as Label;
                                if (lblReq != null && !string.IsNullOrWhiteSpace(_wymaganiaProducenta))
                                    lblReq.Text = "Wymagania: " + _wymaganiaProducenta;
                            }
                        }
                    }
                }
            }
            catch (Exception ex) { MessageBox.Show("Błąd danych: " + ex.Message); }
        }

        private void LoadFiles()
        {
            listFiles.Items.Clear();
            if (!Directory.Exists(_complaintFolderPath)) return;
            foreach (string file in Directory.GetFiles(_complaintFolderPath))
            {
                listFiles.Items.Add(new ListViewItem { Text = Path.GetFileName(file), Tag = file, Checked = true });
            }
        }
        #endregion

        #region Generowanie Treści
        private string GenerateHtmlBody()
        {
            // Etykiety tabeli zależne od języka
            bool isPl = (_jezykProducenta == "PL");
            string lProd = isPl ? "Produkt:" : "Product:";
            string lSN = isPl ? "Nr Seryjny:" : "Serial No:";
            string lFV = isPl ? "Faktura:" : "Invoice:";
            string lDate = isPl ? "Data zakupu:" : "Date:";

            StringBuilder sbRows = new StringBuilder();
            sbRows.Append($"<tr><td style='padding:5px; font-weight:bold; width:150px; background:#f0f0f0;'>{lProd}</td><td>{txtEdytowalnyKod.Text}</td></tr>");

            if (chkIncSN.Checked) sbRows.Append($"<tr><td style='padding:5px; font-weight:bold; background:#f0f0f0;'>{lSN}</td><td>{txtEdytowalnySN.Text}</td></tr>");
            if (chkIncFV.Checked) sbRows.Append($"<tr><td style='padding:5px; font-weight:bold; background:#f0f0f0;'>{lFV}</td><td>{txtEdytowalnaFaktura.Text}</td></tr>");
            if (chkIncData.Checked) sbRows.Append($"<tr><td style='padding:5px; font-weight:bold; background:#f0f0f0;'>{lDate}</td><td>{dtpEdytowalnaData.Value:dd.MM.yyyy}</td></tr>");

            string opisHtml = txtEdytowalnyOpis.Text.Replace("\n", "<br/>");
            string uwagiHtml = txtEmailNotes.Text.Replace("\n", "<br/>");

            StringBuilder sbFiles = new StringBuilder("<ul>");
            foreach (ListViewItem item in listFiles.CheckedItems) sbFiles.Append($"<li>{item.Text}</li>");
            if (_czyProducentWymagaFormularza) sbFiles.Append($"<li>Zgłoszenie_{_producent}.pdf</li>");
            sbFiles.Append("</ul>");

            // Pobierz szablon (najpierw DB, potem fallback w zależności od języka)
            string html = GetTemplateFromDb() ?? GetModernFallbackTemplate();

            html = html.Replace("{{TabelaDanych}}", $"<table style='width:100%; border-collapse:collapse; border:1px solid #ddd; font-family:sans-serif; font-size:14px;'>{sbRows}</table>");
            html = html.Replace("{{NrZgloszenia}}", _nrZgloszenia);
            html = html.Replace("{{OpisUsterki}}", opisHtml);
            html = html.Replace("{{DodatkoweUwagi}}", uwagiHtml);
            html = html.Replace("{{ListaZalacznikow}}", sbFiles.ToString());
            html = html.Replace("{{PracownikImie}}", Program.fullName);

            return html;
        }

        private string GetModernFallbackTemplate()
        {
            // NOWOŚĆ: Logika wyboru języka i USUNIĘCIE sztywnego podpisu

            if (_jezykProducenta == "PL")
            {
                // Wersja POLSKA
                return @"<html>
                <body style='font-family:""Segoe UI"",Arial;color:#333;line-height:1.5;'>
                    <div style='border-bottom:3px solid #0078D7;padding-bottom:10px;margin-bottom:20px;'>
                        <h2 style='margin:0;color:#0078D7;'>Zgłoszenie Reklamacyjne</h2>
                        <p style='margin:5px 0 0 0;color:#666;'>Nr Zgłoszenia: <strong>{{NrZgloszenia}}</strong></p>
                    </div>
                    <p>Dzień dobry,</p>
                    <p>Zgłaszamy usterkę w następującym produkcie:</p>
                    {{TabelaDanych}}
                    <div style='margin-top:20px;'>
                        <h3 style='font-size:14px;border-bottom:1px solid #ccc;padding-bottom:5px;'>Opis Usterki:</h3>
                        <div style='background-color:#f9f9f9;padding:10px;border:1px solid #eee;'>{{OpisUsterki}}</div>
                    </div>
                    <div style='margin-top:15px;'>
                        <strong>Dodatkowe uwagi:</strong><br/>
                        {{DodatkoweUwagi}}
                    </div>
                    <div style='margin-top:15px;font-size:0.9em;color:#555;'>
                        <strong>Załączone pliki:</strong>
                        {{ListaZalacznikow}}
                    </div>
                    <br/><br/>
                    </body>
                </html>";
            }
            else
            {
                // Wersja ANGIELSKA (Default)
                return @"<html>
                <body style='font-family:""Segoe UI"",Arial;color:#333;line-height:1.5;'>
                    <div style='border-bottom:3px solid #0078D7;padding-bottom:10px;margin-bottom:20px;'>
                        <h2 style='margin:0;color:#0078D7;'>Warranty Claim / Zgłoszenie Reklamacyjne</h2>
                        <p style='margin:5px 0 0 0;color:#666;'>Ref No: <strong>{{NrZgloszenia}}</strong></p>
                    </div>
                    <p>Dear Sirs,</p>
                    <p>We are reporting a defect in the following product:</p>
                    {{TabelaDanych}}
                    <div style='margin-top:20px;'>
                        <h3 style='font-size:14px;border-bottom:1px solid #ccc;padding-bottom:5px;'>Fault Description:</h3>
                        <div style='background-color:#f9f9f9;padding:10px;border:1px solid #eee;'>{{OpisUsterki}}</div>
                    </div>
                    <div style='margin-top:15px;'>
                        <strong>Additional Notes:</strong><br/>
                        {{DodatkoweUwagi}}
                    </div>
                    <div style='margin-top:15px;font-size:0.9em;color:#555;'>
                        <strong>Attached files:</strong>
                        {{ListaZalacznikow}}
                    </div>
                    <br/><br/>
                    </body>
                </html>";
            }
        }

        private string GetTemplateFromDb()
        {
            try
            {
                using (var con = DatabaseHelper.GetConnection())
                {
                    con.Open();
                    string sql = "SELECT TrescRtf FROM SzablonyEmail WHERE Aktywny=1 AND (Nazwa = @n1 OR Nazwa = 'Domyślny') ORDER BY CASE WHEN Nazwa = @n1 THEN 0 ELSE 1 END LIMIT 1";
                    using (var cmd = new MySqlCommand(sql, con))
                    {
                        cmd.Parameters.AddWithValue("@n1", $"Zgłoszenie {_producent}");
                        var res = cmd.ExecuteScalar();
                        if (res != null) { try { return RtfPipe.Rtf.ToHtml(res.ToString()); } catch { return res.ToString(); } }
                    }
                }
            }
            catch { }
            return null;
        }
        #endregion

        #region WYSYŁKA i ZAPIS DO BAZY
        private async void BtnSend_Click(object sender, EventArgs e)
        {
            var konto = cmbNadawca.SelectedItem as KontoPocztowe;
            if (konto == null) { MessageBox.Show("Wybierz konto!"); return; }
            if (string.IsNullOrWhiteSpace(txtEmailTo.Text)) { MessageBox.Show("Brak odbiorcy!"); return; }

            btnSend.Enabled = false;
            try
            {
                // 1. PDF
                string pdfPath = null;
                if (_czyProducentWymagaFormularza) pdfPath = await GenerateManufacturerPdfAsync();

                // 2. Załączniki
                List<string> atts = new List<string>();
                if (pdfPath != null && File.Exists(pdfPath)) atts.Add(pdfPath);

                foreach (ListViewItem i in listFiles.CheckedItems)
                {
                    string f = i.Tag.ToString();
                    if (!f.Equals(pdfPath, StringComparison.OrdinalIgnoreCase)) atts.Add(f);
                }

                // 3. Podpis + HTML
                string html = GenerateHtmlBody();

                // TU JEST DODAWANIE PODPISU - i to jedyne miejsce (bo usunęliśmy ze sztywnego szablonu)
                if (!string.IsNullOrWhiteSpace(konto.Podpis))
                {
                    string podpis = konto.Podpis;
                    if (!podpis.Contains("<div") && !podpis.Contains("<span")) podpis = podpis.Replace("\n", "<br>");
                    podpis = podpis.Replace("{{PracownikImie}}", Program.fullName).Replace("{{PracownikEmail}}", konto.AdresEmail);
                    html += "<br><br>" + podpis;
                }

                // 4. Wysyłka
                var svc = new EmailService();
                await svc.WyslijEmailAsync(konto, txtEmailTo.Text, txtEmailSubject.Text, html, atts);

                // 5. Zmiana statusu
                using (var c = DatabaseHelper.GetConnection())
                {
                    c.Open();
                    new MySqlCommand($"UPDATE Zgloszenia SET StatusProducent='Zgłoszono do {_producent}', CzekamyNaDostawe='Czekamy' WHERE NrZgloszenia='{_nrZgloszenia}'", c).ExecuteNonQuery();
                }

                // 6. LOGOWANIE
                try
                {
                    var dziennik = new DziennikLogger();
                    await dziennik.DodajAsync(Program.fullName, $"Wysłano e-mail do {_producent}", _nrZgloszenia);
                    var dzialanie = new Dzialaniee();
                    dzialanie.DodajNoweDzialanie(_nrZgloszenia, Program.fullName, $"Wysłano e-mail do {_producent} (SMTP)");

                    await LogToCentrumKontaktu(txtEmailSubject.Text, html);

                    UpdateManager.NotifySubscribers();
                }
                catch { }

                MessageBox.Show("Wysłano!", "Sukces");
                Close();
            }
            catch (Exception ex) { MessageBox.Show("Błąd: " + ex.Message); }
            finally { btnSend.Enabled = true; }
        }

        private async Task LogToCentrumKontaktu(string temat, string trescHtml)
        {
            try
            {
                using (var con = DatabaseHelper.GetConnection())
                {
                    await con.OpenAsync();
                    string sql = "INSERT INTO CentrumKontaktu (ZgloszenieID, KlientID, DataWyslania, Typ, Tytul, Tresc, Uzytkownik, Kierunek) VALUES (@zg, @kl, @dt, 'Maildoproducenta', @tyt, @tr, @us, 'OUT')";
                    using (var cmd = new MySqlCommand(sql, con))
                    {
                        cmd.Parameters.AddWithValue("@zg", _dbZgloszenieIdInt);
                        cmd.Parameters.AddWithValue("@kl", _dbKlientIdInt);
                        cmd.Parameters.AddWithValue("@dt", DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"));
                        cmd.Parameters.AddWithValue("@tyt", temat);
                        cmd.Parameters.AddWithValue("@tr", trescHtml);
                        cmd.Parameters.AddWithValue("@us", Program.fullName);
                        await cmd.ExecuteNonQueryAsync();
                    }
                }
            }
            catch (Exception ex) { MessageBox.Show("Błąd zapisu historii: " + ex.Message); }
        }

        private Task<string> GenerateManufacturerPdfAsync()
        {
            return Task.Run(() => {
                string pdfName = $"Zgloszenie_{_producent}_{_nrZgloszenia.Replace('/', '.')}.pdf";
                string outPath = Path.Combine(_complaintFolderPath, pdfName);
                string tplPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Formularze", $"{_producent}.pdf");
                string fontPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Fonts", "NotoSans-Regular.ttf");

                if (!File.Exists(tplPath)) return null;

                var f = new Dictionary<string, string>();
                f["Data"] = DateTime.Now.ToString("dd.MM.yyyy");
                f["NrZgloszenia"] = _nrZgloszenia;
                f["Produkt"] = txtEdytowalnyKod.Text;
                f["Wada"] = txtEdytowalnyOpis.Text;
                f["Datazakupu"] = chkIncFV.Checked ? (txtEdytowalnaFaktura.Text + " " + dtpEdytowalnaData.Value.ToString("dd.MM.yyyy")) : "-";
                f["Zglaszajacy"] = Program.fullName;

                if (_producent.ToUpper() == "HELLA")
                {
                    f["Okolicznosc"] = txtHellaOkolicznosci?.Text ?? "";
                    f["Uwagi"] = txtHellaUwagi?.Text ?? "";
                    f["NazwaHella"] = txtEdytowalnyKod.Text;
                }
                else if (_producent.ToUpper() == "STRANDS")
                {
                    f["Nrkabla"] = txtStrandsNrKabla?.Text ?? "";
                    f["Jakdlugo"] = $"{numStrandsCzas?.Value} {cmbStrandsJednostka?.Text}";
                    f["Typ"] = cmbStrandsPojazd?.Text ?? "";
                    f["V"] = cmbStrandsNapiecie?.Text ?? "";
                    f["gdzie"] = cmbStrandsMontaz1?.Text ?? "";
                    f["gdzie2"] = cmbStrandsMontaz2?.Text ?? "";
                }

                try { PdfFormHelperSharp5.FillAndFlatten(tplPath, outPath, f, fontPath, true, 10f); return outPath; }
                catch { return null; }
            });
        }
        #endregion

        #region UI Logic
        private void CreateManufacturerTabs()
        {
            var flow = new FlowLayoutPanel { Dock = DockStyle.Fill, FlowDirection = FlowDirection.TopDown, AutoScroll = true, Padding = new Padding(10) };

            if (_producent.ToUpper() == "HELLA")
            {
                txtHellaOkolicznosci = new TextBox { Width = 600 };
                txtHellaUwagi = new TextBox { Width = 600 };
                AddRow(flow, "Okoliczności:", txtHellaOkolicznosci);
                AddRow(flow, "Uwagi:", txtHellaUwagi);
            }
            else if (_producent.ToUpper() == "STRANDS")
            {
                txtStrandsNrKabla = new TextBox { Width = 600 };
                numStrandsCzas = new NumericUpDown { Width = 200 };
                cmbStrandsJednostka = new ComboBox { Width = 140, DropDownStyle = ComboBoxStyle.DropDownList };
                cmbStrandsJednostka.Items.AddRange(new object[] { "Day", "Week", "Month" }); cmbStrandsJednostka.SelectedIndex = 0;
                var pTime = new FlowLayoutPanel { Width = 600, Height = 30 };
                pTime.Controls.Add(numStrandsCzas); pTime.Controls.Add(cmbStrandsJednostka);

                cmbStrandsPojazd = new ComboBox { Width = 600, DropDownStyle = ComboBoxStyle.DropDownList };
                cmbStrandsPojazd.Items.AddRange(new object[] { "Car", "Truck" });
                cmbStrandsNapiecie = new ComboBox { Width = 600, DropDownStyle = ComboBoxStyle.DropDownList };
                cmbStrandsNapiecie.Items.AddRange(new object[] { "12V", "24V" });
                cmbStrandsMontaz1 = new ComboBox { Width = 600, DropDownStyle = ComboBoxStyle.DropDownList };
                cmbStrandsMontaz1.Items.AddRange(new object[] { "Yes", "No" });
                cmbStrandsMontaz2 = new ComboBox { Width = 600, DropDownStyle = ComboBoxStyle.DropDownList };
                cmbStrandsMontaz2.Items.AddRange(new object[] { "Yes", "No" });

                AddRow(flow, "Nr z kabla:", txtStrandsNrKabla);
                flow.Controls.Add(new Label { Text = "Czas używania:", AutoSize = true }); flow.Controls.Add(pTime);
                AddRow(flow, "Pojazd:", cmbStrandsPojazd);
                AddRow(flow, "Napięcie:", cmbStrandsNapiecie);
                AddRow(flow, "Przekaźnik?", cmbStrandsMontaz1);
                AddRow(flow, "Wtyczka DT?", cmbStrandsMontaz2);
            }

            if (pnlSpecyficzneContainer != null) pnlSpecyficzneContainer.Controls.Add(flow);
        }

        private void AddRow(FlowLayoutPanel p, string text, Control c)
        {
            p.Controls.Add(new Label { Text = text, AutoSize = true, Margin = new Padding(0, 10, 0, 0) });
            p.Controls.Add(c);
        }

        private void RefreshHtmlPreview()
        {
            if (webView != null && webView.CoreWebView2 != null)
                webView.NavigateToString(GenerateHtmlBody());
        }

        private void ShowEmailPreviewMode()
        {
            picPreview.Visible = false;
            webView.Visible = true;
            lblPreviewTitle.Text = "Podgląd: Wiadomość E-mail (HTML)";
            btnShowMailPreview.Visible = false;
            RefreshHtmlPreview();
            if (listFiles.SelectedItems.Count > 0) listFiles.SelectedItems.Clear();
        }

        private void ListFiles_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (listFiles.SelectedItems.Count == 0) return;

            string p = listFiles.SelectedItems[0].Tag.ToString();
            string ext = Path.GetExtension(p).ToUpper();

            lblPreviewTitle.Text = $"Podgląd pliku: {Path.GetFileName(p)}";
            btnShowMailPreview.Visible = true;

            if (new[] { ".JPG", ".PNG", ".BMP", ".JPEG", ".GIF" }.Contains(ext))
            {
                webView.Visible = false;
                picPreview.Visible = true;
                picPreview.ImageLocation = p;
            }
            else if (new[] { ".PDF", ".TXT", ".HTML", ".HTM", ".MP4", ".WEBM", ".AVI", ".MOV" }.Contains(ext))
            {
                picPreview.Visible = false;
                webView.Visible = true;
                webView.CoreWebView2.Navigate(p);
            }
            else
            {
                picPreview.Visible = false;
                webView.Visible = true;
                webView.NavigateToString("<html><body style='background:#f0f0f0; display:flex; justify-content:center; align-items:center; height:100%; font-family:sans-serif;'><h2>Brak podglądu dla tego typu pliku.</h2></body></html>");
            }
        }

        private async void BtnTranslate_Click(object sender, EventArgs e)
        {
            try
            {
                var t = new Translator(_deepLApiKey);
                if (!string.IsNullOrWhiteSpace(txtEdytowalnyOpis.Text))
                {
                    var r = await t.TranslateTextAsync(txtEdytowalnyOpis.Text, "PL", "en-GB");
                    if (chkZachowajPolski.Checked)
                        txtEdytowalnyOpis.AppendText(Environment.NewLine + Environment.NewLine + "[EN]: " + r.Text);
                    else
                        txtEdytowalnyOpis.Text = r.Text;
                }
            }
            catch (Exception ex) { MessageBox.Show("DeepL Error: " + ex.Message); }
        }
        #endregion
    
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