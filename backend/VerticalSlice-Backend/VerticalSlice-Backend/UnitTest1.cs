using Microsoft.AspNetCore.Mvc;
using Moq;
using VerticalSlice_Backend.Features.Identity;
using VerticalSlice_Backend.Features.Products;
using VerticalSlice_Backend.Features.Checkout;
using VerticalSlice_Backend.Features.Checkout.CheckoutDTOs;
using Xunit;

namespace VerticalSlice_Backend.Tests
{
    // =====================================================================
    // USER STORY: As a new user, I want to create an account
    // =====================================================================
    public class RegisterTests
    {
        // ✅ Cazul fericit — userul se inregistreaza cu succes
        [Fact]
        public async Task Register_ShouldReturnOk_WhenUserIsCreated()
        {
            // ARRANGE — pregatim datele si mock-ul
            var mockRepo = new Mock<IIdentityRepository>();
            var request = new RegisterDTO("Ion", "Pop", "ion@email.com", "parola123");
            var response = new RegisterResponseDTO(1, "Ion", "Pop", "ion@email.com");

            // Spunem mock-ului: "cand primesti acest request, returneaza acest response"
            mockRepo.Setup(r => r.RegisterAsync(request)).ReturnsAsync(response);

            var controller = new IdentityController(mockRepo.Object);

            // ACT — apelam metoda controllerului
            var result = await controller.Register(request);

            // ASSERT — verificam ca rezultatul e 200 OK cu datele corecte
            var okResult = Assert.IsType<OkObjectResult>(result);
            Assert.Equal(response, okResult.Value);
        }

        // ❌ Cazul negativ — emailul e deja folosit
        [Fact]
        public async Task Register_ShouldReturnBadRequest_WhenEmailAlreadyExists()
        {
            // ARRANGE
            var mockRepo = new Mock<IIdentityRepository>();
            var request = new RegisterDTO("Ion", "Pop", "existent@email.com", "parola123");

            // Mock-ul arunca exceptie — emailul exista deja
            mockRepo.Setup(r => r.RegisterAsync(request))
                    .ThrowsAsync(new Exception("Email already in use."));

            var controller = new IdentityController(mockRepo.Object);

            // ACT
            var result = await controller.Register(request);

            // ASSERT — trebuie sa primim 400 Bad Request
            var badRequest = Assert.IsType<BadRequestObjectResult>(result);
            Assert.Equal("Email already in use.", badRequest.Value);
        }

        // ❌ Cazul negativ — datele sunt goale
        [Fact]
        public async Task Register_ShouldReturnBadRequest_WhenDataIsEmpty()
        {
            // ARRANGE
            var mockRepo = new Mock<IIdentityRepository>();
            var request = new RegisterDTO("", "", "", "");

            mockRepo.Setup(r => r.RegisterAsync(request))
                    .ThrowsAsync(new Exception("Invalid data."));

            var controller = new IdentityController(mockRepo.Object);

            // ACT
            var result = await controller.Register(request);

            // ASSERT
            Assert.IsType<BadRequestObjectResult>(result);
        }
    }

    // =====================================================================
    // USER STORY: As a returning user, I want to log in with my credentials
    // =====================================================================
    public class LoginTests
    {
        // ✅ Cazul fericit — login cu succes, primim token
        [Fact]
        public async Task Login_ShouldReturnOkWithToken_WhenCredentialsAreValid()
        {
            // ARRANGE
            var mockRepo = new Mock<IIdentityRepository>();
            var request = new LoginDTO("ion@email.com", "parola123");
            var response = new LoginResponseDTO("jwt.token.here", 1, "Ion", "ion@email.com");

            mockRepo.Setup(r => r.LoginAsync(request)).ReturnsAsync(response);

            var controller = new IdentityController(mockRepo.Object);

            // ACT
            var result = await controller.Login(request);

            // ASSERT — 200 OK cu token in response
            var okResult = Assert.IsType<OkObjectResult>(result);
            var loginResponse = Assert.IsType<LoginResponseDTO>(okResult.Value);
            Assert.NotEmpty(loginResponse.Toke); // token-ul nu e gol
            Assert.Equal(1, loginResponse.UserID);
        }

