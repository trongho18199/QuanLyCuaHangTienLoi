using ConvenienceStore.Data.Repositories;
using System;
using System.Drawing;
using System.Windows.Forms;

namespace ConvenienceStore.UI.Forms
{
    
    public partial class FrmChangePassword : DevExpress.XtraEditors.XtraForm
    {
        private const int MinLength = 6;

        public FrmChangePassword()
        {
            InitializeComponent();
            lblUser.Text = "Tài khoản: " + (AppSession.CurrentUser != null ? AppSession.CurrentUser.Username : "");
            chkShow.CheckedChanged += (s, e) =>
            {
                bool hide = !chkShow.Checked;
                txtOld.Properties.UseSystemPasswordChar = hide;
                txtNew.Properties.UseSystemPasswordChar = hide;
                txtConfirm.Properties.UseSystemPasswordChar = hide;
            };
            btnSave.Click += btnSave_Click;
        }

        private void btnSave_Click(object sender, EventArgs e)
        {
            string oldPass = txtOld.Text;
            string newPass = txtNew.Text;
            string confirm = txtConfirm.Text;

            if (oldPass.Length == 0) { Warn("Vui lòng nhập mật khẩu hiện tại.", txtOld); return; }
            if (newPass.Length < MinLength) { Warn("Mật khẩu mới phải có ít nhất " + MinLength + " ký tự.", txtNew); return; }
            if (newPass.Trim().Length == 0) { Warn("Mật khẩu mới không được chỉ gồm khoảng trắng.", txtNew); return; }
            if (newPass == oldPass) { Warn("Mật khẩu mới phải khác mật khẩu hiện tại.", txtNew); return; }
            if (string.Equals(newPass, AppSession.CurrentUser.Username, StringComparison.OrdinalIgnoreCase))
            { Warn("Mật khẩu mới không được trùng tên đăng nhập.", txtNew); return; }
            if (newPass != confirm) { Warn("Nhập lại mật khẩu mới chưa khớp.", txtConfirm); return; }

            try
            {
                var result = new UserRepository().ChangePassword(AppSession.CurrentUser.Id, oldPass, newPass);
                if (!result.Success)
                {
                    Warn(result.Message, txtOld);
                    return;
                }

                MessageBox.Show("Đổi mật khẩu thành công.", "Thông báo",
                    MessageBoxButtons.OK, MessageBoxIcon.Information);
                DialogResult = DialogResult.OK;
                Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Không thể đổi mật khẩu:\n" + ex.Message, "Lỗi",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void Warn(string message, DevExpress.XtraEditors.TextEdit focus)
        {
            MessageBox.Show(message, "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            focus.SelectAll();
            focus.Focus();
        }
    }
}
