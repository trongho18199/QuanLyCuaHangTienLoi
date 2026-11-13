using System;
using System.Collections.Generic;
using System.Linq;
using ConvenienceStore.Business.Models;
using ConvenienceStore.Data;
using ConvenienceStore.Data.Repositories;
namespace ConvenienceStore.Business.Services
{
    public sealed class TradeService
    {
        private readonly Func<ITradeSession> createSession;
        public TradeService() : this(() => new LinqTradeSession()) { }
        public TradeService(Func<ITradeSession> sessionFactory)
        { createSession = sessionFactory ?? throw new ArgumentNullException("sessionFactory"); }

        
        private static readonly Dictionary<TradeAction, string> MenuCode = new Dictionary<TradeAction, string>
        {
            { TradeAction.Purchase, "mnuPurchaseReceipt" },
            { TradeAction.Sale,     "mnuSales" },
            { TradeAction.Stock,    "mnuStockMovement" },
            { TradeAction.Revenue,  "mnuRevenueReport" },
            { TradeAction.Cancel,   "fnInvoiceCancel" },
            { TradeAction.Adjust,   "mnuStockAdjust" },
            { TradeAction.Return,   "fnSaleReturn" },
            { TradeAction.Shift,    "mnuShift" },
            { TradeAction.SupplierDebt, "mnuSupplierDebt" },
        };


        private static void Require(ITradeSession db, int actorId, TradeAction action)
        {
            var user = db.Users.Where(x => x.Id == actorId && x.IsActive)
                .Select(x => new { x.RoleId, RoleName = x.Role.Name }).SingleOrDefault();
            string code = MenuCode[action];
            bool ok = user != null &&
                (user.RoleName == RoleRepository.AdminRoleName ||
                 db.RolePermissions.Any(p => p.RoleId == user.RoleId && p.FunctionCode == code));
            if (!ok)
                throw new TradeException("Tài khoản chưa đăng nhập, bị khóa hoặc không có quyền thực hiện chức năng này.");
        }
        private static List<CartLine> ValidateLines(List<CartLine> lines, bool allowRepeatedProducts = false)
        {
            if (lines == null || lines.Count == 0) throw new TradeException("Chưa có sản phẩm trong phiếu.");
            if (lines.Count > 500) throw new TradeException("Mỗi phiếu tối đa 500 dòng sản phẩm.");
            if (lines.Any(x => x == null || x.ProductId <= 0 || x.Quantity < 1 || x.Quantity > 1000000))
                throw new TradeException("Sản phẩm hoặc số lượng không hợp lệ (1 đến 1.000.000).");
            if (!allowRepeatedProducts && lines.Select(x => x.ProductId).Distinct().Count() != lines.Count)
                throw new TradeException("Một sản phẩm chỉ được có một dòng; hãy gộp số lượng trước khi lưu.");
            return lines.OrderBy(x => x.ProductId).ToList(); 
        }
        private static DateTime EndExclusive(DateTime from, DateTime to)
        {
            if (from.Date > to.Date || to.Date == DateTime.MaxValue.Date)
                throw new TradeException("Khoảng ngày không hợp lệ.");
            return to.Date.AddDays(1);
        }
        
        private static Promotion ResolvePromotion(ITradeSession db, string rawCode)
        {
            string code = (rawCode ?? "").Trim();
            if (code.Length == 0) throw new TradeException("Hãy nhập mã khuyến mãi.");
            var promo = code.Length > 30 ? null : db.Promotions.SingleOrDefault(x => x.Code == code);
            if (promo == null) throw new TradeException("Mã khuyến mãi \"" + code + "\" không tồn tại.");
            if (!promo.IsActive) throw new TradeException("Mã khuyến mãi " + promo.Code + " đã ngừng áp dụng.");
            if (promo.DiscountPercent <= 0m || promo.DiscountPercent > 100m)
                throw new TradeException("Mã khuyến mãi " + promo.Code + " đang cấu hình sai mức giảm. Hãy báo quản lý.");
            DateTime today = DateTime.Today;
            if (promo.ValidFrom.HasValue && today < promo.ValidFrom.Value.Date)
                throw new TradeException("Mã khuyến mãi " + promo.Code + " chưa đến ngày áp dụng (từ " + promo.ValidFrom.Value.ToString("dd/MM/yyyy") + ").");
            if (promo.ValidTo.HasValue && today > promo.ValidTo.Value.Date)
                throw new TradeException("Mã khuyến mãi " + promo.Code + " đã hết hạn (đến " + promo.ValidTo.Value.ToString("dd/MM/yyyy") + ").");
            return promo;
        }
        public PromotionOption FindPromotion(int actorId, string code)
        {
            using (var db = createSession())
            {
                Require(db, actorId, TradeAction.Sale);
                var promo = ResolvePromotion(db, code);
                return new PromotionOption { Code = promo.Code, DiscountPercent = promo.DiscountPercent, Description = promo.Description };
            }
        }

        public LookupData LoadOptions(int actorId, TradeAction action)
        {
            using (var db = createSession())
            {
                Require(db, actorId, action);
                var result = new LookupData {
                    Products = db.Products.OrderBy(x => x.Name).Select(x => new ProductOption {
                        Id = x.Id, Name = x.Name, Barcode = x.Barcode, Price = x.Price, Stock = x.Stock }).ToList(),
                    Suppliers = db.Suppliers.Where(x => x.IsActive).OrderBy(x => x.Name)
                        .Select(x => new PartyOption { Id = x.Id, Name = x.Name }).ToList(),
                    Customers = db.Customers.Where(x => x.IsActive).OrderBy(x => x.Name)
                        .Select(x => new PartyOption { Id = x.Id, Name = x.Name, IsVip = x.IsVip }).ToList()
                };
                result.Customers.Insert(0, new PartyOption { Id = 0, Name = "Khách lẻ" });
                return result;
            }
        }

