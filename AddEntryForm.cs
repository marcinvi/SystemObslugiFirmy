using System;
using System.Collections.Generic;
using System.Data;
using System.Windows.Forms;

namespace Reklamacje_Dane
{
    public partial class AddEntryForm : Form
    {
        public Dictionary<string, object> Values { get; private set; }

        public AddEntryForm(DataColumnCollection columns)
        {
            InitializeComponent();
            GenerateControls(columns);
        

            // Włącz sprawdzanie pisowni dla wszystkich TextBoxów
            EnableSpellCheckOnAllTextBoxes();
        }

        private void GenerateControls(DataColumnCollection columns)
        {
            Values = new Dictionary<string, object>();
            int y = 10;

            foreach (DataColumn column in columns)
            {
                Label lbl = new Label();
                lbl.Text = column.ColumnName;
                lbl.Location = new System.Drawing.Point(10, y);
                lbl.AutoSize = true;
                this.Controls.Add(lbl);

                TextBox txt = new TextBox();
                txt.Name = "txt" + column.ColumnName;
                txt.Location = new System.Drawing.Point(150, y - 3);
                txt.Width = 200;
                this.Controls.Add(txt);

                y += 30;
            }

            Button btnOK = new Button();
            btnOK.Text = "OK";
            btnOK.Location = new System.Drawing.Point(150, y);
            btnOK.Click += BtnOK_Click;
            this.Controls.Add(btnOK);

            this.Height = y + 70;
            this.Width = 400;
        }

        private void BtnOK_Click(object sender, EventArgs e)
        {
            CollectValuesAndClose();
        }

        private void CollectValuesAndClose()
        {
            foreach (Control control in this.Controls)
            {
                if (control is TextBox txt)
                {
                    string columnName = txt.Name.Substring(3); // Usuwa "txt" z nazwy
                    Values[columnName] = txt.Text;
                }
            }

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