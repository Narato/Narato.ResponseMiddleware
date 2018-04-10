using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.Logging;
using System;
using System.Linq;

namespace Narato.ResponseMiddleware.ExceptionHandlers
{
    public class AggregateExceptionUnwrappingFilter : IActionFilter
    {
        private readonly ILogger _logger;

        public AggregateExceptionUnwrappingFilter(ILoggerFactory loggerFactory)
        {
            _logger = loggerFactory.CreateLogger<AggregateExceptionUnwrappingFilter>();
        }

        public void OnActionExecuting(ActionExecutingContext context)
        {
            //do nothing
        }

        public void OnActionExecuted(ActionExecutedContext context)
        {
            if (context.Exception == null)
                return;

            if (context.Exception is AggregateException)
            {
                var innerExceptions = ((AggregateException)context.Exception).Flatten().InnerExceptions;
                if (innerExceptions.Count == 1)
                {
                    context.Exception = innerExceptions.First();
                } else
                {
                    var exceptionListAsString = string.Join(',', ((AggregateException)context.Exception).Flatten().InnerExceptions.Select(ex => ex.ToString()));
                    _logger.LogInformation("Encountered an AggregateException with multiple InnerExceptions. So we didn't unwrap them: " + exceptionListAsString);
                }          
            }
        }
    }
}
