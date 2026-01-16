using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows.Forms;
using MySql.Data.MySqlClient;

namespace Reklamacje_Dane
{
    /// <summary>
    /// 🚀 ZOPTYMALIZOWANY Smart Search Service - SZYBKI!
    /// </summary>
    public static class SmartSearchService
    {
        private static readonly DatabaseService _dbService = new DatabaseService(DatabaseHelper.GetConnectionString());

        // Cache dla BŁYSKAWICZNEGO dostępu
        private static List<EnhancedProductViewModel> _productsCache;
        private static DateTime _productsCacheTime = DateTime.MinValue;
        private static readonly TimeSpan CacheValidTime = TimeSpan.FromMinutes(10);

        #region ===== PRODUCT SEARCH (ULTRA SZYBKIE) =====

        /// <summary>
        /// 🚀 PRELOAD CACHE - Załaduj wszystkie produkty do RAM na starcie
        /// </summary>
        public static async Task PreloadProductCacheAsync()
        {
            await RefreshProductCacheAsync();
        }

        /// <summary>
        /// 🔍 SMART PRODUCT SEARCH - ZOPTYMALIZOWANE z early exit
        /// </summary>
        public static async Task<List<EnhancedProductViewModel>> SmartSearchProductsAsync(string query)
        {
            // ✅ DEBUG: Start timer
            var stopwatch = System.Diagnostics.Stopwatch.StartNew();

            if (string.IsNullOrWhiteSpace(query) || query.Length < 2)
                return new List<EnhancedProductViewModel>();

            // Pobierz/odśwież cache
            await RefreshProductCacheIfNeededAsync();

            // ✅ DEBUG: Check cache
            var cacheCheckTime = stopwatch.ElapsedMilliseconds;

            query = query.ToLower().Trim();
            var keywords = query.Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries)
                                .Where(k => k.Length > 1 && !IsStopWord(k))
                                .ToList();

            if (!keywords.Any())
                return new List<EnhancedProductViewModel>();

            var results = new List<(EnhancedProductViewModel product, double score, int confidencePercent)>();

            // ✅ DEBUG: Counters
            int totalProducts = _productsCache.Count;
            int skippedByEarlyExit = 0;
            int processedWithFuzzy = 0;

            foreach (var product in _productsCache)
            {
                // ⚡ OPTYMALIZACJA 1: Quick pre-filter (100x szybsze niż fuzzy matching!)
                string allText = $"{product.NazwaSystemowa} {product.NazwaKrotka} {product.KodProducenta} {product.Producent} {product.KodEnova} {product.Kategoria}".ToLower();

                bool anyKeywordFound = false;
                foreach (var keyword in keywords)
                {
                    if (allText.Contains(keyword))
                    {
                        anyKeywordFound = true;
                        break;
                    }
                }

                // Skip products that don't contain ANY keyword
                if (!anyKeywordFound)
                {
                    skippedByEarlyExit++;
                    continue;
                }

                // ✅ DEBUG: This product passed early exit
                processedWithFuzzy++;

                // ⚡ OPTYMALIZACJA 2: Teraz dopiero fuzzy matching (tylko na ~1% produktów)
                double totalScore = 0;
                int matchCount = 0;
                var matchDetails = new List<string>();

                // Wyszukuj w KAŻDYM polu
                foreach (var keyword in keywords)
                {
                    // 1. NazwaSystemowa (najważniejsze)
                    var (matchedNazwa, scoreNazwa) = FuzzyMatch(keyword, product.NazwaSystemowa);
                    if (matchedNazwa)
                    {
                        totalScore += scoreNazwa * 20;
                        matchCount++;
                        matchDetails.Add($"Nazwa: {Math.Round(scoreNazwa * 100)}%");
                    }

                    // 2. NazwaKrotka
                    var (matchedKrotka, scoreKrotka) = FuzzyMatch(keyword, product.NazwaKrotka);
                    if (matchedKrotka)
                    {
                        totalScore += scoreKrotka * 15;
                        matchCount++;
                    }

                    // 3. KodProducenta
                    var (matchedKodProd, scoreKodProd) = FuzzyMatch(keyword, product.KodProducenta);
                    if (matchedKodProd)
                    {
                        totalScore += scoreKodProd * 12;
                        matchCount++;
                    }

                    // 4. Producent
                    var (matchedProducent, scoreProducent) = FuzzyMatch(keyword, product.Producent);
                    if (matchedProducent)
                    {
                        totalScore += scoreProducent * 10;
                        matchCount++;
                    }

                    // 5. KodEnova
                    var (matchedKodEnova, scoreKodEnova) = FuzzyMatch(keyword, product.KodEnova);
                    if (matchedKodEnova)
                    {
                        totalScore += scoreKodEnova * 8;
                        matchCount++;
                    }

                    // 6. Kategoria
                    var (matchedKategoria, scoreKategoria) = FuzzyMatch(keyword, product.Kategoria);
                    if (matchedKategoria)
                    {
                        totalScore += scoreKategoria * 5;
                        matchCount++;
                    }
                }

                if (matchCount > 0)
                {
                    // Bonus za więcej dopasowanych słów
                    double keywordBonus = 1.0 + (matchCount * 0.2);
                    totalScore *= keywordBonus;

                    // Bonus za liczbę zgłoszeń
                    if (product.ComplaintCount > 0)
                    {
                        totalScore += Math.Log(product.ComplaintCount + 1) * 10;
                    }

                    // Oblicz confidence (0-100%)
                    int confidence = CalculateConfidence(totalScore, keywords.Count, matchCount);

                    product.MatchDetails = string.Join(", ", matchDetails);
                    product.ConfidencePercent = confidence;

                    results.Add((product, totalScore, confidence));
                }
            }

