using System;
using System.Linq;
using System.Windows.Forms;
using ConvenienceStore.Business.Models;
using ConvenienceStore.Business.Services;
using ConvenienceStore.Data;
using ConvenienceStore.UI.Helpers;
namespace ConvenienceStore.UI.Forms
{
    public partial class FrmStockMovement : DevExpress.XtraEditors.XtraForm
    {
        private readonly int actorId; private readonly TradeService service;
        public FrmStockMovement() : this(0) { }
        public FrmStockMovement(int currentUserId) : this(currentUserId, new TradeService()) { }
        public FrmStockMovement(int currentUserId, TradeService tradeService)
        {
            actorId = currentUserId; service = tradeService; InitializeComponent();
            btnFilter.Click += (s, e) => TradeUi.Run(this, LoadReport);
            btnExport.Click += (s, e) => TradeUi.Run(this, () => TradeUi.ExportCsv(this, tabs.SelectedIndex == 0 ? dgvMovements : (tabs.SelectedIndex == 1 ? dgvBalances : tabs.SelectedIndex == 2 ? dgvExpiring : tabs.SelectedIndex == 3 ? dgvBatchHistory : dgvBatches), "TonKho.csv"));
        }
        private void Form_Load(object sender, EventArgs e)
        {
            if (TradeUi.IsDesigner(this)) return;
            dtpFrom.Value = new DateTime(DateTime.Today.Year, DateTime.Today.Month, 1); dtpTo.Value = DateTime.Today;
            cboKind.Items.AddRange(new object[] { "Tất cả loại", "Nhập hàng", "Bán hàng", "Hủy hóa đơn (hoàn kho)", "Điều chỉnh tồn kho" }); cboKind.SelectedIndex = 0;
            TradeUi.Grid(dgvMovements, "CreatedAt|Thời gian|120|dd/MM/yyyy HH:mm", "ReferenceNumber|Chứng từ|180", "ProductName|Sản phẩm|155", "Kind|Loại|80", "Quantity|Biến động|65|N0", "StockBefore|Trước|60|N0", "StockAfter|Sau|60|N0", "Note|Ghi chú|200");
            TradeUi.Grid(dgvBalances, "ProductName|Sản phẩm|210", "Opening|Tồn đầu|90|N0", "Purchased|Nhập|90|N0", "Sold|Bán (ròng)|90|N0", "Adjusted|Điều chỉnh|90|N0", "Closing|Tồn cuối kỳ|100|N0", "Current|Tồn hiện tại|100|N0");
            TradeUi.Grid(dgvExpiring, "ProductName|Sản phẩm|230", "BatchNumber|Số lô|120", "ExpiryDate|Hạn dùng|100|dd/MM/yyyy", "DaysLeft|Còn ngày|90|N0", "RemainingQuantity|Số lượng|100|N0");
            TradeUi.Grid(dgvBatchHistory, "CreatedAt|Thời gian|120|dd/MM/yyyy HH:mm", "ReferenceNumber|Chứng từ|180", "ProductName|Sản phẩm|180", "BatchNumber|Số lô|140", "ExpiryDate|Hạn dùng|100|dd/MM/yyyy", "Kind|Nghiệp vụ|120", "Quantity|Số lượng|80|N0");
            TradeUi.Grid(dgvBatches, "ProductName|Sản phẩm|220", "BatchNumber|Số lô|140", "ReceivedAt|Ngày nhập|110|dd/MM/yyyy", "ExpiryDate|Hạn dùng|110|dd/MM/yyyy", "RemainingQuantity|Tồn lô|90|N0", "Status|Trạng thái|130");
            TradeUi.Run(this, () => {
                var options = service.LoadOptions(actorId, TradeAction.Stock).Products;
                options.Insert(0, new ProductOption { Id = 0, Name = "Tất cả sản phẩm" });
                cboProduct.DataSource = options; cboProduct.DisplayMember = "Name"; cboProduct.ValueMember = "Id"; LoadReport();
            });
        }
        private void LoadReport()
        {
            var product = cboProduct.SelectedItem as ProductOption;
            var data = service.Stock(actorId, dtpFrom.Value, dtpTo.Value, product == null || product.Id == 0 ? (int?)null : product.Id,
                cboKind.SelectedIndex <= 0 ? (StockMovementKind?)null : (StockMovementKind)cboKind.SelectedIndex);
            dgvMovements.DataSource = data.Movements; dgvBalances.DataSource = data.Balances; dgvExpiring.DataSource = data.Expiring; dgvBatchHistory.DataSource = data.BatchHistory; dgvBatches.DataSource = data.Batches;
        }
    }
}
