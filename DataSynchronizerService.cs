using System;
using System.Collections.Generic;
using System.Data.SQLite;
using System.Threading.Tasks;
using Google.Apis.Sheets.v4;
using Google.Apis.Sheets.v4.Data;

/// <summary>
/// Serwis odpowiedzialny za synchronizację danych z Google Sheets
/// do lokalnej bazy SQLite. Wykorzystuje parametryzowane zapytania
/// oraz asynchroniczny dostęp do bazy.
/// </summary>
public class DataSynchronizerService
{
    private readonly SheetsService _service;
    private readonly string _spreadsheetId;
    private readonly DatabaseService _dbService;

    public DataSynchronizerService(SheetsService service, string spreadsheetId, DatabaseService dbService)
    {
        _service = service;
        _spreadsheetId = spreadsheetId;
        _dbService = dbService;
    }

    /// <summary>
    /// Synchronizuje tabelę "Zgloszenia" z arkuszem Google.
    /// </summary>
    public async Task SynchronizeZgloszeniaAsync()
    {
        var range = "Zgloszenia!A:Z";
        var request = _service.Spreadsheets.Values.Get(_spreadsheetId, range);
        ValueRange response = await request.ExecuteAsync();
        var values = response.Values;
        if (values == null)
            return;

        const string query = @"INSERT OR REPLACE INTO Zgloszenia (
                    nrZgloszenia, klient, produkt, dataZgloszenia, dataZakupu, nrFaktury, nrSeryjny,
                    usterka, gwarancyjnaPlatna, statusKlient, nrWRL, nrKWZ2, gdzieZgloszono,
                    nrFakturyPrzychodu, kwotaFakturyPrzychoduNetto, kwotaFakturyPrzychoduBrutto,
                    nrFakturyKosztowej, kwotaFakturyKosztowejNetto, kwotaFakturyKosztowejBrutto,
                    statusProducent, nrDokumentuProducenta, czekamyNaDostawe, nrKlienta,
                    nrProduktu, dzialania, iloscSztuk, tsEna
                ) VALUES (
                    @nrZgloszenia, @klient, @produkt, @dataZgloszenia, @dataZakupu, @nrFaktury, @nrSeryjny,
                    @usterka, @gwarancyjnaPlatna, @statusKlient, @nrWRL, @nrKWZ2, @gdzieZgloszono,
                    @nrFakturyPrzychodu, @kwotaFakturyPrzychoduNetto, @kwotaFakturyPrzychoduBrutto,
                    @nrFakturyKosztowej, @kwotaFakturyKosztowejNetto, @kwotaFakturyKosztowejBrutto,
                    @statusProducent, @nrDokumentuProducenta, @czekamyNaDostawe, @nrKlienta,
                    @nrProduktu, @dzialania, @iloscSztuk, @tsEna);";