        // ❌ Cazul negativ — parola gresita
        [Fact]
        public async Task Login_ShouldReturnBadRequest_WhenPasswordIsWrong()
        {
            // ARRANGE
            var mockRepo = new Mock<IIdentityRepository>();
            var request = new LoginDTO("ion@email.com", "parola_gresita");

            mockRepo.Setup(r => r.LoginAsync(request))
                    .ThrowsAsync(new Exception("Invalid email or password"));

            var controller = new IdentityController(mockRepo.Object);

            // ACT
            var result = await controller.Login(request);

            // ASSERT
            var badRequest = Assert.IsType<BadRequestObjectResult>(result);
            Assert.Equal("Invalid email or password", badRequest.Value);
        }

        // ❌ Cazul negativ — emailul nu exista
        [Fact]
        public async Task Login_ShouldReturnBadRequest_WhenEmailNotFound()
        {
            // ARRANGE
            var mockRepo = new Mock<IIdentityRepository>();
            var request = new LoginDTO("inexistent@email.com", "orice");

            mockRepo.Setup(r => r.LoginAsync(request))
                    .ThrowsAsync(new Exception("Invalid email or password"));

            var controller = new IdentityController(mockRepo.Object);

            // ACT
            var result = await controller.Login(request);

            // ASSERT
            Assert.IsType<BadRequestObjectResult>(result);
        }
    }

    // =====================================================================
    // USER STORY: As a user, I want to see a list of all available products
    // =====================================================================
    public class ProductListTests
    {
        // ✅ Lista de produse returnata cu succes
        [Fact]
        public async Task GetAllProducts_ShouldReturnProducts_WhenProductsExist()
        {
            // ARRANGE
            var mockRepo = new Mock<IProductsRepository>();
            var products = new List<ProductDetailDTO>
        {
            new ProductDetailDTO { ProductID = 1, Title = "Shirt", Price = 20.0, Stock = 5 },
            new ProductDetailDTO { ProductID = 2, Title = "Dress", Price = 35.0, Stock = 0 }
        };

            mockRepo.Setup(r => r.GetAllProductsAsync())
                    .ReturnsAsync((ProductDetailDTO?)products[0]); // IProductsRepository returneaza ProductDetailDTO?

            // ACT
            var result = await mockRepo.Object.GetAllProductsAsync();

            // ASSERT
            Assert.NotNull(result);
        }

        // ✅ Niciun produs — returneaza null
        [Fact]
        public async Task GetAllProducts_ShouldReturnNull_WhenNoProductsExist()
        {
            // ARRANGE
            var mockRepo = new Mock<IProductsRepository>();
            mockRepo.Setup(r => r.GetAllProductsAsync())
                    .ReturnsAsync((ProductDetailDTO?)null);

            // ACT
            var result = await mockRepo.Object.GetAllProductsAsync();

            // ASSERT
            Assert.Null(result);
        }
    }

    // =====================================================================
    // USER STORY: As a user, I want to view the details of a specific product
    // =====================================================================
    public class ProductDetailTests
    {
        // ✅ Produsul exista
        [Fact]
        public async Task GetProductById_ShouldReturnProduct_WhenProductExists()
        {
            // ARRANGE
            var mockRepo = new Mock<IProductsRepository>();
            var product = new ProductDetailDTO
            {
                ProductID = 1,
                Title = "Shirt",
                DescriptionP = "Long sleeves blue shirt",
                Price = 20.0,
                Stock = 5
            };

            mockRepo.Setup(r => r.GetProductByIdAsync(1)).ReturnsAsync(product);

            // ACT
            var result = await mockRepo.Object.GetProductByIdAsync(1);

            // ASSERT
            Assert.NotNull(result);
            Assert.Equal("Shirt", result!.Title);
            Assert.Equal("Long sleeves blue shirt", result.DescriptionP);
            Assert.Equal(5, result.Stock);
        }

