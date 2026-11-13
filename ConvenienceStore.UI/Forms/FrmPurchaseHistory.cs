using System;
using System.Drawing;
using System.Drawing.Printing;
using System.Globalization;
using System.Linq;
using System.Windows.Forms;
using ConvenienceStore.Business.Models;
using ConvenienceStore.Business.Services;
using ConvenienceStore.UI.Helpers;
namespace ConvenienceStore.UI.Forms
{
    
    public partial class FrmPurchaseHistory : DevExpress.XtraEditors.XtraForm
    {
        private static readonly CultureInfo Vn = CultureInfo.GetCultureInfo("vi-VN");
        private readonly int actorId; private readonly CatalogService service;
        private readonly PrintDocument printDocument = new PrintDocument();
        private bool loading; private int printIndex; private PurchaseDetail current;
        public FrmPurchaseHistory() : this(0) { }
        public FrmPurchaseHistory(int currentUserId) : this(currentUserId, new CatalogService()) { }
        public FrmPurchaseHistory(int currentUserId, CatalogService catalogService)
        {
            actorId = currentUserId; service = catalogService; InitializeComponent();
            printDocument.DocumentName = "Phieu nhap hang";
            printDocument.BeginPrint += (s, e) => { printIndex = 0; };
            printDocument.PrintPage += PrintReceiptPage;
            btnSearch.Click += (s, e) => CatalogUi.Run(this, LoadReceipts);
            CatalogUi.LiveSearch(this, txtKeyword, LoadReceipts);
            txtKeyword.KeyDown += (s, e) => { if (e.KeyCode == Keys.Enter) { e.SuppressKeyPress = true; CatalogUi.Run(this, LoadReceipts); } };
            dgvReceipts.SelectionChanged += (s, e) => { if (!loading) CatalogUi.Run(this, ShowDetail); };
            btnExport.Click += (s, e) => CatalogUi.Run(this, () => CatalogUi.ExportCsv(this, dgvReceipts, "PhieuNhap.csv"));
            btnPrint.Click += (s, e) => {
                if (current == null) { MessageBox.Show(this, "Hãy chọn một phiếu nhập trong danh sách trước khi in.", "Chưa chọn phiếu nhập"); return; }
                CatalogUi.Run(this, () => {
                    using (var preview = new PrintPreviewDialog { Document = printDocument, Width = 900, Height = 720 })
                        preview.ShowDialog(this);
                });
            };
        }
        private void Form_Load(object sender, EventArgs e)
        {
            if (CatalogUi.IsDesigner(this)) return;
            dtpFrom.Value = new DateTime(DateTime.Today.Year, DateTime.Today.Month, 1); dtpTo.Value = DateTime.Today;
            CatalogUi.Grid(dgvReceipts, "Number|Số phiếu|190", "CreatedAt|Thời gian|120|dd/MM/yyyy HH:mm", "SupplierName|Nhà cung cấp|170",
                "CreatedBy|Người lập|120", "Total|Tổng tiền|110|N0", "Note|Ghi chú|180");
            CatalogUi.Grid(dgvLines, "No|STT|40", "ProductName|Sản phẩm|220", "Quantity|SL|50|N0", "UnitCost|Đơn giá nhập|100|N0", "LineTotal|Thành tiền|100|N0");
            CatalogUi.Run(this, LoadReceipts);
        }
        private PurchaseListRow Selected()
        { return dgvReceipts.CurrentRow == null ? null : dgvReceipts.CurrentRow.DataBoundItem as PurchaseListRow; }
        private void LoadReceipts()
        {
            loading = true;
            try { dgvReceipts.DataSource = service.PurchaseReceipts(actorId, dtpFrom.Value, dtpTo.Value, txtKeyword.Text); }
            finally { loading = false; }
            CatalogUi.Run(this, ShowDetail);
        }
        private void ShowDetail()
        {
            var row = Selected();
            if (row == null)
            {
                current = null; dgvLines.DataSource = null; lblInfo.Text = "";
                lblSummary.Text = "Không có phiếu nhập nào trong khoảng ngày đã chọn."; return;
            }
            current = service.GetPurchaseReceipt(actorId, row.Id);
            dgvLines.DataSource = current.Lines;
            lblInfo.Text = current.Number + "   |   " + current.CreatedAt.ToString("dd/MM/yyyy HH:mm") + "   |   Nhà cung cấp: " + current.SupplierName + "   |   Người lập: " + current.CreatedBy;
            lblSummary.Text = "Số dòng hàng:  " + current.Lines.Count.ToString("N0", Vn) + "\r\n"
                + "Tổng số lượng:  " + current.Lines.Sum(x => x.Quantity).ToString("N0", Vn) + "\r\n"
                + "Tổng tiền nhập:  " + Money(current.Total)
                + (string.IsNullOrWhiteSpace(current.Note) ? "" : "\r\n\r\nGhi chú:  " + current.Note);
        }
        private static string Money(decimal value) { return value.ToString("N0", Vn) + " đ"; }

