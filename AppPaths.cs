using System;
using System.IO;
using MySql.Data.MySqlClient;

namespace Reklamacje_Dane
{
    public static class AppPaths
    {
        private static readonly object _lock = new object();
        private static bool _loaded;
        private static string _dataRootPath;

        public static string GetDataRootPath()
        {
            lock (_lock)
            {
                if (!_loaded)
                {
                    _dataRootPath = LoadDataRootPath() ?? GetDefaultDataRootPath();
                    _loaded = true;
                }

                return _dataRootPath;
            }
        }

        public static void SetDataRootPath(string path)
        {
            lock (_lock)
            {
                _dataRootPath = string.IsNullOrWhiteSpace(path) ? GetDefaultDataRootPath() : path;
                _loaded = true;
            }
        }

        private static string LoadDataRootPath()
        {
            try
            {
                using (var con = DatabaseHelper.GetConnection())
                {
                    con.Open();
                    using (var cmd = new MySqlCommand("SELECT WartoscZaszyfrowana FROM Ustawienia WHERE Klucz = @k", con))
                    {
                        cmd.Parameters.AddWithValue("@k", "DataFolderPath");
                        var value = cmd.ExecuteScalar()?.ToString();
                        if (string.IsNullOrWhiteSpace(value))
                        {
                            return null;
                        }

                        if (!Path.IsPathRooted(value))
                        {
                            value = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, value);
                        }

                        return value;
                    }
                }
            }
            catch
            {
                return null;
            }
        }

        private static string GetDefaultDataRootPath()
        {
            return Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Dane");
        }
    }
}
