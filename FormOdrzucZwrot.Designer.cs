// Plik: FormOdrzucZwrot.Designer.cs (WERSJA Z NOWOCZESNYM UI)
// Opis: Całkowicie przeprojektowany interfejs, spójny wizualnie
//       z resztą nowoczesnych formularzy w aplikacji.

namespace Reklamacje_Dane
{
    partial class FormOdrzucZwrot
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
            this.btnOdrzuc = new System.Windows.Forms.Button();
            this.btnAnuluj = new System.Windows.Forms.Button();
            this.panelMainContainer = new System.Windows.Forms.Panel();
            this.cardPanelDane = new CardPanel();
            this.txtUzasadnienie = new System.Windows.Forms.TextBox();
            this.label2 = new System.Windows.Forms.Label();
            this.comboPowod = new System.Windows.Forms.ComboBox();
            this.label1 = new System.Windows.Forms.Label();
            this.lblCardDaneTitle = new System.Windows.Forms.Label();
            this.panelTopHeader.SuspendLayout();
            this.panelBottomActions.SuspendLayout();
            this.panelMainContainer.SuspendLayout();
            this.cardPanelDane.SuspendLayout();
            this.SuspendLayout();
            // 
            // panelTopHeader
            // 
            this.panelTopHeader.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(25)))), ((int)(((byte)(118)))), ((int)(((byte)(210)))));
            this.panelTopHeader.Controls.Add(this.lblTitle);
            this.panelTopHeader.Dock = System.Windows.Forms.DockStyle.Top;
            this.panelTopHeader.Location = new System.Drawing.Point(0, 0);
            this.panelTopHeader.Name = "panelTopHeader";
            this.panelTopHeader.Size = new System.Drawing.Size(582, 60);
            this.panelTopHeader.TabIndex = 15;
            // 
            // lblTitle
            // 
            this.lblTitle.AutoSize = true;
            this.lblTitle.Font = new System.Drawing.Font("Segoe UI", 13.8F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(238)));
            this.lblTitle.ForeColor = System.Drawing.Color.White;
            this.lblTitle.Location = new System.Drawing.Point(12, 14);
            this.lblTitle.Name = "lblTitle";
            this.lblTitle.Size = new System.Drawing.Size(175, 32);
            this.lblTitle.TabIndex = 0;
            this.lblTitle.Text = "Odrzuć zwrot";
            // 
            // panelBottomActions
            // 
            this.panelBottomActions.BackColor = System.Drawing.Color.WhiteSmoke;
            this.panelBottomActions.Controls.Add(this.btnOdrzuc);
            this.panelBottomActions.Controls.Add(this.btnAnuluj);
            this.panelBottomActions.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.panelBottomActions.Location = new System.Drawing.Point(0, 383);
            this.panelBottomActions.Name = "panelBottomActions";
            this.panelBottomActions.Size = new System.Drawing.Size(582, 70);
            this.panelBottomActions.TabIndex = 16;
            // 
            // btnOdrzuc
            // 
            this.btnOdrzuc.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btnOdrzuc.BackColor = System.Drawing.Color.Firebrick;
            this.btnOdrzuc.FlatAppearance.BorderSize = 0;
            this.btnOdrzuc.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnOdrzuc.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(238)));
            this.btnOdrzuc.ForeColor = System.Drawing.Color.White;
            this.btnOdrzuc.Location = new System.Drawing.Point(380, 15);
            this.btnOdrzuc.Name = "btnOdrzuc";
            this.btnOdrzuc.Size = new System.Drawing.Size(190, 40);
            this.btnOdrzuc.TabIndex = 11;
            this.btnOdrzuc.Text = "Potwierdź odrzucenie";
            this.btnOdrzuc.UseVisualStyleBackColor = false;
            this.btnOdrzuc.Click += new System.EventHandler(this.btnOdrzuc_Click);
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
            this.btnAnuluj.Location = new System.Drawing.Point(254, 15);
            this.btnAnuluj.Name = "btnAnuluj";
            this.btnAnuluj.Size = new System.Drawing.Size(120, 40);
            this.btnAnuluj.TabIndex = 10;
            this.btnAnuluj.Text = "Anuluj";
            this.btnAnuluj.UseVisualStyleBackColor = false;
            // 
            // panelMainContainer
            // 
            this.panelMainContainer.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(240)))), ((int)(((byte)(242)))), ((int)(((byte)(245)))));
            this.panelMainContainer.Controls.Add(this.cardPanelDane);
            this.panelMainContainer.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panelMainContainer.Location = new System.Drawing.Point(0, 60);
            this.panelMainContainer.Name = "panelMainContainer";
            this.panelMainContainer.Padding = new System.Windows.Forms.Padding(10);
            this.panelMainContainer.Size = new System.Drawing.Size(582, 323);
            this.panelMainContainer.TabIndex = 17;
            // 
            // cardPanelDane
            // 
            this.cardPanelDane.BackColor = System.Drawing.Color.White;
            this.cardPanelDane.BorderRadius = 5;
            this.cardPanelDane.Controls.Add(this.txtUzasadnienie);
            this.cardPanelDane.Controls.Add(this.label2);
            this.cardPanelDane.Controls.Add(this.comboPowod);
            this.cardPanelDane.Controls.Add(this.label1);
            this.cardPanelDane.Controls.Add(this.lblCardDaneTitle);
            this.cardPanelDane.Location = new System.Drawing.Point(13, 13);
            this.cardPanelDane.Name = "cardPanelDane";
            this.cardPanelDane.Size = new System.Drawing.Size(557, 297);
            this.cardPanelDane.TabIndex = 0;
            // 
            // txtUzasadnienie
            // 
            this.txtUzasadnienie.Font = new System.Drawing.Font("Segoe UI", 9F);
            this.txtUzasadnienie.Location = new System.Drawing.Point(20, 150);
            this.txtUzasadnienie.Multiline = true;
            this.txtUzasadnienie.Name = "txtUzasadnienie";
            this.txtUzasadnienie.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
            this.txtUzasadnienie.Size = new System.Drawing.Size(517, 127);
            this.txtUzasadnienie.TabIndex = 4;
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Font = new System.Drawing.Font("Segoe UI", 9F);
            this.label2.Location = new System.Drawing.Point(16, 127);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(262, 20);
            this.label2.TabIndex = 3;
            this.label2.Text = "Uzasadnienie dla kupującego (opcja):";
            // 
            // comboPowod
            // 
            this.comboPowod.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.comboPowod.Font = new System.Drawing.Font("Segoe UI", 9F);
            this.comboPowod.FormattingEnabled = true;
            this.comboPowod.Location = new System.Drawing.Point(20, 80);
            this.comboPowod.Name = "comboPowod";
            this.comboPowod.Size = new System.Drawing.Size(517, 28);
            this.comboPowod.TabIndex = 2;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Bold);
            this.label1.Location = new System.Drawing.Point(16, 57);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(248, 20);
            this.label1.TabIndex = 1;
            this.label1.Text = "Powód odrzucenia (wymagany):";
            // 
            // lblCardDaneTitle
            // 
            this.lblCardDaneTitle.AutoSize = true;
            this.lblCardDaneTitle.Font = new System.Drawing.Font("Segoe UI", 10.8F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(238)));
            this.lblCardDaneTitle.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(25)))), ((int)(((byte)(118)))), ((int)(((byte)(210)))));
            this.lblCardDaneTitle.Location = new System.Drawing.Point(15, 14);
            this.lblCardDaneTitle.Name = "lblCardDaneTitle";
            this.lblCardDaneTitle.Size = new System.Drawing.Size(206, 25);
            this.lblCardDaneTitle.TabIndex = 0;
            this.lblCardDaneTitle.Text = "Przyczyna odrzucenia";
            // 
            // FormOdrzucZwrot
            // 
            this.AcceptButton = this.btnOdrzuc;
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.CancelButton = this.btnAnuluj;
            this.ClientSize = new System.Drawing.Size(582, 453);
            this.Controls.Add(this.panelMainContainer);
            this.Controls.Add(this.panelBottomActions);
            this.Controls.Add(this.panelTopHeader);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "FormOdrzucZwrot";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "Odrzuć Zwrot";
            this.Load += new System.EventHandler(this.FormOdrzucZwrot_Load);
            this.panelTopHeader.ResumeLayout(false);
            this.panelTopHeader.PerformLayout();
            this.panelBottomActions.ResumeLayout(false);
            this.panelMainContainer.ResumeLayout(false);
            this.cardPanelDane.ResumeLayout(false);
            this.cardPanelDane.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Panel panelTopHeader;
        private System.Windows.Forms.Label lblTitle;
        private System.Windows.Forms.Panel panelBottomActions;
        private System.Windows.Forms.Button btnOdrzuc;
        private System.Windows.Forms.Button btnAnuluj;
        private System.Windows.Forms.Panel panelMainContainer;
        private CardPanel cardPanelDane;
        private System.Windows.Forms.Label lblCardDaneTitle;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.ComboBox comboPowod;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.TextBox txtUzasadnienie;
    }
}