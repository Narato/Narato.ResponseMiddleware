using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Extensions.DependencyInjection;
using Narato.ResponseMiddleware.Models.Models;
using Narato.StringExtensions;
using System;
using System.Collections.Generic;
using System.Net;
using System.Text;
using Xunit;

namespace Narato.ResponseMiddleware.IntegrationTest.ExceptionHandlers
{
    public class AggregateExceptionUnwrappingFilterTest
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
                            config.AddAggregateExceptionUnwrappingFilter();
                        });

                    services.AddResponseMiddleware();
                });

            return new TestServer(builder);
        }

        // note that in this test, we DO add the AggregateExceptionUnwrapFilter
        [Fact]
        public async void TestAggregateExceptionShouldBeUnwrapped()
        {
            // Arrange
            var server = SetupServer();

            // Act
            var response = await server.CreateClient().GetAsync("exceptionHandler/simpleAggregate");
            var message = await response.Content.ReadAsStringAsync();
            var errorContent = message.FromJson<ErrorContent>();

            // Assert
            Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
            Assert.Equal("meep", errorContent.Message);
        }

        [Fact]
        public async void TestMultiAggregateExceptionShouldNotBeUnwrapped()
        {
            // Arrange
            var server = SetupServer();

            // Act
            var response = await server.CreateClient().GetAsync("exceptionHandler/advancedAggregate");
            var message = await response.Content.ReadAsStringAsync();
            var errorContent = message.FromJson<ErrorContent>();

            // Assert
            Assert.Equal(HttpStatusCode.InternalServerError, response.StatusCode);
            Assert.Equal("One or more errors occurred. (meep) (moop)", errorContent.Message);
        }
    }
}
