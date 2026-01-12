using System;
using System.Windows.Forms;

namespace Reklamacje_Dane
{
    public static class RtfHelper
    {
        // Ukryty RichTextBox do konwersji (singleton dla wydajności)
        private static RichTextBox _rtb = new RichTextBox();

        public static string ConvertRtfToText(string rtfContent)
        {
            if (string.IsNullOrWhiteSpace(rtfContent)) return "";

            // Sprawdzenie czy to w ogóle jest RTF (zaczyna się od {\rtf)
            if (!rtfContent.Trim().StartsWith("{\\rtf"))
            {
                return rtfContent; // To zwykły tekst, zwracamy bez zmian
            }

            try
            {
                _rtb.Rtf = rtfContent;
                return _rtb.Text;
            }
            catch
            {
                return rtfContent; // Jeśli format jest błędny, zwróć oryginał
            }
        }
    }
}