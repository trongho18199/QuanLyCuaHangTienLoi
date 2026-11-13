using ConvenienceStore.UI.Forms;
using System;
using System.Windows.Forms;
using ConvenienceStore.Data.Database;

namespace ConvenienceStore.UI
{
    internal static class Program
    {
        
        
        
        [STAThread]
        static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            DevExpress.LookAndFeel.UserLookAndFeel.Default.SetSkinStyle(DevExpress.LookAndFeel.SkinStyle.WXI);

            try
            {
                if (!EnsureDatabase()) return;
            }
            catch (System.Configuration.ConfigurationErrorsException ex)
            {
                MessageBox.Show(
                    "Không thể đọc cấu hình ứng dụng. Vui lòng kiểm tra file cấu hình hoặc liên hệ người quản trị.\n\n" + ex.Message,
                    "Lỗi cấu hình", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
            catch (System.IO.IOException ex)
            {
                MessageBox.Show(
                    "Không thể sao lưu/đọc cấu hình người dùng. Vui lòng kiểm tra quyền truy cập thư mục AppData.\n\n" + ex.Message,
                    "Lỗi cấu hình", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
            catch (UnauthorizedAccessException ex)
            {
                MessageBox.Show(
                    "Không có quyền truy cập file cấu hình người dùng.\n\n" + ex.Message,
                    "Lỗi cấu hình", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            using (var login = new FrmLogin())
            {
                if (login.ShowDialog() != DialogResult.OK)
                    return;
            }

            Application.Run(new FrmMain());
        }

        private static bool EnsureDatabase()
        {
            
            
            var completed = DatabaseConfiguration.IsSetupCompleted();
            ShowRecoveredConfigurationNotice();
            if (completed)
            {
                var settings = DatabaseConfiguration.Load();
                ShowRecoveredConfigurationNotice();
                if (DatabaseInitializer.CanConnect(settings)) return true;
            }

            using (var setup = new FrmDatabaseSetup())
            {
                ShowRecoveredConfigurationNotice();
                return setup.ShowDialog() == DialogResult.OK;
            }
        }
        private static void ShowRecoveredConfigurationNotice()
        {
            var backup = DatabaseConfiguration.ConsumeRecoveryNotice();
            if (!String.IsNullOrEmpty(backup))
            {
                MessageBox.Show(
                    "File cấu hình người dùng bị lỗi đã được sao lưu tại:\n" + backup +
                    "\n\nVui lòng kiểm tra và lưu lại cấu hình SQL Server.",
                    "Đã khôi phục cấu hình", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
        }
    }
}
