using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Extensions.DependencyInjection;
using Narato.ResponseMiddleware.IntegrationTest.Mappers.TestClasses;
using Narato.ResponseMiddleware.Mappers.Interfaces;
using Narato.ResponseMiddleware.Models.Models;
using Narato.StringExtensions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using Xunit;

namespace Narato.ResponseMiddleware.IntegrationTest.Mappers
{
    public class ExceptionToActionResultMapperHookTest
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
                            config.AddResponseFilters();
                        });

                    services.AddTransient<IExceptionToActionResultMapperHook, ConflictMapperHook> ();

                    services.AddResponseMiddleware();
                });

            return new TestServer(builder);
        }

        [Fact]
        public async void TestConflictGetsHandledCorrectly()
        {
            // Arrange
            var server = SetupServer();

            // Act

            var response = await server.CreateClient().GetAsync("exceptionHandler/conflict");
            var message = await response.Content.ReadAsStringAsync();

            var responseObject = message.FromJson<ErrorContent>();

            // Assert
            Assert.Equal(HttpStatusCode.Conflict, response.StatusCode);
            Assert.Equal("noperope", responseObject.Message);
        }
    }
}
