using System;
using System.Linq;
using System.Windows.Forms;
using ConvenienceStore.Business.Models;
using ConvenienceStore.Business.Services;
using ConvenienceStore.UI.Helpers;
namespace ConvenienceStore.UI.Forms
{
    public partial class FrmCustomer : DevExpress.XtraEditors.XtraForm
    {
        private readonly int actorId; private readonly CatalogService service; private bool loading;
        public FrmCustomer() : this(0) { }
        public FrmCustomer(int currentUserId) : this(currentUserId, new CatalogService()) { }
        public FrmCustomer(int currentUserId, CatalogService catalogService)
        {
            actorId = currentUserId; service = catalogService; InitializeComponent();
            cboTier.Items.AddRange(new object[] { "Thường", "VIP (giảm 10%)" }); cboTier.SelectedIndex = 0;
            btnSearch.Click += (s, e) => CatalogUi.Run(this, () => LoadData(0));
            CatalogUi.LiveSearch(this, txtSearch, () => LoadData(0));
            txtSearch.KeyDown += (s, e) => { if (e.KeyCode == Keys.Enter) { e.SuppressKeyPress = true; CatalogUi.Run(this, () => LoadData(0)); } };
            dgvItems.SelectionChanged += (s, e) => { if (!loading) ShowSelected(); };
            btnAdd.Click += (s, e) => CatalogUi.Run(this, () => {
                int id = service.SaveCustomer(actorId, ReadInput(0));
                LoadData(id); MessageBox.Show(this, "Đã thêm khách hàng.", "Thành công");
            });
            btnUpdate.Click += (s, e) => {
                var row = Selected();
                if (row == null) { MessageBox.Show(this, "Hãy chọn khách hàng cần cập nhật trong danh sách.", "Chưa chọn dòng"); return; }
                CatalogUi.Run(this, () => {
                    service.SaveCustomer(actorId, ReadInput(row.Id));
                    LoadData(row.Id); MessageBox.Show(this, "Đã cập nhật khách hàng.", "Thành công");
                });
            };
            btnDelete.Click += (s, e) => {
                var row = Selected();
                if (row == null) { MessageBox.Show(this, "Hãy chọn khách hàng cần xóa trong danh sách.", "Chưa chọn dòng"); return; }
                if (MessageBox.Show(this, "Xóa khách hàng \"" + row.Name + "\"?", "Xác nhận xóa", MessageBoxButtons.YesNo, MessageBoxIcon.Question) != DialogResult.Yes) return;
                CatalogUi.Run(this, () => {
                    var result = service.DeleteCustomer(actorId, row.Id);
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
            CatalogUi.Grid(dgvItems, "Id|Mã|50", "Name|Tên khách hàng|200", "Phone|Điện thoại|110", "Tier|Hạng thành viên|130", "Status|Trạng thái|90");
            CatalogUi.Run(this, () => LoadData(0));
        }
        private CustomerRow Selected() { return dgvItems.CurrentRow == null ? null : dgvItems.CurrentRow.DataBoundItem as CustomerRow; }
        private CustomerInput ReadInput(int id)
        { return new CustomerInput { Id = id, Name = txtName.Text, Phone = txtPhone.Text, IsVip = cboTier.SelectedIndex == 1, IsActive = chkActive.Checked }; }
        private void ClearInputs() { txtName.Clear(); txtPhone.Clear(); cboTier.SelectedIndex = 0; chkActive.Checked = true; }
        private void ShowSelected()
        {
            var row = Selected(); if (row == null) return;
            txtName.Text = row.Name; txtPhone.Text = row.Phone; cboTier.SelectedIndex = row.IsVip ? 1 : 0; chkActive.Checked = row.IsActive;
        }
        private void LoadData(int selectId)
        {
            loading = true;
            try
            {
                dgvItems.DataSource = service.Customers(actorId, txtSearch.Text);
                dgvItems.ClearSelection(); dgvItems.CurrentCell = null;
                if (selectId != 0)
                    foreach (DataGridViewRow row in dgvItems.Rows)
                        if (((CustomerRow)row.DataBoundItem).Id == selectId) { row.Selected = true; dgvItems.CurrentCell = row.Cells[0]; break; }
            }
            finally { loading = false; }
            ShowSelected();
        }
    }
}
