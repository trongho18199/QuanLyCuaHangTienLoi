using System;
using System.Collections.Generic;
using System.Linq;
using ConvenienceStore.Business.Models;
using ConvenienceStore.Data;
using ConvenienceStore.Data.Repositories;
namespace ConvenienceStore.Business.Services
{
    
    
    
    
    public sealed class CatalogService
    {
        private const string CategoryMenu = "mnuCategory", ProductMenu = "mnuProduct", SupplierMenu = "mnuSupplier",
            CustomerMenu = "mnuCustomer", InvoiceMenu = "mnuInvoice", PromotionMenu = "mnuPromotion",
            PurchaseHistoryMenu = "mnuPurchaseHistory";
        private readonly Func<ICatalogSession> createSession;
        public CatalogService() : this(() => new LinqCatalogSession()) { }
        public CatalogService(Func<ICatalogSession> sessionFactory)
        { createSession = sessionFactory ?? throw new ArgumentNullException("sessionFactory"); }

        

        
        internal static void Require(ICatalogSession db, int actorId, params string[] menuCodes)
        {
            var user = db.Users.Where(x => x.Id == actorId && x.IsActive)
                .Select(x => new { x.RoleId, RoleName = x.Role.Name }).SingleOrDefault();
            var codes = menuCodes.ToList();
            bool ok = user != null &&
                (user.RoleName == RoleRepository.AdminRoleName ||
                 db.RolePermissions.Any(p => p.RoleId == user.RoleId && codes.Contains(p.FunctionCode)));
            if (!ok)
                throw new TradeException("Tài khoản chưa đăng nhập, bị khóa hoặc không có quyền thực hiện chức năng này.");
        }
        private static string Text(string value, string label, int max, bool required)
        {
            string v = (value ?? "").Trim();
            if (required && v.Length == 0) throw new TradeException(label + " không được để trống.");
            if (v.Length > max) throw new TradeException(label + " tối đa " + max + " ký tự.");
            return v;
        }
        private static string Keyword(string value)
        {
            string v = (value ?? "").Trim();
            return v.Length > 100 ? v.Substring(0, 100) : v;
        }
        
        private static string Phone(string value)
        {
            string v = Text(value, "Số điện thoại", 30, false);
            if (v.Length == 0) return null;
            if (v.Any(c => !((c >= '0' && c <= '9') || " +-.()".IndexOf(c) >= 0)) || v.Count(c => c >= '0' && c <= '9') < 8)
                throw new TradeException("Số điện thoại không hợp lệ (chỉ gồm số, tối thiểu 8 chữ số).");
            return v;
        }
        private static DateTime EndExclusive(DateTime from, DateTime to)
        {
            if (from.Date > to.Date || to.Date == DateTime.MaxValue.Date)
                throw new TradeException("Khoảng ngày không hợp lệ.");
            return to.Date.AddDays(1);
        }
        private static T Find<T>(IQueryable<T> source, System.Linq.Expressions.Expression<Func<T, bool>> match, string what) where T : class
        {
            var entity = source.SingleOrDefault(match);
            if (entity == null) throw new TradeException(what + " không còn tồn tại. Hãy tải lại danh sách.");
            return entity;
        }

        

        public List<CategoryRow> Categories(int actorId, string keyword)
        {
            string kw = Keyword(keyword);
            using (var db = createSession())
            {
                Require(db, actorId, CategoryMenu);
                var query = db.Categories;
                if (kw.Length > 0) query = query.Where(x => x.Code.Contains(kw) || x.Name.Contains(kw));
                return query.OrderBy(x => x.Code).Select(x => new CategoryRow {
                    Id = x.Id, Code = x.Code, Name = x.Name, ProductCount = x.Products.Count() }).ToList();
            }
        }
        
