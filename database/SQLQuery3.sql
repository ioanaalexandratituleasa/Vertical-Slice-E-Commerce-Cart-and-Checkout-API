INSERT INTO Users(UserID, FName, LName, Email, Pasword)
VALUES( 1, ' Ioana', 'Popescu', 'popescuioana@mail.com', '1234')

INSERT INTO Users(UserID, FName, LName, Email, Pasword)
VALUES( 2, ' Andrei', 'Iordan', 'andreiiordan@mail.com', '5678')

INSERT INTO Products( ProductID, Title, DescriptionP, Price, ImageP)
VALUES (1, 'Shirt', 'Long sleeves blue shirt', 20.00, 'poza1.jpg')

INSERT INTO Products( ProductID, Title, DescriptionP, Price, ImageP)
VALUES (2, 'Dress', 'Mini dress with flowers' , 35.00, 'poza2.jpg')


INSERT INTO OrderTable(OrderID, DateOrder, Address, UserID)
VALUES( 1, '2026-04-12', 'Street Revolution', 2) 

INSERT INTO OrderItems(OrderID, ProductID, TotalPrice, Quantity)
VALUES (1, 1, 40.00, 2) 

INSERT INTO OrderItems(OrderID, ProductID, TotalPrice, Quantity)
VALUES (1, 2, 35.00, 1) 

INSERT INTO Book(UserID, ProductID)
VALUES(1,1)