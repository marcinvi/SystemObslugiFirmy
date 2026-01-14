using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;

namespace Reklamacje_Dane
{
    internal sealed class ProgressInfo
    {
        public int Percent { get; set; }
        public string Message { get; set; }
    }

    internal sealed class CsvDocument
    {
        public CsvDocument(string documentNumber)
        {
            DocumentNumber = documentNumber;
            Positions = new List<CsvPosition>();
        }

        public string DocumentNumber { get; }
        public DateTime? DocumentDate { get; set; }
        public string Kontrahent { get; set; }
        public string Pelny { get; set; }
        public string KodPocztowy { get; set; }
        public string Miejscowosc { get; set; }
        public List<CsvPosition> Positions { get; }
    }

    internal sealed class CsvPosition
    {
        public string Towar { get; set; }
        public decimal? Ilosc { get; set; }
    }

    internal sealed class AutoMatchOptions
    {
        public bool IncludeClientMatch { get; set; }
        public int ScoreThreshold { get; set; }
        public int MinScoreDelta { get; set; }
    }

    internal sealed class MatchInput
    {
        public int Id { get; set; }
        public string DataZgloszenia { get; set; }
        public string DataZakupu { get; set; }
        public string NrFaktury { get; set; }
        public ProductInfo Product { get; set; }
        public ClientInfo Client { get; set; }
    }

    internal sealed class ProductInfo
    {
        public string NazwaSystemowa { get; set; }
        public string NazwaKrotka { get; set; }
        public string KodEnova { get; set; }
        public string KodProducenta { get; set; }
    }

    internal sealed class ClientInfo
    {
        public string ImieNazwisko { get; set; }
        public string NazwaFirmy { get; set; }
        public string KodPocztowy { get; set; }
        public string Miejscowosc { get; set; }
    }

    internal sealed class AppliedMatch
    {
        public int ZgloszenieId { get; set; }
        public string OldNrFaktury { get; set; }
        public string NewNrFaktury { get; set; }
        public string OldDataZakupu { get; set; }
        public string NewDataZakupu { get; set; }
        public int Score { get; set; }
        public string DocDate { get; set; }
        public string Kontrahent { get; set; }
        public string TowarSample { get; set; }
        public string Dokument { get; set; }
    }

    internal sealed class CandidateInfo
    {
        public string Dokument { get; set; }
        public int Score { get; set; }
        public string DocDate { get; set; }
        public string Kontrahent { get; set; }
        public string TowarSample { get; set; }
    }

    internal sealed class ReviewMatch
    {
        public int ZgloszenieId { get; set; }
        public string OldNrFaktury { get; set; }
        public string OldDataZakupu { get; set; }
        public string Reason { get; set; }
        public CandidateInfo Top1 { get; set; }
        public CandidateInfo Top2 { get; set; }
    }

    internal sealed class MatchRunResult
    {
        public List<AppliedMatch> AppliedMatches { get; } = new List<AppliedMatch>();
        public List<ReviewMatch> ReviewMatches { get; } = new List<ReviewMatch>();
    }

    internal static class AutoMatchUtils
    {
        private static readonly CultureInfo PlCulture = new CultureInfo("pl-PL");
        private static readonly CultureInfo InvariantCulture = CultureInfo.InvariantCulture;
        private static readonly string[] DateFormats =
        {
            "dd.MM.yyyy",
            "dd.MM.yyyy HH:mm:ss",
            "dd.MM.yyyy H:mm:ss",
            "yyyy-MM-dd",
            "yyyy-MM-dd HH:mm:ss",
            "yyyy-MM-dd H:mm:ss",
            "yyyy-MM-ddTHH:mm:ss",
            "yyyy-MM-ddTHH:mm:ss.fff"
        };

