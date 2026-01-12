using PdfSharp.Fonts;
using System;
using System.IO;
using System.Reflection;

namespace Reklamacje_Dane
{
    public class PdfFontResolver : IFontResolver
    {
        public string DefaultFontName => "Arial";

        // Ta metoda jest teraz odpowiedzialna za znalezienie danych czcionki na podstawie nazwy pliku
        public byte[] GetFont(string faceName)
        {
            // faceName to teraz bezpośrednio nazwa pliku, np. "arialbd.ttf"
            var assembly = Assembly.GetExecutingAssembly();
            // Nazwa zasobu to: [Domyślna przestrzeń nazw].[Nazwa folderu].[Nazwa pliku]
            string resourceName = $"Reklamacje_Dane.Fonts.{faceName}";

            try
            {
                using (Stream stream = assembly.GetManifestResourceStream(resourceName))
                {
                    if (stream == null)
                        throw new FileNotFoundException($"Nie można znaleźć zasobu czcionki '{resourceName}'. Upewnij się, że plik czcionki istnieje w folderze 'Fonts' i jego 'Build Action' jest ustawione na 'Embedded Resource'.");

                    using (var reader = new MemoryStream())
                    {
                        stream.CopyTo(reader);
                        return reader.ToArray();
                    }
                }
            }
            catch (Exception ex)
            {
                // Złapanie wyjątku, aby dostarczyć bardziej szczegółowych informacji w razie problemu
                throw new InvalidOperationException($"Wystąpił krytyczny błąd podczas ładowania czcionki '{faceName}'. Zobacz wyjątek wewnętrzny.", ex);
            }
        }

        // Ta metoda mapuje rodzinę czcionki i style na konkretną nazwę pliku
        public FontResolverInfo ResolveTypeface(string familyName, bool isBold, bool isItalic)
        {
            string fontFile;

            if (isBold && isItalic)
            {
                // Nie ma standardowego pliku "arialbi.ttf". Użyjemy pogrubionego jako najlepszego zamiennika.
                fontFile = "arialbd.ttf";
            }
            else if (isBold)
            {
                fontFile = "arialbd.ttf";
            }
            else if (isItalic)
            {
                fontFile = "ariali.ttf";
            }
            else
            {
                fontFile = "arial.ttf";
            }

            // Zwracamy obiekt z konkretną nazwą pliku do załadowania
            return new FontResolverInfo(fontFile);
        }
    }
}