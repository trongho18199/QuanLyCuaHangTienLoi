using System;
using System.Windows.Forms;
using System.Globalization;
using Microsoft.Reporting.WinForms;
using ConvenienceStore.Business.Services;
using ConvenienceStore.UI.Helpers;
namespace ConvenienceStore.UI.Forms
{
    public partial class FrmRevenueReport : DevExpress.XtraEditors.XtraForm
    {
        private readonly int actorId; private readonly TradeService service;
        public FrmRevenueReport() : this(0) { }
        public FrmRevenueReport(int currentUserId) : this(currentUserId, new TradeService()) { }
        public FrmRevenueReport(int currentUserId, TradeService tradeService)
        {
            actorId = currentUserId; service = tradeService; InitializeComponent();
            btnFilter.Click += (s, e) => TradeUi.Run(this, LoadReport);
            btnRdlc.Click += (s, e) => TradeUi.Run(this, PreviewRdlcReport);
            btnExport.Click += (s, e) => TradeUi.Run(this, () => TradeUi.ExportCsv(this, tabs.SelectedIndex == 0 ? dgvDays : dgvInvoices, "DoanhThu.csv"));
        }
        private void Form_Load(object sender, EventArgs e)
        {
            if (TradeUi.IsDesigner(this)) return;
            dtpFrom.Value = new DateTime(DateTime.Today.Year, DateTime.Today.Month, 1); dtpTo.Value = DateTime.Today;
            TradeUi.Grid(dgvDays, "Date|Ngày|110|dd/MM/yyyy", "InvoiceCount|Số hóa đơn|85|N0", "Subtotal|Tiền hàng|130|N2", "DiscountAmount|Tổng giảm giá|120|N2", "PromotionAmount|Trong đó KM|110|N2", "Revenue|Doanh thu|140|N2");
            TradeUi.Grid(dgvInvoices, "Number|Hóa đơn|190", "CreatedAt|Thời gian|125|dd/MM/yyyy HH:mm", "CustomerName|Khách hàng|130", "Subtotal|Tiền hàng|100|N2", "DiscountAmount|Tổng giảm giá|95|N2", "PromotionCode|Mã KM|70", "PromotionAmount|Giảm KM|90|N2", "Total|Thanh toán|110|N2");
            TradeUi.Run(this, LoadReport);
        }

        private void PreviewRdlcReport()
        {
            var data = service.Revenue(actorId, dtpFrom.Value, dtpTo.Value);
            var vi = CultureInfo.GetCultureInfo("vi-VN");

            var viewer = new ReportViewer();
            viewer.Dock = DockStyle.Fill;
            viewer.ProcessingMode = ProcessingMode.Local;
            viewer.LocalReport.ReportEmbeddedResource = "ConvenienceStore.UI.Reports.BaoCaoDoanhThu.rdlc";
            viewer.LocalReport.DataSources.Clear();
            viewer.LocalReport.DataSources.Add(new ReportDataSource("DailyRevenue", data.Days));

            viewer.LocalReport.SetParameters(new[]
            {
                new ReportParameter("FromDate", dtpFrom.Value.ToString("dd/MM/yyyy")),
                new ReportParameter("ToDate", dtpTo.Value.ToString("dd/MM/yyyy")),
                new ReportParameter("InvoiceCount", data.Invoices.Count.ToString("N0", vi)),
                new ReportParameter("Revenue", MoneyText(data.Revenue, vi)),
                new ReportParameter("Cost", data.CostOfGoods.HasValue
                    ? MoneyText(data.CostOfGoods.Value, vi) : "Chưa xác định"),
                new ReportParameter("GrossProfit", data.GrossProfit.HasValue
                    ? MoneyText(data.GrossProfit.Value, vi) : "Chưa xác định"),
                new ReportParameter("ProfitStatus", data.ProfitStatus ?? "")
            });

            var preview = new Form();
            preview.Text = "Xem trước báo cáo doanh thu";
            preview.StartPosition = FormStartPosition.CenterParent;
            preview.WindowState = FormWindowState.Maximized;
            preview.MinimumSize = new System.Drawing.Size(900, 650);
            preview.Controls.Add(viewer);
            preview.Load += (s, e) =>
            {
                viewer.RefreshReport();
                viewer.SetDisplayMode(DisplayMode.PrintLayout);
                viewer.ZoomMode = ZoomMode.PageWidth;
            };
            preview.ShowDialog(this);
        }

        private static string MoneyText(decimal value, CultureInfo culture)
        {
            return value.ToString("N2", culture) + " đ";
        }

        private void LoadReport()
        {
            var data = service.Revenue(actorId, dtpFrom.Value, dtpTo.Value);
            dgvDays.DataSource = data.Days; dgvInvoices.DataSource = data.Invoices;

            lblSummary.Font = new System.Drawing.Font("Segoe UI", 10F, System.Drawing.FontStyle.Bold);
            lblSummary.Height = 50;
            lblSummary.Text = data.Invoices.Count + " hóa đơn | Doanh thu: " + data.Revenue.ToString("N2") + " đ | Giá vốn: " + (data.CostOfGoods.HasValue ? data.CostOfGoods.Value.ToString("N2") + " đ" : "Chưa xác định") + " | Lợi nhuận gộp: " + (data.GrossProfit.HasValue ? data.GrossProfit.Value.ToString("N2") + " đ" : "Chưa xác định") + "\r\n" + data.ProfitStatus;
        }
    }
}
