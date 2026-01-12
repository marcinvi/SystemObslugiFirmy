namespace Reklamacje_Dane
{
    partial class DpdCourierPanelForm
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
            this.tabControlMain = new System.Windows.Forms.TabControl();
            this.tabPageShipment = new System.Windows.Forms.TabPage();
            this.btnGenerateLabel = new System.Windows.Forms.Button();
            this.btnGenerateProtocol = new System.Windows.Forms.Button();
            this.groupBoxPickup = new System.Windows.Forms.GroupBox();
            this.labelPickupTime = new System.Windows.Forms.Label();
            this.comboBoxPickupTime = new System.Windows.Forms.ComboBox();
            this.dateTimePickerPickup = new System.Windows.Forms.DateTimePicker();
            this.labelPickupDate = new System.Windows.Forms.Label();
            this.btnOrderCourier = new System.Windows.Forms.Button();
            this.groupBoxServices = new System.Windows.Forms.GroupBox();
            this.checkBoxROD = new System.Windows.Forms.CheckBox();
            this.checkBoxCOD = new System.Windows.Forms.CheckBox();
            this.textBoxCODAmount = new System.Windows.Forms.TextBox();
            this.labelCODAmount = new System.Windows.Forms.Label();
            this.checkBoxDeclaredValue = new System.Windows.Forms.CheckBox();
            this.textBoxDeclaredValueAmount = new System.Windows.Forms.TextBox();
            this.labelDeclaredValue = new System.Windows.Forms.Label();
            this.groupBoxPackage = new System.Windows.Forms.GroupBox();
            this.labelContent = new System.Windows.Forms.Label();
            this.textBoxContent = new System.Windows.Forms.TextBox();
            this.labelSizeZ = new System.Windows.Forms.Label();
            this.numericUpDownSizeZ = new System.Windows.Forms.NumericUpDown();
            this.labelSizeY = new System.Windows.Forms.Label();
            this.numericUpDownSizeY = new System.Windows.Forms.NumericUpDown();
            this.labelSizeX = new System.Windows.Forms.Label();
            this.numericUpDownSizeX = new System.Windows.Forms.NumericUpDown();
            this.labelWeight = new System.Windows.Forms.Label();
            this.numericUpDownWeight = new System.Windows.Forms.NumericUpDown();
            this.groupBoxReceiver = new System.Windows.Forms.GroupBox();
            this.btnValidateReceiverPostalCode = new System.Windows.Forms.Button();
            this.labelReceiverPhone = new System.Windows.Forms.Label();
            this.textBoxReceiverPhone = new System.Windows.Forms.TextBox();
            this.labelReceiverPostalCode = new System.Windows.Forms.Label();
            this.textBoxReceiverPostalCode = new System.Windows.Forms.TextBox();
            this.labelReceiverCity = new System.Windows.Forms.Label();
            this.textBoxReceiverCity = new System.Windows.Forms.TextBox();
            this.labelReceiverAddress = new System.Windows.Forms.Label();
            this.textBoxReceiverAddress = new System.Windows.Forms.TextBox();
            this.labelReceiverName = new System.Windows.Forms.Label();
            this.textBoxReceiverName = new System.Windows.Forms.TextBox();
            this.groupBoxSender = new System.Windows.Forms.GroupBox();
            this.btnValidateSenderPostalCode = new System.Windows.Forms.Button();
            this.labelSenderPhone = new System.Windows.Forms.Label();
            this.textBoxSenderPhone = new System.Windows.Forms.TextBox();
            this.labelSenderPostalCode = new System.Windows.Forms.Label();
            this.textBoxSenderPostalCode = new System.Windows.Forms.TextBox();
            this.labelSenderCity = new System.Windows.Forms.Label();
            this.textBoxSenderCity = new System.Windows.Forms.TextBox();
            this.labelSenderAddress = new System.Windows.Forms.Label();
            this.textBoxSenderAddress = new System.Windows.Forms.TextBox();
            this.labelSenderName = new System.Windows.Forms.Label();
            this.textBoxSenderName = new System.Windows.Forms.TextBox();
            this.btnCreateShipment = new System.Windows.Forms.Button();
            this.tabPageTracking = new System.Windows.Forms.TabPage();
            this.dataGridViewEvents = new System.Windows.Forms.DataGridView();
            this.btnTrackPackage = new System.Windows.Forms.Button();
            this.textBoxWaybillTrack = new System.Windows.Forms.TextBox();
            this.labelWaybillTrack = new System.Windows.Forms.Label();
            this.tabPageJournal = new System.Windows.Forms.TabPage();
            this.textBoxJournal = new System.Windows.Forms.TextBox();
            this.statusStrip1 = new System.Windows.Forms.StatusStrip();
            this.toolStripStatusLabel = new System.Windows.Forms.ToolStripStatusLabel();
            this.tabControlMain.SuspendLayout();
            this.tabPageShipment.SuspendLayout();
            this.groupBoxPickup.SuspendLayout();
            this.groupBoxServices.SuspendLayout();
            this.groupBoxPackage.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDownSizeZ)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDownSizeY)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDownSizeX)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDownWeight)).BeginInit();
            this.groupBoxReceiver.SuspendLayout();
            this.groupBoxSender.SuspendLayout();
            this.tabPageTracking.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dataGridViewEvents)).BeginInit();
            this.tabPageJournal.SuspendLayout();
            this.statusStrip1.SuspendLayout();
            this.SuspendLayout();
            // 
            // tabControlMain
            // 
            this.tabControlMain.Controls.Add(this.tabPageShipment);
            this.tabControlMain.Controls.Add(this.tabPageTracking);
            this.tabControlMain.Controls.Add(this.tabPageJournal);
            this.tabControlMain.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tabControlMain.Location = new System.Drawing.Point(0, 0);
            this.tabControlMain.Name = "tabControlMain";
            this.tabControlMain.SelectedIndex = 0;
            this.tabControlMain.Size = new System.Drawing.Size(784, 681);
            this.tabControlMain.TabIndex = 0;
            // 
            // tabPageShipment
            // 
            this.tabPageShipment.Controls.Add(this.btnGenerateLabel);
            this.tabPageShipment.Controls.Add(this.btnGenerateProtocol);
            this.tabPageShipment.Controls.Add(this.groupBoxPickup);
            this.tabPageShipment.Controls.Add(this.btnOrderCourier);
            this.tabPageShipment.Controls.Add(this.groupBoxServices);
            this.tabPageShipment.Controls.Add(this.groupBoxPackage);
            this.tabPageShipment.Controls.Add(this.groupBoxReceiver);
            this.tabPageShipment.Controls.Add(this.groupBoxSender);
            this.tabPageShipment.Controls.Add(this.btnCreateShipment);
            this.tabPageShipment.Location = new System.Drawing.Point(4, 22);
            this.tabPageShipment.Name = "tabPageShipment";
            this.tabPageShipment.Padding = new System.Windows.Forms.Padding(3);
            this.tabPageShipment.Size = new System.Drawing.Size(776, 655);
            this.tabPageShipment.TabIndex = 0;
            this.tabPageShipment.Text = "Tworzenie Przesyłki";
            this.tabPageShipment.UseVisualStyleBackColor = true;
            // 
            // btnGenerateLabel
            // 
            this.btnGenerateLabel.Enabled = false;
            this.btnGenerateLabel.Location = new System.Drawing.Point(395, 608);
            this.btnGenerateLabel.Name = "btnGenerateLabel";
            this.btnGenerateLabel.Size = new System.Drawing.Size(120, 30);
            this.btnGenerateLabel.TabIndex = 8;
            this.btnGenerateLabel.Text = "Pobierz Etykietę";
            this.btnGenerateLabel.UseVisualStyleBackColor = true;
            this.btnGenerateLabel.Click += new System.EventHandler(this.BtnGenerateLabel_Click);
            // 
            // btnGenerateProtocol
            // 
            this.btnGenerateProtocol.Enabled = false;
            this.btnGenerateProtocol.Location = new System.Drawing.Point(521, 608);
            this.btnGenerateProtocol.Name = "btnGenerateProtocol";
            this.btnGenerateProtocol.Size = new System.Drawing.Size(120, 30);
            this.btnGenerateProtocol.TabIndex = 7;
            this.btnGenerateProtocol.Text = "Pobierz Protokół";
            this.btnGenerateProtocol.UseVisualStyleBackColor = true;
            this.btnGenerateProtocol.Click += new System.EventHandler(this.BtnGenerateProtocol_Click);
            // 
            // groupBoxPickup
            // 
            this.groupBoxPickup.Controls.Add(this.labelPickupTime);
            this.groupBoxPickup.Controls.Add(this.comboBoxPickupTime);
            this.groupBoxPickup.Controls.Add(this.dateTimePickerPickup);
            this.groupBoxPickup.Controls.Add(this.labelPickupDate);
            this.groupBoxPickup.Location = new System.Drawing.Point(8, 529);
            this.groupBoxPickup.Name = "groupBoxPickup";
            this.groupBoxPickup.Size = new System.Drawing.Size(760, 64);
            this.groupBoxPickup.TabIndex = 6;
            this.groupBoxPickup.TabStop = false;
            this.groupBoxPickup.Text = "Dane Odbioru";
            // 
            // labelPickupTime
            // 
            this.labelPickupTime.AutoSize = true;
            this.labelPickupTime.Location = new System.Drawing.Point(384, 29);
            this.labelPickupTime.Name = "labelPickupTime";
            this.labelPickupTime.Size = new System.Drawing.Size(91, 13);
            this.labelPickupTime.TabIndex = 3;
            this.labelPickupTime.Text = "Godzina odbioru:";
            // 
            // comboBoxPickupTime
            // 
            this.comboBoxPickupTime.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.comboBoxPickupTime.FormattingEnabled = true;
            this.comboBoxPickupTime.Location = new System.Drawing.Point(481, 25);
            this.comboBoxPickupTime.Name = "comboBoxPickupTime";
            this.comboBoxPickupTime.Size = new System.Drawing.Size(264, 21);
            this.comboBoxPickupTime.TabIndex = 2;
            // 
            // dateTimePickerPickup
            // 
            this.dateTimePickerPickup.Location = new System.Drawing.Point(97, 25);
            this.dateTimePickerPickup.Name = "dateTimePickerPickup";
            this.dateTimePickerPickup.Size = new System.Drawing.Size(200, 20);
            this.dateTimePickerPickup.TabIndex = 1;
            // 
            // labelPickupDate
            // 
            this.labelPickupDate.AutoSize = true;
            this.labelPickupDate.Location = new System.Drawing.Point(16, 29);
            this.labelPickupDate.Name = "labelPickupDate";
            this.labelPickupDate.Size = new System.Drawing.Size(75, 13);
            this.labelPickupDate.TabIndex = 0;
            this.labelPickupDate.Text = "Data odbioru:";
            // 
            // btnOrderCourier
            // 
            this.btnOrderCourier.Enabled = false;
            this.btnOrderCourier.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(238)));
            this.btnOrderCourier.Location = new System.Drawing.Point(647, 608);
            this.btnOrderCourier.Name = "btnOrderCourier";
            this.btnOrderCourier.Size = new System.Drawing.Size(121, 30);
            this.btnOrderCourier.TabIndex = 5;
            this.btnOrderCourier.Text = "2. Zamów Kuriera";
            this.btnOrderCourier.UseVisualStyleBackColor = true;
            this.btnOrderCourier.Click += new System.EventHandler(this.BtnOrderCourier_Click);
            // 
            // groupBoxServices
            // 
            this.groupBoxServices.Controls.Add(this.checkBoxROD);
            this.groupBoxServices.Controls.Add(this.checkBoxCOD);
            this.groupBoxServices.Controls.Add(this.textBoxCODAmount);
            this.groupBoxServices.Controls.Add(this.labelCODAmount);
            this.groupBoxServices.Controls.Add(this.checkBoxDeclaredValue);
            this.groupBoxServices.Controls.Add(this.textBoxDeclaredValueAmount);
            this.groupBoxServices.Controls.Add(this.labelDeclaredValue);
            this.groupBoxServices.Location = new System.Drawing.Point(8, 411);
            this.groupBoxServices.Name = "groupBoxServices";
            this.groupBoxServices.Size = new System.Drawing.Size(760, 102);
            this.groupBoxServices.TabIndex = 4;
            this.groupBoxServices.TabStop = false;
            this.groupBoxServices.Text = "Usługi Dodatkowe";
            // 
            // checkBoxROD
            // 
            this.checkBoxROD.AutoSize = true;
            this.checkBoxROD.Location = new System.Drawing.Point(19, 74);
            this.checkBoxROD.Name = "checkBoxROD";
            this.checkBoxROD.Size = new System.Drawing.Size(139, 17);
            this.checkBoxROD.TabIndex = 6;
            this.checkBoxROD.Text = "Zwrot Dokumentów (ROD)";
            this.checkBoxROD.UseVisualStyleBackColor = true;
            // 
            // checkBoxCOD
            // 
            this.checkBoxCOD.AutoSize = true;
            this.checkBoxCOD.Location = new System.Drawing.Point(19, 51);
            this.checkBoxCOD.Name = "checkBoxCOD";
            this.checkBoxCOD.Size = new System.Drawing.Size(126, 17);
            this.checkBoxCOD.TabIndex = 5;
            this.checkBoxCOD.Text = "Pobranie (COD)";
            this.checkBoxCOD.UseVisualStyleBackColor = true;
            this.checkBoxCOD.CheckedChanged += new System.EventHandler(this.CheckBoxCOD_CheckedChanged);
            // 
            // textBoxCODAmount
            // 
            this.textBoxCODAmount.Enabled = false;
            this.textBoxCODAmount.Location = new System.Drawing.Point(269, 49);
            this.textBoxCODAmount.Name = "textBoxCODAmount";
            this.textBoxCODAmount.Size = new System.Drawing.Size(100, 20);
            this.textBoxCODAmount.TabIndex = 4;
            // 
            // labelCODAmount
            // 
            this.labelCODAmount.AutoSize = true;
            this.labelCODAmount.Location = new System.Drawing.Point(179, 52);
            this.labelCODAmount.Name = "labelCODAmount";
            this.labelCODAmount.Size = new System.Drawing.Size(84, 13);
            this.labelCODAmount.TabIndex = 3;
            this.labelCODAmount.Text = "Kwota Pobrania:";
            // 
            // checkBoxDeclaredValue
            // 
            this.checkBoxDeclaredValue.AutoSize = true;
            this.checkBoxDeclaredValue.Location = new System.Drawing.Point(19, 28);
            this.checkBoxDeclaredValue.Name = "checkBoxDeclaredValue";
            this.checkBoxDeclaredValue.Size = new System.Drawing.Size(154, 17);
            this.checkBoxDeclaredValue.TabIndex = 2;
            this.checkBoxDeclaredValue.Text = "Ubezpieczenie";
            this.checkBoxDeclaredValue.UseVisualStyleBackColor = true;
            this.checkBoxDeclaredValue.CheckedChanged += new System.EventHandler(this.CheckBoxDeclaredValue_CheckedChanged);
            // 
            // textBoxDeclaredValueAmount
            // 
            this.textBoxDeclaredValueAmount.Enabled = false;
            this.textBoxDeclaredValueAmount.Location = new System.Drawing.Point(269, 25);
            this.textBoxDeclaredValueAmount.Name = "textBoxDeclaredValueAmount";
            this.textBoxDeclaredValueAmount.Size = new System.Drawing.Size(100, 20);
            this.textBoxDeclaredValueAmount.TabIndex = 1;
            // 
            // labelDeclaredValue
            // 
            this.labelDeclaredValue.AutoSize = true;
            this.labelDeclaredValue.Location = new System.Drawing.Point(179, 28);
            this.labelDeclaredValue.Name = "labelDeclaredValue";
            this.labelDeclaredValue.Size = new System.Drawing.Size(84, 13);
            this.labelDeclaredValue.TabIndex = 0;
            this.labelDeclaredValue.Text = "Wartość (PLN):";
            // 
            // groupBoxPackage
            // 
            this.groupBoxPackage.Controls.Add(this.labelContent);
            this.groupBoxPackage.Controls.Add(this.textBoxContent);
            this.groupBoxPackage.Controls.Add(this.labelSizeZ);
            this.groupBoxPackage.Controls.Add(this.numericUpDownSizeZ);
            this.groupBoxPackage.Controls.Add(this.labelSizeY);
            this.groupBoxPackage.Controls.Add(this.numericUpDownSizeY);
            this.groupBoxPackage.Controls.Add(this.labelSizeX);
            this.groupBoxPackage.Controls.Add(this.numericUpDownSizeX);
            this.groupBoxPackage.Controls.Add(this.labelWeight);
            this.groupBoxPackage.Controls.Add(this.numericUpDownWeight);
            this.groupBoxPackage.Location = new System.Drawing.Point(8, 285);
            this.groupBoxPackage.Name = "groupBoxPackage";
            this.groupBoxPackage.Size = new System.Drawing.Size(760, 120);
            this.groupBoxPackage.TabIndex = 3;
            this.groupBoxPackage.TabStop = false;
            this.groupBoxPackage.Text = "Parametry Paczki";
            // 
            // labelContent
            // 
            this.labelContent.AutoSize = true;
            this.labelContent.Location = new System.Drawing.Point(16, 74);
            this.labelContent.Name = "labelContent";
            this.labelContent.Size = new System.Drawing.Size(63, 13);
            this.labelContent.TabIndex = 9;
            this.labelContent.Text = "Zawartość:";
            // 
            // textBoxContent
            // 
            this.textBoxContent.Location = new System.Drawing.Point(85, 71);
            this.textBoxContent.Name = "textBoxContent";
            this.textBoxContent.Size = new System.Drawing.Size(660, 20);
            this.textBoxContent.TabIndex = 8;
            // 
            // labelSizeZ
            // 
            this.labelSizeZ.AutoSize = true;
            this.labelSizeZ.Location = new System.Drawing.Point(583, 37);
            this.labelSizeZ.Name = "labelSizeZ";
            this.labelSizeZ.Size = new System.Drawing.Size(73, 13);
            this.labelSizeZ.TabIndex = 7;
            this.labelSizeZ.Text = "Głębokość (cm):";
            // 
            // numericUpDownSizeZ
            // 
            this.numericUpDownSizeZ.Location = new System.Drawing.Point(662, 35);
            this.numericUpDownSizeZ.Maximum = new decimal(new int[] {
            200,
            0,
            0,
            0});
            this.numericUpDownSizeZ.Minimum = new decimal(new int[] {
            1,
            0,
            0,
            0});
            this.numericUpDownSizeZ.Name = "numericUpDownSizeZ";
            this.numericUpDownSizeZ.Size = new System.Drawing.Size(83, 20);
            this.numericUpDownSizeZ.TabIndex = 6;
            this.numericUpDownSizeZ.Value = new decimal(new int[] {
            10,
            0,
            0,
            0});
            // 
            // labelSizeY
            // 
            this.labelSizeY.AutoSize = true;
            this.labelSizeY.Location = new System.Drawing.Point(384, 37);
            this.labelSizeY.Name = "labelSizeY";
            this.labelSizeY.Size = new System.Drawing.Size(83, 13);
            this.labelSizeY.TabIndex = 5;
            this.labelSizeY.Text = "Szerokość (cm):";
            // 
            // numericUpDownSizeY
            // 
            this.numericUpDownSizeY.Location = new System.Drawing.Point(473, 35);
            this.numericUpDownSizeY.Maximum = new decimal(new int[] {
            200,
            0,
            0,
            0});
            this.numericUpDownSizeY.Minimum = new decimal(new int[] {
            1,
            0,
            0,
            0});
            this.numericUpDownSizeY.Name = "numericUpDownSizeY";
            this.numericUpDownSizeY.Size = new System.Drawing.Size(83, 20);
            this.numericUpDownSizeY.TabIndex = 4;
            this.numericUpDownSizeY.Value = new decimal(new int[] {
            10,
            0,
            0,
            0});
            // 
            // labelSizeX
            // 
            this.labelSizeX.AutoSize = true;
            this.labelSizeX.Location = new System.Drawing.Point(198, 37);
            this.labelSizeX.Name = "labelSizeX";
            this.labelSizeX.Size = new System.Drawing.Size(77, 13);
            this.labelSizeX.TabIndex = 3;
            this.labelSizeX.Text = "Długość (cm):";
            // 
            // numericUpDownSizeX
            // 
            this.numericUpDownSizeX.Location = new System.Drawing.Point(281, 35);
            this.numericUpDownSizeX.Maximum = new decimal(new int[] {
            200,
            0,
            0,
            0});
            this.numericUpDownSizeX.Minimum = new decimal(new int[] {
            1,
            0,
            0,
            0});
            this.numericUpDownSizeX.Name = "numericUpDownSizeX";
            this.numericUpDownSizeX.Size = new System.Drawing.Size(83, 20);
            this.numericUpDownSizeX.TabIndex = 2;
            this.numericUpDownSizeX.Value = new decimal(new int[] {
            10,
            0,
            0,
            0});
            // 
            // labelWeight
            // 
            this.labelWeight.AutoSize = true;
            this.labelWeight.Location = new System.Drawing.Point(16, 37);
            this.labelWeight.Name = "labelWeight";
            this.labelWeight.Size = new System.Drawing.Size(63, 13);
            this.labelWeight.TabIndex = 1;
            this.labelWeight.Text = "Waga (kg):";
            // 
            // numericUpDownWeight
            // 
            this.numericUpDownWeight.DecimalPlaces = 2;
            this.numericUpDownWeight.Location = new System.Drawing.Point(85, 35);
            this.numericUpDownWeight.Maximum = new decimal(new int[] {
            700,
            0,
            0,
            0});
            this.numericUpDownWeight.Minimum = new decimal(new int[] {
            1,
            0,
            0,
            65536});
            this.numericUpDownWeight.Name = "numericUpDownWeight";
            this.numericUpDownWeight.Size = new System.Drawing.Size(83, 20);
            this.numericUpDownWeight.TabIndex = 0;
            this.numericUpDownWeight.Value = new decimal(new int[] {
            1,
            0,
            0,
            0});
            // 
            // groupBoxReceiver
            // 
            this.groupBoxReceiver.Controls.Add(this.btnValidateReceiverPostalCode);
            this.groupBoxReceiver.Controls.Add(this.labelReceiverPhone);
            this.groupBoxReceiver.Controls.Add(this.textBoxReceiverPhone);
            this.groupBoxReceiver.Controls.Add(this.labelReceiverPostalCode);
            this.groupBoxReceiver.Controls.Add(this.textBoxReceiverPostalCode);
            this.groupBoxReceiver.Controls.Add(this.labelReceiverCity);
            this.groupBoxReceiver.Controls.Add(this.textBoxReceiverCity);
            this.groupBoxReceiver.Controls.Add(this.labelReceiverAddress);
            this.groupBoxReceiver.Controls.Add(this.textBoxReceiverAddress);
            this.groupBoxReceiver.Controls.Add(this.labelReceiverName);
            this.groupBoxReceiver.Controls.Add(this.textBoxReceiverName);
            this.groupBoxReceiver.Location = new System.Drawing.Point(8, 146);
            this.groupBoxReceiver.Name = "groupBoxReceiver";
            this.groupBoxReceiver.Size = new System.Drawing.Size(760, 133);
            this.groupBoxReceiver.TabIndex = 2;
            this.groupBoxReceiver.TabStop = false;
            this.groupBoxReceiver.Text = "Odbiorca";
            // 
            // btnValidateReceiverPostalCode
            // 
            this.btnValidateReceiverPostalCode.Location = new System.Drawing.Point(269, 100);
            this.btnValidateReceiverPostalCode.Name = "btnValidateReceiverPostalCode";
            this.btnValidateReceiverPostalCode.Size = new System.Drawing.Size(75, 23);
            this.btnValidateReceiverPostalCode.TabIndex = 10;
            this.btnValidateReceiverPostalCode.Text = "Sprawdź";
            this.btnValidateReceiverPostalCode.UseVisualStyleBackColor = true;
            this.btnValidateReceiverPostalCode.Click += new System.EventHandler(this.BtnValidateReceiverPostalCode_Click);
            // 
            // labelReceiverPhone
            // 
            this.labelReceiverPhone.AutoSize = true;
            this.labelReceiverPhone.Location = new System.Drawing.Point(384, 105);
            this.labelReceiverPhone.Name = "labelReceiverPhone";
            this.labelReceiverPhone.Size = new System.Drawing.Size(46, 13);
            this.labelReceiverPhone.TabIndex = 9;
            this.labelReceiverPhone.Text = "Telefon:";
            // 
            // textBoxReceiverPhone
            // 
            this.textBoxReceiverPhone.Location = new System.Drawing.Point(436, 102);
            this.textBoxReceiverPhone.Name = "textBoxReceiverPhone";
            this.textBoxReceiverPhone.Size = new System.Drawing.Size(309, 20);
            this.textBoxReceiverPhone.TabIndex = 8;
            // 
            // labelReceiverPostalCode
            // 
            this.labelReceiverPostalCode.AutoSize = true;
            this.labelReceiverPostalCode.Location = new System.Drawing.Point(16, 105);
            this.labelReceiverPostalCode.Name = "labelReceiverPostalCode";
            this.labelReceiverPostalCode.Size = new System.Drawing.Size(77, 13);
            this.labelReceiverPostalCode.TabIndex = 7;
            this.labelReceiverPostalCode.Text = "Kod pocztowy:";
            // 
            // textBoxReceiverPostalCode
            // 
            this.textBoxReceiverPostalCode.Location = new System.Drawing.Point(137, 102);
            this.textBoxReceiverPostalCode.Name = "textBoxReceiverPostalCode";
            this.textBoxReceiverPostalCode.Size = new System.Drawing.Size(126, 20);
            this.textBoxReceiverPostalCode.TabIndex = 6;
            // 
            // labelReceiverCity
            // 
            this.labelReceiverCity.AutoSize = true;
            this.labelReceiverCity.Location = new System.Drawing.Point(16, 79);
            this.labelReceiverCity.Name = "labelReceiverCity";
            this.labelReceiverCity.Size = new System.Drawing.Size(71, 13);
            this.labelReceiverCity.TabIndex = 5;
            this.labelReceiverCity.Text = "Miejscowość:";
            // 
            // textBoxReceiverCity
            // 
            this.textBoxReceiverCity.Location = new System.Drawing.Point(137, 76);
            this.textBoxReceiverCity.Name = "textBoxReceiverCity";
            this.textBoxReceiverCity.Size = new System.Drawing.Size(608, 20);
            this.textBoxReceiverCity.TabIndex = 4;
            // 
            // labelReceiverAddress
            // 
            this.labelReceiverAddress.AutoSize = true;
            this.labelReceiverAddress.Location = new System.Drawing.Point(16, 53);
            this.labelReceiverAddress.Name = "labelReceiverAddress";
            this.labelReceiverAddress.Size = new System.Drawing.Size(37, 13);
            this.labelReceiverAddress.TabIndex = 3;
            this.labelReceiverAddress.Text = "Adres:";
            // 
            // textBoxReceiverAddress
            // 
            this.textBoxReceiverAddress.Location = new System.Drawing.Point(137, 50);
            this.textBoxReceiverAddress.Name = "textBoxReceiverAddress";
            this.textBoxReceiverAddress.Size = new System.Drawing.Size(608, 20);
            this.textBoxReceiverAddress.TabIndex = 2;
            // 
            // labelReceiverName
            // 
            this.labelReceiverName.AutoSize = true;
            this.labelReceiverName.Location = new System.Drawing.Point(16, 27);
            this.labelReceiverName.Name = "labelReceiverName";
            this.labelReceiverName.Size = new System.Drawing.Size(115, 13);
            this.labelReceiverName.TabIndex = 1;
            this.labelReceiverName.Text = "Nazwa / Imię i nazwisko:";
            // 
            // textBoxReceiverName
            // 
            this.textBoxReceiverName.Location = new System.Drawing.Point(137, 24);
            this.textBoxReceiverName.Name = "textBoxReceiverName";
            this.textBoxReceiverName.Size = new System.Drawing.Size(608, 20);
            this.textBoxReceiverName.TabIndex = 0;
            // 
            // groupBoxSender
            // 
            this.groupBoxSender.Controls.Add(this.btnValidateSenderPostalCode);
            this.groupBoxSender.Controls.Add(this.labelSenderPhone);
            this.groupBoxSender.Controls.Add(this.textBoxSenderPhone);
            this.groupBoxSender.Controls.Add(this.labelSenderPostalCode);
            this.groupBoxSender.Controls.Add(this.textBoxSenderPostalCode);
            this.groupBoxSender.Controls.Add(this.labelSenderCity);
            this.groupBoxSender.Controls.Add(this.textBoxSenderCity);
            this.groupBoxSender.Controls.Add(this.labelSenderAddress);
            this.groupBoxSender.Controls.Add(this.textBoxSenderAddress);
            this.groupBoxSender.Controls.Add(this.labelSenderName);
            this.groupBoxSender.Controls.Add(this.textBoxSenderName);
            this.groupBoxSender.Location = new System.Drawing.Point(8, 7);
            this.groupBoxSender.Name = "groupBoxSender";
            this.groupBoxSender.Size = new System.Drawing.Size(760, 133);
            this.groupBoxSender.TabIndex = 1;
            this.groupBoxSender.TabStop = false;
            this.groupBoxSender.Text = "Nadawca";
            // 
            // btnValidateSenderPostalCode
            // 
            this.btnValidateSenderPostalCode.Location = new System.Drawing.Point(269, 100);
            this.btnValidateSenderPostalCode.Name = "btnValidateSenderPostalCode";
            this.btnValidateSenderPostalCode.Size = new System.Drawing.Size(75, 23);
            this.btnValidateSenderPostalCode.TabIndex = 10;
            this.btnValidateSenderPostalCode.Text = "Sprawdź";
            this.btnValidateSenderPostalCode.UseVisualStyleBackColor = true;
            this.btnValidateSenderPostalCode.Click += new System.EventHandler(this.BtnValidateSenderPostalCode_Click);
            // 
            // labelSenderPhone
            // 
            this.labelSenderPhone.AutoSize = true;
            this.labelSenderPhone.Location = new System.Drawing.Point(384, 105);
            this.labelSenderPhone.Name = "labelSenderPhone";
            this.labelSenderPhone.Size = new System.Drawing.Size(46, 13);
            this.labelSenderPhone.TabIndex = 9;
            this.labelSenderPhone.Text = "Telefon:";
            // 
            // textBoxSenderPhone
            // 
            this.textBoxSenderPhone.Location = new System.Drawing.Point(436, 102);
            this.textBoxSenderPhone.Name = "textBoxSenderPhone";
            this.textBoxSenderPhone.Size = new System.Drawing.Size(309, 20);
            this.textBoxSenderPhone.TabIndex = 8;
            // 
            // labelSenderPostalCode
            // 
            this.labelSenderPostalCode.AutoSize = true;
            this.labelSenderPostalCode.Location = new System.Drawing.Point(16, 105);
            this.labelSenderPostalCode.Name = "labelSenderPostalCode";
            this.labelSenderPostalCode.Size = new System.Drawing.Size(77, 13);
            this.labelSenderPostalCode.TabIndex = 7;
            this.labelSenderPostalCode.Text = "Kod pocztowy:";
            // 
            // textBoxSenderPostalCode
            // 
            this.textBoxSenderPostalCode.Location = new System.Drawing.Point(137, 102);
            this.textBoxSenderPostalCode.Name = "textBoxSenderPostalCode";
            this.textBoxSenderPostalCode.Size = new System.Drawing.Size(126, 20);
            this.textBoxSenderPostalCode.TabIndex = 6;
            this.textBoxSenderPostalCode.Leave += new System.EventHandler(this.TextBoxSenderPostalCode_Leave);
            // 
            // labelSenderCity
            // 
            this.labelSenderCity.AutoSize = true;
            this.labelSenderCity.Location = new System.Drawing.Point(16, 79);
            this.labelSenderCity.Name = "labelSenderCity";
            this.labelSenderCity.Size = new System.Drawing.Size(71, 13);
            this.labelSenderCity.TabIndex = 5;
            this.labelSenderCity.Text = "Miejscowość:";
            // 
            // textBoxSenderCity
            // 
            this.textBoxSenderCity.Location = new System.Drawing.Point(137, 76);
            this.textBoxSenderCity.Name = "textBoxSenderCity";
            this.textBoxSenderCity.Size = new System.Drawing.Size(608, 20);
            this.textBoxSenderCity.TabIndex = 4;
            // 
            // labelSenderAddress
            // 
            this.labelSenderAddress.AutoSize = true;
            this.labelSenderAddress.Location = new System.Drawing.Point(16, 53);
            this.labelSenderAddress.Name = "labelSenderAddress";
            this.labelSenderAddress.Size = new System.Drawing.Size(37, 13);
            this.labelSenderAddress.TabIndex = 3;
            this.labelSenderAddress.Text = "Adres:";
            // 
            // textBoxSenderAddress
            // 
            this.textBoxSenderAddress.Location = new System.Drawing.Point(137, 50);
            this.textBoxSenderAddress.Name = "textBoxSenderAddress";
            this.textBoxSenderAddress.Size = new System.Drawing.Size(608, 20);
            this.textBoxSenderAddress.TabIndex = 2;
            // 
            // labelSenderName
            // 
            this.labelSenderName.AutoSize = true;
            this.labelSenderName.Location = new System.Drawing.Point(16, 27);
            this.labelSenderName.Name = "labelSenderName";
            this.labelSenderName.Size = new System.Drawing.Size(115, 13);
            this.labelSenderName.TabIndex = 1;
            this.labelSenderName.Text = "Nazwa / Imię i nazwisko:";
            // 
            // textBoxSenderName
            // 
            this.textBoxSenderName.Location = new System.Drawing.Point(137, 24);
            this.textBoxSenderName.Name = "textBoxSenderName";
            this.textBoxSenderName.Size = new System.Drawing.Size(608, 20);
            this.textBoxSenderName.TabIndex = 0;
            // 
            // btnCreateShipment
            // 
            this.btnCreateShipment.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(238)));
            this.btnCreateShipment.Location = new System.Drawing.Point(8, 608);
            this.btnCreateShipment.Name = "btnCreateShipment";
            this.btnCreateShipment.Size = new System.Drawing.Size(170, 30);
            this.btnCreateShipment.TabIndex = 0;
            this.btnCreateShipment.Text = "1. Utwórz Przesyłkę";
            this.btnCreateShipment.UseVisualStyleBackColor = true;
            this.btnCreateShipment.Click += new System.EventHandler(this.BtnCreateShipment_Click);
            // 
            // tabPageTracking
            // 
            this.tabPageTracking.Controls.Add(this.dataGridViewEvents);
            this.tabPageTracking.Controls.Add(this.btnTrackPackage);
            this.tabPageTracking.Controls.Add(this.textBoxWaybillTrack);
            this.tabPageTracking.Controls.Add(this.labelWaybillTrack);
            this.tabPageTracking.Location = new System.Drawing.Point(4, 22);
            this.tabPageTracking.Name = "tabPageTracking";
            this.tabPageTracking.Padding = new System.Windows.Forms.Padding(3);
            this.tabPageTracking.Size = new System.Drawing.Size(776, 655);
            this.tabPageTracking.TabIndex = 1;
            this.tabPageTracking.Text = "Śledzenie";
            this.tabPageTracking.UseVisualStyleBackColor = true;
            // 
            // dataGridViewEvents
            // 
            this.dataGridViewEvents.AllowUserToAddRows = false;
            this.dataGridViewEvents.AllowUserToDeleteRows = false;
            this.dataGridViewEvents.AutoSizeColumnsMode = System.Windows.Forms.DataGridViewAutoSizeColumnsMode.Fill;
            this.dataGridViewEvents.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dataGridViewEvents.Location = new System.Drawing.Point(8, 33);
            this.dataGridViewEvents.Name = "dataGridViewEvents";
            this.dataGridViewEvents.ReadOnly = true;
            this.dataGridViewEvents.Size = new System.Drawing.Size(760, 614);
            this.dataGridViewEvents.TabIndex = 3;
            // 
            // btnTrackPackage
            // 
            this.btnTrackPackage.Location = new System.Drawing.Point(395, 6);
            this.btnTrackPackage.Name = "btnTrackPackage";
            this.btnTrackPackage.Size = new System.Drawing.Size(75, 23);
            this.btnTrackPackage.TabIndex = 2;
            this.btnTrackPackage.Text = "Śledź";
            this.btnTrackPackage.UseVisualStyleBackColor = true;
            this.btnTrackPackage.Click += new System.EventHandler(this.BtnTrackPackage_Click);
            // 
            // textBoxWaybillTrack
            // 
            this.textBoxWaybillTrack.Location = new System.Drawing.Point(125, 8);
            this.textBoxWaybillTrack.Name = "textBoxWaybillTrack";
            this.textBoxWaybillTrack.Size = new System.Drawing.Size(264, 20);
            this.textBoxWaybillTrack.TabIndex = 1;
            // 
            // labelWaybillTrack
            // 
            this.labelWaybillTrack.AutoSize = true;
            this.labelWaybillTrack.Location = new System.Drawing.Point(8, 11);
            this.labelWaybillTrack.Name = "labelWaybillTrack";
            this.labelWaybillTrack.Size = new System.Drawing.Size(111, 13);
            this.labelWaybillTrack.TabIndex = 0;
            this.labelWaybillTrack.Text = "Numer listu przewozowego:";
            // 
            // tabPageJournal
            // 
            this.tabPageJournal.Controls.Add(this.textBoxJournal);
            this.tabPageJournal.Location = new System.Drawing.Point(4, 22);
            this.tabPageJournal.Name = "tabPageJournal";
            this.tabPageJournal.Padding = new System.Windows.Forms.Padding(3);
            this.tabPageJournal.Size = new System.Drawing.Size(776, 655);
            this.tabPageJournal.TabIndex = 2;
            this.tabPageJournal.Text = "Dziennik Zdarzeń";
            this.tabPageJournal.UseVisualStyleBackColor = true;
            // 
            // textBoxJournal
            // 
            this.textBoxJournal.Dock = System.Windows.Forms.DockStyle.Fill;
            this.textBoxJournal.Location = new System.Drawing.Point(3, 3);
            this.textBoxJournal.Multiline = true;
            this.textBoxJournal.Name = "textBoxJournal";
            this.textBoxJournal.ReadOnly = true;
            this.textBoxJournal.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
            this.textBoxJournal.Size = new System.Drawing.Size(770, 649);
            this.textBoxJournal.TabIndex = 0;
            // 
            // statusStrip1
            // 
            this.statusStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.toolStripStatusLabel});
            this.statusStrip1.Location = new System.Drawing.Point(0, 659);
            this.statusStrip1.Name = "statusStrip1";
            this.statusStrip1.Size = new System.Drawing.Size(784, 22);
            this.statusStrip1.TabIndex = 1;
            this.statusStrip1.Text = "statusStrip1";
            // 
            // toolStripStatusLabel
            // 
            this.toolStripStatusLabel.Name = "toolStripStatusLabel";
            this.toolStripStatusLabel.Size = new System.Drawing.Size(43, 17);
            this.toolStripStatusLabel.Text = "Gotowy";
            // 
            // DpdCourierPanelForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(784, 681);
            this.Controls.Add(this.statusStrip1);
            this.Controls.Add(this.tabControlMain);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.MaximizeBox = false;
            this.Name = "DpdCourierPanelForm";
            this.Text = "Inteligentny Panel Kuriera DPD";
            this.Load += new System.EventHandler(this.DpdCourierPanelForm_Load);
            this.tabControlMain.ResumeLayout(false);
            this.tabPageShipment.ResumeLayout(false);
            this.groupBoxPickup.ResumeLayout(false);
            this.groupBoxPickup.PerformLayout();
            this.groupBoxServices.ResumeLayout(false);
            this.groupBoxServices.PerformLayout();
            this.groupBoxPackage.ResumeLayout(false);
            this.groupBoxPackage.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDownSizeZ)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDownSizeY)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDownSizeX)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDownWeight)).EndInit();
            this.groupBoxReceiver.ResumeLayout(false);
            this.groupBoxReceiver.PerformLayout();
            this.groupBoxSender.ResumeLayout(false);
            this.groupBoxSender.PerformLayout();
            this.tabPageTracking.ResumeLayout(false);
            this.tabPageTracking.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dataGridViewEvents)).EndInit();
            this.tabPageJournal.ResumeLayout(false);
            this.tabPageJournal.PerformLayout();
            this.statusStrip1.ResumeLayout(false);
            this.statusStrip1.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();
        }

        #endregion

        private System.Windows.Forms.TabControl tabControlMain;
        private System.Windows.Forms.TabPage tabPageShipment;
        private System.Windows.Forms.Button btnCreateShipment;
        private System.Windows.Forms.TabPage tabPageTracking;
        private System.Windows.Forms.TabPage tabPageJournal;
        private System.Windows.Forms.GroupBox groupBoxSender;
        private System.Windows.Forms.Label labelSenderName;
        private System.Windows.Forms.TextBox textBoxSenderName;
        private System.Windows.Forms.Label labelSenderPhone;
        private System.Windows.Forms.TextBox textBoxSenderPhone;
        private System.Windows.Forms.Label labelSenderPostalCode;
        private System.Windows.Forms.TextBox textBoxSenderPostalCode;
        private System.Windows.Forms.Label labelSenderCity;
        private System.Windows.Forms.TextBox textBoxSenderCity;
        private System.Windows.Forms.Label labelSenderAddress;
        private System.Windows.Forms.TextBox textBoxSenderAddress;
        private System.Windows.Forms.GroupBox groupBoxReceiver;
        private System.Windows.Forms.Label labelReceiverPhone;
        private System.Windows.Forms.TextBox textBoxReceiverPhone;
        private System.Windows.Forms.Label labelReceiverPostalCode;
        private System.Windows.Forms.TextBox textBoxReceiverPostalCode;
        private System.Windows.Forms.Label labelReceiverCity;
        private System.Windows.Forms.TextBox textBoxReceiverCity;
        private System.Windows.Forms.Label labelReceiverAddress;
        private System.Windows.Forms.TextBox textBoxReceiverAddress;
        private System.Windows.Forms.Label labelReceiverName;
        private System.Windows.Forms.TextBox textBoxReceiverName;
        private System.Windows.Forms.GroupBox groupBoxPackage;
        private System.Windows.Forms.Label labelWeight;
        private System.Windows.Forms.NumericUpDown numericUpDownWeight;
        private System.Windows.Forms.Label labelSizeX;
        private System.Windows.Forms.NumericUpDown numericUpDownSizeX;
        private System.Windows.Forms.Label labelSizeZ;
        private System.Windows.Forms.NumericUpDown numericUpDownSizeZ;
        private System.Windows.Forms.Label labelSizeY;
        private System.Windows.Forms.NumericUpDown numericUpDownSizeY;
        private System.Windows.Forms.Label labelContent;
        private System.Windows.Forms.TextBox textBoxContent;
        private System.Windows.Forms.GroupBox groupBoxServices;
        private System.Windows.Forms.CheckBox checkBoxDeclaredValue;
        private System.Windows.Forms.TextBox textBoxDeclaredValueAmount;
        private System.Windows.Forms.Label labelDeclaredValue;
        private System.Windows.Forms.CheckBox checkBoxCOD;
        private System.Windows.Forms.TextBox textBoxCODAmount;
        private System.Windows.Forms.Label labelCODAmount;
        private System.Windows.Forms.CheckBox checkBoxROD;
        private System.Windows.Forms.Button btnOrderCourier;
        private System.Windows.Forms.GroupBox groupBoxPickup;
        private System.Windows.Forms.Label labelPickupTime;
        private System.Windows.Forms.ComboBox comboBoxPickupTime;
        private System.Windows.Forms.DateTimePicker dateTimePickerPickup;
        private System.Windows.Forms.Label labelPickupDate;
        private System.Windows.Forms.Button btnValidateSenderPostalCode;
        private System.Windows.Forms.Button btnValidateReceiverPostalCode;
        private System.Windows.Forms.TextBox textBoxJournal;
        private System.Windows.Forms.StatusStrip statusStrip1;
        private System.Windows.Forms.ToolStripStatusLabel toolStripStatusLabel;
        private System.Windows.Forms.Button btnGenerateLabel;
        private System.Windows.Forms.Button btnGenerateProtocol;
        private System.Windows.Forms.DataGridView dataGridViewEvents;
        private System.Windows.Forms.Button btnTrackPackage;
        private System.Windows.Forms.TextBox textBoxWaybillTrack;
        private System.Windows.Forms.Label labelWaybillTrack;
    }
}