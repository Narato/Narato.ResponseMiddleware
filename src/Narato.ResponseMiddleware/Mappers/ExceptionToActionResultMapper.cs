using Narato.ResponseMiddleware.Mappers.Interfaces;
using System;
using Microsoft.AspNetCore.Mvc;
using Narato.ResponseMiddleware.Models.Exceptions.Interfaces;
using Narato.ResponseMiddleware.Models.Models;
using Narato.ResponseMiddleware.Models.Exceptions;
using Narato.ResponseMiddleware.Models.ActionResults;
using Microsoft.AspNetCore.Hosting;

namespace Narato.ResponseMiddleware.Mappers
{
    public class ExceptionToActionResultMapper : IExceptionToActionResultMapper
    {
        private readonly IHostingEnvironment _hostingEnvironment;

        public ExceptionToActionResultMapper(IHostingEnvironment hostingEnvironment)
        {
            _hostingEnvironment = hostingEnvironment;
        }

        public IActionResult Map(Exception ex)
        {
            if (ex is IValidationException<object>)
            {
                var typedEx = ex as IValidationException<object>;
                var errorContent = new ValidationErrorContent<object>() { ValidationMessages = typedEx.ValidationMessages };
                return new BadRequestObjectResult(errorContent);
            }

            if (ex is EntityNotFoundException)
            {
                var typedEx = ex as EntityNotFoundException;
                if (string.IsNullOrEmpty(typedEx.Message))
                {
                    return new NotFoundResult();
                }

                var errorContent = new ErrorContent
                {
                    Code = typedEx.ErrorCode,
                    Message = typedEx.Message
                };
                return new NotFoundObjectResult(errorContent);
            }

            if (ex is UnauthorizedException)
            {
                return new UnauthorizedResult();
            }

            if (ex is ForbiddenException)
            {
                return new ForbidResult();
            }

            if (ex is ExceptionWithFeedback)
            {
                var typedEx = ex as ExceptionWithFeedback;
                var errorContent = new ErrorContent
                {
                    Code = typedEx.ErrorCode,
                    Message = typedEx.Message
                };
                return new InternalServerObjectResult(errorContent);
            }

            var message = "Something went wrong. Contact support and give them the identifier found below.";
            // if development ==> expose exception message
            if (_hostingEnvironment.IsDevelopment()/*Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT") != null && Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT").ToLower().Equals("development")*/)
            {
                message = ex.Message;
            }
            // catch all (just Exception)
            var catchAllErrorContent = new ErrorContent
            {
                Code = "",
                Message = message
            };
            return new InternalServerObjectResult(catchAllErrorContent);
        }
    }
}
