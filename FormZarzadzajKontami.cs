using System;
using System.Windows.Forms;
using System.Collections.Generic;

namespace Reklamacje_Dane
{
    public partial class FormZarzadzajKontami : Form
    {
        private ContactRepository _repo = new ContactRepository();

        public FormZarzadzajKontami()
        {
            InitializeComponent(); // To wywołuje metodę z Designera
            WczytajKonta();
        

            // Włącz sprawdzanie pisowni dla wszystkich TextBoxów
            EnableSpellCheckOnAllTextBoxes();
        }

        private void WczytajKonta()
        {
            try
            {
                var konta = _repo.PobierzWszystkieKonta();
                dgvKonta.DataSource = null;
                dgvKonta.DataSource = konta;

                // Ukrywamy kolumny techniczne
                string[] ukryte = { "Haslo", "Podpis", "Id", "Login", "Pop3Host", "Pop3Port", "Pop3Ssl", "ImapHost", "ImapPort", "ImapSsl", "SmtpHost", "SmtpPort", "SmtpSsl" };
                foreach (var kol in ukryte)
                {
                    if (dgvKonta.Columns[kol] != null) dgvKonta.Columns[kol].Visible = false;
                }

                // Nagłówki
                if (dgvKonta.Columns["NazwaWyswietlana"] != null) dgvKonta.Columns["NazwaWyswietlana"].HeaderText = "Nazwa Konta";
                if (dgvKonta.Columns["AdresEmail"] != null) dgvKonta.Columns["AdresEmail"].HeaderText = "Adres E-mail";
                if (dgvKonta.Columns["Protokol"] != null) dgvKonta.Columns["Protokol"].HeaderText = "Typ";
                if (dgvKonta.Columns["CzyDomyslne"] != null) dgvKonta.Columns["CzyDomyslne"].HeaderText = "Domyślne?";

                dgvKonta.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
            }
            catch (Exception ex)
            {
                MessageBox.Show("Błąd wczytywania listy: " + ex.Message);
            }
        }

        private void BtnDodaj_Click(object sender, EventArgs e)
        {
            using (var frm = new FormDodajKonto(null))
            {
                if (frm.ShowDialog() == DialogResult.OK) WczytajKonta();
            }
        }

        private void BtnEdytuj_Click(object sender, EventArgs e)
        {
            if (dgvKonta.SelectedRows.Count == 0) return;
            var konto = dgvKonta.SelectedRows[0].DataBoundItem as KontoPocztowe;

            if (konto != null)
            {
                using (var frm = new FormDodajKonto(konto))
                {
                    if (frm.ShowDialog() == DialogResult.OK) WczytajKonta();
                }
            }
        }

        private void BtnUsun_Click(object sender, EventArgs e)
        {
            if (dgvKonta.SelectedRows.Count == 0) return;
            var konto = dgvKonta.SelectedRows[0].DataBoundItem as KontoPocztowe;
            if (konto == null) return;

            if (MessageBox.Show($"Usunąć konto {konto.AdresEmail}?", "Potwierdzenie", MessageBoxButtons.YesNo, MessageBoxIcon.Warning) == DialogResult.Yes)
            {
                _repo.UsunKonto(konto.Id);
                WczytajKonta();
            }
        }

        private void BtnZamknij_Click(object sender, EventArgs e)
        {
            this.Close();
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