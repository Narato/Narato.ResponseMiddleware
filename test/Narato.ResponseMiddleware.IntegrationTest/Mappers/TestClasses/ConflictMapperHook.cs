using Narato.ResponseMiddleware.Mappers.Interfaces;
using System;
using Microsoft.AspNetCore.Mvc;
using Narato.ResponseMiddleware.Models.Models;
using Microsoft.AspNetCore.Http;

namespace Narato.ResponseMiddleware.IntegrationTest.Mappers.TestClasses
{
    public class ConflictMapperHook : IExceptionToActionResultMapperHook
    {
        public IActionResult Map(Exception ex)
        {
            if (ex is TestConflictException)
            {
                var errorContent = new ErrorContent
                {
                    Code = "testcode",
                    Message = ex.Message
                };
                var conflictResult = new ObjectResult(errorContent);
                conflictResult.StatusCode = StatusCodes.Status409Conflict;
                return conflictResult;
            }
            return null;
        }
    }
}
