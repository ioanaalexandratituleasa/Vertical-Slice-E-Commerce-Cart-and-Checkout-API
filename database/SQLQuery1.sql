DROP TABLE OrderItems;
DROP TABLE Book;
DROP TABLE OrderTable;
DROP TABLE Products;
DROP TABLE Users;

CREATE TABLE Users (
    UserID  INT IDENTITY(1,1) PRIMARY KEY,
    FName   VARCHAR(255) NOT NULL,
    LName   VARCHAR(255) NOT NULL,
    Email   VARCHAR(255) NOT NULL,
    Pasword VARCHAR(255)
);

CREATE TABLE Products (
    ProductID    INT IDENTITY(1,1) PRIMARY KEY,
    Title        VARCHAR(255) NOT NULL,
    DescriptionP VARCHAR(300),
    Price        FLOAT NOT NULL,
    ImageP       VARCHAR(255),
    Stock        INT DEFAULT 0
);

CREATE TABLE OrderTable (
    OrderID   INT IDENTITY(1,1) PRIMARY KEY,
    DateOrder DATE NOT NULL,
    Address   VARCHAR(255) NOT NULL,
    UserID    INT,
    CONSTRAINT fk_users
    FOREIGN KEY (UserID) REFERENCES Users(UserID)
);

CREATE TABLE Book (
    UserID    INT,
    ProductID INT,
    CONSTRAINT fk_user
    FOREIGN KEY (UserID) REFERENCES Users(UserID),
    CONSTRAINT product_fk
    FOREIGN KEY (ProductID) REFERENCES Products(ProductID)
);

CREATE TABLE OrderItems (
    OrderID    INT,
    ProductID  INT,
    TotalPrice DECIMAL(18,2) NOT NULL,
    Quantity   INT NOT NULL,
    CONSTRAINT FK_OrderItems_Order
    FOREIGN KEY (OrderID) REFERENCES OrderTable(OrderID),
    CONSTRAINT FK_OrderItems_Product
    FOREIGN KEY (ProductID) REFERENCES Products(ProductID)
);