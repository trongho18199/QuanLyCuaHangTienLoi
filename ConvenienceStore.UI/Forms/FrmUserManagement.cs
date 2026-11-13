using ConvenienceStore.Data.Repositories;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text.RegularExpressions;
using System.Windows.Forms;

namespace ConvenienceStore.UI.Forms
{
    
    public partial class FrmUserManagement : DevExpress.XtraEditors.XtraForm
    {
        private const int MinPasswordLength = 6;
        private static readonly Regex UsernameRule = new Regex(@"^[A-Za-z0-9._]{3,50}$");

        private readonly UserRepository _users = new UserRepository();
        private readonly RoleRepository _roles = new RoleRepository();

        
        private readonly TextBox txtSearch = new TextBox();
        private readonly DataGridView grid = new DataGridView();

        
        private readonly TextBox txtUsername = new TextBox();
        private readonly TextBox txtFullName = new TextBox();
        private readonly ComboBox cboRole = new ComboBox();
        private readonly TextBox txtPassword = new TextBox();
        private readonly CheckBox chkShowPassword = new CheckBox();
        private readonly CheckBox chkActive = new CheckBox();
        private readonly Label lblPasswordHint = new Label();
        private readonly Label lblMode = new Label();
        private readonly DevExpress.XtraEditors.SimpleButton btnNew = new DevExpress.XtraEditors.SimpleButton();
        private readonly DevExpress.XtraEditors.SimpleButton btnSave = new DevExpress.XtraEditors.SimpleButton();
        private readonly DevExpress.XtraEditors.SimpleButton btnReset = new DevExpress.XtraEditors.SimpleButton();
        private readonly DevExpress.XtraEditors.SimpleButton btnDelete = new DevExpress.XtraEditors.SimpleButton();

        private int _selectedId;      
        private bool _loading;

        public FrmUserManagement()
        {
            InitializeComponent();
            Load += FrmUserManagement_Load;
        }

        

        private void txtSearch_TextChanged(object sender, EventArgs e) { if (!_loading) LoadUsers(_selectedId); }

