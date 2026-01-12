namespace Reklamacje_Dane
{
    partial class FormFileViewer
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
            this.splitContainer = new System.Windows.Forms.SplitContainer();
            this.listViewFiles = new System.Windows.Forms.ListView();
            this.imageListLarge = new System.Windows.Forms.ImageList(this.components);
            this.panelRight = new System.Windows.Forms.Panel();
            this.btnRotateRight = new System.Windows.Forms.Button();
            this.btnRotateLeft = new System.Windows.Forms.Button();
            this.lblFileName = new System.Windows.Forms.Label();
            this.btnOpenFolder = new System.Windows.Forms.Button();
            this.btnDelete = new System.Windows.Forms.Button();
            this.btnOpen = new System.Windows.Forms.Button();
            this.panelPreviewContainer = new System.Windows.Forms.Panel();
            this.webView2Preview = new Microsoft.Web.WebView2.WinForms.WebView2();
            this.elementHostVideo = new System.Windows.Forms.Integration.ElementHost();
            this.panelImageContainer = new System.Windows.Forms.Panel();
            this.pictureBoxPreview = new System.Windows.Forms.PictureBox();
            this.lblPreviewNotAvailable = new System.Windows.Forms.Label();
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer)).BeginInit();
            this.splitContainer.Panel1.SuspendLayout();
            this.splitContainer.Panel2.SuspendLayout();
            this.splitContainer.SuspendLayout();
            this.panelRight.SuspendLayout();
            this.panelPreviewContainer.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.webView2Preview)).BeginInit();
            this.panelImageContainer.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBoxPreview)).BeginInit();
            this.SuspendLayout();
            // 
            // splitContainer
            // 
            this.splitContainer.Dock = System.Windows.Forms.DockStyle.Fill;
            this.splitContainer.Location = new System.Drawing.Point(0, 0);
            this.splitContainer.Name = "splitContainer";
            // 
            // splitContainer.Panel1
            // 
            this.splitContainer.Panel1.Controls.Add(this.listViewFiles);
            this.splitContainer.Panel1MinSize = 250;
            // 
            // splitContainer.Panel2
            // 
            this.splitContainer.Panel2.Controls.Add(this.panelRight);
            this.splitContainer.Panel2MinSize = 500;
            this.splitContainer.Size = new System.Drawing.Size(1262, 753);
            this.splitContainer.SplitterDistance = 450;
            this.splitContainer.TabIndex = 0;
            // 
            // listViewFiles
            // 
            this.listViewFiles.Dock = System.Windows.Forms.DockStyle.Fill;
            this.listViewFiles.HideSelection = false;
            this.listViewFiles.LargeImageList = this.imageListLarge;
            this.listViewFiles.Location = new System.Drawing.Point(0, 0);
            this.listViewFiles.MultiSelect = false;
            this.listViewFiles.Name = "listViewFiles";
            this.listViewFiles.Size = new System.Drawing.Size(450, 753);
            this.listViewFiles.TabIndex = 0;
            this.listViewFiles.UseCompatibleStateImageBehavior = false;
            this.listViewFiles.SelectedIndexChanged += new System.EventHandler(this.listViewFiles_SelectedIndexChanged);
            // 
            // imageListLarge
            // 
            this.imageListLarge.ColorDepth = System.Windows.Forms.ColorDepth.Depth32Bit;
            this.imageListLarge.ImageSize = new System.Drawing.Size(48, 48);
            this.imageListLarge.TransparentColor = System.Drawing.Color.Transparent;
            // 
            // panelRight
            // 
            this.panelRight.Controls.Add(this.btnRotateRight);
            this.panelRight.Controls.Add(this.btnRotateLeft);
            this.panelRight.Controls.Add(this.lblFileName);
            this.panelRight.Controls.Add(this.btnOpenFolder);
            this.panelRight.Controls.Add(this.btnDelete);
            this.panelRight.Controls.Add(this.btnOpen);
            this.panelRight.Controls.Add(this.panelPreviewContainer);
            this.panelRight.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panelRight.Location = new System.Drawing.Point(0, 0);
            this.panelRight.Name = "panelRight";
            this.panelRight.Size = new System.Drawing.Size(808, 753);
            this.panelRight.TabIndex = 0;
            // 
            // btnRotateRight
            // 
            this.btnRotateRight.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btnRotateRight.Font = new System.Drawing.Font("Segoe UI Symbol", 12F, System.Drawing.FontStyle.Bold);
            this.btnRotateRight.Location = new System.Drawing.Point(477, 56);
            this.btnRotateRight.Name = "btnRotateRight";
            this.btnRotateRight.Size = new System.Drawing.Size(42, 42);
            this.btnRotateRight.TabIndex = 6;
            this.btnRotateRight.Text = "⟳";
            this.btnRotateRight.UseVisualStyleBackColor = true;
            this.btnRotateRight.Visible = false;
            this.btnRotateRight.Click += new System.EventHandler(this.btnRotateRight_Click);
            // 
            // btnRotateLeft
            // 
            this.btnRotateLeft.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btnRotateLeft.Font = new System.Drawing.Font("Segoe UI Symbol", 12F, System.Drawing.FontStyle.Bold);
            this.btnRotateLeft.Location = new System.Drawing.Point(429, 56);
            this.btnRotateLeft.Name = "btnRotateLeft";
            this.btnRotateLeft.Size = new System.Drawing.Size(42, 42);
            this.btnRotateLeft.TabIndex = 5;
            this.btnRotateLeft.Text = "⟲";
            this.btnRotateLeft.UseVisualStyleBackColor = true;
            this.btnRotateLeft.Visible = false;
            this.btnRotateLeft.Click += new System.EventHandler(this.btnRotateLeft_Click);
            // 
            // lblFileName
            // 
            this.lblFileName.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.lblFileName.Font = new System.Drawing.Font("Segoe UI Semibold", 12F, System.Drawing.FontStyle.Bold);
            this.lblFileName.Location = new System.Drawing.Point(16, 13);
            this.lblFileName.Name = "lblFileName";
            this.lblFileName.Size = new System.Drawing.Size(770, 32);
            this.lblFileName.TabIndex = 4;
            this.lblFileName.Text = "Wybierz plik z listy...";
            // 
            // btnOpenFolder
            // 
            this.btnOpenFolder.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.btnOpenFolder.Font = new System.Drawing.Font("Segoe UI", 9F);
            this.btnOpenFolder.Location = new System.Drawing.Point(21, 699);
            this.btnOpenFolder.Name = "btnOpenFolder";
            this.btnOpenFolder.Size = new System.Drawing.Size(180, 42);
            this.btnOpenFolder.TabIndex = 3;
            this.btnOpenFolder.Text = "Otwórz lokalizację folderu";
            this.btnOpenFolder.UseVisualStyleBackColor = true;
            this.btnOpenFolder.Click += new System.EventHandler(this.btnOpenFolder_Click);
            // 
            // btnDelete
            // 
            this.btnDelete.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btnDelete.BackColor = System.Drawing.Color.IndianRed;
            this.btnDelete.Enabled = false;
            this.btnDelete.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnDelete.Font = new System.Drawing.Font("Segoe UI", 10.2F, System.Drawing.FontStyle.Bold);
            this.btnDelete.ForeColor = System.Drawing.Color.White;
            this.btnDelete.Location = new System.Drawing.Point(525, 56);
            this.btnDelete.Name = "btnDelete";
            this.btnDelete.Size = new System.Drawing.Size(120, 42);
            this.btnDelete.TabIndex = 2;
            this.btnDelete.Text = "Usuń";
            this.btnDelete.UseVisualStyleBackColor = false;
            this.btnDelete.Click += new System.EventHandler(this.btnDelete_Click);
            // 
            // btnOpen
            // 
            this.btnOpen.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btnOpen.BackColor = System.Drawing.Color.SeaGreen;
            this.btnOpen.Enabled = false;
            this.btnOpen.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnOpen.Font = new System.Drawing.Font("Segoe UI", 10.2F, System.Drawing.FontStyle.Bold);
            this.btnOpen.ForeColor = System.Drawing.Color.White;
            this.btnOpen.Location = new System.Drawing.Point(662, 56);
            this.btnOpen.Name = "btnOpen";
            this.btnOpen.Size = new System.Drawing.Size(120, 42);
            this.btnOpen.TabIndex = 1;
            this.btnOpen.Text = "Otwórz";
            this.btnOpen.UseVisualStyleBackColor = false;
            this.btnOpen.Click += new System.EventHandler(this.btnOpen_Click);
            // 
            // panelPreviewContainer
            // 
            this.panelPreviewContainer.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.panelPreviewContainer.BackColor = System.Drawing.SystemColors.ControlDark;
            this.panelPreviewContainer.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.panelPreviewContainer.Controls.Add(this.webView2Preview);
            this.panelPreviewContainer.Controls.Add(this.elementHostVideo);
            this.panelPreviewContainer.Controls.Add(this.panelImageContainer);
            this.panelPreviewContainer.Controls.Add(this.lblPreviewNotAvailable);
            this.panelPreviewContainer.Location = new System.Drawing.Point(21, 104);
            this.panelPreviewContainer.Name = "panelPreviewContainer";
            this.panelPreviewContainer.Size = new System.Drawing.Size(761, 589);
            this.panelPreviewContainer.TabIndex = 0;
            // 
            // webView2Preview
            // 
            this.webView2Preview.AllowExternalDrop = true;
            this.webView2Preview.CreationProperties = null;
            this.webView2Preview.DefaultBackgroundColor = System.Drawing.Color.White;
            this.webView2Preview.Dock = System.Windows.Forms.DockStyle.Fill;
            this.webView2Preview.Location = new System.Drawing.Point(0, 0);
            this.webView2Preview.Name = "webView2Preview";
            this.webView2Preview.Size = new System.Drawing.Size(759, 587);
            this.webView2Preview.TabIndex = 4;
            this.webView2Preview.Visible = false;
            this.webView2Preview.ZoomFactor = 1D;
            // 
            // elementHostVideo
            // 
            this.elementHostVideo.Dock = System.Windows.Forms.DockStyle.Fill;
            this.elementHostVideo.Location = new System.Drawing.Point(0, 0);
            this.elementHostVideo.Name = "elementHostVideo";
            this.elementHostVideo.Size = new System.Drawing.Size(759, 587);
            this.elementHostVideo.TabIndex = 3;
            this.elementHostVideo.Text = "elementHost1";
            this.elementHostVideo.Visible = false;
            this.elementHostVideo.Child = null;
            // 
            // panelImageContainer
            // 
            this.panelImageContainer.AutoScroll = true;
            this.panelImageContainer.BackColor = System.Drawing.SystemColors.ControlLightLight;
            this.panelImageContainer.Controls.Add(this.pictureBoxPreview);
            this.panelImageContainer.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panelImageContainer.Location = new System.Drawing.Point(0, 0);
            this.panelImageContainer.Name = "panelImageContainer";
            this.panelImageContainer.Size = new System.Drawing.Size(759, 587);
            this.panelImageContainer.TabIndex = 5;
            this.panelImageContainer.Visible = false;
            // 
            // pictureBoxPreview
            // 
            this.pictureBoxPreview.BackColor = System.Drawing.SystemColors.ControlLightLight;
            this.pictureBoxPreview.Location = new System.Drawing.Point(0, 0);
            this.pictureBoxPreview.Name = "pictureBoxPreview";
            this.pictureBoxPreview.Size = new System.Drawing.Size(50, 50);
            this.pictureBoxPreview.SizeMode = System.Windows.Forms.PictureBoxSizeMode.Zoom;
            this.pictureBoxPreview.TabIndex = 0;
            this.pictureBoxPreview.TabStop = false;
            // 
            // lblPreviewNotAvailable
            // 
            this.lblPreviewNotAvailable.Dock = System.Windows.Forms.DockStyle.Fill;
            this.lblPreviewNotAvailable.Font = new System.Drawing.Font("Segoe UI", 12F);
            this.lblPreviewNotAvailable.Location = new System.Drawing.Point(0, 0);
            this.lblPreviewNotAvailable.Name = "lblPreviewNotAvailable";
            this.lblPreviewNotAvailable.Size = new System.Drawing.Size(759, 587);
            this.lblPreviewNotAvailable.TabIndex = 2;
            this.lblPreviewNotAvailable.Text = "Wybierz plik z listy po lewej stronie.";
            this.lblPreviewNotAvailable.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // FormFileViewer
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1262, 753);
            this.Controls.Add(this.splitContainer);
            this.MinimumSize = new System.Drawing.Size(800, 600);
            this.Name = "FormFileViewer";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "Przeglądarka plików zgłoszenia";
            this.Load += new System.EventHandler(this.FormFileViewer_Load);
            this.splitContainer.Panel1.ResumeLayout(false);
            this.splitContainer.Panel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer)).EndInit();
            this.splitContainer.ResumeLayout(false);
            this.panelRight.ResumeLayout(false);
            this.panelPreviewContainer.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.webView2Preview)).EndInit();
            this.panelImageContainer.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.pictureBoxPreview)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.SplitContainer splitContainer;
        private System.Windows.Forms.ListView listViewFiles;
        private System.Windows.Forms.Panel panelRight;
        private System.Windows.Forms.Panel panelPreviewContainer;
        private System.Windows.Forms.PictureBox pictureBoxPreview;
        private System.Windows.Forms.Button btnDelete;
        private System.Windows.Forms.Button btnOpen;
        private System.Windows.Forms.Button btnOpenFolder;
        private System.Windows.Forms.ImageList imageListLarge;
        private System.Windows.Forms.Label lblFileName;
        private System.Windows.Forms.Label lblPreviewNotAvailable;
        private System.Windows.Forms.Integration.ElementHost elementHostVideo;
        private Microsoft.Web.WebView2.WinForms.WebView2 webView2Preview;
        private System.Windows.Forms.Panel panelImageContainer;
        private System.Windows.Forms.Button btnRotateLeft;
        private System.Windows.Forms.Button btnRotateRight;
    }
}
