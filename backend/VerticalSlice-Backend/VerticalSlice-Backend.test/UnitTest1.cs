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
            _mockRepo = new Mock<IIdentityRepository>();

            _controller = new IdentityController(_mockRepo.Object);
        }

        [Fact]
        public async Task Register_ShouldReturnOk_WhenUserIsRegisteredSuccessfully()
        {
            var request = new RegisterDTO("John", "Doe", "john@email.com", "Password123!");
            var expectedResponse = new RegisterResponseDTO(1, "John", "Doe", "john@email.com");

            _mockRepo.Setup(repo => repo.RegisterAsync(request))
                     .ReturnsAsync(expectedResponse);

            var result = await _controller.Register(request);

            var okResult = Assert.IsType<OkObjectResult>(result);
            Assert.Equal(200, okResult.StatusCode);
            Assert.Equal(expectedResponse, okResult.Value);
        }

        [Fact]
        public async Task Register_ShouldReturnBadRequest_WhenEmailAlreadyExists()
        {
            var request = new RegisterDTO("John", "Doe", "existing@email.com", "Password123!");

            _mockRepo.Setup(repo => repo.RegisterAsync(request))
                     .ThrowsAsync(new Exception("Email already in use."));

            var result = await _controller.Register(request);

            var badRequestResult = Assert.IsType<BadRequestObjectResult>(result);
            Assert.Equal(400, badRequestResult.StatusCode);
            Assert.Equal("Email already in use.", badRequestResult.Value);
        }

        [Fact]
        public async Task Login_ShouldReturnOk_WhenCredentialsAreValid()
        {
            var request = new LoginDTO("test@email.com", "Password123!");
            var expectedResponse = new LoginResponseDTO("fake-jwt-token", 1, "John", "test@email.com");

            _mockRepo.Setup(repo => repo.LoginAsync(request))
                     .ReturnsAsync(expectedResponse);

            var result = await _controller.Login(request);

            var okResult = Assert.IsType<OkObjectResult>(result);
            var actualResponse = Assert.IsType<LoginResponseDTO>(okResult.Value);

            Assert.Equal("fake-jwt-token", actualResponse.Toke);
            Assert.Equal(200, okResult.StatusCode);
        }

        [Fact]
        public async Task Login_ShouldReturnBadRequest_WhenCredentialsAreInvalid()
        {
            
            var request = new LoginDTO("wrong@email.com", "wrong-pass");

            _mockRepo.Setup(repo => repo.LoginAsync(request))
                     .ThrowsAsync(new Exception("Invalid email or password"));

            var result = await _controller.Login(request);

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
            var itemWithFakePrice = new OrderItemDTO(ProductID: 1, TotalPrice: 0m, Quantity: 2);
            var request = new CheckoutCreateDTO(UserID: 1, Address: "Test", Items: new List<OrderItemDTO> { itemWithFakePrice });

            _mockCheckoutRepo.Setup(repo => repo.PlaceOrderAsync(It.IsAny<CheckoutCreateDTO>()))
                             .Returns(Task.CompletedTask);

            var result = await _controller.Post(request);
            Assert.IsType<OkObjectResult>(result);
        }
        [Fact]
        public async Task Post_ReturnsOk_WhenOrderIsPlacedSuccessfully()
        {
            var items = new List<OrderItemDTO>
    {
        new OrderItemDTO(1, 100.5m, 2) 
    };
            var request = new CheckoutCreateDTO(1, "Strada Victoriei", items);

            _mockCheckoutRepo.Setup(repo => repo.PlaceOrderAsync(It.IsAny<CheckoutCreateDTO>()))
                             .Returns(Task.CompletedTask);

            var result = await _controller.Post(request);

            var okResult = Assert.IsType<OkObjectResult>(result);
            var message = okResult.Value.GetType().GetProperty("message").GetValue(okResult.Value, null);
            Assert.Equal("Comandă finalizată cu succes!", message);
        }

        [Fact]
        public async Task Post_ReturnsBadRequest_WhenStockIsInsufficient()
        {
            var request = new CheckoutCreateDTO(1, "Adresa", new List<OrderItemDTO>());

            _mockCheckoutRepo.Setup(repo => repo.PlaceOrderAsync(It.IsAny<CheckoutCreateDTO>()))
                             .ThrowsAsync(new Exception("Stock insufficient"));

            var result = await _controller.Post(request);

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

            var result = await _controller.GetProductById(productId);

            var okResult = Assert.IsType<OkObjectResult>(result);
            var returnedProduct = Assert.IsType<ProductDetailDTO>(okResult.Value);
            Assert.Equal(productId, returnedProduct.ProductID);
        }

        [Fact]
        public async Task GetProductById_ReturnsNotFound_WhenProductDoesNotExist()
        {
            int productId = 999;
            _mockProductsRepo.Setup(repo => repo.GetProductByIdAsync(productId))
                             .ReturnsAsync((ProductDetailDTO)null);

            var result = await _controller.GetProductById(productId);

            var notFoundResult = Assert.IsType<NotFoundObjectResult>(result);
            var message = notFoundResult.Value.GetType().GetProperty("message").GetValue(notFoundResult.Value, null);
            Assert.Equal($"Product with ID{productId} not found", message);
        }

        [Fact]
        public async Task GetAllProducts_ReturnsOk_WithListOfProducts()
        {
            var fakeList = new List<ProductDetailDTO>
        {
            new ProductDetailDTO { ProductID = 1, Title = "P1" },
            new ProductDetailDTO { ProductID = 2, Title = "P2" }
        };

            _mockProductsRepo.Setup(repo => repo.GetAllProductsAsync())
                             .ReturnsAsync(fakeList);

            var result = await _controller.GetAllProducts();

            var okResult = Assert.IsType<OkObjectResult>(result);
            var returnedList = Assert.IsType<List<ProductDetailDTO>>(okResult.Value);
            Assert.Equal(2, returnedList.Count);
        }
    }
}