            // Sortuj i zwróć top 50
            var finalResults = results.OrderByDescending(r => r.score)
                         .Take(50)
                         .Select(r => r.product)
                         .ToList();

            // ✅ DEBUG: Show results
           

            return finalResults;
        }

        #endregion

        #region ===== CLIENT SEARCH =====

        public static async Task<List<EnhancedClientViewModel>> SmartSearchClientsAsync(string query, ComplaintInitialData initialData = null)
        {
            // ✅ NAPRAWA: Jeśli mamy initialData, szukaj nawet jeśli query pusty!
            bool hasQuery = !string.IsNullOrWhiteSpace(query) && query.Length >= 2;
            bool hasInitialData = initialData != null &&
                                 (!string.IsNullOrWhiteSpace(initialData.Email) ||
                                  !string.IsNullOrWhiteSpace(initialData.Telefon) ||
                                  !string.IsNullOrWhiteSpace(initialData.NIP));

            // Jeśli NIE MA ani query ani initialData - return
            if (!hasQuery && !hasInitialData)
                return new List<EnhancedClientViewModel>();

            var keywords = hasQuery
                ? query.ToLower().Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries)
                       .Where(k => k.Length > 1)
                       .ToList()
                : new List<string>();

            string sql = @"SELECT 
                            k.Id, k.ImieNazwisko, k.NazwaFirmy, k.NIP, k.Ulica, 
                            k.KodPocztowy, k.Miejscowosc, k.Email, k.Telefon,
                            COUNT(z.Id) as ComplaintCount
                           FROM Klienci k
                           LEFT JOIN Zgloszenia z ON z.KlientID = k.Id
                           GROUP BY k.Id";

            var dt = await _dbService.GetDataTableAsync(sql);
            var results = new List<(EnhancedClientViewModel client, double score, int confidence)>();

