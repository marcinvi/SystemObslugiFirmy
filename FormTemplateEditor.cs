using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using System.Threading.Tasks;

namespace Reklamacje_Dane
{
    public partial class FormTemplateEditor : Form
    {
        // UI Controls
        private SplitContainer splitContainerMain;
        private ListBox lbSzablony;
        private TextBox txtNazwaSzablonu;
        private RichTextBox rtbEdytor;
        private TreeView treeZmienne;
        private Button btnZapisz;
        private Button btnUsun;
        private Button btnNowy;
        private ToolStrip toolStripFormatting;

        private readonly EmailTemplateService _service;

        // Korzystamy z globalnej klasy SzablonEmail (zdefiniowanej w osobnym pliku lub namespace)
        private SzablonEmail _currentTemplate;

        // USUNIĘTO klasę public class SzablonEmail { ... } stąd!

        public FormTemplateEditor()
        {
            InitializeComponentCode();
            _service = new EmailTemplateService();
            this.Load += FormTemplateEditor_Load;
        

            // Włącz sprawdzanie pisowni dla wszystkich TextBoxów
            EnableSpellCheckOnAllTextBoxes();
        }

        private async void FormTemplateEditor_Load(object sender, EventArgs e)
        {
            ZaladujDrzewoZmiennych();
            await OdswiezListeSzablonow();
            WyczyscEdytor();
        }

        private async Task OdswiezListeSzablonow()
        {
            lbSzablony.Items.Clear();
            var szablony = await _service.GetActiveTemplatesAsync();
            foreach (var s in szablony)
            {
                // Tutaj nie musimy tworzyć TemplateItem, bo nadpisaliśmy ToString() w klasie SzablonEmail
                // Ale dla zachowania spójności z Twoim kodem zostawiam, tylko naprawiam typy
                lbSzablony.Items.Add(new TemplateItem { Id = s.Id, Name = s.Nazwa, TemplateObject = s });
            }
        }

        private void ZaladujDrzewoZmiennych()
        {
            treeZmienne.Nodes.Clear();

            var nodeZgloszenie = treeZmienne.Nodes.Add("Zgłoszenie");
            AddVar(nodeZgloszenie, "Numer Zgłoszenia", "{{NrZgloszenia}}");
            AddVar(nodeZgloszenie, "Data Zgłoszenia", "{{DataZgloszenia}}");
            AddVar(nodeZgloszenie, "Opis Usterki", "{{OpisUsterki}}");
            AddVar(nodeZgloszenie, "Data Zakupu", "{{DataZakupu}}");
            AddVar(nodeZgloszenie, "Nr Faktury", "{{NrFaktury}}");
            AddVar(nodeZgloszenie, "Status Ogólny", "{{StatusOgolny}}");
            AddVar(nodeZgloszenie, "Status Klient", "{{StatusKlient}}");
            AddVar(nodeZgloszenie, "Źródło", "{{Zrodlo}}");

            var nodeKlient = treeZmienne.Nodes.Add("Klient");
            AddVar(nodeKlient, "Imię i Nazwisko", "{{KlientNazwa}}");
            AddVar(nodeKlient, "E-mail", "{{KlientEmail}}");
            AddVar(nodeKlient, "Telefon", "{{KlientTelefon}}");
            AddVar(nodeKlient, "Adres Pełny", "{{KlientAdres}}");

            var nodeProdukt = treeZmienne.Nodes.Add("Produkt");
            AddVar(nodeProdukt, "Nazwa Produktu", "{{ProduktNazwa}}");
            AddVar(nodeProdukt, "Numer Seryjny", "{{ProduktSN}}");

            var nodeAllegro = treeZmienne.Nodes.Add("Allegro");
            AddVar(nodeAllegro, "ID Dyskusji", "{{AllegroDisputeId}}");
            AddVar(nodeAllegro, "ID Konta", "{{AllegroAccountId}}");

            var nodeSystem = treeZmienne.Nodes.Add("Systemowe/Pracownik");
            AddVar(nodeSystem, "Imię Pracownika", "{{PracownikImie}}");
            AddVar(nodeSystem, "Email Pracownika", "{{PracownikEmail}}");

            treeZmienne.ExpandAll();
        }

        private void AddVar(TreeNode parent, string text, string tag)
        {
            var node = parent.Nodes.Add(text);
            node.Tag = tag;
            node.ToolTipText = tag;
        }

