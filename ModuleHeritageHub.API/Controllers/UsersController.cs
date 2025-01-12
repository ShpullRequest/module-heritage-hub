using Microsoft.AspNetCore.Mvc;

namespace ModuleHeritageHub.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class UsersController : ControllerBase
    {
        public UsersController() { }

        [HttpGet("me")]
        public async Task<IActionResult> GetMe()
        {
            await Task.Delay(1000);
            return Ok();
        }

        [HttpPut("me")]
        public async Task<IActionResult> UpdateMe()
        {
            await Task.Delay(1000);
            return Ok();
        }

        [HttpGet("list")]
        public async Task<IActionResult> GetList()
        {
            await Task.Delay(1000);
            return Ok();
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Update(Guid id)
        {
            await Task.Delay(1000);
            return Ok();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(Guid id)
        {
            await Task.Delay(1000);
            return Ok();
        }
    }
}