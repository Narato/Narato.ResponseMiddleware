using Microsoft.AspNetCore.Mvc;
using System;

namespace Narato.ResponseMiddleware.IntegrationTest.Controllers
{
    [Route("exceptionHandler")]
    public class ExceptioHandlerFilterTestController
    {
        [HttpGet("noException")]
        public string TestNoException()
        {
            return "meep";
        }

        [HttpGet("exception")]
        public string TestException()
        {
            throw new Exception("nope");
        }
    }
}