            foreach (DataRow row in dt.Rows)
            {
                var client = new EnhancedClientViewModel
                {
                    Id = Convert.ToInt32(row["Id"]),
                    ImieNazwisko = row["ImieNazwisko"]?.ToString() ?? "",
                    NazwaFirmy = row["NazwaFirmy"]?.ToString() ?? "",
                    NIP = row["NIP"]?.ToString() ?? "",
                    Ulica = row["Ulica"]?.ToString() ?? "",
                    KodPocztowy = row["KodPocztowy"]?.ToString() ?? "",
                    Miejscowosc = row["Miejscowosc"]?.ToString() ?? "",
                    Email = row["Email"]?.ToString() ?? "",
                    Telefon = row["Telefon"]?.ToString() ?? "",
                    ComplaintCount = Convert.ToInt32(row["ComplaintCount"])
                };

                double totalScore = 0;
                int matchCount = 0;
                var matchDetails = new List<string>();

                // PERFECT MATCHES
                if (initialData != null)
                {
                    if (!string.IsNullOrWhiteSpace(client.NIP) &&
                        !string.IsNullOrWhiteSpace(initialData.NIP) &&
                        client.NIP.Equals(initialData.NIP, StringComparison.OrdinalIgnoreCase))
                    {
                        totalScore += 500;
                        matchDetails.Add("NIP: 100%");
                        client.HasConflict = false;
                    }

                    if (!string.IsNullOrWhiteSpace(client.Email) &&
                        !string.IsNullOrWhiteSpace(initialData.Email) &&
                        client.Email.Equals(initialData.Email, StringComparison.OrdinalIgnoreCase))
                    {
                        totalScore += 300;
                        matchDetails.Add("Email: 100%");

                        if (!string.IsNullOrWhiteSpace(client.Telefon) &&
                            !string.IsNullOrWhiteSpace(initialData.Telefon) &&
                            !NormalizeTelephone(client.Telefon).Equals(NormalizeTelephone(initialData.Telefon)))
                        {
                            client.HasConflict = true;
                            client.ConflictReason = "Email się zgadza, ale telefon różny!";
                        }
                    }

                    if (!string.IsNullOrWhiteSpace(client.Telefon) &&
                        !string.IsNullOrWhiteSpace(initialData.Telefon) &&
                        NormalizeTelephone(client.Telefon).Equals(NormalizeTelephone(initialData.Telefon)))
                    {
                        totalScore += 300;
                        matchDetails.Add("Telefon: 100%");

                        if (!string.IsNullOrWhiteSpace(client.Email) &&
                            !string.IsNullOrWhiteSpace(initialData.Email) &&
                            !client.Email.Equals(initialData.Email, StringComparison.OrdinalIgnoreCase))
                        {
                            client.HasConflict = true;
                            client.ConflictReason = "Telefon się zgadza, ale email różny!";
                        }
                    }
                }

                // FUZZY MATCHES
                foreach (var keyword in keywords)
                {
                    var (matchedImie, scoreImie) = FuzzyMatch(keyword, client.ImieNazwisko);
                    if (matchedImie)
                    {
                        totalScore += scoreImie * 15;
                        matchCount++;
                        matchDetails.Add($"Imię: {Math.Round(scoreImie * 100)}%");
                    }

                    var (matchedFirma, scoreFirma) = FuzzyMatch(keyword, client.NazwaFirmy);
                    if (matchedFirma)
                    {
                        totalScore += scoreFirma * 15;
                        matchCount++;
                        matchDetails.Add($"Firma: {Math.Round(scoreFirma * 100)}%");
                    }

                    var (matchedMiasto, scoreMiasto) = FuzzyMatch(keyword, client.Miejscowosc);
                    if (matchedMiasto)
                    {
                        totalScore += scoreMiasto * 5;
                        matchCount++;
                    }
                }

                if (totalScore > 0)
                {
                    int confidence = CalculateConfidence(totalScore, keywords.Count, matchCount);
                    client.MatchDetails = string.Join(", ", matchDetails);
                    client.ConfidencePercent = confidence;

                    results.Add((client, totalScore, confidence));
                }
            }

            // ✅ DEBUG: Show client search results
            var finalResults = results.OrderByDescending(r => r.score)
                         .Take(50)
                         .Select(r => r.client)
                         .ToList();

          

