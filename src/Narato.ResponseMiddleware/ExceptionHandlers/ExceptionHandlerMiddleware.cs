using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Microsoft.Net.Http.Headers;
using Narato.ResponseMiddleware.Mappers.Interfaces;
using Microsoft.AspNetCore.Mvc.Internal;

namespace Narato.ResponseMiddleware.ExceptionHandlers
{
    // TODO: middleware doesn't seem to work because IActionResult can't execute on a HttpContext
    //public class ExceptionHandlerMiddleware
    //{
    //    private readonly RequestDelegate _next;
    //    private readonly ILogger _logger;
    //    private readonly IExceptionToActionResultMapper _exceptionToActionResultMapper;
    //    private readonly Func<object, Task> _clearCacheHeadersDelegate;

    //    public ExceptionHandlerMiddleware(RequestDelegate next, ILoggerFactory loggerFactory, IExceptionToActionResultMapper exceptionToActionResultMapper)
    //    {
    //        _next = next;
    //        _logger = loggerFactory.CreateLogger<ExceptionHandlerMiddleware>();
    //        _exceptionToActionResultMapper = exceptionToActionResultMapper;
    //        _clearCacheHeadersDelegate = ClearCacheHeaders;
    //    }

    //    public async Task Invoke(HttpContext context)
    //    {
    //        try
    //        {
    //            await _next(context);
    //        } catch (Exception ex)
    //        {
    //            _logger.LogError(0, ex, "An unhandled exception has occurred: " + ex.Message);
    //            // We can't do anything if the response has already started, just abort.
    //            if (context.Response.HasStarted)
    //            {
    //                _logger.LogWarning("The response has already started, the error handler will not be executed.");
    //                throw;
    //            }

    //            try
    //            {
    //                context.Response.OnStarting(_clearCacheHeadersDelegate, context.Response);
    //                var actionResult = _exceptionToActionResultMapper.Map(ex);
    //                await actionResult.ExecuteResultAsync(context);
    //                return;
    //            } catch (Exception ex2)
    //            {
    //                // Suppress secondary exceptions, re-throw the original.
    //                _logger.LogError(0, ex2, "An exception was thrown attempting to execute the error handler.");
    //            }
    //            throw;
    //        }
    //    }

    //    private Task ClearCacheHeaders(object state)
    //    {
    //        var response = (HttpResponse)state;
    //        response.Headers[HeaderNames.CacheControl] = "no-cache";
    //        response.Headers[HeaderNames.Pragma] = "no-cache";
    //        response.Headers[HeaderNames.Expires] = "-1";
    //        response.Headers.Remove(HeaderNames.ETag);
    //        return TaskCache.CompletedTask;
    //    }
    //}
}