        public DocumentResult Purchase(int actorId, PurchaseRequest request)
        {
            if (request == null || request.OperationId == Guid.Empty) throw new TradeException("Thiếu mã thao tác lưu phiếu.");
            var lines = ValidateLines(request.Lines, true);
            if ((request.Note ?? "").Length > 300) throw new TradeException("Ghi chú tối đa 300 ký tự.");
            using (var db = createSession())
            using (var tx = db.BeginTransaction())
            {
                Require(db, actorId, TradeAction.Purchase);
                var saved = db.Purchases.SingleOrDefault(x => x.OperationId == request.OperationId);
                if (saved != null) return new DocumentResult { Id = saved.Id, Number = saved.Number, Total = saved.Total, AlreadySaved = true };
                var supplier = db.Suppliers.SingleOrDefault(x => x.Id == request.SupplierId && x.IsActive);
                if (supplier == null) throw new TradeException("Hãy chọn nhà cung cấp đang hoạt động.");
                var ids = lines.Select(x => x.ProductId).Distinct().ToList();
                var products = db.Products.Where(x => ids.Contains(x.Id)).ToDictionary(x => x.Id);
                if (products.Count != ids.Count) throw new TradeException("Có sản phẩm đã bị xóa. Hãy tải lại danh sách.");
                var receipt = new PurchaseReceipt { OperationId = request.OperationId,
                    Number = "PN-" + DateTime.Now.ToString("yyyyMMddHHmmss") + "-" + request.OperationId.ToString("N").Substring(0, 8),
                    CreatedAt = DateTime.Now, SupplierId = supplier.Id, SupplierName = supplier.Name,
                    CreatedById = actorId, Note = (request.Note ?? "").Trim() };
                
                var runningStock = products.ToDictionary(x => x.Key, x => x.Value.Stock);
                var items = lines.Select(line => {
                    var product = products[line.ProductId];
                    string batch = string.IsNullOrWhiteSpace(line.BatchNumber) ? "KHONG_LO" : line.BatchNumber.Trim();
                    if (batch.Length > 50) throw new TradeException("Số lô tối đa 50 ký tự.");
                    if (line.ExpiryDate.HasValue && line.ExpiryDate.Value.Date < receipt.CreatedAt.Date) throw new TradeException("Hạn sử dụng không được trước ngày nhập.");
                    if (line.UnitPrice <= 0m) throw new TradeException("Đơn giá nhập phải lớn hơn 0.");
                    var cost = Pricing.Money(line.UnitPrice);
                    if (cost != line.UnitPrice) throw new TradeException("Đơn giá nhập chỉ có tối đa 2 chữ số thập phân.");
                    int before = runningStock[product.Id];
                    if (before < 0 || before > int.MaxValue - line.Quantity) throw new TradeException("Số tồn kho không hợp lệ hoặc vượt giới hạn.");
                    runningStock[product.Id] = before + line.Quantity;
                    return new { Line = line, Product = product, Batch = batch, Before = before,
                        Detail = new PurchaseReceiptLine { ProductId = product.Id, ProductName = product.Name,
                            Quantity = line.Quantity, UnitCost = cost, LineTotal = Pricing.Money(cost * line.Quantity), BatchNumber = batch, ExpiryDate = line.ExpiryDate } };
                }).ToList();
                receipt.Lines.AddRange(items.Select(x => x.Detail));
                receipt.Total = Pricing.Money(receipt.Lines.Sum(x => x.LineTotal));
                if (request.InitialPaid.HasValue && (request.InitialPaid.Value < 0 || request.InitialPaid.Value > receipt.Total || Pricing.Money(request.InitialPaid.Value) != request.InitialPaid.Value))
                    throw new TradeException("Số tiền đã trả phải từ 0 đến tổng phiếu và tối đa hai chữ số thập phân.");
                var movements = items.Select(x => new StockMovement { ProductId = x.Product.Id, ProductName = x.Product.Name,
                    CreatedAt = receipt.CreatedAt, Kind = StockMovementKind.Purchase, Quantity = x.Line.Quantity,
                    StockBefore = x.Before, StockAfter = x.Before + x.Line.Quantity, ReferenceNumber = receipt.Number,
                    CreatedById = actorId, PurchaseReceipt = receipt }).ToList();
                movements.ForEach(x => db.Add(x));
                db.Add(receipt);
                db.SaveChanges();
                if (request.InitialPaid.HasValue) db.Add(new SupplierPayable { PurchaseReceiptId = receipt.Id, OpeningPaid = request.InitialPaid.Value, CreatedAt = receipt.CreatedAt, CreatedById = actorId });
                items.ForEach(x => {
                    ReconcileBatches(db, x.Product);
                    var batch = new ProductBatch { ProductId = x.Product.Id, BatchNumber = x.Batch, ExpiryDate = x.Line.ExpiryDate, ReceivedAt = receipt.CreatedAt, Quantity = x.Line.Quantity, RemainingQuantity = x.Line.Quantity, PurchaseReceiptId = receipt.Id, UnitCost = x.Line.UnitPrice };
                    db.Add(batch); db.SaveChanges();
                    LogBatch(db, batch, x.Line.Quantity, receipt.Number, "Nhập", actorId, null);
                    x.Product.Stock += x.Line.Quantity;
                });
                db.SaveChanges();
                tx.Commit();
                return new DocumentResult { Id = receipt.Id, Number = receipt.Number, Total = receipt.Total };
            }
        }

