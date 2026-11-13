using ConvenienceStore.Data.Repositories;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;

namespace ConvenienceStore.UI.Forms
{
    
    public partial class FrmRoleManagement : DevExpress.XtraEditors.XtraForm
    {
        private readonly RoleRepository _repo = new RoleRepository();

        private readonly ListBox lstRoles = new ListBox();
        private readonly TextBox txtRoleName = new TextBox();
        private readonly DevExpress.XtraEditors.SimpleButton btnAdd = new DevExpress.XtraEditors.SimpleButton();
        private readonly DevExpress.XtraEditors.SimpleButton btnRename = new DevExpress.XtraEditors.SimpleButton();
        private readonly DevExpress.XtraEditors.SimpleButton btnDelete = new DevExpress.XtraEditors.SimpleButton();

        private readonly GroupBox grpPermissions = new GroupBox();
        private readonly CheckedListBox clbPermissions = new CheckedListBox();
        private readonly DevExpress.XtraEditors.SimpleButton btnCheckAll = new DevExpress.XtraEditors.SimpleButton();
        private readonly DevExpress.XtraEditors.SimpleButton btnUncheckAll = new DevExpress.XtraEditors.SimpleButton();
        private readonly DevExpress.XtraEditors.SimpleButton btnSave = new DevExpress.XtraEditors.SimpleButton();
        private readonly Label lblNote = new Label();

        private const string NoteText =
            "Các chức năng \"Đổi mật khẩu\", \"Đăng xuất\", \"Thoát\" ai cũng dùng được. " +
            "Thay đổi áp dụng khi người dùng đăng nhập lại.";
        private const string AdminNoteText = "Nhóm Admin luôn có toàn quyền, không thể thay đổi.";

        private RoleListItem _current;   
        private bool _loading;           
        private bool _dirty;             

        
        public event EventHandler PermissionsSaved;

        public FrmRoleManagement()
        {
            InitializeComponent();
            Load += (s, e) => LoadRoles(null);
            FormClosing += FrmRoleManagement_FormClosing;
        }

        

        private void LoadRoles(int? selectId)
        {
            _loading = true;
            try
            {
                List<RoleListItem> roles = _repo.GetAll();
                lstRoles.DataSource = null;
                lstRoles.DataSource = roles;

                int id = selectId ?? (_current != null ? _current.Id : 0);
                var pick = roles.FirstOrDefault(r => r.Id == id) ?? roles.FirstOrDefault();
                if (pick != null) lstRoles.SelectedItem = pick;
                else lstRoles.ClearSelected();
            }
            catch (Exception ex)
            {
                ShowError("Không tải được danh sách nhóm quyền:\n" + ex.Message);
            }
            finally
            {
                _loading = false;
            }
            ShowRole(lstRoles.SelectedItem as RoleListItem);
        }

        private void ShowRole(RoleListItem role)
        {
            _loading = true;
            try
            {
                _current = role;
                for (int i = 0; i < clbPermissions.Items.Count; i++) clbPermissions.SetItemChecked(i, false);

                bool hasRole = role != null;
                bool isAdmin = hasRole && role.Name == RoleRepository.AdminRoleName;

                txtRoleName.Text = hasRole ? role.Name : "";
                btnRename.Enabled = hasRole && !isAdmin;
                btnDelete.Enabled = hasRole && !isAdmin;
                btnSave.Enabled = hasRole && !isAdmin;
                btnCheckAll.Enabled = hasRole && !isAdmin;
                btnUncheckAll.Enabled = hasRole && !isAdmin;
                clbPermissions.Enabled = !isAdmin;
                lblNote.Text = isAdmin ? AdminNoteText : NoteText;
                grpPermissions.Text = hasRole
                    ? "Chức năng được phép sử dụng - nhóm \"" + role.Name + "\""
                    : "Chức năng được phép sử dụng";

                if (hasRole)
                {
                    var codes = new HashSet<string>(_repo.GetPermissionCodes(role.Id));

                    for (int i = 0; i < clbPermissions.Items.Count; i++)
                        clbPermissions.SetItemChecked(i, isAdmin || codes.Contains(((PermissionItem)clbPermissions.Items[i]).Code));
                }
                _dirty = false;
            }
            catch (Exception ex)
            {
                ShowError("Không tải được phân quyền:\n" + ex.Message);
            }
            finally
            {
                _loading = false;
            }
        }

        

        private void lstRoles_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (_loading) return;

            var target = lstRoles.SelectedItem as RoleListItem;
            if (_current != null && target != null && target.Id == _current.Id) return;

            if (_dirty)
            {
                var answer = MessageBox.Show(
                    "Bạn đã thay đổi phân quyền của nhóm \"" + _current.Name + "\" nhưng chưa lưu.\nLưu thay đổi trước khi chuyển nhóm?",
                    "Chưa lưu", MessageBoxButtons.YesNoCancel, MessageBoxIcon.Question);

                if (answer == DialogResult.Cancel)
                {
                    SelectCurrentAgain();
                    return;
                }
                if (answer == DialogResult.Yes && !SaveCurrent())
                {
                    SelectCurrentAgain();
                    return;
                }
            }
            ShowRole(target);
        }