        public List<CategoryOption> CategoryOptions(int actorId)
        {
            using (var db = createSession())
            {
                Require(db, actorId, CategoryMenu, ProductMenu);
                return db.Categories.OrderBy(x => x.Name)
                    .Select(x => new CategoryOption { Id = x.Id, Name = x.Name }).ToList();
            }
        }
        public int SaveCategory(int actorId, CategoryInput input)
        {
            if (input == null) throw new TradeException("Thiếu dữ liệu danh mục.");
            string code = Text(input.Code, "Mã danh mục", 30, true).ToUpperInvariant();
            if (code.Any(c => !(char.IsLetterOrDigit(c) || c == '-' || c == '_')))
                throw new TradeException("Mã danh mục chỉ được gồm chữ, số, dấu gạch ngang (-) hoặc gạch dưới (_).");
            string name = Text(input.Name, "Tên danh mục", 100, true);
            using (var db = createSession())
            using (var tx = db.BeginTransaction())
            {
                Require(db, actorId, CategoryMenu);
                if (db.Categories.Any(x => x.Id != input.Id && x.Code == code))
                    throw new TradeException("Mã danh mục \"" + code + "\" đã tồn tại.");
                if (db.Categories.Any(x => x.Id != input.Id && x.Name == name))
                    throw new TradeException("Danh mục \"" + name + "\" đã tồn tại.");
                Category entity;
                if (input.Id == 0) { entity = new Category { Code = code, Name = name }; db.Add(entity); }
                else { entity = Find(db.Categories, x => x.Id == input.Id, "Danh mục"); entity.Code = code; entity.Name = name; }
                db.SaveChanges();
                tx.Commit();
                return entity.Id;
            }
        }
        public void DeleteCategory(int actorId, int id)
        {
            using (var db = createSession())
            using (var tx = db.BeginTransaction())
            {
                Require(db, actorId, CategoryMenu);
                var entity = Find(db.Categories, x => x.Id == id, "Danh mục");
                if (db.Products.Any(x => x.CategoryId == id))
                    throw new TradeException("Danh mục \"" + entity.Name + "\" đang có sản phẩm nên không thể xóa. Hãy chuyển các sản phẩm sang danh mục khác trước.");
                db.Remove(entity);
                db.SaveChanges();
                tx.Commit();
            }
        }

        

        public List<ProductRow> Products(int actorId, string keyword, int? categoryId)
        {
            string kw = Keyword(keyword);
            using (var db = createSession())
            {
                Require(db, actorId, ProductMenu);
                var query = db.Products;
                if (categoryId.HasValue) { int cid = categoryId.Value; query = query.Where(x => x.CategoryId == cid); }
                if (kw.Length > 0) query = query.Where(x => x.Name.Contains(kw) || (x.Barcode != null && x.Barcode.Contains(kw)));
                return query.OrderBy(x => x.Name).Select(x => new ProductRow {
                    Id = x.Id, Barcode = x.Barcode, Name = x.Name, CategoryId = x.CategoryId,
                    CategoryName = x.Category.Name, Price = x.Price, Stock = x.Stock }).ToList();
            }
        }
        
        public int SaveProduct(int actorId, ProductInput input)
        {
            if (input == null) throw new TradeException("Thiếu dữ liệu sản phẩm.");
            string name = Text(input.Name, "Tên sản phẩm", 100, true);
            string barcode = Text(input.Barcode, "Mã sản phẩm", 50, false);
            if (barcode.Length == 0) barcode = null;   
            if (input.CategoryId <= 0) throw new TradeException("Hãy chọn danh mục cho sản phẩm.");
            if (input.Price <= 0m) throw new TradeException("Giá bán phải lớn hơn 0.");
            decimal price = Pricing.Money(input.Price);
            if (price != input.Price) throw new TradeException("Giá bán chỉ có tối đa 2 chữ số thập phân.");
            using (var db = createSession())
            using (var tx = db.BeginTransaction())
            {
                Require(db, actorId, ProductMenu);
                if (!db.Categories.Any(x => x.Id == input.CategoryId))
                    throw new TradeException("Danh mục đã chọn không còn tồn tại. Hãy tải lại danh sách.");
                if (barcode != null && db.Products.Any(x => x.Id != input.Id && x.Barcode == barcode))
                    throw new TradeException("Mã sản phẩm \"" + barcode + "\" đã được dùng cho sản phẩm khác.");
                Product entity;
                if (input.Id == 0)
                {
                    entity = new Product { Name = name, Barcode = barcode, CategoryId = input.CategoryId, Price = price, Stock = 0 };
                    db.Add(entity);
                }
                else
                {
                    entity = Find(db.Products, x => x.Id == input.Id, "Sản phẩm");
                    entity.Name = name; entity.Barcode = barcode; entity.CategoryId = input.CategoryId; entity.Price = price;
                }
                db.SaveChanges();
                tx.Commit();
                return entity.Id;
            }
        }
        public void DeleteProduct(int actorId, int id)
        {
            using (var db = createSession())
            using (var tx = db.BeginTransaction())
            {
                Require(db, actorId, ProductMenu);
                var entity = Find(db.Products, x => x.Id == id, "Sản phẩm");
                if (entity.Stock != 0)
                    throw new TradeException("Sản phẩm \"" + entity.Name + "\" còn tồn " + entity.Stock.ToString("N0") + " nên không thể xóa. Hãy kiểm kê hoặc điều chỉnh tồn về 0 trước.");
                if (db.Movements.Any(x => x.ProductId == id) ||
                    db.PurchaseLines.Any(x => x.ProductId == id) ||
                    db.SaleLines.Any(x => x.ProductId == id) ||
                    db.Batches.Any(x => x.ProductId == id) ||
                    db.BatchMovements.Any(x => x.ProductId == id) ||
                    db.ReturnLines.Any(x => x.ProductId == id))
                    throw new TradeException("Sản phẩm \"" + entity.Name + "\" đã phát sinh nhập, bán, lô hàng hoặc đổi/trả nên không thể xóa vì cần giữ lịch sử.");
                db.Remove(entity);
                db.SaveChanges();
                tx.Commit();
            }
        }

        

