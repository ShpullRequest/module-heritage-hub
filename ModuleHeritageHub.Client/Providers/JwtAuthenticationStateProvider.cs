using System.Security.Claims;
using System.IdentityModel.Tokens.Jwt;
using ModuleHeritageHub.Client.Services;
using Microsoft.AspNetCore.Components.Authorization;

namespace ModuleHeritageHub.Client.Providers
{
    public class JwtAuthStateProvider(AuthService authService) : AuthenticationStateProvider
    {
        public override async Task<AuthenticationState> GetAuthenticationStateAsync()
        {
            var userClaims = await authService.GetUserClaims();
            return new AuthenticationState(userClaims);
        }

        public void MarkUserAsAuthenticated(string token)
        {
            var identity = new ClaimsIdentity(ParseClaimsFromJwt(token), "Bearer");
            var user = new ClaimsPrincipal(identity);
            NotifyAuthenticationStateChanged(Task.FromResult(new AuthenticationState(user)));
        }

        public void MarkUserAsLoggedOut()
        {
            var user = new ClaimsPrincipal(new ClaimsIdentity());
            NotifyAuthenticationStateChanged(Task.FromResult(new AuthenticationState(user)));
        }


        private IEnumerable<Claim> ParseClaimsFromJwt(string jwt)
        {
            var claims = new List<Claim>();
            var jwtHandler = new JwtSecurityTokenHandler();
            var jsonToken = jwtHandler.ReadJwtToken(jwt);

            claims.AddRange(jsonToken.Claims);
            return claims;
        }
    }
}