        private void SelectCurrentAgain()
        {
            _loading = true;
            var same = lstRoles.Items.Cast<RoleListItem>().FirstOrDefault(r => _current != null && r.Id == _current.Id);
            if (same != null) lstRoles.SelectedItem = same;
            _loading = false;
        }

        private void clbPermissions_ItemCheck(object sender, ItemCheckEventArgs e)
        {
            if (_loading) return;

            
            if (_current != null && _current.Name == RoleRepository.AdminRoleName && e.NewValue == CheckState.Unchecked)
            {
                var item = (PermissionItem)clbPermissions.Items[e.Index];
                if (RoleRepository.AdminLockedCodes.Contains(item.Code))
                {
                    e.NewValue = CheckState.Checked;
                    return;
                }
            }
            _dirty = true;
        }

        private void SetAllChecked(bool check)
        {
            if (_current == null) return;
            bool isAdmin = _current.Name == RoleRepository.AdminRoleName;

            _loading = true;
            for (int i = 0; i < clbPermissions.Items.Count; i++)
            {
                var item = (PermissionItem)clbPermissions.Items[i];
                bool locked = isAdmin && RoleRepository.AdminLockedCodes.Contains(item.Code);
                clbPermissions.SetItemChecked(i, locked || check);
            }
            _loading = false;
            _dirty = true;
        }

        private void btnSave_Click(object sender, EventArgs e)
        {
            if (SaveCurrent())
                MessageBox.Show("Đã lưu phân quyền cho nhóm \"" + _current.Name + "\".\nNgười dùng thuộc nhóm này cần đăng nhập lại để áp dụng.",
                    "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        private bool SaveCurrent()
        {
            if (_current == null) return false;
            try
            {
                var codes = clbPermissions.CheckedItems.Cast<PermissionItem>().Select(p => p.Code).ToList();
                var result = _repo.SavePermissions(_current.Id, codes);
                if (!result.Success) { ShowWarn(result.Message); return false; }

                _dirty = false;
                var handler = PermissionsSaved;
                if (handler != null) handler(this, EventArgs.Empty);
                return true;
            }
            catch (Exception ex)
            {
                ShowError("Không lưu được phân quyền:\n" + ex.Message);
                return false;
            }
        }

        private void btnAdd_Click(object sender, EventArgs e)
        {
            if (!ConfirmDiscardChanges()) return;
            try
            {
                string name = txtRoleName.Text.Trim();
                var result = _repo.Add(name);
                if (!result.Success) { ShowWarn(result.Message); txtRoleName.Focus(); return; }

                _dirty = false;
                LoadRoles(_repo.GetAll().First(r => r.Name == name).Id);
            }
            catch (Exception ex) { ShowError("Không thêm được nhóm quyền:\n" + ex.Message); }
        }

        private void btnRename_Click(object sender, EventArgs e)
        {
            if (_current == null) return;
            try
            {
                var result = _repo.Rename(_current.Id, txtRoleName.Text);
                if (!result.Success) { ShowWarn(result.Message); txtRoleName.Focus(); return; }

                int id = _current.Id;
                _dirty = false;
                LoadRoles(id);
            }
            catch (Exception ex) { ShowError("Không đổi tên được:\n" + ex.Message); }
        }

        private void btnDelete_Click(object sender, EventArgs e)
        {
            if (_current == null) return;

            if (MessageBox.Show("Xóa nhóm quyền \"" + _current.Name + "\"?\nToàn bộ phân quyền của nhóm cũng bị xóa.",
                    "Xác nhận xóa", MessageBoxButtons.YesNo, MessageBoxIcon.Question) != DialogResult.Yes)
                return;

            try
            {
                var result = _repo.Delete(_current.Id);
                if (!result.Success) { ShowWarn(result.Message); return; }

                _current = null;
                _dirty = false;
                LoadRoles(null);
            }
            catch (Exception ex) { ShowError("Không xóa được nhóm quyền:\n" + ex.Message); }
        }

        
        private bool ConfirmDiscardChanges()
        {
            if (!_dirty) return true;
            return MessageBox.Show("Phân quyền đang sửa chưa được lưu và sẽ bị bỏ. Tiếp tục?",
                "Chưa lưu", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes;
        }

        private void FrmRoleManagement_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (!_dirty) return;
            var answer = MessageBox.Show("Phân quyền đang sửa chưa được lưu. Lưu trước khi đóng?",
                "Chưa lưu", MessageBoxButtons.YesNoCancel, MessageBoxIcon.Question);
            if (answer == DialogResult.Cancel) e.Cancel = true;
            else if (answer == DialogResult.Yes && !SaveCurrent()) e.Cancel = true;
        }

        private static void ShowWarn(string msg)
        {
            MessageBox.Show(msg, "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
        }

        private static void ShowError(string msg)
        {
            MessageBox.Show(msg, "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
        }
    }
}
