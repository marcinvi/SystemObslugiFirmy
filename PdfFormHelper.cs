using System;
using System.Collections.Generic;
using System.IO;

namespace Reklamacje_Dane
{
    public static class PdfFormHelper
    {
        /// <summary>
        /// Wypełnia pola formularza AcroForm i (opcjonalnie) spłaszcza je.
        /// - Wykrywa XFA i zwraca czytelny błąd (iText7 bez pdfXFA nie obsłuży XFA).
        /// - Dwupasowo: najpierw z flatten, a jeśli się wysypie, zapisuje bez flatteningu.
        /// Zgodne z C# 7.3; pełne kwalifikacje iText (brak kolizji z PdfSharp).
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

            // 1) Detekcja XFA (jeśli XFA -> rzuć czytelny wyjątek)
            using (var r0 = new iText.Kernel.Pdf.PdfReader(templatePath))
            using (var w0 = new iText.Kernel.Pdf.PdfWriter(new MemoryStream())) // roboczy writer do samej inspekcji
            using (var p0 = new iText.Kernel.Pdf.PdfDocument(r0, w0))
            {
                var f0 = iText.Forms.PdfAcroForm.GetAcroForm(p0, true);
                try
                {
                    var xfa = f0.GetXfaForm(); // iText.Forms.Xfa.XfaForm
                    if (xfa != null && xfa.IsXfaPresent())
                        throw new InvalidOperationException("Ten PDF jest formularzem XFA. iText7 wymaga pdfXFA (dodatek/licencja) albo musisz przekonwertować plik do AcroForm (np. wydruk do PDF).");
                }
                catch (InvalidOperationException) { throw; }
                catch
                {
                    // GetXfaForm może nie być dostępne w bardzo starych wersjach;
                    // wtedy i tak przejdziemy dalej — jeśli to XFA, padnie później, ale z naszym fallbackiem.
                }
            }

            // 2) Główna próba — z flatten (jeśli proszono)
            try
            {
                InternalFill(templatePath, outputPath, fieldValues, fontPath, flatten, fontSize);
                return;
            }
            catch (Exception ex)
            {
                // Jeśli nie proszono o flatten albo błąd nie dotyczy flatteningu, rzuć dalej.
                if (!flatten)
                    throw new Exception("[PDF] Błąd wypełniania formularza (bez flatteningu): " + ex.ToString());

                // 3) Fallback — zapisz bez flatteningu (żeby mieć działający plik)
                try
                {
                    InternalFill(templatePath, outputPath, fieldValues, fontPath, false, fontSize);
                }
                catch (Exception ex2)
                {
                    throw new Exception("[PDF] Fallback bez flatteningu również nie powiódł się.\nPierwszy błąd:\n" + ex.ToString() + "\n\nDrugi błąd:\n" + ex2.ToString());
                }
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
            using (var reader = new iText.Kernel.Pdf.PdfReader(templatePath))
            using (var writer = new iText.Kernel.Pdf.PdfWriter(outputPath))
            using (var pdf = new iText.Kernel.Pdf.PdfDocument(reader, writer))
            {
                var form = iText.Forms.PdfAcroForm.GetAcroForm(pdf, true);
                form.SetNeedAppearances(false);

                iText.Kernel.Font.PdfFont font = null;
                if (!string.IsNullOrEmpty(fontPath) && File.Exists(fontPath))
                {
                    // W Twojej wersji iText 3. argument to PdfDocument:
                    font = iText.Kernel.Font.PdfFontFactory.CreateFont(
                        fontPath,
                        "Identity-H",
                        pdf
                    );
                }

                foreach (var kv in fieldValues)
                {
                    var name = kv.Key ?? string.Empty;
                    var val = kv.Value ?? string.Empty;

                    iText.Forms.Fields.PdfFormField field = form.GetField(name);
                    if (field == null)
                    {
                        System.Diagnostics.Debug.WriteLine("[PDF] Brak pola: " + name);
                        continue;
                    }

                    if (font != null)
                    {
                        field.SetFont(font);
                        field.SetFontSize(fontSize);
                    }

                    field.SetValue(val);
                    field.SetReadOnly(true);
                }

                if (flatten)
                {
                    form.FlattenFields();
                }

                pdf.Close();
            }
        }
    }
}
