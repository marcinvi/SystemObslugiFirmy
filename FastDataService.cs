using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Dapper;
using MySql.Data.MySqlClient;

namespace Reklamacje_Dane
{
    public class FastDataService
    {
        private MySqlConnection GetConn() => DatabaseHelper.GetConnection();

        /// <summary>
        /// Callback do aktualizacji statusu ładowania w UI.
        /// </summary>
        public Action<string> OnProgress { get; set; }

        private void ReportProgress(string msg)
        {
            Debug.WriteLine($"[FastDataService] {msg}");
            OnProgress?.Invoke(msg);
        }

        public async Task<List<ComplaintViewModel>> LoadAllComplaintsAsync()
        {
            DapperTypeHandlerBootstrap.EnsureRegistered();

            var swTotal = Stopwatch.StartNew();
            var sw = Stopwatch.StartNew();

            ReportProgress("Łączenie z bazą...");

            // === JEDNO zapytanie SQL: JOINy + GROUP_CONCAT + SearchVector budowany w bazie ===
            var sql = @"
                SELECT 
                    z.Id, z.NrZgloszenia, z.DataZgloszenia,
                    COALESCE(z.StatusOgolny, '') AS Status,
                    COALESCE(z.StatusKlient, '') AS StatusKlient,
                    COALESCE(z.StatusProducent, '') AS StatusProducent,
                    
                    COALESCE(k.ImieNazwisko, k.NazwaFirmy, '') AS Klient,
                    COALESCE(k.ImieNazwisko, '') AS KlientImieNazwisko,
                    COALESCE(k.NazwaFirmy, '') AS KlientNazwaFirmy,
                    COALESCE(k.NIP, '') AS KlientNip,
                    COALESCE(k.Ulica, '') AS KlientUlica,
                    COALESCE(k.KodPocztowy, '') AS KlientKodPocztowy,
                    COALESCE(k.Miejscowosc, '') AS KlientMiejscowosc,
                    COALESCE(k.Email, '') AS KlientEmail,
                    COALESCE(k.Telefon, '') AS KlientTelefon,

                    COALESCE(p.NazwaSystemowa, '') AS Produkt,
                    COALESCE(p.NazwaSystemowa, '') AS NazwaSystemowa,
                    COALESCE(p.NazwaKrotka, '') AS NazwaKrotka,
                    COALESCE(p.KodEnova, '') AS KodEnova,
                    COALESCE(p.KodProducenta, '') AS KodProducenta,
                    COALESCE(p.Kategoria, '') AS Kategoria,
                    COALESCE(p.Wymagania, '') AS ProduktWymagania,
                    COALESCE(p.Producent, '') AS Producent,

                    COALESCE(pr.KontaktMail, '') AS ProducentKontaktMail,
                    COALESCE(pr.Adres, '') AS ProducentAdres,
                    COALESCE(pr.PlEng, '') AS ProducentPlEng,
                    COALESCE(pr.Jezyk, '') AS ProducentJezyk,
                    COALESCE(pr.Formularz, '') AS ProducentFormularz,
                    COALESCE(pr.Wymagania, '') AS ProducentWymagania,

                    COALESCE(z.NrSeryjny, '') AS SN,
                    COALESCE(z.NrFaktury, '') AS FV,
                    COALESCE(z.NrFakturyPrzychodu, '') AS NrFakturyPrzychodu,
                    COALESCE(z.KwotaFakturyPrzychoduNetto, 0) AS KwotaFakturyPrzychoduNetto,
                    COALESCE(z.NrFakturyKosztowej, '') AS NrFakturyKosztowej,
                    COALESCE(z.Skad, '') AS Skad,

                    CASE
                      WHEN z.DataZakupu IS NULL OR z.DataZakupu IN ('', '-') THEN NULL
                      WHEN z.DataZakupu LIKE '__.__.____' THEN STR_TO_DATE(z.DataZakupu, '%d.%m.%Y')
                      WHEN z.DataZakupu LIKE '____-__-__' THEN STR_TO_DATE(z.DataZakupu, '%Y-%m-%d')
                      ELSE NULL
                    END AS DataZakupu,

                    COALESCE(z.OpisUsterki, '') AS OpisUsterki,
                    COALESCE(z.Produkt, '') AS ProduktOpis,
                    COALESCE(z.allegroBuyerLogin, '') AS AllegroBuyerLogin,
                    COALESCE(z.allegroOrderId, '') AS AllegroOrderId,
                    COALESCE(z.allegroDisputeId, '') AS AllegroDisputeId,
                    COALESCE(z.AllegroAccountId, '') AS AllegroAccountId,
                    COALESCE(z.GwarancjaPlatna, '') AS GwarancjaPlatna,
                    COALESCE(z.CzekamyNaDostawe, '') AS CzekamyNaDostawe,
                    COALESCE(z.NrWRL, '') AS NrWRL,
                    COALESCE(z.NrKWZ2, '') AS NrKWZ2,
                    COALESCE(z.NrRMA, '') AS NrRMA,
                    COALESCE(z.NrKPZN, '') AS NrKPZN,
                    CAST(NULLIF(NULLIF(z.CzyNotaRozliczona, ''), '-') AS SIGNED) AS CzyNotaRozliczona,
                    COALESCE(z.KwotaZwrotu, '') AS KwotaZwrotu,

                    COALESCE(d_agg.DzialaniaText, '') AS Dzialania,

                    LOWER(CONCAT_WS(' ',
                        z.NrZgloszenia,
                        z.DataZgloszenia,
                        COALESCE(z.StatusOgolny, ''),
                        COALESCE(z.StatusKlient, ''),
                        COALESCE(z.StatusProducent, ''),
                        COALESCE(k.ImieNazwisko, ''),
                        COALESCE(k.NazwaFirmy, ''),
                        COALESCE(k.NIP, ''),
                        COALESCE(k.Ulica, ''),
                        COALESCE(k.KodPocztowy, ''),
                        COALESCE(k.Miejscowosc, ''),
                        COALESCE(k.Email, ''),
                        COALESCE(k.Telefon, ''),
                        COALESCE(p.NazwaSystemowa, ''),
                        COALESCE(p.NazwaKrotka, ''),
                        COALESCE(p.KodEnova, ''),
                        COALESCE(p.KodProducenta, ''),
                        COALESCE(p.Kategoria, ''),
                        COALESCE(p.Wymagania, ''),
                        COALESCE(p.Producent, ''),
                        COALESCE(pr.KontaktMail, ''),
                        COALESCE(pr.Adres, ''),
                        COALESCE(z.NrSeryjny, ''),
                        COALESCE(z.NrFaktury, ''),
                        COALESCE(z.NrFakturyPrzychodu, ''),
                        COALESCE(z.NrFakturyKosztowej, ''),
                        COALESCE(z.Skad, ''),
                        COALESCE(z.OpisUsterki, ''),
                        COALESCE(z.Produkt, ''),
                        COALESCE(z.allegroBuyerLogin, ''),
                        COALESCE(z.allegroOrderId, ''),
                        COALESCE(z.allegroDisputeId, ''),
                        COALESCE(z.AllegroAccountId, ''),
                        COALESCE(z.GwarancjaPlatna, ''),
                        COALESCE(z.CzekamyNaDostawe, ''),
                        COALESCE(z.NrWRL, ''),
                        COALESCE(z.NrKWZ2, ''),
                        COALESCE(z.NrRMA, ''),
                        COALESCE(z.NrKPZN, ''),
                        COALESCE(z.KwotaZwrotu, ''),
                        COALESCE(d_agg.DzialaniaText, '')
                    )) AS SearchVector

                FROM Zgloszenia z
                LEFT JOIN klienci k ON k.Id = z.KlientID
                LEFT JOIN Produkty p ON p.Id = z.ProduktID
                LEFT JOIN Producenci pr ON pr.NazwaProducenta = p.Producent
                LEFT JOIN (
                    SELECT NrZgloszenia, 
                           GROUP_CONCAT(CONCAT(COALESCE(DataDzialania,''), ' ', Tresc) SEPARATOR ' ') AS DzialaniaText
                    FROM dzialania 
                    WHERE Tresc IS NOT NULL AND Tresc != ''
                    GROUP BY NrZgloszenia
                ) d_agg ON d_agg.NrZgloszenia = z.NrZgloszenia

                ORDER BY z.DataZgloszenia DESC;
            ";

            ReportProgress("Pobieranie danych z bazy...");

            var complaints = await Task.Run(async () =>
            {
                using (var conn = GetConn())
                {
                    await conn.OpenAsync();

                    // Zwiększ limit GROUP_CONCAT na tej sesji
                    await conn.ExecuteAsync("SET SESSION group_concat_max_len = 1000000;");

                    sw.Stop();
                    ReportProgress($"Połączono ({sw.ElapsedMilliseconds}ms). Wykonywanie zapytania...");
                    sw.Restart();

                    var result = (await conn.QueryAsync<ComplaintViewModel>(sql, commandTimeout: 120)).AsList();

                    sw.Stop();
                    ReportProgress($"SQL: {sw.ElapsedMilliseconds}ms, pobrano {result.Count} zgłoszeń");

                    return result;
                }
            });

            swTotal.Stop();
            ReportProgress($"Gotowe! Łącznie: {swTotal.ElapsedMilliseconds}ms");

            return complaints;
        }
    }
}
