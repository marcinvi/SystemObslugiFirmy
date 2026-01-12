namespace Reklamacje_Dane
{
    partial class FormSzablonyDzialan
    {
        private System.ComponentModel.IContainer components = null;
        protected override void Dispose(bool disposing) { /* ... */ }
        #region Windows Form Designer generated code
        private void InitializeComponent()
        {
            this.listBoxTemplates = new System.Windows.Forms.ListBox();
            this.txtTemplate = new System.Windows.Forms.TextBox();
            this.btnAdd = new System.Windows.Forms.Button();
            this.btnUpdate = new System.Windows.Forms.Button();
            this.btnDelete = new System.Windows.Forms.Button();
            this.label1 = new System.Windows.Forms.Label();
            this.SuspendLayout();
            // 
            // listBoxTemplates
            // 
            this.listBoxTemplates.FormattingEnabled = true;
            this.listBoxTemplates.ItemHeight = 16;
            this.listBoxTemplates.Location = new System.Drawing.Point(12, 12);
            this.listBoxTemplates.Name = "listBoxTemplates";
            this.listBoxTemplates.Size = new System.Drawing.Size(560, 196);
            this.listBoxTemplates.TabIndex = 0;
            this.listBoxTemplates.SelectedIndexChanged += new System.EventHandler(this.listBoxTemplates_SelectedIndexChanged);
            // 
            // txtTemplate
            // 
            this.txtTemplate.Location = new System.Drawing.Point(12, 234);
            this.txtTemplate.Name = "txtTemplate";
            this.txtTemplate.Size = new System.Drawing.Size(560, 22);
            this.txtTemplate.TabIndex = 1;
            // 
            // btnAdd
            // 
            this.btnAdd.Location = new System.Drawing.Point(235, 262);
            this.btnAdd.Name = "btnAdd";
            this.btnAdd.Size = new System.Drawing.Size(110, 30);
            this.btnAdd.TabIndex = 2;
            this.btnAdd.Text = "Dodaj nowy";
            this.btnAdd.UseVisualStyleBackColor = true;
            this.btnAdd.Click += new System.EventHandler(this.btnAdd_Click);
            // 
            // btnUpdate
            // 
            this.btnUpdate.Location = new System.Drawing.Point(351, 262);
            this.btnUpdate.Name = "btnUpdate";
            this.btnUpdate.Size = new System.Drawing.Size(110, 30);
            this.btnUpdate.TabIndex = 3;
            this.btnUpdate.Text = "Aktualizuj";
            this.btnUpdate.UseVisualStyleBackColor = true;
            this.btnUpdate.Click += new System.EventHandler(this.btnUpdate_Click);
            // 
            // btnDelete
            // 
            this.btnDelete.Location = new System.Drawing.Point(467, 262);
            this.btnDelete.Name = "btnDelete";
            this.btnDelete.Size = new System.Drawing.Size(105, 30);
            this.btnDelete.TabIndex = 4;
            this.btnDelete.Text = "Usuń";
            this.btnDelete.UseVisualStyleBackColor = true;
            this.btnDelete.Click += new System.EventHandler(this.btnDelete_Click);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(12, 215);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(101, 16);
            this.label1.TabIndex = 5;
            this.label1.Text = "Treść szablonu:";
            // 
            // FormSzablonyDzialan
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(584, 304);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.btnDelete);
            this.Controls.Add(this.btnUpdate);
            this.Controls.Add(this.btnAdd);
            this.Controls.Add(this.txtTemplate);
            this.Controls.Add(this.listBoxTemplates);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "FormSzablonyDzialan";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "Zarządzaj Szablonami Działań";
            this.Load += new System.EventHandler(this.FormSzablonyDzialan_Load);
            this.ResumeLayout(false);
            this.PerformLayout();
        }
        #endregion
        private System.Windows.Forms.ListBox listBoxTemplates;
        private System.Windows.Forms.TextBox txtTemplate;
        private System.Windows.Forms.Button btnAdd;
        private System.Windows.Forms.Button btnUpdate;
        private System.Windows.Forms.Button btnDelete;
        private System.Windows.Forms.Label label1;
    }
}