        public List<SupplierRow> Suppliers(int actorId, string keyword)
        {
            string kw = Keyword(keyword);
            using (var db = createSession())
            {
                Require(db, actorId, SupplierMenu);
                var query = db.Suppliers;
                if (kw.Length > 0) query = query.Where(x => x.Name.Contains(kw) || (x.Phone != null && x.Phone.Contains(kw)));
                return query.OrderBy(x => x.Name).Select(x => new SupplierRow {
                    Id = x.Id, Name = x.Name, Phone = x.Phone, IsActive = x.IsActive }).ToList();
            }
        }
        public int SaveSupplier(int actorId, SupplierInput input)
        {
            if (input == null) throw new TradeException("Thiếu dữ liệu nhà cung cấp.");
            string name = Text(input.Name, "Tên nhà cung cấp", 100, true);
            string phone = Phone(input.Phone);
            using (var db = createSession())
            using (var tx = db.BeginTransaction())
            {
                Require(db, actorId, SupplierMenu);
                if (db.Suppliers.Any(x => x.Id != input.Id && x.Name == name))
                    throw new TradeException("Nhà cung cấp \"" + name + "\" đã tồn tại.");
                Supplier entity;
                if (input.Id == 0) { entity = new Supplier { Name = name, Phone = phone, IsActive = input.IsActive }; db.Add(entity); }
                else
                {
                    entity = Find(db.Suppliers, x => x.Id == input.Id, "Nhà cung cấp");
                    entity.Name = name; entity.Phone = phone; entity.IsActive = input.IsActive;
                }
                db.SaveChanges();
                tx.Commit();
                return entity.Id;
            }
        }
        
        public DeleteResult DeleteSupplier(int actorId, int id)
        {
            using (var db = createSession())
            using (var tx = db.BeginTransaction())
            {
                Require(db, actorId, SupplierMenu);
                var entity = Find(db.Suppliers, x => x.Id == id, "Nhà cung cấp");
                DeleteResult result;
                if (db.Purchases.Any(x => x.SupplierId == id))
                {
                    result = new DeleteResult { Deactivated = true };
                    if (entity.IsActive)
                    {
                        entity.IsActive = false;
                        result.Message = "Nhà cung cấp \"" + entity.Name + "\" đã có phiếu nhập nên không thể xóa; đã chuyển sang ngừng hoạt động.";
                    }
                    else result.Message = "Nhà cung cấp \"" + entity.Name + "\" đã có phiếu nhập nên không thể xóa và đang ở trạng thái ngừng hoạt động.";
                }
                else
                {
                    db.Remove(entity);
                    result = new DeleteResult { Deactivated = false, Message = "Đã xóa nhà cung cấp \"" + entity.Name + "\"." };
                }
                db.SaveChanges();
                tx.Commit();
                return result;
            }
        }

        

