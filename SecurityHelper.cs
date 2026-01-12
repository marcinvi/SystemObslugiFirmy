using System;
using System.IO;
using System.Security.Cryptography;
using System.Text;

namespace Reklamacje_Dane
{
    public static class SecurityHelper
    {
        // Główny klucz szyfrowania
        private static readonly string Key = "EnaReklamacjeSystemSecureKey2025";

        public static string Encrypt(string plainText)
        {
            if (string.IsNullOrEmpty(plainText)) return plainText;

            byte[] iv = new byte[16];
            byte[] array;

            using (Aes aes = Aes.Create())
            {
                aes.Key = DeriveKey(Key);
                aes.IV = iv;

                ICryptoTransform encryptor = aes.CreateEncryptor(aes.Key, aes.IV);

                using (MemoryStream memoryStream = new MemoryStream())
                {
                    using (CryptoStream cryptoStream = new CryptoStream((Stream)memoryStream, encryptor, CryptoStreamMode.Write))
                    {
                        using (StreamWriter streamWriter = new StreamWriter((Stream)cryptoStream))
                        {
                            streamWriter.Write(plainText);
                        }
                        array = memoryStream.ToArray();
                    }
                }
            }

            return Convert.ToBase64String(array);
        }

        public static string Decrypt(string cipherText)
        {
            if (string.IsNullOrEmpty(cipherText)) return cipherText;

            try
            {
                byte[] iv = new byte[16];
                byte[] buffer = Convert.FromBase64String(cipherText);

                using (Aes aes = Aes.Create())
                {
                    aes.Key = DeriveKey(Key);
                    aes.IV = iv;
                    ICryptoTransform decryptor = aes.CreateDecryptor(aes.Key, aes.IV);

                    using (MemoryStream memoryStream = new MemoryStream(buffer))
                    {
                        using (CryptoStream cryptoStream = new CryptoStream((Stream)memoryStream, decryptor, CryptoStreamMode.Read))
                        {
                            using (StreamReader streamReader = new StreamReader((Stream)cryptoStream))
                            {
                                return streamReader.ReadToEnd();
                            }
                        }
                    }
                }
            }
            catch
            {
                return ""; // W razie błędu deszyfrowania zwróć pusty ciąg
            }
        }

        private static byte[] DeriveKey(string password)
        {
            // POPRAWKA: Sól musi mieć co najmniej 8 bajtów.
            // Używamy stałej soli (zamiast losowej), aby móc potem odszyfrować hasło tym samym algorytmem.
            byte[] salt = new byte[] {
                0x49, 0x76, 0x61, 0x6e, 0x20, 0x4d, 0x65, 0x64,
                0x76, 0x65, 0x64, 0x65, 0x76
            };

            using (var rfc2898 = new Rfc2898DeriveBytes(password, salt, 1000))
            {
                return rfc2898.GetBytes(32); // AES-256 wymaga klucza 32-bajtowego
            }
        }
    }
}