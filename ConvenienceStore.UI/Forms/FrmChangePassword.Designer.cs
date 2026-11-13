namespace ConvenienceStore.UI.Forms
{
    partial class FrmChangePassword
    {
        private DevExpress.XtraEditors.TextEdit txtOld, txtNew, txtConfirm;
        private DevExpress.XtraEditors.CheckEdit chkShow;
        private DevExpress.XtraEditors.SimpleButton btnSave, btnCancel;
        private DevExpress.XtraEditors.LabelControl lblTitle, lblUser, lblOld, lblNew, lblConfirm, lblHint;

        private void InitializeComponent()
        {
            this.txtOld = new DevExpress.XtraEditors.TextEdit();
            this.txtNew = new DevExpress.XtraEditors.TextEdit();
            this.txtConfirm = new DevExpress.XtraEditors.TextEdit();
            this.chkShow = new DevExpress.XtraEditors.CheckEdit();
            this.btnSave = new DevExpress.XtraEditors.SimpleButton();
            this.btnCancel = new DevExpress.XtraEditors.SimpleButton();
            this.lblTitle = new DevExpress.XtraEditors.LabelControl();
            this.lblUser = new DevExpress.XtraEditors.LabelControl();
            this.lblOld = new DevExpress.XtraEditors.LabelControl();
            this.lblNew = new DevExpress.XtraEditors.LabelControl();
            this.lblConfirm = new DevExpress.XtraEditors.LabelControl();
            this.lblHint = new DevExpress.XtraEditors.LabelControl();
            ((System.ComponentModel.ISupportInitialize)(this.txtOld.Properties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.txtNew.Properties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.txtConfirm.Properties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.chkShow.Properties)).BeginInit();
            this.SuspendLayout();
            this.lblTitle.Name = "lblTitle"; this.lblTitle.Text = "ĐỔI MẬT KHẨU";
            this.lblTitle.Appearance.Font = new System.Drawing.Font("Segoe UI", 14F, System.Drawing.FontStyle.Bold);
            this.lblTitle.Location = new System.Drawing.Point(20, 15);
            this.lblUser.Name = "lblUser"; this.lblUser.Text = "Tài khoản:"; this.lblUser.Location = new System.Drawing.Point(22, 48);
            this.lblOld.Name = "lblOld"; this.lblOld.Text = "Mật khẩu hiện tại:"; this.lblOld.Location = new System.Drawing.Point(20, 84);
            this.lblNew.Name = "lblNew"; this.lblNew.Text = "Mật khẩu mới:"; this.lblNew.Location = new System.Drawing.Point(20, 116);
            this.lblConfirm.Name = "lblConfirm"; this.lblConfirm.Text = "Nhập lại mật khẩu mới:"; this.lblConfirm.Location = new System.Drawing.Point(20, 148);
            this.txtOld.Name = "txtOld"; this.txtOld.Location = new System.Drawing.Point(180, 80); this.txtOld.Size = new System.Drawing.Size(215, 22); this.txtOld.Properties.UseSystemPasswordChar = true; this.txtOld.Properties.MaxLength = 100;
            this.txtNew.Name = "txtNew"; this.txtNew.Location = new System.Drawing.Point(180, 112); this.txtNew.Size = new System.Drawing.Size(215, 22); this.txtNew.Properties.UseSystemPasswordChar = true; this.txtNew.Properties.MaxLength = 100;
            this.txtConfirm.Name = "txtConfirm"; this.txtConfirm.Location = new System.Drawing.Point(180, 144); this.txtConfirm.Size = new System.Drawing.Size(215, 22); this.txtConfirm.Properties.UseSystemPasswordChar = true; this.txtConfirm.Properties.MaxLength = 100;
            this.lblHint.Name = "lblHint"; this.lblHint.Text = "Mật khẩu mới tối thiểu 6 ký tự."; this.lblHint.Location = new System.Drawing.Point(180, 172);
            this.chkShow.Name = "chkShow"; this.chkShow.Properties.Caption = "Hiện mật khẩu"; this.chkShow.Location = new System.Drawing.Point(180, 194); this.chkShow.Size = new System.Drawing.Size(160, 24);
            this.btnSave.Name = "btnSave"; this.btnSave.Text = "Lưu"; this.btnSave.Location = new System.Drawing.Point(210, 226); this.btnSave.Size = new System.Drawing.Size(90, 30);
            this.btnCancel.Name = "btnCancel"; this.btnCancel.Text = "Hủy"; this.btnCancel.Location = new System.Drawing.Point(310, 226); this.btnCancel.Size = new System.Drawing.Size(90, 30); this.btnCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.AcceptButton = this.btnSave; this.CancelButton = this.btnCancel;
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font; this.ClientSize = new System.Drawing.Size(420, 270);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog; this.MaximizeBox = false; this.MinimizeBox = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent; this.Text = "Đổi mật khẩu"; this.Name = "FrmChangePassword";
            this.Controls.AddRange(new System.Windows.Forms.Control[] { this.lblTitle, this.lblUser, this.lblOld, this.lblNew, this.lblConfirm, this.txtOld, this.txtNew, this.txtConfirm, this.lblHint, this.chkShow, this.btnSave, this.btnCancel });
            ((System.ComponentModel.ISupportInitialize)(this.txtOld.Properties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.txtNew.Properties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.txtConfirm.Properties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.chkShow.Properties)).EndInit();
            this.ResumeLayout(false); this.PerformLayout();
        }
    }
}
