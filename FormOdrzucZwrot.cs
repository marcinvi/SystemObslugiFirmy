// Plik: FormOdrzucZwrot.cs (WERSJA Z POLSKIMI TŁUMACZENIAMI)
// Opis: Przetłumaczono powody odrzucenia zwrotu na język polski
//       w interfejsie użytkownika.

using Reklamacje_Dane.Allegro.Returns;
using Reklamacje_Dane.Models;
using System;
using System.Collections.Generic;
using System.Windows.Forms;

namespace Reklamacje_Dane
{
    public partial class FormOdrzucZwrot : Form
    {
        public RejectCustomerReturnRequest Result { get; private set; }

        private class RejectionReasonItem
        {
            public string Code { get; set; }
            public string Description { get; set; }
            public override string ToString() => Description;
        }

        public FormOdrzucZwrot()
        {
            InitializeComponent();
        

            // Włącz sprawdzanie pisowni dla wszystkich TextBoxów
            EnableSpellCheckOnAllTextBoxes();
        }

        private void FormOdrzucZwrot_Load(object sender, EventArgs e)
        {
            PopulateReasons();
        }

        private void PopulateReasons()
        {
            // ########## POPRAWKA - Przetłumaczono opisy na język polski ##########
            var reasons = new List<RejectionReasonItem>
            {
                new RejectionReasonItem { Code = "BUYER_FAULT", Description = "Wina kupującego" },
                new RejectionReasonItem { Code = "SENT_AFTER_TIME", Description = "Wysłano po terminie" },
                new RejectionReasonItem { Code = "WRONG_ADDRESS", Description = "Wysłano na zły adres" },
                new RejectionReasonItem { Code = "RETURN_NOT_REGISTERED_IN_SYSTEM", Description = "Zwrot niezarejestrowany w systemie" },
                new RejectionReasonItem { Code = "OTHER", Description = "Inny powód (wymaga uzasadnienia)" }
            };

            comboPowod.DataSource = reasons;
            comboPowod.DisplayMember = "Description";
            comboPowod.ValueMember = "Code";
            comboPowod.SelectedIndex = 0;
        }

        private void btnOdrzuc_Click(object sender, EventArgs e)
        {
            if (comboPowod.SelectedItem == null)
            {
                MessageBox.Show("Proszę wybrać powód odrzucenia.", "Błąd Walidacji", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            var selectedReason = (RejectionReasonItem)comboPowod.SelectedItem;

            Result = new RejectCustomerReturnRequest
            {
                Rejection = new RejectionDetails
                {
                    Code = selectedReason.Code, // Do API wysyłamy angielski kod
                    Reason = txtUzasadnienie.Text.Trim()
                }
            };

            this.DialogResult = DialogResult.OK;
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