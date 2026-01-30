using System;
using System.Collections.Generic;
using System.Data;
using MySql.Data.MySqlClient;
using System.Drawing;
using System.IO;
using System.Threading.Tasks;

namespace Reklamacje_Dane
{
    // --- MODELE DANYCH ---

    public class SzablonCzesci
    {
        public int Id { get; set; }
        public int ProduktId { get; set; }
        public string NazwaCzesci { get; set; }
    }

    public class StanMagazynowy
    {
        public int Id { get; set; }
        public string NrZgloszenia { get; set; }
        public string Model { get; set; }
        public string Producent { get; set; } // Pola wymagane przez Formularz
        public string NumerSeryjny { get; set; }
        public int ProduktId { get; set; }
        public string StatusFizyczny { get; set; }
        public string Lokalizacja { get; set; }
        public bool CzyDawca { get; set; }
        public string Uwagi { get; set; }

        // Pola pomocnicze do kolorowania (Obliczane w SQL)
        public int LiczbaCzesciRazem { get; set; }
        public int LiczbaCzesciDostepnych { get; set; }
    }

    public class DostepnaCzescView
    {
        public int Id { get; set; }
        public string NazwaCzesci { get; set; }
        public string ModelDawcy { get; set; }
        public string SnDawcy { get; set; }
        public string ZgloszenieDawcy { get; set; }
        public string Lokalizacja { get; set; }
        public string StanOpis { get; set; }
        public string TypPochodzenia { get; set; }
    }

    public class MagazynService
    {
        public class StanIlosciowyView
        {
            public string NazwaCzesci { get; set; }
            public int Ilosc { get; set; }
            public string Modele { get; set; }
        }

        private readonly DatabaseService _dbService;

        public MagazynService()
        {
            _dbService = new DatabaseService(DatabaseHelper.GetConnectionString());
        }

        // ====================================================================
        // 1. SZABLONY
        // ====================================================================

        public async Task<List<SzablonCzesci>> PobierzSzablonyDlaProduktuAsync(int produktId)
        {
            var list = new List<SzablonCzesci>();
            string sql = "SELECT Id, ProduktId, NazwaCzesci FROM SzablonyCzesci WHERE ProduktId = @pid ORDER BY NazwaCzesci";
            var dt = await _dbService.GetDataTableAsync(sql, new MySqlParameter("@pid", produktId));
            foreach (DataRow row in dt.Rows)
            {
                list.Add(new SzablonCzesci
                {
                    Id = Convert.ToInt32(row["Id"]),
                    ProduktId = Convert.ToInt32(row["ProduktId"]),
                    NazwaCzesci = row["NazwaCzesci"].ToString()
                });
            }
            return list;
        }

        public async Task DodajSzablonCzesciAsync(int produktId, string nazwa)
        {
            string sql = "INSERT INTO SzablonyCzesci (ProduktId, NazwaCzesci) VALUES (@pid, @nazwa)";
            await _dbService.ExecuteNonQueryAsync(sql, new MySqlParameter("@pid", produktId), new MySqlParameter("@nazwa", nazwa));
        }

        public async Task UsunSzablonCzesciAsync(int szablonId)
        {
            string sql = "DELETE FROM SzablonyCzesci WHERE Id = @id";
            await _dbService.ExecuteNonQueryAsync(sql, new MySqlParameter("@id", szablonId));
        }

        public async Task<List<string>> PobierzSugestieCzesciAsync(string aktualnaKategoria)
        {
            var lista = new List<string>();
            string sql = @"SELECT s.NazwaCzesci, SUM(CASE WHEN p.Kategoria = @kat THEN 100 ELSE 1 END) as Waga
                           FROM SzablonyCzesci s JOIN Produkty p ON s.ProduktId = p.Id
                           GROUP BY s.NazwaCzesci ORDER BY Waga DESC, s.NazwaCzesci ASC";
            try
            {
                var dt = await _dbService.GetDataTableAsync(sql, new MySqlParameter("@kat", aktualnaKategoria ?? ""));
                foreach (DataRow row in dt.Rows) lista.Add(row["NazwaCzesci"].ToString());
            }
            catch { }
            return lista;
        }

        // ====================================================================
        // 2. STAN MAGAZYNOWY (URZĄDZENIA)
        // ====================================================================

