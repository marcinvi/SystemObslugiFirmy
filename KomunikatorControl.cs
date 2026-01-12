// ############################################################################
// Plik: KomunikatorControl.cs (WERSJA OSTATECZNA, POPRAWIONA)
// Opis: Łączy logikę unikania "ATTACH DATABASE" z istniejącą strukturą
//       kontrolek (MessageBubbleControl) i bazy danych (tabela Wiadomosci).
// ############################################################################

using System;
using System.Collections.Generic;
using System.Data;
using System.Threading.Tasks;
using System.Windows.Forms;
using MySql.Data.MySqlClient;

namespace Reklamacje_Dane
{
    public partial class KomunikatorControl : UserControl
    {
        private readonly DatabaseService _dbServiceBaza;
        private readonly DatabaseService _dbServiceMagazyn;
        private readonly string _parentFullName;
        private readonly string _userRole;

        public KomunikatorControl(DatabaseService dbServiceBaza, DatabaseService dbServiceMagazyn, string parentFullName, string userRole)
        {
            InitializeComponent();
            _dbServiceBaza = dbServiceBaza;
            _dbServiceMagazyn = dbServiceMagazyn;
            _parentFullName = parentFullName;
            _userRole = userRole;

            // Upewnij się, że w pliku KomunikatorControl.Designer.cs masz zdarzenie Click dla przycisku btnNowaWiadomosc
            // Ta linia jest kluczowa, aby przycisk działał.
            this.btnNowaWiadomosc.Click += btnNowaWiadomosc_Click;
        }

        private async void KomunikatorControl_Load(object sender, EventArgs e)
        {
            if (!this.DesignMode)
            {
                await LoadMessagesAsync();
            }
        }

        public async Task LoadMessagesAsync()
        {
            flowLayoutPanelMessages.Controls.Clear();
            try
            {
                // === POPRAWKA: Rozdzielenie zapytań, aby uniknąć ATTACH ===

                // KROK 1: Pobierz wszystkich użytkowników z Baza.db do słownika w pamięci.
                // Używamy Id jako klucza, bo tak jest w tabeli Wiadomosci (kolumna NadawcaId).
                var usersDict = new Dictionary<long, string>();
                var usersDt = await _dbServiceBaza.GetDataTableAsync("SELECT Id, \"Nazwa Wyświetlana\" FROM Uzytkownicy");
                foreach (DataRow row in usersDt.Rows)
                {
                    usersDict[Convert.ToInt64(row["Id"])] = row["Nazwa Wyświetlana"].ToString();
                }

                // KROK 2: Pobierz wiadomości z bazy Magazyn.db (tabela Wiadomosci).
                string query = @"
                    SELECT Id, Tytul, Tresc, DataWyslania, NadawcaId,
                           CzyOdczytana, CzyOdpowiedziano, DotyczyZwrotuId
                    FROM Wiadomosci
                    WHERE OdbiorcaId = @currentUserId
                    ORDER BY DataWyslania DESC
                ";
                var parameters = new MySqlParameter("@currentUserId", SessionManager.CurrentUserId);
                var messagesDt = await _dbServiceMagazyn.GetDataTableAsync(query, parameters);

                // KROK 3: Połącz dane w aplikacji, używając Twojej kontrolki MessageBubbleControl.
                foreach (DataRow row in messagesDt.Rows)
                {
                    var bubble = new MessageBubbleControl
                    {
                        Width = flowLayoutPanelMessages.ClientSize.Width - 25,
                        MessageId = Convert.ToInt32(row["Id"]),
                        SenderId = Convert.ToInt32(row["NadawcaId"]),
                        MessageTitle = row["Tytul"]?.ToString(),
                        ReturnId = row["DotyczyZwrotuId"] != DBNull.Value ? (int?)Convert.ToInt32(row["DotyczyZwrotuId"]) : null
                    };

                    long senderId = Convert.ToInt64(row["NadawcaId"]);
                    // Jeśli z jakiegoś powodu użytkownika nie ma w słowniku, wyświetl "Nieznany".
                    string senderName = usersDict.ContainsKey(senderId) ? usersDict[senderId] : "Nieznany nadawca";

                    bubble.SetMessageData(
                        senderName,
                        row["Tresc"].ToString(),
                        Convert.ToDateTime(row["DataWyslania"]),
                        Convert.ToInt32(row["CzyOdczytana"]) == 1,
                        Convert.ToInt32(row["CzyOdpowiedziano"]) == 1
                    );

                    bubble.DoubleClick += MessageBubble_DoubleClick;
                    bubble.ReplyClicked += MessageBubble_ReplyClicked;
                    foreach (Control control in bubble.Controls)
                    {
                        control.DoubleClick += (s, e) => MessageBubble_DoubleClick(bubble, e);
                    }

                    flowLayoutPanelMessages.Controls.Add(bubble);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Błąd podczas ładowania wiadomości: " + ex.Message, "Błąd", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private async void MessageBubble_DoubleClick(object sender, EventArgs e)
        {
            if (!(sender is MessageBubbleControl bubble) || !bubble.ReturnId.HasValue) return;

            await MarkAsRead(bubble.MessageId);

            Form formToOpen = null;
            if (_userRole == "Magazyn")
            {
                formToOpen = new FormZwrotSzczegoly(bubble.ReturnId.Value, _parentFullName);
            }
            else if (_userRole == "Handlowiec")
            {
                formToOpen = new FormHandlowiecSzczegoly(bubble.ReturnId.Value, _parentFullName);
            }

            if (formToOpen != null)
            {
                formToOpen.ShowDialog(this.ParentForm);
                formToOpen.Dispose();
                await LoadMessagesAsync();
            }
        }

        private async void MessageBubble_ReplyClicked(object sender, EventArgs e)
        {
            if (!(sender is MessageBubbleControl bubble)) return;

            await MarkAsRead(bubble.MessageId);

            using (var form = new FormNowaWiadomosc(bubble.MessageId, bubble.SenderId, bubble.MessageTitle, bubble.ReturnId))
            {
                if (form.ShowDialog() == DialogResult.OK)
                {
                    await LoadMessagesAsync();
                }
            }
        }

        private async Task MarkAsRead(int messageId)
        {
            await _dbServiceMagazyn.ExecuteNonQueryAsync("UPDATE Wiadomosci SET CzyOdczytana = 1 WHERE Id = @id", new MySqlParameter("@id", messageId));
        }

        private async void btnNowaWiadomosc_Click(object sender, EventArgs e)
        {
            using (var form = new FormNowaWiadomosc())
            {
                if (form.ShowDialog() == DialogResult.OK)
                {
                    await LoadMessagesAsync();
                }
            }
        }
    }
}