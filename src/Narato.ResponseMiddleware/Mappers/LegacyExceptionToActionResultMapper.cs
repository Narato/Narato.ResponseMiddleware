using Narato.ResponseMiddleware.Mappers.Interfaces;
using System;
using Microsoft.AspNetCore.Mvc;
using Narato.ResponseMiddleware.Models.Exceptions;
using Narato.ResponseMiddleware.Models.Legacy.Extensions;
using Microsoft.AspNetCore.Http;
using Narato.Correlations.Correlations.Interfaces;
using Narato.ResponseMiddleware.Models.Legacy.Models;
using Narato.ResponseMiddleware.Models.Exceptions.Interfaces;
using System.Linq;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Logging;
using System.Collections.Generic;
using Narato.ResponseMiddleware.Models.Mvc.ActionResults;

namespace Narato.ResponseMiddleware.Mappers
{
    public class LegacyExceptionToActionResultMapper : IExceptionToActionResultMapper
    {
        private readonly IEnumerable<IExceptionToActionResultMapperHook> _mapperHooks;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly ICorrelationIdProvider _correlationIdProvider;
        private readonly IHostingEnvironment _hostingEnvironment;
        private readonly ILogger _logger;

        public LegacyExceptionToActionResultMapper(IEnumerable<IExceptionToActionResultMapperHook> mapperHooks, IHttpContextAccessor httpContextAccessor, ICorrelationIdProvider correlationIdProvider, IHostingEnvironment hostingEnvironment, ILogger<LegacyExceptionToActionResultMapper> logger)
        {
            _mapperHooks = mapperHooks;
            _httpContextAccessor = httpContextAccessor;
            _correlationIdProvider = correlationIdProvider;
            _hostingEnvironment = hostingEnvironment;
            _logger = logger;
        }

        // Override this method if you use anything other than IValidationException<string>
        // first check if it is your IValidationException, and then call base.Map(ex)
        public virtual IActionResult Map(Exception ex)
        {
            foreach (var hook in _mapperHooks)
            {
                var result = hook.Map(ex);
                if (result != null)
                    return result;
            }

            var absolutePath = _httpContextAccessor.HttpContext.Request.Path;
            if (ex is IValidationException<string>)
            {
                var typedEx = ex as IValidationException<string>;
                var response = new ErrorResponse(typedEx.ValidationMessages.ToFeedbackItems().ToList(), absolutePath, 400);
                response.Identifier = _correlationIdProvider.GetCorrelationId();
                response.Title = "Validation failed.";
                return new BadRequestObjectResult(response);
            }

            if (ex is EntityNotFoundException)
            {
                var typedEx = ex as EntityNotFoundException;
                if (string.IsNullOrEmpty(typedEx.Message))
                {
                    return new NotFoundResult();
                }

                var response = new ErrorResponse(new FeedbackItem { Description = typedEx.Message, Type = FeedbackType.Error }, absolutePath, 404);
                response.Identifier = _correlationIdProvider.GetCorrelationId();
                response.Title = "Entity could not be found.";
                return new NotFoundObjectResult(response);
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
                var response = new ErrorResponse(typedEx.Message.ToFeedbackItem(FeedbackType.Error), absolutePath, 500);
                response.Identifier = _correlationIdProvider.GetCorrelationId();
                response.Title = "Unexpected internal server error.";
                return new InternalServerObjectResult(response);
            }

            _logger.LogTrace($"Exception of type {ex.GetType().Name} was mapped by the catch all mapper.");
            var message = "Something went wrong. Contact support and give them the identifier found below.";
            // if development ==> expose exception message
            if (_hostingEnvironment.IsDevelopment())
            {
                message = ex.Message;
            }
            // catch all (just Exception)
            var catchAllResponse = new ErrorResponse(message.ToFeedbackItem(FeedbackType.Error), absolutePath, 500);
            catchAllResponse.Identifier = _correlationIdProvider.GetCorrelationId();
            catchAllResponse.Title = "Unexpected internal server error.";
            return new InternalServerObjectResult(catchAllResponse);
        }
    }
}
