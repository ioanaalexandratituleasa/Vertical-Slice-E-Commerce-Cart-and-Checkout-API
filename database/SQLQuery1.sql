CREATE TABLE Users(
UserID int PRIMARY KEY,
FName varchar(255) Not null,
LName varchar(255) not null,
Email varchar(255) not null,
Pasword varchar(255)
)

CREATE TABLE Products(
ProductID int primary key,
Title varchar(255) not null,
DescriptionP varchar(300),
Price float not null,
ImageP varchar(255)
)

ALTER TABLE Products
ADD Stock INT DEFAULT 0 

CREATE TABLE OrderTable(
OrderID int primary key,
DateOrder date not null,
Address varchar(255) not null,

UserID int,
CONSTRAINT fk_users 
FOREIGN KEY(UserID) 
REFERENCES Users(UserID)
)


CREATE TABLE Book(
UserID int,
ProductID int,

CONSTRAINT fk_user
FOREIGN KEY(UserID)
REFERENCES Users(UserID),

CONSTRAINT product_fk
FOREIGN KEY(ProductID)
REFERENCES Products(ProductID)
)

CREATE TABLE OrderItems (
    OrderID int,
    ProductID int,
    TotalPrice decimal(18, 2) not null,
    Quantity int not null,
    
    CONSTRAINT FK_OrderItems_Order
    FOREIGN KEY (OrderID) 
    REFERENCES OrderTable(OrderID),

    CONSTRAINT FK_OrderItems_Product 
    FOREIGN KEY (ProductID)
    REFERENCES Products(ProductID)
)
