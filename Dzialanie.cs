using System;
using MySql.Data.MySqlClient;

namespace Reklamacje_Dane
{
    /// <summary>
    /// Klasa obsługująca zapisywanie działań użytkowników do tabeli Dzialania.
    /// </summary>
    public class Dzialanie
    {
        /// <summary>
        /// Dodaje nowe działanie – prosty zapis, bez transakcji.
        /// </summary>
        public void DodajNoweDzialanie(string nrZgloszenia, string uzytkownik, string tresc)
        {
            try
            {
                using (var con = DatabaseHelper.GetConnection())
                {
                    con.Open();
                    const string query = @"
                        INSERT INTO Dzialania 
                        (DataDzialania, Uzytkownik, Tresc, NrZgloszenia) 
                        VALUES (@data, @uzytkownik, @tresc, @nrZgloszenia)";

                    using (var cmd = new MySqlCommand(query, con))
                    {
                        cmd.Parameters.AddWithValue("@data", DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"));
                        cmd.Parameters.AddWithValue("@uzytkownik", uzytkownik);
                        cmd.Parameters.AddWithValue("@tresc", tresc);
                        cmd.Parameters.AddWithValue("@nrZgloszenia", nrZgloszenia);
                        cmd.ExecuteNonQuery();
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"BŁĄD KRYTYCZNY: Nie udało się zapisać działania. Szczegóły: {ex.Message}");
            }
        }

        /// <summary>
        /// Dodaje nowe działanie w ramach istniejącej transakcji (np. większa operacja).
        /// </summary>
        public void DodajNoweDzialanie(MySqlConnection con, MySqlTransaction transaction,
            string nrZgloszenia, string uzytkownik, string tresc)
        {
            const string query = @"
                INSERT INTO Dzialania 
                (DataDzialania, Uzytkownik, Tresc, NrZgloszenia) 
                VALUES (@data, @uzytkownik, @tresc, @nrZgloszenia)";

            using (var cmd = new MySqlCommand(query, con, transaction))
            {
                cmd.Parameters.AddWithValue("@data", DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"));
                cmd.Parameters.AddWithValue("@uzytkownik", uzytkownik);
                cmd.Parameters.AddWithValue("@tresc", tresc);
                cmd.Parameters.AddWithValue("@nrZgloszenia", nrZgloszenia);
                cmd.ExecuteNonQuery();
            }
        }
    }

    /// <summary>
    /// Klasa aliasowa – zostawiona tylko dla kompatybilności ze starym kodem,
    /// który używał nazwy "Dzialaniee".
    /// </summary>
    public class Dzialaniee : Dzialanie
    {
        // Pusta – dziedziczy wszystko z Dzialanie
    }
}