        public DocumentResult Checkout(int actorId, SaleRequest request)
        {
            if (request == null || request.OperationId == Guid.Empty) throw new TradeException("Thiếu mã thao tác thanh toán.");
            var lines = ValidateLines(request.Lines);
            using (var db = createSession())
            using (var tx = db.BeginTransaction())
            {
                Require(db, actorId, TradeAction.Sale);
                var shift = db.Shifts.SingleOrDefault(x => x.Id == request.ShiftId && x.OpenedById == actorId && x.Status == "Đang mở");
                if (shift == null) throw new TradeException("Bạn chưa mở ca bán hàng. Hãy mở ca trước khi thanh toán.");
                var saved = db.Sales.SingleOrDefault(x => x.OperationId == request.OperationId);
                if (saved != null) return new DocumentResult { Id = saved.Id, Number = saved.Number, Total = saved.Total, Change = saved.Change, AlreadySaved = true };
                Customer customer = null;
                if (request.CustomerId.HasValue)
                {
                    customer = db.Customers.SingleOrDefault(x => x.Id == request.CustomerId.Value && x.IsActive);
                    if (customer == null) throw new TradeException("Khách hàng không còn hoạt động. Hãy tải lại danh sách.");
                }
                var ids = lines.Select(x => x.ProductId).ToList();
                var products = db.Products.Where(x => ids.Contains(x.Id)).ToDictionary(x => x.Id);
                if (products.Count != lines.Count) throw new TradeException("Có sản phẩm đã bị xóa. Hãy tải lại giỏ hàng.");
                Promotion promotion = string.IsNullOrWhiteSpace(request.PromotionCode) ? null : ResolvePromotion(db, request.PromotionCode);
                string paymentMethod = string.IsNullOrWhiteSpace(request.PaymentMethod) ? PaymentMethods.Cash : request.PaymentMethod.Trim();
                if (!PaymentMethods.All.Contains(paymentMethod)) throw new TradeException("Phương thức thanh toán không hợp lệ.");
                if (paymentMethod != PaymentMethods.Cash && request.CashReceived != request.ExpectedTotal)
                    throw new TradeException("Với thanh toán không dùng tiền mặt, số tiền khách đưa phải đúng bằng tổng tiền.");
                var invoice = new SaleInvoice { OperationId = request.OperationId,
                    Number = "HD-" + DateTime.Now.ToString("yyyyMMddHHmmss") + "-" + request.OperationId.ToString("N").Substring(0, 8),
                    CreatedAt = DateTime.Now, CustomerId = request.CustomerId, CustomerName = customer == null ? "Khách lẻ" : customer.Name,
                    CreatedById = actorId, DiscountPercent = customer != null && customer.IsVip ? Pricing.VipPercent : 0m,
                    PaymentMethod = paymentMethod, ShiftId = shift.Id };
                
                var details = lines.Select(line => {
                    var product = products[line.ProductId];
                    if (product.Stock < line.Quantity) throw new TradeException(product.Name + ": chỉ còn " + product.Stock + " sản phẩm.");
                    if (product.Price <= 0m) throw new TradeException(product.Name + ": chưa có giá bán hợp lệ.");
                    return new SaleInvoiceLine { ProductId = product.Id, ProductName = product.Name,
                        Quantity = line.Quantity, UnitPrice = Pricing.Money(product.Price), LineTotal = Pricing.Money(product.Price * line.Quantity) };
                }).ToList();
                invoice.Lines.AddRange(details);
                invoice.Subtotal = Pricing.Money(details.Sum(x => x.LineTotal));
                decimal memberDiscount = Pricing.Discount(invoice.Subtotal, invoice.DiscountPercent > 0m);
                
                invoice.PromotionCode = promotion == null ? null : promotion.Code;
                invoice.PromotionAmount = promotion == null ? 0m : Pricing.PromotionDiscount(invoice.Subtotal - memberDiscount, promotion.DiscountPercent);
                invoice.DiscountAmount = Pricing.Money(memberDiscount + invoice.PromotionAmount);
                invoice.Total = Pricing.Money(invoice.Subtotal - invoice.DiscountAmount);
                if (request.ExpectedTotal != invoice.Total) throw new TradeException("Giá bán, hạng thành viên hoặc khuyến mãi đã thay đổi. Hãy tải lại giỏ hàng rồi xác nhận số tiền mới.");
                invoice.CashReceived = Pricing.Money(request.CashReceived);
                if (request.CashReceived != invoice.CashReceived || invoice.CashReceived < invoice.Total)
                    throw new TradeException("Tiền khách đưa chưa đủ hoặc có quá 2 chữ số thập phân.");
                invoice.Change = Pricing.Money(invoice.CashReceived - invoice.Total);
                
                var movements = lines.Select(line => {
                    var product = products[line.ProductId];
                    return new StockMovement { ProductId = product.Id, ProductName = product.Name,
                        CreatedAt = invoice.CreatedAt, Kind = StockMovementKind.Sale, Quantity = -line.Quantity,
                        StockBefore = product.Stock, StockAfter = product.Stock - line.Quantity, ReferenceNumber = invoice.Number,
                        CreatedById = actorId, SaleInvoice = invoice };
                }).ToList();
                db.Add(invoice);
                db.SaveChanges();
                foreach (var line in lines)
                {
                    var detail = details.Single(x => x.ProductId == line.ProductId);
                    RemoveBatches(db, products[line.ProductId], line.Quantity, invoice.Number, "Bán", actorId, detail.Id);
                    products[line.ProductId].Stock -= line.Quantity;
                }
                movements.ForEach(x => db.Add(x));
                db.SaveChanges();
                tx.Commit();
                return new DocumentResult { Id = invoice.Id, Number = invoice.Number, Total = invoice.Total, Change = invoice.Change };
            }
        }

        private static List<ProductBatch> ReconcileBatches(ITradeSession db, Product product)
        {
            var batches = db.Batches.Where(x => x.ProductId == product.Id).OrderBy(x => x.Id).ToList();
            int known = batches.Sum(x => x.RemainingQuantity);
            if (known > product.Stock || known < 0) throw new TradeException(product.Name + ": tồn theo lô vượt tồn tổng. Cần kiểm kê và xử lý dữ liệu trước.");
            if (known < product.Stock)
            {
                int missing = product.Stock - known;
                var legacy = new ProductBatch { ProductId = product.Id, BatchNumber = "TỒN KHÔNG RÕ LÔ", ReceivedAt = DateTime.Now, Quantity = missing, RemainingQuantity = missing };
                db.Add(legacy); db.SaveChanges(); batches.Add(legacy);
            }
            return batches;
        }
        private static void LogBatch(ITradeSession db, ProductBatch batch, int quantity, string reference, string kind, int actorId, int? lineId)
        {
            db.Add(new BatchMovement { BatchId = batch.Id, ProductId = batch.ProductId, SaleInvoiceLineId = lineId, CreatedAt = DateTime.Now, ReferenceNumber = reference, Kind = kind, Quantity = quantity, CreatedById = actorId, UnitCost = batch.UnitCost });
        }
        private static void RemoveBatches(ITradeSession db, Product product, int quantity, string reference, string kind, int actorId, int? lineId)
        {
            bool sale = kind == "Bán" || kind == "Xuất hàng đổi";
            var batches = ReconcileBatches(db, product).Where(x => x.RemainingQuantity > 0 && (!sale || !x.ExpiryDate.HasValue || x.ExpiryDate.Value.Date >= DateTime.Today))
                .OrderBy(x => x.ExpiryDate.HasValue ? 0 : 1).ThenBy(x => x.ExpiryDate).ThenBy(x => x.Id).ToList();
            if (batches.Sum(x => (long)x.RemainingQuantity) < quantity)
                throw new TradeException(product.Name + (sale ? ": không đủ hàng còn hạn sử dụng để xuất bán." : ": không đủ tồn theo lô."));
            foreach (var b in batches)
            {
                int take = Math.Min(quantity, b.RemainingQuantity);
                b.RemainingQuantity -= take; quantity -= take;
                LogBatch(db, b, -take, reference, kind, actorId, lineId);
                if (quantity == 0) break;
            }
            if (quantity != 0) throw new TradeException("Không đủ tồn theo lô.");
        }
        private static void RestoreBatches(ITradeSession db, SaleInvoiceLine line, int quantity, string reference, string kind, int actorId)
        {
            var history = db.BatchMovements.Where(x => x.SaleInvoiceLineId == line.Id).OrderBy(x => x.Id).ToList();
            var groups = history.GroupBy(x => x.BatchId).Select(g => new { Id = g.Key, Remaining = -g.Sum(x => x.Quantity) }).Where(x => x.Remaining > 0).ToList();
            foreach (var group in groups)
            {
                var batch = db.Batches.Single(x => x.Id == group.Id);
                int take = Math.Min(quantity, group.Remaining);
                batch.RemainingQuantity = checked(batch.RemainingQuantity + take);
                LogBatch(db, batch, take, reference, kind, actorId, line.Id);
                quantity -= take;
                if (quantity == 0) break;
            }
            if (quantity > 0)
            {
                
                if (history.Count != 0) throw new TradeException("Lịch sử xuất lô không đủ để hoàn hàng; cần kiểm tra dữ liệu.");
                var unknown = new ProductBatch { ProductId = line.ProductId, BatchNumber = "HÀNG TRẢ KHÔNG RÕ LÔ", ReceivedAt = DateTime.Now, Quantity = quantity, RemainingQuantity = quantity };
                db.Add(unknown); db.SaveChanges(); LogBatch(db, unknown, -line.Quantity, reference, "Xuất cũ không rõ lô", actorId, line.Id); LogBatch(db, unknown, quantity, reference, kind, actorId, line.Id);
            }
        }
        private static string KindName(StockMovementKind kind)
        {
            switch (kind)
            {
                case StockMovementKind.Purchase: return "Nhập hàng";
                case StockMovementKind.Sale: return "Bán hàng";
                case StockMovementKind.SaleCancel: return "Hủy hóa đơn";
                case StockMovementKind.Adjustment: return "Điều chỉnh";
                case StockMovementKind.Return: return "Đổi/trả hàng";
                default: return kind.ToString();
            }
        }
        private static string Reason(string value, string label)
        {
            string v = (value ?? "").Trim();
            if (v.Length == 0) throw new TradeException("Hãy nhập " + label + ".");
            if (v.Length > 200) throw new TradeException(char.ToUpper(label[0]) + label.Substring(1) + " tối đa 200 ký tự.");
            return v;
        }

        
        
        
        
