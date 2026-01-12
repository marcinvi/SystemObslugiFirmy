using System;
using System.Collections.Generic;
using System.Data;
using System.Windows.Forms;

namespace Reklamacje_Dane
{
    partial class AddEntryForm
    {
        /// <summary>
        /// Metoda wymagana do obsługi projektanta — nie modyfikuj jej
        /// zawartości w edytorze kodu.
        /// </summary>
        private void InitializeComponent()
        {
            this.SuspendLayout();
            // 
            // AddEntryForm
            // 
            this.ClientSize = new System.Drawing.Size(400, 400);
            this.Name = "AddEntryForm";
            this.Text = "Dodaj zgłoszenie";
            this.Load += new System.EventHandler(this.AddEntryForm_Load);
            this.ResumeLayout(false);
        }

        private void AddEntryForm_Load(object sender, System.EventArgs e)
        {
            // Metoda Load pozostaje pusta, bo cała logika jest w konstruktorze i metodzie GenerateControls
        }
    }
}