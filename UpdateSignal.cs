// ############################################################################
// Plik: UpdateSignal.cs (NOWY - System Semafora Plikowego)
// ############################################################################

using System;
using System.IO;
using System.Threading.Tasks;

namespace Reklamacje_Dane
{
    public static class UpdateSignal
    {
        private static DateTime _lastKnownUpdate = DateTime.MinValue;

        /// <summary>
        /// Daje sygnał innym komputerom: "Zmieniłem coś w bazie!".
        /// Zapisuje aktualną datę do pliku txt na dysku sieciowym.
        /// </summary>
        public static void Touch()
        {
            try
            {
                string path = DbConfig.SignalFilePath;
                // Wystarczy nadpisać plik czymkolwiek, np. datą.
                // To zmienia "LastWriteTime" pliku, co jest dla nas sygnałem.
                File.WriteAllText(path, DateTime.Now.ToString("O"));
            }
            catch (Exception ex)
            {
                // Jeśli nie uda się dotknąć pliku sygnałowego (np. sieć padła),
                // to trudno - program i tak zadziała, tylko inni nie odświeżą się automatycznie.
                Console.WriteLine("Błąd aktualizacji sygnału: " + ex.Message);
            }
        }

        /// <summary>
        /// Sprawdza, czy ktoś inny zmodyfikował bazę (czy plik txt jest nowszy niż myślimy).
        /// </summary>
        public static bool IsUpdateNeeded()
        {
            try
            {
                string path = DbConfig.SignalFilePath;
                if (!File.Exists(path)) return false; // Brak pliku = brak zmian

                DateTime fileTime = File.GetLastWriteTime(path);

                // Jeśli czas pliku jest nowszy niż to co zapamiętaliśmy...
                if (fileTime > _lastKnownUpdate)
                {
                    _lastKnownUpdate = fileTime;
                    return true; // ... to znaczy że trzeba przeładować dane!
                }
            }
            catch { /* Błąd sieci - bezpiecznie zakładamy, że nie trzeba odświeżać */ }

            return false;
        }
    }
}