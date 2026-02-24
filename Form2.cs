using PdfSharp.Drawing;
using PdfSharp.Drawing.Layout;
using PdfSharp.Fonts;
using PdfSharp.Pdf;
using Reklamacje_Dane;
using Reklamacje_Dane.Allegro.Issues;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using MySql.Data.MySqlClient;
using System.Diagnostics;
using System.Drawing;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Reklamacje_Dane
{
    public partial class Form2 : Form
    {
        private string nrZgloszenia;
        private int idZgloszeniaInt = 0;
        public string NrZgloszeniaPublic => nrZgloszenia;

        private MagazynService _magazynService = new MagazynService();
        private int nrKlienta;
        private int nrProduktu;
        private string kategoriaProduktu;
        private string producentProduktu;
        private string originalOpisUsterki;
        private string _allegroDisputeId;
        private string nrSeryjnyZgloszenia;

        private DpdTrackingService _trackingService;
        private readonly DatabaseService _dbService = new DatabaseService(DatabaseHelper.GetConnectionString());
        private readonly ContactRepository _repo = new ContactRepository();
        private readonly ContextMenuStrip _quickActionsMenu = new ContextMenuStrip();
        private readonly ToolTip _phoneToolTip = new ToolTip();
        private Button _btnFetchPart;
        private Button _btnRefreshData;

        public Form2(string nrZgloszenia)
        {
            InitializeComponent();
            this.StartPosition = FormStartPosition.CenterScreen;
            this.nrZgloszenia = nrZgloszenia;
            try { GlobalFontSettings.FontResolver = new PdfFontResolver(); } catch { }

            AttachEventHandlers();
            InitializeExtraMenuButtons();

            // Włącz sprawdzanie pisowni dla wszystkich TextBoxów
            EnableSpellCheckOnAllTextBoxes();
        }

        private void AttachEventHandlers()
        {
            this.Load += Form2_Load;

            // Kontrolki
            clientInfoControl1.ChangeClientRequested += OnChangeClientRequested;
            productInfoControl1.EditProductRequested += OnEditProductRequested;
            this.textBox1.TextChanged += (s, e) => { if (originalOpisUsterki != null) btnZapiszOpis.Visible = textBox1.Text != originalOpisUsterki; };
            this.btnZapiszOpis.Click += btnZapiszOpis_Click;

            // Główne przyciski
            this.btnAllegroModule.Click += btnAllegroModule_Click;
            this.buttonWyslijMail.Click += buttonWyslijMail_Click;
            this.button1.Click += button1_Click;

            // Menu kuriera
            this.zamowOdKlientaMenuItem.Click += zamowOdKlientaMenuItem_Click;
            this.zamowDoKlientaMenuItem.Click += zamowDoKlientaMenuItem_Click;

            this.button2.Click += button2_Click;

            this.button3.Click += button3_Click;
            this.button4.Click += button4_Click;
            this.button5.Click += button5_Click;
            this.button6.Click += button6_Click;
            this.button7.Click += button7_Click;
            this.button8.Click += button8_Click;
            this.button9.Click += button9_Click;
            this.button11.Click += button11_Click;
            this.btnPrintToPdf.Click += btnPrintToPdf_Click;
            this.btnBackToDetails.Click += btnBackToDetails_Click;

            this.btnAddAction.Click += btnAddAction_Click;
            this.btnAddAction.MouseHover += btnAddAction_MouseHover;
            this.btnMagazyn.Click += btnMagazyn_Click;

            // Naprawa layoutu przy zmianie rozmiaru
            this.Resize += (s, e) => {
                ResizeBubbles(flowLayoutPanelHistory);
                ResizeBubbles(flowChatRight);
            };
        }

        private void InitializeExtraMenuButtons()
        {
            if (panelLeftSidebar == null) return;

            var btnBg = Color.FromArgb(13, 71, 161);
            var btnHover = Color.FromArgb(21, 101, 192);

            _btnFetchPart = new Button();
            ConfigureMenuButton(_btnFetchPart, "🧰 Pobierz z magazynu części", btnBg, btnHover);
            _btnFetchPart.Click += btnFetchPart_Click;

            _btnRefreshData = new Button();
            ConfigureMenuButton(_btnRefreshData, "🔄 Odśwież dane", btnBg, btnHover);
            _btnRefreshData.Click += async (s, e) => await LoadData();

            panelLeftSidebar.Controls.Add(_btnFetchPart);
            panelLeftSidebar.Controls.Add(_btnRefreshData);

            if (panelLeftSidebar.Controls.Contains(button9))
            {
                panelLeftSidebar.Controls.SetChildIndex(_btnFetchPart, panelLeftSidebar.Controls.GetChildIndex(button9));
            }
            if (panelLeftSidebar.Controls.Contains(buttonWyslijMail))
            {
                panelLeftSidebar.Controls.SetChildIndex(_btnRefreshData, panelLeftSidebar.Controls.GetChildIndex(buttonWyslijMail));
            }
        }

        

        private void ResizeBubbles(FlowLayoutPanel panel)
        {
            if (panel == null) return;
            panel.SuspendLayout();
            int newWidth = panel.ClientSize.Width - 25;
            if (newWidth < 100) newWidth = 100;

            foreach (Control c in panel.Controls)
            {
                c.Width = newWidth;
            }
            panel.ResumeLayout(true);
        }

        private async void Form2_Load(object sender, EventArgs e)
        {
            try
            {
                UpdateFilesButton();
                await LoadData();
                await LoadChatRightPanel();
                await PopulateQuickActionsMenu();
                await OdswiezPrzyciskMagazynu();
                AttachPhoneClickLogic();
            }
            catch (Exception ex) { MessageBox.Show($"Błąd startu: {ex.Message}"); }
        }

        private async void button2_Click(object sender, EventArgs e)
        {
            if (!string.IsNullOrEmpty(this.kategoriaProduktu) && this.kategoriaProduktu.Contains("Lodówka"))
            {
                UruchomAkcje(new Form6(this.nrZgloszenia));
                return;
            }

            string nazwaProducenta = "", emailProducenta = "";
            try
            {
                using (var con = DatabaseHelper.GetConnectionAsync())
                {
                    await con.OpenAsync();
                    string q = @"SELECT pr.NazwaProducenta, pr.KontaktMail 
                                 FROM Zgloszenia z 
                                 LEFT JOIN Produkty p ON z.ProduktID = p.Id 
                                 LEFT JOIN Producenci pr ON p.Producent = pr.NazwaProducenta 
                                 WHERE z.NrZgloszenia = @nr";

                    using (var cmd = new MySqlCommand(q, con))
                    {
                        cmd.Parameters.AddWithValue("@nr", this.nrZgloszenia);
                        using (var r = await cmd.ExecuteReaderAsync())
                        {
                            if (await r.ReadAsync())
                            {
                                nazwaProducenta = r["NazwaProducenta"]?.ToString();
                                emailProducenta = r["KontaktMail"]?.ToString();
                            }
                        }
                    }
                }
            }
            catch (Exception ex) { MessageBox.Show("Błąd bazy danych przy pobieraniu producenta: " + ex.Message); return; }

            if (string.IsNullOrEmpty(nazwaProducenta))
            {
                MessageBox.Show("Nie znaleziono producenta przypisanego do tego produktu.\nSprawdź edycję produktu i bazę producentów.", "Brak danych");
                return;
            }

            var formReport = new FormReportToManufacturer(this.nrZgloszenia, nazwaProducenta, emailProducenta);
            formReport.ShowDialog();
            await LoadData();
        }

        private async Task LoadData()
        {
            flowLayoutPanelHistory.SuspendLayout();
            while (flowLayoutPanelHistory.Controls.Count > 0)
            {
                var c = flowLayoutPanelHistory.Controls[0];
                flowLayoutPanelHistory.Controls.Remove(c);
                c.Dispose();
            }

            try
            {
                using (var con = DatabaseHelper.GetConnectionAsync())
                {
                    await con.OpenAsync();

                    string query = @"SELECT z.*, k.ImieNazwisko, k.NazwaFirmy, k.Email, k.Telefon, k.Ulica, k.KodPocztowy, k.Miejscowosc, 
                                     p.NazwaKrotka, p.Kategoria, p.Producent 
                                     FROM Zgloszenia z 
                                     LEFT JOIN Klienci k ON z.KlientID = k.Id 
                                     LEFT JOIN Produkty p ON z.ProduktID = p.Id 
                                     WHERE z.NrZgloszenia = @nrZgloszenia";

                    using (var cmd = new MySqlCommand(query, con))
                    {
                        cmd.Parameters.AddWithValue("@nrZgloszenia", this.nrZgloszenia);
                        using (var reader = await cmd.ExecuteReaderAsync())
                        {
                            if (await reader.ReadAsync())
                            {
                                this.idZgloszeniaInt = SafeToInt(reader["Id"]);
                                this.nrKlienta = SafeToInt(reader["KlientID"]);
                                this.nrProduktu = SafeToInt(reader["ProduktID"]);
                                this.kategoriaProduktu = reader["Kategoria"]?.ToString() ?? "Brak";
                                this.producentProduktu = reader["Producent"]?.ToString() ?? "Brak";
                                this.originalOpisUsterki = reader["OpisUsterki"]?.ToString() ?? "";
                                this._allegroDisputeId = reader["allegroDisputeId"]?.ToString();
                                this.nrSeryjnyZgloszenia = reader["NrSeryjny"]?.ToString() ?? "";

                                lblHeaderTitle.Text = $"Zgłoszenie numer: {reader["NrZgloszenia"]}";
                                this.Text = $"Zgłoszenie: {reader["NrZgloszenia"]}";
                                lblHeaderStatus.Text = $"{reader["StatusOgolny"]} | Status klient: {reader["StatusKlient"]} | Status producent: {reader["StatusProducent"]}";

                                await clientInfoControl1.LoadClientData(this.nrKlienta);
                                await productInfoControl1.LoadPurchaseData(this.nrZgloszenia);

                                textBox1.Text = this.originalOpisUsterki;
                                WypelnijFlowDokumenty(reader);
                                btnAllegroModule.Visible = !string.IsNullOrEmpty(this._allegroDisputeId);
                                AttachPhoneClickLogic();
                            }
                        }
                    }

                    // --- 1. POBRANIE PRZYPOMNIEŃ (ZADAŃ) ---
                    var reminderEvents = new List<TimelineEvent>();
                    string przypomnieniaQuery = @"SELECT Id, DataPrzypomnienia, Tresc, PrzypisanyUzytkownik 
                                                  FROM przypomnienia 
                                                  WHERE DotyczyZgloszenia = @nrZgloszenia 
                                                  AND (CzyZrealizowane = 0 OR CzyZrealizowane IS NULL) 
                                                  AND (Status != 'Completed' OR Status IS NULL)";
                    using (var cmd = new MySqlCommand(przypomnieniaQuery, con))
                    {
                        cmd.Parameters.AddWithValue("@nrZgloszenia", this.nrZgloszenia);
                        using (var reader = await cmd.ExecuteReaderAsync())
                        {
                            while (await reader.ReadAsync())
                            {
                                DateTime rDate = DateTime.Now;
                                DateTime.TryParse(reader["DataPrzypomnienia"]?.ToString(), out rDate);

                                reminderEvents.Add(new TimelineEvent
                                {
                                    EventDate = rDate,
                                    Content = "[ZADANIE] " + (reader["Tresc"]?.ToString() ?? ""),
                                    Author = reader["PrzypisanyUzytkownik"]?.ToString() ?? "Wszyscy Handlowcy",
                                    Tag = SafeToInt(reader["Id"]),
                                    IsReminder = true
                                });
                            }
                        }
                    }

                    // --- 2. POBRANIE DZIAŁAŃ (HISTORII) ---
                    var timelineEvents = new List<TimelineEvent>();
                    string dzialaniaQuery = "SELECT Id, DataDzialania, Tresc, Uzytkownik FROM Dzialania WHERE NrZgloszenia = @nrZgloszenia";
                    using (var cmd = new MySqlCommand(dzialaniaQuery, con))
                    {
                        cmd.Parameters.AddWithValue("@nrZgloszenia", this.nrZgloszenia);
                        using (var reader = await cmd.ExecuteReaderAsync())
                        {
                            while (await reader.ReadAsync())
                            {
                                if (DateTime.TryParse(reader["DataDzialania"]?.ToString(), out DateTime date))
                                {
                                    timelineEvents.Add(new TimelineEvent
                                    {
                                        EventDate = date,
                                        Content = reader["Tresc"]?.ToString(),
                                        Author = reader["Uzytkownik"]?.ToString() ?? "System",
                                        Tag = SafeToInt(reader["Id"]),
                                        IsReminder = false
                                    });
                                }
                            }
                        }
                    }
                    timelineEvents.Sort();

                    // --- 3. WSTAWIANIE DO UI ---

                    // Najpierw ładujemy przypomnienia (Będą na samej górze i podświetlone)
                    foreach (var ev in reminderEvents)
                    {
                        var itemControl = new TimelineItemControl();
                        itemControl.Setup(ev.EventDate,   ev.Content, ev.Author, TimelineItemType.Action);
                        itemControl.DataTag = ev.Tag;
                        itemControl.BackColor = Color.FromArgb(255, 235, 238); // Czerwonawe tło

                        // Zmiana zachowania: Kliknięcie lewym klawiszem -> Oznacz jako zrealizowane
                        itemControl.EditClicked += async (sender, args) => {
                            if (itemControl.DataTag is int remId)
                            {
                                if (MessageBox.Show("Czy na pewno chcesz oznaczyć to zadanie jako zrealizowane?", "Realizacja zadania", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
                                {
                                    await _dbService.ExecuteNonQueryAsync("UPDATE przypomnienia SET CzyZrealizowane = 1, Status = 'Completed' WHERE Id = @id", new MySqlParameter("@id", remId));
                                    await LoadData(); // Przeładuj widok
                                }
                            }
                        };

                        // Zmiana zachowania: Usuń działanie (z menu prawego klawisza) -> Oznacz jako zrealizowane
                        itemControl.DeleteClicked += async (sender, args) => {
                            if (itemControl.DataTag is int remId)
                            {
                                await _dbService.ExecuteNonQueryAsync("UPDATE przypomnienia SET CzyZrealizowane = 1, Status = 'Completed' WHERE Id = @id", new MySqlParameter("@id", remId));
                                await LoadData();
                            }
                        };

                        flowLayoutPanelHistory.Controls.Add(itemControl);
                    }

                    // Następnie zwykła historia (posortowana datami)
                    foreach (var ev in timelineEvents)
                    {
                        var type = DetermineEventType(ev.Content, ev.Author);
                        var itemControl = new TimelineItemControl();
                        itemControl.Setup(ev.EventDate, ev.Author, ev.Content, type);
                        itemControl.DataTag = ev.Tag;

                        // Standardowe akcje dla logów
                        itemControl.EditClicked += (sender, args) => { if (itemControl.DataTag is int actionId) EditAction(actionId); };
                        itemControl.DeleteClicked += async (sender, args) => { if (itemControl.DataTag is int actionId) await DeleteActionAsync(actionId); };
                        itemControl.OpenTrackingClicked += async (sender, trackingNumber) => { await ShowTrackingDetails(trackingNumber); };

                        flowLayoutPanelHistory.Controls.Add(itemControl);
                    }
                }
                btnZapiszOpis.Visible = false;
            }
            catch (Exception ex) { MessageBox.Show($"Błąd wczytywania: {ex.Message}"); }
            finally
            {
                flowLayoutPanelHistory.ResumeLayout();
                ResizeBubbles(flowLayoutPanelHistory);
                flowLayoutPanelHistory.PerformLayout();
                if (flowLayoutPanelHistory.Parent != null) flowLayoutPanelHistory.Parent.PerformLayout();
            }
        }

        private void AttachPhoneClickLogic()
        {
            AttachClickRecursive(clientInfoControl1);
        }

        private void AttachClickRecursive(Control parent)
        {
            foreach (Control c in parent.Controls)
            {
                if (c is Label lbl && IsPhoneNumber(lbl.Text))
                {
                    lbl.Cursor = Cursors.Hand;
                    lbl.ForeColor = Color.RoyalBlue;
                    lbl.Click -= LblPhone_Click;
                    lbl.Click += LblPhone_Click;
                    _phoneToolTip.SetToolTip(lbl, "Kliknij, aby zadzwonić");
                }
                if (c.HasChildren) AttachClickRecursive(c);
            }
        }

        private void LblPhone_Click(object sender, EventArgs e)
        {
            if (sender is Label lbl)
            {
                string numer = lbl.Text.Replace(" ", "").Replace("-", "").Trim();
                if (!string.IsNullOrEmpty(numer))
                {
                    if (PhoneApiClient.Instance != null)
                    {
                        try
                        {
                            PhoneApiClient.Instance.Dial(numer);
                            MessageBox.Show($"Łap telefon! Wybieranie numeru: {numer}...", "Telefon", MessageBoxButtons.OK, MessageBoxIcon.Information);
                        }
                        catch (Exception ex) { MessageBox.Show("Błąd: " + ex.Message); }
                    }
                }
            }
        }

        private bool IsPhoneNumber(string text)
        {
            if (string.IsNullOrWhiteSpace(text)) return false;
            string clean = Regex.Replace(text, @"[^\d]", "");
            return clean.Length >= 9 && clean.Length <= 15;
        }

        private static int SafeToInt(object value, int defaultValue = 0)
        {
            if (value == null || value == DBNull.Value) return defaultValue;
            if (value is int intValue) return intValue;
            if (value is long longValue) return (int)longValue;
            if (value is short shortValue) return shortValue;
            if (value is byte byteValue) return byteValue;
            if (value is decimal decimalValue) return (int)decimalValue;
            if (value is double doubleValue) return (int)doubleValue;
            if (value is float floatValue) return (int)floatValue;

            var text = value.ToString();
            if (string.IsNullOrWhiteSpace(text)) return defaultValue;
            return int.TryParse(text, NumberStyles.Integer, CultureInfo.InvariantCulture, out int parsed)
                ? parsed
                : defaultValue;
        }

        private void WypelnijFlowDokumenty(DbDataReader reader)
        {
            flowDocuments.Controls.Clear();
            AddDocLink("WRL", reader["NrWRL"]);
            AddDocLink("KWZ 2", reader["NrKWZ2"]);
            AddDocLink("KPZN", reader["NrKPZN"]);

            string dostawa = reader["CzekamyNaDostawe"]?.ToString();
            if (!string.IsNullOrWhiteSpace(dostawa))
            {
                Label lbl = new Label { Text = $"Dostawa: {dostawa}", AutoSize = true, ForeColor = Color.DarkRed, Font = new Font("Segoe UI", 9, FontStyle.Bold), Margin = new Padding(0, 3, 15, 3) };
                flowDocuments.Controls.Add(lbl);
            }
            int czyNota = SafeToInt(reader["CzyNotaRozliczona"]);
            if (czyNota == 1)
            {
                Label lbl = new Label { Text = "Nota: TAK", AutoSize = true, ForeColor = Color.Green, Margin = new Padding(0, 3, 10, 3) };
                flowDocuments.Controls.Add(lbl);
            }
        }

        private void AddDocLink(string prefix, object value)
        {
            if (value != DBNull.Value && !string.IsNullOrWhiteSpace(value.ToString()))
            {
                string numer = value.ToString();
                LinkLabel link = new LinkLabel();
                link.Text = $"{prefix}: {numer}";
                link.AutoSize = true;
                link.LinkColor = Color.FromArgb(21, 101, 192);
                link.Margin = new Padding(0, 3, 15, 3);
                link.Click += (s, e) => { Clipboard.SetText(numer); ToastManager.ShowToast("Skopiowano", $"{prefix} {numer}", NotificationType.Info); };
                flowDocuments.Controls.Add(link);
            }
        }

        private async Task LoadChatRightPanel()
        {
            flowChatRight.SuspendLayout();
            flowChatRight.Controls.Clear();
            try
            {
                if (this.idZgloszeniaInt == 0) return;
                var messages = await Task.Run(() => _repo.GetHistoryForThread(null, this.idZgloszeniaInt));
                if (messages.Count == 0) flowChatRight.Controls.Add(new Label { Text = "Brak wiadomości.", AutoSize = false, Width = flowChatRight.Width, TextAlign = ContentAlignment.MiddleCenter, ForeColor = Color.Gray, Padding = new Padding(0, 20, 0, 0) });

                foreach (var msg in messages)
                {
                    bool isIncoming = (msg.Kierunek == "IN");
                    string typeCode = "UNKNOWN";
                    string t = (msg.Typ ?? "").ToUpper();
                    if (t.Contains("SMS")) typeCode = "SMS"; else if (t.Contains("MAIL") || t.Contains("MULTI")) typeCode = "MAIL"; else if (t.Contains("ALLEGRO")) typeCode = "ALLEGRO";

                    string textToDisplay = msg.Tresc ?? "";
                    if (textToDisplay.TrimStart().StartsWith("{\\rtf")) textToDisplay = StripRtf(textToDisplay);
                    if (textToDisplay.Contains("<br>") || textToDisplay.Contains("<div>")) { textToDisplay = textToDisplay.Replace("<br>", "\n").Replace("<br/>", "\n"); textToDisplay = Regex.Replace(textToDisplay, "<.*?>", string.Empty); }
                    if (textToDisplay.Length > 600) textToDisplay = textToDisplay.Substring(0, 600) + "\n[...]";

                    ChatBubble bubble = new ChatBubble(textToDisplay, msg.Data.ToString("dd.MM HH:mm"), isIncoming, typeCode);

                    if (typeCode == "MAIL")
                    {
                        bubble.Tag = msg.Tresc; bubble.Cursor = Cursors.Hand;
                        bubble.DoubleClick += (s, e) => { int.TryParse(msg.Id, out int idW); string uid = _repo.PobierzMessageIdPoId(idW); if (string.IsNullOrEmpty(uid)) uid = msg.Id; new FormPodgladEmail(bubble.Tag.ToString(), uid, $"Wiadomość z {msg.Data}").Show(); };
                    }
                    flowChatRight.Controls.Add(bubble);
                }
                if (flowChatRight.Controls.Count > 0) flowChatRight.ScrollControlIntoView(flowChatRight.Controls[flowChatRight.Controls.Count - 1]);
            }
            catch (Exception ex) { Console.WriteLine(ex.Message); }
            finally { flowChatRight.ResumeLayout(); ResizeBubbles(flowChatRight); }
        }

        private void StyleTrackingGrid()
        {
            var dgv = dgvHistoriaPrzesylki;
            dgv.BorderStyle = BorderStyle.None;
            dgv.BackgroundColor = Color.White;
            dgv.CellBorderStyle = DataGridViewCellBorderStyle.SingleHorizontal;
            dgv.ColumnHeadersBorderStyle = DataGridViewHeaderBorderStyle.None;
            dgv.EnableHeadersVisualStyles = false;
            dgv.RowHeadersVisible = false;
            dgv.AllowUserToResizeRows = false;
            dgv.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            dgv.MultiSelect = false;
            dgv.ReadOnly = true;

            dgv.Font = new Font("Segoe UI", 10F);
            dgv.ColumnHeadersHeight = 50;
            dgv.RowTemplate.Height = 45;

            dgv.ColumnHeadersDefaultCellStyle.BackColor = Color.FromArgb(21, 101, 192);
            dgv.ColumnHeadersDefaultCellStyle.ForeColor = Color.White;
            dgv.ColumnHeadersDefaultCellStyle.Font = new Font("Segoe UI", 10F, FontStyle.Bold);
            dgv.ColumnHeadersDefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleLeft;
            dgv.ColumnHeadersDefaultCellStyle.Padding = new Padding(15, 0, 0, 0);

            dgv.DefaultCellStyle.BackColor = Color.White;
            dgv.DefaultCellStyle.ForeColor = Color.FromArgb(64, 64, 64);
            dgv.DefaultCellStyle.SelectionBackColor = Color.FromArgb(227, 242, 253);
            dgv.DefaultCellStyle.SelectionForeColor = Color.Black;
            dgv.DefaultCellStyle.Padding = new Padding(15, 0, 0, 0);
            dgv.DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleLeft;

            dgv.AlternatingRowsDefaultCellStyle.BackColor = Color.FromArgb(250, 252, 255);

            if (dgv.Columns.Count > 0)
            {
                if (dgv.Columns.Contains("DataStatusu"))
                {
                    dgv.Columns["DataStatusu"].HeaderText = "📅 Data i Godzina";
                    dgv.Columns["DataStatusu"].Width = 180;
                }
                if (dgv.Columns.Contains("OpisStatusu"))
                {
                    dgv.Columns["OpisStatusu"].HeaderText = "📝 Status Przesyłki";
                    dgv.Columns["OpisStatusu"].AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;
                }
                if (dgv.Columns.Contains("Oddzial"))
                {
                    dgv.Columns["Oddzial"].HeaderText = "🏢 Oddział DPD";
                    dgv.Columns["Oddzial"].Width = 250;
                }
            }
            dgv.ClearSelection();
        }

        private string StripRtf(string rtfString) { try { using (RichTextBox rtb = new RichTextBox()) { rtb.Rtf = rtfString; return rtb.Text; } } catch { return rtfString; } }
        private async Task OdswiezPrzyciskMagazynu() { var stan = await _magazynService.PobierzStanAsync(this.nrZgloszenia); if (stan != null) { btnMagazyn.Text = $"Magazyn: {stan.StatusFizyczny}"; if (stan.CzyDawca) btnMagazyn.BackColor = Color.Purple; else if (stan.StatusFizyczny.Contains("Odesłany")) btnMagazyn.BackColor = Color.Gray; else btnMagazyn.BackColor = Color.ForestGreen; } else { btnMagazyn.Text = "Przyjmij na Magazyn"; btnMagazyn.BackColor = Color.FromArgb(100, 100, 100); } }
        private async void btnMagazyn_Click(object sender, EventArgs e) { string model = "Nieznany"; try { using (var con = DatabaseHelper.GetConnection()) { await con.OpenAsync(); var cmd = new MySqlCommand("SELECT NazwaSystemowa FROM Produkty WHERE Id=@id", con); cmd.Parameters.AddWithValue("@id", this.nrProduktu); var res = await cmd.ExecuteScalarAsync(); if (res != null) model = res.ToString(); } } catch { } using (var form = new FormMagazynAction(this.nrZgloszenia, this.nrProduktu, model, this.nrSeryjnyZgloszenia, this.kategoriaProduktu)) { form.ShowDialog(this); if (form.CzyZmieniono) { await OdswiezPrzyciskMagazynu(); await LoadData(); } } }
        private void EditAction(int actionId) { using (var editForm = new FormEditAction(actionId, "", this.nrZgloszenia)) { if (editForm.ShowDialog(this) == DialogResult.OK) _ = LoadData(); } }
        private async Task DeleteActionAsync(int actionId) { if (MessageBox.Show("Usunąć działanie?", "Potwierdzenie", MessageBoxButtons.YesNo) == DialogResult.Yes) { await _dbService.ExecuteNonQueryAsync("DELETE FROM Dzialania WHERE Id = @id", new MySqlParameter("@id", actionId)); await new DziennikLogger().DodajAsync(Program.fullName, "Usunięto działanie", this.nrZgloszenia); await LoadData(); } }

        private async Task ShowTrackingDetails(string trackingNumber)
        {
            try
            {
                splitContainerMain.Visible = false;
                pnlShipmentHistory.Visible = true;
                pnlShipmentHistory.BackColor = Color.White;
                pnlShipmentHistory.Padding = new Padding(20);

                btnBackToDetails.Text = "← WRÓĆ DO ZGŁOSZENIA";
                btnBackToDetails.BackColor = Color.FromArgb(240, 240, 240);
                btnBackToDetails.ForeColor = Color.Black;
                btnBackToDetails.FlatStyle = FlatStyle.Flat;
                btnBackToDetails.FlatAppearance.BorderSize = 0;
                btnBackToDetails.Height = 50;
                btnBackToDetails.Cursor = Cursors.Hand;
                btnBackToDetails.Font = new Font("Segoe UI", 10F, FontStyle.Bold);

                if (_trackingService == null) _trackingService = new DpdTrackingService();

                dgvHistoriaPrzesylki.DataSource = null;
                var history = await _trackingService.GetShipmentHistoryAsync(trackingNumber);

                dgvHistoriaPrzesylki.DataSource = history;
                StyleTrackingGrid();

                if (history == null || history.Count == 0)
                {
                    MessageBox.Show("Brak historii dla tego numeru przesyłki lub numer jest niepoprawny.", "DPD Info", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Błąd pobierania statusu DPD:\n" + ex.Message, "Błąd", MessageBoxButtons.OK, MessageBoxIcon.Error);
                btnBackToDetails_Click(null, null);
            }
        }

        private void btnBackToDetails_Click(object sender, EventArgs e) { pnlShipmentHistory.Visible = false; splitContainerMain.Visible = true; }
        private async void UruchomAkcje(Form formularzAkcji) { formularzAkcji.ShowDialog(this); await LoadData(); await LoadChatRightPanel(); UpdateFilesButton(); }
        private async Task<string> GetClientNameByIdAsync(int clientId) { if (clientId <= 0) return "Nieznany"; var result = await _dbService.ExecuteScalarAsync("SELECT ImieNazwisko FROM Klienci WHERE Id = @id", new MySqlParameter("@id", clientId)); return result?.ToString() ?? "Nieznany"; }
        private async void OnChangeClientRequested(object sender, EventArgs e) { using (var form3 = new Form3(this.nrKlienta, true)) { if (form3.ShowDialog() == DialogResult.OK && form3.NowoWybranyKlientId.HasValue && form3.NowoWybranyKlientId.Value != this.nrKlienta) { await _dbService.ExecuteNonQueryAsync("UPDATE Zgloszenia SET KlientID = @nid WHERE NrZgloszenia = @nr", new MySqlParameter("@nid", form3.NowoWybranyKlientId.Value), new MySqlParameter("@nr", this.nrZgloszenia)); await LoadData(); } } }
        private void OnEditProductRequested(object sender, int produktId) { if (produktId > 0) UruchomAkcje(new Form15(produktId.ToString())); }
        private async void btnZapiszOpis_Click(object sender, EventArgs e) { await _dbService.ExecuteNonQueryAsync("UPDATE Zgloszenia SET OpisUsterki = @opis WHERE NrZgloszenia = @nr", new MySqlParameter("@opis", textBox1.Text), new MySqlParameter("@nr", this.nrZgloszenia)); await new DziennikLogger().DodajAsync(Program.fullName, "Zaktualizowano opis usterki", this.nrZgloszenia); originalOpisUsterki = textBox1.Text; btnZapiszOpis.Visible = false; }
        private void UpdateFilesButton() { string targetFolder = Path.Combine(AppPaths.GetDataRootPath(), this.nrZgloszenia.Replace('/', '.')); int count = Directory.Exists(targetFolder) ? Directory.GetFiles(targetFolder).Length : 0; button9.Text = $"  📂 Zobacz Pliki ({count})"; }

        private void btnPrintToPdf_Click(object sender, EventArgs e) { try { string path = Path.Combine(AppPaths.GetDataRootPath(), $"Zgloszenie_{this.nrZgloszenia.Replace('/', '_')}.pdf"); CreatePdf(path); Process.Start(new ProcessStartInfo(path) { UseShellExecute = true }); } catch (Exception ex) { MessageBox.Show("Błąd PDF: " + ex.Message); } }

        private void CreatePdf(string filePath)
        {
            PdfDocument document = new PdfDocument();
            document.Info.Title = $"Zgłoszenie {this.nrZgloszenia}";
            PdfPage page = document.AddPage();
            page.Orientation = PdfSharp.PageOrientation.Landscape;

            XGraphics gfx = XGraphics.FromPdfPage(page);
            XTextFormatter tf = new XTextFormatter(gfx);

            XFont fontTitle = new XFont("Arial", 16, XFontStyleEx.Bold);
            XFont fontHeader = new XFont("Arial", 12, XFontStyleEx.Bold);
            XFont fontRegular = new XFont("Arial", 10);
            XFont fontSmall = new XFont("Arial", 8);
            XFont fontBoldSmall = new XFont("Arial", 10, XFontStyleEx.Bold);

            double leftMargin = 40;
            double topMargin = 40;
            double bottomMargin = 40;
            double rightMargin = page.Width.Point - 40;
            double contentWidth = rightMargin - leftMargin;
            double currentY = topMargin;

            gfx.DrawString(lblHeaderTitle.Text, fontTitle, XBrushes.Black, new XRect(0, currentY, page.Width.Point, 0), XStringFormats.TopCenter);
            currentY += 25;
            gfx.DrawString(lblHeaderStatus.Text, fontRegular, XBrushes.DarkSlateGray, new XRect(0, currentY, page.Width.Point, 0), XStringFormats.TopCenter);
            currentY += 40;

            double columnWidth = contentWidth / 2 - 10;
            double productDataX = leftMargin + columnWidth + 20;
            double columnsStartY = currentY;

            var clientData = clientInfoControl1.GetDataForPrinting();
            var productData = productInfoControl1.GetDataForPrinting();

            double clientH = DrawSectionAndReturnHeight(gfx, "DANE KLIENTA", leftMargin, columnsStartY, columnWidth, fontHeader, fontRegular, clientData);
            double productH = DrawSectionAndReturnHeight(gfx, "DANE PRODUKTU", productDataX, columnsStartY, columnWidth, fontHeader, fontRegular, productData);

            currentY = columnsStartY + Math.Max(clientH, productH) + 30;

            gfx.DrawString("OPIS USTERKI:", fontHeader, XBrushes.Black, leftMargin, currentY);
            currentY += 20;

            string opisText = textBox1.Text;
            double opisHeight = CalculateWrappedTextHeight(gfx, opisText, fontRegular, contentWidth);

            if (currentY + opisHeight > page.Height.Point - bottomMargin)
            {
                page = document.AddPage();
                page.Orientation = PdfSharp.PageOrientation.Landscape;
                gfx = XGraphics.FromPdfPage(page);
                tf = new XTextFormatter(gfx);
                currentY = topMargin;
            }

            tf.DrawString(opisText, fontRegular, XBrushes.Black, new XRect(leftMargin, currentY, contentWidth, opisHeight + 10), XStringFormats.TopLeft);
            currentY += opisHeight + 20;

            gfx.DrawString("HISTORIA ZGŁOSZENIA:", fontHeader, XBrushes.Black, leftMargin, currentY);
            currentY += 25;

            foreach (Control ctrl in flowLayoutPanelHistory.Controls)
            {
                if (ctrl is TimelineItemControl item)
                {
                    string dateText = item.Controls.Find("lblDate", true).FirstOrDefault()?.Text ?? "";
                    string headerText = item.Controls.Find("lblHeader", true).FirstOrDefault()?.Text ?? "";
                    string contentText = item.Controls.Find("lblContent", true).FirstOrDefault()?.Text ?? "";

                    string fullHeader = $"{dateText} | {headerText}";

                    double headerH = CalculateWrappedTextHeight(gfx, fullHeader, fontBoldSmall, contentWidth);
                    double contentH = CalculateWrappedTextHeight(gfx, contentText, fontRegular, contentWidth);
                    double totalItemHeight = headerH + contentH + 15;

                    if (currentY + totalItemHeight > page.Height.Point - bottomMargin)
                    {
                        page = document.AddPage();
                        page.Orientation = PdfSharp.PageOrientation.Landscape;
                        gfx = XGraphics.FromPdfPage(page);
                        tf = new XTextFormatter(gfx);
                        currentY = topMargin;

                        gfx.DrawString("(cd. Historii)", fontSmall, XBrushes.Gray, leftMargin, currentY - 15);
                    }

                    var headerRect = new XRect(leftMargin, currentY, contentWidth, headerH);
                    tf.DrawString(fullHeader, fontBoldSmall, XBrushes.DarkBlue, headerRect, XStringFormats.TopLeft);
                    currentY += headerH;

                    var contentRect = new XRect(leftMargin, currentY, contentWidth, contentH);
                    tf.DrawString(contentText, fontRegular, XBrushes.Black, contentRect, XStringFormats.TopLeft);
                    currentY += contentH + 5;

                    gfx.DrawLine(XPens.LightGray, leftMargin, currentY, rightMargin, currentY);
                    currentY += 10;
                }
            }

            document.Save(filePath);
        }

        private double CalculateWrappedTextHeight(XGraphics gfx, string text, XFont font, double maxWidth)
        {
            if (string.IsNullOrWhiteSpace(text)) return 0;

            var lines = text.Split(new[] { "\r\n", "\r", "\n" }, StringSplitOptions.None);
            double totalHeight = 0;
            double lineHeight = font.GetHeight();

            foreach (var line in lines)
            {
                if (string.IsNullOrWhiteSpace(line))
                {
                    totalHeight += lineHeight;
                    continue;
                }

                double lineWidth = gfx.MeasureString(line, font).Width;

                if (lineWidth <= maxWidth)
                {
                    totalHeight += lineHeight;
                }
                else
                {
                    int linesCount = (int)Math.Ceiling(lineWidth / maxWidth);
                    totalHeight += linesCount * lineHeight;
                }
            }
            return totalHeight + 5;
        }

        private double DrawSectionAndReturnHeight(XGraphics gfx, string title, double x, double y, double width, XFont titleFont, XFont regularFont, Dictionary<string, string> data)
        {
            double startY = y;
            XTextFormatter tf = new XTextFormatter(gfx);

            gfx.DrawString(title, titleFont, XBrushes.Black, x, y);
            y += 20;

            foreach (var entry in data)
            {
                if (!string.IsNullOrEmpty(entry.Value))
                {
                    string line = $"{entry.Key}: {entry.Value}";
                    double height = CalculateWrappedTextHeight(gfx, line, regularFont, width);

                    var rect = new XRect(x, y, width, height + 5);
                    tf.DrawString(line, regularFont, XBrushes.Black, rect, XStringFormats.TopLeft);

                    y += height;
                }
            }
            return y - startY;
        }

        private async Task PopulateQuickActionsMenu() { _quickActionsMenu.Items.Clear(); var dt = await _dbService.GetDataTableAsync("SELECT Tresc FROM SzablonyDzialan ORDER BY Kolejnosc"); foreach (DataRow row in dt.Rows) _quickActionsMenu.Items.Add(row["Tresc"].ToString(), null, OnQuickAction_Click); _quickActionsMenu.Items.Add(new ToolStripSeparator()); _quickActionsMenu.Items.Add("Zarządzaj szablonami...", null, (s, e) => { new FormSzablonyDzialan().ShowDialog(); _ = PopulateQuickActionsMenu(); }); btnAddAction.ContextMenuStrip = _quickActionsMenu; }
        private void btnAddAction_Click(object sender, EventArgs e) { using (var f = new FormDodajDzialanie(this.nrZgloszenia)) if (f.ShowDialog() == DialogResult.OK) _ = LoadData(); }
        private void btnAddAction_MouseHover(object sender, EventArgs e) { btnAddAction.ContextMenuStrip?.Show(btnAddAction, 0, btnAddAction.Height); }
        private async void OnQuickAction_Click(object sender, EventArgs e) { if (sender is ToolStripMenuItem item) { new Dzialaniee().DodajNoweDzialanie(nrZgloszenia, Program.fullName, item.Text); await LoadData(); } }
        private void btnAllegroModule_Click(object sender, EventArgs e) { if (!string.IsNullOrEmpty(_allegroDisputeId)) { using (var f = new FormAllegroIssue(_allegroDisputeId)) { f.ShowDialog(this); _ = LoadData(); } } else MessageBox.Show("To zgłoszenie nie jest powiązane z Allegro.", "Informacja"); }
        private void buttonWyslijMail_Click(object sender, EventArgs e) => UruchomAkcje(new Form4(this.nrZgloszenia));
        private void button1_Click(object sender, EventArgs e) => UruchomAkcje(new Form5(this.nrZgloszenia, TrybZamowieniaKuriera.WysylkaDoKlienta));
        private void zamowOdKlientaMenuItem_Click(object sender, EventArgs e) => UruchomAkcje(new Form5(this.nrZgloszenia, TrybZamowieniaKuriera.OdbiorOdKlienta));
        private void zamowDoKlientaMenuItem_Click(object sender, EventArgs e) => UruchomAkcje(new Form5(this.nrZgloszenia, TrybZamowieniaKuriera.WysylkaDoKlienta));
        private void button3_Click(object sender, EventArgs e) => UruchomAkcje(new Form11(this.nrZgloszenia));
        private async void button4_Click(object sender, EventArgs e) { using (var f = new Form12(this.nrZgloszenia)) f.ShowDialog(this); await LoadData(); if (!string.IsNullOrEmpty(_allegroDisputeId) && MessageBox.Show("Otworzyć Allegro?", "Pytanie", MessageBoxButtons.YesNo) == DialogResult.Yes) using (var a = new FormAllegroIssue(_allegroDisputeId)) a.ShowDialog(this); }
        private void button5_Click(object sender, EventArgs e) => UruchomAkcje(new WRLForm(this.nrZgloszenia));
        private void button6_Click(object sender, EventArgs e) => UruchomAkcje(new KWZForm(this.nrZgloszenia));
        private void button7_Click(object sender, EventArgs e) => UruchomAkcje(new FakturaForm(this.nrZgloszenia));
        private void button8_Click(object sender, EventArgs e) { new FormUploader(this.nrZgloszenia, PhoneApiClient.Instance).Show(); }
        private void button9_Click(object sender, EventArgs e) => new FormFileViewer(this.nrZgloszenia).ShowDialog(this);
        private void button11_Click(object sender, EventArgs e) => new FormDpdTracking().ShowDialog(this);

        private async void btnFetchPart_Click(object sender, EventArgs e)
        {
            using (var formSzukaj = new FormWybierzCzesc())
            {
                if (formSzukaj.ShowDialog() == DialogResult.OK && formSzukaj.WybranaCzesc != null)
                {
                    var czesc = formSzukaj.WybranaCzesc;
                    await _magazynService.UzyjCzescAsync(czesc.Id, this.nrZgloszenia);

                    string logBiorca = $"NAPRAWA: Zamontowano część '{czesc.NazwaCzesci}' pochodzącą z dawcy: {czesc.ModelDawcy} (Zgł. {czesc.ZgloszenieDawcy}).";
                    await new DziennikLogger().DodajAsync(Program.fullName, logBiorca, this.nrZgloszenia);
                    new Dzialaniee().DodajNoweDzialanie(this.nrZgloszenia, Program.fullName, logBiorca);

                    if (!string.IsNullOrEmpty(czesc.ZgloszenieDawcy))
                    {
                        string logDawca = $"MAGAZYN: Część '{czesc.NazwaCzesci}' została pobrana i użyta do naprawy zgłoszenia {this.nrZgloszenia}.";
                        await new DziennikLogger().DodajAsync(Program.fullName, logDawca, czesc.ZgloszenieDawcy);
                        new Dzialaniee().DodajNoweDzialanie(czesc.ZgloszenieDawcy, Program.fullName, logDawca);
                    }

                    MessageBox.Show("Część przypisana do zgłoszenia.", "Sukces", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    await LoadData();
                }
            }
        }

        private struct TimelineEvent : IComparable<TimelineEvent>
        {
            public DateTime EventDate { get; set; }
            public string Content { get; set; }
            public object Tag { get; set; }
            public string Author { get; set; }
            public bool IsReminder { get; set; }
            public int CompareTo(TimelineEvent other) => other.EventDate.CompareTo(this.EventDate);
        }

        private TimelineItemType DetermineEventType(string text, string author)
        {
            string t = text.ToUpper();
            if (t.Contains("DPD") || t.Contains("KURIER")) return TimelineItemType.Courier;
            if (t.Contains("ZMIANA STATUSU")) return TimelineItemType.Status;
            if (t.Contains("WIADOMOŚĆ") || author.ToUpper().Contains("ALLEGRO")) return TimelineItemType.Message;
            if (t.Contains("WRL") || t.Contains("KWZ")) return TimelineItemType.Document;
            return TimelineItemType.Action;
        }

        private void EnableSpellCheckOnAllTextBoxes()
        {
            try
            {
                foreach (Control control in GetAllControls(this))
                {
                    if (control is RichTextBox richTextBox)
                    {
                        richTextBox.EnableSpellCheck(true);
                    }
                    else if (control is TextBox textBox && !(textBox is SpellCheckTextBox))
                    {
                        textBox.EnableSpellCheck(false);
                    }
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Błąd włączania sprawdzania pisowni: {ex.Message}");
            }
        }

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