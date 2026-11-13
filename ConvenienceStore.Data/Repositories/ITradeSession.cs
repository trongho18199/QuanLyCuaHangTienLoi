using System;
using System.Linq;
namespace ConvenienceStore.Data.Repositories
{
    public interface ITradeTransaction : IDisposable { void Commit(); }
    
    public interface ITradeSession : IDisposable
    {
        IQueryable<Product> Products { get; }
        IQueryable<User> Users { get; }
        IQueryable<RolePermission> RolePermissions { get; }
        IQueryable<Supplier> Suppliers { get; }
        IQueryable<Customer> Customers { get; }
        IQueryable<Promotion> Promotions { get; }
        IQueryable<PurchaseReceipt> Purchases { get; }
        IQueryable<SaleInvoice> Sales { get; }
        IQueryable<SaleInvoiceLine> SaleLines { get; }
        IQueryable<StockMovement> Movements { get; }
        IQueryable<SaleReturn> Returns { get; }
        IQueryable<SaleReturnLine> ReturnLines { get; }
        IQueryable<Shift> Shifts { get; }
        IQueryable<CashFundReceipt> CashFundReceipts { get; }
        IQueryable<ShiftCashEntry> ShiftCashEntries { get; }
        IQueryable<ProductBatch> Batches { get; }
        IQueryable<SupplierPayable> SupplierPayables { get; }
        IQueryable<SupplierPayment> SupplierPayments { get; }
        IQueryable<BatchMovement> BatchMovements { get; }
        ITradeTransaction BeginTransaction();
        void Add(PurchaseReceipt receipt);
        void Add(SaleInvoice invoice);
        void Add(StockMovement movement);
        void Add(SaleReturn saleReturn);
        void Add(SaleReturnLine line);
        void Add(Shift shift);
        void Add(CashFundReceipt receipt);
        void Add(ShiftCashEntry entry);
        void Add(ProductBatch batch);
        void Add(BatchMovement movement);
        void Add(SupplierPayable payable);
        void Add(SupplierPayment payment);
        void SaveChanges();
    }
}