        public async Task<List<StanMagazynowy>> PobierzWszystkieUrzadzeniaAsync()
        {
            var list = new List<StanMagazynowy>();

            // POPRAWKA: Dodano "z.ProduktID" do SELECT, żeby było dostępne w wyniku
            string sql = @"
                SELECT 
                    m.*, 
                    p.NazwaSystemowa, 
                    p.Producent,
                    z.NrSeryjny,
                    z.ProduktID,  -- <--- TO JEST KLUCZOWE, TEGO BRAKOWAŁO
                    (SELECT COUNT(*) FROM SzablonyCzesci s WHERE s.ProduktId = p.Id) AS TotalParts,
                    (SELECT COUNT(*) FROM DostepneCzesci d WHERE d.MagazynDawcaId = m.Id AND d.CzyDostepna = 1) AS ExistingParts
                FROM MagazynZwrotow m 
                LEFT JOIN Zgloszenia z ON m.NrZgloszenia = z.NrZgloszenia
                LEFT JOIN Produkty p ON z.ProduktID = p.Id
                WHERE (m.StatusFizyczny IS NULL OR m.StatusFizyczny NOT LIKE '%Odesłany%')
                ORDER BY m.Id DESC";

            var dt = await _dbService.GetDataTableAsync(sql);
            foreach (DataRow r in dt.Rows)
            {
                string model = !string.IsNullOrEmpty(r["Model"].ToString()) ? r["Model"].ToString() : r["NazwaSystemowa"].ToString();
                string sn = !string.IsNullOrEmpty(r["NumerSeryjny"].ToString()) ? r["NumerSeryjny"].ToString() : r["NrSeryjny"].ToString();

                int total = r["TotalParts"] != DBNull.Value ? Convert.ToInt32(r["TotalParts"]) : 0;
                int existing = r["ExistingParts"] != DBNull.Value ? Convert.ToInt32(r["ExistingParts"]) : 0;

                string uwagiBaza = r["UwagiMagazynowe"].ToString();
                string statusKompletności = "";

                if (total > 0)
                {
                    if (existing >= total) statusKompletności = "[KOMPLETNY]";
                    else statusKompletności = $"[BRAKI: {existing}/{total}]";
                }
                else if (Convert.ToBoolean(r["CzyDawca"]))
                {
                    statusKompletności = $"[Części: {existing}]";
                }

                string displayUwagi = $"{statusKompletności} {uwagiBaza}".Trim();

                list.Add(new StanMagazynowy
                {
                    Id = Convert.ToInt32(r["Id"]),
                    NrZgloszenia = r["NrZgloszenia"].ToString(),
                    Model = model,
                    NumerSeryjny = sn,
                    Producent = r["Producent"]?.ToString() ?? "",
                    // TERAZ TO ZADZIAŁA, BO KOLUMNA JEST W WYNIKU:
                    ProduktId = r["ProduktID"] != DBNull.Value ? Convert.ToInt32(r["ProduktID"]) : 0,
                    StatusFizyczny = r["StatusFizyczny"].ToString(),
                    Lokalizacja = r["Lokalizacja"].ToString(),
                    CzyDawca = r["CzyDawca"] != DBNull.Value && Convert.ToInt32(r["CzyDawca"]) == 1,
                    Uwagi = displayUwagi,
                    LiczbaCzesciRazem = total,
                    LiczbaCzesciDostepnych = existing
                });
            }
            return list;
        }

        public async Task<StanMagazynowy> PobierzStanAsync(string nrZgloszenia)
        {
            string query = "SELECT Id, StatusFizyczny, Lokalizacja, CzyDawca, UwagiMagazynowe FROM MagazynZwrotow WHERE NrZgloszenia = @nr";
            var dt = await _dbService.GetDataTableAsync(query, new MySqlParameter("@nr", nrZgloszenia));
            if (dt.Rows.Count > 0)
            {
                var row = dt.Rows[0];
                return new StanMagazynowy
                {
                    Id = Convert.ToInt32(row["Id"]),
                    NrZgloszenia = nrZgloszenia,
                    StatusFizyczny = row["StatusFizyczny"].ToString(),
                    Lokalizacja = row["Lokalizacja"].ToString(),
                    CzyDawca = row["CzyDawca"] != DBNull.Value && Convert.ToInt32(row["CzyDawca"]) == 1,
                    Uwagi = row["UwagiMagazynowe"].ToString()
                };
            }
            return null;
        }

