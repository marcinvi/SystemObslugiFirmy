using System;
using MySql.Data.MySqlClient;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace Reklamacje_Dane
{
    public static class SecureSettingsService
    {
        private static byte[] Entropy => Encoding.UTF8.GetBytes("ReklamacjeDane-Entropy-v1");

        public static async Task<string> GetDecryptedSettingAsync(string key)
        {
            if (string.IsNullOrWhiteSpace(key)) return null;

            using (var con = DatabaseHelper.GetConnection()) // Baza.db
            {
                await con.OpenAsync();
                using (var cmd = new MySqlCommand("SELECT WartoscZaszyfrowana FROM Ustawienia WHERE Klucz=@k LIMIT 1;", con))
                {
                    cmd.Parameters.AddWithValue("@k", key);
                    var obj = await cmd.ExecuteScalarAsync();
                    if (obj == null || obj == DBNull.Value) return null;

                    try
                    {
                        var b64 = Convert.ToString(obj);
                        var cipher = Convert.FromBase64String(b64);
                        var plain = ProtectedData.Unprotect(cipher, Entropy, DataProtectionScope.CurrentUser);
                        return Encoding.UTF8.GetString(plain);
                    }
                    catch { return null; }
                }
            }
        }

        public static async Task SetEncryptedSettingAsync(string key, string value)
        {
            if (string.IsNullOrWhiteSpace(key)) return;

            var plain = Encoding.UTF8.GetBytes(value ?? "");
            var cipher = ProtectedData.Protect(plain, Entropy, DataProtectionScope.CurrentUser);
            var b64 = Convert.ToBase64String(cipher);

            using (var con = DatabaseHelper.GetConnection()) // Baza.db
            {
                await con.OpenAsync();

                // UPDATE -> jeśli nic nie zmieniono, INSERT
                using (var upd = new MySqlCommand("UPDATE Ustawienia SET WartoscZaszyfrowana=@v WHERE Klucz=@k;", con))
                {
                    upd.Parameters.AddWithValue("@v", b64);
                    upd.Parameters.AddWithValue("@k", key);
                    var changed = await upd.ExecuteNonQueryAsync();

                    if (changed == 0)
                    {
                        using (var ins = new MySqlCommand("INSERT INTO Ustawienia (Klucz, WartoscZaszyfrowana) VALUES (@k, @v);", con))
                        {
                            ins.Parameters.AddWithValue("@k", key);
                            ins.Parameters.AddWithValue("@v", b64);
                            await ins.ExecuteNonQueryAsync();
                        }
                    }
                }
            }
        }
    }
}