        public DocumentResult CancelInvoice(int actorId, int invoiceId, string reason)
        {
            string why = Reason(reason, "lý do hủy hóa đơn");
            using (var db = createSession())
            using (var tx = db.BeginTransaction())
            {
                Require(db, actorId, TradeAction.Cancel);
                var invoice = db.Sales.SingleOrDefault(x => x.Id == invoiceId);
                if (invoice == null) throw new TradeException("Không tìm thấy hóa đơn. Hãy tải lại danh sách.");
                if (invoice.IsCancelled)
                    throw new TradeException("Hóa đơn " + invoice.Number + " đã được hủy" +
                        (invoice.CancelledAt.HasValue ? " lúc " + invoice.CancelledAt.Value.ToString("dd/MM/yyyy HH:mm") : "") + ".");
                if (db.Returns.Any(x => x.SaleInvoiceId == invoice.Id))
                    throw new TradeException("Hóa đơn đã phát sinh đổi/trả; không thể hủy toàn bộ vì sẽ hoàn kho trùng.");
                if (invoice.PaymentMethod == PaymentMethods.Cash)
                    RecordCash(db, actorId, invoice.Number, "Hoàn tiền hủy hóa đơn", -invoice.Total);
                var lines = invoice.Lines.OrderBy(x => x.ProductId).ToList();
                var ids = lines.Select(x => x.ProductId).ToList();
                var products = db.Products.Where(x => ids.Contains(x.Id)).ToDictionary(x => x.Id);
                if (products.Count != lines.Count) throw new TradeException("Có sản phẩm của hóa đơn đã bị xóa khỏi danh mục nên không thể hoàn kho.");
                DateTime now = DateTime.Now;
                
                var movements = lines.Select(line => {
                    var product = products[line.ProductId];
                    if (product.Stock > int.MaxValue - line.Quantity) throw new TradeException(product.Name + ": tồn kho vượt giới hạn khi hoàn lại.");
                    return new StockMovement { ProductId = product.Id, ProductName = product.Name, CreatedAt = now,
                        Kind = StockMovementKind.SaleCancel, Quantity = line.Quantity, StockBefore = product.Stock,
                        StockAfter = product.Stock + line.Quantity, ReferenceNumber = invoice.Number, CreatedById = actorId,
                        SaleInvoice = invoice, Note = "Hủy hóa đơn: " + why };
                }).ToList();
                foreach (var line in lines) { RestoreBatches(db, line, line.Quantity, invoice.Number, "Hủy", actorId); products[line.ProductId].Stock += line.Quantity; }
                invoice.IsCancelled = true; invoice.CancelledAt = now; invoice.CancelledById = actorId; invoice.CancelReason = why;
                movements.ForEach(x => db.Add(x));
                db.SaveChanges();
                tx.Commit();
                return new DocumentResult { Id = invoice.Id, Number = invoice.Number, Total = invoice.Total };
            }
        }

        
        
        
        
        public DocumentResult AdjustStock(int actorId, StockAdjustRequest request)
        {
            if (request == null || request.ProductId <= 0) throw new TradeException("Hãy chọn sản phẩm cần điều chỉnh.");
            string why = Reason(request.Reason, "lý do điều chỉnh");
            if (request.NewStock < 0 || request.NewStock > 1000000000)
                throw new TradeException("Tồn thực tế phải từ 0 đến 1.000.000.000.");
            using (var db = createSession())
            using (var tx = db.BeginTransaction())
            {
                Require(db, actorId, TradeAction.Adjust);
                var product = db.Products.SingleOrDefault(x => x.Id == request.ProductId);
                if (product == null) throw new TradeException("Sản phẩm không còn tồn tại. Hãy tải lại danh sách.");
                if (product.Stock != request.ExpectedStock)
                    throw new TradeException(product.Name + ": tồn kho đã đổi từ " + request.ExpectedStock + " thành " + product.Stock +
                        " kể từ lúc bạn mở màn hình (có thể vừa có hàng nhập/bán). Hãy tải lại rồi kiểm đếm lại.");
                int difference = request.NewStock - product.Stock;
                if (difference == 0) throw new TradeException("Tồn thực tế bằng tồn trên hệ thống, không cần điều chỉnh.");
                DateTime now = DateTime.Now;
                string number = "DC-" + now.ToString("yyyyMMddHHmmss") + "-" + Guid.NewGuid().ToString("N").Substring(0, 8);
                if (difference < 0) RemoveBatches(db, product, -difference, number, "Kiểm kê giảm", actorId, null);
                else
                {
                    ReconcileBatches(db, product);
                    var batch = new ProductBatch { ProductId = product.Id, BatchNumber = "KIỂM KÊ KHÔNG RÕ LÔ", ReceivedAt = now, Quantity = difference, RemainingQuantity = difference };
                    db.Add(batch); db.SaveChanges(); LogBatch(db, batch, difference, number, "Kiểm kê tăng", actorId, null);
                }
                db.Add(new StockMovement { ProductId = product.Id, ProductName = product.Name, CreatedAt = now,
                    Kind = StockMovementKind.Adjustment, Quantity = difference, StockBefore = product.Stock,
                    StockAfter = request.NewStock, ReferenceNumber = number, CreatedById = actorId, Note = why });
                product.Stock = request.NewStock;
                db.SaveChanges();
                tx.Commit();
                return new DocumentResult { Id = product.Id, Number = number };
            }
        }

