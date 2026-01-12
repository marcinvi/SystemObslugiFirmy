namespace Reklamacje_Dane
{
    partial class FormUstawienia
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
            this.listViewKonta = new System.Windows.Forms.ListView();
            this.columnHeader1 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.columnHeader2 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.btnDodaj = new System.Windows.Forms.Button();
            this.btnUsun = new System.Windows.Forms.Button();
            this.btnAutoryzuj = new System.Windows.Forms.Button();
            this.btnTestRefresh = new System.Windows.Forms.Button();
            this.button1 = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // listViewKonta
            // 
            this.listViewKonta.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.columnHeader1,
            this.columnHeader2});
            this.listViewKonta.FullRowSelect = true;
            this.listViewKonta.GridLines = true;
            this.listViewKonta.HideSelection = false;
            this.listViewKonta.Location = new System.Drawing.Point(12, 12);
            this.listViewKonta.MultiSelect = false;
            this.listViewKonta.Name = "listViewKonta";
            this.listViewKonta.Size = new System.Drawing.Size(776, 385);
            this.listViewKonta.TabIndex = 0;
            this.listViewKonta.UseCompatibleStateImageBehavior = false;
            this.listViewKonta.View = System.Windows.Forms.View.Details;
            // 
            // columnHeader1
            // 
            this.columnHeader1.Text = "ID";
            // 
            // columnHeader2
            // 
            this.columnHeader2.Text = "Nazwa konta (Seller ID)";
            this.columnHeader2.Width = 200;
            // 
            // btnDodaj
            // 
            this.btnDodaj.Location = new System.Drawing.Point(13, 404);
            this.btnDodaj.Name = "btnDodaj";
            this.btnDodaj.Size = new System.Drawing.Size(120, 34);
            this.btnDodaj.TabIndex = 1;
            this.btnDodaj.Text = "Dodaj konto";
            this.btnDodaj.UseVisualStyleBackColor = true;
            this.btnDodaj.Click += new System.EventHandler(this.btnDodaj_Click);
            // 
            // btnUsun
            // 
            this.btnUsun.Location = new System.Drawing.Point(139, 404);
            this.btnUsun.Name = "btnUsun";
            this.btnUsun.Size = new System.Drawing.Size(120, 34);
            this.btnUsun.TabIndex = 2;
            this.btnUsun.Text = "Usuń konto";
            this.btnUsun.UseVisualStyleBackColor = true;
            this.btnUsun.Click += new System.EventHandler(this.btnUsun_Click);
            // 
            // btnAutoryzuj
            // 
            this.btnAutoryzuj.Font = new System.Drawing.Font("Microsoft Sans Serif", 7.8F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(238)));
            this.btnAutoryzuj.Location = new System.Drawing.Point(265, 404);
            this.btnAutoryzuj.Name = "btnAutoryzuj";
            this.btnAutoryzuj.Size = new System.Drawing.Size(150, 34);
            this.btnAutoryzuj.TabIndex = 3;
            this.btnAutoryzuj.Text = "Autoryzuj konto";
            this.btnAutoryzuj.UseVisualStyleBackColor = true;
            this.btnAutoryzuj.Click += new System.EventHandler(this.btnAutoryzuj_Click);
            // 
            // btnTestRefresh
            // 
            this.btnTestRefresh.Font = new System.Drawing.Font("Microsoft Sans Serif", 7.8F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(238)));
            this.btnTestRefresh.Location = new System.Drawing.Point(421, 404);
            this.btnTestRefresh.Name = "btnTestRefresh";
            this.btnTestRefresh.Size = new System.Drawing.Size(150, 34);
            this.btnTestRefresh.TabIndex = 4;
            this.btnTestRefresh.Text = "Testuj Odświeżenie";
            this.btnTestRefresh.UseVisualStyleBackColor = true;
            this.btnTestRefresh.Click += new System.EventHandler(this.btnTestRefresh_Click);
            // 
            // button1
            // 
            this.button1.Font = new System.Drawing.Font("Microsoft Sans Serif", 7.8F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(238)));
            this.button1.Location = new System.Drawing.Point(577, 404);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(150, 34);
            this.button1.TabIndex = 5;
            this.button1.Text = "Konta mailowe";
            this.button1.UseVisualStyleBackColor = true;
            this.button1.Click += new System.EventHandler(this.button1_Click);
            // 
            // FormUstawienia
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(800, 450);
            this.Controls.Add(this.button1);
            this.Controls.Add(this.btnTestRefresh);
            this.Controls.Add(this.btnAutoryzuj);
            this.Controls.Add(this.btnUsun);
            this.Controls.Add(this.btnDodaj);
            this.Controls.Add(this.listViewKonta);
            this.Name = "FormUstawienia";
            this.Text = "Ustawienia Kont Allegro";
            this.Load += new System.EventHandler(this.FormUstawienia_Load);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.ListView listViewKonta;
        private System.Windows.Forms.ColumnHeader columnHeader1;
        private System.Windows.Forms.ColumnHeader columnHeader2;
        private System.Windows.Forms.Button btnDodaj;
        private System.Windows.Forms.Button btnUsun;
        private System.Windows.Forms.Button btnAutoryzuj;
        private System.Windows.Forms.Button btnTestRefresh;
        private System.Windows.Forms.Button button1;
    }
}