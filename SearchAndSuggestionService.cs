using System;
using System.Collections.Generic;
using System.Data;
using MySql.Data.MySqlClient;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

// Upewnij się, że ta przestrzeń nazw jest poprawna i że masz using do przestrzeni nazw, 
// w której zdefiniowane są Twoje modele (np. using Reklamacje_Dane.Models;)
namespace Reklamacje_Dane
{
    public static class SearchAndSuggestionService
    {
        private static readonly DatabaseService _dbService = new DatabaseService(DatabaseHelper.GetConnectionString());
        private static readonly HashSet<string> _productStopWords = new HashSet<string> { "z", "i", "do", "światłem", "oraz", "dla" };

        /// <summary>
        /// ULEPSZONE Wyszukiwanie klientów z ważoną punktacją.
        /// </summary>
        public static async Task<List<ClientViewModel>> SearchClientsAsync(string queryText, ComplaintInitialData initialData)
        {
            var keywords = queryText.ToLower().Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries).Where(k => k.Length > 1).ToList();
            if (!keywords.Any() && initialData == null) return new List<ClientViewModel>();

            string query = "SELECT Id, ImieNazwisko, NazwaFirmy, NIP, Ulica, KodPocztowy, Miejscowosc, Email, Telefon FROM Klienci";
            var dt = await _dbService.GetDataTableAsync(query);
            var results = new List<ClientViewModel>();

            var weights = new Dictionary<string, double>
            {
                { "ImieNazwisko", 10.0 },
                { "NazwaFirmy", 10.0 },
                { "NIP", 8.0 },
                { "Email", 5.0 },
                { "Miejscowosc", 3.0 },
                { "Ulica", 2.0 },
                { "Telefon", 5.0 }
            };

            foreach (DataRow row in dt.Rows)
            {
                var client = new ClientViewModel
                {
                    Id = Convert.ToInt32(row["Id"]),
                    ImieNazwisko = row["ImieNazwisko"]?.ToString() ?? "",
                    NazwaFirmy = row["NazwaFirmy"]?.ToString() ?? "",
                    NIP = row["NIP"]?.ToString() ?? "",
                    Ulica = row["Ulica"]?.ToString() ?? "",
                    KodPocztowy = row["KodPocztowy"]?.ToString() ?? "",
                    Miejscowosc = row["Miejscowosc"]?.ToString() ?? "",
                    Email = row["Email"]?.ToString() ?? "",
                    Telefon = row["Telefon"]?.ToString() ?? ""
                };

                double score = 0;
                var matchedKeywords = new HashSet<string>();

                if (initialData != null)
                {
                    if (!string.IsNullOrWhiteSpace(client.NIP) && !string.IsNullOrWhiteSpace(initialData.NIP) && client.NIP.Equals(initialData.NIP, StringComparison.OrdinalIgnoreCase)) score += 200;
                    if (!string.IsNullOrWhiteSpace(client.Email) && !string.IsNullOrWhiteSpace(initialData.Email) && client.Email.Equals(initialData.Email, StringComparison.OrdinalIgnoreCase)) score += 200;
                    if (!string.IsNullOrWhiteSpace(client.Telefon) && !string.IsNullOrWhiteSpace(initialData.Telefon) && client.Telefon.Replace(" ", "").Equals(initialData.Telefon.Replace(" ", ""), StringComparison.OrdinalIgnoreCase)) score += 200;
                }

                var clientData = new Dictionary<string, string>
                {
                    { "ImieNazwisko", client.ImieNazwisko.ToLower() },
                    { "NazwaFirmy", client.NazwaFirmy.ToLower() },
                    { "NIP", client.NIP.ToLower() },
                    { "Email", client.Email.ToLower() },
                    { "Miejscowosc", client.Miejscowosc.ToLower() },
                    { "Ulica", client.Ulica.ToLower() },
                    { "Telefon", client.Telefon.ToLower() }
                };

                foreach (var keyword in keywords)
                {
                    foreach (var field in clientData)
                    {
                        if (string.IsNullOrWhiteSpace(field.Value)) continue;
                        if (field.Value.Contains(keyword))
                        {
                            score += weights[field.Key];
                            matchedKeywords.Add(keyword);
                        }
                    }
                }

                if (matchedKeywords.Count > 1)
                {
                    score *= (1 + 0.2 * (matchedKeywords.Count - 1));
                }

                if (score > 0)
                {
                    client.RelevanceScore = score;
                    results.Add(client);
                }
            }