            return finalResults;
        }

        #endregion

        #region ===== VALIDATION & CHECKS =====

        public static async Task<(bool Exists, string ComplaintNumber)> CheckSerialNumberAsync(string serialNumber)
        {
            if (string.IsNullOrWhiteSpace(serialNumber)) return (false, null);

            string query = "SELECT NrZgloszenia FROM Zgloszenia WHERE NrSeryjny = @sn LIMIT 1";
            var result = await _dbService.ExecuteScalarAsync(query, new MySqlParameter("@sn", serialNumber));
            return (result != null, result?.ToString());
        }

        public static async Task<(bool Exists, string ComplaintNumber)> CheckInvoiceNumberAsync(string invoiceNumber)
        {
            if (string.IsNullOrWhiteSpace(invoiceNumber)) return (false, null);

            string query = "SELECT NrZgloszenia FROM Zgloszenia WHERE NrFaktury = @fv LIMIT 1";
            var result = await _dbService.ExecuteScalarAsync(query, new MySqlParameter("@fv", invoiceNumber));
            return (result != null, result?.ToString());
        }

        public static bool ValidateInvoiceDateConsistency(string invoiceNumber, DateTime purchaseDate)
        {
            if (string.IsNullOrWhiteSpace(invoiceNumber)) return true;

            var parts = invoiceNumber.Split(new[] { '/' }, StringSplitOptions.RemoveEmptyEntries);
            int monthFromInvoice = -1;

            if (parts.Length >= 3)
            {
                var first = parts[0].Trim();
                var last = parts[parts.Length - 1].Trim();
                bool hasPrefix = first.StartsWith("FV", StringComparison.OrdinalIgnoreCase);
                bool hasSuffix = last.StartsWith("FV", StringComparison.OrdinalIgnoreCase);

                if (hasPrefix && parts.Length > 2)
                {
                    int.TryParse(parts[2], out monthFromInvoice);
                }
                else if (hasSuffix && parts.Length > 1)
                {
                    int.TryParse(parts[1], out monthFromInvoice);
                }
            }

            if (monthFromInvoice <= 0)
            {
                var regex = new Regex(@"\/(\d{1,2})\/");
                var match = regex.Match(invoiceNumber);
                if (match.Success)
                {
                    int.TryParse(match.Groups[1].Value, out monthFromInvoice);
                }
            }

            if (monthFromInvoice <= 0)
            {
                return true;
            }

            return monthFromInvoice == purchaseDate.Month;
        }

        #endregion

        #region ===== HELPER METHODS =====

        private static (bool matched, double score) FuzzyMatch(string keyword, string text)
        {
            if (string.IsNullOrWhiteSpace(text))
                return (false, 0);

            text = text.ToLower();
            keyword = keyword.ToLower();

            // 1. Exact match
            if (text.Contains(keyword))
            {
                var words = text.Split(new[] { ' ', '-', '_' }, StringSplitOptions.RemoveEmptyEntries);
                if (words.Any(w => w == keyword))
                    return (true, 1.0);

                return (true, 0.9);
            }

            // 2. Starts with
            if (text.StartsWith(keyword))
                return (true, 0.85);

            // 3. Fuzzy match (Levenshtein)
            var words2 = text.Split(new[] { ' ', '-', '_' }, StringSplitOptions.RemoveEmptyEntries);
            foreach (var word in words2)
            {
                int distance = LevenshteinDistance(keyword, word);
                int maxLen = Math.Max(keyword.Length, word.Length);

                double similarity = 1.0 - ((double)distance / maxLen);

                if (similarity >= 0.7)
                    return (true, similarity);
            }

            return (false, 0);
        }

        private static int LevenshteinDistance(string s1, string s2)
        {
            int[,] d = new int[s1.Length + 1, s2.Length + 1];

            for (int i = 0; i <= s1.Length; i++)
                d[i, 0] = i;

            for (int j = 0; j <= s2.Length; j++)
                d[0, j] = j;

            for (int j = 1; j <= s2.Length; j++)
            {
                for (int i = 1; i <= s1.Length; i++)
                {
                    int cost = (s1[i - 1] == s2[j - 1]) ? 0 : 1;
                    d[i, j] = Math.Min(Math.Min(d[i - 1, j] + 1, d[i, j - 1] + 1), d[i - 1, j - 1] + cost);
                }
            }

            return d[s1.Length, s2.Length];
        }

        private static int CalculateConfidence(double totalScore, int keywordCount, int matchCount)
        {
            double baseConfidence = Math.Min(totalScore / (keywordCount * 20.0), 1.0);
            double completenessBonus = (double)matchCount / keywordCount;
            double finalConfidence = (baseConfidence * 0.7) + (completenessBonus * 0.3);

            return (int)Math.Round(finalConfidence * 100);
        }

        private static string NormalizeTelephone(string phone)
        {
            if (string.IsNullOrWhiteSpace(phone)) return "";

            string cleaned = new string(phone.Where(char.IsDigit).ToArray());

            if (cleaned.StartsWith("48") && cleaned.Length > 9)
                cleaned = cleaned.Substring(2);

            return cleaned;
        }

        private static bool IsStopWord(string word)
        {
            var stopWords = new HashSet<string> { "z", "i", "do", "dla", "oraz", "ze", "na", "w" };
            return stopWords.Contains(word.ToLower());
        }

        private static async Task RefreshProductCacheIfNeededAsync()
        {
            if (_productsCache == null || (DateTime.Now - _productsCacheTime) > CacheValidTime)
            {
                await RefreshProductCacheAsync();
            }
        }

        private static async Task RefreshProductCacheAsync()
        {
            string sql = @"SELECT 
                            p.Id, p.NazwaSystemowa, p.NazwaKrotka, p.KodEnova, 
                            p.KodProducenta, p.Kategoria, p.Producent,
                            COUNT(z.Id) as ComplaintCount
                           FROM Produkty p
                           LEFT JOIN Zgloszenia z ON z.ProduktID = p.Id
                           GROUP BY p.Id";

            var dt = await _dbService.GetDataTableAsync(sql);
            var products = new List<EnhancedProductViewModel>();

            foreach (DataRow row in dt.Rows)
            {
                products.Add(new EnhancedProductViewModel
                {
                    Id = Convert.ToInt32(row["Id"]),
                    NazwaSystemowa = row["NazwaSystemowa"]?.ToString() ?? "",
                    NazwaKrotka = row["NazwaKrotka"]?.ToString() ?? "",
                    KodEnova = row["KodEnova"]?.ToString() ?? "",
                    KodProducenta = row["KodProducenta"]?.ToString() ?? "",
                    Kategoria = row["Kategoria"]?.ToString() ?? "",
                    Producent = row["Producent"]?.ToString() ?? "",
                    ComplaintCount = Convert.ToInt32(row["ComplaintCount"])
                });
            }

            _productsCache = products;
            _productsCacheTime = DateTime.Now;
        }

        public static void InvalidateCache()
        {
            _productsCacheTime = DateTime.MinValue;
        }

        #endregion
    }
}
