// ############################################################################
// Plik: TimelineItemControl.cs (LOGIKA - Z MENU KONTEKSTOWYM I OBSŁUGĄ KURIERA)
// ############################################################################

using System;
using System.Drawing;
using System.Text.RegularExpressions;
using System.Windows.Forms;

namespace Reklamacje_Dane
{
    public enum TimelineItemType
    {
        Action, Message, Courier, Status, Document, System
    }

    public partial class TimelineItemControl : UserControl
    {
        public object DataTag { get; set; } // ID działania z bazy
        public TimelineItemType ItemType { get; private set; }

        // Zdarzenia dla Form2
        public event EventHandler EditClicked;
        public event EventHandler DeleteClicked;
        public event EventHandler<string> OpenTrackingClicked; // Przekazuje nr listu

        private ContextMenuStrip _contextMenu;
        private string _trackingNumber; // Przechowuje wykryty numer listu

        public TimelineItemControl()
        {
            InitializeComponent();
            this.BorderStyle = BorderStyle.FixedSingle;
            InitializeContextMenu();
        }

        private void InitializeContextMenu()
        {
            _contextMenu = new ContextMenuStrip();
            this.ContextMenuStrip = _contextMenu;
            // Przypisujemy to samo menu do wszystkich dzieci (labeli), żeby działało wszędzie
            lblContent.ContextMenuStrip = _contextMenu;
            lblHeader.ContextMenuStrip = _contextMenu;
            lblDate.ContextMenuStrip = _contextMenu;
            panelLeftBorder.ContextMenuStrip = _contextMenu;
        }

        public void Setup(DateTime date, string author, string content, TimelineItemType type)
        {
            lblDate.Text = date.ToString("dd.MM.yyyy HH:mm");
            lblHeader.Text = author;
            lblContent.Text = content;
            ItemType = type;

            // Kolorowanie
            switch (type)
            {
                case TimelineItemType.Courier:
                    panelLeftBorder.BackColor = Color.DarkOrange;
                    this.BackColor = Color.FromArgb(255, 248, 225);
                    ExtractTrackingNumber(content); // Szukamy numeru listu
                    break;
                case TimelineItemType.Status:
                    panelLeftBorder.BackColor = Color.ForestGreen;
                    this.BackColor = Color.FromArgb(235, 255, 235);
                    break;
                case TimelineItemType.Message:
                    panelLeftBorder.BackColor = Color.DodgerBlue;
                    this.BackColor = Color.FromArgb(235, 245, 255);
                    break;
                case TimelineItemType.Document:
                    panelLeftBorder.BackColor = Color.Purple;
                    this.BackColor = Color.FromArgb(245, 235, 255);
                    break;
                default:
                    panelLeftBorder.BackColor = Color.Gray;
                    this.BackColor = Color.WhiteSmoke;
                    break;
            }

            BuildMenu(); // Budujemy menu dynamicznie w zależności od typu
            ResizeControl();
        }

        private void ExtractTrackingNumber(string content)
        {
            // Prosty Regex szukający ciągów cyfr/liter typowych dla DPD (np. 14 cyfr lub z końcówką)
            // Szukamy po słowie "listu:" lub podobnych, albo po prostu długiego ciągu
            var match = Regex.Match(content, @"Numer listu:?\s*([A-Z0-9]+)", RegexOptions.IgnoreCase);
            if (match.Success)
            {
                _trackingNumber = match.Groups[1].Value;
                this.Cursor = Cursors.Hand; // Zmieniamy kursor, żeby sugerować klikalność
                lblContent.Cursor = Cursors.Hand;
            }
        }

        private void BuildMenu()
        {
            _contextMenu.Items.Clear();

            if (ItemType == TimelineItemType.Courier && !string.IsNullOrEmpty(_trackingNumber))
            {
                // Menu dla Kuriera
                _contextMenu.Items.Add("Pokaż historię przesyłki", null, (s, e) => OpenTrackingClicked?.Invoke(this, _trackingNumber));
                _contextMenu.Items.Add(new ToolStripSeparator());
                _contextMenu.Items.Add("Kopiuj numer przesyłki", null, (s, e) => Clipboard.SetText(_trackingNumber));
                _contextMenu.Items.Add("Kopiuj link do śledzenia", null, (s, e) =>
                    Clipboard.SetText($"https://tracktrace.dpd.com.pl/parcelDetails?typ=1&p1={_trackingNumber}"));
            }
            else if (ItemType == TimelineItemType.Action)
            {
                // Menu dla Działań (Ręcznych)
                _contextMenu.Items.Add("Edytuj treść", null, (s, e) => EditClicked?.Invoke(this, EventArgs.Empty));
                _contextMenu.Items.Add("Usuń działanie", null, (s, e) => DeleteClicked?.Invoke(this, EventArgs.Empty));
            }

            // Opcja wspólna
            _contextMenu.Items.Add("Kopiuj treść", null, (s, e) => Clipboard.SetText(lblContent.Text));
        }

        // Obsługa kliknięcia LEWYM przyciskiem
        private void Control_Click(object sender, EventArgs e)
        {
            // Jeśli to kurier i mamy numer -> otwieramy tracking
            if (ItemType == TimelineItemType.Courier && !string.IsNullOrEmpty(_trackingNumber))
            {
                OpenTrackingClicked?.Invoke(this, _trackingNumber);
            }
            // Jeśli to zwykłe działanie -> edycja (tak jak kiedyś dwuklik)
            else if (ItemType == TimelineItemType.Action)
            {
                EditClicked?.Invoke(this, EventArgs.Empty);
            }
        }

        public void ResizeControl()
        {
            if (lblContent == null) return;
            int padding = 15;
            lblContent.MaximumSize = new Size(this.Width - 40, 0);
            int contentBottom = lblContent.Location.Y + lblContent.Height;
            this.Height = Math.Max(70, contentBottom + padding);
        }

        protected override void OnResize(EventArgs e)
        {
            base.OnResize(e);
            ResizeControl();
        }
    }
}