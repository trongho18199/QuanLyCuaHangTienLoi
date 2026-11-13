using System;
using System.Collections.Generic;
namespace ConvenienceStore.Business.Models
{
    
    public static class StockPolicy
    {
        public const int LowStockThreshold = 10;
    }
    public class DashboardDayRow
    {
        public DateTime Date { get; set; }
        public int InvoiceCount { get; set; }
        public decimal Revenue { get; set; }
    }
    public class TopProductRow
    {
        public int Rank { get; set; }
        public string ProductName { get; set; }
        public int Quantity { get; set; }
        public decimal Revenue { get; set; }
    }
    public class LowStockRow
    {
        public string ProductName { get; set; }
        public string Barcode { get; set; }
        public string CategoryName { get; set; }
        public int Stock { get; set; }
        public string Status { get { return Stock <= 0 ? "Hết hàng" : "Sắp hết"; } }
    }
    public class DashboardData
    {
        public decimal TodayRevenue { get; set; }
        public int TodayInvoices { get; set; }
        public decimal MonthRevenue { get; set; }
        public int MonthInvoices { get; set; }
        public int LowStockCount { get; set; }
        public int OutOfStockCount { get; set; }
        public List<DashboardDayRow> Days { get; set; }
        public List<TopProductRow> TopProducts { get; set; }
        public List<LowStockRow> LowStock { get; set; }
    }
}
