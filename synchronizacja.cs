using System;
using System.Globalization;
using MySql.Data.MySqlClient;
using Google.Apis.Sheets.v4;
using Reklamacje_Dane; // Upewnij się, że ta przestrzeń nazw jest poprawna

public class DataSynchronizer
{
    private SheetsService service;
    private string spreadsheetId;
    // Usunęliśmy pole 'database', ponieważ będziemy pobierać nowe połączenia

    public DataSynchronizer(SheetsService service, string spreadsheetId)
    {
        this.service = service;
        this.spreadsheetId = spreadsheetId;
    }

    public void SynchronizeWithGoogleSheets()
    {
        // Tworzymy nowe połączenie, które będzie żyło tylko na czas synchronizacji.
        // Blok 'using' gwarantuje, że połączenie zostanie zamknięte po zakończeniu operacji.
        using (var connection = Database.GetNewOpenConnection())
        {
            SynchronizeZgloszenia(connection);
            SynchronizeKlienci(connection);
            SynchronizeProdukty(connection);
            SynchronizeProducenci(connection);
            SynchronizePrzypomnienia(connection);
            SynchronizeKategorie(connection);
            SynchronizeStatusy(connection);
            SynchronizeDziennik(connection);
        }
    }

    // Poniżej znajdują się wszystkie metody synchronizacji,
    // przepisane z użyciem zapytań sparametryzowanych i transakcji dla wydajności.

