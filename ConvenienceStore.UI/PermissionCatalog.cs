using System.Linq;

namespace ConvenienceStore.UI
{
    
    public class PermissionItem
    {
        public string Code { get; private set; }
        public string Group { get; private set; }
        public string Text { get; private set; }

        public PermissionItem(string code, string group, string text)
        {
            Code = code; Group = group; Text = text;
        }

        public override string ToString()
        {
            return Group + "  ›  " + Text;
        }
    }

    
    
    
    
    public static class PermissionCatalog
    {
        public static readonly PermissionItem[] All =
        {
            new PermissionItem("mnuUserManagement",  "Quản trị",  "Quản lý tài khoản"),
            new PermissionItem("mnuRoleManagement",  "Quản trị",  "Quản lý nhóm quyền"),

            new PermissionItem("mnuCategory",        "Danh mục",  "Danh mục sản phẩm"),
            new PermissionItem("mnuProduct",         "Danh mục",  "Sản phẩm"),
            new PermissionItem("mnuSupplier",        "Danh mục",  "Nhà cung cấp"),
            new PermissionItem("mnuCustomer",        "Danh mục",  "Khách hàng"),
            new PermissionItem("mnuPromotion",       "Danh mục",  "Khuyến mãi"),

            new PermissionItem("mnuPurchaseReceipt", "Nghiệp vụ", "Nhập hàng"),
            new PermissionItem("mnuPurchaseHistory", "Nghiệp vụ", "Tra cứu phiếu nhập"),
            new PermissionItem("mnuSales",           "Nghiệp vụ", "Bán hàng"),
            new PermissionItem("mnuShift",            "Nghiệp vụ", "Quản lý ca bán hàng"),
            new PermissionItem("mnuSupplierDebt",     "Nghiệp vụ", "Công nợ nhà cung cấp"),
            new PermissionItem("mnuInvoice",         "Nghiệp vụ", "Hóa đơn"),
            new PermissionItem("mnuStockMovement",   "Nghiệp vụ", "Lịch sử nhập xuất tồn"),
            new PermissionItem("mnuStockAdjust",     "Nghiệp vụ", "Điều chỉnh tồn kho"),
            new PermissionItem("fnInvoiceCancel",    "Nghiệp vụ", "Hủy hóa đơn (nút trong form Hóa đơn)"),
            new PermissionItem("fnSaleReturn",        "Nghiệp vụ", "Đổi/trả hàng (nút trong form Hóa đơn)"),

            new PermissionItem("mnuRevenueReport",   "Báo cáo",   "Báo cáo doanh thu"),
            new PermissionItem("mnuDashboard",       "Báo cáo",   "Tổng quan (Dashboard)"),
        };

        
        public static readonly string[] AlwaysAllowed = { "mnuChangePassword", "mnuLogout", "mnuExit" };

        public static bool IsAlwaysAllowed(string menuName)
        {
            return AlwaysAllowed.Contains(menuName);
        }
    }
}
