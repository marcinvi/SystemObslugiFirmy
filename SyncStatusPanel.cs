using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Reklamacje_Dane
{
    public class SyncStatusPanel : UserControl
    {
        private ListView _list;
        private ColumnHeader _colSrc, _colWhen, _colResult, _colInfo;

        public SyncStatusPanel()
        {
            this.Dock = DockStyle.Top;
            this.Height = 120;
            this.Padding = new Padding(8);

            _list = new ListView
            {
                View = View.Details,
                Dock = DockStyle.Fill,
                FullRowSelect = true
            };
            _colSrc = new ColumnHeader { Text = "Źródło", Width = 90 };
            _colWhen = new ColumnHeader { Text = "Czas", Width = 150 };
            _colResult = new ColumnHeader { Text = "Wynik", Width = 80 };
            _colInfo = new ColumnHeader { Text = "Info", Width = 500 };
            _list.Columns.AddRange(new[] { _colSrc, _colWhen, _colResult, _colInfo });
            this.Controls.Add(_list);
        }

        public async Task RefreshAsync()
        {
            var sources = new[] { "ALLEGRO", "DPD", "GOOGLE" };
            var runsAll = new List<SyncRun>();
            foreach (var s in sources)
            {
                var part = await SyncRunLogger.LatestAsync(s, 3);
                runsAll.AddRange(part);
            }
            var items = runsAll
                .OrderByDescending(r => r.StartedAt)
                .Take(10)
                .Select(r =>
                {
                    var when = r.FinishedAt ?? r.StartedAt;
                    var res = r.Ok ? "OK" : (r.FinishedAt == null ? "W toku" : "BŁĄD");
                    var info = r.Ok ? (r.Details ?? $"fetched:{r.RowsFetched} / written:{r.RowsWritten}") : (r.ErrorMessage ?? "");
                    return new ListViewItem(new[] { r.Source, when.ToString("yyyy-MM-dd HH:mm"), res, info });
                })
                .ToArray();

            _list.BeginUpdate();
            _list.Items.Clear();
            _list.Items.AddRange(items);
            _list.EndUpdate();
        }
    }
}