        public List<CustomerRow> Customers(int actorId, string keyword)
        {
            string kw = Keyword(keyword);
            using (var db = createSession())
            {
                Require(db, actorId, CustomerMenu);
                var query = db.Customers;
                if (kw.Length > 0) query = query.Where(x => x.Name.Contains(kw) || (x.Phone != null && x.Phone.Contains(kw)));
                return query.OrderBy(x => x.Name).Select(x => new CustomerRow {
                    Id = x.Id, Name = x.Name, Phone = x.Phone, IsVip = x.IsVip, IsActive = x.IsActive }).ToList();
            }
        }
        public int SaveCustomer(int actorId, CustomerInput input)
        {
            if (input == null) throw new TradeException("Thiếu dữ liệu khách hàng.");
            string name = Text(input.Name, "Tên khách hàng", 100, true);
            string phone = Phone(input.Phone);
            using (var db = createSession())
            using (var tx = db.BeginTransaction())
            {
                Require(db, actorId, CustomerMenu);
                
                if (phone != null && db.Customers.Any(x => x.Id != input.Id && x.Phone == phone))
                    throw new TradeException("Số điện thoại \"" + phone + "\" đã thuộc về một khách hàng khác.");
                Customer entity;
                if (input.Id == 0)
                {
                    entity = new Customer { Name = name, Phone = phone, IsVip = input.IsVip, IsActive = input.IsActive };
                    db.Add(entity);
                }
                else
                {
                    entity = Find(db.Customers, x => x.Id == input.Id, "Khách hàng");
                    entity.Name = name; entity.Phone = phone; entity.IsVip = input.IsVip; entity.IsActive = input.IsActive;
                }
                db.SaveChanges();
                tx.Commit();
                return entity.Id;
            }
        }
        
        public DeleteResult DeleteCustomer(int actorId, int id)
        {
            using (var db = createSession())
            using (var tx = db.BeginTransaction())
            {
                Require(db, actorId, CustomerMenu);
                var entity = Find(db.Customers, x => x.Id == id, "Khách hàng");
                DeleteResult result;
                if (db.Sales.Any(x => x.CustomerId == id))
                {
                    result = new DeleteResult { Deactivated = true };
                    if (entity.IsActive)
                    {
                        entity.IsActive = false;
                        result.Message = "Khách hàng \"" + entity.Name + "\" đã có hóa đơn nên không thể xóa; đã chuyển sang ngừng hoạt động.";
                    }
                    else result.Message = "Khách hàng \"" + entity.Name + "\" đã có hóa đơn nên không thể xóa và đang ở trạng thái ngừng hoạt động.";
                }
                else
                {
                    db.Remove(entity);
                    result = new DeleteResult { Deactivated = false, Message = "Đã xóa khách hàng \"" + entity.Name + "\"." };
                }
                db.SaveChanges();
                tx.Commit();
                return result;
            }
        }

        

        public List<PromotionRow> Promotions(int actorId, string keyword)
        {
            string kw = Keyword(keyword);
            using (var db = createSession())
            {
                Require(db, actorId, PromotionMenu);
                var query = db.Promotions;
                if (kw.Length > 0) query = query.Where(x => x.Code.Contains(kw) || (x.Description != null && x.Description.Contains(kw)));
                return query.OrderBy(x => x.Code).Select(x => new PromotionRow {
                    Id = x.Id, Code = x.Code, Description = x.Description, DiscountPercent = x.DiscountPercent,
                    ValidFrom = x.ValidFrom, ValidTo = x.ValidTo, IsActive = x.IsActive }).ToList();
            }
        }
        public int SavePromotion(int actorId, PromotionInput input)
        {
            if (input == null) throw new TradeException("Thiếu dữ liệu khuyến mãi.");
            string code = Text(input.Code, "Mã khuyến mãi", 30, true).ToUpperInvariant();
            if (code.Any(c => !((c >= 'A' && c <= 'Z') || (c >= '0' && c <= '9') || c == '-' || c == '_')))
                throw new TradeException("Mã khuyến mãi chỉ gồm chữ cái không dấu, chữ số, dấu gạch ngang hoặc gạch dưới (không có khoảng trắng).");
            string description = Text(input.Description, "Mô tả", 200, false);
            if (description.Length == 0) description = null;
            if (input.DiscountPercent <= 0m || input.DiscountPercent > 100m)
                throw new TradeException("Phần trăm giảm phải lớn hơn 0 và không quá 100.");
            if (Math.Round(input.DiscountPercent, 2) != input.DiscountPercent)
                throw new TradeException("Phần trăm giảm chỉ có tối đa 2 chữ số thập phân.");
            DateTime? validFrom = input.ValidFrom.HasValue ? input.ValidFrom.Value.Date : (DateTime?)null;
            DateTime? validTo = input.ValidTo.HasValue ? input.ValidTo.Value.Date : (DateTime?)null;
            if (validFrom.HasValue && validTo.HasValue && validFrom.Value > validTo.Value)
                throw new TradeException("Ngày bắt đầu không được sau ngày kết thúc.");
            using (var db = createSession())
            using (var tx = db.BeginTransaction())
            {
                Require(db, actorId, PromotionMenu);
                if (db.Promotions.Any(x => x.Id != input.Id && x.Code == code))
                    throw new TradeException("Mã khuyến mãi \"" + code + "\" đã tồn tại.");
                Promotion entity;
                if (input.Id == 0) { entity = new Promotion(); db.Add(entity); }
                else entity = Find(db.Promotions, x => x.Id == input.Id, "Khuyến mãi");
                entity.Code = code; entity.Description = description; entity.DiscountPercent = input.DiscountPercent;
                entity.ValidFrom = validFrom; entity.ValidTo = validTo; entity.IsActive = input.IsActive;
                db.SaveChanges();
                tx.Commit();
                return entity.Id;
            }
        }
        
