using Narato.ResponseMiddleware.Mappers.Interfaces;
using System;
using Microsoft.AspNetCore.Mvc;
using Narato.ResponseMiddleware.Models.Exceptions.Interfaces;
using Narato.ResponseMiddleware.Models.Models;
using Narato.ResponseMiddleware.Models.Exceptions;
using Narato.ResponseMiddleware.Models.ActionResults;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Logging;

namespace Narato.ResponseMiddleware.Mappers
{
    public class ExceptionToActionResultMapper : IExceptionToActionResultMapper
    {
        private readonly IHostingEnvironment _hostingEnvironment;
        private readonly ILogger _logger;

        public ExceptionToActionResultMapper(IHostingEnvironment hostingEnvironment, ILogger<ExceptionToActionResultMapper> logger)
        {
            _hostingEnvironment = hostingEnvironment;
            _logger = logger;
        }

        public virtual IActionResult Map(Exception ex)
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

            _logger.LogTrace($"Exception of type {ex.GetType().Name} was mapped by the catch all mapper.");
            var message = "Something went wrong. Contact support and give them the identifier found below.";
            // if development ==> expose exception message
            if (_hostingEnvironment.IsDevelopment())
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
