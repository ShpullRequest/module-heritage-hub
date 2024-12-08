using Microsoft.AspNetCore.Mvc;

namespace ModuleHeritageHub.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ImagesController : ControllerBase
    {
        public ImagesController() { }
        [HttpGet("{id}")]
        public async Task<IActionResult> Get(Guid id)
        {
            await Task.Delay(1000);
            return Ok();
        }

        [HttpPost("upload")]
        public async Task<IActionResult> Upload()
        {
            await Task.Delay(1000);
            return Created("/api/images/test", null);
        }
    }
}