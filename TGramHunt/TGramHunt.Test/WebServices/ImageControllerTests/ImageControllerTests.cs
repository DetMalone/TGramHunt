using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using NSubstitute;
using TGramHunt.Contract;
using TGramHunt.Services.Services.IServices;
using TGramHunt.WebServices;

namespace TGramHunt.Test.WebServices.ImageControllerTests
{
    [TestClass]
    public class ImageControllerTests
    {
        #region Constructor

        public ImageControllerTests()
        {
            _smallFileService = Substitute.For<ISmallFilesService>();
            _controller = new ImageController(_smallFileService);

            // image controller uses httpcontext directly, so need to set context as well
            _controller.ControllerContext.HttpContext = new DefaultHttpContext();
        }

        #endregion

        #region Test methods

        [DataTestMethod]
        [DataRow("some_id_string", "image/gif")]
        public async Task TestGet_Exists(string id, string expectedContentType)
        {
            // Arrange 
            var smallFile = new SmallFile() { Id = id, File = new byte[] { } };
            _smallFileService.Get(id).Returns(smallFile);

            // Act 
            var result = await _controller.Get(id);
            var fileContentResult = result as FileContentResult;

            // Assert
            Assert.IsNotNull(result);
            Assert.IsInstanceOfType(result, typeof(FileContentResult));
            Assert.IsNotNull(fileContentResult);
            Assert.AreEqual(expectedContentType, fileContentResult.ContentType);
            Assert.AreEqual(StatusCodes.Status200OK, _controller.Response.StatusCode);
        }

        [DataTestMethod]
        [DataRow("some_id_string")]
        public async Task TestGet_NotExists(string id)
        {
            // Arrange 
            _smallFileService.Get(Arg.Any<string>()).Returns(default(SmallFile?));

            // Act 
            var result = await _controller.Get(id);

            // Assert
            Assert.IsNotNull(result);
            Assert.IsInstanceOfType(result, typeof(NotFoundResult));
        }

        [DataTestMethod]
        [DataRow("some_id_string")]
        public async Task TestGet_Cached(string id)
        {
            // Arrange
            var emptyFile = new SmallFile() { Id = id, File = new byte[] { } };
            _smallFileService.Get(id).Returns(emptyFile);
            _controller.ControllerContext.HttpContext.Request.Headers.IfModifiedSince =
                new DateTime(2022, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc).ToString("R");

            // Act
            var result = await _controller.Get(id);
            var response = _controller.Response;

            // Assert
            Assert.IsInstanceOfType(result, typeof(ContentResult));
            Assert.AreEqual(StatusCodes.Status304NotModified, response.StatusCode);
        }

        #endregion

        #region Private fields

        private readonly ISmallFilesService _smallFileService;
        private readonly ImageController _controller;

        #endregion
    }
}
