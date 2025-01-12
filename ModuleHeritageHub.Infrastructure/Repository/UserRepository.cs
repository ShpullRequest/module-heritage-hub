using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using ModuleHeritageHub.Domain.Config;
using ModuleHeritageHub.Domain.DTO;
using ModuleHeritageHub.Domain.Model;
using ModuleHeritageHub.Infrastructure.DB;

namespace ModuleHeritageHub.Infrastructure.Repository
{
    public class UserRepository(DBContext context, IOptions<JwtConfig> jwtConfig)
    {
        private readonly DBContext _context = context;
        private readonly JwtConfig _jwtConfig = jwtConfig.Value;

        public async Task<AuthDTO> LoginUser(string login, string password)
        {
            
            var user = await _context.Users.FirstOrDefaultAsync(u => u.Login == login) ?? throw new UnauthorizedAccessException("Invalid login or password.");
            
            var isPasswordValid = BCrypt.Net.BCrypt.Verify(password, user.Password);
            if (!isPasswordValid)
            {
                throw new UnauthorizedAccessException("Invalid login or password.");
            }

            return GenerateToken(user);
        }

        public async Task<AuthDTO> RegisterUser(string login, string password, UserRole role)
        {
            var hashedPassword = BCrypt.Net.BCrypt.HashPassword(password);

            var user = new User
            {
                Login = login,
                Password = hashedPassword,
                Role = role
            };

            _context.Users.Add(user);
            await _context.SaveChangesAsync();

            return GenerateToken(user);
        }


        protected AuthDTO GenerateToken(User user) 
        {
            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_jwtConfig.SecretKey));
            var credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var claims = new[]
            {
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                new Claim(JwtRegisteredClaimNames.Sub, user.Login),
                new Claim(ClaimTypes.Role, user.Role.ToString())
            };

            var tokenLifetime = TimeSpan.Parse(_jwtConfig.TokenLifetime);
            var token = new JwtSecurityToken(
                issuer: _jwtConfig.Issuer,
                audience: _jwtConfig.Audience,
                claims: claims,
                expires: DateTime.UtcNow.Add(tokenLifetime),
                signingCredentials: credentials
            );

            var tokenHandler = new JwtSecurityTokenHandler();
            var jwt = tokenHandler.WriteToken(token);

            return new AuthDTO
            {
                Token = jwt,
            };
        }
    }
}