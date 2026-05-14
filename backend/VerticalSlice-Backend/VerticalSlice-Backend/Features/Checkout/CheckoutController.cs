using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using VerticalSlice_Backend.Features.Checkout.CheckoutDTOs;
using VerticalSlice_Backend.Features.Products;

namespace VerticalSlice_Backend.Features.Checkout.CheckoutDTOs
{
    [Route("api/[controller]")]
    [ApiController]
    public class CheckoutController : ControllerBase
    {
        private readonly ICheckoutRepository _checkoutRepository;

        public CheckoutController(ICheckoutRepository checkoutRepository)
        {
            _checkoutRepository = checkoutRepository;
        }


        [HttpPost]
        public async Task<IActionResult> Post([FromBody] CheckoutCreateDTO request)
        {
            try
            {
               await _checkoutRepository.PlaceOrderAsync(request);
                return Ok(new { message = "Comandă finalizată cu succes!" });
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
    }
}