            return results.OrderByDescending(r => r.RelevanceScore).Take(50).ToList();
        }

        /// <summary>
        /// ULEPSZONE Wyszukiwanie produktów z ważoną punktacją i bonusem za liczbę zgłoszeń.
        /// </summary>
        public static async Task<List<ProductViewModel>> SearchProductsAsync(string queryText)
        {
            if (string.IsNullOrWhiteSpace(queryText) || queryText.Length < 2)
            {
                return new List<ProductViewModel>();
            }

            var complaintCounts = await GetProductComplaintCountsAsync();
            var keywords = queryText.ToLower().Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries)
                                    .Where(k => !_productStopWords.Contains(k)).ToList();
            if (!keywords.Any()) return new List<ProductViewModel>();

            string query = "SELECT Id, NazwaKrotka AS Nazwa, Producent, KodEnova, KodProducenta, Kategoria FROM Produkty";
            var allProductsDt = await _dbService.GetDataTableAsync(query);
            var results = new List<(ProductViewModel product, double score)>();

            var weights = new Dictionary<string, double>
            {
                { "Nazwa", 10.0 },
                { "Producent", 5.0 },
                { "Kategoria", 3.0 },
                { "KodEnova", 2.0 },
                { "KodProducenta", 2.0 }
            };
            const double complaintBonusWeight = 50.0;

            foreach (DataRow row in allProductsDt.Rows)
            {
                var product = new ProductViewModel
                {
                    Id = Convert.ToInt32(row["Id"]),
                    Nazwa = row["Nazwa"]?.ToString() ?? "",
                    Producent = row["Producent"]?.ToString() ?? "",
                    KodEnova = row["KodEnova"]?.ToString() ?? "",
                    KodProducenta = row["KodProducenta"]?.ToString() ?? "",
                    Kategoria = row["Kategoria"]?.ToString() ?? ""
                };

                double currentScore = 0;
                var matchedKeywords = new HashSet<string>();
                var productData = new Dictionary<string, string>
                {
                    { "Nazwa", product.Nazwa.ToLower() },
                    { "Producent", product.Producent.ToLower() },
                    { "Kategoria", product.Kategoria.ToLower() },
                    { "KodEnova", product.KodEnova.ToLower() },
                    { "KodProducenta", product.KodProducenta.ToLower() }
                };

                foreach (var keyword in keywords)
                {
                    foreach (var field in productData)
                    {
                        if (string.IsNullOrWhiteSpace(field.Value)) continue;
                        double fieldWeight = weights[field.Key];
                        var fieldWords = field.Value.Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);

                        if (fieldWords.Contains(keyword)) { currentScore += 20 * fieldWeight; matchedKeywords.Add(keyword); }
                        else if (fieldWords.Any(w => w.StartsWith(keyword))) { currentScore += 10 * fieldWeight; matchedKeywords.Add(keyword); }
                        else if (field.Value.Contains(keyword)) { currentScore += 1 * fieldWeight; matchedKeywords.Add(keyword); }
                    }
                }

                if (matchedKeywords.Count > 1) { currentScore *= (1 + 0.5 * (matchedKeywords.Count - 1)); }

                if (complaintCounts.TryGetValue(product.Id, out int count))
                {
                    currentScore += Math.Log(count + 1) * complaintBonusWeight;
                }

