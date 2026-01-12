namespace Reklamacje_Dane
{
    partial class FormZglosNieobecnosc
    {
        private System.ComponentModel.IContainer components = null;
        protected override void Dispose(bool disposing) { if (disposing && (components != null)) { components.Dispose(); } base.Dispose(disposing); }
        #region Windows Form Designer generated code
        private void InitializeComponent()
        {
            this.label1 = new System.Windows.Forms.Label();
            this.dtpDataOd = new System.Windows.Forms.DateTimePicker();
            this.dtpDataDo = new System.Windows.Forms.DateTimePicker();
            this.label2 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.comboZastepca = new System.Windows.Forms.ComboBox();
            this.btnOk = new System.Windows.Forms.Button();
            this.btnCancel = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(13, 13);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(62, 17);
            this.label1.TabIndex = 0;
            this.label1.Text = "Data od:";
            // 
            // dtpDataOd
            // 
            this.dtpDataOd.Location = new System.Drawing.Point(16, 34);
            this.dtpDataOd.Name = "dtpDataOd";
            this.dtpDataOd.Size = new System.Drawing.Size(200, 22);
            this.dtpDataOd.TabIndex = 1;
            // 
            // dtpDataDo
            // 
            this.dtpDataDo.Location = new System.Drawing.Point(236, 34);
            this.dtpDataDo.Name = "dtpDataDo";
            this.dtpDataDo.Size = new System.Drawing.Size(200, 22);
            this.dtpDataDo.TabIndex = 3;
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(233, 13);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(62, 17);
            this.label2.TabIndex = 2;
            this.label2.Text = "Data do:";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(13, 75);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(217, 17);
            this.label3.TabIndex = 4;
            this.label3.Text = "Osoba zastępująca (na delegacji):";
            // 
            // comboZastepca
            // 
            this.comboZastepca.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.comboZastepca.FormattingEnabled = true;
            this.comboZastepca.Location = new System.Drawing.Point(16, 96);
            this.comboZastepca.Name = "comboZastepca";
            this.comboZastepca.Size = new System.Drawing.Size(420, 24);
            this.comboZastepca.TabIndex = 5;
            // 
            // btnOk
            // 
            this.btnOk.Location = new System.Drawing.Point(226, 140);
            this.btnOk.Name = "btnOk";
            this.btnOk.Size = new System.Drawing.Size(100, 30);
            this.btnOk.TabIndex = 6;
            this.btnOk.Text = "Zapisz";
            this.btnOk.UseVisualStyleBackColor = true;
            this.btnOk.Click += new System.EventHandler(this.btnOk_Click);
            // 
            // btnCancel
            // 
            this.btnCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.btnCancel.Location = new System.Drawing.Point(332, 140);
            this.btnCancel.Name = "btnCancel";
            this.btnCancel.Size = new System.Drawing.Size(100, 30);
            this.btnCancel.TabIndex = 7;
            this.btnCancel.Text = "Anuluj";
            this.btnCancel.UseVisualStyleBackColor = true;
            // 
            // FormZglosNieobecnosc
            // 
            this.AcceptButton = this.btnOk;
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.CancelButton = this.btnCancel;
            this.ClientSize = new System.Drawing.Size(452, 183);
            this.Controls.Add(this.btnCancel);
            this.Controls.Add(this.btnOk);
            this.Controls.Add(this.comboZastepca);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.dtpDataDo);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.dtpDataOd);
            this.Controls.Add(this.label1);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "FormZglosNieobecnosc";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "Zgłoś Urlop / Nieobecność";
            this.Load += new System.EventHandler(this.FormZglosNieobecnosc_Load);
            this.ResumeLayout(false);
            this.PerformLayout();
        }
        #endregion
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.DateTimePicker dtpDataOd;
        private System.Windows.Forms.DateTimePicker dtpDataDo;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.ComboBox comboZastepca;
        private System.Windows.Forms.Button btnOk;
        private System.Windows.Forms.Button btnCancel;
    }
}