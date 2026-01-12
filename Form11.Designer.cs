// Plik: Form11.Designer.cs
namespace Reklamacje_Dane
{
    partial class Form11
    {
        private System.ComponentModel.IContainer components = null;

        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        private void InitializeComponent()
        {
            this.label1 = new System.Windows.Forms.Label();
            this.statusProducentaComboBox = new System.Windows.Forms.ComboBox();
            this.zatwierdzButton = new System.Windows.Forms.Button();
            this.panelNowyProdukt = new System.Windows.Forms.Panel();
            this.textBoxKwz2 = new System.Windows.Forms.TextBox();
            this.labelKwz2 = new System.Windows.Forms.Label();
            this.towarDostarczonyCheckBox = new System.Windows.Forms.CheckBox();
            this.panelNota = new System.Windows.Forms.Panel();
            this.notaRozliczonaCheckBox = new System.Windows.Forms.CheckBox();
            this.textBoxKpzn = new System.Windows.Forms.TextBox();
            this.labelKpzn = new System.Windows.Forms.Label();
            this.textBoxKwz2Nota = new System.Windows.Forms.TextBox();
            this.labelKwz2Nota = new System.Windows.Forms.Label();
            this.textBoxNumerNoty = new System.Windows.Forms.TextBox();
            this.labelNumerNoty = new System.Windows.Forms.Label();
            this.notaWystawionaCheckBox = new System.Windows.Forms.CheckBox();
            this.panelNaprawa = new System.Windows.Forms.Panel();
            this.textBoxNrwrl = new System.Windows.Forms.TextBox();
            this.labelNrwrl = new System.Windows.Forms.Label();
            this.panelNowyProdukt.SuspendLayout();
            this.panelNota.SuspendLayout();
            this.panelNaprawa.SuspendLayout();
            this.SuspendLayout();
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Font = new System.Drawing.Font("Segoe UI Semibold", 9F, System.Drawing.FontStyle.Bold);
            this.label1.Location = new System.Drawing.Point(28, 25);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(142, 20);
            this.label1.TabIndex = 0;
            this.label1.Text = "Status u producenta:";
            // 
            // statusProducentaComboBox
            // 
            this.statusProducentaComboBox.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.statusProducentaComboBox.Font = new System.Drawing.Font("Segoe UI", 9F);
            this.statusProducentaComboBox.FormattingEnabled = true;
            this.statusProducentaComboBox.Location = new System.Drawing.Point(32, 48);
            this.statusProducentaComboBox.Name = "statusProducentaComboBox";
            this.statusProducentaComboBox.Size = new System.Drawing.Size(430, 28);
            this.statusProducentaComboBox.TabIndex = 1;
            // 
            // zatwierdzButton
            // 
            this.zatwierdzButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.zatwierdzButton.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(68)))), ((int)(((byte)(114)))), ((int)(((byte)(196)))));
            this.zatwierdzButton.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.zatwierdzButton.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Bold);
            this.zatwierdzButton.ForeColor = System.Drawing.Color.White;
            this.zatwierdzButton.Location = new System.Drawing.Point(185, 304);
            this.zatwierdzButton.Name = "zatwierdzButton";
            this.zatwierdzButton.Size = new System.Drawing.Size(125, 35);
            this.zatwierdzButton.TabIndex = 10;
            this.zatwierdzButton.Text = "Zatwierdź";
            this.zatwierdzButton.UseVisualStyleBackColor = false;
            this.zatwierdzButton.Click += new System.EventHandler(this.zatwierdzButton_Click);
            // 
            // panelNowyProdukt
            // 
            this.panelNowyProdukt.Controls.Add(this.textBoxKwz2);
            this.panelNowyProdukt.Controls.Add(this.labelKwz2);
            this.panelNowyProdukt.Controls.Add(this.towarDostarczonyCheckBox);
            this.panelNowyProdukt.Location = new System.Drawing.Point(23, 93);
            this.panelNowyProdukt.Name = "panelNowyProdukt";
            this.panelNowyProdukt.Size = new System.Drawing.Size(450, 100);
            this.panelNowyProdukt.TabIndex = 4;
            this.panelNowyProdukt.Visible = false;
            // 
            // textBoxKwz2
            // 
            this.textBoxKwz2.Location = new System.Drawing.Point(9, 66);
            this.textBoxKwz2.Name = "textBoxKwz2";
            this.textBoxKwz2.Size = new System.Drawing.Size(415, 22);
            this.textBoxKwz2.TabIndex = 2;
            this.textBoxKwz2.Visible = false;
            // 
            // labelKwz2
            // 
            this.labelKwz2.AutoSize = true;
            this.labelKwz2.Location = new System.Drawing.Point(6, 46);
            this.labelKwz2.Name = "labelKwz2";
            this.labelKwz2.Size = new System.Drawing.Size(98, 16);
            this.labelKwz2.TabIndex = 1;
            this.labelKwz2.Text = "Numer KWZ 2:";
            this.labelKwz2.Visible = false;
            // 
            // towarDostarczonyCheckBox
            // 
            this.towarDostarczonyCheckBox.AutoSize = true;
            this.towarDostarczonyCheckBox.Location = new System.Drawing.Point(9, 13);
            this.towarDostarczonyCheckBox.Name = "towarDostarczonyCheckBox";
            this.towarDostarczonyCheckBox.Size = new System.Drawing.Size(135, 20);
            this.towarDostarczonyCheckBox.TabIndex = 0;
            this.towarDostarczonyCheckBox.Text = "Towar dostarczony";
            this.towarDostarczonyCheckBox.UseVisualStyleBackColor = true;
            // 
            // panelNota
            // 
            this.panelNota.Controls.Add(this.notaRozliczonaCheckBox);
            this.panelNota.Controls.Add(this.textBoxKpzn);
            this.panelNota.Controls.Add(this.labelKpzn);
            this.panelNota.Controls.Add(this.textBoxKwz2Nota);
            this.panelNota.Controls.Add(this.labelKwz2Nota);
            this.panelNota.Controls.Add(this.textBoxNumerNoty);
            this.panelNota.Controls.Add(this.labelNumerNoty);
            this.panelNota.Controls.Add(this.notaWystawionaCheckBox);
            this.panelNota.Location = new System.Drawing.Point(23, 93);
            this.panelNota.Name = "panelNota";
            this.panelNota.Size = new System.Drawing.Size(450, 205);
            this.panelNota.TabIndex = 5;
            this.panelNota.Visible = false;
            // 
            // notaRozliczonaCheckBox
            // 
            this.notaRozliczonaCheckBox.AutoSize = true;
            this.notaRozliczonaCheckBox.Location = new System.Drawing.Point(9, 178);
            this.notaRozliczonaCheckBox.Name = "notaRozliczonaCheckBox";
            this.notaRozliczonaCheckBox.Size = new System.Drawing.Size(268, 20);
            this.notaRozliczonaCheckBox.TabIndex = 7;
            this.notaRozliczonaCheckBox.Text = "Rozliczono notę z przyszłym zamówieniem";
            this.notaRozliczonaCheckBox.UseVisualStyleBackColor = true;
            // 
            // textBoxKpzn
            // 
            this.textBoxKpzn.Location = new System.Drawing.Point(9, 150);
            this.textBoxKpzn.Name = "textBoxKpzn";
            this.textBoxKpzn.Size = new System.Drawing.Size(415, 22);
            this.textBoxKpzn.TabIndex = 6;
            // 
            // labelKpzn
            // 
            this.labelKpzn.AutoSize = true;
            this.labelKpzn.Location = new System.Drawing.Point(6, 130);
            this.labelKpzn.Name = "labelKpzn";
            this.labelKpzn.Size = new System.Drawing.Size(89, 16);
            this.labelKpzn.TabIndex = 5;
            this.labelKpzn.Text = "Numer KPZN:";
            // 
            // textBoxKwz2Nota
            // 
            this.textBoxKwz2Nota.Location = new System.Drawing.Point(9, 105);
            this.textBoxKwz2Nota.Name = "textBoxKwz2Nota";
            this.textBoxKwz2Nota.Size = new System.Drawing.Size(415, 22);
            this.textBoxKwz2Nota.TabIndex = 4;
            // 
            // labelKwz2Nota
            // 
            this.labelKwz2Nota.AutoSize = true;
            this.labelKwz2Nota.Location = new System.Drawing.Point(6, 85);
            this.labelKwz2Nota.Name = "labelKwz2Nota";
            this.labelKwz2Nota.Size = new System.Drawing.Size(98, 16);
            this.labelKwz2Nota.TabIndex = 3;
            this.labelKwz2Nota.Text = "Numer KWZ 2:";
            // 
            // textBoxNumerNoty
            // 
            this.textBoxNumerNoty.Location = new System.Drawing.Point(9, 60);
            this.textBoxNumerNoty.Name = "textBoxNumerNoty";
            this.textBoxNumerNoty.Size = new System.Drawing.Size(415, 22);
            this.textBoxNumerNoty.TabIndex = 2;
            // 
            // labelNumerNoty
            // 
            this.labelNumerNoty.AutoSize = true;
            this.labelNumerNoty.Location = new System.Drawing.Point(6, 40);
            this.labelNumerNoty.Name = "labelNumerNoty";
            this.labelNumerNoty.Size = new System.Drawing.Size(193, 16);
            this.labelNumerNoty.TabIndex = 1;
            this.labelNumerNoty.Text = "Numer noty korygującej (RMA):";
            // 
            // notaWystawionaCheckBox
            // 
            this.notaWystawionaCheckBox.AutoSize = true;
            this.notaWystawionaCheckBox.Location = new System.Drawing.Point(9, 13);
            this.notaWystawionaCheckBox.Name = "notaWystawionaCheckBox";
            this.notaWystawionaCheckBox.Size = new System.Drawing.Size(147, 20);
            this.notaWystawionaCheckBox.TabIndex = 0;
            this.notaWystawionaCheckBox.Text = "Nota została wystawiona";
            this.notaWystawionaCheckBox.UseVisualStyleBackColor = true;
            // 
            // panelNaprawa
            // 
            this.panelNaprawa.Controls.Add(this.textBoxNrwrl);
            this.panelNaprawa.Controls.Add(this.labelNrwrl);
            this.panelNaprawa.Location = new System.Drawing.Point(23, 93);
            this.panelNaprawa.Name = "panelNaprawa";
            this.panelNaprawa.Size = new System.Drawing.Size(450, 60);
            this.panelNaprawa.TabIndex = 6;
            this.panelNaprawa.Visible = false;
            // 
            // textBoxNrwrl
            // 
            this.textBoxNrwrl.Location = new System.Drawing.Point(9, 30);
            this.textBoxNrwrl.Name = "textBoxNrwrl";
            this.textBoxNrwrl.Size = new System.Drawing.Size(415, 22);
            this.textBoxNrwrl.TabIndex = 1;
            // 
            // labelNrwrl
            // 
            this.labelNrwrl.AutoSize = true;
            this.labelNrwrl.Location = new System.Drawing.Point(6, 10);
            this.labelNrwrl.Name = "labelNrwrl";
            this.labelNrwrl.Size = new System.Drawing.Size(89, 16);
            this.labelNrwrl.TabIndex = 0;
            this.labelNrwrl.Text = "Numer WRL:";
            // 
            // Form11
            // 
            this.AcceptButton = this.zatwierdzButton;
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.White;
            this.ClientSize = new System.Drawing.Size(495, 351);
            this.Controls.Add(this.panelNaprawa);
            this.Controls.Add(this.panelNota);
            this.Controls.Add(this.panelNowyProdukt);
            this.Controls.Add(this.zatwierdzButton);
            this.Controls.Add(this.statusProducentaComboBox);
            this.Controls.Add(this.label1);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "Form11";
            this.Text = "Zmiana statusu u producenta";
            this.Load += new System.EventHandler(this.Form11_Load);
            this.panelNowyProdukt.ResumeLayout(false);
            this.panelNowyProdukt.PerformLayout();
            this.panelNota.ResumeLayout(false);
            this.panelNota.PerformLayout();
            this.panelNaprawa.ResumeLayout(false);
            this.panelNaprawa.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();
        }

        #endregion

        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.ComboBox statusProducentaComboBox;
        private System.Windows.Forms.Button zatwierdzButton;
        private System.Windows.Forms.Panel panelNowyProdukt;
        private System.Windows.Forms.TextBox textBoxKwz2;
        private System.Windows.Forms.Label labelKwz2;
        private System.Windows.Forms.CheckBox towarDostarczonyCheckBox;
        private System.Windows.Forms.Panel panelNota;
        private System.Windows.Forms.CheckBox notaRozliczonaCheckBox;
        private System.Windows.Forms.TextBox textBoxKpzn;
        private System.Windows.Forms.Label labelKpzn;
        private System.Windows.Forms.TextBox textBoxKwz2Nota;
        private System.Windows.Forms.Label labelKwz2Nota;
        private System.Windows.Forms.TextBox textBoxNumerNoty;
        private System.Windows.Forms.Label labelNumerNoty;
        private System.Windows.Forms.CheckBox notaWystawionaCheckBox;
        private System.Windows.Forms.Panel panelNaprawa;
        private System.Windows.Forms.TextBox textBoxNrwrl;
        private System.Windows.Forms.Label labelNrwrl;
    }
}