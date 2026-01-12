// Plik: ToastManager.cs (POPRAWIONY - Dodałem namespace)
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;

namespace Reklamacje_Dane
{
  //  public enum NotificationType { Info, Success, Warning, Error }

    public static class ToastManager
    {
        private static List<FormPowiadomieniaToast> openToasts = new List<FormPowiadomieniaToast>();
        private static object lockObject = new object();

        public static void ShowToast(string title, string message, NotificationType type = NotificationType.Info)
        {
            lock (lockObject)
            {
                // Upewniamy się, że wywołanie jest w wątku UI (bezpieczeństwo wielowątkowe)
                if (Application.OpenForms.Count > 0)
                {
                    var mainForm = Application.OpenForms[0];
                    if (mainForm.InvokeRequired)
                    {
                        mainForm.Invoke(new MethodInvoker(() => ShowToastInternal(title, message, type)));
                        return;
                    }
                }

                ShowToastInternal(title, message, type);
            }
        }

        private static void ShowToastInternal(string title, string message, NotificationType type)
        {
            var toast = new FormPowiadomieniaToast(title, message, type);

            // Ustaw pozycję Y na podstawie już otwartych powiadomień
            int yPos = Screen.PrimaryScreen.WorkingArea.Bottom - toast.Height - 10;
            if (openToasts.Any())
            {
                yPos = openToasts.Min(f => f.Location.Y) - toast.Height - 5;
            }

            toast.Location = new Point(Screen.PrimaryScreen.WorkingArea.Right - toast.Width - 10, yPos);

            // Zarządzanie listą otwartych tostów
            toast.FormClosed += (s, e) =>
            {
                lock (lockObject)
                {
                    openToasts.Remove((FormPowiadomieniaToast)s);
                    RepositionToasts();
                }
            };

            openToasts.Add(toast);
            toast.Show(); // Toast nie blokuje wątku, więc Show() jest ok
        }

        private static void RepositionToasts()
        {
            // Przesuń istniejące tosty w dół, gdy któryś z góry zniknie
            int yPos = Screen.PrimaryScreen.WorkingArea.Bottom;
            // Kopiujemy listę do tablicy, żeby uniknąć błędu modyfikacji kolekcji podczas iteracji
            var toastsSnapshot = openToasts.OrderByDescending(f => f.Location.Y).ToList();

            foreach (var form in toastsSnapshot)
            {
                yPos -= (form.Height + 5);
                form.Location = new Point(form.Location.X, yPos);
            }
        }
    }
}