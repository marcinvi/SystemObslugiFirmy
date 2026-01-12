partial class FormPowiadomieniaToast
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
        this.lblTitle = new System.Windows.Forms.Label();
        this.lblMessage = new System.Windows.Forms.Label();
        this.timerClose = new System.Windows.Forms.Timer(this.components);
        this.btnClose = new System.Windows.Forms.Button();
        this.tableLayoutPanel1 = new System.Windows.Forms.TableLayoutPanel();
        this.pictureBoxIcon = new System.Windows.Forms.PictureBox();
        this.tableLayoutPanel1.SuspendLayout();
        ((System.ComponentModel.ISupportInitialize)(this.pictureBoxIcon)).BeginInit();
        this.SuspendLayout();
        // 
        // lblTitle
        // 
        this.lblTitle.AutoEllipsis = true;
        this.lblTitle.BackColor = System.Drawing.Color.Transparent;
        this.lblTitle.Dock = System.Windows.Forms.DockStyle.Fill;
        this.lblTitle.Font = new System.Drawing.Font("Segoe UI Semibold", 10.2F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(238)));
        this.lblTitle.ForeColor = System.Drawing.Color.White;
        this.lblTitle.Location = new System.Drawing.Point(58, 0);
        this.lblTitle.Name = "lblTitle";
        this.lblTitle.Size = new System.Drawing.Size(329, 35);
        this.lblTitle.TabIndex = 0;
        this.lblTitle.Text = "Tytuł Powiadomienia";
        this.lblTitle.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
        // 
        // lblMessage
        // 
        this.lblMessage.BackColor = System.Drawing.Color.Transparent;
        this.lblMessage.Dock = System.Windows.Forms.DockStyle.Fill;
        this.lblMessage.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(238)));
        this.lblMessage.ForeColor = System.Drawing.Color.WhiteSmoke;
        this.lblMessage.Location = new System.Drawing.Point(58, 35);
        this.lblMessage.Name = "lblMessage";
        this.lblMessage.Size = new System.Drawing.Size(329, 55);
        this.lblMessage.TabIndex = 1;
        this.lblMessage.Text = "To jest treść wiadomości powiadomienia, która może być nieco dłuższa i wyjaśniać" +
    " więcej szczegółów.";
        // 
        // timerClose
        // 
        this.timerClose.Interval = 70000;
        this.timerClose.Tick += new System.EventHandler(this.timerClose_Tick);
        // 
        // btnClose
        // 
        this.btnClose.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
        this.btnClose.BackColor = System.Drawing.Color.Transparent;
        this.btnClose.FlatAppearance.BorderSize = 0;
        this.btnClose.FlatAppearance.MouseDownBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(192)))), ((int)(((byte)(0)))), ((int)(((byte)(0)))));
        this.btnClose.FlatAppearance.MouseOverBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(128)))), ((int)(((byte)(128)))));
        this.btnClose.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
        this.btnClose.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(238)));
        this.btnClose.ForeColor = System.Drawing.Color.White;
        this.btnClose.Location = new System.Drawing.Point(370, 2);
        this.btnClose.Name = "btnClose";
        this.btnClose.Size = new System.Drawing.Size(28, 28);
        this.btnClose.TabIndex = 2;
        this.btnClose.Text = "✕";
        this.btnClose.UseVisualStyleBackColor = false;
        this.btnClose.Click += new System.EventHandler(this.btnClose_Click);
        // 
        // tableLayoutPanel1
        // 
        this.tableLayoutPanel1.BackColor = System.Drawing.Color.Transparent;
        this.tableLayoutPanel1.ColumnCount = 2;
        this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 55F));
        this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
        this.tableLayoutPanel1.Controls.Add(this.pictureBoxIcon, 0, 0);
        this.tableLayoutPanel1.Controls.Add(this.lblTitle, 1, 0);
        this.tableLayoutPanel1.Controls.Add(this.lblMessage, 1, 1);
        this.tableLayoutPanel1.Dock = System.Windows.Forms.DockStyle.Fill;
        this.tableLayoutPanel1.Location = new System.Drawing.Point(0, 0);
        this.tableLayoutPanel1.Name = "tableLayoutPanel1";
        this.tableLayoutPanel1.RowCount = 2;
        this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 35F));
        this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
        this.tableLayoutPanel1.Size = new System.Drawing.Size(400, 90);
        this.tableLayoutPanel1.TabIndex = 3;
        // 
        // pictureBoxIcon
        // 
        this.pictureBoxIcon.BackColor = System.Drawing.Color.Transparent;
        this.pictureBoxIcon.Dock = System.Windows.Forms.DockStyle.Fill;
        this.pictureBoxIcon.Location = new System.Drawing.Point(3, 3);
        this.pictureBoxIcon.Name = "pictureBoxIcon";
        this.tableLayoutPanel1.SetRowSpan(this.pictureBoxIcon, 2);
        this.pictureBoxIcon.Size = new System.Drawing.Size(49, 84);
        this.pictureBoxIcon.SizeMode = System.Windows.Forms.PictureBoxSizeMode.CenterImage;
        this.pictureBoxIcon.TabIndex = 2;
        this.pictureBoxIcon.TabStop = false;
        // 
        // FormPowiadomieniaToast
        // 
        this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
        this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
        this.BackColor = System.Drawing.Color.RoyalBlue;
        this.ClientSize = new System.Drawing.Size(400, 90);
        this.Controls.Add(this.btnClose);
        this.Controls.Add(this.tableLayoutPanel1);
        this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
        this.Name = "FormPowiadomieniaToast";
        this.ShowInTaskbar = false;
        this.StartPosition = System.Windows.Forms.FormStartPosition.Manual;
        this.Text = "FormPowiadomieniaToast";
        this.TopMost = true;
        this.Load += new System.EventHandler(this.FormPowiadomieniaToast_Load);
        this.tableLayoutPanel1.ResumeLayout(false);
        ((System.ComponentModel.ISupportInitialize)(this.pictureBoxIcon)).EndInit();
        this.ResumeLayout(false);

    }

    #endregion

    private System.Windows.Forms.Label lblTitle;
    private System.Windows.Forms.Label lblMessage;
    private System.Windows.Forms.Timer timerClose;
    private System.Windows.Forms.Button btnClose;
    private System.Windows.Forms.TableLayoutPanel tableLayoutPanel1;
    private System.Windows.Forms.PictureBox pictureBoxIcon;
}