        private void FrmUserManagement_Load(object sender, EventArgs e)
        {
            try
            {
                cboRole.DisplayMember = "Name";
                cboRole.ValueMember = "Id";
                cboRole.DataSource = _roles.GetAll();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Không tải được nhóm quyền:\n" + ex.Message, "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            LoadUsers(0);
        }

        
        private void LoadUsers(int selectId)
        {
            _loading = true;
            List<UserListItem> list;
            try
            {
                list = _users.GetAll(txtSearch.Text);
            }
            catch (Exception ex)
            {
                _loading = false;
                MessageBox.Show("Không tải được danh sách tài khoản:\n" + ex.Message, "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            grid.DataSource = list;
            _loading = false;

            int index = selectId > 0 ? list.FindIndex(u => u.Id == selectId) : -1;
            if (index >= 0)
            {
                grid.ClearSelection();
                grid.Rows[index].Selected = true;
                grid.CurrentCell = grid.Rows[index].Cells[0];
                ShowUser(list[index]);
            }
            else
            {
                grid.ClearSelection();
                EnterNewMode(false);
            }
        }

        private void grid_SelectionChanged(object sender, EventArgs e)
        {
            if (_loading || grid.SelectedRows.Count == 0) return;
            var item = grid.SelectedRows[0].DataBoundItem as UserListItem;
            if (item != null) ShowUser(item);
        }

        private void grid_CellFormatting(object sender, DataGridViewCellFormattingEventArgs e)
        {
            if (e.RowIndex < 0) return;
            var item = grid.Rows[e.RowIndex].DataBoundItem as UserListItem;
            if (item != null && !item.IsActive)
                e.CellStyle.ForeColor = Color.Gray;
        }

        

        private bool IsSelf(int userId)
        {
            return AppSession.CurrentUser != null && AppSession.CurrentUser.Id == userId;
        }

        private void EnterNewMode(bool focusUsername = true)
        {
            _selectedId = 0;
            lblMode.Text = "Thêm tài khoản mới";
            txtUsername.Text = "";
            txtUsername.Enabled = true;
            txtFullName.Text = "";
            txtPassword.Text = "";
            if (cboRole.Items.Count > 0) cboRole.SelectedIndex = 0;
            cboRole.Enabled = true;
            chkActive.Checked = true;
            chkActive.Enabled = true;
            lblPasswordHint.Text = "Bắt buộc khi thêm mới (tối thiểu " + MinPasswordLength + " ký tự).";
            btnReset.Enabled = false;
            btnDelete.Enabled = false;
            if (focusUsername) txtUsername.Focus();
        }

        private void ShowUser(UserListItem u)
        {
            _selectedId = u.Id;
            bool self = IsSelf(u.Id);

            lblMode.Text = self ? "Sửa tài khoản (đây là tài khoản của bạn)" : "Sửa tài khoản";
            txtUsername.Text = u.Username;
            txtUsername.Enabled = false;
            txtFullName.Text = u.FullName ?? "";
            txtPassword.Text = "";
            cboRole.SelectedValue = u.RoleId;
            chkActive.Checked = u.IsActive;

            
            cboRole.Enabled = !self;
            chkActive.Enabled = !self;
            btnDelete.Enabled = !self;
            btnReset.Enabled = true;
            lblPasswordHint.Text = "Chỉ nhập khi muốn đặt lại mật khẩu, rồi bấm \"Đặt lại mật khẩu\".";
        }

        

        private void btnSave_Click(object sender, EventArgs e)
        {
            string username = txtUsername.Text.Trim();
            string fullName = txtFullName.Text.Trim();

            if (_selectedId == 0)
            {
                if (!UsernameRule.IsMatch(username))
                { Warn("Tên đăng nhập gồm 3-50 ký tự: chữ không dấu, số, dấu chấm hoặc gạch dưới, không có khoảng trắng.", txtUsername); return; }
                if (!CheckPassword(txtPassword.Text, username)) return;
            }
            if (fullName.Length == 0) { Warn("Vui lòng nhập họ tên.", txtFullName); return; }
            if (cboRole.SelectedValue == null) { Warn("Vui lòng chọn nhóm quyền.", cboRole); return; }

            int roleId = (int)cboRole.SelectedValue;

            try
            {
                int actingId = AppSession.CurrentUser.Id;
                if (_selectedId == 0)
                {
                    var add = _users.Add(username, txtPassword.Text, fullName, roleId, chkActive.Checked);
                    if (!add.Success) { Warn(add.Message, txtUsername); return; }

                    Info("Đã thêm tài khoản \"" + username + "\".");
                    var created = _users.GetAll(username).FirstOrDefault(u => u.Username == username);
                    txtSearch.Text = "";
                    LoadUsers(created != null ? created.Id : 0);
                }
                else
                {
                    var update = _users.Update(_selectedId, fullName, roleId, chkActive.Checked, actingId);
                    if (!update.Success) { Warn(update.Message, null); return; }

                    Info("Đã cập nhật tài khoản.");
                    LoadUsers(_selectedId);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Không lưu được tài khoản:\n" + ex.Message, "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void btnReset_Click(object sender, EventArgs e)
        {
            if (_selectedId == 0) return;
            if (!CheckPassword(txtPassword.Text, txtUsername.Text)) return;

            if (MessageBox.Show("Đặt lại mật khẩu của \"" + txtUsername.Text + "\" thành mật khẩu vừa nhập?",
                    "Xác nhận", MessageBoxButtons.YesNo, MessageBoxIcon.Question) != DialogResult.Yes)
                return;

            try
            {
                var result = _users.ResetPassword(_selectedId, txtPassword.Text);
                if (!result.Success) { Warn(result.Message, null); return; }

                txtPassword.Text = "";
                Info("Đã đặt lại mật khẩu.");
            }
            catch (Exception ex)
            {
                MessageBox.Show("Không đặt lại được mật khẩu:\n" + ex.Message, "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void btnDelete_Click(object sender, EventArgs e)
        {
            if (_selectedId == 0) return;

            if (MessageBox.Show("Xóa tài khoản \"" + txtUsername.Text + "\"? Thao tác này không hoàn tác được.\n" +
                                "(Nếu chỉ muốn ngăn đăng nhập, hãy bỏ tick \"đang hoạt động\" rồi Lưu.)",
                    "Xác nhận xóa", MessageBoxButtons.YesNo, MessageBoxIcon.Warning) != DialogResult.Yes)
                return;

            try
            {
                var result = _users.Delete(_selectedId, AppSession.CurrentUser.Id);
                if (!result.Success) { Warn(result.Message, null); return; }

                Info("Đã xóa tài khoản.");
                LoadUsers(0);
            }
            catch (Exception ex)
            {
                MessageBox.Show("Không xóa được tài khoản:\n" + ex.Message, "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        

        private bool CheckPassword(string password, string username)
        {
            if (password.Length < MinPasswordLength)
            { Warn("Mật khẩu phải có ít nhất " + MinPasswordLength + " ký tự.", txtPassword); return false; }
            if (password.Trim().Length == 0)
            { Warn("Mật khẩu không được chỉ gồm khoảng trắng.", txtPassword); return false; }
            if (string.Equals(password, username.Trim(), StringComparison.OrdinalIgnoreCase))
            { Warn("Mật khẩu không được trùng tên đăng nhập.", txtPassword); return false; }
            return true;
        }

        private static void Warn(string message, Control focus)
        {
            MessageBox.Show(message, "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            if (focus != null) focus.Focus();
        }

        private static void Info(string message)
        {
            MessageBox.Show(message, "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }
    }
}
