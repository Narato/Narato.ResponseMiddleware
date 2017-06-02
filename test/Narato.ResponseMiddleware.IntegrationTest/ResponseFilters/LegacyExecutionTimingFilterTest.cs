using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Extensions.DependencyInjection;
using Narato.ResponseMiddleware.ExceptionHandlers;
using Narato.ResponseMiddleware.Models.Legacy.Models;
using Narato.ResponseMiddleware.ResponseFilters;
using Narato.StringExtensions;
using Xunit;

namespace Narato.ResponseMiddleware.IntegrationTest.ResponseFilters
{
    public class LegacyExecutionTimingFilterTest
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
                            config.AddResponseFilters(true);
                        });

                    services.AddResponseMiddleware(true);
                });

            return new TestServer(builder);
        }

        [Fact]
        public async void TestTimingGetsSet()
        {
            // Arrange
            var server = SetupServer();

            // Act

            var response = await server.CreateClient().GetAsync("exceptionHandler/noException");
            var message = await response.Content.ReadAsStringAsync();

            var responseObject = message.FromJson<Response<string>>();

            // Assert
            Assert.True(responseObject.Generation.Duration > 0);
            Assert.True(responseObject.Generation.TimeStamp.Year > 1000);
        }

        [Fact]
        public async void TestTimingGetsSetsOnExceptionsToo()
        {
            var server = SetupServer();

            var response = await server.CreateClient().GetAsync("exceptionHandler/exception");
            var message = await response.Content.ReadAsStringAsync();

            var responseObject = message.FromJson<Response>();

            Assert.True(responseObject.Generation.TimeStamp.Year > 1000);
        }

        [Fact]
        public async void TestTimingGetsSetOnLegacyExceptionsButNonLegacyFilterOnException()
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
                            config.Filters.Add(typeof(LegacyExecutionTimingFilter));
                            config.Filters.Add(typeof(LegacyModelValidationFilter));
                            config.Filters.Add(typeof(ExceptionHandlerFilter));

                        });

                    services.AddResponseMiddleware(true);
                });

            var server = new TestServer(builder);

            var response = await server.CreateClient().GetAsync("exceptionHandler/exception");
            var message = await response.Content.ReadAsStringAsync();

            var responseObject = message.FromJson<Response>();

            Assert.True(responseObject.Generation.TimeStamp.Year > 1000);
        }
    }
}
