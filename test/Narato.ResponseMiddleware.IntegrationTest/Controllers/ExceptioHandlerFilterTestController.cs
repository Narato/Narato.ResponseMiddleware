using Microsoft.AspNetCore.Mvc;
using Narato.ResponseMiddleware.IntegrationTest.Mappers.TestClasses;
using Narato.ResponseMiddleware.Models.Exceptions;
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

        [HttpGet("conflict")]
        public string TestConflict()
        {
            throw new TestConflictException("noperope");
        }

        [HttpGet("simpleAggregate")]
        public string SimpleAggregate()
        {
            throw new AggregateException(new EntityNotFoundException("ENF", "meep"));
        }

        [HttpGet("advancedAggregate")]
        public string AdvancedAggregate()
        {
            throw new AggregateException(new EntityNotFoundException("ENF", "meep"), new Exception("moop"));
        }
    }
}
