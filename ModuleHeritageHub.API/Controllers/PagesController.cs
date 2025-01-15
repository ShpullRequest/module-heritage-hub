using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ModuleHeritageHub.Domain.DTO;
using ModuleHeritageHub.Domain.Exceptions;
using ModuleHeritageHub.Domain.Model;
using ModuleHeritageHub.Infrastructure.JWT;
using ModuleHeritageHub.Infrastructure.Repository;
using Npgsql;

namespace ModuleHeritageHub.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class PagesController(PageRepository pageRepository, JwtResolver jwtResolver) : ControllerBase 
    {
        [HttpGet]
        [ProducesResponseType(typeof(PageDTO[]), 200)]
        public async Task<IActionResult> Get()
        {
            PageDTO[] pages;
            try 
            {
                pages = await pageRepository.GetList();
            }
            catch (System.Exception ex)
            {
                return Problem(ex.Message);
            }
            
            return Ok(pages);
        }

        [HttpGet("{id}")]
        [ProducesResponseType(typeof(PageDTO), 200)]
        public async Task<IActionResult> GetById(Guid id)
        {
            PageDTO page;
            try 
            {
                page = await pageRepository.GetById(id);
            }
            catch (NotFoundException ex)
            {
                return Problem(ex.Message, statusCode: StatusCodes.Status404NotFound);
            }
            catch (System.Exception ex)
            {
                return Problem(ex.Message);
            }

            return Ok(page);
        }

        [HttpGet("{id}/versions")]
        [ProducesResponseType(typeof(PageVersionDTO[]), 200)]
        public async Task<IActionResult> GetVersions(Guid id)
        {
            PageVersionDTO[] pageVersions;
            try 
            {
                pageVersions = await pageRepository.GetVersions(id);
            }
            catch (NotFoundException ex)
            {
                return Problem(ex.Message, statusCode: StatusCodes.Status404NotFound);
            }
            catch (System.Exception ex)
            {
                return Problem(ex.Message);
            }

            return Ok(pageVersions);
        }

        [HttpPost]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Roles = nameof(UserRole.ADMIN))]
        [ProducesResponseType(typeof(PageDTO), 200)]
        public async Task<IActionResult> Create([FromBody] PageCreateDTO data)
        {
            PageDTO page;
            try
            {
                page = await pageRepository.Create(data, jwtResolver.UserId);
            }
            catch (DbUpdateException ex) when (ex.InnerException is PostgresException psqlException
                                && psqlException.SqlState == PostgresErrorCodes.ForeignKeyViolation)
            {
                return Problem("No related data exists", statusCode: StatusCodes.Status409Conflict);
            }
            catch (System.Exception ex)
            {
                return Problem(ex.Message);
            }


            return Created("/api/pages/" + page.Id.ToString(), page);
        }

        [HttpPatch("{id}")]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Roles = nameof(UserRole.ADMIN))]
        [ProducesResponseType(typeof(PageDTO), 200)]
        public async Task<IActionResult> Update(Guid id, [FromBody] PageEditDTO data)
        {
            PageDTO page;
            try 
            {
                page = await pageRepository.Update(id, data);
            }
            catch (NotFoundException ex)
            {
                return Problem(ex.Message, statusCode: StatusCodes.Status404NotFound);
            }
            catch (InvalidOperationException ex)
            {
                return Problem(ex.Message, statusCode: StatusCodes.Status409Conflict);
            }
            catch(System.Exception ex) 
            {
                return Problem(ex.Message);
            }
            return Ok(page);
        }

        [HttpDelete("{id}")]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Roles = nameof(UserRole.ADMIN))]
        public async Task<IActionResult> Delete(Guid id)
        {
            try 
            {
                await pageRepository.Delete(id);
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

        [HttpPost("{id}/versions")]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Roles = nameof(UserRole.ADMIN))]
        [ProducesResponseType(typeof(PageDTO), 200)]
        public async Task<IActionResult> CreateVersion(Guid id, [FromBody] PageVersionCreateEditDTO data)
        {
            PageDTO page;
            try
            {
                page = await pageRepository.CreateVersion(id, data, jwtResolver.UserId);
            }
            catch (NotFoundException ex)
            {
                return Problem(ex.Message, statusCode: StatusCodes.Status404NotFound);
            }
            catch (DbUpdateException ex) when (ex.InnerException is PostgresException psqlException
                                && psqlException.SqlState == PostgresErrorCodes.ForeignKeyViolation)
            {
                return Problem("No related data exists", statusCode: StatusCodes.Status409Conflict);
            }
            catch (System.Exception ex)
            {
                return Problem(ex.Message);
            }


            return Created("/api/pages/" + page.Id.ToString() + "/versions", page);
        }

        [HttpPatch("{id}/versions/{versionId}")]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Roles = nameof(UserRole.ADMIN))]
        [ProducesResponseType(typeof(PageVersionDTO), 200)]
        public async Task<IActionResult> UpdateVersion(Guid id, Guid versionId, [FromBody] PageVersionCreateEditDTO data)
        {
            PageVersionDTO pageVersion;
            try
            {
                pageVersion = await pageRepository.UpdateVersion(id, versionId, data, jwtResolver.UserId);
            }
            catch (NotFoundException ex)
            {
                return Problem(ex.Message, statusCode: StatusCodes.Status404NotFound);
            }
            catch (DbUpdateException ex) when (ex.InnerException is PostgresException psqlException
                                && psqlException.SqlState == PostgresErrorCodes.ForeignKeyViolation)
            {
                return Problem("No related data exists", statusCode: StatusCodes.Status409Conflict);
            }
            catch (InvalidOperationException ex)
            {
                return Problem(ex.Message, statusCode: StatusCodes.Status409Conflict);
            }
            catch (System.Exception ex)
            {
                return Problem(ex.Message);
            }


            return Ok(pageVersion);
        }

        [HttpDelete("{id}/versions/{versionId}")] 
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Roles = nameof(UserRole.ADMIN))]
        [ProducesResponseType(typeof(PageDTO), 200)]
        public async Task<IActionResult> DeleteVersion(Guid id, Guid versionId)
        {
            PageDTO page;
            try 
            {
                page = await pageRepository.DeleteVersion(id, versionId);
            }
            catch (NotFoundException ex)
            {
                return Problem(ex.Message, statusCode: StatusCodes.Status404NotFound);
            }
            catch (InvalidOperationException ex)
            {
                return Problem(ex.Message, statusCode: StatusCodes.Status409Conflict);
            }
            catch(System.Exception ex) 
            {
                return Problem(ex.Message);
            }
            return Ok(page);
        }
    }
}