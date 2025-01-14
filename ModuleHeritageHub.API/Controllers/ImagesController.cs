using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ModuleHeritageHub.Domain.DTO;
using ModuleHeritageHub.Infrastructure.JWT;
using ModuleHeritageHub.Infrastructure.Repository;

namespace ModuleHeritageHub.API.Controllers
{
    [ApiController]
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    [Route("api/[controller]")]
    public class ImagesController(ImageRepository imageRepository) : ControllerBase
    {
        [HttpPost("upload")]
        public async Task<IActionResult> Upload([FromForm] IFormFile file)
        {
            if (file == null) 
            {
                return Problem("File not provided", statusCode: StatusCodes.Status400BadRequest);
            } 
            else if (file.Length < 0 || file.Length > 16 * 1024 * 1024) 
            {
                return Problem("File length must be between 0 and 16MB", statusCode: StatusCodes.Status400BadRequest);
            } 
            else if (file.ContentType != "image/jpeg" && file.ContentType != "image/png") 
            {
                return Problem("File must be JPEG or PNG", statusCode: StatusCodes.Status400BadRequest);
            }
            
            ImageDTO image;
            try 
            {
                image = await imageRepository.Create(file.FileName);
            }
            catch(System.Exception ex)
            {
                return Problem(ex.Message);
            }

            var directory = Path.GetDirectoryName(image.Path);
            if (directory != null && !Directory.Exists(directory))
            {
                Directory.CreateDirectory(directory);
            }

            using (var fileStream = new FileStream(image.Path, FileMode.Create))
            {
                await file.CopyToAsync(fileStream);
            }

            return Created("/static/" + image.Path, image);
        }
    }
}