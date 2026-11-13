using System;
using System.Data.SqlClient;
using System.IO;
using System.Xml;
using System.Xml.Linq;

namespace ConvenienceStore.Data.Database
{
    public sealed class DatabaseConnectionSettings
    {
        public string Server { get; set; }
        public string Database { get; set; }
        public bool WindowsAuthentication { get; set; }
        public string UserName { get; set; }
        public string Password { get; set; }

        public DatabaseConnectionSettings Clone()
        {
            return new DatabaseConnectionSettings { Server = Server, Database = Database,
                WindowsAuthentication = WindowsAuthentication, UserName = UserName, Password = Password };
        }

        public string ToConnectionString(string databaseName)
        {
            var builder = new SqlConnectionStringBuilder {
                DataSource = (Server ?? "").Trim(), InitialCatalog = databaseName,
                IntegratedSecurity = WindowsAuthentication, Encrypt = false,
                TrustServerCertificate = true, ConnectTimeout = 5 };
            if (!WindowsAuthentication) { builder.UserID = (UserName ?? "").Trim(); builder.Password = Password ?? ""; }
            return builder.ConnectionString;
        }
    }

    
    
    
    
    
    public static class DatabaseConfiguration
    {
        private static string pendingRecoveryNotice;

        public static string ConsumeRecoveryNotice()
        {
            var notice = pendingRecoveryNotice;
            pendingRecoveryNotice = null;
            return notice;
        }

        private static string SettingsFile
        {
            get
            {
                return Path.Combine(
                    Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData),
                    "ConvenienceStoreManager", "database.config");
            }
        }

        public static DatabaseConnectionSettings Load()
        {
            var defaults = ReadDefaults();
            var root = LoadSettingsDocument();
            if (root == null) return defaults;

            defaults.Server = Read(root, "Server", defaults.Server);
            defaults.Database = Read(root, "Database", defaults.Database);
            bool windows;
            if (Boolean.TryParse(Read(root, "WindowsAuthentication", "True"), out windows))
                defaults.WindowsAuthentication = windows;
            defaults.UserName = Read(root, "UserName", "");
            defaults.Password = Read(root, "Password", "");
            return defaults;
        }

        public static bool IsSetupCompleted()
        {
            var root = LoadSettingsDocument();
            bool completed;
            return root != null && Boolean.TryParse(Read(root, "SetupCompleted", "False"), out completed) && completed;
        }

        public static void Save(DatabaseConnectionSettings value)
        {
            if (value == null) throw new ArgumentNullException("value");
            if (String.IsNullOrWhiteSpace(value.Server) || String.IsNullOrWhiteSpace(value.Database))
                throw new ArgumentException("Server và tên database không được để trống.");

            string file = SettingsFile;
            string folder = Path.GetDirectoryName(file);
            Directory.CreateDirectory(folder);
            
            string temp = file + "." + Guid.NewGuid().ToString("N") + ".tmp";
            try
            {
                var document = new XDocument(
                    new XElement("DatabaseConfiguration",
                        new XElement("Server", value.Server),
                        new XElement("Database", value.Database),
                        new XElement("WindowsAuthentication", value.WindowsAuthentication),
                        new XElement("UserName", value.UserName ?? ""),
                        new XElement("Password", value.Password ?? ""),
                        new XElement("SetupCompleted", "True")));
                document.Save(temp);
                if (File.Exists(file))
                    File.Replace(temp, file, null);
                else
                    File.Move(temp, file);
            }
            finally
            {
                if (File.Exists(temp)) File.Delete(temp);
            }
        }

        public static string ConnectionString()
        {
            var value = Load();
            return value.ToConnectionString(value.Database);
        }

        private static XElement LoadSettingsDocument()
        {
            string file = SettingsFile;
            if (!File.Exists(file)) return null;
            try
            {
                using (var stream = new FileStream(file, FileMode.Open, FileAccess.Read, FileShare.Read))
                {
                    
                    var xmlSettings = new XmlReaderSettings { DtdProcessing = DtdProcessing.Prohibit, XmlResolver = null };
                    using (var reader = XmlReader.Create(stream, xmlSettings))
                    {
                        var document = XDocument.Load(reader);
                        if (document.Root == null || document.Root.Name != "DatabaseConfiguration")
                            throw new XmlException("Cấu hình database không đúng định dạng.");
                        return document.Root;
                    }
                }
            }
            catch (XmlException)
            {
                BackupCorruptSettings(file);
                return null;
            }
        }

        private static void BackupCorruptSettings(string file)
        {
            string backup = file + "." + DateTime.Now.ToString("yyyyMMdd_HHmmss_fff") + ".bak";
            for (int i = 1; File.Exists(backup); i++)
                backup = file + "." + DateTime.Now.ToString("yyyyMMdd_HHmmss_fff") + "." + i + ".bak";
            File.Move(file, backup);
            pendingRecoveryNotice = backup;
        }

        private static string Read(XElement root, string key, string fallback)
        {
            var item = root.Element(key);
            return item == null ? fallback : item.Value;
        }

        private static DatabaseConnectionSettings ReadDefaults()
        {
            
            
            var result = new DatabaseConnectionSettings
            {
                Server = @"(localdb)\MSSQLLocalDB",
                Database = "ConvenienceStoreDB",
                WindowsAuthentication = true,
                UserName = "",
                Password = ""
            };
            
            string configFile = AppDomain.CurrentDomain.SetupInformation.ConfigurationFile;
            if (!File.Exists(configFile)) return result;
            try
            {
                var xmlSettings = new XmlReaderSettings { DtdProcessing = DtdProcessing.Prohibit, XmlResolver = null };
                using (var reader = XmlReader.Create(configFile, xmlSettings))
                {
                    var document = XDocument.Load(reader);
                    var section = document.Root == null ? null : document.Root.Element("connectionStrings");
                    if (section == null) return result;
                    foreach (var add in section.Elements("add"))
                    {
                        if ((string)add.Attribute("name") != "ConvenienceStoreDB") continue;
                        var connectionString = (string)add.Attribute("connectionString");
                        if (String.IsNullOrEmpty(connectionString)) continue;
                        var connection = new SqlConnectionStringBuilder(connectionString);
                        if (!String.IsNullOrWhiteSpace(connection.DataSource)) result.Server = connection.DataSource;
                        if (!String.IsNullOrWhiteSpace(connection.InitialCatalog)) result.Database = connection.InitialCatalog;
                        result.WindowsAuthentication = connection.IntegratedSecurity;
                        result.UserName = connection.UserID;
                        result.Password = connection.Password;
                        break;
                    }
                }
            }
            catch (XmlException) {  }
            catch (ArgumentException) {  }
            return result;
        }
    }
}
