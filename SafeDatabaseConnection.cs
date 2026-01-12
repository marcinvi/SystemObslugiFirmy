using System;
using System.Windows.Forms;
using MySql.Data.MySqlClient;

namespace Reklamacje_Dane
{
    /// <summary>
    /// Helper do bezpiecznego łączenia z bazą z auto-diagnostyką
    /// </summary>
    public static class SafeDatabaseConnection
    {
        private static bool _diagnosticsShown = false;

        /// <summary>
        /// Próbuje połączyć się z bazą. Jeśli nie może - automatycznie uruchamia diagnostykę.
        /// </summary>
        public static MySqlConnection GetConnectionSafe()
        {
            try
            {
                var conn = new MySqlConnection(DbConfig.ConnectionString);
                conn.Open();
                conn.Close();
                _diagnosticsShown = false; // Reset flagi po sukcesie
                return new MySqlConnection(DbConfig.ConnectionString);
            }
            catch (MySqlException ex)
            {
                // Pokaż diagnostykę tylko raz na sesję
                if (!_diagnosticsShown)
                {
                    _diagnosticsShown = true;
                    ShowDiagnosticsWithAutoFix(ex);
                }
                throw; // Rzuć wyjątek dalej
            }
        }

        /// <summary>
        /// Testuje połączenie bez otwierania formy diagnostyki
        /// </summary>
        public static bool TestConnection(out string errorMessage)
        {
            try
            {
                using (var conn = new MySqlConnection(DbConfig.ConnectionString))
                {
                    conn.Open();
                    errorMessage = null;
                    return true;
                }
            }
            catch (Exception ex)
            {
                errorMessage = ex.Message;
                return false;
            }
        }

        /// <summary>
        /// Pokazuje diagnostykę z możliwością auto-naprawy
        /// </summary>
        private static void ShowDiagnosticsWithAutoFix(MySqlException ex)
        {
            // Błędy związane z połączeniem (serwer nie działa)
            if (ex.Number == 0 || ex.Number == 1042 || ex.Number == 2003)
            {
                var result = MessageBox.Show(
                    $"Nie można połączyć się z MySQL/MariaDB!\n\n" +
                    $"Błąd: {ex.Message}\n\n" +
                    $"TAK - Spróbuj uruchomić usługę automatycznie\n" +
                    $"NIE - Otwórz narzędzie diagnostyczne\n" +
                    $"ANULUJ - Wróć do aplikacji",
                    "Błąd połączenia z bazą",
                    MessageBoxButtons.YesNoCancel,
                    MessageBoxIcon.Error
                );

                if (result == DialogResult.Yes)
                {
                    // Próba auto-startu
                    if (MySQLServiceManager.TryStartService(out string msg))
                    {
                        MessageBox.Show(
                            msg + "\n\nSpróbuj połączyć się ponownie.",
                            "Sukces",
                            MessageBoxButtons.OK,
                            MessageBoxIcon.Information
                        );
                        _diagnosticsShown = false; // Pozwól spróbować ponownie
                    }
                    else
                    {
                        // Jeśli auto-start nie zadziałał, pokaż diagnostykę
                        MessageBox.Show(
                            msg + "\n\nOtwieram narzędzie diagnostyczne...",
                            "Błąd",
                            MessageBoxButtons.OK,
                            MessageBoxIcon.Warning
                        );
                        
                        try
                        {
                            var diagnostics = new FormDatabaseDiagnostics();
                            diagnostics.ShowDialog();
                        }
                        catch { }
                    }
                }
                else if (result == DialogResult.No)
                {
                    // Bezpośrednio diagnostyka
                    try
                    {
                        var diagnostics = new FormDatabaseDiagnostics();
                        diagnostics.ShowDialog();
                    }
                    catch (Exception diagEx)
                    {
                        MessageBox.Show(
                            $"Nie można uruchomić diagnostyki: {diagEx.Message}",
                            "Błąd",
                            MessageBoxButtons.OK,
                            MessageBoxIcon.Error
                        );
                    }
                }
            }
            else
            {
                // Inne błędy (hasło, baza danych itp.) - bezpośrednio diagnostyka
                ShowDiagnostics(ex);
            }
        }

        /// <summary>
        /// Pokazuje diagnostykę z konkretnym błędem
        /// </summary>
        private static void ShowDiagnostics(MySqlException ex)
        {
            var result = MessageBox.Show(
                $"Nie można połączyć się z bazą danych!\n\n" +
                $"Błąd: {ex.Message}\n" +
                $"Kod: {ex.Number}\n\n" +
                $"Czy chcesz uruchomić narzędzie diagnostyczne?",
                "Błąd połączenia z bazą",
                MessageBoxButtons.YesNo,
                MessageBoxIcon.Error
            );

            if (result == DialogResult.Yes)
            {
                try
                {
                    var diagnostics = new FormDatabaseDiagnostics();
                    diagnostics.ShowDialog();
                }
                catch (Exception diagEx)
                {
                    MessageBox.Show(
                        $"Nie można uruchomić diagnostyki: {diagEx.Message}",
                        "Błąd",
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Error
                    );
                }
            }
        }

        /// <summary>
        /// Reset flagi diagnostyki (np. po udanym połączeniu)
        /// </summary>
        public static void ResetDiagnosticsFlag()
        {
            _diagnosticsShown = false;
        }
    }
}
