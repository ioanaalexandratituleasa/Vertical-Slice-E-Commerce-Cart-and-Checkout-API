This is a complete e-commerce platform developed using a modern Vertical Slice Architecture. The project emphasizes modularity, separation of concerns, and stability through rigorous automated testing.

Backend
ADO.NET (No ORM)
SQL Server Express
JWT Authentication
BCrypt.Net-Next (Password hashing)
xUnit + Moq (Unit testing)

Frontend
Angular 17+ 
TypeScript
Bootstrap 5
RxJS / Signals

Prerequisites
Ensure you have the following installed:

.NET SDK 10.0
SQL Server Express
Node.js
Angular CLI

Testing
Frontend: Karma & Jasmine
Backend: xUnit

Main Features
1. Identity & Security

JWT Authentication (Login) and User Registration.

Complex data validation for payloads (UserID, Address, Email).

Automatic redirection of unauthorized users to the Login page.

2. Product Catalog & Cart

Dynamic product listing with Loading States.

Product details with advanced stock logic (In Stock, Low Stock, Out of Stock).

Integrated shopping cart system with authentication status verification.

Database Configuration
1. Create the Database
Open SQL Server Management Studio (SSMS) and connect to your instance. Create a new database named VerticalSlice:
CREATE DATABASE VerticalSlice;

2. Create the Tables
Run the following script in the VerticalSlice database:
sqlUSE VerticalSlice;

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
    CONSTRAINT fk_users FOREIGN KEY (UserID) REFERENCES Users(UserID)
);

CREATE TABLE Book (
    UserID    INT,
    ProductID INT,
    CONSTRAINT fk_user    FOREIGN KEY (UserID)    REFERENCES Users(UserID),
    CONSTRAINT product_fk FOREIGN KEY (ProductID) REFERENCES Products(ProductID)
);

CREATE TABLE OrderItems (
    OrderID    INT,
    ProductID  INT,
    TotalPrice DECIMAL(18,2) NOT NULL,
    Quantity   INT NOT NULL,
    CONSTRAINT FK_OrderItems_Order   FOREIGN KEY (OrderID)   REFERENCES OrderTable(OrderID),
    CONSTRAINT FK_OrderItems_Product FOREIGN KEY (ProductID) REFERENCES Products(ProductID)
);

4. Insert Test Data
sqlUSE VerticalSlice;

INSERT INTO Products (Title, DescriptionP, Price, ImageP, Stock)
VALUES ('Shirt', 'Long sleeves blue shirt', 20.00, '/images/poza1.jpg', 10);

INSERT INTO Products (Title, DescriptionP, Price, ImageP, Stock)
VALUES ('Dress', 'Mini dress with flowers', 35.00, '/images/poza2.jpg', 5);


Backend Configuration
1. Clone the Repository
git clone <repository-url>
cd VerticalSlice-Backend

2. Configure Connection String
Open VerticalSlice-Backend/appsettings.json and update Server with your SQL Server instance name:
json{
  "ConnectionStrings": {
    "DefaultConnection": "Server=NUMELE_TAU\\SQLEXPRESS;Database=VerticalSlice;Trusted_Connection=True;TrustServerCertificate=True;"
  },
  "Jwt": {
    "Key": "SuperSecretKey12345SuperSecretKey12345",
    "Issuer": "VerticalSliceAPI",
    "Audience": "VerticalSliceClient"
  }
}

3. Restore NuGet Packages
dotnet restore

Frontend Configuration
1. Navigate to the frontend folder
cd frontend

2. Install dependencies
npm install

Running the Application
Backend
cd VerticalSlice-Backend/VerticalSlice-Backend
dotnet run

Frontend
cd frontend
ng serve

Running Tests
Backend Unit Tests (xUnit)
cd VerticalSlice_Backend.Tests
dotnet test

Frontend Tests
cd frontend
ng test
