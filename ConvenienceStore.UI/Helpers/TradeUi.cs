using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data.Linq;
using System.Drawing;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using ConvenienceStore.Business.Models;
namespace ConvenienceStore.UI.Helpers
{
    internal static class TradeUi
    {
        public static bool IsDesigner(Control form)
        { return LicenseManager.UsageMode == LicenseUsageMode.Designtime || (form.Site != null && form.Site.DesignMode); }
        public static bool Run(IWin32Window owner, Action action)
        {
            try { action(); return true; }
            catch (TradeException ex) { MessageBox.Show(owner, ex.Message, "Kiểm tra dữ liệu / quyền", MessageBoxButtons.OK, MessageBoxIcon.Warning); }
            catch (ChangeConflictException) { MessageBox.Show(owner, "Dữ liệu vừa được thay đổi ở máy khác (ví dụ tồn kho, giá bán). Hãy tải lại rồi kiểm tra và thử lại.", "Dữ liệu đã thay đổi"); }
            catch (Exception ex)
            {
                System.Diagnostics.Trace.TraceError(ex.ToString());
                MessageBox.Show(owner, "Chưa thực hiện được thao tác. Kiểm tra kết nối SQL Server và đã chạy script Database\\ConvenienceStoreDB.sql. Nếu lỗi xảy ra khi lưu, giữ nguyên phiếu và thử lại để kiểm tra mã giao dịch; không tạo phiếu khác ngay.", "Không thực hiện được", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            return false;
        }
        public static void Grid(DataGridView grid, params string[] specs)
        {
            grid.AutoGenerateColumns = false; grid.ReadOnly = true;
            grid.AllowUserToAddRows = false; grid.AllowUserToDeleteRows = false;
            grid.SelectionMode = DataGridViewSelectionMode.FullRowSelect; grid.MultiSelect = false;
            grid.RowHeadersVisible = false; grid.BackgroundColor = Color.White; grid.BorderStyle = BorderStyle.None;
            grid.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
            grid.EnableHeadersVisualStyles = false;
            grid.ColumnHeadersDefaultCellStyle.BackColor = Color.FromArgb(28, 57, 73);
            grid.ColumnHeadersDefaultCellStyle.ForeColor = Color.White;
            grid.ColumnHeadersHeight = 36; grid.RowTemplate.Height = 30;
            grid.AlternatingRowsDefaultCellStyle.BackColor = Color.FromArgb(241, 247, 247);
            grid.Columns.Clear();
            grid.Columns.AddRange(specs.Select(spec => {
                var parts = spec.Split('|');
                var col = new DataGridViewTextBoxColumn { DataPropertyName = parts[0], Name = parts[0], HeaderText = parts[1], FillWeight = parts.Length > 2 ? float.Parse(parts[2], CultureInfo.InvariantCulture) : 100f };
                if (parts.Length > 3) col.DefaultCellStyle.Format = parts[3];
                return (DataGridViewColumn)col;
            }).ToArray());
        }
        public static void ExportCsv(IWin32Window owner, DataGridView grid, string filename)
        {
            using (var dialog = new SaveFileDialog { Filter = "CSV UTF-8 (*.csv)|*.csv", FileName = filename })
            {
                if (dialog.ShowDialog(owner) != DialogResult.OK) return;
                Func<object, string> field = x => {
                    string value = Convert.ToString(x, CultureInfo.InvariantCulture) ?? "";
                    
                    if (value.Length > 0 && "=+@".Contains(value[0])) value = "'" + value;
                    return "\"" + value.Replace("\"", "\"\"") + "\"";
                };
                var header = string.Join(",", grid.Columns.Cast<DataGridViewColumn>().Select(x => field(x.HeaderText)));
                var body = grid.Rows.Cast<DataGridViewRow>()
                    .Select(row => string.Join(",", row.Cells.Cast<DataGridViewCell>().Select(x => field(x.Value))));
                File.WriteAllLines(dialog.FileName, new[] { header }.Concat(body), new UTF8Encoding(true));
            }
        }
    }
}
