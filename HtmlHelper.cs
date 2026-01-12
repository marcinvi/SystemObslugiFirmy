using System;
using System.Text.RegularExpressions;
using System.Net;

namespace Reklamacje_Dane
{
    public static class HtmlHelper
    {
        public static string ConvertHtmlToText(string html)
        {
            if (string.IsNullOrEmpty(html))
                return string.Empty;

            // 1. Dekodowanie HTML na start (żeby zamienić &lt; na < itp.)
            string text = WebUtility.HtmlDecode(html);

            // 2. USUWANIE CAŁYCH BLOKÓW TECHNICZNYCH (To naprawi Twój błąd)
            // Usuwamy wszystko co jest pomiędzy <style>...</style>, <script>...</script>, <head>...</head>
            // Używamy flagi Singleline (?s), żeby kropka łapała też nowe linie
            text = Regex.Replace(text, @"<head[^>]*>.*?</head>", "", RegexOptions.IgnoreCase | RegexOptions.Singleline);
            text = Regex.Replace(text, @"<style[^>]*>.*?</style>", "", RegexOptions.IgnoreCase | RegexOptions.Singleline);
            text = Regex.Replace(text, @"<script[^>]*>.*?</script>", "", RegexOptions.IgnoreCase | RegexOptions.Singleline);
            text = Regex.Replace(text, @"<xml[^>]*>.*?</xml>", "", RegexOptions.IgnoreCase | RegexOptions.Singleline);

            // 3. Usuwanie komentarzy i warunków Outlooka ()
            text = Regex.Replace(text, @"", "", RegexOptions.IgnoreCase | RegexOptions.Singleline);
            text = Regex.Replace(text, @"<!\[if !supportLists\]>", "", RegexOptions.IgnoreCase);
            text = Regex.Replace(text, @"<!\[endif\]>", "", RegexOptions.IgnoreCase);

            // 4. Zamiana ważnych tagów na nowe linie
            // <br>, <p>, <div>, <li> zamieniamy na Enter
            text = Regex.Replace(text, @"<(br|p|div|li)[^>]*>", Environment.NewLine, RegexOptions.IgnoreCase);

            // 5. Usunięcie WSZYSTKICH pozostałych tagów HTML (np. <b>, <span>, <font>, <o:p>)
            text = Regex.Replace(text, @"<[^>]+>", "", RegexOptions.IgnoreCase);

            // 6. Czyszczenie wielokrotnych pustych linii i spacji
            // Zamień wielokrotne entery na podwójny enter
            text = Regex.Replace(text, @"(\r\n|\r|\n){3,}", Environment.NewLine + Environment.NewLine);

            // Opcjonalnie: Usuń nadmiarowe spacje (więcej niż 2 obok siebie)
            text = Regex.Replace(text, @" {2,}", " ");

            return text.Trim();
        }
    }
}