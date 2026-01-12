using System;

namespace Reklamacje_Dane
{
    public class SzablonEmail
    {
        public int Id { get; set; }
        public string Nazwa { get; set; }
        public string TrescRtf { get; set; }
        public bool Aktywny { get; set; } = true;

        // Nadpisanie ToString pozwala łatwo wyświetlać nazwę w ListBox/ComboBox
        public override string ToString()
        {
            return Nazwa;
        }
    }
}