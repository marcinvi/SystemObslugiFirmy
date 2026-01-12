using System;
using MySql.Data.MySqlClient;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Reklamacje_Dane
{
    public partial class KWZForm : Form
    {
        private readonly string nrZgloszenia;
        private readonly DatabaseService _dbService;

        public KWZForm(string nrZgloszenia)
        {
            InitializeComponent();
            this.StartPosition = FormStartPosition.CenterScreen;
            this.nrZgloszenia = nrZgloszenia;
            _dbService = new DatabaseService(DatabaseHelper.GetConnectionString());
        }

        private void LoginForm_Load(object sender, EventArgs e) { }

        private async void btnLogin_Click(object sender, EventArgs e)
        {
            string numerKWZ = this.KWZ.Text.Trim();
            if (string.IsNullOrEmpty(numerKWZ))
            {
                ToastManager.ShowToast("Błąd walidacji", "Proszę wprowadzić numer KWZ 2.", NotificationType.Warning);
                return;
            }

            try
            {
                using (var con = DatabaseHelper.GetConnection())
                {
                    await con.OpenAsync();
                    using (var transaction = con.BeginTransaction())
                    {
                        try
                        {
                            // 1. Aktualizacja tabeli Zgloszenia
                            string query = "UPDATE Zgloszenia SET NrKWZ2 = @nrKWZ2 WHERE NrZgloszenia = @nrZgloszenia";
                            using (var cmd = new MySqlCommand(query, con, transaction))
                            {
                                cmd.Parameters.AddWithValue("@nrKWZ2", numerKWZ);
                                cmd.Parameters.AddWithValue("@nrZgloszenia", this.nrZgloszenia);
                                await cmd.ExecuteNonQueryAsync();
                            }

                            // 2. Dodanie wpisu do dziennika
                            var dziennik = new DziennikLogger();
                            await dziennik.DodajAsync(con, transaction, Program.fullName, $"Dodano numer KWZ 2: {numerKWZ}", this.nrZgloszenia);

                            // 3. Dodanie wpisu do działań
                            var dzialanie = new Dzialaniee();
                            dzialanie.DodajNoweDzialanie(con, transaction, nrZgloszenia, Program.fullName, $"Dodano numer KWZ 2: {numerKWZ}");

                            transaction.Commit();
                            ToastManager.ShowToast("Sukces", $"Numer KWZ 2 {numerKWZ} został pomyślnie zapisany.", NotificationType.Success);
                        }
                        catch (Exception ex)
                        {
                            transaction.Rollback();
                            ToastManager.ShowToast("Błąd zapisu", $"Wystąpił błąd podczas zapisu numeru KWZ 2: {ex.Message}", NotificationType.Error);
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