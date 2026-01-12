// Plik: FormDodajZwrotReczny.Designer.cs (WERSJA FINALNA Z ZAZNACZ WSZYSTKICH)
// Opis: Dodano CheckBox do zaznaczania wszystkich handlowców i poprawiono walidację.

namespace Reklamacje_Dane
{
    partial class FormDodajZwrotReczny
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
            this.btnZapisz = new System.Windows.Forms.Button();
            this.btnAnuluj = new System.Windows.Forms.Button();
            this.panelTopHeader = new System.Windows.Forms.Panel();
            this.lblTitle = new System.Windows.Forms.Label();
            this.panelBottomActions = new System.Windows.Forms.Panel();
            this.panelMainContainer = new System.Windows.Forms.Panel();
            this.cardPanelHandlowcy = new Reklamacje_Dane.CardPanel();
            this.chkWszyscyHandlowcy = new System.Windows.Forms.CheckBox();
            this.checkedListBoxHandlowcy = new System.Windows.Forms.CheckedListBox();
            this.lblCardHandlowcyTitle = new System.Windows.Forms.Label();
            this.cardPanelDane = new Reklamacje_Dane.CardPanel();
            this.comboStanProduktu = new System.Windows.Forms.ComboBox();
            this.label11 = new System.Windows.Forms.Label();
            this.txtUwagi = new System.Windows.Forms.TextBox();
            this.label10 = new System.Windows.Forms.Label();
            this.txtTelefon = new System.Windows.Forms.TextBox();
            this.label8 = new System.Windows.Forms.Label();
            this.txtMiasto = new System.Windows.Forms.TextBox();
            this.txtKodPocztowy = new System.Windows.Forms.TextBox();
            this.label7 = new System.Windows.Forms.Label();
            this.txtUlica = new System.Windows.Forms.TextBox();
            this.label6 = new System.Windows.Forms.Label();
            this.txtImieNazwisko = new System.Windows.Forms.TextBox();
            this.label5 = new System.Windows.Forms.Label();
            this.comboProdukt = new System.Windows.Forms.ComboBox();
            this.label4 = new System.Windows.Forms.Label();
            this.comboPrzewoznik = new System.Windows.Forms.ComboBox();
            this.label2 = new System.Windows.Forms.Label();
            this.txtNumerListu = new System.Windows.Forms.TextBox();
            this.label1 = new System.Windows.Forms.Label();
            this.lblCardDaneTitle = new System.Windows.Forms.Label();
            this.panelTopHeader.SuspendLayout();
            this.panelBottomActions.SuspendLayout();
            this.panelMainContainer.SuspendLayout();
            this.cardPanelHandlowcy.SuspendLayout();
            this.cardPanelDane.SuspendLayout();
            this.SuspendLayout();
            // 
            // btnZapisz
            // 
            this.btnZapisz.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btnZapisz.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(76)))), ((int)(((byte)(175)))), ((int)(((byte)(80)))));
            this.btnZapisz.FlatAppearance.BorderSize = 0;
            this.btnZapisz.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnZapisz.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(238)));
            this.btnZapisz.ForeColor = System.Drawing.Color.White;
            this.btnZapisz.Location = new System.Drawing.Point(590, 15);
            this.btnZapisz.Name = "btnZapisz";
            this.btnZapisz.Size = new System.Drawing.Size(150, 40);
            this.btnZapisz.TabIndex = 8;
            this.btnZapisz.Text = "Zapisz i powiadom";
            this.btnZapisz.UseVisualStyleBackColor = false;
            this.btnZapisz.Click += new System.EventHandler(this.btnZapisz_Click);
            // 
            // btnAnuluj
            // 
            this.btnAnuluj.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btnAnuluj.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(224)))), ((int)(((byte)(224)))), ((int)(((byte)(224)))));
            this.btnAnuluj.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.btnAnuluj.FlatAppearance.BorderSize = 0;
            this.btnAnuluj.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnAnuluj.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Bold);
            this.btnAnuluj.ForeColor = System.Drawing.Color.Black;
            this.btnAnuluj.Location = new System.Drawing.Point(464, 15);
            this.btnAnuluj.Name = "btnAnuluj";
            this.btnAnuluj.Size = new System.Drawing.Size(120, 40);
            this.btnAnuluj.TabIndex = 9;
            this.btnAnuluj.Text = "Anuluj";
            this.btnAnuluj.UseVisualStyleBackColor = false;
            // 
            // panelTopHeader
            // 
            this.panelTopHeader.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(25)))), ((int)(((byte)(118)))), ((int)(((byte)(210)))));
            this.panelTopHeader.Controls.Add(this.lblTitle);
            this.panelTopHeader.Dock = System.Windows.Forms.DockStyle.Top;
            this.panelTopHeader.Location = new System.Drawing.Point(0, 0);
            this.panelTopHeader.Name = "panelTopHeader";
            this.panelTopHeader.Size = new System.Drawing.Size(752, 60);
            this.panelTopHeader.TabIndex = 12;
            // 
            // lblTitle
            // 
            this.lblTitle.AutoSize = true;
            this.lblTitle.Font = new System.Drawing.Font("Segoe UI", 13.8F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(238)));
            this.lblTitle.ForeColor = System.Drawing.Color.White;
            this.lblTitle.Location = new System.Drawing.Point(12, 14);
            this.lblTitle.Name = "lblTitle";
            this.lblTitle.Size = new System.Drawing.Size(243, 32);
            this.lblTitle.TabIndex = 0;
            this.lblTitle.Text = "Dodaj Zwrot Ręczny";
            // 
            // panelBottomActions
            // 
            this.panelBottomActions.BackColor = System.Drawing.Color.WhiteSmoke;
            this.panelBottomActions.Controls.Add(this.btnAnuluj);
            this.panelBottomActions.Controls.Add(this.btnZapisz);
            this.panelBottomActions.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.panelBottomActions.Location = new System.Drawing.Point(0, 663);
            this.panelBottomActions.Name = "panelBottomActions";
            this.panelBottomActions.Size = new System.Drawing.Size(752, 70);
            this.panelBottomActions.TabIndex = 13;
            // 
            // panelMainContainer
            // 
            this.panelMainContainer.AutoScroll = true;
            this.panelMainContainer.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(240)))), ((int)(((byte)(242)))), ((int)(((byte)(245)))));
            this.panelMainContainer.Controls.Add(this.cardPanelHandlowcy);
            this.panelMainContainer.Controls.Add(this.cardPanelDane);
            this.panelMainContainer.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panelMainContainer.Location = new System.Drawing.Point(0, 60);
            this.panelMainContainer.Name = "panelMainContainer";
            this.panelMainContainer.Padding = new System.Windows.Forms.Padding(10);
            this.panelMainContainer.Size = new System.Drawing.Size(752, 603);
            this.panelMainContainer.TabIndex = 14;
            // 
            // cardPanelHandlowcy
            // 
            this.cardPanelHandlowcy.BackColor = System.Drawing.Color.White;
            this.cardPanelHandlowcy.BorderRadius = 5;
            this.cardPanelHandlowcy.Controls.Add(this.chkWszyscyHandlowcy);
            this.cardPanelHandlowcy.Controls.Add(this.checkedListBoxHandlowcy);
            this.cardPanelHandlowcy.Controls.Add(this.lblCardHandlowcyTitle);
            this.cardPanelHandlowcy.Location = new System.Drawing.Point(465, 13);
            this.cardPanelHandlowcy.Name = "cardPanelHandlowcy";
            this.cardPanelHandlowcy.Size = new System.Drawing.Size(275, 577);
            this.cardPanelHandlowcy.TabIndex = 1;
            // 
            // chkWszyscyHandlowcy
            // 
            this.chkWszyscyHandlowcy.AutoSize = true;
            this.chkWszyscyHandlowcy.Font = new System.Drawing.Font("Segoe UI", 9F);
            this.chkWszyscyHandlowcy.Location = new System.Drawing.Point(19, 53);
            this.chkWszyscyHandlowcy.Name = "chkWszyscyHandlowcy";
            this.chkWszyscyHandlowcy.Size = new System.Drawing.Size(207, 24);
            this.chkWszyscyHandlowcy.TabIndex = 3;
            this.chkWszyscyHandlowcy.Text = "Zaznacz/Odznacz wszystkich";
            this.chkWszyscyHandlowcy.UseVisualStyleBackColor = true;
         
            // 
            // checkedListBoxHandlowcy
            // 
            this.checkedListBoxHandlowcy.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.checkedListBoxHandlowcy.CheckOnClick = true;
            this.checkedListBoxHandlowcy.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(238)));
            this.checkedListBoxHandlowcy.FormattingEnabled = true;
            this.checkedListBoxHandlowcy.Location = new System.Drawing.Point(19, 83);
            this.checkedListBoxHandlowcy.Name = "checkedListBoxHandlowcy";
            this.checkedListBoxHandlowcy.Size = new System.Drawing.Size(237, 464);
            this.checkedListBoxHandlowcy.TabIndex = 2;
            // 
            // lblCardHandlowcyTitle
            // 
            this.lblCardHandlowcyTitle.AutoSize = true;
            this.lblCardHandlowcyTitle.Font = new System.Drawing.Font("Segoe UI", 10.8F, System.Drawing.FontStyle.Bold);
            this.lblCardHandlowcyTitle.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(25)))), ((int)(((byte)(118)))), ((int)(((byte)(210)))));
            this.lblCardHandlowcyTitle.Location = new System.Drawing.Point(14, 14);
            this.lblCardHandlowcyTitle.Name = "lblCardHandlowcyTitle";
            this.lblCardHandlowcyTitle.Size = new System.Drawing.Size(217, 25);
            this.lblCardHandlowcyTitle.TabIndex = 1;
            this.lblCardHandlowcyTitle.Text = "Powiadom Handlowców";
            // 
            // cardPanelDane
            // 
            this.cardPanelDane.BackColor = System.Drawing.Color.White;
            this.cardPanelDane.BorderRadius = 5;
            this.cardPanelDane.Controls.Add(this.comboStanProduktu);
            this.cardPanelDane.Controls.Add(this.label11);
            this.cardPanelDane.Controls.Add(this.txtUwagi);
            this.cardPanelDane.Controls.Add(this.label10);
            this.cardPanelDane.Controls.Add(this.txtTelefon);
            this.cardPanelDane.Controls.Add(this.label8);
            this.cardPanelDane.Controls.Add(this.txtMiasto);
            this.cardPanelDane.Controls.Add(this.txtKodPocztowy);
            this.cardPanelDane.Controls.Add(this.label7);
            this.cardPanelDane.Controls.Add(this.txtUlica);
            this.cardPanelDane.Controls.Add(this.label6);
            this.cardPanelDane.Controls.Add(this.txtImieNazwisko);
            this.cardPanelDane.Controls.Add(this.label5);
            this.cardPanelDane.Controls.Add(this.comboProdukt);
            this.cardPanelDane.Controls.Add(this.label4);
            this.cardPanelDane.Controls.Add(this.comboPrzewoznik);
            this.cardPanelDane.Controls.Add(this.label2);
            this.cardPanelDane.Controls.Add(this.txtNumerListu);
            this.cardPanelDane.Controls.Add(this.label1);
            this.cardPanelDane.Controls.Add(this.lblCardDaneTitle);
            this.cardPanelDane.Location = new System.Drawing.Point(13, 13);
            this.cardPanelDane.Name = "cardPanelDane";
            this.cardPanelDane.Size = new System.Drawing.Size(436, 577);
            this.cardPanelDane.TabIndex = 0;
            // 
            // comboStanProduktu
            // 
            this.comboStanProduktu.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.comboStanProduktu.Font = new System.Drawing.Font("Segoe UI", 9F);
            this.comboStanProduktu.FormattingEnabled = true;
            this.comboStanProduktu.Location = new System.Drawing.Point(20, 206);
            this.comboStanProduktu.Name = "comboStanProduktu";
            this.comboStanProduktu.Size = new System.Drawing.Size(396, 28);
            this.comboStanProduktu.TabIndex = 20;
            // 
            // label11
            // 
            this.label11.AutoSize = true;
            this.label11.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Bold);
            this.label11.Location = new System.Drawing.Point(16, 183);
            this.label11.Name = "label11";
            this.label11.Size = new System.Drawing.Size(193, 20);
            this.label11.TabIndex = 19;
            this.label11.Text = "Stan produktu (wymagany):";
            // 
            // txtUwagi
            // 
            this.txtUwagi.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(238)));
            this.txtUwagi.Location = new System.Drawing.Point(20, 500);
            this.txtUwagi.Multiline = true;
            this.txtUwagi.Name = "txtUwagi";
            this.txtUwagi.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
            this.txtUwagi.Size = new System.Drawing.Size(396, 60);
            this.txtUwagi.TabIndex = 18;
            // 
            // label10
            // 
            this.label10.AutoSize = true;
            this.label10.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(238)));
            this.label10.Location = new System.Drawing.Point(16, 477);
            this.label10.Name = "label10";
            this.label10.Size = new System.Drawing.Size(54, 20);
            this.label10.TabIndex = 17;
            this.label10.Text = "Uwagi:";
            // 
            // txtTelefon
            // 
            this.txtTelefon.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(238)));
            this.txtTelefon.Location = new System.Drawing.Point(20, 437);
            this.txtTelefon.Name = "txtTelefon";
            this.txtTelefon.Size = new System.Drawing.Size(396, 27);
            this.txtTelefon.TabIndex = 16;
            // 
            // label8
            // 
            this.label8.AutoSize = true;
            this.label8.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(238)));
            this.label8.Location = new System.Drawing.Point(16, 414);
            this.label8.Name = "label8";
            this.label8.Size = new System.Drawing.Size(61, 20);
            this.label8.TabIndex = 15;
            this.label8.Text = "Telefon:";
            // 
            // txtMiasto
            // 
            this.txtMiasto.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(238)));
            this.txtMiasto.Location = new System.Drawing.Point(155, 374);
            this.txtMiasto.Name = "txtMiasto";
            this.txtMiasto.Size = new System.Drawing.Size(261, 27);
            this.txtMiasto.TabIndex = 14;
            // 
            // txtKodPocztowy
            // 
            this.txtKodPocztowy.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(238)));
            this.txtKodPocztowy.Location = new System.Drawing.Point(20, 374);
            this.txtKodPocztowy.Name = "txtKodPocztowy";
            this.txtKodPocztowy.Size = new System.Drawing.Size(129, 27);
            this.txtKodPocztowy.TabIndex = 13;
            // 
            // label7
            // 
            this.label7.AutoSize = true;
            this.label7.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(238)));
            this.label7.Location = new System.Drawing.Point(16, 351);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(161, 20);
            this.label7.TabIndex = 12;
            this.label7.Text = "Kod pocztowy i Miasto:";
            // 
            // txtUlica
            // 
            this.txtUlica.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(238)));
            this.txtUlica.Location = new System.Drawing.Point(20, 311);
            this.txtUlica.Name = "txtUlica";
            this.txtUlica.Size = new System.Drawing.Size(396, 27);
            this.txtUlica.TabIndex = 11;
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(238)));
            this.label6.Location = new System.Drawing.Point(16, 288);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(43, 20);
            this.label6.TabIndex = 10;
            this.label6.Text = "Ulica:";
            // 
            // txtImieNazwisko
            // 
            this.txtImieNazwisko.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(238)));
            this.txtImieNazwisko.Location = new System.Drawing.Point(20, 258);
            this.txtImieNazwisko.Name = "txtImieNazwisko";
            this.txtImieNazwisko.Size = new System.Drawing.Size(396, 27);
            this.txtImieNazwisko.TabIndex = 9;
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(238)));
            this.label5.Location = new System.Drawing.Point(16, 235);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(113, 20);
            this.label5.TabIndex = 8;
            this.label5.Text = "Imię i nazwisko:";
            // 
            // comboProdukt
            // 
            this.comboProdukt.AutoCompleteMode = System.Windows.Forms.AutoCompleteMode.SuggestAppend;
            this.comboProdukt.AutoCompleteSource = System.Windows.Forms.AutoCompleteSource.ListItems;
            this.comboProdukt.Font = new System.Drawing.Font("Segoe UI", 9F);
            this.comboProdukt.FormattingEnabled = true;
            this.comboProdukt.Location = new System.Drawing.Point(20, 143);
            this.comboProdukt.Name = "comboProdukt";
            this.comboProdukt.Size = new System.Drawing.Size(396, 28);
            this.comboProdukt.TabIndex = 7;
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(238)));
            this.label4.Location = new System.Drawing.Point(16, 120);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(123, 20);
            this.label4.TabIndex = 6;
            this.label4.Text = "Zwracany produkt:";
            // 
            // comboPrzewoznik
            // 
            this.comboPrzewoznik.AutoCompleteMode = System.Windows.Forms.AutoCompleteMode.SuggestAppend;
            this.comboPrzewoznik.AutoCompleteSource = System.Windows.Forms.AutoCompleteSource.ListItems;
            this.comboPrzewoznik.Font = new System.Drawing.Font("Segoe UI", 9F);
            this.comboPrzewoznik.FormattingEnabled = true;
            this.comboPrzewoznik.Location = new System.Drawing.Point(230, 80);
            this.comboPrzewoznik.Name = "comboPrzewoznik";
            this.comboPrzewoznik.Size = new System.Drawing.Size(186, 28);
            this.comboPrzewoznik.TabIndex = 5;
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(238)));
            this.label2.Location = new System.Drawing.Point(226, 57);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(84, 20);
            this.label2.TabIndex = 4;
            this.label2.Text = "Przewoźnik:";
            // 
            // txtNumerListu
            // 
            this.txtNumerListu.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(238)));
            this.txtNumerListu.Location = new System.Drawing.Point(20, 80);
            this.txtNumerListu.Name = "txtNumerListu";
            this.txtNumerListu.Size = new System.Drawing.Size(200, 27);
            this.txtNumerListu.TabIndex = 3;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Bold);
            this.label1.Location = new System.Drawing.Point(16, 57);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(183, 20);
            this.label1.TabIndex = 2;
            this.label1.Text = "Numer listu (wymagany):";
            // 
            // lblCardDaneTitle
            // 
            this.lblCardDaneTitle.AutoSize = true;
            this.lblCardDaneTitle.Font = new System.Drawing.Font("Segoe UI", 10.8F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(238)));
            this.lblCardDaneTitle.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(25)))), ((int)(((byte)(118)))), ((int)(((byte)(210)))));
            this.lblCardDaneTitle.Location = new System.Drawing.Point(15, 14);
            this.lblCardDaneTitle.Name = "lblCardDaneTitle";
            this.lblCardDaneTitle.Size = new System.Drawing.Size(199, 25);
            this.lblCardDaneTitle.TabIndex = 0;
            this.lblCardDaneTitle.Text = "Dane Zwrotu i Klienta";
            // 
            // FormDodajZwrotReczny
            // 
            this.AcceptButton = this.btnZapisz;
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.CancelButton = this.btnAnuluj;
            this.ClientSize = new System.Drawing.Size(752, 733);
            this.Controls.Add(this.panelMainContainer);
            this.Controls.Add(this.panelBottomActions);
            this.Controls.Add(this.panelTopHeader);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "FormDodajZwrotReczny";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "Dodaj Zwrot Ręczny";
            this.Load += new System.EventHandler(this.FormDodajZwrotReczny_Load);
            this.panelTopHeader.ResumeLayout(false);
            this.panelTopHeader.PerformLayout();
            this.panelBottomActions.ResumeLayout(false);
            this.panelMainContainer.ResumeLayout(false);
            this.cardPanelHandlowcy.ResumeLayout(false);
            this.cardPanelHandlowcy.PerformLayout();
            this.cardPanelDane.ResumeLayout(false);
            this.cardPanelDane.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion
        private System.Windows.Forms.Button btnZapisz;
        private System.Windows.Forms.Button btnAnuluj;
        private System.Windows.Forms.Panel panelTopHeader;
        private System.Windows.Forms.Label lblTitle;
        private System.Windows.Forms.Panel panelBottomActions;
        private System.Windows.Forms.Panel panelMainContainer;
        private CardPanel cardPanelDane;
        private System.Windows.Forms.Label lblCardDaneTitle;
        private System.Windows.Forms.TextBox txtNumerListu;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.ComboBox comboPrzewoznik;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.ComboBox comboProdukt;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.TextBox txtImieNazwisko;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.TextBox txtUlica;
        private System.Windows.Forms.Label label7;
        private System.Windows.Forms.TextBox txtKodPocztowy;
        private System.Windows.Forms.TextBox txtMiasto;
        private System.Windows.Forms.Label label8;
        private System.Windows.Forms.TextBox txtTelefon;
        private System.Windows.Forms.Label label10;
        private System.Windows.Forms.TextBox txtUwagi;
        private CardPanel cardPanelHandlowcy;
        private System.Windows.Forms.CheckedListBox checkedListBoxHandlowcy;
        private System.Windows.Forms.Label lblCardHandlowcyTitle;
        private System.Windows.Forms.ComboBox comboStanProduktu;
        private System.Windows.Forms.Label label11;
        private System.Windows.Forms.CheckBox chkWszyscyHandlowcy;
    }
}