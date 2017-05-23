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
using Narato.ResponseMiddleware.Models.Legacy.ActionResults;
using Microsoft.AspNetCore.Hosting;

namespace Narato.ResponseMiddleware.Mappers
{
    public class LegacyExceptionToActionResultMapper : IExceptionToActionResultMapper
    {
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly ICorrelationIdProvider _correlationIdProvider;
        private readonly IHostingEnvironment _hostingEnvironment;

        public LegacyExceptionToActionResultMapper(IHttpContextAccessor httpContextAccessor, ICorrelationIdProvider correlationIdProvider, IHostingEnvironment hostingEnvironment)
        {
            _httpContextAccessor = httpContextAccessor;
            _correlationIdProvider = correlationIdProvider;
            _hostingEnvironment = hostingEnvironment;
        }

        public IActionResult Map(Exception ex)
        {
            var absolutePath = _httpContextAccessor.HttpContext.Request.Path;

            if (ex is IValidationException<object>)
            {
                var typedEx = ex as IValidationException<object>;
                var response = new ErrorResponse(typedEx.ValidationMessages.ToFeedbackItems().ToList(), absolutePath, 400);
                response.Identifier = _correlationIdProvider.GetCorrelationId();//typedEx.GetTrackingGuid();
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
                response.Identifier = _correlationIdProvider.GetCorrelationId();//typedEx.GetTrackingGuid();
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
                response.Identifier = _correlationIdProvider.GetCorrelationId(); // typedEx.GetTrackingGuid();
                response.Title = "Unexpected internal server error.";
                return new InternalServerErrorWithResponse(response);
            }

            var message = "Something went wrong. Contact support and give them the identifier found below.";
            // if development ==> expose exception message
            if (_hostingEnvironment.IsDevelopment()/*Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT") != null && Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT").ToLower().Equals("development")*/)
            {
                message = ex.Message;
            }
            // catch all (just Exception)
            var catchAllResponse = new ErrorResponse(message.ToFeedbackItem(FeedbackType.Error), absolutePath, 500);
            catchAllResponse.Identifier = _correlationIdProvider.GetCorrelationId(); // ex.GetTrackingGuid();
            catchAllResponse.Title = "Unexpected internal server error.";
            return new InternalServerErrorWithResponse(catchAllResponse);
        }
    }
}
