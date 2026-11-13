namespace ConvenienceStore.UI.Forms
{
    partial class FrmSupplierDebt
    {
        private System.Windows.Forms.TextBox keyword;
        private System.Windows.Forms.DataGridView grid, history;
        private DevExpress.XtraEditors.LabelControl summary, lblSearch;
        private DevExpress.XtraEditors.SimpleButton btnFind, btnRegister, btnPay;
        private System.Windows.Forms.Panel top;
        private System.Windows.Forms.SplitContainer split;
        private void InitializeComponent()
        {
            this.keyword = new System.Windows.Forms.TextBox();
            this.grid = new System.Windows.Forms.DataGridView(); this.history = new System.Windows.Forms.DataGridView();
            this.summary = new DevExpress.XtraEditors.LabelControl(); this.lblSearch = new DevExpress.XtraEditors.LabelControl();
            this.btnFind = new DevExpress.XtraEditors.SimpleButton(); this.btnRegister = new DevExpress.XtraEditors.SimpleButton(); this.btnPay = new DevExpress.XtraEditors.SimpleButton();
            this.top = new System.Windows.Forms.Panel(); this.split = new System.Windows.Forms.SplitContainer();
            ((System.ComponentModel.ISupportInitialize)(this.grid)).BeginInit(); ((System.ComponentModel.ISupportInitialize)(this.history)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.split)).BeginInit(); this.split.Panel1.SuspendLayout(); this.split.Panel2.SuspendLayout(); this.split.SuspendLayout(); this.top.SuspendLayout(); this.SuspendLayout();
            this.top.Name = "top"; this.top.Dock = System.Windows.Forms.DockStyle.Top; this.top.Height = 100;
            this.lblSearch.Text = "Tìm phiếu / nhà cung cấp:"; this.lblSearch.Location = new System.Drawing.Point(20, 16);
            this.keyword.Name = "keyword"; this.keyword.SetBounds(245, 12, 290, 30);
            this.btnFind.Name = "btnFind"; this.btnFind.Text = "Tìm / tải lại"; this.btnFind.SetBounds(550, 10, 130, 34);
            this.btnRegister.Name = "btnRegister"; this.btnRegister.Text = "Xác lập phiếu cũ"; this.btnRegister.SetBounds(690, 10, 165, 34);
            this.btnPay.Name = "btnPay"; this.btnPay.Text = "Ghi thanh toán"; this.btnPay.SetBounds(865, 10, 150, 34);
            this.summary.Name = "summary"; this.summary.AutoSizeMode = DevExpress.XtraEditors.LabelAutoSizeMode.None; this.summary.SetBounds(20, 58, 1000, 30);
            this.top.Controls.AddRange(new System.Windows.Forms.Control[] { this.lblSearch, this.keyword, this.btnFind, this.btnRegister, this.btnPay, this.summary });
            this.split.Name = "split"; this.split.Dock = System.Windows.Forms.DockStyle.Fill; this.split.Orientation = System.Windows.Forms.Orientation.Horizontal;
            this.split.Size = new System.Drawing.Size(1084, 581); this.split.SplitterDistance = 360;
            this.grid.Name = "grid"; this.grid.Dock = System.Windows.Forms.DockStyle.Fill;
            this.history.Name = "history"; this.history.Dock = System.Windows.Forms.DockStyle.Fill;
            this.split.Panel1.Controls.Add(this.grid); this.split.Panel2.Controls.Add(this.history);
            this.Controls.Add(this.split); this.Controls.Add(this.top);
            this.Name = "FrmSupplierDebt"; this.Text = "Công nợ nhà cung cấp"; this.ClientSize = new System.Drawing.Size(1084, 681);
            this.MinimumSize = new System.Drawing.Size(900, 600); this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            ((System.ComponentModel.ISupportInitialize)(this.grid)).EndInit(); ((System.ComponentModel.ISupportInitialize)(this.history)).EndInit();
            this.split.Panel1.ResumeLayout(false); this.split.Panel2.ResumeLayout(false); ((System.ComponentModel.ISupportInitialize)(this.split)).EndInit(); this.split.ResumeLayout(false); this.top.ResumeLayout(false); this.top.PerformLayout(); this.ResumeLayout(false);
        }
    }
}
