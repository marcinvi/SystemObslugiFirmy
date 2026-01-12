namespace Reklamacje_Dane
{
    partial class TimelineItemControl
    {
        /// <summary> 
        /// Wymagana zmienna projektanta.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary> 
        /// Wyczyść wszystkie używane zasoby.
        /// </summary>
        /// <param name="disposing">prawda, jeżeli zarządzane zasoby powinny zostać zlikwidowane; Fałsz w przeciwnym wypadku.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Kod wygenerowany przez Projektanta składników

        /// <summary> 
        /// Metoda wymagana do obsługi projektanta — nie należy modyfikować 
        /// jej zawartości w edytorze kodu.
        /// </summary>
        private void InitializeComponent()
        {
            this.panelLeftBorder = new System.Windows.Forms.Panel();
            this.lblDate = new System.Windows.Forms.Label();
            this.lblHeader = new System.Windows.Forms.Label();
            this.lblContent = new System.Windows.Forms.Label();
            this.SuspendLayout();
            // 
            // panelLeftBorder
            // 
            this.panelLeftBorder.BackColor = System.Drawing.Color.Gray;
            this.panelLeftBorder.Dock = System.Windows.Forms.DockStyle.Left;
            this.panelLeftBorder.Location = new System.Drawing.Point(5, 5);
            this.panelLeftBorder.Name = "panelLeftBorder";
            this.panelLeftBorder.Size = new System.Drawing.Size(5, 70);
            this.panelLeftBorder.TabIndex = 0;
            this.panelLeftBorder.Click += new System.EventHandler(this.Control_Click);
            // 
            // lblDate
            // 
            this.lblDate.AutoSize = true;
            this.lblDate.Font = new System.Drawing.Font("Segoe UI", 8F);
            this.lblDate.ForeColor = System.Drawing.Color.Gray;
            this.lblDate.Location = new System.Drawing.Point(16, 5);
            this.lblDate.Name = "lblDate";
            this.lblDate.Size = new System.Drawing.Size(111, 19);
            this.lblDate.TabIndex = 1;
            this.lblDate.Text = "01.01.2025 12:00";
            this.lblDate.Click += new System.EventHandler(this.Control_Click);
            // 
            // lblHeader
            // 
            this.lblHeader.AutoSize = true;
            this.lblHeader.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Bold);
            this.lblHeader.Location = new System.Drawing.Point(16, 24);
            this.lblHeader.Name = "lblHeader";
            this.lblHeader.Size = new System.Drawing.Size(49, 20);
            this.lblHeader.TabIndex = 2;
            this.lblHeader.Text = "Autor";
            this.lblHeader.Click += new System.EventHandler(this.Control_Click);
            // 
            // lblContent
            // 
            this.lblContent.AutoSize = true;
            this.lblContent.Font = new System.Drawing.Font("Segoe UI", 9F);
            this.lblContent.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(64)))), ((int)(((byte)(64)))), ((int)(((byte)(64)))));
            this.lblContent.Location = new System.Drawing.Point(16, 48);
            this.lblContent.Name = "lblContent";
            this.lblContent.Size = new System.Drawing.Size(95, 20);
            this.lblContent.TabIndex = 3;
            this.lblContent.Text = "Treść dymka";
            this.lblContent.Click += new System.EventHandler(this.Control_Click);
            // 
            // TimelineItemControl
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.White;
            this.Controls.Add(this.lblContent);
            this.Controls.Add(this.lblHeader);
            this.Controls.Add(this.lblDate);
            this.Controls.Add(this.panelLeftBorder);
            this.Name = "TimelineItemControl";
            this.Padding = new System.Windows.Forms.Padding(5);
            this.Size = new System.Drawing.Size(400, 80);
            this.Click += new System.EventHandler(this.Control_Click);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Panel panelLeftBorder;
        private System.Windows.Forms.Label lblDate;
        private System.Windows.Forms.Label lblHeader;
        private System.Windows.Forms.Label lblContent;
    }
}