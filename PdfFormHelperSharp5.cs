using System;
using System.Collections.Generic;
using System.IO;
using iTextSharp.text.pdf;

namespace Reklamacje_Dane
{
    public static class PdfFormHelperSharp5
    {
        /// <summary>
        /// Wypełnia pola AcroForm (czcionka Unicode), opcjonalnie flatten.
        /// Wykrywa XFA i rzuca czytelny wyjątek.
        /// </summary>
        public static void FillAndFlatten(
            string templatePath,
            string outputPath,
            IDictionary<string, string> fieldValues,
            string fontPath,
            bool flatten,
            float fontSize)
        {
            if (!File.Exists(templatePath))
                throw new FileNotFoundException("Brak pliku szablonu PDF", templatePath);

            var outDir = Path.GetDirectoryName(outputPath);
            if (!string.IsNullOrEmpty(outDir)) Directory.CreateDirectory(outDir);

            // 1) Szybka inspekcja XFA
            var r0 = new PdfReader(templatePath);
            try
            {
                var af0 = r0.AcroFields;
                if (af0 != null && af0.Xfa != null && af0.Xfa.XfaPresent)
                    throw new InvalidOperationException("Ten PDF jest formularzem XFA. Wypełnianie wymaga konwersji do AcroForm (np. 'Drukuj do PDF') albo użycia pdfXFA.");
            }
            finally
            {
                r0.Close();
            }

            // 2) Główna próba z ewentualnym flatten; jeśli padnie → fallback bez flatten
            try
            {
                InternalFill(templatePath, outputPath, fieldValues, fontPath, flatten, fontSize);
            }
            catch (Exception ex)
            {
                if (!flatten) throw new Exception("[PDF] Błąd wypełniania (bez flatteningu): " + ex);
                // Fallback: zapisz bez flatteningu
                InternalFill(templatePath, outputPath, fieldValues, fontPath, false, fontSize);
            }
        }

        private static void InternalFill(
            string templatePath,
            string outputPath,
            IDictionary<string, string> fieldValues,
            string fontPath,
            bool flatten,
            float fontSize)
        {
            PdfReader reader = null;
            FileStream fs = null;
            PdfStamper stamper = null;

            try
            {
                reader = new PdfReader(templatePath);
                fs = new FileStream(outputPath, FileMode.Create, FileAccess.Write);
                stamper = new PdfStamper(reader, fs);

                var fields = stamper.AcroFields;
                fields.GenerateAppearances = true;

                BaseFont bf = null;
                if (!string.IsNullOrEmpty(fontPath) && File.Exists(fontPath))
                {
                    // Unicode + embed
                    bf = BaseFont.CreateFont(fontPath, BaseFont.IDENTITY_H, BaseFont.EMBEDDED);
                    // globalnie dodaj substytucję – działa nawet gdy nie ustawimy ręcznie fontu na każdym polu
                    fields.AddSubstitutionFont(bf);
                }

                foreach (var kv in fieldValues)
                {
                    var name = kv.Key ?? string.Empty;
                    var val = kv.Value ?? string.Empty;

                    if (bf != null)
                    {
                        // ustaw czcionkę/rozmiar na polu – pewność polskich znaków i renderu
                        fields.SetFieldProperty(name, "textfont", bf, null);
                        fields.SetFieldProperty(name, "textsize", fontSize, null);
                    }

                    // SetField zwraca false gdy pole nie istnieje – nie przerywamy, ale to logujemy
                    bool ok = fields.SetField(name, val);
                    if (!ok)
                        System.Diagnostics.Debug.WriteLine("[PDF] Brak pola lub nie można ustawić: " + name);

                    // opcjonalnie: oznacz jako readonly (jeśli chcesz edytowalne, zakomentuj)
                    var item = fields.GetFieldItem(name);
                    if (item != null)
                    {
                        for (int i = 0; i < item.Size; i++)
                        {
                            var merged = item.GetMerged(i);
                            merged.Put(PdfName.FF, new PdfNumber(PdfFormField.FF_READ_ONLY));
                        }
                    }
                }

                stamper.FormFlattening = flatten;
            }
            finally
            {
                if (stamper != null) stamper.Close();
                if (fs != null) fs.Close();
                if (reader != null) reader.Close();
            }
        }
    }
}
