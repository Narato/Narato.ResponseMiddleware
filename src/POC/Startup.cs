using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using POC.Shizzle;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.AspNetCore.Http;
using Narato.ResponseMiddleware.ExceptionHandlers;
using Narato.ResponseMiddleware.Mappers;
using Narato.ResponseMiddleware.Mappers.Interfaces;
using Narato.ResponseMiddleware.Correlations;
using Narato.ResponseMiddleware.Correlations.Interfaces;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using Narato.ResponseMiddleware.ResponseFilters;
using Microsoft.AspNetCore.Diagnostics;

namespace POC
{
    public class Startup
    {
        public Startup(IHostingEnvironment env)
        {
            var builder = new ConfigurationBuilder()
                .SetBasePath(env.ContentRootPath)
                .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true)
                .AddJsonFile($"appsettings.{env.EnvironmentName}.json", optional: true)
                .AddEnvironmentVariables();
            Configuration = builder.Build();
        }

        public IConfigurationRoot Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.TryAddSingleton<IHttpContextAccessor, HttpContextAccessor>();
            services.TryAddSingleton<IActionContextAccessor, ActionContextAccessor>();
            // Add framework services.
            services.AddMvc(
            //Add this filter globally so every request runs this filter to recored execution time
            config =>
            {
                config.Filters.Add(typeof(ExceptionHandlerFilter));
                config.Filters.Add(typeof(LegacyResponseFilter));
            });

            services.AddSingleton<IMeep, Meep>();
            services.AddTransient<IExceptionToActionResultMapper, LegacyExceptionToActionResultMapper>();
            services.AddTransient<ICorrelationIdProvider, CorrelationIdProvider>();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env, ILoggerFactory loggerFactory)
        {
            loggerFactory.AddConsole(Configuration.GetSection("Logging"));
            loggerFactory.AddDebug();

            app.UseMiddleware<ExceptionHandlerMiddleware>();
            app.UseMvc();
        }
    }
}
