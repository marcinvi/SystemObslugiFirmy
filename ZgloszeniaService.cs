using System;
using System.Collections.Generic;
using System.Data;
using MySql.Data.MySqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Reklamacje_Dane
{
    public class ZgloszeniaService
    {
        private readonly DatabaseService _dbService;

        public ZgloszeniaService(DatabaseService dbService)
        {
            _dbService = dbService ?? throw new ArgumentNullException(nameof(dbService));
        }

        public async Task<DataTable> GetComplaintsAsync(
            string globalSearch,
            Dictionary<string, string> columnFilters,
            bool filterDataZgloszenia,
            DateTime dataZgloszeniaOd,
            DateTime dataZgloszeniaDo,
            bool filterDataZakupu,
            DateTime dataZakupuOd,
            DateTime dataZakupuDo,
            List<string> statusOgolny,
            List<string> statusKlient,
            List<string> statusProducent)
        {
            try
            {
                var queryBuilder = new StringBuilder(@"
                SELECT
                    z.NrZgloszenia,
                    CASE
                        WHEN k.ImieNazwisko IS NOT NULL AND k.NazwaFirmy IS NOT NULL THEN CONCAT(k.ImieNazwisko, ' | ', k.NazwaFirmy)
                        WHEN k.ImieNazwisko IS NOT NULL THEN k.ImieNazwisko
                        WHEN k.NazwaFirmy IS NOT NULL THEN k.NazwaFirmy
                        ELSE 'Brak klienta'
                    END AS Klient,
                    p.NazwaKrotka AS Produkt,
                    z.DataZgloszenia, z.DataZakupu, z.NrFaktury, z.NrSeryjny, z.OpisUsterki, z.GwarancjaPlatna,
                    z.StatusOgolny, z.StatusKlient, z.StatusProducent, z.CzekamyNaDostawe, z.NrWRL, z.NrKWZ2,
                    z.NrRMA, z.NrKPZN, z.CzyNotaRozliczona, z.Skad, z.allegroDisputeId, z.allegroOrderId,
                    z.allegroBuyerLogin, z.AllegroAccountId
                FROM Zgloszenia z
                LEFT JOIN Klienci k ON z.KlientID = k.Id
                LEFT JOIN Produkty p ON z.ProduktID = p.Id
                ");

                var conditions = new List<string>();
                var parameters = new List<MySqlParameter>();

                if (!string.IsNullOrWhiteSpace(globalSearch))
                {
                    var normalizedPhoneSearch = new string(globalSearch.Where(char.IsDigit).ToArray());
                    var searchableColumns = new[] { "z.NrZgloszenia", "k.ImieNazwisko", "k.NazwaFirmy", "p.NazwaKrotka", "z.NrFaktury", "z.NrSeryjny", "z.OpisUsterki" };
                    var globalSearchConditions = new List<string>();
                    globalSearchConditions.AddRange(searchableColumns.Select(c => $"{c} LIKE @GlobalSearch"));
                    parameters.Add(new MySqlParameter("@GlobalSearch", $"%{globalSearch}%"));
                    if (!string.IsNullOrEmpty(normalizedPhoneSearch))
                    {
                        globalSearchConditions.Add("REPLACE(REPLACE(REPLACE(k.Telefon, ' ', ''), '+48', ''), '-', '') LIKE @PhoneSearch");
                        parameters.Add(new MySqlParameter("@PhoneSearch", $"%{normalizedPhoneSearch}%"));
                    }
                    conditions.Add($"({string.Join(" OR ", globalSearchConditions)})");
                }

                if (columnFilters != null)
                {
                    foreach (var kvp in columnFilters)
                    {
                        var paramName = $"@Filter_{Guid.NewGuid():N}";
                        conditions.Add($"{kvp.Key} LIKE {paramName}");
                        parameters.Add(new MySqlParameter(paramName, $"%{kvp.Value}%"));
                    }
                }

                if (filterDataZgloszenia)
                {
                    conditions.Add("z.DataZgloszenia BETWEEN @DataZgloszeniaOd AND @DataZgloszeniaDo");
                    parameters.Add(new MySqlParameter("@DataZgloszeniaOd", dataZgloszeniaOd.Date));
                    parameters.Add(new MySqlParameter("@DataZgloszeniaDo", dataZgloszeniaDo.Date.AddDays(1).AddSeconds(-1)));
                }

                if (filterDataZakupu)
                {
                    conditions.Add("z.DataZakupu BETWEEN @DataZakupuOd AND @DataZakupuDo");
                    parameters.Add(new MySqlParameter("@DataZakupuOd", dataZakupuOd.Date));
                    parameters.Add(new MySqlParameter("@DataZakupuDo", dataZakupuDo.Date.AddDays(1).AddSeconds(-1)));
                }

                void AddStatus(List<string> values, string column)
                {
                    if (values != null && values.Any())
                    {
                        var baseParamName = column.Replace('.', '_');
                        var paramNames = values.Select((s, i) => $"@{baseParamName}{i}").ToList();
                        conditions.Add($"{column} IN ({string.Join(", ", paramNames)})");
                        for (int i = 0; i < values.Count; i++)
                            parameters.Add(new MySqlParameter(paramNames[i], values[i]));
                    }
                }

                AddStatus(statusOgolny, "z.StatusOgolny");
                AddStatus(statusKlient, "z.StatusKlient");
                AddStatus(statusProducent, "z.StatusProducent");

                if (conditions.Any())
                {
                    queryBuilder.Append(" WHERE ").Append(string.Join(" AND ", conditions));
                }

                return await _dbService.GetDataTableAsync(queryBuilder.ToString(), parameters.ToArray());
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Błąd pobierania zgłoszeń: {ex.Message}");
                return new DataTable();
            }
        }

        public async Task<List<string>> GetDistinctValuesAsync(string columnName)
        {
            try
            {
                var query = $"SELECT DISTINCT {columnName} FROM Zgloszenia WHERE {columnName} IS NOT NULL AND {columnName} != ''";
                var dt = await _dbService.GetDataTableAsync(query);
                return dt.AsEnumerable().Select(r => r[0].ToString()).OrderBy(v => v).ToList();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Błąd pobierania wartości dla {columnName}: {ex.Message}");
                return new List<string>();
            }
        }
    }
}
