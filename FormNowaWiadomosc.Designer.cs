namespace Reklamacje_Dane
{
    partial class FormNowaWiadomosc
    {
        private System.ComponentModel.IContainer components = null;
        protected override void Dispose(bool disposing) { if (disposing && (components != null)) { components.Dispose(); } base.Dispose(disposing); }
        #region Windows Form Designer generated code
        private void InitializeComponent()
        {
            this.label1 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.txtTytul = new System.Windows.Forms.TextBox();
            this.label3 = new System.Windows.Forms.Label();
            this.txtTresc = new System.Windows.Forms.TextBox();
            this.btnWyslij = new System.Windows.Forms.Button();
            this.btnAnuluj = new System.Windows.Forms.Button();
            this.checkedListBoxOdbiorcy = new System.Windows.Forms.CheckedListBox();
            this.SuspendLayout();
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(238)));
            this.label1.Location = new System.Drawing.Point(12, 19);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(30, 20);
            this.label1.TabIndex = 0;
            this.label1.Text = "Do:";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(238)));
            this.label2.Location = new System.Drawing.Point(12, 142);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(41, 20);
            this.label2.TabIndex = 2;
            this.label2.Text = "Tytuł:";
            // 
            // txtTytul
            // 
            this.txtTytul.Location = new System.Drawing.Point(16, 165);
            this.txtTytul.Name = "txtTytul";
            this.txtTytul.Size = new System.Drawing.Size(554, 22);
            this.txtTytul.TabIndex = 3;
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(238)));
            this.label3.Location = new System.Drawing.Point(12, 201);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(46, 20);
            this.label3.TabIndex = 4;
            this.label3.Text = "Treść:";
            // 
            // txtTresc
            // 
            this.txtTresc.Location = new System.Drawing.Point(16, 224);
            this.txtTresc.Multiline = true;
            this.txtTresc.Name = "txtTresc";
            this.txtTresc.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
            this.txtTresc.Size = new System.Drawing.Size(554, 180);
            this.txtTresc.TabIndex = 5;
            // 
            // btnWyslij
            // 
            this.btnWyslij.Location = new System.Drawing.Point(364, 421);
            this.btnWyslij.Name = "btnWyslij";
            this.btnWyslij.Size = new System.Drawing.Size(100, 30);
            this.btnWyslij.TabIndex = 6;
            this.btnWyslij.Text = "Wyślij";
            this.btnWyslij.UseVisualStyleBackColor = true;
            this.btnWyslij.Click += new System.EventHandler(this.btnWyslij_Click);
            // 
            // btnAnuluj
            // 
            this.btnAnuluj.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.btnAnuluj.Location = new System.Drawing.Point(470, 421);
            this.btnAnuluj.Name = "btnAnuluj";
            this.btnAnuluj.Size = new System.Drawing.Size(100, 30);
            this.btnAnuluj.TabIndex = 7;
            this.btnAnuluj.Text = "Anuluj";
            this.btnAnuluj.UseVisualStyleBackColor = true;
            // 
            // checkedListBoxOdbiorcy
            // 
            this.checkedListBoxOdbiorcy.FormattingEnabled = true;
            this.checkedListBoxOdbiorcy.Location = new System.Drawing.Point(16, 43);
            this.checkedListBoxOdbiorcy.Name = "checkedListBoxOdbiorcy";
            this.checkedListBoxOdbiorcy.Size = new System.Drawing.Size(554, 89);
            this.checkedListBoxOdbiorcy.TabIndex = 8;
            // 
            // FormNowaWiadomosc
            // 
            this.AcceptButton = this.btnWyslij;
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.CancelButton = this.btnAnuluj;
            this.ClientSize = new System.Drawing.Size(582, 463);
            this.Controls.Add(this.checkedListBoxOdbiorcy);
            this.Controls.Add(this.btnAnuluj);
            this.Controls.Add(this.btnWyslij);
            this.Controls.Add(this.txtTresc);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.txtTytul);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.label1);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "FormNowaWiadomosc";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "Nowa Wiadomość";
            this.Load += new System.EventHandler(this.FormNowaWiadomosc_Load);
            this.ResumeLayout(false);
            this.PerformLayout();
        }
        #endregion
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.TextBox txtTytul;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.TextBox txtTresc;
        private System.Windows.Forms.Button btnWyslij;
        private System.Windows.Forms.Button btnAnuluj;
        private System.Windows.Forms.CheckedListBox checkedListBoxOdbiorcy;
    }
}