using System;
using System.Windows.Forms;

namespace Reklamacje_Dane
{
    /// <summary>
    /// Przechowuje informacje o pojedynczym module (przycisku w menu nawigacyjnym).
    /// Wersja uproszczona, bez ikon.
    /// </summary>
    public class ModuleInfo
    {
        public string Title { get; }
        public Type UserControlType { get; }
        public string RequiredRole { get; }

        public ModuleInfo(string title, Type userControlType, string requiredRole = null)
        {
            Title = title;
            UserControlType = userControlType;
            RequiredRole = requiredRole;

            if (!typeof(UserControl).IsAssignableFrom(userControlType))
            {
                throw new ArgumentException("Typ przekazany do ModuleInfo musi dziedziczyć z UserControl.");
            }
        }
    }
}