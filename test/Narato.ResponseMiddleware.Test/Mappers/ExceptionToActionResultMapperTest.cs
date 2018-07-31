using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Moq;
using Narato.ResponseMiddleware.Mappers;
using Narato.ResponseMiddleware.Mappers.Interfaces;
using Narato.ResponseMiddleware.Models.Exceptions;
using Narato.ResponseMiddleware.Models.Models;
using Narato.ResponseMiddleware.Models.Mvc.ActionResults;
using System;
using System.Collections.Generic;
using System.Linq;
using Xunit;

namespace Narato.ResponseMiddleware.Test.Mappers
{
    public class ExceptionToActionResultMapperTest
    {
        [Fact]
        public void TestMapValidationException()
        {
            // Arrange
            var hostingEnvironmentMock = new Mock<IHostingEnvironment>();
            var loggerMock = new Mock<ILogger<ExceptionToActionResultMapper>>();
            var hooks = new List<IExceptionToActionResultMapperHook>();
            var mapper = new ExceptionToActionResultMapper(hooks, hostingEnvironmentMock.Object, loggerMock.Object);

            var validationDictionary = new ModelValidationDictionary<string>();
            validationDictionary.Add("name", "cannot contain numbers.");
            var ex = new ValidationException<string>(validationDictionary);

            // Act
            var actionResult = mapper.Map(ex);

            // Assert
            Assert.IsType<BadRequestObjectResult>(actionResult);
            var badRequestActionResult = actionResult as BadRequestObjectResult;
            Assert.IsType<ValidationErrorContent<object>>(badRequestActionResult.Value);
            Assert.Equal("cannot contain numbers.", ((ValidationErrorContent<object>)badRequestActionResult.Value).ValidationMessages.Values.First().First());
        }

        [Fact]
        public void TestMapEntityNotFoundExceptionWithoutMessage()
        {
            // Arrange
            var hostingEnvironmentMock = new Mock<IHostingEnvironment>();
            var loggerMock = new Mock<ILogger<ExceptionToActionResultMapper>>();
            var hooks = new List<IExceptionToActionResultMapperHook>();
            var mapper = new ExceptionToActionResultMapper(hooks, hostingEnvironmentMock.Object, loggerMock.Object);

            var ex = new EntityNotFoundException();

            // Act
            var actionResult = mapper.Map(ex);

            // Assert
            Assert.IsType<NotFoundResult>(actionResult);
        }

        [Fact]
        public void TestMapEntityNotFoundExceptionWitMessage()
        {
            // Arrange
            var hostingEnvironmentMock = new Mock<IHostingEnvironment>();
            var loggerMock = new Mock<ILogger<ExceptionToActionResultMapper>>();
            var hooks = new List<IExceptionToActionResultMapperHook>();
            var mapper = new ExceptionToActionResultMapper(hooks, hostingEnvironmentMock.Object, loggerMock.Object);

            var ex = new EntityNotFoundException("meep", "moop");

            // Act
            var actionResult = mapper.Map(ex);

            // Assert
            Assert.IsType<NotFoundObjectResult>(actionResult);

            var notfoundObjectResult = actionResult as NotFoundObjectResult;
            Assert.Equal("meep", ((ErrorContent)notfoundObjectResult.Value).Code);
        }

        [Fact]
        public void TestMapUnauthorizedException()
        {
            // Arrange
            var hostingEnvironmentMock = new Mock<IHostingEnvironment>();
            var loggerMock = new Mock<ILogger<ExceptionToActionResultMapper>>();
            var hooks = new List<IExceptionToActionResultMapperHook>();
            var mapper = new ExceptionToActionResultMapper(hooks, hostingEnvironmentMock.Object, loggerMock.Object);

            var ex = new UnauthorizedException();

            // Act
            var actionResult = mapper.Map(ex);

            // Assert
            Assert.IsType<UnauthorizedResult>(actionResult);
        }