    private void SynchronizeZgloszenia(MySqlConnection connection)
    {
        var range = "Zgloszenia!A:Z";
        var request = service.Spreadsheets.Values.Get(spreadsheetId, range);
        var response = request.Execute();
        var values = response.Values;
        if (values == null || values.Count <= 1) return; // <= 1 aby pominąć nagłówek

        using (var transaction = connection.BeginTransaction())
        {
            var query = @"INSERT OR REPLACE INTO Zgloszenia (nrZgloszenia, klient, produkt, dataZgloszenia, dataZakupu, nrFaktury, nrSeryjny, usterka, gwarancyjnaPlatna, statusKlient, nrWRL, nrKWZ2, gdzieZgloszono, nrFakturyPrzychodu, kwotaFakturyPrzychoduNetto, kwotaFakturyPrzychoduBrutto, nrFakturyKosztowej, kwotaFakturyKosztowejNetto, kwotaFakturyKosztowejBrutto, statusProducent, nrDokumentuProducenta, czekamyNaDostawe, nrKlienta, nrProduktu, dzialania, iloscSztuk, tsEna) VALUES (@nrZgloszenia, @klient, @produkt, @dataZgloszenia, @dataZakupu, @nrFaktury, @nrSeryjny, @usterka, @gwarancyjnaPlatna, @statusKlient, @nrWRL, @nrKWZ2, @gdzieZgloszono, @nrFakturyPrzychodu, @kwotaFakturyPrzychoduNetto, @kwotaFakturyPrzychoduBrutto, @nrFakturyKosztowej, @kwotaFakturyKosztowejNetto, @kwotaFakturyKosztowejBrutto, @statusProducent, @nrDokumentuProducenta, @czekamyNaDostawe, @nrKlienta, @nrProduktu, @dzialania, @iloscSztuk, @tsEna);";
            using (var command = new MySqlCommand(query, connection))
            {
                for (int i = 1; i < values.Count; i++) // Zaczynamy od 1, aby pominąć nagłówek
                {
                    var row = values[i];
                    if (row.Count == 0 || string.IsNullOrWhiteSpace(row[0]?.ToString())) continue;

                    command.Parameters.Clear();
                    command.Parameters.AddWithValue("@nrZgloszenia", row[0].ToString());
                    command.Parameters.AddWithValue("@klient", row.Count > 1 ? row[1].ToString() : "Brak danych klienta");
                    command.Parameters.AddWithValue("@produkt", row.Count > 2 ? row[2].ToString() : "Brak danych produktu");

                    DateTime.TryParse(row.Count > 3 ? row[3].ToString() : "", out DateTime dataZgloszenia);
                    command.Parameters.AddWithValue("@dataZgloszenia", dataZgloszenia != DateTime.MinValue ? dataZgloszenia.ToString("dd.MM.yyyy") : "Brak daty zgłoszenia");

                    DateTime.TryParse(row.Count > 4 ? row[4].ToString() : "", out DateTime dataZakupu);
                    command.Parameters.AddWithValue("@dataZakupu", dataZakupu != DateTime.MinValue ? dataZakupu.ToString("dd.MM.yyyy") : "Brak daty zakupu");

                    command.Parameters.AddWithValue("@nrFaktury", row.Count > 5 ? row[5].ToString() : "Brak nr faktury");
                    command.Parameters.AddWithValue("@nrSeryjny", row.Count > 6 ? row[6].ToString() : "Brak nr seryjnego");
                    command.Parameters.AddWithValue("@usterka", row.Count > 7 ? row[7].ToString() : "Brak opisu usterki");
                    command.Parameters.AddWithValue("@gwarancyjnaPlatna", row.Count > 8 ? row[8].ToString() : "Brak informacji");
                    command.Parameters.AddWithValue("@statusKlient", row.Count > 9 ? row[9].ToString() : "Brak statusu klienta");
                    command.Parameters.AddWithValue("@nrWRL", row.Count > 10 ? row[10].ToString() : "Brak nr WRL");
                    command.Parameters.AddWithValue("@nrKWZ2", row.Count > 11 ? row[11].ToString() : "Brak nr KWZ2");
                    command.Parameters.AddWithValue("@gdzieZgloszono", row.Count > 12 ? row[12].ToString() : "Brak informacji gdzie zgłoszono");
                    command.Parameters.AddWithValue("@nrFakturyPrzychodu", row.Count > 13 ? row[13].ToString() : "Brak nr faktury przychodu");

                    double.TryParse(row.Count > 14 ? row[14].ToString().Replace(",", ".") : "0", NumberStyles.Any, CultureInfo.InvariantCulture, out double kwotaNetto);
                    command.Parameters.AddWithValue("@kwotaFakturyPrzychoduNetto", kwotaNetto);

                    double.TryParse(row.Count > 15 ? row[15].ToString().Replace(",", ".") : "0", NumberStyles.Any, CultureInfo.InvariantCulture, out double kwotaBrutto);
                    command.Parameters.AddWithValue("@kwotaFakturyPrzychoduBrutto", kwotaBrutto);

                    command.Parameters.AddWithValue("@nrFakturyKosztowej", row.Count > 16 ? row[16].ToString() : "Brak nr faktury kosztowej");

                    double.TryParse(row.Count > 17 ? row[17].ToString().Replace(",", ".") : "0", NumberStyles.Any, CultureInfo.InvariantCulture, out double kwotaKosztNetto);
                    command.Parameters.AddWithValue("@kwotaFakturyKosztowejNetto", kwotaKosztNetto);

                    double.TryParse(row.Count > 18 ? row[18].ToString().Replace(",", ".") : "0", NumberStyles.Any, CultureInfo.InvariantCulture, out double kwotaKosztBrutto);
                    command.Parameters.AddWithValue("@kwotaFakturyKosztowejBrutto", kwotaKosztBrutto);

                    command.Parameters.AddWithValue("@statusProducent", row.Count > 19 ? row[19].ToString() : "Brak statusu producenta");
                    command.Parameters.AddWithValue("@nrDokumentuProducenta", row.Count > 20 ? row[20].ToString() : "Brak nr dokumentu producenta");
                    command.Parameters.AddWithValue("@czekamyNaDostawe", row.Count > 21 ? row[21].ToString() : "Brak informacji");
                    command.Parameters.AddWithValue("@nrKlienta", row.Count > 22 ? row[22].ToString() : "Brak nr klienta");
                    command.Parameters.AddWithValue("@nrProduktu", row.Count > 23 ? row[23].ToString() : "Brak nr produktu");
                    command.Parameters.AddWithValue("@dzialania", row.Count > 24 ? row[24].ToString() : "Brak działań");

                    int.TryParse(row.Count > 25 ? row[25].ToString() : "0", out int iloscSztuk);
                    command.Parameters.AddWithValue("@iloscSztuk", iloscSztuk);

                    command.Parameters.AddWithValue("@tsEna", row.Count > 26 ? row[26].ToString() : "Brak informacji");

                    command.ExecuteNonQuery();
                }
            }
            transaction.Commit();
        }
    }

