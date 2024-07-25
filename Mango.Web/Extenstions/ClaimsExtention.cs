using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;

namespace Mango.Web.Extenstions
{
    public static class ClaimsExtention
    {
        public static string? GetId(this ClaimsPrincipal user)
        {
            return user.Claims.SingleOrDefault(c => c.Type == JwtRegisteredClaimNames.Sub)?.Value;
        }
    }
}
