// Plik: EncryptionHelper.cs (Wersja kompatybilna z .NET Framework)
using System;
using System.IO;
using System.Security.Cryptography;
using System.Text;

namespace Reklamacje_Dane
{
    public static class EncryptionHelper
    {
        public static byte[] MasterKey { get; set; }

        public static readonly byte[] Salt = new byte[]
        {
            0x1A, 0x2B, 0x3C, 0x4D, 0x5E, 0x6F, 0x7A, 0x8B,
            0x9C, 0xAD, 0xBE, 0xCF, 0xD1, 0xE2, 0xF3, 0x04
        };
        public static string HashPassword(string password)
        {
            if (string.IsNullOrEmpty(password))
                return null;

            using (var sha256 = SHA256.Create())
            {
                // Łączymy hasło z solą, aby utrudnić ataki tęczowymi tablicami
                byte[] passwordBytes = Encoding.UTF8.GetBytes(password);
                byte[] combinedBytes = new byte[passwordBytes.Length + Salt.Length];

                Buffer.BlockCopy(passwordBytes, 0, combinedBytes, 0, passwordBytes.Length);
                Buffer.BlockCopy(Salt, 0, combinedBytes, passwordBytes.Length, Salt.Length);

                byte[] hashedBytes = sha256.ComputeHash(combinedBytes);
                return Convert.ToBase64String(hashedBytes);
            }
        }
        public static string EncryptString(string plainText)
        {
            if (string.IsNullOrEmpty(plainText))
                return null;

            if (MasterKey == null || MasterKey.Length != 32)
                throw new InvalidOperationException("Klucz główny (MasterKey) jest nieustawiony lub ma nieprawidłową długość.");

            using (var aes = Aes.Create())
            {
                aes.Key = MasterKey;
                // IV (Wektor inicjalizacyjny) musi być losowy dla każdego szyfrowania
                aes.GenerateIV();
                byte[] iv = aes.IV;

                using (var memoryStream = new MemoryStream())
                {
                    // Zapisujemy IV na początku strumienia
                    memoryStream.Write(iv, 0, iv.Length);

                    using (var cryptoStream = new CryptoStream(memoryStream, aes.CreateEncryptor(), CryptoStreamMode.Write))
                    {
                        byte[] plaintextBytes = Encoding.UTF8.GetBytes(plainText);
                        cryptoStream.Write(plaintextBytes, 0, plaintextBytes.Length);
                        cryptoStream.FlushFinalBlock();
                    }
                    return Convert.ToBase64String(memoryStream.ToArray());
                }
            }
        }

        public static string DecryptString(string encryptedText)
        {
            if (string.IsNullOrEmpty(encryptedText))
                return null;

            if (MasterKey == null || MasterKey.Length != 32)
                throw new InvalidOperationException("Klucz główny (MasterKey) jest nieustawiony lub ma nieprawidłową długość.");

            try
            {
                byte[] encryptedData = Convert.FromBase64String(encryptedText);

                using (var aes = Aes.Create())
                {
                    aes.Key = MasterKey;

                    // Odczytujemy IV z początku zaszyfrowanych danych
                    byte[] iv = new byte[aes.BlockSize / 8];
                    if (encryptedData.Length < iv.Length)
                        return null; // Dane są uszkodzone

                    Array.Copy(encryptedData, 0, iv, 0, iv.Length);
                    aes.IV = iv;

                    using (var memoryStream = new MemoryStream())
                    {
                        using (var cryptoStream = new CryptoStream(memoryStream, aes.CreateDecryptor(), CryptoStreamMode.Write))
                        {
                            // Deszyfrujemy resztę danych (po IV)
                            cryptoStream.Write(encryptedData, iv.Length, encryptedData.Length - iv.Length);
                            cryptoStream.FlushFinalBlock();
                        }
                        return Encoding.UTF8.GetString(memoryStream.ToArray());
                    }
                }
            }
            catch (Exception)
            {
                return null; // Błąd podczas deszyfrowania
            }
        }
    }
}