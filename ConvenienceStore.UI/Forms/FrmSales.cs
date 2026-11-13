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
    public partial class FrmSales : DevExpress.XtraEditors.XtraForm
    {
        private readonly int actorId;
        private readonly TradeService service;
        private readonly BindingList<CartLine> cart = new BindingList<CartLine>();
        private List<ProductOption> products = new List<ProductOption>();
        private Guid operationId = Guid.NewGuid();
        private decimal total;
        private PromotionOption promotion;
        private int? shiftId;
        private const string PromoHint = "Nhập mã rồi bấm Áp dụng (hoặc Enter). Khuyến mãi tính sau giảm giá VIP.";
        public FrmSales() : this(0) { }
        public FrmSales(int currentUserId) : this(currentUserId, new TradeService()) { }
        public FrmSales(int currentUserId, TradeService tradeService)
        {
            actorId = currentUserId; service = tradeService; InitializeComponent();
            btnReload.Click += (s, e) => TradeUi.Run(this, ReloadOptions);
            btnAdd.Click += (s, e) => TradeUi.Run(this, AddLine);
            btnRemove.Click += (s, e) => { if (dgvCart.CurrentRow != null) { cart.Remove((CartLine)dgvCart.CurrentRow.DataBoundItem); RefreshTotal(); } };
            btnNew.Click += (s, e) => { if (ConfirmClear()) ClearCart(); };
            btnSave.Click += Save_Click;
            btnApplyPromo.Click += (s, e) => TradeUi.Run(this, ApplyPromotion);
            btnClearPromo.Click += (s, e) => { ResetPromotion(true); RefreshTotal(); };
            txtPromo.KeyDown += (s, e) => { if (e.KeyCode == Keys.Enter) { e.SuppressKeyPress = true; TradeUi.Run(this, ApplyPromotion); } };
            
            txtPromo.TextChanged += (s, e) => { if (promotion != null && !string.Equals(txtPromo.Text.Trim(), promotion.Code, StringComparison.OrdinalIgnoreCase)) { ResetPromotion(false); RefreshTotal(); } };
            cboCustomer.SelectedIndexChanged += (s, e) => RefreshTotal(); numCash.ValueChanged += (s, e) => RefreshTotal();
            cboPayment.SelectedIndexChanged += (s, e) => { bool cash = (cboPayment.SelectedItem as string) == PaymentMethods.Cash; numCash.Enabled = cash; if (!cash) numCash.Value = total; RefreshTotal(); };
            FormClosing += (s, e) => { if (cart.Count > 0 && !ConfirmClear()) e.Cancel = true; };
        }
        private bool ConfirmClear()
        { return cart.Count == 0 || MessageBox.Show(this, "Bỏ các dòng chưa lưu của phiếu hiện tại?", "Xác nhận", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes; }
        private void Form_Load(object sender, EventArgs e)
        {
            if (TradeUi.IsDesigner(this)) return;
            TradeUi.Grid(dgvCart, "ProductName|Sản phẩm|220", "Quantity|Số lượng|75|N0", "UnitPrice|Đơn giá bán|100|N2", "LineTotal|Thành tiền|110|N2");
            dgvCart.DataSource = cart;
            cboPayment.DataSource = PaymentMethods.All.ToList(); cboPayment.SelectedIndex = 0;
            var shift = service.CurrentShift(actorId);
            shiftId = shift == null ? (int?)null : shift.Id;
            lblSubtitle.Text = shift == null ? "Chưa mở ca bán hàng - hãy mở ca trước khi thanh toán" : "Ca " + shift.Number + "  /  Chọn hàng  /  Ưu đãi  /  Thanh toán";
            TradeUi.Run(this, ReloadOptions); RefreshTotal();
        }
        private void ReloadOptions()
        {
            var data = service.LoadOptions(actorId, TradeAction.Sale);
            products = data.Products;
            cboProduct.DataSource = products; cboProduct.DisplayMember = "Display"; cboProduct.ValueMember = "Id";
            var selected = cboCustomer.SelectedValue; cboCustomer.DataSource = data.Customers; cboCustomer.DisplayMember = "Display"; cboCustomer.ValueMember = "Id"; if (selected != null) cboCustomer.SelectedValue = selected;
            cart.Join(products, line => line.ProductId, p => p.Id, (line, p) => new { line, p }).ToList()
                .ForEach(x => { x.line.UnitPrice = x.p.Price; x.line.ProductName = x.p.Name; });
            cart.ResetBindings(); RefreshTotal();
        }
        private void AddLine()
        {
            var product = cboProduct.SelectedItem as ProductOption;
            if (product == null) throw new TradeException("Chưa có sản phẩm. Nhờ bạn phụ trách danh mục tạo sản phẩm trước.");
            var line = cart.FirstOrDefault(x => x.ProductId == product.Id);
            int quantity = (int)numQuantity.Value + (line == null ? 0 : line.Quantity);
            if (quantity > 1000000) throw new TradeException("Số lượng mỗi sản phẩm tối đa 1.000.000.");
            if (quantity > product.Stock) throw new TradeException(product.Name + ": tồn kho chỉ còn " + product.Stock + ".");
            decimal price = product.Price;
            if (price <= 0m) throw new TradeException("Đơn giá phải lớn hơn 0.");
            if (line == null) cart.Add(new CartLine { ProductId = product.Id, ProductName = product.Name, Quantity = quantity, UnitPrice = price });
            else { line.Quantity = quantity; line.UnitPrice = price; }
            cart.ResetBindings(); RefreshTotal();
        }
        private void ApplyPromotion()
        {
            var found = service.FindPromotion(actorId, txtPromo.Text);
            txtPromo.Text = found.Code; promotion = found;
            lblPromoStatus.Text = "Đã áp dụng " + found.Code + ": giảm " + found.DiscountPercent.ToString("0.##") + "%" + (string.IsNullOrEmpty(found.Description) ? "" : " - " + found.Description);
            RefreshTotal();
        }
        private void ResetPromotion(bool clearText)
        {
            promotion = null;
            if (clearText) txtPromo.Clear();
            lblPromoStatus.Text = PromoHint;
        }
        private void ClearCart()
        { cart.Clear(); operationId = Guid.NewGuid(); numCash.Value = 0m; ResetPromotion(true); RefreshTotal(); }
        private void RefreshTotal()
        {
            TradeUi.Run(this, () => {
                decimal subtotal = Pricing.Money(cart.Sum(x => x.LineTotal));
                var customer = cboCustomer.SelectedItem as PartyOption; decimal discount = Pricing.Discount(subtotal, customer != null && customer.IsVip);
                decimal promoDiscount = promotion == null ? 0m : Pricing.PromotionDiscount(subtotal - discount, promotion.DiscountPercent);
                total = Pricing.Money(subtotal - discount - promoDiscount); lblSubtotal.Text = "Tiền hàng: " + subtotal.ToString("N2") + " đ"; lblDiscount.Text = "Giảm giá VIP: " + discount.ToString("N2") + " đ"; lblPromoDiscount.Text = (promotion == null ? "Khuyến mãi" : "Khuyến mãi " + promotion.Code) + ": " + promoDiscount.ToString("N2") + " đ"; lblTotal.Text = "Phải trả: " + total.ToString("N2") + " đ"; lblChange.Text = numCash.Value >= total ? "Tiền thừa: " + (numCash.Value - total).ToString("N2") + " đ" : "Còn thiếu: " + (total - numCash.Value).ToString("N2") + " đ";
            });
        }
        private void Save_Click(object sender, EventArgs e)
        {
            btnSave.Enabled = false;
            try
            {
                string paymentMethod = cboPayment.SelectedItem as string;
                string paymentReference = null;
                if (paymentMethod == PaymentMethods.Transfer)
                {
                    if (cart.Count == 0 || total <= 0m)
                    {
                        MessageBox.Show(this, "Hãy thêm sản phẩm trước khi tạo mã chuyển khoản.", "Chưa có hàng");
                        return;
                    }
                    string reference = VietQrPayment.Reference(operationId);
                    paymentReference = reference;
                    string qrUrl;
                    try { qrUrl = VietQrPayment.CreateImageUrl(total, reference); }
                    catch (Exception ex)
                    {
                        MessageBox.Show(this, ex.Message, "Không tạo được VietQR", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                        return;
                    }
                    using (var qr = new FrmQrPayment(qrUrl, reference, total))
                        if (qr.ShowDialog(this) != DialogResult.OK) return;
                }
                TradeUi.Run(this, () => {
                    var customer = cboCustomer.SelectedItem as PartyOption;
                    var activeShift = service.CurrentShift(actorId);
                    shiftId = activeShift == null ? (int?)null : activeShift.Id;
                    var result = service.Checkout(actorId, new SaleRequest { OperationId = operationId, ShiftId = shiftId, CustomerId = customer == null || customer.Id == 0 ? (int?)null : customer.Id, Lines = cart.ToList(), ExpectedTotal = total, CashReceived = numCash.Value, PromotionCode = promotion == null ? null : promotion.Code, PaymentMethod = paymentMethod });
                    MessageBox.Show(this, (result.AlreadySaved ? "Giao dịch đã được lưu trước đó: " : "Đã lưu thành công: ") + result.Number + "\nTổng tiền: " + result.Total.ToString("N2") + " đ" + "\nPhương thức: " + paymentMethod + (paymentReference == null ? "" : "\nMã chuyển khoản: " + paymentReference) + "\nTiền thừa: " + result.Change.ToString("N2") + " đ", "Hoàn tất", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    ClearCart();
                    ReloadOptions();
                });
            }
            finally { btnSave.Enabled = true; }
        }
    }
}
