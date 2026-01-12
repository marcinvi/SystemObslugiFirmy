// Plik: FormZwrotWplaty.Designer.cs (WERSJA Z NOWOCZESNYM UI)
// Opis: Całkowicie przeprojektowany interfejs, spójny wizualnie
//       z resztą nowoczesnych formularzy w aplikacji.

namespace Reklamacje_Dane
{
    partial class FormZwrotWplaty
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
            this.panelTopHeader = new System.Windows.Forms.Panel();
            this.lblTitle = new System.Windows.Forms.Label();
            this.panelBottomActions = new System.Windows.Forms.Panel();
            this.lblSumaZwrotu = new System.Windows.Forms.Label();
            this.btnZlecZwrot = new System.Windows.Forms.Button();
            this.btnAnuluj = new System.Windows.Forms.Button();
            this.panelMainContainer = new System.Windows.Forms.Panel();
            this.cardPanelUzasadnienie = new Reklamacje_Dane.CardPanel();
            this.txtKomentarz = new System.Windows.Forms.TextBox();
            this.label2 = new System.Windows.Forms.Label();
            this.comboPowod = new System.Windows.Forms.ComboBox();
            this.label1 = new System.Windows.Forms.Label();
            this.lblCardUzasadnienieTitle = new System.Windows.Forms.Label();
            this.cardPanelDostawa = new Reklamacje_Dane.CardPanel();
            this.txtKwotaDostawy = new System.Windows.Forms.TextBox();
            this.chkZwrotDostawy = new System.Windows.Forms.CheckBox();
            this.lblCardDostawaTitle = new System.Windows.Forms.Label();
            this.cardPanelPozycje = new Reklamacje_Dane.CardPanel();
            this.tlpLineItems = new System.Windows.Forms.TableLayoutPanel();
            this.lblCardPozycjeTitle = new System.Windows.Forms.Label();
            this.panelTopHeader.SuspendLayout();
            this.panelBottomActions.SuspendLayout();
            this.panelMainContainer.SuspendLayout();
            this.cardPanelUzasadnienie.SuspendLayout();
            this.cardPanelDostawa.SuspendLayout();
            this.cardPanelPozycje.SuspendLayout();
            this.SuspendLayout();
            // 
            // panelTopHeader
            // 
            this.panelTopHeader.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(25)))), ((int)(((byte)(118)))), ((int)(((byte)(210)))));
            this.panelTopHeader.Controls.Add(this.lblTitle);
            this.panelTopHeader.Dock = System.Windows.Forms.DockStyle.Top;
            this.panelTopHeader.Location = new System.Drawing.Point(0, 0);
            this.panelTopHeader.Margin = new System.Windows.Forms.Padding(2, 2, 2, 2);
            this.panelTopHeader.Name = "panelTopHeader";
            this.panelTopHeader.Size = new System.Drawing.Size(588, 49);
            this.panelTopHeader.TabIndex = 14;
            // 
            // lblTitle
            // 
            this.lblTitle.AutoSize = true;
            this.lblTitle.Font = new System.Drawing.Font("Segoe UI", 13.8F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(238)));
            this.lblTitle.ForeColor = System.Drawing.Color.White;
            this.lblTitle.Location = new System.Drawing.Point(9, 11);
            this.lblTitle.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.lblTitle.Name = "lblTitle";
            this.lblTitle.Size = new System.Drawing.Size(288, 25);
            this.lblTitle.TabIndex = 0;
            this.lblTitle.Text = "Zwrot wpłaty dla zamówienia...";
            // 
            // panelBottomActions
            // 
            this.panelBottomActions.BackColor = System.Drawing.Color.WhiteSmoke;
            this.panelBottomActions.Controls.Add(this.lblSumaZwrotu);
            this.panelBottomActions.Controls.Add(this.btnZlecZwrot);
            this.panelBottomActions.Controls.Add(this.btnAnuluj);
            this.panelBottomActions.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.panelBottomActions.Location = new System.Drawing.Point(0, 555);
            this.panelBottomActions.Margin = new System.Windows.Forms.Padding(2, 2, 2, 2);
            this.panelBottomActions.Name = "panelBottomActions";
            this.panelBottomActions.Size = new System.Drawing.Size(588, 57);
            this.panelBottomActions.TabIndex = 15;
            // 
            // lblSumaZwrotu
            // 
            this.lblSumaZwrotu.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.lblSumaZwrotu.AutoSize = true;
            this.lblSumaZwrotu.Font = new System.Drawing.Font("Segoe UI", 10.2F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(238)));
            this.lblSumaZwrotu.Location = new System.Drawing.Point(10, 20);
            this.lblSumaZwrotu.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.lblSumaZwrotu.Name = "lblSumaZwrotu";
            this.lblSumaZwrotu.Size = new System.Drawing.Size(149, 19);
            this.lblSumaZwrotu.TabIndex = 12;
            this.lblSumaZwrotu.Text = "Suma do zwrotu: 0 zł";
            // 
            // btnZlecZwrot
            // 
            this.btnZlecZwrot.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btnZlecZwrot.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(76)))), ((int)(((byte)(175)))), ((int)(((byte)(80)))));
            this.btnZlecZwrot.FlatAppearance.BorderSize = 0;
            this.btnZlecZwrot.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnZlecZwrot.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(238)));
            this.btnZlecZwrot.ForeColor = System.Drawing.Color.White;
            this.btnZlecZwrot.Location = new System.Drawing.Point(467, 12);
            this.btnZlecZwrot.Margin = new System.Windows.Forms.Padding(2, 2, 2, 2);
            this.btnZlecZwrot.Name = "btnZlecZwrot";
            this.btnZlecZwrot.Size = new System.Drawing.Size(112, 32);
            this.btnZlecZwrot.TabIndex = 11;
            this.btnZlecZwrot.Text = "Zleć zwrot wpłaty";
            this.btnZlecZwrot.UseVisualStyleBackColor = false;
            this.btnZlecZwrot.Click += new System.EventHandler(this.btnZlecZwrot_Click);
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
            this.btnAnuluj.Location = new System.Drawing.Point(372, 12);
            this.btnAnuluj.Margin = new System.Windows.Forms.Padding(2, 2, 2, 2);
            this.btnAnuluj.Name = "btnAnuluj";
            this.btnAnuluj.Size = new System.Drawing.Size(90, 32);
            this.btnAnuluj.TabIndex = 10;
            this.btnAnuluj.Text = "Anuluj";
            this.btnAnuluj.UseVisualStyleBackColor = false;
            // 
            // panelMainContainer
            // 
            this.panelMainContainer.AutoScroll = true;
            this.panelMainContainer.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(240)))), ((int)(((byte)(242)))), ((int)(((byte)(245)))));
            this.panelMainContainer.Controls.Add(this.cardPanelUzasadnienie);
            this.panelMainContainer.Controls.Add(this.cardPanelDostawa);
            this.panelMainContainer.Controls.Add(this.cardPanelPozycje);
            this.panelMainContainer.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panelMainContainer.Location = new System.Drawing.Point(0, 49);
            this.panelMainContainer.Margin = new System.Windows.Forms.Padding(2, 2, 2, 2);
            this.panelMainContainer.Name = "panelMainContainer";
            this.panelMainContainer.Padding = new System.Windows.Forms.Padding(8, 8, 8, 8);
            this.panelMainContainer.Size = new System.Drawing.Size(588, 506);
            this.panelMainContainer.TabIndex = 16;
            // 
            // cardPanelUzasadnienie
            // 
            this.cardPanelUzasadnienie.BackColor = System.Drawing.Color.White;
            this.cardPanelUzasadnienie.BorderRadius = 5;
            this.cardPanelUzasadnienie.Controls.Add(this.txtKomentarz);
            this.cardPanelUzasadnienie.Controls.Add(this.label2);
            this.cardPanelUzasadnienie.Controls.Add(this.comboPowod);
            this.cardPanelUzasadnienie.Controls.Add(this.label1);
            this.cardPanelUzasadnienie.Controls.Add(this.lblCardUzasadnienieTitle);
            this.cardPanelUzasadnienie.Dock = System.Windows.Forms.DockStyle.Top;
            this.cardPanelUzasadnienie.Location = new System.Drawing.Point(8, 195);
            this.cardPanelUzasadnienie.Margin = new System.Windows.Forms.Padding(2, 2, 2, 8);
            this.cardPanelUzasadnienie.Name = "cardPanelUzasadnienie";
            this.cardPanelUzasadnienie.Padding = new System.Windows.Forms.Padding(1);
            this.cardPanelUzasadnienie.Size = new System.Drawing.Size(572, 190);
            this.cardPanelUzasadnienie.TabIndex = 2;
            // 
            // txtKomentarz
            // 
            this.txtKomentarz.Font = new System.Drawing.Font("Segoe UI", 9F);
            this.txtKomentarz.Location = new System.Drawing.Point(15, 102);
            this.txtKomentarz.Margin = new System.Windows.Forms.Padding(2, 2, 2, 2);
            this.txtKomentarz.Multiline = true;
            this.txtKomentarz.Name = "txtKomentarz";
            this.txtKomentarz.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
            this.txtKomentarz.Size = new System.Drawing.Size(541, 50);
            this.txtKomentarz.TabIndex = 4;
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Font = new System.Drawing.Font("Segoe UI", 9F);
            this.label2.Location = new System.Drawing.Point(12, 84);
            this.label2.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(225, 15);
            this.label2.TabIndex = 3;
            this.label2.Text = "Wiadomość do kupującego (opcjonalna):";
            // 
            // comboPowod
            // 
            this.comboPowod.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.comboPowod.Font = new System.Drawing.Font("Segoe UI", 9F);
            this.comboPowod.FormattingEnabled = true;
            this.comboPowod.Location = new System.Drawing.Point(15, 50);
            this.comboPowod.Margin = new System.Windows.Forms.Padding(2, 2, 2, 2);
            this.comboPowod.Name = "comboPowod";
            this.comboPowod.Size = new System.Drawing.Size(226, 23);
            this.comboPowod.TabIndex = 2;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Bold);
            this.label1.Location = new System.Drawing.Point(12, 31);
            this.label1.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(161, 15);
            this.label1.TabIndex = 1;
            this.label1.Text = "Powód zwrotu (wymagany):";
            // 
            // lblCardUzasadnienieTitle
            // 
            this.lblCardUzasadnienieTitle.AutoSize = true;
            this.lblCardUzasadnienieTitle.Font = new System.Drawing.Font("Segoe UI", 10.8F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(238)));
            this.lblCardUzasadnienieTitle.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(25)))), ((int)(((byte)(118)))), ((int)(((byte)(210)))));
            this.lblCardUzasadnienieTitle.Location = new System.Drawing.Point(11, 11);
            this.lblCardUzasadnienieTitle.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.lblCardUzasadnienieTitle.Name = "lblCardUzasadnienieTitle";
            this.lblCardUzasadnienieTitle.Size = new System.Drawing.Size(101, 20);
            this.lblCardUzasadnienieTitle.TabIndex = 0;
            this.lblCardUzasadnienieTitle.Text = "Uzasadnienie";
            // 
            // cardPanelDostawa
            // 
            this.cardPanelDostawa.BackColor = System.Drawing.Color.White;
            this.cardPanelDostawa.BorderRadius = 5;
            this.cardPanelDostawa.Controls.Add(this.txtKwotaDostawy);
            this.cardPanelDostawa.Controls.Add(this.chkZwrotDostawy);
            this.cardPanelDostawa.Controls.Add(this.lblCardDostawaTitle);
            this.cardPanelDostawa.Dock = System.Windows.Forms.DockStyle.Top;
            this.cardPanelDostawa.Location = new System.Drawing.Point(8, 114);
            this.cardPanelDostawa.Margin = new System.Windows.Forms.Padding(2, 2, 2, 8);
            this.cardPanelDostawa.Name = "cardPanelDostawa";
            this.cardPanelDostawa.Padding = new System.Windows.Forms.Padding(1);
            this.cardPanelDostawa.Size = new System.Drawing.Size(572, 81);
            this.cardPanelDostawa.TabIndex = 1;
            // 
            // txtKwotaDostawy
            // 
            this.txtKwotaDostawy.Font = new System.Drawing.Font("Segoe UI", 9F);
            this.txtKwotaDostawy.Location = new System.Drawing.Point(165, 41);
            this.txtKwotaDostawy.Margin = new System.Windows.Forms.Padding(2, 2, 2, 2);
            this.txtKwotaDostawy.Name = "txtKwotaDostawy";
            this.txtKwotaDostawy.Size = new System.Drawing.Size(76, 23);
            this.txtKwotaDostawy.TabIndex = 2;
            this.txtKwotaDostawy.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            this.txtKwotaDostawy.TextChanged += new System.EventHandler(this.txtKwota_TextChanged);
            // 
            // chkZwrotDostawy
            // 
            this.chkZwrotDostawy.AutoSize = true;
            this.chkZwrotDostawy.Font = new System.Drawing.Font("Segoe UI", 9F);
            this.chkZwrotDostawy.Location = new System.Drawing.Point(15, 42);
            this.chkZwrotDostawy.Margin = new System.Windows.Forms.Padding(2, 2, 2, 2);
            this.chkZwrotDostawy.Name = "chkZwrotDostawy";
            this.chkZwrotDostawy.Size = new System.Drawing.Size(139, 19);
            this.chkZwrotDostawy.TabIndex = 1;
            this.chkZwrotDostawy.Text = "Zwróć koszt dostawy:";
            this.chkZwrotDostawy.UseVisualStyleBackColor = true;
            this.chkZwrotDostawy.CheckedChanged += new System.EventHandler(this.chkZwrotDostawy_CheckedChanged);
            // 
            // lblCardDostawaTitle
            // 
            this.lblCardDostawaTitle.AutoSize = true;
            this.lblCardDostawaTitle.Font = new System.Drawing.Font("Segoe UI", 10.8F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(238)));
            this.lblCardDostawaTitle.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(25)))), ((int)(((byte)(118)))), ((int)(((byte)(210)))));
            this.lblCardDostawaTitle.Location = new System.Drawing.Point(11, 11);
            this.lblCardDostawaTitle.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.lblCardDostawaTitle.Name = "lblCardDostawaTitle";
            this.lblCardDostawaTitle.Size = new System.Drawing.Size(70, 20);
            this.lblCardDostawaTitle.TabIndex = 0;
            this.lblCardDostawaTitle.Text = "Dostawa";
            // 
            // cardPanelPozycje
            // 
            this.cardPanelPozycje.AutoScroll = true;
            this.cardPanelPozycje.BackColor = System.Drawing.Color.White;
            this.cardPanelPozycje.BorderRadius = 5;
            this.cardPanelPozycje.Controls.Add(this.tlpLineItems);
            this.cardPanelPozycje.Controls.Add(this.lblCardPozycjeTitle);
            this.cardPanelPozycje.Dock = System.Windows.Forms.DockStyle.Top;
            this.cardPanelPozycje.Location = new System.Drawing.Point(8, 8);
            this.cardPanelPozycje.Margin = new System.Windows.Forms.Padding(2, 2, 2, 8);
            this.cardPanelPozycje.Name = "cardPanelPozycje";
            this.cardPanelPozycje.Padding = new System.Windows.Forms.Padding(1);
            this.cardPanelPozycje.Size = new System.Drawing.Size(572, 106);
            this.cardPanelPozycje.TabIndex = 0;
            // 
            // tlpLineItems
            // 
            this.tlpLineItems.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.tlpLineItems.AutoSize = true;
            this.tlpLineItems.ColumnCount = 4;
            this.tlpLineItems.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 60F));
            this.tlpLineItems.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 15F));
            this.tlpLineItems.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 15F));
            this.tlpLineItems.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 10F));
            this.tlpLineItems.Font = new System.Drawing.Font("Segoe UI", 9F);
            this.tlpLineItems.Location = new System.Drawing.Point(15, 41);
            this.tlpLineItems.Margin = new System.Windows.Forms.Padding(2, 2, 2, 2);
            this.tlpLineItems.Name = "tlpLineItems";
            this.tlpLineItems.RowCount = 1;
            this.tlpLineItems.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 32F));
            this.tlpLineItems.Size = new System.Drawing.Size(540, 32);
            this.tlpLineItems.TabIndex = 1;
            // 
            // lblCardPozycjeTitle
            // 
            this.lblCardPozycjeTitle.AutoSize = true;
            this.lblCardPozycjeTitle.Font = new System.Drawing.Font("Segoe UI", 10.8F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(238)));
            this.lblCardPozycjeTitle.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(25)))), ((int)(((byte)(118)))), ((int)(((byte)(210)))));
            this.lblCardPozycjeTitle.Location = new System.Drawing.Point(11, 11);
            this.lblCardPozycjeTitle.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.lblCardPozycjeTitle.Name = "lblCardPozycjeTitle";
            this.lblCardPozycjeTitle.Size = new System.Drawing.Size(136, 20);
            this.lblCardPozycjeTitle.TabIndex = 0;
            this.lblCardPozycjeTitle.Text = "Pozycje do zwrotu";
            // 
            // FormZwrotWplaty
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(588, 612);
            this.Controls.Add(this.panelMainContainer);
            this.Controls.Add(this.panelBottomActions);
            this.Controls.Add(this.panelTopHeader);
            this.Margin = new System.Windows.Forms.Padding(2, 2, 2, 2);
            this.MinimumSize = new System.Drawing.Size(604, 495);
            this.Name = "FormZwrotWplaty";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "Zwrot Wpłaty";
            this.Load += new System.EventHandler(this.FormZwrotWplaty_Load);
            this.panelTopHeader.ResumeLayout(false);
            this.panelTopHeader.PerformLayout();
            this.panelBottomActions.ResumeLayout(false);
            this.panelBottomActions.PerformLayout();
            this.panelMainContainer.ResumeLayout(false);
            this.cardPanelUzasadnienie.ResumeLayout(false);
            this.cardPanelUzasadnienie.PerformLayout();
            this.cardPanelDostawa.ResumeLayout(false);
            this.cardPanelDostawa.PerformLayout();
            this.cardPanelPozycje.ResumeLayout(false);
            this.cardPanelPozycje.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Panel panelTopHeader;
        private System.Windows.Forms.Label lblTitle;
        private System.Windows.Forms.Panel panelBottomActions;
        private System.Windows.Forms.Button btnZlecZwrot;
        private System.Windows.Forms.Button btnAnuluj;
        private System.Windows.Forms.Panel panelMainContainer;
        private CardPanel cardPanelPozycje;
        private System.Windows.Forms.Label lblCardPozycjeTitle;
        private CardPanel cardPanelDostawa;
        private System.Windows.Forms.Label lblCardDostawaTitle;
        private CardPanel cardPanelUzasadnienie;
        private System.Windows.Forms.Label lblCardUzasadnienieTitle;
        private System.Windows.Forms.TableLayoutPanel tlpLineItems;
        private System.Windows.Forms.TextBox txtKwotaDostawy;
        private System.Windows.Forms.CheckBox chkZwrotDostawy;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.ComboBox comboPowod;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.TextBox txtKomentarz;
        private System.Windows.Forms.Label lblSumaZwrotu;
    }
}