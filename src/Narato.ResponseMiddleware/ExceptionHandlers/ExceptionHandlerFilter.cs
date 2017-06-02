using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.Logging;
using Narato.ResponseMiddleware.Mappers.Interfaces;

namespace Narato.ResponseMiddleware.ExceptionHandlers
{
    public class ExceptionHandlerFilter : IActionFilter // We don't use ExceptionFilterAttribute because we want greater control over when this filter gets called
    {
        private readonly ILogger _logger;
        private readonly IExceptionToActionResultMapper _exceptionToActionResultMapper;

        public ExceptionHandlerFilter(ILoggerFactory loggerFactory, IExceptionToActionResultMapper exceptionToActionResultMapper)
        {
            _logger = loggerFactory.CreateLogger<ExceptionHandlerFilter>();
            _exceptionToActionResultMapper = exceptionToActionResultMapper;
        }

        public void OnActionExecuting(ActionExecutingContext context)
        {
            // do nothing
        }

        public void OnActionExecuted(ActionExecutedContext context)
        {
            if (context.Exception == null)
                return;

            _logger.LogError(0, context.Exception, "An exception has occurred, and will be mapped to a fitting IActionResult: " + context.Exception.Message);
            var actionResult = _exceptionToActionResultMapper.Map(context.Exception);
            context.ExceptionHandled = true;
            context.Result = actionResult;

        }
    }
}
