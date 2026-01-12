using System;
using System.Windows.Forms;

namespace Reklamacje_Dane
{
    /// <summary>
    /// Program testowy do diagnostyki bazy danych
    /// 
    /// JAK URUCHOMIĆ DIAGNOSTYKĘ:
    /// Dodaj w kodzie (np. przy błędzie połączenia):
    /// 
    ///     var diagnostics = new FormDatabaseDiagnostics();
    ///     diagnostics.ShowDialog();
    /// 
    /// LUB użyj MySQLServiceManager.ShowFixOptions();
    /// 
    /// UWAGA: Testowy punkt wejścia wykomentowany aby uniknąć konfliktu z głównym Program.cs
    /// </summary>
    static class DatabaseDiagnosticsProgram
    {
        // Testowy punkt wejścia - WYKOMENTOWANY
        // Aby użyć diagnostyki, wywołaj: new FormDatabaseDiagnostics().ShowDialog();
        
        /*
        [STAThread]
        static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(new FormDatabaseDiagnostics());
        }
        */
        
        /// <summary>
        /// Uruchom narzędzie diagnostyczne
        /// </summary>
        public static void RunDiagnostics()
        {
            var diagnostics = new FormDatabaseDiagnostics();
            diagnostics.ShowDialog();
        }
    }
}
