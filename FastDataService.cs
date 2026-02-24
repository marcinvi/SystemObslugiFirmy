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
    // --- KLASY DTO ---
    public class DzialanieDto
    {
        public string NrZgloszenia { get; set; }
        public string DataDzialania { get; set; }
        public string Tresc { get; set; }
    }

    public class ProducentDto
    {
        public string NazwaProducenta { get; set; }
        public string KontaktMail { get; set; }
        public string Adres { get; set; }
        public string PlEng { get; set; }
        public string Jezyk { get; set; }
        public string Formularz { get; set; }
        public string Wymagania { get; set; }
    }

    public class FastDataService
    {
        private MySqlConnection GetConn() => DatabaseHelper.GetConnection();

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

            var sqlMain = @"
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
                    
                    /* Obcinamy dla bezpieczeństwa sieci */
                    LEFT(COALESCE(z.OpisUsterki, ''), 1500) AS OpisUsterki,
                    LEFT(COALESCE(z.Produkt, ''), 1500) AS ProduktOpis,
                    
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
                    COALESCE(z.KwotaZwrotu, '') AS KwotaZwrotu

                FROM Zgloszenia z
                LEFT JOIN klienci k ON k.Id = z.KlientID
                LEFT JOIN Produkty p ON p.Id = z.ProduktID;
            ";

            var sqlActions = @"
                SELECT 
                    NrZgloszenia, 
                    DataDzialania, 
                    LEFT(Tresc, 1500) AS Tresc
                FROM dzialania 
                WHERE Tresc IS NOT NULL AND Tresc != '';
            ";

            var sqlProducers = @"
                SELECT NazwaProducenta, KontaktMail, Adres, PlEng, Jezyk, Formularz, Wymagania 
                FROM Producenci;
            ";

            List<ComplaintViewModel> complaints = null;
            List<DzialanieDto> actions = null;
            List<ProducentDto> producers = null;

            ReportProgress("Rozpoczynam pobieranie danych (Tryb Synchroniczny - Szybki)...");

            // CAŁOŚĆ WRZUCAMY DO JEDNEGO WĄTKU W TLE, ABY UŻYĆ SYNCHRONICZNEGO DAPPERA
            await Task.Run(() =>
            {
                using (var conn = GetConn())
                {
                    // Używamy .Open() zamiast .OpenAsync()
                    conn.Open();

                    var swQuery = Stopwatch.StartNew();

                    // Używamy .Query() zamiast .QueryAsync() - omija błąd sterownika
                    complaints = conn.Query<ComplaintViewModel>(sqlMain, commandTimeout: 120).ToList();
                    ReportProgress($"[DB] Zgłoszenia pobrane w: {swQuery.ElapsedMilliseconds}ms (Ilość: {complaints.Count})");

                    swQuery.Restart();
                    actions = conn.Query<DzialanieDto>(sqlActions, commandTimeout: 120).ToList();
                    ReportProgress($"[DB] Działania pobrane w: {swQuery.ElapsedMilliseconds}ms (Ilość: {actions.Count})");

                    swQuery.Restart();
                    producers = conn.Query<ProducentDto>(sqlProducers, commandTimeout: 120).ToList();
                    ReportProgress($"[DB] Producenci pobrani w: {swQuery.ElapsedMilliseconds}ms (Ilość: {producers.Count})");
                }
            });

            var swMap = Stopwatch.StartNew();
            ReportProgress($"Mapowanie relacji w RAM...");

            var actionsLookup = actions
                .GroupBy(x => x.NrZgloszenia ?? "")
                .ToDictionary(g => g.Key, g => g.ToList());

            var producersLookup = producers
                .Where(p => !string.IsNullOrWhiteSpace(p.NazwaProducenta))
                .ToDictionary(p => p.NazwaProducenta, p => p, StringComparer.OrdinalIgnoreCase);

            Parallel.ForEach(complaints, c =>
            {
                if (!string.IsNullOrWhiteSpace(c.Producent) && producersLookup.TryGetValue(c.Producent, out var prod))
                {
                    c.ProducentKontaktMail = prod.KontaktMail ?? "";
                    c.ProducentAdres = prod.Adres ?? "";
                    c.ProducentPlEng = prod.PlEng ?? "";
                    c.ProducentJezyk = prod.Jezyk ?? "";
                    c.ProducentFormularz = prod.Formularz ?? "";
                    c.ProducentWymagania = prod.Wymagania ?? "";
                }

                if (c.NrZgloszenia != null && actionsLookup.TryGetValue(c.NrZgloszenia, out var cActions))
                {
                    var sb = new StringBuilder();
                    foreach (var act in cActions)
                    {
                        sb.Append(act.DataDzialania).Append(": ").Append(act.Tresc).Append(" | ");
                    }
                    c.Dzialania = sb.ToString();
                }
                else
                {
                    c.Dzialania = "";
                }

                c.BuildSearchVector();

                if (!string.IsNullOrEmpty(c.Dzialania))
                {
                    c.SearchVector += " " + c.Dzialania.ToLowerInvariant();
                }
            });

            var sortedList = complaints.OrderByDescending(x => x.DataZgloszenia ?? DateTime.MinValue).ToList();

            swMap.Stop();
            swTotal.Stop();
            ReportProgress($"Zakończono! Czas mapowania RAM: {swMap.ElapsedMilliseconds}ms. Całkowity czas ładowania: {swTotal.ElapsedMilliseconds}ms");

            return sortedList;
        }
    }
}