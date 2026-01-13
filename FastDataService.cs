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
                    z.DataZakupu,
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
                    COALESCE(z.CzyNotaRozliczona, '') AS CzyNotaRozliczona,
                    COALESCE(z.KwotaZwrotu, '') AS KwotaZwrotu,
                    COALESCE(z.Dzialania, '') AS Dzialania
                FROM Zgloszenia z
                LEFT JOIN klienci k ON k.Id = z.KlientID
                LEFT JOIN Produkty p ON p.Id = z.ProduktID
                LEFT JOIN Producenci pr ON pr.NazwaProducenta = p.Producent
           
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
