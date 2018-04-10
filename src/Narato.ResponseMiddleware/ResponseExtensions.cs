using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;
using Narato.Correlations;
using Narato.ResponseMiddleware.ExceptionHandlers;
using Narato.ResponseMiddleware.Mappers;
using Narato.ResponseMiddleware.Mappers.Interfaces;
using Narato.ResponseMiddleware.ResponseFilters;
using System;

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
            {
                services.AddCorrelations();
                services.AddTransient<IExceptionToActionResultMapper, LegacyExceptionToActionResultMapper>();
            } else
            {
                services.AddTransient<IExceptionToActionResultMapper, ExceptionToActionResultMapper>();
            }

            return services;
        }

        public static void AddResponseFilters(this MvcOptions config, bool legacy = false)
        {
            if (legacy)
            {
                config.Filters.Add(typeof(LegacyExecutionTimingFilter));
                config.Filters.Add(typeof(LegacyResponseFilter));
                config.Filters.Add(typeof(LegacyModelValidationFilter));
            } else
            {
                config.Filters.Add(typeof(ModelValidationFilter));
            }
            config.Filters.Add(typeof(ExceptionHandlerFilter));
        }

        public static void AddAggregateExceptionUnwrappingFilter(this MvcOptions config)
        {
            config.Filters.Add(typeof(AggregateExceptionUnwrappingFilter));
        }
    }
}
