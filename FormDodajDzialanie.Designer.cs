namespace Reklamacje_Dane
{
    partial class FormDodajDzialanie
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.txtAction = new System.Windows.Forms.TextBox();
            this.btnSave = new System.Windows.Forms.Button();
            this.btnCancel = new System.Windows.Forms.Button();
            this.labelNotification = new System.Windows.Forms.Label();
            this.btnToggleEmail = new System.Windows.Forms.Button();
            this.btnToggleSms = new System.Windows.Forms.Button();
            this.btnToggleAllegro = new System.Windows.Forms.Button();
            this.comboBoxTemplates = new System.Windows.Forms.ComboBox();
            this.labelTemplate = new System.Windows.Forms.Label();
            this.SuspendLayout();
            // 
            // labelTemplate
            // 
            this.labelTemplate.AutoSize = true;
            this.labelTemplate.Font = new System.Drawing.Font("Segoe UI Semibold", 9F, System.Drawing.FontStyle.Bold);
            this.labelTemplate.Location = new System.Drawing.Point(12, 15);
            this.labelTemplate.Name = "labelTemplate";
            this.labelTemplate.Size = new System.Drawing.Size(65, 20);
            this.labelTemplate.TabIndex = 0;
            this.labelTemplate.Text = "Szablon:";
            // 
            // comboBoxTemplates
            // 
            this.comboBoxTemplates.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
            | System.Windows.Forms.AnchorStyles.Right)));
            this.comboBoxTemplates.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.comboBoxTemplates.FormattingEnabled = true;
            this.comboBoxTemplates.Location = new System.Drawing.Point(83, 12);
            this.comboBoxTemplates.Name = "comboBoxTemplates";
            this.comboBoxTemplates.Size = new System.Drawing.Size(493, 24);
            this.comboBoxTemplates.TabIndex = 1;
            this.comboBoxTemplates.SelectedIndexChanged += new System.EventHandler(this.comboBoxTemplates_SelectedIndexChanged);
            // 
            // txtAction
            // 
            this.txtAction.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
            | System.Windows.Forms.AnchorStyles.Left)
            | System.Windows.Forms.AnchorStyles.Right)));
            this.txtAction.Location = new System.Drawing.Point(15, 45);
            this.txtAction.Multiline = true;
            this.txtAction.Name = "txtAction";
            this.txtAction.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
            this.txtAction.Size = new System.Drawing.Size(561, 100);
            this.txtAction.TabIndex = 2;
            // 
            // labelNotification
            // 
            this.labelNotification.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.labelNotification.AutoSize = true;
            this.labelNotification.Font = new System.Drawing.Font("Segoe UI Semibold", 9F, System.Drawing.FontStyle.Bold);
            this.labelNotification.Location = new System.Drawing.Point(12, 160);
            this.labelNotification.Name = "labelNotification";
            this.labelNotification.Size = new System.Drawing.Size(126, 20);
            this.labelNotification.TabIndex = 3;
            this.labelNotification.Text = "Poinformuj klienta:";
            // 
            // btnToggleEmail
            // 
            this.btnToggleEmail.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.btnToggleEmail.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnToggleEmail.Location = new System.Drawing.Point(144, 158);
            this.btnToggleEmail.Name = "btnToggleEmail";
            this.btnToggleEmail.Size = new System.Drawing.Size(85, 27);
            this.btnToggleEmail.TabIndex = 4;
            this.btnToggleEmail.Text = "Mailowo";
            this.btnToggleEmail.UseVisualStyleBackColor = true;
            this.btnToggleEmail.Visible = false;
            this.btnToggleEmail.Click += new System.EventHandler(this.btnToggleEmail_Click);
            // 
            // btnToggleSms
            // 
            this.btnToggleSms.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.btnToggleSms.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnToggleSms.Location = new System.Drawing.Point(235, 158);
            this.btnToggleSms.Name = "btnToggleSms";
            this.btnToggleSms.Size = new System.Drawing.Size(85, 27);
            this.btnToggleSms.TabIndex = 5;
            this.btnToggleSms.Text = "SMS";
            this.btnToggleSms.UseVisualStyleBackColor = true;
            this.btnToggleSms.Visible = false;
            this.btnToggleSms.Click += new System.EventHandler(this.btnToggleSms_Click);
            // 
            // btnToggleAllegro
            // 
            this.btnToggleAllegro.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.btnToggleAllegro.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnToggleAllegro.Location = new System.Drawing.Point(326, 158);
            this.btnToggleAllegro.Name = "btnToggleAllegro";
            this.btnToggleAllegro.Size = new System.Drawing.Size(100, 27);
            this.btnToggleAllegro.TabIndex = 6;
            this.btnToggleAllegro.Text = "Na Allegro";
            this.btnToggleAllegro.UseVisualStyleBackColor = true;
            this.btnToggleAllegro.Visible = false;
            this.btnToggleAllegro.Click += new System.EventHandler(this.btnToggleAllegro_Click);
            // 
            // btnCancel
            // 
            this.btnCancel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btnCancel.BackColor = System.Drawing.Color.Gray;
            this.btnCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.btnCancel.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnCancel.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Bold);
            this.btnCancel.ForeColor = System.Drawing.Color.White;
            this.btnCancel.Location = new System.Drawing.Point(350, 199);
            this.btnCancel.Name = "btnCancel";
            this.btnCancel.Size = new System.Drawing.Size(110, 32);
            this.btnCancel.TabIndex = 7;
            this.btnCancel.Text = "Anuluj";
            this.btnCancel.UseVisualStyleBackColor = false;
            this.btnCancel.Click += new System.EventHandler(this.btnCancel_Click);
            // 
            // btnSave
            // 
            this.btnSave.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btnSave.BackColor = System.Drawing.Color.ForestGreen;
            this.btnSave.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnSave.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Bold);
            this.btnSave.ForeColor = System.Drawing.Color.White;
            this.btnSave.Location = new System.Drawing.Point(466, 199);
            this.btnSave.Name = "btnSave";
            this.btnSave.Size = new System.Drawing.Size(110, 32);
            this.btnSave.TabIndex = 8;
            this.btnSave.Text = "Zapisz";
            this.btnSave.UseVisualStyleBackColor = false;
            this.btnSave.Click += new System.EventHandler(this.btnSave_Click);
            // 
            // FormDodajDzialanie
            // 
            this.AcceptButton = this.btnSave;
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(230)))), ((int)(((byte)(230)))), ((int)(((byte)(230)))));
            this.CancelButton = this.btnCancel;
            this.ClientSize = new System.Drawing.Size(588, 243);
            this.Controls.Add(this.labelTemplate);
            this.Controls.Add(this.comboBoxTemplates);
            this.Controls.Add(this.btnToggleAllegro);
            this.Controls.Add(this.btnToggleSms);
            this.Controls.Add(this.btnToggleEmail);
            this.Controls.Add(this.labelNotification);
            this.Controls.Add(this.btnCancel);
            this.Controls.Add(this.btnSave);
            this.Controls.Add(this.txtAction);
            this.MinimumSize = new System.Drawing.Size(606, 290);
            this.Name = "FormDodajDzialanie";
            this.Text = "Dodaj działanie";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.TextBox txtAction;
        private System.Windows.Forms.Button btnSave;
        private System.Windows.Forms.Button btnCancel;
        private System.Windows.Forms.Label labelNotification;
        private System.Windows.Forms.Button btnToggleEmail;
        private System.Windows.Forms.Button btnToggleSms;
        private System.Windows.Forms.Button btnToggleAllegro;
        private System.Windows.Forms.ComboBox comboBoxTemplates;
        private System.Windows.Forms.Label labelTemplate;
    }
}