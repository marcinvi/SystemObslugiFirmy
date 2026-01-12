using System;
using System.Data;
using MySql.Data.MySqlClient;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Collections.Generic;
namespace Reklamacje_Dane
{
    public partial class FormPodsumowanieZwrotu : Form
    {
        private readonly int _returnDbId;
        private readonly DatabaseService _dbServiceMagazyn;
        private readonly DatabaseService _dbServiceBaza;
        private DataRow _dbDataRow;

        public FormPodsumowanieZwrotu(int returnDbId)
        {
            InitializeComponent();
            _returnDbId = returnDbId;
            _dbServiceMagazyn = new DatabaseService(MagazynDatabaseHelper.GetConnectionString());
            _dbServiceBaza = new DatabaseService(DatabaseHelper.GetConnectionString());
        

            // Włącz sprawdzanie pisowni dla wszystkich TextBoxów
            EnableSpellCheckOnAllTextBoxes();
        }

        private async void FormPodsumowanieZwrotu_Load(object sender, EventArgs e)
        {
            this.Cursor = Cursors.WaitCursor;
            await LoadReturnDataAsync();
            await PopulateControls();
            this.Cursor = Cursors.Default;
        }

        private async Task LoadReturnDataAsync()
        {
            try
            {
                var dt = await _dbServiceMagazyn.GetDataTableAsync("SELECT * FROM AllegroCustomerReturns WHERE Id = @id", new MySqlParameter("@id", _returnDbId));
                if (dt.Rows.Count > 0)
                {
                    _dbDataRow = dt.Rows[0];
                }
                else
                {
                    MessageBox.Show("Nie odnaleziono zwrotu o podanym ID.", "Błąd", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    this.Close();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Błąd wczytywania szczegółów zwrotu: " + ex.Message, "Błąd", MessageBoxButtons.OK, MessageBoxIcon.Error);
                this.Close();
            }
        }

        private async Task PopulateControls()
        {
            if (_dbDataRow == null) return;

            lblTitle.Text = $"Podsumowanie Zwrotu: {_dbDataRow["ReferenceNumber"]}";
            this.Text = lblTitle.Text;

            // Info ogólne
            if (_dbDataRow["AllegroAccountId"] != DBNull.Value)
            {
                lblAllegroAccount.Text = (await _dbServiceBaza.ExecuteScalarAsync("SELECT AccountName FROM AllegroAccounts WHERE Id = @id", new MySqlParameter("@id", _dbDataRow["AllegroAccountId"])))?.ToString() ?? "Nieznane";
            }
            lblBuyerLogin.Text = _dbDataRow["BuyerLogin"]?.ToString();
            lblOrderDate.Text = _dbDataRow["CreatedAt"] != DBNull.Value ? Convert.ToDateTime(_dbDataRow["CreatedAt"]).ToString("dd.MM.yyyy HH:mm") : "Brak";
            lblInvoice.Text = _dbDataRow["InvoiceNumber"]?.ToString() ?? "Brak";

            // Ocena magazynu
            if (_dbDataRow["StanProduktuId"] != DBNull.Value)
            {
                lblStanProduktu.Text = (await _dbServiceMagazyn.ExecuteScalarAsync("SELECT Nazwa FROM Statusy WHERE Id = @id", new MySqlParameter("@id", _dbDataRow["StanProduktuId"])))?.ToString() ?? "Brak";
            }
            if (_dbDataRow["PrzyjetyPrzezId"] != DBNull.Value)
            {
                lblPrzyjetyPrzez.Text = (await _dbServiceBaza.ExecuteScalarAsync("SELECT \"Nazwa Wyświetlana\" FROM Uzytkownicy WHERE Id = @id", new MySqlParameter("@id", _dbDataRow["PrzyjetyPrzezId"])))?.ToString() ?? "Brak";
            }
            lblUwagiMagazynu.Text = _dbDataRow["UwagiMagazynu"]?.ToString();
            lblDataPrzyjecia.Text = _dbDataRow["DataPrzyjecia"] != DBNull.Value ? Convert.ToDateTime(_dbDataRow["DataPrzyjecia"]).ToString("dd.MM.yyyy HH:mm") : "Brak";

            // Decyzja handlowca
            string decyzja = "Brak";
            if (_dbDataRow["DecyzjaHandlowcaId"] != DBNull.Value)
            {
                decyzja = (await _dbServiceMagazyn.ExecuteScalarAsync("SELECT Nazwa FROM Statusy WHERE Id = @id", new MySqlParameter("@id", _dbDataRow["DecyzjaHandlowcaId"])))?.ToString() ?? "Brak";
            }
            lblDecyzjaHandlowca.Text = decyzja;
            lblKomentarzHandlowca.Text = _dbDataRow["KomentarzHandlowca"]?.ToString();

            // Pokaż odpowiedni przycisk finalizujący
            if (decyzja == "Przekaż do reklamacji")
            {
                btnPrzekazanoDoReklamacji.Visible = true;
                btnArchiwizuj.Visible = false;
            }
            else
            {
                btnPrzekazanoDoReklamacji.Visible = false;
                btnArchiwizuj.Visible = true;
            }
        }

        private async void btnArchiwizuj_Click(object sender, EventArgs e)
        {
            var confirm = MessageBox.Show("Czy na pewno chcesz zarchiwizować ten zwrot? Zniknie on z aktywnej listy.", "Potwierdzenie", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
            if (confirm == DialogResult.Yes)
            {
                try
                {
                    // Zmieniamy status na archiwalny (musisz dodać taki status do tabeli Statusy)
                    var statusArchiwalnyId = await _dbServiceMagazyn.ExecuteScalarAsync("SELECT Id FROM Statusy WHERE Nazwa = 'Archiwalny'");
                    if (statusArchiwalnyId == null)
                    {
                        MessageBox.Show("Brak statusu 'Archiwalny' w bazie danych. Skonfiguruj go w ustawieniach.", "Błąd Konfiguracji", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        return;
                    }

                    await _dbServiceMagazyn.ExecuteNonQueryAsync(
                        "UPDATE AllegroCustomerReturns SET StatusWewnetrznyId = @statusId WHERE Id = @id",
                        new MySqlParameter("@statusId", statusArchiwalnyId),
                        new MySqlParameter("@id", _returnDbId));

                    ToastManager.ShowToast("Sukces", "Zwrot został zarchiwizowany.", NotificationType.Success);
                    this.DialogResult = DialogResult.OK;
                    this.Close();
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Błąd podczas archiwizacji: " + ex.Message, "Błąd", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
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