using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Reklamacje_Dane
{
    public partial class FormPhoneGallery : Form
    {
        // Lista wybranych URLi
        public List<string> SelectedPaths { get; set; } = new List<string>();
        private PhoneClient _client;

        // Elementy UI
        private FlowLayoutPanel flowPanel;
        private Button btnOk;

        public FormPhoneGallery(PhoneClient client)
        {
            InitializeCustomUI(); // Tworzenie wyglądu
            _client = client;

            // Ładowanie po wyświetleniu okna
            this.Load += async (s, e) => await LoadPhotosAsync();
        

            // Włącz sprawdzanie pisowni dla wszystkich TextBoxów
            EnableSpellCheckOnAllTextBoxes();
        }

        private void InitializeCustomUI()
        {
            this.Text = "Galeria Telefonu";
            this.Size = new Size(800, 600);
            this.StartPosition = FormStartPosition.CenterParent;

            flowPanel = new FlowLayoutPanel { Dock = DockStyle.Fill, AutoScroll = true, BackColor = Color.White };

            Panel bottomPanel = new Panel { Dock = DockStyle.Bottom, Height = 50, BackColor = Color.WhiteSmoke };
            btnOk = new Button { Text = "Dodaj zaznaczone", Dock = DockStyle.Right, Width = 150, FlatStyle = FlatStyle.Flat, BackColor = Color.SteelBlue, ForeColor = Color.White };
            btnOk.Click += (s, e) => { this.DialogResult = DialogResult.OK; this.Close(); };

            bottomPanel.Controls.Add(btnOk);
            this.Controls.Add(flowPanel);
            this.Controls.Add(bottomPanel);
        }

        private async Task LoadPhotosAsync()
        {
            try
            {
                var items = await _client.GetPhonePhotosAsync();

                if (items == null || items.Count == 0)
                {
                    MessageBox.Show("Brak zdjęć lub brak uprawnień w telefonie.");
                    return;
                }

                flowPanel.Controls.Clear();
                foreach (var item in items)
                {
                    var pic = new PictureBox
                    {
                        Width = 140,
                        Height = 140,
                        SizeMode = PictureBoxSizeMode.Zoom,
                        BorderStyle = BorderStyle.FixedSingle,
                        Tag = item.url,
                        Cursor = Cursors.Hand,
                        Margin = new Padding(5)
                    };

                    pic.Click += (s, e) =>
                    {
                        if (SelectedPaths.Contains(item.url))
                        {
                            SelectedPaths.Remove(item.url);
                            pic.BackColor = Color.Transparent;
                            pic.Padding = new Padding(0);
                        }
                        else
                        {
                            SelectedPaths.Add(item.url);
                            pic.BackColor = Color.DodgerBlue;
                            pic.Padding = new Padding(4);
                        }
                    };

                    flowPanel.Controls.Add(pic);

                    // Pobieranie miniatury w tle
                    _ = Task.Run(async () =>
                    {
                        byte[] data = await _client.DownloadPhotoAsync(item.url, true);
                        if (data != null)
                        {
                            this.Invoke((MethodInvoker)delegate
                            {
                                try { using (var ms = new MemoryStream(data)) pic.Image = Image.FromStream(ms); } catch { }
                            });
                        }
                    });
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Błąd galerii: " + ex.Message);
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