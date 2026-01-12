using System;
using System.Collections.Generic;
using System.Data;
using MySql.Data.MySqlClient;
using System.Threading.Tasks;

namespace Reklamacje_Dane
{
    public class EmailTemplateService
    {
        private readonly DatabaseService _dbService;
        private readonly string _connectionString;

        public EmailTemplateService()
        {
            _dbService = new DatabaseService(DatabaseHelper.GetConnectionString());
            _connectionString = DatabaseHelper.GetConnectionString();
        }

        // 1. POBIERANIE (Dostosowane do tabeli EmailTemplates)
        public async Task<List<SzablonEmail>> GetActiveTemplatesAsync()
        {
            // ZMIANA: Używamy tabeli EmailTemplates i kolumny CzyAktywny
            const string query = "SELECT Id, Nazwa, TrescRtf FROM EmailTemplates WHERE CzyAktywny = 1 ORDER BY Nazwa";

            try
            {
                // Próba pobrania przez DatabaseService
                var dt = await _dbService.GetDataTableAsync(query);
                var result = new List<SzablonEmail>();

                foreach (DataRow row in dt.Rows)
                {
                    result.Add(new SzablonEmail
                    {
                        Id = Convert.ToInt32(row["Id"]),
                        Nazwa = row["Nazwa"].ToString(),
                        TrescRtf = row["TrescRtf"].ToString(),
                        Aktywny = true
                    });
                }
                return result;
            }
            catch (Exception ex)
            {
                // Fallback na bezpośrednie połączenie w razie problemów z dbService
                Console.WriteLine($"Błąd dbService: {ex.Message}. Próba bezpośrednia.");
                return await GetActiveTemplatesDirectAsync();
            }
        }

        private async Task<List<SzablonEmail>> GetActiveTemplatesDirectAsync()
        {
            var list = new List<SzablonEmail>();
            using (var con = new MySqlConnection(_connectionString))
            {
                await con.OpenAsync();
                // ZMIANA: Tabela EmailTemplates
                string sql = "SELECT Id, Nazwa, TrescRtf FROM EmailTemplates WHERE CzyAktywny = 1 ORDER BY Nazwa";
                try
                {
                    using (var cmd = new MySqlCommand(sql, con))
                    using (var reader = await cmd.ExecuteReaderAsync())
                    {
                        while (await reader.ReadAsync())
                        {
                            list.Add(new SzablonEmail
                            {
                                Id = Convert.ToInt32(reader["Id"]),
                                Nazwa = reader["Nazwa"].ToString(),
                                TrescRtf = reader["TrescRtf"].ToString(),
                                Aktywny = true
                            });
                        }
                    }
                }
                catch { /* Ignoruj błędy */ }
            }
            return list;
        }

        // 2. ZAPISYWANIE (Dostosowane do EmailTemplates)
        public async Task SaveTemplateAsync(SzablonEmail template)
        {
            string query = template.Id > 0
                ? "UPDATE EmailTemplates SET Nazwa = @nazwa, TrescRtf = @tresc WHERE Id = @id"
                : "INSERT INTO EmailTemplates (Nazwa, TrescRtf, CzyAktywny) VALUES (@nazwa, @tresc, 1)";

            var parameters = new List<MySqlParameter>
            {
                new MySqlParameter("@nazwa", template.Nazwa),
                new MySqlParameter("@tresc", template.TrescRtf)
            };
            if (template.Id > 0)
                parameters.Add(new MySqlParameter("@id", template.Id));

            await _dbService.ExecuteNonQueryAsync(query, parameters.ToArray());
        }

        // 3. USUWANIE (Dostosowane do EmailTemplates)
        public async Task DeleteTemplateAsync(int id)
        {
            // ZMIANA: Ustawiamy CzyAktywny = 0
            const string query = "UPDATE EmailTemplates SET CzyAktywny = 0 WHERE Id = @id";
            await _dbService.ExecuteNonQueryAsync(query, new MySqlParameter("@id", id));
        }

        // Aliasy dla zgodności nazw metod
        public Task ZapiszSzablonAsync(SzablonEmail s) => SaveTemplateAsync(s);
        public Task UsunSzablonAsync(int id) => DeleteTemplateAsync(id);
    }
}