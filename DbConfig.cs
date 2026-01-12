using System;
using System.IO;
using MySql.Data.MySqlClient;

namespace Reklamacje_Dane
{
    public static class DbConfig
    {
        // 1. Dane połączenia z MariaDB
        private const string Server = "localhost";
        private const string Database = "ReklamacjeDB";
        private const string User = "root";
        private const string Password = "Bigbrother5"; // <--- WPISZ SWOJE HASŁO

        public static string ConnectionString
        {
            get
            {
                var builder = new MySqlConnectionStringBuilder
                {
                    Server = Server,
                    Database = Database,
                    UserID = User,
                    Password = Password,
                    CharacterSet = "utf8mb4",
                    AllowZeroDateTime = true,
                    ConvertZeroDateTime = true,
                    SslMode = MySqlSslMode.Disabled,

                    // OPTYMALIZACJA DLA 50+ UŻYTKOWNIKÓW
                    Pooling = true,                      // Włącz connection pooling
                    MinimumPoolSize = 5,                 // Min 5 połączeń w puli
                    MaximumPoolSize = 100,               // Max 100 połączeń w puli
                    ConnectionLifeTime = 300,            // 5 minut życia połączenia
                    ConnectionTimeout = 30,              // 30s timeout połączenia
                    DefaultCommandTimeout = 60,          // 60s timeout dla komend
                    ConnectionReset = true,              // Resetuj stan połączenia

                    // Dodatkowa wydajność
                    AllowUserVariables = true,           // Zmienne w zapytaniach
                    UseCompression = false,              // Bez kompresji dla LAN
                    PersistSecurityInfo = false,         // Bezpieczeństwo
                    AutoEnlist = false                   // Wyłącz auto-enlist w TransactionScope
                };
                return builder.ToString();
            }
        }

        // 2. Kompatybilność ze starym kodem (Mapowanie na MariaDB)
        public static string ConnectionStringBaza => ConnectionString;
        public static string ConnectionStringMagazyn => ConnectionString;

        // 3. Naprawa błędu CS0117 (SignalFilePath i BaseNetworkPath)
        // MariaDB nie potrzebuje plików sygnałowych, ale kod ich szuka.
        // Ustawiamy je na folder lokalny aplikacji, żeby nie sypało błędami.
        public static string BaseNetworkPath => AppDomain.CurrentDomain.BaseDirectory;
        public static string SignalFileName = "ostatnia_aktualizacja.txt";
        public static string SignalFilePath => Path.Combine(BaseNetworkPath, SignalFileName);
    }
}