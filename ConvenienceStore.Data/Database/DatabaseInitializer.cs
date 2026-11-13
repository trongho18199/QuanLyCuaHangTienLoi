using System;
using System.Data.SqlClient;
using System.IO;
using System.Text.RegularExpressions;

namespace ConvenienceStore.Data.Database
{
    public static class DatabaseInitializer
    {
        public static bool CanConnect(DatabaseConnectionSettings settings)
        {
            try { OpenValidationConnection(settings); return true; }
            catch (SqlException) { return false; }
        }

        private static void OpenValidationConnection(DatabaseConnectionSettings settings)
        {
            var connectionString = new SqlConnectionStringBuilder(settings.ToConnectionString(settings.Database));
            connectionString.Pooling = false;
            using (var connection = new SqlConnection(connectionString.ConnectionString))
                connection.Open();
        }

        public static void EnsureDatabase(DatabaseConnectionSettings settings)
        {
            Validate(settings);
            var scriptPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Database", "ConvenienceStoreDB.sql");
            if (!File.Exists(scriptPath)) throw new FileNotFoundException("Không tìm thấy script tạo CSDL.", scriptPath);
            string database = settings.Database.Trim();
            string script = File.ReadAllText(scriptPath).Replace("ConvenienceStoreDB", database);
            using (var connection = new SqlConnection(settings.ToConnectionString("master")))
            {
                connection.Open();
                foreach (string batch in Regex.Split(script, @"(?im)^\s*GO\s*$"))
                {
                    if (String.IsNullOrWhiteSpace(batch)) continue;
                    using (var command = connection.CreateCommand()) { command.CommandText = batch; command.CommandTimeout = 60; command.ExecuteNonQuery(); }
                }
            }
            try { OpenValidationConnection(settings); }
            catch (SqlException ex)
            {
                throw new InvalidOperationException("Đã tạo CSDL nhưng chưa kết nối lại được: " + ex.Message, ex);
            }
        }

        private static void Validate(DatabaseConnectionSettings settings)
        {
            if (settings == null || String.IsNullOrWhiteSpace(settings.Server)) throw new InvalidOperationException("Server không được để trống.");
            if (String.IsNullOrWhiteSpace(settings.Database) || !Regex.IsMatch(settings.Database.Trim(), "^[A-Za-z_][A-Za-z0-9_]*$"))
                throw new InvalidOperationException("Tên database chỉ được gồm chữ, số và dấu gạch dưới.");
            if (!settings.WindowsAuthentication && String.IsNullOrWhiteSpace(settings.UserName)) throw new InvalidOperationException("Hãy nhập tài khoản SQL Server.");
        }
    }
}
