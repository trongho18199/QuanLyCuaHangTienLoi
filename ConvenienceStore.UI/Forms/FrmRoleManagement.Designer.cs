using System;
using System.Drawing;
using System.Windows.Forms;
namespace ConvenienceStore.UI.Forms
{
    partial class FrmRoleManagement
    {
        

        private void InitializeComponent()
        {
            Text = "Quản lý nhóm quyền";
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(820, 500);
            MinimumSize = new Size(760, 460);
            StartPosition = FormStartPosition.CenterParent;

            
            var grpRoles = new GroupBox
            {
                Text = "Nhóm quyền",
                Dock = DockStyle.Left,
                Width = 330,
                Padding = new Padding(10)
            };

            var pnlRoleButtons = new Panel { Dock = DockStyle.Bottom, Height = 90 };
            var lblName = new Label { Text = "Tên nhóm:", AutoSize = true, Location = new Point(0, 8) };
            txtRoleName.Location = new Point(70, 4);
            txtRoleName.Width = 235;
            txtRoleName.MaxLength = 50;

            btnAdd.Text = "Thêm mới";
            btnAdd.SetBounds(0, 42, 95, 30);
            btnAdd.Click += btnAdd_Click;
            btnRename.Text = "Đổi tên";
            btnRename.SetBounds(105, 42, 95, 30);
            btnRename.Click += btnRename_Click;
            btnDelete.Text = "Xóa";
            btnDelete.SetBounds(210, 42, 95, 30);
            btnDelete.Click += btnDelete_Click;
            pnlRoleButtons.Controls.AddRange(new Control[] { lblName, txtRoleName, btnAdd, btnRename, btnDelete });

            lstRoles.Dock = DockStyle.Fill;
            lstRoles.IntegralHeight = false;
            lstRoles.SelectedIndexChanged += lstRoles_SelectedIndexChanged;

            grpRoles.Controls.Add(lstRoles);
            grpRoles.Controls.Add(pnlRoleButtons);

            
            grpPermissions.Text = "Chức năng được phép sử dụng";
            grpPermissions.Dock = DockStyle.Fill;
            grpPermissions.Padding = new Padding(10);

            var pnlBottom = new Panel { Dock = DockStyle.Bottom, Height = 90 };
            lblNote.AutoSize = false;
            lblNote.SetBounds(0, 0, 440, 36);
            lblNote.ForeColor = Color.DimGray;
            lblNote.Text = NoteText;

            btnCheckAll.Text = "Chọn tất cả";
            btnCheckAll.SetBounds(0, 48, 105, 30);
            btnCheckAll.Click += (s, e) => SetAllChecked(true);
            btnUncheckAll.Text = "Bỏ chọn tất cả";
            btnUncheckAll.SetBounds(115, 48, 105, 30);
            btnUncheckAll.Click += (s, e) => SetAllChecked(false);
            btnSave.Text = "Lưu phân quyền";
            btnSave.SetBounds(240, 48, 130, 30);
            btnSave.Click += btnSave_Click;
            pnlBottom.Controls.AddRange(new Control[] { lblNote, btnCheckAll, btnUncheckAll, btnSave });

            clbPermissions.Dock = DockStyle.Fill;
            clbPermissions.CheckOnClick = true;
            clbPermissions.IntegralHeight = false;
            clbPermissions.ItemCheck += clbPermissions_ItemCheck;
            foreach (var p in PermissionCatalog.All) clbPermissions.Items.Add(p);

            grpPermissions.Controls.Add(clbPermissions);
            grpPermissions.Controls.Add(pnlBottom);

            Controls.Add(grpPermissions);   
            Controls.Add(grpRoles);
        }

    }
}
