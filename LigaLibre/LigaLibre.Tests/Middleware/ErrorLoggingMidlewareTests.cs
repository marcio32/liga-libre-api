using LigaLibre.API.Middlewares;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Moq;

namespace LigaLibre.Tests.Middleware
{
    public class ErrorLoggingMidlewareTests
    {
        [Fact]
        public async Task InvokeAsync_NoException_CallsNext()
        {
            // Arrange
            var context = new DefaultHttpContext();
            var nextCalled = false;
            RequestDelegate next = (HttpContext ctx) =>
            {
                nextCalled = true;
                return Task.CompletedTask;
            };

            var mockLogger = new Mock<ILogger<ErrorLoggingMiddleware>>();
            var midleware = new ErrorLoggingMiddleware(next, mockLogger.Object);

            // Act
            await midleware.InvokeAsync(context);

            // Assert
            Assert.True(nextCalled);
        }

    }
}
