namespace Reklamacje_Dane
{
    partial class FormUploader
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
            this.panelDropZone = new System.Windows.Forms.Panel();
            this.labelDrop = new System.Windows.Forms.Label();
            this.btnBrowse = new System.Windows.Forms.Button();
            this.listBoxFiles = new System.Windows.Forms.ListBox();
            this.labelTitle = new System.Windows.Forms.Label();
            this.pictureBoxPreview = new System.Windows.Forms.PictureBox();
            this.progressBarCopy = new System.Windows.Forms.ProgressBar();
            this.lblPreview = new System.Windows.Forms.Label();
            this.btnFromPhone = new System.Windows.Forms.Button();
            this.panelDropZone.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBoxPreview)).BeginInit();
            this.SuspendLayout();
            // 
            // panelDropZone
            // 
            this.panelDropZone.AllowDrop = true;
            this.panelDropZone.BackColor = System.Drawing.Color.WhiteSmoke;
            this.panelDropZone.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.panelDropZone.Controls.Add(this.labelDrop);
            this.panelDropZone.Location = new System.Drawing.Point(12, 40);
            this.panelDropZone.Name = "panelDropZone";
            this.panelDropZone.Size = new System.Drawing.Size(760, 100);
            this.panelDropZone.TabIndex = 0;
            this.panelDropZone.DragDrop += new System.Windows.Forms.DragEventHandler(this.panelDropZone_DragDrop);
            this.panelDropZone.DragEnter += new System.Windows.Forms.DragEventHandler(this.panelDropZone_DragEnter);
            this.panelDropZone.DragLeave += new System.EventHandler(this.panelDropZone_DragLeave);
            // 
            // labelDrop
            // 
            this.labelDrop.Dock = System.Windows.Forms.DockStyle.Fill;
            this.labelDrop.Font = new System.Drawing.Font("Segoe UI", 10F);
            this.labelDrop.ForeColor = System.Drawing.Color.Gray;
            this.labelDrop.Location = new System.Drawing.Point(0, 0);
            this.labelDrop.Name = "labelDrop";
            this.labelDrop.Size = new System.Drawing.Size(758, 98);
            this.labelDrop.TabIndex = 0;
            this.labelDrop.Text = "Przeciągnij i upuść pliki tutaj";
            this.labelDrop.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // btnBrowse
            // 
            this.btnBrowse.Font = new System.Drawing.Font("Segoe UI", 9F);
            this.btnBrowse.Location = new System.Drawing.Point(652, 146);
            this.btnBrowse.Name = "btnBrowse";
            this.btnBrowse.Size = new System.Drawing.Size(120, 30);
            this.btnBrowse.TabIndex = 1;
            this.btnBrowse.Text = "Przeglądaj...";
            this.btnBrowse.UseVisualStyleBackColor = true;
            this.btnBrowse.Click += new System.EventHandler(this.btnBrowse_Click);
            // 
            // listBoxFiles
            // 
            this.listBoxFiles.Font = new System.Drawing.Font("Segoe UI", 9F);
            this.listBoxFiles.FormattingEnabled = true;
            this.listBoxFiles.ItemHeight = 20;
            this.listBoxFiles.Location = new System.Drawing.Point(12, 182);
            this.listBoxFiles.Name = "listBoxFiles";
            this.listBoxFiles.Size = new System.Drawing.Size(360, 244);
            this.listBoxFiles.TabIndex = 2;
            this.listBoxFiles.SelectedIndexChanged += new System.EventHandler(this.listBoxFiles_SelectedIndexChanged);
            // 
            // labelTitle
            // 
            this.labelTitle.AutoSize = true;
            this.labelTitle.Font = new System.Drawing.Font("Segoe UI Semibold", 10.2F, System.Drawing.FontStyle.Bold);
            this.labelTitle.Location = new System.Drawing.Point(12, 9);
            this.labelTitle.Name = "labelTitle";
            this.labelTitle.Size = new System.Drawing.Size(248, 23);
            this.labelTitle.TabIndex = 3;
            this.labelTitle.Text = "Dodaj załączniki do zgłoszenia:";
            // 
            // pictureBoxPreview
            // 
            this.pictureBoxPreview.BackColor = System.Drawing.Color.WhiteSmoke;
            this.pictureBoxPreview.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.pictureBoxPreview.Location = new System.Drawing.Point(390, 211);
            this.pictureBoxPreview.Name = "pictureBoxPreview";
            this.pictureBoxPreview.Size = new System.Drawing.Size(382, 215);
            this.pictureBoxPreview.SizeMode = System.Windows.Forms.PictureBoxSizeMode.Zoom;
            this.pictureBoxPreview.TabIndex = 4;
            this.pictureBoxPreview.TabStop = false;
            // 
            // progressBarCopy
            // 
            this.progressBarCopy.Location = new System.Drawing.Point(12, 442);
            this.progressBarCopy.Name = "progressBarCopy";
            this.progressBarCopy.Size = new System.Drawing.Size(760, 23);
            this.progressBarCopy.TabIndex = 5;
            this.progressBarCopy.Visible = false;
            // 
            // lblPreview
            // 
            this.lblPreview.AutoSize = true;
            this.lblPreview.Font = new System.Drawing.Font("Segoe UI", 9F);
            this.lblPreview.Location = new System.Drawing.Point(386, 182);
            this.lblPreview.Name = "lblPreview";
            this.lblPreview.Size = new System.Drawing.Size(67, 20);
            this.lblPreview.TabIndex = 6;
            this.lblPreview.Text = "Podgląd:";
            // 
            // btnFromPhone
            // 
            this.btnFromPhone.Font = new System.Drawing.Font("Segoe UI", 9F);
            this.btnFromPhone.Location = new System.Drawing.Point(621, 182);
            this.btnFromPhone.Name = "btnFromPhone";
            this.btnFromPhone.Size = new System.Drawing.Size(150, 30);
            this.btnFromPhone.TabIndex = 7;
            this.btnFromPhone.Text = "+ dodaj z telefonu";
            this.btnFromPhone.UseVisualStyleBackColor = true;
            this.btnFromPhone.Click += new System.EventHandler(this.btnFromPhone_Click);
            // 
            // FormUploader
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(784, 477);
            this.Controls.Add(this.btnFromPhone);
            this.Controls.Add(this.lblPreview);
            this.Controls.Add(this.progressBarCopy);
            this.Controls.Add(this.pictureBoxPreview);
            this.Controls.Add(this.labelTitle);
            this.Controls.Add(this.listBoxFiles);
            this.Controls.Add(this.btnBrowse);
            this.Controls.Add(this.panelDropZone);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedToolWindow;
            this.Name = "FormUploader";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Dodawanie załączników";
            this.panelDropZone.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.pictureBoxPreview)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }
        #endregion

        private System.Windows.Forms.Panel panelDropZone;
        private System.Windows.Forms.Label labelDrop;
        private System.Windows.Forms.Button btnBrowse;
        private System.Windows.Forms.ListBox listBoxFiles;
        private System.Windows.Forms.Label labelTitle;
        private System.Windows.Forms.PictureBox pictureBoxPreview;
        private System.Windows.Forms.ProgressBar progressBarCopy;
        private System.Windows.Forms.Label lblPreview;
        private System.Windows.Forms.Button btnFromPhone;
    }
}