using System;
using System.Collections.Generic;
using System.Linq;
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
            var sql = @"
                SELECT 
                    z.Id, z.NrZgloszenia, z.DataZgloszenia, 
                    COALESCE(z.StatusOgolny, '') AS Status,
                    COALESCE(k.ImieNazwisko, k.NazwaFirmy, '') AS Klient,
                    COALESCE(k.NIP, '') AS KlientNip,
                    COALESCE(k.ImieNazwisko, '') AS KlientImieNazwisko,
                    COALESCE(k.NazwaFirmy, '') AS KlientNazwaFirmy,
                    COALESCE(k.Email, '') AS KlientEmail,
                    COALESCE(k.Telefon, '') AS KlientTelefon,
                    COALESCE(k.Ulica, '') AS KlientUlica,
                    COALESCE(k.KodPocztowy, '') AS KlientKodPocztowy,
                    COALESCE(k.Miejscowosc, '') AS KlientMiejscowosc,
                    COALESCE(p.NazwaSystemowa, '') AS Produkt,
                    COALESCE(p.NazwaSystemowa, '') AS NazwaSystemowa,
                    COALESCE(p.NazwaKrotka, '') AS NazwaKrotka,
                    COALESCE(p.KodEnova, '') AS KodEnova,
                    COALESCE(p.KodProducenta, '') AS KodProducenta,
                    COALESCE(p.Kategoria, '') AS Kategoria,
                    COALESCE(p.Wymagania, '') AS ProduktWymagania,
                    COALESCE(z.NrSeryjny, '') AS SN,
                    COALESCE(z.NrFaktury, '') AS FV,
                    COALESCE(z.Skad, '') AS Skad,
                    COALESCE(pr.NazwaProducenta, '') AS Producent,
                    z.DataZakupu,
                    COALESCE(z.GwarancjaPlatna, '') AS GwarancjaPlatna,
                    COALESCE(z.StatusDpd, '') AS StatusDpd,
                    COALESCE(z.StatusKlient, '') AS StatusKlient,
                    COALESCE(z.StatusProducent, '') AS StatusProducent,
                    COALESCE(z.CzekamyNaDostawe, '') AS CzekamyNaDostawe,
                    z.DataZamkniecia,
                    COALESCE(z.NrWRL, '') AS NrWRL,
                    COALESCE(z.NrKWZ2, '') AS NrKWZ2,
                    COALESCE(z.NrRMA, '') AS NrRMA,
                    COALESCE(z.NrKPZN, '') AS NrKPZN,
                    COALESCE(z.CzyNotaRozliczona, '') AS CzyNotaRozliczona,
                    COALESCE(z.Usterka, '') AS Usterka,
                    COALESCE(z.OpisUsterki, '') AS OpisUsterki,
                    COALESCE(z.Uwagi, '') AS Uwagi,
                    COALESCE(z.KwotaZwrotu, '') AS KwotaZwrotu,
                    COALESCE(z.NrFakturyPrzychodu, '') AS NrFakturyPrzychodu,
                    COALESCE(CAST(z.KwotaFakturyPrzychoduNetto AS CHAR), '') AS KwotaFakturyPrzychoduNetto,
                    COALESCE(z.NrFakturyKosztowej, '') AS NrFakturyKosztowej,
                    COALESCE(z.Dzialania, '') AS Dzialania,
                    COALESCE(z.allegroBuyerLogin, '') AS AllegroBuyerLogin,
                    COALESCE(z.allegroOrderId, '') AS AllegroOrderId,
                    COALESCE(z.allegroDisputeId, '') AS AllegroDisputeId,
                    COALESCE(CAST(z.allegroAccountId AS CHAR), '') AS AllegroAccountId,
                    COALESCE(u.`Nazwa Wyświetlana`, u.Login, '') AS Opiekun,
                    COALESCE(pr.KontaktMail, '') AS ProducentKontaktMail,
                    COALESCE(pr.Adres, '') AS ProducentAdres,
                    COALESCE(pr.PlEng, '') AS ProducentPlEng,
                    COALESCE(pr.Jezyk, '') AS ProducentJezyk,
                    COALESCE(pr.Formularz, '') AS ProducentFormularz,
                    COALESCE(pr.Wymagania, '') AS ProducentWymagania
                FROM Zgloszenia z
                LEFT JOIN klienci k ON k.Id = z.KlientID
                LEFT JOIN Produkty p ON p.Id = z.ProduktID
                LEFT JOIN Producenci pr ON pr.NazwaProducenta = p.Producent
                LEFT JOIN Uzytkownicy u ON u.Id = z.PrzypisanyDo
                ORDER BY z.DataZgloszenia DESC LIMIT 10000"; // Pobieramy dużo

            using (var conn = GetConn())
            {
                await conn.OpenAsync();
                var result = await conn.QueryAsync<ComplaintViewModel>(sql);
                var list = result.ToList();
                // Budujemy wektor w wielu wątkach (super szybko)
                Parallel.ForEach(list, item => item.BuildSearchVector());
                return list;
            }
        }
    }
}
