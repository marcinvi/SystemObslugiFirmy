using Microsoft.VisualBasic;
using System;
using System.Drawing;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Reklamacje_Dane
{
    public class FormCzescSzczegoly : Form
    {
        private readonly DostepnaCzescView _czesc;
        private readonly MagazynService _service;

        // Zmienne klasowe
        private TableLayoutPanel _layout;
        private Panel pnlContent;
        private Panel pnlHeader;

        public FormCzescSzczegoly(DostepnaCzescView czesc)
        {
            _czesc = czesc;
            _service = new MagazynService();
            InitializeComponent_Manual();
        }

        private void InitializeComponent_Manual()
        {
            // --- 1. OKNO GŁÓWNE ---
            this.Text = "Szczegóły Części";
            this.Size = new Size(700, 550);
            this.StartPosition = FormStartPosition.CenterParent;
            this.FormBorderStyle = FormBorderStyle.FixedDialog;
            this.MaximizeBox = false; // Zablokowane, żeby nie psuć layoutu, ale Resize działa
            this.BackColor = Color.White;
            this.MinimumSize = new Size(500, 400);

            // --- 2. NAGŁÓWEK ---
            pnlHeader = new Panel
            {
                Dock = DockStyle.Top,
                Height = 60,
                BackColor = Color.FromArgb(45, 66, 91),
                Padding = new Padding(15)
            };

            Label lblTitle = new Label
            {
                Text = _czesc.NazwaCzesci.ToUpper(),
                ForeColor = Color.White,
                Font = new Font("Segoe UI", 12, FontStyle.Bold),
                Dock = DockStyle.Fill,
                TextAlign = ContentAlignment.MiddleLeft,
                AutoEllipsis = true
            };
            pnlHeader.Controls.Add(lblTitle);

            // --- 3. TREŚĆ ---
            pnlContent = new Panel { Dock = DockStyle.Fill, AutoScroll = true, Padding = new Padding(20) };

            _layout = new TableLayoutPanel
            {
                Dock = DockStyle.Top,
                AutoSize = true,
                AutoSizeMode = AutoSizeMode.GrowAndShrink,
                ColumnCount = 1,
                RowCount = 0,
                Padding = new Padding(0, 10, 0, 0)
            };
            _layout.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100F));

            pnlContent.Controls.Add(_layout);

            // Dodajemy dane
            AddDataRow("Pochodzenie / Model:", _czesc.ModelDawcy);
            AddDataRow("Numer Zgłoszenia:", _czesc.ZgloszenieDawcy);
            AddDataRow("Numer Seryjny Dawcy:", _czesc.SnDawcy);
            AddDataRow("Lokalizacja:", _czesc.Lokalizacja);
            AddDataRow("Typ / Źródło:", _czesc.TypPochodzenia);
            AddDataRow("Opis Stanu:", _czesc.StanOpis);

            // OBSŁUGA ZMIANY ROZMIARU (To naprawia zawijanie tekstu!)
            pnlContent.SizeChanged += (s, e) => AktualizujSzerokoscTekstow();

            // --- 4. PANEL PRZYCISKÓW ---
            Panel pnlButtons = new Panel { Dock = DockStyle.Bottom, Height = 80, BackColor = Color.WhiteSmoke };

            // Obliczamy pozycje przycisków dynamicznie lub na sztywno, ale szerzej
            int btnWidth = 180;
            int gap = 20;
            int startX = (this.ClientSize.Width - (3 * btnWidth + 2 * gap)) / 2;
            if (startX < 10) startX = 10;

            Button btnUzyj = CreateButton("UŻYJ DO NAPRAWY", Color.SeaGreen, 20); // Pozycje poprawię w Resize
            btnUzyj.Click += async (s, e) => await AkcjaUzyj();

            Button btnUsun = CreateButton("USUŃ (POMYŁKA)", Color.IndianRed, 220);
            btnUsun.Click += async (s, e) => await AkcjaUsun("Usunięcie (korekta)");

            Button btnUtylizacja = CreateButton("UTYLIZACJA", Color.Gray, 420);
            btnUtylizacja.Click += async (s, e) => await AkcjaUsun("Utylizacja");

            pnlButtons.Controls.AddRange(new Control[] { btnUzyj, btnUsun, btnUtylizacja });

            // Centrowanie przycisków przy starcie
            pnlButtons.Resize += (s, e) => {
                int totalW = 3 * 180 + 40; // 3 guziki + odstępy
                int x = (pnlButtons.Width - totalW) / 2;
                if (x < 10) x = 10;
                btnUzyj.Left = x;
                btnUsun.Left = x + 180 + 20;
                btnUtylizacja.Left = x + 360 + 40;
            };

            // Składanie
            this.Controls.Add(pnlContent);
            this.Controls.Add(pnlButtons);
            this.Controls.Add(pnlHeader);

            // Wymuszenie pierwszego przeliczenia
            AktualizujSzerokoscTekstow();
        }

        private void AddDataRow(string title, string value)
        {
            if (string.IsNullOrWhiteSpace(value)) value = "-";

            Label lblTitle = new Label
            {
                Text = title,
                ForeColor = Color.Gray,
                Font = new Font("Segoe UI", 9, FontStyle.Regular),
                AutoSize = true,
                Margin = new Padding(3, 15, 3, 2)
            };

            Label lblValue = new Label
            {
                Text = value,
                ForeColor = Color.Black,
                Font = new Font("Segoe UI", 11, FontStyle.Bold),
                AutoSize = true,
                Tag = "VALUE_LABEL", // Oznaczamy label, żeby go potem znaleźć
                Anchor = AnchorStyles.Left | AnchorStyles.Right
            };

            _layout.RowCount += 2;
            _layout.RowStyles.Add(new RowStyle(SizeType.AutoSize));
            _layout.RowStyles.Add(new RowStyle(SizeType.AutoSize));
            _layout.Controls.Add(lblTitle, 0, _layout.RowCount - 2);
            _layout.Controls.Add(lblValue, 0, _layout.RowCount - 1);
        }

        // --- KLUCZOWA METODA NAPRAWIAJĄCA ZAWIJANIE ---
        private void AktualizujSzerokoscTekstow()
        {
            if (_layout == null || pnlContent == null) return;

            // Obliczamy dostępną szerokość: Szerokość panelu - marginesy (lewy+prawy) - pasek przewijania (~20px)
            int dostepnaSzerokosc = pnlContent.ClientSize.Width - pnlContent.Padding.Horizontal - 25;

            if (dostepnaSzerokosc < 100) return; // Zabezpieczenie

            _layout.SuspendLayout();
            foreach (Control c in _layout.Controls)
            {
                if (c is Label lbl && lbl.Tag?.ToString() == "VALUE_LABEL")
                {
                    // Resetujemy, a potem ustawiamy max szerokość. 
                    // To zmusza Label do przeliczenia wysokości i zawinięcia tekstu.
                    lbl.MaximumSize = new Size(dostepnaSzerokosc, 0);
                }
            }
            _layout.ResumeLayout();
        }

        private Button CreateButton(string text, Color color, int x)
        {
            return new Button
            {
                Text = text,
                Location = new Point(x, 20),
                Size = new Size(180, 45), // Duże, wygodne przyciski
                FlatStyle = FlatStyle.Flat,
                BackColor = color,
                ForeColor = Color.White,
                Font = new Font("Segoe UI", 9, FontStyle.Bold),
                Cursor = Cursors.Hand
            };
        }

        // --- AKCJE ---
        private async Task AkcjaUzyj()
        {
            using (var searchForm = new FormWybierzZgloszenie())
            {
                if (searchForm.ShowDialog() == DialogResult.OK)
                {
                    string nr = searchForm.WybranyNumerZgloszenia;
                    if (string.IsNullOrWhiteSpace(nr)) return;
                    try
                    {
                        await _service.UzyjCzescAsync(_czesc.Id, nr);
                        string logBiorca = $"WYDANIE CZĘŚCI: {_czesc.NazwaCzesci} (ID: {_czesc.Id}) do zgłoszenia {nr}. Użytkownik: {Program.fullName}";
                        await new DziennikLogger().DodajAsync(Program.fullName, logBiorca, nr);
                        new Dzialaniee().DodajNoweDzialanie(nr, Program.fullName, logBiorca);

                        if (!string.IsNullOrWhiteSpace(_czesc.ZgloszenieDawcy) && _czesc.ZgloszenieDawcy != "-" && _czesc.ZgloszenieDawcy != nr)
                        {
                            string logDawca = $"MAGAZYN: Część '{_czesc.NazwaCzesci}' (ID: {_czesc.Id}) została wydana do zgłoszenia {nr}.";
                            await new DziennikLogger().DodajAsync(Program.fullName, logDawca, _czesc.ZgloszenieDawcy);
                            new Dzialaniee().DodajNoweDzialanie(_czesc.ZgloszenieDawcy, Program.fullName, logDawca);
                        }
                        MessageBox.Show($"Część wydana do zgłoszenia {nr}.", "Sukces");
                        this.Close();
                    }
                    catch (Exception ex) { MessageBox.Show("Błąd: " + ex.Message); }
                }
            }
        }

        private async Task AkcjaUsun(string powodDomyslny)
        {
            string powod = Interaction.InputBox($"Usuwasz: {_czesc.NazwaCzesci}\nPodaj powód:", "Usuwanie", powodDomyslny);
            if (string.IsNullOrWhiteSpace(powod)) return;
            try
            {
                await _service.UsunCzescZBankuAsync(_czesc.Id);
                string log = $"USUNIĘCIE CZĘŚCI: {_czesc.NazwaCzesci} (ID: {_czesc.Id}). Powód: {powod}. Użytkownik: {Program.fullName}";
                string nrZgl = _czesc.ZgloszenieDawcy != "-" ? _czesc.ZgloszenieDawcy : "MAGAZYN";
                await new DziennikLogger().DodajAsync(Program.fullName, log, nrZgl);
                MessageBox.Show("Część usunięta.", "Info");
                this.Close();
            }
            catch (Exception ex) { MessageBox.Show("Błąd: " + ex.Message); }
        }
    }
}
