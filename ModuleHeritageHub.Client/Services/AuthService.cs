using Microsoft.JSInterop;
using System.Security.Claims;
using System.IdentityModel.Tokens.Jwt;

namespace ModuleHeritageHub.Client.Services
{
    public class AuthService(IJSRuntime jsRuntime)
    {

        public async Task<bool> IsAuthenticated()
        {
            var token = await jsRuntime.InvokeAsync<string>("localStorage.getItem", "authToken");
            return !string.IsNullOrEmpty(token);
        }

        public async Task<ClaimsPrincipal> GetUserClaims()
        {
            var token = await jsRuntime.InvokeAsync<string>("localStorage.getItem", "authToken");

            if (string.IsNullOrEmpty(token))
                return new ClaimsPrincipal(new ClaimsIdentity());

            var handler = new JwtSecurityTokenHandler();
            var jwtToken = handler.ReadJwtToken(token);
            var claims = jwtToken?.Claims;

            var identity = new ClaimsIdentity(claims, "Bearer");
            return new ClaimsPrincipal(identity);
        }

        public async Task SetToken(string token)
        {
            await jsRuntime.InvokeVoidAsync("localStorage.setItem", "authToken", token);
        }

        public async Task RemoveToken()
        {
            await jsRuntime.InvokeVoidAsync("localStorage.removeItem", "authToken");
        }

        public async Task<string?> GetToken()
        {
            return await jsRuntime.InvokeAsync<string>("localStorage.getItem", "authToken");
        }
    }
}
