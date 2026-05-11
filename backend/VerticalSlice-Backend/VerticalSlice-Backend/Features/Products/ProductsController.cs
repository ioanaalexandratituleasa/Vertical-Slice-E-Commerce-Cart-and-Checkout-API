using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace VerticalSlice_Backend.Features.Products
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProductsController : ControllerBase
    {
        private readonly ProductsRepository _productsRepository;

        public ProductsController(ProductsRepository productsRepository)
        {
            _productsRepository = productsRepository;
        }


        [HttpGet("{id:int}")]
        public async Task<IActionResult> GetProductById(int id)
        {
            var product = await _productsRepository.GetProductByIdAsync(id);

            if (product is null)
                return NotFound(new { message = $"Product with ID{id} not found" });
            return Ok(product);
        }

        [HttpGet]

        public async Task<IActionResult> GetAllProducts()
        {
            var products = await _productsRepository.GetAllProductsAsync();
            return Ok(products);
        }
    }
}