        [Fact]
        public void TestMapForbiddenException()
        {
            // Arrange
            var hostingEnvironmentMock = new Mock<IHostingEnvironment>();
            var loggerMock = new Mock<ILogger<ExceptionToActionResultMapper>>();
            var hooks = new List<IExceptionToActionResultMapperHook>();
            var mapper = new ExceptionToActionResultMapper(hooks, hostingEnvironmentMock.Object, loggerMock.Object);

            var ex = new ForbiddenException();

            // Act
            var actionResult = mapper.Map(ex);

            // Assert
            Assert.IsType<ForbidResult>(actionResult);
        }

        [Fact]
        public void TestMapExceptionWithFeedback()
        {
            // Arrange
            var hostingEnvironmentMock = new Mock<IHostingEnvironment>();
            var loggerMock = new Mock<ILogger<ExceptionToActionResultMapper>>();
            var hooks = new List<IExceptionToActionResultMapperHook>();
            var mapper = new ExceptionToActionResultMapper(hooks, hostingEnvironmentMock.Object, loggerMock.Object);

            var ex = new ExceptionWithFeedback("meep", "moop");

            // Act
            var actionResult = mapper.Map(ex);

            // Assert
            Assert.IsType<InternalServerObjectResult>(actionResult);

            var internalServerObjectResult = actionResult as InternalServerObjectResult;
            Assert.Equal("meep", ((ErrorContent)internalServerObjectResult.Value).Code);
            Assert.Equal("moop", ((ErrorContent)internalServerObjectResult.Value).Message);
        }

        //[Fact]
        //public void TestMapCatchAllInDevelopment()
        //{
        //    // Arrange
        //    var hostingEnvironmentMock = new Mock<IHostingEnvironment>();
        //    hostingEnvironmentMock.Setup(hem => hem.IsEnvironment()).Returns(true);
        //    var hooks = new List<IExceptionToActionResultMapperHook>();
        //    var loggerMock = new Mock<ILogger<ExceptionToActionResultMapper>>();
        //    var mapper = new ExceptionToActionResultMapper(hooks, hostingEnvironmentMock.Object, loggerMock.Object);

        //    var ex = new Exception("meep");

        //    // Act
        //    var actionResult = mapper.Map(ex);

        //    // Assert
        //    Assert.IsType(typeof(InternalServerObjectResult), actionResult);

        //    var internalServerObjectResult = actionResult as InternalServerObjectResult;
        //    Assert.Equal("meep", ((ErrorContent)internalServerObjectResult.Value).Message);
        //}

        //[Fact]
        //public void TestMapCatchAllInProduction()
        //{
        //    // Arrange
        //    var hostingEnvironmentMock = new Mock<IHostingEnvironment>();
        //    hostingEnvironmentMock.Setup(hem => hem.IsDevelopment()).Returns(false);
        //    var mapper = new ExceptionToActionResultMapper(hostingEnvironmentMock.Object);

        //    var ex = new Exception("meep");

        //    // Act
        //    var actionResult = mapper.Map(ex);

        //    // Assert
        //    Assert.IsType(typeof(InternalServerObjectResult), actionResult);

        //    var internalServerObjectResult = actionResult as InternalServerObjectResult;
        //    Assert.Equal("Something went wrong. Contact support and give them the identifier found below.", ((ErrorContent)internalServerObjectResult.Value).Message);
        //}

        [Fact]
        public void TestMappingHook()
        {
            // Arrange
            var hostingEnvironmentMock = new Mock<IHostingEnvironment>();
            var loggerMock = new Mock<ILogger<ExceptionToActionResultMapper>>();
            var hookMock = new Mock<IExceptionToActionResultMapperHook>();
            var returnActionResult = new ObjectResult("meep");
            returnActionResult.StatusCode = StatusCodes.Status409Conflict;
            hookMock.Setup(hm => hm.Map(It.IsAny<Exception>())).Returns(returnActionResult);

            var hooks = new List<IExceptionToActionResultMapperHook>();
            hooks.Add(hookMock.Object);

            var mapper = new ExceptionToActionResultMapper(hooks, hostingEnvironmentMock.Object, loggerMock.Object);

            var ex = new ExceptionWithFeedback("meep", "moop");

            // Act
            var actionResult = mapper.Map(ex);

            // Assert
            Assert.IsType<ObjectResult>(actionResult);
            Assert.Equal(StatusCodes.Status409Conflict, ((ObjectResult)actionResult).StatusCode);
        }
    }
}