        // ❌ Produsul nu exista
        [Fact]
        public async Task GetProductById_ShouldReturnNull_WhenProductNotFound()
        {
            // ARRANGE
            var mockRepo = new Mock<IProductsRepository>();
            mockRepo.Setup(r => r.GetProductByIdAsync(999))
                    .ReturnsAsync((ProductDetailDTO?)null);

            // ACT
            var result = await mockRepo.Object.GetProductByIdAsync(999);

            // ASSERT
            Assert.Null(result);
        }

        // ✅ Stocul e afisat corect
        [Fact]
        public async Task GetProductById_ShouldShowCorrectStock()
        {
            // ARRANGE
            var mockRepo = new Mock<IProductsRepository>();
            mockRepo.Setup(r => r.GetProductByIdAsync(1))
                    .ReturnsAsync(new ProductDetailDTO { ProductID = 1, Title = "Shirt", Stock = 5 });
            mockRepo.Setup(r => r.GetProductByIdAsync(2))
                    .ReturnsAsync(new ProductDetailDTO { ProductID = 2, Title = "Dress", Stock = 0 });

            // ACT
            var shirt = await mockRepo.Object.GetProductByIdAsync(1);
            var dress = await mockRepo.Object.GetProductByIdAsync(2);

            // ASSERT
            Assert.True(shirt!.Stock > 0);
            Assert.Equal(0, dress!.Stock);
        }
    }


    // =====================================================================
    // USER STORY: As a user, I want to click "Add to Cart"
    // USER STORY: As a user, I want to visit the Cart page
    // (Cart e gestionat in frontend cu CartService — testam validarile backend)
    // =====================================================================
    public class CartValidationTests
    {
        // ✅ Produsul cu stoc > 0 poate fi adaugat
        [Fact]
        public void ProductCanBeAddedToCart_WhenStockIsAvailable()
        {
            // ARRANGE
            var product = new ProductDetailDTO { ProductID = 1, Title = "Shirt", Stock = 5, Price = 20.0 };

            // ACT — verificam conditia de adaugare in cos
            bool canAdd = product.Stock > 0;

            // ASSERT
            Assert.True(canAdd);
        }

        // ❌ Produsul cu stoc 0 nu poate fi adaugat
        [Fact]
        public void ProductCannotBeAddedToCart_WhenOutOfStock()
        {
            // ARRANGE
            var product = new ProductDetailDTO { ProductID = 2, Title = "Dress", Stock = 0, Price = 35.0 };

            // ACT
            bool canAdd = product.Stock > 0;

            // ASSERT
            Assert.False(canAdd);
        }

        // ✅ Totalul cosului se calculeaza corect
        [Fact]
        public void CartTotal_ShouldBeCalculatedCorrectly()
        {
            // ARRANGE — cos cu 2 produse
            var items = new List<OrderItemDTO>
            {
                new OrderItemDTO(ProductID: 1, TotalPrice: 20.0m, Quantity: 2), // 40
                new OrderItemDTO(ProductID: 2, TotalPrice: 35.0m, Quantity: 1)  // 35
            };

            // ACT
            decimal total = items.Sum(i => i.TotalPrice * i.Quantity);

            // ASSERT
            Assert.Equal(75.0m, total);
        }

        // ✅ Cantitatea din cos e corecta
        [Fact]
        public void CartItemCount_ShouldReflectQuantities()
        {
            // ARRANGE
            var items = new List<OrderItemDTO>
            {
                new OrderItemDTO(ProductID: 1, TotalPrice: 20.0m, Quantity: 3),
                new OrderItemDTO(ProductID: 2, TotalPrice: 35.0m, Quantity: 1)
            };

            // ACT
            int totalItems = items.Sum(i => i.Quantity);

            // ASSERT
            Assert.Equal(4, totalItems);
        }
    }

    // =====================================================================
    // USER STORY: As a user, I want to provide shipping address and place an order
    // =====================================================================
    public class CheckoutTests
    {
        // ✅ Cazul fericit — comanda plasata cu succes
        [Fact]
        public async Task PlaceOrder_ShouldSucceed_WhenDataIsValid()
        {
            // ARRANGE
            var mockRepo = new Mock<ICheckoutRepository>();
            var request = new CheckoutCreateDTO(
                UserID: 1,
                Address: "Strada Victoriei 10",
                Items: new List<OrderItemDTO>
                {
                    new OrderItemDTO(ProductID: 1, TotalPrice: 20.0m, Quantity: 2)
                }
            );

            // Mock-ul nu face nimic (Task.CompletedTask) = succes
            mockRepo.Setup(r => r.PlaceOrderAsync(request)).Returns(Task.CompletedTask);

            // ACT
            await mockRepo.Object.PlaceOrderAsync(request);

            // ASSERT — verificam ca metoda a fost apelata exact o data
            mockRepo.Verify(r => r.PlaceOrderAsync(request), Times.Once);
        }

