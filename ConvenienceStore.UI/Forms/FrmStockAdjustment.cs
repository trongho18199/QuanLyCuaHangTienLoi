using System;
using System.Collections.Generic;
using System.Drawing;
using System.Globalization;
using System.Linq;
using System.Windows.Forms;
using ConvenienceStore.Business.Models;
using ConvenienceStore.Business.Services;
using ConvenienceStore.UI.Helpers;
namespace ConvenienceStore.UI.Forms
{
    
    public partial class FrmStockAdjustment : DevExpress.XtraEditors.XtraForm
    {
        private static readonly CultureInfo Vn = CultureInfo.GetCultureInfo("vi-VN");
        private readonly int actorId; private readonly TradeService service;
        public FrmStockAdjustment() : this(0) { }
        public FrmStockAdjustment(int currentUserId) : this(currentUserId, new TradeService()) { }
        public FrmStockAdjustment(int currentUserId, TradeService tradeService)
        {
            actorId = currentUserId; service = tradeService; InitializeComponent();
            btnReload.Click += (s, e) => CatalogUi.Run(this, ReloadOptions);
            btnSave.Click += (s, e) => CatalogUi.Run(this, Save);
            cboProduct.SelectedIndexChanged += (s, e) => ShowProduct();
            nudNew.ValueChanged += (s, e) => ShowDifference();
        }
        private void Form_Load(object sender, EventArgs e)
        {
            if (CatalogUi.IsDesigner(this)) return;
            CatalogUi.Run(this, ReloadOptions);
        }
        private ProductOption Selected() { return cboProduct.SelectedItem as ProductOption; }
        private void ReloadOptions()
        {
            var keep = Selected();
            List<ProductOption> products = service.LoadOptions(actorId, TradeAction.Adjust).Products;
            cboProduct.DataSource = products; cboProduct.DisplayMember = "Display"; cboProduct.ValueMember = "Id";
            if (keep != null) cboProduct.SelectedValue = keep.Id;
            ShowProduct();
        }
        private void ShowProduct()
        {
            var product = Selected();
            if (product == null) { txtCurrent.Text = ""; nudNew.Value = 0; ShowDifference(); return; }
            txtCurrent.Text = product.Stock.ToString("N0", Vn);
            nudNew.Value = Math.Min(product.Stock, nudNew.Maximum);
            ShowDifference();
        }
        private void ShowDifference()
        {
            var product = Selected();
            if (product == null) { lblDiff.Text = ""; return; }
            decimal diff = nudNew.Value - product.Stock;
            lblDiff.Text = (diff > 0 ? "+" : "") + diff.ToString("N0", Vn);
            lblDiff.ForeColor = diff == 0 ? Color.DimGray : diff > 0 ? Color.FromArgb(17, 121, 114) : Color.Firebrick;
        }
        private void Save()
        {
            var product = Selected();
            if (product == null) throw new TradeException("Chưa có sản phẩm để điều chỉnh.");
            int newStock = (int)nudNew.Value;
            if (newStock == product.Stock) throw new TradeException("Tồn thực tế bằng tồn trên hệ thống, không cần điều chỉnh.");
            if (string.IsNullOrWhiteSpace(txtReason.Text)) { txtReason.Focus(); throw new TradeException("Hãy nhập lý do điều chỉnh."); }
            string question = "Điều chỉnh tồn kho \"" + product.Name + "\" từ " + product.Stock.ToString("N0", Vn) + " thành " +
                newStock.ToString("N0", Vn) + " (chênh lệch " + lblDiff.Text + ")?\r\n\r\nThao tác được ghi vào lịch sử kho và không tự hoàn tác.";
            if (MessageBox.Show(this, question, "Xác nhận điều chỉnh", MessageBoxButtons.YesNo, MessageBoxIcon.Question) != DialogResult.Yes) return;
            var result = service.AdjustStock(actorId, new StockAdjustRequest {
                ProductId = product.Id, ExpectedStock = product.Stock, NewStock = newStock, Reason = txtReason.Text });
            txtReason.Clear();
            ReloadOptions();
            MessageBox.Show(this, "Đã điều chỉnh tồn kho. Chứng từ: " + result.Number, "Thành công", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }
    }
}
