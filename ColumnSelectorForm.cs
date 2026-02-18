using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;

namespace Reklamacje_Dane
{
    public partial class ColumnSelectorForm : Form
    {
        private CheckedListBox _clbColumns;
        private Dictionary<string, string> _allColumns;
        private Dictionary<string, bool> _currentVisibility;
        public Dictionary<string, bool> Result { get; private set; }

        public ColumnSelectorForm(Dictionary<string, string> availableColumns, Dictionary<string, bool> currentVisibility)
        {
            _allColumns = availableColumns;
            _currentVisibility = currentVisibility;

            this.Text = "Wybierz widoczne kolumny";
            this.Size = new Size(400, 600);
            this.StartPosition = FormStartPosition.CenterParent;
            this.FormBorderStyle = FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.BackColor = Color.White;

            var txtSearch = new TextBox
            {
                Dock = DockStyle.Top,
                Font = new Font("Segoe UI", 10),
            };

            txtSearch.SetPlaceholder("Szukaj kolumny...");
            txtSearch.TextChanged += (s, e) => FilterColumns(txtSearch.Text);

            txtSearch.TextChanged += (s, e) => FilterColumns(txtSearch.Text);

            var pnlTop = new Panel { Dock = DockStyle.Top, Height = 30, BackColor = Color.White };
            var linkAll = new LinkLabel { Text = "Zaznacz wszystko", AutoSize = true, Top = 8, Left = 10 };
            linkAll.Click += (s, e) => SetAll(true);
            var linkNone = new LinkLabel { Text = "Odznacz wszystko", AutoSize = true, Top = 8, Left = 120 };
            linkNone.Click += (s, e) => SetAll(false);
            pnlTop.Controls.Add(linkAll);
            pnlTop.Controls.Add(linkNone);

            _clbColumns = new CheckedListBox
            {
                Dock = DockStyle.Fill,
                CheckOnClick = true,
                Padding = new Padding(10),
                BorderStyle = BorderStyle.None,
                Font = new Font("Segoe UI", 9.5f)
            };

            var pnlBottom = new Panel { Dock = DockStyle.Bottom, Height = 50, BackColor = Color.WhiteSmoke };
            var btnOk = new Button { Text = "Zapisz", DialogResult = DialogResult.OK, Location = new Point(280, 10), Height = 30, BackColor = Color.FromArgb(0, 120, 215), ForeColor = Color.White, FlatStyle = FlatStyle.Flat };
            var btnCancel = new Button { Text = "Anuluj", DialogResult = DialogResult.Cancel, Location = new Point(190, 10), Height = 30, FlatStyle = FlatStyle.Flat, BackColor = Color.White };
            pnlBottom.Controls.Add(btnOk);
            pnlBottom.Controls.Add(btnCancel);

            this.Controls.Add(_clbColumns);
            this.Controls.Add(pnlTop);
            this.Controls.Add(txtSearch);
            this.Controls.Add(pnlBottom);

            FilterColumns(""); // Inicjalne ładowanie

            btnOk.Click += (s, e) =>
            {
                Result = new Dictionary<string, bool>();
                // Musimy zaktualizować stan na podstawie tego co jest w liście, 
                // ale też pamiętać o ukrytych przez filtr elementach
                foreach (var kvp in _allColumns)
                {
                    // Domyślnie bierzemy stary stan
                    bool isVisible = _currentVisibility.ContainsKey(kvp.Key) && _currentVisibility[kvp.Key];

                    // Jeśli element jest widoczny w liście, bierzemy stan z listy
                    foreach (ColumnItem item in _clbColumns.Items)
                    {
                        if (item.Key == kvp.Key)
                        {
                            isVisible = _clbColumns.CheckedItems.Contains(item);
                            break;
                        }
                    }
                    Result[kvp.Key] = isVisible;
                }
            };
        }

        private void FilterColumns(string text)
        {
            _clbColumns.Items.Clear();
            var query = text.ToLower();

            foreach (var col in _allColumns)
            {
                if (col.Value.ToLower().Contains(query))
                {
                    bool isVisible = _currentVisibility.ContainsKey(col.Key) && _currentVisibility[col.Key];
                    _clbColumns.Items.Add(new ColumnItem { Key = col.Key, Display = col.Value }, isVisible);
                }
            }
        }

        private void SetAll(bool state)
        {
            for (int i = 0; i < _clbColumns.Items.Count; i++) _clbColumns.SetItemChecked(i, state);
        }

        private class ColumnItem
        {
            public string Key { get; set; }
            public string Display { get; set; }
            public override string ToString() => Display;
        }
    }
}