using System;

namespace Narato.ResponseMiddleware.IntegrationTest.Mappers.TestClasses
{
    public class TestConflictException : Exception
    {
        public TestConflictException(string message) : base(message)
        {
        }
    }
}
