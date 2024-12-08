using Microsoft.AspNetCore.Mvc;

namespace ModuleHeritageHub.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuthController : ControllerBase
    {
        public AuthController() { }

        [HttpPost("register")]
        public async Task<IActionResult> Register()
        {
            await Task.Delay(1000);
            return Created("/api/users/1", null);
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login() 
        {
            await Task.Delay(1000);
            return Ok();
        }
    }
}