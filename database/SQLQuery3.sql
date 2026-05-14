INSERT INTO Users (FName, LName, Email, Password)
VALUES ('Ioana', 'Popescu', 'popescuioana@mail.com', '1234')

INSERT INTO Users (FName, LName, Email, Password)
VALUES ('Andrei', 'Iordan', 'andreiiordan@mail.com', '5678')

INSERT INTO Products (Title, DescriptionP, Price, ImageP, Stock)
VALUES ('Shirt', 'Long sleeves blue shirt', 20.00, 'poza1.jpg', 2)


INSERT INTO Products (Title, DescriptionP, Price, ImageP, Stock)
VALUES ('Skirt', 'Maxi red skirt', 25.00, '/images/poza2.jpg', 10)

INSERT INTO Products (Title, DescriptionP, Price, ImageP, Stock)
VALUES ('Dress', 'Mini dress with flowers', 35.00, 'poza3.jpg', 0)

INSERT INTO OrderTable (DateOrder, Address, UserID)
VALUES ('2026-04-12', 'Street Revolution', 7)

INSERT INTO OrderItems (OrderID, ProductID, TotalPrice, Quantity)
VALUES (1, 2, 40.00, 2)

INSERT INTO OrderItems (OrderID, ProductID, TotalPrice, Quantity)
VALUES (1, 3, 35.00, 1)

INSERT INTO Book (UserID, ProductID)
VALUES (1, 1)

SELECT * FROM Users
SELECT * FROM Products
SELECT * FROM OrderTable
SELECT * FROM OrderItems

USE VerticalSlice;
DELETE FROM Users WHERE UserID = 3;
DELETE FROM Users WHERE UserID = 1;

delete from OrderTable where OrderID=22

Delete from Products where ProductID = 5
Update  Products Set ImageP = '/images/poza2.jpg' where ProductID=2
Update  Products Set ImageP = '/images/poza1.jpg' where ProductID=3
Update  Products Set ImageP = '/images/poza3.jpg' where ProductID=7
