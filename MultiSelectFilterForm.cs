using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;

namespace Reklamacje_Dane
{
    public partial class MultiSelectFilterForm : Form
    {
        private CheckedListBox _clbValues;
        public List<string> SelectedValues { get; private set; }

        public MultiSelectFilterForm(List<string> allValues, List<string> currentSelection)
        {
            this.Text = "Filtruj wartości";
            this.Size = new Size(320, 450);
            this.StartPosition = FormStartPosition.CenterParent;
            this.FormBorderStyle = FormBorderStyle.SizableToolWindow;
            this.BackColor = Color.White;

            var txtSearch = new TextBox
            {
                Dock = DockStyle.Top,
                Font = new Font("Segoe UI", 10),
            };

            txtSearch.SetPlaceholder("Szukaj na liście...");
            txtSearch.TextChanged += (s, e) => FilterList(allValues, txtSearch.Text);

            _clbValues = new CheckedListBox
            {
                Dock = DockStyle.Fill,
                CheckOnClick = true,
                BorderStyle = BorderStyle.None,
                Font = new Font("Segoe UI", 9.5f),
                Padding = new Padding(5)
            };

            var pnlBottom = new Panel { Dock = DockStyle.Bottom, Height = 50, BackColor = Color.WhiteSmoke };
            var btnOk = CreateButton("Zastosuj", Color.FromArgb(0, 120, 215), Color.White);
            var btnClear = CreateButton("Wyczyść", Color.White, Color.Black);

            btnOk.Location = new Point(210, 10);
            btnClear.Location = new Point(10, 10);

            btnClear.Click += (s, e) => { for (int i = 0; i < _clbValues.Items.Count; i++) _clbValues.SetItemChecked(i, false); };
            btnOk.Click += (s, e) => { SelectedValues = _clbValues.CheckedItems.Cast<string>().ToList(); this.DialogResult = DialogResult.OK; };

            pnlBottom.Controls.Add(btnClear);
            pnlBottom.Controls.Add(btnOk);

            this.Controls.Add(_clbValues);
            this.Controls.Add(txtSearch);
            this.Controls.Add(pnlBottom);

            foreach (var val in allValues.OrderBy(x => x)) _clbValues.Items.Add(val, currentSelection.Contains(val));
        }

        private Button CreateButton(string text, Color bg, Color fg)
        {
            return new Button
            {
                Text = text,
                Size = new Size(90, 30),
                FlatStyle = FlatStyle.Flat,
                BackColor = bg,
                ForeColor = fg,
                Font = new Font("Segoe UI", 9)
            };
        }

        private void FilterList(List<string> allData, string filter)
        {
            _clbValues.Items.Clear();
            var visible = string.IsNullOrWhiteSpace(filter) ? allData : allData.Where(x => x.ToLower().Contains(filter.ToLower())).ToList();
            foreach (var val in visible) _clbValues.Items.Add(val);
        }
    }
}