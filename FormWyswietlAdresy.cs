// Plik: FormWyswietlAdresy.cs
using System;
using System.Collections.Generic;
using System.Data;
using System.Windows.Forms;

namespace Reklamacje_Dane
{
    public partial class FormWyswietlAdresy : Form
    {
        public FormWyswietlAdresy(DataRow data)
        {
            InitializeComponent();
            PopulateAddresses(data);
        

            // Włącz sprawdzanie pisowni dla wszystkich TextBoxów
            EnableSpellCheckOnAllTextBoxes();
        }

        private void PopulateAddresses(DataRow data)
        {
            // Adres dostawy
            lblDeliveryName.Text = $"{data["Delivery_FirstName"]} {data["Delivery_LastName"]}".Trim();
            lblDeliveryAddress.Text = $"{data["Delivery_Street"]}\n{data["Delivery_ZipCode"]} {data["Delivery_City"]}".Trim();
            lblDeliveryPhone.Text = data["Delivery_PhoneNumber"]?.ToString();
            if (string.IsNullOrWhiteSpace(lblDeliveryName.Text)) lblDeliveryName.Text = "Brak danych";
            if (string.IsNullOrWhiteSpace(lblDeliveryAddress.Text.Replace("\n", ""))) lblDeliveryAddress.Text = "Brak danych";
            if (string.IsNullOrWhiteSpace(lblDeliveryPhone.Text)) lblDeliveryPhone.Text = "Brak danych";

            // Adres kupującego
            lblBuyerName.Text = $"{data["Buyer_FirstName"]} {data["Buyer_LastName"]}".Trim();
            lblBuyerAddress.Text = $"{data["Buyer_Street"]}\n{data["Buyer_ZipCode"]} {data["Buyer_City"]}".Trim();
            lblBuyerPhone.Text = data["Buyer_PhoneNumber"]?.ToString();
            if (string.IsNullOrWhiteSpace(lblBuyerName.Text)) lblBuyerName.Text = "Brak danych";
            if (string.IsNullOrWhiteSpace(lblBuyerAddress.Text.Replace("\n", ""))) lblBuyerAddress.Text = "Brak danych";
            if (string.IsNullOrWhiteSpace(lblBuyerPhone.Text)) lblBuyerPhone.Text = "Brak danych";

            // Adres do faktury
            lblInvoiceCompany.Text = data["Invoice_CompanyName"]?.ToString();
            lblInvoiceAddress.Text = $"{data["Invoice_Street"]}\n{data["Invoice_ZipCode"]} {data["Invoice_City"]}".Trim();
            lblInvoiceTaxId.Text = $"NIP: {data["Invoice_TaxId"]}";
            if (string.IsNullOrWhiteSpace(lblInvoiceCompany.Text)) lblInvoiceCompany.Text = "Brak danych (prawdopodobnie os. fizyczna)";
            if (string.IsNullOrWhiteSpace(lblInvoiceAddress.Text.Replace("\n", ""))) lblInvoiceAddress.Text = "Brak danych";
            if (string.IsNullOrWhiteSpace(data["Invoice_TaxId"]?.ToString())) lblInvoiceTaxId.Text = "Brak NIP";

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