    private void SynchronizeKlienci(MySqlConnection connection)
    {
        var range = "Klienci!A:K";
        var request = service.Spreadsheets.Values.Get(spreadsheetId, range);
        var response = request.Execute();
        var values = response.Values;
        if (values == null || values.Count <= 1) return;

        using (var transaction = connection.BeginTransaction())
        {
            var query = @"INSERT OR REPLACE INTO Klienci (id, dane, imieNazwisko, nazwaFirmy, nip, ulicaNumerDomu, kodPocztowy, miejscowosc, mail, telefon, firma) VALUES (@id, @dane, @imieNazwisko, @nazwaFirmy, @nip, @ulicaNumerDomu, @kodPocztowy, @miejscowosc, @mail, @telefon, @firma);";
            using (var command = new MySqlCommand(query, connection))
            {
                for (int i = 1; i < values.Count; i++)
                {
                    var row = values[i];
                    if (row.Count == 0 || string.IsNullOrWhiteSpace(row[0]?.ToString())) continue;

                    command.Parameters.Clear();
                    command.Parameters.AddWithValue("@id", row[0].ToString());
                    command.Parameters.AddWithValue("@dane", row.Count > 1 ? row[1].ToString() : "Brak danych");
                    command.Parameters.AddWithValue("@imieNazwisko", row.Count > 2 ? row[2].ToString() : "Brak imienia i nazwiska");
                    command.Parameters.AddWithValue("@nazwaFirmy", row.Count > 3 ? row[3].ToString() : "Brak nazwy firmy");
                    command.Parameters.AddWithValue("@nip", row.Count > 4 ? row[4].ToString() : "Brak NIP");
                    command.Parameters.AddWithValue("@ulicaNumerDomu", row.Count > 5 ? row[5].ToString() : "Brak adresu");
                    command.Parameters.AddWithValue("@kodPocztowy", row.Count > 6 ? row[6].ToString() : "Brak kodu pocztowego");
                    command.Parameters.AddWithValue("@miejscowosc", row.Count > 7 ? row[7].ToString() : "Brak miejscowości");
                    command.Parameters.AddWithValue("@mail", row.Count > 8 ? row[8].ToString() : "Brak maila");
                    command.Parameters.AddWithValue("@telefon", row.Count > 9 ? row[9].ToString() : "Brak telefonu");
                    command.Parameters.AddWithValue("@firma", row.Count > 10 ? row[10].ToString() : "Brak informacji");
                    command.ExecuteNonQuery();
                }
            }
            transaction.Commit();
        }
    }

    private void SynchronizeProdukty(MySqlConnection connection)
    {
        var range = "Produkty!A:G";
        var request = service.Spreadsheets.Values.Get(spreadsheetId, range);
        var response = request.Execute();
        var values = response.Values;
        if (values == null || values.Count <= 1) return;

        using (var transaction = connection.BeginTransaction())
        {
            var query = @"INSERT OR REPLACE INTO Produkty (id, nazwaSystemowa, nazwaKrotka, kodEnova, kodProducenta, kategoria, producent) VALUES (@id, @nazwaSystemowa, @nazwaKrotka, @kodEnova, @kodProducenta, @kategoria, @producent);";
            using (var command = new MySqlCommand(query, connection))
            {
                for (int i = 1; i < values.Count; i++)
                {
                    var row = values[i];
                    if (row.Count == 0 || string.IsNullOrWhiteSpace(row[0]?.ToString())) continue;

                    command.Parameters.Clear();
                    command.Parameters.AddWithValue("@id", row[0].ToString());
                    command.Parameters.AddWithValue("@nazwaSystemowa", row.Count > 1 ? row[1].ToString() : "Brak nazwy systemowej");
                    command.Parameters.AddWithValue("@nazwaKrotka", row.Count > 2 ? row[2].ToString() : "Brak nazwy krótkiej");
                    command.Parameters.AddWithValue("@kodEnova", row.Count > 3 ? row[3].ToString() : "Brak kodu Enova");
                    command.Parameters.AddWithValue("@kodProducenta", row.Count > 4 ? row[4].ToString() : "Brak kodu producenta");
                    command.Parameters.AddWithValue("@kategoria", row.Count > 5 ? row[5].ToString() : "Brak kategorii");
                    command.Parameters.AddWithValue("@producent", row.Count > 6 ? row[6].ToString() : "Brak producenta");
                    command.ExecuteNonQuery();
                }
            }
            transaction.Commit();
        }
    }

