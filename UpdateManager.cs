using System;

namespace Reklamacje_Dane
{
    /// <summary>
    /// Statyczna klasa do zarządzania globalnymi zdarzeniami w aplikacji,
    /// np. do powiadamiania głównego formularza o konieczności odświeżenia danych.
    /// </summary>
    public static class UpdateManager
    {
        public static event Action OnUpdateNeeded;

        /// <summary>
        /// Metoda, którą można wywołać z dowolnego miejsca w aplikacji,
        /// aby powiadomić wszystkich subskrybentów o potrzebie aktualizacji.
        /// </summary>
        public static void NotifySubscribers()
        {
            OnUpdateNeeded?.Invoke();
        }
    }
}