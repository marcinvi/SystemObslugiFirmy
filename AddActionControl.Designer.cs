// Plik: AddActionControl.Designer.cs
namespace Reklamacje_Dane
{
    partial class AddActionControl
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
            this.btnToggleEmail = new System.Windows.Forms.Button(); // Zmieniono na Button
            this.btnToggleSms = new System.Windows.Forms.Button();   // Zmieniono na Button
            this.btnToggleAllegro = new System.Windows.Forms.Button(); // Zmieniono na Button
            this.SuspendLayout();
            //
            // txtAction
            //
            this.txtAction.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
            | System.Windows.Forms.AnchorStyles.Right)));
            this.txtAction.Location = new System.Drawing.Point(15, 15);
            this.txtAction.Multiline = true;
            this.txtAction.Name = "txtAction";
            this.txtAction.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
            this.txtAction.Size = new System.Drawing.Size(520, 70);
            this.txtAction.TabIndex = 0;
            //
            // btnSave
            //
            this.btnSave.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btnSave.BackColor = System.Drawing.Color.ForestGreen;
            this.btnSave.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnSave.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Bold);
            this.btnSave.ForeColor = System.Drawing.Color.White;
            this.btnSave.Location = new System.Drawing.Point(425, 127);
            this.btnSave.Name = "btnSave";
            this.btnSave.Size = new System.Drawing.Size(110, 32);
            this.btnSave.TabIndex = 1;
            this.btnSave.Text = "Zapisz";
            this.btnSave.UseVisualStyleBackColor = false;
            this.btnSave.Click += new System.EventHandler(this.btnSave_Click);
            //
            // btnCancel
            //
            this.btnCancel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btnCancel.BackColor = System.Drawing.Color.Gray;
            this.btnCancel.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnCancel.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Bold);
            this.btnCancel.ForeColor = System.Drawing.Color.White;
            this.btnCancel.Location = new System.Drawing.Point(309, 127);
            this.btnCancel.Name = "btnCancel";
            this.btnCancel.Size = new System.Drawing.Size(110, 32);
            this.btnCancel.TabIndex = 2;
            this.btnCancel.Text = "Anuluj";
            this.btnCancel.UseVisualStyleBackColor = false;
           // this.btnCancel.Click += new System.EventHandler(this.btnCancel_Click);
            //
            // label1
            //
            this.label1.AutoSize = true;
            this.label1.Font = new System.Drawing.Font("Segoe UI Semibold", 9F, System.Drawing.FontStyle.Bold);
            this.label1.Location = new System.Drawing.Point(11, 101);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(139, 20);
            this.label1.TabIndex = 3;
            this.label1.Text = "Poinformuj klienta:";
            //
            // btnToggleEmail
            //
            this.btnToggleEmail.BackColor = System.Drawing.Color.LightGray;
            this.btnToggleEmail.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnToggleEmail.Font = new System.Drawing.Font("Segoe UI", 9F);
            this.btnToggleEmail.Location = new System.Drawing.Point(153, 98);
            this.btnToggleEmail.Name = "btnToggleEmail";
            this.btnToggleEmail.Size = new System.Drawing.Size(85, 27);
            this.btnToggleEmail.TabIndex = 4;
            this.btnToggleEmail.Text = "Mailowo";
            this.btnToggleEmail.UseVisualStyleBackColor = false;
            this.btnToggleEmail.Visible = false;
            this.btnToggleEmail.Click += new System.EventHandler(this.btnToggleEmail_Click);
            //
            // btnToggleSms
            //
            this.btnToggleSms.BackColor = System.Drawing.Color.LightGray;
            this.btnToggleSms.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnToggleSms.Font = new System.Drawing.Font("Segoe UI", 9F);
            this.btnToggleSms.Location = new System.Drawing.Point(244, 98);
            this.btnToggleSms.Name = "btnToggleSms";
            this.btnToggleSms.Size = new System.Drawing.Size(85, 27);
            this.btnToggleSms.TabIndex = 5;
            this.btnToggleSms.Text = "SMS";
            this.btnToggleSms.UseVisualStyleBackColor = false;
            this.btnToggleSms.Visible = false;
            this.btnToggleSms.Click += new System.EventHandler(this.btnToggleSms_Click);
            //
            // btnToggleAllegro
            //
            this.btnToggleAllegro.BackColor = System.Drawing.Color.LightGray;
            this.btnToggleAllegro.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnToggleAllegro.Font = new System.Drawing.Font("Segoe UI", 9F);
            this.btnToggleAllegro.Location = new System.Drawing.Point(335, 98);
            this.btnToggleAllegro.Name = "btnToggleAllegro";
            this.btnToggleAllegro.Size = new System.Drawing.Size(100, 27);
            this.btnToggleAllegro.TabIndex = 6;
            this.btnToggleAllegro.Text = "Na Allegro";
            this.btnToggleAllegro.UseVisualStyleBackColor = false;
            this.btnToggleAllegro.Visible = false;
            this.btnToggleAllegro.Click += new System.EventHandler(this.btnToggleAllegro_Click);
            //
            // AddActionControl
            //
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(230)))), ((int)(((byte)(230)))), ((int)(((byte)(230)))));
            this.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.Controls.Add(this.btnToggleAllegro);
            this.Controls.Add(this.btnToggleSms);
            this.Controls.Add(this.btnToggleEmail);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.btnCancel);
            this.Controls.Add(this.btnSave);
            this.Controls.Add(this.txtAction);
            this.Name = "AddActionControl";
            this.Padding = new System.Windows.Forms.Padding(12);
            this.Size = new System.Drawing.Size(550, 162);
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