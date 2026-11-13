using System;
using System.Collections.Generic;
namespace ConvenienceStore.Business.Models
{
    public enum TradeAction { Purchase, Sale, Stock, Revenue, Cancel, Adjust, Return, Shift, SupplierDebt }
    public class TradeException : Exception { public TradeException(string message) : base(message) { } }
    public class ProductOption
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Barcode { get; set; }
        public decimal Price { get; set; }
        public int Stock { get; set; }
        public string Display { get { return Name + " | " + Barcode + " | Tồn: " + Stock; } }
    }
    public class PartyOption
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public bool IsVip { get; set; }
        public string Display { get { return Name + (IsVip ? " (VIP -10%)" : ""); } }
    }
    public class PromotionOption
    {
        public string Code { get; set; }
        public decimal DiscountPercent { get; set; }
        public string Description { get; set; }
    }
    public class LookupData
    {
        public List<ProductOption> Products { get; set; }
        public List<PartyOption> Suppliers { get; set; }
        public List<PartyOption> Customers { get; set; }
    }
    public class CartLine
    {
        public int ProductId { get; set; }
        public string ProductName { get; set; }
        public int Quantity { get; set; }
        public decimal UnitPrice { get; set; }
        public string BatchNumber { get; set; }
        public DateTime? ExpiryDate { get; set; }
        public decimal LineTotal { get { return Pricing.Money(Quantity * UnitPrice); } }
    }
    public class SaleRequest
    {
        public Guid OperationId { get; set; }
        public int? CustomerId { get; set; }
        public List<CartLine> Lines { get; set; }
        public decimal ExpectedTotal { get; set; }
        public decimal CashReceived { get; set; }
        public string PromotionCode { get; set; }
        public string PaymentMethod { get; set; }
        public int? ShiftId { get; set; }
    }
    public class PurchaseRequest
    {
        public Guid OperationId { get; set; }
        public int SupplierId { get; set; }
        public List<CartLine> Lines { get; set; }
        public string Note { get; set; }
        public decimal? InitialPaid { get; set; }
    }
    public class SupplierDebtRow
    {
        public int ReceiptId { get; set; }
        public int? PayableId { get; set; }
        public string Number { get; set; }
        public DateTime CreatedAt { get; set; }
        public string SupplierName { get; set; }
        public decimal Total { get; set; }
        public decimal? Paid { get; set; }
        public decimal? Balance { get; set; }
        public string Status { get { return !Balance.HasValue ? "Chưa xác định" : Balance.Value == 0 ? "Đã thanh toán" : "Còn nợ"; } }
    }
    public class SupplierPaymentRow
    {
        public DateTime CreatedAt { get; set; }
        public decimal Amount { get; set; }
        public string Method { get; set; }
        public string Note { get; set; }
    }
    public class DocumentResult
    {
        public int Id { get; set; }
        public string Number { get; set; }
        public decimal Total { get; set; }
        public decimal Change { get; set; }
        public bool AlreadySaved { get; set; }
        public decimal RefundAmount { get; set; }
    }
    public static class PaymentMethods
    {
        public const string Cash = "Tiền mặt";
        public const string Transfer = "Chuyển khoản";
        public const string Card = "Thẻ ngân hàng";
        public const string EWallet = "Ví điện tử";
        public static readonly string[] All = { Cash, Transfer, Card, EWallet };
    }
    public class ReturnRequest
    {
        public int InvoiceId { get; set; }
        public List<ReturnLineRequest> Lines { get; set; }
        public string ReturnKind { get; set; }
        public string Reason { get; set; }
        public string ReplacementBarcode { get; set; }
        public bool CashSettlement { get; set; }
    }
    public class ReturnLineRequest
    {
        public int SaleInvoiceLineId { get; set; }
        public int Quantity { get; set; }
    }
    public class InvoiceReportRow
    {
        public int Id { get; set; }
        public string Number { get; set; }
        public DateTime CreatedAt { get; set; }
        public string CustomerName { get; set; }
        public decimal Subtotal { get; set; }
        public decimal DiscountAmount { get; set; }
        public string PromotionCode { get; set; }
        public decimal PromotionAmount { get; set; }
        public decimal Total { get; set; }
    }
    public class DailyRevenueRow
    {
        public DateTime Date { get; set; }
        public int InvoiceCount { get; set; }
        public decimal Subtotal { get; set; }
        public decimal DiscountAmount { get; set; }
        public decimal PromotionAmount { get; set; }
        public decimal Revenue { get; set; }
    }
    public class RevenueData
    {
        public List<InvoiceReportRow> Invoices { get; set; }
        public List<DailyRevenueRow> Days { get; set; }
        public decimal Subtotal { get; set; }
        public decimal Discount { get; set; }
        public decimal Promotion { get; set; }
        public decimal Revenue { get; set; }
        public decimal? CostOfGoods { get; set; }
        public decimal? GrossProfit { get; set; }
        public string ProfitStatus { get; set; }
    }
    public class MovementRow
    {
        public DateTime CreatedAt { get; set; }
        public string ReferenceNumber { get; set; }
        public string ProductName { get; set; }
        public string Kind { get; set; }
        public int Quantity { get; set; }
        public int StockBefore { get; set; }
        public int StockAfter { get; set; }
        public string Note { get; set; }
    }
    public class StockRow
    {
        public int ProductId { get; set; }
        public string ProductName { get; set; }
        public int Opening { get; set; }
        public int Purchased { get; set; }
        
        public int Sold { get; set; }
        
        public int Adjusted { get; set; }
        public int Closing { get; set; }
        public int Current { get; set; }
    }
    
    public class StockAdjustRequest
    {
        public int ProductId { get; set; }
        public int ExpectedStock { get; set; }
        public int NewStock { get; set; }
        public string Reason { get; set; }
    }
    public class StockData
    {
        public List<MovementRow> Movements { get; set; }
        public List<StockRow> Balances { get; set; }
        public List<ExpiryRow> Expiring { get; set; }
        public List<BatchMovementRow> BatchHistory { get; set; }
        public List<BatchStockRow> Batches { get; set; }
    }
    public class BatchStockRow
    {
        public string ProductName { get; set; }
        public string BatchNumber { get; set; }
        public DateTime ReceivedAt { get; set; }
        public DateTime? ExpiryDate { get; set; }
        public int RemainingQuantity { get; set; }
        public string Status { get; set; }
    }
    public class BatchMovementRow
    {
        public DateTime CreatedAt { get; set; }
        public string ReferenceNumber { get; set; }
        public string ProductName { get; set; }
        public string BatchNumber { get; set; }
        public DateTime? ExpiryDate { get; set; }
        public string Kind { get; set; }
        public int Quantity { get; set; }
    }
    public class ShiftInfo {
        public decimal? FundAmount { get; set; }
        public DateTime? FundTransferredAt { get; set; }
        public DateTime? ClosedAt { get; set; }
        public string Note { get; set; }
        public int Id { get; set; }
        public string Number { get; set; }
        public DateTime OpenedAt { get; set; }
        public decimal OpeningCash { get; set; }
        public decimal CashSales { get; set; }
        public decimal CashAdjustments { get; set; }
        public decimal ExpectedCash { get; set; }
        public string Status { get; set; }
        public decimal? ClosingCash { get; set; }
        public decimal? Difference { get; set; }
    }
    public class ExpiryRow
    {
        public string ProductName { get; set; }
        public string BatchNumber { get; set; }
        public DateTime? ExpiryDate { get; set; }
        public int RemainingQuantity { get; set; }
        public int DaysLeft { get; set; }
    }
    public static class Pricing
    {
        public const decimal VipPercent = 10m;
        public static decimal Money(decimal value)
        {
            if (value < 0m || value > 999999999999.99m)
                throw new TradeException("Giá trị tiền vượt phạm vi cho phép.");
            return Math.Round(value, 2, MidpointRounding.AwayFromZero);
        }
        public static decimal Discount(decimal subtotal, bool vip)
        { return Money(subtotal * (vip ? VipPercent : 0m) / 100m); }
        
        public static decimal PromotionDiscount(decimal amountAfterVip, decimal percent)
        { return Money(amountAfterVip * percent / 100m); }
    }
}
