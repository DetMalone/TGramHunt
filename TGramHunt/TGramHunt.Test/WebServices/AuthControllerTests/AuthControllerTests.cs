using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using NSubstitute;
using System.Security.Claims;
using TGramHunt.Contract;
using TGramHunt.Helpers;
using TGramHunt.Services.Helpers.IHelpers;
using TGramHunt.Services.Services.IServices;
using TGramHunt.WebServices;

namespace TGramHunt.Test.WebServices.AuthControllerTests
{
    [TestClass]
    public class AuthControllerTests
    {
        public AuthControllerTests()
        {
            _signInManager = Substitute.For<FakeSignInManager>();
            _logOutHelper = new LogOutHelper(_signInManager);
            _telegramUserService = Substitute.For<ITelegramUserService>();
            _userService = Substitute.For<IUserService>();
            _controller = new AuthController(_signInManager, _telegramUserService, _logOutHelper, _userService);
            _session = Substitute.For<ISession>();
            _controller.ControllerContext.HttpContext = new DefaultHttpContext();
            _controller.HttpContext.Session = _session;
        }

        [TestMethod]
        public async Task TestIsSignedIn_True()
        {
            // Arrange
            var user = _MakeUser();
            _userService.GetIfAuthorized(_controller.User).Returns(user);

            // Act
            var isSignedIn = await _controller.IsSignedIn();

            // Assert
            await _userService.Received().GetIfAuthorized(_controller.User);
            Assert.IsTrue(isSignedIn);

        }

        [TestMethod]
        public async Task TestIsSignedIn_False()
        {
            // Arrange
            _userService.GetIfAuthorized(Arg.Any<ClaimsPrincipal>()).Returns(default(User?));

            // Act
            var isSignedIn = await _controller.IsSignedIn();

            // Assert
            await _userService.Received().GetIfAuthorized(Arg.Any<ClaimsPrincipal>());
            Assert.IsFalse(isSignedIn);
        }

        [TestMethod]
        public async Task TestLogOut()
        {
            // Arrange
            _signInManager.SignOutAsync().Returns(Task.CompletedTask);

            // Act
            var result = await _controller.Logout();

            // Assert
            await _signInManager.Received().SignOutAsync();
            _session.Received().Clear();
            Assert.IsInstanceOfType(result, typeof(RedirectResult));
        }

        [TestMethod]
        public async Task TestTgcallback_UserFound()
        {
            // Arrange
            var user = _MakeUser();
            _telegramUserService.GetAndCreateIfNotExistsAuthTelegramWidget(Arg.Any<Dictionary<string, string>>()).Returns(user);

            // Act
            var result = await _controller.Tgcallback();

            // Assert
            await _signInManager.Received().SignInAsync(user, true);
            await _telegramUserService.Received().GetAndCreateIfNotExistsAuthTelegramWidget(Arg.Any<Dictionary<string, string>>());
            Assert.IsInstanceOfType(result, typeof(RedirectResult));
        }

        [TestMethod]
        public async Task TestTgcallback_UserNotFound()
        {
            // Arrange
            var user = default(User?);
            var session = Substitute.For<ISession>();
            _controller.HttpContext.Session = session;
            _telegramUserService.GetAndCreateIfNotExistsAuthTelegramWidget(Arg.Any<Dictionary<string, string>>()).Returns(user);

            // Act
            var result = await _controller.Tgcallback();

            // Assert
            await _signInManager.Received().SignOutAsync();
            await _telegramUserService.Received().GetAndCreateIfNotExistsAuthTelegramWidget(Arg.Any<Dictionary<string, string>>());
            session.Received().Clear();
            Assert.IsInstanceOfType(result, typeof(RedirectResult));
        }

        #region Private methods

        User _MakeUser() => new User("username", "user@email");

        #endregion

        #region Private fields

        private readonly AuthController _controller;
        private readonly FakeSignInManager _signInManager;
        private readonly ITelegramUserService _telegramUserService;
        private readonly LogOutHelper _logOutHelper;
        private readonly IUserService _userService;
        private readonly ISession _session;

        #endregion
    }
}
