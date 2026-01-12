using System;
using System.Windows.Forms;

namespace Reklamacje_Dane
{
    public partial class FormMailProgress : Form
    {
        public FormMailProgress()
        {
            InitializeComponent();
        }

        public void Init(int max, string initialStatus)
        {
            progressBar.Minimum = 0;
            progressBar.Maximum = Math.Max(1, max);
            progressBar.Value = 0;
            lblStatus.Text = initialStatus ?? "";
            Refresh();
        }

        public void Step(string status = null)
        {
            if (!string.IsNullOrWhiteSpace(status)) lblStatus.Text = status;
            if (progressBar.Value < progressBar.Maximum) progressBar.Value++;
            Refresh();
        }
    }
}
