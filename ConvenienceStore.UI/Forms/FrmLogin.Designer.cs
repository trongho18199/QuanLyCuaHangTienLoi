namespace ConvenienceStore.UI.Forms
{
    partial class FrmLogin
    {
        private System.ComponentModel.IContainer components = null;

        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        private void InitializeComponent()
        {
            this.lblTitle = new DevExpress.XtraEditors.LabelControl();
            this.lblUsername = new DevExpress.XtraEditors.LabelControl();
            this.lblPassword = new DevExpress.XtraEditors.LabelControl();
            this.txtUsername = new DevExpress.XtraEditors.TextEdit();
            this.txtPassword = new DevExpress.XtraEditors.TextEdit();
            this.chkShowPassword = new DevExpress.XtraEditors.CheckEdit();
            this.btnLogin = new DevExpress.XtraEditors.SimpleButton();
            this.btnExit = new DevExpress.XtraEditors.SimpleButton();
            this.SuspendLayout();
            
            
            
            this.lblTitle.Font = new System.Drawing.Font("Microsoft Sans Serif", 14F, System.Drawing.FontStyle.Bold);
            this.lblTitle.Location = new System.Drawing.Point(60, 20);
            this.lblTitle.Name = "lblTitle";
            this.lblTitle.Size = new System.Drawing.Size(200, 24);
            this.lblTitle.TabIndex = 0;
            this.lblTitle.Text = "ĐĂNG NHẬP HỆ THỐNG";
            
            
            
            this.lblUsername.Location = new System.Drawing.Point(30, 70);
            this.lblUsername.Name = "lblUsername";
            this.lblUsername.Size = new System.Drawing.Size(88, 13);
            this.lblUsername.TabIndex = 1;
            this.lblUsername.Text = "Tên đăng nhập:";
            
            
            
            this.txtUsername.Location = new System.Drawing.Point(130, 67);
            this.txtUsername.Name = "txtUsername";
            this.txtUsername.Size = new System.Drawing.Size(200, 20);
            this.txtUsername.TabIndex = 2;
            
            
            
            this.lblPassword.Location = new System.Drawing.Point(30, 105);
            this.lblPassword.Name = "lblPassword";
            this.lblPassword.Size = new System.Drawing.Size(55, 13);
            this.lblPassword.TabIndex = 3;
            this.lblPassword.Text = "Mật khẩu:";
            
            
            
            this.txtPassword.Location = new System.Drawing.Point(130, 102);
            this.txtPassword.Name = "txtPassword";
            this.txtPassword.Size = new System.Drawing.Size(200, 20);
            this.txtPassword.TabIndex = 4;
            this.txtPassword.Properties.UseSystemPasswordChar = true;
            
            
            
            this.chkShowPassword.Location = new System.Drawing.Point(130, 130);
            this.chkShowPassword.Name = "chkShowPassword";
            this.chkShowPassword.Size = new System.Drawing.Size(101, 17);
            this.chkShowPassword.TabIndex = 5;
            this.chkShowPassword.Properties.Caption = "Hiện mật khẩu";
            this.chkShowPassword.CheckedChanged += new System.EventHandler(this.chkShowPassword_CheckedChanged);
            
            
            
            this.btnLogin.Location = new System.Drawing.Point(130, 165);
            this.btnLogin.Name = "btnLogin";
            this.btnLogin.Size = new System.Drawing.Size(95, 30);
            this.btnLogin.TabIndex = 6;
            this.btnLogin.Text = "Đăng nhập";
            this.btnLogin.Click += new System.EventHandler(this.btnLogin_Click);
            
            
            
            this.btnExit.Location = new System.Drawing.Point(235, 165);
            this.btnExit.Name = "btnExit";
            this.btnExit.Size = new System.Drawing.Size(95, 30);
            this.btnExit.TabIndex = 7;
            this.btnExit.Text = "Thoát";
            this.btnExit.Click += new System.EventHandler(this.btnExit_Click);
            
            
            
            this.AcceptButton = this.btnLogin;
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.CancelButton = this.btnExit;
            this.ClientSize = new System.Drawing.Size(380, 220);
            this.Controls.Add(this.lblTitle);
            this.Controls.Add(this.lblUsername);
            this.Controls.Add(this.txtUsername);
            this.Controls.Add(this.lblPassword);
            this.Controls.Add(this.txtPassword);
            this.Controls.Add(this.chkShowPassword);
            this.Controls.Add(this.btnLogin);
            this.Controls.Add(this.btnExit);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "FrmLogin";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Đăng nhập";
            this.ResumeLayout(false);
            this.PerformLayout();
        }

        #endregion

        private DevExpress.XtraEditors.LabelControl lblTitle;
        private DevExpress.XtraEditors.LabelControl lblUsername;
        private DevExpress.XtraEditors.LabelControl lblPassword;
        private DevExpress.XtraEditors.TextEdit txtUsername;
        private DevExpress.XtraEditors.TextEdit txtPassword;
        private DevExpress.XtraEditors.CheckEdit chkShowPassword;
        private DevExpress.XtraEditors.SimpleButton btnLogin;
        private DevExpress.XtraEditors.SimpleButton btnExit;
    }
}
