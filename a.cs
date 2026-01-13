using System;
using System.Data;
using System.Globalization;
using Dapper;
using MySql.Data.Types;

namespace Reklamacje_Dane
{
    /// <summary>
    /// Dapper nie mapuje automatycznie MySqlDateTime -> DateTime?
    /// Ten handler robi konwersjê i ogarnia te¿ puste / '-' / niewa¿ne daty.
    /// </summary>
    public sealed class MySqlDateTimeNullableHandler : SqlMapper.TypeHandler<DateTime?>
    {
        public override void SetValue(IDbDataParameter parameter, DateTime? value)
            => parameter.Value = value ?? (object)DBNull.Value;

        public override DateTime? Parse(object value)
        {
            if (value == null || value is DBNull) return null;

            if (value is DateTime dt) return dt;

            if (value is MySqlDateTime mdt)
            {
                if (!mdt.IsValidDateTime) return null;
                return mdt.GetDateTime();
            }

            if (value is string s)
            {
                s = s.Trim();
                if (string.IsNullOrWhiteSpace(s) || s == "-") return null;

                // czêsto spotykane formaty w PL
                var pl = CultureInfo.GetCultureInfo("pl-PL");
                if (DateTime.TryParse(s, pl, DateTimeStyles.None, out var parsed)) return parsed;
                if (DateTime.TryParse(s, CultureInfo.InvariantCulture, DateTimeStyles.None, out parsed)) return parsed;
            }

            // ostatnia deska ratunku
            return Convert.ToDateTime(value, CultureInfo.InvariantCulture);
        }
    }

    public static class DapperTypeHandlerBootstrap
    {
        private static bool _registered;
        private static readonly object _lock = new object();

        public static void EnsureRegistered()
        {
            if (_registered) return;

            lock (_lock)
            {
                if (_registered) return;

                // Rejestrujemy handler dla DateTime?
                SqlMapper.AddTypeHandler(new MySqlDateTimeNullableHandler());

                _registered = true;
            }
        }
    }
}
