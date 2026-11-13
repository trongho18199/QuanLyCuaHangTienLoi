

IF DB_ID(N'ConvenienceStoreDB') IS NULL
    CREATE DATABASE ConvenienceStoreDB;
GO
USE ConvenienceStoreDB;
GO

IF OBJECT_ID(N'dbo.Roles') IS NULL
CREATE TABLE dbo.Roles (
    Id   INT IDENTITY(1,1) NOT NULL CONSTRAINT PK_Roles PRIMARY KEY,
    Name NVARCHAR(50) NOT NULL
);

IF OBJECT_ID(N'dbo.Users') IS NULL
CREATE TABLE dbo.Users (
    Id           INT IDENTITY(1,1) NOT NULL CONSTRAINT PK_Users PRIMARY KEY,
    Username     NVARCHAR(50)  NOT NULL,
    PasswordHash NVARCHAR(200) NOT NULL,
    FullName     NVARCHAR(100) NULL,
    IsActive     BIT NOT NULL,
    RoleId       INT NOT NULL CONSTRAINT FK_Users_Roles FOREIGN KEY REFERENCES dbo.Roles(Id),
    CONSTRAINT UQ_Users_Username UNIQUE (Username)
);

IF OBJECT_ID(N'dbo.Shifts') IS NULL
CREATE TABLE dbo.Shifts (
    Id INT IDENTITY(1,1) NOT NULL CONSTRAINT PK_Shifts PRIMARY KEY,
    Number NVARCHAR(40) NOT NULL,
    OpenedAt DATETIME2(7) NOT NULL,
    OpenedById INT NOT NULL CONSTRAINT FK_Shifts_OpenedBy FOREIGN KEY REFERENCES dbo.Users(Id),
    OpeningCash DECIMAL(18,2) NOT NULL,
    Status NVARCHAR(20) NOT NULL,
    ClosedAt DATETIME2(7) NULL,
    ClosedById INT NULL CONSTRAINT FK_Shifts_ClosedBy FOREIGN KEY REFERENCES dbo.Users(Id),
    ClosingCash DECIMAL(18,2) NULL,
    ExpectedCash DECIMAL(18,2) NULL,
    Difference DECIMAL(18,2) NULL,
    Note NVARCHAR(300) NULL
);
IF NOT EXISTS (SELECT 1 FROM sys.indexes WHERE name = N'UX_Shifts_Number' AND object_id = OBJECT_ID(N'dbo.Shifts'))
    CREATE UNIQUE INDEX UX_Shifts_Number ON dbo.Shifts(Number);

IF OBJECT_ID(N'dbo.CashFundReceipts', N'U') IS NULL
CREATE TABLE dbo.CashFundReceipts (
    Id INT IDENTITY(1,1) NOT NULL CONSTRAINT PK_CashFundReceipts PRIMARY KEY,
    ShiftId INT NOT NULL CONSTRAINT FK_CashFundReceipts_Shift REFERENCES dbo.Shifts(Id),
    CreatedAt DATETIME2 NOT NULL,
    CreatedById INT NOT NULL CONSTRAINT FK_CashFundReceipts_User REFERENCES dbo.Users(Id),
    Amount DECIMAL(18,2) NOT NULL CONSTRAINT CK_CashFundReceipts_Amount CHECK (Amount >= 0),
    CONSTRAINT UQ_CashFundReceipts_Shift UNIQUE (ShiftId)
);
IF OBJECT_ID(N'dbo.ShiftCashEntries') IS NULL
CREATE TABLE dbo.ShiftCashEntries (
    Id INT IDENTITY(1,1) NOT NULL CONSTRAINT PK_ShiftCashEntries PRIMARY KEY,
    ShiftId INT NOT NULL CONSTRAINT FK_ShiftCashEntries_Shift FOREIGN KEY REFERENCES dbo.Shifts(Id),
    CreatedAt DATETIME2 NOT NULL,
    CreatedById INT NOT NULL CONSTRAINT FK_ShiftCashEntries_User FOREIGN KEY REFERENCES dbo.Users(Id),
    ReferenceNumber NVARCHAR(40) NOT NULL,
    Description NVARCHAR(100) NOT NULL,
    Amount DECIMAL(18,2) NOT NULL
);

IF OBJECT_ID(N'dbo.RolePermissions') IS NULL
CREATE TABLE dbo.RolePermissions (
    Id           INT IDENTITY(1,1) NOT NULL CONSTRAINT PK_RolePermissions PRIMARY KEY,
    RoleId       INT NOT NULL CONSTRAINT FK_RolePermissions_Roles FOREIGN KEY REFERENCES dbo.Roles(Id),
    FunctionCode NVARCHAR(100) NOT NULL,
    CONSTRAINT UQ_RolePermissions UNIQUE (RoleId, FunctionCode)
);

