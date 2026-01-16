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
            await EnsureStatusMappingTableAsync();
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
            await EnsureStatusMappingTableAsync();
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

        public async Task<int?> GetTemplateIdForStatusAsync(string statusName)
        {
            if (string.IsNullOrWhiteSpace(statusName)) return null;

            await EnsureStatusMappingTableAsync();
            var result = await _dbService.ExecuteScalarAsync(
                "SELECT TemplateId FROM EmailTemplateStatusMap WHERE StatusName = @s LIMIT 1",
                new MySqlParameter("@s", statusName));

            if (result == null || result == DBNull.Value) return null;
            return Convert.ToInt32(result);
        }

        public async Task UpsertTemplateStatusAsync(int templateId, string statusName)
        {
            if (templateId <= 0 || string.IsNullOrWhiteSpace(statusName)) return;

            await EnsureStatusMappingTableAsync();
            await _dbService.ExecuteNonQueryAsync(
                @"INSERT INTO EmailTemplateStatusMap (StatusName, TemplateId)
                  VALUES (@status, @template)
                  ON DUPLICATE KEY UPDATE TemplateId = VALUES(TemplateId)",
                new MySqlParameter("@status", statusName),
                new MySqlParameter("@template", templateId));
        }

        public async Task ClearTemplateStatusAsync(int templateId)
        {
            if (templateId <= 0) return;
            await EnsureStatusMappingTableAsync();
            await _dbService.ExecuteNonQueryAsync(
                "DELETE FROM EmailTemplateStatusMap WHERE TemplateId = @id",
                new MySqlParameter("@id", templateId));
        }

        public async Task<string> GetStatusForTemplateAsync(int templateId)
        {
            if (templateId <= 0) return null;

            await EnsureStatusMappingTableAsync();
            var result = await _dbService.ExecuteScalarAsync(
                "SELECT StatusName FROM EmailTemplateStatusMap WHERE TemplateId = @id LIMIT 1",
                new MySqlParameter("@id", templateId));
            return result?.ToString();
        }

        private async Task EnsureStatusMappingTableAsync()
        {
            const string sql = @"
                CREATE TABLE IF NOT EXISTS EmailTemplateStatusMap (
                    StatusName VARCHAR(255) PRIMARY KEY,
                    TemplateId INT NOT NULL
                )";
            await _dbService.ExecuteNonQueryAsync(sql);
        }

        // Aliasy dla zgodności nazw metod
        public Task ZapiszSzablonAsync(SzablonEmail s) => SaveTemplateAsync(s);
        public Task UsunSzablonAsync(int id) => DeleteTemplateAsync(id);
    }
}
