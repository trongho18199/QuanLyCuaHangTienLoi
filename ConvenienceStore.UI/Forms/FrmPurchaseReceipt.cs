using System;
using System.ComponentModel;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;
using ConvenienceStore.Business.Models;
using ConvenienceStore.Business.Services;
using ConvenienceStore.UI.Helpers;
namespace ConvenienceStore.UI.Forms
{
    public partial class FrmPurchaseReceipt : DevExpress.XtraEditors.XtraForm
    {
        private readonly int actorId;
        private readonly TradeService service;
        private readonly BindingList<CartLine> cart = new BindingList<CartLine>();
        private List<ProductOption> products = new List<ProductOption>();
        private Guid operationId = Guid.NewGuid();
        private decimal total;
        public FrmPurchaseReceipt() : this(0) { }
        public FrmPurchaseReceipt(int currentUserId) : this(currentUserId, new TradeService()) { }
        public FrmPurchaseReceipt(int currentUserId, TradeService tradeService)
        {
            actorId = currentUserId; service = tradeService; InitializeComponent();
            btnReload.Click += (s, e) => TradeUi.Run(this, ReloadOptions);
            btnAdd.Click += (s, e) => TradeUi.Run(this, AddLine);
            btnRemove.Click += (s, e) => { if (dgvCart.CurrentRow != null) { cart.Remove((CartLine)dgvCart.CurrentRow.DataBoundItem); RefreshTotal(); } };
            btnNew.Click += (s, e) => { if (ConfirmClear()) ClearCart(); };
            btnSave.Click += Save_Click;

            FormClosing += (s, e) => { if (cart.Count > 0 && !ConfirmClear()) e.Cancel = true; };
        }
        private bool ConfirmClear()
        { return cart.Count == 0 || MessageBox.Show(this, "Bỏ các dòng chưa lưu của phiếu hiện tại?", "Xác nhận", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes; }
        private void Form_Load(object sender, EventArgs e)
        {
            if (TradeUi.IsDesigner(this)) return;
            TradeUi.Grid(dgvCart, "ProductName|Sản phẩm|190", "BatchNumber|Số lô|100", "ExpiryDate|Hạn dùng|90|dd/MM/yyyy", "Quantity|Số lượng|75|N0", "UnitPrice|Đơn giá nhập|100|N2", "LineTotal|Thành tiền|110|N2");
            dgvCart.DataSource = cart;
            TradeUi.Run(this, ReloadOptions); RefreshTotal();
        }
        private void ReloadOptions()
        {
            var data = service.LoadOptions(actorId, TradeAction.Purchase);
            products = data.Products;
            cboProduct.DataSource = products; cboProduct.DisplayMember = "Display"; cboProduct.ValueMember = "Id";
            var selected = cboSupplier.SelectedValue; cboSupplier.DataSource = data.Suppliers; cboSupplier.DisplayMember = "Display"; cboSupplier.ValueMember = "Id"; if (selected != null) cboSupplier.SelectedValue = selected;

            cart.ResetBindings(); RefreshTotal();
        }
        private void AddLine()
        {
            var product = cboProduct.SelectedItem as ProductOption;
            if (product == null) throw new TradeException("Chưa có sản phẩm. Nhờ bạn phụ trách danh mục tạo sản phẩm trước.");
            var line = cart.FirstOrDefault(x => x.ProductId == product.Id && string.Equals(x.BatchNumber, txtBatch.Text.Trim(), StringComparison.OrdinalIgnoreCase) && x.ExpiryDate == (dtpExpiry.Checked ? dtpExpiry.Value.Date : (DateTime?)null) && x.UnitPrice == numUnitPrice.Value);
            int quantity = (int)numQuantity.Value + (line == null ? 0 : line.Quantity);
            if (quantity > 1000000) throw new TradeException("Số lượng mỗi sản phẩm tối đa 1.000.000.");

            decimal price = numUnitPrice.Value;
            if (price <= 0m) throw new TradeException("Đơn giá phải lớn hơn 0.");
            string batch = txtBatch.Text.Trim();
            if (batch.Length == 0) throw new TradeException("Hãy nhập số lô.");
            if (dtpExpiry.Checked && dtpExpiry.Value.Date < DateTime.Today) throw new TradeException("Hạn sử dụng đã qua.");
            if (line == null) cart.Add(new CartLine { ProductId = product.Id, ProductName = product.Name, Quantity = (int)numQuantity.Value, UnitPrice = price, BatchNumber = batch, ExpiryDate = dtpExpiry.Checked ? dtpExpiry.Value.Date : (DateTime?)null });
            else { line.Quantity = quantity; }
            cart.ResetBindings(); RefreshTotal();
        }
        private void ClearCart()
        { cart.Clear(); operationId = Guid.NewGuid(); txtNote.Clear(); RefreshTotal(); }
        private void RefreshTotal()
        {
            TradeUi.Run(this, () => {
                decimal subtotal = Pricing.Money(cart.Sum(x => x.LineTotal));
                total = subtotal; lblTotal.Text = "Tổng tiền nhập: " + total.ToString("N2") + " đ";
            });
        }
        private void Save_Click(object sender, EventArgs e)
        {
            btnSave.Enabled = false;
            try
            {
                TradeUi.Run(this, () => {
                    var supplier = cboSupplier.SelectedItem as PartyOption; if (supplier == null) throw new TradeException("Hãy chọn nhà cung cấp.");
                    string paidText = CatalogUi.PromptText(this, "Thanh toán nhà cung cấp", "Tổng phiếu: " + total.ToString("N2") + " đ. Nhập số tiền đã trả ngay (0 nếu chưa trả):", 30);
                    if (paidText == null) return;
                    decimal paid;
                    if (!decimal.TryParse(paidText, System.Globalization.NumberStyles.Number, System.Globalization.CultureInfo.CurrentCulture, out paid) || paid < 0m || paid > total) throw new TradeException("Số tiền đã trả không hợp lệ.");
                    var result = service.Purchase(actorId, new PurchaseRequest { OperationId = operationId, SupplierId = supplier.Id, Note = txtNote.Text, Lines = cart.ToList(), InitialPaid = paid });
                    MessageBox.Show(this, (result.AlreadySaved ? "Giao dịch đã được lưu trước đó: " : "Đã lưu thành công: ") + result.Number + "\nTổng tiền: " + result.Total.ToString("N2") + " đ" + "", "Hoàn tất", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    ClearCart();
                    ReloadOptions();
                });
            }
            finally { btnSave.Enabled = true; }
        }
    }
}
