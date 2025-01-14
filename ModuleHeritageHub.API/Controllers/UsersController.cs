using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ModuleHeritageHub.Domain.DTO;
using ModuleHeritageHub.Domain.Model;
using ModuleHeritageHub.Infrastructure.JWT;
using ModuleHeritageHub.Infrastructure.Repository;
using MyApp.Exceptions;
using Npgsql;

namespace ModuleHeritageHub.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    public class UsersController(UserRepository userRepository, JwtResolver jwtResolver) : ControllerBase
    {
        [HttpGet("me")]
        public async Task<IActionResult> GetMe()
        {
            UserDTO usr;
            try 
            {
                usr = await userRepository.GetById(jwtResolver.UserId);
            }
            catch(System.Exception ex)
            {
                return Problem(ex.Message);
            }
            

            return Ok(usr);
        }

        [HttpPatch("me")]
        public async Task<IActionResult> UpdateMe([FromBody] UserUpdateDTO data)
        {
            UserDTO usr;
            try
            {
                usr = await userRepository.Update(jwtResolver.UserId, data);
            } 
            catch (DbUpdateException ex) when (ex.InnerException is PostgresException psqlException
                                && psqlException.SqlState == PostgresErrorCodes.ForeignKeyViolation)
            {
                return Problem("Image not exists", statusCode: StatusCodes.Status409Conflict);
            }
            catch(System.Exception ex) 
            {
                return Problem(ex.Message);
            }

            return Ok(usr);
        }

        [HttpGet("list")]
        [Authorize(Roles = nameof(UserRole.ADMIN))]
        public async Task<IActionResult> GetList()
        {
            UserDTO[] users;
            try 
            {
                users = await userRepository.GetList();
            }
            catch(System.Exception)
            {
                return Problem();
            }

            return Ok(users);
        }

        [HttpPatch("{id}")]
        [Authorize(Roles = nameof(UserRole.ADMIN))]

        public async Task<IActionResult> Update(Guid id, [FromBody] UserUpdateDTO data)
        {
            UserDTO usr;
            try
            {
                usr = await userRepository.Update(id, data);
            } 
            catch (DbUpdateException ex) when (ex.InnerException is PostgresException psqlException
                                && psqlException.SqlState == PostgresErrorCodes.ForeignKeyViolation)
            {
                return Problem("Image not exists", statusCode: StatusCodes.Status409Conflict);
            }
             catch (NotFoundException ex)
            {
                return Problem(ex.Message, statusCode: StatusCodes.Status404NotFound);
            }
            catch(System.Exception ex) 
            {
                return Problem(ex.Message);
            }

            return Ok(usr);
        }

        [HttpDelete("{id}")]
        [Authorize(Roles = nameof(UserRole.ADMIN))]

        public async Task<IActionResult> Delete(Guid id)
        {
            if (id == jwtResolver.UserId) 
            {
                return Problem("You can't delete yourself", statusCode: StatusCodes.Status400BadRequest);
            }

            try 
            {
                await userRepository.Delete(id);
            }
            catch (NotFoundException ex)
            {
                return Problem(ex.Message, statusCode: StatusCodes.Status404NotFound);
            }
            catch(System.Exception ex) 
            {
                return Problem(ex.Message);
            }
            return Ok();
        }
    }
}