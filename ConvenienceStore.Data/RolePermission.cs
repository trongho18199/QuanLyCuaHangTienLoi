using System.Data.Linq.Mapping;

namespace ConvenienceStore.Data
{
    
    
    
    
    
    
    [Table(Name = "dbo.RolePermissions")]
    public class RolePermission
    {
        [Column(IsPrimaryKey = true, IsDbGenerated = true, AutoSync = AutoSync.OnInsert, DbType = "Int NOT NULL IDENTITY")]
        public int Id { get; set; }

        [Column(DbType = "Int NOT NULL")]
        public int RoleId { get; set; }

        [Column(DbType = "NVarChar(100) NOT NULL", CanBeNull = false)]
        public string FunctionCode { get; set; }
    }
}
