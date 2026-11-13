using ConvenienceStore.Data.Repositories;
using System;
using System.Windows.Forms;

namespace ConvenienceStore.UI.Forms
{
    public partial class FrmLogin : DevExpress.XtraEditors.XtraForm
    {
        public FrmLogin()
        {
            InitializeComponent();
        }

        private void btnLogin_Click(object sender, EventArgs e)
        {
            string username = txtUsername.Text.Trim();
            string password = txtPassword.Text;

            if (username.Length == 0 || password.Length == 0)
            {
                MessageBox.Show("Vui lòng nhập tên đăng nhập và mật khẩu.", "Thông báo",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            try
            {
                
                var user = new UserRepository().FindByCredentials(username, password);

                if (user == null)
                {
                    MessageBox.Show("Sai tên đăng nhập hoặc mật khẩu.", "Đăng nhập thất bại",
                        MessageBoxButtons.OK, MessageBoxIcon.Error);
                    txtPassword.Text = "";
                    txtPassword.Focus();
                    return;
                }

                if (!user.IsActive)
                {
                    MessageBox.Show("Tài khoản đã bị khóa.", "Đăng nhập thất bại",
                        MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }

                AppSession.CurrentUser = user;

                DialogResult = DialogResult.OK;
                Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Không kết nối được cơ sở dữ liệu:\n" + ex.Message, "Lỗi",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void btnExit_Click(object sender, EventArgs e)
        {
            DialogResult = DialogResult.Cancel;
            Close();
        }

        private void chkShowPassword_CheckedChanged(object sender, EventArgs e)
        {
            txtPassword.Properties.UseSystemPasswordChar = !chkShowPassword.Checked;
        }
    }
}
