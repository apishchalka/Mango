using Mango.Web.Configuration;
using Mango.Web.Service.IService;
using Newtonsoft.Json.Linq;

namespace Mango.Web.Service
{
    public class TokenProvider : ITokenProvider
    {
        private readonly IHttpContextAccessor contextAccessor;

        public TokenProvider(IHttpContextAccessor contextAccessor)
        {
            this.contextAccessor = contextAccessor;
        }
        
        public void ClearToken()
        {
            contextAccessor.HttpContext.Response.Cookies.Delete(MangoConfig.TokenName);
        }

        public string? GetToken()
        {
            var hasToken = contextAccessor.HttpContext.Request.Cookies.TryGetValue(MangoConfig.TokenName, out var tokenValue);

            return hasToken ? tokenValue : null;
        }

        public void SetToken(string token)
        {
            contextAccessor.HttpContext.Response.Cookies.Append(MangoConfig.TokenName, token);
        }
    }
}