        public static string NormalizeText(string input)
        {
            if (string.IsNullOrWhiteSpace(input)) return string.Empty;

            var builder = new StringBuilder(input.Length);
            foreach (char ch in input.ToUpperInvariant())
            {
                if (char.IsLetterOrDigit(ch) || char.IsWhiteSpace(ch))
                {
                    builder.Append(ch);
                }
                else
                {
                    builder.Append(' ');
                }
            }

            var result = new StringBuilder(builder.Length);
            bool prevSpace = true;
            foreach (char ch in builder.ToString())
            {
                if (char.IsWhiteSpace(ch))
                {
                    if (!prevSpace)
                    {
                        result.Append(' ');
                        prevSpace = true;
                    }
                }
                else
                {
                    result.Append(ch);
                    prevSpace = false;
                }
            }

            return result.ToString().Trim();
        }

        public static string NormalizePostal(string input)
        {
            if (string.IsNullOrWhiteSpace(input)) return string.Empty;
            var digits = new string(input.Where(char.IsDigit).ToArray());
            return digits;
        }

        public static bool TryParseFlexibleDate(string input, out DateTime parsed)
        {
            parsed = default;
            if (string.IsNullOrWhiteSpace(input)) return false;

            if (DateTime.TryParseExact(input.Trim(), DateFormats, PlCulture, DateTimeStyles.None, out parsed))
            {
                return true;
            }

            if (DateTime.TryParseExact(input.Trim(), DateFormats, InvariantCulture, DateTimeStyles.None, out parsed))
            {
                return true;
            }

            if (DateTime.TryParse(input, PlCulture, DateTimeStyles.None, out parsed))
            {
                return true;
            }

            return DateTime.TryParse(input, InvariantCulture, DateTimeStyles.None, out parsed);
        }

        public static decimal? TryParseDecimal(string input)
        {
            if (string.IsNullOrWhiteSpace(input)) return null;
            if (decimal.TryParse(input, NumberStyles.Any, PlCulture, out var pl)) return pl;
            if (decimal.TryParse(input, NumberStyles.Any, InvariantCulture, out var inv)) return inv;
            return null;
        }
    }

    internal sealed class AutoMatcher
    {
        private sealed class DocumentIndex
        {
            private readonly List<CsvDocument> _datedDocuments;
            private readonly List<DateTime> _dates;
            private readonly List<CsvDocument> _undatedDocuments;

            public DocumentIndex(IEnumerable<CsvDocument> documents)
            {
                _datedDocuments = documents.Where(d => d.DocumentDate.HasValue)
                    .OrderBy(d => d.DocumentDate.Value)
                    .ToList();
                _dates = _datedDocuments.Select(d => d.DocumentDate.Value.Date).ToList();
                _undatedDocuments = documents.Where(d => !d.DocumentDate.HasValue).ToList();
            }

            public IReadOnlyList<CsvDocument> GetDocuments(DateTime? start, DateTime? end)
            {
                if (!start.HasValue || !end.HasValue)
                {
                    return _datedDocuments.Concat(_undatedDocuments).ToList();
                }

                int startIndex = LowerBound(_dates, start.Value.Date);
                int endIndex = UpperBound(_dates, end.Value.Date);
                var range = _datedDocuments.Skip(startIndex).Take(endIndex - startIndex).ToList();
                return range;
            }

            private static int LowerBound(List<DateTime> list, DateTime value)
            {
                int lo = 0;
                int hi = list.Count;
                while (lo < hi)
                {
                    int mid = (lo + hi) / 2;
                    if (list[mid] < value) lo = mid + 1; else hi = mid;
                }
                return lo;
            }

            private static int UpperBound(List<DateTime> list, DateTime value)
            {
                int lo = 0;
                int hi = list.Count;
                while (lo < hi)
                {
                    int mid = (lo + hi) / 2;
                    if (list[mid] <= value) lo = mid + 1; else hi = mid;
                }
                return lo;
            }
        }

        private sealed class ScoredCandidate
        {
            public CsvDocument Document { get; set; }
            public int Score { get; set; }
            public string TowarSample { get; set; }
            public int ProductScore { get; set; }
            public int ClientScore { get; set; }
        }