        public DocumentResult ReturnSale(int actorId, ReturnRequest request)
        {
            if (request == null || request.InvoiceId <= 0) throw new TradeException("Hãy chọn hóa đơn cần đổi/trả.");
            if (request.Lines == null || request.Lines.Count == 0) throw new TradeException("Hãy chọn ít nhất một dòng hàng.");
            string reason = Reason(request.Reason, "lý do đổi/trả hàng");
            string kind = string.Equals(request.ReturnKind, "Đổi hàng", StringComparison.OrdinalIgnoreCase) ? "Đổi hàng" : "Trả hàng";
            using (var db = createSession()) using (var tx = db.BeginTransaction())
            {
                Require(db, actorId, TradeAction.Return);
                var invoice = db.Sales.SingleOrDefault(x => x.Id == request.InvoiceId);
                if (invoice == null) throw new TradeException("Không tìm thấy hóa đơn.");
                if (invoice.IsCancelled) throw new TradeException("Không thể đổi/trả hóa đơn đã hủy.");
                var requested = request.Lines.GroupBy(x => x.SaleInvoiceLineId).Select(g => new { Id = g.Key, Qty = g.Sum(x => x.Quantity) }).ToList();
                if (requested.Any(x => x.Id <= 0 || x.Qty < 1 || x.Qty > 1000000)) throw new TradeException("Số lượng đổi/trả không hợp lệ.");
                var invoiceLines = db.SaleLines.Where(x => x.SaleInvoiceId == invoice.Id && requested.Select(r => r.Id).Contains(x.Id)).ToList();
                if (invoiceLines.Count != requested.Count) throw new TradeException("Có dòng hàng không thuộc hóa đơn đã chọn.");
                var returned = db.ReturnLines.Where(x => requested.Select(r => r.Id).Contains(x.SaleInvoiceLineId)).GroupBy(x => x.SaleInvoiceLineId)
                    .Select(g => new { Id = g.Key, Qty = g.Sum(x => x.Quantity) }).ToDictionary(x => x.Id, x => x.Qty);
                var items = requested.Select(r => {
                    var line = invoiceLines.Single(x => x.Id == r.Id);
                    int already = returned.ContainsKey(r.Id) ? returned[r.Id] : 0;
                    if (already + r.Qty > line.Quantity) throw new TradeException(line.ProductName + ": số lượng đổi/trả vượt số đã mua.");
                    var product = db.Products.SingleOrDefault(x => x.Id == line.ProductId);
                    if (product == null) throw new TradeException("Sản phẩm " + line.ProductName + " không còn trong danh mục.");
                    if (product.Stock > int.MaxValue - r.Qty) throw new TradeException("Tồn kho vượt giới hạn khi nhận hàng trả.");
                    decimal paidUnit = invoice.Subtotal <= 0m ? line.UnitPrice : Pricing.Money(line.UnitPrice * invoice.Total / invoice.Subtotal);
                    return new { Line = line, Product = product, Qty = r.Qty, Total = Pricing.Money(paidUnit * r.Qty) };
                }).ToList();
                Product replacement = null;
                decimal replacementAmount = 0m;
                int replacementQuantity = 0;
                if (kind == "Đổi hàng")
                {
                    string barcode = (request.ReplacementBarcode ?? "").Trim();
                    if (barcode.Length == 0) throw new TradeException("Hãy nhập mã vạch sản phẩm đổi.");
                    replacement = db.Products.SingleOrDefault(x => x.Barcode == barcode);
                    if (replacement == null) throw new TradeException("Không tìm thấy sản phẩm đổi theo mã vạch.");
                    replacementQuantity = items.Sum(x => x.Qty);
                    if (replacement.Stock < replacementQuantity) throw new TradeException(replacement.Name + ": tồn kho không đủ để đổi.");
                    replacementAmount = Pricing.Money(replacement.Price * replacementQuantity);
                }
                DateTime now = DateTime.Now;
                string number = "DT-" + now.ToString("yyyyMMddHHmmss") + "-" + Guid.NewGuid().ToString("N").Substring(0, 8);
                decimal returnedAmount = Pricing.Money(items.Sum(x => x.Total));
                var doc = new SaleReturn { Number = number, CreatedAt = now, SaleInvoiceId = invoice.Id, CreatedById = actorId, ReturnKind = kind, Reason = reason, RefundAmount = Math.Round(returnedAmount - replacementAmount, 2, MidpointRounding.AwayFromZero), ReplacementProductId = replacement == null ? (int?)null : replacement.Id, ReplacementProductName = replacement == null ? null : replacement.Name, ReplacementQuantity = replacementQuantity, ReplacementAmount = replacementAmount };
                if (request.CashSettlement && doc.RefundAmount != 0m) RecordCash(db, actorId, number, "Đổi/trả hàng", -doc.RefundAmount);
                db.Add(doc); db.SaveChanges();
                foreach (var x in items)
                {
                    int before = x.Product.Stock;
                    RestoreBatches(db, x.Line, x.Qty, number, "Trả", actorId);
                    x.Product.Stock += x.Qty;
                    db.Add(new SaleReturnLine { SaleReturnId = doc.Id, SaleInvoiceLineId = x.Line.Id, ProductId = x.Product.Id, ProductName = x.Product.Name, Quantity = x.Qty, UnitPrice = x.Line.UnitPrice, LineTotal = x.Total });
                    db.Add(new StockMovement { ProductId = x.Product.Id, ProductName = x.Product.Name, CreatedAt = now, Kind = StockMovementKind.Return, Quantity = x.Qty, StockBefore = before, StockAfter = x.Product.Stock, ReferenceNumber = number, CreatedById = actorId, SaleInvoiceId = invoice.Id, Note = kind + ": " + reason });
                }
                if (replacement != null)
                {
                    db.SaveChanges();
                    RemoveBatches(db, replacement, replacementQuantity, number, "Xuất hàng đổi", actorId, null);
                    int before = replacement.Stock; replacement.Stock -= replacementQuantity;
                    db.Add(new StockMovement { ProductId = replacement.Id, ProductName = replacement.Name, CreatedAt = now, Kind = StockMovementKind.Sale, Quantity = -replacementQuantity, StockBefore = before, StockAfter = replacement.Stock, ReferenceNumber = number, CreatedById = actorId, SaleInvoiceId = invoice.Id, Note = "Hàng đổi thay thế cho " + number });
                }
                db.SaveChanges(); tx.Commit();
                return new DocumentResult { Id = doc.Id, Number = doc.Number, Total = doc.RefundAmount, RefundAmount = doc.RefundAmount };
            }
        }

