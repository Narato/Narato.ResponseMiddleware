﻿using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.Logging;
using Narato.ResponseMiddleware.Mappers.Interfaces;
using System.Threading.Tasks;

namespace Narato.ResponseMiddleware.ExceptionHandlers
{
    public class ExceptionHandlerFilter : ExceptionFilterAttribute
    {
        private readonly ILogger _logger;
        private readonly IExceptionToActionResultMapper _exceptionToActionResultMapper;

        public ExceptionHandlerFilter(ILoggerFactory loggerFactory, IExceptionToActionResultMapper exceptionToActionResultMapper)
        {
            _logger = loggerFactory.CreateLogger<ExceptionHandlerFilter>();
            _exceptionToActionResultMapper = exceptionToActionResultMapper;
        }

        public override Task OnExceptionAsync(ExceptionContext context)
        {
            var actionResult = _exceptionToActionResultMapper.Map(context.Exception);
            context.ExceptionHandled = true;
            return actionResult.ExecuteResultAsync(context);
        }
    }
}