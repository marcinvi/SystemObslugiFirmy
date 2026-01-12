namespace Reklamacje_Dane
{
    public class KontoPocztowe
    {
        public int Id { get; set; }
        public string NazwaWyswietlana { get; set; }
        public string AdresEmail { get; set; }
        public string Login { get; set; }
        public string Haslo { get; set; }

        // Wybór protokołu: "POP3" lub "IMAP"
        public string Protokol { get; set; }

        // Dane serwera przychodzącego (używamy jednych albo drugich)
        public string Pop3Host { get; set; }
        public int Pop3Port { get; set; }
        public bool Pop3Ssl { get; set; }

        public string ImapHost { get; set; }
        public int ImapPort { get; set; }
        public bool ImapSsl { get; set; }

        // SMTP (Wychodząca - wspólne dla obu)
        public string SmtpHost { get; set; }
        public int SmtpPort { get; set; }
        public bool SmtpSsl { get; set; }

        public string Podpis { get; set; }
        public bool CzyDomyslne { get; set; }
    }
}