        public List<SupplierDebtRow> SupplierDebts(int actorId, string keyword)
        {
            string term = (keyword ?? "").Trim();
            using (var db = createSession())
            {
                Require(db, actorId, TradeAction.SupplierDebt);
                var receipts = db.Purchases.Where(x => term == "" || x.Number.Contains(term) || x.SupplierName.Contains(term))
                    .OrderByDescending(x => x.CreatedAt).Select(x => new { x.Id, x.Number, x.CreatedAt, x.SupplierName, x.Total }).ToList();
                var payables = db.SupplierPayables.ToList().ToDictionary(x => x.PurchaseReceiptId);
                var payments = db.SupplierPayments.ToList().GroupBy(x => x.PayableId).ToDictionary(x => x.Key, x => x.Sum(p => p.Amount));
                return receipts.Select(x => {
                    SupplierPayable p; bool known = payables.TryGetValue(x.Id, out p);
                    decimal paid = known ? p.OpeningPaid + (payments.ContainsKey(p.Id) ? payments[p.Id] : 0m) : 0m;
                    return new SupplierDebtRow { ReceiptId = x.Id, PayableId = known ? p.Id : (int?)null, Number = x.Number, CreatedAt = x.CreatedAt, SupplierName = x.SupplierName, Total = x.Total, Paid = known ? paid : (decimal?)null, Balance = known ? x.Total - paid : (decimal?)null };
                }).ToList();
            }
        }
        public void RegisterSupplierDebt(int actorId, int receiptId, decimal alreadyPaid)
        {
            using (var db = createSession()) using (var tx = db.BeginTransaction())
            {
                Require(db, actorId, TradeAction.SupplierDebt);
                var receipt = db.Purchases.SingleOrDefault(x => x.Id == receiptId);
                if (receipt == null) throw new TradeException("Không tìm thấy phiếu nhập.");
                if (db.SupplierPayables.Any(x => x.PurchaseReceiptId == receiptId)) throw new TradeException("Phiếu đã được xác lập công nợ.");
                if (alreadyPaid < 0m || alreadyPaid > receipt.Total || Pricing.Money(alreadyPaid) != alreadyPaid) throw new TradeException("Số đã trả không hợp lệ.");
                db.Add(new SupplierPayable { PurchaseReceiptId = receiptId, OpeningPaid = alreadyPaid, CreatedAt = DateTime.Now, CreatedById = actorId });
                db.SaveChanges(); tx.Commit();
            }
        }
        public void PaySupplier(int actorId, int payableId, Guid operationId, decimal amount, string method, string note)
        {
            if (operationId == Guid.Empty || amount <= 0m || Pricing.Money(amount) != amount) throw new TradeException("Mã thao tác hoặc số tiền không hợp lệ.");
            if (method != PaymentMethods.Cash && method != PaymentMethods.Transfer) throw new TradeException("Chọn tiền mặt hoặc chuyển khoản.");
            if ((note ?? "").Length > 200) throw new TradeException("Ghi chú tối đa 200 ký tự.");
            using (var db = createSession()) using (var tx = db.BeginTransaction())
            {
                Require(db, actorId, TradeAction.SupplierDebt);
                var existing = db.SupplierPayments.SingleOrDefault(x => x.OperationId == operationId);
                if (existing != null) { if (existing.PayableId != payableId || existing.Amount != amount || existing.Method != method) throw new TradeException("Mã thanh toán đã được sử dụng cho giao dịch khác."); return; }
                var payable = db.SupplierPayables.SingleOrDefault(x => x.Id == payableId);
                if (payable == null) throw new TradeException("Không tìm thấy công nợ.");
                var receipt = db.Purchases.Single(x => x.Id == payable.PurchaseReceiptId);
                decimal paid = payable.OpeningPaid + db.SupplierPayments.Where(x => x.PayableId == payableId).Select(x => x.Amount).ToList().Sum();
                if (amount > receipt.Total - paid) throw new TradeException("Số tiền vượt công nợ còn lại. Hãy tải lại danh sách.");
                if (method == PaymentMethods.Cash)
                    RecordCash(db, actorId, receipt.Number, "Trả công nợ nhà cung cấp", -amount);
                db.Add(new SupplierPayment { PayableId = payableId, OperationId = operationId, CreatedAt = DateTime.Now, CreatedById = actorId, Amount = amount, Method = method, Note = (note ?? "").Trim() });
                db.SaveChanges(); tx.Commit();
            }
        }
        public List<SupplierPaymentRow> SupplierPaymentHistory(int actorId, int payableId)
        {
            using (var db = createSession())
            {
                Require(db, actorId, TradeAction.SupplierDebt);
                return db.SupplierPayments.Where(x => x.PayableId == payableId).OrderByDescending(x => x.CreatedAt)
                    .Select(x => new SupplierPaymentRow { CreatedAt = x.CreatedAt, Amount = x.Amount, Method = x.Method, Note = x.Note }).ToList();
            }
        }

        public List<ShiftInfo> ShiftHistory(int actorId)
        {
            using (var db = createSession())
            {
                Require(db, actorId, TradeAction.Shift);
                return db.Shifts.Where(x => x.OpenedById == actorId).OrderByDescending(x => x.Id)
                    .ToList().Select(x => ShiftInfo(db, x)).ToList();
            }
        }

