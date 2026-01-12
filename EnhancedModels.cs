using System;
using System.Collections.Generic;
using System.ComponentModel;

namespace Reklamacje_Dane
{
    /// <summary>
    /// 🎯 Enhanced Product ViewModel - Z confidence scoring
    /// </summary>
    public class EnhancedProductViewModel
    {
        public int Id { get; set; }

        [DisplayName("Nazwa Produktu")]
        public string NazwaSystemowa { get; set; }

        [DisplayName("Nazwa Krótka")]
        public string NazwaKrotka { get; set; }

        [DisplayName("Kod Enova")]
        public string KodEnova { get; set; }

        [DisplayName("Kod Producenta")]
        public string KodProducenta { get; set; }

        [DisplayName("Kategoria")]
        public string Kategoria { get; set; }

        [DisplayName("Producent")]
        public string Producent { get; set; }

        [Browsable(false)]
        public int ComplaintCount { get; set; }

        [DisplayName("Dopasowanie")]
        public int ConfidencePercent { get; set; }

        [Browsable(false)]
        public string MatchDetails { get; set; }

        /// <summary>
        /// ✅ NAPRAWA: DisplayName BEZ [Browsable(false)] - żeby DataGridView mógł bindować!
        /// </summary>
        [DisplayName("Produkt")]
        public string DisplayName
        {
            get
            {
                if (!string.IsNullOrWhiteSpace(NazwaKrotka))
                    return NazwaKrotka;
                if (!string.IsNullOrWhiteSpace(NazwaSystemowa))
                    return NazwaSystemowa;
                return $"[ID:{Id}]";
            }
        }

        [DisplayName("Match")]
        public string ConfidenceDisplay
        {
            get
            {
                if (ConfidencePercent >= 90) return $"✓✓✓ {ConfidencePercent}%";
                if (ConfidencePercent >= 70) return $"✓✓  {ConfidencePercent}%";
                if (ConfidencePercent >= 50) return $"✓   {ConfidencePercent}%";
                return $"    {ConfidencePercent}%";
            }
        }

        [Browsable(false)]
        public string TooltipText
        {
            get
            {
                var lines = new System.Text.StringBuilder();
                lines.AppendLine($"Nazwa: {DisplayName}");
                if (!string.IsNullOrWhiteSpace(Producent))
                    lines.AppendLine($"Producent: {Producent}");
                if (!string.IsNullOrWhiteSpace(KodProducenta))
                    lines.AppendLine($"Kod Prod: {KodProducenta}");
                if (!string.IsNullOrWhiteSpace(KodEnova))
                    lines.AppendLine($"Kod Enova: {KodEnova}");
                if (ComplaintCount > 0)
                    lines.AppendLine($"Zgłoszeń: {ComplaintCount}");
                if (!string.IsNullOrWhiteSpace(MatchDetails))
                    lines.AppendLine($"Match: {MatchDetails}");
                return lines.ToString().Trim();
            }
        }
    }

    /// <summary>
    /// 🎯 Enhanced Client ViewModel
    /// </summary>
    public class EnhancedClientViewModel
    {
        public int Id { get; set; }

        [DisplayName("Imię i Nazwisko")]
        public string ImieNazwisko { get; set; }

        [DisplayName("Nazwa Firmy")]
        public string NazwaFirmy { get; set; }

        [DisplayName("NIP")]
        public string NIP { get; set; }

        [DisplayName("Ulica")]
        public string Ulica { get; set; }

        [DisplayName("Kod")]
        public string KodPocztowy { get; set; }

        [DisplayName("Miejscowość")]
        public string Miejscowosc { get; set; }

        [DisplayName("Email")]
        public string Email { get; set; }

        [DisplayName("Telefon")]
        public string Telefon { get; set; }

        [DisplayName("Zgłoszeń")]
        public int ComplaintCount { get; set; }

        [Browsable(false)]
        public int ConfidencePercent { get; set; }

        [Browsable(false)]
        public string MatchDetails { get; set; }

        [Browsable(false)]
        public bool HasConflict { get; set; }

        [Browsable(false)]
        public string ConflictReason { get; set; }

        [Browsable(false)]
        public bool IsCompany => !string.IsNullOrWhiteSpace(NazwaFirmy);

        [DisplayName("Klient")]
        public string DisplayName => IsCompany ? $"{NazwaFirmy} ({ImieNazwisko})" : ImieNazwisko;

        [DisplayName("Adres")]
        public string FullAddress => $"{Ulica}, {KodPocztowy} {Miejscowosc}".Trim(' ', ',');

        [DisplayName("Match")]
        public string ConfidenceDisplay
        {
            get
            {
                string emoji = ConfidencePercent >= 90 ? "✓✓✓" : ConfidencePercent >= 70 ? "✓✓" : "✓";
                string conflict = HasConflict ? " ⚠" : "";
                return $"{emoji} {ConfidencePercent}%{conflict}";
            }
        }

        [Browsable(false)]
        public string TooltipText
        {
            get
            {
                var lines = new System.Text.StringBuilder();
                lines.AppendLine($"Klient: {DisplayName}");
                lines.AppendLine($"Adres: {FullAddress}");
                if (!string.IsNullOrWhiteSpace(Email))
                    lines.AppendLine($"Email: {Email}");
                if (!string.IsNullOrWhiteSpace(Telefon))
                    lines.AppendLine($"Tel: {Telefon}");
                if (ComplaintCount > 0)
                    lines.AppendLine($"Zgłoszeń: {ComplaintCount}");
                if (HasConflict)
                    lines.AppendLine($"⚠ KONFLIKT: {ConflictReason}");
                if (!string.IsNullOrWhiteSpace(MatchDetails))
                    lines.AppendLine($"Match: {MatchDetails}");
                return lines.ToString().Trim();
            }
        }
    }

    public class ComplaintSummaryViewModel
    {
        public int CompletionPercent { get; set; }
        public bool HasClient { get; set; }
        public string ClientDisplay { get; set; }
        public bool IsNewClient { get; set; }
        public bool HasProduct { get; set; }
        public string ProductDisplay { get; set; }
        public bool HasInvoice { get; set; }
        public string InvoiceNumber { get; set; }
        public bool HasSerialNumber { get; set; }
        public string SerialNumber { get; set; }
        public bool HasDate { get; set; }
        public DateTime? PurchaseDate { get; set; }
        public bool HasDescription { get; set; }
        public string Description { get; set; }
        public string WarrantyStatus { get; set; }
        public bool IsWarrantyValid { get; set; }
        public string Source { get; set; }
        public List<string> Warnings { get; set; } = new List<string>();
        public List<string> Errors { get; set; } = new List<string>();
        public bool ReadyToSave => HasClient && HasProduct && HasDescription && Errors.Count == 0;
        public string StatusEmoji
        {
            get
            {
                if (CompletionPercent >= 90) return "✅";
                if (CompletionPercent >= 50) return "⚠️";
                return "📝";
            }
        }
    }

    public class ProductSearchResult
    {
        public EnhancedProductViewModel Product { get; set; }
        public int Score { get; set; }
        public string HighlightedName { get; set; }
        public bool IsRecommended => Score >= 90;
    }
}