namespace ConvenienceStore.UI.Forms
{
    partial class FrmQrPayment
    {
        private System.ComponentModel.IContainer components = null;
        private DevExpress.XtraEditors.LabelControl lblTitle;
        private DevExpress.XtraEditors.LabelControl lblAccount;
        private DevExpress.XtraEditors.LabelControl lblAmount;
        private DevExpress.XtraEditors.LabelControl lblReference;
        private DevExpress.XtraEditors.LabelControl lblLoadStatus;
        private DevExpress.XtraEditors.PictureEdit pictureQr;
        private DevExpress.XtraEditors.TextEdit txtReference;
        private DevExpress.XtraEditors.CheckEdit chkReceived;
        private DevExpress.XtraEditors.SimpleButton btnConfirm;
        private DevExpress.XtraEditors.SimpleButton btnCancel;

        protected override void Dispose(bool disposing)
        {
            if (disposing && components != null) components.Dispose();
            base.Dispose(disposing);
        }

        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            this.lblTitle = new DevExpress.XtraEditors.LabelControl();
            this.lblAccount = new DevExpress.XtraEditors.LabelControl();
            this.lblAmount = new DevExpress.XtraEditors.LabelControl();
            this.lblReference = new DevExpress.XtraEditors.LabelControl();
            this.lblLoadStatus = new DevExpress.XtraEditors.LabelControl();
            this.pictureQr = new DevExpress.XtraEditors.PictureEdit();
            this.txtReference = new DevExpress.XtraEditors.TextEdit();
            this.chkReceived = new DevExpress.XtraEditors.CheckEdit();
            this.btnConfirm = new DevExpress.XtraEditors.SimpleButton();
            this.btnCancel = new DevExpress.XtraEditors.SimpleButton();
            ((System.ComponentModel.ISupportInitialize)(this.pictureQr.Properties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.txtReference.Properties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.chkReceived.Properties)).BeginInit();
            this.SuspendLayout();
            this.lblTitle.Appearance.Font = new System.Drawing.Font("Segoe UI", 16F, System.Drawing.FontStyle.Bold);
            this.lblTitle.Appearance.Options.UseFont = true;
            this.lblTitle.Location = new System.Drawing.Point(24, 20);
            this.lblTitle.Name = "lblTitle";
            this.lblTitle.Size = new System.Drawing.Size(274, 30);
            this.lblTitle.Text = "THANH TOÁN VIETQR";
            this.lblAccount.Appearance.Font = new System.Drawing.Font("Segoe UI", 11F, System.Drawing.FontStyle.Bold);
            this.lblAccount.Appearance.Options.UseFont = true;
            this.lblAccount.Location = new System.Drawing.Point(24, 65);
            this.lblAccount.Name = "lblAccount";
            this.lblAccount.Size = new System.Drawing.Size(236, 20);
            this.lblAccount.Text = "Vietcombank - 1060498270";
            this.lblAmount.Appearance.Font = new System.Drawing.Font("Segoe UI", 14F, System.Drawing.FontStyle.Bold);
            this.lblAmount.Appearance.ForeColor = System.Drawing.Color.FromArgb(17, 121, 114);
            this.lblAmount.Appearance.Options.UseFont = true;
            this.lblAmount.Appearance.Options.UseForeColor = true;
            this.lblAmount.Location = new System.Drawing.Point(24, 96);
            this.lblAmount.Name = "lblAmount";
            this.lblAmount.Size = new System.Drawing.Size(147, 25);
            this.lblAmount.Text = "Số tiền: 0 đ";
            this.pictureQr.Location = new System.Drawing.Point(24, 138);
            this.pictureQr.Name = "pictureQr";
            this.pictureQr.Properties.AllowFocused = false;
            this.pictureQr.Properties.NullText = "Đang tải mã QR...";
            this.pictureQr.Properties.ReadOnly = true;
            this.pictureQr.Properties.ShowCameraMenuItem = DevExpress.XtraEditors.Controls.CameraMenuItemVisibility.Auto;
            this.pictureQr.Properties.ShowMenu = false;
            this.pictureQr.Properties.SizeMode = DevExpress.XtraEditors.Controls.PictureSizeMode.Zoom;
            this.pictureQr.Size = new System.Drawing.Size(430, 430);
            this.pictureQr.TabIndex = 0;
            this.lblReference.Location = new System.Drawing.Point(478, 143);
            this.lblReference.Name = "lblReference";
            this.lblReference.Size = new System.Drawing.Size(118, 17);
            this.lblReference.Text = "Nội dung chuyển khoản";
            this.txtReference.Location = new System.Drawing.Point(478, 168);
            this.txtReference.Name = "txtReference";
            this.txtReference.Properties.ReadOnly = true;
            this.txtReference.Size = new System.Drawing.Size(280, 24);
            this.txtReference.TabIndex = 1;
            this.lblLoadStatus.AutoSizeMode = DevExpress.XtraEditors.LabelAutoSizeMode.None;
            this.lblLoadStatus.Location = new System.Drawing.Point(478, 213);
            this.lblLoadStatus.Name = "lblLoadStatus";
            this.lblLoadStatus.Size = new System.Drawing.Size(280, 48);
            this.lblLoadStatus.Text = "Đang tải mã VietQR...";
            this.chkReceived.Location = new System.Drawing.Point(478, 300);
            this.chkReceived.Name = "chkReceived";
            this.chkReceived.Properties.Caption = "Đã kiểm tra và nhận đủ tiền";
            this.chkReceived.Size = new System.Drawing.Size(280, 24);
            this.chkReceived.TabIndex = 2;
            this.btnConfirm.Appearance.BackColor = System.Drawing.Color.FromArgb(17, 121, 114);
            this.btnConfirm.Appearance.ForeColor = System.Drawing.Color.White;
            this.btnConfirm.Appearance.Options.UseBackColor = true;
            this.btnConfirm.Appearance.Options.UseForeColor = true;
            this.btnConfirm.DialogResult = System.Windows.Forms.DialogResult.OK;
            this.btnConfirm.Location = new System.Drawing.Point(478, 350);
            this.btnConfirm.Name = "btnConfirm";
            this.btnConfirm.Size = new System.Drawing.Size(132, 40);
            this.btnConfirm.TabIndex = 3;
            this.btnConfirm.Text = "Xác nhận";
            this.btnCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.btnCancel.Location = new System.Drawing.Point(626, 350);
            this.btnCancel.Name = "btnCancel";
            this.btnCancel.Size = new System.Drawing.Size(132, 40);
            this.btnCancel.TabIndex = 4;
            this.btnCancel.Text = "Hủy";
            this.AcceptButton = this.btnConfirm;
            this.AutoScaleDimensions = new System.Drawing.SizeF(96F, 96F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Dpi;
            this.CancelButton = this.btnCancel;
            this.ClientSize = new System.Drawing.Size(782, 595);
            this.Controls.Add(this.btnCancel);
            this.Controls.Add(this.btnConfirm);
            this.Controls.Add(this.chkReceived);
            this.Controls.Add(this.lblLoadStatus);
            this.Controls.Add(this.txtReference);
            this.Controls.Add(this.lblReference);
            this.Controls.Add(this.pictureQr);
            this.Controls.Add(this.lblAmount);
            this.Controls.Add(this.lblAccount);
            this.Controls.Add(this.lblTitle);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "FrmQrPayment";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "Thanh toán chuyển khoản";
            this.Load += new System.EventHandler(this.FrmQrPayment_Load);
            ((System.ComponentModel.ISupportInitialize)(this.pictureQr.Properties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.txtReference.Properties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.chkReceived.Properties)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();
        }
    }
}
