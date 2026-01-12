using System.Drawing;
using System.Windows.Forms;

namespace Reklamacje_Dane
{
    partial class FormPolaczenie
    {
        private System.ComponentModel.IContainer components = null;

        // Kontrolki
        private Label lblInfo;
        private Label lblNumer;
        private Label lblKlient;
        private ListBox listZgloszenia;
        private Button btnClose;

        // Panel SMS
        private Panel pnlSms;
        private ComboBox cmbSzablony;
        private Button btnWyslijSms;
        private Label lblSmsInfo;

        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null)) components.Dispose();
            base.Dispose(disposing);
        }

        private void InitializeComponent()
        {
            this.lblInfo = new System.Windows.Forms.Label();
            this.lblNumer = new System.Windows.Forms.Label();
            this.lblKlient = new System.Windows.Forms.Label();
            this.listZgloszenia = new System.Windows.Forms.ListBox();
            this.btnClose = new System.Windows.Forms.Button();
            this.pnlSms = new System.Windows.Forms.Panel();
            this.cmbSzablony = new System.Windows.Forms.ComboBox();
            this.btnWyslijSms = new System.Windows.Forms.Button();
            this.lblSmsInfo = new System.Windows.Forms.Label();
            this.pnlSms.SuspendLayout();
            this.SuspendLayout();
            // 
            // lblInfo
            // 
            this.lblInfo.Dock = System.Windows.Forms.DockStyle.Top;
            this.lblInfo.Font = new System.Drawing.Font("Segoe UI", 12F, System.Drawing.FontStyle.Bold);
            this.lblInfo.ForeColor = System.Drawing.Color.White;
            this.lblInfo.Location = new System.Drawing.Point(0, 0);
            this.lblInfo.Name = "lblInfo";
            this.lblInfo.Size = new System.Drawing.Size(745, 40);
            this.lblInfo.TabIndex = 5;
            this.lblInfo.Text = "📞 DZWONI TELEFON";
            this.lblInfo.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // lblNumer
            // 
            this.lblNumer.Dock = System.Windows.Forms.DockStyle.Top;
            this.lblNumer.Font = new System.Drawing.Font("Segoe UI", 24F, System.Drawing.FontStyle.Bold);
            this.lblNumer.ForeColor = System.Drawing.Color.Yellow;
            this.lblNumer.Location = new System.Drawing.Point(0, 40);
            this.lblNumer.Name = "lblNumer";
            this.lblNumer.Size = new System.Drawing.Size(745, 60);
            this.lblNumer.TabIndex = 4;
            this.lblNumer.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // lblKlient
            // 
            this.lblKlient.Dock = System.Windows.Forms.DockStyle.Top;
            this.lblKlient.Font = new System.Drawing.Font("Segoe UI", 14F);
            this.lblKlient.ForeColor = System.Drawing.Color.White;
            this.lblKlient.Location = new System.Drawing.Point(0, 100);
            this.lblKlient.Name = "lblKlient";
            this.lblKlient.Padding = new System.Windows.Forms.Padding(0, 0, 0, 10);
            this.lblKlient.Size = new System.Drawing.Size(745, 70);
            this.lblKlient.TabIndex = 3;
            this.lblKlient.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // listZgloszenia
            // 
            this.listZgloszenia.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(50)))), ((int)(((byte)(50)))), ((int)(((byte)(50)))));
            this.listZgloszenia.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.listZgloszenia.Dock = System.Windows.Forms.DockStyle.Fill;
            this.listZgloszenia.Font = new System.Drawing.Font("Consolas", 10F);
            this.listZgloszenia.ForeColor = System.Drawing.Color.White;
            this.listZgloszenia.IntegralHeight = false;
            this.listZgloszenia.ItemHeight = 20;
            this.listZgloszenia.Location = new System.Drawing.Point(0, 170);
            this.listZgloszenia.Name = "listZgloszenia";
            this.listZgloszenia.Size = new System.Drawing.Size(745, 250);
            this.listZgloszenia.TabIndex = 0;
            this.listZgloszenia.SelectedIndexChanged += new System.EventHandler(this.ListZgloszenia_DoubleClick);
            // 
            // btnClose
            // 
            this.btnClose.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(220)))), ((int)(((byte)(53)))), ((int)(((byte)(69)))));
            this.btnClose.Cursor = System.Windows.Forms.Cursors.Hand;
            this.btnClose.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.btnClose.FlatAppearance.BorderSize = 0;
            this.btnClose.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnClose.Font = new System.Drawing.Font("Segoe UI", 12F, System.Drawing.FontStyle.Bold);
            this.btnClose.ForeColor = System.Drawing.Color.White;
            this.btnClose.Location = new System.Drawing.Point(0, 500);
            this.btnClose.Name = "btnClose";
            this.btnClose.Size = new System.Drawing.Size(745, 50);
            this.btnClose.TabIndex = 2;
            this.btnClose.Text = "ZAMKNIJ OKNO";
            this.btnClose.UseVisualStyleBackColor = false;
            this.btnClose.Click += new System.EventHandler(this.BtnClose_Click);
            // 
            // pnlSms
            // 
            this.pnlSms.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(45)))), ((int)(((byte)(45)))), ((int)(((byte)(48)))));
            this.pnlSms.Controls.Add(this.cmbSzablony);
            this.pnlSms.Controls.Add(this.btnWyslijSms);
            this.pnlSms.Controls.Add(this.lblSmsInfo);
            this.pnlSms.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.pnlSms.Location = new System.Drawing.Point(0, 420);
            this.pnlSms.Name = "pnlSms";
            this.pnlSms.Padding = new System.Windows.Forms.Padding(10);
            this.pnlSms.Size = new System.Drawing.Size(745, 80);
            this.pnlSms.TabIndex = 1;
            // 
            // cmbSzablony
            // 
            this.cmbSzablony.BackColor = System.Drawing.Color.White;
            this.cmbSzablony.Dock = System.Windows.Forms.DockStyle.Fill;
            this.cmbSzablony.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmbSzablony.Font = new System.Drawing.Font("Segoe UI", 12F);
            this.cmbSzablony.Location = new System.Drawing.Point(10, 30);
            this.cmbSzablony.Name = "cmbSzablony";
            this.cmbSzablony.Size = new System.Drawing.Size(625, 36);
            this.cmbSzablony.TabIndex = 0;
            // 
            // btnWyslijSms
            // 
            this.btnWyslijSms.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(120)))), ((int)(((byte)(215)))));
            this.btnWyslijSms.Cursor = System.Windows.Forms.Cursors.Hand;
            this.btnWyslijSms.Dock = System.Windows.Forms.DockStyle.Right;
            this.btnWyslijSms.FlatAppearance.BorderSize = 0;
            this.btnWyslijSms.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnWyslijSms.ForeColor = System.Drawing.Color.White;
            this.btnWyslijSms.Location = new System.Drawing.Point(635, 30);
            this.btnWyslijSms.Name = "btnWyslijSms";
            this.btnWyslijSms.Size = new System.Drawing.Size(100, 40);
            this.btnWyslijSms.TabIndex = 1;
            this.btnWyslijSms.Text = "Wyślij SMS";
            this.btnWyslijSms.UseVisualStyleBackColor = false;
            this.btnWyslijSms.Click += new System.EventHandler(this.BtnWyslijSms_Click);
            // 
            // lblSmsInfo
            // 
            this.lblSmsInfo.Dock = System.Windows.Forms.DockStyle.Top;
            this.lblSmsInfo.Font = new System.Drawing.Font("Segoe UI", 8F);
            this.lblSmsInfo.ForeColor = System.Drawing.Color.LightGray;
            this.lblSmsInfo.Location = new System.Drawing.Point(10, 10);
            this.lblSmsInfo.Name = "lblSmsInfo";
            this.lblSmsInfo.Size = new System.Drawing.Size(725, 20);
            this.lblSmsInfo.TabIndex = 2;
            this.lblSmsInfo.Text = "Szybka akcja SMS:";
            // 
            // FormPolaczenie
            // 
            this.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(30)))), ((int)(((byte)(30)))), ((int)(((byte)(30)))));
            this.ClientSize = new System.Drawing.Size(745, 550);
            this.Controls.Add(this.listZgloszenia);
            this.Controls.Add(this.pnlSms);
            this.Controls.Add(this.btnClose);
            this.Controls.Add(this.lblKlient);
            this.Controls.Add(this.lblNumer);
            this.Controls.Add(this.lblInfo);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
            this.Name = "FormPolaczenie";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Połączenie przychodzące";
            this.TopMost = true;
            this.pnlSms.ResumeLayout(false);
            this.ResumeLayout(false);

        }
    }
}