using ConvenienceStore.Data.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;
using DevExpress.XtraTab;
using DevExpress.XtraTab.ViewInfo;

namespace ConvenienceStore.UI.Forms
{
    public partial class FrmMain : DevExpress.XtraEditors.XtraForm
    {
        public FrmMain()
        {
            InitializeComponent();
            
            IsMdiContainer = false;
            InitializeDocumentTabs();
        }

        
        
        
        
        private void InitializeDocumentTabs()
        {
            
            tabDocuments.ClosePageButtonShowMode = ClosePageButtonShowMode.InAllTabPageHeaders;
            tabDocuments.CloseButtonClick += TabDocuments_CloseButtonClick;
        }

        
        private void TabDocuments_CloseButtonClick(object sender, EventArgs e)
        {
            var args = e as ClosePageButtonEventArgs;
            var page = args == null ? null : args.Page as XtraTabPage;
            CloseDocument(page);
        }

        private void CloseDocument(XtraTabPage page)
        {
            if (page == null || page.IsDisposed) return;

            Form child = page.Tag as Form;
            if (child != null && !child.IsDisposed)
            {
                
                child.Close();
                return;
            }

            RemoveDocumentPage(page);
        }

        private void RemoveDocumentPage(XtraTabPage page)
        {
            if (page == null || page.IsDisposed) return;
            if (tabDocuments.TabPages.Contains(page)) tabDocuments.TabPages.Remove(page);
            page.Dispose();
        }

        protected override bool ProcessCmdKey(ref Message msg, Keys keyData)
        {
            if (keyData == (Keys.Control | Keys.W) || keyData == (Keys.Control | Keys.F4))
            {
                CloseDocument(tabDocuments.SelectedTabPage);
                return true;
            }

            return base.ProcessCmdKey(ref msg, keyData);
        }

        private void FrmMain_Load(object sender, EventArgs e)
        {
            var user = AppSession.CurrentUser;
            if (user != null)
            {
                lblStatusUser.Text = "Người dùng: " + (string.IsNullOrEmpty(user.FullName) ? user.Username : user.FullName);
                lblStatusRole.Text = "| Quyền: " + AppSession.RoleName;
            }

            ApplyPermissions();
            if (user == null) return;

            
            if (AppSession.IsAdmin && mnuDashboard.Available)
                OpenChildWith(() => new FrmDashboard(user.Id));
            else if (string.Equals(AppSession.RoleName, "ThuNgan", StringComparison.OrdinalIgnoreCase) && mnuSales.Available)
                OpenChildWith(() => new FrmSales(user.Id));
            else if (string.Equals(AppSession.RoleName, "ThuKho", StringComparison.OrdinalIgnoreCase) && mnuPurchaseReceipt.Available)
                OpenChildWith(() => new FrmPurchaseReceipt(user.Id));
            else if (mnuDashboard.Available)
                OpenChildWith(() => new FrmDashboard(user.Id));
            else if (mnuSales.Available)
                OpenChildWith(() => new FrmSales(user.Id));
            else if (mnuPurchaseReceipt.Available)
                OpenChildWith(() => new FrmPurchaseReceipt(user.Id));
            else if (mnuShift.Available)
                OpenChildWith(() => new FrmShiftManagement(user.Id));
        }

        

        
        
        
        
        private void ApplyPermissions()
        {
            HashSet<string> allowed = null;
            try
            {
                if (AppSession.CurrentUser != null)
                    allowed = new HashSet<string>(new RoleRepository().GetPermissionCodes(AppSession.CurrentUser.RoleId));
            }
            catch (Exception)
            {
                allowed = null; 
            }

            foreach (ToolStripMenuItem top in menuStrip.Items)
                ApplyPermission(top, allowed);
        }

        private void ApplyPermission(ToolStripMenuItem item, HashSet<string> allowed)
        {
            foreach (ToolStripItem child in item.DropDownItems)
            {
                var sub = child as ToolStripMenuItem;
                if (sub == null) continue;
                sub.Available = IsAllowed(sub, allowed);
            }
            item.Available = item.DropDownItems.OfType<ToolStripMenuItem>().Any(m => m.Available);
        }

        private static bool IsAllowed(ToolStripMenuItem menu, HashSet<string> allowed)
        {
            if (PermissionCatalog.IsAlwaysAllowed(menu.Name)) return true;

            if (AppSession.IsAdmin) return true;

            if (allowed != null) return allowed.Contains(menu.Name);

            string roles = menu.Tag as string;   
            return !string.IsNullOrEmpty(roles) && roles.Split(',').Contains(AppSession.RoleName);
        }

        

