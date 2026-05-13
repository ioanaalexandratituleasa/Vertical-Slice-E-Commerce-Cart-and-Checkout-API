using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using VerticalSlice_Backend.Features.Products;

namespace VerticalSlice_Backend.Features.Checkout
{
    [Route("api/[controller]")]
    [ApiController]
    public class Checkout : ControllerBase
    {
        private readonly CheckoutRepository _checkoutRepository;

        public Checkout(CheckoutRepository checkoutRepository)
        {
            _checkoutRepository = checkoutRepository;
        }


        [HttpPost]
        public IActionResult Post([FromBody] CheckoutDTO request)
        {
            try
            {
                _checkoutRepository.PlaceOrderAsync(request);
                return Ok(new { message = "Comandă finalizată cu succes!" });
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
    }
}
