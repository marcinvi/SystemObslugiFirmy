// Plik: DelegacjeControl.cs (Wersja z możliwością filtrowania po użytkowniku)
using System;
using System.Collections.Generic; // Poprawka błędu kompilacji CS0246
using System.Data;
using MySql.Data.MySqlClient;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Reklamacje_Dane
{
    public partial class DelegacjeControl : UserControl
    {
        private readonly DatabaseService _dbBaza;
        private readonly DatabaseService _dbMagazyn;
        private DataTable _handlowcy;
        private DataTable _delegacje;
        private readonly long? _filterByUserId; // Pole do przechowywania ID użytkownika do filtrowania

        // Konstruktor domyślny, który wywołuje nowy z wartością null
        public DelegacjeControl() : this(null)
        {
        }

        // Nowy konstruktor, który pozwala na filtrowanie delegacji
        public DelegacjeControl(long? userIdToFilter = null)
        {
            InitializeComponent();
            _dbBaza = new DatabaseService(DatabaseHelper.GetConnectionString());
            _dbMagazyn = new DatabaseService(MagazynDatabaseHelper.GetConnectionString());
            _filterByUserId = userIdToFilter; // Zapisujemy ID do filtrowania

            this.Load += async (_, __) => await LoadAllAsync();
            btnRefresh.Click += async (_, __) => await LoadAllAsync();
            btnAdd.Click += async (_, __) => await AddOrUpdateAsync();
            btnClear.Click += (_, __) => ClearEditor();
            dgv.CellClick += Dgv_CellClick;
            btnDelete.Click += async (_, __) => await DeleteSelectedAsync();
            chkAktywneOnly.CheckedChanged += async (_, __) => await LoadDelegacjeAsync();
        }

        private async Task LoadAllAsync()
        {
            await LoadHandlowcyAsync();
            await LoadDelegacjeAsync();
            BindUsersToCombos();
            ClearEditor();

            // Jeśli filtrujemy, automatycznie ustawiamy ComboBox na tego użytkownika i go blokujemy
            if (_filterByUserId.HasValue)
            {
                cmbUzytkownik.SelectedValue = _filterByUserId.Value;
                cmbUzytkownik.Enabled = false;
            }
        }

        private async Task LoadHandlowcyAsync()
        {
            _handlowcy = await _dbBaza.GetDataTableAsync(@"
                SELECT Id, `Nazwa Wyświetlana` AS Nazwa
                FROM Uzytkownicy
                WHERE IFNULL(Rola,'')='Handlowiec'
                ORDER BY Nazwa");
        }

        private async Task LoadDelegacjeAsync()
        {
            var parameters = new List<MySqlParameter>();
            string sql = @"SELECT d.Id, d.UzytkownikId, d.ZastepcaId, d.DataOd, d.DataDo, d.CzyAktywna
                           FROM Delegacje d";

            var conditions = new List<string>();
            if (chkAktywneOnly.Checked)
            {
                conditions.Add("IFNULL(d.CzyAktywna,1)=1");
            }
            // Nowy warunek filtrujący po ID użytkownika
            if (_filterByUserId.HasValue)
            {
                conditions.Add("d.UzytkownikId = @userId");
                parameters.Add(new MySqlParameter("@userId", _filterByUserId.Value));
            }

            if (conditions.Any())
            {
                sql += " WHERE " + string.Join(" AND ", conditions);
            }

            sql += " ORDER BY d.DataOd DESC";
            _delegacje = await _dbMagazyn.GetDataTableAsync(sql, parameters.ToArray());

            var view = new DataTable();
            view.Columns.Add("Id", typeof(long));
            view.Columns.Add("Użytkownik", typeof(string));
            view.Columns.Add("Zastępca", typeof(string));
            view.Columns.Add("Data od", typeof(DateTime));
            view.Columns.Add("Data do", typeof(DateTime));
            view.Columns.Add("Aktywna", typeof(bool));

            foreach (DataRow row in _delegacje.Rows)
            {
                long uzytkownikId = Convert.ToInt64(row["UzytkownikId"]);
                long zastepcaId = Convert.ToInt64(row["ZastepcaId"]);

                string uzytkownikNazwa = _handlowcy.AsEnumerable()
                    .FirstOrDefault(h => h.Field<long>("Id") == uzytkownikId)?.Field<string>("Nazwa") ?? $"[ID: {uzytkownikId}]";

                string zastepcaNazwa = _handlowcy.AsEnumerable()
                    .FirstOrDefault(h => h.Field<long>("Id") == zastepcaId)?.Field<string>("Nazwa") ?? $"[ID: {zastepcaId}]";

                view.Rows.Add(
                    Convert.ToInt64(row["Id"]),
                    uzytkownikNazwa,
                    zastepcaNazwa,
                    Convert.ToDateTime(row["DataOd"]),
                    Convert.ToDateTime(row["DataDo"]),
                    Convert.ToInt32(row["CzyAktywna"]) != 0
                );
            }

            dgv.DataSource = view;
            FormatGrid();
        }

        private void BindUsersToCombos()
        {
            cmbUzytkownik.DisplayMember = "Nazwa";
            cmbUzytkownik.ValueMember = "Id";
            cmbUzytkownik.DataSource = _handlowcy.Copy();

            cmbZastepca.DisplayMember = "Nazwa";
            cmbZastepca.ValueMember = "Id";
            cmbZastepca.DataSource = _handlowcy.Copy();
        }

        private void FormatGrid()
        {
            dgv.RowHeadersVisible = false;
            dgv.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
            dgv.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            dgv.AllowUserToAddRows = false;
            dgv.AllowUserToDeleteRows = false;
            if (dgv.Columns["Id"] != null)
            {
                dgv.Columns["Id"].HeaderText = "ID";
                dgv.Columns["Id"].Width = 60;
            }
        }

        private void ClearEditor()
        {
            txtId.Text = "";
            if (!_filterByUserId.HasValue)
            {
                if (cmbUzytkownik.Items.Count > 0) cmbUzytkownik.SelectedIndex = -1;
            }
            if (cmbZastepca.Items.Count > 0) cmbZastepca.SelectedIndex = -1;
            dtpOd.Value = DateTime.Today;
            dtpDo.Value = DateTime.Today.AddDays(7);
            chkAktywna.Checked = true;
            dgv.ClearSelection();
        }

        private void Dgv_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex < 0) return;

            long id = Convert.ToInt64(dgv.Rows[e.RowIndex].Cells["Id"].Value);
            DataRow originalRow = _delegacje.AsEnumerable().FirstOrDefault(r => r.Field<long>("Id") == id);
            if (originalRow == null) return;

            txtId.Text = originalRow["Id"].ToString();
            cmbUzytkownik.SelectedValue = Convert.ToInt64(originalRow["UzytkownikId"]);
            cmbZastepca.SelectedValue = Convert.ToInt64(originalRow["ZastepcaId"]);
            dtpOd.Value = Convert.ToDateTime(originalRow["DataOd"]);
            dtpDo.Value = Convert.ToDateTime(originalRow["DataDo"]);
            chkAktywna.Checked = Convert.ToInt32(originalRow["CzyAktywna"]) != 0;
        }

        private bool ValidateEditor(out string error)
        {
            error = "";
            if (cmbUzytkownik.SelectedValue == null || cmbZastepca.SelectedValue == null)
            {
                error = "Wybierz użytkownika i zastępcę.";
                return false;
            }
            long u = Convert.ToInt64(cmbUzytkownik.SelectedValue);
            long z = Convert.ToInt64(cmbZastepca.SelectedValue);
            if (u == z)
            {
                error = "Nie można delegować na samego siebie.";
                return false;
            }
            if (dtpOd.Value.Date > dtpDo.Value.Date)
            {
                error = "Data Od nie może być późniejsza niż Data Do.";
                return false;
            }

            var overlapping = _delegacje.AsEnumerable().Any(r =>
            {
                if (!string.IsNullOrEmpty(txtId.Text) && Convert.ToInt64(r["Id"]) == Convert.ToInt64(txtId.Text)) return false;
                if (Convert.ToInt64(r["UzytkownikId"]) != u) return false;
                if (Convert.ToInt32(r["CzyAktywna"]) == 0) return false;
                var od = Convert.ToDateTime(r["DataOd"]).Date;
                var dd = Convert.ToDateTime(r["DataDo"]).Date;
                return dtpOd.Value.Date <= dd && dtpDo.Value.Date >= od;
            });
            if (overlapping)
            {
                error = "Wybrany użytkownik ma już aktywną delegację w tym okresie.";
                return false;
            }
            return true;
        }

        private async Task AddOrUpdateAsync()
        {
            if (!ValidateEditor(out string err))
            {
                MessageBox.Show(err, "Błąd walidacji", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            long u = Convert.ToInt64(cmbUzytkownik.SelectedValue);
            long z = Convert.ToInt64(cmbZastepca.SelectedValue);
            var p = new[]
            {
                new MySqlParameter("@u", u),
                new MySqlParameter("@z", z),
                new MySqlParameter("@od", dtpOd.Value.Date),
                new MySqlParameter("@do", dtpDo.Value.Date),
                new MySqlParameter("@a", chkAktywna.Checked ? 1 : 0)
            };

            if (string.IsNullOrWhiteSpace(txtId.Text))
            {
                await _dbMagazyn.ExecuteNonQueryAsync(@"
                    INSERT INTO Delegacje (UzytkownikId, ZastepcaId, DataOd, DataDo, CzyAktywna)
                    VALUES (@u, @z, @od, @do, @a)", p);
            }
            else
            {
                var pp = p.Concat(new[] { new MySqlParameter("@id", Convert.ToInt64(txtId.Text)) }).ToArray();
                await _dbMagazyn.ExecuteNonQueryAsync(@"
                    UPDATE Delegacje SET UzytkownikId=@u, ZastepcaId=@z, DataOd=@od, DataDo=@do, CzyAktywna=@a
                    WHERE Id=@id", pp);
            }

            await LoadDelegacjeAsync();
            ClearEditor();
        }

        private async Task DeleteSelectedAsync()
        {
            if (dgv.SelectedRows.Count == 0) return;
            var id = Convert.ToInt64(dgv.SelectedRows[0].Cells["Id"].Value);
            if (MessageBox.Show("Czy na pewno chcesz usunąć wybraną delegację?", "Potwierdzenie usunięcia", MessageBoxButtons.YesNo, MessageBoxIcon.Question) != DialogResult.Yes) return;

            await _dbMagazyn.ExecuteNonQueryAsync("DELETE FROM Delegacje WHERE Id=@id",
                new MySqlParameter("@id", id));
            await LoadDelegacjeAsync();
            ClearEditor();
        }
    }
}