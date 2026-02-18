using System;
using System.Collections.Generic;
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

        public async Task<List<ComplaintViewModel>> LoadAllComplaintsAsync()
        {
            DapperTypeHandlerBootstrap.EnsureRegistered();

            // --- ZAPYTANIA SQL (BEZ LIMITU - WSZYSTKIE DANE) ---

            // SQL 1: Główne dane zgłoszeń
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
                    COALESCE(z.KwotaZwrotu, '') AS KwotaZwrotu

                FROM Zgloszenia z
                LEFT JOIN klienci k ON k.Id = z.KlientID
                LEFT JOIN Produkty p ON p.Id = z.ProduktID
                LEFT JOIN Producenci pr ON pr.NazwaProducenta = p.Producent
                ORDER BY z.DataZgloszenia DESC; 
            ";

            // SQL 2: Pobierz wszystkie działania (tylko ID i treść)
            var sqlActions = @"
                SELECT NrZgloszenia, DataDzialania, Tresc
                FROM dzialania 
                WHERE Tresc IS NOT NULL AND Tresc != '';
            ";

            // --- WYKONANIE RÓWNOLEGŁE NA DWÓCH POŁĄCZENIACH ---

            // Zadanie 1: Pobierz zgłoszenia na własnym połączeniu
            var taskComplaints = Task.Run(async () =>
            {
                using (var conn = GetConn())
                {
                    await conn.OpenAsync();
                    return (await conn.QueryAsync<ComplaintViewModel>(sqlMain)).ToList();
                }
            });

            // Zadanie 2: Pobierz działania na własnym połączeniu
            var taskActions = Task.Run(async () =>
            {
                using (var conn = GetConn())
                {
                    await conn.OpenAsync();
                    return (await conn.QueryAsync<dynamic>(sqlActions)).ToList();
                }
            });

            // Czekamy na oba (teraz to zadziała, bo są na osobnych połączeniach)
            await Task.WhenAll(taskComplaints, taskActions);

            var complaints = taskComplaints.Result;
            var actions = taskActions.Result;

            // --- ŁĄCZENIE DANYCH W PAMIĘCI (SZYBKO) ---

            // Grupujemy działania po numerze zgłoszenia dla szybkiego dostępu
            var actionsLookup = actions
                .GroupBy(x => (string)x.NrZgloszenia)
                .ToDictionary(g => g.Key, g => g.ToList());

            // Równoległe łączenie i budowanie wyszukiwarki
            Parallel.ForEach(complaints, c =>
            {
                // Znajdź działania dla tego zgłoszenia
                if (c.NrZgloszenia != null && actionsLookup.TryGetValue(c.NrZgloszenia, out var cActions))
                {
                    // Sklejamy treść działań do ukrytego pola wyszukiwania
                    var sb = new StringBuilder();
                    // Dodajemy spację startową
                    sb.Append(" ");

                    foreach (var act in cActions)
                    {
                        // Dodajemy tylko treść do wyszukiwania
                        sb.Append(act.DataDzialania).Append(" ").Append(act.Tresc).Append(" ");
                    }

                    // Ustawiamy właściwość (jeśli chcesz ją widzieć w tooltipie)
                    // c.Dzialania = sb.ToString(); 

                    // Budujemy główny wektor
                    c.BuildSearchVector();

                    // Doklejamy historię do wektora wyszukiwania
                    c.SearchVector += sb.ToString().ToLower();
                }
                else
                {
                    // Brak działań - po prostu budujemy standardowy wektor
                    c.Dzialania = "";
                    c.BuildSearchVector();
                }
            });

            return complaints;
        }
    }
}