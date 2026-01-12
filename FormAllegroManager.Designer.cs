namespace Reklamacje_Dane
{
    partial class FormAllegroManager
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
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle1 = new System.Windows.Forms.DataGridViewCellStyle();
            this.panelTop = new System.Windows.Forms.Panel();
            this.btnRefresh = new System.Windows.Forms.Button();
            this.cmbAccounts = new System.Windows.Forms.ComboBox();
            this.lblAccount = new System.Windows.Forms.Label();
            this.dgvDisputes = new System.Windows.Forms.DataGridView();
            this.panelTop.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dgvDisputes)).BeginInit();
            this.SuspendLayout();
            // 
            // panelTop
            // 
            this.panelTop.Controls.Add(this.btnRefresh);
            this.panelTop.Controls.Add(this.cmbAccounts);
            this.panelTop.Controls.Add(this.lblAccount);
            this.panelTop.Dock = System.Windows.Forms.DockStyle.Top;
            this.panelTop.Location = new System.Drawing.Point(0, 0);
            this.panelTop.Name = "panelTop";
            this.panelTop.Size = new System.Drawing.Size(982, 50);
            this.panelTop.TabIndex = 0;
            // 
            // btnRefresh
            // 
            this.btnRefresh.Location = new System.Drawing.Point(370, 12);
            this.btnRefresh.Name = "btnRefresh";
            this.btnRefresh.Size = new System.Drawing.Size(94, 29);
            this.btnRefresh.TabIndex = 2;
            this.btnRefresh.Text = "Odśwież";
            this.btnRefresh.UseVisualStyleBackColor = true;
            this.btnRefresh.Click += new System.EventHandler(this.btnRefresh_Click);
            // 
            // cmbAccounts
            // 
            this.cmbAccounts.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmbAccounts.FormattingEnabled = true;
            this.cmbAccounts.Location = new System.Drawing.Point(120, 13);
            this.cmbAccounts.Name = "cmbAccounts";
            this.cmbAccounts.Size = new System.Drawing.Size(230, 24);
            this.cmbAccounts.TabIndex = 1;
            this.cmbAccounts.SelectedIndexChanged += new System.EventHandler(this.cmbAccounts_SelectedIndexChanged);
            // 
            // lblAccount
            // 
            this.lblAccount.AutoSize = true;
            this.lblAccount.Location = new System.Drawing.Point(12, 16);
            this.lblAccount.Name = "lblAccount";
            this.lblAccount.Size = new System.Drawing.Size(95, 16);
            this.lblAccount.TabIndex = 0;
            this.lblAccount.Text = "Wybierz konto:";
            // 
            // dgvDisputes
            // 
            this.dgvDisputes.AllowUserToAddRows = false;
            this.dgvDisputes.AllowUserToDeleteRows = false;
            this.dgvDisputes.BackgroundColor = System.Drawing.SystemColors.Control;
            this.dgvDisputes.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.dgvDisputes.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            dataGridViewCellStyle1.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft;
            dataGridViewCellStyle1.BackColor = System.Drawing.SystemColors.Window;
            dataGridViewCellStyle1.Font = new System.Drawing.Font("Microsoft Sans Serif", 7.8F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(238)));
            dataGridViewCellStyle1.ForeColor = System.Drawing.SystemColors.ControlText;
            dataGridViewCellStyle1.SelectionBackColor = System.Drawing.SystemColors.Highlight;
            dataGridViewCellStyle1.SelectionForeColor = System.Drawing.SystemColors.HighlightText;
            dataGridViewCellStyle1.WrapMode = System.Windows.Forms.DataGridViewTriState.False;
            this.dgvDisputes.DefaultCellStyle = dataGridViewCellStyle1;
            this.dgvDisputes.Dock = System.Windows.Forms.DockStyle.Fill;
            this.dgvDisputes.Location = new System.Drawing.Point(0, 50);
            this.dgvDisputes.Name = "dgvDisputes";
            this.dgvDisputes.ReadOnly = true;
            this.dgvDisputes.RowHeadersVisible = false;
            this.dgvDisputes.RowHeadersWidth = 51;
            this.dgvDisputes.RowTemplate.Height = 24;
            this.dgvDisputes.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
            this.dgvDisputes.Size = new System.Drawing.Size(982, 503);
            this.dgvDisputes.TabIndex = 1;
            this.dgvDisputes.CellContentClick += new System.Windows.Forms.DataGridViewCellEventHandler(this.dgvDisputes_CellContentClick);
            this.dgvDisputes.CellDoubleClick += new System.Windows.Forms.DataGridViewCellEventHandler(this.dgvDisputes_CellDoubleClick);
            // 
            // FormAllegroManager
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(982, 553);
            this.Controls.Add(this.dgvDisputes);
            this.Controls.Add(this.panelTop);
            this.MinimumSize = new System.Drawing.Size(800, 600);
            this.Name = "FormAllegroManager";
            this.Text = "Manager Dyskusji Allegro";
            this.panelTop.ResumeLayout(false);
            this.panelTop.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dgvDisputes)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Panel panelTop;
        private System.Windows.Forms.ComboBox cmbAccounts;
        private System.Windows.Forms.Label lblAccount;
        private System.Windows.Forms.DataGridView dgvDisputes;
        private System.Windows.Forms.Button btnRefresh;
    }
}