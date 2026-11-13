using System;
using System.Data;
using System.Data.Common;
using System.Linq;
namespace ConvenienceStore.Data.Repositories
{
    public sealed class LinqTradeSession : ITradeSession
    {
        private readonly CSDbContext db = new CSDbContext();
        public IQueryable<Product> Products { get { return db.Products; } }
        public IQueryable<User> Users { get { return db.Users; } }
        public IQueryable<RolePermission> RolePermissions { get { return db.RolePermissions; } }
        public IQueryable<Supplier> Suppliers { get { return db.Suppliers; } }
        public IQueryable<Customer> Customers { get { return db.Customers; } }
        public IQueryable<Promotion> Promotions { get { return db.Promotions; } }
        public IQueryable<PurchaseReceipt> Purchases { get { return db.PurchaseReceipts; } }
        public IQueryable<SaleInvoice> Sales { get { return db.SaleInvoices; } }
        public IQueryable<SaleInvoiceLine> SaleLines { get { return db.SaleInvoiceLines; } }
        public IQueryable<StockMovement> Movements { get { return db.StockMovements; } }
        public IQueryable<SaleReturn> Returns { get { return db.SaleReturns; } }
        public IQueryable<SaleReturnLine> ReturnLines { get { return db.SaleReturnLines; } }
        public IQueryable<Shift> Shifts { get { return db.GetTable<Shift>(); } }
        public IQueryable<ShiftCashEntry> ShiftCashEntries { get { return db.GetTable<ShiftCashEntry>(); } }
        public IQueryable<ProductBatch> Batches { get { return db.GetTable<ProductBatch>(); } }
        public IQueryable<SupplierPayable> SupplierPayables { get { return db.GetTable<SupplierPayable>(); } }
        public IQueryable<SupplierPayment> SupplierPayments { get { return db.GetTable<SupplierPayment>(); } }
        public IQueryable<BatchMovement> BatchMovements { get { return db.GetTable<BatchMovement>(); } }
        public IQueryable<CashFundReceipt> CashFundReceipts { get { return db.GetTable<CashFundReceipt>(); } }
        public void Add(CashFundReceipt x) { db.GetTable<CashFundReceipt>().InsertOnSubmit(x); }
        public ITradeTransaction BeginTransaction()
        {
            
            return new Transaction(db.BeginTransaction(IsolationLevel.Serializable));
        }
        public void Add(PurchaseReceipt x) { db.PurchaseReceipts.InsertOnSubmit(x); }
        public void Add(SaleInvoice x) { db.SaleInvoices.InsertOnSubmit(x); }
        public void Add(StockMovement x) { db.StockMovements.InsertOnSubmit(x); }
        public void Add(SaleReturn x) { db.SaleReturns.InsertOnSubmit(x); }
        public void Add(SaleReturnLine x) { db.SaleReturnLines.InsertOnSubmit(x); }
        public void Add(Shift x) { db.GetTable<Shift>().InsertOnSubmit(x); }
        public void Add(ShiftCashEntry x) { db.GetTable<ShiftCashEntry>().InsertOnSubmit(x); }
        public void Add(ProductBatch x) { db.GetTable<ProductBatch>().InsertOnSubmit(x); }
        public void Add(BatchMovement x) { db.GetTable<BatchMovement>().InsertOnSubmit(x); }
        public void Add(SupplierPayable x) { db.GetTable<SupplierPayable>().InsertOnSubmit(x); }
        public void Add(SupplierPayment x) { db.GetTable<SupplierPayment>().InsertOnSubmit(x); }
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
