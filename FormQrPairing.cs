using System;
using System.Drawing;
using System.Windows.Forms;
using Newtonsoft.Json;
using QRCoder;

namespace Reklamacje_Dane
{
    public class FormQrPairing : Form
    {
        private readonly QrPairingServer _server;
        private readonly PictureBox _pictureBox;
        private readonly Label _statusLabel;
        private readonly Button _btnClose;

        public QrPairingRequest PairingRequest { get; private set; }

        public FormQrPairing(QrPairingPayload payload, QrPairingServer server)
        {
            _server = server ?? throw new ArgumentNullException(nameof(server));
            if (payload == null) throw new ArgumentNullException(nameof(payload));

            Text = "Parowanie przez QR";
            ClientSize = new Size(420, 520);
            FormBorderStyle = FormBorderStyle.FixedDialog;
            MaximizeBox = false;
            MinimizeBox = false;
            StartPosition = FormStartPosition.CenterParent;

            _pictureBox = new PictureBox
            {
                Location = new Point(30, 20),
                Size = new Size(360, 360),
                SizeMode = PictureBoxSizeMode.Zoom,
                BorderStyle = BorderStyle.FixedSingle
            };

            _statusLabel = new Label
            {
                Location = new Point(30, 400),
                Size = new Size(360, 50),
                TextAlign = ContentAlignment.MiddleCenter,
                Text = "Czekam na zeskanowanie QR z telefonu...",
                ForeColor = Color.DodgerBlue,
                BackColor = Color.AliceBlue,
                BorderStyle = BorderStyle.FixedSingle
            };

            _btnClose = new Button
            {
                Location = new Point(30, 460),
                Size = new Size(360, 35),
                Text = "Zamknij",
                BackColor = Color.Gainsboro,
                FlatStyle = FlatStyle.Flat
            };
            _btnClose.FlatAppearance.BorderSize = 0;
            _btnClose.Click += (s, e) => Close();

            Controls.Add(_pictureBox);
            Controls.Add(_statusLabel);
            Controls.Add(_btnClose);

            GenerateQr(payload);
            _server.PairingReceived += HandlePairingReceived;
        }

        private void GenerateQr(QrPairingPayload payload)
        {
            string json = JsonConvert.SerializeObject(payload);
            using (var generator = new QRCodeGenerator())
            using (QRCodeData data = generator.CreateQrCode(json, QRCodeGenerator.ECCLevel.Q))
            using (var qrCode = new QRCode(data))
            {
                _pictureBox.Image = qrCode.GetGraphic(10);
            }
        }

        private void HandlePairingReceived(QrPairingRequest request)
        {
            if (InvokeRequired)
            {
                BeginInvoke(new Action(() => HandlePairingReceived(request)));
                return;
            }

            PairingRequest = request;
            _statusLabel.Text = "✅ Odebrano dane z telefonu.";
            _statusLabel.ForeColor = Color.ForestGreen;
            DialogResult = DialogResult.OK;
            Close();
        }

        protected override void OnFormClosed(FormClosedEventArgs e)
        {
            _server.PairingReceived -= HandlePairingReceived;
            base.OnFormClosed(e);
        }
    }
}