    private void SynchronizeProducenci(MySqlConnection connection)
    {
        var range = "Producenci!A:F";
        var request = service.Spreadsheets.Values.Get(spreadsheetId, range);
        var response = request.Execute();
        var values = response.Values;
        if (values == null || values.Count <= 1) return;

        using (var transaction = connection.BeginTransaction())
        {
            var query = @"INSERT OR REPLACE INTO Producenci (id, nazwaProducenta, kontaktMail, adres, plEng, formularz) VALUES (@id, @nazwaProducenta, @kontaktMail, @adres, @plEng, @formularz);";
            using (var command = new MySqlCommand(query, connection))
            {
                for (int i = 1; i < values.Count; i++)
                {
                    var row = values[i];
                    if (row.Count == 0 || string.IsNullOrWhiteSpace(row[0]?.ToString())) continue;

                    command.Parameters.Clear();
                    command.Parameters.AddWithValue("@id", row[0].ToString());
                    command.Parameters.AddWithValue("@nazwaProducenta", row.Count > 1 ? row[1].ToString() : "Brak nazwy producenta");
                    command.Parameters.AddWithValue("@kontaktMail", row.Count > 2 ? row[2].ToString() : "Brak maila");
                    command.Parameters.AddWithValue("@adres", row.Count > 3 ? row[3].ToString() : "Brak adresu");
                    command.Parameters.AddWithValue("@plEng", row.Count > 4 ? row[4].ToString() : "Brak informacji PL/ENG");
                    command.Parameters.AddWithValue("@formularz", row.Count > 5 ? row[5].ToString() : "Brak formularza");
                    command.ExecuteNonQuery();
                }
            }
            transaction.Commit();
        }
    }

    private void SynchronizePrzypomnienia(MySqlConnection connection)
    {
        var range = "Przypomnienia!A:F";
        var request = service.Spreadsheets.Values.Get(spreadsheetId, range);
        var response = request.Execute();
        var values = response.Values;
        if (values == null || values.Count <= 1) return;

        using (var transaction = connection.BeginTransaction())
        {
            var query = @"INSERT OR REPLACE INTO Przypomnienia (dataDodania, tresc, dotyczyZgloszenia, dataPrzypomnienia, realizacja, lp) VALUES (@dataDodania, @tresc, @dotyczyZgloszenia, @dataPrzypomnienia, @realizacja, @lp);";
            using (var command = new MySqlCommand(query, connection))
            {
                for (int i = 1; i < values.Count; i++)
                {
                    var row = values[i];
                    if (row.Count == 0 || string.IsNullOrWhiteSpace(row[0]?.ToString())) continue;

                    command.Parameters.Clear();
                    DateTime.TryParse(row.Count > 0 ? row[0].ToString() : "", out DateTime dataDodania);
                    command.Parameters.AddWithValue("@dataDodania", dataDodania != DateTime.MinValue ? dataDodania.ToString("dd.MM.yyyy") : "Brak daty dodania");

                    command.Parameters.AddWithValue("@tresc", row.Count > 1 ? row[1].ToString() : "Brak treści");
                    command.Parameters.AddWithValue("@dotyczyZgloszenia", row.Count > 2 ? row[2].ToString() : "Brak zgłoszenia");

                    DateTime.TryParse(row.Count > 3 ? row[3].ToString() : "", out DateTime dataPrzypomnienia);
                    command.Parameters.AddWithValue("@dataPrzypomnienia", dataPrzypomnienia != DateTime.MinValue ? dataPrzypomnienia.ToString("dd.MM.yyyy") : "Brak daty przypomnienia");

                    command.Parameters.AddWithValue("@realizacja", row.Count > 4 ? row[4].ToString() : "Brak realizacji");
                    command.Parameters.AddWithValue("@lp", row.Count > 5 ? row[5].ToString() : "Brak lp");
                    command.ExecuteNonQuery();
                }
            }
            transaction.Commit();
        }
    }

