// Plik: HandlowiecControl.Designer.cs
// Opis: Nowoczesny interfejs dla modułu Handlowca, spójny wizualnie
//       z modułem Magazynu.

namespace Reklamacje_Dane
{
    partial class HandlowiecControl
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

        #region Component Designer generated code

        private void InitializeComponent()
        {
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle1 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle2 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle3 = new System.Windows.Forms.DataGridViewCellStyle();
            this.panelTop = new System.Windows.Forms.Panel();
            this.panelFiltryButtons = new System.Windows.Forms.Panel();
            this.btnFilterZakonczone = new System.Windows.Forms.Button();
            this.btnFilterDoDecyzji = new System.Windows.Forms.Button();
            this.lblLastRefresh = new System.Windows.Forms.Label();
            this.mainSplitContainer = new System.Windows.Forms.SplitContainer();
            this.dgvReturns = new System.Windows.Forms.DataGridView();
            this.panelSearch = new System.Windows.Forms.Panel();
            this.lblTotalCount = new System.Windows.Forms.Label();
            this.txtSearch = new System.Windows.Forms.TextBox();
            this.label2 = new System.Windows.Forms.Label();
            this.panelKomunikator = new System.Windows.Forms.Panel();
            this.refreshIcon = new System.Windows.Forms.PictureBox();
            this.panelTop.SuspendLayout();
            this.panelFiltryButtons.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.mainSplitContainer)).BeginInit();
            this.mainSplitContainer.Panel1.SuspendLayout();
            this.mainSplitContainer.Panel2.SuspendLayout();
            this.mainSplitContainer.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dgvReturns)).BeginInit();
            this.panelSearch.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.refreshIcon)).BeginInit();
            this.SuspendLayout();
            // 
            // panelTop
            // 
            this.panelTop.BackColor = System.Drawing.Color.WhiteSmoke;
            this.panelTop.Controls.Add(this.panelFiltryButtons);
            this.panelTop.Controls.Add(this.lblLastRefresh);
            this.panelTop.Controls.Add(this.refreshIcon);
            this.panelTop.Dock = System.Windows.Forms.DockStyle.Top;
            this.panelTop.Location = new System.Drawing.Point(0, 0);
            this.panelTop.Margin = new System.Windows.Forms.Padding(2, 2, 2, 2);
            this.panelTop.Name = "panelTop";
            this.panelTop.Size = new System.Drawing.Size(900, 43);
            this.panelTop.TabIndex = 5;
            // 
            // panelFiltryButtons
            // 
            this.panelFiltryButtons.Controls.Add(this.btnFilterZakonczone);
            this.panelFiltryButtons.Controls.Add(this.btnFilterDoDecyzji);
            this.panelFiltryButtons.Location = new System.Drawing.Point(8, 4);
            this.panelFiltryButtons.Margin = new System.Windows.Forms.Padding(2, 2, 2, 2);
            this.panelFiltryButtons.Name = "panelFiltryButtons";
            this.panelFiltryButtons.Size = new System.Drawing.Size(450, 32);
            this.panelFiltryButtons.TabIndex = 5;
            // 
            // btnFilterZakonczone
            // 
            this.btnFilterZakonczone.Dock = System.Windows.Forms.DockStyle.Left;
            this.btnFilterZakonczone.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnFilterZakonczone.Font = new System.Drawing.Font("Segoe UI", 9F);
            this.btnFilterZakonczone.Location = new System.Drawing.Point(150, 0);
            this.btnFilterZakonczone.Margin = new System.Windows.Forms.Padding(2, 2, 2, 2);
            this.btnFilterZakonczone.Name = "btnFilterZakonczone";
            this.btnFilterZakonczone.Size = new System.Drawing.Size(150, 32);
            this.btnFilterZakonczone.TabIndex = 1;
            this.btnFilterZakonczone.Text = "Sprawy zakończone";
            this.btnFilterZakonczone.UseVisualStyleBackColor = true;
            // 
            // btnFilterDoDecyzji
            // 
            this.btnFilterDoDecyzji.Dock = System.Windows.Forms.DockStyle.Left;
            this.btnFilterDoDecyzji.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnFilterDoDecyzji.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Bold);
            this.btnFilterDoDecyzji.Location = new System.Drawing.Point(0, 0);
            this.btnFilterDoDecyzji.Margin = new System.Windows.Forms.Padding(2, 2, 2, 2);
            this.btnFilterDoDecyzji.Name = "btnFilterDoDecyzji";
            this.btnFilterDoDecyzji.Size = new System.Drawing.Size(150, 32);
            this.btnFilterDoDecyzji.TabIndex = 0;
            this.btnFilterDoDecyzji.Text = "Nowe sprawy (do decyzji)";
            this.btnFilterDoDecyzji.UseVisualStyleBackColor = true;
            // 
            // lblLastRefresh
            // 
            this.lblLastRefresh.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.lblLastRefresh.AutoSize = true;
            this.lblLastRefresh.Font = new System.Drawing.Font("Segoe UI", 9F);
            this.lblLastRefresh.Location = new System.Drawing.Point(736, 13);
            this.lblLastRefresh.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.lblLastRefresh.Name = "lblLastRefresh";
            this.lblLastRefresh.Size = new System.Drawing.Size(147, 15);
            this.lblLastRefresh.TabIndex = 4;
            this.lblLastRefresh.Text = "Ostatnie odświeżenie: brak";
            // 
            // mainSplitContainer
            // 
            this.mainSplitContainer.Dock = System.Windows.Forms.DockStyle.Fill;
            this.mainSplitContainer.Location = new System.Drawing.Point(0, 43);
            this.mainSplitContainer.Margin = new System.Windows.Forms.Padding(2, 2, 2, 2);
            this.mainSplitContainer.Name = "mainSplitContainer";
            this.mainSplitContainer.Orientation = System.Windows.Forms.Orientation.Horizontal;
            // 
            // mainSplitContainer.Panel1
            // 
            this.mainSplitContainer.Panel1.Controls.Add(this.dgvReturns);
            this.mainSplitContainer.Panel1.Controls.Add(this.panelSearch);
            // 
            // mainSplitContainer.Panel2
            // 
            this.mainSplitContainer.Panel2.Controls.Add(this.panelKomunikator);
            this.mainSplitContainer.Size = new System.Drawing.Size(900, 569);
            this.mainSplitContainer.SplitterDistance = 365;
            this.mainSplitContainer.SplitterWidth = 3;
            this.mainSplitContainer.TabIndex = 6;
            // 
            // dgvReturns
            // 
            this.dgvReturns.AllowUserToAddRows = false;
            this.dgvReturns.AllowUserToDeleteRows = false;
            this.dgvReturns.AllowUserToResizeRows = false;
            dataGridViewCellStyle1.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(240)))), ((int)(((byte)(240)))), ((int)(((byte)(240)))));
            this.dgvReturns.AlternatingRowsDefaultCellStyle = dataGridViewCellStyle1;
            this.dgvReturns.BackgroundColor = System.Drawing.Color.FromArgb(((int)(((byte)(224)))), ((int)(((byte)(224)))), ((int)(((byte)(224)))));
            this.dgvReturns.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.dgvReturns.CellBorderStyle = System.Windows.Forms.DataGridViewCellBorderStyle.SingleHorizontal;
            dataGridViewCellStyle2.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft;
            dataGridViewCellStyle2.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(68)))), ((int)(((byte)(114)))), ((int)(((byte)(196)))));
            dataGridViewCellStyle2.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Bold);
            dataGridViewCellStyle2.ForeColor = System.Drawing.Color.White;
            dataGridViewCellStyle2.SelectionBackColor = System.Drawing.SystemColors.Highlight;
            dataGridViewCellStyle2.SelectionForeColor = System.Drawing.SystemColors.HighlightText;
            dataGridViewCellStyle2.WrapMode = System.Windows.Forms.DataGridViewTriState.True;
            this.dgvReturns.ColumnHeadersDefaultCellStyle = dataGridViewCellStyle2;
            this.dgvReturns.ColumnHeadersHeight = 30;
            this.dgvReturns.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.DisableResizing;
            dataGridViewCellStyle3.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft;
            dataGridViewCellStyle3.BackColor = System.Drawing.Color.White;
            dataGridViewCellStyle3.Font = new System.Drawing.Font("Segoe UI", 8.25F);
            dataGridViewCellStyle3.ForeColor = System.Drawing.SystemColors.ControlText;
            dataGridViewCellStyle3.SelectionBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(33)))), ((int)(((byte)(150)))), ((int)(((byte)(243)))));
            dataGridViewCellStyle3.SelectionForeColor = System.Drawing.Color.White;
            dataGridViewCellStyle3.WrapMode = System.Windows.Forms.DataGridViewTriState.False;
            this.dgvReturns.DefaultCellStyle = dataGridViewCellStyle3;
            this.dgvReturns.Dock = System.Windows.Forms.DockStyle.Fill;
            this.dgvReturns.EnableHeadersVisualStyles = false;
            this.dgvReturns.Location = new System.Drawing.Point(0, 0);
            this.dgvReturns.Margin = new System.Windows.Forms.Padding(2, 2, 2, 2);
            this.dgvReturns.Name = "dgvReturns";
            this.dgvReturns.ReadOnly = true;
            this.dgvReturns.RowHeadersVisible = false;
            this.dgvReturns.RowTemplate.Height = 28;
            this.dgvReturns.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
            this.dgvReturns.Size = new System.Drawing.Size(712, 365);
            this.dgvReturns.TabIndex = 3;
            this.dgvReturns.CellContentClick += new System.Windows.Forms.DataGridViewCellEventHandler(this.dgvReturns_CellContentClick);
            // 
            // panelSearch
            // 
            this.panelSearch.BackColor = System.Drawing.Color.WhiteSmoke;
            this.panelSearch.Controls.Add(this.lblTotalCount);
            this.panelSearch.Controls.Add(this.txtSearch);
            this.panelSearch.Controls.Add(this.label2);
            this.panelSearch.Dock = System.Windows.Forms.DockStyle.Right;
            this.panelSearch.Location = new System.Drawing.Point(712, 0);
            this.panelSearch.Margin = new System.Windows.Forms.Padding(2, 2, 2, 2);
            this.panelSearch.MinimumSize = new System.Drawing.Size(188, 0);
            this.panelSearch.Name = "panelSearch";
            this.panelSearch.Size = new System.Drawing.Size(188, 365);
            this.panelSearch.TabIndex = 2;
            // 
            // lblTotalCount
            // 
            this.lblTotalCount.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.lblTotalCount.AutoSize = true;
            this.lblTotalCount.Font = new System.Drawing.Font("Segoe UI", 9F);
            this.lblTotalCount.Location = new System.Drawing.Point(5, 69);
            this.lblTotalCount.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.lblTotalCount.Name = "lblTotalCount";
            this.lblTotalCount.Size = new System.Drawing.Size(121, 15);
            this.lblTotalCount.TabIndex = 5;
            this.lblTotalCount.Text = "Wyświetlono: 0 spraw";
            // 
            // txtSearch
            // 
            this.txtSearch.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.txtSearch.Font = new System.Drawing.Font("Segoe UI", 9F);
            this.txtSearch.Location = new System.Drawing.Point(8, 32);
            this.txtSearch.Margin = new System.Windows.Forms.Padding(2, 2, 2, 2);
            this.txtSearch.Name = "txtSearch";
            this.txtSearch.Size = new System.Drawing.Size(174, 23);
            this.txtSearch.TabIndex = 1;
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Font = new System.Drawing.Font("Segoe UI Semibold", 9F, System.Drawing.FontStyle.Bold);
            this.label2.Location = new System.Drawing.Point(5, 14);
            this.label2.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(101, 15);
            this.label2.TabIndex = 0;
            this.label2.Text = "Wyszukaj sprawę:";
            // 
            // panelKomunikator
            // 
            this.panelKomunikator.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panelKomunikator.Location = new System.Drawing.Point(0, 0);
            this.panelKomunikator.Margin = new System.Windows.Forms.Padding(2, 2, 2, 2);
            this.panelKomunikator.Name = "panelKomunikator";
            this.panelKomunikator.Size = new System.Drawing.Size(900, 201);
            this.panelKomunikator.TabIndex = 0;
            // 
            // refreshIcon
            // 
            this.refreshIcon.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.refreshIcon.Cursor = System.Windows.Forms.Cursors.Hand;
            this.refreshIcon.Location = new System.Drawing.Point(716, 12);
            this.refreshIcon.Margin = new System.Windows.Forms.Padding(2);
            this.refreshIcon.Name = "refreshIcon";
            this.refreshIcon.Size = new System.Drawing.Size(16, 18);
            this.refreshIcon.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage;
            this.refreshIcon.TabIndex = 3;
            this.refreshIcon.TabStop = false;
            // 
            // HandlowiecControl
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.White;
            this.Controls.Add(this.mainSplitContainer);
            this.Controls.Add(this.panelTop);
            this.Margin = new System.Windows.Forms.Padding(2, 2, 2, 2);
            this.Name = "HandlowiecControl";
            this.Size = new System.Drawing.Size(900, 612);
            this.Load += new System.EventHandler(this.HandlowiecControl_Load);
            this.panelTop.ResumeLayout(false);
            this.panelTop.PerformLayout();
            this.panelFiltryButtons.ResumeLayout(false);
            this.mainSplitContainer.Panel1.ResumeLayout(false);
            this.mainSplitContainer.Panel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.mainSplitContainer)).EndInit();
            this.mainSplitContainer.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.dgvReturns)).EndInit();
            this.panelSearch.ResumeLayout(false);
            this.panelSearch.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.refreshIcon)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Panel panelTop;
        private System.Windows.Forms.Panel panelFiltryButtons;
        private System.Windows.Forms.Button btnFilterZakonczone;
        private System.Windows.Forms.Button btnFilterDoDecyzji;
        private System.Windows.Forms.Label lblLastRefresh;
        private System.Windows.Forms.PictureBox refreshIcon;
        private System.Windows.Forms.SplitContainer mainSplitContainer;
        private System.Windows.Forms.DataGridView dgvReturns;
        private System.Windows.Forms.Panel panelSearch;
        private System.Windows.Forms.Label lblTotalCount;
        private System.Windows.Forms.TextBox txtSearch;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Panel panelKomunikator;
    }
}