using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Extensions.DependencyInjection;
using Narato.ResponseMiddleware.ExceptionHandlers;
using Narato.ResponseMiddleware.Mappers;
using Narato.ResponseMiddleware.Mappers.Interfaces;
using Narato.ResponseMiddleware.Models.Models;
using Narato.ResponseMiddleware.ResponseFilters;
using Narato.StringExtensions;
using System;
using System.Net;
using Xunit;

namespace Narato.ResponseMiddleware.IntegrationTest.ExceptionHandlers
{
    public class ExceptionHandlerFilterTest
    {
        private TestServer SetupServer()
        {
            var builder = new WebHostBuilder()
                .UseEnvironment("development")
                .Configure(app =>
                {
                    app.UseMvc();

                })
                .ConfigureServices(services =>
                {
                    services.AddMvc(
                        //Add this filter globally so every request runs this filter to recored execution time
                        config =>
                        {
                            config.Filters.Add(typeof(ExceptionHandlerFilter));
                            //config.Filters.Add(typeof(LegacyResponseFilter));
                        });

                    services.AddTransient<IExceptionToActionResultMapper, ExceptionToActionResultMapper>();
                });

            return new TestServer(builder);
        }

        [Fact]
        public async void TestExceptionHandlerDoesNothingWhenNoExceptionThrown()
        {
            // Arrange
            var server = SetupServer();

            // Act

            var response = await server.CreateClient().GetAsync("exceptionHandler/noException");
            var message = await response.Content.ReadAsStringAsync();

            // Assert
            Assert.Equal("meep", message);
        }

        [Fact]
        public async void TestExceptionHandlerHandlesException()
        {
            // Arrange
            var server = SetupServer();

            // Act

            var response = await server.CreateClient().GetAsync("exceptionHandler/exception");
            var message = await response.Content.ReadAsStringAsync();
            var errorContent = message.FromJson<ErrorContent>();

            // Assert
            Assert.Equal(HttpStatusCode.InternalServerError, response.StatusCode);
            Assert.Equal("nope", errorContent.Message);
        }
    }
}
