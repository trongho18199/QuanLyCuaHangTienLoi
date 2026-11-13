namespace ConvenienceStore.UI.Forms
{
    partial class FrmMain
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
            this.menuStrip = new System.Windows.Forms.MenuStrip();
            this.statusStrip = new System.Windows.Forms.StatusStrip();
            this.tabDocuments = new DevExpress.XtraTab.XtraTabControl();
            this.lblStatusUser = new System.Windows.Forms.ToolStripStatusLabel();
            this.lblStatusRole = new System.Windows.Forms.ToolStripStatusLabel();
            this.mnuSystem = new System.Windows.Forms.ToolStripMenuItem();
            this.mnuAdmin = new System.Windows.Forms.ToolStripMenuItem();
            this.mnuCatalog = new System.Windows.Forms.ToolStripMenuItem();
            this.mnuBusiness = new System.Windows.Forms.ToolStripMenuItem();
            this.mnuReport = new System.Windows.Forms.ToolStripMenuItem();
            this.mnuChangePassword = new System.Windows.Forms.ToolStripMenuItem();
            this.mnuLogout = new System.Windows.Forms.ToolStripMenuItem();
            this.mnuExit = new System.Windows.Forms.ToolStripMenuItem();
            this.mnuUserManagement = new System.Windows.Forms.ToolStripMenuItem();
            this.mnuRoleManagement = new System.Windows.Forms.ToolStripMenuItem();
            this.mnuCategory = new System.Windows.Forms.ToolStripMenuItem();
            this.mnuProduct = new System.Windows.Forms.ToolStripMenuItem();
            this.mnuSupplier = new System.Windows.Forms.ToolStripMenuItem();
            this.mnuCustomer = new System.Windows.Forms.ToolStripMenuItem();
            this.mnuPromotion = new System.Windows.Forms.ToolStripMenuItem();
            this.mnuPurchaseReceipt = new System.Windows.Forms.ToolStripMenuItem();
            this.mnuSales = new System.Windows.Forms.ToolStripMenuItem();
            this.mnuInvoice = new System.Windows.Forms.ToolStripMenuItem();
            this.mnuStockMovement = new System.Windows.Forms.ToolStripMenuItem();
            this.mnuPurchaseHistory = new System.Windows.Forms.ToolStripMenuItem();
            this.mnuStockAdjust = new System.Windows.Forms.ToolStripMenuItem();
            this.mnuRevenueReport = new System.Windows.Forms.ToolStripMenuItem();
            this.mnuDashboard = new System.Windows.Forms.ToolStripMenuItem();
            this.mnuShift = new System.Windows.Forms.ToolStripMenuItem();
            this.mnuSupplierDebt = new System.Windows.Forms.ToolStripMenuItem();
            this.menuStrip.SuspendLayout();
            this.statusStrip.SuspendLayout();
            this.SuspendLayout();
            
            
            
            this.menuStrip.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.mnuSystem,
            this.mnuAdmin,
            this.mnuCatalog,
            this.mnuBusiness,
            this.mnuReport
            });
            this.menuStrip.Location = new System.Drawing.Point(0, 0);
            this.menuStrip.Name = "menuStrip";
            this.menuStrip.Size = new System.Drawing.Size(900, 24);
            this.menuStrip.TabIndex = 0;
            this.menuStrip.Text = "menuStrip";
            
            
            
            this.mnuSystem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.mnuChangePassword,
            this.mnuLogout,
            this.mnuExit
            });
            this.mnuSystem.Name = "mnuSystem";
            this.mnuSystem.Text = "Hệ thống";
            
            
            
            this.mnuChangePassword.Name = "mnuChangePassword";
            this.mnuChangePassword.Tag = "Admin,ThuKho,ThuNgan";
            this.mnuChangePassword.Text = "Đổi mật khẩu";
            this.mnuChangePassword.Click += new System.EventHandler(this.MenuItem_Click);
            
            
            
            this.mnuLogout.Name = "mnuLogout";
            this.mnuLogout.Tag = "Admin,ThuKho,ThuNgan";
            this.mnuLogout.Text = "Đăng xuất";
            this.mnuLogout.Click += new System.EventHandler(this.MenuItem_Click);
            
            
            
            this.mnuExit.Name = "mnuExit";
            this.mnuExit.Tag = "Admin,ThuKho,ThuNgan";
            this.mnuExit.Text = "Thoát";
            this.mnuExit.Click += new System.EventHandler(this.MenuItem_Click);
            
            
            
            this.mnuAdmin.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.mnuUserManagement,
            this.mnuRoleManagement
            });
            this.mnuAdmin.Name = "mnuAdmin";
            this.mnuAdmin.Text = "Quản trị";
            
            
            
            this.mnuUserManagement.Name = "mnuUserManagement";
            this.mnuUserManagement.Tag = "Admin";
            this.mnuUserManagement.Text = "Quản lý tài khoản";
            this.mnuUserManagement.Click += new System.EventHandler(this.MenuItem_Click);
            
            
            
            this.mnuRoleManagement.Name = "mnuRoleManagement";
            this.mnuRoleManagement.Tag = "Admin";
            this.mnuRoleManagement.Text = "Quản lý nhóm quyền";
            this.mnuRoleManagement.Click += new System.EventHandler(this.MenuItem_Click);
            
            
            
            this.mnuCatalog.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.mnuCategory,
            this.mnuProduct,
            this.mnuSupplier,
            this.mnuCustomer,
            this.mnuPromotion
            });
            this.mnuCatalog.Name = "mnuCatalog";
            this.mnuCatalog.Text = "Danh mục";
            
            
            
            this.mnuCategory.Name = "mnuCategory";
            this.mnuCategory.Tag = "Admin,ThuKho";
            this.mnuCategory.Text = "Danh mục sản phẩm";
            this.mnuCategory.Click += new System.EventHandler(this.MenuItem_Click);
            
            
            
            this.mnuProduct.Name = "mnuProduct";
            this.mnuProduct.Tag = "Admin,ThuKho";
            this.mnuProduct.Text = "Sản phẩm";
            this.mnuProduct.Click += new System.EventHandler(this.MenuItem_Click);
            
            
            
            this.mnuSupplier.Name = "mnuSupplier";
            this.mnuSupplier.Tag = "Admin,ThuKho";
            this.mnuSupplier.Text = "Nhà cung cấp";
            this.mnuSupplier.Click += new System.EventHandler(this.MenuItem_Click);
            
            
            
            this.mnuCustomer.Name = "mnuCustomer";
            this.mnuCustomer.Tag = "Admin,ThuNgan";
            this.mnuCustomer.Text = "Khách hàng";
            this.mnuCustomer.Click += new System.EventHandler(this.MenuItem_Click);
            
            
            
            this.mnuPromotion.Name = "mnuPromotion";
            this.mnuPromotion.Tag = "Admin";
            this.mnuPromotion.Text = "Khuyến mãi";
            this.mnuPromotion.Click += new System.EventHandler(this.MenuItem_Click);
            
            
            
            this.mnuBusiness.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.mnuPurchaseReceipt,
            this.mnuShift,
            this.mnuSupplierDebt,
            this.mnuPurchaseHistory,
            this.mnuSales,
            this.mnuInvoice,
            this.mnuStockMovement,
            this.mnuStockAdjust
            });
            this.mnuBusiness.Name = "mnuBusiness";
            this.mnuBusiness.Text = "Nghiệp vụ";
            
            
            
            this.mnuShift.Name = "mnuShift";
            this.mnuShift.Tag = "Admin,ThuNgan";
            this.mnuShift.Text = "Quản lý ca bán hàng";
            this.mnuShift.Click += new System.EventHandler(this.MenuItem_Click);
            this.mnuSupplierDebt.Name = "mnuSupplierDebt";
            this.mnuSupplierDebt.Text = "Công nợ nhà cung cấp";
            this.mnuSupplierDebt.Tag = "Admin";
            this.mnuSupplierDebt.Click += new System.EventHandler(this.MenuItem_Click);
            
            
            
            this.mnuPurchaseReceipt.Name = "mnuPurchaseReceipt";
            this.mnuPurchaseReceipt.Tag = "Admin,ThuKho";
            this.mnuPurchaseReceipt.Text = "Nhập hàng";
            this.mnuPurchaseReceipt.Click += new System.EventHandler(this.MenuItem_Click);
            
            
            
            this.mnuPurchaseHistory.Name = "mnuPurchaseHistory";
            this.mnuPurchaseHistory.Tag = "Admin,ThuKho";
            this.mnuPurchaseHistory.Text = "Tra cứu phiếu nhập";
            this.mnuPurchaseHistory.Click += new System.EventHandler(this.MenuItem_Click);
            
            
            
            this.mnuSales.Name = "mnuSales";
            this.mnuSales.Tag = "Admin,ThuNgan";
            this.mnuSales.Text = "Bán hàng";
            this.mnuSales.Click += new System.EventHandler(this.MenuItem_Click);
            
            
            
            this.mnuInvoice.Name = "mnuInvoice";
            this.mnuInvoice.Tag = "Admin,ThuNgan";
            this.mnuInvoice.Text = "Hóa đơn";
            this.mnuInvoice.Click += new System.EventHandler(this.MenuItem_Click);
            
            
            
            this.mnuStockMovement.Name = "mnuStockMovement";
            this.mnuStockMovement.Tag = "Admin,ThuKho";
            this.mnuStockMovement.Text = "Lịch sử nhập xuất tồn";
            this.mnuStockMovement.Click += new System.EventHandler(this.MenuItem_Click);
            
            
            
            this.mnuStockAdjust.Name = "mnuStockAdjust";
            this.mnuStockAdjust.Tag = "Admin,ThuKho";
            this.mnuStockAdjust.Text = "Điều chỉnh tồn kho";
            this.mnuStockAdjust.Click += new System.EventHandler(this.MenuItem_Click);
            
            
            
            this.mnuReport.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.mnuRevenueReport,
            this.mnuDashboard
            });
            this.mnuReport.Name = "mnuReport";
            this.mnuReport.Text = "Báo cáo";
            
            
            
            this.mnuRevenueReport.Name = "mnuRevenueReport";
            this.mnuRevenueReport.Tag = "Admin";
            this.mnuRevenueReport.Text = "Báo cáo doanh thu";
            this.mnuRevenueReport.Click += new System.EventHandler(this.MenuItem_Click);
            
            
            
            this.mnuDashboard.Name = "mnuDashboard";
            this.mnuDashboard.Tag = "Admin";
            this.mnuDashboard.Text = "Tổng quan (Dashboard)";
            this.mnuDashboard.Click += new System.EventHandler(this.MenuItem_Click);
            
            
            
            this.tabDocuments.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tabDocuments.Name = "tabDocuments";
            this.tabDocuments.TabIndex = 2;
            
            
            
            this.statusStrip.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.lblStatusUser,
            this.lblStatusRole});
            this.statusStrip.Location = new System.Drawing.Point(0, 528);
            this.statusStrip.Name = "statusStrip";
            this.statusStrip.Size = new System.Drawing.Size(900, 22);
            this.statusStrip.TabIndex = 1;
            this.statusStrip.Text = "statusStrip";
            
            
            
            this.lblStatusUser.Name = "lblStatusUser";
            this.lblStatusUser.Size = new System.Drawing.Size(80, 17);
            this.lblStatusUser.Text = "Người dùng: ";
            
            
            
            this.lblStatusRole.Name = "lblStatusRole";
            this.lblStatusRole.Size = new System.Drawing.Size(60, 17);
            this.lblStatusRole.Text = "Quyền: ";
            
            
            
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(900, 550);
            
            this.IsMdiContainer = false;
            this.Controls.Add(this.tabDocuments);
            this.Controls.Add(this.statusStrip);
            this.Controls.Add(this.menuStrip);
            this.MainMenuStrip = this.menuStrip;
            this.Name = "FrmMain";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Quản lý cửa hàng tiện lợi";
            this.WindowState = System.Windows.Forms.FormWindowState.Maximized;
            this.Load += new System.EventHandler(this.FrmMain_Load);
            this.menuStrip.ResumeLayout(false);
            this.menuStrip.PerformLayout();
            this.statusStrip.ResumeLayout(false);
            this.statusStrip.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();
        }

        #endregion

        private System.Windows.Forms.MenuStrip menuStrip;
        private System.Windows.Forms.StatusStrip statusStrip;
        private DevExpress.XtraTab.XtraTabControl tabDocuments;
        private System.Windows.Forms.ToolStripStatusLabel lblStatusUser;
        private System.Windows.Forms.ToolStripStatusLabel lblStatusRole;
        private System.Windows.Forms.ToolStripMenuItem mnuSystem;
        private System.Windows.Forms.ToolStripMenuItem mnuAdmin;
        private System.Windows.Forms.ToolStripMenuItem mnuCatalog;
        private System.Windows.Forms.ToolStripMenuItem mnuBusiness;
        private System.Windows.Forms.ToolStripMenuItem mnuReport;
        private System.Windows.Forms.ToolStripMenuItem mnuChangePassword;
        private System.Windows.Forms.ToolStripMenuItem mnuLogout;
        private System.Windows.Forms.ToolStripMenuItem mnuExit;
        private System.Windows.Forms.ToolStripMenuItem mnuUserManagement;
        private System.Windows.Forms.ToolStripMenuItem mnuRoleManagement;
        private System.Windows.Forms.ToolStripMenuItem mnuCategory;
        private System.Windows.Forms.ToolStripMenuItem mnuProduct;
        private System.Windows.Forms.ToolStripMenuItem mnuSupplier;
        private System.Windows.Forms.ToolStripMenuItem mnuCustomer;
        private System.Windows.Forms.ToolStripMenuItem mnuPromotion;
        private System.Windows.Forms.ToolStripMenuItem mnuPurchaseReceipt;
        private System.Windows.Forms.ToolStripMenuItem mnuSales;
        private System.Windows.Forms.ToolStripMenuItem mnuInvoice;
        private System.Windows.Forms.ToolStripMenuItem mnuStockMovement;
        private System.Windows.Forms.ToolStripMenuItem mnuPurchaseHistory;
        private System.Windows.Forms.ToolStripMenuItem mnuStockAdjust;
        private System.Windows.Forms.ToolStripMenuItem mnuRevenueReport;
        private System.Windows.Forms.ToolStripMenuItem mnuDashboard;
        private System.Windows.Forms.ToolStripMenuItem mnuShift;
        private System.Windows.Forms.ToolStripMenuItem mnuSupplierDebt;
    }
}