                if (currentScore > 0)
                {
                    results.Add((product, currentScore));
                }
            }

            return results.OrderByDescending(r => r.score).Select(r => r.product).Take(100).ToList();
        }

        public static async Task<(bool Exists, string ComplaintNumber)> CheckSerialNumberExistsAsync(string serialNumber)
        {
            if (string.IsNullOrWhiteSpace(serialNumber)) return (false, null);
            string query = "SELECT NrZgloszenia FROM Zgloszenia WHERE NrSeryjny = @nrSeryjny LIMIT 1";
            var result = await _dbService.ExecuteScalarAsync(query, new MySqlParameter("@nrSeryjny", serialNumber));
            return (result != null, result?.ToString());
        }

        public static async Task<(bool Exists, string ComplaintNumber)> CheckInvoiceNumberExistsAsync(string invoiceNumber)
        {
            if (string.IsNullOrWhiteSpace(invoiceNumber)) return (false, null);
            string query = "SELECT NrZgloszenia FROM Zgloszenia WHERE NrFaktury = @nrFaktury LIMIT 1";
            var result = await _dbService.ExecuteScalarAsync(query, new MySqlParameter("@nrFaktury", invoiceNumber));
            return (result != null, result?.ToString());
        }

        public static bool CheckInvoiceDateConsistency(string invoiceNumber, DateTime purchaseDate)
        {
            if (string.IsNullOrWhiteSpace(invoiceNumber)) return true;
            var regex = new Regex(@"\/(\d{1,2})\/");
            var match = regex.Match(invoiceNumber);
            if (match.Success && int.TryParse(match.Groups[1].Value, out int monthFromInvoice))
            {
                return monthFromInvoice == purchaseDate.Month;
            }
            return true;
        }

        public static async Task<int> GetClientComplaintCountAsync(int clientId)
        {
            string query = "SELECT COUNT(Id) FROM Zgloszenia WHERE KlientID = @clientId";
            var result = await _dbService.ExecuteScalarAsync(query, new MySqlParameter("@clientId", clientId));
            return Convert.ToInt32(result);
        }

        public static async Task<ExistingComplaintInfo> CheckForExistingComplaintAsync(int clientId, int productId)
        {
            string query = "SELECT NrZgloszenia, NrFaktury, NrSeryjny, DataZakupu FROM Zgloszenia WHERE KlientID = @clientId AND ProduktID = @productId LIMIT 1";
            var parameters = new[] { new MySqlParameter("@clientId", clientId), new MySqlParameter("@productId", productId) };
            var dt = await _dbService.GetDataTableAsync(query, parameters);

            if (dt.Rows.Count > 0)
            {
                var row = dt.Rows[0];
                DateTime.TryParse(row["DataZakupu"]?.ToString(), out DateTime dataZakupu);
                return new ExistingComplaintInfo
                {
                    NrZgloszenia = row["NrZgloszenia"]?.ToString(),
                    NrFaktury = row["NrFaktury"]?.ToString(),
                    NrSeryjny = row["NrSeryjny"]?.ToString(),
                    DataZakupu = dataZakupu == DateTime.MinValue ? (DateTime?)null : dataZakupu
                };
            }
            return null;
        }

        public static async Task<Dictionary<int, int>> GetProductComplaintCountsAsync()
        {
            var complaintCounts = new Dictionary<int, int>();
            string query = "SELECT ProduktID, COUNT(Id) as ComplaintCount FROM Zgloszenia WHERE ProduktID IS NOT NULL GROUP BY ProduktID";
            var dt = await _dbService.GetDataTableAsync(query);

            foreach (DataRow row in dt.Rows)
            {
                if (row["ProduktID"] != DBNull.Value)
                {
                    int productId = Convert.ToInt32(row["ProduktID"]);
                    int count = Convert.ToInt32(row["ComplaintCount"]);
                    complaintCounts[productId] = count;
                }
            }
            return complaintCounts;
        }
    }
}