    private void SynchronizeKategorie(MySqlConnection connection)
    {
        var range = "Kategorie!A:B";
        var request = service.Spreadsheets.Values.Get(spreadsheetId, range);
        var response = request.Execute();
        var values = response.Values;
        if (values == null || values.Count <= 1) return;

        using (var transaction = connection.BeginTransaction())
        {
            var query = @"INSERT OR REPLACE INTO Kategorie (id, nazwaKategorii) VALUES (@id, @nazwaKategorii);";
            using (var command = new MySqlCommand(query, connection))
            {
                for (int i = 1; i < values.Count; i++)
                {
                    var row = values[i];
                    if (row.Count == 0 || string.IsNullOrWhiteSpace(row[0]?.ToString())) continue;

                    command.Parameters.Clear();
                    command.Parameters.AddWithValue("@id", row[0].ToString());
                    command.Parameters.AddWithValue("@nazwaKategorii", row.Count > 1 ? row[1].ToString() : "Brak nazwy kategorii");
                    command.ExecuteNonQuery();
                }
            }
            transaction.Commit();
        }
    }

    private void SynchronizeStatusy(MySqlConnection connection)
    {
        var range = "Statusy!A:B";
        var request = service.Spreadsheets.Values.Get(spreadsheetId, range);
        var response = request.Execute();
        var values = response.Values;
        if (values == null || values.Count <= 1) return;

        using (var transaction = connection.BeginTransaction())
        {
            var query = @"INSERT OR REPLACE INTO Statusy (statusKlient, statusProducent) VALUES (@statusKlient, @statusProducent);";
            using (var command = new MySqlCommand(query, connection))
            {
                for (int i = 1; i < values.Count; i++)
                {
                    var row = values[i];
                    if (row.Count == 0 || string.IsNullOrWhiteSpace(row[0]?.ToString())) continue;

                    command.Parameters.Clear();
                    command.Parameters.AddWithValue("@statusKlient", row.Count > 0 ? row[0].ToString() : "Brak statusu klienta");
                    command.Parameters.AddWithValue("@statusProducent", row.Count > 1 ? row[1].ToString() : "Brak statusu producenta");
                    command.ExecuteNonQuery();
                }
            }
            transaction.Commit();
        }
    }

    private void SynchronizeDziennik(MySqlConnection connection)
    {
        var range = "Dziennik!A:E";
        var request = service.Spreadsheets.Values.Get(spreadsheetId, range);
        var response = request.Execute();
        var values = response.Values;
        if (values == null || values.Count <= 1) return;

        using (var transaction = connection.BeginTransaction())
        {
            var query = @"INSERT OR REPLACE INTO Dziennik (id, data, login, tresc, dotyczyZgloszenia) VALUES (@id, @data, @login, @tresc, @dotyczyZgloszenia);";
            using (var command = new MySqlCommand(query, connection))
            {
                for (int i = 1; i < values.Count; i++)
                {
                    var row = values[i];
                    if (row.Count == 0 || string.IsNullOrWhiteSpace(row[0]?.ToString())) continue;

                    command.Parameters.Clear();
                    command.Parameters.AddWithValue("@id", row[0].ToString());

                    DateTime.TryParse(row.Count > 1 ? row[1].ToString() : "", out DateTime data);
                    command.Parameters.AddWithValue("@data", data != DateTime.MinValue ? data.ToString("dd.MM.yyyy") : "Brak daty");

                    command.Parameters.AddWithValue("@login", row.Count > 2 ? row[2].ToString() : "Brak loginu");
                    command.Parameters.AddWithValue("@tresc", row.Count > 3 ? row[3].ToString() : "Brak treści");
                    command.Parameters.AddWithValue("@dotyczyZgloszenia", row.Count > 4 ? row[4].ToString() : "Brak zgłoszenia");
                    command.ExecuteNonQuery();
                }
            }
            transaction.Commit();
        }
    }
}