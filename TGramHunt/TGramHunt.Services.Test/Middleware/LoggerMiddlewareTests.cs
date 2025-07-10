using Moq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Microsoft.AspNetCore.Http;
using TGramHunt.Services.Middleware;
using TGramHunt.Services.Services.IServices;
using System.Net;

namespace TGramHunt.Services.Test.Middleware
{
    [TestClass]
    public class LoggerMiddlewareTests
    {
        private const string CORRECT_USERNAME = "userName";

        private LoggerMiddleware _loggerMiddleware;

        private Exception _loggedEx;
        private string _loggedUserName;

        private HttpContext _mockCorrectContext;
        private RequestDelegate _mockCorrectRequest;

        private bool _isNext;

        public LoggerMiddlewareTests()
        {
            var mockLoggService = new Mock<ILoggService>();
            mockLoggService.Setup(service => service.Log(It.IsAny<Exception>(), It.IsAny<string>()))
                .Callback<Exception, string>((e, name) => (_loggedEx, _loggedUserName) = (e, name));
            mockLoggService.Setup(service => service.Log(It.IsAny<string>(), It.IsAny<string>()))
                .Callback<string, string>((e, name) => _loggedUserName = name);

            _loggerMiddleware = new LoggerMiddleware(mockLoggService.Object);

            var emptyHttpRequest = new Mock<HttpRequest>();
            var emptyHttpResponse = new Mock<HttpResponse>();
            var mockContext = new Mock<HttpContext>();
            mockContext.Setup(context => context.User.Identity.Name)
                .Returns(CORRECT_USERNAME);
            mockContext.Setup(context => context.Request)
                .Returns(() => emptyHttpRequest.Object);
            mockContext.Setup(context => context.Response)
                .Returns(() => emptyHttpResponse.Object);

            _mockCorrectContext = mockContext.Object;

            var mockRequest = new Mock<RequestDelegate>();
            mockRequest.Setup(request => request(It.IsAny<HttpContext>()))
                .Callback(() => _isNext = true);
            _mockCorrectRequest = mockRequest.Object;
        }

        [TestMethod]
        public void InvokeAsync_NullException_LoggsNothing()
        {
            try
            {
                var testTask = _loggerMiddleware.InvokeAsync(null, null);
                testTask.Wait();

                Assert.Fail();
            }
            catch
            {
                Assert.AreEqual(null, _loggedEx);
                Assert.AreEqual(null, _loggedUserName);
            }
        }

        [TestMethod]
        public void InvokeAsync_CorrectHttpContextAndNullRequest_CorrectLoggs()
        {
            try
            {
                var testTask = _loggerMiddleware.InvokeAsync(_mockCorrectContext, null);
                testTask.Wait();

                Assert.Fail();
            }
            catch
            {
                Assert.IsTrue(_loggedEx is NullReferenceException);
                Assert.AreEqual(CORRECT_USERNAME, _loggedUserName);
            }
        }

        [TestMethod]
        public void InvokeAsync_CorrectHttpContextAndRequest_CorrectLoggs()
        {
            var testTask = _loggerMiddleware.InvokeAsync(_mockCorrectContext, _mockCorrectRequest);
            testTask.Wait();

            Assert.AreEqual(null, _loggedEx);
            Assert.AreEqual(CORRECT_USERNAME, _loggedUserName);
            Assert.AreEqual(true, _isNext);
        }
    }
}
