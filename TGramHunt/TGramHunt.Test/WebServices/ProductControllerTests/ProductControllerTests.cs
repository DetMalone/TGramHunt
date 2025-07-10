using Microsoft.AspNetCore.Mvc;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using NSubstitute;
using System.Security.Claims;
using TGramHunt.Services.Services.IServices;
using TGramHunt.Services.Helpers.IHelpers;
using TGramHunt.WebServices;
using TGramHunt.Contract;

namespace TGramHunt.Test.WebServices.ProductControllerTests
{
    [TestClass]
    public class ProductControllerTests
    {
        #region Constructor
        public ProductControllerTests()
        {
            _productService = Substitute.For<IProductService>();
            _userService = Substitute.For<IUserService>();
            _tgTagHelper = Substitute.For<ITgTagHelper>();
            _fakeResult = Substitute.For<TagResult>();

            _controller = new ProductController(_productService, _userService, _tgTagHelper);

        }

        #endregion

        #region Test methods

        [TestMethod]
        public async Task TestCheckByTag_Unauthorized()
        {
            // Arrange
            _userService.GetIfAuthorized(Arg.Any<ClaimsPrincipal>()).Returns(default(Contract.User?));

            // Act 
            var result = await _controller.CheckByTag(string.Empty);

            // Assert
            Assert.IsInstanceOfType(result, typeof(UnauthorizedResult));
        }

        [TestMethod]
        public async Task TestCheckByTag_InvalidTag()
        {
            // Arrange
            var user = new Contract.User();
            _userService.GetIfAuthorized(Arg.Any<ClaimsPrincipal>()).Returns(user);
            var tag = string.Empty;

            // Act 
            var result = await _controller.CheckByTag(tag);

            // Assert
            Assert.IsInstanceOfType(result, typeof(BadRequestObjectResult));
        }

        [DataTestMethod]
        [DataRow(true)]
        [DataRow(false)]
        public async Task TestCheckByTag_ProductExist(bool isProductExist)
        {
            // Arrange
            var user = new Contract.User();
            var tag = "@correct_tag";
            _userService.GetIfAuthorized(Arg.Any<ClaimsPrincipal>()).Returns(user);
            _productService.IsProductExist(tag).Returns(isProductExist);

            // Act 
            var result = await _controller.CheckByTag(tag);
            var jsonResult = result as JsonResult;

            // Assert
            Assert.IsInstanceOfType(result, typeof(JsonResult));
            Assert.IsNotNull(jsonResult);
            Assert.IsNotNull(jsonResult.Value);
            Assert.IsInstanceOfType(jsonResult.Value, typeof(bool?));
            Assert.AreEqual(isProductExist, (bool)jsonResult.Value);
        }

        [TestMethod]
        public async Task TestCheckTagInfo_Unauthorized()
        {
            // Arrange
            _userService.GetIfAuthorized(Arg.Any<ClaimsPrincipal>()).Returns(default(Contract.User?));

            // Act 
            var result = await _controller.CheckTagInfo(string.Empty);

            // Assert
            Assert.IsInstanceOfType(result, typeof(UnauthorizedResult));
        }

        [TestMethod]
        public async Task TestCheckTagInfo_ShouldReturnFalse_IfTagNotFound()
        {
            //Arrange
            var user = new Contract.User();
            _userService.GetIfAuthorized(Arg.Any<ClaimsPrincipal>()).Returns(user);

            var tag = "@non_existent_tag";

            _fakeResult.isEmpty = true;
            _tgTagHelper.CheckTag(tag, false).Returns(Task.FromResult<TagResult>(_fakeResult));

            //Act
            var controllerResult = await _controller.CheckTagInfo(tag);
            var jsonResult = controllerResult as JsonResult;

            //Assert
            Assert.IsInstanceOfType(jsonResult, typeof(JsonResult));
            Assert.IsNotNull(jsonResult);
            Assert.IsNotNull(jsonResult.Value);
            Assert.IsInstanceOfType(jsonResult.Value, typeof(bool?));
            Assert.AreEqual(false, (bool)jsonResult.Value);
        }

        [TestMethod]
        public async Task TestCheckTagInfo_ShouldReturnTrue_IfTagFound()
        {
            //Arrange
            var user = new Contract.User();
            _userService.GetIfAuthorized(Arg.Any<ClaimsPrincipal>()).Returns(user);

            var tag = "@existent_tag";

            _fakeResult.isEmpty = false;

            _tgTagHelper.CheckTag(tag, false).Returns(Task.FromResult<TagResult>(_fakeResult));

            //Act
            var controllerResult = await _controller.CheckTagInfo(tag);
            var jsonResult = controllerResult as JsonResult;

            //Assert
            Assert.IsInstanceOfType(jsonResult, typeof(JsonResult));
            Assert.IsNotNull(jsonResult);
            Assert.IsNotNull(jsonResult.Value);
            Assert.IsInstanceOfType(jsonResult.Value, typeof(bool?));
            Assert.AreEqual(true, (bool)jsonResult.Value);
        }
        #endregion

        #region Private fields

        private readonly IProductService _productService;
        private readonly IUserService _userService;
        private readonly ProductController _controller;
        private readonly ITgTagHelper _tgTagHelper;
        private readonly TagResult _fakeResult;
        #endregion
    }
}
