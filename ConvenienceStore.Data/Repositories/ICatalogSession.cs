using System;
using System.Linq;
namespace ConvenienceStore.Data.Repositories
{
    
    public interface ICatalogSession : IDisposable
    {
        IQueryable<Category> Categories { get; }
        IQueryable<Product> Products { get; }
        IQueryable<Supplier> Suppliers { get; }
        IQueryable<Customer> Customers { get; }
        IQueryable<User> Users { get; }
        IQueryable<RolePermission> RolePermissions { get; }
        IQueryable<PurchaseReceipt> Purchases { get; }
        IQueryable<PurchaseReceiptLine> PurchaseLines { get; }
        IQueryable<SaleInvoice> Sales { get; }
        IQueryable<SaleInvoiceLine> SaleLines { get; }
        IQueryable<StockMovement> Movements { get; }
        IQueryable<ProductBatch> Batches { get; }
        IQueryable<BatchMovement> BatchMovements { get; }
        IQueryable<SaleReturnLine> ReturnLines { get; }
        IQueryable<Promotion> Promotions { get; }
        ITradeTransaction BeginTransaction();
        void Add(Category entity);
        void Add(Product entity);
        void Add(Supplier entity);
        void Add(Customer entity);
        void Add(Promotion entity);
        void Remove(Category entity);
        void Remove(Product entity);
        void Remove(Supplier entity);
        void Remove(Customer entity);
        void Remove(Promotion entity);
        void SaveChanges();
    }
}
