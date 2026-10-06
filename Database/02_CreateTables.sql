USE HealthPlus;
GO

/* =========================================================
   1. USERS
   ========================================================= */

CREATE TABLE Users
(
    Id INT IDENTITY(1,1) NOT NULL,

    Email NVARCHAR(255) NOT NULL,
    PasswordHash NVARCHAR(500) NOT NULL,

    FullName NVARCHAR(100) NOT NULL,
    Phone NVARCHAR(20) NULL,
    Address NVARCHAR(500) NULL,

    Role NVARCHAR(20) NOT NULL
        CONSTRAINT DF_Users_Role DEFAULT N'Customer',

    IsActive BIT NOT NULL
        CONSTRAINT DF_Users_IsActive DEFAULT 1,

    CreatedAt DATETIME2 NOT NULL
        CONSTRAINT DF_Users_CreatedAt DEFAULT GETDATE(),

    UpdatedAt DATETIME2 NOT NULL
        CONSTRAINT DF_Users_UpdatedAt DEFAULT GETDATE(),

    CONSTRAINT PK_Users
        PRIMARY KEY (Id),

    CONSTRAINT UQ_Users_Email
        UNIQUE (Email),

    CONSTRAINT CK_Users_Role
        CHECK (Role IN (N'Customer', N'Admin'))
);
GO

CREATE INDEX IX_Users_Email
ON Users(Email);
GO


/* =========================================================
   2. CATEGORIES
   ========================================================= */

CREATE TABLE Categories
(
    Id INT IDENTITY(1,1) NOT NULL,

    Name NVARCHAR(100) NOT NULL,
    Description NVARCHAR(500) NULL,

    IsActive BIT NOT NULL
        CONSTRAINT DF_Categories_IsActive DEFAULT 1,

    CreatedAt DATETIME2 NOT NULL
        CONSTRAINT DF_Categories_CreatedAt DEFAULT GETDATE(),

    CONSTRAINT PK_Categories
        PRIMARY KEY (Id),

    CONSTRAINT UQ_Categories_Name
        UNIQUE (Name)
);
GO


/* =========================================================
   3. PRODUCTS
   ========================================================= */

CREATE TABLE Products
(
    Id INT IDENTITY(1,1) NOT NULL,

    CategoryId INT NOT NULL,

    Name NVARCHAR(200) NOT NULL,
    Description NVARCHAR(MAX) NULL,

    Price DECIMAL(18,2) NOT NULL
        CONSTRAINT DF_Products_Price DEFAULT 0,

    Stock INT NOT NULL
        CONSTRAINT DF_Products_Stock DEFAULT 0,

    ImageUrl NVARCHAR(500) NULL,

    IsActive BIT NOT NULL
        CONSTRAINT DF_Products_IsActive DEFAULT 1,

    CreatedAt DATETIME2 NOT NULL
        CONSTRAINT DF_Products_CreatedAt DEFAULT GETDATE(),

    UpdatedAt DATETIME2 NOT NULL
        CONSTRAINT DF_Products_UpdatedAt DEFAULT GETDATE(),

    CONSTRAINT PK_Products
        PRIMARY KEY (Id),

    CONSTRAINT FK_Products_Categories
        FOREIGN KEY (CategoryId)
        REFERENCES Categories(Id),

    CONSTRAINT CK_Products_Price
        CHECK (Price >= 0),

    CONSTRAINT CK_Products_Stock
        CHECK (Stock >= 0)
);
GO

CREATE INDEX IX_Products_CategoryId
ON Products(CategoryId);
GO

CREATE INDEX IX_Products_Name
ON Products(Name);
GO


/* =========================================================
   4. CART ITEMS
   ========================================================= */

CREATE TABLE CartItems
(
    Id INT IDENTITY(1,1) NOT NULL,

    UserId INT NOT NULL,
    ProductId INT NOT NULL,

    Quantity INT NOT NULL,

    CreatedAt DATETIME2 NOT NULL
        CONSTRAINT DF_CartItems_CreatedAt DEFAULT GETDATE(),

    CONSTRAINT PK_CartItems
        PRIMARY KEY (Id),

    CONSTRAINT FK_CartItems_Users
        FOREIGN KEY (UserId)
        REFERENCES Users(Id)
        ON DELETE CASCADE,

    CONSTRAINT FK_CartItems_Products
        FOREIGN KEY (ProductId)
        REFERENCES Products(Id),

    CONSTRAINT UQ_CartItems_User_Product
        UNIQUE (UserId, ProductId),

    CONSTRAINT CK_CartItems_Quantity
        CHECK (Quantity > 0)
);
GO

