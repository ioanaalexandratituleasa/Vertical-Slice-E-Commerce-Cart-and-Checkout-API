
DROP TABLE OrderItems;
DROP TABLE Book;
DROP TABLE OrderTable;

CREATE TABLE OrderTable (
    OrderID   INT IDENTITY(1,1) PRIMARY KEY,
    DateOrder DATE NOT NULL,
    Address   VARCHAR(255) NOT NULL,
    UserID    INT,
    CONSTRAINT fk_users
    FOREIGN KEY (UserID) REFERENCES Users(UserID)
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

CREATE TABLE Book (
    UserID    INT,
    ProductID INT,
    CONSTRAINT fk_user
    FOREIGN KEY (UserID) REFERENCES Users(UserID),
    CONSTRAINT product_fk
    FOREIGN KEY (ProductID) REFERENCES Products(ProductID)
);
SELECT COLUMNPROPERTY(OBJECT_ID('OrderTable'), 'OrderID', 'IsIdentity');