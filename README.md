# ConvenienceStoreManager

## Tài khoản Admin mặc định

- **Tên đăng nhập:** `admin`
- **Mật khẩu:** `Admin@123`

Tài khoản này được thêm khi chạy script tạo CSDL trên một database chưa có người dùng `admin`. Script không đặt lại mật khẩu nếu tài khoản đã tồn tại. 

## Giới thiệu

Ứng dụng quản lý cửa hàng tiện lợi trên Windows, viết bằng WinForms và .NET Framework 4.8. Giao diện dùng DevExpress 26.1; dữ liệu lưu trong SQL Server qua LINQ to SQL. Solution gồm ba project:

| Project | Vai trò |
| --- | --- |
| `ConvenienceStore.Data` | Cấu hình kết nối, mô hình và truy cập CSDL |
| `ConvenienceStore.Business` | Nghiệp vụ, kiểm tra quyền, tính tiền và báo cáo |
| `ConvenienceStore.UI` | Giao diện WinForms, đăng nhập và các màn hình chức năng |

## Cài đặt và chạy

1. Chuẩn bị Windows, .NET Framework 4.8, SQL Server hoặc SQL Server LocalDB, Visual Studio với workload **.NET desktop development**, và các DLL DevExpress 26.1. Project UI hiện tham chiếu DevExpress tại `D:\DevExpress\Bin\Framework`; nếu máy dùng đường dẫn khác, cập nhật các `HintPath` trong `ConvenienceStore.UI/ConvenienceStore.UI.csproj`.
2. Mở `ConvenienceStoreManager.slnx`, build solution và đặt `ConvenienceStore.UI` làm Startup Project.
3. Chạy ứng dụng. Nếu chưa có cấu hình CSDL hoặc không kết nối được, màn hình **Cấu hình CSDL** sẽ mở. Nhập máy chủ, tên database và kiểu xác thực rồi chọn **Lưu và tạo CSDL**. Ứng dụng tạo database bằng `ConvenienceStore.Data/Database/ConvenienceStoreDB.sql` khi database chưa kết nối được.
4. Đăng nhập bằng tài khoản Admin ở đầu tài liệu.

Giá trị kết nối mặc định trong `ConvenienceStore.UI/App.config` là `(localdb)\MSSQLLocalDB`, database `ConvenienceStoreDB`, xác thực Windows. Lựa chọn lưu từ màn hình cấu hình nằm tại `%LOCALAPPDATA%\ConvenienceStoreManager\database.config` và được ưu tiên dùng ở lần chạy sau. Nếu cần nâng cấp một database **đã kết nối được**, hãy chạy lại `ConvenienceStore.Data/Database/ConvenienceStoreDB.sql` bằng SQL Server Management Studio hoặc công cụ SQL tương đương; màn hình cấu hình không tự chạy lại script cho database đang kết nối được. Sao lưu dữ liệu trước khi nâng cấp.

## Chức năng

- **Quản trị:** quản lý tài khoản, nhóm quyền và quyền theo chức năng; đổi mật khẩu.
- **Danh mục:** danh mục sản phẩm, sản phẩm, nhà cung cấp, khách hàng và mã khuyến mãi theo phần trăm.
- **Nghiệp vụ:** nhập hàng và tra cứu phiếu nhập; bán hàng, tra cứu/in/hủy hóa đơn, đổi hoặc trả từng dòng hóa đơn; quản lý ca, công nợ nhà cung cấp, lịch sử nhập xuất tồn, tồn theo lô và điều chỉnh tồn kho.
- **Báo cáo:** doanh thu, giá vốn và lợi nhuận gộp khi đủ dữ liệu, cùng Dashboard doanh thu và cảnh báo tồn thấp.

Ứng dụng tạo ba nhóm quyền mặc định: `Admin`, `ThuKho`, `ThuNgan`. Admin có toàn quyền; quyền của các nhóm khác được kiểm tra trong service theo bảng `RolePermissions`. Các chức năng luôn cho phép là đổi mật khẩu, đăng xuất và thoát.

## Bán hàng và VietQR

- Phương thức thanh toán gồm tiền mặt, chuyển khoản, thẻ và ví điện tử. Khi chọn chuyển khoản, ứng dụng lấy ảnh VietQR qua Internet và yêu cầu người dùng xác nhận đã nhận đủ tiền trước khi lưu giao dịch. Ứng dụng không tự xác nhận giao dịch ngân hàng.
- Cấu hình VietQR nằm trong `ConvenienceStore.UI/App.config`: `VietQrBankId`, `VietQrAccountNo` và `VietQrTemplate`. Ảnh QR được tải ở định dạng PNG từ `img.vietqr.io`; cần kết nối Internet để hiển thị.
- Khách VIP được giảm 10%; khuyến mãi theo mã được tính trên số tiền sau giảm VIP.
- Bán hàng cần một ca đang mở. Tiền mặt từ bán hàng và các khoản thu/chi tiền mặt được ghi để đối soát khi chốt ca. Các phương thức không dùng tiền mặt chưa được đối soát theo từng phương thức.

## Kho, công nợ và báo cáo

- Tồn kho thay đổi khi nhập, bán, hủy hóa đơn, đổi/trả hoặc điều chỉnh. Biến động được ghi trong `StockMovements`; sản phẩm mới có tồn bằng 0. Ngưỡng cảnh báo tồn thấp là 10.
- Hàng có lô được xuất theo hạn gần nhất. Lô đã quá hạn không được chọn để bán hoặc làm hàng đổi; hàng không có ngày hết hạn vẫn có thể bán. Tồn cũ thiếu thông tin lô được xếp vào lô không rõ.
- Phiếu nhập mới có thể ghi số tiền đã trả ban đầu. Phiếu nhập cũ chưa xác định số đã trả cần được cập nhật trong màn hình công nợ. Thanh toán công nợ hỗ trợ tiền mặt hoặc chuyển khoản; khoản thanh toán tiền mặt được ghi vào ca đang mở của người thực hiện.
- Hủy hóa đơn hoàn hàng vào kho và giữ hóa đơn để tra cứu, nhưng không tính hóa đơn đó vào doanh thu. Hóa đơn đã có đổi/trả không được hủy toàn bộ.
- Báo cáo doanh thu chỉ tính hóa đơn chưa hủy. Giá vốn và lợi nhuận gộp chỉ hiện khi lịch sử lô và giá vốn đầy đủ, đồng thời không có đổi/trả liên quan trong kỳ; báo cáo nêu lý do khi chưa thể tính.

## Giao diện tab

Mỗi màn hình nghiệp vụ mở trong một tab. Chọn lại chức năng đang mở sẽ chuyển đến tab đó. Có thể đóng tab bằng nút **X**, `Ctrl+W` hoặc `Ctrl+F4`; thao tác đóng vẫn gọi xử lý `FormClosing` của màn hình.

## Thêm chức năng có phân quyền

1. Khai báo mã chức năng trong `ConvenienceStore.UI/PermissionCatalog.cs`.
2. Thêm mục menu và nhánh mở form trong `FrmMain.Designer.cs` và `FrmMain.cs`.
3. Thêm quyền mặc định cần thiết vào `ConvenienceStore.Data/Database/ConvenienceStoreDB.sql` và áp dụng script cho database hiện có.
