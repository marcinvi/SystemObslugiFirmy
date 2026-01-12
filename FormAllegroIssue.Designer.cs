using System;
using System.Windows.Forms;

namespace Reklamacje_Dane
{
    public partial class FormAllegroIssue
    {
        private System.ComponentModel.IContainer components = null;

        private Panel pnlHeader;
        private Label lblDisputeId;
        private Label lblBuyerLogin;
        private Label lblInternalComplaintId;

        private SplitContainer splitMain;
        private Panel pnlActions;
        private GroupBox gbStatusAndDeadlines;
        private Label lblCurrentStatus;
        private Label lblDecisionTime;
        private ProgressBar progressDecision;
        private Label lblResolutionTime;
        private ProgressBar progressResolution;

        private GroupBox gbChangeStatus;
        private ComboBox cbClaimStatus;
        private TextBox txtStatusMessage;
        private Button btnChangeStatus;

        private Panel pnlPartialRefund;
        private Label lblPartialRefundInfo;
        private TextBox txtPartialRefundAmount;
        private ComboBox cbPartialRefundCurrency;

        private GroupBox gbReturnDecision;
        private Button btnReturnRequiredCustom;
        private Button btnReturnNotRequired;
        private Button btnEndRequest;

        private GroupBox gbOrder;
        private Label lblProductName;
        private Button btnViewOrder;
        private Button btnAddTrackingNumber;

        private Panel pnlCommunication;
        private FlowLayoutPanel pnlChatHistory;
        private Panel pnlNewMessage;
        private Button btnSendMessage;
        private TextBox txtNewMessage;
        private Button btnAddAttachment;
        private OpenFileDialog openFileDialog1;

        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
                components.Dispose();
            base.Dispose(disposing);
        }

