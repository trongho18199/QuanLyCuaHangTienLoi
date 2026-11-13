using System;
using System.Drawing;
using System.Globalization;
using System.Linq;
using System.Windows.Forms;
using ConvenienceStore.Business.Models;
using ConvenienceStore.Business.Services;
using ConvenienceStore.Data.Repositories;
using ConvenienceStore.UI.Helpers;
namespace ConvenienceStore.UI.Forms
{
    public partial class FrmInvoice : DevExpress.XtraEditors.XtraForm
    {
        private static readonly CultureInfo Vn = CultureInfo.GetCultureInfo("vi-VN");
        private readonly int actorId; private readonly CatalogService service; private readonly TradeService trade;
        private bool loading; private InvoiceDetail current;
        public FrmInvoice() : this(0) { }
        public FrmInvoice(int currentUserId) : this(currentUserId, new CatalogService()) { }
        public FrmInvoice(int currentUserId, CatalogService catalogService) : this(currentUserId, catalogService, new TradeService()) { }
        public FrmInvoice(int currentUserId, CatalogService catalogService, TradeService tradeService)
        {
            actorId = currentUserId; service = catalogService; trade = tradeService; InitializeComponent();
            btnSearch.Click += (s, e) => CatalogUi.Run(this, LoadInvoices);
            CatalogUi.LiveSearch(this, txtKeyword, LoadInvoices);
            txtKeyword.KeyDown += (s, e) => { if (e.KeyCode == Keys.Enter) { e.SuppressKeyPress = true; CatalogUi.Run(this, LoadInvoices); } };
            dgvInvoices.SelectionChanged += (s, e) => { if (!loading) CatalogUi.Run(this, ShowDetail); };
            btnExport.Click += (s, e) => CatalogUi.Run(this, () => CatalogUi.ExportCsv(this, dgvInvoices, "HoaDon.csv"));
            btnCancelInvoice.Click += (s, e) => CancelCurrentInvoice();
            btnReturn.Click += (s, e) => ReturnCurrentLine();
            dgvInvoices.CellFormatting += (s, e) => {
                if (e.RowIndex < 0 || e.RowIndex >= dgvInvoices.Rows.Count) return;
                var row = dgvInvoices.Rows[e.RowIndex].DataBoundItem as InvoiceListRow;
                if (row != null && row.IsCancelled) e.CellStyle.ForeColor = Color.Gray;   
            };
            btnPrint.Click += (s, e) => {
                if (current == null) { MessageBox.Show(this, "Hãy chọn một hóa đơn trong danh sách trước khi in.", "Chưa chọn hóa đơn"); return; }
                using (var preview = new FrmInvoiceReport(current)) preview.ShowDialog(this);
            };
        }
        private void Form_Load(object sender, EventArgs e)
        {
            if (CatalogUi.IsDesigner(this)) return;
            btnCancelInvoice.Visible = CanCancel();
            btnReturn.Visible = CanReturn();
            dtpFrom.Value = new DateTime(DateTime.Today.Year, DateTime.Today.Month, 1); dtpTo.Value = DateTime.Today;
            CatalogUi.Grid(dgvInvoices, "Number|Số hóa đơn|190", "CreatedAt|Thời gian|120|dd/MM/yyyy HH:mm", "CustomerName|Khách hàng|150", "Cashier|Thu ngân|120",
                "Subtotal|Tiền hàng|90|N0", "DiscountAmount|Tổng giảm giá|85|N0", "PromotionCode|Mã KM|65", "Total|Thành tiền|100|N0", "Status|Trạng thái|80");
            CatalogUi.Grid(dgvLines, "No|STT|40", "ProductName|Sản phẩm|220", "Quantity|SL|50|N0", "UnitPrice|Đơn giá|90|N0", "LineTotal|Thành tiền|100|N0");
            CatalogUi.Run(this, LoadInvoices);
        }
        private InvoiceListRow Selected()
        { return dgvInvoices.CurrentRow == null ? null : dgvInvoices.CurrentRow.DataBoundItem as InvoiceListRow; }
        private void LoadInvoices()
        {
            loading = true;
            try { dgvInvoices.DataSource = service.Invoices(actorId, dtpFrom.Value, dtpTo.Value, txtKeyword.Text); }
            finally { loading = false; }
            CatalogUi.Run(this, ShowDetail);
        }
        private void ShowDetail()
        {
            var row = Selected();
            if (row == null)
            {
                current = null; dgvLines.DataSource = null; lblInfo.Text = ""; btnCancelInvoice.Enabled = false; btnReturn.Enabled = false;
                lblSummary.Text = "Không có hóa đơn nào trong khoảng ngày đã chọn."; return;
            }
            current = service.GetInvoice(actorId, row.Id);
            dgvLines.DataSource = current.Lines;
            lblInfo.Text = current.Number + "   |   " + current.CreatedAt.ToString("dd/MM/yyyy HH:mm") + "   |   Khách: " + current.CustomerName + "   |   Thu ngân: " + current.Cashier;
            if (current.IsCancelled)
                lblInfo.Text += "   |   ĐÃ HỦY" + (current.CancelledAt.HasValue ? " lúc " + current.CancelledAt.Value.ToString("dd/MM/yyyy HH:mm") : "")
                    + (string.IsNullOrEmpty(current.CancelledBy) ? "" : " bởi " + current.CancelledBy) + ": " + Shorten(current.CancelReason, 60);
            lblInfo.ForeColor = current.IsCancelled ? Color.Firebrick : SystemColors.ControlText;
            btnCancelInvoice.Enabled = !current.IsCancelled;
            btnReturn.Enabled = !current.IsCancelled;
            lblSummary.Text = (current.IsCancelled ? "HÓA ĐƠN ĐÃ HỦY (không tính doanh thu)\r\n" : "")
                + "Tiền hàng:  " + Money(current.Subtotal) + "\r\n"
                + "Giảm giá thành viên (" + current.DiscountPercent.ToString("0.##", Vn) + "%):  " + Money(current.MemberDiscountAmount) + "\r\n"
                + (current.PromotionAmount > 0m ? "Khuyến mãi" + PromoLabel(current) + ":  " + Money(current.PromotionAmount) + "\r\n" : "")
                + "Thành tiền:  " + Money(current.Total) + "\r\n"
                + "Thanh toán:  " + current.PaymentMethod + "\r\n"
                + (current.PaymentMethod == PaymentMethods.Transfer ? "Mã chuyển khoản:  " + current.PaymentReference + "\r\n" : "")
                + "Tiền khách đưa:  " + Money(current.CashReceived) + "\r\n"
                + "Tiền thừa:  " + Money(current.Change);
        }
        private static string Shorten(string value, int max)
        { return string.IsNullOrEmpty(value) || value.Length <= max ? value : value.Substring(0, max) + "…"; }
        
