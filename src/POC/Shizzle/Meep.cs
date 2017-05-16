using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

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