        private void MenuItem_Click(object sender, EventArgs e)
        {
            var item = (ToolStripMenuItem)sender;

            if (item == mnuExit)
            {
                Application.Exit();
            }
            else if (item == mnuLogout)
            {
                if (MessageBox.Show("Bạn có muốn đăng xuất?", "Đăng xuất",
                    MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
                {
                    AppSession.CurrentUser = null;
                    Application.Restart();
                }
            }
            else if (item == mnuChangePassword)
            {
                using (var f = new FrmChangePassword())
                    f.ShowDialog(this);
            }
            else if (item == mnuUserManagement)
            {
                OpenChild<FrmUserManagement>(null);
            }
            else if (item == mnuRoleManagement)
            {
                OpenChild<FrmRoleManagement>(f => f.PermissionsSaved += (s, ev) => ApplyPermissions());
            }
            else if (item == mnuCategory)
            {
                OpenChildWith(() => new FrmCategory(AppSession.CurrentUser.Id));
            }
            else if (item == mnuProduct)
            {
                OpenChildWith(() => new FrmProduct(AppSession.CurrentUser.Id));
            }
            else if (item == mnuSupplier)
            {
                OpenChildWith(() => new FrmSupplier(AppSession.CurrentUser.Id));
            }
            else if (item == mnuCustomer)
            {
                OpenChildWith(() => new FrmCustomer(AppSession.CurrentUser.Id));
            }
            else if (item == mnuInvoice)
            {
                OpenChildWith(() => new FrmInvoice(AppSession.CurrentUser.Id));
            }
            else if (item == mnuPromotion)
            {
                OpenChildWith(() => new FrmPromotion(AppSession.CurrentUser.Id));
            }
            else if (item == mnuPurchaseHistory)
            {
                OpenChildWith(() => new FrmPurchaseHistory(AppSession.CurrentUser.Id));
            }
            else if (item == mnuStockAdjust)
            {
                OpenChildWith(() => new FrmStockAdjustment(AppSession.CurrentUser.Id));
            }
            else if (item == mnuDashboard)
            {
                OpenChildWith(() => new FrmDashboard(AppSession.CurrentUser.Id));
            }
            else if (item == mnuPurchaseReceipt)
            {
                OpenChildWith(() => new FrmPurchaseReceipt(AppSession.CurrentUser.Id));
            }
            else if (item == mnuSales)
            {
                OpenChildWith(() => new FrmSales(AppSession.CurrentUser.Id));
            }
            else if (item == mnuShift)
            {
                OpenChildWith(() => new FrmShiftManagement(AppSession.CurrentUser.Id));
            }
            else if (item == mnuSupplierDebt)
            {
                OpenChildWith(() => new FrmSupplierDebt(AppSession.CurrentUser.Id));
            }
            else if (item == mnuStockMovement)
            {
                OpenChildWith(() => new FrmStockMovement(AppSession.CurrentUser.Id));
            }
            else if (item == mnuRevenueReport)
            {
                OpenChildWith(() => new FrmRevenueReport(AppSession.CurrentUser.Id));
            }
            else
            {
                
                MessageBox.Show("Chức năng \"" + item.Text + "\" chưa được triển khai.", "Thông báo",
                    MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
        }

        
        private void OpenChild<T>(Action<T> setup) where T : Form, new()
        {
            foreach (XtraTabPage page in tabDocuments.TabPages)
            {
                Form open = page.Tag as Form;
                if (open is T)
                {
                    tabDocuments.SelectedTabPage = page;
                    return;
                }
            }

            var form = new T();
            if (setup != null) setup(form);
            OpenFormInTab(form);
        }

        
        private void OpenChildWith<T>(Func<T> create) where T : Form
        {
            foreach (XtraTabPage page in tabDocuments.TabPages)
            {
                Form open = page.Tag as Form;
                if (open is T)
                {
                    tabDocuments.SelectedTabPage = page;
                    return;
                }
            }

            OpenFormInTab(create());
        }

        
        private void OpenFormInTab(Form form)
        {
            if (form == null) return;

            string title = string.IsNullOrWhiteSpace(form.Text) ? form.GetType().Name : form.Text;
            var page = new XtraTabPage { Text = title, Name = "tab" + form.GetType().Name, Tag = form };

            
            form.TopLevel = false;
            form.FormBorderStyle = FormBorderStyle.None;
            form.Dock = DockStyle.Fill;
            form.Margin = Padding.Empty;

            form.TextChanged += (s, e) => page.Text =
                string.IsNullOrWhiteSpace(form.Text) ? form.GetType().Name : form.Text;
            form.FormClosed += (s, e) => RemoveDocumentPage(page);

            page.Controls.Add(form);
            tabDocuments.TabPages.Add(page);
            tabDocuments.SelectedTabPage = page;
            form.Show();
            form.BringToFront();
        }

    }
}
