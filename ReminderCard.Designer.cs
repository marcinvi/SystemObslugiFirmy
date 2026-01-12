namespace Reklamacje_Dane
{
    partial class StandardReminderCard
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
            this.components = new System.ComponentModel.Container();
            this.lblReminderText = new System.Windows.Forms.Label();
            this.lblComplaintNumber = new System.Windows.Forms.Label();
            this.btnGoToComplaint = new System.Windows.Forms.Label();
            this.btnMarkAsDone = new System.Windows.Forms.Label();
            this.indicatorPanel = new System.Windows.Forms.Panel();
            this.toolTip1 = new System.Windows.Forms.ToolTip(this.components);
            this.SuspendLayout();
            // 
            // lblReminderText
            // 
            this.lblReminderText.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
            | System.Windows.Forms.AnchorStyles.Left)
            | System.Windows.Forms.AnchorStyles.Right)));
            this.lblReminderText.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(238)));
            this.lblReminderText.Location = new System.Drawing.Point(17, 6);
            this.lblReminderText.Name = "lblReminderText";
            this.lblReminderText.Size = new System.Drawing.Size(490, 32);
            this.lblReminderText.TabIndex = 0;
            this.lblReminderText.Text = "Treść przypomnienia, która może być dłuższa i zawijać się do następnej linii...";
            this.lblReminderText.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // lblComplaintNumber
            // 
            this.lblComplaintNumber.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)
            | System.Windows.Forms.AnchorStyles.Right)));
            this.lblComplaintNumber.Font = new System.Drawing.Font("Segoe UI", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(238)));
            this.lblComplaintNumber.ForeColor = System.Drawing.Color.DimGray;
            this.lblComplaintNumber.Location = new System.Drawing.Point(17, 38);
            this.lblComplaintNumber.Name = "lblComplaintNumber";
            this.lblComplaintNumber.Size = new System.Drawing.Size(490, 18);
            this.lblComplaintNumber.TabIndex = 1;
            this.lblComplaintNumber.Text = "Zgłoszenie: R/123/2025";
            // 
            // btnGoToComplaint
            // 
            this.btnGoToComplaint.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btnGoToComplaint.Cursor = System.Windows.Forms.Cursors.Hand;
            this.btnGoToComplaint.Font = new System.Drawing.Font("Segoe UI Symbol", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnGoToComplaint.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(64)))), ((int)(((byte)(64)))), ((int)(((byte)(64)))));
            this.btnGoToComplaint.Location = new System.Drawing.Point(513, 19);
            this.btnGoToComplaint.Name = "btnGoToComplaint";
            this.btnGoToComplaint.Size = new System.Drawing.Size(28, 28);
            this.btnGoToComplaint.TabIndex = 2;
            this.btnGoToComplaint.Text = "👁️";
            this.toolTip1.SetToolTip(this.btnGoToComplaint, "Otwórz zgłoszenie");
            this.btnGoToComplaint.Click += new System.EventHandler(this.btnGoToComplaint_Click);
            // 
            // btnMarkAsDone
            // 
            this.btnMarkAsDone.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btnMarkAsDone.Cursor = System.Windows.Forms.Cursors.Hand;
            this.btnMarkAsDone.Font = new System.Drawing.Font("Segoe UI Symbol", 14.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnMarkAsDone.ForeColor = System.Drawing.Color.SeaGreen;
            this.btnMarkAsDone.Location = new System.Drawing.Point(547, 19);
            this.btnMarkAsDone.Name = "btnMarkAsDone";
            this.btnMarkAsDone.Size = new System.Drawing.Size(28, 28);
            this.btnMarkAsDone.TabIndex = 3;
            this.btnMarkAsDone.Text = "✔";
            this.toolTip1.SetToolTip(this.btnMarkAsDone, "Oznacz jako wykonane");
            this.btnMarkAsDone.Click += new System.EventHandler(this.btnMarkAsDone_Click);
            // 
            // indicatorPanel
            // 
            this.indicatorPanel.BackColor = System.Drawing.Color.Silver;
            this.indicatorPanel.Dock = System.Windows.Forms.DockStyle.Left;
            this.indicatorPanel.Location = new System.Drawing.Point(0, 0);
            this.indicatorPanel.Name = "indicatorPanel";
            this.indicatorPanel.Size = new System.Drawing.Size(8, 62);
            this.indicatorPanel.TabIndex = 4;
            // 
            // StandardReminderCard
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.White;
            this.Controls.Add(this.indicatorPanel);
            this.Controls.Add(this.btnGoToComplaint);
            this.Controls.Add(this.lblComplaintNumber);
            this.Controls.Add(this.lblReminderText);
            this.Controls.Add(this.btnMarkAsDone);
            this.Margin = new System.Windows.Forms.Padding(3, 3, 3, 1);
            this.Name = "StandardReminderCard";
            this.Size = new System.Drawing.Size(584, 62);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Label lblReminderText;
        private System.Windows.Forms.Label lblComplaintNumber;
        private System.Windows.Forms.Label btnGoToComplaint;
        private System.Windows.Forms.Label btnMarkAsDone;
        private System.Windows.Forms.Panel indicatorPanel;
        private System.Windows.Forms.ToolTip toolTip1;
    }
}