        public DeleteResult DeletePromotion(int actorId, int id)
        {
            using (var db = createSession())
            using (var tx = db.BeginTransaction())
            {
                Require(db, actorId, PromotionMenu);
                var entity = Find(db.Promotions, x => x.Id == id, "Khuyến mãi");
                string code = entity.Code;
                DeleteResult result;
                if (db.Sales.Any(x => x.PromotionCode == code))
                {
                    result = new DeleteResult { Deactivated = true };
                    if (entity.IsActive)
                    {
                        entity.IsActive = false;
                        result.Message = "Mã \"" + code + "\" đã được dùng trên hóa đơn nên không thể xóa; đã chuyển sang ngừng áp dụng.";
                    }
                    else result.Message = "Mã \"" + code + "\" đã được dùng trên hóa đơn nên không thể xóa và đang ở trạng thái ngừng áp dụng.";
                }
                else
                {
                    db.Remove(entity);
                    result = new DeleteResult { Deactivated = false, Message = "Đã xóa mã khuyến mãi \"" + code + "\"." };
                }
                db.SaveChanges();
                tx.Commit();
                return result;
            }
        }

        

        public List<PurchaseListRow> PurchaseReceipts(int actorId, DateTime from, DateTime to, string keyword)
        {
            var end = EndExclusive(from, to); var start = from.Date;
            string kw = Keyword(keyword);
            using (var db = createSession())
            {
                Require(db, actorId, PurchaseHistoryMenu);
                var query = db.Purchases.Where(x => x.CreatedAt >= start && x.CreatedAt < end);
                if (kw.Length > 0) query = query.Where(x => x.Number.Contains(kw) || x.SupplierName.Contains(kw));
                var rows = query.OrderByDescending(x => x.CreatedAt).ThenByDescending(x => x.Id)
                    .Select(x => new { x.Id, x.Number, x.CreatedAt, x.SupplierName,
                        FullName = x.CreatedBy.FullName, Username = x.CreatedBy.Username, x.Total, x.Note }).ToList();
                return rows.Select(x => new PurchaseListRow {
                    Id = x.Id, Number = x.Number, CreatedAt = x.CreatedAt, SupplierName = x.SupplierName,
                    CreatedBy = CashierName(x.FullName, x.Username), Total = x.Total, Note = x.Note }).ToList();
            }
        }
        public PurchaseDetail GetPurchaseReceipt(int actorId, int id)
        {
            using (var db = createSession())
            {
                Require(db, actorId, PurchaseHistoryMenu);
                var r = db.Purchases.Where(x => x.Id == id).Select(x => new {
                    x.Id, x.Number, x.CreatedAt, x.SupplierName, FullName = x.CreatedBy.FullName,
                    Username = x.CreatedBy.Username, x.Note, x.Total }).SingleOrDefault();
                if (r == null) throw new TradeException("Không tìm thấy phiếu nhập. Hãy tải lại danh sách.");
                var lines = db.PurchaseLines.Where(x => x.PurchaseReceiptId == id).OrderBy(x => x.Id)
                    .Select(x => new { x.ProductName, x.Quantity, x.UnitCost, x.LineTotal }).ToList();
                return new PurchaseDetail {
                    Id = r.Id, Number = r.Number, CreatedAt = r.CreatedAt, SupplierName = r.SupplierName,
                    CreatedBy = CashierName(r.FullName, r.Username), Note = r.Note, Total = r.Total,
                    Lines = lines.Select((x, index) => new PurchaseLineRow { No = index + 1, ProductName = x.ProductName,
                        Quantity = x.Quantity, UnitCost = x.UnitCost, LineTotal = x.LineTotal }).ToList() };
            }
        }

        

