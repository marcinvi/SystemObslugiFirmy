using System.Security.Cryptography;
using System.Text;

namespace ReklamacjeAPI.Security;

public static class EncryptionHelper
{
    public static byte[]? MasterKey { get; set; }

    public static readonly byte[] Salt =
    [
        0x1A, 0x2B, 0x3C, 0x4D, 0x5E, 0x6F, 0x7A, 0x8B,
        0x9C, 0xAD, 0xBE, 0xCF, 0xD1, 0xE2, 0xF3, 0x04
    ];

    public static string? EncryptString(string? plainText)
    {
        if (string.IsNullOrEmpty(plainText))
        {
            return null;
        }

        if (MasterKey == null || MasterKey.Length != 32)
        {
            throw new InvalidOperationException("Klucz główny (MasterKey) jest nieustawiony lub ma nieprawidłową długość.");
        }

        using var aes = Aes.Create();
        aes.Key = MasterKey;
        aes.GenerateIV();
        var iv = aes.IV;

        using var memoryStream = new MemoryStream();
        memoryStream.Write(iv, 0, iv.Length);

        using (var cryptoStream = new CryptoStream(memoryStream, aes.CreateEncryptor(), CryptoStreamMode.Write))
        {
            var plaintextBytes = Encoding.UTF8.GetBytes(plainText);
            cryptoStream.Write(plaintextBytes, 0, plaintextBytes.Length);
            cryptoStream.FlushFinalBlock();
        }

        return Convert.ToBase64String(memoryStream.ToArray());
    }

    public static string? DecryptString(string? encryptedText)
    {
        if (string.IsNullOrEmpty(encryptedText))
        {
            return null;
        }

        if (MasterKey == null || MasterKey.Length != 32)
        {
            throw new InvalidOperationException("Klucz główny (MasterKey) jest nieustawiony lub ma nieprawidłową długość.");
        }

        try
        {
            var encryptedData = Convert.FromBase64String(encryptedText);
            using var aes = Aes.Create();
            aes.Key = MasterKey;

            var ivLength = aes.BlockSize / 8;
            if (encryptedData.Length < ivLength)
            {
                return null;
            }

            var iv = new byte[ivLength];
            Array.Copy(encryptedData, 0, iv, 0, ivLength);
            aes.IV = iv;

            using var memoryStream = new MemoryStream();
            using (var cryptoStream = new CryptoStream(memoryStream, aes.CreateDecryptor(), CryptoStreamMode.Write))
            {
                cryptoStream.Write(encryptedData, ivLength, encryptedData.Length - ivLength);
                cryptoStream.FlushFinalBlock();
            }

            return Encoding.UTF8.GetString(memoryStream.ToArray());
        }
        catch
        {
            return null;
        }
    }
}
