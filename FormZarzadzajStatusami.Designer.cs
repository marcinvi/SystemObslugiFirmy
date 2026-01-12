// Plik: FormZarzadzajStatusami.Designer.cs
namespace Reklamacje_Dane
{
    partial class FormZarzadzajStatusami
    {
        private System.ComponentModel.IContainer components = null;
        protected override void Dispose(bool disposing) { if (disposing && (components != null)) { components.Dispose(); } base.Dispose(disposing); }
        #region Windows Form Designer generated code
        private void InitializeComponent()
        {
            this.listBoxStatusy = new System.Windows.Forms.ListBox();
            this.txtNazwaStatusu = new System.Windows.Forms.TextBox();
            this.btnDodaj = new System.Windows.Forms.Button();
            this.btnUsun = new System.Windows.Forms.Button();
            this.label1 = new System.Windows.Forms.Label();
            this.SuspendLayout();
            // 
            // listBoxStatusy
            // 
            this.listBoxStatusy.FormattingEnabled = true;
            this.listBoxStatusy.ItemHeight = 16;
            this.listBoxStatusy.Location = new System.Drawing.Point(12, 12);
            this.listBoxStatusy.Name = "listBoxStatusy";
            this.listBoxStatusy.Size = new System.Drawing.Size(458, 196);
            this.listBoxStatusy.TabIndex = 0;
            this.listBoxStatusy.SelectedIndexChanged += new System.EventHandler(this.listBoxStatusy_SelectedIndexChanged);
            // 
            // txtNazwaStatusu
            // 
            this.txtNazwaStatusu.Location = new System.Drawing.Point(12, 234);
            this.txtNazwaStatusu.Name = "txtNazwaStatusu";
            this.txtNazwaStatusu.Size = new System.Drawing.Size(458, 22);
            this.txtNazwaStatusu.TabIndex = 1;
            // 
            // btnDodaj
            // 
            this.btnDodaj.Location = new System.Drawing.Point(264, 262);
            this.btnDodaj.Name = "btnDodaj";
            this.btnDodaj.Size = new System.Drawing.Size(100, 30);
            this.btnDodaj.TabIndex = 2;
            this.btnDodaj.Text = "Dodaj";
            this.btnDodaj.UseVisualStyleBackColor = true;
            this.btnDodaj.Click += new System.EventHandler(this.btnDodaj_Click);
            // 
            // btnUsun
            // 
            this.btnUsun.Location = new System.Drawing.Point(370, 262);
            this.btnUsun.Name = "btnUsun";
            this.btnUsun.Size = new System.Drawing.Size(100, 30);
            this.btnUsun.TabIndex = 3;
            this.btnUsun.Text = "Usuń";
            this.btnUsun.UseVisualStyleBackColor = true;
            this.btnUsun.Click += new System.EventHandler(this.btnUsun_Click);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(12, 215);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(101, 16);
            this.label1.TabIndex = 4;
            this.label1.Text = "Nazwa statusu:";
            // 
            // FormZarzadzajStatusami
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(482, 303);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.btnUsun);
            this.Controls.Add(this.btnDodaj);
            this.Controls.Add(this.txtNazwaStatusu);
            this.Controls.Add(this.listBoxStatusy);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "FormZarzadzajStatusami";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "Zarządzaj Statusami Decyzji";
            this.ResumeLayout(false);
            this.PerformLayout();
        }
        #endregion
        private System.Windows.Forms.ListBox listBoxStatusy;
        private System.Windows.Forms.TextBox txtNazwaStatusu;
        private System.Windows.Forms.Button btnDodaj;
        private System.Windows.Forms.Button btnUsun;
        private System.Windows.Forms.Label label1;
    }
}