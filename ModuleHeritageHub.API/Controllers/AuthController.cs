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
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            
            AuthDTO result;
            try 
            {
                result = await userRepository.RegisterUser(data.Login, data.Password, data.Role);
            } 
            catch (DbUpdateException ex) when (ex.InnerException is PostgresException psqlException
                                && psqlException.SqlState == PostgresErrorCodes.UniqueViolation)
            {
                return Conflict();
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
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            AuthDTO result;
            try
            {
                result = await userRepository.LoginUser(data.Login, data.Password);
            } 
            catch (UnauthorizedAccessException) {
                return Unauthorized();
            }
            catch (System.Exception) 
            {
                return Problem();
            }


            return Ok(result);
        }
    }
}