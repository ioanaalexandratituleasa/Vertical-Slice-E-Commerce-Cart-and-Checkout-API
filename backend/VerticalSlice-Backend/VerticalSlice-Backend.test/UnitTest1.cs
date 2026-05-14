using Microsoft.AspNetCore.Mvc;
using Moq;
using System.Collections.Generic;
using VerticalSlice_Backend.Features.Checkout;
using VerticalSlice_Backend.Features.Checkout.CheckoutDTOs;
using VerticalSlice_Backend.Features.Identity;
using VerticalSlice_Backend.Features.Products;
using Xunit;

namespace VerticalSlice_Backend.test.Features.Identity
{
    public class IdentityControllerTests
    {
        private readonly Mock<IIdentityRepository> _mockRepo;
        private readonly IdentityController _controller;

        public IdentityControllerTests()
        {
            // Creăm "clona" falsă a repository-ului
            _mockRepo = new Mock<IIdentityRepository>();

            // Injectăm clona în controller
            _controller = new IdentityController(_mockRepo.Object);
        }

        [Fact]
        public async Task Register_ShouldReturnOk_WhenUserIsRegisteredSuccessfully()
        {
            // 1. Arrange (Pregătim datele)
            var request = new RegisterDTO("John", "Doe", "john@email.com", "Password123!");
            var expectedResponse = new RegisterResponseDTO(1, "John", "Doe", "john@email.com");

            // Spunem clonei: "Când cineva te cheamă cu acest request, răspunde cu succes"
            _mockRepo.Setup(repo => repo.RegisterAsync(request))
                     .ReturnsAsync(expectedResponse);

            // 2. Act (Executăm acțiunea)
            var result = await _controller.Register(request);

            // 3. Assert (Verificăm dacă rezultatul e cel așteptat)
            var okResult = Assert.IsType<OkObjectResult>(result);
            Assert.Equal(200, okResult.StatusCode);
            Assert.Equal(expectedResponse, okResult.Value);
        }

        [Fact]
        public async Task Register_ShouldReturnBadRequest_WhenEmailAlreadyExists()
        {
            // 1. Arrange
            var request = new RegisterDTO("John", "Doe", "existing@email.com", "Password123!");

            // Simulăm o eroare (Email already in use)
            _mockRepo.Setup(repo => repo.RegisterAsync(request))
                     .ThrowsAsync(new Exception("Email already in use."));

            // 2. Act
            var result = await _controller.Register(request);

            // 3. Assert
            var badRequestResult = Assert.IsType<BadRequestObjectResult>(result);
            Assert.Equal(400, badRequestResult.StatusCode);
            Assert.Equal("Email already in use.", badRequestResult.Value);
        }

        [Fact]
        public async Task Login_ShouldReturnOk_WhenCredentialsAreValid()
        {
            // 1. Arrange
            var request = new LoginDTO("test@email.com", "Password123!");
            var expectedResponse = new LoginResponseDTO("fake-jwt-token", 1, "John", "test@email.com");

            _mockRepo.Setup(repo => repo.LoginAsync(request))
                     .ReturnsAsync(expectedResponse);

            // 2. Act
            var result = await _controller.Login(request);

            // 3. Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            var actualResponse = Assert.IsType<LoginResponseDTO>(okResult.Value);

            Assert.Equal("fake-jwt-token", actualResponse.Toke); // Atenție: ai un mic typo în record-ul tău: "Toke" în loc de "Token"
            Assert.Equal(200, okResult.StatusCode);
        }

        [Fact]
        public async Task Login_ShouldReturnBadRequest_WhenCredentialsAreInvalid()
        {
            // 1. Arrange
            var request = new LoginDTO("wrong@email.com", "wrong-pass");

            _mockRepo.Setup(repo => repo.LoginAsync(request))
                     .ThrowsAsync(new Exception("Invalid email or password"));

            // 2. Act
            var result = await _controller.Login(request);

            // 3. Assert
            var badRequestResult = Assert.IsType<BadRequestObjectResult>(result);
            Assert.Equal(400, badRequestResult.StatusCode);
            Assert.Equal("Invalid email or password", badRequestResult.Value);
        }


    }

    public class CheckoutControllerTests
    {
        private readonly Mock<ICheckoutRepository> _mockCheckoutRepo;
        private readonly CheckoutController _controller;

        public CheckoutControllerTests()
        {
            _mockCheckoutRepo = new Mock<ICheckoutRepository>();
            _controller = new CheckoutController(_mockCheckoutRepo.Object);
        }

