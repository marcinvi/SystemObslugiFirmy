// AppEvents.cs
using System;

namespace Reklamacje_Dane
{
    public static class AppEvents
    {
        /// <summary>
        /// Gdy zwroty w bazie ulegną zmianie (dodanie, przekazanie, edycja) – powiadom UI.
        /// </summary>
        public static event Action ZwrotyChanged;

        public static void RaiseZwrotyChanged()
        {
            try { ZwrotyChanged?.Invoke(); }
            catch { /* brak – nie chcemy zablokować logiki nawet jeśli ktoś źle obsłuży event */ }
        }
    }
}