        public async Task PrzyjmijNaMagazynAsync(string nrZgloszenia, string model, string sn, string uwagi)
        {
            string checkSql = "SELECT COUNT(*) FROM MagazynZwrotow WHERE NrZgloszenia = @nr";
            var exists = await _dbService.ExecuteScalarAsync(checkSql, new MySqlParameter("@nr", nrZgloszenia));
            if (Convert.ToInt32(exists) > 0) return;

            string sql = @"INSERT INTO MagazynZwrotow (NrZgloszenia, Model, NumerSeryjny, DataPrzyjecia, StatusFizyczny, Lokalizacja, UwagiMagazynowe, CzyDawca)
                           VALUES (@nr, @model, @sn, @data, 'Przyjęty na stan', 'Magazyn Przyjęć', @uwagi, 0)";
            await _dbService.ExecuteNonQueryAsync(sql,
                new MySqlParameter("@nr", nrZgloszenia), new MySqlParameter("@model", model),
                new MySqlParameter("@sn", sn), new MySqlParameter("@data", DateTime.Now.ToString("yyyy-MM-dd HH:mm")),
                new MySqlParameter("@uwagi", uwagi));
        }

        public async Task AktualizujStatusAsync(string nrZgloszenia, string status, string lokalizacja, bool czyDawca, string uwagi)
        {
            string sql = @"UPDATE MagazynZwrotow SET StatusFizyczny = @status, Lokalizacja = @lok, CzyDawca = @dawca, UwagiMagazynowe = @uwagi WHERE NrZgloszenia = @nr";
            await _dbService.ExecuteNonQueryAsync(sql,
                new MySqlParameter("@status", status), new MySqlParameter("@lok", lokalizacja),
                new MySqlParameter("@dawca", czyDawca ? 1 : 0), new MySqlParameter("@uwagi", uwagi),
                new MySqlParameter("@nr", nrZgloszenia));
        }

        public async Task<int> PobierzIdMagazynoweAsync(string nrZgloszenia)
        {
            string sql = "SELECT Id FROM MagazynZwrotow WHERE NrZgloszenia = @nr";
            var res = await _dbService.ExecuteScalarAsync(sql, new MySqlParameter("@nr", nrZgloszenia));
            return res != null ? Convert.ToInt32(res) : 0;
        }

        // ====================================================================
        // 3. LOKALIZACJE
        // ====================================================================

        public async Task<List<string>> PobierzLokalizacjeAsync()
        {
            var list = new List<string>();
            try
            {
                var dt = await _dbService.GetDataTableAsync("SELECT Nazwa FROM SlownikLokalizacji ORDER BY Nazwa");
                foreach (DataRow row in dt.Rows) list.Add(row["Nazwa"].ToString());
            }
            catch { }
            return list;
        }

        public async Task DodajLokalizacjeAsync(string nazwa)
        {
            await _dbService.ExecuteNonQueryAsync("INSERT OR IGNORE INTO SlownikLokalizacji (Nazwa) VALUES (@n)", new MySqlParameter("@n", nazwa));
        }

        public async Task UsunLokalizacjeAsync(string nazwa)
        {
            await _dbService.ExecuteNonQueryAsync("DELETE FROM SlownikLokalizacji WHERE Nazwa = @n", new MySqlParameter("@n", nazwa));
        }

        // ====================================================================
        // 4. CZĘŚCI
        // ====================================================================

        public async Task ZapiszCzesciZDemontazuAsync(int magazynZwrotId, List<string> nazwyCzesci)
        {
            await _dbService.ExecuteNonQueryAsync("DELETE FROM DostepneCzesci WHERE MagazynDawcaId = @id", new MySqlParameter("@id", magazynZwrotId));
            if (nazwyCzesci.Count == 0) return;

            string sql = "INSERT INTO DostepneCzesci (MagazynDawcaId, NazwaCzesci, StanOpis, CzyDostepna, TypPochodzenia) VALUES (@id, @nazwa, 'Używana - z demontażu', 1, 'Demontaż')";
            using (var con = DatabaseHelper.GetConnection())
            {
                await con.OpenAsync();
                using (var tr = con.BeginTransaction())
                {
                    foreach (var n in nazwyCzesci)
                    {
                        using (var cmd = new MySqlCommand(sql, con, tr))
                        {
                            cmd.Parameters.AddWithValue("@id", magazynZwrotId);
                            cmd.Parameters.AddWithValue("@nazwa", n);
                            await cmd.ExecuteNonQueryAsync();
                        }
                    }
                    tr.Commit();
                }
            }
        }

        public async Task DodajNadwyzkeCzesciAsync(int produktId, string nazwaCzesci, int ilosc, string lokalizacja, string uwagi)
        {
            string sql = @"INSERT INTO DostepneCzesci (ProduktId, NazwaCzesci, StanOpis, CzyDostepna, Lokalizacja, TypPochodzenia, MagazynDawcaId)
                           VALUES (@pid, @nazwa, @opis, 1, @lok, 'Nadwyżka', NULL)";

            using (var con = Database.GetNewOpenConnection())
            using (var tr = con.BeginTransaction())
            {
                try
                {
                    using (var cmd = new MySqlCommand(sql, con, tr))
                    {
                        cmd.Parameters.AddWithValue("@pid", produktId);
                        cmd.Parameters.AddWithValue("@nazwa", nazwaCzesci);
                        cmd.Parameters.AddWithValue("@opis", uwagi);
                        cmd.Parameters.AddWithValue("@lok", lokalizacja);
                        for (int i = 0; i < ilosc; i++) await cmd.ExecuteNonQueryAsync();
                    }
                    tr.Commit();
                }
                catch { tr.Rollback(); throw; }
            }
        }

