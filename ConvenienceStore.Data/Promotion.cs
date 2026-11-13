using System;
using System.Data.Linq.Mapping;

namespace ConvenienceStore.Data
{
    
    
    
    
    
    
    [Table(Name = "dbo.Promotions")]
    public class Promotion
    {
        [Column(IsPrimaryKey = true, IsDbGenerated = true, AutoSync = AutoSync.OnInsert, DbType = "Int NOT NULL IDENTITY")]
        public int Id { get; set; }

        [Column(DbType = "NVarChar(30) NOT NULL", CanBeNull = false)]
        public string Code { get; set; }

        [Column(DbType = "NVarChar(200)", CanBeNull = true)]
        public string Description { get; set; }

        [Column(DbType = "Decimal(5,2) NOT NULL")]
        public decimal DiscountPercent { get; set; }

        [Column(DbType = "Date", CanBeNull = true)]
        public DateTime? ValidFrom { get; set; }

        [Column(DbType = "Date", CanBeNull = true)]
        public DateTime? ValidTo { get; set; }

        [Column(DbType = "Bit NOT NULL")]
        public bool IsActive { get; set; }
    }
}