IF OBJECT_ID(N'dbo.Categories') IS NULL
CREATE TABLE dbo.Categories (
    Id   INT IDENTITY(1,1) NOT NULL CONSTRAINT PK_Categories PRIMARY KEY,
    Code NVARCHAR(30) NOT NULL,
    Name NVARCHAR(100) NOT NULL
);


IF COL_LENGTH(N'dbo.Categories', N'Code') IS NULL
    ALTER TABLE dbo.Categories ADD Code NVARCHAR(30) NULL;
EXEC sys.sp_executesql N'UPDATE dbo.Categories
    SET Code = N''DM'' + CONVERT(NVARCHAR(20), Id)
    WHERE Code IS NULL OR LTRIM(RTRIM(Code)) = N'''';';
IF EXISTS (
    SELECT 1 FROM sys.columns
    WHERE object_id = OBJECT_ID(N'dbo.Categories') AND name = N'Code' AND is_nullable = 1
)
    ALTER TABLE dbo.Categories ALTER COLUMN Code NVARCHAR(30) NOT NULL;
IF NOT EXISTS (SELECT 1 FROM sys.indexes WHERE name = N'UX_Categories_Code' AND object_id = OBJECT_ID(N'dbo.Categories'))
    CREATE UNIQUE INDEX UX_Categories_Code ON dbo.Categories(Code);

IF OBJECT_ID(N'dbo.Products') IS NULL
CREATE TABLE dbo.Products (
    Id         INT IDENTITY(1,1) NOT NULL CONSTRAINT PK_Products PRIMARY KEY,
    Name       NVARCHAR(100) NOT NULL,
    Barcode    NVARCHAR(50) NULL,
    Price      DECIMAL(18,2) NOT NULL,
    Stock      INT NOT NULL,
    CategoryId INT NOT NULL CONSTRAINT FK_Products_Categories FOREIGN KEY REFERENCES dbo.Categories(Id)
);


BEGIN TRY
    IF NOT EXISTS (SELECT 1 FROM sys.indexes
                   WHERE name = N'UX_Products_Barcode' AND object_id = OBJECT_ID(N'dbo.Products'))
        EXEC (N'CREATE UNIQUE INDEX UX_Products_Barcode ON dbo.Products(Barcode) WHERE Barcode IS NOT NULL');
END TRY
BEGIN CATCH
    
    IF ERROR_NUMBER() NOT IN (1913, 1505, 1521) THROW;
END CATCH


IF OBJECT_ID(N'dbo.Suppliers') IS NULL
CREATE TABLE dbo.Suppliers (
    Id INT IDENTITY(1,1) NOT NULL CONSTRAINT PK_Suppliers PRIMARY KEY,
    Name NVARCHAR(100) NOT NULL,
    Phone NVARCHAR(30) NULL,
    IsActive BIT NOT NULL
);

IF OBJECT_ID(N'dbo.Customers') IS NULL
CREATE TABLE dbo.Customers (
    Id INT IDENTITY(1,1) NOT NULL CONSTRAINT PK_Customers PRIMARY KEY,
    Name NVARCHAR(100) NOT NULL,
    Phone NVARCHAR(30) NULL,
    IsVip BIT NOT NULL,
    IsActive BIT NOT NULL
);

IF OBJECT_ID(N'dbo.PurchaseReceipts') IS NULL
CREATE TABLE dbo.PurchaseReceipts (
    Id INT IDENTITY(1,1) NOT NULL CONSTRAINT PK_PurchaseReceipts PRIMARY KEY,
    OperationId UNIQUEIDENTIFIER NOT NULL,
    Number NVARCHAR(40) NOT NULL,
    CreatedAt DATETIME2(7) NOT NULL,
    SupplierId INT NOT NULL CONSTRAINT FK_PurchaseReceipts_Suppliers FOREIGN KEY REFERENCES dbo.Suppliers(Id),
    CreatedById INT NOT NULL CONSTRAINT FK_PurchaseReceipts_Users FOREIGN KEY REFERENCES dbo.Users(Id),
    SupplierName NVARCHAR(100) NOT NULL,
    Note NVARCHAR(300) NULL,
    Total DECIMAL(18,2) NOT NULL
);

IF OBJECT_ID(N'dbo.SupplierPayables') IS NULL
CREATE TABLE dbo.SupplierPayables (
    Id INT IDENTITY(1,1) NOT NULL CONSTRAINT PK_SupplierPayables PRIMARY KEY,
    PurchaseReceiptId INT NOT NULL CONSTRAINT FK_SupplierPayables_Receipt FOREIGN KEY REFERENCES dbo.PurchaseReceipts(Id),
    OpeningPaid DECIMAL(18,2) NOT NULL,
    CreatedAt DATETIME2 NOT NULL,
    CreatedById INT NOT NULL CONSTRAINT FK_SupplierPayables_User FOREIGN KEY REFERENCES dbo.Users(Id),
    CONSTRAINT UQ_SupplierPayables_Receipt UNIQUE (PurchaseReceiptId),
    CONSTRAINT CK_SupplierPayables_OpeningPaid CHECK (OpeningPaid >= 0)
);
IF OBJECT_ID(N'dbo.SupplierPayments') IS NULL
CREATE TABLE dbo.SupplierPayments (
    Id INT IDENTITY(1,1) NOT NULL CONSTRAINT PK_SupplierPayments PRIMARY KEY,
    PayableId INT NOT NULL CONSTRAINT FK_SupplierPayments_Payable FOREIGN KEY REFERENCES dbo.SupplierPayables(Id),
    OperationId UNIQUEIDENTIFIER NOT NULL CONSTRAINT UQ_SupplierPayments_Operation UNIQUE,
    CreatedAt DATETIME2 NOT NULL,
    CreatedById INT NOT NULL CONSTRAINT FK_SupplierPayments_User FOREIGN KEY REFERENCES dbo.Users(Id),
    Amount DECIMAL(18,2) NOT NULL CONSTRAINT CK_SupplierPayments_Amount CHECK (Amount > 0),
    Method NVARCHAR(30) NOT NULL,
    Note NVARCHAR(200) NULL
);

IF OBJECT_ID(N'dbo.PurchaseReceiptLines') IS NULL
CREATE TABLE dbo.PurchaseReceiptLines (
    Id INT IDENTITY(1,1) NOT NULL CONSTRAINT PK_PurchaseReceiptLines PRIMARY KEY,
    PurchaseReceiptId INT NOT NULL CONSTRAINT FK_PurchaseReceiptLines_PurchaseReceipts FOREIGN KEY REFERENCES dbo.PurchaseReceipts(Id),
    ProductId INT NOT NULL CONSTRAINT FK_PurchaseReceiptLines_Products FOREIGN KEY REFERENCES dbo.Products(Id),
    ProductName NVARCHAR(100) NOT NULL,
    Quantity INT NOT NULL,
    UnitCost DECIMAL(18,2) NOT NULL,
    LineTotal DECIMAL(18,2) NOT NULL
);
IF COL_LENGTH(N'dbo.PurchaseReceiptLines', N'BatchNumber') IS NULL
    ALTER TABLE dbo.PurchaseReceiptLines ADD BatchNumber NVARCHAR(50) NULL, ExpiryDate DATE NULL;

IF OBJECT_ID(N'dbo.ProductBatches') IS NULL
CREATE TABLE dbo.ProductBatches (
    Id INT IDENTITY(1,1) NOT NULL CONSTRAINT PK_ProductBatches PRIMARY KEY,
    ProductId INT NOT NULL CONSTRAINT FK_ProductBatches_Products FOREIGN KEY REFERENCES dbo.Products(Id),
    BatchNumber NVARCHAR(50) NOT NULL,
    ExpiryDate DATE NULL,
    ReceivedAt DATETIME2(7) NOT NULL,
    Quantity INT NOT NULL,
    RemainingQuantity INT NOT NULL,
    PurchaseReceiptId INT NULL CONSTRAINT FK_ProductBatches_PurchaseReceipts FOREIGN KEY REFERENCES dbo.PurchaseReceipts(Id)
);
IF NOT EXISTS (SELECT 1 FROM sys.indexes WHERE name = N'IX_ProductBatches_ProductExpiry' AND object_id = OBJECT_ID(N'dbo.ProductBatches'))
    CREATE INDEX IX_ProductBatches_ProductExpiry ON dbo.ProductBatches(ProductId, ExpiryDate, Id);
IF COL_LENGTH(N'dbo.ProductBatches', N'UnitCost') IS NULL
    ALTER TABLE dbo.ProductBatches ADD UnitCost DECIMAL(18,2) NULL;

IF OBJECT_ID(N'dbo.BatchMovements') IS NULL
CREATE TABLE dbo.BatchMovements (
    Id INT IDENTITY(1,1) NOT NULL CONSTRAINT PK_BatchMovements PRIMARY KEY,
    BatchId INT NOT NULL CONSTRAINT FK_BatchMovements_Batch FOREIGN KEY REFERENCES dbo.ProductBatches(Id),
    ProductId INT NOT NULL CONSTRAINT FK_BatchMovements_Product FOREIGN KEY REFERENCES dbo.Products(Id),
    SaleInvoiceLineId INT NULL,
    CreatedAt DATETIME2(7) NOT NULL,
    ReferenceNumber NVARCHAR(40) NOT NULL,
    Kind NVARCHAR(30) NOT NULL,
    Quantity INT NOT NULL,
    CreatedById INT NOT NULL CONSTRAINT FK_BatchMovements_User FOREIGN KEY REFERENCES dbo.Users(Id)
);
IF COL_LENGTH(N'dbo.BatchMovements', N'UnitCost') IS NULL
    ALTER TABLE dbo.BatchMovements ADD UnitCost DECIMAL(18,2) NULL;

IF OBJECT_ID(N'dbo.SaleInvoices') IS NULL
CREATE TABLE dbo.SaleInvoices (
    Id INT IDENTITY(1,1) NOT NULL CONSTRAINT PK_SaleInvoices PRIMARY KEY,
    OperationId UNIQUEIDENTIFIER NOT NULL,
    Number NVARCHAR(40) NOT NULL,
    CreatedAt DATETIME2(7) NOT NULL,
    CustomerId INT NULL CONSTRAINT FK_SaleInvoices_Customers FOREIGN KEY REFERENCES dbo.Customers(Id),
    CustomerName NVARCHAR(100) NOT NULL,
    CreatedById INT NOT NULL CONSTRAINT FK_SaleInvoices_Users FOREIGN KEY REFERENCES dbo.Users(Id),
    Subtotal DECIMAL(18,2) NOT NULL,
    DiscountPercent DECIMAL(18,2) NOT NULL,
    DiscountAmount DECIMAL(18,2) NOT NULL,
    Total DECIMAL(18,2) NOT NULL,
    CashReceived DECIMAL(18,2) NOT NULL,
    Change DECIMAL(18,2) NOT NULL
);

IF OBJECT_ID(N'dbo.SaleInvoiceLines') IS NULL
CREATE TABLE dbo.SaleInvoiceLines (
    Id INT IDENTITY(1,1) NOT NULL CONSTRAINT PK_SaleInvoiceLines PRIMARY KEY,
    SaleInvoiceId INT NOT NULL CONSTRAINT FK_SaleInvoiceLines_SaleInvoices FOREIGN KEY REFERENCES dbo.SaleInvoices(Id),
    ProductId INT NOT NULL CONSTRAINT FK_SaleInvoiceLines_Products FOREIGN KEY REFERENCES dbo.Products(Id),
    ProductName NVARCHAR(100) NOT NULL,
    Quantity INT NOT NULL,
    UnitPrice DECIMAL(18,2) NOT NULL,
    LineTotal DECIMAL(18,2) NOT NULL
);
IF NOT EXISTS (SELECT 1 FROM sys.foreign_keys WHERE name = N'FK_BatchMovements_SaleLine')
    ALTER TABLE dbo.BatchMovements ADD CONSTRAINT FK_BatchMovements_SaleLine FOREIGN KEY (SaleInvoiceLineId) REFERENCES dbo.SaleInvoiceLines(Id);

IF OBJECT_ID(N'dbo.StockMovements') IS NULL
CREATE TABLE dbo.StockMovements (
    Id INT IDENTITY(1,1) NOT NULL CONSTRAINT PK_StockMovements PRIMARY KEY,
    CreatedAt DATETIME2(7) NOT NULL,
    ProductId INT NOT NULL CONSTRAINT FK_StockMovements_Products FOREIGN KEY REFERENCES dbo.Products(Id),
    ProductName NVARCHAR(100) NOT NULL,
    Kind INT NOT NULL,  
    Quantity INT NOT NULL,  
    StockBefore INT NOT NULL,
    StockAfter INT NOT NULL,
    ReferenceNumber NVARCHAR(40) NOT NULL,
    CreatedById INT NOT NULL CONSTRAINT FK_StockMovements_Users FOREIGN KEY REFERENCES dbo.Users(Id),
    PurchaseReceiptId INT NULL CONSTRAINT FK_StockMovements_PurchaseReceipts FOREIGN KEY REFERENCES dbo.PurchaseReceipts(Id),
    SaleInvoiceId INT NULL CONSTRAINT FK_StockMovements_SaleInvoices FOREIGN KEY REFERENCES dbo.SaleInvoices(Id)
);



IF OBJECT_ID(N'dbo.Promotions') IS NULL
CREATE TABLE dbo.Promotions (
    Id INT IDENTITY(1,1) NOT NULL CONSTRAINT PK_Promotions PRIMARY KEY,
    Code NVARCHAR(30) NOT NULL CONSTRAINT UQ_Promotions_Code UNIQUE,
    Description NVARCHAR(200) NULL,
    DiscountPercent DECIMAL(5,2) NOT NULL CONSTRAINT CK_Promotions_Percent CHECK (DiscountPercent > 0 AND DiscountPercent <= 100),
    ValidFrom DATE NULL,   
    ValidTo DATE NULL,     
    IsActive BIT NOT NULL CONSTRAINT DF_Promotions_IsActive DEFAULT (1),
    CONSTRAINT CK_Promotions_Dates CHECK (ValidFrom IS NULL OR ValidTo IS NULL OR ValidFrom <= ValidTo)
);



IF COL_LENGTH(N'dbo.SaleInvoices', N'PromotionCode') IS NULL
    ALTER TABLE dbo.SaleInvoices ADD PromotionCode NVARCHAR(30) NULL;
IF COL_LENGTH(N'dbo.SaleInvoices', N'PaymentMethod') IS NULL
    ALTER TABLE dbo.SaleInvoices ADD PaymentMethod NVARCHAR(30) NOT NULL
        CONSTRAINT DF_SaleInvoices_PaymentMethod DEFAULT (N'Tiền mặt');
IF COL_LENGTH(N'dbo.SaleInvoices', N'ShiftId') IS NULL
    ALTER TABLE dbo.SaleInvoices ADD ShiftId INT NULL CONSTRAINT FK_SaleInvoices_Shifts FOREIGN KEY REFERENCES dbo.Shifts(Id);
IF COL_LENGTH(N'dbo.SaleInvoices', N'PromotionAmount') IS NULL
    ALTER TABLE dbo.SaleInvoices ADD PromotionAmount DECIMAL(18,2) NOT NULL
        CONSTRAINT DF_SaleInvoices_PromotionAmount DEFAULT (0);



IF COL_LENGTH(N'dbo.SaleInvoices', N'IsCancelled') IS NULL
    ALTER TABLE dbo.SaleInvoices ADD IsCancelled BIT NOT NULL
        CONSTRAINT DF_SaleInvoices_IsCancelled DEFAULT (0);
IF COL_LENGTH(N'dbo.SaleInvoices', N'CancelledAt') IS NULL
    ALTER TABLE dbo.SaleInvoices ADD CancelledAt DATETIME2(7) NULL;
IF COL_LENGTH(N'dbo.SaleInvoices', N'CancelledById') IS NULL
    ALTER TABLE dbo.SaleInvoices ADD CancelledById INT NULL
        CONSTRAINT FK_SaleInvoices_CancelledBy FOREIGN KEY REFERENCES dbo.Users(Id);
IF COL_LENGTH(N'dbo.SaleInvoices', N'CancelReason') IS NULL
    ALTER TABLE dbo.SaleInvoices ADD CancelReason NVARCHAR(200) NULL;
IF COL_LENGTH(N'dbo.StockMovements', N'Note') IS NULL
    ALTER TABLE dbo.StockMovements ADD Note NVARCHAR(200) NULL;

IF OBJECT_ID(N'dbo.SaleReturns') IS NULL
CREATE TABLE dbo.SaleReturns (
    Id INT IDENTITY(1,1) NOT NULL CONSTRAINT PK_SaleReturns PRIMARY KEY,
    Number NVARCHAR(40) NOT NULL,
    CreatedAt DATETIME2(7) NOT NULL,
    SaleInvoiceId INT NOT NULL CONSTRAINT FK_SaleReturns_SaleInvoices FOREIGN KEY REFERENCES dbo.SaleInvoices(Id),
    CreatedById INT NOT NULL CONSTRAINT FK_SaleReturns_Users FOREIGN KEY REFERENCES dbo.Users(Id),
    ReturnKind NVARCHAR(20) NOT NULL,
    Reason NVARCHAR(200) NOT NULL,
    RefundAmount DECIMAL(18,2) NOT NULL
);
IF COL_LENGTH(N'dbo.SaleReturns', N'ReplacementProductId') IS NULL
    ALTER TABLE dbo.SaleReturns ADD ReplacementProductId INT NULL, ReplacementProductName NVARCHAR(100) NULL,
        ReplacementQuantity INT NOT NULL CONSTRAINT DF_SaleReturns_ReplacementQuantity DEFAULT (0),
        ReplacementAmount DECIMAL(18,2) NOT NULL CONSTRAINT DF_SaleReturns_ReplacementAmount DEFAULT (0);

IF OBJECT_ID(N'dbo.SaleReturnLines') IS NULL
CREATE TABLE dbo.SaleReturnLines (
    Id INT IDENTITY(1,1) NOT NULL CONSTRAINT PK_SaleReturnLines PRIMARY KEY,
    SaleReturnId INT NOT NULL CONSTRAINT FK_SaleReturnLines_SaleReturns FOREIGN KEY REFERENCES dbo.SaleReturns(Id),
    SaleInvoiceLineId INT NOT NULL CONSTRAINT FK_SaleReturnLines_SaleInvoiceLines FOREIGN KEY REFERENCES dbo.SaleInvoiceLines(Id),
    ProductId INT NOT NULL CONSTRAINT FK_SaleReturnLines_Products FOREIGN KEY REFERENCES dbo.Products(Id),
    ProductName NVARCHAR(100) NOT NULL,
    Quantity INT NOT NULL,
    UnitPrice DECIMAL(18,2) NOT NULL,
    LineTotal DECIMAL(18,2) NOT NULL
);

IF NOT EXISTS (SELECT 1 FROM sys.indexes WHERE name = N'IX_SaleReturns_SaleInvoiceId')
    CREATE INDEX IX_SaleReturns_SaleInvoiceId ON dbo.SaleReturns(SaleInvoiceId);
IF NOT EXISTS (SELECT 1 FROM sys.indexes WHERE name = N'IX_SaleReturnLines_SaleInvoiceLineId')
    CREATE INDEX IX_SaleReturnLines_SaleInvoiceLineId ON dbo.SaleReturnLines(SaleInvoiceLineId);


GO


IF NOT EXISTS (SELECT 1 FROM sys.indexes WHERE name = N'IX_PurchaseReceipts_OperationId') CREATE UNIQUE INDEX IX_PurchaseReceipts_OperationId ON dbo.PurchaseReceipts(OperationId);
IF NOT EXISTS (SELECT 1 FROM sys.indexes WHERE name = N'IX_PurchaseReceipts_Number') CREATE UNIQUE INDEX IX_PurchaseReceipts_Number ON dbo.PurchaseReceipts(Number);
IF NOT EXISTS (SELECT 1 FROM sys.indexes WHERE name = N'IX_PurchaseReceipts_SupplierId') CREATE INDEX IX_PurchaseReceipts_SupplierId ON dbo.PurchaseReceipts(SupplierId);
IF NOT EXISTS (SELECT 1 FROM sys.indexes WHERE name = N'IX_PurchaseReceipts_CreatedById') CREATE INDEX IX_PurchaseReceipts_CreatedById ON dbo.PurchaseReceipts(CreatedById);
IF NOT EXISTS (SELECT 1 FROM sys.indexes WHERE name = N'IX_PurchaseReceiptLines_PurchaseReceiptId') CREATE INDEX IX_PurchaseReceiptLines_PurchaseReceiptId ON dbo.PurchaseReceiptLines(PurchaseReceiptId);
IF NOT EXISTS (SELECT 1 FROM sys.indexes WHERE name = N'IX_PurchaseReceiptLines_ProductId') CREATE INDEX IX_PurchaseReceiptLines_ProductId ON dbo.PurchaseReceiptLines(ProductId);
IF NOT EXISTS (SELECT 1 FROM sys.indexes WHERE name = N'IX_SaleInvoices_OperationId') CREATE UNIQUE INDEX IX_SaleInvoices_OperationId ON dbo.SaleInvoices(OperationId);
IF NOT EXISTS (SELECT 1 FROM sys.indexes WHERE name = N'IX_SaleInvoices_Number') CREATE UNIQUE INDEX IX_SaleInvoices_Number ON dbo.SaleInvoices(Number);
IF NOT EXISTS (SELECT 1 FROM sys.indexes WHERE name = N'IX_SaleInvoices_CustomerId') CREATE INDEX IX_SaleInvoices_CustomerId ON dbo.SaleInvoices(CustomerId);
IF NOT EXISTS (SELECT 1 FROM sys.indexes WHERE name = N'IX_SaleInvoices_CreatedById') CREATE INDEX IX_SaleInvoices_CreatedById ON dbo.SaleInvoices(CreatedById);
IF NOT EXISTS (SELECT 1 FROM sys.indexes WHERE name = N'IX_SaleInvoiceLines_SaleInvoiceId') CREATE INDEX IX_SaleInvoiceLines_SaleInvoiceId ON dbo.SaleInvoiceLines(SaleInvoiceId);
IF NOT EXISTS (SELECT 1 FROM sys.indexes WHERE name = N'IX_SaleInvoiceLines_ProductId') CREATE INDEX IX_SaleInvoiceLines_ProductId ON dbo.SaleInvoiceLines(ProductId);
IF NOT EXISTS (SELECT 1 FROM sys.indexes WHERE name = N'IX_StockMovements_ProductId') CREATE INDEX IX_StockMovements_ProductId ON dbo.StockMovements(ProductId);
IF NOT EXISTS (SELECT 1 FROM sys.indexes WHERE name = N'IX_StockMovements_CreatedById') CREATE INDEX IX_StockMovements_CreatedById ON dbo.StockMovements(CreatedById);
IF NOT EXISTS (SELECT 1 FROM sys.indexes WHERE name = N'IX_StockMovements_PurchaseReceiptId') CREATE INDEX IX_StockMovements_PurchaseReceiptId ON dbo.StockMovements(PurchaseReceiptId);
IF NOT EXISTS (SELECT 1 FROM sys.indexes WHERE name = N'IX_StockMovements_SaleInvoiceId') CREATE INDEX IX_StockMovements_SaleInvoiceId ON dbo.StockMovements(SaleInvoiceId);
GO


INSERT INTO dbo.Roles (Name)
SELECT v.Name FROM (VALUES (N'Admin'), (N'ThuKho'), (N'ThuNgan')) AS v(Name)
WHERE NOT EXISTS (SELECT 1 FROM dbo.Roles r WHERE r.Name = v.Name);


INSERT INTO dbo.Users (Username, PasswordHash, FullName, IsActive, RoleId)
SELECT N'admin', N'e86f78a8a3caf0b60d8e74e5942aa6d86dc150cd3c03338aef25b7d2d7e3acc7',
       N'Quản trị viên', 1, r.Id
FROM dbo.Roles r
WHERE r.Name = N'Admin' AND NOT EXISTS (SELECT 1 FROM dbo.Users WHERE Username = N'admin');
GO


INSERT INTO dbo.RolePermissions (RoleId, FunctionCode)
SELECT r.Id, v.Code
FROM (VALUES
    (N'Admin', N'mnuUserManagement'), (N'Admin', N'mnuRoleManagement'),
    (N'Admin', N'mnuCategory'), (N'Admin', N'mnuProduct'), (N'Admin', N'mnuSupplier'),
    (N'Admin', N'mnuCustomer'), (N'Admin', N'mnuPromotion'),
    (N'Admin', N'mnuPurchaseReceipt'), (N'Admin', N'mnuSales'), (N'Admin', N'mnuInvoice'),
    (N'Admin', N'mnuStockMovement'), (N'Admin', N'mnuRevenueReport'), (N'Admin', N'mnuDashboard'),
    (N'Admin', N'mnuPurchaseHistory'), (N'Admin', N'mnuStockAdjust'), (N'Admin', N'fnInvoiceCancel'), (N'Admin', N'fnSaleReturn'),
    (N'ThuKho', N'mnuCategory'), (N'ThuKho', N'mnuProduct'), (N'ThuKho', N'mnuSupplier'),
    (N'ThuKho', N'mnuPurchaseReceipt'), (N'ThuKho', N'mnuStockMovement'),
    (N'ThuKho', N'mnuPurchaseHistory'), (N'ThuKho', N'mnuStockAdjust'),
    (N'ThuNgan', N'mnuCustomer'), (N'ThuNgan', N'mnuSales'), (N'ThuNgan', N'mnuInvoice'), (N'ThuNgan', N'fnSaleReturn'), (N'ThuNgan', N'mnuShift')
) AS v(RoleName, Code)
JOIN dbo.Roles r ON r.Name = v.RoleName
WHERE NOT EXISTS (SELECT 1 FROM dbo.RolePermissions p WHERE p.RoleId = r.Id AND p.FunctionCode = v.Code);
GO


INSERT INTO dbo.Promotions (Code, Description, DiscountPercent, ValidFrom, ValidTo, IsActive)
SELECT v.Code, v.Description, v.Pct, NULL, NULL, 1
FROM (VALUES
    (N'KM5',  N'Giảm 5% hóa đơn',  CAST(5.00 AS DECIMAL(5,2))),
    (N'KM10', N'Giảm 10% hóa đơn', CAST(10.00 AS DECIMAL(5,2)))
) AS v(Code, Description, Pct)
WHERE NOT EXISTS (SELECT 1 FROM dbo.Promotions p WHERE p.Code = v.Code);
GO




INSERT INTO dbo.Categories (Code, Name)
SELECT v.Code, v.Name
FROM (VALUES
    (N'DOUONG', N'Đồ uống'), (N'BANHKEO', N'Bánh kẹo & snack'), (N'TPPK', N'Mì & thực phẩm khô'),
    (N'SUA', N'Sữa & chế phẩm'), (N'GIAVI', N'Gia vị & đồ hộp'), (N'DOCANHAN', N'Đồ dùng cá nhân')
) AS v(Code, Name)
WHERE NOT EXISTS (SELECT 1 FROM dbo.Categories c WHERE c.Code = v.Code OR c.Name = v.Name);
GO

INSERT INTO dbo.Products (Name, Barcode, Price, Stock, CategoryId)
SELECT v.Name, v.Barcode, v.Price, v.Stock, c.Id
FROM (VALUES
    (N'Nước suối 500ml',             N'893100000001',  5000, 120, N'Đồ uống'),
    (N'Nước ngọt cola lon 330ml',    N'893100000002', 10000,  96, N'Đồ uống'),
    (N'Trà xanh đóng chai 455ml',    N'893100000003', 11000,  72, N'Đồ uống'),
    (N'Cà phê sữa lon 180ml',        N'893100000004', 14000,  60, N'Đồ uống'),
    (N'Bánh quy bơ gói 150g',        N'893100000005', 22000,  40, N'Bánh kẹo & snack'),
    (N'Snack khoai tây 54g',         N'893100000006', 12000,  80, N'Bánh kẹo & snack'),
    (N'Kẹo bạc hà gói 100g',         N'893100000007',  9000,  50, N'Bánh kẹo & snack'),
    (N'Mì gói tôm',                  N'893100000008',  4500, 200, N'Mì & thực phẩm khô'),
    (N'Mì ly hải sản',               N'893100000009', 14000,  60, N'Mì & thực phẩm khô'),
    (N'Gạo thơm túi 5kg',            N'893100000010', 98000,  25, N'Mì & thực phẩm khô'),
    (N'Sữa tươi hộp 1 lít',          N'893100000011', 32000,  45, N'Sữa & chế phẩm'),
    (N'Sữa chua uống chai 100ml',    N'893100000012',  7000,  90, N'Sữa & chế phẩm'),
    (N'Nước mắm chai 500ml',         N'893100000013', 38000,  30, N'Gia vị & đồ hộp'),
    (N'Cá hộp sốt cà 155g',          N'893100000014', 24000,  35, N'Gia vị & đồ hộp'),
    (N'Dầu ăn chai 1 lít',           N'893100000015', 52000,  28, N'Gia vị & đồ hộp'),
    (N'Nước rửa tay 250ml',          N'893100000016', 35000,  30, N'Đồ dùng cá nhân'),
    (N'Bàn chải đánh răng',          N'893100000017', 18000,  40, N'Đồ dùng cá nhân'),
    (N'Khăn giấy hộp',               N'893100000018', 16000,  55, N'Đồ dùng cá nhân')
) AS v(Name, Barcode, Price, Stock, Cat)
JOIN dbo.Categories c ON c.Name = v.Cat
WHERE NOT EXISTS (SELECT 1 FROM dbo.Products p WHERE p.Barcode = v.Barcode);
GO

INSERT INTO dbo.Suppliers (Name, Phone, IsActive)
SELECT v.Name, v.Phone, 1
FROM (VALUES
    (N'Công ty TNHH Phân phối Minh Phát', N'0281110001'),
    (N'Công ty CP Thực phẩm Hòa Bình',    N'0281110002'),
    (N'Đại lý Nước giải khát Thanh Tâm',  N'0281110003')
) AS v(Name, Phone)
WHERE NOT EXISTS (SELECT 1 FROM dbo.Suppliers s WHERE s.Name = v.Name);
GO

INSERT INTO dbo.Customers (Name, Phone, IsVip, IsActive)
SELECT v.Name, v.Phone, v.IsVip, 1
FROM (VALUES
    (N'Nguyễn Văn An', N'0900000001', CAST(1 AS BIT)),
    (N'Trần Thị Bình', N'0900000002', CAST(1 AS BIT)),
    (N'Lê Hoàng Cường', N'0900000003', CAST(0 AS BIT)),
    (N'Phạm Thu Dung', N'0900000004', CAST(0 AS BIT))
) AS v(Name, Phone, IsVip)
WHERE NOT EXISTS (SELECT 1 FROM dbo.Customers c WHERE c.Phone = v.Phone);
GO
