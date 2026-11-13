using System;
using System.Collections.Generic;
namespace ConvenienceStore.Business.Models
{
    
    public class CategoryInput { public int Id { get; set; } public string Code { get; set; } public string Name { get; set; } }
    public class CategoryRow
    {
        public int Id { get; set; }
        public string Code { get; set; }
        public string Name { get; set; }
        public int ProductCount { get; set; }
    }
    public class CategoryOption { public int Id { get; set; } public string Name { get; set; } }

    
    public class ProductInput
    {
        public int Id { get; set; }
        public string Barcode { get; set; }
        public string Name { get; set; }
        public int CategoryId { get; set; }
        public decimal Price { get; set; }
    }
    public class ProductRow
    {
        public int Id { get; set; }
        public string Barcode { get; set; }
        public string Name { get; set; }
        public int CategoryId { get; set; }
        public string CategoryName { get; set; }
        public decimal Price { get; set; }
        public int Stock { get; set; }
    }

    
    public class SupplierInput
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Phone { get; set; }
        public bool IsActive { get; set; }
    }
    public class SupplierRow
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Phone { get; set; }
        public bool IsActive { get; set; }
        public string Status { get { return IsActive ? "Hoạt động" : "Ngừng hoạt động"; } }
    }

    
    public class CustomerInput
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Phone { get; set; }
        public bool IsVip { get; set; }
        public bool IsActive { get; set; }
    }
    public class CustomerRow
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Phone { get; set; }
        public bool IsVip { get; set; }
        public bool IsActive { get; set; }
        public string Tier { get { return IsVip ? "VIP (giảm " + Pricing.VipPercent.ToString("0.##") + "%)" : "Thường"; } }
        public string Status { get { return IsActive ? "Hoạt động" : "Ngừng hoạt động"; } }
    }

    
    public class DeleteResult
    {
        public bool Deactivated { get; set; }
        public string Message { get; set; }
    }

    
    public class PromotionInput
    {
        public int Id { get; set; }
        public string Code { get; set; }
        public string Description { get; set; }
        public decimal DiscountPercent { get; set; }
        public DateTime? ValidFrom { get; set; }
        public DateTime? ValidTo { get; set; }
        public bool IsActive { get; set; }
    }
    public class PromotionRow
    {
        public int Id { get; set; }
        public string Code { get; set; }
        public string Description { get; set; }
        public decimal DiscountPercent { get; set; }
        public DateTime? ValidFrom { get; set; }
        public DateTime? ValidTo { get; set; }
        public bool IsActive { get; set; }
        public string Status
        {
            get
            {
                if (!IsActive) return "Ngừng áp dụng";
                DateTime today = DateTime.Today;
                if (ValidFrom.HasValue && today < ValidFrom.Value.Date) return "Chưa đến hạn";
                if (ValidTo.HasValue && today > ValidTo.Value.Date) return "Hết hạn";
                return "Đang áp dụng";
            }
        }
    }

    
    public class PurchaseListRow
    {
        public int Id { get; set; }
        public string Number { get; set; }
        public DateTime CreatedAt { get; set; }
        public string SupplierName { get; set; }
        public string CreatedBy { get; set; }
        public decimal Total { get; set; }
        public string Note { get; set; }
    }
    public class PurchaseLineRow
    {
        public int No { get; set; }
        public string ProductName { get; set; }
        public int Quantity { get; set; }
        public decimal UnitCost { get; set; }
        public decimal LineTotal { get; set; }
    }
    public class PurchaseDetail
    {
        public int Id { get; set; }
        public string Number { get; set; }
        public DateTime CreatedAt { get; set; }
        public string SupplierName { get; set; }
        public string CreatedBy { get; set; }
        public string Note { get; set; }
        public decimal Total { get; set; }
        public List<PurchaseLineRow> Lines { get; set; }
    }

    
    public class InvoiceListRow
    {
        public int Id { get; set; }
        public string Number { get; set; }
        public DateTime CreatedAt { get; set; }
        public string CustomerName { get; set; }
        public string Cashier { get; set; }
        public decimal Subtotal { get; set; }
        public decimal DiscountAmount { get; set; }
        public string PromotionCode { get; set; }
        public decimal Total { get; set; }
        public bool IsCancelled { get; set; }
        public string Status { get { return IsCancelled ? "Đã hủy" : "Hoàn tất"; } }
    }
    public class InvoiceLineRow
    {
        public int Id { get; set; }
        public int ProductId { get; set; }
        public int No { get; set; }
        public string ProductName { get; set; }
        public int Quantity { get; set; }
        public decimal UnitPrice { get; set; }
        public decimal LineTotal { get; set; }
    }
    public class InvoiceDetail
    {
        public int Id { get; set; }
        public Guid OperationId { get; set; }
        public string PaymentReference { get { return "TT" + OperationId.ToString("N").Substring(0, 12).ToUpperInvariant(); } }
        public string Number { get; set; }
        public DateTime CreatedAt { get; set; }
        public string CustomerName { get; set; }
        public string Cashier { get; set; }
        public decimal Subtotal { get; set; }
        
        public decimal DiscountPercent { get; set; }
        
        public decimal DiscountAmount { get; set; }
        public string PromotionCode { get; set; }
        public decimal PromotionAmount { get; set; }
        public decimal MemberDiscountAmount { get { return DiscountAmount - PromotionAmount; } }
        public decimal Total { get; set; }
        public decimal CashReceived { get; set; }
        public decimal Change { get; set; }
        public string PaymentMethod { get; set; }
        public bool IsCancelled { get; set; }
        public DateTime? CancelledAt { get; set; }
        public string CancelledBy { get; set; }
        public string CancelReason { get; set; }
        public List<InvoiceLineRow> Lines { get; set; }
    }
}
