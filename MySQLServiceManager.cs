using System;
using System.Diagnostics;
using System.ServiceProcess;
using System.Linq;
using System.Windows.Forms;

namespace Reklamacje_Dane
{
    /// <summary>
    /// Automatyczne uruchamianie MySQL/MariaDB
    /// </summary>
    public static class MySQLServiceManager
    {
        // Możliwe nazwy usług MySQL/MariaDB
        private static readonly string[] ServiceNames = new[]
        {
            "MySQL",
            "MySQL80",
            "MySQL57",
            "MariaDB",
            "MariaDB 10.11",
            "MariaDB 10.6",
            "MYSQL80",
            "wampmysqld",
            "wampmysqld64"
        };

        /// <summary>
        /// Próbuje automatycznie uruchomić usługę MySQL/MariaDB
        /// </summary>
        public static bool TryStartService(out string message)
        {
            try
            {
                // Znajdź usługę
                var service = FindMySQLService();
                
                if (service == null)
                {
                    message = "Nie znaleziono usługi MySQL/MariaDB.\n\n" +
                             "Możliwe rozwiązania:\n" +
                             "1. Zainstaluj MySQL/MariaDB\n" +
                             "2. Użyj XAMPP → Start MySQL\n" +
                             "3. Uruchom usługę ręcznie";
                    return false;
                }

                // Sprawdź status
                if (service.Status == ServiceControllerStatus.Running)
                {
                    message = $"Usługa '{service.ServiceName}' już działa!";
                    return true;
                }

                // Próbuj uruchomić
                message = $"Próbuję uruchomić usługę '{service.ServiceName}'...";
                
                service.Start();
                service.WaitForStatus(ServiceControllerStatus.Running, TimeSpan.FromSeconds(30));
                
                message = $"✅ Usługa '{service.ServiceName}' została uruchomiona!";
                return true;
            }
            catch (InvalidOperationException ex)
            {
                // Usługa istnieje ale nie może się uruchomić
                message = $"❌ Nie można uruchomić usługi:\n{ex.Message}\n\n" +
                         "Spróbuj ręcznie:\n" +
                         "1. Win+R → services.msc\n" +
                         "2. Znajdź MySQL/MariaDB\n" +
                         "3. Kliknij prawym → Start\n\n" +
                         "LUB uruchom jako Administrator.";
                return false;
            }
            catch (System.ComponentModel.Win32Exception ex)
            {
                // Brak uprawnień
                message = $"❌ Brak uprawnień administratora!\n\n" +
                         $"Uruchom aplikację jako Administrator:\n" +
                         $"1. Kliknij prawym na ikonę\n" +
                         $"2. Wybierz 'Uruchom jako administrator'\n\n" +
                         $"Szczegóły: {ex.Message}";
                return false;
            }
            catch (Exception ex)
            {
                message = $"❌ Nieoczekiwany błąd:\n{ex.Message}";
                return false;
            }
        }

        /// <summary>
        /// Znajduje usługę MySQL/MariaDB w systemie
        /// </summary>
        private static ServiceController FindMySQLService()
        {
            try
            {
                var services = ServiceController.GetServices();
                
                foreach (var serviceName in ServiceNames)
                {
                    var service = services.FirstOrDefault(s => 
                        s.ServiceName.Equals(serviceName, StringComparison.OrdinalIgnoreCase));
                    
                    if (service != null)
                    {
                        return service;
                    }
                }
                
                // Spróbuj znaleźć przez częściową nazwę
                var mysqlService = services.FirstOrDefault(s => 
                    s.ServiceName.IndexOf("mysql", StringComparison.OrdinalIgnoreCase) >= 0 ||
                    s.ServiceName.IndexOf("mariadb", StringComparison.OrdinalIgnoreCase) >= 0);
                
                return mysqlService;
            }
            catch
            {
                return null;
            }
        }

        /// <summary>
        /// Sprawdza czy jakakolwiek usługa MySQL/MariaDB działa
        /// </summary>
        public static bool IsServiceRunning()
        {
            try
            {
                var service = FindMySQLService();
                return service != null && service.Status == ServiceControllerStatus.Running;
            }
            catch
            {
                return false;
            }
        }

        /// <summary>
        /// Próbuje uruchomić XAMPP MySQL (jeśli zainstalowany)
        /// </summary>
        public static bool TryStartXAMPP(out string message)
        {
            try
            {
                // Typowe ścieżki XAMPP
                var xamppPaths = new[]
                {
                    @"C:\xampp\mysql_start.bat",
                    @"C:\xampp\mysql\bin\mysqld.exe",
                    @"D:\xampp\mysql_start.bat"
                };

                foreach (var path in xamppPaths)
                {
                    if (System.IO.File.Exists(path))
                    {
                        Process.Start(path);
                        message = "Uruchamiam MySQL przez XAMPP...\nSprawdź XAMPP Control Panel.";
                        return true;
                    }
                }

                message = "XAMPP nie został znaleziony.\nUruchom XAMPP Control Panel ręcznie.";
                return false;
            }
            catch (Exception ex)
            {
                message = $"Błąd uruchamiania XAMPP: {ex.Message}";
                return false;
            }
        }

        /// <summary>
        /// Pokazuje dialog z opcjami rozwiązania problemu
        /// </summary>
        public static void ShowFixOptions()
        {
            var message = "Nie można połączyć się z MySQL/MariaDB.\n\n" +
                         "Wybierz akcję:\n\n" +
                         "TAK - Spróbuj uruchomić automatycznie (wymaga Administratora)\n" +
                         "NIE - Otwórz narzędzie diagnostyczne\n" +
                         "ANULUJ - Zamknij";

            var result = MessageBox.Show(
                message,
                "Problem z bazą danych",
                MessageBoxButtons.YesNoCancel,
                MessageBoxIcon.Warning
            );

            if (result == DialogResult.Yes)
            {
                // Próba auto-startu
                if (TryStartService(out string msg))
                {
                    MessageBox.Show(msg, "Sukces", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
                else
                {
                    MessageBox.Show(msg, "Błąd", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    
                    // Zapytaj czy spróbować XAMPP
                    if (MessageBox.Show(
                        "Czy chcesz spróbować uruchomić MySQL przez XAMPP?",
                        "XAMPP",
                        MessageBoxButtons.YesNo,
                        MessageBoxIcon.Question) == DialogResult.Yes)
                    {
                        TryStartXAMPP(out string xamppMsg);
                        MessageBox.Show(xamppMsg, "XAMPP", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    }
                }
            }
            else if (result == DialogResult.No)
            {
                // Otwórz diagnostykę
                var diagnostics = new FormDatabaseDiagnostics();
                diagnostics.ShowDialog();
            }
        }
    }
}
