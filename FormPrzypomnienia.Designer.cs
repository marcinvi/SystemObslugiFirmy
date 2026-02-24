namespace Reklamacje_Dane
{
    partial class FormPrzypomnienia
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.panelTop = new System.Windows.Forms.Panel();
            this.btnDodajNowe = new System.Windows.Forms.Button();
            this.btnOdswiez = new System.Windows.Forms.Button();
            this.cmbFiltr = new System.Windows.Forms.ComboBox();
            this.label1 = new System.Windows.Forms.Label();
            this.flowLayoutZadania = new System.Windows.Forms.FlowLayoutPanel();
            this.panelTop.SuspendLayout();
            this.SuspendLayout();
            // 
            // panelTop
            // 
            this.panelTop.BackColor = System.Drawing.Color.White;
            this.panelTop.Controls.Add(this.btnDodajNowe);
            this.panelTop.Controls.Add(this.btnOdswiez);
            this.panelTop.Controls.Add(this.cmbFiltr);
            this.panelTop.Controls.Add(this.label1);
            this.panelTop.Dock = System.Windows.Forms.DockStyle.Top;
            this.panelTop.Location = new System.Drawing.Point(0, 0);
            this.panelTop.Name = "panelTop";
            this.panelTop.Size = new System.Drawing.Size(984, 70);
            this.panelTop.TabIndex = 0;
            // 
            // btnDodajNowe
            // 
            this.btnDodajNowe.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btnDodajNowe.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(21)))), ((int)(((byte)(101)))), ((int)(((byte)(192)))));
            this.btnDodajNowe.Cursor = System.Windows.Forms.Cursors.Hand;
            this.btnDodajNowe.FlatAppearance.BorderSize = 0;
            this.btnDodajNowe.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnDodajNowe.Font = new System.Drawing.Font("Segoe UI", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(238)));
            this.btnDodajNowe.ForeColor = System.Drawing.Color.White;
            this.btnDodajNowe.Location = new System.Drawing.Point(798, 17);
            this.btnDodajNowe.Name = "btnDodajNowe";
            this.btnDodajNowe.Size = new System.Drawing.Size(164, 38);
            this.btnDodajNowe.TabIndex = 3;
            this.btnDodajNowe.Text = "+ Dodaj Zadanie";
            this.btnDodajNowe.UseVisualStyleBackColor = false;
            // 
            // btnOdswiez
            // 
            this.btnOdswiez.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btnOdswiez.BackColor = System.Drawing.Color.WhiteSmoke;
            this.btnOdswiez.Cursor = System.Windows.Forms.Cursors.Hand;
            this.btnOdswiez.FlatAppearance.BorderSize = 0;
            this.btnOdswiez.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnOdswiez.Font = new System.Drawing.Font("Segoe UI", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(238)));
            this.btnOdswiez.ForeColor = System.Drawing.Color.Black;
            this.btnOdswiez.Location = new System.Drawing.Point(673, 17);
            this.btnOdswiez.Name = "btnOdswiez";
            this.btnOdswiez.Size = new System.Drawing.Size(119, 38);
            this.btnOdswiez.TabIndex = 2;
            this.btnOdswiez.Text = "🔄 Odśwież";
            this.btnOdswiez.UseVisualStyleBackColor = false;
            // 
            // cmbFiltr
            // 
            this.cmbFiltr.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmbFiltr.Font = new System.Drawing.Font("Segoe UI", 11.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(238)));
            this.cmbFiltr.FormattingEnabled = true;
            this.cmbFiltr.Location = new System.Drawing.Point(100, 20);
            this.cmbFiltr.Name = "cmbFiltr";
            this.cmbFiltr.Size = new System.Drawing.Size(250, 33);
            this.cmbFiltr.TabIndex = 1;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Font = new System.Drawing.Font("Segoe UI Semibold", 11.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(238)));
            this.label1.Location = new System.Drawing.Point(21, 25);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(73, 25);
            this.label1.TabIndex = 0;
            this.label1.Text = "Widok:";
            // 
            // flowLayoutZadania
            // 
            this.flowLayoutZadania.AutoScroll = true;
            this.flowLayoutZadania.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(240)))), ((int)(((byte)(242)))), ((int)(((byte)(245)))));
            this.flowLayoutZadania.Dock = System.Windows.Forms.DockStyle.Fill;
            this.flowLayoutZadania.FlowDirection = System.Windows.Forms.FlowDirection.TopDown;
            this.flowLayoutZadania.Location = new System.Drawing.Point(0, 70);
            this.flowLayoutZadania.Name = "flowLayoutZadania";
            this.flowLayoutZadania.Padding = new System.Windows.Forms.Padding(0, 10, 0, 10);
            this.flowLayoutZadania.Size = new System.Drawing.Size(984, 541);
            this.flowLayoutZadania.TabIndex = 1;
            this.flowLayoutZadania.WrapContents = false;
            // 
            // FormPrzypomnienia
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 20F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(984, 611);
            this.Controls.Add(this.flowLayoutZadania);
            this.Controls.Add(this.panelTop);
            this.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(238)));
            this.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this.Name = "FormPrzypomnienia";
            this.ShowIcon = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Lista Zadań / Przypomnień";
            this.panelTop.ResumeLayout(false);
            this.panelTop.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Panel panelTop;
        private System.Windows.Forms.Button btnDodajNowe;
        private System.Windows.Forms.Button btnOdswiez;
        private System.Windows.Forms.ComboBox cmbFiltr;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.FlowLayoutPanel flowLayoutZadania;
    }
}