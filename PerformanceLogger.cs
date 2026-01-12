using System;
using System.Diagnostics;
using System.IO;
using System.Threading;

namespace Reklamacje_Dane
{
    /// <summary>
    /// Logger do monitorowania wydajności zapytań bazodanowych
    /// </summary>
    public sealed class PerformanceLogger
    {
        private static readonly Lazy<PerformanceLogger> _instance =
            new Lazy<PerformanceLogger>(() => new PerformanceLogger(), LazyThreadSafetyMode.ExecutionAndPublication);

        private readonly string _logFilePath;
        private readonly object _lockObject = new object();
        private readonly bool _isEnabled;

        // Progi logowania (w milisekundach)
        private const int SlowQueryThresholdMs = 1000;    // Zapytania wolniejsze niż 1s
        private const int WarningQueryThresholdMs = 500;  // Ostrzeżenie dla zapytań > 500ms

        public static PerformanceLogger Instance => _instance.Value;

        private PerformanceLogger()
        {
            // Sprawdź, czy logowanie jest włączone (można kontrolować przez App.config)
            _isEnabled = System.Configuration.ConfigurationManager.AppSettings["EnablePerformanceLogging"] == "true";

            if (_isEnabled)
            {
                string logDir = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Logs");
                if (!Directory.Exists(logDir))
                {
                    Directory.CreateDirectory(logDir);
                }

                _logFilePath = Path.Combine(logDir, $"performance_{DateTime.Now:yyyyMMdd}.log");
            }
        }

        /// <summary>
        /// Loguje zapytanie do bazy danych wraz z czasem wykonania
        /// </summary>
        public void LogQuery(string query, long executionTimeMs, bool isSuccess, string errorMessage = null)
        {
            if (!_isEnabled) return;

            try
            {
                // Loguj tylko wolne zapytania lub błędy
                if (executionTimeMs < WarningQueryThresholdMs && isSuccess) return;

                string level = "INFO";
                if (!isSuccess)
                    level = "ERROR";
                else if (executionTimeMs >= SlowQueryThresholdMs)
                    level = "SLOW";
                else if (executionTimeMs >= WarningQueryThresholdMs)
                    level = "WARNING";

                string sanitizedQuery = SanitizeQuery(query);
                string logEntry = $"{DateTime.Now:yyyy-MM-dd HH:mm:ss.fff} | {level} | {executionTimeMs}ms | {sanitizedQuery}";

                if (!isSuccess && !string.IsNullOrEmpty(errorMessage))
                {
                    logEntry += $" | ERROR: {errorMessage}";
                }

                lock (_lockObject)
                {
                    File.AppendAllText(_logFilePath, logEntry + Environment.NewLine);
                }
            }
            catch
            {
                // Ignoruj błędy logowania - nie przerywaj działania aplikacji
            }
        }

        /// <summary>
        /// Czyści query z wartości parametrów dla bezpieczeństwa
        /// </summary>
        private string SanitizeQuery(string query)
        {
            if (string.IsNullOrEmpty(query)) return "";

            // Usuń wielokrotne spacje i znaki nowej linii
            query = System.Text.RegularExpressions.Regex.Replace(query, @"\s+", " ");

            // Ogranicz długość
            if (query.Length > 500)
            {
                query = query.Substring(0, 500) + "...";
            }

            return query.Trim();
        }

        /// <summary>
        /// Pomocnicza klasa do mierzenia czasu wykonania
        /// </summary>
        public QueryTimer StartTimer(string query)
        {
            return new QueryTimer(query, this);
        }

        /// <summary>
        /// Klasa pomocnicza do automatycznego logowania czasu wykonania
        /// Użycie: using (var timer = PerformanceLogger.Instance.StartTimer(query)) { ... }
        /// </summary>
        public sealed class QueryTimer : IDisposable
        {
            private readonly Stopwatch _stopwatch;
            private readonly string _query;
            private readonly PerformanceLogger _logger;
            private bool _isDisposed;

            public QueryTimer(string query, PerformanceLogger logger)
            {
                _query = query;
                _logger = logger;
                _stopwatch = Stopwatch.StartNew();
            }

            public void Dispose()
            {
                if (_isDisposed) return;

                _stopwatch.Stop();
                _logger.LogQuery(_query, _stopwatch.ElapsedMilliseconds, isSuccess: true);
                _isDisposed = true;
            }

            public void MarkError(string errorMessage)
            {
                if (_isDisposed) return;

                _stopwatch.Stop();
                _logger.LogQuery(_query, _stopwatch.ElapsedMilliseconds, isSuccess: false, errorMessage: errorMessage);
                _isDisposed = true;
            }
        }

        /// <summary>
        /// Zwraca statystyki wydajności (dla dashboardu admina)
        /// </summary>
        public PerformanceStats GetTodayStats()
        {
            if (!_isEnabled || !File.Exists(_logFilePath))
                return new PerformanceStats();

            var stats = new PerformanceStats();

            try
            {
                lock (_lockObject)
                {
                    string[] lines = File.ReadAllLines(_logFilePath);
                    foreach (string line in lines)
                    {
                        stats.TotalQueries++;

                        if (line.Contains("| SLOW |"))
                            stats.SlowQueries++;
                        else if (line.Contains("| WARNING |"))
                            stats.WarningQueries++;
                        else if (line.Contains("| ERROR |"))
                            stats.FailedQueries++;
                    }
                }
            }
            catch
            {
                // Ignoruj błędy odczytu
            }

            return stats;
        }
    }

    /// <summary>
    /// Statystyki wydajności zapytań
    /// </summary>
    public class PerformanceStats
    {
        public int TotalQueries { get; set; }
        public int SlowQueries { get; set; }
        public int WarningQueries { get; set; }
        public int FailedQueries { get; set; }

        public double SlowQueryPercentage => TotalQueries > 0 ? (SlowQueries * 100.0 / TotalQueries) : 0;
    }
}
