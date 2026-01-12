using System;
using System.Collections.Generic;
using System.Data;
using MySql.Data.MySqlClient;
using System.Drawing;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Reklamacje_Dane
{
    public partial class AllegroOpiekunowieControl : UserControl
    {
        private readonly DatabaseService _dbBaza;     // Baza.db
        private readonly DatabaseService _dbMagazyn;  // Magazyn.db

        private DataTable _accounts;      // AllegroAccounts z Baza.db
        private DataTable _handlowcy;     // Uzytkownicy (Rola=Handlowiec) z Baza.db
        private DataTable _mapowania;     // AllegroAccountOpiekun z Magazyn.db

        public AllegroOpiekunowieControl()
        {
            InitializeComponent();
            _dbBaza = new DatabaseService(DatabaseHelper.GetConnectionString());
            _dbMagazyn = new DatabaseService(MagazynDatabaseHelper.GetConnectionString());

            this.Load += async (_, __) => await LoadAllAsync();
            btnRefresh.Click += async (_, __) => await LoadAllAsync();
            btnSaveAll.Click += async (_, __) => await SaveAllAsync();
        }

        private async Task LoadAllAsync()
        {
            try
            {
                // konta z Baza.db
                _accounts = await _dbBaza.GetDataTableAsync(@"
                    SELECT Id, AccountName
                    FROM AllegroAccounts
                    ORDER BY AccountName");

                // handlowcy z Baza.db
                _handlowcy = await _dbBaza.GetDataTableAsync(@"
                    SELECT Id, `Nazwa Wyświetlana` AS Nazwa
                    FROM Uzytkownicy
                    WHERE IFNULL(Rola,'')='Handlowiec'
                    ORDER BY Nazwa");

                // mapowania z Magazyn.db
                _mapowania = await _dbMagazyn.GetDataTableAsync(@"
                    SELECT AllegroAccountId, OpiekunId
                    FROM AllegroAccountOpiekun");

                // zbuduj widok do grida
                var view = new DataTable();
                view.Columns.Add("Id", typeof(long));
                view.Columns.Add("Konto Allegro", typeof(string));
                view.Columns.Add("OpiekunId", typeof(long));
                view.Columns.Add("Opiekun (Handlowiec)", typeof(string));

                foreach (DataRow r in _accounts.Rows)
                {
                    long accId = Convert.ToInt64(r["Id"]);
                    string name = Convert.ToString(r["AccountName"]);
                    long? opId = null;

                    var m = _mapowania.AsEnumerable().FirstOrDefault(x => Convert.ToInt64(x["AllegroAccountId"]) == accId);
                    if (m != null) opId = Convert.ToInt64(m["OpiekunId"]);
                    // (opcjonalnie) fallback z Baza.db (kolumna OpiekunId w AllegroAccounts)
                    // jeśli chcesz użyć tego fallbacku, odkomentuj:
                    // if (opId == null)
                    // {
                    //     var a = _dbBaza.GetDataTableAsync("SELECT OpiekunId FROM AllegroAccounts WHERE Id=@id",
                    //         new MySqlParameter("@id", accId)).Result;
                    //     if (a.Rows.Count>0 && a.Rows[0]["OpiekunId"] != DBNull.Value) opId = Convert.ToInt64(a.Rows[0]["OpiekunId"]);
                    // }

                    string opName = opId.HasValue
                        ? _handlowcy.AsEnumerable().FirstOrDefault(h => Convert.ToInt64(h["Id"]) == opId.Value)?.
                          Field<string>("Nazwa") ?? ""
                        : "";

                    view.Rows.Add(accId, name, opId ?? -1, opName);
                }

                dgv.DataSource = view;
                ConfigureGrid();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Błąd ładowania opiekunów: " + ex.Message, "Błąd", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void ConfigureGrid()
        {
            dgv.RowHeadersVisible = false;
            dgv.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
            dgv.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            dgv.AllowUserToAddRows = false;
            dgv.AllowUserToDeleteRows = false;

            if (dgv.Columns["Id"] != null) dgv.Columns["Id"].Visible = false;
            if (dgv.Columns["Opiekun (Handlowiec)"] != null) dgv.Columns["Opiekun (Handlowiec)"].Visible = false;

            // kolumna combobox z listą handlowców
            if (dgv.Columns["HandlowiecCol"] == null)
            {
                var cb = new DataGridViewComboBoxColumn
                {
                    Name = "HandlowiecCol",
                    HeaderText = "Opiekun (Handlowiec)",
                    DataPropertyName = "OpiekunId",
                    DisplayStyle = DataGridViewComboBoxDisplayStyle.DropDownButton,
                    FlatStyle = FlatStyle.Flat
                };
                // źródło danych dla combobox
                var src = new List<KeyValuePair<long, string>>();
                src.Add(new KeyValuePair<long, string>(-1, "— brak —"));
                foreach (DataRow h in _handlowcy.Rows)
                    src.Add(new KeyValuePair<long, string>(Convert.ToInt64(h["Id"]), Convert.ToString(h["Nazwa"])));
                cb.DataSource = src;
                cb.DisplayMember = "Value";
                cb.ValueMember = "Key";
                dgv.Columns.Add(cb);
            }

            if (dgv.Columns["OpiekunId"] != null) dgv.Columns["OpiekunId"].Visible = false;
        }

        private async Task SaveAllAsync()
        {
            try
            {
                if (dgv.DataSource is DataTable dt)
                {
                    foreach (DataRow r in dt.Rows)
                    {
                        long accId = Convert.ToInt64(r["Id"]);
                        long opId = Convert.ToInt64(r["OpiekunId"]); // -1 = brak

                        // upsert do Magazyn.db
                        if (opId <= 0)
                        {
                            // usunięcie mapowania
                            await _dbMagazyn.ExecuteNonQueryAsync(
                                "DELETE FROM AllegroAccountOpiekun WHERE AllegroAccountId=@id",
                                new MySqlParameter("@id", accId));
                        }
                        else
                        {
                            // insert or replace
                            await _dbMagazyn.ExecuteNonQueryAsync(@"
                            INSERT INTO AllegroAccountOpiekun (AllegroAccountId, OpiekunId)
                            VALUES (@aid, @oid)
                            ON CONFLICT(AllegroAccountId) DO UPDATE SET OpiekunId = excluded.OpiekunId",
                                new MySqlParameter("@aid", accId),
                                new MySqlParameter("@oid", opId));
                        }
                    }

                    MessageBox.Show("Zapisano mapowanie opiekunów.", "OK", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    await LoadAllAsync();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Błąd zapisu: " + ex.Message, "Błąd", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
    }
}
