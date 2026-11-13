using System;
using System.Linq;
using System.Windows.Forms;
using ConvenienceStore.Business.Models;
using ConvenienceStore.Business.Services;
using ConvenienceStore.UI.Helpers;
namespace ConvenienceStore.UI.Forms
{
    public partial class FrmSupplier : DevExpress.XtraEditors.XtraForm
    {
        private readonly int actorId; private readonly CatalogService service; private bool loading;
        public FrmSupplier() : this(0) { }
        public FrmSupplier(int currentUserId) : this(currentUserId, new CatalogService()) { }
        public FrmSupplier(int currentUserId, CatalogService catalogService)
        {
            actorId = currentUserId; service = catalogService; InitializeComponent();
            btnSearch.Click += (s, e) => CatalogUi.Run(this, () => LoadData(0));
            CatalogUi.LiveSearch(this, txtSearch, () => LoadData(0));
            txtSearch.KeyDown += (s, e) => { if (e.KeyCode == Keys.Enter) { e.SuppressKeyPress = true; CatalogUi.Run(this, () => LoadData(0)); } };
            dgvItems.SelectionChanged += (s, e) => { if (!loading) ShowSelected(); };
            btnAdd.Click += (s, e) => CatalogUi.Run(this, () => {
                int id = service.SaveSupplier(actorId, ReadInput(0));
                LoadData(id); MessageBox.Show(this, "Đã thêm nhà cung cấp.", "Thành công");
            });
            btnUpdate.Click += (s, e) => {
                var row = Selected();
                if (row == null) { MessageBox.Show(this, "Hãy chọn nhà cung cấp cần cập nhật trong danh sách.", "Chưa chọn dòng"); return; }
                CatalogUi.Run(this, () => {
                    service.SaveSupplier(actorId, ReadInput(row.Id));
                    LoadData(row.Id); MessageBox.Show(this, "Đã cập nhật nhà cung cấp.", "Thành công");
                });
            };
            btnDelete.Click += (s, e) => {
                var row = Selected();
                if (row == null) { MessageBox.Show(this, "Hãy chọn nhà cung cấp cần xóa trong danh sách.", "Chưa chọn dòng"); return; }
                if (MessageBox.Show(this, "Xóa nhà cung cấp \"" + row.Name + "\"?", "Xác nhận xóa", MessageBoxButtons.YesNo, MessageBoxIcon.Question) != DialogResult.Yes) return;
                CatalogUi.Run(this, () => {
                    var result = service.DeleteSupplier(actorId, row.Id);
                    if (!result.Deactivated) ClearInputs();
                    LoadData(result.Deactivated ? row.Id : 0);
                    MessageBox.Show(this, result.Message, "Kết quả");
                });
            };
            btnClear.Click += (s, e) => { ClearInputs(); dgvItems.ClearSelection(); txtName.Focus(); };
        }
        private void Form_Load(object sender, EventArgs e)
        {
            if (CatalogUi.IsDesigner(this)) return;
            CatalogUi.Grid(dgvItems, "Id|Mã|50", "Name|Tên nhà cung cấp|220", "Phone|Điện thoại|110", "Status|Trạng thái|110");
            CatalogUi.Run(this, () => LoadData(0));
        }
        private SupplierRow Selected() { return dgvItems.CurrentRow == null ? null : dgvItems.CurrentRow.DataBoundItem as SupplierRow; }
        private SupplierInput ReadInput(int id) { return new SupplierInput { Id = id, Name = txtName.Text, Phone = txtPhone.Text, IsActive = chkActive.Checked }; }
        private void ClearInputs() { txtName.Clear(); txtPhone.Clear(); chkActive.Checked = true; }
        private void ShowSelected()
        {
            var row = Selected(); if (row == null) return;
            txtName.Text = row.Name; txtPhone.Text = row.Phone; chkActive.Checked = row.IsActive;
        }
        private void LoadData(int selectId)
        {
            loading = true;
            try
            {
                dgvItems.DataSource = service.Suppliers(actorId, txtSearch.Text);
                dgvItems.ClearSelection(); dgvItems.CurrentCell = null;
                if (selectId != 0)
                    foreach (DataGridViewRow row in dgvItems.Rows)
                        if (((SupplierRow)row.DataBoundItem).Id == selectId) { row.Selected = true; dgvItems.CurrentCell = row.Cells[0]; break; }
            }
            finally { loading = false; }
            ShowSelected();
        }
    }
}