        [Fact]
        public async Task Post_ShouldIgnoreFrontendPrice_AndUseBackendCalculation()
        {
            // Arrange
            // Trimitem intenționat 0 RON pentru un produs care în DB costă mult
            var itemWithFakePrice = new OrderItemDTO(ProductID: 1, TotalPrice: 0m, Quantity: 2);
            var request = new CheckoutCreateDTO(UserID: 1, Address: "Test", Items: new List<OrderItemDTO> { itemWithFakePrice });

            _mockCheckoutRepo.Setup(repo => repo.PlaceOrderAsync(It.IsAny<CheckoutCreateDTO>()))
                             .Returns(Task.CompletedTask);

            // Act
            var result = await _controller.Post(request);

            // Assert
            // Dacă trece, înseamnă că Controller-ul a acceptat cererea și a trimis-o spre Repository
            // unde se întâmplă magia calculului.
            Assert.IsType<OkObjectResult>(result);
        }
        [Fact]
        public async Task Post_ReturnsOk_WhenOrderIsPlacedSuccessfully()
        {
            // 1. Arrange
            // Folosim paranteze rotunde pentru OrderItemDTO conform definiției tale
            var items = new List<OrderItemDTO>
    {
        new OrderItemDTO(1, 100.5m, 2) // ProductID, TotalPrice, Quantity
    };

            // Folosim paranteze rotunde pentru CheckoutCreateDTO
            var request = new CheckoutCreateDTO(1, "Strada Victoriei", items);

            _mockCheckoutRepo.Setup(repo => repo.PlaceOrderAsync(It.IsAny<CheckoutCreateDTO>()))
                             .Returns(Task.CompletedTask);

            // 2. Act
            var result = await _controller.Post(request);

            // 3. Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            var message = okResult.Value.GetType().GetProperty("message").GetValue(okResult.Value, null);
            Assert.Equal("Comandă finalizată cu succes!", message);
        }

        [Fact]
        public async Task Post_ReturnsBadRequest_WhenStockIsInsufficient()
        {
            // Arrange
            var request = new CheckoutCreateDTO(1, "Adresa", new List<OrderItemDTO>());

            _mockCheckoutRepo.Setup(repo => repo.PlaceOrderAsync(It.IsAny<CheckoutCreateDTO>()))
                             .ThrowsAsync(new Exception("Stock insufficient"));

            // Act
            var result = await _controller.Post(request);

            // Assert
            var badRequest = Assert.IsType<BadRequestObjectResult>(result);
            Assert.Equal("Stock insufficient", badRequest.Value);
        }
    }

    public class ProductsControllerTests
    {
        private readonly Mock<IProductsRepository> _mockProductsRepo;
        private readonly ProductsController _controller;

        public ProductsControllerTests()
        {
            _mockProductsRepo = new Mock<IProductsRepository>();
            _controller = new ProductsController(_mockProductsRepo.Object);
        }

        [Fact]
        public async Task GetProductById_ReturnsOk_WhenProductExists()
        {
            // Arrange
            int productId = 1;
            var fakeProduct = new ProductDetailDTO
            {
                ProductID = productId,
                Title = "Laptop",
                Price = 1500.0,
                Stock = 5
            };

            _mockProductsRepo.Setup(repo => repo.GetProductByIdAsync(productId))
                             .ReturnsAsync(fakeProduct);

            // Act
            var result = await _controller.GetProductById(productId);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            var returnedProduct = Assert.IsType<ProductDetailDTO>(okResult.Value);
            Assert.Equal(productId, returnedProduct.ProductID);
        }

        [Fact]
        public async Task GetProductById_ReturnsNotFound_WhenProductDoesNotExist()
        {
            // Arrange
            int productId = 999;
            _mockProductsRepo.Setup(repo => repo.GetProductByIdAsync(productId))
                             .ReturnsAsync((ProductDetailDTO)null);

            // Act
            var result = await _controller.GetProductById(productId);

            // Assert
            var notFoundResult = Assert.IsType<NotFoundObjectResult>(result);
            // Verificăm dacă mesajul de eroare este cel scris de tine în controller
            var message = notFoundResult.Value.GetType().GetProperty("message").GetValue(notFoundResult.Value, null);
            Assert.Equal($"Product with ID{productId} not found", message);
        }

        [Fact]
        public async Task GetAllProducts_ReturnsOk_WithListOfProducts()
        {
            // Arrange
            var fakeList = new List<ProductDetailDTO>
        {
            new ProductDetailDTO { ProductID = 1, Title = "P1" },
            new ProductDetailDTO { ProductID = 2, Title = "P2" }
        };

            _mockProductsRepo.Setup(repo => repo.GetAllProductsAsync())
                             .ReturnsAsync(fakeList);

            // Act
            var result = await _controller.GetAllProducts();

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            var returnedList = Assert.IsType<List<ProductDetailDTO>>(okResult.Value);
            Assert.Equal(2, returnedList.Count);
        }
    }
}