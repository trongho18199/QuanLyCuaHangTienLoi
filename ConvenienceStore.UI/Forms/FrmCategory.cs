using System;
using System.Linq;
using System.Windows.Forms;
using ConvenienceStore.Business.Models;
using ConvenienceStore.Business.Services;
using ConvenienceStore.UI.Helpers;
namespace ConvenienceStore.UI.Forms
{
    public partial class FrmCategory : DevExpress.XtraEditors.XtraForm
    {
        private readonly int actorId; private readonly CatalogService service; private bool loading;
        public FrmCategory() : this(0) { }
        public FrmCategory(int currentUserId) : this(currentUserId, new CatalogService()) { }
        public FrmCategory(int currentUserId, CatalogService catalogService)
        {
            actorId = currentUserId; service = catalogService; InitializeComponent();
            btnSearch.Click += (s, e) => CatalogUi.Run(this, () => LoadData(0));
            CatalogUi.LiveSearch(this, txtSearch, () => LoadData(0));
            txtSearch.KeyDown += (s, e) => { if (e.KeyCode == Keys.Enter) { e.SuppressKeyPress = true; CatalogUi.Run(this, () => LoadData(0)); } };
            dgvItems.SelectionChanged += (s, e) => { if (!loading) ShowSelected(); };
            btnAdd.Click += (s, e) => CatalogUi.Run(this, () => {
                int id = service.SaveCategory(actorId, new CategoryInput { Code = txtCode.Text, Name = txtName.Text });
                LoadData(id); MessageBox.Show(this, "Đã thêm danh mục.", "Thành công");
            });
            btnUpdate.Click += (s, e) => {
                var row = Selected();
                if (row == null) { MessageBox.Show(this, "Hãy chọn danh mục cần cập nhật trong danh sách.", "Chưa chọn dòng"); return; }
                CatalogUi.Run(this, () => {
                    service.SaveCategory(actorId, new CategoryInput { Id = row.Id, Code = txtCode.Text, Name = txtName.Text });
                    LoadData(row.Id); MessageBox.Show(this, "Đã cập nhật danh mục.", "Thành công");
                });
            };
            btnDelete.Click += (s, e) => {
                var row = Selected();
                if (row == null) { MessageBox.Show(this, "Hãy chọn danh mục cần xóa trong danh sách.", "Chưa chọn dòng"); return; }
                if (MessageBox.Show(this, "Xóa danh mục \"" + row.Name + "\"?", "Xác nhận xóa", MessageBoxButtons.YesNo, MessageBoxIcon.Question) != DialogResult.Yes) return;
                CatalogUi.Run(this, () => { service.DeleteCategory(actorId, row.Id); ClearInputs(); LoadData(0); });
            };
            btnClear.Click += (s, e) => { ClearInputs(); dgvItems.ClearSelection(); txtCode.Focus(); };
        }
        private void Form_Load(object sender, EventArgs e)
        {
            if (CatalogUi.IsDesigner(this)) return;
            CatalogUi.Grid(dgvItems, "Code|Mã danh mục|130", "Name|Tên danh mục|250", "ProductCount|Số sản phẩm|100|N0");
            CatalogUi.Run(this, () => LoadData(0));
        }
        private CategoryRow Selected() { return dgvItems.CurrentRow == null ? null : dgvItems.CurrentRow.DataBoundItem as CategoryRow; }
        private void ClearInputs() { txtCode.Clear(); txtName.Clear(); }
        private void ShowSelected() { var row = Selected(); if (row != null) { txtCode.Text = row.Code; txtName.Text = row.Name; } }
        private void LoadData(int selectId)
        {
            loading = true;
            try
            {
                dgvItems.DataSource = service.Categories(actorId, txtSearch.Text);
                dgvItems.ClearSelection(); dgvItems.CurrentCell = null;
                if (selectId != 0)
                    foreach (DataGridViewRow row in dgvItems.Rows)
                        if (((CategoryRow)row.DataBoundItem).Id == selectId) { row.Selected = true; dgvItems.CurrentCell = row.Cells[0]; break; }
            }
            finally { loading = false; }
            ShowSelected();
        }
    }
}
