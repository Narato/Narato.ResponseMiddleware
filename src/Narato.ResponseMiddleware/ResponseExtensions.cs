using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;
using Narato.ResponseMiddleware.ExceptionHandlers;
using Narato.ResponseMiddleware.Mappers;
using Narato.ResponseMiddleware.Mappers.Interfaces;
using Narato.ResponseMiddleware.ResponseFilters;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Narato.ResponseMiddleware
{
    public static class ResponseExtensions
    {
        public static IServiceCollection AddResponseMiddleware(this IServiceCollection services, bool legacy = false)
        {
            if (services == null)
            {
                throw new ArgumentNullException(nameof(services));
            }

            if (legacy)
                services.AddTransient<IExceptionToActionResultMapper, LegacyExceptionToActionResultMapper>();
            else
                services.AddTransient<IExceptionToActionResultMapper, ExceptionToActionResultMapper>();

            return services;
        }

        public static void AddResponseFilters(this MvcOptions config, bool legacy = false)
        {
            config.Filters.Add(typeof(ExceptionHandlerFilter));
            if (legacy)
            {
                config.Filters.Add(typeof(LegacyResponseFilter));
                config.Filters.Add(typeof(LegacyExecutionTimingFilter));
            }
        }
    }
}
