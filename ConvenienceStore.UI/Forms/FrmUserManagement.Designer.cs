using System;
using System.Drawing;
using System.Windows.Forms;
namespace ConvenienceStore.UI.Forms
{
    partial class FrmUserManagement
    {
        

        private void InitializeComponent()
        {
            Text = "Quản lý tài khoản";
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(960, 540);
            MinimumSize = new Size(900, 520);
            StartPosition = FormStartPosition.CenterParent;

            
            var pnlTop = new Panel { Dock = DockStyle.Top, Height = 42 };
            pnlTop.Controls.Add(new Label { Text = "Tìm kiếm:", AutoSize = true, Location = new Point(12, 14) });
            txtSearch.SetBounds(80, 10, 260, 23);
            txtSearch.TextChanged += txtSearch_TextChanged;
            pnlTop.Controls.Add(txtSearch);
            pnlTop.Controls.Add(new Label
            {
                Text = "(theo tên đăng nhập hoặc họ tên)",
                AutoSize = true,
                ForeColor = Color.DimGray,
                Location = new Point(348, 14)
            });

            
            grid.Dock = DockStyle.Fill;
            grid.AutoGenerateColumns = false;
            grid.ReadOnly = true;
            grid.AllowUserToAddRows = false;
            grid.AllowUserToDeleteRows = false;
            grid.AllowUserToResizeRows = false;
            grid.MultiSelect = false;
            grid.RowHeadersVisible = false;
            grid.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            grid.BackgroundColor = SystemColors.Window;
            grid.Columns.Add(new DataGridViewTextBoxColumn { DataPropertyName = "Username", HeaderText = "Tên đăng nhập", Width = 130 });
            grid.Columns.Add(new DataGridViewTextBoxColumn { DataPropertyName = "FullName", HeaderText = "Họ tên", AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill });
            grid.Columns.Add(new DataGridViewTextBoxColumn { DataPropertyName = "RoleName", HeaderText = "Nhóm quyền", Width = 110 });
            grid.Columns.Add(new DataGridViewTextBoxColumn { DataPropertyName = "StatusText", HeaderText = "Trạng thái", Width = 90 });
            grid.SelectionChanged += grid_SelectionChanged;
            grid.CellFormatting += grid_CellFormatting;

            
            var grp = new GroupBox { Text = "Thông tin tài khoản", Dock = DockStyle.Right, Width = 350 };

            lblMode.AutoSize = true;
            lblMode.Font = new Font("Microsoft Sans Serif", 9F, FontStyle.Bold);
            lblMode.Location = new Point(15, 25);

            AddField(grp, "Tên đăng nhập (*):", txtUsername, 55);
            txtUsername.MaxLength = 50;
            AddField(grp, "Họ tên (*):", txtFullName, 90);
            txtFullName.MaxLength = 100;

            grp.Controls.Add(new Label { Text = "Nhóm quyền (*):", AutoSize = true, Location = new Point(15, 129) });
            cboRole.DropDownStyle = ComboBoxStyle.DropDownList;
            cboRole.SetBounds(125, 125, 205, 23);
            grp.Controls.Add(cboRole);

            AddField(grp, "Mật khẩu:", txtPassword, 160);
            txtPassword.MaxLength = 100;
            txtPassword.UseSystemPasswordChar = true;

            lblPasswordHint.AutoSize = false;
            lblPasswordHint.SetBounds(125, 186, 210, 30);
            lblPasswordHint.ForeColor = Color.DimGray;
            grp.Controls.Add(lblPasswordHint);

            chkShowPassword.Text = "Hiện mật khẩu";
            chkShowPassword.AutoSize = true;
            chkShowPassword.Location = new Point(125, 218);
            chkShowPassword.CheckedChanged += (s, e) => txtPassword.UseSystemPasswordChar = !chkShowPassword.Checked;
            grp.Controls.Add(chkShowPassword);

            chkActive.Text = "Tài khoản đang hoạt động";
            chkActive.AutoSize = true;
            chkActive.Location = new Point(125, 245);
            grp.Controls.Add(chkActive);

            btnNew.Text = "Thêm mới";
            btnNew.SetBounds(15, 295, 150, 32);
            btnNew.Click += (s, e) => EnterNewMode();

            btnSave.Text = "Lưu";
            btnSave.SetBounds(180, 295, 150, 32);
            btnSave.Click += btnSave_Click;

            btnReset.Text = "Đặt lại mật khẩu";
            btnReset.SetBounds(15, 337, 150, 32);
            btnReset.Click += btnReset_Click;

            btnDelete.Text = "Xóa tài khoản";
            btnDelete.SetBounds(180, 337, 150, 32);
            btnDelete.Click += btnDelete_Click;

            grp.Controls.AddRange(new Control[] { lblMode, btnNew, btnSave, btnReset, btnDelete });

            Controls.Add(grid);     
            Controls.Add(grp);
            Controls.Add(pnlTop);
        }

        private static void AddField(GroupBox grp, string caption, TextBox box, int y)
        {
            grp.Controls.Add(new Label { Text = caption, AutoSize = true, Location = new Point(15, y + 4) });
            box.SetBounds(125, y, 205, 23);
            grp.Controls.Add(box);
        }

    }
}
