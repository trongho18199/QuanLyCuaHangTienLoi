using System;
using System.Data.Linq.Mapping;

namespace ConvenienceStore.Data
{
    [Table(Name = "dbo.SaleReturns")]
    public class SaleReturn
    {
        [Column(IsPrimaryKey = true, IsDbGenerated = true, DbType = "Int NOT NULL IDENTITY")]
        public int Id { get; set; }
        [Column(DbType = "NVarChar(40) NOT NULL")]
        public string Number { get; set; }
        [Column(DbType = "DateTime2 NOT NULL")]
        public DateTime CreatedAt { get; set; }
        [Column(DbType = "Int NOT NULL")]
        public int SaleInvoiceId { get; set; }
        [Column(DbType = "Int NOT NULL")]
        public int CreatedById { get; set; }
        [Column(DbType = "NVarChar(20) NOT NULL")]
        public string ReturnKind { get; set; }
        [Column(DbType = "NVarChar(200) NOT NULL")]
        public string Reason { get; set; }
        [Column(DbType = "Decimal(18,2) NOT NULL")]
        public decimal RefundAmount { get; set; }
        [Column(DbType = "Int")]
        public int? ReplacementProductId { get; set; }
        [Column(DbType = "NVarChar(100)")]
        public string ReplacementProductName { get; set; }
        [Column(DbType = "Int NOT NULL")]
        public int ReplacementQuantity { get; set; }
        [Column(DbType = "Decimal(18,2) NOT NULL")]
        public decimal ReplacementAmount { get; set; }
    }

    [Table(Name = "dbo.SaleReturnLines")]
    public class SaleReturnLine
    {
        [Column(IsPrimaryKey = true, IsDbGenerated = true, DbType = "Int NOT NULL IDENTITY")]
        public int Id { get; set; }
        [Column(DbType = "Int NOT NULL")]
        public int SaleReturnId { get; set; }
        [Column(DbType = "Int NOT NULL")]
        public int SaleInvoiceLineId { get; set; }
        [Column(DbType = "Int NOT NULL")]
        public int ProductId { get; set; }
        [Column(DbType = "NVarChar(100) NOT NULL")]
        public string ProductName { get; set; }
        [Column(DbType = "Int NOT NULL")]
        public int Quantity { get; set; }
        [Column(DbType = "Decimal(18,2) NOT NULL")]
        public decimal UnitPrice { get; set; }
        [Column(DbType = "Decimal(18,2) NOT NULL")]
        public decimal LineTotal { get; set; }
    }

    public partial class SaleInvoice
    {
        private string _PaymentMethod;
        [Column(DbType = "NVarChar(30) NOT NULL")]
        public string PaymentMethod
        {
            get { return _PaymentMethod ?? "Tiền mặt"; }
            set { _PaymentMethod = value; }
        }
        private int? _ShiftId;
        [Column(DbType = "Int")]
        public int? ShiftId { get { return _ShiftId; } set { _ShiftId = value; } }
    }

    public partial class PurchaseReceiptLine
    {
        [Column(DbType = "NVarChar(50)")] public string BatchNumber { get; set; }
        [Column(DbType = "Date")] public DateTime? ExpiryDate { get; set; }
    }

    [Table(Name = "dbo.SupplierPayables")]
    public class SupplierPayable
    {
        [Column(IsPrimaryKey = true, IsDbGenerated = true, DbType = "Int NOT NULL IDENTITY")] public int Id { get; set; }
        [Column(DbType = "Int NOT NULL")] public int PurchaseReceiptId { get; set; }
        [Column(DbType = "Decimal(18,2) NOT NULL")] public decimal OpeningPaid { get; set; }
        [Column(DbType = "DateTime2 NOT NULL")] public DateTime CreatedAt { get; set; }
        [Column(DbType = "Int NOT NULL")] public int CreatedById { get; set; }
    }
    [Table(Name = "dbo.SupplierPayments")]
    public class SupplierPayment
    {
        [Column(IsPrimaryKey = true, IsDbGenerated = true, DbType = "Int NOT NULL IDENTITY")] public int Id { get; set; }
        [Column(DbType = "Int NOT NULL")] public int PayableId { get; set; }
        [Column(DbType = "UniqueIdentifier NOT NULL")] public Guid OperationId { get; set; }
        [Column(DbType = "DateTime2 NOT NULL")] public DateTime CreatedAt { get; set; }
        [Column(DbType = "Int NOT NULL")] public int CreatedById { get; set; }
        [Column(DbType = "Decimal(18,2) NOT NULL")] public decimal Amount { get; set; }
        [Column(DbType = "NVarChar(30) NOT NULL")] public string Method { get; set; }
        [Column(DbType = "NVarChar(200)")] public string Note { get; set; }
    }

