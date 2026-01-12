// Plik: Form14.Designer.cs
namespace Reklamacje_Dane
{
    partial class Form14
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
            this.components = new System.ComponentModel.Container();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Form14));
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle1 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle2 = new System.Windows.Forms.DataGridViewCellStyle();
            this.btnWczytajZArkusz = new System.Windows.Forms.Button();
            this.dgvZgloszenia = new System.Windows.Forms.DataGridView();
            this.btnZapisz = new System.Windows.Forms.Button();
            this.panel1 = new System.Windows.Forms.Panel();
            this.label1 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.label4 = new System.Windows.Forms.Label();
            this.label5 = new System.Windows.Forms.Label();
            this.label6 = new System.Windows.Forms.Label();
            this.label7 = new System.Windows.Forms.Label();
            this.txtNrSeryjny = new System.Windows.Forms.TextBox();
            this.txtNrFaktury = new System.Windows.Forms.TextBox();
            this.txtKlient = new System.Windows.Forms.TextBox();
            this.txtProdukt = new System.Windows.Forms.TextBox();
            this.btnDodajKlienta = new System.Windows.Forms.Button();
            this.btnDodajProdukt = new System.Windows.Forms.Button();
            this.dgvKlientSuggestions = new System.Windows.Forms.DataGridView();
            this.dgvProduktSuggestions = new System.Windows.Forms.DataGridView();
            this.lblNrSeryjnyWarning = new System.Windows.Forms.Label();
            this.lblNrFakturyWarning = new System.Windows.Forms.Label();
            this.toolTip1 = new System.Windows.Forms.ToolTip(this.components);
            ((System.ComponentModel.ISupportInitialize)(this.dgvZgloszenia)).BeginInit();
            this.panel1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dgvKlientSuggestions)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.dgvProduktSuggestions)).BeginInit();
            this.SuspendLayout();
            //
            // btnWczytajZArkusz
            //
            this.btnWczytajZArkusz.Location = new System.Drawing.Point(12, 12);
            this.btnWczytajZArkusz.Name = "btnWczytajZArkusz";
            this.btnWczytajZArkusz.Size = new System.Drawing.Size(150, 30);
            this.btnWczytajZArkusz.TabIndex = 0;
            this.btnWczytajZArkusz.Text = "Wczytaj z Arkusza";
            this.btnWczytajZArkusz.UseVisualStyleBackColor = true;
            this.btnWczytajZArkusz.Click += new System.EventHandler(this.btnWczytajZArkusz_Click);
            //
            // dgvZgloszenia
            //
            this.dgvZgloszenia.AllowUserToAddRows = false;
            this.dgvZgloszenia.AllowUserToDeleteRows = false;
            this.dgvZgloszenia.AutoSizeColumnsMode = System.Windows.Forms.DataGridViewAutoSizeColumnsMode.Fill;
            this.dgvZgloszenia.BackgroundColor = System.Drawing.Color.White;
            this.dgvZgloszenia.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dgvZgloszenia.Location = new System.Drawing.Point(12, 48);
            this.dgvZgloszenia.Name = "dgvZgloszenia";
            this.dgvZgloszenia.ReadOnly = true;
            this.dgvZgloszenia.RowHeadersVisible = false;
            this.dgvZgloszenia.RowHeadersWidth = 51;
            this.dgvZgloszenia.RowTemplate.Height = 24;
            this.dgvZgloszenia.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
            this.dgvZgloszenia.Size = new System.Drawing.Size(500, 400);
            this.dgvZgloszenia.TabIndex = 1;
            this.dgvZgloszenia.SelectionChanged += new System.EventHandler(this.dgvZgloszenia_SelectionChanged);
            //
            // btnZapisz
            //
            this.btnZapisz.BackColor = System.Drawing.Color.ForestGreen;
            this.btnZapisz.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnZapisz.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Bold);
            this.btnZapisz.ForeColor = System.Drawing.Color.White;
            this.btnZapisz.Location = new System.Drawing.Point(540, 418);
            this.btnZapisz.Name = "btnZapisz";
            this.btnZapisz.Size = new System.Drawing.Size(150, 30);
            this.btnZapisz.TabIndex = 2;
            this.btnZapisz.Text = "Zapisz do bazy";
            this.btnZapisz.UseVisualStyleBackColor = false;
            this.btnZapisz.Click += new System.EventHandler(this.btnZapisz_Click);
            //
            // panel1
            //
            this.panel1.BackColor = System.Drawing.Color.White;
            this.panel1.Controls.Add(this.lblNrFakturyWarning);
            this.panel1.Controls.Add(this.lblNrSeryjnyWarning);
            this.panel1.Controls.Add(this.btnDodajProdukt);
            this.panel1.Controls.Add(this.btnDodajKlienta);
            this.panel1.Controls.Add(this.txtProdukt);
            this.panel1.Controls.Add(this.txtKlient);
            this.panel1.Controls.Add(this.txtNrFaktury);
            this.panel1.Controls.Add(this.txtNrSeryjny);
            this.panel1.Controls.Add(this.label7);
            this.panel1.Controls.Add(this.label6);
            this.panel1.Controls.Add(this.label5);
            this.panel1.Controls.Add(this.label4);
            this.panel1.Controls.Add(this.label3);
            this.panel1.Controls.Add(this.label2);
            this.panel1.Controls.Add(this.label1);
            this.panel1.Controls.Add(this.dgvKlientSuggestions);
            this.panel1.Controls.Add(this.dgvProduktSuggestions);
            this.panel1.Location = new System.Drawing.Point(535, 48);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(400, 350);
            this.panel1.TabIndex = 3;
            //
            // label1
            //
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(12, 15);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(120, 20);
            this.label1.TabIndex = 0;
            this.label1.Text = "Data zgłoszenia:";
            //
            // label2
            //
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(12, 45);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(55, 20);
            this.label2.TabIndex = 1;
            this.label2.Text = "Klient:";
            //
            // label3
            //
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(12, 105);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(65, 20);
            this.label3.TabIndex = 2;
            this.label3.Text = "Produkt:";
            //
            // label4
            //
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(12, 165);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(89, 20);
            this.label4.TabIndex = 3;
            this.label4.Text = "Nr Faktury:";
            //
            // label5
            //
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(12, 225);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(85, 20);
            this.label5.TabIndex = 4;
            this.label5.Text = "Nr Seryjny:";
            //
            // label6
            //
            this.label6.AutoSize = true;
            this.label6.Location = new System.Drawing.Point(12, 285);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(65, 20);
            this.label6.TabIndex = 5;
            this.label6.Text = "Usterka:";
            //
            // label7
            //
            this.label7.AutoSize = true;
            this.label7.Location = new System.Drawing.Point(12, 315);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(60, 20);
            this.label7.TabIndex = 6;
            this.label7.Text = "Status:";
            //
            // txtNrSeryjny
            //
            this.txtNrSeryjny.Location = new System.Drawing.Point(12, 248);
            this.txtNrSeryjny.Name = "txtNrSeryjny";
            this.txtNrSeryjny.Size = new System.Drawing.Size(250, 27);
            this.txtNrSeryjny.TabIndex = 7;
            this.txtNrSeryjny.Leave += new System.EventHandler(this.txtNrSeryjny_Leave);
            //
            // txtNrFaktury
            //
            this.txtNrFaktury.Location = new System.Drawing.Point(12, 188);
            this.txtNrFaktury.Name = "txtNrFaktury";
            this.txtNrFaktury.Size = new System.Drawing.Size(250, 27);
            this.txtNrFaktury.TabIndex = 8;
            this.txtNrFaktury.Leave += new System.EventHandler(this.txtNrFaktury_Leave);
            //
            // txtKlient
            //
            this.txtKlient.Location = new System.Drawing.Point(12, 68);
            this.txtKlient.Name = "txtKlient";
            this.txtKlient.Size = new System.Drawing.Size(250, 27);
            this.txtKlient.TabIndex = 9;
            this.txtKlient.TextChanged += new System.EventHandler(this.txtKlient_TextChanged);
            this.txtKlient.Leave += new System.EventHandler(this.txtKlient_Leave);
            //
            // txtProdukt
            //
            this.txtProdukt.Location = new System.Drawing.Point(12, 128);
            this.txtProdukt.Name = "txtProdukt";
            this.txtProdukt.Size = new System.Drawing.Size(250, 27);
            this.txtProdukt.TabIndex = 10;
            this.txtProdukt.TextChanged += new System.EventHandler(this.txtProdukt_TextChanged);
            this.txtProdukt.Leave += new System.EventHandler(this.txtProdukt_Leave);
            //
            // btnDodajKlienta
            //
            this.btnDodajKlienta.Location = new System.Drawing.Point(268, 68);
            this.btnDodajKlienta.Name = "btnDodajKlienta";
            this.btnDodajKlienta.Size = new System.Drawing.Size(120, 27);
            this.btnDodajKlienta.TabIndex = 11;
            this.btnDodajKlienta.Text = "Dodaj Klienta";
            this.btnDodajKlienta.UseVisualStyleBackColor = true;
            this.btnDodajKlienta.Click += new System.EventHandler(this.btnDodajKlienta_Click);
            //
            // btnDodajProdukt
            //
            this.btnDodajProdukt.Location = new System.Drawing.Point(268, 128);
            this.btnDodajProdukt.Name = "btnDodajProdukt";
            this.btnDodajProdukt.Size = new System.Drawing.Size(120, 27);
            this.btnDodajProdukt.TabIndex = 12;
            this.btnDodajProdukt.Text = "Dodaj Produkt";
            this.btnDodajProdukt.UseVisualStyleBackColor = true;
            this.btnDodajProdukt.Click += new System.EventHandler(this.btnDodajProdukt_Click);
            //
            // dgvKlientSuggestions
            //
            this.dgvKlientSuggestions.AllowUserToAddRows = false;
            this.dgvKlientSuggestions.AllowUserToDeleteRows = false;
            this.dgvKlientSuggestions.AutoSizeColumnsMode = System.Windows.Forms.DataGridViewAutoSizeColumnsMode.Fill;
            this.dgvKlientSuggestions.BackgroundColor = System.Drawing.Color.White;
            this.dgvKlientSuggestions.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dgvKlientSuggestions.Location = new System.Drawing.Point(12, 95);
            this.dgvKlientSuggestions.Name = "dgvKlientSuggestions";
            this.dgvKlientSuggestions.ReadOnly = true;
            this.dgvKlientSuggestions.RowHeadersVisible = false;
            this.dgvKlientSuggestions.RowHeadersWidth = 51;
            this.dgvKlientSuggestions.RowTemplate.Height = 24;
            this.dgvKlientSuggestions.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
            this.dgvKlientSuggestions.Size = new System.Drawing.Size(376, 100);
            this.dgvKlientSuggestions.TabIndex = 13;
            this.dgvKlientSuggestions.Visible = false;
            this.dgvKlientSuggestions.CellClick += new System.Windows.Forms.DataGridViewCellEventHandler(this.dgvKlientSuggestions_CellClick);
            //
            // dgvProduktSuggestions
            //
            this.dgvProduktSuggestions.AllowUserToAddRows = false;
            this.dgvProduktSuggestions.AllowUserToDeleteRows = false;
            this.dgvProduktSuggestions.AutoSizeColumnsMode = System.Windows.Forms.DataGridViewAutoSizeColumnsMode.Fill;
            this.dgvProduktSuggestions.BackgroundColor = System.Drawing.Color.White;
            this.dgvProduktSuggestions.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dgvProduktSuggestions.ColumnHeadersVisible = false;
            this.dgvProduktSuggestions.Location = new System.Drawing.Point(12, 155);
            this.dgvProduktSuggestions.Name = "dgvProduktSuggestions";
            this.dgvProduktSuggestions.ReadOnly = true;
            this.dgvProduktSuggestions.RowHeadersVisible = false;
            this.dgvProduktSuggestions.RowHeadersWidth = 51;
            this.dgvProduktSuggestions.RowTemplate.Height = 24;
            this.dgvProduktSuggestions.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
            this.dgvProduktSuggestions.Size = new System.Drawing.Size(376, 100);
            this.dgvProduktSuggestions.TabIndex = 14;
            this.dgvProduktSuggestions.Visible = false;
            this.dgvProduktSuggestions.CellClick += new System.Windows.Forms.DataGridViewCellEventHandler(this.dgvProduktSuggestions_CellClick);
            //
            // lblNrSeryjnyWarning
            //
            this.lblNrSeryjnyWarning.AutoSize = true;
            this.lblNrSeryjnyWarning.ForeColor = System.Drawing.Color.Red;
            this.lblNrSeryjnyWarning.Location = new System.Drawing.Point(268, 248);
            this.lblNrSeryjnyWarning.Name = "lblNrSeryjnyWarning";
            this.lblNrSeryjnyWarning.Size = new System.Drawing.Size(0, 20);
            this.lblNrSeryjnyWarning.TabIndex = 15;
            //
            // lblNrFakturyWarning
            //
            this.lblNrFakturyWarning.AutoSize = true;
            this.lblNrFakturyWarning.ForeColor = System.Drawing.Color.Red;
            this.lblNrFakturyWarning.Location = new System.Drawing.Point(268, 188);
            this.lblNrFakturyWarning.Name = "lblNrFakturyWarning";
            this.lblNrFakturyWarning.Size = new System.Drawing.Size(0, 20);
            this.lblNrFakturyWarning.TabIndex = 16;
            //
            // toolTip1
            //
            this.toolTip1.AutoPopDelay = 5000;
            this.toolTip1.InitialDelay = 100;
            this.toolTip1.IsBalloon = true;
            this.toolTip1.ReshowDelay = 100;
            this.toolTip1.ToolTipIcon = System.Windows.Forms.ToolTipIcon.Warning;
            this.toolTip1.ToolTipTitle = "Duplikat";
            //
            // Form14
            //
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 20F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(240)))), ((int)(((byte)(240)))), ((int)(((byte)(240)))));
            this.ClientSize = new System.Drawing.Size(950, 470);
            this.Controls.Add(this.panel1);
            this.Controls.Add(this.btnZapisz);
            this.Controls.Add(this.dgvZgloszenia);
            this.Controls.Add(this.btnWczytajZArkusz);
            this.Font = new System.Drawing.Font("Segoe UI", 9F);
            this.Name = "Form14";
            this.Text = "Dodaj z Google Sheets";
            //this.Load += new System.EventHandler(this.Form14_Load);
            ((System.ComponentModel.ISupportInitialize)(this.dgvZgloszenia)).EndInit();
            this.panel1.ResumeLayout(false);
            this.panel1.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dgvKlientSuggestions)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.dgvProduktSuggestions)).EndInit();
            this.ResumeLayout(false);
        }

        #endregion

        private System.Windows.Forms.Button btnWczytajZArkusz;
        private System.Windows.Forms.DataGridView dgvZgloszenia;
        private System.Windows.Forms.Button btnZapisz;
        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.Label label7;
        private System.Windows.Forms.TextBox txtNrSeryjny;
        private System.Windows.Forms.TextBox txtNrFaktury;
        private System.Windows.Forms.TextBox txtKlient;
        private System.Windows.Forms.TextBox txtProdukt;
        private System.Windows.Forms.Button btnDodajKlienta;
        private System.Windows.Forms.Button btnDodajProdukt;
        private System.Windows.Forms.DataGridView dgvKlientSuggestions;
        private System.Windows.Forms.DataGridView dgvProduktSuggestions;
        private System.Windows.Forms.Label lblNrSeryjnyWarning;
        private System.Windows.Forms.Label lblNrFakturyWarning;
        private System.Windows.Forms.ToolTip toolTip1;
    }
}