        public async Task<List<string>> PobierzZapisaneCzesciDlaDawcyAsync(int magazynZwrotId)
        {
            var l = new List<string>();
            var dt = await _dbService.GetDataTableAsync("SELECT NazwaCzesci FROM DostepneCzesci WHERE MagazynDawcaId = @id", new MySqlParameter("@id", magazynZwrotId));
            foreach (DataRow r in dt.Rows) l.Add(r["NazwaCzesci"].ToString());
            return l;
        }

        public async Task<DataTable> PobierzListeProduktowAsync()
        {
            return await _dbService.GetDataTableAsync("SELECT Id, NazwaSystemowa FROM Produkty ORDER BY NazwaSystemowa");
        }

        // --- TO JEST METODA, KTÓREJ BRAKOWAŁO (Wrapper) ---
        public async Task<List<DostepnaCzescView>> PobierzWszystkieCzesciAsync()
        {
            return await PobierzDostepneCzesciAsync("");
        }

        public async Task<List<DostepnaCzescView>> PobierzDostepneCzesciAsync(string fraza = "")
        {
            var list = new List<DostepnaCzescView>();
            string sql = @"
                SELECT d.Id, d.NazwaCzesci, d.Lokalizacja, d.TypPochodzenia, d.StanOpis,
                       m.Model AS ModelDawcy, m.NumerSeryjny AS SnDawcy, m.NrZgloszenia AS ZgloszenieDawcy,
                       p.NazwaSystemowa AS NazwaProduktu
                FROM DostepneCzesci d 
                LEFT JOIN MagazynZwrotow m ON d.MagazynDawcaId = m.Id
                LEFT JOIN Produkty p ON d.ProduktId = p.Id
                WHERE d.CzyDostepna = 1";

            var pars = new List<MySqlParameter>();
            if (!string.IsNullOrEmpty(fraza))
            {
                sql += " AND (d.NazwaCzesci LIKE @q OR m.Model LIKE @q OR p.NazwaSystemowa LIKE @q)";
                pars.Add(new MySqlParameter("@q", $"%{fraza}%"));
            }
            sql += " ORDER BY d.NazwaCzesci";

            var dt = await _dbService.GetDataTableAsync(sql, pars.ToArray());
            foreach (DataRow r in dt.Rows)
            {
                string model = r["TypPochodzenia"]?.ToString() == "Nadwyżka"
                    ? (r["NazwaProduktu"]?.ToString() ?? "Produkt") + " (NOWA)"
                    : r["ModelDawcy"]?.ToString() ?? "Dawca";

                list.Add(new DostepnaCzescView
                {
                    Id = Convert.ToInt32(r["Id"]),
                    NazwaCzesci = r["NazwaCzesci"].ToString(),
                    ModelDawcy = model,
                    SnDawcy = r["SnDawcy"]?.ToString() ?? "-",
                    ZgloszenieDawcy = r["ZgloszenieDawcy"]?.ToString() ?? "-",
                    Lokalizacja = r["Lokalizacja"]?.ToString(),
                    StanOpis = r["StanOpis"]?.ToString(),
                    TypPochodzenia = r["TypPochodzenia"]?.ToString()
                });
            }
            return list;
        }

        public async Task<List<StanIlosciowyView>> PobierzPodsumowanieCzesciAsync()
        {
            var list = new List<StanIlosciowyView>();
            string sql = @"SELECT NazwaCzesci, COUNT(*) as Ilosc FROM DostepneCzesci WHERE CzyDostepna=1 GROUP BY NazwaCzesci ORDER BY NazwaCzesci";
            var dt = await _dbService.GetDataTableAsync(sql);
            foreach (DataRow r in dt.Rows)
            {
                list.Add(new StanIlosciowyView { NazwaCzesci = r["NazwaCzesci"].ToString(), Ilosc = Convert.ToInt32(r["Ilosc"]), Modele = "Różne" });
            }
            return list;
        }