        private static bool CanCancel()
        {
            try
            {
                var user = AppSession.CurrentUser;
                if (user == null || AppSession.IsAdmin) return true;
                return new RoleRepository().GetPermissionCodes(user.RoleId).Contains("fnInvoiceCancel");
            }
            catch (Exception) { return true; }
        }
        private static bool CanReturn()
        {
            try
            {
                var user = AppSession.CurrentUser;
                if (user == null || AppSession.IsAdmin) return true;
                return new RoleRepository().GetPermissionCodes(user.RoleId).Contains("fnSaleReturn");
            }
            catch (Exception) { return true; }
        }
        private void CancelCurrentInvoice()
        {
            var inv = current;
            if (inv == null) { MessageBox.Show(this, "Hãy chọn một hóa đơn trong danh sách trước khi hủy.", "Chưa chọn hóa đơn"); return; }
            if (inv.IsCancelled) { MessageBox.Show(this, "Hóa đơn này đã được hủy.", "Đã hủy"); return; }
            string question = "Hủy hóa đơn " + inv.Number + " (" + Money(inv.Total) + ")?\r\n\r\nSố lượng của " + inv.Lines.Count +
                " dòng hàng sẽ được hoàn lại vào kho và hóa đơn không còn tính vào doanh thu. Không thể hoàn tác.";
            if (MessageBox.Show(this, question, "Xác nhận hủy hóa đơn", MessageBoxButtons.YesNo, MessageBoxIcon.Warning) != DialogResult.Yes) return;
            string reason = CatalogUi.PromptText(this, "Lý do hủy hóa đơn", "Nhập lý do hủy hóa đơn " + inv.Number + " (bắt buộc):", 200);
            if (reason == null) return;
            CatalogUi.Run(this, () => {
                trade.CancelInvoice(actorId, inv.Id, reason);
                int id = inv.Id;
                LoadInvoices();
                foreach (DataGridViewRow gridRow in dgvInvoices.Rows)
                    if (((InvoiceListRow)gridRow.DataBoundItem).Id == id) { dgvInvoices.CurrentCell = gridRow.Cells[0]; break; }
                MessageBox.Show(this, "Đã hủy hóa đơn và hoàn hàng vào kho.", "Thành công", MessageBoxButtons.OK, MessageBoxIcon.Information);
            });
        }
        private void ReturnCurrentLine()
        {
            if (current == null || current.IsCancelled) { MessageBox.Show(this, "Hãy chọn hóa đơn hợp lệ, chưa bị hủy.", "Không thể đổi/trả"); return; }
            var line = dgvLines.CurrentRow == null ? null : dgvLines.CurrentRow.DataBoundItem as InvoiceLineRow;
            if (line == null) { MessageBox.Show(this, "Hãy chọn dòng hàng cần đổi/trả.", "Chưa chọn dòng"); return; }
            string qtyText = CatalogUi.PromptText(this, "Số lượng", "Nhập số lượng đổi/trả (tối đa " + line.Quantity + "):", 9);
            int qty;
            if (qtyText == null || !int.TryParse(qtyText, out qty) || qty < 1 || qty > line.Quantity) { if (qtyText != null) MessageBox.Show(this, "Số lượng không hợp lệ.", "Lỗi"); return; }
            var kindResult = MessageBox.Show(this, "Chọn Có nếu đổi hàng, Không nếu trả hàng.", "Loại xử lý", MessageBoxButtons.YesNoCancel, MessageBoxIcon.Question);
            if (kindResult == DialogResult.Cancel) return;
            string kind = kindResult == DialogResult.Yes ? "Đổi hàng" : "Trả hàng";
            string replacementBarcode = kind == "Đổi hàng" ? CatalogUi.PromptText(this, "Mã vạch hàng đổi", "Nhập mã vạch sản phẩm thay thế (số lượng bằng hàng trả):", 50) : null;
            if (kind == "Đổi hàng" && replacementBarcode == null) return;
            string reason = CatalogUi.PromptText(this, "Lý do đổi/trả", "Nhập lý do (bắt buộc):", 200);
            if (reason == null) return;
            var settlement = MessageBox.Show(this, "Khoản hoàn/thu thêm được thanh toán bằng tiền mặt?\nChọn Không nếu thanh toán ngoài tiền mặt; chọn Hủy để dừng.", "Phương thức đối soát", MessageBoxButtons.YesNoCancel, MessageBoxIcon.Question);
            if (settlement == DialogResult.Cancel) return;
            CatalogUi.Run(this, () => {
                var result = trade.ReturnSale(actorId, new ReturnRequest { InvoiceId = current.Id, ReturnKind = kind, ReplacementBarcode = replacementBarcode, CashSettlement = settlement == DialogResult.Yes, Reason = reason, Lines = new System.Collections.Generic.List<ReturnLineRequest> { new ReturnLineRequest { SaleInvoiceLineId = line.Id, Quantity = qty } } });
                LoadInvoices();
                MessageBox.Show(this, "Đã lập phiếu " + result.Number + ".\n" + (result.RefundAmount >= 0m ? "Hoàn khách: " : "Thu thêm: ") + Money(Math.Abs(result.RefundAmount)), "Đổi/trả thành công", MessageBoxButtons.OK, MessageBoxIcon.Information);
            });
        }
        private static string Money(decimal value) { return value.ToString("N0", Vn) + " đ"; }
        private static string PromoLabel(InvoiceDetail inv) { return string.IsNullOrEmpty(inv.PromotionCode) ? "" : " (" + inv.PromotionCode + ")"; }

    }
}
