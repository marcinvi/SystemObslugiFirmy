using System.Reflection;
using System.Windows.Forms;

namespace Reklamacje_Dane
{
    public static class DataGridViewExtensions
    {
        // Włącza DoubleBuffered dla DataGridView (zmniejsza migotanie i poprawia scroll)
        public static void EnableDoubleBuffer(this DataGridView grid)
        {
            typeof(DataGridView).InvokeMember("DoubleBuffered",
                BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.SetProperty,
                null, grid, new object[] { true });
        }
    }
}