        private static string CashierName(string fullName, string username)
        { return string.IsNullOrWhiteSpace(fullName) ? username : fullName; }

        public List<InvoiceListRow> Invoices(int actorId, DateTime from, DateTime to, string keyword)
        {
            var end = EndExclusive(from, to); var start = from.Date;
            string kw = Keyword(keyword);
            using (var db = createSession())
            {
                Require(db, actorId, InvoiceMenu);
                var query = db.Sales.Where(x => x.CreatedAt >= start && x.CreatedAt < end);
                if (kw.Length > 0) query = query.Where(x => x.Number.Contains(kw) || x.CustomerName.Contains(kw));
                var rows = query.OrderByDescending(x => x.CreatedAt).ThenByDescending(x => x.Id)
                    .Select(x => new { x.Id, x.Number, x.CreatedAt, x.CustomerName,
                        FullName = x.CreatedBy.FullName, Username = x.CreatedBy.Username,
                        x.Subtotal, x.DiscountAmount, x.PromotionCode, x.Total, x.IsCancelled }).ToList();
                return rows.Select(x => new InvoiceListRow {
                    Id = x.Id, Number = x.Number, CreatedAt = x.CreatedAt, CustomerName = x.CustomerName,
                    Cashier = CashierName(x.FullName, x.Username), Subtotal = x.Subtotal,
                    DiscountAmount = x.DiscountAmount, PromotionCode = x.PromotionCode, Total = x.Total, IsCancelled = x.IsCancelled }).ToList();
            }
        }
        public InvoiceDetail GetInvoice(int actorId, int id)
        {
            using (var db = createSession())
            {
                Require(db, actorId, InvoiceMenu);
                var i = db.Sales.Where(x => x.Id == id).Select(x => new {
                    x.Id, x.OperationId, x.Number, x.CreatedAt, x.CustomerName,
                    FullName = x.CreatedBy.FullName, Username = x.CreatedBy.Username,
                    x.Subtotal, x.DiscountPercent, x.DiscountAmount, x.PromotionCode, x.PromotionAmount,
                    x.Total, x.CashReceived, x.Change, x.PaymentMethod, x.IsCancelled, x.CancelledAt, x.CancelledById, x.CancelReason }).SingleOrDefault();
                if (i == null) throw new TradeException("Không tìm thấy hóa đơn. Hãy tải lại danh sách.");
                var lines = db.SaleLines.Where(x => x.SaleInvoiceId == id).OrderBy(x => x.Id)
                    .Select(x => new { x.Id, x.ProductId, x.ProductName, x.Quantity, x.UnitPrice, x.LineTotal }).ToList();
                string cancelledBy = null;
                if (i.CancelledById.HasValue)
                {
                    int cancelUserId = i.CancelledById.Value;
                    var who = db.Users.Where(x => x.Id == cancelUserId).Select(x => new { x.FullName, x.Username }).SingleOrDefault();
                    if (who != null) cancelledBy = CashierName(who.FullName, who.Username);
                }
                return new InvoiceDetail {
                    Id = i.Id, OperationId = i.OperationId, Number = i.Number, CreatedAt = i.CreatedAt, CustomerName = i.CustomerName,
                    Cashier = CashierName(i.FullName, i.Username), Subtotal = i.Subtotal,
                    DiscountPercent = i.DiscountPercent, DiscountAmount = i.DiscountAmount,
                    PromotionCode = i.PromotionCode, PromotionAmount = i.PromotionAmount, Total = i.Total,
                    CashReceived = i.CashReceived, Change = i.Change, PaymentMethod = i.PaymentMethod,
                    IsCancelled = i.IsCancelled, CancelledAt = i.CancelledAt, CancelledBy = cancelledBy, CancelReason = i.CancelReason,
                    Lines = lines.Select((x, index) => new InvoiceLineRow { Id = x.Id, ProductId = x.ProductId, No = index + 1, ProductName = x.ProductName,
                        Quantity = x.Quantity, UnitPrice = x.UnitPrice, LineTotal = x.LineTotal }).ToList() };
            }
        }
    }
}
