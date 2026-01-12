using System;
using System.IO;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Reklamacje_Dane
{
    public static class BackupService
    {
        // Definiujemy nazwy plików baz danych jako stałe, aby uniknąć literówek
        private const string BazaDbFileName = "baza.db";
        private const string MagazynDbFileName = "magazyn.db";

        /// <summary>
        /// Tworzy kopię zapasową baz danych w podfolderze "Backup".
        /// </summary>
        /// <param name="showSuccessMessage">Czy pokazywać komunikat o powodzeniu operacji.</param>
        public static async Task CreateBackupAsync(bool showSuccessMessage = true)
        {
            try
            {
                // 1. Ustalanie ścieżek
                // Pobieramy ścieżkę do folderu, w którym znajduje się plik .exe aplikacji
                string baseDirectory = AppDomain.CurrentDomain.BaseDirectory;

                // Tworzymy ścieżkę do podfolderu "Backup"
                string backupDir = Path.Combine(baseDirectory, "Backup");
                Directory.CreateDirectory(backupDir); // Jeśli folder nie istnieje, zostanie utworzony

                // Tworzymy pełne ścieżki do plików baz danych, które mają być w tym samym folderze co .exe
                string bazaDbPath = Path.Combine(baseDirectory, BazaDbFileName);
                string magazynDbPath = Path.Combine(baseDirectory, MagazynDbFileName);

                // 2. Wykonanie kopii zapasowej dla obu plików
                await BackupFileAsync(bazaDbPath, backupDir);
                await BackupFileAsync(magazynDbPath, backupDir);

                // 3. Komunikat o sukcesie (jeśli wymagany)
                if (showSuccessMessage)
                {
                    MessageBox.Show(
                        $"Kopie zapasowe obu baz danych zostały pomyślnie utworzone w folderze:\n{backupDir}",
                        "Kopia zapasowa wykonana",
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Information);
                }
            }
            catch (Exception ex)
            {
                // 4. Obsługa błędów
                MessageBox.Show(
                    $"Wystąpił błąd podczas tworzenia kopii zapasowej:\n{ex.Message}",
                    "Błąd krytyczny",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error);
            }
        }

        /// <summary>
        /// Prywatna metoda pomocnicza do kopiowania pojedynczego pliku.
        /// </summary>
        private static Task BackupFileAsync(string sourcePath, string backupDir)
        {
            return Task.Run(() =>
            {
                // Sprawdzamy, czy plik źródłowy w ogóle istnieje
                if (!File.Exists(sourcePath))
                {
                    // Zamiast rzucać błędem, po prostu informujemy w konsoli i przerywamy.
                    // Dzięki temu, jeśli jedna z baz nie istnieje, backup drugiej wciąż się wykona.
                    Console.WriteLine($"Plik bazy danych nie istnieje, pomijam kopię: {sourcePath}");
                    return;
                }

                // Tworzenie unikalnej nazwy dla pliku kopii zapasowej z datą i godziną
                string fileName = Path.GetFileNameWithoutExtension(sourcePath);
                string extension = Path.GetExtension(sourcePath);
                string timestamp = DateTime.Now.ToString("yyyy-MM-dd_HH-mm-ss");

                string destinationFileName = $"{fileName}_{timestamp}{extension}";
                string destinationPath = Path.Combine(backupDir, destinationFileName);

                // Kopiowanie pliku. "true" oznacza, że jeśli plik o tej samej nazwie już istnieje, zostanie nadpisany.
                File.Copy(sourcePath, destinationPath, true);
            });
        }
    }
}