using System;
using System.Data;
using MySql.Data.MySqlClient;
using System.Threading.Tasks;
using System.Collections.Generic;

namespace Reklamacje_Dane
{
    public class ComplaintSearchService
    {
        public ComplaintSearchService() { }

        public ComplaintSearchService(string dbPath = null) { }

        private MySqlConnection GetConn()
        {
            // Zakładam, że DatabaseHelper zwraca już MySqlConnection po Twoich zmianach
            return DatabaseHelper.GetConnection();
        }

        public async Task<DataTable> GetComplaintsAsync()
        {
            // ZMIANA: printf -> CONCAT, IFNULL -> COALESCE (standard SQL)
            // MariaDB używa GROUP_CONCAT tak samo, ale składnia łączenia stringów to CONCAT
            var sql = @"
WITH dz AS (
    SELECT d.NrZgloszenia,
           GROUP_CONCAT(
               CONCAT(d.DataDzialania, ' ', COALESCE(d.Uzytkownik,''), ': ', REPLACE(d.Tresc, '\n', ' ')) 
               SEPARATOR ' | '
           ) AS Dzialania
    FROM Dzialania d
    GROUP BY d.NrZgloszenia
)
SELECT 
    z.Id,
    z.NrZgloszenia,
    z.KlientID,
    z.ProduktID,
    z.DataZgloszenia,
    z.DataZakupu,
    z.NrFaktury,
    z.NrSeryjny,
    z.OpisUsterki,
    z.GwarancjaPlatna,
    z.StatusOgolny,
    z.StatusKlient,
    z.StatusProducent,
    z.CzekamyNaDostawe,
    z.NrWRL,
    z.NrKWZ2,
    z.NrRMA,
    z.NrKPZN,
    z.CzyNotaRozliczona,
    z.Skad,
    z.allegroDisputeId,
    z.allegroOrderId,
    z.allegroBuyerLogin,
    z.AllegroAccountId,
    z.KwotaZwrotu,

    k.ImieNazwisko,
    k.NazwaFirmy,
    k.NIP,
    k.Email,
    k.Telefon,

    p.Kategoria,
    p.NazwaKrotka,
    p.NazwaSystemowa,
    p.KodEnova,
    p.KodProducenta,
    p.Producent AS ProducentNazwa,

    pr.NazwaProducenta,
    pr.Jezyk,
    pr.Formularz,
    pr.KontaktMail,

    dz.Dzialania

FROM Zgloszenia z
LEFT JOIN klienci k          ON k.Id = z.KlientID
LEFT JOIN Produkty p         ON p.Id = z.ProduktID
LEFT JOIN Producenci pr      ON pr.NazwaProducenta = p.Producent
LEFT JOIN dz                 ON dz.NrZgloszenia = z.NrZgloszenia
ORDER BY z.DataZgloszenia DESC, z.NrZgloszenia DESC;";

            var dt = new DataTable();

            using (var conn = GetConn())
            using (var cmd = new MySqlCommand(sql, conn))
            {
                await conn.OpenAsync();
                using (var da = new MySqlDataAdapter(cmd))
                {
                    da.Fill(dt);
                }
            }
            return dt;
        }

        public async Task<List<string>> GetDistinctValuesAsync(string table, string column)
        {
            var list = new List<string>();

            // POPRAWKA: Usunięto błędną literę 'E' z końca zapytania
            // Dodano backticki `` wokół nazw kolumn i tabel (bezpieczeństwo MariaDB)
            var sql = $"SELECT DISTINCT `{column}` FROM `{table}` WHERE `{column}` IS NOT NULL AND TRIM(`{column}`) <> '' ORDER BY `{column}` ASC;";

            using (var conn = GetConn())
            using (var cmd = new MySqlCommand(sql, conn))
            {
                await conn.OpenAsync();
                using (var rdr = await cmd.ExecuteReaderAsync())
                {
                    while (await rdr.ReadAsync())
                    {
                        var val = rdr[0]?.ToString();
                        if (!string.IsNullOrWhiteSpace(val))
                            list.Add(val);
                    }
                }
            }
            return list;
        }
    }
}