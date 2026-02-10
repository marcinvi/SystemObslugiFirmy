using System;
using System.Security.Cryptography;
using System.Text;

namespace ReklamacjeAPI.Services;

public static class PasswordCompatibilityHelper
{
    public static string HashForFormsCompatibility(string password)
    {
        if (password == null) throw new ArgumentNullException(nameof(password));

        using var deriveBytes = new Rfc2898DeriveBytes(password, 16, 10000, HashAlgorithmName.SHA1);
        byte[] salt = deriveBytes.Salt;
        byte[] hash = deriveBytes.GetBytes(20);

        byte[] combined = new byte[36];
        Buffer.BlockCopy(salt, 0, combined, 0, 16);
        Buffer.BlockCopy(hash, 0, combined, 16, 20);

        return Convert.ToBase64String(combined);
    }

    public static bool Verify(string enteredPassword, string storedHashedPassword)
    {
        if (string.IsNullOrWhiteSpace(storedHashedPassword)) return false;

        string candidate = storedHashedPassword.Trim();
        int sp = candidate.LastIndexOf(' ');
        if (sp >= 0 && sp < candidate.Length - 1) candidate = candidate.Substring(sp + 1);

        if (candidate.StartsWith("$2", StringComparison.Ordinal))
        {
            return BCrypt.Net.BCrypt.Verify(enteredPassword, candidate);
        }

        try
        {
            byte[] hashBytes = Convert.FromBase64String(candidate);

            if (hashBytes.Length >= 36)
            {
                byte[] salt = new byte[16];
                Buffer.BlockCopy(hashBytes, 0, salt, 0, 16);
                using var pbkdf2 = new Rfc2898DeriveBytes(enteredPassword, salt, 10000, HashAlgorithmName.SHA1);
                byte[] hash = pbkdf2.GetBytes(hashBytes.Length - 16);
                for (int i = 0; i < hash.Length; i++)
                {
                    if (hashBytes[i + 16] != hash[i]) return false;
                }
                return true;
            }

            if (hashBytes.Length == 32)
            {
                using var sha = SHA256.Create();
                byte[] h = sha.ComputeHash(Encoding.UTF8.GetBytes(enteredPassword));
                for (int i = 0; i < 32; i++)
                {
                    if (hashBytes[i] != h[i]) return false;
                }
                return true;
            }
        }
        catch
        {
            // Ignorujemy błędy Base64, przechodzimy dalej.
        }

        if (candidate.Length == 64 && IsHex(candidate))
        {
            byte[] raw = HexToBytes(candidate);
            using var sha = SHA256.Create();
            byte[] h = sha.ComputeHash(Encoding.UTF8.GetBytes(enteredPassword));
            for (int i = 0; i < 32; i++)
            {
                if (raw[i] != h[i]) return false;
            }
            return true;
        }

        return string.Equals(storedHashedPassword, enteredPassword, StringComparison.Ordinal);
    }

    private static bool IsHex(string s)
    {
        foreach (char c in s)
        {
            if (!((c >= '0' && c <= '9') || (c >= 'a' && c <= 'f') || (c >= 'A' && c <= 'F')))
                return false;
        }
        return true;
    }

    private static byte[] HexToBytes(string s)
    {
        int len = s.Length / 2;
        var bytes = new byte[len];
        for (int i = 0; i < len; i++)
        {
            bytes[i] = Convert.ToByte(s.Substring(i * 2, 2), 16);
        }
        return bytes;
    }
}
