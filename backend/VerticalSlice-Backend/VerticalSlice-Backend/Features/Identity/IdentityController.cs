using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace VerticalSlice_Backend.Features.Identity
{
    [Route("api/[controller]")]
    [ApiController]
    public class IdentityController : ControllerBase
    {
        private readonly IIdentityRepository _identityRepository;

        public IdentityController(IIdentityRepository identityRepository)
        {
            _identityRepository = identityRepository;
        }

        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] RegisterDTO request)
        {
            try
            {
                var result = await _identityRepository.RegisterAsync(request);
                return Ok(result);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginDTO request)
        {
            try
            {
                var result = await _identityRepository.LoginAsync(request);
                return Ok(result);
            } catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

    }
}
