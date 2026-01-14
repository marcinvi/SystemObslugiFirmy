using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace Reklamacje_Dane
{
    internal sealed class CsvLoadResult
    {
        public List<CsvDocument> Documents { get; } = new List<CsvDocument>();
        public HashSet<string> DocumentNumbers { get; } = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
        public List<string> MissingFiles { get; } = new List<string>();
        public int TotalRows { get; set; }
    }

    internal static class CsvSaleLoader
    {
        private static readonly Encoding EnovaEncoding = Encoding.GetEncoding(1250);

        public static CsvLoadResult LoadDocuments(IEnumerable<string> paths, IProgress<ProgressInfo> progress)
        {
            if (paths == null) throw new ArgumentNullException(nameof(paths));

            var result = new CsvLoadResult();
            var documentsByNumber = new Dictionary<string, CsvDocument>(StringComparer.OrdinalIgnoreCase);
            var fileList = paths.ToList();
            int fileCount = fileList.Count;
            int fileIndex = 0;

            foreach (var path in fileList)
            {
                fileIndex++;
                if (!File.Exists(path))
                {
                    result.MissingFiles.Add(path);
                    progress?.Report(new ProgressInfo
                    {
                        Percent = (int)(fileIndex / (double)fileCount * 100),
                        Message = $"Brak pliku: {Path.GetFileName(path)}"
                    });
                    continue;
                }

                var lines = File.ReadLines(path, EnovaEncoding).ToList();
                if (lines.Count == 0) continue;

                var header = lines[0].Split('\t');
                var headerMap = BuildHeaderMap(header);

                int idxDokument = GetIndex(headerMap, "Dokument");
                int idxData = GetIndex(headerMap, "Data");
                int idxKontrahent = GetIndex(headerMap, "Kontrahent");
                int idxPelny = GetIndex(headerMap, "Pełny");
                int idxKodPocztowy = GetIndex(headerMap, "Kod pocztowy");
                int idxMiejscowosc = GetIndex(headerMap, "Miejscowość");
                int idxTowar = GetIndex(headerMap, "Towar");
                int idxIlosc = GetIndex(headerMap, "Ilość");

                for (int i = 1; i < lines.Count; i++)
                {
                    var line = lines[i];
                    if (string.IsNullOrWhiteSpace(line)) continue;

                    var fields = line.Split('\t');
                    if (idxDokument < 0 || idxDokument >= fields.Length) continue;

                    string dokument = fields[idxDokument].Trim();
                    if (string.IsNullOrWhiteSpace(dokument)) continue;

                    if (!documentsByNumber.TryGetValue(dokument, out var doc))
                    {
                        doc = new CsvDocument(dokument);
                        documentsByNumber[dokument] = doc;
                    }

                    if (idxData >= 0 && idxData < fields.Length && doc.DocumentDate == null)
                    {
                        if (AutoMatchUtils.TryParseFlexibleDate(fields[idxData], out var dt))
                        {
                            doc.DocumentDate = dt.Date;
                        }
                    }

                    if (idxKontrahent >= 0 && idxKontrahent < fields.Length && string.IsNullOrWhiteSpace(doc.Kontrahent))
                    {
                        doc.Kontrahent = fields[idxKontrahent].Trim();
                    }

                    if (idxPelny >= 0 && idxPelny < fields.Length && string.IsNullOrWhiteSpace(doc.Pelny))
                    {
                        doc.Pelny = fields[idxPelny].Trim();
                    }

                    if (idxKodPocztowy >= 0 && idxKodPocztowy < fields.Length && string.IsNullOrWhiteSpace(doc.KodPocztowy))
                    {
                        doc.KodPocztowy = fields[idxKodPocztowy].Trim();
                    }

                    if (idxMiejscowosc >= 0 && idxMiejscowosc < fields.Length && string.IsNullOrWhiteSpace(doc.Miejscowosc))
                    {
                        doc.Miejscowosc = fields[idxMiejscowosc].Trim();
                    }

                    string towar = idxTowar >= 0 && idxTowar < fields.Length ? fields[idxTowar].Trim().Trim('"') : null;
                    decimal? ilosc = idxIlosc >= 0 && idxIlosc < fields.Length ? AutoMatchUtils.TryParseDecimal(fields[idxIlosc]) : null;

                    doc.Positions.Add(new CsvPosition { Towar = towar, Ilosc = ilosc });
                    result.TotalRows++;
                }

                progress?.Report(new ProgressInfo
                {
                    Percent = (int)(fileIndex / (double)fileCount * 100),
                    Message = $"Wczytano {Path.GetFileName(path)} ({lines.Count - 1} wierszy)"
                });
            }

            result.Documents.AddRange(documentsByNumber.Values);
            foreach (var doc in result.Documents)
            {
                if (!string.IsNullOrWhiteSpace(doc.DocumentNumber))
                {
                    result.DocumentNumbers.Add(doc.DocumentNumber.Trim());
                }
            }

            return result;
        }

        private static Dictionary<string, int> BuildHeaderMap(string[] header)
        {
            var map = new Dictionary<string, int>(StringComparer.OrdinalIgnoreCase);
            for (int i = 0; i < header.Length; i++)
            {
                var key = header[i].Trim();
                if (!map.ContainsKey(key))
                {
                    map[key] = i;
                }
            }

            return map;
        }

        private static int GetIndex(Dictionary<string, int> map, string name)
        {
            return map.TryGetValue(name, out var idx) ? idx : -1;
        }
    }
}
