using Microsoft.AspNetCore.Mvc;

namespace ModuleHeritageHub.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class PagesController : ControllerBase 
    {
        public PagesController() { }

        [HttpGet]
        public async Task<IActionResult> Get()
        {
            await Task.Delay(1000);
            return Ok();
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(Guid id)
        {
            await Task.Delay(1000);
            return Ok();
        }

        [HttpGet("{id}/versions")]
        public async Task<IActionResult> GetVersions(Guid id)
        {
            await Task.Delay(1000);
            return Ok();
        }

        [HttpPost]
        public async Task<IActionResult> Create()
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

        [HttpPost("{id}/versions")]
        public async Task<IActionResult> CreateVersion(Guid id)
        {
            await Task.Delay(1000);
            return Ok();
        }

        [HttpPut("{id}/versions/{versionId}")]
        public async Task<IActionResult> UpdateVersion(Guid id, Guid versionId)
        {
            await Task.Delay(1000);
            return Ok();
        }

        [HttpDelete("{id}/versions/{versionId}")] 
        public async Task<IActionResult> DeleteVersion(Guid id, Guid versionId)
        {
            await Task.Delay(1000);
            return Ok();
        }
    }
}