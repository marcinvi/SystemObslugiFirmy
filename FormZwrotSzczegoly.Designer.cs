// ############################################################################
// Plik: FormZwrotSzczegoly.Designer.cs (WERSJA Z NOWOCZESNYM, INTUICYJNYM UI)
// Opis: Całkowicie przeprojektowany interfejs użytkownika dla formularza
//       szczegółów zwrotu. Układ oparty na kartach, z logicznym
//       rozmieszczeniem kontrolek i intuicyjnym panelem akcji.
// Autor projektu: Gemini
// ############################################################################

namespace Reklamacje_Dane
{
    partial class FormZwrotSzczegoly
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
            this.panelTopHeader = new System.Windows.Forms.Panel();
            this.lblReturnStatus = new System.Windows.Forms.Label();
            this.lblReturnNumber = new System.Windows.Forms.Label();
            this.panelMainContainer = new System.Windows.Forms.Panel();
            this.cardPanelMagazyn = new Reklamacje_Dane.CardPanel();
            this.txtUwagiMagazynu = new System.Windows.Forms.TextBox();
            this.label10 = new System.Windows.Forms.Label();
            this.comboStanProduktu = new System.Windows.Forms.ComboBox();
            this.label9 = new System.Windows.Forms.Label();
            this.lblCardMagazynTitle = new System.Windows.Forms.Label();
            this.cardPanelKupujacy = new Reklamacje_Dane.CardPanel();
            this.btnShowOtherAddresses = new System.Windows.Forms.Button();
            this.lblBuyerPhone = new System.Windows.Forms.Label();
            this.label16 = new System.Windows.Forms.Label();
            this.lblBuyerAddress = new System.Windows.Forms.Label();
            this.label18 = new System.Windows.Forms.Label();
            this.lblBuyerName = new System.Windows.Forms.Label();
            this.label20 = new System.Windows.Forms.Label();
            this.lblCardKupujacyTitle = new System.Windows.Forms.Label();
            this.cardPanelPrzesylka = new Reklamacje_Dane.CardPanel();
            this.lblCarrier = new System.Windows.Forms.Label();
            this.label8 = new System.Windows.Forms.Label();
            this.lblWaybill = new System.Windows.Forms.Label();
            this.label7 = new System.Windows.Forms.Label();
            this.lblCardPrzesylkaTitle = new System.Windows.Forms.Label();
            this.cardPanelProdukt = new Reklamacje_Dane.CardPanel();
            this.lblReason = new System.Windows.Forms.Label();
            this.label5 = new System.Windows.Forms.Label();
            this.lblQuantity = new System.Windows.Forms.Label();
            this.label12 = new System.Windows.Forms.Label();
            this.lblOfferId = new System.Windows.Forms.Label();
            this.label13 = new System.Windows.Forms.Label();
            this.lblProductName = new System.Windows.Forms.Label();
            this.label4 = new System.Windows.Forms.Label();
            this.lblCardProduktTitle = new System.Windows.Forms.Label();
            this.panelBottomActions = new System.Windows.Forms.Panel();
            this.btnPrzekazDoHandlowca = new System.Windows.Forms.Button();
            this.btnAnuluj = new System.Windows.Forms.Button();
            this.panelTopHeader.SuspendLayout();
            this.panelMainContainer.SuspendLayout();
            this.cardPanelMagazyn.SuspendLayout();
            this.cardPanelKupujacy.SuspendLayout();
            this.cardPanelPrzesylka.SuspendLayout();
            this.cardPanelProdukt.SuspendLayout();
            this.panelBottomActions.SuspendLayout();
            this.SuspendLayout();
            // 
            // panelTopHeader
            // 
            this.panelTopHeader.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(25)))), ((int)(((byte)(118)))), ((int)(((byte)(210)))));
            this.panelTopHeader.Controls.Add(this.lblReturnStatus);
            this.panelTopHeader.Controls.Add(this.lblReturnNumber);
            this.panelTopHeader.Dock = System.Windows.Forms.DockStyle.Top;
            this.panelTopHeader.Location = new System.Drawing.Point(0, 0);
            this.panelTopHeader.Margin = new System.Windows.Forms.Padding(2, 2, 2, 2);
            this.panelTopHeader.Name = "panelTopHeader";
            this.panelTopHeader.Size = new System.Drawing.Size(663, 49);
            this.panelTopHeader.TabIndex = 0;
            // 
            // lblReturnStatus
            // 
            this.lblReturnStatus.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.lblReturnStatus.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(238)));
            this.lblReturnStatus.ForeColor = System.Drawing.Color.White;
            this.lblReturnStatus.Location = new System.Drawing.Point(473, 16);
            this.lblReturnStatus.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.lblReturnStatus.Name = "lblReturnStatus";
            this.lblReturnStatus.Size = new System.Drawing.Size(180, 19);
            this.lblReturnStatus.TabIndex = 1;
            this.lblReturnStatus.Text = "Status: Oczekuje na przyjęcie";
            this.lblReturnStatus.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // lblReturnNumber
            // 
            this.lblReturnNumber.AutoSize = true;
            this.lblReturnNumber.Font = new System.Drawing.Font("Segoe UI", 13.8F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(238)));
            this.lblReturnNumber.ForeColor = System.Drawing.Color.White;
            this.lblReturnNumber.Location = new System.Drawing.Point(9, 11);
            this.lblReturnNumber.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.lblReturnNumber.Name = "lblReturnNumber";
            this.lblReturnNumber.Size = new System.Drawing.Size(226, 25);
            this.lblReturnNumber.TabIndex = 0;
            this.lblReturnNumber.Text = "Zwrot #ZWR/123/45/67";
            // 
            // panelMainContainer
            // 
            this.panelMainContainer.AutoScroll = true;
            this.panelMainContainer.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(240)))), ((int)(((byte)(242)))), ((int)(((byte)(245)))));
            this.panelMainContainer.Controls.Add(this.cardPanelMagazyn);
            this.panelMainContainer.Controls.Add(this.cardPanelKupujacy);
            this.panelMainContainer.Controls.Add(this.cardPanelPrzesylka);
            this.panelMainContainer.Controls.Add(this.cardPanelProdukt);
            this.panelMainContainer.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panelMainContainer.Location = new System.Drawing.Point(0, 49);
            this.panelMainContainer.Margin = new System.Windows.Forms.Padding(2, 2, 2, 2);
            this.panelMainContainer.Name = "panelMainContainer";
            this.panelMainContainer.Padding = new System.Windows.Forms.Padding(11, 8, 11, 8);
            this.panelMainContainer.Size = new System.Drawing.Size(663, 553);
            this.panelMainContainer.TabIndex = 1;
            // 
            // cardPanelMagazyn
            // 
            this.cardPanelMagazyn.BackColor = System.Drawing.Color.White;
            this.cardPanelMagazyn.BorderRadius = 5;
            this.cardPanelMagazyn.Controls.Add(this.txtUwagiMagazynu);
            this.cardPanelMagazyn.Controls.Add(this.label10);
            this.cardPanelMagazyn.Controls.Add(this.comboStanProduktu);
            this.cardPanelMagazyn.Controls.Add(this.label9);
            this.cardPanelMagazyn.Controls.Add(this.lblCardMagazynTitle);
            this.cardPanelMagazyn.Dock = System.Windows.Forms.DockStyle.Top;
            this.cardPanelMagazyn.Location = new System.Drawing.Point(11, 337);
            this.cardPanelMagazyn.Margin = new System.Windows.Forms.Padding(2, 2, 2, 8);
            this.cardPanelMagazyn.Name = "cardPanelMagazyn";
            this.cardPanelMagazyn.Padding = new System.Windows.Forms.Padding(1);
            this.cardPanelMagazyn.Size = new System.Drawing.Size(641, 187);
            this.cardPanelMagazyn.TabIndex = 3;
            // 
            // txtUwagiMagazynu
            // 
            this.txtUwagiMagazynu.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.txtUwagiMagazynu.Font = new System.Drawing.Font("Segoe UI", 9F);
            this.txtUwagiMagazynu.Location = new System.Drawing.Point(15, 114);
            this.txtUwagiMagazynu.Margin = new System.Windows.Forms.Padding(2, 2, 2, 2);
            this.txtUwagiMagazynu.Multiline = true;
            this.txtUwagiMagazynu.Name = "txtUwagiMagazynu";
            this.txtUwagiMagazynu.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
            this.txtUwagiMagazynu.Size = new System.Drawing.Size(612, 58);
            this.txtUwagiMagazynu.TabIndex = 5;
            // 
            // label10
            // 
            this.label10.AutoSize = true;
            this.label10.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(238)));
            this.label10.Location = new System.Drawing.Point(12, 95);
            this.label10.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.label10.Name = "label10";
            this.label10.Size = new System.Drawing.Size(101, 15);
            this.label10.TabIndex = 4;
            this.label10.Text = "Uwagi magazynu:";
            // 
            // comboStanProduktu
            // 
            this.comboStanProduktu.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.comboStanProduktu.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.comboStanProduktu.Font = new System.Drawing.Font("Segoe UI", 9F);
            this.comboStanProduktu.FormattingEnabled = true;
            this.comboStanProduktu.Location = new System.Drawing.Point(15, 62);
            this.comboStanProduktu.Margin = new System.Windows.Forms.Padding(2, 2, 2, 2);
            this.comboStanProduktu.Name = "comboStanProduktu";
            this.comboStanProduktu.Size = new System.Drawing.Size(612, 23);
            this.comboStanProduktu.TabIndex = 3;
            // 
            // label9
            // 
            this.label9.AutoSize = true;
            this.label9.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(238)));
            this.label9.Location = new System.Drawing.Point(12, 43);
            this.label9.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.label9.Name = "label9";
            this.label9.Size = new System.Drawing.Size(115, 15);
            this.label9.TabIndex = 2;
            this.label9.Text = "Oceń stan produktu:";
            // 
            // lblCardMagazynTitle
            // 
            this.lblCardMagazynTitle.AutoSize = true;
            this.lblCardMagazynTitle.Font = new System.Drawing.Font("Segoe UI", 10.8F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(238)));
            this.lblCardMagazynTitle.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(25)))), ((int)(((byte)(118)))), ((int)(((byte)(210)))));
            this.lblCardMagazynTitle.Location = new System.Drawing.Point(11, 12);
            this.lblCardMagazynTitle.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.lblCardMagazynTitle.Name = "lblCardMagazynTitle";
            this.lblCardMagazynTitle.Size = new System.Drawing.Size(123, 20);
            this.lblCardMagazynTitle.TabIndex = 0;
            this.lblCardMagazynTitle.Text = "Panel Magazynu";
            // 
            // cardPanelKupujacy
            // 
            this.cardPanelKupujacy.BackColor = System.Drawing.Color.White;
            this.cardPanelKupujacy.BorderRadius = 5;
            this.cardPanelKupujacy.Controls.Add(this.btnShowOtherAddresses);
            this.cardPanelKupujacy.Controls.Add(this.lblBuyerPhone);
            this.cardPanelKupujacy.Controls.Add(this.label16);
            this.cardPanelKupujacy.Controls.Add(this.lblBuyerAddress);
            this.cardPanelKupujacy.Controls.Add(this.label18);
            this.cardPanelKupujacy.Controls.Add(this.lblBuyerName);
            this.cardPanelKupujacy.Controls.Add(this.label20);
            this.cardPanelKupujacy.Controls.Add(this.lblCardKupujacyTitle);
            this.cardPanelKupujacy.Dock = System.Windows.Forms.DockStyle.Top;
            this.cardPanelKupujacy.Location = new System.Drawing.Point(11, 223);
            this.cardPanelKupujacy.Margin = new System.Windows.Forms.Padding(2, 2, 2, 8);
            this.cardPanelKupujacy.Name = "cardPanelKupujacy";
            this.cardPanelKupujacy.Padding = new System.Windows.Forms.Padding(1);
            this.cardPanelKupujacy.Size = new System.Drawing.Size(641, 114);
            this.cardPanelKupujacy.TabIndex = 2;
            // 
            // btnShowOtherAddresses
            // 
            this.btnShowOtherAddresses.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btnShowOtherAddresses.Font = new System.Drawing.Font("Segoe UI", 7.8F);
            this.btnShowOtherAddresses.Location = new System.Drawing.Point(551, 61);
            this.btnShowOtherAddresses.Margin = new System.Windows.Forms.Padding(2, 2, 2, 2);
            this.btnShowOtherAddresses.Name = "btnShowOtherAddresses";
            this.btnShowOtherAddresses.Size = new System.Drawing.Size(75, 23);
            this.btnShowOtherAddresses.TabIndex = 8;
            this.btnShowOtherAddresses.Text = "Inne adresy...";
            this.btnShowOtherAddresses.UseVisualStyleBackColor = true;
            // 
            // lblBuyerPhone
            // 
            this.lblBuyerPhone.AutoSize = true;
            this.lblBuyerPhone.Font = new System.Drawing.Font("Segoe UI", 9F);
            this.lblBuyerPhone.Location = new System.Drawing.Point(109, 85);
            this.lblBuyerPhone.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.lblBuyerPhone.Name = "lblBuyerPhone";
            this.lblBuyerPhone.Size = new System.Drawing.Size(30, 15);
            this.lblBuyerPhone.TabIndex = 7;
            this.lblBuyerPhone.Text = "Brak";
            // 
            // label16
            // 
            this.label16.AutoSize = true;
            this.label16.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Bold);
            this.label16.Location = new System.Drawing.Point(12, 85);
            this.label16.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.label16.Name = "label16";
            this.label16.Size = new System.Drawing.Size(52, 15);
            this.label16.TabIndex = 6;
            this.label16.Text = "Telefon:";
            // 
            // lblBuyerAddress
            // 
            this.lblBuyerAddress.AutoSize = true;
            this.lblBuyerAddress.Font = new System.Drawing.Font("Segoe UI", 9F);
            this.lblBuyerAddress.Location = new System.Drawing.Point(109, 65);
            this.lblBuyerAddress.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.lblBuyerAddress.Name = "lblBuyerAddress";
            this.lblBuyerAddress.Size = new System.Drawing.Size(30, 15);
            this.lblBuyerAddress.TabIndex = 5;
            this.lblBuyerAddress.Text = "Brak";
            // 
            // label18
            // 
            this.label18.AutoSize = true;
            this.label18.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Bold);
            this.label18.Location = new System.Drawing.Point(12, 65);
            this.label18.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.label18.Name = "label18";
            this.label18.Size = new System.Drawing.Size(42, 15);
            this.label18.TabIndex = 4;
            this.label18.Text = "Adres:";
            // 
            // lblBuyerName
            // 
            this.lblBuyerName.AutoSize = true;
            this.lblBuyerName.Font = new System.Drawing.Font("Segoe UI", 9F);
            this.lblBuyerName.Location = new System.Drawing.Point(109, 45);
            this.lblBuyerName.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.lblBuyerName.Name = "lblBuyerName";
            this.lblBuyerName.Size = new System.Drawing.Size(30, 15);
            this.lblBuyerName.TabIndex = 3;
            this.lblBuyerName.Text = "Brak";
            // 
            // label20
            // 
            this.label20.AutoSize = true;
            this.label20.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Bold);
            this.label20.Location = new System.Drawing.Point(12, 45);
            this.label20.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.label20.Name = "label20";
            this.label20.Size = new System.Drawing.Size(97, 15);
            this.label20.TabIndex = 2;
            this.label20.Text = "Imię i Nazwisko:";
            // 
            // lblCardKupujacyTitle
            // 
            this.lblCardKupujacyTitle.AutoSize = true;
            this.lblCardKupujacyTitle.Font = new System.Drawing.Font("Segoe UI", 10.8F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(238)));
            this.lblCardKupujacyTitle.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(25)))), ((int)(((byte)(118)))), ((int)(((byte)(210)))));
            this.lblCardKupujacyTitle.Location = new System.Drawing.Point(11, 12);
            this.lblCardKupujacyTitle.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.lblCardKupujacyTitle.Name = "lblCardKupujacyTitle";
            this.lblCardKupujacyTitle.Size = new System.Drawing.Size(131, 20);
            this.lblCardKupujacyTitle.TabIndex = 0;
            this.lblCardKupujacyTitle.Text = "Dane Kupującego";
            // 
            // cardPanelPrzesylka
            // 
            this.cardPanelPrzesylka.BackColor = System.Drawing.Color.White;
            this.cardPanelPrzesylka.BorderRadius = 5;
            this.cardPanelPrzesylka.Controls.Add(this.lblCarrier);
            this.cardPanelPrzesylka.Controls.Add(this.label8);
            this.cardPanelPrzesylka.Controls.Add(this.lblWaybill);
            this.cardPanelPrzesylka.Controls.Add(this.label7);
            this.cardPanelPrzesylka.Controls.Add(this.lblCardPrzesylkaTitle);
            this.cardPanelPrzesylka.Dock = System.Windows.Forms.DockStyle.Top;
            this.cardPanelPrzesylka.Location = new System.Drawing.Point(11, 150);
            this.cardPanelPrzesylka.Margin = new System.Windows.Forms.Padding(2, 2, 2, 8);
            this.cardPanelPrzesylka.Name = "cardPanelPrzesylka";
            this.cardPanelPrzesylka.Padding = new System.Windows.Forms.Padding(1);
            this.cardPanelPrzesylka.Size = new System.Drawing.Size(641, 73);
            this.cardPanelPrzesylka.TabIndex = 1;
            // 
            // lblCarrier
            // 
            this.lblCarrier.AutoSize = true;
            this.lblCarrier.Font = new System.Drawing.Font("Segoe UI", 9F);
            this.lblCarrier.Location = new System.Drawing.Point(408, 45);
            this.lblCarrier.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.lblCarrier.Name = "lblCarrier";
            this.lblCarrier.Size = new System.Drawing.Size(30, 15);
            this.lblCarrier.TabIndex = 4;
            this.lblCarrier.Text = "Brak";
            // 
            // label8
            // 
            this.label8.AutoSize = true;
            this.label8.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Bold);
            this.label8.Location = new System.Drawing.Point(334, 45);
            this.label8.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.label8.Name = "label8";
            this.label8.Size = new System.Drawing.Size(75, 15);
            this.label8.TabIndex = 3;
            this.label8.Text = "Przewoźnik:";
            // 
            // lblWaybill
            // 
            this.lblWaybill.AutoSize = true;
            this.lblWaybill.Font = new System.Drawing.Font("Segoe UI", 9F);
            this.lblWaybill.Location = new System.Drawing.Point(109, 45);
            this.lblWaybill.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.lblWaybill.Name = "lblWaybill";
            this.lblWaybill.Size = new System.Drawing.Size(30, 15);
            this.lblWaybill.TabIndex = 2;
            this.lblWaybill.Text = "Brak";
            // 
            // label7
            // 
            this.label7.AutoSize = true;
            this.label7.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Bold);
            this.label7.Location = new System.Drawing.Point(12, 45);
            this.label7.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(53, 15);
            this.label7.TabIndex = 1;
            this.label7.Text = "Nr Listu:";
            // 
            // lblCardPrzesylkaTitle
            // 
            this.lblCardPrzesylkaTitle.AutoSize = true;
            this.lblCardPrzesylkaTitle.Font = new System.Drawing.Font("Segoe UI", 10.8F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(238)));
            this.lblCardPrzesylkaTitle.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(25)))), ((int)(((byte)(118)))), ((int)(((byte)(210)))));
            this.lblCardPrzesylkaTitle.Location = new System.Drawing.Point(11, 12);
            this.lblCardPrzesylkaTitle.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.lblCardPrzesylkaTitle.Name = "lblCardPrzesylkaTitle";
            this.lblCardPrzesylkaTitle.Size = new System.Drawing.Size(75, 20);
            this.lblCardPrzesylkaTitle.TabIndex = 0;
            this.lblCardPrzesylkaTitle.Text = "Przesyłka";
            // 
            // cardPanelProdukt
            // 
            this.cardPanelProdukt.BackColor = System.Drawing.Color.White;
            this.cardPanelProdukt.BorderRadius = 5;
            this.cardPanelProdukt.Controls.Add(this.lblReason);
            this.cardPanelProdukt.Controls.Add(this.label5);
            this.cardPanelProdukt.Controls.Add(this.lblQuantity);
            this.cardPanelProdukt.Controls.Add(this.label12);
            this.cardPanelProdukt.Controls.Add(this.lblOfferId);
            this.cardPanelProdukt.Controls.Add(this.label13);
            this.cardPanelProdukt.Controls.Add(this.lblProductName);
            this.cardPanelProdukt.Controls.Add(this.label4);
            this.cardPanelProdukt.Controls.Add(this.lblCardProduktTitle);
            this.cardPanelProdukt.Dock = System.Windows.Forms.DockStyle.Top;
            this.cardPanelProdukt.Location = new System.Drawing.Point(11, 8);
            this.cardPanelProdukt.Margin = new System.Windows.Forms.Padding(2, 2, 2, 8);
            this.cardPanelProdukt.Name = "cardPanelProdukt";
            this.cardPanelProdukt.Padding = new System.Windows.Forms.Padding(1);
            this.cardPanelProdukt.Size = new System.Drawing.Size(641, 142);
            this.cardPanelProdukt.TabIndex = 0;
            // 
            // lblReason
            // 
            this.lblReason.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.lblReason.Font = new System.Drawing.Font("Segoe UI", 9F);
            this.lblReason.Location = new System.Drawing.Point(109, 94);
            this.lblReason.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.lblReason.Name = "lblReason";
            this.lblReason.Size = new System.Drawing.Size(517, 37);
            this.lblReason.TabIndex = 8;
            this.lblReason.Text = "Brak";
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Bold);
            this.label5.Location = new System.Drawing.Point(12, 94);
            this.label5.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(48, 15);
            this.label5.TabIndex = 7;
            this.label5.Text = "Powód:";
            // 
            // lblQuantity
            // 
            this.lblQuantity.AutoSize = true;
            this.lblQuantity.Font = new System.Drawing.Font("Segoe UI", 9F);
            this.lblQuantity.Location = new System.Drawing.Point(408, 72);
            this.lblQuantity.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.lblQuantity.Name = "lblQuantity";
            this.lblQuantity.Size = new System.Drawing.Size(13, 15);
            this.lblQuantity.TabIndex = 6;
            this.lblQuantity.Text = "0";
            // 
            // label12
            // 
            this.label12.AutoSize = true;
            this.label12.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Bold);
            this.label12.Location = new System.Drawing.Point(334, 72);
            this.label12.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.label12.Name = "label12";
            this.label12.Size = new System.Drawing.Size(35, 15);
            this.label12.TabIndex = 5;
            this.label12.Text = "Ilość:";
            // 
            // lblOfferId
            // 
            this.lblOfferId.AutoSize = true;
            this.lblOfferId.Font = new System.Drawing.Font("Segoe UI", 9F);
            this.lblOfferId.Location = new System.Drawing.Point(109, 72);
            this.lblOfferId.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.lblOfferId.Name = "lblOfferId";
            this.lblOfferId.Size = new System.Drawing.Size(30, 15);
            this.lblOfferId.TabIndex = 4;
            this.lblOfferId.Text = "Brak";
            // 
            // label13
            // 
            this.label13.AutoSize = true;
            this.label13.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Bold);
            this.label13.Location = new System.Drawing.Point(12, 72);
            this.label13.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.label13.Name = "label13";
            this.label13.Size = new System.Drawing.Size(63, 15);
            this.label13.TabIndex = 3;
            this.label13.Text = "ID Oferty:";
            // 
            // lblProductName
            // 
            this.lblProductName.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.lblProductName.Font = new System.Drawing.Font("Segoe UI", 9F);
            this.lblProductName.Location = new System.Drawing.Point(109, 45);
            this.lblProductName.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.lblProductName.Name = "lblProductName";
            this.lblProductName.Size = new System.Drawing.Size(517, 16);
            this.lblProductName.TabIndex = 2;
            this.lblProductName.Text = "Brak";
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Bold);
            this.label4.Location = new System.Drawing.Point(12, 45);
            this.label4.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(55, 15);
            this.label4.TabIndex = 1;
            this.label4.Text = "Produkt:";
            // 
            // lblCardProduktTitle
            // 
            this.lblCardProduktTitle.AutoSize = true;
            this.lblCardProduktTitle.Font = new System.Drawing.Font("Segoe UI", 10.8F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(238)));
            this.lblCardProduktTitle.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(25)))), ((int)(((byte)(118)))), ((int)(((byte)(210)))));
            this.lblCardProduktTitle.Location = new System.Drawing.Point(11, 12);
            this.lblCardProduktTitle.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.lblCardProduktTitle.Name = "lblCardProduktTitle";
            this.lblCardProduktTitle.Size = new System.Drawing.Size(136, 20);
            this.lblCardProduktTitle.TabIndex = 0;
            this.lblCardProduktTitle.Text = "Zwracany Produkt";
            // 
            // panelBottomActions
            // 
            this.panelBottomActions.BackColor = System.Drawing.Color.WhiteSmoke;
            this.panelBottomActions.Controls.Add(this.btnPrzekazDoHandlowca);
            this.panelBottomActions.Controls.Add(this.btnAnuluj);
            this.panelBottomActions.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.panelBottomActions.Location = new System.Drawing.Point(0, 602);
            this.panelBottomActions.Margin = new System.Windows.Forms.Padding(2, 2, 2, 2);
            this.panelBottomActions.Name = "panelBottomActions";
            this.panelBottomActions.Size = new System.Drawing.Size(663, 57);
            this.panelBottomActions.TabIndex = 2;
            // 
            // btnPrzekazDoHandlowca
            // 
            this.btnPrzekazDoHandlowca.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btnPrzekazDoHandlowca.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(152)))), ((int)(((byte)(0)))));
            this.btnPrzekazDoHandlowca.FlatAppearance.BorderSize = 0;
            this.btnPrzekazDoHandlowca.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnPrzekazDoHandlowca.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Bold);
            this.btnPrzekazDoHandlowca.ForeColor = System.Drawing.Color.White;
            this.btnPrzekazDoHandlowca.Location = new System.Drawing.Point(481, 12);
            this.btnPrzekazDoHandlowca.Margin = new System.Windows.Forms.Padding(2, 2, 2, 2);
            this.btnPrzekazDoHandlowca.Name = "btnPrzekazDoHandlowca";
            this.btnPrzekazDoHandlowca.Size = new System.Drawing.Size(172, 32);
            this.btnPrzekazDoHandlowca.TabIndex = 2;
            this.btnPrzekazDoHandlowca.Text = "✔ Przekaż do Handlowca";
            this.btnPrzekazDoHandlowca.UseVisualStyleBackColor = false;
            // 
            // btnAnuluj
            // 
            this.btnAnuluj.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btnAnuluj.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(224)))), ((int)(((byte)(224)))), ((int)(((byte)(224)))));
            this.btnAnuluj.FlatAppearance.BorderSize = 0;
            this.btnAnuluj.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnAnuluj.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Bold);
            this.btnAnuluj.ForeColor = System.Drawing.Color.Black;
            this.btnAnuluj.Location = new System.Drawing.Point(274, 12);
            this.btnAnuluj.Margin = new System.Windows.Forms.Padding(2, 2, 2, 2);
            this.btnAnuluj.Name = "btnAnuluj";
            this.btnAnuluj.Size = new System.Drawing.Size(98, 32);
            this.btnAnuluj.TabIndex = 1;
            this.btnAnuluj.Text = "Anuluj";
            this.btnAnuluj.UseVisualStyleBackColor = false;
            // 
            // FormZwrotSzczegoly
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(663, 659);
            this.Controls.Add(this.panelMainContainer);
            this.Controls.Add(this.panelBottomActions);
            this.Controls.Add(this.panelTopHeader);
            this.Margin = new System.Windows.Forms.Padding(2, 2, 2, 2);
            this.MinimumSize = new System.Drawing.Size(679, 698);
            this.Name = "FormZwrotSzczegoly";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "Szczegóły Zwrotu";
            this.panelTopHeader.ResumeLayout(false);
            this.panelTopHeader.PerformLayout();
            this.panelMainContainer.ResumeLayout(false);
            this.cardPanelMagazyn.ResumeLayout(false);
            this.cardPanelMagazyn.PerformLayout();
            this.cardPanelKupujacy.ResumeLayout(false);
            this.cardPanelKupujacy.PerformLayout();
            this.cardPanelPrzesylka.ResumeLayout(false);
            this.cardPanelPrzesylka.PerformLayout();
            this.cardPanelProdukt.ResumeLayout(false);
            this.cardPanelProdukt.PerformLayout();
            this.panelBottomActions.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        // Definicje kontrolek
        private System.Windows.Forms.Panel panelTopHeader;
        private System.Windows.Forms.Label lblReturnStatus;
        private System.Windows.Forms.Label lblReturnNumber;
        private System.Windows.Forms.Panel panelMainContainer;
        private System.Windows.Forms.Panel panelBottomActions;
        private System.Windows.Forms.Button btnPrzekazDoHandlowca;
        private System.Windows.Forms.Button btnAnuluj;

        // Karty
        private Reklamacje_Dane.CardPanel cardPanelProdukt;
        private System.Windows.Forms.Label lblCardProduktTitle;
        private Reklamacje_Dane.CardPanel cardPanelPrzesylka;
        private System.Windows.Forms.Label lblCardPrzesylkaTitle;
        private Reklamacje_Dane.CardPanel cardPanelKupujacy;
        private System.Windows.Forms.Label lblCardKupujacyTitle;
        private Reklamacje_Dane.CardPanel cardPanelMagazyn;
        private System.Windows.Forms.Label lblCardMagazynTitle;

        // Kontrolki z danymi
        private System.Windows.Forms.TextBox txtUwagiMagazynu;
        private System.Windows.Forms.Label label10;
        private System.Windows.Forms.ComboBox comboStanProduktu;
        private System.Windows.Forms.Label label9;
        private System.Windows.Forms.Button btnShowOtherAddresses;
        private System.Windows.Forms.Label lblBuyerPhone;
        private System.Windows.Forms.Label label16;
        private System.Windows.Forms.Label lblBuyerAddress;
        private System.Windows.Forms.Label label18;
        private System.Windows.Forms.Label lblBuyerName;
        private System.Windows.Forms.Label label20;
        private System.Windows.Forms.Label lblCarrier;
        private System.Windows.Forms.Label label8;
        private System.Windows.Forms.Label lblWaybill;
        private System.Windows.Forms.Label label7;
        private System.Windows.Forms.Label lblReason;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.Label lblQuantity;
        private System.Windows.Forms.Label label12;
        private System.Windows.Forms.Label lblOfferId;
        private System.Windows.Forms.Label label13;
        private System.Windows.Forms.Label lblProductName;
        private System.Windows.Forms.Label label4;
    }

  

         
        
    
}