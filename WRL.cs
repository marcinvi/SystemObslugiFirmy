using System;
using MySql.Data.MySqlClient;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Reklamacje_Dane
{
    public partial class WRLForm : Form
    {
        private readonly string nrZgloszenia;
        private readonly DatabaseService _dbService;

        public WRLForm(string nrZgloszenia)
        {
            InitializeComponent();
            this.StartPosition = FormStartPosition.CenterScreen;
            this.nrZgloszenia = nrZgloszenia;
            _dbService = new DatabaseService(DatabaseHelper.GetConnectionString());
        }

        private void WRLForm_Load(object sender, EventArgs e) { }
        private void label2_Click(object sender, EventArgs e) { }

        private async void btnLogin_Click(object sender, EventArgs e)
        {
            string numerWRL = WRL.Text.Trim();
            if (string.IsNullOrEmpty(numerWRL))
            {
                ToastManager.ShowToast("Błąd walidacji", "Proszę wprowadzić numer WRL.", NotificationType.Warning);
                return;
            }

            try
            {
                // Użycie transakcji do zapewnienia spójności danych
                using (var con = DatabaseHelper.GetConnection())
                {
                    await con.OpenAsync();
                    using (var transaction = con.BeginTransaction())
                    {
                        try
                        {
                            // 1. Aktualizacja tabeli Zgloszenia
                            string query = "UPDATE Zgloszenia SET NrWRL = @nrWRL WHERE NrZgloszenia = @nrZgloszenia";
                            using (var cmd = new MySqlCommand(query, con, transaction))
                            {
                                cmd.Parameters.AddWithValue("@nrWRL", numerWRL);
                                cmd.Parameters.AddWithValue("@nrZgloszenia", this.nrZgloszenia);
                                await cmd.ExecuteNonQueryAsync();
                            }

                            // 2. Dodanie wpisu do dziennika
                            var dziennik = new DziennikLogger();
                            await dziennik.DodajAsync(con, transaction, Program.fullName, $"Dodano numer WRL: {numerWRL}", this.nrZgloszenia);

                            // 3. Dodanie wpisu do działań
                            var dzialanie = new Dzialaniee();
                            dzialanie.DodajNoweDzialanie(con, transaction, nrZgloszenia, Program.fullName, $"Dodano numer WRL: {numerWRL}");

                            transaction.Commit();
                            ToastManager.ShowToast("Sukces", $"Numer WRL {numerWRL} został pomyślnie zapisany.", NotificationType.Success);
                        }
                        catch (Exception ex)
                        {
                            transaction.Rollback();
                            ToastManager.ShowToast("Błąd zapisu", $"Wystąpił błąd podczas zapisu numeru WRL: {ex.Message}", NotificationType.Error);
                            return;
                        }
                    }
                }

                UpdateManager.NotifySubscribers();
                this.Close();
            }
            catch (Exception ex)
            {
                ToastManager.ShowToast("Błąd połączenia", $"Wystąpił błąd podczas łączenia z bazą danych: {ex.Message}", NotificationType.Error);
            }
        }
    }
}