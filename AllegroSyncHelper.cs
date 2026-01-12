// ############################################################################
// Plik: AllegroSyncHelper.cs (BRAKUJĄCY PLIK)
// Opis: Klasa odpowiedzialna za synchronizację kont Allegro przy starcie.
// ############################################################################

using System;
using System.Data;
using MySql.Data.MySqlClient;
using System.Drawing;
using System.Threading.Tasks;

namespace Reklamacje_Dane
{
    public static class AllegroSyncHelper
    {
        public struct SyncReport
        {
            public string Message;
            public Color Color;
        }

        public static async Task PerformInitialSyncAsync(IProgress<SyncReport> progress)
        {
            var dbService = new DatabaseService(DatabaseHelper.GetConnectionString());
            progress.Report(new SyncReport { Message = "Pobieram listę autoryzowanych kont Allegro...", Color = Color.Black });
            DataTable accountsDt;
            try
            {
                accountsDt = await dbService.GetDataTableAsync("SELECT Id, AccountName FROM AllegroAccounts WHERE IsAuthorized = 1");
            }
            catch (Exception ex)
            {
                progress.Report(new SyncReport { Message = $"BŁĄD: Nie udało się połączyć z bazą danych: {ex.Message}", Color = Color.DarkRed });
                return;
            }

            if (accountsDt.Rows.Count == 0)
            {
                progress.Report(new SyncReport { Message = "Nie znaleziono autoryzowanych kont Allegro.", Color = Color.Orange });
                return;
            }

            using (var con = Database.GetNewOpenConnection())
            {
                foreach (DataRow row in accountsDt.Rows)
                {
                    int accountId = Convert.ToInt32(row["Id"]);
                    string accountName = row["AccountName"].ToString();
                    progress.Report(new SyncReport { Message = $"Sprawdzam konto: {accountName}...", Color = Color.Black });

                    try
                    {
                        var apiClient = await DatabaseHelper.GetApiClientForAccountAsync(accountId, con);
                        if (apiClient != null)
                        {
                            progress.Report(new SyncReport { Message = $"OK: Token dla konta '{accountName}' jest aktualny.", Color = Color.DarkGreen });
                        }
                        else
                        {
                            progress.Report(new SyncReport { Message = $"INFO: Token dla konta '{accountName}' wygasł. Wymagana ponowna autoryzacja w panelu Admina.", Color = Color.OrangeRed });
                        }
                    }
                    catch (Exception ex)
                    {
                        progress.Report(new SyncReport { Message = $"BŁĄD dla konta '{accountName}': {ex.Message}", Color = Color.DarkRed });
                    }
                }
            }
            progress.Report(new SyncReport { Message = "\nZakończono sprawdzanie kont.", Color = Color.Blue });
        }
    }
}