        public MatchRunResult Match(IEnumerable<MatchInput> inputs, IReadOnlyList<CsvDocument> documents, AutoMatchOptions options, IProgress<ProgressInfo> progress)
        {
            if (inputs == null) throw new ArgumentNullException(nameof(inputs));
            if (documents == null) throw new ArgumentNullException(nameof(documents));
            if (options == null) throw new ArgumentNullException(nameof(options));

            var index = new DocumentIndex(documents);
            var results = new MatchRunResult();
            var inputList = inputs.ToList();
            int total = inputList.Count;
            int processed = 0;

            foreach (var input in inputList)
            {
                processed++;
                if (processed % 25 == 0 || processed == total)
                {
                    progress?.Report(new ProgressInfo
                    {
                        Percent = total == 0 ? 100 : (int)(processed / (double)total * 100),
                        Message = $"Dopasowywanie {processed}/{total}..."
                    });
                }

                var range = GetDateRange(input, out bool noDateFilter, out string dateReason);
                var candidates = index.GetDocuments(range.start, range.end);

                var scored = candidates.Select(d => ScoreDocument(input, d, options.IncludeClientMatch))
                    .Where(s => s.Score > 0)
                    .OrderByDescending(s => s.Score)
                    .ThenBy(s => s.Document.DocumentNumber)
                    .ToList();

                var top1 = scored.FirstOrDefault();
                var top2 = scored.Skip(1).FirstOrDefault();

                if (top1 == null)
                {
                    results.ReviewMatches.Add(new ReviewMatch
                    {
                        ZgloszenieId = input.Id,
                        OldNrFaktury = input.NrFaktury,
                        OldDataZakupu = input.DataZakupu,
                        Reason = BuildReason("LOW_CONFIDENCE", noDateFilter, dateReason),
                        Top1 = null,
                        Top2 = null
                    });
                    continue;
                }

                int secondScore = top2?.Score ?? 0;
                bool accepted = top1.Score >= options.ScoreThreshold && (top1.Score - secondScore) >= options.MinScoreDelta;

                if (accepted)
                {
                    string newNr = string.IsNullOrWhiteSpace(input.NrFaktury) ? top1.Document.DocumentNumber : null;
                    string newDate = string.IsNullOrWhiteSpace(input.DataZakupu) && top1.Document.DocumentDate.HasValue
                        ? top1.Document.DocumentDate.Value.ToString("yyyy-MM-dd")
                        : null;

                    results.AppliedMatches.Add(new AppliedMatch
                    {
                        ZgloszenieId = input.Id,
                        OldNrFaktury = input.NrFaktury,
                        NewNrFaktury = newNr,
                        OldDataZakupu = input.DataZakupu,
                        NewDataZakupu = newDate,
                        Score = top1.Score,
                        DocDate = top1.Document.DocumentDate?.ToString("yyyy-MM-dd"),
                        Kontrahent = top1.Document.Kontrahent,
                        TowarSample = top1.TowarSample,
                        Dokument = top1.Document.DocumentNumber
                    });
                }
                else
                {
                    results.ReviewMatches.Add(new ReviewMatch
                    {
                        ZgloszenieId = input.Id,
                        OldNrFaktury = input.NrFaktury,
                        OldDataZakupu = input.DataZakupu,
                        Reason = BuildReason("LOW_CONFIDENCE", noDateFilter, dateReason),
                        Top1 = CreateCandidateInfo(top1),
                        Top2 = top2 == null ? null : CreateCandidateInfo(top2)
                    });
                }
            }

            progress?.Report(new ProgressInfo { Percent = 100, Message = "Dopasowywanie zakończone." });
            return results;
        }

        private static CandidateInfo CreateCandidateInfo(ScoredCandidate candidate)
        {
            if (candidate == null) return null;
            return new CandidateInfo
            {
                Dokument = candidate.Document.DocumentNumber,
                Score = candidate.Score,
                DocDate = candidate.Document.DocumentDate?.ToString("yyyy-MM-dd"),
                Kontrahent = candidate.Document.Kontrahent,
                TowarSample = candidate.TowarSample
            };
        }

