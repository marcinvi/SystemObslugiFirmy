// Plik: FormZwrotWplaty.cs
// Opis: Formularz do zlecania zwrotu wpłaty – dostosowany UI i logika.

// Używamy modeli z Allegro, m.in. PaymentRefundRequest, RefundLineItem, PaymentId itd.
using Reklamacje_Dane.Allegro;
using Reklamacje_Dane.Allegro.Models;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Windows.Forms;

namespace Reklamacje_Dane
{
    public partial class FormZwrotWplaty : Form
    {
        private readonly OrderDetails _orderDetails;

        /// <summary>
        /// Wynik wypełnienia formularza – obiekt PaymentRefundRequest, który
        /// później przekazujemy do AllegroApiClient.RefundPaymentAsync().
        /// </summary>
        public PaymentRefundRequest Result { get; private set; }

        private class RefundReasonItem
        {
            public string Code { get; set; }
            public string Description { get; set; }
            public override string ToString() => Description;
        }

        public FormZwrotWplaty(OrderDetails orderDetails)
        {
            InitializeComponent();
            _orderDetails = orderDetails;
        

            // Włącz sprawdzanie pisowni dla wszystkich TextBoxów
            EnableSpellCheckOnAllTextBoxes();
        }

        private void FormZwrotWplaty_Load(object sender, EventArgs e)
        {
            lblTitle.Text = $"Zwrot wpłaty dla zamówienia: {_orderDetails.Id}";
            PopulateLineItems();
            PopulateDelivery();
            PopulateReasons();
            UpdateTotalAmount();
        }

