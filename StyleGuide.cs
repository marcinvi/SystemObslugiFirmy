using System.Drawing;
using System.Windows.Forms;

namespace Reklamacje_Dane
{
    /// <summary>
    /// Centralne miejsce do zarządzania wyglądem aplikacji (kolory, czcionki).
    /// Dzięki tej klasie łatwo zmienisz motyw całej aplikacji w jednym miejscu.
    /// </summary>
    public static class StyleGuide
    {
        // === GŁÓWNA PALETA KOLORÓW ===

        // Kolory tła
        public static readonly Color FormBackground = Color.FromArgb(240, 243, 245); // Jasnoszary, czysty kolor tła
        public static readonly Color PanelBackground = Color.White; // Tło dla paneli, kontenerów
        public static readonly Color InputBackground = Color.FromArgb(252, 252, 252); // Tło dla pól tekstowych

        // Kolory tekstu
        public static readonly Color TextPrimary = Color.FromArgb(45, 45, 45); // Główny kolor tekstu
        public static readonly Color TextSecondary = Color.FromArgb(110, 110, 110); // Drugorzędny, do opisów
        public static readonly Color TextOnPrimaryBackground = Color.White; // Tekst na przyciskach/akcentach

        // Kolory akcentów i akcji
        public static readonly Color PrimaryColor = Color.FromArgb(0, 123, 255); // Główny kolor marki (niebieski)
        public static readonly Color SuccessColor = Color.FromArgb(40, 167, 69); // Kolor dla operacji udanych
        public static readonly Color DangerColor = Color.FromArgb(220, 53, 69);   // Kolor dla błędów i usunięcia
        public static readonly Color WarningColor = Color.FromArgb(255, 193, 7);   // Kolor dla ostrzeżeń

        // Kolory ramek i separatorów
        public static readonly Color BorderColor = Color.FromArgb(220, 223, 226); // Delikatny kolor ramki

        // === CZCIONKI ===

        public static readonly Font RegularFont = new Font("Segoe UI", 9.75F, FontStyle.Regular, GraphicsUnit.Point, 238);
        public static readonly Font BoldFont = new Font("Segoe UI", 9.75F, FontStyle.Bold, GraphicsUnit.Point, 238);
        public static readonly Font TitleFont = new Font("Segoe UI Semibold", 14F, FontStyle.Bold, GraphicsUnit.Point, 238);
        public static readonly Font SmallFont = new Font("Segoe UI", 8.25F, FontStyle.Regular, GraphicsUnit.Point, 238);

        /// <summary>
        /// Pomocnicza metoda do aplikowania podstawowego stylu do kontrolki.
        /// </summary>
        public static void ApplyBaseStyle(Control control)
        {
            control.Font = RegularFont;
            control.ForeColor = TextPrimary;
            control.BackColor = PanelBackground;
        }
    }
}