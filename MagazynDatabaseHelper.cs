// ############################################################################
// Plik: MagazynDatabaseHelper.cs (POPRAWIONY - SIECIOWY)
// ############################################################################

using System;
using MySql.Data.MySqlClient;

namespace Reklamacje_Dane
{
    public static class MagazynDatabaseHelper
    {
        // Teraz pobieramy ścieżkę SIECIOWĄ z DbConfig, zamiast lokalnej!
        public static string MagazynConnectionString => DbConfig.ConnectionStringMagazyn;

        public static string GetConnectionString() => MagazynConnectionString;

        public static MySqlConnection GetConnection()
        {
            return new MySqlConnection(MagazynConnectionString);
        }

        public static void EnsureInitialized()
        {
            _ = MagazynConnectionString;
        }
    }
}