using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Reklamacje_Dane
{
    public partial class FormNowaWiadomosc : Form
    {
        private readonly MessageService _messageService;
        private readonly int? _replyToMessageId;
        private readonly int? _relatedReturnId;

        // Konstruktor dla nowej wiadomości
        public FormNowaWiadomosc(int? relatedReturnId = null)
        {
            InitializeComponent();
            _messageService = new MessageService(DatabaseHelper.GetConnectionString(), MagazynDatabaseHelper.GetConnectionString());
            _relatedReturnId = relatedReturnId;
        

            // Włącz sprawdzanie pisowni dla wszystkich TextBoxów
            EnableSpellCheckOnAllTextBoxes();
        }

        // Konstruktor dla odpowiedzi
        public FormNowaWiadomosc(int replyToMessageId, int originalSenderId, string originalTitle, int? relatedReturnId) : this(relatedReturnId)
        {
            _replyToMessageId = replyToMessageId;
            this.Text = "Odpowiedz na wiadomość";
            txtTytul.Text = originalTitle.StartsWith("Re: ") ? originalTitle : "Re: " + originalTitle;

            // Wczytaj użytkowników i zaznacz oryginalnego nadawcę
            Load += async (s, e) => {
                await SelectRecipient(originalSenderId);
            };
        }

        private async void FormNowaWiadomosc_Load(object sender, EventArgs e)
        {
            await LoadUsersAsync();
        }

        private async Task LoadUsersAsync()
        {
            var users = await _messageService.GetUsersAsync();
            ((ListBox)checkedListBoxOdbiorcy).DataSource = users;
            ((ListBox)checkedListBoxOdbiorcy).DisplayMember = "NazwaWyswietlana";
            ((ListBox)checkedListBoxOdbiorcy).ValueMember = "Id";
        }

        private async Task SelectRecipient(int userId)
        {
            // Czekamy na załadowanie danych
            if (checkedListBoxOdbiorcy.DataSource == null)
                await LoadUsersAsync();

            for (int i = 0; i < checkedListBoxOdbiorcy.Items.Count; i++)
            {
                if (((MessageService.User)checkedListBoxOdbiorcy.Items[i]).Id == userId)
                {
                    checkedListBoxOdbiorcy.SetItemChecked(i, true);
                    break;
                }
            }
        }

        private async void btnWyslij_Click(object sender, EventArgs e)
        {
            if (checkedListBoxOdbiorcy.CheckedItems.Count == 0)
            {
                MessageBox.Show("Wybierz przynajmniej jednego odbiorcę.", "Błąd", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }
            if (string.IsNullOrWhiteSpace(txtTresc.Text))
            {
                MessageBox.Show("Treść wiadomości nie może być pusta.", "Błąd", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            try
            {
                var recipientIds = checkedListBoxOdbiorcy.CheckedItems
                    .Cast<MessageService.User>()
                    .Select(u => u.Id);

                await _messageService.SendMessageAsync(
                    _replyToMessageId,
                    _relatedReturnId,
                    recipientIds,
                    txtTytul.Text.Trim(),
                    txtTresc.Text.Trim());

                MessageBox.Show("Wiadomość została wysłana.", "Sukces", MessageBoxButtons.OK, MessageBoxIcon.Information);
                this.DialogResult = DialogResult.OK;
                this.Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Błąd podczas wysyłania wiadomości: " + ex.Message, "Błąd", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
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