CREATE INDEX IX_CartItems_UserId
ON CartItems(UserId);
GO


/* =========================================================
   5. ORDERS
   ========================================================= */

CREATE TABLE Orders
(
    Id INT IDENTITY(1,1) NOT NULL,

    OrderCode NVARCHAR(30) NOT NULL,

    UserId INT NOT NULL,

    OrderDate DATETIME2 NOT NULL
        CONSTRAINT DF_Orders_OrderDate DEFAULT GETDATE(),

    TotalAmount DECIMAL(18,2) NOT NULL,

    Status NVARCHAR(30) NOT NULL
        CONSTRAINT DF_Orders_Status DEFAULT N'Pending',

    PaymentStatus NVARCHAR(30) NOT NULL
        CONSTRAINT DF_Orders_PaymentStatus DEFAULT N'Pending',

    ShippingAddress NVARCHAR(500) NOT NULL,
    Phone NVARCHAR(20) NOT NULL,

    CreatedAt DATETIME2 NOT NULL
        CONSTRAINT DF_Orders_CreatedAt DEFAULT GETDATE(),

    CONSTRAINT PK_Orders
        PRIMARY KEY (Id),

    CONSTRAINT UQ_Orders_OrderCode
        UNIQUE (OrderCode),

    CONSTRAINT FK_Orders_Users
        FOREIGN KEY (UserId)
        REFERENCES Users(Id),

    CONSTRAINT CK_Orders_TotalAmount
        CHECK (TotalAmount >= 0),

    CONSTRAINT CK_Orders_Status
        CHECK
        (
            Status IN
            (
                N'Pending',
                N'Paid',
                N'Processing',
                N'Shipped',
                N'Delivered',
                N'Cancelled'
            )
        ),

    CONSTRAINT CK_Orders_PaymentStatus
        CHECK
        (
            PaymentStatus IN
            (
                N'Pending',
                N'Paid',
                N'Failed'
            )
        )
);
GO

CREATE INDEX IX_Orders_UserId
ON Orders(UserId);
GO

CREATE INDEX IX_Orders_OrderDate
ON Orders(OrderDate);
GO


/* =========================================================
   6. ORDER ITEMS
   ========================================================= */

CREATE TABLE OrderItems
(
    Id INT IDENTITY(1,1) NOT NULL,

    OrderId INT NOT NULL,
    ProductId INT NOT NULL,

    ProductName NVARCHAR(200) NOT NULL,

    UnitPrice DECIMAL(18,2) NOT NULL,

    Quantity INT NOT NULL,

    CONSTRAINT PK_OrderItems
        PRIMARY KEY (Id),

    CONSTRAINT FK_OrderItems_Orders
        FOREIGN KEY (OrderId)
        REFERENCES Orders(Id)
        ON DELETE CASCADE,

    CONSTRAINT FK_OrderItems_Products
        FOREIGN KEY (ProductId)
        REFERENCES Products(Id),

    CONSTRAINT CK_OrderItems_UnitPrice
        CHECK (UnitPrice >= 0),

    CONSTRAINT CK_OrderItems_Quantity
        CHECK (Quantity > 0)
);
GO

CREATE INDEX IX_OrderItems_OrderId
ON OrderItems(OrderId);
GO


/* =========================================================
   7. PAYMENTS
   ========================================================= */

CREATE TABLE Payments
(
    Id INT IDENTITY(1,1) NOT NULL,

    OrderId INT NOT NULL,

    Method NVARCHAR(30) NOT NULL,

    Amount DECIMAL(18,2) NOT NULL,

    Status NVARCHAR(30) NOT NULL
        CONSTRAINT DF_Payments_Status DEFAULT N'Pending',

    TransactionCode NVARCHAR(100) NULL,

    PaymentDate DATETIME2 NULL,

    CONSTRAINT PK_Payments
        PRIMARY KEY (Id),

    CONSTRAINT FK_Payments_Orders
        FOREIGN KEY (OrderId)
        REFERENCES Orders(Id)
        ON DELETE CASCADE,

    CONSTRAINT CK_Payments_Amount
        CHECK (Amount >= 0),

    CONSTRAINT CK_Payments_Method
        CHECK
        (
            Method IN
            (
                N'COD',
                N'MockCard'
            )
        ),

    CONSTRAINT CK_Payments_Status
        CHECK
        (
            Status IN
            (
                N'Pending',
                N'Paid',
                N'Failed'
            )
        )
);
GO

CREATE INDEX IX_Payments_OrderId
ON Payments(OrderId);
GO