        public ShiftInfo CurrentShift(int actorId)
        {
            using (var db = createSession())
            {
                Require(db, actorId, TradeAction.Shift);
                var s = db.Shifts.Where(x => x.OpenedById == actorId && x.Status == "Đang mở").OrderByDescending(x => x.Id).FirstOrDefault();
                return s == null ? null : ShiftInfo(db, s);
            }
        }

        public ShiftInfo OpenShift(int actorId, decimal openingCash, string note)
        {
            if (openingCash < 0m || openingCash != Pricing.Money(openingCash)) throw new TradeException("Tiền đầu ca không hợp lệ.");
            using (var db = createSession()) using (var tx = db.BeginTransaction())
            {
                Require(db, actorId, TradeAction.Shift);
                if (db.Shifts.Any(x => x.OpenedById == actorId && x.Status == "Đang mở")) throw new TradeException("Bạn đang có một ca chưa chốt.");
                DateTime now = DateTime.Now;
                var shift = new Shift { Number = "CA-" + now.ToString("yyyyMMddHHmmss") + "-" + Guid.NewGuid().ToString("N").Substring(0, 6), OpenedAt = now, OpenedById = actorId, OpeningCash = openingCash, Status = "Đang mở", Note = (note ?? "").Trim() };
                db.Add(shift); db.SaveChanges(); tx.Commit(); return ShiftInfo(db, shift);
            }
        }

        public ShiftInfo CloseShift(int actorId, decimal closingCash, string note)
        {
            if (closingCash < 0m || closingCash != Pricing.Money(closingCash)) throw new TradeException("Tiền chốt ca không hợp lệ.");
            using (var db = createSession()) using (var tx = db.BeginTransaction())
            {
                Require(db, actorId, TradeAction.Shift);
                var shift = db.Shifts.Where(x => x.OpenedById == actorId && x.Status == "Đang mở").OrderByDescending(x => x.Id).FirstOrDefault();
                if (shift == null) throw new TradeException("Không có ca đang mở.");
                var info = ShiftInfo(db, shift);
                decimal expected = info.ExpectedCash;
                db.Add(new CashFundReceipt { ShiftId = shift.Id, CreatedAt = DateTime.Now, CreatedById = actorId, Amount = closingCash });
                shift.ClosingCash = closingCash; shift.ExpectedCash = expected; shift.Difference = Math.Round(closingCash - expected, 2, MidpointRounding.AwayFromZero); shift.ClosedAt = DateTime.Now; shift.ClosedById = actorId; shift.Status = "Đã chốt"; shift.Note = (note ?? "").Trim();
                db.SaveChanges(); tx.Commit(); return ShiftInfo(db, shift);
            }
        }

        private static ShiftInfo ShiftInfo(ITradeSession db, Shift s)
        {
            var invoices = db.Sales.Where(x => x.ShiftId == s.Id && x.PaymentMethod == PaymentMethods.Cash).Select(x => new { x.Number, x.IsCancelled, x.CashReceived, x.Change }).ToList();
            var cancelled = invoices.Where(x => x.IsCancelled).Select(x => x.Number).ToList();
            var logged = db.ShiftCashEntries.Where(x => x.Description == "Hoàn tiền hủy hóa đơn" && cancelled.Contains(x.ReferenceNumber)).Select(x => x.ReferenceNumber).ToList();
            var cash = invoices.Where(x => !x.IsCancelled || logged.Contains(x.Number)).Sum(x => x.CashReceived - x.Change);
            var changes = db.ShiftCashEntries.Where(x => x.ShiftId == s.Id).Select(x => x.Amount).ToList().Sum();
            var fund = db.CashFundReceipts.SingleOrDefault(x => x.ShiftId == s.Id);
            return new ShiftInfo { FundAmount = fund == null ? (decimal?)null : fund.Amount, FundTransferredAt = fund == null ? (DateTime?)null : fund.CreatedAt, Id = s.Id, Number = s.Number, OpenedAt = s.OpenedAt, ClosedAt = s.ClosedAt, Note = s.Note, OpeningCash = s.OpeningCash, CashSales = cash, CashAdjustments = changes, ExpectedCash = s.ExpectedCash ?? (s.OpeningCash + cash + changes), Status = s.Status, ClosingCash = s.ClosingCash, Difference = s.Difference };
        }

        private static void RecordCash(ITradeSession db, int actorId, string reference, string description, decimal amount)
        {
            var shift = db.Shifts.SingleOrDefault(x => x.OpenedById == actorId && x.Status == "Đang mở");
            if (shift == null) throw new TradeException("Cần mở ca của người thực hiện để ghi nhận thu/chi tiền mặt.");
            db.Add(new ShiftCashEntry { ShiftId = shift.Id, CreatedAt = DateTime.Now, CreatedById = actorId, ReferenceNumber = reference, Description = description, Amount = amount });
        }