        public async Task<List<DostepnaCzescView>> PobierzHistorieZuzytychCzesciAsync()
        {
            var list = new List<DostepnaCzescView>();
            string sql = @"SELECT d.Id, d.NazwaCzesci, m.Model, d.UzytoWZgloszeniu, d.DataUzycia 
                           FROM DostepneCzesci d LEFT JOIN MagazynZwrotow m ON d.MagazynDawcaId=m.Id 
                           WHERE d.CzyDostepna=0 ORDER BY d.DataUzycia DESC";
            var dt = await _dbService.GetDataTableAsync(sql);
            foreach (DataRow r in dt.Rows)
            {
                list.Add(new DostepnaCzescView
                {
                    Id = Convert.ToInt32(r["Id"]),
                    NazwaCzesci = r["NazwaCzesci"].ToString(),
                    ModelDawcy = r["Model"]?.ToString() ?? "Nadwyżka",
                    ZgloszenieDawcy = r["UzytoWZgloszeniu"]?.ToString(),
                    Lokalizacja = r["DataUzycia"]?.ToString()
                });
            }
            return list;
        }

        // --- 5. OPERACJE ---

        public async Task UsunUrzadzenieZMagazynuAsync(int id)
        {
            await _dbService.ExecuteNonQueryAsync("DELETE FROM DostepneCzesci WHERE MagazynDawcaId = @id", new MySqlParameter("@id", id));
            await _dbService.ExecuteNonQueryAsync("DELETE FROM MagazynZwrotow WHERE Id = @id", new MySqlParameter("@id", id));
        }

        public async Task UsunCzescZBankuAsync(int id)
        {
            await _dbService.ExecuteNonQueryAsync("DELETE FROM DostepneCzesci WHERE Id = @id", new MySqlParameter("@id", id));
        }

        public async Task UzyjCzescAsync(int czescId, string nrZgloszenia)
        {
            await _dbService.ExecuteNonQueryAsync("UPDATE DostepneCzesci SET CzyDostepna=0, UzytoWZgloszeniu=@nr, DataUzycia=@d WHERE Id=@id",
                new MySqlParameter("@nr", nrZgloszenia), new MySqlParameter("@d", DateTime.Now.ToString("yyyy-MM-dd HH:mm")), new MySqlParameter("@id", czescId));
        }

        public async Task RozchodCzescAsync(int czescId, string komentarz)
        {
            string opis = $"ROZCHÓD: {komentarz}";
            await _dbService.ExecuteNonQueryAsync("UPDATE DostepneCzesci SET CzyDostepna=0, UzytoWZgloszeniu=@opis, DataUzycia=@d WHERE Id=@id",
                new MySqlParameter("@opis", opis), new MySqlParameter("@d", DateTime.Now.ToString("yyyy-MM-dd HH:mm")), new MySqlParameter("@id", czescId));
        }

        public async Task PrzywrocCzescNaStanAsync(int czescId)
        {
            await _dbService.ExecuteNonQueryAsync("UPDATE DostepneCzesci SET CzyDostepna=1, UzytoWZgloszeniu=NULL, DataUzycia=NULL WHERE Id=@id", new MySqlParameter("@id", czescId));
        }

        // --- 6. ZDJĘCIA ---

        public async Task DodajZdjecieAsync(int magazynId, string sciezka)
        {
            string folder = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Dane", $"ID_{magazynId}");
            if (!Directory.Exists(folder)) Directory.CreateDirectory(folder);
            string nazwa = $"{DateTime.Now:yyyyMMdd_HHmmss}_{Path.GetFileName(sciezka)}";
            string cel = Path.Combine(folder, nazwa);
            File.Copy(sciezka, cel);

            string relPath = Path.Combine("Dane", $"ID_{magazynId}", nazwa);
            await _dbService.ExecuteNonQueryAsync("INSERT INTO ZdjeciaMagazynowe (MagazynId, NazwaPliku, SciezkaPliku) VALUES (@m, @n, @s)",
                new MySqlParameter("@m", magazynId), new MySqlParameter("@n", nazwa), new MySqlParameter("@s", relPath));
        }

        public async Task<List<Image>> PobierzZdjeciaDlaMagazynuAsync(int magazynId)
        {
            var l = new List<Image>();
            var dt = await _dbService.GetDataTableAsync("SELECT SciezkaPliku FROM ZdjeciaMagazynowe WHERE MagazynId=@m", new MySqlParameter("@m", magazynId));
            foreach (DataRow r in dt.Rows)
            {
                try
                {
                    string path = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, r["SciezkaPliku"].ToString());
                    if (File.Exists(path))
                    {
                        using (var fs = new FileStream(path, FileMode.Open, FileAccess.Read)) l.Add(Image.FromStream(fs));
                    }
                }
                catch { }
            }
            return l;
        }
    }
}
