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
            System.Threading.Thread.Sleep(10);
            return "meep";
        }

        [HttpGet("exception")]
        public string TestException()
        {
            throw new Exception("nope");
        }
    }
}
