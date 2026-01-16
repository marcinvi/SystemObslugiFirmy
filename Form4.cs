using Reklamacje_Dane.Allegro;
using Reklamacje_Dane.Allegro.Issues;
using System;
using System.Collections.Generic;
using System.Data;
using MySql.Data.MySqlClient;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Net.Mail;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Reklamacje_Dane
{
    public partial class Form4 : Form
    {
        // POLA PRYWATNE
        private string nrZgloszenia;
        private string _initialMessage;
        private string _initialStatusKlient;
        private Dictionary<string, string> daneZgloszenia = new Dictionary<string, string>();

        // SERWISY
        private readonly DatabaseService _dbService;
        private readonly EmailTemplateService _emailTemplateService;
        private readonly EmailService _emailService;
        private readonly ContactRepository _repo;

        // FLAGI WYSYŁKI
        private bool _sendEmail = false;
        private bool _sendSms = false;
        private bool _sendAllegro = false;

        // DRAG & DROP
        private Rectangle dragBoxFromMouseDown;
        private int rowIndexFromMouseDown;

        // KONSTRUKTOR
        public Form4(string nrZgloszenia, string initialMessage = "", string statusKlient = null)
        {
            this.StartPosition = FormStartPosition.CenterScreen;
            InitializeComponent();
            KonfigurujKolumnyDgv();

            this.nrZgloszenia = nrZgloszenia;
            this._initialMessage = initialMessage;
            this._initialStatusKlient = statusKlient;

            _dbService = new DatabaseService(DatabaseHelper.GetConnectionString());
            _emailTemplateService = new EmailTemplateService();
            _emailService = new EmailService();
            _repo = new ContactRepository();
        

            // Włącz sprawdzanie pisowni dla wszystkich TextBoxów
            EnableSpellCheckOnAllTextBoxes();
        }

        private void KonfigurujKolumnyDgv()
        {
            dgvTemplates.Columns.Clear();

            var idColumn = new DataGridViewTextBoxColumn
            {
                Name = "colId",
                Visible = false
            };
            dgvTemplates.Columns.Add(idColumn);

            var chkColumn = new DataGridViewCheckBoxColumn
            {
                Name = "colWybor",
                HeaderText = "Wybierz",
                Width = 50
            };
            dgvTemplates.Columns.Add(chkColumn);

            var nameColumn = new DataGridViewTextBoxColumn
            {
                Name = "colNazwa",
                HeaderText = "Nazwa Szablonu",
                ReadOnly = true,
                AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill
            };
            dgvTemplates.Columns.Add(nameColumn);

            // Ukryta kolumna z treścią RTF
            var rtfColumn = new DataGridViewTextBoxColumn
            {
                Name = "colRtf",
                Visible = false
            };
            dgvTemplates.Columns.Add(rtfColumn);
        }

        private async void Form4_Load(object sender, EventArgs e)
        {
            if (cmbCzcionka.Items.Count > 0) cmbCzcionka.SelectedItem = "Segoe UI";
            if (cmbRozmiar.Items.Count > 0) cmbRozmiar.SelectedItem = "10";

            await WczytajDaneDoPodmianyAsync();
            await WczytajSzablonyAsync();
            WczytajKontaPocztowe();

            if (!string.IsNullOrEmpty(_initialMessage))
            {
                rtbPodgladWiadomosci.Text = _initialMessage;
            }

            await ApplyStatusTemplateAsync();

            bool maAllegro = daneZgloszenia.ContainsKey("{{AllegroDisputeId}}") &&
                             !string.IsNullOrEmpty(daneZgloszenia["{{AllegroDisputeId}}"]);
            btnToggleAllegro.Enabled = maAllegro;

            UpdateToggleButtonStates();
        }

        // --- GŁÓWNA METODA POBIERANIA DANYCH ---
        private async Task WczytajDaneDoPodmianyAsync()
        {
            try
            {
                daneZgloszenia.Clear();

                string query = @"
            SELECT 
                z.Id, 
                z.NrZgloszenia, 
                z.DataZgloszenia, 
                z.Skad, 
                z.OpisUsterki, 
                z.DataZakupu, 
                z.NrFaktury,
                z.NrSeryjny,
                z.StatusOgolny,
                z.StatusKlient,
                
                k.Id AS KlientID, 
                k.ImieNazwisko, 
                k.Email, 
                k.Telefon, 
                k.Ulica, 
                k.KodPocztowy, 
                k.Miejscowosc AS Miasto,
                
                p.NazwaKrotka AS ProduktNazwa,
                
                z.allegroDisputeId, 
                z.AllegroAccountId
            FROM Zgloszenia z 
            LEFT JOIN klienci k ON z.KlientID = k.Id 
            LEFT JOIN Produkty p ON z.ProduktID = p.Id 
            WHERE z.NrZgloszenia = @nr";

                var dt = await _dbService.GetDataTableAsync(query, new MySqlParameter("@nr", nrZgloszenia));

                if (dt.Rows.Count > 0)
                {
                    var r = dt.Rows[0];

                    // 2. MAPOWANIE
                    daneZgloszenia.Add("{{NrZgloszenia}}", r["NrZgloszenia"]?.ToString());
                    daneZgloszenia.Add("{{DataZgloszenia}}", FormatujDate(r["DataZgloszenia"]));

                    daneZgloszenia.Add("{{KlientNazwa}}", r["ImieNazwisko"]?.ToString());
                    daneZgloszenia.Add("{{KlientEmail}}", r["Email"]?.ToString());
                    daneZgloszenia.Add("{{KlientTelefon}}", r["Telefon"]?.ToString());

                    string ulica = r["Ulica"]?.ToString() ?? "";
                    string kod = r["KodPocztowy"]?.ToString() ?? "";
                    string miasto = r["Miasto"]?.ToString() ?? "";
                    string adres = $"{ulica}, {kod} {miasto}".Trim(',', ' ');
                    daneZgloszenia.Add("{{KlientAdres}}", adres);

                    daneZgloszenia.Add("{{ProduktNazwa}}", r["ProduktNazwa"]?.ToString());
                    daneZgloszenia.Add("{{ProduktSN}}", r["NrSeryjny"]?.ToString());

                    daneZgloszenia.Add("{{OpisUsterki}}", r["OpisUsterki"]?.ToString());
                    daneZgloszenia.Add("{{DataZakupu}}", FormatujDate(r["DataZakupu"]));
                    daneZgloszenia.Add("{{NrFaktury}}", r["NrFaktury"]?.ToString());
                    daneZgloszenia.Add("{{StatusOgolny}}", r["StatusOgolny"]?.ToString());
                    daneZgloszenia.Add("{{StatusKlient}}", r["StatusKlient"]?.ToString());

                    // Systemowe
                    daneZgloszenia.Add("{{PracownikImie}}", Program.fullName);
                    daneZgloszenia.Add("{{Zrodlo}}", r["Skad"]?.ToString());
                    daneZgloszenia.Add("{{ZgloszenieID}}", r["Id"].ToString());
                    daneZgloszenia.Add("{{KlientID}}", r["KlientID"].ToString());

                    // Allegro
                    if (dt.Columns.Contains("allegroDisputeId"))
                        daneZgloszenia.Add("{{AllegroDisputeId}}", r["allegroDisputeId"]?.ToString());
                    if (dt.Columns.Contains("AllegroAccountId"))
                        daneZgloszenia.Add("{{AllegroAccountId}}", r["AllegroAccountId"]?.ToString());

                    if (dgvTemplates.Rows.Count > 0)
                    {
                        GenerujPodglad();
                    }
                }
                else
                {
                    MessageBox.Show($"Nie znaleziono zgłoszenia nr: {nrZgloszenia}", "Info", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Błąd SQL:\n" + ex.Message, "Krytyczny błąd", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private string FormatujDate(object dbValue)
        {
            if (dbValue == null || dbValue == DBNull.Value) return "";
            string val = dbValue.ToString();
            if (DateTime.TryParse(val, out DateTime d)) return d.ToString("dd.MM.yyyy");
            return val;
        }

        // --- GENEROWANIE PODGLĄDU ---
        private void GenerujPodglad()
        {
            if (daneZgloszenia.Count == 0) return;

            rtbPodgladWiadomosci.SuspendLayout();

            var tempRtb = new RichTextBox();
            bool uzytoSzablonu = false;

            foreach (DataGridViewRow row in dgvTemplates.Rows)
            {
                if (Convert.ToBoolean(row.Cells["colWybor"].Value))
                {
                    if (!uzytoSzablonu)
                    {
                        tempRtb.AppendText("Dzień dobry,\n\n");
                        uzytoSzablonu = true;
                    }

                    tempRtb.Select(tempRtb.TextLength, 0);
                    tempRtb.SelectedRtf = row.Cells["colRtf"].Value.ToString();
                    tempRtb.AppendText("\n");
                }
            }

            if (uzytoSzablonu)
            {
                // ### POPRAWKA: Usunięto automatyczne dodawanie "Pozdrawiam, {{PracownikImie}}" ###
                // ### Dzięki temu w mailu nie będzie podwójnego podpisu (jest w stopce HTML). ###
                // ### Podpis dla SMS/Allegro dodamy w metodach wysyłania. ###

                rtbPodgladWiadomosci.Rtf = tempRtb.Rtf;
            }

            // PODMIANA ZMIENNYCH
            foreach (var para in daneZgloszenia)
            {
                string klucz = para.Key;
                string wartosc = para.Value ?? "";

                int start = 0;
                while (start < rtbPodgladWiadomosci.TextLength)
                {
                    int index = rtbPodgladWiadomosci.Find(klucz, start, RichTextBoxFinds.None);
                    if (index == -1) break;

                    rtbPodgladWiadomosci.Select(index, klucz.Length);
                    var currentFont = rtbPodgladWiadomosci.SelectionFont;
                    rtbPodgladWiadomosci.SelectedText = wartosc;

                    if (currentFont != null)
                    {
                        rtbPodgladWiadomosci.Select(index, wartosc.Length);
                        rtbPodgladWiadomosci.SelectionFont = currentFont;
                    }
                    start = index + wartosc.Length;
                }
            }

            rtbPodgladWiadomosci.ResumeLayout();
        }

        // --- POZOSTAŁE METODY ---

        private async Task WczytajSzablonyAsync()
        {
            try
            {
                // Używamy nowej metody zwracającej List<SzablonEmail>
                var szablony = await _emailTemplateService.GetActiveTemplatesAsync();

                dgvTemplates.Rows.Clear();
                foreach (var t in szablony)
                {
                    // Mapujemy właściwości nowej klasy (Nazwa, TrescRtf) do kolumn GridView
                    dgvTemplates.Rows.Add(t.Id, false, t.Nazwa, t.TrescRtf);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Błąd ładowania szablonów: " + ex.Message);
            }
        }

        private async Task ApplyStatusTemplateAsync()
        {
            if (string.IsNullOrWhiteSpace(_initialStatusKlient))
            {
                return;
            }

            var templateId = await _emailTemplateService.GetTemplateIdForStatusAsync(_initialStatusKlient);
            if (!templateId.HasValue)
            {
                return;
            }

            foreach (DataGridViewRow row in dgvTemplates.Rows)
            {
                if (row.Cells["colId"].Value == null) continue;
                if (!int.TryParse(row.Cells["colId"].Value.ToString(), out int rowId)) continue;
                if (rowId == templateId.Value)
                {
                    row.Cells["colWybor"].Value = true;
                    GenerujPodglad();
                    return;
                }
            }
        }

        private void WczytajKontaPocztowe()
        {
            try
            {
                var konta = _repo.PobierzKontaPocztowe();
                if (konta == null || konta.Count == 0)
                {
                    cmbKontoEmail.Items.Add("Brak kont!");
                    cmbKontoEmail.Enabled = false;
                    return;
                }
                cmbKontoEmail.DataSource = konta;
                cmbKontoEmail.DisplayMember = "AdresEmail";

                string zrodlo = daneZgloszenia.ContainsKey("{{Zrodlo}}") ? daneZgloszenia["{{Zrodlo}}"].ToLower() : "";
                KontoPocztowe wybrane = konta.FirstOrDefault(k => k.AdresEmail.ToLower().Contains(zrodlo));

                if (wybrane == null) wybrane = konta.FirstOrDefault(k => k.CzyDomyslne);
                if (wybrane == null) wybrane = konta.FirstOrDefault();

                if (wybrane != null) cmbKontoEmail.SelectedItem = wybrane;
            }
            catch (Exception ex) { MessageBox.Show("Błąd kont: " + ex.Message); }
        }

        private void UpdateToggleButtonStates()
        {
            btnToggleEmail.BackColor = _sendEmail ? Color.ForestGreen : Color.LightGray;
            btnToggleEmail.ForeColor = _sendEmail ? Color.White : Color.Black;
            cmbKontoEmail.Enabled = _sendEmail;

            btnToggleSms.BackColor = _sendSms ? Color.ForestGreen : Color.LightGray;
            btnToggleSms.ForeColor = _sendSms ? Color.White : Color.Black;

            btnToggleAllegro.BackColor = _sendAllegro ? Color.ForestGreen : Color.LightGray;
            btnToggleAllegro.ForeColor = _sendAllegro ? Color.White : Color.Black;
        }

        // --- WYSYŁKA ---
        private async void buttonWyslij_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(rtbPodgladWiadomosci.Text)) { MessageBox.Show("Treść jest pusta."); return; }
            if (!_sendEmail && !_sendSms && !_sendAllegro) { MessageBox.Show("Wybierz kanał wysyłki."); return; }

            bool error = false;
            string log = "";

            if (_sendEmail)
            {
                try
                {
                    await SendEmailInternal();
                    log += "[EMAIL: OK] ";
                }
                catch (SmtpFailedRecipientException ex)
                {
                    error = true;
                    log += "[EMAIL: BŁĄD ADRESATA] ";
                    MessageBox.Show($"Nie wysłano maila.\nAdres '{ex.FailedRecipient}' nie istnieje (Błąd 5.1.1).", "Błąd Adresu", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
                catch (Exception ex)
                {
                    error = true;
                    log += $"[EMAIL: BŁĄD {ex.Message}] ";
                    MessageBox.Show("Błąd Email: " + ex.Message);
                }
            }

            if (_sendSms)
            {
                try
                {
                    await SendSmsInternal();
                    log += "[SMS: OK] ";
                }
                catch (Exception ex)
                {
                    error = true;
                    log += $"[SMS: BŁĄD {ex.Message}] ";
                    MessageBox.Show("Błąd SMS: " + ex.Message);
                }
            }

            if (_sendAllegro)
            {
                try
                {
                    await SendAllegroInternal();
                    log += "[ALLEGRO: OK] ";
                }
                catch (Exception ex)
                {
                    error = true;
                    log += $"[ALLEGRO: BŁĄD {ex.Message}] ";
                    MessageBox.Show("Błąd Allegro: " + ex.Message);
                }
            }

            if (!string.IsNullOrEmpty(log))
            {
                await LogToDatabase(log);
                if (!error)
                {
                    MessageBox.Show("Wysłano pomyślnie!", "Sukces");
                    this.Close();
                }
            }
        }

        private async Task SendEmailInternal()
        {
            if (!daneZgloszenia.ContainsKey("{{KlientEmail}}") || string.IsNullOrEmpty(daneZgloszenia["{{KlientEmail}}"]))
                throw new Exception("Brak adresu e-mail klienta.");

            string doKogo = daneZgloszenia["{{KlientEmail}}"];
            var nadawca = cmbKontoEmail.SelectedItem as KontoPocztowe;
            if (nadawca == null) throw new Exception("Nie wybrano konta nadawcy.");

            string temat = $"Zgłoszenie reklamacyjne nr {nrZgloszenia} - {daneZgloszenia["{{ProduktNazwa}}"]}";

            string htmlBody = "";
            try { htmlBody = RtfPipe.Rtf.ToHtml(rtbPodgladWiadomosci.Rtf); }
            catch { htmlBody = rtbPodgladWiadomosci.Text.Replace("\n", "<br>"); }

            // --- TUTAJ DOKLEJAMY STOPKĘ HTML (tylko dla maila) ---
            if (!string.IsNullOrWhiteSpace(nadawca.Podpis))
            {
                string podpisGotowy = "";
                string surowyPodpis = nadawca.Podpis.Trim();

                if (surowyPodpis.StartsWith("{\\rtf"))
                {
                    try { podpisGotowy = RtfPipe.Rtf.ToHtml(surowyPodpis); } catch { podpisGotowy = surowyPodpis; }
                }
                else if (surowyPodpis.Contains("<div") || surowyPodpis.Contains("<table") || surowyPodpis.Contains("<html"))
                {
                    podpisGotowy = surowyPodpis;
                }
                else
                {
                    podpisGotowy = surowyPodpis.Replace("\n", "<br>");
                    podpisGotowy = $@"<div style='font-family: Arial, sans-serif; font-size: 13px; color: #555; margin-top: 10px;'>{podpisGotowy}</div>";
                }

                // AKTUALIZACJA ZMIENNYCH PRACOWNIKA
                daneZgloszenia["{{PracownikImie}}"] = Program.fullName;
                string emailPracownika = Program.currentUserEmail;
                if (string.IsNullOrEmpty(emailPracownika)) emailPracownika = nadawca.AdresEmail;

                if (daneZgloszenia.ContainsKey("{{PracownikEmail}}")) daneZgloszenia["{{PracownikEmail}}"] = emailPracownika;
                else daneZgloszenia.Add("{{PracownikEmail}}", emailPracownika);

                foreach (var para in daneZgloszenia)
                {
                    if (!string.IsNullOrEmpty(para.Key))
                        podpisGotowy = podpisGotowy.Replace(para.Key, para.Value ?? "");
                }

                htmlBody += "<br><br><br>" + podpisGotowy;
            }

            List<string> pliki = new List<string>();
            foreach (var item in lbxAttachments.Items) pliki.Add(item.ToString());

            await _emailService.WyslijEmailAsync(nadawca, doKogo, temat, htmlBody, pliki);
        }

        private async Task SendSmsInternal()
        {
            string tel = daneZgloszenia["{{KlientTelefon}}"];
            if (string.IsNullOrEmpty(tel)) throw new Exception("Brak telefonu klienta.");
            if (PhoneClient.Instance == null) throw new Exception("Telefon niepołączony.");

            // ### POPRAWKA: Dodajemy podpis tekstowy DLA SMS ###
            string trescSms = rtbPodgladWiadomosci.Text;
            trescSms += $"\n\nPozdrawiam,\n{Program.fullName}";

            if (!await PhoneClient.Instance.SendSmsAsync(tel, trescSms))
                throw new Exception("Błąd bramki SMS.");
        }

        private async Task SendAllegroInternal()
        {
            string disputeId = daneZgloszenia["{{AllegroDisputeId}}"];
            int accId = int.Parse(daneZgloszenia["{{AllegroAccountId}}"]);

            // ### POPRAWKA: Dodajemy podpis tekstowy DLA ALLEGRO ###
            string trescAllegro = rtbPodgladWiadomosci.Text;
            trescAllegro += $"\n\nPozdrawiam,\n{Program.fullName}";

            using (var con = Database.GetNewOpenConnection())
            {
                var api = await DatabaseHelper.GetApiClientForAccountAsync(accId, con);
                await api.SendMessageAsync(disputeId, new NewMessageRequest { Text = trescAllegro });
            }
        }

        private async Task LogToDatabase(string status)
        {
            using (var con = DatabaseHelper.GetConnection())
            {
                await con.OpenAsync();
                string sql = "INSERT INTO CentrumKontaktu (ZgloszenieID, KlientID, DataWyslania, Typ, Tytul, Tresc, Uzytkownik, Kierunek) VALUES (@zg, @kl, @dt, @typ, @tyt, @tr, @us, 'OUT')";
                using (var cmd = new MySqlCommand(sql, con))
                {
                    cmd.Parameters.AddWithValue("@zg", daneZgloszenia["{{ZgloszenieID}}"]);
                    cmd.Parameters.AddWithValue("@kl", daneZgloszenia["{{KlientID}}"]);
                    cmd.Parameters.AddWithValue("@dt", DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"));
                    cmd.Parameters.AddWithValue("@typ", "Multi");
                    cmd.Parameters.AddWithValue("@tyt", status);
                    cmd.Parameters.AddWithValue("@tr", rtbPodgladWiadomosci.Rtf);
                    cmd.Parameters.AddWithValue("@us", Program.fullName);
                    await cmd.ExecuteNonQueryAsync();
                }
            }
        }

        // --- OBSŁUGA UI ---
        private void dgvTemplates_CellValueChanged(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex >= 0 && dgvTemplates.Columns[e.ColumnIndex].Name == "colWybor")
            {
                GenerujPodglad();
            }
        }
        private void dgvTemplates_CurrentCellDirtyStateChanged(object sender, EventArgs e) { if (dgvTemplates.IsCurrentCellDirty) dgvTemplates.CommitEdit(DataGridViewDataErrorContexts.Commit); }

        private void btnToggleEmail_Click(object sender, EventArgs e) { _sendEmail = !_sendEmail; UpdateToggleButtonStates(); }
        private void btnToggleSms_Click(object sender, EventArgs e) { _sendSms = !_sendSms; UpdateToggleButtonStates(); }
        private void btnToggleAllegro_Click(object sender, EventArgs e) { _sendAllegro = !_sendAllegro; UpdateToggleButtonStates(); }

        private void btnDodajZalacznik_Click(object sender, EventArgs e)
        {
            using (var ofd = new OpenFileDialog { Multiselect = true })
                if (ofd.ShowDialog() == DialogResult.OK)
                    foreach (var f in ofd.FileNames) lbxAttachments.Items.Add(f);
        }
        private void btnUsunZalacznik_Click(object sender, EventArgs e) { if (lbxAttachments.SelectedItem != null) lbxAttachments.Items.Remove(lbxAttachments.SelectedItem); }

        private void dgvTemplates_MouseDown(object sender, MouseEventArgs e)
        {
            var h = dgvTemplates.HitTest(e.X, e.Y);
            if (h.RowIndex != -1)
            {
                rowIndexFromMouseDown = h.RowIndex;
                dragBoxFromMouseDown = new Rectangle(new Point(e.X - 5, e.Y - 5), new Size(10, 10));
            }
            else dragBoxFromMouseDown = Rectangle.Empty;
        }
        private void dgvTemplates_MouseMove(object sender, MouseEventArgs e)
        {
            if ((e.Button & MouseButtons.Left) == MouseButtons.Left)
            {
                if (dragBoxFromMouseDown != Rectangle.Empty && !dragBoxFromMouseDown.Contains(e.X, e.Y))
                    dgvTemplates.DoDragDrop(dgvTemplates.Rows[rowIndexFromMouseDown], DragDropEffects.Move);
            }
        }
        private void dgvTemplates_DragOver(object sender, DragEventArgs e) { e.Effect = DragDropEffects.Move; }
        private void dgvTemplates_DragDrop(object sender, DragEventArgs e)
        {
            Point p = dgvTemplates.PointToClient(new Point(e.X, e.Y));
            int idx = dgvTemplates.HitTest(p.X, p.Y).RowIndex;
            if (idx >= 0 && rowIndexFromMouseDown >= 0)
            {
                var row = dgvTemplates.Rows[rowIndexFromMouseDown];
                dgvTemplates.Rows.RemoveAt(rowIndexFromMouseDown);
                dgvTemplates.Rows.Insert(idx, row);
                GenerujPodglad();
            }
        }

        private void btnBold_Click(object sender, EventArgs e) { ToggleStyle(FontStyle.Bold); }
        private void btnItalic_Click(object sender, EventArgs e) { ToggleStyle(FontStyle.Italic); }
        private void btnUnderline_Click(object sender, EventArgs e) { ToggleStyle(FontStyle.Underline); }
        private void ToggleStyle(FontStyle s)
        {
            if (rtbPodgladWiadomosci.SelectionFont != null)
            {
                var f = rtbPodgladWiadomosci.SelectionFont;
                rtbPodgladWiadomosci.SelectionFont = new Font(f, f.Style ^ s);
            }
            rtbPodgladWiadomosci.Focus();
        }
        private void cmbCzcionka_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (cmbCzcionka.SelectedItem != null)
            {
                var size = rtbPodgladWiadomosci.SelectionFont?.Size ?? 10;
                rtbPodgladWiadomosci.SelectionFont = new Font(cmbCzcionka.SelectedItem.ToString(), size);
            }
        }
        private void cmbRozmiar_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (cmbRozmiar.SelectedItem != null && float.TryParse(cmbRozmiar.SelectedItem.ToString(), out float s))
            {
                var name = rtbPodgladWiadomosci.SelectionFont?.Name ?? "Segoe UI";
                rtbPodgladWiadomosci.SelectionFont = new Font(name, s);
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