        private static (DateTime? start, DateTime? end) GetDateRange(MatchInput input, out bool noDateFilter, out string reason)
        {
            noDateFilter = false;
            reason = null;
            if (AutoMatchUtils.TryParseFlexibleDate(input.DataZgloszenia, out var zglDate))
            {
                return (zglDate.AddMonths(-24), zglDate.AddDays(1));
            }

            if (string.IsNullOrWhiteSpace(input.NrFaktury) && AutoMatchUtils.TryParseFlexibleDate(input.DataZakupu, out var zakDate))
            {
                return (zakDate.AddDays(-45), zakDate.AddDays(45));
            }

            noDateFilter = true;
            reason = string.IsNullOrWhiteSpace(input.DataZgloszenia) ? "NO_DATE" : "NO_DATE_PARSE";
            return (null, null);
        }

        private static string BuildReason(string baseReason, bool noDateFilter, string dateReason)
        {
            if (!noDateFilter) return baseReason;
            if (string.IsNullOrWhiteSpace(dateReason)) return $"{baseReason};NO_DATE_FILTER";
            return $"{baseReason};NO_DATE_FILTER;{dateReason}";
        }

        private static ScoredCandidate ScoreDocument(MatchInput input, CsvDocument doc, bool includeClient)
        {
            var productScore = ScoreProduct(input.Product, doc);
            var clientScore = includeClient ? ScoreClient(input.Client, doc) : 0;
            int score = productScore.score + clientScore;

            return new ScoredCandidate
            {
                Document = doc,
                Score = score,
                TowarSample = productScore.towarSample,
                ProductScore = productScore.score,
                ClientScore = clientScore
            };
        }

        private static (int score, string towarSample) ScoreProduct(ProductInfo product, CsvDocument doc)
        {
            if (product == null || doc?.Positions == null || doc.Positions.Count == 0) return (0, null);

            string kodEnova = AutoMatchUtils.NormalizeText(product.KodEnova);
            string kodProducenta = AutoMatchUtils.NormalizeText(product.KodProducenta);
            string nazwaKrotka = AutoMatchUtils.NormalizeText(product.NazwaKrotka);
            string nazwaSystemowa = AutoMatchUtils.NormalizeText(product.NazwaSystemowa);

            int bestScore = 0;
            string bestTowar = null;

            foreach (var pos in doc.Positions)
            {
                string towarNorm = AutoMatchUtils.NormalizeText(pos.Towar);
                int score = 0;

                if (!string.IsNullOrWhiteSpace(kodEnova) && towarNorm.Contains(kodEnova)) score += 100;
                if (!string.IsNullOrWhiteSpace(kodProducenta) && towarNorm.Contains(kodProducenta)) score += 70;
                if (!string.IsNullOrWhiteSpace(nazwaKrotka) && towarNorm.Contains(nazwaKrotka)) score += 40;
                if (!string.IsNullOrWhiteSpace(nazwaSystemowa) && towarNorm.Contains(nazwaSystemowa)) score += 25;

                if (score > bestScore)
                {
                    bestScore = score;
                    bestTowar = pos.Towar;
                }
            }

            return (bestScore, bestTowar);
        }

        private static int ScoreClient(ClientInfo client, CsvDocument doc)
        {
            if (client == null || doc == null) return 0;

            int score = 0;
            string klientKod = AutoMatchUtils.NormalizePostal(client.KodPocztowy);
            string docKod = AutoMatchUtils.NormalizePostal(doc.KodPocztowy);
            if (!string.IsNullOrWhiteSpace(klientKod) && klientKod == docKod) score += 30;

            string klientMiejsc = AutoMatchUtils.NormalizeText(client.Miejscowosc);
            string docMiejsc = AutoMatchUtils.NormalizeText(doc.Miejscowosc);
            if (!string.IsNullOrWhiteSpace(klientMiejsc) && klientMiejsc == docMiejsc) score += 20;

            string klientNazwa = AutoMatchUtils.NormalizeText(!string.IsNullOrWhiteSpace(client.NazwaFirmy)
                ? client.NazwaFirmy
                : client.ImieNazwisko);
            if (!string.IsNullOrWhiteSpace(klientNazwa))
            {
                string docNames = AutoMatchUtils.NormalizeText(string.Join(" ", new[] { doc.Kontrahent, doc.Pelny }));
                if (!string.IsNullOrWhiteSpace(docNames) && docNames.Contains(klientNazwa)) score += 20;
            }

            return score;
        }
    }
}
