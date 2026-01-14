using System;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;

namespace Reklamacje_Dane
{
    partial class FormAutoMatchFv
    {
        private IContainer components = null;
        private Panel panelTop;
        private Button btnScan;
        private Button btnSave;
        private CheckBox chkYear2022;
        private CheckBox chkYear2023;
        private CheckBox chkYear2024;
        private CheckBox chkYear2025;
        private CheckBox chkIncludeClient;
        private NumericUpDown nudScoreThreshold;
        private NumericUpDown nudMinDelta;
        private Label lblThreshold;
        private Label lblMinDelta;
        private ProgressBar progressBar;
        private Label lblStatus;
        private TabControl tabResults;
        private TabPage tabProposed;
        private TabPage tabReview;
        private DataGridView dgvProposed;
        private DataGridView dgvReview;
        private Label lblYears;

        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            this.panelTop = new System.Windows.Forms.Panel();
            this.btnScan = new System.Windows.Forms.Button();
            this.btnSave = new System.Windows.Forms.Button();
            this.chkYear2022 = new System.Windows.Forms.CheckBox();
            this.chkYear2023 = new System.Windows.Forms.CheckBox();
            this.chkYear2024 = new System.Windows.Forms.CheckBox();
            this.chkYear2025 = new System.Windows.Forms.CheckBox();
            this.chkIncludeClient = new System.Windows.Forms.CheckBox();
            this.nudScoreThreshold = new System.Windows.Forms.NumericUpDown();
            this.nudMinDelta = new System.Windows.Forms.NumericUpDown();
            this.lblThreshold = new System.Windows.Forms.Label();
            this.lblMinDelta = new System.Windows.Forms.Label();
            this.progressBar = new System.Windows.Forms.ProgressBar();
            this.lblStatus = new System.Windows.Forms.Label();
            this.tabResults = new System.Windows.Forms.TabControl();
            this.tabProposed = new System.Windows.Forms.TabPage();
            this.dgvProposed = new System.Windows.Forms.DataGridView();
            this.tabReview = new System.Windows.Forms.TabPage();
            this.dgvReview = new System.Windows.Forms.DataGridView();
            this.lblYears = new System.Windows.Forms.Label();
            this.panelTop.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.nudScoreThreshold)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.nudMinDelta)).BeginInit();
            this.tabResults.SuspendLayout();
            this.tabProposed.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dgvProposed)).BeginInit();
            this.tabReview.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dgvReview)).BeginInit();
            this.SuspendLayout();
            // 
            // panelTop
            // 
            this.panelTop.BackColor = System.Drawing.Color.WhiteSmoke;
            this.panelTop.Controls.Add(this.btnScan);
            this.panelTop.Controls.Add(this.btnSave);
            this.panelTop.Controls.Add(this.chkYear2022);
            this.panelTop.Controls.Add(this.chkYear2023);
            this.panelTop.Controls.Add(this.chkYear2024);
            this.panelTop.Controls.Add(this.chkYear2025);
            this.panelTop.Controls.Add(this.chkIncludeClient);
            this.panelTop.Controls.Add(this.nudScoreThreshold);
            this.panelTop.Controls.Add(this.nudMinDelta);
            this.panelTop.Controls.Add(this.lblThreshold);
            this.panelTop.Controls.Add(this.lblMinDelta);
            this.panelTop.Controls.Add(this.progressBar);
            this.panelTop.Controls.Add(this.lblStatus);
            this.panelTop.Controls.Add(this.lblYears);
            this.panelTop.Dock = System.Windows.Forms.DockStyle.Top;
            this.panelTop.Location = new System.Drawing.Point(0, 0);
            this.panelTop.Name = "panelTop";
            this.panelTop.Padding = new System.Windows.Forms.Padding(12);
            this.panelTop.Size = new System.Drawing.Size(1182, 150);
            this.panelTop.TabIndex = 0;
            // 
            // btnScan
            // 
            this.btnScan.Location = new System.Drawing.Point(14, 15);
            this.btnScan.Name = "btnScan";
            this.btnScan.Size = new System.Drawing.Size(220, 36);
            this.btnScan.TabIndex = 0;
            this.btnScan.Text = "Skanuj (bez zapisu) – podgląd";
            this.btnScan.UseVisualStyleBackColor = true;
            this.btnScan.Click += new System.EventHandler(this.btnScan_Click);
            // 
            // btnSave
            // 
            this.btnSave.Location = new System.Drawing.Point(240, 15);
            this.btnSave.Name = "btnSave";
            this.btnSave.Size = new System.Drawing.Size(220, 36);
            this.btnSave.TabIndex = 1;
            this.btnSave.Text = "Zapisz uzupełnienia do bazy";
            this.btnSave.UseVisualStyleBackColor = true;
            this.btnSave.Click += new System.EventHandler(this.btnSave_Click);
            // 
            // lblYears
            // 
            this.lblYears.AutoSize = true;
            this.lblYears.Location = new System.Drawing.Point(12, 67);
            this.lblYears.Name = "lblYears";
            this.lblYears.Size = new System.Drawing.Size(86, 20);
            this.lblYears.TabIndex = 2;
            this.lblYears.Text = "Raporty lat:";
            // 
            // chkYear2022
            // 
            this.chkYear2022.AutoSize = true;
            this.chkYear2022.Location = new System.Drawing.Point(104, 66);
            this.chkYear2022.Name = "chkYear2022";
            this.chkYear2022.Size = new System.Drawing.Size(60, 24);
            this.chkYear2022.TabIndex = 3;
            this.chkYear2022.Text = "2022";
            this.chkYear2022.UseVisualStyleBackColor = true;
            // 
            // chkYear2023
            // 
            this.chkYear2023.AutoSize = true;
            this.chkYear2023.Location = new System.Drawing.Point(170, 66);
            this.chkYear2023.Name = "chkYear2023";
            this.chkYear2023.Size = new System.Drawing.Size(60, 24);
            this.chkYear2023.TabIndex = 4;
            this.chkYear2023.Text = "2023";
            this.chkYear2023.UseVisualStyleBackColor = true;
            // 
            // chkYear2024
            // 
            this.chkYear2024.AutoSize = true;
            this.chkYear2024.Location = new System.Drawing.Point(236, 66);
            this.chkYear2024.Name = "chkYear2024";
            this.chkYear2024.Size = new System.Drawing.Size(60, 24);
            this.chkYear2024.TabIndex = 5;
            this.chkYear2024.Text = "2024";
            this.chkYear2024.UseVisualStyleBackColor = true;
            // 
            // chkYear2025
            // 
            this.chkYear2025.AutoSize = true;
            this.chkYear2025.Location = new System.Drawing.Point(302, 66);
            this.chkYear2025.Name = "chkYear2025";
            this.chkYear2025.Size = new System.Drawing.Size(60, 24);
            this.chkYear2025.TabIndex = 6;
            this.chkYear2025.Text = "2025";
            this.chkYear2025.UseVisualStyleBackColor = true;
            // 
            // chkIncludeClient
            // 
            this.chkIncludeClient.AutoSize = true;
            this.chkIncludeClient.Location = new System.Drawing.Point(14, 100);
            this.chkIncludeClient.Name = "chkIncludeClient";
            this.chkIncludeClient.Size = new System.Drawing.Size(435, 24);
            this.chkIncludeClient.TabIndex = 7;
            this.chkIncludeClient.Text = "Uwzględniaj dopasowanie klienta (kod/miejscowość/nazwa)";
            this.chkIncludeClient.UseVisualStyleBackColor = true;
            // 
            // nudScoreThreshold
            // 
            this.nudScoreThreshold.Location = new System.Drawing.Point(542, 25);
            this.nudScoreThreshold.Maximum = new decimal(new int[] {
            500,
            0,
            0,
            0});
            this.nudScoreThreshold.Minimum = new decimal(new int[] {
            0,
            0,
            0,
            0});
            this.nudScoreThreshold.Name = "nudScoreThreshold";
            this.nudScoreThreshold.Size = new System.Drawing.Size(90, 27);
            this.nudScoreThreshold.TabIndex = 8;
            // 
            // nudMinDelta
            // 
            this.nudMinDelta.Location = new System.Drawing.Point(542, 63);
            this.nudMinDelta.Maximum = new decimal(new int[] {
            500,
            0,
            0,
            0});
            this.nudMinDelta.Minimum = new decimal(new int[] {
            0,
            0,
            0,
            0});
            this.nudMinDelta.Name = "nudMinDelta";
            this.nudMinDelta.Size = new System.Drawing.Size(90, 27);
            this.nudMinDelta.TabIndex = 9;
            // 
            // lblThreshold
            // 
            this.lblThreshold.AutoSize = true;
            this.lblThreshold.Location = new System.Drawing.Point(430, 27);
            this.lblThreshold.Name = "lblThreshold";
            this.lblThreshold.Size = new System.Drawing.Size(104, 20);
            this.lblThreshold.TabIndex = 10;
            this.lblThreshold.Text = "Próg pewności";
            // 
            // lblMinDelta
            // 
            this.lblMinDelta.AutoSize = true;
            this.lblMinDelta.Location = new System.Drawing.Point(390, 65);
            this.lblMinDelta.Name = "lblMinDelta";
            this.lblMinDelta.Size = new System.Drawing.Size(144, 20);
            this.lblMinDelta.TabIndex = 11;
            this.lblMinDelta.Text = "Minimalna przewaga";
            // 
            // progressBar
            // 
            this.progressBar.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.progressBar.Location = new System.Drawing.Point(675, 25);
            this.progressBar.Name = "progressBar";
            this.progressBar.Size = new System.Drawing.Size(495, 23);
            this.progressBar.TabIndex = 12;
            // 
            // lblStatus
            // 
            this.lblStatus.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.lblStatus.Location = new System.Drawing.Point(672, 63);
            this.lblStatus.Name = "lblStatus";
            this.lblStatus.Size = new System.Drawing.Size(498, 60);
            this.lblStatus.TabIndex = 13;
            this.lblStatus.Text = "Status: gotowy";
            // 
            // tabResults
            // 
            this.tabResults.Controls.Add(this.tabProposed);
            this.tabResults.Controls.Add(this.tabReview);
            this.tabResults.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tabResults.Location = new System.Drawing.Point(0, 150);
            this.tabResults.Name = "tabResults";
            this.tabResults.SelectedIndex = 0;
            this.tabResults.Size = new System.Drawing.Size(1182, 553);
            this.tabResults.TabIndex = 1;
            // 
            // tabProposed
            // 
            this.tabProposed.Controls.Add(this.dgvProposed);
            this.tabProposed.Location = new System.Drawing.Point(4, 29);
            this.tabProposed.Name = "tabProposed";
            this.tabProposed.Padding = new System.Windows.Forms.Padding(3);
            this.tabProposed.Size = new System.Drawing.Size(1174, 520);
            this.tabProposed.TabIndex = 0;
            this.tabProposed.Text = "Proponowane uzupełnienia";
            this.tabProposed.UseVisualStyleBackColor = true;
            // 
            // dgvProposed
            // 
            this.dgvProposed.AllowUserToAddRows = false;
            this.dgvProposed.AllowUserToDeleteRows = false;
            this.dgvProposed.AutoSizeColumnsMode = System.Windows.Forms.DataGridViewAutoSizeColumnsMode.Fill;
            this.dgvProposed.Dock = System.Windows.Forms.DockStyle.Fill;
            this.dgvProposed.Location = new System.Drawing.Point(3, 3);
            this.dgvProposed.Name = "dgvProposed";
            this.dgvProposed.ReadOnly = true;
            this.dgvProposed.RowHeadersVisible = false;
            this.dgvProposed.Size = new System.Drawing.Size(1168, 514);
            this.dgvProposed.TabIndex = 0;
            // 
            // tabReview
            // 
            this.tabReview.Controls.Add(this.dgvReview);
            this.tabReview.Location = new System.Drawing.Point(4, 29);
            this.tabReview.Name = "tabReview";
            this.tabReview.Padding = new System.Windows.Forms.Padding(3);
            this.tabReview.Size = new System.Drawing.Size(1174, 520);
            this.tabReview.TabIndex = 1;
            this.tabReview.Text = "Do ręcznej weryfikacji";
            this.tabReview.UseVisualStyleBackColor = true;
            // 
            // dgvReview
            // 
            this.dgvReview.AllowUserToAddRows = false;
            this.dgvReview.AllowUserToDeleteRows = false;
            this.dgvReview.AutoSizeColumnsMode = System.Windows.Forms.DataGridViewAutoSizeColumnsMode.Fill;
            this.dgvReview.Dock = System.Windows.Forms.DockStyle.Fill;
            this.dgvReview.Location = new System.Drawing.Point(3, 3);
            this.dgvReview.Name = "dgvReview";
            this.dgvReview.ReadOnly = true;
            this.dgvReview.RowHeadersVisible = false;
            this.dgvReview.Size = new System.Drawing.Size(1168, 514);
            this.dgvReview.TabIndex = 0;
            // 
            // FormAutoMatchFv
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 20F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1182, 703);
            this.Controls.Add(this.tabResults);
            this.Controls.Add(this.panelTop);
            this.Font = new System.Drawing.Font("Segoe UI", 9F);
            this.Name = "FormAutoMatchFv";
            this.Text = "Automatyczne uzupełnianie NrFaktury/DataZakupu";
            this.panelTop.ResumeLayout(false);
            this.panelTop.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.nudScoreThreshold)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.nudMinDelta)).EndInit();
            this.tabResults.ResumeLayout(false);
            this.tabProposed.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.dgvProposed)).EndInit();
            this.tabReview.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.dgvReview)).EndInit();
            this.ResumeLayout(false);
        }
    }
}
