using System;
using System.Drawing;
using System.Globalization;
using System.Linq;
using System.Windows.Forms;
using ConvenienceStore.Business.Models;
using ConvenienceStore.Business.Services;
using ConvenienceStore.UI.Helpers;

namespace ConvenienceStore.UI.Forms
{
    public partial class FrmSupplierDebt : DevExpress.XtraEditors.XtraForm
    {
        private readonly int actorId;
        private readonly TradeService service = new TradeService();
        private Guid operationId = Guid.NewGuid();

        public FrmSupplierDebt() : this(0) { }
        public FrmSupplierDebt(int userId)
        {
            actorId = userId; InitializeComponent();
            CatalogUi.LiveSearch(this, keyword, Reload);
            btnFind.Click += (s, e) => TradeUi.Run(this, Reload);
            btnRegister.Click += (s, e) => TradeUi.Run(this, Register);
            btnPay.Click += (s, e) => TradeUi.Run(this, Pay);
            TradeUi.Grid(grid, "Number|Phiếu nhập|180", "CreatedAt|Ngày nhập|120|dd/MM/yyyy", "SupplierName|Nhà cung cấp|190", "Total|Tổng tiền|125|N2", "Paid|Đã trả|125|N2", "Balance|Còn nợ|125|N2", "Status|Trạng thái|130");
            TradeUi.Grid(history, "CreatedAt|Ngày thanh toán|160|dd/MM/yyyy HH:mm", "Amount|Số tiền|150|N2", "Method|Phương thức|150", "Note|Ghi chú|300");
            grid.SelectionChanged += (s, e) => TradeUi.Run(this, ShowHistory);
            Load += (s, e) => { if (!TradeUi.IsDesigner(this)) TradeUi.Run(this, Reload); };
        }
        private SupplierDebtRow Selected() { return grid.CurrentRow == null ? null : grid.CurrentRow.DataBoundItem as SupplierDebtRow; }
        private void Reload()
        {
            var rows = service.SupplierDebts(actorId, keyword.Text);
            grid.DataSource = rows;
            summary.Text = "Tổng nợ đã xác định: " + rows.Where(x => x.Balance.HasValue).Sum(x => x.Balance.Value).ToString("N2") + " đ | Phiếu chưa xác định: " + rows.Count(x => !x.Balance.HasValue) + ". Lịch sử thanh toán ở bảng bên dưới.";
            ShowHistory();
        }
        private void ShowHistory()
        {
            var row = Selected(); history.DataSource = row == null || !row.PayableId.HasValue ? null : service.SupplierPaymentHistory(actorId, row.PayableId.Value);
        }
        private decimal? AskAmount(string title, string prompt)
        {
            string text = CatalogUi.PromptText(this, title, prompt, 30); if (text == null) return null;
            decimal value;
            if (!decimal.TryParse(text, NumberStyles.Number, CultureInfo.CurrentCulture, out value)) throw new TradeException("Số tiền không hợp lệ.");
            return value;
        }
        private void Register()
        {
            var row = Selected(); if (row == null) throw new TradeException("Hãy chọn phiếu nhập.");
            if (row.PayableId.HasValue) throw new TradeException("Phiếu đã được xác lập công nợ.");
            var paid = AskAmount("Xác lập công nợ", "Phiếu " + row.Number + " tổng " + row.Total.ToString("N2") + " đ. Nhập tổng số đã trả đến nay:");
            if (!paid.HasValue) return;
            if (MessageBox.Show(this, "Xác lập số đã trả " + paid.Value.ToString("N2") + " đ?", "Xác nhận", MessageBoxButtons.YesNo) != DialogResult.Yes) return;
            service.RegisterSupplierDebt(actorId, row.ReceiptId, paid.Value); Reload();
        }
        private void Pay()
        {
            var row = Selected(); if (row == null || !row.PayableId.HasValue) throw new TradeException("Chọn phiếu đã xác lập công nợ.");
            if (row.Balance <= 0m) throw new TradeException("Phiếu đã thanh toán đủ.");
            var amount = AskAmount("Thanh toán công nợ", "Còn nợ " + row.Balance.Value.ToString("N2") + " đ. Nhập số tiền thanh toán:");
            if (!amount.HasValue) return;
            var method = MessageBox.Show(this, "Chọn Có: tiền mặt; Không: chuyển khoản; Hủy: dừng.", "Phương thức", MessageBoxButtons.YesNoCancel);
            if (method == DialogResult.Cancel) return;
            string note = CatalogUi.PromptText(this, "Ghi chú", "Nhập ghi chú hoặc mã tham chiếu (có thể để trống):", 200);
            if (note == null) return;
            if (MessageBox.Show(this, "Ghi thanh toán " + amount.Value.ToString("N2") + " đ cho " + row.Number + "?", "Xác nhận", MessageBoxButtons.YesNo) != DialogResult.Yes) return;
            service.PaySupplier(actorId, row.PayableId.Value, operationId, amount.Value, method == DialogResult.Yes ? PaymentMethods.Cash : PaymentMethods.Transfer, note);
            operationId = Guid.NewGuid(); Reload(); MessageBox.Show(this, "Đã ghi thanh toán công nợ.");
        }
    }
}
