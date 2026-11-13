using System;
using System.Drawing;
using System.Globalization;
using System.Linq;
using DevExpress.XtraCharts;
using System.Windows.Forms;
using ConvenienceStore.Business.Models;
using ConvenienceStore.Business.Services;
using ConvenienceStore.UI.Helpers;
namespace ConvenienceStore.UI.Forms
{
    
    public partial class FrmDashboard : DevExpress.XtraEditors.XtraForm
    {
        private static readonly CultureInfo Vn = CultureInfo.GetCultureInfo("vi-VN");
        private readonly int actorId; private readonly DashboardService service;
        public FrmDashboard() : this(0) { }
        public FrmDashboard(int currentUserId) : this(currentUserId, new DashboardService()) { }
        public FrmDashboard(int currentUserId, DashboardService dashboardService)
        {
            actorId = currentUserId; service = dashboardService; InitializeComponent();
            btnRefresh.Click += (s, e) => CatalogUi.Run(this, LoadData);
            dgvLow.CellFormatting += (s, e) => {
                if (e.RowIndex < 0 || e.RowIndex >= dgvLow.Rows.Count) return;
                var row = dgvLow.Rows[e.RowIndex].DataBoundItem as LowStockRow;
                if (row != null && row.Stock <= 0) e.CellStyle.ForeColor = Color.Firebrick;
            };
        }
        private void Form_Load(object sender, EventArgs e)
        {
            if (CatalogUi.IsDesigner(this)) return;
            lblCard4Title.Text = "Sản phẩm tồn thấp (≤ " + StockPolicy.LowStockThreshold + ")";
            CatalogUi.Grid(dgvDays, "Date|Ngày|130|dd/MM/yyyy", "InvoiceCount|Số hóa đơn|110|N0", "Revenue|Doanh thu|150|N0");
            CatalogUi.Grid(dgvTop, "Rank|#|40", "ProductName|Sản phẩm|230", "Quantity|Đã bán|80|N0", "Revenue|Doanh thu|110|N0");
            CatalogUi.Grid(dgvLow, "ProductName|Sản phẩm|220", "Barcode|Mã SP|100", "CategoryName|Danh mục|110", "Stock|Tồn|60|N0", "Status|Tình trạng|90");
            CatalogUi.Run(this, LoadData);
        }
        private static string Money(decimal value) { return value.ToString("N0", Vn) + " đ"; }
        private void LoadData()
        {
            var data = service.Load(actorId);
            lblCard1Value.Text = Money(data.TodayRevenue); lblCard1Sub.Text = data.TodayInvoices.ToString("N0", Vn) + " hóa đơn";
            lblCard2Value.Text = Money(data.MonthRevenue); lblCard2Sub.Text = data.MonthInvoices.ToString("N0", Vn) + " hóa đơn";
            lblCard3Value.Text = Money(data.MonthInvoices == 0 ? 0m : Math.Round(data.MonthRevenue / data.MonthInvoices, 0));
            lblCard3Sub.Text = data.TodayInvoices == 0 ? "Hôm nay chưa có hóa đơn" : "Hôm nay: " + Money(Math.Round(data.TodayRevenue / data.TodayInvoices, 0));
            lblCard4Value.Text = data.LowStockCount.ToString("N0", Vn);
            lblCard4Value.ForeColor = data.LowStockCount > 0 ? Color.Firebrick : Color.FromArgb(28, 57, 73);
            lblCard4Sub.Text = "Đã hết hàng: " + data.OutOfStockCount.ToString("N0", Vn);

            dgvDays.DataSource = data.Days; 
            dgvTop.DataSource = data.TopProducts; 
            dgvLow.DataSource = data.LowStock;
            var series = new Series("Số lượng", ViewType.Bar);
            foreach (var row in data.TopProducts.Take(5)) series.Points.Add(new SeriesPoint(row.ProductName, row.Quantity));
            topChart.Series.Clear(); 
            topChart.Series.Add(series);
        }
    }
}
