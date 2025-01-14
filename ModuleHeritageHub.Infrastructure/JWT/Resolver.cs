using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using Microsoft.AspNetCore.Http;
using ModuleHeritageHub.Domain.Model;

namespace ModuleHeritageHub.Infrastructure.JWT
{
    public class JwtResolver(IHttpContextAccessor httpContextAccessor)
    {
        public Guid UserId
        {
            get
            {
                var userId = (httpContextAccessor.HttpContext?.User.FindFirstValue(JwtRegisteredClaimNames.Jti)) ?? throw new Exception("User is not authenticated");
                return Guid.Parse(userId);
            }
        }

        public string Login
        {
            get
            {
                var login = (httpContextAccessor.HttpContext?.User.FindFirstValue(JwtRegisteredClaimNames.Sub)) ?? throw new Exception("User is not authenticated");
                return login;
            }
        }

        public UserRole Role
        {
            get
            {
                var role = (httpContextAccessor.HttpContext?.User.FindFirstValue(ClaimTypes.Role)) ?? throw new Exception("User is not authenticated");
                return (UserRole)Enum.Parse(typeof(UserRole), role);
            }
        }
    }
}