        private void InitializeComponent()
        {
            this.pnlHeader = new System.Windows.Forms.Panel();
            this.lblInternalComplaintId = new System.Windows.Forms.Label();
            this.lblBuyerLogin = new System.Windows.Forms.Label();
            this.lblDisputeId = new System.Windows.Forms.Label();
            this.splitMain = new System.Windows.Forms.SplitContainer();
            this.pnlActions = new System.Windows.Forms.Panel();
            this.btnEndRequest = new System.Windows.Forms.Button();
            this.gbOrder = new System.Windows.Forms.GroupBox();
            this.lblProductName = new System.Windows.Forms.Label();
            this.btnAddTrackingNumber = new System.Windows.Forms.Button();
            this.btnViewOrder = new System.Windows.Forms.Button();
            this.gbReturnDecision = new System.Windows.Forms.GroupBox();
            this.btnReturnNotRequired = new System.Windows.Forms.Button();
            this.btnReturnRequiredCustom = new System.Windows.Forms.Button();
            this.gbChangeStatus = new System.Windows.Forms.GroupBox();
            this.pnlPartialRefund = new System.Windows.Forms.Panel();
            this.cbPartialRefundCurrency = new System.Windows.Forms.ComboBox();
            this.txtPartialRefundAmount = new System.Windows.Forms.TextBox();
            this.lblPartialRefundInfo = new System.Windows.Forms.Label();
            this.btnChangeStatus = new System.Windows.Forms.Button();
            this.txtStatusMessage = new System.Windows.Forms.TextBox();
            this.cbClaimStatus = new System.Windows.Forms.ComboBox();
            this.gbStatusAndDeadlines = new System.Windows.Forms.GroupBox();
            this.lblResolutionTime = new System.Windows.Forms.Label();
            this.progressResolution = new System.Windows.Forms.ProgressBar();
            this.lblDecisionTime = new System.Windows.Forms.Label();
            this.progressDecision = new System.Windows.Forms.ProgressBar();
            this.lblCurrentStatus = new System.Windows.Forms.Label();
            this.pnlCommunication = new System.Windows.Forms.Panel();
            this.pnlNewMessage = new System.Windows.Forms.Panel();
            this.btnAddAttachment = new System.Windows.Forms.Button();
            this.btnSendMessage = new System.Windows.Forms.Button();
            this.txtNewMessage = new System.Windows.Forms.TextBox();
            this.pnlChatHistory = new System.Windows.Forms.FlowLayoutPanel();
            this.openFileDialog1 = new System.Windows.Forms.OpenFileDialog();
            this.pnlHeader.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.splitMain)).BeginInit();
            this.splitMain.Panel1.SuspendLayout();
            this.splitMain.Panel2.SuspendLayout();
            this.splitMain.SuspendLayout();
            this.pnlActions.SuspendLayout();
            this.gbOrder.SuspendLayout();
            this.gbReturnDecision.SuspendLayout();
            this.gbChangeStatus.SuspendLayout();
            this.pnlPartialRefund.SuspendLayout();
            this.gbStatusAndDeadlines.SuspendLayout();
            this.pnlCommunication.SuspendLayout();
            this.pnlNewMessage.SuspendLayout();
            this.SuspendLayout();
            // 
            // pnlHeader
            // 
            this.pnlHeader.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(51)))), ((int)(((byte)(51)))), ((int)(((byte)(76)))));
            this.pnlHeader.Controls.Add(this.lblInternalComplaintId);
            this.pnlHeader.Controls.Add(this.lblBuyerLogin);
            this.pnlHeader.Controls.Add(this.lblDisputeId);
            this.pnlHeader.Dock = System.Windows.Forms.DockStyle.Top;
            this.pnlHeader.Location = new System.Drawing.Point(0, 0);
            this.pnlHeader.Margin = new System.Windows.Forms.Padding(4);
            this.pnlHeader.Name = "pnlHeader";
            this.pnlHeader.Size = new System.Drawing.Size(1685, 49);
            this.pnlHeader.TabIndex = 0;
            // 
            // lblInternalComplaintId
            // 
            this.lblInternalComplaintId.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.lblInternalComplaintId.AutoSize = true;
            this.lblInternalComplaintId.ForeColor = System.Drawing.Color.WhiteSmoke;
            this.lblInternalComplaintId.Location = new System.Drawing.Point(1373, 15);
            this.lblInternalComplaintId.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.lblInternalComplaintId.Name = "lblInternalComplaintId";
            this.lblInternalComplaintId.Size = new System.Drawing.Size(121, 16);
            this.lblInternalComplaintId.TabIndex = 2;
            this.lblInternalComplaintId.Text = "Nr reklamacji: ZGL/";
            // 
            // lblBuyerLogin
            // 
            this.lblBuyerLogin.Anchor = System.Windows.Forms.AnchorStyles.Top;
            this.lblBuyerLogin.AutoSize = true;
            this.lblBuyerLogin.ForeColor = System.Drawing.Color.WhiteSmoke;
            this.lblBuyerLogin.Location = new System.Drawing.Point(777, 15);
            this.lblBuyerLogin.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.lblBuyerLogin.Name = "lblBuyerLogin";
            this.lblBuyerLogin.Size = new System.Drawing.Size(72, 16);
            this.lblBuyerLogin.TabIndex = 1;
            this.lblBuyerLogin.Text = "Kupujący: -";
            // 
            // lblDisputeId
            // 
            this.lblDisputeId.AutoSize = true;
            this.lblDisputeId.Font = new System.Drawing.Font("Segoe UI Semibold", 9.75F, System.Drawing.FontStyle.Bold);
            this.lblDisputeId.ForeColor = System.Drawing.Color.White;
            this.lblDisputeId.Location = new System.Drawing.Point(16, 14);
            this.lblDisputeId.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.lblDisputeId.Name = "lblDisputeId";
            this.lblDisputeId.Size = new System.Drawing.Size(103, 23);
            this.lblDisputeId.TabIndex = 0;
            this.lblDisputeId.Text = "Dyskusja ID:";
            // 
            // splitMain
            // 
            this.splitMain.Dock = System.Windows.Forms.DockStyle.Fill;
            this.splitMain.FixedPanel = System.Windows.Forms.FixedPanel.Panel1;
            this.splitMain.Location = new System.Drawing.Point(0, 49);
            this.splitMain.Margin = new System.Windows.Forms.Padding(4);
            this.splitMain.Name = "splitMain";
            // 
            // splitMain.Panel1
            // 
            this.splitMain.Panel1.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(240)))), ((int)(((byte)(242)))), ((int)(((byte)(245)))));
            this.splitMain.Panel1.Controls.Add(this.pnlActions);
            // 
            // splitMain.Panel2
            // 
            this.splitMain.Panel2.Controls.Add(this.pnlCommunication);
            this.splitMain.Size = new System.Drawing.Size(1685, 789);
            this.splitMain.SplitterDistance = 546;
            this.splitMain.SplitterWidth = 5;
            this.splitMain.TabIndex = 1;
            // 
            // pnlActions
            // 
            this.pnlActions.Controls.Add(this.btnEndRequest);
            this.pnlActions.Controls.Add(this.gbOrder);
            this.pnlActions.Controls.Add(this.gbReturnDecision);
            this.pnlActions.Controls.Add(this.gbChangeStatus);
            this.pnlActions.Controls.Add(this.gbStatusAndDeadlines);
            this.pnlActions.Location = new System.Drawing.Point(0, 0);
            this.pnlActions.Margin = new System.Windows.Forms.Padding(4);
            this.pnlActions.Name = "pnlActions";
            this.pnlActions.Padding = new System.Windows.Forms.Padding(11, 10, 11, 10);
            this.pnlActions.Size = new System.Drawing.Size(558, 789);
            this.pnlActions.TabIndex = 0;
            // 
            // btnEndRequest
            // 
            this.btnEndRequest.Location = new System.Drawing.Point(20, 618);
            this.btnEndRequest.Margin = new System.Windows.Forms.Padding(4);
            this.btnEndRequest.Name = "btnEndRequest";
            this.btnEndRequest.Size = new System.Drawing.Size(519, 36);
            this.btnEndRequest.TabIndex = 2;
            this.btnEndRequest.Text = "Poproś o zakończenie dyskusji";
            this.btnEndRequest.UseVisualStyleBackColor = true;
            this.btnEndRequest.Click += new System.EventHandler(this.btnEndRequest_Click);
            // 
            // gbOrder
            // 
            this.gbOrder.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.gbOrder.Controls.Add(this.lblProductName);
            this.gbOrder.Controls.Add(this.btnAddTrackingNumber);
            this.gbOrder.Controls.Add(this.btnViewOrder);
            this.gbOrder.Location = new System.Drawing.Point(11, 659);
            this.gbOrder.Margin = new System.Windows.Forms.Padding(4);
            this.gbOrder.Name = "gbOrder";
            this.gbOrder.Padding = new System.Windows.Forms.Padding(4);
            this.gbOrder.Size = new System.Drawing.Size(523, 118);
            this.gbOrder.TabIndex = 3;
            this.gbOrder.TabStop = false;
            this.gbOrder.Text = "Szczegóły zamówienia";
            // 
            // lblProductName
            // 
            this.lblProductName.AutoEllipsis = true;
            this.lblProductName.Location = new System.Drawing.Point(8, 0);
            this.lblProductName.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.lblProductName.Name = "lblProductName";
            this.lblProductName.Size = new System.Drawing.Size(523, 22);
            this.lblProductName.TabIndex = 0;
            this.lblProductName.Text = "Produkt: Ładowanie...";
            // 
            // btnAddTrackingNumber
            // 
            this.btnAddTrackingNumber.Location = new System.Drawing.Point(4, 74);
            this.btnAddTrackingNumber.Margin = new System.Windows.Forms.Padding(4);
            this.btnAddTrackingNumber.Name = "btnAddTrackingNumber";
            this.btnAddTrackingNumber.Size = new System.Drawing.Size(519, 36);
            this.btnAddTrackingNumber.TabIndex = 2;
            this.btnAddTrackingNumber.Text = "Dodaj numer przesyłki do zamówienia";
            this.btnAddTrackingNumber.UseVisualStyleBackColor = true;
            this.btnAddTrackingNumber.Click += new System.EventHandler(this.btnAddTrackingNumber_Click);
            // 
            // btnViewOrder
            // 
            this.btnViewOrder.Location = new System.Drawing.Point(4, 30);
            this.btnViewOrder.Margin = new System.Windows.Forms.Padding(4);
            this.btnViewOrder.Name = "btnViewOrder";
            this.btnViewOrder.Size = new System.Drawing.Size(519, 36);
            this.btnViewOrder.TabIndex = 1;
            this.btnViewOrder.Text = "Zobacz zamówienie na Allegro";
            this.btnViewOrder.UseVisualStyleBackColor = true;
            this.btnViewOrder.Click += new System.EventHandler(this.btnViewOrder_Click);
            // 
            // gbReturnDecision
            // 
            this.gbReturnDecision.Controls.Add(this.btnReturnNotRequired);
            this.gbReturnDecision.Controls.Add(this.btnReturnRequiredCustom);
            this.gbReturnDecision.Dock = System.Windows.Forms.DockStyle.Top;
            this.gbReturnDecision.Location = new System.Drawing.Point(11, 212);
            this.gbReturnDecision.Margin = new System.Windows.Forms.Padding(4);
            this.gbReturnDecision.Name = "gbReturnDecision";
            this.gbReturnDecision.Padding = new System.Windows.Forms.Padding(4);
            this.gbReturnDecision.Size = new System.Drawing.Size(536, 76);
            this.gbReturnDecision.TabIndex = 2;
            this.gbReturnDecision.TabStop = false;
            this.gbReturnDecision.Text = "Decyzja o przesyłce";
            // 
            // btnReturnNotRequired
            // 
            this.btnReturnNotRequired.Location = new System.Drawing.Point(276, 26);
            this.btnReturnNotRequired.Margin = new System.Windows.Forms.Padding(4);
            this.btnReturnNotRequired.Name = "btnReturnNotRequired";
            this.btnReturnNotRequired.Size = new System.Drawing.Size(255, 36);
            this.btnReturnNotRequired.TabIndex = 1;
            this.btnReturnNotRequired.Text = "Zwrot niewymagany";
            this.btnReturnNotRequired.UseVisualStyleBackColor = true;
            this.btnReturnNotRequired.Click += new System.EventHandler(this.btnReturnNotRequired_Click);
            // 
            // btnReturnRequiredCustom
            // 
            this.btnReturnRequiredCustom.Location = new System.Drawing.Point(12, 26);
            this.btnReturnRequiredCustom.Margin = new System.Windows.Forms.Padding(4);
            this.btnReturnRequiredCustom.Name = "btnReturnRequiredCustom";
            this.btnReturnRequiredCustom.Size = new System.Drawing.Size(255, 36);
            this.btnReturnRequiredCustom.TabIndex = 0;
            this.btnReturnRequiredCustom.Text = "Zwrot wymagany (własny koszt)";
            this.btnReturnRequiredCustom.UseVisualStyleBackColor = true;
            this.btnReturnRequiredCustom.Click += new System.EventHandler(this.btnReturnRequiredCustom_Click);
            // 
            // gbChangeStatus
            // 
            this.gbChangeStatus.Controls.Add(this.pnlPartialRefund);
            this.gbChangeStatus.Controls.Add(this.btnChangeStatus);
            this.gbChangeStatus.Controls.Add(this.txtStatusMessage);
            this.gbChangeStatus.Controls.Add(this.cbClaimStatus);
            this.gbChangeStatus.Location = new System.Drawing.Point(13, 296);
            this.gbChangeStatus.Margin = new System.Windows.Forms.Padding(4);
            this.gbChangeStatus.Name = "gbChangeStatus";
            this.gbChangeStatus.Padding = new System.Windows.Forms.Padding(4);
            this.gbChangeStatus.Size = new System.Drawing.Size(536, 329);
            this.gbChangeStatus.TabIndex = 1;
            this.gbChangeStatus.TabStop = false;
            this.gbChangeStatus.Text = "Akcje i zmiana statusu reklamacji";
            // 
            // pnlPartialRefund
            // 
            this.pnlPartialRefund.Controls.Add(this.cbPartialRefundCurrency);
            this.pnlPartialRefund.Controls.Add(this.txtPartialRefundAmount);
            this.pnlPartialRefund.Controls.Add(this.lblPartialRefundInfo);
            this.pnlPartialRefund.Location = new System.Drawing.Point(12, 218);
            this.pnlPartialRefund.Margin = new System.Windows.Forms.Padding(4);
            this.pnlPartialRefund.Name = "pnlPartialRefund";
            this.pnlPartialRefund.Size = new System.Drawing.Size(519, 44);
            this.pnlPartialRefund.TabIndex = 3;
            this.pnlPartialRefund.Visible = false;
            // 
            // cbPartialRefundCurrency
            // 
            this.cbPartialRefundCurrency.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cbPartialRefundCurrency.FormattingEnabled = true;
            this.cbPartialRefundCurrency.Location = new System.Drawing.Point(321, 9);
            this.cbPartialRefundCurrency.Margin = new System.Windows.Forms.Padding(4);
            this.cbPartialRefundCurrency.Name = "cbPartialRefundCurrency";
            this.cbPartialRefundCurrency.Size = new System.Drawing.Size(85, 24);
            this.cbPartialRefundCurrency.TabIndex = 2;
            // 
            // txtPartialRefundAmount
            // 
            this.txtPartialRefundAmount.Location = new System.Drawing.Point(181, 12);
            this.txtPartialRefundAmount.Margin = new System.Windows.Forms.Padding(4);
            this.txtPartialRefundAmount.Name = "txtPartialRefundAmount";
            this.txtPartialRefundAmount.Size = new System.Drawing.Size(132, 22);
            this.txtPartialRefundAmount.TabIndex = 1;
            // 
            // lblPartialRefundInfo
            // 
            this.lblPartialRefundInfo.AutoSize = true;
            this.lblPartialRefundInfo.Location = new System.Drawing.Point(5, 12);
            this.lblPartialRefundInfo.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.lblPartialRefundInfo.Name = "lblPartialRefundInfo";
            this.lblPartialRefundInfo.Size = new System.Drawing.Size(168, 16);
            this.lblPartialRefundInfo.TabIndex = 0;
            this.lblPartialRefundInfo.Text = "Kwota częściowego zwrotu:";
            // 
            // btnChangeStatus
            // 
            this.btnChangeStatus.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(123)))), ((int)(((byte)(255)))));
            this.btnChangeStatus.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnChangeStatus.Font = new System.Drawing.Font("Segoe UI", 9.75F, System.Drawing.FontStyle.Bold);
            this.btnChangeStatus.ForeColor = System.Drawing.Color.White;
            this.btnChangeStatus.Location = new System.Drawing.Point(12, 270);
            this.btnChangeStatus.Margin = new System.Windows.Forms.Padding(4);
            this.btnChangeStatus.Name = "btnChangeStatus";
            this.btnChangeStatus.Size = new System.Drawing.Size(519, 44);
            this.btnChangeStatus.TabIndex = 2;
            this.btnChangeStatus.Text = "Zmień status reklamacji";
            this.btnChangeStatus.UseVisualStyleBackColor = false;
            this.btnChangeStatus.Click += new System.EventHandler(this.btnChangeStatus_Click);
            // 
            // txtStatusMessage
            // 
            this.txtStatusMessage.Location = new System.Drawing.Point(12, 70);
            this.txtStatusMessage.Margin = new System.Windows.Forms.Padding(4);
            this.txtStatusMessage.Multiline = true;
            this.txtStatusMessage.Name = "txtStatusMessage";
            this.txtStatusMessage.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
            this.txtStatusMessage.Size = new System.Drawing.Size(517, 139);
            this.txtStatusMessage.TabIndex = 1;
            // 
            // cbClaimStatus
            // 
            this.cbClaimStatus.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cbClaimStatus.FormattingEnabled = true;
            this.cbClaimStatus.Location = new System.Drawing.Point(12, 33);
            this.cbClaimStatus.Margin = new System.Windows.Forms.Padding(4);
            this.cbClaimStatus.Name = "cbClaimStatus";
            this.cbClaimStatus.Size = new System.Drawing.Size(517, 24);
            this.cbClaimStatus.TabIndex = 0;
            this.cbClaimStatus.SelectedIndexChanged += new System.EventHandler(this.cbClaimStatus_SelectedIndexChanged);
            // 
            // gbStatusAndDeadlines
            // 
            this.gbStatusAndDeadlines.Controls.Add(this.lblResolutionTime);
            this.gbStatusAndDeadlines.Controls.Add(this.progressResolution);
            this.gbStatusAndDeadlines.Controls.Add(this.lblDecisionTime);
            this.gbStatusAndDeadlines.Controls.Add(this.progressDecision);
            this.gbStatusAndDeadlines.Controls.Add(this.lblCurrentStatus);
            this.gbStatusAndDeadlines.Dock = System.Windows.Forms.DockStyle.Top;
            this.gbStatusAndDeadlines.Location = new System.Drawing.Point(11, 10);
            this.gbStatusAndDeadlines.Margin = new System.Windows.Forms.Padding(4);
            this.gbStatusAndDeadlines.Name = "gbStatusAndDeadlines";
            this.gbStatusAndDeadlines.Padding = new System.Windows.Forms.Padding(4);
            this.gbStatusAndDeadlines.Size = new System.Drawing.Size(536, 202);
            this.gbStatusAndDeadlines.TabIndex = 0;
            this.gbStatusAndDeadlines.TabStop = false;
            this.gbStatusAndDeadlines.Text = "Status i Terminy";
            // 
            // lblResolutionTime
            // 
            this.lblResolutionTime.AutoSize = true;
            this.lblResolutionTime.Location = new System.Drawing.Point(8, 127);
            this.lblResolutionTime.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.lblResolutionTime.Name = "lblResolutionTime";
            this.lblResolutionTime.Size = new System.Drawing.Size(202, 16);
            this.lblResolutionTime.TabIndex = 4;
            this.lblResolutionTime.Text = "Czas na rozpatrzenie (14 dni): n/d";
            // 
            // progressResolution
            // 
            this.progressResolution.Location = new System.Drawing.Point(12, 150);
            this.progressResolution.Margin = new System.Windows.Forms.Padding(4);
            this.progressResolution.Name = "progressResolution";
            this.progressResolution.Size = new System.Drawing.Size(519, 28);
            this.progressResolution.TabIndex = 3;
            // 
            // lblDecisionTime
            // 
            this.lblDecisionTime.AutoSize = true;
            this.lblDecisionTime.Location = new System.Drawing.Point(8, 64);
            this.lblDecisionTime.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.lblDecisionTime.Name = "lblDecisionTime";
            this.lblDecisionTime.Size = new System.Drawing.Size(187, 16);
            this.lblDecisionTime.TabIndex = 2;
            this.lblDecisionTime.Text = "Czas na decyzję zwrotu (3 dni):";
            // 
            // progressDecision
            // 
            this.progressDecision.Location = new System.Drawing.Point(12, 87);
            this.progressDecision.Margin = new System.Windows.Forms.Padding(4);
            this.progressDecision.Name = "progressDecision";
            this.progressDecision.Size = new System.Drawing.Size(519, 28);
            this.progressDecision.TabIndex = 1;
            // 
            // lblCurrentStatus
            // 
            this.lblCurrentStatus.AutoSize = true;
            this.lblCurrentStatus.Font = new System.Drawing.Font("Segoe UI", 9.75F, System.Drawing.FontStyle.Bold);
            this.lblCurrentStatus.Location = new System.Drawing.Point(8, 31);
            this.lblCurrentStatus.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.lblCurrentStatus.Name = "lblCurrentStatus";
            this.lblCurrentStatus.Size = new System.Drawing.Size(171, 23);
            this.lblCurrentStatus.TabIndex = 0;
            this.lblCurrentStatus.Text = "Status: Ładowanie...";
            // 
            // pnlCommunication
            // 
            this.pnlCommunication.BackColor = System.Drawing.Color.White;
            this.pnlCommunication.Controls.Add(this.pnlNewMessage);
            this.pnlCommunication.Controls.Add(this.pnlChatHistory);
            this.pnlCommunication.Dock = System.Windows.Forms.DockStyle.Fill;
            this.pnlCommunication.Location = new System.Drawing.Point(0, 0);
            this.pnlCommunication.Margin = new System.Windows.Forms.Padding(4);
            this.pnlCommunication.Name = "pnlCommunication";
            this.pnlCommunication.Size = new System.Drawing.Size(1134, 789);
            this.pnlCommunication.TabIndex = 0;
            // 
            // pnlNewMessage
            // 
            this.pnlNewMessage.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(240)))), ((int)(((byte)(242)))), ((int)(((byte)(245)))));
            this.pnlNewMessage.Controls.Add(this.btnAddAttachment);
            this.pnlNewMessage.Controls.Add(this.btnSendMessage);
            this.pnlNewMessage.Controls.Add(this.txtNewMessage);
            this.pnlNewMessage.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.pnlNewMessage.Location = new System.Drawing.Point(0, 666);
            this.pnlNewMessage.Margin = new System.Windows.Forms.Padding(4);
            this.pnlNewMessage.Name = "pnlNewMessage";
            this.pnlNewMessage.Padding = new System.Windows.Forms.Padding(11, 10, 11, 10);
            this.pnlNewMessage.Size = new System.Drawing.Size(1134, 123);
            this.pnlNewMessage.TabIndex = 1;
            // 
            // btnAddAttachment
            // 
            this.btnAddAttachment.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btnAddAttachment.Location = new System.Drawing.Point(966, 10);
            this.btnAddAttachment.Margin = new System.Windows.Forms.Padding(4);
            this.btnAddAttachment.Name = "btnAddAttachment";
            this.btnAddAttachment.Size = new System.Drawing.Size(153, 37);
            this.btnAddAttachment.TabIndex = 2;
            this.btnAddAttachment.Text = "Dodaj załącznik";
            this.btnAddAttachment.UseVisualStyleBackColor = true;
            this.btnAddAttachment.Click += new System.EventHandler(this.btnAddAttachment_Click);
            // 
            // btnSendMessage
            // 
            this.btnSendMessage.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btnSendMessage.Location = new System.Drawing.Point(966, 68);
            this.btnSendMessage.Margin = new System.Windows.Forms.Padding(4);
            this.btnSendMessage.Name = "btnSendMessage";
            this.btnSendMessage.Size = new System.Drawing.Size(153, 44);
            this.btnSendMessage.TabIndex = 1;
            this.btnSendMessage.Text = "Wyślij";
            this.btnSendMessage.UseVisualStyleBackColor = true;
            this.btnSendMessage.Click += new System.EventHandler(this.btnSendMessage_Click);
            // 
            // txtNewMessage
            // 
            this.txtNewMessage.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.txtNewMessage.Location = new System.Drawing.Point(15, 10);
            this.txtNewMessage.Margin = new System.Windows.Forms.Padding(4);
            this.txtNewMessage.Multiline = true;
            this.txtNewMessage.Name = "txtNewMessage";
            this.txtNewMessage.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
            this.txtNewMessage.Size = new System.Drawing.Size(934, 102);
            this.txtNewMessage.TabIndex = 0;
            // 
            // pnlChatHistory
            // 
            this.pnlChatHistory.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.pnlChatHistory.AutoScroll = true;
            this.pnlChatHistory.BackColor = System.Drawing.Color.White;
            this.pnlChatHistory.FlowDirection = System.Windows.Forms.FlowDirection.TopDown;
            this.pnlChatHistory.Location = new System.Drawing.Point(4, 0);
            this.pnlChatHistory.Margin = new System.Windows.Forms.Padding(4);
            this.pnlChatHistory.Name = "pnlChatHistory";
            this.pnlChatHistory.Size = new System.Drawing.Size(1236, 666);
            this.pnlChatHistory.TabIndex = 0;
            this.pnlChatHistory.WrapContents = false;
            // 
            // openFileDialog1
            // 
            this.openFileDialog1.FileName = "openFileDialog1";
            // 
            // FormAllegroIssue
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1685, 838);
            this.Controls.Add(this.splitMain);
            this.Controls.Add(this.pnlHeader);
            this.Margin = new System.Windows.Forms.Padding(4);
            this.MinimumSize = new System.Drawing.Size(1359, 875);
            this.Name = "FormAllegroIssue";
            this.Text = "Zarządzanie dyskusją Allegro";
            this.Load += new System.EventHandler(this.FormAllegroIssue_Load);
            this.pnlHeader.ResumeLayout(false);
            this.pnlHeader.PerformLayout();
            this.splitMain.Panel1.ResumeLayout(false);
            this.splitMain.Panel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.splitMain)).EndInit();
            this.splitMain.ResumeLayout(false);
            this.pnlActions.ResumeLayout(false);
            this.gbOrder.ResumeLayout(false);
            this.gbReturnDecision.ResumeLayout(false);
            this.gbChangeStatus.ResumeLayout(false);
            this.gbChangeStatus.PerformLayout();
            this.pnlPartialRefund.ResumeLayout(false);
            this.pnlPartialRefund.PerformLayout();
            this.gbStatusAndDeadlines.ResumeLayout(false);
            this.gbStatusAndDeadlines.PerformLayout();
            this.pnlCommunication.ResumeLayout(false);
            this.pnlNewMessage.ResumeLayout(false);
            this.pnlNewMessage.PerformLayout();
            this.ResumeLayout(false);

        }
    }
}