        private void WyczyscEdytor()
        {
            // Teraz SzablonEmail odnosi się do poprawnej klasy
            _currentTemplate = new SzablonEmail { Id = 0, Nazwa = "Nowy Szablon", TrescRtf = "", Aktywny = true };
            txtNazwaSzablonu.Text = _currentTemplate.Nazwa;
            rtbEdytor.Clear();
            lbSzablony.SelectedIndex = -1;
        }

        private void LbSzablony_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (lbSzablony.SelectedItem is TemplateItem item)
            {
                _currentTemplate = item.TemplateObject;
                txtNazwaSzablonu.Text = _currentTemplate.Nazwa;
                try { rtbEdytor.Rtf = _currentTemplate.TrescRtf; }
                catch { rtbEdytor.Text = _currentTemplate.TrescRtf; }
            }
        }

        private void TreeZmienne_NodeMouseDoubleClick(object sender, TreeNodeMouseClickEventArgs e)
        {
            if (e.Node.Tag != null)
            {
                string zmienna = e.Node.Tag.ToString();
                rtbEdytor.SelectedText = zmienna;
                rtbEdytor.Focus();
            }
        }

        private async void BtnZapisz_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(txtNazwaSzablonu.Text))
            {
                MessageBox.Show("Podaj nazwę szablonu.");
                return;
            }

            _currentTemplate.Nazwa = txtNazwaSzablonu.Text;
            _currentTemplate.TrescRtf = rtbEdytor.Rtf;
            _currentTemplate.Aktywny = true;

            await _service.ZapiszSzablonAsync(_currentTemplate);
            MessageBox.Show("Zapisano szablon.");
            await OdswiezListeSzablonow();
        }

        private async void BtnUsun_Click(object sender, EventArgs e)
        {
            if (_currentTemplate != null && _currentTemplate.Id > 0)
            {
                if (MessageBox.Show("Czy na pewno usunąć ten szablon?", "Potwierdzenie", MessageBoxButtons.YesNo) == DialogResult.Yes)
                {
                    await _service.UsunSzablonAsync(_currentTemplate.Id);
                    await OdswiezListeSzablonow();
                    WyczyscEdytor();
                }
            }
        }

        private void BtnNowy_Click(object sender, EventArgs e)
        {
            WyczyscEdytor();
            txtNazwaSzablonu.Focus();
        }

        private void ToggleBold(object sender, EventArgs e) => ToggleStyle(FontStyle.Bold);
        private void ToggleItalic(object sender, EventArgs e) => ToggleStyle(FontStyle.Italic);
        private void ToggleUnderline(object sender, EventArgs e) => ToggleStyle(FontStyle.Underline);

        private void ToggleStyle(FontStyle style)
        {
            if (rtbEdytor.SelectionFont != null)
            {
                var currentFont = rtbEdytor.SelectionFont;
                var newStyle = currentFont.Style ^ style;
                rtbEdytor.SelectionFont = new Font(currentFont, newStyle);
            }
            rtbEdytor.Focus();
        }

        // --- DESIGNER CODE ---
        private void InitializeComponentCode()
        {
            this.Text = "Edytor Szablonów Wiadomości";
            this.Size = new Size(1000, 700);
            this.StartPosition = FormStartPosition.CenterScreen;

            splitContainerMain = new SplitContainer { Dock = DockStyle.Fill };

            var panelLeft = new Panel { Dock = DockStyle.Left, Width = 200, Padding = new Padding(5) };
            var lblSzablony = new Label { Text = "Dostępne Szablony:", Dock = DockStyle.Top, Height = 25, Font = new Font("Segoe UI", 9, FontStyle.Bold) };
            lbSzablony = new ListBox { Dock = DockStyle.Fill };
            lbSzablony.SelectedIndexChanged += LbSzablony_SelectedIndexChanged;
            btnNowy = new Button { Text = "Nowy Szablon", Dock = DockStyle.Bottom, Height = 35, BackColor = Color.LightBlue };
            btnNowy.Click += BtnNowy_Click;

            panelLeft.Controls.Add(lbSzablony);
            panelLeft.Controls.Add(lblSzablony);
            panelLeft.Controls.Add(btnNowy);

            var panelRight = new Panel { Dock = DockStyle.Right, Width = 220, Padding = new Padding(5) };
            var lblZmienne = new Label { Text = "Dostępne Zmienne (2x Klik):", Dock = DockStyle.Top, Height = 25, Font = new Font("Segoe UI", 9, FontStyle.Bold) };
            treeZmienne = new TreeView { Dock = DockStyle.Fill };
            treeZmienne.NodeMouseDoubleClick += TreeZmienne_NodeMouseDoubleClick;

            panelRight.Controls.Add(treeZmienne);
            panelRight.Controls.Add(lblZmienne);

            var panelCenter = new Panel { Dock = DockStyle.Fill, Padding = new Padding(10) };
            var panelTopCenter = new Panel { Dock = DockStyle.Top, Height = 60 };
            var lblNazwa = new Label { Text = "Nazwa szablonu:", Location = new Point(0, 5), AutoSize = true };
            txtNazwaSzablonu = new TextBox { Location = new Point(0, 25), Width = 300 };

            toolStripFormatting = new ToolStrip { Dock = DockStyle.Bottom };
            toolStripFormatting.Items.Add(new ToolStripButton("B", null, ToggleBold) { Font = new Font("Segoe UI", 9, FontStyle.Bold), ToolTipText = "Pogrubienie" });
            toolStripFormatting.Items.Add(new ToolStripButton("I", null, ToggleItalic) { Font = new Font("Segoe UI", 9, FontStyle.Italic), ToolTipText = "Kursywa" });
            toolStripFormatting.Items.Add(new ToolStripButton("U", null, ToggleUnderline) { Font = new Font("Segoe UI", 9, FontStyle.Underline), ToolTipText = "Podkreślenie" });

            panelTopCenter.Controls.Add(lblNazwa);
            panelTopCenter.Controls.Add(txtNazwaSzablonu);
            panelTopCenter.Controls.Add(toolStripFormatting);

            rtbEdytor = new RichTextBox { Dock = DockStyle.Fill, BorderStyle = BorderStyle.FixedSingle, Font = new Font("Segoe UI", 10) };

            var panelBottomCenter = new Panel { Dock = DockStyle.Bottom, Height = 50, Padding = new Padding(0, 10, 0, 0) };
            btnZapisz = new Button { Text = "Zapisz Zmiany", Dock = DockStyle.Right, Width = 120, BackColor = Color.LightGreen };
            btnZapisz.Click += BtnZapisz_Click;
            btnUsun = new Button { Text = "Usuń", Dock = DockStyle.Left, Width = 80, BackColor = Color.IndianRed, ForeColor = Color.White };
            btnUsun.Click += BtnUsun_Click;

            panelBottomCenter.Controls.Add(btnZapisz);
            panelBottomCenter.Controls.Add(btnUsun);

            panelCenter.Controls.Add(rtbEdytor);
            panelCenter.Controls.Add(panelTopCenter);
            panelCenter.Controls.Add(panelBottomCenter);

            splitContainerMain.Panel1.Controls.Add(panelLeft);
            splitContainerMain.Panel2.Controls.Add(panelCenter); // Uproszczone dodawanie do splita
            // Dla pełnego układu (lewy, środek, prawy) lepiej dodać panelLeft do this, panelRight do this, a panelCenter fill.
            // Ale tutaj:
            this.Controls.Add(panelCenter);
            this.Controls.Add(panelLeft);
            this.Controls.Add(panelRight);
        }

        private class TemplateItem
        {
            public int Id { get; set; }
            public string Name { get; set; }
            public SzablonEmail TemplateObject { get; set; } // Teraz to jest ten poprawny typ
            public override string ToString() => Name;
        }
    
        /// <summary>
        /// Włącza sprawdzanie pisowni po polsku dla wszystkich TextBoxów w formularzu
        /// </summary>
        private void EnableSpellCheckOnAllTextBoxes()
        {
            try
            {
                // Włącz sprawdzanie pisowni dla wszystkich kontrolek typu TextBox i RichTextBox
                foreach (Control control in GetAllControls(this))
                {
                    if (control is RichTextBox richTextBox)
                    {
                        richTextBox.EnableSpellCheck(true);
                    }
                    else if (control is TextBox textBox && !(textBox is SpellCheckTextBox))
                    {
                        // Dla zwykłych TextBoxów - bez podkreślania (bo nie obsługują kolorów)
                        textBox.EnableSpellCheck(false);
                    }
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Błąd włączania sprawdzania pisowni: {ex.Message}");
            }
        }

        /// <summary>
        /// Rekurencyjnie pobiera wszystkie kontrolki z kontenera
        /// </summary>
        private IEnumerable<Control> GetAllControls(Control container)
        {
            foreach (Control control in container.Controls)
            {
                yield return control;

                if (control.HasChildren)
                {
                    foreach (Control child in GetAllControls(control))
                    {
                        yield return child;
                    }
                }
            }
        }
}
}