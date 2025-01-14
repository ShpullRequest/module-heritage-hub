using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using ModuleHeritageHub.Domain.Config;
using ModuleHeritageHub.Domain.DTO;
using ModuleHeritageHub.Domain.Model;

namespace ModuleHeritageHub.Infrastructure.JWT
{
    public class Jwt {
        private JwtConfig cfg;
        private SecurityKey key;

        public Jwt(IOptions<JwtConfig> cfg) 
        {
            this.cfg = cfg.Value;
            this.key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(this.cfg.SecretKey));
        }

        public AuthDTO Generate(User user) 
        {
            var credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var claims = new[]
            {
                new Claim(JwtRegisteredClaimNames.Jti, user.Id.ToString()),
                new Claim(JwtRegisteredClaimNames.Sub, user.Login),
                new Claim(ClaimTypes.Role, user.Role.ToString())
            };

            var tokenLifetime = TimeSpan.Parse(cfg.TokenLifetime);
            var token = new JwtSecurityToken(
                issuer: cfg.Issuer,
                audience: cfg.Audience,
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