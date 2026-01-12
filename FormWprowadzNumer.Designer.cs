namespace Reklamacje_Dane
{
    partial class FormWprowadzNumer
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
            this.txtNumerListu = new System.Windows.Forms.TextBox();
            this.btnZatwierdz = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Font = new System.Drawing.Font("Segoe UI Semibold", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(238)));
            this.label1.Location = new System.Drawing.Point(25, 22);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(248, 20);
            this.label1.TabIndex = 0;
            this.label1.Text = "Wprowadź numer listu przewozowego:";
            // 
            // txtNumerListu
            // 
            this.txtNumerListu.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(238)));
            this.txtNumerListu.Location = new System.Drawing.Point(29, 45);
            this.txtNumerListu.Name = "txtNumerListu";
            this.txtNumerListu.Size = new System.Drawing.Size(395, 27);
            this.txtNumerListu.TabIndex = 1;
            // 
            // btnZatwierdz
            // 
            this.btnZatwierdz.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(68)))), ((int)(((byte)(114)))), ((int)(((byte)(196)))));
            this.btnZatwierdz.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnZatwierdz.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(238)));
            this.btnZatwierdz.ForeColor = System.Drawing.Color.White;
            this.btnZatwierdz.Location = new System.Drawing.Point(163, 91);
            this.btnZatwierdz.Name = "btnZatwierdz";
            this.btnZatwierdz.Size = new System.Drawing.Size(125, 35);
            this.btnZatwierdz.TabIndex = 2;
            this.btnZatwierdz.Text = "Zatwierdź";
            this.btnZatwierdz.UseVisualStyleBackColor = false;
            this.btnZatwierdz.Click += new System.EventHandler(this.btnZatwierdz_Click);
            // 
            // FormWprowadzNumer
            // 
            this.AcceptButton = this.btnZatwierdz;
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.White;
            this.ClientSize = new System.Drawing.Size(452, 149);
            this.Controls.Add(this.btnZatwierdz);
            this.Controls.Add(this.txtNumerListu);
            this.Controls.Add(this.label1);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "FormWprowadzNumer";
            this.Text = "Potwierdzenie zamówienia kuriera";
            this.ResumeLayout(false);
            this.PerformLayout();
        }

        #endregion

        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.TextBox txtNumerListu;
        private System.Windows.Forms.Button btnZatwierdz;
    }
}