// Plik: WeryfikacjaControl.cs
// Wklej cały ten plik w miejsce istniejącego.
using System;
using System.Collections.Generic;
using System.Data;
using MySql.Data.MySqlClient;
using System.Drawing;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Reklamacje_Dane
{
    public partial class WeryfikacjaControl : UserControl
    {
        private readonly DatabaseService _dbServiceMagazyn;
        private readonly DatabaseService _dbServiceBaza;
        private readonly string _currentUserName;
        private int _selectedReturnId = -1;
        private Timer _refreshTimer;
        private DataRow _selectedReturnDataRow;

        private class ComboItem
        {
            public int Id { get; set; }
            public string Nazwa { get; set; }
            public override string ToString() => Nazwa;
        }

        public WeryfikacjaControl(string currentUserName)
        {
            InitializeComponent();
            _currentUserName = currentUserName;
            _dbServiceMagazyn = new DatabaseService(MagazynDatabaseHelper.GetConnectionString());
            _dbServiceBaza = new DatabaseService(DatabaseHelper.GetConnectionString());
            this.Load += WeryfikacjaControl_Load;
            this.Disposed += (s, e) => _refreshTimer?.Dispose();
        }

        private async void WeryfikacjaControl_Load(object sender, EventArgs e)
        {
            SetEditMode(false);
            panelDetails.Enabled = false;
            ApplyModernGridStyle(); // NOWOŚĆ: Aplikowanie stylów
            await LoadZadaniaDoWeryfikacjiAsync();
            await LoadHandlowcyAsync();
            InitializeContextMenuForTextBoxes();
            CreateCopyButtons();

            _refreshTimer = new Timer();
            _refreshTimer.Interval = 30000;
            _refreshTimer.Tick += async (s, ev) => await LoadZadaniaDoWeryfikacjiAsync();
            _refreshTimer.Start();
        }

        // NOWOŚĆ: Metoda do stylizacji siatki
        private void ApplyModernGridStyle()
        {
            dgvZadania.BorderStyle = BorderStyle.None;
            dgvZadania.AlternatingRowsDefaultCellStyle.BackColor = Color.FromArgb(242, 242, 242);
            dgvZadania.CellBorderStyle = DataGridViewCellBorderStyle.SingleHorizontal;
            dgvZadania.DefaultCellStyle.SelectionBackColor = Color.FromArgb(33, 150, 243);
            dgvZadania.DefaultCellStyle.SelectionForeColor = Color.White;
            dgvZadania.BackgroundColor = Color.White;
            dgvZadania.EnableHeadersVisualStyles = false;
            dgvZadania.ColumnHeadersBorderStyle = DataGridViewHeaderBorderStyle.None;
            dgvZadania.ColumnHeadersDefaultCellStyle.BackColor = Color.FromArgb(68, 114, 196);
            dgvZadania.ColumnHeadersDefaultCellStyle.ForeColor = Color.White;
            dgvZadania.ColumnHeadersDefaultCellStyle.Font = new Font("Segoe UI", 9F, FontStyle.Bold);
            dgvZadania.ColumnHeadersHeight = 30;
            dgvZadania.RowTemplate.Height = 28;
        }

        private void InitializeContextMenuForTextBoxes()
        {
            var menu = new ContextMenuStrip();
            var copyItem = new ToolStripMenuItem("Kopiuj");
            copyItem.Click += (s, e) =>
            {
                if (menu.SourceControl is TextBox tb)
                {
                    Clipboard.SetText(tb.Text);
                }
            };
            menu.Items.Add(copyItem);

            txtKlient.ContextMenuStrip = menu;
            txtAdres.ContextMenuStrip = menu;
            txtTelefon.ContextMenuStrip = menu;
            txtProdukt.ContextMenuStrip = menu;
            txtNumerFaktury.ContextMenuStrip = menu;
            txtDataDodania.ContextMenuStrip = menu;
            txtStanProduktu.ContextMenuStrip = menu;
            txtUwagiMagazynu.ContextMenuStrip = menu;
            txtListPrzewozowy.ContextMenuStrip = menu;
            txtPrzyjetyPrzez.ContextMenuStrip = menu;
        }

        private void CreateCopyButtons()
        {
            AddCopyButton(txtKlient, groupBoxInfo);
            AddCopyButton(txtAdres, groupBoxInfo);
            AddCopyButton(txtTelefon, groupBoxInfo);
            AddCopyButton(txtProdukt, groupBoxInfo);
            AddCopyButton(txtNumerFaktury, groupBoxInfo);
            AddCopyButton(txtListPrzewozowy, groupBoxDaneMagazynowe);
        }

        private void AddCopyButton(TextBox associatedTextBox, Control parentControl)
        {
            Button copyButton = new Button
            {
                Size = new Size(24, 24),
                Text = "📋",
                Font = new Font("Segoe UI Emoji", 8F),
                FlatStyle = FlatStyle.Flat,
                Tag = associatedTextBox,
                Cursor = Cursors.Hand,
                // POPRAWKA: Ustawienie kotwicy, aby przycisk nie wychodził poza widok
                Anchor = AnchorStyles.Top | AnchorStyles.Right
            };
            copyButton.FlatAppearance.BorderSize = 0;
            // POPRAWKA: Pozycjonowanie względem prawego brzegu panelu, a nie kontrolki
            copyButton.Location = new Point(parentControl.Width - 40, associatedTextBox.Top - 2);
            copyButton.Click += CopyButton_Click;

            parentControl.Controls.Add(copyButton);
            copyButton.BringToFront();
        }

        private void CopyButton_Click(object sender, EventArgs e)
        {
            if (sender is Button btn && btn.Tag is TextBox tb)
            {
                if (!string.IsNullOrEmpty(tb.Text) && tb.Text != "Brak" && tb.Text != "-")
                {
                    Clipboard.SetText(tb.Text);
                    ToastManager.ShowToast("Skopiowano", $"Skopiowano: {tb.Text}", NotificationType.Info);
                }
            }
        }

        private void SetEditMode(bool isEditing)
        {
            var textBoxes = new[] { txtKlient, txtAdres, txtTelefon, txtProdukt, txtNumerFaktury };
            foreach (var tb in textBoxes)
            {
                tb.ReadOnly = !isEditing;
                tb.BorderStyle = isEditing ? BorderStyle.Fixed3D : BorderStyle.None;
                tb.BackColor = isEditing ? Color.White : SystemColors.Control;
            }

            btnEdytuj.Visible = !isEditing;
            btnZapisz.Visible = isEditing;
            btnAnuluj.Visible = isEditing;
            groupBoxAkcja.Enabled = !isEditing;
        }

        private void btnEdytuj_Click(object sender, EventArgs e) => SetEditMode(true);

        private void btnAnuluj_Click(object sender, EventArgs e)
        {
            SetEditMode(false);
            dgvZadania_SelectionChanged(null, null);
        }

        private async void btnZapisz_Click(object sender, EventArgs e)
        {
            if (_selectedReturnId == -1) return;
            try
            {
                string[] klientParts = txtKlient.Text.Split(new[] { ' ' }, 2, StringSplitOptions.RemoveEmptyEntries);
                string imie = klientParts.Length > 0 ? klientParts[0] : "";
                string nazwisko = klientParts.Length > 1 ? klientParts[1] : "";
                string[] adresParts = txtAdres.Text.Split(new[] { ',' }, 2, StringSplitOptions.RemoveEmptyEntries);
                string ulica = adresParts.Length > 0 ? adresParts[0].Trim() : "";
                string kodMiasto = adresParts.Length > 1 ? adresParts[1].Trim() : "";
                string kodPocztowy = new string(kodMiasto.TakeWhile(c => char.IsDigit(c) || c == '-').ToArray());
                string miasto = new string(kodMiasto.SkipWhile(c => char.IsDigit(c) || c == '-').ToArray()).Trim();
                await _dbServiceMagazyn.ExecuteNonQueryAsync(@"
                    UPDATE AllegroCustomerReturns SET
                        Buyer_FirstName = @imie, Buyer_LastName = @nazwisko,
                        Buyer_Street = @ulica, Buyer_ZipCode = @kod, Buyer_City = @miasto,
                        Buyer_PhoneNumber = @telefon, ProductName = @produkt, InvoiceNumber = @faktura
                    WHERE Id = @id",
                    new MySqlParameter("@imie", imie),
                    new MySqlParameter("@nazwisko", nazwisko),
                    new MySqlParameter("@ulica", ulica),
                    new MySqlParameter("@kod", kodPocztowy),
                    new MySqlParameter("@miasto", miasto),
                    new MySqlParameter("@telefon", txtTelefon.Text),
                    new MySqlParameter("@produkt", txtProdukt.Text),
                    new MySqlParameter("@faktura", txtNumerFaktury.Text),
                    new MySqlParameter("@id", _selectedReturnId)
                );
                ToastManager.ShowToast("Sukces", "Dane zwrotu zostały zaktualizowane.", NotificationType.Success);
                SetEditMode(false);
                await LoadZadaniaDoWeryfikacjiAsync();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Błąd podczas zapisywania danych: " + ex.Message, "Błąd krytyczny", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
        private async Task LoadHandlowcyAsync()
        {
            try
            {
                var dt = await _dbServiceBaza.GetDataTableAsync("SELECT Id, `Nazwa Wyświetlana` FROM Uzytkownicy WHERE Rola = 'Handlowiec' ORDER BY `Nazwa Wyświetlana`");
                var handlowcy = new List<ComboItem>();
                foreach (DataRow row in dt.Rows)
                {
                    handlowcy.Add(new ComboItem { Id = Convert.ToInt32(row["Id"]), Nazwa = row["Nazwa Wyświetlana"].ToString() });
                }
                comboHandlowcy.DataSource = handlowcy;
                comboHandlowcy.DisplayMember = "Nazwa";
                comboHandlowcy.ValueMember = "Id";
                comboHandlowcy.SelectedIndex = -1;
            }
            catch (Exception ex)
            {
                MessageBox.Show("Błąd podczas wczytywania listy handlowców: " + ex.Message, "Błąd", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
        private async Task LoadZadaniaDoWeryfikacjiAsync()
        {
            try
            {
                int? selectedId = dgvZadania.SelectedRows.Count > 0 ? (int?)Convert.ToInt32(dgvZadania.SelectedRows[0].Cells["Id"].Value) : null;
                string query = @"SELECT acr.Id, acr.ReferenceNumber AS 'Numer Zwrotu', CONCAT(acr.Buyer_FirstName, ' ', acr.Buyer_LastName) AS Klient, acr.ProductName AS Produkt, acr.CreatedAt AS 'Data Dodania' FROM AllegroCustomerReturns acr LEFT JOIN Statusy s ON acr.StatusWewnetrznyId = s.Id WHERE s.Nazwa = 'Oczekuje na przyjęcie' AND acr.AllegroReturnId IS NULL ORDER BY acr.CreatedAt ASC";
                var dt = await _dbServiceMagazyn.GetDataTableAsync(query);
                dgvZadania.DataSource = dt;
                FormatGrid();
                if (selectedId.HasValue)
                {
                    foreach (DataGridViewRow row in dgvZadania.Rows)
                    {
                        if (Convert.ToInt32(row.Cells["Id"].Value) == selectedId.Value)
                        {
                            row.Selected = true;
                            dgvZadania.FirstDisplayedScrollingRowIndex = row.Index;
                            break;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Błąd podczas wczytywania zadań do weryfikacji: " + ex.Message, "Błąd", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
        private void FormatGrid()
        {
            if (dgvZadania.DataSource == null || dgvZadania.Columns.Count == 0) return;
            if (dgvZadania.Columns.Contains("Id")) dgvZadania.Columns["Id"].Visible = false;
            if (dgvZadania.Columns.Contains("Numer Zwrotu")) dgvZadania.Columns["Numer Zwrotu"].Width = 150;
            if (dgvZadania.Columns.Contains("Klient")) dgvZadania.Columns["Klient"].Width = 200;
            if (dgvZadania.Columns.Contains("Produkt")) dgvZadania.Columns["Produkt"].AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;
            if (dgvZadania.Columns.Contains("Data Dodania")) dgvZadania.Columns["Data Dodania"].Width = 130;
        }

        private async void dgvZadania_SelectionChanged(object sender, EventArgs e)
        {
            if (dgvZadania.SelectedRows.Count == 0)
            {
                panelDetails.Enabled = false;
                _selectedReturnId = -1;
                _selectedReturnDataRow = null;
                ClearDetails();
                return;
            }
            try
            {
                panelDetails.Enabled = true;
                _selectedReturnId = Convert.ToInt32(dgvZadania.SelectedRows[0].Cells["Id"].Value);
                var dt = await _dbServiceMagazyn.GetDataTableAsync("SELECT * FROM AllegroCustomerReturns WHERE Id = @id", new MySqlParameter("@id", _selectedReturnId));

                if (dt.Rows.Count > 0)
                {
                    _selectedReturnDataRow = dt.Rows[0];
                    txtKlient.Text = $"{_selectedReturnDataRow["Buyer_FirstName"]} {_selectedReturnDataRow["Buyer_LastName"]}";
                    txtAdres.Text = $"{_selectedReturnDataRow["Buyer_Street"]}, {_selectedReturnDataRow["Buyer_ZipCode"]} {_selectedReturnDataRow["Buyer_City"]}";
                    txtTelefon.Text = _selectedReturnDataRow["Buyer_PhoneNumber"]?.ToString();
                    txtProdukt.Text = _selectedReturnDataRow["ProductName"]?.ToString();
                    txtNumerFaktury.Text = _selectedReturnDataRow["InvoiceNumber"]?.ToString() ?? "Brak";
                    txtDataDodania.Text = Convert.ToDateTime(_selectedReturnDataRow["CreatedAt"]).ToString("dd.MM.yyyy HH:mm");
                    txtListPrzewozowy.Text = _selectedReturnDataRow["Waybill"]?.ToString() ?? "Brak";
                    txtUwagiMagazynu.Text = _selectedReturnDataRow["UwagiMagazynu"]?.ToString() ?? "Brak uwag.";
                    if (_selectedReturnDataRow["StanProduktuId"] != DBNull.Value)
                    {
                        var stanId = Convert.ToInt32(_selectedReturnDataRow["StanProduktuId"]);
                        var stanNazwa = await _dbServiceMagazyn.ExecuteScalarAsync("SELECT Nazwa FROM Statusy WHERE Id = @id", new MySqlParameter("@id", stanId));
                        txtStanProduktu.Text = stanNazwa?.ToString() ?? "Nieznany";
                    }
                    else { txtStanProduktu.Text = "Nieokreślony"; }

                    if (_selectedReturnDataRow["PrzyjetyPrzezId"] != DBNull.Value && _selectedReturnDataRow["DataPrzyjecia"] != DBNull.Value)
                    {
                        var pracownikId = Convert.ToInt32(_selectedReturnDataRow["PrzyjetyPrzezId"]);
                        var dataPrzyjecia = Convert.ToDateTime(_selectedReturnDataRow["DataPrzyjecia"]).ToString("dd.MM.yyyy HH:mm");
                        var pracownikNazwa = await _dbServiceBaza.ExecuteScalarAsync("SELECT \"Nazwa Wyświetlana\" FROM Uzytkownicy WHERE Id = @id", new MySqlParameter("@id", pracownikId));
                        txtPrzyjetyPrzez.Text = $"{pracownikNazwa?.ToString() ?? "Nieznany"} w dniu {dataPrzyjecia}";
                    }
                    else { txtPrzyjetyPrzez.Text = "Informacja niekompletna"; }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Błąd podczas wczytywania szczegółów zwrotu: " + ex.Message, "Błąd", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void ClearDetails()
        {
            txtKlient.Text = "";
            txtAdres.Text = "";
            txtTelefon.Text = "";
            txtProdukt.Text = "";
            txtNumerFaktury.Text = "";
            txtDataDodania.Text = "";
            txtStanProduktu.Text = "";
            txtUwagiMagazynu.Text = "";
            txtListPrzewozowy.Text = "";
            txtPrzyjetyPrzez.Text = "";
            comboHandlowcy.SelectedIndex = -1;
            SetEditMode(false);
        }

        private async void btnPrzypisz_Click(object sender, EventArgs e)
        {
            if (_selectedReturnId == -1)
            {
                MessageBox.Show("Proszę wybrać zwrot z listy.", "Brak zaznaczenia", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }
            if (comboHandlowcy.SelectedItem == null)
            {
                MessageBox.Show("Proszę wybrać handlowca z listy.", "Brak zaznaczenia", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            string referenceNumber = dgvZadania.SelectedRows[0].Cells["Numer Zwrotu"].Value.ToString();
            var selectedHandlowiec = (ComboItem)comboHandlowcy.SelectedItem;

            var confirm = MessageBox.Show($"Czy na pewno chcesz przypisać zwrot nr {referenceNumber} do handlowca: {selectedHandlowiec.Nazwa}?", "Potwierdzenie", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
            if (confirm == DialogResult.No) return;

            try
            {
                var statusId = await _dbServiceMagazyn.ExecuteScalarAsync("SELECT Id FROM Statusy WHERE Nazwa = 'Oczekuje na decyzję handlowca'");
                if (statusId == null) throw new Exception("Nie znaleziono w bazie statusu 'Oczekuje na decyzję handlowca'.");

                await _dbServiceMagazyn.ExecuteNonQueryAsync(
                    "UPDATE AllegroCustomerReturns SET HandlowiecOpiekunId = @handlowiecId, StatusWewnetrznyId = @statusId WHERE Id = @returnId",
                    new MySqlParameter("@handlowiecId", selectedHandlowiec.Id),
                    new MySqlParameter("@statusId", Convert.ToInt32(statusId)),
                    new MySqlParameter("@returnId", _selectedReturnId)
                );

                string trescWiadomosci = $"Przypisano Ci nowy zwrot ręczny nr {referenceNumber} do weryfikacji i podjęcia decyzji.";
                await _dbServiceMagazyn.ExecuteNonQueryAsync(@"
                    INSERT INTO Wiadomosci (NadawcaId, OdbiorcaId, Tytul, Tresc, DataWyslania, DotyczyZwrotuId)
                    VALUES (@nadawca, @odbiorca, @tytul, @tresc, @data, @zwrotId)",
                    new MySqlParameter("@nadawca", SessionManager.CurrentUserId),
                    new MySqlParameter("@odbiorca", selectedHandlowiec.Id),
                    new MySqlParameter("@tytul", $"Nowe zadanie: Zwrot ręczny {referenceNumber}"),
                    new MySqlParameter("@tresc", trescWiadomosci),
                    new MySqlParameter("@data", DateTime.Now),
                    new MySqlParameter("@zwrotId", _selectedReturnId));

                ToastManager.ShowToast("Sukces", "Zwrot został pomyślnie przypisany do handlowca.", NotificationType.Success);
                await LoadZadaniaDoWeryfikacjiAsync();
                ClearDetails();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Wystąpił błąd podczas przypisywania zadania: " + ex.Message, "Błąd krytyczny", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private async void btnPrzekazDoReklamacji_Click(object sender, EventArgs e)
        {
            if (_selectedReturnId == -1 || _selectedReturnDataRow == null)
            {
                MessageBox.Show("Proszę wybrać zwrot z listy.", "Brak zaznaczenia", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            var confirm = MessageBox.Show("Czy na pewno chcesz przekazać ten zwrot bezpośrednio do działu reklamacji?", "Potwierdzenie", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
            if (confirm == DialogResult.No) return;

            try
            {
                var statusId = await _dbServiceMagazyn.ExecuteScalarAsync("SELECT Id FROM Statusy WHERE Nazwa = 'Przekazano do reklamacji'");
                if (statusId == null) throw new Exception("Brak statusu 'Przekazano do reklamacji' w bazie.");

                await _dbServiceMagazyn.ExecuteNonQueryAsync(
                    "UPDATE AllegroCustomerReturns SET StatusWewnetrznyId = @statusId WHERE Id = @id",
                    new MySqlParameter("@statusId", statusId),
                    new MySqlParameter("@id", _selectedReturnId));

                await _dbServiceBaza.ExecuteNonQueryAsync(@"INSERT INTO NiezarejestrowaneZwrotyReklamacyjne  
                    (DataPrzekazania, PrzekazanePrzez, IdZwrotuWMagazynie, DaneKlienta, DaneProduktu, NumerFaktury, UwagiMagazynu)
                    VALUES (@data, @kto, @idZwrotu, @klient, @produkt, @fv, @uwagi)",
                    new MySqlParameter("@data", DateTime.Now),
                    new MySqlParameter("@kto", SessionManager.CurrentUserLogin),
                    new MySqlParameter("@idZwrotu", _selectedReturnId),
                    new MySqlParameter("@klient", txtKlient.Text + " | " + txtAdres.Text),
                    new MySqlParameter("@produkt", txtProdukt.Text),
                    new MySqlParameter("@fv", _selectedReturnDataRow["InvoiceNumber"]?.ToString()),
                    new MySqlParameter("@uwagi", _selectedReturnDataRow["UwagiMagazynu"]?.ToString())
                );

                ToastManager.ShowToast("Przekazano", "Zwrot został przekazany do działu reklamacji.", NotificationType.Info);
                await LoadZadaniaDoWeryfikacjiAsync();
                ClearDetails();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Wystąpił błąd podczas przekazywania do reklamacji: " + ex.Message, "Błąd krytyczny", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
    }
}