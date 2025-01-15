using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ModuleHeritageHub.Domain.DTO;
using ModuleHeritageHub.Infrastructure.Repository;
using Npgsql;

namespace ModuleHeritageHub.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuthController(UserRepository userRepository) : ControllerBase
    {
        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] RegisterDTO data)
        {        
            AuthDTO result;
            try 
            {
                result = await userRepository.Register(data.Login, data.Password, data.Role, data.FirstName, data.LastName);
            } 
            catch (DbUpdateException ex) when (ex.InnerException is PostgresException psqlException
                                && psqlException.SqlState == PostgresErrorCodes.UniqueViolation)
            {
                return Problem("User with this login already registered", statusCode: StatusCodes.Status409Conflict);
            } 
            catch (System.Exception) 
            {
                return Problem();
            }


            return Created("/api/users/me", result);
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginDTO data) 
        {
            AuthDTO result;
            try
            {
                result = await userRepository.Login(data.Login, data.Password);
            } 
            catch (UnauthorizedAccessException) {
                return Forbid();
            }
            catch (System.Exception) 
            {
                return Problem();
            }


            return Ok(result);
        }
    }
}