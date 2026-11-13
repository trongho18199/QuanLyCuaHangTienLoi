using System;
using System.Collections.Generic;
using System.Linq;
using ConvenienceStore.Business.Models;
using ConvenienceStore.Data.Repositories;
namespace ConvenienceStore.Business.Services
{
    
    public sealed class DashboardService
    {
        private const string DashboardMenu = "mnuDashboard";
        private readonly Func<ICatalogSession> createSession;
        public DashboardService() : this(() => new LinqCatalogSession()) { }
        public DashboardService(Func<ICatalogSession> sessionFactory)
        { createSession = sessionFactory ?? throw new ArgumentNullException("sessionFactory"); }

        public DashboardData Load(int actorId)
        {
            DateTime today = DateTime.Today, tomorrow = today.AddDays(1);
            DateTime monthStart = new DateTime(today.Year, today.Month, 1);
            DateTime weekStart = today.AddDays(-6), topStart = today.AddDays(-29);
            DateTime since = monthStart < weekStart ? monthStart : weekStart;
            int threshold = StockPolicy.LowStockThreshold;
            using (var db = createSession())
            {
                CatalogService.Require(db, actorId, DashboardMenu);
                
                var sales = db.Sales.Where(x => !x.IsCancelled && x.CreatedAt >= since && x.CreatedAt < tomorrow)
                    .Select(x => new { x.CreatedAt, x.Total }).ToList();
                var todaySales = sales.Where(x => x.CreatedAt >= today).ToList();
                var monthSales = sales.Where(x => x.CreatedAt >= monthStart).ToList();
                var days = Enumerable.Range(0, 7).Select(i => {
                    DateTime d = weekStart.AddDays(i);
                    var rows = sales.Where(x => x.CreatedAt.Date == d).ToList();
                    return new DashboardDayRow { Date = d, InvoiceCount = rows.Count, Revenue = rows.Sum(x => x.Total) };
                }).ToList();
                var top = db.SaleLines
                    .Where(l => !l.SaleInvoice.IsCancelled && l.SaleInvoice.CreatedAt >= topStart && l.SaleInvoice.CreatedAt < tomorrow)
                    .GroupBy(l => l.ProductId)
                    .Select(g => new { Name = g.Max(l => l.ProductName), Quantity = g.Sum(l => l.Quantity), Revenue = g.Sum(l => l.LineTotal) })
                    .OrderByDescending(x => x.Quantity).Take(10).ToList();
                var low = db.Products.Where(x => x.Stock <= threshold);
                var lowRows = low.OrderBy(x => x.Stock).ThenBy(x => x.Name).Take(50)
                    .Select(x => new LowStockRow { ProductName = x.Name, Barcode = x.Barcode, CategoryName = x.Category.Name, Stock = x.Stock }).ToList();
                return new DashboardData {
                    TodayRevenue = todaySales.Sum(x => x.Total), TodayInvoices = todaySales.Count,
                    MonthRevenue = monthSales.Sum(x => x.Total), MonthInvoices = monthSales.Count,
                    LowStockCount = low.Count(), OutOfStockCount = db.Products.Count(x => x.Stock <= 0),
                    Days = days,
                    TopProducts = top.Select((x, i) => new TopProductRow { Rank = i + 1, ProductName = x.Name, Quantity = x.Quantity, Revenue = x.Revenue }).ToList(),
                    LowStock = lowRows };
            }
        }
    }
}
