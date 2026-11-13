using System;
using System.Linq;
using System.Windows.Forms;
using ConvenienceStore.Business.Models;
using ConvenienceStore.Business.Services;
using ConvenienceStore.UI.Helpers;
namespace ConvenienceStore.UI.Forms
{
    public partial class FrmPromotion : DevExpress.XtraEditors.XtraForm
    {
        private readonly int actorId; private readonly CatalogService service; private bool loading;
        public FrmPromotion() : this(0) { }
        public FrmPromotion(int currentUserId) : this(currentUserId, new CatalogService()) { }
        public FrmPromotion(int currentUserId, CatalogService catalogService)
        {
            actorId = currentUserId; service = catalogService; InitializeComponent();
            btnSearch.Click += (s, e) => CatalogUi.Run(this, () => LoadData(0));
            CatalogUi.LiveSearch(this, txtSearch, () => LoadData(0));
            txtSearch.KeyDown += (s, e) => { if (e.KeyCode == Keys.Enter) { e.SuppressKeyPress = true; CatalogUi.Run(this, () => LoadData(0)); } };
            dgvItems.SelectionChanged += (s, e) => { if (!loading) ShowSelected(); };
            btnAdd.Click += (s, e) => CatalogUi.Run(this, () => {
                int id = service.SavePromotion(actorId, ReadInput(0));
                LoadData(id); MessageBox.Show(this, "Đã thêm mã khuyến mãi.", "Thành công");
            });
            btnUpdate.Click += (s, e) => {
                var row = Selected();
                if (row == null) { MessageBox.Show(this, "Hãy chọn mã khuyến mãi cần cập nhật trong danh sách.", "Chưa chọn dòng"); return; }
                CatalogUi.Run(this, () => {
                    service.SavePromotion(actorId, ReadInput(row.Id));
                    LoadData(row.Id); MessageBox.Show(this, "Đã cập nhật mã khuyến mãi.", "Thành công");
                });
            };
            btnDelete.Click += (s, e) => {
                var row = Selected();
                if (row == null) { MessageBox.Show(this, "Hãy chọn mã khuyến mãi cần xóa trong danh sách.", "Chưa chọn dòng"); return; }
                if (MessageBox.Show(this, "Xóa mã khuyến mãi \"" + row.Code + "\"?", "Xác nhận xóa", MessageBoxButtons.YesNo, MessageBoxIcon.Question) != DialogResult.Yes) return;
                CatalogUi.Run(this, () => {
                    var result = service.DeletePromotion(actorId, row.Id);
                    if (!result.Deactivated) ClearInputs();
                    LoadData(result.Deactivated ? row.Id : 0);
                    MessageBox.Show(this, result.Message, "Kết quả");
                });
            };
            btnClear.Click += (s, e) => { ClearInputs(); dgvItems.ClearSelection(); txtCode.Focus(); };
        }
        private void Form_Load(object sender, EventArgs e)
        {
            if (CatalogUi.IsDesigner(this)) return;
            CatalogUi.Grid(dgvItems, "Code|Mã|90", "Description|Mô tả|190", "DiscountPercent|Giảm (%)|70|0.##", "ValidFrom|Từ ngày|90|dd/MM/yyyy", "ValidTo|Đến ngày|90|dd/MM/yyyy", "Status|Trạng thái|110");
            CatalogUi.Run(this, () => LoadData(0));
        }
        private PromotionRow Selected() { return dgvItems.CurrentRow == null ? null : dgvItems.CurrentRow.DataBoundItem as PromotionRow; }
        private PromotionInput ReadInput(int id)
        {
            return new PromotionInput { Id = id, Code = txtCode.Text, Description = txtDescription.Text, DiscountPercent = nudPercent.Value,
                ValidFrom = dtpFrom.Checked ? (DateTime?)dtpFrom.Value.Date : null,
                ValidTo = dtpTo.Checked ? (DateTime?)dtpTo.Value.Date : null, IsActive = chkActive.Checked };
        }
        private void ClearInputs()
        {
            txtCode.Clear(); txtDescription.Clear(); nudPercent.Value = 5M;
            dtpFrom.Checked = false; dtpTo.Checked = false; chkActive.Checked = true;
        }
        private static void SetDate(DateTimePicker picker, DateTime? value)
        {
            if (value.HasValue) { picker.Value = value.Value; picker.Checked = true; }
            else picker.Checked = false;
        }
        private void ShowSelected()
        {
            var row = Selected(); if (row == null) return;
            txtCode.Text = row.Code; txtDescription.Text = row.Description;
            nudPercent.Value = Math.Min(Math.Max(row.DiscountPercent, nudPercent.Minimum), nudPercent.Maximum);
            SetDate(dtpFrom, row.ValidFrom); SetDate(dtpTo, row.ValidTo); chkActive.Checked = row.IsActive;
        }
        private void LoadData(int selectId)
        {
            loading = true;
            try
            {
                dgvItems.DataSource = service.Promotions(actorId, txtSearch.Text);
                dgvItems.ClearSelection(); dgvItems.CurrentCell = null;
                if (selectId != 0)
                    foreach (DataGridViewRow row in dgvItems.Rows)
                        if (((PromotionRow)row.DataBoundItem).Id == selectId) { row.Selected = true; dgvItems.CurrentCell = row.Cells[0]; break; }
            }
            finally { loading = false; }
            ShowSelected();
        }
    }
}
