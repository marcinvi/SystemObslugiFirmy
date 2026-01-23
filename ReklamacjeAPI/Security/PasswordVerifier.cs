using System.Security.Cryptography;
using System.Text;

namespace ReklamacjeAPI.Security;

public static class PasswordVerifier
{
    public static bool Verify(string password, string storedHash)
    {
        if (string.IsNullOrWhiteSpace(storedHash))
        {
            return false;
        }

        var candidate = ExtractHashCandidate(storedHash);

        if (LooksLikeBcrypt(candidate))
        {
            try
            {
                return BCrypt.Net.BCrypt.Verify(password, candidate);
            }
            catch (BCrypt.Net.SaltParseException)
            {
                // Fallback to legacy formats below
            }
        }

        if (TryVerifyPbkdf2(password, candidate))
        {
            return true;
        }

        if (TryVerifySha256(password, candidate))
        {
            return true;
        }

        return string.Equals(candidate, password, StringComparison.Ordinal);
    }

    private static string ExtractHashCandidate(string storedHash)
    {
        var trimmed = storedHash.Trim();
        var spaceIndex = trimmed.LastIndexOf(' ');
        return spaceIndex >= 0 && spaceIndex < trimmed.Length - 1
            ? trimmed[(spaceIndex + 1)..]
            : trimmed;
    }

    private static bool LooksLikeBcrypt(string candidate)
    {
        return candidate.StartsWith("$2a$", StringComparison.Ordinal)
            || candidate.StartsWith("$2b$", StringComparison.Ordinal)
            || candidate.StartsWith("$2y$", StringComparison.Ordinal);
    }

    private static bool TryVerifyPbkdf2(string password, string candidate)
    {
        try
        {
            var hashBytes = Convert.FromBase64String(candidate);
            if (hashBytes.Length < 20)
            {
                return false;
            }

            var salt = new byte[16];
            Buffer.BlockCopy(hashBytes, 0, salt, 0, 16);

            using var pbkdf2 = new Rfc2898DeriveBytes(password, salt, 10000);
            var hash = pbkdf2.GetBytes(hashBytes.Length - 16);
            for (var i = 0; i < hash.Length; i++)
            {
                if (hashBytes[i + 16] != hash[i])
                {
                    return false;
                }
            }
            return true;
        }
        catch (FormatException)
        {
            return false;
        }
    }

    private static bool TryVerifySha256(string password, string candidate)
    {
        try
        {
            var hashBytes = Convert.FromBase64String(candidate);
            if (hashBytes.Length == 32)
            {
                using var sha = SHA256.Create();
                var computed = sha.ComputeHash(Encoding.UTF8.GetBytes(password));
                return SlowEquals(hashBytes, computed);
            }
        }
        catch (FormatException)
        {
            // Fall through to hex support
        }

        if (candidate.Length == 64 && IsHex(candidate))
        {
            var raw = HexToBytes(candidate);
            using var sha = SHA256.Create();
            var computed = sha.ComputeHash(Encoding.UTF8.GetBytes(password));
            return SlowEquals(raw, computed);
        }

        return false;
    }

    private static bool IsHex(string value)
    {
        foreach (var c in value)
        {
            if (!((c >= '0' && c <= '9') || (c >= 'a' && c <= 'f') || (c >= 'A' && c <= 'F')))
            {
                return false;
            }
        }
        return true;
    }

    private static byte[] HexToBytes(string value)
    {
        var len = value.Length / 2;
        var bytes = new byte[len];
        for (var i = 0; i < len; i++)
        {
            bytes[i] = Convert.ToByte(value.Substring(i * 2, 2), 16);
        }
        return bytes;
    }

    private static bool SlowEquals(byte[] left, byte[] right)
    {
        if (left.Length != right.Length)
        {
            return false;
        }

        var diff = 0;
        for (var i = 0; i < left.Length; i++)
        {
            diff |= left[i] ^ right[i];
        }
        return diff == 0;
    }
}