        private void PrintReceiptPage(object sender, PrintPageEventArgs e)
        {
            var rec = current;
            if (rec == null) { e.HasMorePages = false; return; }
            using (var title = new Font("Segoe UI", 16F, FontStyle.Bold))
            using (var bold = new Font("Segoe UI", 10F, FontStyle.Bold))
            using (var normal = new Font("Segoe UI", 10F))
            using (var center = new StringFormat { Alignment = StringAlignment.Center })
            using (var right = new StringFormat { Alignment = StringAlignment.Far })
            using (var trim = new StringFormat { Trimming = StringTrimming.EllipsisCharacter, FormatFlags = StringFormatFlags.NoWrap })
            {
                var g = e.Graphics;
                float left = e.MarginBounds.Left, width = e.MarginBounds.Width, y = e.MarginBounds.Top;
                float h = normal.GetHeight(g) + 6;
                if (printIndex == 0)
                {
                    g.DrawString("CỬA HÀNG TIỆN LỢI", title, Brushes.Black, new RectangleF(left, y, width, 30), center); y += 34;
                    g.DrawString("PHIẾU NHẬP HÀNG", bold, Brushes.Black, new RectangleF(left, y, width, h), center); y += h + 6;
                    g.DrawString("Số: " + rec.Number, normal, Brushes.Black, new RectangleF(left, y, width, h), trim); y += h;
                    g.DrawString("Thời gian: " + rec.CreatedAt.ToString("dd/MM/yyyy HH:mm"), normal, Brushes.Black, new RectangleF(left, y, width, h), trim); y += h;
                    g.DrawString("Nhà cung cấp: " + rec.SupplierName, normal, Brushes.Black, new RectangleF(left, y, width, h), trim); y += h;
                    g.DrawString("Người lập: " + rec.CreatedBy, normal, Brushes.Black, new RectangleF(left, y, width, h), trim); y += h;
                    if (!string.IsNullOrWhiteSpace(rec.Note)) { g.DrawString("Ghi chú: " + rec.Note, normal, Brushes.Black, new RectangleF(left, y, width, h), trim); y += h; }
                    y += 8;
                }
                float cNo = 40, cQty = 60, cPrice = 110, cTotal = 120, cName = width - cNo - cQty - cPrice - cTotal;
                Action<string, string, string, string, string, Font> row = (no, name, qty, price, total, font) =>
                {
                    float x = left;
                    g.DrawString(no, font, Brushes.Black, new RectangleF(x, y, cNo, h), trim); x += cNo;
                    g.DrawString(name, font, Brushes.Black, new RectangleF(x, y, cName - 6, h), trim); x += cName;
                    g.DrawString(qty, font, Brushes.Black, new RectangleF(x, y, cQty - 6, h), right); x += cQty;
                    g.DrawString(price, font, Brushes.Black, new RectangleF(x, y, cPrice - 6, h), right); x += cPrice;
                    g.DrawString(total, font, Brushes.Black, new RectangleF(x, y, cTotal, h), right);
                    y += h;
                };
                row("STT", "Sản phẩm", "SL", "Đơn giá nhập", "Thành tiền", bold);
                g.DrawLine(Pens.Black, left, y, left + width, y); y += 4;
                float reserve = h * 2 + 20; 
                while (printIndex < rec.Lines.Count && y + h < e.MarginBounds.Bottom - reserve)
                {
                    var line = rec.Lines[printIndex++];
                    row(line.No.ToString(), line.ProductName, line.Quantity.ToString("N0", Vn), line.UnitCost.ToString("N0", Vn), line.LineTotal.ToString("N0", Vn), normal);
                }
                if (printIndex < rec.Lines.Count) { e.HasMorePages = true; return; }
                g.DrawLine(Pens.Black, left, y, left + width, y); y += 6;
                g.DrawString("Tổng tiền nhập:", bold, Brushes.Black, new RectangleF(left, y, width - cTotal - 100, h), right);
                g.DrawString(Money(rec.Total), bold, Brushes.Black, new RectangleF(left + width - cTotal - 100, y, cTotal + 100, h), right);
                e.HasMorePages = false;
            }
        }
    }
}
