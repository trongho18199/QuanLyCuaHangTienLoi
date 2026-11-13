using System;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using ConvenienceStore.Business.Models;
using ConvenienceStore.Business.Services;
using ConvenienceStore.UI.Helpers;
namespace ConvenienceStore.UI.Forms
{
    public partial class FrmProduct : DevExpress.XtraEditors.XtraForm
    {
        private readonly int actorId; private readonly CatalogService service; private bool loading;
        public FrmProduct() : this(0) { }
        public FrmProduct(int currentUserId) : this(currentUserId, new CatalogService()) { }
        public FrmProduct(int currentUserId, CatalogService catalogService)
        {
            actorId = currentUserId; service = catalogService; InitializeComponent();
            btnSearch.Click += (s, e) => CatalogUi.Run(this, () => LoadData(0));
            CatalogUi.LiveSearch(this, txtSearch, () => LoadData(0));
            txtSearch.KeyDown += (s, e) => { if (e.KeyCode == Keys.Enter) { e.SuppressKeyPress = true; CatalogUi.Run(this, () => LoadData(0)); } };
            cboFilter.SelectionChangeCommitted += (s, e) => CatalogUi.Run(this, () => LoadData(0));
            dgvItems.SelectionChanged += (s, e) => { if (!loading) ShowSelected(); };
            
            dgvItems.CellFormatting += (s, e) => {
                if (e.RowIndex < 0 || e.RowIndex >= dgvItems.Rows.Count || e.ColumnIndex < 0 || dgvItems.Columns[e.ColumnIndex].DataPropertyName != "Stock") return;
                var row = dgvItems.Rows[e.RowIndex].DataBoundItem as ProductRow;
                if (row == null || row.Stock > StockPolicy.LowStockThreshold) return;
                e.CellStyle.ForeColor = Color.Firebrick;
                e.CellStyle.Font = new Font(dgvItems.Font, FontStyle.Bold);
            };
            btnAdd.Click += (s, e) => CatalogUi.Run(this, () => {
                int id = service.SaveProduct(actorId, ReadInput(0));
                LoadData(id); MessageBox.Show(this, "Đã thêm sản phẩm. Tồn kho ban đầu là 0; hãy lập phiếu nhập để tăng tồn.", "Thành công");
            });
            btnUpdate.Click += (s, e) => {
                var row = Selected();
                if (row == null) { MessageBox.Show(this, "Hãy chọn sản phẩm cần cập nhật trong danh sách.", "Chưa chọn dòng"); return; }
                CatalogUi.Run(this, () => {
                    service.SaveProduct(actorId, ReadInput(row.Id));
                    LoadData(row.Id); MessageBox.Show(this, "Đã cập nhật sản phẩm.", "Thành công");
                });
            };
            btnDelete.Click += (s, e) => {
                var row = Selected();
                if (row == null) { MessageBox.Show(this, "Hãy chọn sản phẩm cần xóa trong danh sách.", "Chưa chọn dòng"); return; }
                if (MessageBox.Show(this, "Xóa sản phẩm \"" + row.Name + "\"?", "Xác nhận xóa", MessageBoxButtons.YesNo, MessageBoxIcon.Question) != DialogResult.Yes) return;
                CatalogUi.Run(this, () => { service.DeleteProduct(actorId, row.Id); ClearInputs(); LoadData(0); });
            };
            btnClear.Click += (s, e) => CatalogUi.Run(this, () => { LoadData(0); ClearInputs(); txtBarcode.Focus(); });
        }
        private void Form_Load(object sender, EventArgs e)
        {
            if (CatalogUi.IsDesigner(this)) return;
            CatalogUi.Grid(dgvItems, "Barcode|Mã SP|110", "Name|Tên sản phẩm|200", "CategoryName|Danh mục|120", "Price|Giá bán|90|N0", "Stock|Tồn|60|N0");
            CatalogUi.Run(this, () => { LoadCategories(); LoadData(0); });
        }
        private void LoadCategories()
        {
            var options = service.CategoryOptions(actorId);
            var filter = options.Select(x => new CategoryOption { Id = x.Id, Name = x.Name }).ToList();
            filter.Insert(0, new CategoryOption { Id = 0, Name = "Tất cả danh mục" });
            cboFilter.DataSource = filter; cboFilter.DisplayMember = "Name"; cboFilter.ValueMember = "Id";
            cboCategory.DataSource = options; cboCategory.DisplayMember = "Name"; cboCategory.ValueMember = "Id";
            if (options.Count == 0) cboCategory.SelectedIndex = -1;
        }
        private ProductRow Selected() { return dgvItems.CurrentRow == null ? null : dgvItems.CurrentRow.DataBoundItem as ProductRow; }
        private ProductInput ReadInput(int id)
        {
            return new ProductInput { Id = id, Barcode = txtBarcode.Text, Name = txtName.Text,
                CategoryId = cboCategory.SelectedValue is int ? (int)cboCategory.SelectedValue : 0, Price = nudPrice.Value };
        }
        private void ClearInputs()
        {
            txtBarcode.Clear(); txtName.Clear(); nudPrice.Value = 0;
            cboCategory.SelectedIndex = cboCategory.Items.Count > 0 ? 0 : -1;
        }
        private void ShowSelected()
        {
            var row = Selected(); if (row == null) return;
            txtBarcode.Text = row.Barcode; txtName.Text = row.Name;
            cboCategory.SelectedValue = row.CategoryId;
            nudPrice.Value = Math.Min(row.Price, nudPrice.Maximum);
        }
        private void LoadData(int selectId)
        {
            loading = true;
            try
            {
                int? categoryId = cboFilter.SelectedValue is int && (int)cboFilter.SelectedValue > 0 ? (int?)(int)cboFilter.SelectedValue : null;
                dgvItems.DataSource = service.Products(actorId, txtSearch.Text, categoryId);
                dgvItems.ClearSelection(); dgvItems.CurrentCell = null;
                if (selectId != 0)
                    foreach (DataGridViewRow row in dgvItems.Rows)
                        if (((ProductRow)row.DataBoundItem).Id == selectId) { row.Selected = true; dgvItems.CurrentCell = row.Cells[0]; break; }
            }
            finally { loading = false; }
            ShowSelected();
        }
    }
}