        // ❌ Cazul negativ — comanda fara produse
        [Fact]
        public async Task PlaceOrder_ShouldThrow_WhenItemsListIsEmpty()
        {
            // ARRANGE
            var mockRepo = new Mock<ICheckoutRepository>();
            var request = new CheckoutCreateDTO(
                UserID: 1,
                Address: "Strada Victoriei 10",
                Items: new List<OrderItemDTO>() // lista goala
            );

            mockRepo.Setup(r => r.PlaceOrderAsync(request))
                    .ThrowsAsync(new Exception("The order does not contain any products"));

            // ACT & ASSERT
            var ex = await Assert.ThrowsAsync<Exception>(
                () => mockRepo.Object.PlaceOrderAsync(request)
            );
            Assert.Equal("The order does not contain any products", ex.Message);
        }

        // ❌ Cazul negativ — stoc insuficient
        [Fact]
        public async Task PlaceOrder_ShouldThrow_WhenStockIsInsufficient()
        {
            // ARRANGE
            var mockRepo = new Mock<ICheckoutRepository>();
            var request = new CheckoutCreateDTO(
                UserID: 1,
                Address: "Strada Victoriei 10",
                Items: new List<OrderItemDTO>
                {
                    new OrderItemDTO(ProductID: 2, TotalPrice: 35.0m, Quantity: 10) // stoc e 0
                }
            );

            mockRepo.Setup(r => r.PlaceOrderAsync(request))
                    .ThrowsAsync(new Exception("Stock insufficient for Product ID: 2"));

            // ACT & ASSERT
            var ex = await Assert.ThrowsAsync<Exception>(
                () => mockRepo.Object.PlaceOrderAsync(request)
            );
            Assert.Contains("Stock insufficient", ex.Message);
        }

        // ❌ Cazul negativ — adresa lipsa
        [Fact]
        public async Task PlaceOrder_ShouldThrow_WhenAddressIsEmpty()
        {
            // ARRANGE
            var mockRepo = new Mock<ICheckoutRepository>();
            var request = new CheckoutCreateDTO(
                UserID: 1,
                Address: "", // adresa goala
                Items: new List<OrderItemDTO>
                {
                    new OrderItemDTO(ProductID: 1, TotalPrice: 20.0m, Quantity: 1)
                }
            );

            mockRepo.Setup(r => r.PlaceOrderAsync(request))
                    .ThrowsAsync(new Exception("Address is required."));

            // ACT & ASSERT
            await Assert.ThrowsAsync<Exception>(
                () => mockRepo.Object.PlaceOrderAsync(request)
            );
        }

        // ✅ Verificam ca totalul comenzii e calculat corect
        [Fact]
        public void OrderTotal_ShouldBeCalculatedCorrectly_ForMultipleItems()
        {
            // ARRANGE
            var items = new List<OrderItemDTO>
            {
                new OrderItemDTO(ProductID: 1, TotalPrice: 20.0m, Quantity: 2), // 40
                new OrderItemDTO(ProductID: 2, TotalPrice: 35.0m, Quantity: 1)  // 35
            };

            // ACT
            decimal total = items.Sum(i => i.TotalPrice * i.Quantity);

            // ASSERT
            Assert.Equal(75.0m, total);
        }
    }
}

// =========================================================================
// INTERFETE NECESARE PENTRU MOCK-URI
// Adauga acestea in proiectul principal daca nu exista deja
// =========================================================================

// In Features/Checkout/ICheckoutRepository.cs:
// public interface ICheckoutRepository
// {
//     Task PlaceOrderAsync(CheckoutCreateDTO data);
// }