        foreach (IList<object> row in values)
        {
            string nrZgloszenia = row.Count > 0 ? row[0]?.ToString() ?? string.Empty : string.Empty;
            string klient = row.Count > 1 ? row[1]?.ToString() ?? string.Empty : string.Empty;
            string produkt = row.Count > 2 ? row[2]?.ToString() ?? string.Empty : string.Empty;
            string dataZgloszenia = row.Count > 3 ? row[3]?.ToString() ?? string.Empty : string.Empty;
            string dataZakupu = row.Count > 4 ? row[4]?.ToString() ?? string.Empty : string.Empty;
            string nrFaktury = row.Count > 5 ? row[5]?.ToString() ?? string.Empty : string.Empty;
            string nrSeryjny = row.Count > 6 ? row[6]?.ToString() ?? string.Empty : string.Empty;
            string usterka = row.Count > 7 ? row[7]?.ToString() ?? string.Empty : string.Empty;
            string gwarancyjnaPlatna = row.Count > 8 ? row[8]?.ToString() ?? string.Empty : string.Empty;
            string statusKlient = row.Count > 9 ? row[9]?.ToString() ?? string.Empty : string.Empty;
            string nrWRL = row.Count > 10 ? row[10]?.ToString() ?? string.Empty : string.Empty;
            string nrKWZ2 = row.Count > 11 ? row[11]?.ToString() ?? string.Empty : string.Empty;
            string gdzieZgloszono = row.Count > 12 ? row[12]?.ToString() ?? string.Empty : string.Empty;
            string nrFakturyPrzychodu = row.Count > 13 ? row[13]?.ToString() ?? string.Empty : string.Empty;
            double kwotaFakturyPrzychoduNetto = row.Count > 14 && double.TryParse(row[14]?.ToString(), out double netto) ? netto : 0.0;
            double kwotaFakturyPrzychoduBrutto = row.Count > 15 && double.TryParse(row[15]?.ToString(), out double brutto) ? brutto : 0.0;
            string nrFakturyKosztowej = row.Count > 16 ? row[16]?.ToString() ?? string.Empty : string.Empty;
            double kwotaFakturyKosztowejNetto = row.Count > 17 && double.TryParse(row[17]?.ToString(), out double kn) ? kn : 0.0;
            double kwotaFakturyKosztowejBrutto = row.Count > 18 && double.TryParse(row[18]?.ToString(), out double kb) ? kb : 0.0;
            string statusProducent = row.Count > 19 ? row[19]?.ToString() ?? string.Empty : string.Empty;
            string nrDokumentuProducenta = row.Count > 20 ? row[20]?.ToString() ?? string.Empty : string.Empty;
            string czekamyNaDostawe = row.Count > 21 ? row[21]?.ToString() ?? string.Empty : string.Empty;
            string nrKlienta = row.Count > 22 ? row[22]?.ToString() ?? string.Empty : string.Empty;
            string nrProduktu = row.Count > 23 ? row[23]?.ToString() ?? string.Empty : string.Empty;
            string dzialania = row.Count > 24 ? row[24]?.ToString() ?? string.Empty : string.Empty;
            int iloscSztuk = row.Count > 25 && int.TryParse(row[25]?.ToString(), out int ilosc) ? ilosc : 0;
            string tsEna = row.Count > 26 ? row[26]?.ToString() ?? string.Empty : string.Empty;

            var parameters = new[]
            {
                new SQLiteParameter("@nrZgloszenia", nrZgloszenia),
                new SQLiteParameter("@klient", klient),
                new SQLiteParameter("@produkt", produkt),
                new SQLiteParameter("@dataZgloszenia", dataZgloszenia),
                new SQLiteParameter("@dataZakupu", dataZakupu),
                new SQLiteParameter("@nrFaktury", nrFaktury),
                new SQLiteParameter("@nrSeryjny", nrSeryjny),
                new SQLiteParameter("@usterka", usterka),
                new SQLiteParameter("@gwarancyjnaPlatna", gwarancyjnaPlatna),
                new SQLiteParameter("@statusKlient", statusKlient),
                new SQLiteParameter("@nrWRL", nrWRL),
                new SQLiteParameter("@nrKWZ2", nrKWZ2),
                new SQLiteParameter("@gdzieZgloszono", gdzieZgloszono),
                new SQLiteParameter("@nrFakturyPrzychodu", nrFakturyPrzychodu),
                new SQLiteParameter("@kwotaFakturyPrzychoduNetto", kwotaFakturyPrzychoduNetto),
                new SQLiteParameter("@kwotaFakturyPrzychoduBrutto", kwotaFakturyPrzychoduBrutto),
                new SQLiteParameter("@nrFakturyKosztowej", nrFakturyKosztowej),
                new SQLiteParameter("@kwotaFakturyKosztowejNetto", kwotaFakturyKosztowejNetto),
                new SQLiteParameter("@kwotaFakturyKosztowejBrutto", kwotaFakturyKosztowejBrutto),
                new SQLiteParameter("@statusProducent", statusProducent),
                new SQLiteParameter("@nrDokumentuProducenta", nrDokumentuProducenta),
                new SQLiteParameter("@czekamyNaDostawe", czekamyNaDostawe),
                new SQLiteParameter("@nrKlienta", nrKlienta),
                new SQLiteParameter("@nrProduktu", nrProduktu),
                new SQLiteParameter("@dzialania", dzialania),
                new SQLiteParameter("@iloscSztuk", iloscSztuk),
                new SQLiteParameter("@tsEna", tsEna)
            };

            try
            {
                await _dbService.ExecuteNonQueryAsync(query, parameters);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Błąd synchronizacji zgłoszenia {nrZgloszenia}: {ex.Message}");
            }
        }
    }
}
