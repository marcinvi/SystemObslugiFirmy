using System;
using System.Security.Cryptography;

namespace Reklamacje_Dane
{
    /// <summary>
    /// Klasa pomocnicza do bezpiecznego zarządzania hasłami.
    /// Centralizuje logikę haszowania i weryfikacji.
    /// </summary>
    public static class PasswordHelper
    {
        private const int SaltSize = 16;
        private const int HashSize = 20;
        private const int Iterations = 10000;

        /// <summary>
        /// Tworzy hash hasła przy użyciu PBKDF2 z losową solą.
        /// </summary>
        /// <param name="password">Hasło w postaci czystego tekstu.</param>
        /// <returns>String Base64 zawierający sól i hash.</returns>
        public static string HashPassword(string password)
        {
            byte[] salt;
            new RNGCryptoServiceProvider().GetBytes(salt = new byte[SaltSize]);

            using (var pbkdf2 = new Rfc2898DeriveBytes(password, salt, Iterations, HashAlgorithmName.SHA1))
            {
                byte[] hash = pbkdf2.GetBytes(HashSize);

                byte[] hashBytes = new byte[SaltSize + HashSize];
                Array.Copy(salt, 0, hashBytes, 0, SaltSize);
                Array.Copy(hash, 0, hashBytes, SaltSize, HashSize);

                return Convert.ToBase64String(hashBytes);
            }
        }

        /// <summary>
        /// Weryfikuje, czy podane hasło pasuje do zapisanego hasha.
        /// </summary>
        /// <param name="password">Hasło do sprawdzenia.</param>
        /// <param name="hashedPassword">Hash z bazy danych.</param>
        /// <returns>True, jeśli hasła są zgodne.</returns>
        public static bool VerifyPassword(string password, string hashedPassword)
        {
            if (string.IsNullOrWhiteSpace(hashedPassword))
            {
                return false;
            }

            try
            {
                byte[] hashBytes = Convert.FromBase64String(hashedPassword);

                if (hashBytes.Length != SaltSize + HashSize)
                {
                    // Jeśli format jest nieprawidłowy, dla bezpieczeństwa można spróbować porównania jako plain text (jeśli masz takie stare hasła w bazie)
                    return password == hashedPassword;
                }

                byte[] salt = new byte[SaltSize];
                Array.Copy(hashBytes, 0, salt, 0, SaltSize);

                using (var pbkdf2 = new Rfc2898DeriveBytes(password, salt, Iterations, HashAlgorithmName.SHA1))
                {
                    byte[] hash = pbkdf2.GetBytes(HashSize);

                    // Porównanie odporne na ataki czasowe (timing attacks)
                    int diff = 0;
                    for (int i = 0; i < HashSize; i++)
                    {
                        diff |= (hashBytes[i + SaltSize] ^ hash[i]);
                    }
                    return diff == 0;
                }
            }
            catch
            {
                // Jeśli hash nie jest w formacie Base64, spróbuj porównania jako plain text
                return password == hashedPassword;
            }
        }
    }
}