        public RevenueData Revenue(int actorId, DateTime from, DateTime to)
        {
            var end = EndExclusive(from, to); var start = from.Date;
            using (var db = createSession())
            {
                Require(db, actorId, TradeAction.Revenue);
                var rows = db.Sales.Where(x => !x.IsCancelled && x.CreatedAt >= start && x.CreatedAt < end)
                    .OrderByDescending(x => x.CreatedAt).ThenByDescending(x => x.Id)
                    .Select(x => new InvoiceReportRow { Id = x.Id, Number = x.Number, CreatedAt = x.CreatedAt,
                        CustomerName = x.CustomerName, Subtotal = x.Subtotal, DiscountAmount = x.DiscountAmount,
                        PromotionCode = x.PromotionCode, PromotionAmount = x.PromotionAmount, Total = x.Total }).ToList();
                var invoiceIds = rows.Select(x => x.Id).ToList();
                var soldLines = db.SaleLines.Where(x => invoiceIds.Contains(x.SaleInvoiceId)).Select(x => new { x.Id, x.Quantity }).ToList();
                var lineIds = soldLines.Select(x => x.Id).ToList();
                var batchCosts = db.BatchMovements.Where(x => x.SaleInvoiceLineId.HasValue && lineIds.Contains(x.SaleInvoiceLineId.Value) && x.Kind == "Bán")
                    .Select(x => new { x.SaleInvoiceLineId, x.Quantity, x.UnitCost }).ToList();
                bool hasReturns = db.Returns.Any(x => invoiceIds.Contains(x.SaleInvoiceId) || (x.CreatedAt >= start && x.CreatedAt < end));
                bool missingCosts = soldLines.Any(line => batchCosts.Where(x => x.SaleInvoiceLineId == line.Id).Sum(x => -x.Quantity) != line.Quantity || batchCosts.Any(x => x.SaleInvoiceLineId == line.Id && !x.UnitCost.HasValue));
                decimal? cost = hasReturns || missingCosts ? (decimal?)null : batchCosts.Sum(x => -x.Quantity * x.UnitCost.Value);
                decimal revenue = rows.Sum(x => x.Total);
                return new RevenueData { Invoices = rows, CostOfGoods = cost, GrossProfit = cost.HasValue ? revenue - cost.Value : (decimal?)null,
                    ProfitStatus = hasReturns ? "Chưa tính lợi nhuận: có đổi/trả liên quan trong kỳ." : missingCosts ? "Chưa tính lợi nhuận: có hàng thiếu giá vốn hoặc lịch sử lô." : "Lợi nhuận gộp (chưa trừ chi phí vận hành)",
                    Days = rows.GroupBy(x => x.CreatedAt.Date).OrderBy(x => x.Key).Select(g => new DailyRevenueRow {
                        Date = g.Key, InvoiceCount = g.Count(), Subtotal = g.Sum(x => x.Subtotal),
                        DiscountAmount = g.Sum(x => x.DiscountAmount), PromotionAmount = g.Sum(x => x.PromotionAmount), Revenue = g.Sum(x => x.Total) }).ToList(),
                    Subtotal = rows.Sum(x => x.Subtotal), Discount = rows.Sum(x => x.DiscountAmount), Promotion = rows.Sum(x => x.PromotionAmount), Revenue = rows.Sum(x => x.Total) };
            }
        }
        public StockData Stock(int actorId, DateTime from, DateTime to, int? productId, StockMovementKind? kind)
        {
            var end = EndExclusive(from, to); var start = from.Date;
            using (var db = createSession())
            using (var tx = db.BeginTransaction())
            {
                Require(db, actorId, TradeAction.Stock);
                var productQuery = db.Products;
                if (productId.HasValue)
                {
                    int selectedId = productId.Value;
                    productQuery = productQuery.Where(x => x.Id == selectedId);
                }
                var products = productQuery.OrderBy(x => x.Name).ToList();
                var ids = products.Select(x => x.Id).ToList();
                var movements = db.Movements.Where(x => ids.Contains(x.ProductId) && x.CreatedAt >= start).ToList();
                var balances = products.Select(p => {
                    var changes = movements.Where(x => x.ProductId == p.Id).ToList();
                    var period = changes.Where(x => x.CreatedAt < end).ToList();
                    return new StockRow { ProductId = p.Id, ProductName = p.Name,
                        Opening = p.Stock - changes.Sum(x => x.Quantity),
                        Purchased = period.Where(x => x.Kind == StockMovementKind.Purchase).Sum(x => x.Quantity),
                        Sold = -period.Where(x => x.Kind == StockMovementKind.Sale || x.Kind == StockMovementKind.SaleCancel || x.Kind == StockMovementKind.Return).Sum(x => x.Quantity),
                        Adjusted = period.Where(x => x.Kind == StockMovementKind.Adjustment).Sum(x => x.Quantity),
                        Closing = p.Stock - changes.Where(x => x.CreatedAt >= end).Sum(x => x.Quantity), Current = p.Stock };
                }).ToList();
                var rows = movements.Where(x => x.CreatedAt < end && (!kind.HasValue || x.Kind == kind.Value))
                    .OrderByDescending(x => x.CreatedAt).ThenByDescending(x => x.Id).Select(x => new MovementRow {
                        CreatedAt = x.CreatedAt, ReferenceNumber = x.ReferenceNumber, ProductName = x.ProductName,
                        Kind = KindName(x.Kind), Quantity = x.Quantity,
                        StockBefore = x.StockBefore, StockAfter = x.StockAfter, Note = x.Note }).ToList();
                var expiryLimit = DateTime.Today.AddDays(30);
                var expiring = db.Batches.Where(b => b.RemainingQuantity > 0 && b.ExpiryDate.HasValue && b.ExpiryDate.Value <= expiryLimit)
                    .OrderBy(b => b.ExpiryDate).Select(b => new { b.ProductId, b.BatchNumber, b.ExpiryDate, b.RemainingQuantity }).ToList();
                var productNames = db.Products.ToDictionary(x => x.Id, x => x.Name);
                var batchHistory = (from m in db.BatchMovements join b in db.Batches on m.BatchId equals b.Id
                    where m.CreatedAt >= start && m.CreatedAt < end && ids.Contains(m.ProductId)
                    orderby m.CreatedAt descending, m.Id descending
                    select new { m.CreatedAt, m.ReferenceNumber, m.ProductId, b.BatchNumber, b.ExpiryDate, m.Kind, m.Quantity }).ToList();
                var batchStock = db.Batches.Where(b => ids.Contains(b.ProductId)).OrderBy(b => b.ExpiryDate).ThenBy(b => b.Id).ToList();
                tx.Commit();
                return new StockData { Movements = rows, Balances = balances, Batches = batchStock.Select(b => new BatchStockRow { ProductName = productNames[b.ProductId], BatchNumber = b.BatchNumber, ReceivedAt = b.ReceivedAt, ExpiryDate = b.ExpiryDate, RemainingQuantity = b.RemainingQuantity, Status = b.RemainingQuantity == 0 ? "Hết hàng" : !b.ExpiryDate.HasValue ? "Không rõ hạn" : b.ExpiryDate.Value.Date < DateTime.Today ? "Hết hạn" : b.ExpiryDate.Value.Date <= expiryLimit ? "Sắp hết hạn" : "Còn hạn" }).ToList(), BatchHistory = batchHistory.Select(x => new BatchMovementRow { CreatedAt = x.CreatedAt, ReferenceNumber = x.ReferenceNumber, ProductName = productNames[x.ProductId], BatchNumber = x.BatchNumber, ExpiryDate = x.ExpiryDate, Kind = x.Kind, Quantity = x.Quantity }).ToList(), Expiring = expiring.Select(x => new ExpiryRow { ProductName = productNames.ContainsKey(x.ProductId) ? productNames[x.ProductId] : "", BatchNumber = x.BatchNumber, ExpiryDate = x.ExpiryDate, RemainingQuantity = x.RemainingQuantity, DaysLeft = (x.ExpiryDate.Value.Date - DateTime.Today).Days }).ToList() };
            }
        }
    }
}
