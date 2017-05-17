using Microsoft.AspNetCore.Http;

namespace POC.Shizzle
{
    public class Meep : IMeep
    {
        private readonly IHttpContextAccessor _httpContextAccessor;

        public Meep(IHttpContextAccessor httpContextAccessor)
        {
            _httpContextAccessor = httpContextAccessor;
        }

        public string GetPath()
        {
            return _httpContextAccessor.HttpContext.Request.Path;
        }
    }
}
