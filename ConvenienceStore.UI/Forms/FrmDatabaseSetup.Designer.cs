namespace ConvenienceStore.UI.Forms
{
    partial class FrmDatabaseSetup
    {
        private System.ComponentModel.IContainer components = null;
        private DevExpress.XtraEditors.LabelControl lblTitle, lblServer, lblDatabase, lblUser, lblPassword;
        private DevExpress.XtraEditors.TextEdit txtServer, txtDatabase, txtUser, txtPassword;
        private DevExpress.XtraEditors.CheckEdit chkWindows;
        private DevExpress.XtraEditors.SimpleButton btnTest, btnSave, btnCancel;

        protected override void Dispose(bool disposing) { if (disposing && components != null) components.Dispose(); base.Dispose(disposing); }

        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            this.lblTitle = new DevExpress.XtraEditors.LabelControl(); this.lblServer = new DevExpress.XtraEditors.LabelControl(); this.lblDatabase = new DevExpress.XtraEditors.LabelControl(); this.lblUser = new DevExpress.XtraEditors.LabelControl(); this.lblPassword = new DevExpress.XtraEditors.LabelControl();
            this.txtServer = new DevExpress.XtraEditors.TextEdit(); this.txtDatabase = new DevExpress.XtraEditors.TextEdit(); this.txtUser = new DevExpress.XtraEditors.TextEdit(); this.txtPassword = new DevExpress.XtraEditors.TextEdit(); this.chkWindows = new DevExpress.XtraEditors.CheckEdit();
            this.btnTest = new DevExpress.XtraEditors.SimpleButton(); this.btnSave = new DevExpress.XtraEditors.SimpleButton(); this.btnCancel = new DevExpress.XtraEditors.SimpleButton();
            ((System.ComponentModel.ISupportInitialize)(this.txtServer.Properties)).BeginInit(); ((System.ComponentModel.ISupportInitialize)(this.txtDatabase.Properties)).BeginInit(); ((System.ComponentModel.ISupportInitialize)(this.txtUser.Properties)).BeginInit(); ((System.ComponentModel.ISupportInitialize)(this.txtPassword.Properties)).BeginInit(); ((System.ComponentModel.ISupportInitialize)(this.chkWindows.Properties)).BeginInit(); this.SuspendLayout();
            this.lblTitle.Appearance.Font = new System.Drawing.Font("Segoe UI", 15F, System.Drawing.FontStyle.Bold); this.lblTitle.Appearance.Options.UseFont = true; this.lblTitle.Location = new System.Drawing.Point(24, 20); this.lblTitle.Size = new System.Drawing.Size(500, 28); this.lblTitle.Text = "CẤU HÌNH KẾT NỐI CƠ SỞ DỮ LIỆU";
            this.lblServer.Location = new System.Drawing.Point(24, 78); this.lblServer.Text = "SQL Server / Server"; this.lblServer.Size = new System.Drawing.Size(180, 20); this.txtServer.Location = new System.Drawing.Point(220, 74); this.txtServer.Size = new System.Drawing.Size(330, 28);
            this.lblDatabase.Location = new System.Drawing.Point(24, 120); this.lblDatabase.Text = "Tên database"; this.lblDatabase.Size = new System.Drawing.Size(180, 20); this.txtDatabase.Location = new System.Drawing.Point(220, 116); this.txtDatabase.Size = new System.Drawing.Size(330, 28);
            this.chkWindows.Location = new System.Drawing.Point(220, 158); this.chkWindows.Properties.Caption = "Windows Authentication"; this.chkWindows.Size = new System.Drawing.Size(330, 24); this.chkWindows.Checked = true;
            this.lblUser.Location = new System.Drawing.Point(24, 202); this.lblUser.Text = "Tài khoản SQL"; this.lblUser.Size = new System.Drawing.Size(180, 20); this.txtUser.Location = new System.Drawing.Point(220, 198); this.txtUser.Size = new System.Drawing.Size(330, 28);
            this.lblPassword.Location = new System.Drawing.Point(24, 244); this.lblPassword.Text = "Mật khẩu SQL"; this.lblPassword.Size = new System.Drawing.Size(180, 20); this.txtPassword.Location = new System.Drawing.Point(220, 240); this.txtPassword.Size = new System.Drawing.Size(330, 28); this.txtPassword.Properties.PasswordChar = '*';
            this.btnTest.Location = new System.Drawing.Point(24, 310); this.btnTest.Size = new System.Drawing.Size(160, 40); this.btnTest.Text = "Kiểm tra kết nối";
            this.btnSave.Appearance.BackColor = System.Drawing.Color.FromArgb(17, 121, 114); this.btnSave.Appearance.ForeColor = System.Drawing.Color.White; this.btnSave.Appearance.Options.UseBackColor = true; this.btnSave.Appearance.Options.UseForeColor = true; this.btnSave.Location = new System.Drawing.Point(220, 310); this.btnSave.Size = new System.Drawing.Size(160, 40); this.btnSave.Text = "Lưu và tạo CSDL";
            this.btnCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel; this.btnCancel.Location = new System.Drawing.Point(390, 310); this.btnCancel.Size = new System.Drawing.Size(160, 40); this.btnCancel.Text = "Thoát";
            this.AcceptButton = this.btnSave; this.CancelButton = this.btnCancel; this.AutoScaleDimensions = new System.Drawing.SizeF(96F, 96F); this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Dpi; this.ClientSize = new System.Drawing.Size(584, 385); this.Controls.AddRange(new System.Windows.Forms.Control[] { this.lblTitle, this.lblServer, this.txtServer, this.lblDatabase, this.txtDatabase, this.chkWindows, this.lblUser, this.txtUser, this.lblPassword, this.txtPassword, this.btnTest, this.btnSave, this.btnCancel }); this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog; this.MaximizeBox = false; this.MinimizeBox = false; this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen; this.Text = "Cấu hình CSDL";
            ((System.ComponentModel.ISupportInitialize)(this.txtServer.Properties)).EndInit(); ((System.ComponentModel.ISupportInitialize)(this.txtDatabase.Properties)).EndInit(); ((System.ComponentModel.ISupportInitialize)(this.txtUser.Properties)).EndInit(); ((System.ComponentModel.ISupportInitialize)(this.txtPassword.Properties)).EndInit(); ((System.ComponentModel.ISupportInitialize)(this.chkWindows.Properties)).EndInit(); this.ResumeLayout(false); this.PerformLayout();
        }
    }
}
