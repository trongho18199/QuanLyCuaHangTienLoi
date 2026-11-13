using System;
using System.Data;
using System.Data.Common;
using System.Data.Linq;

namespace ConvenienceStore.Data
{
    
    public partial class CSDbContext
    {
        private static string GetConnectionString() { return Database.DatabaseConfiguration.ConnectionString(); }

        public CSDbContext() : this(GetConnectionString()) { }

        
        public Table<RolePermission> RolePermissions
        {
            get { return this.GetTable<RolePermission>(); }
        }

        
        public Table<Promotion> Promotions
        {
            get { return this.GetTable<Promotion>(); }
        }
        public Table<SaleReturn> SaleReturns { get { return this.GetTable<SaleReturn>(); } }
        public Table<SaleReturnLine> SaleReturnLines { get { return this.GetTable<SaleReturnLine>(); } }

        
        public DbTransaction BeginTransaction(IsolationLevel level)
        {
            if (Connection.State != ConnectionState.Open) Connection.Open();
            Transaction = Connection.BeginTransaction(level);
            return Transaction;
        }
    }
}