        /// <summary>
        /// Dynamicznie buduje listę pozycji z zamówienia w TableLayoutPanel.
        /// </summary>
        private void PopulateLineItems()
        {
            tlpLineItems.SuspendLayout();
            tlpLineItems.Controls.Clear();
            tlpLineItems.RowCount = 1;

            // Nagłówki
            tlpLineItems.Controls.Add(new Label { Text = "Produkt", Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Bold), AutoSize = true }, 0, 0);
            tlpLineItems.Controls.Add(new Label { Text = "Ilość", Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Bold), Anchor = AnchorStyles.Right }, 1, 0);
            tlpLineItems.Controls.Add(new Label { Text = "Kwota zwrotu", Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Bold), Anchor = AnchorStyles.Right }, 2, 0);
            tlpLineItems.Controls.Add(new Label { Text = "Zwróć?", Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Bold), Anchor = AnchorStyles.Right }, 3, 0);

            foreach (var item in _orderDetails.LineItems)
            {
                tlpLineItems.RowCount++;
                tlpLineItems.RowStyles.Add(new RowStyle(SizeType.Absolute, 35F));

                var chk = new CheckBox { Tag = item, Checked = true, Anchor = AnchorStyles.Right };
                var txtPrice = new TextBox { Text = item.Price.Amount, Tag = item, TextAlign = HorizontalAlignment.Right, Anchor = AnchorStyles.Left | AnchorStyles.Right };

                chk.CheckedChanged += (s, e) => UpdateTotalAmount();
                txtPrice.TextChanged += txtKwota_TextChanged;

                tlpLineItems.Controls.Add(new Label { Text = item.Offer.Name, AutoSize = true, Anchor = AnchorStyles.Left }, 0, tlpLineItems.RowCount - 1);
                tlpLineItems.Controls.Add(new Label { Text = item.Quantity.ToString(), Anchor = AnchorStyles.Right }, 1, tlpLineItems.RowCount - 1);
                tlpLineItems.Controls.Add(txtPrice, 2, tlpLineItems.RowCount - 1);
                tlpLineItems.Controls.Add(chk, 3, tlpLineItems.RowCount - 1);
            }
            tlpLineItems.ResumeLayout();
        }

        /// <summary>
        /// Wypełnia dane dla kosztu dostawy.
        /// </summary>
        private void PopulateDelivery()
        {
            if (_orderDetails.Delivery?.Cost != null)
            {
                chkZwrotDostawy.Checked = true;
                txtKwotaDostawy.Text = _orderDetails.Delivery.Cost.Amount;
            }
            else
            {
                chkZwrotDostawy.Enabled = false;
                txtKwotaDostawy.Enabled = false;
                txtKwotaDostawy.Text = "0.00";
            }
        }

        /// <summary>
        /// Przygotowuje listę powodów zwrotu.
        /// </summary>
        private void PopulateReasons()
        {
            var reasons = new List<RefundReasonItem>
            {
                new RefundReasonItem { Code = "REFUND", Description = "Zwrot (np. odstąpienie od umowy)" },
                new RefundReasonItem { Code = "COMPLAINT", Description = "Reklamacja" },
                new RefundReasonItem { Code = "PRODUCT_NOT_AVAILABLE", Description = "Produkt niedostępny" }
            };

            comboPowod.DataSource = reasons;
            comboPowod.DisplayMember = nameof(RefundReasonItem.Description);
            comboPowod.ValueMember = nameof(RefundReasonItem.Code);
            comboPowod.SelectedIndex = 0;
        }

        /// <summary>
        /// Aktualizuje sumę zwrotu na etykiecie.
        /// </summary>
        private void UpdateTotalAmount()
        {
            decimal total = 0;

            for (int i = 1; i < tlpLineItems.RowCount; i++)
            {
                var chk = tlpLineItems.GetControlFromPosition(3, i) as CheckBox;
                if (chk != null && chk.Checked)
                {
                    var txt = tlpLineItems.GetControlFromPosition(2, i) as TextBox;
                    if (txt != null && decimal.TryParse(txt.Text.Replace(',', '.'), NumberStyles.Any, CultureInfo.InvariantCulture, out decimal amount))
                    {
                        total += amount;
                    }
                }
            }

            if (chkZwrotDostawy.Checked)
            {
                if (decimal.TryParse(txtKwotaDostawy.Text.Replace(',', '.'), NumberStyles.Any, CultureInfo.InvariantCulture, out decimal deliveryAmount))
                {
                    total += deliveryAmount;
                }
            }

            lblSumaZwrotu.Text = $"Suma do zwrotu: {total:C}";
        }

        /// <summary>
        /// Obsługa przycisku – buduje PaymentRefundRequest i zamyka formularz.
        /// </summary>
        private void btnZlecZwrot_Click(object sender, EventArgs e)
        {
            if (comboPowod.SelectedItem == null)
            {
                MessageBox.Show("Proszę wybrać powód zwrotu.", "Błąd walidacji", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            // budowa listy pozycji do zwrotu
            var lineItemsToRefund = new List<RefundLineItem>();
            for (int i = 1; i < tlpLineItems.RowCount; i++)
            {
                var chk = tlpLineItems.GetControlFromPosition(3, i) as CheckBox;
                if (chk != null && chk.Checked)
                {
                    var txt = tlpLineItems.GetControlFromPosition(2, i) as TextBox;
                    var originalItem = chk.Tag as LineItem;
                    if (txt != null && originalItem != null)
                    {
                        if (!decimal.TryParse(txt.Text.Replace(',', '.'), NumberStyles.Any, CultureInfo.InvariantCulture, out decimal amount))
                        {
                            MessageBox.Show($"Nieprawidłowa kwota dla produktu: {originalItem.Offer.Name}", "Błąd walidacji", MessageBoxButtons.OK, MessageBoxIcon.Error);
                            return;
                        }

                        lineItemsToRefund.Add(new RefundLineItem
                        {
                            Id = originalItem.Id,
                            Type = "LINE_ITEM",
                            Value = new RefundValue { Amount = amount.ToString("F2", CultureInfo.InvariantCulture), Currency = "PLN" }
                        });
                    }
                }
            }

            // zwrot kosztu dostawy (opcjonalny)
            RefundValue deliveryRefund = null;
            if (chkZwrotDostawy.Checked)
            {
                if (!decimal.TryParse(txtKwotaDostawy.Text.Replace(',', '.'), NumberStyles.Any, CultureInfo.InvariantCulture, out decimal deliveryAmount))
                {
                    MessageBox.Show("Nieprawidłowa kwota dla dostawy.", "Błąd walidacji", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }
                deliveryRefund = new RefundValue { Amount = deliveryAmount.ToString("F2", CultureInfo.InvariantCulture), Currency = "PLN" };
            }

            if (lineItemsToRefund.Count == 0 && deliveryRefund == null)
            {
                MessageBox.Show("Nie wybrano żadnej pozycji do zwrotu.", "Błąd walidacji", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            var selectedReason = (RefundReasonItem)comboPowod.SelectedItem;

            Result = new PaymentRefundRequest
            {
                Payment = new PaymentId { Id = _orderDetails.Payment.Id },
                Reason = selectedReason.Code,
                LineItems = lineItemsToRefund,
                Delivery = deliveryRefund,
                SellerComment = txtKomentarz.Text.Trim()
            };

            this.DialogResult = DialogResult.OK;
            this.Close();
        }

        private void chkZwrotDostawy_CheckedChanged(object sender, EventArgs e)
        {
            txtKwotaDostawy.Enabled = chkZwrotDostawy.Checked;
            UpdateTotalAmount();
        }

        private void txtKwota_TextChanged(object sender, EventArgs e)
        {
            UpdateTotalAmount();
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
