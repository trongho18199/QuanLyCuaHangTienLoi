using System;
using System.Data;
using System.Data.Common;
using System.Linq;
namespace ConvenienceStore.Data.Repositories
{
    public sealed class LinqCatalogSession : ICatalogSession
    {
        private readonly CSDbContext db = new CSDbContext();
        public IQueryable<Category> Categories { get { return db.Categories; } }
        public IQueryable<Product> Products { get { return db.Products; } }
        public IQueryable<Supplier> Suppliers { get { return db.Suppliers; } }
        public IQueryable<Customer> Customers { get { return db.Customers; } }
        public IQueryable<User> Users { get { return db.Users; } }
        public IQueryable<RolePermission> RolePermissions { get { return db.RolePermissions; } }
        public IQueryable<PurchaseReceipt> Purchases { get { return db.PurchaseReceipts; } }
        public IQueryable<PurchaseReceiptLine> PurchaseLines { get { return db.PurchaseReceiptLines; } }
        public IQueryable<SaleInvoice> Sales { get { return db.SaleInvoices; } }
        public IQueryable<SaleInvoiceLine> SaleLines { get { return db.SaleInvoiceLines; } }
        public IQueryable<StockMovement> Movements { get { return db.StockMovements; } }
        public IQueryable<ProductBatch> Batches { get { return db.GetTable<ProductBatch>(); } }
        public IQueryable<BatchMovement> BatchMovements { get { return db.GetTable<BatchMovement>(); } }
        public IQueryable<SaleReturnLine> ReturnLines { get { return db.GetTable<SaleReturnLine>(); } }
        public IQueryable<Promotion> Promotions { get { return db.Promotions; } }
        public ITradeTransaction BeginTransaction()
        {
            
            return new Transaction(db.BeginTransaction(IsolationLevel.Serializable));
        }
        public void Add(Category x) { db.Categories.InsertOnSubmit(x); }
        public void Add(Product x) { db.Products.InsertOnSubmit(x); }
        public void Add(Supplier x) { db.Suppliers.InsertOnSubmit(x); }
        public void Add(Customer x) { db.Customers.InsertOnSubmit(x); }
        public void Add(Promotion x) { db.Promotions.InsertOnSubmit(x); }
        public void Remove(Category x) { db.Categories.DeleteOnSubmit(x); }
        public void Remove(Product x) { db.Products.DeleteOnSubmit(x); }
        public void Remove(Supplier x) { db.Suppliers.DeleteOnSubmit(x); }
        public void Remove(Customer x) { db.Customers.DeleteOnSubmit(x); }
        public void Remove(Promotion x) { db.Promotions.DeleteOnSubmit(x); }
        public void SaveChanges() { db.SubmitChanges(); }
        public void Dispose() { db.Dispose(); }
        private sealed class Transaction : ITradeTransaction
        {
            private readonly DbTransaction transaction;
            private bool committed;
            public Transaction(DbTransaction value) { transaction = value; }
            public void Commit() { transaction.Commit(); committed = true; }
            public void Dispose()
            {
                try { if (!committed) transaction.Rollback(); }
                finally { transaction.Dispose(); }
            }
        }
    }
}
