using System;
using System.Drawing;
using System.Windows.Forms;
using ConvenienceStore.Business.Models;
using ConvenienceStore.Business.Services;
using ConvenienceStore.UI.Helpers;

namespace ConvenienceStore.UI.Forms
{
    public partial class FrmShiftManagement : DevExpress.XtraEditors.XtraForm
    {
        private readonly int actorId;
        private readonly TradeService service = new TradeService();
        private ShiftInfo current;

        public FrmShiftManagement() : this(0) { }
        public FrmShiftManagement(int currentUserId)
        {
            actorId = currentUserId;
            InitializeComponent();
            btnOpen.Click += (s, e) => Open();
            btnClose.Click += (s, e) => CloseShiftUi();
            btnRefresh.Click += (s, e) => TradeUi.Run(this, Reload);
            Activated += (s, e) => { if (!TradeUi.IsDesigner(this)) TradeUi.Run(this, Reload); };
            Load += (s, e) => { if (!TradeUi.IsDesigner(this)) TradeUi.Run(this, Reload); };
        }
        private void Reload()
        {
            historyGrid.DataSource = service.ShiftHistory(actorId);
            current = service.CurrentShift(actorId);
            if (current == null) { lblStatus.Text = "Chưa có ca đang mở."; btnOpen.Enabled = true; btnClose.Enabled = false; numClosing.Enabled = false; numOpening.Enabled = true; return; }
            lblStatus.Text = "Ca: " + current.Number + " | Mở lúc: " + current.OpenedAt.ToString("dd/MM/yyyy HH:mm") + "\r\nTiền đầu ca: " + current.OpeningCash.ToString("N2") + " đ\r\nBán tiền mặt: " + current.CashSales.ToString("N2") + " đ | Thu/chi khác: " + current.CashAdjustments.ToString("N2") + " đ\r\nDự kiến cuối ca: " + current.ExpectedCash.ToString("N2") + " đ";
            btnOpen.Enabled = false; btnClose.Enabled = true; numClosing.Enabled = true; numOpening.Enabled = false;
        }
        private void Open()
        {
            TradeUi.Run(this, () => { service.OpenShift(actorId, numOpening.Value, txtNote.Text); Reload(); MessageBox.Show(this, "Đã mở ca bán hàng.", "Thành công"); });
        }
        private void CloseShiftUi()
        {
            if (MessageBox.Show(this, "Xác nhận tiền mặt thực đếm: " + numClosing.Value.ToString("N2") + " đ?\nToàn bộ tiền thực đếm sẽ được ghi thu vào quỹ tiền mặt cửa hàng khi chốt ca.", "Chốt ca", MessageBoxButtons.YesNo, MessageBoxIcon.Question) != DialogResult.Yes) return;
            TradeUi.Run(this, () => { var result = service.CloseShift(actorId, numClosing.Value, txtNote.Text); Reload(); MessageBox.Show(this, "Đã chốt ca và ghi thu vào quỹ tiền mặt.\nDự kiến: " + result.ExpectedCash.ToString("N2") + " đ\nThực tế: " + result.ClosingCash.Value.ToString("N2") + " đ\nChênh lệch: " + result.Difference.Value.ToString("N2") + " đ", "Đối soát ca", MessageBoxButtons.OK, MessageBoxIcon.Information); });
        }
    }
}
