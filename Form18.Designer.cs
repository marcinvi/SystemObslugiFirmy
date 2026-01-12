// Plik: Form18.Designer.cs
namespace Reklamacje_Dane
{
    partial class Form18
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
            this.txtAction = new System.Windows.Forms.TextBox();
            this.btnSave = new System.Windows.Forms.Button();
            this.btnCancel = new System.Windows.Forms.Button();
            this.label1 = new System.Windows.Forms.Label();
            this.btnToggleEmail = new System.Windows.Forms.Button();
            this.btnToggleSms = new System.Windows.Forms.Button();
            this.btnToggleAllegro = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // txtAction
            // 
            this.txtAction.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.txtAction.Location = new System.Drawing.Point(15, 15);
            this.txtAction.Multiline = true;
            this.txtAction.Name = "txtAction";
            this.txtAction.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
            this.txtAction.Size = new System.Drawing.Size(561, 100);
            this.txtAction.TabIndex = 0;
            // 
            // btnSave
            // 
            this.btnSave.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btnSave.BackColor = System.Drawing.Color.ForestGreen;
            this.btnSave.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnSave.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Bold);
            this.btnSave.ForeColor = System.Drawing.Color.White;
            this.btnSave.Location = new System.Drawing.Point(466, 172);
            this.btnSave.Name = "btnSave";
            this.btnSave.Size = new System.Drawing.Size(110, 32);
            this.btnSave.TabIndex = 5;
            this.btnSave.Text = "Zapisz";
            this.btnSave.UseVisualStyleBackColor = false;
            this.btnSave.Click += new System.EventHandler(this.btnSave_Click);
            // 
            // btnCancel
            // 
            this.btnCancel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btnCancel.BackColor = System.Drawing.Color.Gray;
            this.btnCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.btnCancel.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnCancel.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Bold);
            this.btnCancel.ForeColor = System.Drawing.Color.White;
            this.btnCancel.Location = new System.Drawing.Point(350, 172);
            this.btnCancel.Name = "btnCancel";
            this.btnCancel.Size = new System.Drawing.Size(110, 32);
            this.btnCancel.TabIndex = 4;
            this.btnCancel.Text = "Anuluj";
            this.btnCancel.UseVisualStyleBackColor = false;
            this.btnCancel.Click += new System.EventHandler(this.btnCancel_Click);
            // 
            // label1
            // 
            this.label1.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.label1.AutoSize = true;
            this.label1.Font = new System.Drawing.Font("Segoe UI Semibold", 9F, System.Drawing.FontStyle.Bold);
            this.label1.Location = new System.Drawing.Point(0, 129);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(139, 20);
            this.label1.TabIndex = 3;
            this.label1.Text = "Poinformuj klienta:";
            // 
            // btnToggleEmail
            // 
            this.btnToggleEmail.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.btnToggleEmail.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnToggleEmail.Location = new System.Drawing.Point(145, 127);
            this.btnToggleEmail.Name = "btnToggleEmail";
            this.btnToggleEmail.Size = new System.Drawing.Size(85, 27);
            this.btnToggleEmail.TabIndex = 6;
            this.btnToggleEmail.Text = "Mailowo";
            this.btnToggleEmail.UseVisualStyleBackColor = true;
            this.btnToggleEmail.Visible = false;
            this.btnToggleEmail.Click += new System.EventHandler(this.btnToggleEmail_Click);
            // 
            // btnToggleSms
            // 
            this.btnToggleSms.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.btnToggleSms.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnToggleSms.Location = new System.Drawing.Point(230, 127);
            this.btnToggleSms.Name = "btnToggleSms";
            this.btnToggleSms.Size = new System.Drawing.Size(85, 27);
            this.btnToggleSms.TabIndex = 7;
            this.btnToggleSms.Text = "SMS";
            this.btnToggleSms.UseVisualStyleBackColor = true;
            this.btnToggleSms.Visible = false;
            this.btnToggleSms.Click += new System.EventHandler(this.btnToggleSms_Click);
            // 
            // btnToggleAllegro
            // 
            this.btnToggleAllegro.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.btnToggleAllegro.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnToggleAllegro.Location = new System.Drawing.Point(315, 127);
            this.btnToggleAllegro.Name = "btnToggleAllegro";
            this.btnToggleAllegro.Size = new System.Drawing.Size(100, 27);
            this.btnToggleAllegro.TabIndex = 8;
            this.btnToggleAllegro.Text = "Na Allegro";
            this.btnToggleAllegro.UseVisualStyleBackColor = true;
            this.btnToggleAllegro.Visible = false;
            this.btnToggleAllegro.Click += new System.EventHandler(this.btnToggleAllegro_Click);
            // 
            // Form18
            // 
            this.AcceptButton = this.btnSave;
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(230)))), ((int)(((byte)(230)))), ((int)(((byte)(230)))));
            this.CancelButton = this.btnCancel;
            this.ClientSize = new System.Drawing.Size(588, 216);
            this.Controls.Add(this.btnToggleAllegro);
            this.Controls.Add(this.btnToggleSms);
            this.Controls.Add(this.btnToggleEmail);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.btnCancel);
            this.Controls.Add(this.btnSave);
            this.Controls.Add(this.txtAction);
            this.MinimumSize = new System.Drawing.Size(606, 263);
            this.Name = "Form18";
            this.Padding = new System.Windows.Forms.Padding(12);
            this.Text = "Dodaj działanie";
            this.Load += new System.EventHandler(this.Form18_Load);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.TextBox txtAction;
        private System.Windows.Forms.Button btnSave;
        private System.Windows.Forms.Button btnCancel;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Button btnToggleEmail;
        private System.Windows.Forms.Button btnToggleSms;
        private System.Windows.Forms.Button btnToggleAllegro;
    }
}