    [Table(Name = "dbo.CashFundReceipts")]
    public class CashFundReceipt
    {
        [Column(IsPrimaryKey = true, IsDbGenerated = true, DbType = "Int NOT NULL IDENTITY")] public int Id { get; set; }
        [Column(DbType = "Int NOT NULL")] public int ShiftId { get; set; }
        [Column(DbType = "DateTime2 NOT NULL")] public DateTime CreatedAt { get; set; }
        [Column(DbType = "Int NOT NULL")] public int CreatedById { get; set; }
        [Column(DbType = "Decimal(18,2) NOT NULL")] public decimal Amount { get; set; }
    }
    [Table(Name = "dbo.Shifts")]
    public class Shift
    {
        [Column(IsPrimaryKey = true, IsDbGenerated = true, DbType = "Int NOT NULL IDENTITY")]
        public int Id { get; set; }
        [Column(DbType = "NVarChar(40) NOT NULL")] public string Number { get; set; }
        [Column(DbType = "DateTime2 NOT NULL")] public DateTime OpenedAt { get; set; }
        [Column(DbType = "Int NOT NULL")] public int OpenedById { get; set; }
        [Column(DbType = "Decimal(18,2) NOT NULL")] public decimal OpeningCash { get; set; }
        [Column(DbType = "NVarChar(20) NOT NULL")] public string Status { get; set; }
        [Column(DbType = "DateTime2")] public DateTime? ClosedAt { get; set; }
        [Column(DbType = "Int")] public int? ClosedById { get; set; }
        [Column(DbType = "Decimal(18,2)")] public decimal? ClosingCash { get; set; }
        [Column(DbType = "Decimal(18,2)")] public decimal? ExpectedCash { get; set; }
        [Column(DbType = "Decimal(18,2)")] public decimal? Difference { get; set; }
        [Column(DbType = "NVarChar(300)")] public string Note { get; set; }
    }

    [Table(Name = "dbo.ShiftCashEntries")]
    public class ShiftCashEntry
    {
        [Column(IsPrimaryKey = true, IsDbGenerated = true, DbType = "Int NOT NULL IDENTITY")] public int Id { get; set; }
        [Column(DbType = "Int NOT NULL")] public int ShiftId { get; set; }
        [Column(DbType = "DateTime2 NOT NULL")] public DateTime CreatedAt { get; set; }
        [Column(DbType = "Int NOT NULL")] public int CreatedById { get; set; }
        [Column(DbType = "NVarChar(40) NOT NULL")] public string ReferenceNumber { get; set; }
        [Column(DbType = "NVarChar(100) NOT NULL")] public string Description { get; set; }
        [Column(DbType = "Decimal(18,2) NOT NULL")] public decimal Amount { get; set; }
    }

    [Table(Name = "dbo.ProductBatches")]
    public class ProductBatch
    {
        [Column(IsPrimaryKey = true, IsDbGenerated = true, DbType = "Int NOT NULL IDENTITY")]
        public int Id { get; set; }
        [Column(DbType = "Int NOT NULL")] public int ProductId { get; set; }
        [Column(DbType = "NVarChar(50) NOT NULL")] public string BatchNumber { get; set; }
        [Column(DbType = "Date")] public DateTime? ExpiryDate { get; set; }
        [Column(DbType = "DateTime2 NOT NULL")] public DateTime ReceivedAt { get; set; }
        [Column(DbType = "Int NOT NULL")] public int Quantity { get; set; }
        [Column(DbType = "Int NOT NULL")] public int RemainingQuantity { get; set; }
        [Column(DbType = "Int")] public int? PurchaseReceiptId { get; set; }
        [Column(DbType = "Decimal(18,2)")] public decimal? UnitCost { get; set; }
    }

    [Table(Name = "dbo.BatchMovements")]
    public class BatchMovement
    {
        [Column(IsPrimaryKey = true, IsDbGenerated = true, DbType = "Int NOT NULL IDENTITY")] public int Id { get; set; }
        [Column(DbType = "Int NOT NULL")] public int BatchId { get; set; }
        [Column(DbType = "Int NOT NULL")] public int ProductId { get; set; }
        [Column(DbType = "Int")] public int? SaleInvoiceLineId { get; set; }
        [Column(DbType = "DateTime2 NOT NULL")] public DateTime CreatedAt { get; set; }
        [Column(DbType = "NVarChar(40) NOT NULL")] public string ReferenceNumber { get; set; }
        [Column(DbType = "NVarChar(30) NOT NULL")] public string Kind { get; set; }
        [Column(DbType = "Int NOT NULL")] public int Quantity { get; set; }
        [Column(DbType = "Int NOT NULL")] public int CreatedById { get; set; }
        [Column(DbType = "Decimal(18,2)")] public decimal? UnitCost { get; set; }
    }
}
