using System;
using System.Windows.Forms;
using ConvenienceStore.Data.Database;

namespace ConvenienceStore.UI.Forms
{
    public partial class FrmDatabaseSetup : DevExpress.XtraEditors.XtraForm
    {
        public DatabaseConnectionSettings Settings { get; private set; }

        public FrmDatabaseSetup()
        {
            InitializeComponent();
            var value = DatabaseConfiguration.Load();
            txtServer.Text = value.Server;
            txtDatabase.Text = value.Database;
            chkWindows.Checked = value.WindowsAuthentication;
            txtUser.Text = value.UserName;
            txtPassword.Text = value.Password;
            UpdateAuthenticationState();
            chkWindows.CheckedChanged += (s, e) => UpdateAuthenticationState();
            btnTest.Click += (s, e) => TestConnection(false);
            btnSave.Click += (s, e) => TestConnection(true);
        }

        private DatabaseConnectionSettings ReadSettings()
        {
            return new DatabaseConnectionSettings { Server = txtServer.Text.Trim(), Database = txtDatabase.Text.Trim(),
                WindowsAuthentication = chkWindows.Checked, UserName = txtUser.Text.Trim(), Password = txtPassword.Text };
        }

        private void TestConnection(bool saveAndCreate)
        {
            try
            {
                var value = ReadSettings();
                if (saveAndCreate)
                {
                    
                    if (!DatabaseInitializer.CanConnect(value)) DatabaseInitializer.EnsureDatabase(value);
                    
                    DatabaseConfiguration.Save(value);
                    var backup = DatabaseConfiguration.ConsumeRecoveryNotice();
                    if (!String.IsNullOrEmpty(backup))
                        MessageBox.Show(this, "Đã sao lưu cấu hình lỗi tại:\n" + backup,
                            "Khôi phục cấu hình", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    Settings = value;
                    MessageBox.Show(this, "Đã lưu cấu hình và sẵn sàng sử dụng CSDL.", "Thành công", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    DialogResult = DialogResult.OK;
                }
                else
                {
                    
                    if (!DatabaseInitializer.CanConnect(value))
                        throw new InvalidOperationException("Không thể kết nối. Nếu database chưa tồn tại, hãy bấm Lưu và tạo CSDL.");
                    MessageBox.Show(this, "Kết nối SQL Server thành công.", "Thành công", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
            }
            catch (Exception ex) { MessageBox.Show(this, ex.ToString(), "Chi tiết lỗi cấu hình CSDL", MessageBoxButtons.OK, MessageBoxIcon.Warning); }
        }

        private void UpdateAuthenticationState()
        {
            txtUser.Enabled = !chkWindows.Checked; txtPassword.Enabled = !chkWindows.Checked;
        }
    }
}
